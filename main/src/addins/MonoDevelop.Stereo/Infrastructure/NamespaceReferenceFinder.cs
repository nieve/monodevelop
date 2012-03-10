using System.Collections.Generic;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using Mono.TextEditor;
using MonoDevelop.Core;
using MonoDevelop.CSharp.Resolver;
using MonoDevelop.Ide;
using MonoDevelop.Ide.FindInFiles;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Projects;
using MonoDevelop.Refactoring;
using System.Linq;

namespace MonoDevelop.Stereo
{
	public interface IFindNamespaceReference
	{
		IEnumerable<MemberReference> FindReferences(NamespaceResolveResult resolveResult, IProgressMonitor monitor);
		IEnumerable<MemberReference> FindReferences(Solution solution, NamespaceResolveResult resolveResult, IProgressMonitor monitor);
	}
	
	public class NamespaceReferenceFinder : IFindNamespaceReference
	{
		IExtractProjectFiles projectFilesExtractor;
		ITextEditorResolverProvider resolver;
		public NamespaceReferenceFinder ()
		{
			projectFilesExtractor = new ExtractProjectFiles();
			resolver = new TextEditorResolverProvider();
		}
		
		public NamespaceReferenceFinder (IExtractProjectFiles extractProjectFiles, ITextEditorResolverProvider resolver)
		{
			projectFilesExtractor = extractProjectFiles;
			this.resolver = resolver;
		}
		
		public IEnumerable<MemberReference> FindReferences(NamespaceResolveResult resolveResult, IProgressMonitor monitor){
			return FindReferences(IdeApp.ProjectOperations.CurrentSelectedSolution, resolveResult, monitor);
		}
		
		public IEnumerable<MemberReference> FindReferences(Solution solution, NamespaceResolveResult resolveResult, IProgressMonitor monitor){
			string nspace = resolveResult.Namespace.FullName;
			ICSharpCode.NRefactory.CSharp.Resolver.FindReferences findRefs = new ICSharpCode.NRefactory.CSharp.Resolver.FindReferences();
			foreach (var t in resolveResult.Namespace.Types) {
				var fileName = t.BodyRegion.FileName;
				
				//TODO: can I get where type is being referenced?
//				var scopes = findRefs.GetSearchScopes(t);
//				foreach (var scope in scopes) {
//					var fs = findRefs.GetInterestingFiles (scope, t.Compilation);
//					foreach (var file in fs) {
//						Console.WriteLine ("Ref: " + file.FileName);
//					}
//				}
			}
			
			IEnumerable<FilePath> projFiles = projectFilesExtractor.GetFileNames (solution, monitor);
			foreach (FilePath filePath in projFiles) {
				if (string.IsNullOrWhiteSpace(filePath.Extension)) continue;
				if (monitor != null && monitor.IsCancelRequested) {
					break;
				}
				else {
					TextEditorData editor = TextFileProvider.Instance.GetTextEditorData(filePath);
					
					int lastFoundIndex = 0;
					int column;
					int lineOffset;
					for (var i = 0; i < editor.Lines.Count(); i++){
						var line = editor.GetLineText(i);
						if (string.IsNullOrWhiteSpace(line)) continue;
						
						DomRegion region = new DomRegion(filePath, i, 0);
						if (line != null && line.Contains("using " + nspace + ";")) {
							column = line.IndexOf (nspace);
							lineOffset = editor.Text.IndexOf(line, lastFoundIndex);
							lastFoundIndex = lineOffset + line.Length;
							var offset = editor.LocationToOffset(i, column + 1);
							yield return new MemberReference(null, region, offset, nspace.Length);
						}
						if (LineContainsNamespaceDeclaration(line, nspace)) {
							column = line.IndexOf (nspace);
							lineOffset = editor.Text.IndexOf(line, lastFoundIndex);
							lastFoundIndex = lineOffset + line.Length;
							var offset = editor.LocationToOffset(i, column + 1);
							yield return new MemberReference(null, region, offset, nspace.Length);
						}
						else if (line.Contains(nspace + ".")) {
							MemberReference memRef = TryFindTypePrefixNamespaceRef(nspace, filePath, line, i, editor);
							if (memRef != null) yield return memRef;
						}
					}
				}
			}
			yield break;
		}
		
		private MemberReference TryFindTypePrefixNamespaceRef(string nspace, FilePath filePath, string line, int index, TextEditorData editor){
			var column = line.IndexOf(nspace) + 1;
			int position = editor.LocationToOffset(index, column);
			var document = IdeApp.Workbench.GetDocument(filePath.CanonicalPath);
//			ITextBuffer text = document.GetContent<ITextBuffer> ();
//			text.CursorPosition = position + 1;
			DomRegion region;
			ResolveResult typeResult = resolver.GetLanguageItem(editor, position + 1, out region);
			System.Console.WriteLine (typeResult);
			if (typeResult is NamespaceResolveResult) {
				return new MemberReference(null, region, position, nspace.Length);
			}
			
			return null;
		}
		
		private bool LineContainsNamespaceDeclaration(string line, string nspace){
			return (line != null && 
		    	(line.Trim() == ("namespace " + nspace) || 
			 	line.TrimStart().StartsWith("namespace " + nspace + " ") ||
			 	line.TrimStart().StartsWith("namespace " + nspace + "{") ||
			 	line.TrimStart().StartsWith("namespace " + nspace + ".")));
		}
		
		IEnumerable<MemberReference> FindInnerReferences (IProgressMonitor monitor, string nspace, FilePath filePath)
		{
			var document = IdeApp.Workbench.GetDocument(filePath.CanonicalPath);
			if (document != null){
				ITextBuffer editor = document.GetContent<ITextBuffer> ();
				if (editor.Text.Contains(nspace + ".")){
					var position = -1;
					while ((position = editor.Text.IndexOf(nspace + ".", position + 1)) > -1) {
						if (monitor != null && monitor.IsCancelRequested) {
							break;
						}
						int line, column;
						editor.GetLineColumnFromPosition (position + nspace.Length + 1, out line, out column);
						editor.CursorPosition = position + nspace.Length + 1;
						
						ResolveResult typeResult;
						CurrentRefactoryOperationsHandler.GetItem (document, out typeResult);
						if (typeResult is NamespaceResolveResult) {
							DomRegion region = new DomRegion(filePath, line, column);
							yield return new MemberReference(null, region, position, nspace.Length);
						}
					}
				}
			}
		}
	}
}

