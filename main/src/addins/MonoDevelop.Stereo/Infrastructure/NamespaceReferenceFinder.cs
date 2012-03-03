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
using ICSharpCode.OldNRefactory.Ast;

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
			string nspace = resolveResult.Namespace.FullName;
			
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
					}
					
					//TODO: Looks like this isn't working - fix.
					foreach(var memberRef in FindInnerReferences (monitor, nspace, filePath))
						yield return memberRef;
				}
			}
			yield break;
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

