using System;
using MonoDevelop.Stereo.TextEditor;

namespace MonoDevelop.Stereo
{
	public interface ITextDuplicationContext
	{
		DuplicateText GetTextToDuplicate ();
		bool ActiveDocumentAndEditorExist();
		void AppendDuplicatedText(DuplicateText text);
	}
	
	public class TextDuplicationContext : ITextDuplicationContext {
		IDocumentContext docContext;
		public TextDuplicationContext (IDocumentContext docContext)
		{
			this.docContext = docContext;
		}
		public TextDuplicationContext () : this(new DocumentContext()) {}
		public DuplicateText GetTextToDuplicate ()
		{
			MonoDevelop.Ide.Gui.Document doc = docContext.GetActiveDocument();
			if (doc == null) new EmptyDuplicateText();
			var data = docContext.GetData();
			var editor = doc.Editor;
			if (editor.IsSomethingSelected) {				
				return new SelectedDuplicateText(editor.SelectedText, editor.SelectionRange.EndOffset);
			}
			
			int line, column;
			data.GetLineColumnFromPosition (data.CursorPosition, out line, out column);
			var offset = data.GetPositionFromLineColumn(line + 1, 1);
			return new LineDuplicateText(editor.GetLineText(line), offset, editor.EolMarker);
		}
		public bool ActiveDocumentAndEditorExist ()
		{
			MonoDevelop.Ide.Gui.Document activeDocument = docContext.GetActiveDocument();
			return (activeDocument != null || activeDocument.Editor != null);
		}
		public void AppendDuplicatedText(DuplicateText text){
			var doc = docContext.GetActiveDocument();
			doc.Editor.Insert(text.Offset, text);
		}
	}
}

