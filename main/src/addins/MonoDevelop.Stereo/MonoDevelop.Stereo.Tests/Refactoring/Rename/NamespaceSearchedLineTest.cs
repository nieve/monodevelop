// 
// NamespaceSearchedLineTest.cs
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
using MonoDevelop.Stereo.Refactoring.Rename;
using NUnit.Framework;

namespace MonoDevelop.Stereo.NamespaceSearchedLineTest
{
	[TestFixture]
	public class ContainsNamespaceRef
	{
		[Test]
		public void Returns_true_when_line_contains_namespace_appended_by_dot ()
		{
			NamespaceSearchedLine nsLine = new NamespaceSearchedLine("SomeNamespace", "some line with SomeNamespace. inside");
			
			Assert.IsTrue(nsLine.ContainsNamespaceRef());
		}

		[Test]
		public void Returns_false_when_line_doesnt_contain_namespace_appended_by_dot ()
		{
			NamespaceSearchedLine nsLine = new NamespaceSearchedLine("SomeNamespace", "some line with SomeNamespace . inside");
			
			Assert.IsFalse(nsLine.ContainsNamespaceRef());
		}
	}
	
	[TestFixture]
	public class ContainsNamespaceUsing
	{
		[Test]
		public void Returns_true_when_line_contains_using_namespace_appended_by_semicolon ()
		{
			NamespaceSearchedLine nsLine = new NamespaceSearchedLine("SomeNamespace", "some line with using SomeNamespace; inside");
			
			Assert.IsTrue(nsLine.ContainsNamespaceUsing());
		}

		[Test]
		public void Returns_false_when_line_doesnt_contain_using_namespace_appended_by_semicolon ()
		{
			NamespaceSearchedLine nsLine = new NamespaceSearchedLine("SomeNamespace", "some line with using SomeNamespace ; inside");
			
			Assert.IsFalse(nsLine.ContainsNamespaceUsing());
		}
	}
	
	[TestFixture]
	public class ContainsNamespaceDeclaration
	{
		[Test]
		public void Returns_true_when_line_starts_with_namespace_and_ns_name ()
		{
			NamespaceSearchedLine nsLine1 = new NamespaceSearchedLine("SomeNamespace", "namespace SomeNamespace something else");
			
			Assert.IsTrue(nsLine1.ContainsNamespaceDeclaration());
			
			NamespaceSearchedLine nsLine2 = new NamespaceSearchedLine("SomeNamespace", "namespace SomeNamespace{ something else");
			
			Assert.IsTrue(nsLine2.ContainsNamespaceDeclaration());
			
			NamespaceSearchedLine nsLine3 = new NamespaceSearchedLine("SomeNamespace", "namespace SomeNamespace. something else");
			
			Assert.IsTrue(nsLine3.ContainsNamespaceDeclaration());
			
			NamespaceSearchedLine nsLine4 = new NamespaceSearchedLine("SomeNamespace", "namespace SomeNamespace");
			
			Assert.IsTrue(nsLine4.ContainsNamespaceDeclaration());
			
		}

		[Test]
		public void Returns_false_when_line_doesnt_start_with_namespace_and_ns_name ()
		{
			NamespaceSearchedLine nsLine = new NamespaceSearchedLine("SomeNamespace", "namespace SomeNamespaces something else");
			
			Assert.IsFalse(nsLine.ContainsNamespaceDeclaration());
		}
	}
}

