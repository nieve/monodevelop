using System.Collections.Generic;
using Gtk;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Ide.ProgressMonitoring;
using MonoDevelop.Refactoring;
using MonoDevelop.Refactoring.Rename;
using MonoDevelop.Stereo.Gui;
using System;
using ICSharpCode.NRefactory.Semantics;
using MonoDevelop.Ide.FindInFiles;

namespace MonoDevelop.Stereo.Refactoring.Rename
{
	public class RenameNamespaceRefactoring : RenameRefactoring, IRefactorWithNaming
	{
		IFindNamespaceReference finder;
		INameValidator validator;
		IProgressMonitorFactory factory;
		
		public RenameNamespaceRefactoring () : this(new NamespaceReferenceFinder(), new NamespaceValidator(), new ProgressMonitorFactory()) { }
		
		public RenameNamespaceRefactoring (IFindNamespaceReference namespaceRefFinder, INameValidator validator, IProgressMonitorFactory factory)
		{
			this.Name = "Rename Namespace";
			finder = namespaceRefFinder;
			this.validator = validator;
			this.factory = factory;
		}
		
		public override bool IsValid (RefactoringOptions options)
		{
			return options.ResolveResult is NamespaceResolveResult;
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
	
	public interface IProgressMonitorFactory
	{
		IProgressMonitor Create();
	}
	
	public class ProgressMonitorFactory : IProgressMonitorFactory
	{
		public IProgressMonitor Create(){
			return new MessageDialogProgressMonitor(true, false, false, true);
		}
	}
}

