using System;
using ICSharpCode.NRefactory.Semantics;

namespace MonoDevelop.Stereo
{
	public interface INonexistantTypeContext : IDocumentContext
	{
		MemberResolveResult GetResolvedTypeNameResult ();		
	}
	
	public class NonexistantTypeContext : DocumentContext, INonexistantTypeContext
	{
		public MemberResolveResult GetResolvedTypeNameResult ()
		{
			ResolveResult resolveResult = GetResolvedResult ();
			
			return (resolveResult is MemberResolveResult) ?
				(MemberResolveResult)resolveResult : null;
		}
	}
}

