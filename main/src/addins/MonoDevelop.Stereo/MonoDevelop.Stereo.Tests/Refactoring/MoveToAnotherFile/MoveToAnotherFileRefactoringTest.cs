using System;
using NUnit.Framework;
using MonoDevelop.Stereo.Refactoring.GenerateNewType;
using Rhino.Mocks;
using MonoDevelop.Stereo.Refactoring.MoveToAnotherFile;
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;

namespace MonoDevelop.Stereo.MoveToAnotherFileRefactoringTest
{
	[TestFixture]
	public class IsValid
	{
		IMoveTypeContext ctx = MockRepository.GenerateMock<IMoveTypeContext>();
		MoveToAnotherFileRefactoring subject;
		
		[SetUp]
		public void SetUp(){
			subject = new MoveToAnotherFileRefactoring(ctx, null);
		}
		
		[TearDown]
		public void TearDown(){
			ctx.BackToRecord(BackToRecordOptions.All);
			ctx.Replay();
		} 
		
		[Test]
		public void Validates_when_types_were_found_and_current_location_is_not_type_with_file_name(){
			ctx.Stub(c=>c.IsCurrentPositionTypeDeclarationUnmatchingFileName()).Return(true);
			IType t1 = MockRepository.GenerateStub<IType>();
			IType t2 = MockRepository.GenerateStub<IType>();
			ctx.Stub(c=>c.GetTypes()).Return(new List<IType>{t1,t2});
			
			var validation = subject.IsValid();
			
			Assert.IsTrue(validation);
		}
		
		[Test]
		public void Invalidates_when_one_type_was_found(){
			ctx.Stub(dp=>dp.IsCurrentPositionTypeDeclarationUnmatchingFileName()).Return(true);
			IType t1 = MockRepository.GenerateStub<IType>();
			ctx.Stub(c=>c.GetTypes()).Return(new List<IType>{t1});
			
			var validation = subject.IsValid();
			
			Assert.IsFalse(validation);
		}
		
		[Test]
		public void Invalidates_when_current_location_is_type_with_file_name(){
			ctx.Stub(c=>c.IsCurrentPositionTypeDeclarationUnmatchingFileName()).Return(false);
			IType t1 = MockRepository.GenerateStub<IType>();
			IType t2 = MockRepository.GenerateStub<IType>();
			ctx.Stub(c=>c.GetTypes()).Return(new List<IType>{t1,t2});
			
			var validation = subject.IsValid();
			
			Assert.IsFalse(validation);
		}
	}
}

