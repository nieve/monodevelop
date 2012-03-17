using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace MonoDevelop.Stereo
{
	public class NonConcreteTypeContext : DocumentContext, INonConcreteTypeContext
	{
		public bool IsCurrentLocationNonConcreteType ()
		{
			var result = GetResolvedResult();
			if (result != null) {
				var type = result.Type;
				if (type != null) return (type is ITypeDefinition && (type as ITypeDefinition).IsAbstract) || type.Kind == TypeKind.Interface;
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

