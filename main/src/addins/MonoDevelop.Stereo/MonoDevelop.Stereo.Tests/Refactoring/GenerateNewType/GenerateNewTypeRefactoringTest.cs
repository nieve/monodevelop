using System.Collections.Generic;
using Mono.TextEditor;
using MonoDevelop.Core;
using MonoDevelop.Refactoring;
using MonoDevelop.Stereo;
using MonoDevelop.Stereo.Refactoring.GenerateNewType;
using MonoDevelop.Stereo.Refactoring.NewTypeFormatProviders;
using NUnit.Framework;
using Rhino.Mocks;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;

namespace MonoDevelop.Stereo.GenerateNewTypeRefactoringTest {
	[TestFixture]
	public class PerformingChanges
	{
		INonexistantTypeContext ctx = MockRepository.GenerateMock<INonexistantTypeContext>();
		IResolveTypeContent resolver = MockRepository.GenerateMock<IResolveTypeContent>();
		GenerateNewTypeRefactoring generateClassRefactoring;
		readonly string dir = @"c:\some\path\";
		readonly string fileName = "current.cs";
		List<Change> changes = null;
		string fileContent = "some file content";
		
		[TestFixtureSetUp]
		public void SetUp(){
			generateClassRefactoring = new GenerateNewTypeRefactoring(ctx, resolver);
			generateClassRefactoring.Data = new Mono.TextEditor.TextEditorData{Document = new Document()};
			generateClassRefactoring.InsertionPoint = new InsertionPoint(new DocumentLocation(0,0),NewLineInsertion.None,NewLineInsertion.None);
			generateClassRefactoring.InsertionPoint.LineBefore = NewLineInsertion.Eol;
			generateClassRefactoring.InsertionPoint.LineAfter = NewLineInsertion.None;
		}
		
		[SetUp]
		public void SetTest(){
			UnknownIdentifierResolveResult result = new UnknownIdentifierResolveResult("NonExistant", 0);
			ctx.Stub(c=>c.GetUnknownTypeResolvedResult()).Return(result);
			ctx.Stub(c=>c.GetCurrentFilePath()).Return(new FilePath(dir + fileName));
			
			resolver.Stub(r=>r.GetNewTypeContent(Arg<string>.Is.Equal("NonExistant"), Arg<string>.Is.Anything, Arg<string>.Is.Anything)).Return (fileContent);
			
			changes = generateClassRefactoring.PerformChanges(null, null);
		}
		
		[Test()]
		public void Returns_a_change ()
		{
			Assert.IsNotNull(changes);
			Assert.AreEqual(1, changes.Count);
		}
		
		[Test()]
		public void Returns_a_text_replace_change ()
		{
			Assert.IsInstanceOfType(typeof(TextReplaceChange), changes[0]);
		}
		
		[Test()]
		public void Returns_change_with_new_full_file_name ()
		{
			var change = changes[0] as TextReplaceChange;
			Assert.AreEqual(dir + fileName, change.FileName);
		}
		
		[Test()]
		public void Returns_change_with_new_type_content ()
		{
			var change = changes[0] as TextReplaceChange;
			string contentFormat = "\r\n\r\n\r\n{0}\r\n";
			Assert.AreEqual(contentFormat.ToFormat(fileContent), change.InsertedText);
		}
	}

	[TestFixture]
	public class IsValid {
		INonexistantTypeContext ctx = MockRepository.GenerateMock<INonexistantTypeContext>();
		GenerateNewTypeRefactoring generateClassRefactoring;
		
		[TestFixtureSetUp]
		public void SetUp(){
			generateClassRefactoring = new GenerateNewTypeRefactoring(ctx, null);
		}
		
		[Test()]
		public void Validates_only_expression_that_isnt_member_nor_type ()
		{
			ResolveResult result = new ResolveResult(MockRepository.GenerateStub<IType>());
			ctx.Stub(c=>c.GetUnknownTypeResolvedResult()).Return(result);
			
			Assert.IsTrue(generateClassRefactoring.IsValid());
		}
	}
}

