// 
// ExtractFieldRefactoring.cs
//  
// Author:
//       Nieve <>
// 
// Copyright (c) 2012 Nieve
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.Semantics;
using Mono.TextEditor;
using Mono.TextEditor.PopupWindow;
using MonoDevelop.CSharp.Refactoring.ExtractMethod;
using MonoDevelop.Ide;
using MonoDevelop.Refactoring;
using MonoDevelop.TypeSystem;

namespace MonoDevelop.Stereo.Refactoring.Extract
{
	public class ExtractFieldRefactoring : RefactoringOperation
	{
		IVariableContext context;
		public InsertionPoint InsertionPoint{private get; set;}
		TextEditorData data;
		public ExtractFieldRefactoring () : this(new VariableContext()) {}
		public ExtractFieldRefactoring (IVariableContext context)
		{
			this.Name = "Extract Field";
			this.context = context;
		}
		public override string AccelKey {
			get {
				return "Ctrl+R|Ctrl+F";
			}
		}		
		public override string GetMenuDescription(RefactoringOptions options)
	    {
	    	return "Extract _Field";
	    }		
		public string OperationTitle  {
			get{return "Extract Field";}
		}
		public override void Run (RefactoringOptions options)
		{
			data = options.GetTextEditorData();
			ParsedDocument doc = options.Document.ParsedDocument;
			var member = doc.GetMember (data.Caret.Location);
			EnterInsertionCursorEditMode(options, doc);
		}
		private void EnterInsertionCursorEditMode(RefactoringOptions options, ParsedDocument doc){
			Mono.TextEditor.TextEditor editor = data.Parent;
			if (editor != null) {
				var member = doc.GetMember (data.Caret.Location);
				if (member == null) return;
				MonoDevelop.Ide.Gui.Document document = options.Document;
				var declaringMember = member.CreateResolved (doc.GetTypeResolveContext (document.Compilation, data.Caret.Location));
				var type = declaringMember.DeclaringTypeDefinition.Parts.First ();
				
				List<InsertionPoint> list = CodeGenerationService.GetInsertionPoints (document, type);
				var mode = new InsertionCursorEditMode (editor, list);
				for (int i = 0; i < mode.InsertionPoints.Count; i++) {
					var point = mode.InsertionPoints[i];
					if (point.Location < editor.Caret.Location) {
						mode.CurIndex = i;
					} else {
						break;
					}
				}
				ModeHelpWindow helpWindow = new ModeHelpWindow ();
				helpWindow.TransientFor = IdeApp.Workbench.RootWindow;
				helpWindow.TitleText = "<b>Extract Field -- Targeting</b>";
				helpWindow.Items.Add (new KeyValuePair<string, string> ("<b>Key</b>", "<b>Behavior</b>"));
				helpWindow.Items.Add (new KeyValuePair<string, string> ("<b>Up</b>", "Move to <b>previous</b> target point."));
				helpWindow.Items.Add (new KeyValuePair<string, string> ("<b>Down</b>", "Move to <b>next</b> target point."));
				helpWindow.Items.Add (new KeyValuePair<string, string> ("<b>Enter</b>", "<b>Declare new method</b> at target point."));
				helpWindow.Items.Add (new KeyValuePair<string, string> ("<b>Esc</b>", "<b>Cancel</b> this refactoring."));
				mode.HelpWindow = helpWindow;
				mode.StartMode ();
				
				mode.Exited += delegate(object s, InsertionCursorEventArgs args) {
					if (args.Success) {
						InsertionPoint = args.InsertionPoint;
						BaseRun (options);
					}
				};
			}
		}
		private void BaseRun(RefactoringOptions options){base.Run (options);}
		public override List<Change> PerformChanges (RefactoringOptions options, object prop){
			var variable = ((LocalResolveResult)options.ResolveResult).Variable;
			TextReplaceChange change = new TextReplaceChange();
			change.FileName = options.Document.FileName;
			var indentation = context.GetIndentation(InsertionPoint);
			string text = indentation + variable.Type.Name + " " + variable.Name + ";";
			change.InsertedText = text + context.GetEol();
			change.MoveCaretToReplace = false;
			change.Offset = context.GetOffset(InsertionPoint.Location);
			change.RemovedChars = 0;
			
			//TODO: Add TextReplaceChange to modify local variable declaration to usage (remove type)
			return new List<Change>(){change};
		}
		public override bool IsValid (RefactoringOptions options){
			return options.ResolveResult is LocalResolveResult;
		}
	}
}