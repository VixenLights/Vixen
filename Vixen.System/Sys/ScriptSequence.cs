using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vixen.Sys;
using Vixen.IO;
using Vixen.Script;
using System.IO;
using Vixen.Execution;
using Vixen.Module.Script;
using Vixen.IO.Xml;

namespace Vixen.Sys {
	/// <summary>
	/// Base class for script sequence type module implementations.
	/// </summary>
	[Executor(typeof(ScriptSequenceExecutor))]
	[SequenceReader(typeof(XmlScriptSequenceReader))]
	abstract public class ScriptSequence : Sequence {
		private string _language;

		private const string DIRECTORY_NAME = "Sequence";
		private const string SOURCE_DIRECTORY_NAME = "ScriptSource";
		private const int VERSION = 1;

		[DataPath]
		static private readonly string _sourceDirectory = Path.Combine(Paths.DataRootPath, SOURCE_DIRECTORY_NAME);

		protected ScriptSequence(string language) {
			Length = Forever;

			SourceFiles = new List<SourceFile>();
			FrameworkAssemblies = new HashSet<string>();
			ExternalAssemblies = new HashSet<string>();

			// Required assembly references.
			ExternalAssemblies.Add(VixenSystem.AssemblyFileName);
			FrameworkAssemblies.Add("System.dll");
			FrameworkAssemblies.Add("System.Core.dll");

			Language = language;
		}

		protected ScriptSequence(string language, ScriptSequence original) {
			Length = Forever;

			SourceFiles = new List<SourceFile>(original.SourceFiles);
			FrameworkAssemblies = new HashSet<string>(original.FrameworkAssemblies);
			ExternalAssemblies = new HashSet<string>(original.ExternalAssemblies);

			Language = language;

			ClassName = original.ClassName;
		}

		public string SourceDirectory {
			get { return _sourceDirectory; }
		}

		public List<SourceFile> SourceFiles { get; private set; }

		public string Language {
			get { return _language; }
			set {
				SourceFiles.Clear();
				ScriptModuleManagement manager = Modules.GetManager<IScriptModuleInstance, ScriptModuleManagement>();
				if(!manager.GetLanguages().Any(x => string.Equals(x, value, StringComparison.OrdinalIgnoreCase))) {
					throw new Exception("There is no script type " + value);
				}

				_language = value;
			}
		}

		public string ClassName { get; set; }

		public HashSet<string> FrameworkAssemblies { get; set; }

		public HashSet<string> ExternalAssemblies { get; set; }

		private bool _FileExists(string fileName) {
			fileName = Path.GetFileNameWithoutExtension(fileName);
			return SourceFiles.Any(x => string.Equals(x.Name, fileName, StringComparison.OrdinalIgnoreCase));
		}

		public SourceFile CreateNewFile(string fileName) {
			if(string.IsNullOrWhiteSpace(FilePath)) throw new Exception("Sequence FilePath must be set.");
			if(string.IsNullOrWhiteSpace(Language)) throw new Exception("Sequence Language must be set.");
			if(_FileExists(fileName)) throw new InvalidOperationException("File already exists with that name.");

			// Get the appropriate extension onto the file name.
			ScriptModuleManagement manager = Modules.GetManager<IScriptModuleInstance, ScriptModuleManagement>();
			fileName = Path.ChangeExtension(fileName, manager.GetFileExtension(Language));

			SourceFile sourceFile = null;

			if(SourceFiles.Count == 0) {
				sourceFile = _CreateSkeletonFile(fileName);
			} else {
				sourceFile = _CreateBlankFile(fileName);
			}

			return sourceFile;
		}

		private SourceFile _CreateSkeletonFile(string fileName) {
			ScriptModuleManagement manager = Modules.GetManager<IScriptModuleInstance, ScriptModuleManagement>();
			IScriptSkeletonGenerator skeletonFileGenerator = manager.GetSkeletonGenerator(Language);
			
			// Setting the class name any time the skeleton file is generated so that when
			// the framework is generated, it will be part of the same class.
			string nameSpace = ScriptHostGenerator.UserScriptNamespace;
			ClassName = ScriptHostGenerator.Mangle(Name);

			SourceFile sourceFile = _CreateBlankFile(fileName);
			sourceFile.Contents = skeletonFileGenerator.Generate(nameSpace, ClassName);

			return sourceFile;
		}

		private SourceFile _CreateBlankFile(string fileName) {
			SourceFile sourceFile = new SourceFile(Path.GetFileName(fileName));
			SourceFiles.Add(sourceFile);
			return sourceFile;
		}

		override protected IWriter _GetSequenceWriter() {
			return new XmlScriptSequenceWriter();
		}

		public override int Version {
			get { return VERSION; }
		}
	}
}
