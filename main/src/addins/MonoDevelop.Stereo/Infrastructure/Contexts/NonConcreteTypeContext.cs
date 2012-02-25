using System;
using ICSharpCode.NRefactory.TypeSystem;

namespace MonoDevelop.Stereo
{
	public class NonConcreteTypeContext : DocumentContext, INonConcreteTypeContext
	{
		public bool IsCurrentLocationNonConcreteType ()
		{
			//TODO: Works only for interfaces currently. make it work for abstracts as well.
			var result = GetResolvedResult();
			if (result.Type != null) {
				var type = result.Type;
				if (type != null) return type.Kind == TypeKind.Interface;
			}
			return false;
		}

		public IType GetNonConcreteType ()
		{
			var result = GetResolvedResult();
			return result.Type;
		}
	}
	
	public interface INonConcreteTypeContext : IDocumentContext
	{
		bool IsCurrentLocationNonConcreteType();
		IType GetNonConcreteType();
	}
}

