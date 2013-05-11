using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Common.ScriptSequence.Script;
using Common.ScriptSequence.Surrogate;
using Vixen.Module.SequenceType;

namespace Common.ScriptSequence {
	[DataContract]
	public class ScriptSequenceData : SequenceTypeDataModelBase {
		public ScriptSequenceData() {
			SourceFiles = new SourceFileCollection();
			FrameworkAssemblies = new HashSet<string>();
			ExternalAssemblies = new HashSet<string>();
		}

		[DataMember]
		public Guid ScriptLanguageModuleId { get; set; }

		[DataMember]
		public string ClassName { get; set; }

		[DataMember]
		public string SourceFileDirectory { get; set; }

		[DataMember]
		private SourceFileCollectionSurrogate _sourceFileCollectionSurrogate;
		public SourceFileCollection SourceFiles { get; private set; }

		[DataMember]
		public HashSet<string> FrameworkAssemblies { get; private set; }

		[DataMember]
		public HashSet<string> ExternalAssemblies { get; private set; }

		private void _WriteSourceFileContent() {
			foreach(SourceFile sourceFile in SourceFiles) {
				string filePath = _GetSourceFilePath(sourceFile);
				_EnsureSourceFileDirectoryExists(filePath);
				File.WriteAllText(filePath, sourceFile.Contents);
			}
		}

		private void _EnsureSourceFileDirectoryExists(string filePath) {
			string directoryName = Path.GetDirectoryName(filePath);
			if(!Directory.Exists(directoryName)) {
				Directory.CreateDirectory(directoryName);
			}
		}

		private void _ReadSourceFileContent() {
			foreach(SourceFile sourceFile in SourceFiles) {
				string filePath = _GetSourceFilePath(sourceFile);
				sourceFile.Contents = File.ReadAllText(filePath);
			}
		}

		private string _GetSourceFilePath(SourceFile sourceFile) {
			return Path.Combine(SourceFileDirectory, sourceFile.Name);
		}

		[OnSerializing]
		void SurrogateWrite(StreamingContext context) {
			_sourceFileCollectionSurrogate = new SourceFileCollectionSurrogate(SourceFiles);
			_WriteSourceFileContent();
		}

		[OnDeserialized]
		void SurrogateRead(StreamingContext context) {
			SourceFiles = new SourceFileCollection();
			SourceFiles.AddRange(_sourceFileCollectionSurrogate.SourceFiles.Select(x => new SourceFile(x.Name)));
			_ReadSourceFileContent();
		}
	}
}
