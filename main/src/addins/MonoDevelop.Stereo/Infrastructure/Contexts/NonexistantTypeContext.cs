using System;
using ICSharpCode.NRefactory.Semantics;
using MonoDevelop.Core;

namespace MonoDevelop.Stereo
{
	public interface INonexistantTypeContext
	{
		ResolveResult GetUnknownTypeResolvedResult ();
		FilePath GetCurrentFilePath();
	}
	
	public class NonexistantTypeContext : DocumentContext, INonexistantTypeContext {
		public ResolveResult GetUnknownTypeResolvedResult ()
		{
			ResolveResult resolveResult = GetResolvedResult ();
			
			return (resolveResult is UnknownIdentifierResolveResult || resolveResult is ErrorResolveResult) ?
				resolveResult : null;
		}
	}
}

