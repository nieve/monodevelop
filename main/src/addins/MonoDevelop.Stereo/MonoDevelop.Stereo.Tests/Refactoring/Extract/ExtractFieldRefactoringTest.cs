// 
// ExtractFieldRefactoringTest.cs
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
using NUnit.Framework;
using MonoDevelop.Stereo.Refactoring.Extract;
using Mono.TextEditor;
using Rhino.Mocks;
using MonoDevelop.Refactoring;
using MonoDevelop.Ide.Gui;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;

namespace MonoDevelop.Stereo.Tests.ExtractFieldRefactoringTest
{
	[TestFixture]
	public class PerformChanges
	{
		IVariableContext context = MockRepository.GenerateStub<IVariableContext>();
		static DocumentLocation location = new DocumentLocation(1,1);
		InsertionPoint point = new InsertionPoint(location,NewLineInsertion.BlankLine,NewLineInsertion.BlankLine);
		ExtractFieldRefactoring subject;
		IViewContent viewContent = MockRepository.GenerateMock<IViewContent>();
		IWorkbenchWindow window = MockRepository.GenerateMock<IWorkbenchWindow>();
		IVariable variable = MockRepository.GenerateMock<IVariable>();
		IType type = MockRepository.GenerateMock<IType>();
		
		[TestFixtureSetUp]
		public void SetUp ()
		{
			subject = new ExtractFieldRefactoring(context);
			type.Expect (t=>t.Name).Return("WhiteVanMan");
			variable.Expect (v=>v.Type).Return (type);
			variable.Expect (v=>v.Name).Return ("ollie");
			variable.Expect (v=>v.Region).Return (new DomRegion(5,5,5,8));
			viewContent.Expect(vc=>vc.IsFile).Return(false);
			window.Expect(w=>w.ViewContent).Return (viewContent);
			
			context.Stub (c=>c.GetIndentation(point)).Return("<indent>");
			context.Stub (c=>c.GetIndentation(5)).Return("<indentation>");
			context.Stub (c=>c.GetEol()).Return("<eol>");
			context.Stub (c=>c.GetOffset(location)).Return(42);
			
			subject.InsertionPoint = point;
		}
		[Test]
		public void Returns_a_text_replace_change ()
		{
			var doc = new Document (window);
			RefactoringOptions options = new RefactoringOptions (doc){ResolveResult=new LocalResolveResult(variable)};
			
			var changes = subject.PerformChanges(options, null);
			
			Assert.IsNotNull(changes);
			Assert.That(changes.Count == 2, "expected 1 changes but was " + changes.Count);
			Assert.IsInstanceOfType (typeof(TextReplaceChange),changes[0]);
			Assert.IsInstanceOfType (typeof(TextReplaceChange),changes[1]);
		}
		[Test]
		public void Returns_a_change_with_field_declaration ()
		{
			var doc = new Document (window);
			RefactoringOptions options = new RefactoringOptions (doc){ResolveResult=new LocalResolveResult(variable)};
			
			var changes = subject.PerformChanges(options, null);
			var change = (TextReplaceChange)changes[0];
			Assert.AreEqual("<indent>WhiteVanMan ollie;<eol>",change.InsertedText);
		}
		[Test]
		public void Returns_a_change_with_variable_declaration_removed ()
		{
			var doc = new Document (window);
			RefactoringOptions options = new RefactoringOptions (doc){ResolveResult=new LocalResolveResult(variable)};
			
			var changes = subject.PerformChanges(options, null);
			var change = (TextReplaceChange)changes[1];
			Assert.AreEqual("<indentation>",change.InsertedText);
			Assert.AreEqual(4,change.RemovedChars);
			Assert.AreEqual(0,change.Offset);
		}
	}
}