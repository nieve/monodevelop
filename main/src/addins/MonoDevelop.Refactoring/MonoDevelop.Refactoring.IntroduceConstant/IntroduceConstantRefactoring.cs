// 
// IntroduceConstantRefactoring.cs
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
using ICSharpCode.NRefactory.CSharp;
using MonoDevelop.Core;
using Mono.TextEditor;
using Mono.TextEditor.Highlighting;
using MonoDevelop.Ide;

namespace MonoDevelop.Refactoring.IntroduceConstant
{
	public class IntroduceConstantRefactoring : RefactoringOperation
	{
		public class Parameters
		{
			public string Name {
				get;
				set;
			}
			
			public ICSharpCode.NRefactory.CSharp.Modifiers Modifiers {
				get;
				set;
			}
		}
		
		public override bool IsValid (RefactoringOptions options)
		{
			TextEditorData data = options.GetTextEditorData ();
			LineSegment line = data.Document.GetLine (data.Caret.Line);
			if (!data.IsSomethingSelected && line != null) {
				var stack = line.StartSpan.Clone ();
				Mono.TextEditor.Highlighting.SyntaxModeService.ScanSpans (data.Document, data.Document.SyntaxMode, data.Document.SyntaxMode, stack, line.Offset, data.Caret.Offset);
				foreach (Span span in stack) {
					if (span.Color == "string.single" || span.Color == "string.double")
						return options.Document.CompilationUnit.GetMemberAt (data.Caret.Line, data.Caret.Column) != null;
				}
			}

			INRefactoryASTProvider provider = options.GetASTProvider ();
			if (provider == null)
				return false;
			string expressionText = null;
			if (options.ResolveResult != null && options.ResolveResult.ResolvedExpression != null)
				expressionText = options.ResolveResult.ResolvedExpression.Expression;

			if (string.IsNullOrEmpty (expressionText)) {
				int start, end;
				expressionText = SearchNumber (data, out start, out end);
			}

			Expression expression = provider.ParseExpression (expressionText);
			return expression is PrimitiveExpression;
		}
		
		public override string GetMenuDescription (RefactoringOptions options)
		{
			return GettextCatalog.GetString ("_Introduce Constant...");
		}

		public override void Run (RefactoringOptions options)
		{
			MessageService.ShowCustomDialog (new IntroduceConstantDialog (this, options, new Parameters ()));
		}
		
		public static string SearchString (TextEditorData data, char quote, out int start, out int end)
		{
			if (data.IsSomethingSelected) {
				start = data.SelectionRange.Offset;
				end = data.SelectionRange.EndOffset;
			} else {
				start = end = data.Caret.Offset;
			}
			while (start > 0) {
				if (data.Document.GetCharAt (start) == quote && (start == 0 || data.Document.GetCharAt (start - 1) != '\\'))
					break;
				start--;
			}
			while (end < data.Document.TextLength) {
				if (data.Document.GetCharAt (end) == quote && (end == 0 || data.Document.GetCharAt (end - 1) != '\\'))
					break;
				end++;
			}
			return data.Document.GetTextBetween (start, end + 1);
		}
		
		string SearchNumber (TextEditorData data, out int start, out int end)
		{
			start = data.Caret.Offset;
			while (start > 0 && start < data.Document.TextLength) {
				char ch = data.Document.GetCharAt (start);
				if (!(Char.IsNumber (ch) || ch == '.' || Char.ToUpper (ch) == 'E' || ch == '+' || ch == '-')) {
					start++;
					break;
				}
				start--;
			}
			end = data.Caret.Offset;
			while (end >= 0 && end < data.Document.TextLength) {
				char ch = data.Document.GetCharAt (end);
				if (!(Char.IsNumber (ch) || ch == '.' || Char.ToUpper (ch) == 'E' || ch == '+' || ch == '-'))
					break;
				end++;
			}
			return start < end ? data.Document.GetTextBetween (start, end) : "";
		}
		
		public override List<Change> PerformChanges (RefactoringOptions options, object properties)
		{
			List<Change> result = new List<Change> ();
			Parameters param = properties as Parameters;
			if (param == null)
				return result;
			TextEditorData data = options.GetTextEditorData ();
			IResolver resolver = options.GetResolver ();
			IMember curMember = options.Document.CompilationUnit.GetMemberAt (data.Caret.Line, data.Caret.Column);
			ResolveResult resolveResult = options.ResolveResult;
			int start = 0;
			int end = 0;
			if (resolveResult == null) {
				LineSegment line = data.Document.GetLine (data.Caret.Line);
				if (line != null) {
					var stack = line.StartSpan.Clone ();
					Mono.TextEditor.Highlighting.SyntaxModeService.ScanSpans (data.Document, data.Document.SyntaxMode, data.Document.SyntaxMode, stack, line.Offset, data.Caret.Offset);
					foreach (Span span in stack) {
						if (span.Color == "string.single" || span.Color == "string.double") {
							resolveResult = resolver.Resolve (new ExpressionResult (SearchString (data, span.Color == "string.single" ? '\'' : '"', out start, out end)), TextLocation.Empty);
							end++;
						}
					}
				}
				if (end == 0) {
					resolveResult = resolver.Resolve (new ExpressionResult (SearchNumber (data, out start, out end)), TextLocation.Empty);
				}
			} else {
				start = data.Document.LocationToOffset (resolveResult.ResolvedExpression.Region.BeginLine, resolveResult.ResolvedExpression.Region.BeginColumn);
				end = data.Document.LocationToOffset (resolveResult.ResolvedExpression.Region.EndLine, resolveResult.ResolvedExpression.Region.EndColumn);
			}
			if (start == 0 && end == 0)
				return result;
			INRefactoryASTProvider provider = options.GetASTProvider ();

			var fieldDeclaration = new FieldDeclaration ();
			var varDecl = new VariableInitializer (param.Name, provider.ParseExpression (resolveResult.ResolvedExpression.Expression));
			fieldDeclaration.AddChild (varDecl, FieldDeclaration.Roles.Variable);
			fieldDeclaration.Modifiers = param.Modifiers;
			fieldDeclaration.Modifiers |= ICSharpCode.NRefactory.CSharp.Modifiers.Const;
			fieldDeclaration.ReturnType = resolveResult.ResolvedType.ConvertToTypeReference ();

			TextReplaceChange insertConstant = new TextReplaceChange ();
			insertConstant.FileName = options.Document.FileName;
			insertConstant.Description = string.Format (GettextCatalog.GetString ("Generate constant '{0}'"), param.Name);
			insertConstant.Offset = data.Document.LocationToOffset (curMember.Location.Line, 1);
			insertConstant.InsertedText = provider.OutputNode (options.Dom, fieldDeclaration, options.GetIndent (curMember)) + Environment.NewLine;
			result.Add (insertConstant);

			TextReplaceChange replaceConstant = new TextReplaceChange ();
			replaceConstant.FileName = options.Document.FileName;
			replaceConstant.Description = string.Format (GettextCatalog.GetString ("Replace expression with constant '{0}'"), param.Name);
			replaceConstant.Offset = start;
			replaceConstant.RemovedChars = end - start;
			replaceConstant.InsertedText = param.Name;
			result.Add (replaceConstant);

			return result;
		}
	}
}
