// 
// DerivedTypeContentBuilderTest.cs
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
using ICSharpCode.NRefactory.TypeSystem;
using Rhino.Mocks;
using MonoDevelop.Stereo.Refactoring.NewTypeContentBuilders;
using System.Collections.Generic;

namespace MonoDevelop.Stereo.DerivedTypeContentBuilderTest
{
	[TestFixture]
	public class Build
	{
		DerivedTypeContentBuilder subject = new DerivedTypeContentBuilder();
		string expectedAbstractContent = 
@"	public class Something : SomethingAbstract {
		public override void DoSomething (string name, int age) {
			throw new NotImplementedException ();
		}
		public override string DoSomethingElse (Yokai rokurokubi, Monster oni) {
			throw new NotImplementedException ();
		}
	}";
		string expectedInterfaceContent = 
@"	public class Something : ISomething {
		public void DoSomething (string name, int age) {
			throw new NotImplementedException ();
		}
		public string DoSomethingElse (Yokai rokurokubi, Monster oni) {
			throw new NotImplementedException ();
		}
	}";
		
		IMethod doSomething = MockRepository.GenerateMock<IMethod>();
		IMethod doSomethingElse = MockRepository.GenerateMock<IMethod>();
		IType stringRetType = MockRepository.GenerateMock<IType>();
		IType voidRetType = MockRepository.GenerateMock<IType>();
		IType p1Type = MockRepository.GenerateMock<IType>();
		IType p2Type = MockRepository.GenerateMock<IType>();
		IType p3Type = MockRepository.GenerateMock<IType>();
		IType p4Type = MockRepository.GenerateMock<IType>();
		IParameter p1 = MockRepository.GenerateMock<IParameter>();
		IParameter p2 = MockRepository.GenerateMock<IParameter>();
		IParameter p3 = MockRepository.GenerateMock<IParameter>();
		IParameter p4 = MockRepository.GenerateMock<IParameter>();
		
		[TestFixtureSetUp]
		public void SetUp(){
			stringRetType.Expect(t=>t.Name).Return("string");
			voidRetType.Expect(t=>t.Name).Return("Void");
			p1Type.Expect (t=>t.Name).Return ("string");
			p2Type.Expect (t=>t.Name).Return ("int");
			p3Type.Expect (t=>t.Name).Return ("Yokai");
			p4Type.Expect (t=>t.Name).Return ("Monster");
			p1.Expect(p=>p.Name).Return("name");
			p2.Expect(p=>p.Name).Return("age");
			p3.Expect(p=>p.Name).Return("rokurokubi");
			p4.Expect(p=>p.Name).Return("oni");
			p1.Expect (p=>p.Type).Return(p1Type);
			p2.Expect (p=>p.Type).Return(p2Type);
			p3.Expect (p=>p.Type).Return(p3Type);
			p4.Expect (p=>p.Type).Return(p4Type);
			doSomething.Expect(m=>m.ReturnType).Return(voidRetType);
			doSomethingElse.Expect(m=>m.ReturnType).Return(stringRetType);
			doSomething.Expect(m=>m.Name).Return("DoSomething");
			doSomethingElse.Expect(m=>m.Name).Return("DoSomethingElse");
			doSomething.Expect(m=>m.Parameters).Return(new List<IParameter>{p1,p2});
			doSomethingElse.Expect(m=>m.Parameters).Return(new List<IParameter>{p3,p4});			
		}
		
		[Test]
		public void Returns_abstract_class_implementation ()
		{			
			var content = subject.Build("Something", "SomethingAbstract", "\t", "\r\n", new List<IMethod>{doSomething,doSomethingElse}, false);
			Assert.AreEqual(expectedAbstractContent, content);
		}
		
		[Test]
		public void Returns_interface_implementation ()
		{			
			var content = subject.Build("Something", "ISomething", "\t", "\r\n", new List<IMethod>{doSomething,doSomethingElse}, true);
			Assert.AreEqual(expectedInterfaceContent, content);
		}
	}
}

