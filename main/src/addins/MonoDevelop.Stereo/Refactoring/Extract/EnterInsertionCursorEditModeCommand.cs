using MonoDevelop.Refactoring;
using System;
using Mono.TextEditor;
using MonoDevelop.TypeSystem;
using Mono.TextEditor.PopupWindow;
using MonoDevelop.Ide;
using System.Collections.Generic;
using System.Linq;

namespace MonoDevelop.Stereo.Refactoring.Extract {
	public interface IEnterInsertionCursorEditModeCommand
	{
		void Execute(RefactoringOptions options, EventHandler<InsertionCursorEventArgs> onExit);
	}

	public class EnterInsertionCursorEditModeCommand : IEnterInsertionCursorEditModeCommand {
		public void Execute(RefactoringOptions options, EventHandler<InsertionCursorEventArgs> onExit){
			var data = options.GetTextEditorData();
			ParsedDocument doc = options.Document.ParsedDocument;
			DocumentLocation currentLocation = data.Caret.Location;
			Mono.TextEditor.TextEditor editor = data.Parent;
			var member = doc.GetMember (currentLocation);
			if (editor == null) return;			
			if (member == null) return;
			
			MonoDevelop.Ide.Gui.Document document = options.Document;
			var declaringMember = member.CreateResolved (doc.GetTypeResolveContext (document.Compilation, currentLocation));
			var type = declaringMember.DeclaringTypeDefinition.Parts.First ();
			
			List<InsertionPoint> list = CodeGenerationService.GetInsertionPoints (document, type);
			var mode = new InsertionCursorEditMode (editor, list);
			mode.CurIndex = mode.InsertionPoints.FindLastIndex(p=>p.Location < currentLocation);
			
			ModeHelpWindow helpWindow = new InsertionCursorLayoutModeHelpWindow ();
			helpWindow.TransientFor = IdeApp.Workbench.RootWindow;
			helpWindow.TitleText = "<b>Extract Field -- Targeting</b>";
			helpWindow.Items.Add (new KeyValuePair<string, string> ("<b>Key</b>", "<b>Behavior</b>"));
			helpWindow.Items.Add (new KeyValuePair<string, string> ("<b>Up</b>", "Move to <b>previous</b> target point."));
			helpWindow.Items.Add (new KeyValuePair<string, string> ("<b>Down</b>", "Move to <b>next</b> target point."));
			helpWindow.Items.Add (new KeyValuePair<string, string> ("<b>Enter</b>", "<b>Declare new method</b> at target point."));
			helpWindow.Items.Add (new KeyValuePair<string, string> ("<b>Esc</b>", "<b>Cancel</b> this refactoring."));
			
			mode.HelpWindow = helpWindow;
			mode.StartMode ();
			if (onExit != null) mode.Exited += onExit;
		}
	}

}