using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Vixen.Common;
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
	abstract public class ScriptSequence : Sequence {
		private string _language;

		private const string DIRECTORY_NAME = "Sequence";
		private const string SOURCE_DIRECTORY_NAME = "ScriptSource";
		private const string NEW_FILE_ROOT = "NewFile";

		[DataPath]
		static private readonly string _sourceDirectory = Path.Combine(Paths.DataRootPath, SOURCE_DIRECTORY_NAME);

		new static public ScriptSequence Load(string filePath) {
			// Load the sequence.
			IReader reader = new XmlScriptSequenceReader();
			ScriptSequence instance = (ScriptSequence)reader.Read(filePath);
			return instance;
		}

		protected ScriptSequence(string language) {
			Length = Forever;

			SourceFiles = new List<SourceFile>();
			FrameworkAssemblies = new HashSet<string>();
			ExternalAssemblies = new HashSet<string>();

			// Required assembly references.
			ExternalAssemblies.Add(VixenSystem.AssemblyFileName);
			ExternalAssemblies.Add(CommandStandard.Standard.AssemblyFileName);
			FrameworkAssemblies.Add("System.dll");
			FrameworkAssemblies.Add("System.Core.dll");

			Language = language;
		}

		public string SourceDirectory {
			get { return _sourceDirectory; }
		}

		public List<SourceFile> SourceFiles { get; private set; }

		public string Language {
			get { return _language; }
			set {
				// Create the first source file for them.
				// It has the skeleton in which they will write code.
				SourceFiles.Clear();
				ScriptModuleManagement manager = Modules.GetModuleManager<IScriptModuleInstance, ScriptModuleManagement>();
				IScriptSkeletonGenerator skeletonFileGenerator = manager.GetSkeletonGenerator(value);
				if(skeletonFileGenerator == null) {
					throw new Exception("There is no script type " + value);
				}

				_language = value;

				string nameSpace = ScriptHostGenerator.UserScriptNamespace;
				string className = ScriptHostGenerator.Mangle(Name);

				SourceFile sourceFile = CreateNewFile(CreateNewFileName(manager.GetFileExtension(_language)));
				sourceFile.Contents = skeletonFileGenerator.Generate(nameSpace, className);
			}
		}

		public HashSet<string> FrameworkAssemblies { get; set; }

		public HashSet<string> ExternalAssemblies { get; set; }

		public SourceFile CreateNewFile(string fileName) {
			SourceFile sourceFile = new SourceFile(Path.GetFileName(fileName));
			SourceFiles.Add(sourceFile);
			return sourceFile;
		}

		protected string CreateNewFileName(string fileExtension) {
			int count = 1;
			string fileName;

			fileName = NEW_FILE_ROOT + count;
			while(SourceFiles.Any(x => x.Name == fileName)) {
				count++;
				fileName = NEW_FILE_ROOT + count;
			}

			return fileName + fileExtension;
		}

		override protected IWriter _GetSequenceWriter() {
			return new XmlScriptSequenceWriter();
		}
	}
}
