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

namespace MonoDevelop.Stereo.Refactoring.Rename
{
	public interface INamespaceReferenceController
	{
		IEnumerable<MemberReference> FindReferences(NamespaceResolveResult resolveResult, IProgressMonitor monitor);
		IEnumerable<MemberReference> FindReferences(Solution solution, NamespaceResolveResult resolveResult, IProgressMonitor monitor);
	}
	
	public class NamespaceReferenceController : INamespaceReferenceController
	{
		IExtractProjectFiles projectFilesExtractor;
		IFindNamespaceReferenceInline lineExtractor;
		
		public NamespaceReferenceController () : this(new ExtractProjectFiles(), new InlineNamespaceReferenceFinder()) {}
		
		public NamespaceReferenceController (IExtractProjectFiles extractProjectFiles, IFindNamespaceReferenceInline lineExtractor)
		{
			projectFilesExtractor = extractProjectFiles;
			this.lineExtractor = lineExtractor;
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
					TextEditorData editor = projectFilesExtractor.GetTextEditorData(filePath);
					int lastFoundIndex = 0;
					for (var i = 0; i < editor.Lines.Count(); i++){
						var line = editor.GetLineText(i);
						if (string.IsNullOrWhiteSpace(line)) continue;
						
						var column = -1;
						while ((column = line.IndexOf(nspace, column + 1)) > -1) {
							NamespaceReferenceExtraction attempt = lineExtractor.TryGetReference(nspace, filePath, line, i, column, lastFoundIndex, editor);
							if (attempt.IsSuccessful){
								lastFoundIndex = attempt.LastFoundIndex;
								yield return attempt.NamespaceReference;
							}
						}
					}
				}
			}
			yield break;
		}
	}
}

