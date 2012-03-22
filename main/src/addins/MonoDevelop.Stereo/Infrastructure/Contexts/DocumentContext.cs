using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Refactoring;
using ICSharpCode.NRefactory.Semantics;

namespace MonoDevelop.Stereo
{
	public interface IDocumentContext
	{
		FilePath GetCurrentFilePath();
		Document GetActiveDocument ();
		ITextBuffer GetData (Document doc);
		ITextBuffer GetData ();
		ResolveResult GetResolvedResult ();
	}
	
	public class DocumentContext : IDocumentContext
	{
		ITextBuffer data = null;
		
		public FilePath GetCurrentFilePath(){
			Document doc = GetActiveDocument();
			if (doc == null) return null;
			return doc.FileName;
		}
		
		public Document GetActiveDocument ()
		{
			return IdeApp.Workbench.ActiveDocument;
		}

		public ITextBuffer GetData (Document doc)
		{
			return data ?? doc.GetContent<ITextBuffer> ();
		}

		public ITextBuffer GetData ()
		{
			return GetData(GetActiveDocument());
		}

		public ResolveResult GetResolvedResult ()
		{
			Document doc = GetActiveDocument();
			if (doc == null || doc.FileName == FilePath.Null || IdeApp.ProjectOperations.CurrentSelectedSolution == null)
				return null;
			ITextBuffer data = GetData (doc);
			if (data == null)
				return null;
			int line, column;
			data.GetLineColumnFromPosition (data.CursorPosition, out line, out column);
			
			ResolveResult resolveResult;
			CurrentRefactoryOperationsHandler.GetItem (doc, out resolveResult);
			return resolveResult;
		}		
	}
}

