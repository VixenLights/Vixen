using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Vixen.Common;
using Vixen.IO;
using Vixen.Script;
using System.IO;
using Vixen.Execution;
using Vixen.Sys;

namespace Vixen.Module.Sequence {
	/// <summary>
	/// Base class for script sequence type module implementations.
	/// </summary>
	[Executor(typeof(ScriptSequenceExecutor))]
	//abstract public class ScriptSequenceBase : Sequence<ScriptSequenceReader, ScriptSequenceWriter, ScriptSequenceBase>, ISequenceModuleInstance {
    abstract public class ScriptSequenceBase : Sequence {
		private const string DIRECTORY_NAME = "Sequence";
        private const string SOURCE_DIRECTORY_NAME = "ScriptSource";
		private const string NEW_FILE_ROOT = "NewFile";

		// Has to be in the subclass because you can't perform the late-bound operations
		// on the generic base.
		[DataPath]
		static private readonly string _directory = Path.Combine(Paths.DataRootPath, DIRECTORY_NAME);

		[DataPath]
		static private readonly string _sourceDirectory = Path.Combine(Paths.DataRootPath, SOURCE_DIRECTORY_NAME);

        public ScriptSequenceBase() {
			Length = Forever;

            SourceFiles = new List<SourceFile>();
			FrameworkAssemblies = new HashSet<string>();
			ExternalAssemblies = new HashSet<string>();

			//// Create the first source file for them.
			//SourceFile sourceFile = CreateNewFile(CreateNewFileName());
			////UserScript userScript = new UserScript(this);
			//IUserScript userScript = Script.Registration.GetScriptImplementation(Language);
			//if(userScript != null) {
			//    userScript.Sequence = this;
			//} else {
			//    throw new Exception("There is no script type " + Language);
			//}
			//sourceFile.Contents = userScript.TransformText();

			// Required assembly references.
			ExternalAssemblies.Add(VixenSystem.AssemblyFileName);
			ExternalAssemblies.Add(CommandStandard.Standard.AssemblyFileName);
			FrameworkAssemblies.Add("System.dll");
			FrameworkAssemblies.Add("System.Core.dll");

			////*** TESTING ***
			////OutputChannel channel = new OutputChannel(true) { Name = "Channel 1" };
			//OutputChannel channel = new OutputChannel() { Name = "Channel 1" };
			//channel.Patch.Add(new Guid("{871b3155-29e3-4a9e-861b-d3cc7895bffc}"), 0);
			//Fixture fixture = new Fixture();
			//fixture.InsertChannel(channel);
			//this.InsertFixture(fixture);
		}

		override protected string Directory {
			get { return _directory; }
		}

        public string SourceDirectory {
            get { return _sourceDirectory; }
        }

        public List<SourceFile> SourceFiles { get; private set; }

		private string _language;
		public string Language {
			get { return _language; }
			set {
				// Create the first source file for them.
				// It has the skeleton in which they will write code.
				IScriptSkeletonGenerator skeletonFileGenerator = Script.Registration.GetScriptSkeletonGenerator(value);
				if(skeletonFileGenerator == null) {
					throw new Exception("There is no script type " + value);
				}
				skeletonFileGenerator.Sequence = this;

				_language = value;

				SourceFile sourceFile = CreateNewFile(CreateNewFileName(Script.Registration.GetScriptFileExtension(_language)));
				sourceFile.Contents = skeletonFileGenerator.TransformText();
			}
		}

		public HashSet<string> FrameworkAssemblies { get; private set; }

		public HashSet<string> ExternalAssemblies { get; private set; }

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



		abstract public Guid TypeId { get; }

		public Guid InstanceId { get; set; }

		virtual public void Dispose() { }

		public IModuleDataModel ModuleData { get; set; }

		public string TypeName { get; set; }

		abstract public string FileExtension { get; }
	}
}
