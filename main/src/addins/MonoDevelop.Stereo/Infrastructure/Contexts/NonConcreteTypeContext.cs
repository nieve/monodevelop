using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using Mono.TextEditor;

namespace MonoDevelop.Stereo
{
	public class NonConcreteTypeContext : INonConcreteTypeContext
	{
		IDocumentContext docContext;
		public NonConcreteTypeContext (IDocumentContext docContext)
		{
			this.docContext = docContext;
		}
		public NonConcreteTypeContext () : this(new DocumentContext()) {}
		public bool IsCurrentLocationNonConcreteType ()
		{
			var result = docContext.GetResolvedResult();
			if (result != null) {
				var type = result.Type;
				if (type != null) return (type is ITypeDefinition && (type as ITypeDefinition).IsAbstract) || type.Kind == TypeKind.Interface;
			}
			return false;
		}

		public IType GetNonConcreteType ()
		{
			var result = docContext.GetResolvedResult();
			return result.Type;
		}
		
		public int GetOffset (TextEditorData data, Mono.TextEditor.DocumentLocation location)
		{
			return data.Document.LocationToOffset(location);
		}
	}
	
	public interface INonConcreteTypeContext
	{
		bool IsCurrentLocationNonConcreteType();
		IType GetNonConcreteType();
		int GetOffset (TextEditorData data, DocumentLocation location);
	}
}

