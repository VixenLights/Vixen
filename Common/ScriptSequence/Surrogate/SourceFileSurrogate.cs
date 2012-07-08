using System.Runtime.Serialization;
using ScriptSequence.Script;

namespace ScriptSequence.Surrogate {
	[DataContract]
	class SourceFileSurrogate {
		public SourceFileSurrogate(SourceFile sourceFile) {
			Name = sourceFile.Name;
		}

		[DataMember]
		public string Name { get; private set; }
	}
}
