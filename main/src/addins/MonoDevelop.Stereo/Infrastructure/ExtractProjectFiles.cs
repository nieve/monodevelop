using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MonoDevelop.Core;
using MonoDevelop.Projects;
using Mono.TextEditor;
using MonoDevelop.Ide;

namespace MonoDevelop.Stereo {
	public interface IExtractProjectFiles{
		IEnumerable<FilePath> GetFileNames(Solution solution, IProgressMonitor monitor);
		TextEditorData GetTextEditorData(FilePath filePath);
	}
	
	public class ExtractProjectFiles : IExtractProjectFiles {
		public TextEditorData GetTextEditorData (FilePath filePath)
		{
			return TextFileProvider.Instance.GetTextEditorData(filePath);
		}
		
		public IEnumerable<FilePath> GetFileNames(Solution solution, IProgressMonitor monitor)
	    {
			int counter = 0;
			ReadOnlyCollection<Project> allProjects = solution.GetAllProjects();
			if (monitor != null)
				monitor.BeginTask(GettextCatalog.GetString("Finding references in solution..."), 
          			allProjects.Sum<Project>(p => p.Files.Count));
				foreach (Project project in allProjects) {
					if (monitor != null && monitor.IsCancelRequested) yield break;
					foreach (ProjectFile projectFile in (Collection<ProjectFile>) project.Files) {
						if (monitor != null && monitor.IsCancelRequested) yield break;
						yield return projectFile.FilePath;
						if (monitor != null) {
							if (counter % 10 == 0) monitor.Step(10);
							++counter;
						}
					}
				}
	          if (monitor != null) monitor.EndTask();
	    }
	}
}