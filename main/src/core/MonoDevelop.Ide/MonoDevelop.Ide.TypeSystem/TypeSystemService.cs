// 
// TypeSystemService.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2011 Mike Krüger <mkrueger@novell.com>
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
using System.Collections.Generic;
using System.Linq;
using System.IO; 
using MonoDevelop.Projects;
using Mono.Cecil;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.TypeSystem;
using Mono.Addins;
using MonoDevelop.Core;
using MonoDevelop.Ide;
using Mono.TextEditor;
using System.Threading;
using MonoDevelop.Core.ProgressMonitoring;
using MonoDevelop.Core.Collections;
using System.Xml;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.NRefactory;
using MonoDevelop.Core.AddIns;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using ICSharpCode.NRefactory.Documentation;
using ICSharpCode.NRefactory.CSharp;

namespace MonoDevelop.Ide.TypeSystem
{
	public static class TypeSystemServiceExt
	{
		public static Project GetProject (this IProjectContent content)
		{
			return TypeSystemService.GetProject (content.Location);
		}
		
		public static Project GetSourceProject (this ITypeDefinition type)
		{
			return TypeSystemService.GetProject (type.Compilation.MainAssembly.UnresolvedAssembly.Location);
		}
		
		public static Project GetSourceProject (this IType type)
		{
			return type.GetDefinition ().GetSourceProject ();
		}
		
		public static IProjectContent GetProjectContent (this IType type)
		{
			return TypeSystemService.GetProjectContext (type.GetSourceProject ());
		}
		
		public static TextLocation GetLocation (this IType type)
		{
			return type.GetDefinition ().Region.Begin;
		}
		
		public static bool IsBaseType (this IType type, IType potentialBase)
		{
			return type.GetAllBaseTypes ().Any (t => t.Equals (potentialBase));
		}
		
		public static bool IsObsolete (this IEntity member)
		{
			if (member == null)
				return false;
			return member.Attributes.Any (a => a.AttributeType.FullName == "System.ObsoleteAttribute");
		}
		
		public static IType Resolve (this IUnresolvedTypeDefinition def, Project project)
		{
			var pf = TypeSystemService.GetProjectContext (project).GetFile (def.Region.FileName);
			var ctx = pf.GetTypeResolveContext (TypeSystemService.GetCompilation (project), def.Region.Begin);
			return def.Resolve (ctx);
		}
		
		[Obsolete("Do not use this method. Use type references to resolve types. Type references from full reflection names can be got from ReflectionHelper.ParseReflectionName.")]
		public static ITypeDefinition LookupType (this ICompilation compilation, string ns, string name, int typeParameterCount = -1)
		{
			var tc = Math.Max (typeParameterCount, 0);
			ITypeDefinition result = null;
			foreach (var refAsm in compilation.Assemblies) {
				result = refAsm.GetTypeDefinition (ns, name, tc);
				if (result != null)
					return result;
			}
			if (typeParameterCount < 0) {
				for (int i = 1; i < 50; i++) {
					result = LookupType (compilation, ns, name, i);
					if (result != null)
						return result;
				}
			}
			return null;
		}

		[Obsolete("Do not use this method. Use type references to resolve types. Type references from full reflection names can be got from ReflectionHelper.ParseReflectionName.")]
		public static ITypeDefinition LookupType (this ICompilation compilation, string fullName, int typeParameterCount = -1)
		{
			int idx = fullName.LastIndexOf ('.');
			string ns, name;
			if (idx > 0) {
				ns = fullName.Substring (0, idx);
				name = fullName.Substring (idx + 1);
			} else {
				ns = "";
				name = fullName;
			}
			return compilation.LookupType (ns, name, typeParameterCount);
		}
	}
	
	/// <summary>
	/// The folding parser is used for generating a preliminary parsed document that does not
	/// contain a full dom - only some basic lexical constructs like comments or pre processor directives.
	/// 
	/// This is useful for opening a document the first time to have some folding regions as start that are folded by default.
	/// Otherwise an irritating screen update will occur.
	/// </summary>
	public interface IFoldingParser
	{
		ParsedDocument Parse (string fileName, string content);
	}
	
	public static class TypeSystemService
	{
		const string CurrentVersion = "1.0.1";
		static List<TypeSystemParserNode> parsers;
		public static readonly HashSet<string> FilesSkippedInParseThread = new HashSet<string> ();
		static IEnumerable<TypeSystemParserNode> Parsers {
			get {
				return parsers;
			}
		}
		
		static TypeSystemService ()
		{
			parsers = new List<TypeSystemParserNode> ();
			AddinManager.AddExtensionNodeHandler ("/MonoDevelop/TypeSystem/Parser", delegate (object sender, ExtensionNodeEventArgs args) {
				switch (args.Change) {
				case ExtensionChange.Add:
					parsers.Add ((TypeSystemParserNode)args.ExtensionNode);
					break;
				case ExtensionChange.Remove:
					parsers.Remove ((TypeSystemParserNode)args.ExtensionNode);
					break;
				}
			});
			FileService.FileChanged += delegate(object sender, FileEventArgs e) {
				if (!TrackFileChanges)
					return;
				foreach (var file in e) {
					// Open documents are handled by the Document class itself.
					if (IdeApp.Workbench != null && IdeApp.Workbench.GetDocument (file.FileName) != null)
						continue;
					//
					lock (projectWrapperUpdateLock) {
						foreach (var wrapper in projectContents.Values) {
							var projectFile = wrapper.Project.Files.GetFile (file.FileName);
							if (projectFile != null)
								QueueParseJob (wrapper, new [] { projectFile });
						}
						if (cachedAssemblyContents.ContainsKey (file.FileName))
							CheckModifiedFile (cachedAssemblyContents[file.FileName]);
					}
				}
			};
		}
		
		public static ITypeSystemParser GetParser (string mimeType)
		{
			var provider = Parsers.FirstOrDefault (p => p.CanParse (mimeType));
			return provider != null ? provider.Parser : null;
		}
		
		static TypeSystemParserNode GetTypeSystemParserNode (string mimeType)
		{
			return Parsers.FirstOrDefault (p => p.CanParse (mimeType));
		}
		
		static List<MimeTypeExtensionNode> foldingParsers;

		static IEnumerable<MimeTypeExtensionNode> FoldingParsers {
			get {
				if (foldingParsers == null) {
					foldingParsers = new List<MimeTypeExtensionNode> ();
					AddinManager.AddExtensionNodeHandler ("/MonoDevelop/TypeSystem/FoldingParser", delegate (object sender, ExtensionNodeEventArgs args) {
						switch (args.Change) {
						case ExtensionChange.Add:
							foldingParsers.Add ((MimeTypeExtensionNode)args.ExtensionNode);
							break;
						case ExtensionChange.Remove:
							foldingParsers.Remove ((MimeTypeExtensionNode)args.ExtensionNode);
							break;
						}
					});
				}
				return foldingParsers;
			}
		}
		
		public static IFoldingParser GetFoldingParser (string mimeType)
		{
			var node = FoldingParsers.Where (n => n.MimeType == mimeType).FirstOrDefault ();
			if (node == null)
				return null;
			return node.CreateInstance () as IFoldingParser;
		}

		public static ParsedDocument ParseFile (Project project, string fileName)
		{
			string text;
			
			try {
				if (!File.Exists (fileName))
					return null;
				text = Mono.TextEditor.Utils.TextFileUtility.ReadAllText (fileName);
			} catch (Exception) {
				return null;
			}
			
			return ParseFile (project, fileName, DesktopService.GetMimeTypeForUri (fileName), text);
		}
		static object projectWrapperUpdateLock = new object ();
		public static ParsedDocument ParseFile (Project project, string fileName, string mimeType, TextReader content)
		{
			var parser = GetParser (mimeType);
			if (parser == null)
				return null;
			try {
				var result = parser.Parse (true, fileName, content, project);
				lock (projectWrapperUpdateLock) {
					ProjectContentWrapper wrapper;
					if (project != null) {
						projectContents.TryGetValue (project, out wrapper);
					} else {
						wrapper = null;
					}
					if (wrapper != null && (result.Flags & ParsedDocumentFlags.NonSerializable) != ParsedDocumentFlags.NonSerializable) {
						var oldFile = wrapper.Content.GetFile (fileName);
						wrapper.UpdateContent (c => c.UpdateProjectContent (c.GetFile (fileName), result.ParsedFile));
						UpdateParsedDocument (wrapper, result);
						if (oldFile != null)
							wrapper.InformFileRemoved (new ParsedFileEventArgs (oldFile));
						wrapper.InformFileAdded (new ParsedFileEventArgs (result.ParsedFile));
					}

					// The parsed file could be included in other projects as well, therefore
					// they need to be updated.
					foreach (var cnt in projectContents) {
						if (cnt.Key == project)
							continue;
						// Use the project context because file lookup is faster there than in the project class.
						var file = cnt.Value.Content.GetFile (fileName);
						if (file != null) {
							cnt.Value.UpdateContent (c => c.UpdateProjectContent (file, result.ParsedFile));
							cnt.Value.InformFileRemoved (new ParsedFileEventArgs (file));
							cnt.Value.InformFileAdded (new ParsedFileEventArgs (result.ParsedFile));
						}
					}
				}
				return result;
			} catch (Exception e) {
				LoggingService.LogError ("Exception while parsing :" + e);
				return null;
			}
		}
		
		public static ParsedDocument ParseFile (Project project, string fileName, string mimeType, string content)
		{
			using (var reader = new StringReader (content))
				return ParseFile (project, fileName, mimeType, reader);
		}
		
		public static ParsedDocument ParseFile (Project project, TextEditorData data)
		{
			return ParseFile (project, data.FileName, data.MimeType, data.Text);
		}
		
		public static ParsedDocument ParseFile (string fileName, string mimeType, string text, ProjectContentWrapper wrapper = null)
		{
			using (var reader = new StringReader (text))
				return ParseFile (fileName, mimeType, reader, wrapper);
		}
		
		public static ParsedDocument ParseFile (string fileName, string mimeType, TextReader content, ProjectContentWrapper wrapper = null)
		{
			var parser = GetParser (mimeType);
			if (parser == null)
				return null;
			try {
				var result = parser.Parse (true, fileName, content);
				lock (projectWrapperUpdateLock) {
					if (wrapper != null && (result.Flags & ParsedDocumentFlags.NonSerializable) != ParsedDocumentFlags.NonSerializable) {
						var oldFile = wrapper.Content.GetFile (fileName);
						wrapper.UpdateContent (c => c.UpdateProjectContent (c.GetFile (fileName), result.ParsedFile));
						UpdateParsedDocument (wrapper, result);
						if (oldFile != null)
							wrapper.InformFileRemoved (new ParsedFileEventArgs (oldFile));
						wrapper.InformFileAdded (new ParsedFileEventArgs (result.ParsedFile));
					}
				}
				return result;
			} catch (Exception e) {
				LoggingService.LogError ("Exception while parsing :" + e);
				return null;
			}
		}
		
		public static event EventHandler ParseOperationStarted;
		
		internal static void StartParseOperation ()
		{
			if ((parseStatus++) == 0) {
				if (ParseOperationStarted != null)
					ParseOperationStarted (null, EventArgs.Empty);
			}
		}
		
		public static event EventHandler ParseOperationFinished;
		
		internal static void EndParseOperation ()
		{
			if (parseStatus == 0)
				return;
			if (--parseStatus == 0) {
				if (ParseOperationFinished != null)
					ParseOperationFinished (null, EventArgs.Empty);
			}
		}
		#region Parser Database Handling

		static string GetCacheDirectory (string filename)
		{
			string result;
			var nameNoExtension = Path.GetFileNameWithoutExtension (filename);
			var derivedDataPath = UserProfile.Current.CacheDir.Combine ("DerivedData");
			try {
				// First try to access what we think could be the correct file directly
				if (CheckCacheDirectoryIsCorrect (filename, derivedDataPath.Combine (nameNoExtension), out result))
					return result;
				
				if (Directory.Exists (derivedDataPath)) {
					// next check any directory which contains the filename
					foreach (var subDir in Directory.EnumerateDirectories (derivedDataPath).Where (s=> s.Contains (nameNoExtension))) {
						if (CheckCacheDirectoryIsCorrect (filename, subDir, out result)) {
							return result;
						}
					}
/*					// Finally check every remaining directory
					foreach (var subDir in subDirs.Where (s=> !s.Contains (nameNoExtension)))
						if (CheckCacheDirectoryIsCorrect (filename, subDir, out result))
							return result;*/
				}
			} catch (Exception e) {
				LoggingService.LogError ("Error while getting derived data directories.", e);
			}
			return null;
		}

		struct CacheDirectoryInfo
		{
			public static readonly CacheDirectoryInfo Empty = new CacheDirectoryInfo ();

			public string FileName { get; set; }
			public string Version { get; set; }
		}
		static Dictionary<FilePath, CacheDirectoryInfo> cacheDirectoryCache = new Dictionary<FilePath, CacheDirectoryInfo> ();

		static bool CheckCacheDirectoryIsCorrect (FilePath filename, FilePath candidate, out string result)
		{
			lock (cacheDirectoryCache) {
				CacheDirectoryInfo info;
				if (!cacheDirectoryCache.TryGetValue (candidate, out info)) {
					var dataPath = candidate.Combine ("data.xml");
				
					try {
						if (!File.Exists (dataPath)) {
								cacheDirectoryCache [candidate] = CacheDirectoryInfo.Empty;
							result = null;
							return false;
						}
						using (var reader = XmlReader.Create (dataPath)) {
							while (reader.Read ()) {
								if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "File") {
									info.Version = reader.GetAttribute ("version");
									info.FileName = reader.GetAttribute ("name");
								}
							}
						}
						cacheDirectoryCache [candidate] = info;
					} catch (Exception e) {
						LoggingService.LogError ("Error while reading derived data file " + dataPath, e);
					}
				}
	
				if (info.Version == CurrentVersion && info.FileName == filename) {
					result = candidate;
					return true;
				}
	
				result = null;
				return false;
			}
		}
		
		static string GetName (string baseName, int i)
		{
			if (i == 0)
				return baseName;
			return baseName + "-" + i;
		}
		
		static string CreateCacheDirectory (string fileName)
		{
			try {
				string derivedDataPath = UserProfile.Current.CacheDir.Combine ("DerivedData");
				string name = Path.GetFileNameWithoutExtension (fileName);
				string baseName = Path.Combine (derivedDataPath, name);
				int i = 0;
				while (Directory.Exists (GetName (baseName, i)))
					i++;
				
				string cacheDir = GetName (baseName, i);
				
				Directory.CreateDirectory (cacheDir);

				System.IO.File.WriteAllText (
					Path.Combine (cacheDir, "data.xml"),
					string.Format ("<DerivedData><File name=\"{0}\" version =\"{1}\"/></DerivedData>", fileName,CurrentVersion)
				);

				return cacheDir;
			} catch (Exception e) {
				LoggingService.LogError ("Error creating cache for " + fileName, e);
				return null;
			}
		}
		
		static T DeserializeObject<T> (string path) where T : class
		{
			try {
				using (var fs = new FileStream (path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan)) {
					using (var reader = new BinaryReaderWith7BitEncodedInts (fs)) {
						var s = new FastSerializer ();
						return (T)s.Deserialize (reader);
					}
				}
			} catch (Exception e) {
				LoggingService.LogError ("Error while reading type system cache.", e);
				return default(T);
			}
		}
		
		static void SerializeObject (string path, object obj)
		{
			if (obj == null)
				throw new ArgumentNullException ("obj");
			try {
				using (var fs = new FileStream (path, FileMode.Create, FileAccess.Write)) {
					using (var writer = new BinaryWriterWith7BitEncodedInts (fs)) {
						FastSerializer s = new FastSerializer ();
						s.Serialize (writer, obj);
					}
				}
			} catch (Exception e) {
				Console.WriteLine ("-----------------Serialize stack trace:");
				Console.WriteLine (Environment.StackTrace);
				LoggingService.LogError ("Error while writing type system cache. (object:" + obj.GetType () + ")", e);
			}
		}
		

		/// <summary>
		/// Removes all cache directories which are older than 30 days.
		/// </summary>
		static void CleanupCache ()
		{
			string derivedDataPath = UserProfile.Current.CacheDir.Combine ("DerivedData");
			string[] subDirs;
			
			try {
				if (!Directory.Exists (derivedDataPath))
					return;
				subDirs = Directory.GetDirectories (derivedDataPath);
			} catch (Exception e) {
				LoggingService.LogError ("Error while getting derived data directories.", e);
				return;
			}
			
			foreach (var subDir in subDirs) {
				try {
					var days = Math.Abs ((DateTime.Now - Directory.GetLastWriteTime (subDir)).TotalDays);
					if (days > 30)
						Directory.Delete (subDir, true);
				} catch (Exception e) {
					LoggingService.LogError ("Error while removing outdated cache " + subDir, e);
				}
			}
		}
		
		static void RemoveCache (string cacheDir)
		{
			try {
				Directory.Delete (cacheDir, true);
			} catch (Exception e) {
				LoggingService.LogError ("Error while removing cache " + cacheDir, e);
			}
		}
		
		static void TouchCache (string cacheDir)
		{
			try {
				Directory.SetLastWriteTime (cacheDir, DateTime.Now);
			} catch (Exception e) {
				LoggingService.LogError ("Error while touching cache directory " + cacheDir, e);
			}
		}

		static void StoreExtensionObject (string cacheDir, object extensionObject)
		{
			if (cacheDir == null)
				throw new ArgumentNullException ("cacheDir");
			if (extensionObject == null)
				throw new ArgumentNullException ("extensionObject");
			var fileName = Path.GetTempFileName ();
			SerializeObject (fileName, extensionObject);
			var cacheFile = Path.Combine (cacheDir, extensionObject.GetType ().FullName + ".cache");

			try {
				if (File.Exists (cacheFile))
					File.Delete (cacheFile);
				File.Move (fileName, cacheFile);
			} catch (Exception e) {
				LoggingService.LogError ("Error whil saving cache " + cacheFile + " for extension object:"+ extensionObject, e);
			}
		}

		static void StoreProjectCache (Project project, ProjectContentWrapper wrapper)
		{
			if (!wrapper.WasChanged)
				return;
			string cacheDir = GetCacheDirectory (project.FileName) ?? CreateCacheDirectory (project.FileName);
			TouchCache (cacheDir);
			string fileName = Path.GetTempFileName ();
			
			SerializeObject (fileName, wrapper.Content.RemoveAssemblyReferences (wrapper.Content.AssemblyReferences));
			
			string cacheFile = Path.Combine (cacheDir, "completion.cache");
			
			try {
				if (File.Exists (cacheFile))
					System.IO.File.Delete (cacheFile);
				System.IO.File.Move (fileName, cacheFile);
			} catch (Exception e) {
				LoggingService.LogError ("Error whil saving cache " + cacheFile, e);
			}

			foreach (var extensionObject in wrapper.ExtensionObjects) {
				StoreExtensionObject (cacheDir, extensionObject);
			}
		}
		#endregion
		
		#region Project loading
		public static void Load (WorkspaceItem item)
		{
			InternalLoad (item);
			CleanupCache ();
		}
		
		
		static void InternalLoad (WorkspaceItem item)
		{
			if (item is Workspace) {
				var ws = (Workspace)item;
				foreach (WorkspaceItem it in ws.Items)
					InternalLoad (it);
				ws.ItemAdded += OnWorkspaceItemAdded;
				ws.ItemRemoved += OnWorkspaceItemRemoved;
			} else if (item is Solution) {
				var solution = (Solution)item;
				Parallel.ForEach (solution.GetAllProjects (), project => LoadProject (project));
				Task.Factory.StartNew (delegate {
					ReloadAllReferences ();
				});

				solution.SolutionItemAdded += OnSolutionItemAdded;
				solution.SolutionItemRemoved += OnSolutionItemRemoved;
			}
		}
		
		static void ReloadAllReferences ()
		{
			lock (rwLock) {
				foreach (var wrapper in projectContents)
					wrapper.Value.ReloadAssemblyReferences (wrapper.Key);
			}
		}
		
		[Serializable]
		public class UnresolvedAssemblyDecorator : IUnresolvedAssembly
		{
			ProjectContentWrapper wrapper;
			
			IUnresolvedAssembly assembly {
				get {
					return wrapper.Compilation.MainAssembly.UnresolvedAssembly;
				}
			}
			
			public UnresolvedAssemblyDecorator (ProjectContentWrapper wrapper)
			{
				this.wrapper = wrapper;
			}

			#region IUnresolvedAssembly implementation
			public string AssemblyName {
				get {
					return assembly.AssemblyName;
				}
			}
			
			public string Location {
				get {
					return assembly.Location;
				}
			}

			public IEnumerable<IUnresolvedAttribute> AssemblyAttributes {
				get {
					return assembly.AssemblyAttributes;
				}
			}

			public IEnumerable<IUnresolvedAttribute> ModuleAttributes {
				get {
					return assembly.ModuleAttributes;
				}
			}

			public IEnumerable<IUnresolvedTypeDefinition> TopLevelTypeDefinitions {
				get {
					return assembly.TopLevelTypeDefinitions;
				}
			}
			#endregion

			#region IAssemblyReference implementation
			public IAssembly Resolve (ITypeResolveContext context)
			{
				return assembly.Resolve (context);
			}
			#endregion
		}
		
		[Serializable]
		public class ProjectContentWrapper
		{
			Dictionary<Type, object> extensionObjects = new Dictionary<Type, object> ();
			
			IProjectContent _content;
			public IProjectContent Content {
				get {
					return _content;
				}
				set {
					if (value == null)
						throw new InvalidOperationException ("Project content can't be null");
					_content = value;
				}
			}

			/// <summary>
			/// Gets the extension objects attached to the content wrapper.
			/// </summary>
			public IEnumerable<object> ExtensionObjects {
				get {
					return extensionObjects.Values;
				}
			}

			/// <summary>
			/// Updates an extension object for the wrapper. Note that only one extension object of a certain
			/// type may be stored inside the project content wrapper.
			/// 
			/// The extension objects need to be serializable and are stored in the project cache on project unload.
			/// </summary>
			public void UpdateExtensionObject (object ext)
			{
				if (ext == null)
					throw new ArgumentNullException ("ext");
				extensionObjects[ext.GetType ()] = ext;
			}

			/// <summary>
			/// Gets a specific extension object. This may lazy load an existing extension object from disk,
			/// if called the first time and a serialized extension object exists.
			/// </summary>
			/// <returns>
			/// The extension object. Or null, if no extension object of the specified type was registered.
			/// </returns>
			/// <typeparam name='T'>
			/// The type of the extension object.
			/// </typeparam>
			public T GetExtensionObject<T> () where T : class
			{
				object result;
				if (extensionObjects.TryGetValue (typeof (T), out result))
					return (T)result;

				string cacheDir = GetCacheDirectory (Project.FileName);
				if (cacheDir == null)
					return default(T);

				try {
					string fileName = Path.Combine (cacheDir, typeof (T).FullName + ".cache");
					if (File.Exists (fileName)) {
						var deserialized = DeserializeObject<T> (fileName);
						extensionObjects[typeof(T)] = deserialized;
						return deserialized;
					}
				} catch (Exception) {
					Console.WriteLine ("Can't deserialize :" + typeof (T).FullName);
				}

				return default (T);
			}

			public void UpdateContent (Func<IProjectContent, IProjectContent> updateFunc)
			{
				lock (this) {
					if (Content is LazyProjectLoader) {
						((LazyProjectLoader)Content).ContextTask.Wait ();
					}
					Content = updateFunc (Content);
					compilation = null;
					WasChanged = true;
				}
			}

			public void InformFileRemoved (ParsedFileEventArgs e)
			{
				var handler = FileRemoved;
				if (handler != null)
					handler (this, e);
			}

			public void InformFileAdded (ParsedFileEventArgs e)
			{
				var handler = FileAdded;
				if (handler != null)
					handler (this, e);
			}

			public EventHandler<ParsedFileEventArgs> FileAdded;
			public EventHandler<ParsedFileEventArgs> FileRemoved;


			public bool WasChanged = false;
			
			[NonSerialized]
			ICompilation compilation = null;
			
			public ICompilation Compilation {
				get {
					lock (this) {
						if (compilation == null) {
							compilation = Content.CreateCompilation ();
						}
						return compilation;
					}
				}
			}
			
			public Project Project {
				get;
				private set;
			}
			
			public ProjectContentWrapper (Project project)
			{
				if (project == null)
					throw new ArgumentNullException ("project");
				this.Project = project;
				this.Content = new LazyProjectLoader (this).Content;
			}
			
			public IEnumerable<Project> ReferencedProjects {
				get {
					foreach (var pr in Project.GetReferencedItems (ConfigurationSelector.Default)) {
						var referencedProject = pr as Project;
						if (referencedProject != null)
							yield return referencedProject;
					}
				}
			}

			class LazyProjectLoader : IProjectContent
			{
				readonly ProjectContentWrapper wrapper;
				static ConcurrentDictionary<string, IProjectContent> projectCache = new ConcurrentDictionary<string, IProjectContent> ();
				Task<IProjectContent> contextTask;

				public Task<IProjectContent> ContextTask {
					get {
						return contextTask;
					}
				}

				public IProjectContent Content {
					get {
						return contextTask.Result;
					}
				}

				public LazyProjectLoader (ProjectContentWrapper wrapper)
				{
					this.wrapper = wrapper;
					contextTask = Task.Factory.StartNew (delegate {
	
						IProjectContent content;
						if (projectCache.TryGetValue (this.wrapper.Project.FileName, out content)) {
							if (content != null)
								return content;
						}
	
						var context = LoadProjectCache (this.wrapper.Project);
						if (context != null) {
							return context.SetAssemblyName (this.wrapper.Project.Name) ?? context;
						}

						context = new CSharpProjectContent ();
						context = context.SetLocation (this.wrapper.Project.FileName);
						context = context.SetAssemblyName (this.wrapper.Project.Name);
						QueueParseJob (this.wrapper);
						return context;
					});
				}

				static IProjectContent LoadProjectCache (Project project)
				{
					string cacheDir = GetCacheDirectory (project.FileName);
					if (cacheDir == null) {
						return null;
					}
					
					TouchCache (cacheDir);
					var cache = DeserializeObject<IProjectContent> (Path.Combine (cacheDir, "completion.cache"));
					if (projectCache == null) {
						RemoveCache (cacheDir);
					} else {
						projectCache [project.FileName] = cache;
					}
					return cache;
				}

				#region IAssemblyReference implementation
				IAssembly IAssemblyReference.Resolve (ITypeResolveContext context)
				{
					return Content.Resolve (context);
				}
				#endregion

				#region IUnresolvedAssembly implementation
				string IUnresolvedAssembly.AssemblyName {
					get {
						return Content.AssemblyName;
					}
				}

				string IUnresolvedAssembly.Location {
					get {
						return Content.Location;
					}
				}

				IEnumerable<IUnresolvedAttribute> IUnresolvedAssembly.AssemblyAttributes {
					get {
						return Content.AssemblyAttributes;
					}
				}

				IEnumerable<IUnresolvedAttribute> IUnresolvedAssembly.ModuleAttributes {
					get {
						return Content.ModuleAttributes;
					}
				}

				IEnumerable<IUnresolvedTypeDefinition> IUnresolvedAssembly.TopLevelTypeDefinitions {
					get {
						return Content.TopLevelTypeDefinitions;
					}
				}
				#endregion

				#region IProjectContent implementation
				IParsedFile IProjectContent.GetFile (string fileName)
				{
					return Content.GetFile (fileName);
				}

				ICompilation IProjectContent.CreateCompilation ()
				{
					return Content.CreateCompilation ();
				}

				public ICompilation CreateCompilation (ISolutionSnapshot solutionSnapshot)
				{
					return Content.CreateCompilation (solutionSnapshot);
				}

				IProjectContent IProjectContent.SetAssemblyName (string newAssemblyName)
				{
					return Content.SetAssemblyName (newAssemblyName);
				}

				IProjectContent IProjectContent.SetLocation (string newLocation)
				{
					return Content.SetLocation (newLocation);
				}

				IProjectContent IProjectContent.AddAssemblyReferences (IEnumerable<IAssemblyReference> references)
				{
					return Content.AddAssemblyReferences (references);
				}

				IProjectContent IProjectContent.RemoveAssemblyReferences (IEnumerable<IAssemblyReference> references)
				{
					return Content.RemoveAssemblyReferences (references);
				}

				IProjectContent IProjectContent.UpdateProjectContent (IParsedFile oldFile, IParsedFile newFile)
				{
					return Content.UpdateProjectContent (oldFile, newFile);
				}

				public IProjectContent UpdateProjectContent (IEnumerable<IParsedFile> oldFiles, IEnumerable<IParsedFile> newFiles)
				{
					return Content.UpdateProjectContent (oldFiles, newFiles);
				}

				IEnumerable<IParsedFile> IProjectContent.Files {
					get {
						return Content.Files;
					}
				}

				IEnumerable<IAssemblyReference> IProjectContent.AssemblyReferences {
					get {
						return Content.AssemblyReferences;
					}
				}
				#endregion

				object compilerSettings;
				public IProjectContent SetCompilerSettings (object compilerSettings)
				{
					this.compilerSettings = compilerSettings;
					return this;
				}

				public object CompilerSettings {
					get {
						return compilerSettings;
					}
				}
			}

			bool HasCyclicRefs (ProjectContentWrapper wrapper)
			{
				foreach (var referencedProject in wrapper.ReferencedProjects) {
					ProjectContentWrapper w;
					if (referencedProject == Project || projectContents.TryGetValue (referencedProject, out w) && HasCyclicRefs (w)) {
						return true;
					}
				}
				return false;
			}

			public void ReloadAssemblyReferences (Project project)
			{
				var netProject = project as DotNetProject;
				if (netProject == null)
					return;
				try {
					var contexts = new List<IAssemblyReference> ();
					foreach (var referencedProject in ReferencedProjects) {
						ProjectContentWrapper wrapper;
						if (projectContents.TryGetValue (referencedProject, out wrapper)) {
							if (HasCyclicRefs (wrapper))
								continue;
							contexts.Add (new UnresolvedAssemblyDecorator (wrapper));
						}
					}
					
					AssemblyContext ctx;
					// Add mscorlib reference
					if (netProject.TargetRuntime != null && netProject.TargetRuntime.AssemblyContext != null) {
						var corLibRef = netProject.TargetRuntime.AssemblyContext.GetAssemblyForVersion (
							typeof(object).Assembly.FullName,
							null,
							netProject.TargetFramework
						);
						if (corLibRef != null) {
							ctx = LoadAssemblyContext (corLibRef.Location);
							if (ctx != null)
								contexts.Add (ctx);
						}
					}
					
					// Get the assembly references throught the project, since it may have custom references
					foreach (string file in netProject.GetReferencedAssemblies (ConfigurationSelector.Default, false)) {
						string fileName;
						if (!Path.IsPathRooted (file)) {
							fileName = Path.Combine (Path.GetDirectoryName (netProject.FileName), file);
						} else {
							fileName = Path.GetFullPath (file);
						}
						ctx = LoadAssemblyContext (fileName);
						if (ctx != null)
							contexts.Add (ctx);
					}
					bool changed = WasChanged;
					UpdateContent (c => c.RemoveAssemblyReferences (Content.AssemblyReferences));
					UpdateContent (c => c.AddAssemblyReferences (contexts));
					WasChanged = changed;
				} catch (Exception e) {
					if (netProject.TargetRuntime == null) {
						LoggingService.LogError ("Target runtime was null:" + Project);
					} else if (netProject.TargetRuntime.AssemblyContext == null) {
						LoggingService.LogError ("Target runtime assambly context was null:" + Project);
					}
					LoggingService.LogError ("Error while reloading all references of project:" + Project, e);
				}
			}
		}
		
		static Dictionary<Project, ProjectContentWrapper> projectContents = new Dictionary<Project, ProjectContentWrapper> ();
		static Dictionary<Project, int> referenceCounter = new Dictionary<Project, int> ();

		public static ProjectContentWrapper LoadProject (Project project)
		{
			if (IncLoadCount (project) != 1) 
				return null;
			lock (rwLock) {
				if (projectContents.ContainsKey (project))
					return null;
				try {
					ProjectContentWrapper wrapper;
					projectContents [project] = wrapper = new ProjectContentWrapper (project);
					referenceCounter [project] = 1;
					OnProjectContentLoaded (new ProjectContentEventArgs (project, wrapper.Content));
					project.FileAddedToProject += OnFileAdded;
					project.FileRemovedFromProject += OnFileRemoved;
					project.FileRenamedInProject += OnFileRenamed;
					project.Modified += OnProjectModified;
					var files = project.Files.ToArray ();
					Task.Factory.StartNew (delegate {
						CheckModifiedFiles (project, files, wrapper);
					});

					return wrapper;
				} catch (Exception ex) {
					LoggingService.LogError ("Parser database for project '" + project.Name + " could not be loaded", ex);
				}
				return null;
			}
		}
		
		public static Project GetProject (IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException ("entity");
			return GetProject (entity.ParentAssembly.UnresolvedAssembly.Location);
		}
		
		public static Project GetProject (string location)
		{
			foreach (var wrapper in projectContents) 
				if (wrapper.Value.Compilation.MainAssembly.UnresolvedAssembly.Location == location)
					return wrapper.Key;
			return null;
		}

		#region Project modification handlers
		static void OnFileAdded (object sender, ProjectFileEventArgs args)
		{
			var project = (Project)sender;
			foreach (ProjectFileEventInfo fargs in args) {
				QueueParseJob (projectContents [project], new [] { fargs.ProjectFile });
			}
		}

		static void OnFileRemoved (object sender, ProjectFileEventArgs args)
		{
			var project = (Project)sender;
			foreach (ProjectFileEventInfo fargs in args) {
				var wrapper = projectContents [project];
				var fileName = fargs.ProjectFile.Name;
				var file = wrapper.Content.GetFile (fileName);
				if (file == null)
					continue;
				wrapper.UpdateContent (c => c.UpdateProjectContent (c.GetFile (fileName), null));
				wrapper.InformFileRemoved (new ParsedFileEventArgs (file));

				var tags = wrapper.GetExtensionObject <ProjectCommentTags> ();
				if (tags != null)
					tags.RemoveFile (wrapper.Project, fileName);
			}
		}

		static void OnFileRenamed (object sender, ProjectFileRenamedEventArgs args)
		{
			var project = (Project)sender;
			foreach (ProjectFileRenamedEventInfo fargs in args) {
				var content = projectContents [project];
				var file = content.Content.GetFile (fargs.OldName);
				if (file == null)
					continue;
				content.UpdateContent (c => c.UpdateProjectContent (c.GetFile (fargs.OldName), null));
				content.InformFileRemoved (new ParsedFileEventArgs (file));

				QueueParseJob (content, new [] { fargs.ProjectFile });
			}
		}
		
		static void OnProjectModified (object sender, SolutionItemModifiedEventArgs args)
		{
			if (!args.Any (x => x is SolutionItemModifiedEventInfo && (((SolutionItemModifiedEventInfo)x).Hint == "TargetFramework" || ((SolutionItemModifiedEventInfo)x).Hint == "References")))
				return;
			var project = (Project)sender;
			
			ProjectContentWrapper wrapper;
			projectContents.TryGetValue (project, out wrapper);
			if (wrapper == null)
				return;
			ReloadAllReferences ();
		}
		#endregion
		
		public static event EventHandler<ProjectContentEventArgs> ProjectContentLoaded;

		static void OnProjectContentLoaded (ProjectContentEventArgs e)
		{
			var handler = ProjectContentLoaded;
			if (handler != null)
				handler (null, e);
		}
		
		public static void Unload (WorkspaceItem item)
		{
			if (item is Workspace) {
				var ws = (Workspace)item;
				foreach (WorkspaceItem it in ws.Items)
					Unload (it);
				ws.ItemAdded -= OnWorkspaceItemAdded;
				ws.ItemRemoved -= OnWorkspaceItemRemoved;
			} else if (item is Solution) {
				Solution solution = (Solution)item;
				Parallel.ForEach (solution.GetAllProjects (), project => UnloadProject (project));
				solution.SolutionItemAdded -= OnSolutionItemAdded;
				solution.SolutionItemRemoved -= OnSolutionItemRemoved;
			}
		}
		
		public static void UnloadProject (Project project)
		{
			if (DecLoadCount (project) != 0)
				return;
			lock (projectWrapperUpdateLock) {
				if (referenceCounter.ContainsKey (project) && --referenceCounter [project] <= 0) {
					project.FileAddedToProject -= OnFileAdded;
					project.FileRemovedFromProject -= OnFileRemoved;
					project.FileRenamedInProject -= OnFileRenamed;
					project.Modified -= OnProjectModified;
					
					var wrapper = projectContents [project];
					projectContents.Remove (project);
					referenceCounter.Remove (project);

					StoreProjectCache (project, wrapper);
					OnProjectUnloaded (new ProjectUnloadEventArgs (project, wrapper));
				}
			}
		}
		
		public static event EventHandler<ProjectUnloadEventArgs> ProjectUnloaded;

		static void OnProjectUnloaded (ProjectUnloadEventArgs e)
		{
			var handler = ProjectUnloaded;
			if (handler != null)
				handler (null, e);
		}
		
		static void OnWorkspaceItemAdded (object s, WorkspaceItemEventArgs args)
		{
			Load (args.Item);
		}
		
		static void OnWorkspaceItemRemoved (object s, WorkspaceItemEventArgs args)
		{
			Unload (args.Item);
		}
		
		static void OnSolutionItemAdded (object sender, SolutionItemChangeEventArgs args)
		{
			if (args.SolutionItem is Project) {
				var wrapper = LoadProject ((Project)args.SolutionItem);
				if (wrapper != null)
					wrapper.ReloadAssemblyReferences (wrapper.Project);
			}
		}
		
		static void OnSolutionItemRemoved (object sender, SolutionItemChangeEventArgs args)
		{
			if (args.SolutionItem is Project)
				UnloadProject ((Project)args.SolutionItem);
		}
		
		#endregion

		#region Reference Counting
		static Dictionary<object,int> loadCount = new Dictionary<object,int> ();
		static object rwLock = new object ();
		
		static int DecLoadCount (object ob)
		{
			lock (rwLock) {
				int c;
				if (loadCount.TryGetValue (ob, out c)) {
					c--;
					if (c == 0)
						loadCount.Remove (ob);
					else
						loadCount [ob] = c;
					return c;
				}
				LoggingService.LogError ("DecLoadCount: Object not registered.");
				return 0;
			}
		}
		
		static int IncLoadCount (object ob)
		{
			lock (rwLock) {
				int c;
				if (loadCount.TryGetValue (ob, out c)) {
					c++;
					loadCount [ob] = c;
					return c;
				} else {
					loadCount [ob] = 1;
					return 1;
				}
			}
		}
		#endregion
		
		
		class SimpleAssemblyResolver : IAssemblyResolver
		{
			string lookupPath;
			Dictionary<string, AssemblyDefinition> cache = new Dictionary<string, AssemblyDefinition> ();
			DefaultAssemblyResolver defaultResolver = new DefaultAssemblyResolver ();
			
			public SimpleAssemblyResolver (string lookupPath)
			{
				this.lookupPath = lookupPath;
			}

			public AssemblyDefinition InternalResolve (string fullName)
			{
				AssemblyDefinition result;
				if (cache.TryGetValue (fullName, out result))
					return result;
				
				var name = AssemblyNameReference.Parse (fullName);
				AssemblyDefinition bestFit = null;
				// need to handle different file extension casings. Some dlls from windows tend to end with .Dll or .DLL rather than '.dll'
				foreach (string file in Directory.GetFiles (lookupPath, name.Name + ".*")) {
					string ext = Path.GetExtension (file);
					if (string.IsNullOrEmpty (ext))
						continue;
					ext = ext.ToUpper ();
					if (ext == ".DLL" || ext == ".EXE") {
						result = ReadAssembly (file);
						if (result.FullName != fullName) {
							bestFit = result;
							result = null;
							continue;
						}
						break;
					}
				}
				
				if (result == null) {
					var framework = MonoDevelop.Projects.Services.ProjectService.DefaultTargetFramework;
					var assemblyName = Runtime.SystemAssemblyService.DefaultAssemblyContext.GetAssemblyFullName (fullName, framework);
					var location = Runtime.SystemAssemblyService.DefaultAssemblyContext.GetAssemblyLocation (assemblyName, framework);
					
					if (!string.IsNullOrEmpty (location) && File.Exists (location)) {
						result = ReadAssembly (location);
					}
				}
				if (result == null)
					result = bestFit;
				if (result != null)
					cache [fullName] = result;
				return result;
			}

			#region IAssemblyResolver implementation
			public AssemblyDefinition Resolve (AssemblyNameReference name)
			{
				return InternalResolve (name.FullName) ?? defaultResolver.Resolve (name);
			}

			public AssemblyDefinition Resolve (AssemblyNameReference name, ReaderParameters parameters)
			{
				return InternalResolve (name.FullName) ?? defaultResolver.Resolve (name, parameters);
			}

			public AssemblyDefinition Resolve (string fullName)
			{
				return InternalResolve (fullName) ?? defaultResolver.Resolve (fullName);
			}

			public AssemblyDefinition Resolve (string fullName, ReaderParameters parameters)
			{
				return InternalResolve (fullName) ?? defaultResolver.Resolve (fullName, parameters);
			}
			#endregion
		}
		
		static AssemblyDefinition ReadAssembly (string fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException ("fileName");
			ReaderParameters parameters = new ReaderParameters ();
			parameters.AssemblyResolver = new DefaultAssemblyResolver (); // new SimpleAssemblyResolver (Path.GetDirectoryName (fileName));
			using (var stream = new MemoryStream (File.ReadAllBytes (fileName))) {
				return AssemblyDefinition.ReadAssembly (stream, parameters);
			}
		}
		
		static bool GetXml (string baseName, MonoDevelop.Core.Assemblies.TargetRuntime runtime, out FilePath xmlFileName)
		{
			string filePattern = Path.GetFileNameWithoutExtension (baseName) + ".*";
			try {
				foreach (string fileName in Directory.EnumerateFileSystemEntries (Path.GetDirectoryName (baseName), filePattern)) {
					if (fileName.ToLower ().EndsWith (".xml")) {
						xmlFileName = LookupLocalizedXmlDoc (fileName);
						return true;
					}
				}
			} catch (Exception e) {
				LoggingService.LogError ("Error while retrieving file system entries.", e);
			}
			
			if (MonoDevelop.Core.Platform.IsWindows) {
				string windowsFileName = FindWindowsXmlDocumentation (baseName, runtime);
				if (File.Exists (windowsFileName)) {
					xmlFileName = windowsFileName;
					return true;
				}
			}
			
			xmlFileName = "";
			return false;
		}
		
		#region Lookup XML documentation

		// ProgramFilesX86 is broken on 32-bit WinXP, this is a workaround
		static string GetProgramFilesX86 ()
		{
			return Environment.GetFolderPath (IntPtr.Size == 8?
				Environment.SpecialFolder.ProgramFilesX86 : Environment.SpecialFolder.ProgramFiles);
		}

		static readonly string referenceAssembliesPath = Path.Combine (GetProgramFilesX86 (), @"Reference Assemblies\Microsoft\\Framework");
		static readonly string frameworkPath = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Windows), @"Microsoft.NET\Framework");
		
		static string FindWindowsXmlDocumentation (string assemblyFileName, MonoDevelop.Core.Assemblies.TargetRuntime runtime)
		{
			string fileName;
			ClrVersion version = runtime != null && runtime.CustomFrameworks.Any () ? runtime.CustomFrameworks.First ().ClrVersion : ClrVersion.Default;
			switch (version) {
//			case "1.0":
//				fileName = LookupLocalizedXmlDoc (Path.Combine (frameworkPath, "v1.0.3705", assemblyFileName));
//				break;
			case ClrVersion.Net_1_1:
				fileName = LookupLocalizedXmlDoc (Path.Combine (frameworkPath, "v1.1.4322", assemblyFileName));
				break;
			case ClrVersion.Net_2_0:
			case ClrVersion.Clr_2_1:
				fileName = LookupLocalizedXmlDoc (Path.Combine (frameworkPath, "v2.0.50727", assemblyFileName))
						?? LookupLocalizedXmlDoc (Path.Combine (referenceAssembliesPath, "v3.5"))
						?? LookupLocalizedXmlDoc (Path.Combine (referenceAssembliesPath, "v3.0"))
						?? LookupLocalizedXmlDoc (Path.Combine (referenceAssembliesPath, @".NETFramework\v3.5\Profile\Client"));
				break;
			case ClrVersion.Net_4_0:
			case ClrVersion.Default:
			default:
				fileName = LookupLocalizedXmlDoc (Path.Combine (referenceAssembliesPath, @".NETFramework\v4.0", assemblyFileName))
						?? LookupLocalizedXmlDoc (Path.Combine (frameworkPath, "v4.0.30319", assemblyFileName));
				break;
			}
			return fileName;
		}
		
		static string LookupLocalizedXmlDoc (string fileName)
		{
			return XmlDocumentationProvider.LookupLocalizedXmlDoc (fileName);
		}
		#endregion

		
		[Serializable]
		class AssemblyContext : IUnresolvedAssembly
		{
			public string FileName;
			public DateTime LastWriteTimeUtc;
			
			[NonSerialized]
			internal LazyAssemblyLoader CtxLoader;
			
			public IUnresolvedAssembly Ctx {
				get {
					return CtxLoader.Assembly;
				}
			}

			#region IUnresolvedAssembly implementation
			string IUnresolvedAssembly.AssemblyName {
				get {
					return Ctx.AssemblyName;
				}
			}

			string IUnresolvedAssembly.Location {
				get {
					return Ctx.Location;
				}
			}

			IEnumerable<IUnresolvedAttribute> IUnresolvedAssembly.AssemblyAttributes {
				get {
					return Ctx.AssemblyAttributes;
				}
			}

			IEnumerable<IUnresolvedAttribute> IUnresolvedAssembly.ModuleAttributes {
				get {
					return Ctx.ModuleAttributes;
				}
			}

			IEnumerable<IUnresolvedTypeDefinition> IUnresolvedAssembly.TopLevelTypeDefinitions {
				get {
					return Ctx.TopLevelTypeDefinitions;
				}
			}
			#endregion

			#region IAssemblyReference implementation
			IAssembly IAssemblyReference.Resolve (ITypeResolveContext context)
			{
				return Ctx.Resolve (context);
			}
			#endregion
		}
		
		class LazyAssemblyLoader
		{
			string fileName;
			string cache;
			
			IUnresolvedAssembly assembly;

			void EnsureAssemblyLoaded ()
			{
				lock (this) {
					if (assembly != null)
						return;
					assembly = LoadAssembly () ?? new DefaultUnresolvedAssembly (fileName);
				}
			}

			public IUnresolvedAssembly Assembly {
				get {
					lock (this) {
						EnsureAssemblyLoaded ();
						return assembly;
					}
				}
			}

			public LazyAssemblyLoader (string fileName, string cache)
			{
				this.fileName = fileName;
				this.cache = cache;
			}
			
			IUnresolvedAssembly LoadAssembly ()
			{
				var assemblyPath = Path.Combine (cache, "assembly.data");
				try {
					if (File.Exists (assemblyPath)) {
						var deserializedAssembly = DeserializeObject <IUnresolvedAssembly> (assemblyPath);
						if (deserializedAssembly != null) {
						/*	var provider = deserializedAssembly as IDocumentationProviderContainer;
							if (provider != null)
								provider.DocumentationProvider = new CombinedDocumentationProvider (fileName);*/
							return deserializedAssembly;
						}
					}
				} catch (Exception) {
				}
				var asm = ReadAssembly (fileName);
				if (asm == null)
					return null;
				
				IUnresolvedAssembly assembly;
				try {
					var loader = new CecilLoader ();
					loader.IncludeInternalMembers = true;
					loader.DocumentationProvider = new CombinedDocumentationProvider (fileName);
					assembly = loader.LoadAssembly (asm);
				} catch (Exception e) {
					LoggingService.LogError ("Can't convert assembly: " + fileName, e);
					return null;
				}

				if (cache != null)
					SerializeObject (assemblyPath, assembly);
				return assembly;
			}
		}
		
		[Serializable]
		class CombinedDocumentationProvider : IDocumentationProvider
		{
			string fileName;
			
			[NonSerialized]
			IDocumentationProvider baseProvider = null;

			public IDocumentationProvider BaseProvider {
				get {
					if (baseProvider == null) {
						FilePath xmlDocFile;
						if (GetXml (fileName, null, out xmlDocFile)) {
							try {
								baseProvider = new ICSharpCode.NRefactory.Documentation.XmlDocumentationProvider (xmlDocFile);
							} catch (Exception ex) {
								LoggingService.LogWarning ("Ignoring error while reading xml doc from " + xmlDocFile, ex);
							} 
						}
						if (baseProvider == null)
							baseProvider = new MonoDocDocumentationProvider ();
					}
					return baseProvider;
				}
			}
			
			public CombinedDocumentationProvider (string fileName)
			{
				this.fileName = fileName;
			}

			#region IDocumentationProvider implementation
			public DocumentationComment GetDocumentation (IEntity entity)
			{
				var provider = BaseProvider;
				return provider != null ? provider.GetDocumentation (entity) : null;
			}
			#endregion
		}
		
		static AssemblyContext LoadAssemblyContext (string fileName)
		{
			AssemblyContext loadedContext;
			if (cachedAssemblyContents.TryGetValue (fileName, out loadedContext)) {
				CheckModifiedFile (loadedContext);
				return loadedContext;
			}
			if (!File.Exists (fileName))
				return null;
			string cache = GetCacheDirectory (fileName);
			if (cache != null) {
				TouchCache (cache);
				var deserialized = DeserializeObject <AssemblyContext> (Path.Combine (cache, "assembly.descriptor"));
				if (deserialized != null) {
					deserialized.CtxLoader = new LazyAssemblyLoader (fileName, cache);
					CheckModifiedFile (deserialized);
					cachedAssemblyContents [fileName] = deserialized;
					return deserialized;
				} else {
					RemoveCache (cache);
				}
			}
			cache = CreateCacheDirectory (fileName);

			try {
				var result = new AssemblyContext () {
					FileName = fileName,
					LastWriteTimeUtc = File.GetLastWriteTimeUtc (fileName)
				};
				SerializeObject (Path.Combine (cache, "assembly.descriptor"), result);
				
				result.CtxLoader = new LazyAssemblyLoader (fileName, cache);
				cachedAssemblyContents [fileName] = result;
				return result;
			} catch (Exception ex) {
				LoggingService.LogError ("Error loading assembly " + fileName, ex);
				return null;
			}
		}

		public static IUnresolvedAssembly LoadAssemblyContext (MonoDevelop.Core.Assemblies.TargetRuntime runtime, MonoDevelop.Core.Assemblies.TargetFramework fx, string fileName)
		{
			if (File.Exists (fileName))
				return LoadAssemblyContext (fileName);
			var corLibRef = runtime.AssemblyContext.GetAssemblyForVersion (fileName, null, fx);
			if (corLibRef == null)
				return null;
			return LoadAssemblyContext (corLibRef.Location);
		}
		
		public static IProjectContent GetProjectContext (Project project)
		{
			if (project == null)
				throw new ArgumentNullException ("project");
			var content = GetProjectContentWrapper (project);
			return content.Content;
		}
		
		public static ICompilation GetCompilation (Project project)
		{
			if (project == null)
				throw new ArgumentNullException ("project");
			var content = GetProjectContentWrapper (project);
			return content.Compilation;
		}
		
		public static ProjectContentWrapper GetProjectContentWrapper (Project project)
		{
			if (project == null)
				throw new ArgumentNullException ("project");
			ProjectContentWrapper content;
			if (projectContents.TryGetValue (project, out content)) {
				return content;
			}
			return new ProjectContentWrapper (project);
		}
		
		public static IProjectContent GetContext (FilePath file, string mimeType, string text)
		{
			using (var reader = new StringReader (text)) {
				var parsedDocument = ParseFile (file, mimeType, reader);
				
				var content = new ICSharpCode.NRefactory.CSharp.CSharpProjectContent ();
				return content.UpdateProjectContent (null, parsedDocument.ParsedFile);
			}
		}
		
		static Dictionary<string, AssemblyContext> cachedAssemblyContents = new Dictionary<string, AssemblyContext> ();
		
		public static void ForceUpdate (ProjectContentWrapper context)
		{
			CheckModifiedFiles ();
		}
		
		#region Parser queue
		static bool threadRunning;
		
		public static IProgressMonitorFactory ParseProgressMonitorFactory {
			get;
			set;
		}
		
			
		class InternalProgressMonitor
		: NullProgressMonitor
		{
			public InternalProgressMonitor ()
			{
				StartParseOperation ();
			}
			
			public override void Dispose ()
			{
				EndParseOperation ();
			}
		}

		internal static IProgressMonitor GetParseProgressMonitor ()
		{
			IProgressMonitor mon;
			if (ParseProgressMonitorFactory != null)
				mon = ParseProgressMonitorFactory.CreateProgressMonitor ();
			else
				mon = new NullProgressMonitor ();
			
			return new AggregatedProgressMonitor (mon, new InternalProgressMonitor ());
		}
		
		static Queue<ParsingJob> parseQueue = new Queue<ParsingJob> ();
		class ParsingJob
		{
			public ProjectContentWrapper Context;
			public IEnumerable<ProjectFile> FileList;
//			public Action<string, IProgressMonitor> ParseCallback;
			
			public void Run (IProgressMonitor monitor)
			{
				TypeSystemParserNode node = null;
				ITypeSystemParser parser = null;
				lock (FilesSkippedInParseThread) {
					foreach (var file in (FileList ?? Context.Project.Files)) {
						if (!string.Equals (file.BuildAction, "compile", StringComparison.OrdinalIgnoreCase)) 
							continue;
						var fileName = file.FilePath;
						if (FilesSkippedInParseThread.Contains (fileName))
							continue;
						if (node == null || !node.CanParse (fileName)) {
							node = TypeSystemService.GetTypeSystemParserNode (DesktopService.GetMimeTypeForUri (fileName));
							parser = node != null ? node.Parser : null;
						}
						if (parser == null)
							continue;
						using (var stream = new System.IO.StreamReader (fileName)) {
							var parsedDocument = parser.Parse (false, fileName, stream, Context.Project);
							UpdateParsedDocument (Context, parsedDocument);
							var oldFile = Context.Content.GetFile (fileName);
							Context.UpdateContent (c => c.UpdateProjectContent (c.GetFile (fileName), parsedDocument.ParsedFile));
							if (oldFile != null)
								Context.InformFileRemoved (new ParsedFileEventArgs (oldFile));
							Context.InformFileAdded (new ParsedFileEventArgs (parsedDocument.ParsedFile));
						}
					}
				}
			}
		}

		static void UpdateParsedDocument (ProjectContentWrapper context, ParsedDocument parsedDocument)
		{
			var tags = context.GetExtensionObject <ProjectCommentTags> ();
			if (tags == null) {
				tags = new ProjectCommentTags ();
				context.UpdateExtensionObject (tags);
				tags.Update (context.Project);
			}
			tags.UpdateTags (context.Project, parsedDocument.FileName, parsedDocument.TagComments);
		}

		public static event EventHandler<ProjectFileEventArgs> FileParsed;

		static object parseQueueLock = new object ();
		static AutoResetEvent parseEvent = new AutoResetEvent (false);
		static ManualResetEvent queueEmptied = new ManualResetEvent (true);
		static bool trackingFileChanges;
		
		public static bool TrackFileChanges {
			get {
				return trackingFileChanges;
			}
			set {
				lock (parseQueueLock) {
					if (value != trackingFileChanges) {
						trackingFileChanges = value;
						if (value)
							StartParserThread ();
					}
				}
			}
		}
		
		static int parseStatus;
		
		public static bool IsParsing {
			get { return parseStatus > 0; }
		}
		
		static Dictionary<ProjectContentWrapper, ParsingJob> parseQueueIndex = new Dictionary<ProjectContentWrapper, ParsingJob> ();

		internal static int PendingJobCount {
			get {
				lock (parseQueueLock) {
					return parseQueueIndex.Count;
				}
			}
		}
		
		static void QueueParseJob (ProjectContentWrapper context, IEnumerable<ProjectFile> fileList = null)
		{
			var job = new ParsingJob () {
				Context = context,
				FileList = fileList
			};
			lock (parseQueueLock) {
				RemoveParseJob (context);
				parseQueueIndex [context] = job;
				parseQueue.Enqueue (job);
				parseEvent.Set ();
				
				if (parseQueueIndex.Count == 1)
					queueEmptied.Reset ();
			}
		}
		
		static bool WaitForParseJob (int timeout)
		{
			return parseEvent.WaitOne (5000, true);
		}
		
		static ParsingJob DequeueParseJob ()
		{
			lock (parseQueueLock) {
				if (parseQueue.Count > 0) {
					var job = parseQueue.Dequeue ();
					parseQueueIndex.Remove (job.Context);
					return job;
				}
				return null;
			}
		}
		
		internal static void WaitForParseQueue ()
		{
			queueEmptied.WaitOne ();
		}
		
		static void RemoveParseJob (ProjectContentWrapper project)
		{
			lock (parseQueueLock) {
				ParsingJob job;
				if (parseQueueIndex.TryGetValue (project, out job)) {
					parseQueueIndex.Remove (project);
				}
			}
		}
		
		static void RemoveParseJobs (IProjectContent context)
		{
			lock (parseQueueLock) {
				foreach (var pj in parseQueue) {
					if (pj.Context == context) {
						parseQueueIndex.Remove (pj.Context);
					}
				}
			}
		}
		
		static void StartParserThread ()
		{
			lock (parseQueueLock) {
				if (!threadRunning) {
					threadRunning = true;
					var t = new Thread (new ThreadStart (ParserUpdateThread));
					t.Name = "Background parser";
					t.IsBackground = true;
					t.Priority = ThreadPriority.AboveNormal;
					t.Start ();
				}
			}
		}
		
		static void ParserUpdateThread ()
		{
			try {
				while (trackingFileChanges) {
					WaitForParseJob (5000);
//						CheckModifiedFiles ();
					if (trackingFileChanges)
						ConsumeParsingQueue ();
				}
			} catch (Exception ex) {
				LoggingService.LogError ("Unhandled error in parsing thread", ex);
			}
			lock (parseQueueLock) {
				threadRunning = false;
				if (trackingFileChanges)
					StartParserThread ();
			}
		}

		static bool IsFileModified (ProjectFile file, IParsedFile parsedFile)
		{
			if (parsedFile == null)
				return true;
			try {
				return System.IO.File.GetLastWriteTimeUtc (file.FilePath) > parsedFile.LastWriteTime;
			} catch (Exception) {
				return false;
			}
		}

		static void CheckModifiedFiles (Project project, ProjectFile[] projectFiles, ProjectContentWrapper content)
		{
			try {
				lock (projectWrapperUpdateLock) {
					List<ProjectFile> modifiedFiles = null;
					foreach (var file in projectFiles) {
						if (file.BuildAction == null || !string.Equals (file.BuildAction, "compile", StringComparison.OrdinalIgnoreCase)) 
							continue;
						var fileName = file.Name;
						// if the file is already inside the content a parser exists for it, if not check if it can be parsed.
						var oldFile = content.Content.GetFile (fileName);
						if (oldFile == null) {
							var parser = TypeSystemService.GetParser (DesktopService.GetMimeTypeForUri (fileName));
							if (parser == null)
								continue;
						}
						if (!IsFileModified (file, oldFile))
							continue;
						if (modifiedFiles == null)
							modifiedFiles = new List<ProjectFile> ();
						modifiedFiles.Add (file);
					}
					
					// check if file needs to be removed from project content 
					foreach (var file in content.Content.Files) {
						if (project.GetProjectFile (file.FileName) == null) {
							content.UpdateContent (c => c.UpdateProjectContent (file, null));
							content.InformFileRemoved (new ParsedFileEventArgs (file));
						}
					}
					
					if (modifiedFiles == null)
						return;
					QueueParseJob (content, modifiedFiles);
				}
			} catch (Exception e) {
				LoggingService.LogError ("Exception in check modified files.", e);
			}
		}

		static void CheckModifiedFile (AssemblyContext context)
		{
			try {
				var writeTime = File.GetLastWriteTimeUtc (context.FileName);
				if (writeTime != context.LastWriteTimeUtc) {
					string cache = GetCacheDirectory (context.FileName);
					context.LastWriteTimeUtc = writeTime;
					if (cache != null) {
						context.CtxLoader = new LazyAssemblyLoader (context.FileName, cache);
						SerializeObject (Path.Combine (cache, "assembly.descriptor"), context);
						try {
							// File is reloaded by the lazy loader
							File.Delete (Path.Combine (cache, "assembly.data"));
						} catch (Exception) {
						}
					}
				}
			} catch (Exception e) {
				LoggingService.LogError ("Error while updating assembly " + context.FileName, e);
			}
		}
		
		static void CheckModifiedFiles ()
		{
			Queue<KeyValuePair<Project, ProjectContentWrapper>> list;
			
			lock (rwLock) {
				list = new Queue<KeyValuePair<Project, ProjectContentWrapper>> (projectContents);
			}
			
			while (list.Count > 0) {
				var readydb = list.Dequeue ();
				var files = readydb.Key.Files.ToArray ();
				CheckModifiedFiles (readydb.Key, files, readydb.Value);
			}
			
			Queue<KeyValuePair<string, AssemblyContext>> assemblyList;
			
			lock (rwLock) {
				assemblyList = new Queue<KeyValuePair<string, AssemblyContext>> (cachedAssemblyContents);
			}
			
			while (assemblyList.Count > 0) {
				var readydb = assemblyList.Dequeue ();
				CheckModifiedFile (readydb.Value);
			}
		}
		
		static void ConsumeParsingQueue ()
		{
			int pending = 0;
			IProgressMonitor monitor = null;
			
			try {
				do {
					if (pending > 5 && monitor == null) {
						monitor = GetParseProgressMonitor ();
						monitor.BeginTask (GettextCatalog.GetString ("Generating database"), 0);
					}
					var job = DequeueParseJob ();
					if (job != null) {
						try {
							job.Run (monitor);
						} catch (Exception ex) {
							if (monitor == null)
								monitor = GetParseProgressMonitor ();
							monitor.ReportError (null, ex);
						}
					}
					
					pending = PendingJobCount;
					
				} while (pending > 0);
				queueEmptied.Set ();
			} finally {
				if (monitor != null)
					monitor.Dispose ();
			}
		}
		#endregion
	}
	
	public sealed class ProjectUnloadEventArgs : EventArgs
	{
		public readonly Project Project;
		public readonly TypeSystemService.ProjectContentWrapper Wrapper;

		public ProjectUnloadEventArgs (Project project, TypeSystemService.ProjectContentWrapper wrapper)
		{
			this.Project = project;
			this.Wrapper = wrapper;
		}
	}
}

