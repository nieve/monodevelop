using System.Collections.Generic;
using System.Text;
using Gtk;
using Mono.TextEditor;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Refactoring;
using MonoDevelop.Stereo.Gui;
using MonoDevelop.Stereo.Refactoring.GenerateNewType;
using MonoDevelop.Stereo.Refactoring.NewTypeContentBuilders;
using MonoDevelop.Stereo.Refactoring.QuickFixes;
using ICSharpCode.NRefactory.TypeSystem;
using System.Collections;

namespace MonoDevelop.Stereo.Refactoring.CreateDerivedType
{
	public interface ICreateDerivedTypeRefactoring : IRefactorTask {}
	
	public class CreateDerivedTypeRefactoring : AppendingNewTypeRefactoringOperation, ICreateDerivedTypeRefactoring, IRefactorWithNaming
	{
		INonConcreteTypeContext context;
		InsertionPoint insertionPoint = null;
		IBuildDerivedTypeContent builder;
		INameValidator validator;
		
		public CreateDerivedTypeRefactoring () : this(new NonConcreteTypeContext(), new DerivedTypeContentBuilder(), new TypeNameValidator()) { }		
		
		public CreateDerivedTypeRefactoring (INonConcreteTypeContext context, IBuildDerivedTypeContent builder, INameValidator validator)
		{
			this.context = context;
			this.builder = builder;
			this.validator = validator;
		}
		
		public string OperationTitle  {
			get{return "Insert new derived type name";}
		}
		
		public override void Run (RefactoringOptions options)
		{
			MessageService.ShowCustomDialog((Dialog) new RefactoringNamingDialog(options, this, validator));
		}
		
		public override List<Change> PerformChanges (RefactoringOptions options, object prop)
		{
			string newTypeName = (string) prop;
			var type = context.GetNonConcreteType();
			var fileName = options.Document.FileName;
			MonoDevelop.Ide.Gui.Document openDocument = IdeApp.Workbench.OpenDocument(fileName, (OpenDocumentOptions) 39);
			if (openDocument == null) MessageService.ShowError(string.Format("Can't open file {0}.", fileName));
			else insertionPoint = GetInsertionPoint(openDocument, type);
			
			var methods = GetMethodsToImplement(type);
			List<Change> changes = new List<Change>();
			var textReplaceChange = new TextReplaceChange();
			textReplaceChange.FileName = fileName;
			textReplaceChange.RemovedChars = 0;
			int num = data.Document.LocationToOffset(insertionPoint.Location);
			textReplaceChange.Offset = num;
			
			StringBuilder contentBuilder = new StringBuilder();
			if (insertionPoint.LineBefore == NewLineInsertion.Eol) contentBuilder.Append(data.EolMarker);
			contentBuilder.Append(data.EolMarker);
			contentBuilder.Append(data.EolMarker);
			
			var content = builder.Build(newTypeName, type.Name, indent, data.EolMarker, methods, IsImplementInterface(type));
			contentBuilder.Append(content);
			
			if (insertionPoint.LineAfter == NewLineInsertion.None) contentBuilder.Append(data.EolMarker);
			textReplaceChange.InsertedText = contentBuilder.ToString();
			
			changes.Add(textReplaceChange);			
			return changes;
		}

		bool IsImplementInterface (IType type)
		{
			return type.Kind == TypeKind.Interface;
		}

		public IEnumerable<IMethod> GetMethodsToImplement (IType type)
		{
			return IsImplementInterface (type) ? 
				type.GetMethods(m=>true, GetMemberOptions.IgnoreInheritedMembers) : 
				type.GetMethods(m=>m.IsAbstract, GetMemberOptions.IgnoreInheritedMembers);
		}
		
		public override bool IsValid (RefactoringOptions options)
		{
			return IsValid ();
		}
		
		public bool IsValid ()
		{
			return context.IsCurrentLocationNonConcreteType();
		}
		
		public string Title  { get {return "Create derived type";}}
		public int Position { get {return 2;}}
	}
}