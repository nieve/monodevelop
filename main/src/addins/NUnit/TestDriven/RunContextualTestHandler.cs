//
// RunContextualTestHandler.cs
//
// Author:
//       nieve <>
//
// Copyright (c) 2012 nieve
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
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.NUnit.External;
using MonoDevelop.NUnit;

namespace MonoDevelop.TestDriven
{
	public enum Commands
	{
		ContextSensitive
	}

	public class RunContextualTestHandler : CommandHandler
	{
		protected override void Update (CommandInfo info)
		{
			info.Enabled = true;
		}

		NunitTestInfo GetTestInfo (string path)
		{
			NunitTestInfo info = new NunitTestInfo();
			info.FixtureTypeName = "MainTest";
			info.FixtureTypeNamespace = "Test";
			info.Name = "SuccessTest";
			info.PathName = path;
			info.TestId = "Test.MainTest.SuccessTest";
			info.Tests = new NunitTestInfo[0];
			return info;
		}
		
		protected override void Run ()
		{
			var path = IdeApp.Workbench.ActiveDocument.FileName;
			var assembly = "/home/nieve/Code/Misc/Test/Test/bin/Debug/"+IdeApp.Workbench.ActiveDocument.ProjectContent.AssemblyName+".exe";
			NunitTestInfo testInfo = GetTestInfo(path);
			var root = new TestAssembly(assembly);
			var test = new NUnitTestCase(root,testInfo,testInfo.PathName);
			NUnitService.Instance.RunTest (test, null);
			Console.WriteLine("running inside the RunCurrentTestHandler.");
		}
	}
}

