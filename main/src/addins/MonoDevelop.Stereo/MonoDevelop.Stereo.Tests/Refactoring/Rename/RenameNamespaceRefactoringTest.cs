using NUnit.Framework;
using System;
using MonoDevelop.Stereo.Refactoring.Rename;
using MonoDevelop.Refactoring;
using Rhino.Mocks;
using MonoDevelop.Core;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using MonoDevelop.Stereo.Gui;

namespace MonoDevelop.Stereo.RenameNamespaceRefactoringTest
{
	[TestFixture]
	public class IsValid
	{
		RenameNamespaceRefactoring renameNamespaceRefactoring = new RenameNamespaceRefactoring();
		
		[Test()]
		public void Validates_options_with_namespace_resolve_result ()
		{
			var nspace = MockRepository.GenerateStub<INamespace>();
			var options = new RefactoringOptions{ResolveResult = new NamespaceResolveResult(nspace)};
			Assert.IsTrue(renameNamespaceRefactoring.IsValid(options));
		}
	}
	
	[TestFixture]
	public class PerformChanges
	{
		RenameNamespaceRefactoring renameNamespaceRefactoring;
		INamespaceReferenceController refFinder = MockRepository.GenerateStub<INamespaceReferenceController>();
		IFindNamespaceReferenceInline lineExtractor = MockRepository.GenerateStub<IFindNamespaceReferenceInline>();
		RefactoringOptions options = new RefactoringOptions();
		INameValidator validator = MockRepository.GenerateStub<INameValidator>();
		MonoDevelop.Stereo.Refactoring.Rename.IProgressMonitorFactory factory = MockRepository.GenerateStub<MonoDevelop.Stereo.Refactoring.Rename.IProgressMonitorFactory>();
		
		[SetUp]
		public void SetUp(){
			factory.Stub (f=>f.Create()).Return(MockRepository.GenerateStub<IProgressMonitor>());
			renameNamespaceRefactoring = new RenameNamespaceRefactoring(refFinder, validator, factory, lineExtractor);
			var ns = MockRepository.GenerateStub<INamespace>();
			ns.Expect(n=>n.FullName).Return ("My.Namespace");
			options.ResolveResult = new NamespaceResolveResult(ns);
		}
		
		[Test()]
		public void Returns_null_when_found_references_are_null ()
		{
			refFinder.Stub(f=>f.FindReferences(Arg<NamespaceResolveResult>.Is.Anything, Arg<IProgressMonitor>.Is.Anything))
				.Return(null);
			Assert.IsEmpty(renameNamespaceRefactoring.PerformChanges(options, null));
		}
	}
}

