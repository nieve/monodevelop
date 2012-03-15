// 
// NamespaceReferenceLineExtractor.cs
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
using MonoDevelop.Core;
using Mono.TextEditor;
using MonoDevelop.Ide.FindInFiles;
using ICSharpCode.NRefactory.TypeSystem;
using MonoDevelop.Ide;
using ICSharpCode.NRefactory.Semantics;
using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.CSharp.Resolver;

namespace MonoDevelop.Stereo.Refactoring.Rename
{
	public interface IFindNamespaceReferenceInline
	{
		NamespaceReferenceExtraction TryGetReference(string nspace, FilePath filePath, string line, int lineIndex, int column, 
		                                             int lastFoundIndex, TextEditorData editor);
	}
	
	public class InlineNamespaceReferenceFinder : IFindNamespaceReferenceInline
	{
		ITextEditorResolverProvider resolver;
		
		public InlineNamespaceReferenceFinder (ITextEditorResolverProvider resolver)
		{
			this.resolver = resolver;
		}
		
		public InlineNamespaceReferenceFinder () : this(new TextEditorResolverProvider()) {}
		
		public NamespaceReferenceExtraction TryGetReference (string nspace, MonoDevelop.Core.FilePath filePath, string line, int lineIndex, int column, 
			int lastFoundIndex, TextEditorData editor)
		{
			int lineOffset;
			var nsLine = new NamespaceSearchedLine(nspace, line);
			DomRegion region = new DomRegion (filePath, lineIndex, 0);
			if (nsLine.ContainsNamespaceUsing()) {
				lineOffset = editor.Text.IndexOf (line, lastFoundIndex);
				lastFoundIndex = lineOffset + line.Length;
				var offset = editor.LocationToOffset (lineIndex, column + 1);
				return new NamespaceReferenceExtraction(lastFoundIndex, new MemberReference(null, region, offset, nspace.Length));
			}
			if (nsLine.ContainsNamespaceDeclaration ()) {
				lineOffset = editor.Text.IndexOf (line, lastFoundIndex);
				lastFoundIndex = lineOffset + line.Length;
				var offset = editor.LocationToOffset (lineIndex, column + 1);
				return new NamespaceReferenceExtraction(lastFoundIndex, new MemberReference(null, region, offset, nspace.Length));
			} else if (nsLine.ContainsNamespaceRef()) {
				column = line.IndexOf(nspace + ".", column);
				MemberReference memRef = TryFindTypePrefixNamespaceRef (nspace, filePath, lineIndex, column, editor);
				if (memRef != null)
					return new NamespaceReferenceExtraction(lastFoundIndex,memRef);
			}
			return new NamespaceReferenceExtraction();
		}
		
		private MemberReference TryFindTypePrefixNamespaceRef(string nspace, FilePath filePath, int lineIndex, int column, TextEditorData editor){
			int position = editor.LocationToOffset(lineIndex, column + 1);
			var document = IdeApp.Workbench.GetDocument(filePath.CanonicalPath);
			DomRegion region;
			ResolveResult typeResult = resolver.GetLanguageItem(document, position + 1, out region);
			region = new DomRegion(filePath,region.Begin,region.End);
			if (typeResult is NamespaceResolveResult) {
				return new MemberReference(null, region, position, nspace.Length);
			}
			
			return null;
		}
	}
}

