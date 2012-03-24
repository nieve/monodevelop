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
using MonoDevelop.Refactoring;
using ICSharpCode.NRefactory.Semantics;
using MonoDevelop.CSharp.Refactoring.ExtractMethod;
using MonoDevelop.Ide;
using MonoDevelop.TypeSystem;

namespace MonoDevelop.Stereo.Refactoring.Extract
{
	public class ExtractFieldRefactoring : ExtractMethodRefactoring
	{
		IVariableContext context;
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
			var buffer = options.Document.Editor;
			ParsedDocument doc = options.Document.ParsedDocument;
			var member = doc.GetMember (buffer.Caret.Location);
			var param = new ExtractMethodParameters () {
				DeclaringMember = member.CreateResolved (doc.GetTypeResolveContext (options.Document.Compilation, buffer.Caret.Location)),
				Location = buffer.Caret.Location
			};
			//TODO: look at ExtractMethodDialog to find the dialog for user to set insertion point.
			MessageService.ShowCustomDialog (new ExtractMethodDialog (options, this, param));
		}
		public override List<Change> PerformChanges (RefactoringOptions options, object prop){
			return new List<Change>();
		}
		public override bool IsValid (RefactoringOptions options){
			return options.ResolveResult is LocalResolveResult;
		}
	}
}