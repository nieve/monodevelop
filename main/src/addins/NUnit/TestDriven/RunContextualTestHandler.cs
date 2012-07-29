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
using NUnit.Core;
using ICSharpCode.NRefactory.TypeSystem;
using MonoDevelop.Ide.Gui;
using Mono.TextEditor;

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
			info.Visible = IsInsideTestFixture();
		}

		bool IsInsideTestFixture ()
		{
			MDRefactoringContext context;
			var result = GetTypeDeclarationContext (out context);
			if (result == null)
				return false;

			CSharpTypeResolveContext resolveContext;
			var currentType = GetCurrentTypeContext (context, out resolveContext);

			foreach (CSharpAttribute attr in currentType.Attributes) {
				var attrType = attr.AttributeType.Resolve(resolveContext);
				return attrType.FullName == typeof(TestFixtureAttribute).FullName;
			}

			return false;
		}

		static TypeDeclaration GetTypeDeclarationContext (out MDRefactoringContext context)
		{
			var location = IdeApp.Workbench.ActiveDocument.Editor.Caret.Location;
			var activeDocument = IdeApp.Workbench.ActiveDocument;
			context = new MDRefactoringContext (activeDocument, location);
			return context.GetNode<TypeDeclaration> ();
		}

		static IUnresolvedTypeDefinition GetCurrentTypeContext (MDRefactoringContext context, out CSharpTypeResolveContext resolveContext)
		{
			var activeDocument = IdeApp.Workbench.ActiveDocument;
			var offset = IdeApp.Workbench.ActiveDocument.Editor.Caret.Offset;
			var provider = GetProvider ();
			var currentType = provider.GetTypeAt (offset);
			var csAssem = activeDocument.Compilation.MainAssembly as CSharpAssembly;
			var parsedFile = context.Document.ParsedDocument.ParsedFile as CSharpParsedFile;
			resolveContext = new CSharpTypeResolveContext (csAssem, parsedFile.RootUsingScope.Resolve (context.Compilation));

			return currentType;
		}
		
		static ITextEditorMemberPositionProvider GetProvider ()
		{
			var activeDocument = IdeApp.Workbench.ActiveDocument;
			return activeDocument.GetContent<ITextEditorMemberPositionProvider> ();
		}

		static string GetContextualFullName(){
			var provider = GetProvider ();
			var offset = IdeApp.Workbench.ActiveDocument.Editor.Caret.Offset;
			var member = provider.GetMemberAt (offset);
			if (member != null) return member.FullName;
			return provider.GetTypeAt (offset).FullName;
		}

		UnitTest SearchTest ()
		{
			string fullName = GetContextualFullName();
			foreach (UnitTest t in NUnitService.Instance.RootTests) {
				UnitTest r = SearchTest (t, fullName);
				if (r != null)
					return r;
			}
			return null;
		}
		
		UnitTest SearchTest (UnitTest test, string fullName)
		{
			if (test == null)
				return null;
			if (test.TestId == fullName)
				return test;
			
			UnitTestGroup group = test as UnitTestGroup;
			if (group != null)  {
				foreach (UnitTest t in group.Tests) {
					UnitTest result = SearchTest (t, fullName);
					if (result != null)
						return result;
				}
			}
			return null;
		}

		protected override void Run ()
		{
			var test = SearchTest();
			NUnitService.Instance.RunTest (test, null);
		}
	}
}

