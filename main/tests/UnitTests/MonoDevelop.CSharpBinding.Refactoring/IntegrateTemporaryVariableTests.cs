// 
// IntegrateTemporaryVariableTests.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2009 Novell, Inc (http://www.novell.com)
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
using MonoDevelop.CSharpBinding.Refactoring;
using System.Collections.Generic;
using MonoDevelop.CSharpBinding.Refactoring.IntegrateTemporaryVariable;

namespace MonoDevelop.CSharpBinding.Refactoring.Tests
{
	[TestFixture()]
	public class IntegrateTemporaryVariableTests : UnitTests.TestBase
	{
		void TestIntegrateTemporaryVariable (string inputString, string outputString)
		{
			IntegrateTemporaryVariableRefactoring refactoring = new IntegrateTemporaryVariableRefactoring ();
			RefactoringOptions options = ExtractMethodTests.CreateRefactoringOptions (inputString);
			List<Change> changes = refactoring.PerformChanges (options, null);
			string output = ExtractMethodTests.GetOutput (options, changes);
			Assert.IsTrue (ExtractMethodTests.CompareSource (output, outputString), "Expected:" + Environment.NewLine + outputString + Environment.NewLine + "was:" + Environment.NewLine + output);
		}
		
		[Test()]
		public void IntegrateTemporaryVariableTest ()
		{
			TestIntegrateTemporaryVariable (@"class TestClass
{
	void Test ()
	{
		int $tmp = 5 + 6;
		Console.WriteLine (tmp);
	}
}
", @"class TestClass
{
	void Test ()
	{
		Console.WriteLine (5 + 6);
	}
}");
		}
	}
}
