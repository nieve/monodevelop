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
using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Semantics;
using Mono.TextEditor;
using MonoDevelop.Refactoring;

namespace MonoDevelop.Stereo.Refactoring.Extract
{
	public class ExtractFieldRefactoring : RefactoringOperation
	{
		IVariableContext context;
		public InsertionPoint InsertionPoint{private get; set;}
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
			EventHandler<InsertionCursorEventArgs> onExit = delegate(object s, InsertionCursorEventArgs args) {
				if (args.Success) {
					InsertionPoint = args.InsertionPoint;
					BaseRun (options);
				}
			};
			context.EnterInsertionCursorEditMode(options, onExit);
		}
		private void BaseRun(RefactoringOptions options){base.Run (options);}
		public override List<Change> PerformChanges (RefactoringOptions options, object prop){
			var variable = ((LocalResolveResult)options.ResolveResult).Variable;
			TextReplaceChange change = new TextReplaceChange();
			var fileName = options.Document.FileName;
			change.FileName = fileName;
			var indentation = context.GetIndentation(InsertionPoint);
			string text = indentation + variable.Type.Name + " " + variable.Name + ";";
			change.InsertedText = text + context.GetEol();
			change.MoveCaretToReplace = false;
			change.Offset = context.GetOffset(InsertionPoint.Location);
			change.RemovedChars = 0;
			
			TextReplaceChange removeChange = new TextReplaceChange();
			removeChange.FileName = fileName;
			removeChange.MoveCaretToReplace = false;
			var region = options.ResolveResult.GetDefinitionRegion();
			removeChange.Offset = context.GetOffset(new DocumentLocation(region.EndLine,0));
			removeChange.RemovedChars = region.BeginColumn - 1;
			removeChange.InsertedText = context.GetIndentation(region.EndLine);
			return new List<Change>(){change, removeChange};
		}
		public override bool IsValid (RefactoringOptions options){
			return options.ResolveResult is LocalResolveResult;
		}
	}
}