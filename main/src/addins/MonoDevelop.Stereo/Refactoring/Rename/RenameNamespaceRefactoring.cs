using System.Collections.Generic;
using Gtk;
using ICSharpCode.NRefactory.Semantics;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.FindInFiles;
using MonoDevelop.Refactoring;
using MonoDevelop.Refactoring.Rename;
using MonoDevelop.Stereo.Gui;

namespace MonoDevelop.Stereo.Refactoring.Rename
{
	public class RenameNamespaceRefactoring : RenameRefactoring, IRefactorWithNaming
	{
		INamespaceReferenceController finder;
		INameValidator validator;
		IProgressMonitorFactory factory;
		IFindNamespaceReferenceInline extractor;
		
		public RenameNamespaceRefactoring () : this(new NamespaceReferenceController(), new NamespaceValidator(), 
            new ProgressMonitorFactory(), new InlineNamespaceReferenceFinder()) { }
		
		public RenameNamespaceRefactoring (INamespaceReferenceController namespaceRefFinder, INameValidator validator, 
       		IProgressMonitorFactory factory, IFindNamespaceReferenceInline extactor)
		{
			this.Name = "Rename Namespace";
			finder = namespaceRefFinder;
			this.validator = validator;
			this.factory = factory;
			this.extractor = extactor;
		}
		
		public override bool IsValid (RefactoringOptions options)
		{
			return options.ResolveResult is NamespaceResolveResult;
		}
		
		public override string AccelKey {
			get {
				return "Ctrl+Alt+R";
			}
		}
		
		public override string GetMenuDescription(RefactoringOptions options)
	    {
	    	return "_Rename";
	    }
		
		public string OperationTitle  {
			get{return "Rename Namespace";}
		}
		
		public override void Run (RefactoringOptions options)
		{
			if (IsValid (options))
				MessageService.ShowCustomDialog((Dialog) new RefactoringNamingDialog(options, this, new NamespaceValidator()));
		}
		
		public override List<Change> PerformChanges (RefactoringOptions options, object prop)
		{
      		string newName = (string) prop;
			List<Change> changes = new List<Change>();
			var oldName = ((NamespaceResolveResult)options.ResolveResult).NamespaceName;
			using (var dialogProgressMonitor = factory.Create()) {
		        var references = finder.FindReferences((NamespaceResolveResult)options.ResolveResult, dialogProgressMonitor);
		        if (references == null)
		          return changes;
				foreach (MemberReference memberReference in references)
		        {
					TextReplaceChange textReplaceChange = new TextReplaceChange();
					textReplaceChange.FileName = (string) memberReference.FileName;
					textReplaceChange.Offset = memberReference.Offset;
					textReplaceChange.RemovedChars = oldName.Length;
					textReplaceChange.InsertedText = newName;
					textReplaceChange.Description = string.Format(GettextCatalog.GetString("Replace '{0}' with '{1}'"), (object) oldName, (object) newName);
					changes.Add((Change) textReplaceChange);
		        }
			}
			return changes;
		}
	}
}

