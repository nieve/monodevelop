// 
// DerivedTypeContentBuilder.cs
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
using System.Collections.Generic;
using ICSharpCode.NRefactory.TypeSystem;
using System.Text;
using System.Linq;

namespace MonoDevelop.Stereo.Refactoring.NewTypeContentBuilders
{
	public interface IBuildDerivedTypeContent
	{
		string Build(string typeName, string baseTypeName, string indent, string eol, IEnumerable<IMethod> methods, bool implementInterface);
	}
	
	public class DerivedTypeContentBuilder : IBuildDerivedTypeContent
	{
		string format = "<indent>public class {0} : {1} {{<eol><indent>\t{2}<eol><indent>}}";
		public string Build (string typeName, string baseTypeName, string indent, string eol, IEnumerable<IMethod> methods, bool implementInterface)
		{
			var innerContent = methods.Select(m => GetMethodContent(eol,implementInterface, m))
				.Aggregate((r,current)=>r+eol+"<indent>\t"+current);
			string content = format.ToFormat (typeName, baseTypeName, innerContent);
			content = content.Replace("<indent>", indent).Replace("<eol>", eol);
			return content;
		}

		string GetMethodContent (string eol, bool implementInterface, IMethod method)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("public ");
			if (!implementInterface) builder.Append("override ");
			var retType = method.ReturnType.Name == "Void"?"void":method.ReturnType.Name;
			builder.Append(retType);
			builder.Append(" ");
			builder.Append(method.Name);
			builder.Append(" (");
			if (method.Parameters.Count > 0) {
				var signature = method.Parameters.Select (p => p.Type.Name + " " + p.Name).Aggregate ((res,current) => res + ", " + current);
				builder.Append (signature);
			}
			builder.Append(") {");
			builder.Append(eol);
			builder.Append("<indent>\t\tthrow new NotImplementedException ();");
			builder.Append(eol);
			builder.Append("<indent>\t}");
			
			return builder.ToString ();
		}
	}
}

