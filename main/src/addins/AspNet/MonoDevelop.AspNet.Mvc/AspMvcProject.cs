// 
// AspMvcProject.cs
//  
// Author:
//       Michael Hutchinson <mhutchinson@novell.com>
// 
// Copyright (c) 2009 Novell, Inc. (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using MonoDevelop.Projects;
using MonoDevelop.Core.Serialization;

namespace MonoDevelop.AspNet.Mvc
{
	public class AspMvcProject : AspNetAppProject
	{
		public AspMvcProject ()
		{
		}
		
		public AspMvcProject (string languageName)
			: base (languageName)
		{
		}
		
		public AspMvcProject (string languageName, ProjectCreateInformation info, XmlElement projectOptions)
			: base (languageName, info, projectOptions)
		{
		}	
		
		public override SolutionItemConfiguration CreateConfiguration (string name)
		{
			var conf = new AspMvcProjectConfiguration (name);
			conf.CopyFrom (base.CreateConfiguration (name));			
			return conf;
		}
		
		public override string ProjectType {
			get  { return "AspNetMvc"; }
		}
		
		public override bool SupportsFramework (MonoDevelop.Core.Assemblies.TargetFramework framework)
		{
			return framework.IsCompatibleWithFramework (MonoDevelop.Core.Assemblies.TargetFrameworkMoniker.NET_3_5);
		}
		
		public override IEnumerable<string> GetSpecialDirectories ()
		{
			foreach (string s in BaseGetSpecialDirectories ())
				yield return s;
			yield return "Views";
			yield return "Models";
			yield return "Controllers";
		}
		
		//so iterator can access base.GetSpecialDirectories () verifiably
		IEnumerable<string> BaseGetSpecialDirectories ()
		{
			return base.GetSpecialDirectories ();
		}
		
		public IList<string> GetCodeTemplates (string type)
		{
			List<string> files = new List<string> ();
			HashSet<string> names = new HashSet<string> ();
			
			string asmDir = Path.GetDirectoryName (typeof (AspMvcProject).Assembly.Location);
			
			string[] dirs = new string[] {
				Path.Combine (Path.Combine (this.BaseDirectory, "CodeTemplates"), type),
				Path.Combine (Path.Combine (asmDir, "CodeTemplates"), type)
			};
			
			foreach (string directory in dirs)
				if (Directory.Exists (directory))
					foreach (string file in Directory.GetFiles (directory, "*.tt", SearchOption.TopDirectoryOnly))
						if (names.Add (Path.GetFileName (file)))
						    files.Add (file);
			
			return files;
		}
		
		protected override void PopulateSupportFileList (MonoDevelop.Projects.FileCopySet list, ConfigurationSelector solutionConfiguration)
		{
			base.PopulateSupportFileList (list, solutionConfiguration);
			
			//HACK: workaround for MD not local-copying package references
			foreach (ProjectReference projectReference in References) {
				if (projectReference.Package != null && projectReference.Package.Name == "system.web.mvc") {
					if (projectReference.ReferenceType == ReferenceType.Package)
						foreach (MonoDevelop.Core.Assemblies.SystemAssembly assem in projectReference.Package.Assemblies)
							list.Add (assem.Location);
					break;
				}
			}
		}
	}
}
