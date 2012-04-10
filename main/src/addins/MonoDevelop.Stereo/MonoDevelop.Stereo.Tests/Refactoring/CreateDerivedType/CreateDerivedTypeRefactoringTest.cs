using MonoDevelop.Stereo.Refactoring.CreateDerivedType;
using MonoDevelop.Stereo.Refactoring.NewTypeContentBuilders;
using NUnit.Framework;
using Rhino.Mocks;
using MonoDevelop.Stereo.Gui;
using ICSharpCode.NRefactory.TypeSystem;
using System;
using System.Collections.Generic;
using Mono.TextEditor;
using MonoDevelop.Refactoring;
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.Stereo.Tests.CreateDerivedTypeRefactoringTest
{
	[TestFixture]
	public class IsValid
	{
		CreateDerivedTypeRefactoring subject;
		INonConcreteTypeContext ctx = MockRepository.GenerateStub<INonConcreteTypeContext>();
		IBuildDerivedTypeContent builder = MockRepository.GenerateStub<IBuildDerivedTypeContent>();
		INameValidator validator = MockRepository.GenerateStub<INameValidator>();
		
		[TestFixtureSetUp]
		public void SetUp(){
			subject = new CreateDerivedTypeRefactoring(ctx, builder, validator);
		}
		
		[Test]
		public void Returns_true_when_current_location_is_non_concrete_type ()
		{
			ctx.Stub(c=>c.IsCurrentLocationNonConcreteType()).Return (true);
			
			Assert.IsTrue(subject.IsValid());
		}
	}
	
	//TODO: Finish
	[TestFixture]
	public class PerformChanges
	{
		CreateDerivedTypeRefactoring subject;
		INonConcreteTypeContext ctx = MockRepository.GenerateStub<INonConcreteTypeContext>();
		IBuildDerivedTypeContent builder = MockRepository.GenerateStub<IBuildDerivedTypeContent>();
		INameValidator validator = MockRepository.GenerateStub<INameValidator>();
		IViewContent viewContent = MockRepository.GenerateMock<IViewContent>();
		IWorkbenchWindow window = MockRepository.GenerateMock<IWorkbenchWindow>();
		Mono.TextEditor.TextEditorData data = new Mono.TextEditor.TextEditorData(new TextDocument("1\r\n2"));
		IType type = MockRepository.GenerateMock<IType>();
		List<IMethod> methods = new List<IMethod> ();
		InsertionPoint point = new InsertionPoint(new DocumentLocation(42,42), NewLineInsertion.Eol,NewLineInsertion.Eol);
		string typeName = "Derived";
		string newTypeName = "NewType";
		List<MonoDevelop.Refactoring.Change> changes;
		
		[TestFixtureSetUp]
		public void SetUp(){
			type.Expect(t=>t.Kind).Return(TypeKind.Class);
			type.Expect(t=>t.Name).Return(typeName);
			type.Stub(t=>t.GetMethods(Arg<Predicate<IUnresolvedMethod>>.Is.Anything, Arg<GetMemberOptions>.Is.Anything)).Return(methods);
			builder.Stub(b=>b.Build(newTypeName,typeName,"","\r\n",methods,false)).Return("content");
			subject = new CreateDerivedTypeRefactoring(ctx, builder, validator){Type=type,Data=data,InsertionPoint=point};
			viewContent.Expect(vc=>vc.IsFile).Return(false);
			window.Expect(w=>w.ViewContent).Return (viewContent);
			var doc = new Document (window);
			RefactoringOptions options = new RefactoringOptions (doc);
			changes = subject.PerformChanges(options, newTypeName);
		}
		
		[Test]
		public void Returns_a_change ()
		{
			Assert.IsNotNull(changes);
			Assert.AreEqual (1,changes.Count);
		}
		
		[Test]
		public void Returns_a_text_replace_change ()
		{
			var change = changes[0];
			Assert.IsInstanceOfType(typeof(MonoDevelop.Refactoring.TextReplaceChange),change);
		}
		
		[Test]
		public void Returns_change_with_content ()
		{
			var change = changes[0] as MonoDevelop.Refactoring.TextReplaceChange;
			Assert.That(change.InsertedText.Contains("content"));
		}
	}
}

