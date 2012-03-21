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
		string fileName;
		public IType Type {get;set;}
		public TextEditorData Data {set{data=value;}}
		public InsertionPoint InsertionPoint {set{insertionPoint=value;}}
		
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
			Type = context.GetNonConcreteType();
			fileName = options.Document.FileName;
			MonoDevelop.Ide.Gui.Document openDocument = IdeApp.Workbench.OpenDocument(fileName, (OpenDocumentOptions) 39);
			if (openDocument == null) MessageService.ShowError(string.Format("Can't open file {0}.", fileName));
			else insertionPoint = GetInsertionPoint(openDocument, Type);
			MessageService.ShowCustomDialog((Dialog) new RefactoringNamingDialog(options, this, validator));
		}
		
		public override List<Change> PerformChanges (RefactoringOptions options, object prop)
		{
			string newTypeName = (string) prop;
			
			var methods = GetMethodsToImplement(Type);
			List<Change> changes = new List<Change>();
			var textReplaceChange = new TextReplaceChange();
			textReplaceChange.FileName = fileName;
			textReplaceChange.RemovedChars = 0;			
			textReplaceChange.Offset = context.GetOffset(data, insertionPoint.Location);
			
			StringBuilder contentBuilder = new StringBuilder();
			string eol = data.EolMarker;
			if (insertionPoint.LineBefore == NewLineInsertion.Eol) contentBuilder.Append(eol);
			contentBuilder.Append(eol);
			contentBuilder.Append(eol);
			
			var content = builder.Build(newTypeName, Type.Name, indent, eol, methods, IsImplementInterface(Type));
			contentBuilder.Append(content);
			
			if (insertionPoint.LineAfter == NewLineInsertion.None) contentBuilder.Append(eol);
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