// 
// CSharpFormatter.cs
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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;


using Mono.TextEditor;
using MonoDevelop.CSharp.Formatting;
using MonoDevelop.CSharp.Resolver;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Projects.Text;
using MonoDevelop.Projects.Policies;
using MonoDevelop.Ide;
using System.Linq;
using MonoDevelop.Ide.CodeFormatting;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using MonoDevelop.CSharp.ContextAction;
using ICSharpCode.NRefactory.TypeSystem;
using MonoDevelop.Core;

namespace MonoDevelop.CSharp.Formatting
{

	class FormattingActionFactory : AbstractActionFactory
	{
		Mono.TextEditor.TextEditorData data;

		class ConcreteTextReplaceAction : TextReplaceAction
		{
			Mono.TextEditor.TextEditorData data;

			public ConcreteTextReplaceAction (Mono.TextEditor.TextEditorData data, int offset, int removedChars, string insertedText) : base (offset, removedChars, insertedText)
			{
				this.data = data;
			}
			
			public override void Perform (Script script)
			{
				data.Replace (Offset, RemovedChars, InsertedText);
			}
		}

		public FormattingActionFactory (Mono.TextEditor.TextEditorData data)
		{
			if (data == null)
				throw new ArgumentNullException ("data");
			this.data = data;
		}
		
		public override TextReplaceAction CreateTextReplaceAction (int offset, int removedChars, string insertedText)
		{
			return new ConcreteTextReplaceAction (data, offset, removedChars, insertedText);
		}
	}
	
	public class CSharpFormatter : AbstractAdvancedFormatter
	{
		static internal readonly string MimeType = "text/x-csharp";

		public override bool SupportsOnTheFlyFormatting { get { return true; } }

		public override bool SupportsCorrectingIndent { get { return true; } }

		public override void CorrectIndenting (PolicyContainer policyParent, IEnumerable<string> mimeTypeChain, 
			TextEditorData data, int line)
		{
			LineSegment lineSegment = data.Document.GetLine (line);
			if (lineSegment == null)
				return;

			var policy = policyParent.Get<CSharpFormattingPolicy> (mimeTypeChain);
			var textPolicy = policyParent.Get<TextStylePolicy> (mimeTypeChain);
			var tracker = new DocumentStateTracker<CSharpIndentEngine> (new CSharpIndentEngine (policy, textPolicy), data);
			tracker.UpdateEngine (lineSegment.Offset);
			for (int i = lineSegment.Offset; i < lineSegment.Offset + lineSegment.EditableLength; i++) {
				tracker.Engine.Push (data.Document.GetCharAt (i));
			}

			string curIndent = lineSegment.GetIndentation (data.Document);

			int nlwsp = curIndent.Length;
			if (!tracker.Engine.LineBeganInsideMultiLineComment || (nlwsp < lineSegment.Length && data.Document.GetCharAt (lineSegment.Offset + nlwsp) == '*')) {
				// Possibly replace the indent
				string newIndent = tracker.Engine.ThisLineIndent;
				if (newIndent != curIndent) 
					data.Replace (lineSegment.Offset, nlwsp, newIndent);
			}
			tracker.Dispose ();
		}

		public override void OnTheFlyFormat (MonoDevelop.Ide.Gui.Document doc, int startOffset, int endOffset)
		{
			OnTheFlyFormatter.Format (doc, startOffset, endOffset);
		}


		public string FormatText (CSharpFormattingPolicy policy, TextStylePolicy textPolicy, string mimeType, string input, int startOffset, int endOffset)
		{
			var data = new TextEditorData ();
			data.Document.SuppressHighlightUpdate = true;
			data.Document.MimeType = mimeType;
			data.Document.FileName = "toformat.cs";
			if (textPolicy != null) {
				data.Options.TabsToSpaces = textPolicy.TabsToSpaces;
				data.Options.IndentationSize = data.Options.TabSize = textPolicy.TabWidth;
			}
			data.Text = input;

			// System.Console.WriteLine ("-----");
			// System.Console.WriteLine (data.Text.Replace (" ", ".").Replace ("\t", "->"));
			// System.Console.WriteLine ("-----");
			
			var parser = new CSharpParser ();
			var compilationUnit = parser.Parse (data);
			bool hadErrors = parser.HasErrors;
			
			if (hadErrors) {
//				foreach (var e in parser.ErrorReportPrinter.Errors)
//					Console.WriteLine (e.Message);
				return input.Substring (startOffset, Math.Max (0, Math.Min (endOffset, input.Length) - startOffset));
			}
			var factory = new FormattingActionFactory (data);
			var formattingVisitor = new ICSharpCode.NRefactory.CSharp.AstFormattingVisitor (policy.CreateOptions (), data.Document, factory, data.Options.TabsToSpaces, data.Options.IndentationSize) {
				HadErrors = hadErrors,
				EolMarker = data.EolMarker
			};
			
			compilationUnit.AcceptVisitor (formattingVisitor);
			
			var changes = new List<ICSharpCode.NRefactory.CSharp.Refactoring.Action> ();
			changes.AddRange (formattingVisitor.Changes.
				Where (c => (startOffset <= c.Offset && c.Offset < endOffset)));
			var endPositionChange = factory.CreateTextReplaceAction (endOffset, 0, "//end");
			changes.Add (endPositionChange);
			changes.Sort ((x, y) => ((TextReplaceAction)x).Offset.CompareTo (((TextReplaceAction)y).Offset));
			MDRefactoringContext.MdScript.RunActions (changes, null);
			
			// check if the formatter has produced errors
			parser = new CSharpParser ();
			parser.Parse (data);
			if (parser.HasErrors) {
				LoggingService.LogError ("C# formatter produced source code errors. See console for output.");
				return input.Substring (startOffset, Math.Max (0, Math.Min (endOffset, input.Length) - startOffset));
			}
			
/*			System.Console.WriteLine ("-----");
			System.Console.WriteLine (data.Text.Replace (" ", "^").Replace ("\t", "->"));
			System.Console.WriteLine ("-----");*/
			string result = data.GetTextBetween (startOffset, Math.Min (data.Length, endPositionChange.Offset));
			data.Dispose ();
			return result;
		}

		public override string FormatText (PolicyContainer policyParent, IEnumerable<string> mimeTypeChain, string input, int startOffset, int endOffset)
		{
			var policy = policyParent.Get<CSharpFormattingPolicy> (mimeTypeChain);
			var textPolicy = policyParent.Get<TextStylePolicy> (mimeTypeChain);

			return FormatText (policy, textPolicy, mimeTypeChain.First (), input, startOffset, endOffset);

		}
	}
}
