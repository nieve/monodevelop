using MonoDevelop.Stereo.Refactoring.CreateDerivedType;
using MonoDevelop.Stereo.Refactoring.NewTypeContentBuilders;
using NUnit.Framework;
using Rhino.Mocks;
using MonoDevelop.Stereo.Gui;

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
	public class Run
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
		public void Handles_interfaces ()
		{
			
		}
	}
}

