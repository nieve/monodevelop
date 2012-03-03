using System;
using ICSharpCode.NRefactory.Semantics;

namespace MonoDevelop.Stereo
{
	public interface INonexistantTypeContext : IDocumentContext
	{
		ResolveResult GetUnknownTypeResolvedResult ();		
	}
	
	public class NonexistantTypeContext : DocumentContext, INonexistantTypeContext
	{
		public ResolveResult GetUnknownTypeResolvedResult ()
		{
			ResolveResult resolveResult = GetResolvedResult ();
			
			return (resolveResult is UnknownIdentifierResolveResult || resolveResult is ErrorResolveResult) ?
				resolveResult : null;
		}
	}
}

