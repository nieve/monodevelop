// 
// OnTheFlyFormatter.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
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
using Mono.TextEditor;
using MonoDevelop.Ide;
using System;
using System.Collections.Generic;
using MonoDevelop.Projects.Policies;
using ICSharpCode.NRefactory.CSharp;
using System.Text;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using MonoDevelop.CSharp.Completion;
using MonoDevelop.CSharp.Refactoring;
using MonoDevelop.CSharp.Parser;

namespace MonoDevelop.CSharp.Formatting
{
	public class OnTheFlyFormatter
	{
		public static void Format (MonoDevelop.Ide.Gui.Document data)
		{
			Format (data, 0, data.Editor.Length);
		}

		public static void Format (MonoDevelop.Ide.Gui.Document data, TextLocation location)
		{
			Format (data, location, location);
		} 

		public static void Format (MonoDevelop.Ide.Gui.Document data, TextLocation startLocation, TextLocation endLocation)
		{
			Format (data, data.Editor.LocationToOffset (startLocation), data.Editor.LocationToOffset (endLocation));
		}
		
		public static void Format (MonoDevelop.Ide.Gui.Document data, int startOffset, int endOffset)
		{
			var policyParent = data.Project != null ? data.Project.Policies : PolicyService.DefaultPolicies;
			var mimeTypeChain = DesktopService.GetMimeTypeInheritanceChain (CSharpFormatter.MimeType);
			Format (policyParent, mimeTypeChain, data, startOffset, endOffset);
		}
		
		
		/// <summary>
		/// Builds a compileable stub file out of an entity.
		/// </summary>
		/// <returns>
		/// A string representing the stub
		/// </returns>
		/// <param name='memberStartOffset'>
		/// The offset where the member starts in the returned text.
		/// </param>
		static string BuildStub (MonoDevelop.Ide.Gui.Document data, CSharpCompletionTextEditorExtension.TypeSystemTreeSegment seg, int endOffset, out int memberStartOffset)
		{
			var pf = data.ParsedDocument.ParsedFile as CSharpParsedFile;
			if (pf == null) {
				memberStartOffset = 0;
				return null;
			}
			
			var sb = new StringBuilder ();
			
			int closingBrackets = 0;
			
			// use the member start location to determine the using scope, because this information is in sync, the position in
			// the file may have changed since last parse run (we have up 2 date locations from the type segment tree).
			var scope = pf.GetUsingScope (seg.Entity.Region.Begin);

			while (scope != null && !string.IsNullOrEmpty (scope.NamespaceName)) {
				sb.Append ("namespace Stub {");
				sb.Append (data.Editor.EolMarker);
				closingBrackets++;
				while (scope.Parent != null && scope.Parent.Region == scope.Region)
					scope = scope.Parent;
				scope = scope.Parent;
			}

			var parent = seg.Entity.DeclaringTypeDefinition;
			while (parent != null) {
				sb.Append ("class " + parent.Name + " {");
				sb.Append (data.Editor.EolMarker);
				closingBrackets++;
				parent = parent.DeclaringTypeDefinition;
			}

			memberStartOffset = sb.Length;
			sb.Append (data.Editor.GetTextBetween (seg.Offset, seg.EndOffset));
			
			// Insert at least caret column eol markers otherwise the reindent of the generated closing bracket
			// could interfere with the current indentation.
			var endLocation = data.Editor.OffsetToLocation (endOffset);
			for (int i = 0; i <= endLocation.Column; i++) {
				sb.Append (data.Editor.EolMarker);
			}
			sb.Append (data.Editor.EolMarker);
			sb.Append (new string ('}', closingBrackets));
			
			return sb.ToString ();
		}
		
		static AstFormattingVisitor GetFormattingChanges (PolicyContainer policyParent, IEnumerable<string> mimeTypeChain, MonoDevelop.Ide.Gui.Document document, string input)
		{
			using (var stubData = TextEditorData.CreateImmutable (input)) {
				stubData.Document.FileName = document.FileName;
				var parser = document.HasProject ? new ICSharpCode.NRefactory.CSharp.CSharpParser (TypeSystemParser.GetCompilerArguments (document.Project)) : new ICSharpCode.NRefactory.CSharp.CSharpParser ();
				var compilationUnit = parser.Parse (stubData);
				bool hadErrors = parser.HasErrors;
				// try it out, if the behavior is better when working only with correct code.
				if (hadErrors) {
					return null;
				}
				
				var policy = policyParent.Get<CSharpFormattingPolicy> (mimeTypeChain);
				
				var formattingVisitor = new AstFormattingVisitor (policy.CreateOptions (), stubData.Document, document.Editor.CreateNRefactoryTextEditorOptions ()) {
					HadErrors = hadErrors
				};
				compilationUnit.AcceptVisitor (formattingVisitor);
				return formattingVisitor;
			}
		}
		
		public static void Format (PolicyContainer policyParent, IEnumerable<string> mimeTypeChain, MonoDevelop.Ide.Gui.Document data, int startOffset, int endOffset)
		{
			if (data.ParsedDocument == null)
				return;
			var ext = data.GetContent<CSharpCompletionTextEditorExtension> ();
			if (ext == null)
				return;
			var seg = ext.typeSystemSegmentTree.GetMemberSegmentAt (startOffset);
			if (seg == null)
				return;
			var member = seg.Entity;
			if (member == null || member.Region.IsEmpty || member.BodyRegion.End.IsEmpty)
				return;
			
			// Build stub
			int formatStartOffset;
			var text = BuildStub (data, seg, endOffset, out formatStartOffset);
			int formatLength = endOffset - seg.Offset;
			// Get changes from formatting visitor
			var changes = GetFormattingChanges (policyParent, mimeTypeChain, data, text);
			if (changes == null)
				return;

			// Do the actual formatting
//			var originalVersion = data.Editor.Document.Version;
			int caretOffset = data.Editor.Caret.Offset;

			int realTextDelta = seg.Offset - formatStartOffset;
			int startDelta = 1;
			using (var undo = data.Editor.OpenUndoGroup ()) {
				changes.ApplyChanges (formatStartOffset + startDelta, Math.Max (0, formatLength - startDelta - 1), delegate (int replaceOffset, int replaceLength, string insertText) {
					int translatedOffset = realTextDelta + replaceOffset;
					data.Editor.Document.CommitLineUpdate (data.Editor.OffsetToLineNumber (translatedOffset));
					data.Editor.Replace (translatedOffset, replaceLength, insertText);
				}, (replaceOffset, replaceLength, insertText) => {
					int translatedOffset = realTextDelta + replaceOffset;
					if (translatedOffset < 0 || translatedOffset + replaceLength > data.Editor.Length)
						return true;
					return data.Editor.GetTextAt (translatedOffset, replaceLength) == insertText;
				});

//				var currentVersion = data.Editor.Document.Version;
//				data.Editor.Caret.Offset = originalVersion.MoveOffsetTo (currentVersion, caretOffset, ICSharpCode.NRefactory.Editor.AnchorMovementType.Default);
			}
		}
	}
}