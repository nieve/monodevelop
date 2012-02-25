using System.Collections.Generic;
using System;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Core;
using ICSharpCode.NRefactory.TypeSystem;

namespace MonoDevelop.Stereo
{
	public interface IMoveTypeContext : IDocumentContext
	{
		IEnumerable<IType> GetTypes();
		bool IsCurrentPositionTypeDeclarationUnmatchingFileName();		
	}
	
	public class MoveTypeContext : DocumentContext, IMoveTypeContext
	{
		public IEnumerable<IType> GetTypes ()
		{
			throw new NotImplementedException();
			//TODO: implement!!
//			Document doc = GetActiveDocument ();
//			if (doc == null || doc.FileName == FilePath.Null)
//				return null;
//			return doc.Dom.GetTypes(doc.FileName);
		}
		
		public bool IsCurrentPositionTypeDeclarationUnmatchingFileName() {
//			var res = GetResolvedResult();
//			if (res != null) {
//				return res.Member == null && res.ResolvedType != null 
//					&& res.ResolvedType.Name != GetActiveDocument().FileName.FileNameWithoutExtension;
//			}
			//TODO: Implement!!
			return false;
		}
	}
}

