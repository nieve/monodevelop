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
	
	public class NonexistantTypeContext : INonexistantTypeContext
	{
		IDocumentContext docContext;
		public NonexistantTypeContext () : this(new DocumentContext()) {}
		public NonexistantTypeContext (IDocumentContext docContext)
		{
			this.docContext = docContext;
		}		
		public MonoDevelop.Core.FilePath GetCurrentFilePath ()
		{
			return docContext.GetCurrentFilePath ();
		}
		public ResolveResult GetUnknownTypeResolvedResult ()
		{
			ResolveResult resolveResult = docContext.GetResolvedResult ();
			
			return (resolveResult is UnknownIdentifierResolveResult || resolveResult is ErrorResolveResult) ?
				resolveResult : null;
		}
	}
}

