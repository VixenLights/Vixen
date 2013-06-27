using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BaseSequence;
using Common.ScriptSequence.Script;
using Vixen.Module.Script;
using Vixen.Sys;

namespace Common.ScriptSequence
{
	public abstract class ScriptSequence : Sequence
	{
		private IScriptModuleInstance _language;

		public override ISequenceTypeDataModel SequenceData
		{
			get { return base.SequenceData; }
			set
			{
				base.SequenceData = value;
				Length = Forever;
			}
		}

		public void AddSourceFile(SourceFile sourceFile)
		{
			_ScriptSequenceData.SourceFiles.Add(sourceFile);
		}

		public void AddSourceFiles(IEnumerable<SourceFile> sourceFiles)
		{
			foreach (SourceFile sourceFile in sourceFiles) {
				AddSourceFile(sourceFile);
			}
		}

		public bool RemoveSourceFile(SourceFile sourceFile)
		{
			return _ScriptSequenceData.SourceFiles.Remove(sourceFile);
		}

		public IEnumerable<SourceFile> SourceFiles
		{
			get { return _ScriptSequenceData.SourceFiles; }
		}

		public void ClearSourceFiles()
		{
			_ScriptSequenceData.SourceFiles.Clear();
		}

		public string SourceFileDirectory
		{
			get { return _ScriptSequenceData.SourceFileDirectory; }
			set { _ScriptSequenceData.SourceFileDirectory = value; }
		}

		public IScriptModuleInstance Language
		{
			get
			{
				return _language ??
				       (_language =
				        Vixen.Services.ApplicationServices.Get<IScriptModuleInstance>(_ScriptSequenceData.ScriptLanguageModuleId));
			}
			set
			{
				_language = value;
				_ScriptSequenceData.ScriptLanguageModuleId = value.Descriptor.TypeId;
			}
		}

		public string ClassName
		{
			get { return _ScriptSequenceData.ClassName; }
			set { _ScriptSequenceData.ClassName = value; }
		}

		public HashSet<string> FrameworkAssemblies
		{
			get { return _ScriptSequenceData.FrameworkAssemblies; }
		}

		public HashSet<string> ExternalAssemblies
		{
			get { return _ScriptSequenceData.ExternalAssemblies; }
		}

		public SourceFile CreateNewFile(string fileName)
		{
			if (string.IsNullOrWhiteSpace(Name))
				throw new Exception("Sequence name must be set before the initial file can be created.");
			if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException("fileName");
			if (_FileExists(fileName)) throw new InvalidOperationException("File already exists with that name.");

			if (Language == null) return null;
			fileName = Path.ChangeExtension(fileName, Language.FileExtension);

			SourceFile sourceFile = (_ScriptSequenceData.SourceFiles.Count == 0)
			                        	? _CreateSkeletonFile(fileName)
			                        	: _CreateBlankFile(fileName);

			return sourceFile;
		}

		private ScriptSequenceData _ScriptSequenceData
		{
			get { return (ScriptSequenceData) SequenceData; }
		}

		private IScriptModuleDescriptor _LanguageModuleDescriptor
		{
			get
			{
				if (Language == null) return null;
				return (IScriptModuleDescriptor) Language.Descriptor;
			}
		}

		private bool _FileExists(string fileName)
		{
			fileName = Path.GetFileNameWithoutExtension(fileName);
			return _ScriptSequenceData.SourceFiles.Any(x => string.Equals(x.Name, fileName, StringComparison.OrdinalIgnoreCase));
		}

		private SourceFile _CreateSkeletonFile(string fileName)
		{
			// Setting the class name any time the skeleton file is generated so that when
			// the framework is generated, it will be part of the same class.
			string nameSpace = ScriptHostGenerator.UserScriptNamespace;
			ClassName = ScriptHostGenerator.Mangle(Name);

			SourceFile sourceFile = _CreateBlankFile(fileName);
			sourceFile.Contents = Language.SkeletonGenerator.Generate(nameSpace, ClassName);

			return sourceFile;
		}

		private SourceFile _CreateBlankFile(string fileName)
		{
			SourceFile sourceFile = new SourceFile(Path.GetFileName(fileName));
			AddSourceFile(sourceFile);
			return sourceFile;
		}
	}
}