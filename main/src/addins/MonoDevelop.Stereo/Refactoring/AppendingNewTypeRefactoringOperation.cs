using System;
using MonoDevelop.Refactoring;
using Mono.TextEditor;
using ICSharpCode.NRefactory.TypeSystem;

namespace MonoDevelop.Stereo.Refactoring.GenerateNewType
{
	public class AppendingNewTypeRefactoringOperation : RefactoringOperation
	{
		protected TextEditorData data = null;
		protected string indent = "";
		
		protected InsertionPoint GetInsertionPoint (MonoDevelop.Ide.Gui.Document document, IType type)
		{
			data = document.Editor;
			if (data == null) {
				throw new System.ArgumentNullException ("data");
			}
			var parsedDocument = document.ParsedDocument;
			if (parsedDocument == null) {
				throw new System.ArgumentNullException ("parsedDocument");
			}
			if (type == null) {
				throw new System.ArgumentNullException ("type");
			}
			
			//TODO: Need to get Declaring type!
			var declaringType = parsedDocument.GetTypeResolveContext(document.Compilation, data.OffsetToLocation(data.FindCurrentWordStart(0)));
//			type = (parsedDocument.CompilationUnit.GetTypeAt (type.Location) ?? type);
			DomRegion region = declaringType.CurrentTypeDefinition.BodyRegion;
			var start = region.BeginLine;
			indent = data.GetLine(start).GetIndentation(data.Document);
			var endLine = region.EndLine;
			int num = data.LocationToOffset (endLine, 1);
			while (num < data.Length && data.GetCharAt(num) != '}') {
				num++;
			}
			num++;
			DocumentLocation documentLocation = data.OffsetToLocation (num);
			
			LineSegment lineAfterClassEnd = data.GetLine (endLine + 1);
			NewLineInsertion lineAfter;
			if (lineAfterClassEnd != null && lineAfterClassEnd.EditableLength == lineAfterClassEnd.GetIndentation (data.Document).Length)
				lineAfter = NewLineInsertion.BlankLine;
			else
				lineAfter = NewLineInsertion.None;
			
			return new InsertionPoint (documentLocation, NewLineInsertion.None, lineAfter);
		}
	}
}

