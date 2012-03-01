using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mono.TextEditor;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Projects;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Refactoring;
using MonoDevelop.Ide.FindInFiles;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.OldNRefactory;
using MonoDevelop.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;

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
		public NamespaceReferenceFinder ()
		{
			projectFilesExtractor = new ExtractProjectFiles();
		}
		
		public NamespaceReferenceFinder (IExtractProjectFiles extractProjectFiles)
		{
			projectFilesExtractor = extractProjectFiles;
		}
		
		public IEnumerable<MemberReference> FindReferences(NamespaceResolveResult resolveResult, IProgressMonitor monitor){
			return FindReferences(IdeApp.ProjectOperations.CurrentSelectedSolution, resolveResult, monitor);
		}
		
		public IEnumerable<MemberReference> FindReferences(Solution solution, NamespaceResolveResult resolveResult, IProgressMonitor monitor){
	        MonoDevelop.Ide.Gui.Document doc = IdeApp.Workbench.ActiveDocument;
			string nspace = resolveResult.NamespaceName;
			
			string currentMime = null;
			IEnumerable<FilePath> projFiles = projectFilesExtractor.GetFileNames (solution, monitor);
			foreach (FilePath filePath in projFiles) {
				if (string.IsNullOrWhiteSpace(filePath.Extension)) continue;
				if (monitor != null && monitor.IsCancelRequested) {
					break;
				}
				else {
					string mime = DesktopService.GetMimeTypeForUri(filePath);
					currentMime = mime;
  					TextEditorData editor = TextFileProvider.Instance.GetTextEditorData(filePath);
    				ParsedDocument parsedDoc = TypeSystemService.ParseFile(doc.Project, filePath);
					
					int lastFoundIndex = 0;
					int column;
					int lineOffset;
					for (var i = 0; i < editor.Lines.Count(); i++){
						var line = editor.GetLineText(i);
						if (filePath == @"C:\Code\Mono\TestMonoAddIn\Test\MyTestClass.cs") Console.WriteLine(line);
						DomRegion region = new DomRegion(filePath, i, 0);
						if (line != null && line.Contains("using " + nspace + ";")) {
							column = line.IndexOf (nspace);
							lineOffset = editor.Text.IndexOf(line, lastFoundIndex);
							lastFoundIndex = lineOffset + line.Length;
							var offset = editor.LocationToOffset(i, column + 1);
							yield return new MemberReference(null, region, offset, nspace.Length);
						}
						if (line != null && //TODO: change to regex, needs to start with 'namespace nspace' and not be suffixed by anything other than space & {.
						    (line.Trim() == ("namespace " + nspace) || line.Trim ().StartsWith("namespace " + nspace))) {
							column = line.IndexOf (nspace);
							lineOffset = editor.Text.IndexOf(line, lastFoundIndex);
							lastFoundIndex = lineOffset + line.Length;
							var offset = editor.LocationToOffset(i, column + 1);
							yield return new MemberReference(null, region, offset, nspace.Length);
						}
					}
					
					foreach(var memberRef in FindInnerReferences (monitor, nspace, filePath))
						yield return memberRef;
				}
			}
			yield break;
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
						if (typeResult is TypeResolveResult) {
							DomRegion region = new DomRegion(filePath, line, column);
							yield return new MemberReference(null, region, position, nspace.Length);
						}
					}
				}
			}
		}
	}
}

