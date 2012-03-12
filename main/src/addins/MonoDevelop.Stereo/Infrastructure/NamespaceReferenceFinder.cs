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
//			foreach (var t in resolveResult.Namespace.Types) {
//				System.Console.WriteLine ("type: " + t.FullName);
//				var fileName = t.BodyRegion.FileName;
//				System.Console.WriteLine ("type: " + fileName);
//				
//				var refs = ReferenceFinder.FindReferences(t, (IProgressMonitor)null);
//				foreach (var r in refs) {
//					System.Console.WriteLine ("Ref: " + r.FileName + "offset: " + r.Offset);
//				}
//			}
			
			IEnumerable<FilePath> projFiles = projectFilesExtractor.GetFileNames (solution, monitor);
			foreach (FilePath filePath in projFiles) {
				if (string.IsNullOrWhiteSpace(filePath.Extension)) continue;
				if (monitor != null && monitor.IsCancelRequested) {
					break;
				}
				else {
					TextEditorData editor = TextFileProvider.Instance.GetTextEditorData(filePath);
					
					int lastFoundIndex = 0;
					int lineOffset;
					for (var i = 0; i < editor.Lines.Count(); i++){
						var line = editor.GetLineText(i);
						if (string.IsNullOrWhiteSpace(line)) continue;
						
						var column = -1;
						while ((column = line.IndexOf(nspace, column + 1)) > -1) {
							//TODO: Extract to different class, unit test!
							DomRegion region = new DomRegion (filePath, i, 0);
							if (line != null && line.Contains ("using " + nspace + ";")) {
								lineOffset = editor.Text.IndexOf (line, lastFoundIndex);
								lastFoundIndex = lineOffset + line.Length;
								var offset = editor.LocationToOffset (i, column + 1);
								yield return new MemberReference(null, region, offset, nspace.Length);
							}
							if (LineContainsNamespaceDeclaration (line, nspace)) {
								lineOffset = editor.Text.IndexOf (line, lastFoundIndex);
								lastFoundIndex = lineOffset + line.Length;
								var offset = editor.LocationToOffset (i, column + 1);
								yield return new MemberReference(null, region, offset, nspace.Length);
							} else if (line.Contains (nspace + ".")) {
								column = line.IndexOf(nspace + ".", column);
								MemberReference memRef = TryFindTypePrefixNamespaceRef (nspace, filePath, i, column, editor);
								if (memRef != null)
									yield return memRef;
							}
						}
					}
				}
			}
			yield break;
		}
		
		private MemberReference TryFindTypePrefixNamespaceRef(string nspace, FilePath filePath, int lineIndex, int column, TextEditorData editor){
			int position = editor.LocationToOffset(lineIndex, column + 1);
			var document = IdeApp.Workbench.GetDocument(filePath.CanonicalPath);
			DomRegion region;
			ResolveResult typeResult = resolver.GetLanguageItem(document, position + 1, out region);
			region = new DomRegion(filePath,region.Begin,region.End);
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

