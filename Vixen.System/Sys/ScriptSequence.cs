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

		#region _WriteXml
		protected override XElement _WriteXml() {
			return new XElement("Script",
				_WriteLanguage(),
				_WriteSourceFiles(),
				_WriteFrameworkAssemblies(),
				_WriteExternalAssemblies());
		}

		private XElement _WriteLanguage() {
			return new XElement("Language", Language);
		}

		private XElement _WriteSourceFiles() {
			// Make sure source directory exists.
			string sourcePath = Path.Combine(SourceDirectory, Name);
			Helper.EnsureDirectory(sourcePath);

			// Write the source files and their references.
			return new XElement("SourceFiles", SourceFiles.Select(x => {
				x.Save(sourcePath);
				return new XElement("SourceFile",
					new XAttribute("name", x.Name));
			}));
		}

		private XElement _WriteFrameworkAssemblies() {
			return new XElement("FrameworkAssemblies", FrameworkAssemblies.Select(x =>
				new XElement("Assembly",
					new XAttribute("name", x))));
		}

		private XElement _WriteExternalAssemblies() {
			return new XElement("ExternalAssemblies", ExternalAssemblies.Select(x =>
				new XElement("Assembly",
					new XAttribute("name", x))));
		}

		#endregion

		#region _ReadXml
		protected override void _ReadXml(XElement element) {
			element = element.Element("Script");
			_ReadLanguage(element);
			_ReadSourceFiles(element);
			_ReadFrameworkAssemblies(element);
			_ReadExternalAssemblies(element);
		}

		private void _ReadLanguage(XElement element) {
			Language = element.Element("Language").Value;
		}

		private void _ReadSourceFiles(XElement element) {
            string sourcePath = Path.Combine(SourceDirectory, Name);
			SourceFiles.Clear();
			IEnumerable<string> fileNames = element
				.Element("SourceFiles")
				.Elements("SourceFile")
				.Select(x => x.Attribute("name").Value);
			foreach(string fileName in fileNames) {
				string filePath = Path.Combine(sourcePath, fileName);
				SourceFiles.Add(SourceFile.Load(filePath));
			}
		}

		private void _ReadFrameworkAssemblies(XElement element) {
			FrameworkAssemblies = new HashSet<string>(
				element
					.Element("FrameworkAssemblies")
					.Elements("Assembly")
					.Select(x => x.Attribute("name").Value));
		}

		private void _ReadExternalAssemblies(XElement element) {
			ExternalAssemblies = new HashSet<string>(
				element
					.Element("ExternalAssemblies")
					.Elements("Assembly")
					.Select(x => x.Attribute("name").Value));
		}
		#endregion
	}
}
