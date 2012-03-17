using System;
using System.Collections.Generic;
using System.Text;
using Mono.TextEditor;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Refactoring;
using MonoDevelop.Stereo.Refactoring.NewTypeFormatProviders;
using MonoDevelop.Stereo.Refactoring.QuickFixes;
using ICSharpCode.NRefactory.Semantics;

namespace MonoDevelop.Stereo.Refactoring.GenerateNewType
{
	public class GenerateNewTypeRefactoring : AppendingNewTypeRefactoringOperation, IRefactorTask
	{
		INonexistantTypeContext context;
		IResolveTypeContent fileFormatResolver;
		InsertionPoint insertionPoint = null;
		
		public string Title{ get {return "Generate new type";}}
		public int Position { get {return 0;}}
		
		public TextEditorData Data {
			get {
				return this.data;
			}
			set {
				data = value;
			}
		}

		public InsertionPoint InsertionPoint {
			get {
				return this.insertionPoint;
			}
			set {
				insertionPoint = value;
			}
		}
		
		public GenerateNewTypeRefactoring ()
		{
			this.Name = "Generate new type";
			context = new NonexistantTypeContext();
			fileFormatResolver = new TypeContentResolver();
		}
		
		public GenerateNewTypeRefactoring (INonexistantTypeContext ctx, IResolveTypeContent resolver)
		{
			this.Name = "Generate new type";
			context = ctx;
			fileFormatResolver = resolver;
		}
		
		public override string GetMenuDescription(RefactoringOptions options)
	    {
	    	return "Generate new _type";
	    }
		
		public override bool IsValid(RefactoringOptions options)
		{
			return IsValid ();
		}
		
		public bool IsValid(){
			ResolveResult resolvedTypeName = context.GetUnknownTypeResolvedResult();
			return resolvedTypeName != null;
		}
		
		public override void Run (RefactoringOptions options)
		{
			MonoDevelop.Ide.Gui.Document doc = options.Document;
			var parsedDocument = doc.ParsedDocument;
			var comp = doc.Compilation;
			var type = parsedDocument.GetTypeResolveContext(comp, options.Location).CurrentTypeDefinition;
			insertionPoint = GetInsertionPoint(doc, type);
			base.Run(options);
		}
				
		public override List<Change> PerformChanges (RefactoringOptions options, object properties)
		{
			List<Change> changes = new List<Change>();
			var textReplaceChange = new TextReplaceChange();
			textReplaceChange.FileName = context.GetCurrentFilePath();
			textReplaceChange.RemovedChars = 0;
			int num = data.Document.LocationToOffset(insertionPoint.Location);
			textReplaceChange.Offset = num;
			
			var resolveResult = options.ResolveResult;
			if (resolveResult == null) throw new InvalidOperationException("Cannot generate class here");
			
			string newTypeName = (resolveResult as UnknownIdentifierResolveResult).Identifier;
			StringBuilder contentBuilder = new StringBuilder();
			if (insertionPoint.LineBefore == NewLineInsertion.Eol) contentBuilder.Append(data.EolMarker);
			contentBuilder.Append(data.EolMarker);
			contentBuilder.Append(data.EolMarker);
			var content = fileFormatResolver.GetNewTypeContent(newTypeName, indent, data.EolMarker);
			contentBuilder.Append(content);
			if (insertionPoint.LineAfter == NewLineInsertion.None) contentBuilder.Append(data.EolMarker);
			textReplaceChange.InsertedText = contentBuilder.ToString();
			
			changes.Add(textReplaceChange);
			return changes;
		}
	}
}

