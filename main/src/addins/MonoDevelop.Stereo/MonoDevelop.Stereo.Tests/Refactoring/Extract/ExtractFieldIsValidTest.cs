using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using MonoDevelop.Refactoring;
using MonoDevelop.Stereo.Refactoring.Extract;
using NUnit.Framework;
using Rhino.Mocks;
namespace MonoDevelop.Stereo.Tests.ExtractFieldRefactoringTest {
	[TestFixture]
	public class IsValid
	{
		ExtractFieldRefactoring subject = new ExtractFieldRefactoring();
		
		[Test]
		public void Returns_true_when_resolve_result_is_local_variable ()
		{
			IType type = MockRepository.GenerateMock<IType> ();
			type.Expect(t=>t.Kind).Return (TypeKind.Anonymous);
			IVariable variable = MockRepository.GenerateMock<IVariable> ();
			variable.Expect(v=>v.Type).Return(type);
			var options = new RefactoringOptions(){ResolveResult = new LocalResolveResult(variable)};
			
			var isValid = subject.IsValid(options);
			
			Assert.IsTrue(isValid);
		}
		
		[Test]
		public void Returns_false_when_resolve_result_isnt_local_variable ()
		{
			var options = new RefactoringOptions(){ResolveResult = new ErrorResolveResult(MockRepository.GenerateStub<IType>())};
			
			var isValid = subject.IsValid(options);
			
			Assert.IsFalse(isValid);
		}
	}
}