//
// RunContextualTestHandler.cs
//
// Author:
//       Nieve <nievegoor@gmail.com>
//       Andres G. Aragoneses <knocte@gmail.com>
//
// Copyright (c) 2012 Nieve
// Copyright (c) 2012 Andres G. Aragoneses
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
using MonoDevelop.CSharp.Refactoring.CodeActions;
using ICSharpCode.NRefactory.CSharp;
using NUnit.Framework;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using MonoDevelop.Ide.Gui.Content;

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
			info.Visible = InsideTestFixture();
		}

		bool InsideTestFixture ()
		{
			var location = IdeApp.Workbench.ActiveDocument.Editor.Caret.Location;
			var activeDocument = IdeApp.Workbench.ActiveDocument;
			var context = new MDRefactoringContext (activeDocument, location);
			var result = context.GetNode<TypeDeclaration> ();
			if (result == null)
				return false;

			var provider = activeDocument.GetContent<ITextEditorMemberPositionProvider>();
			var currentType = provider.GetTypeAt(activeDocument.Editor.LocationToOffset (location));
			var csAssem = activeDocument.Compilation.MainAssembly as CSharpAssembly;
			var parsedFile = context.Document.ParsedDocument.ParsedFile as CSharpParsedFile;
			var resolveContext = new CSharpTypeResolveContext(csAssem, parsedFile.RootUsingScope.Resolve(context.Compilation));

			foreach (CSharpAttribute attr in currentType.Attributes) {
				var attrType = attr.AttributeType.Resolve(resolveContext);
				return attrType.FullName == typeof(TestFixtureAttribute).FullName;
			}

			return false;
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

