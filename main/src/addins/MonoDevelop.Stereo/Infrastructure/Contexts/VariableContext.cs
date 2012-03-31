// 
// VariableContext.cs
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
using ICSharpCode.NRefactory.Semantics;
using Mono.TextEditor;

namespace MonoDevelop.Stereo
{
	public interface IVariableContext
	{
		string GetIndentation (InsertionPoint insertionPoint);
		string GetIndentation (int line);
		bool IsCurrentLocationVariable();
		int GetOffset (DocumentLocation location);
		string GetEol ();
	}
	
	public class VariableContext : DocumentContext, IVariableContext {
		public bool IsCurrentLocationVariable ()
		{
			var resolved = GetResolvedResult();
			return resolved is LocalResolveResult;
		}
		public string GetIndentation (InsertionPoint insertionPoint)
		{
			bool isAfterMethod = insertionPoint.LineBefore == NewLineInsertion.Eol;
			var data = GetActiveDocument().Editor;
			return isAfterMethod ? data.GetLineIndent(insertionPoint.Location.Line - 1) : data.GetLineIndent(insertionPoint.Location.Line);
		}
		public string GetIndentation (int line)
		{
			var data = GetActiveDocument().Editor;
			return data.GetLineIndent(line);
		}
		public string GetEol ()
		{
			var editor = GetActiveDocument().Editor;
			return editor.EolMarker;
		}
	}
}

