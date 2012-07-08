using System.Linq;
using System.Runtime.Serialization;
using ScriptSequence.Script;

namespace ScriptSequence.Surrogate {
	[DataContract]
	class SourceFileCollectionSurrogate {
		public SourceFileCollectionSurrogate(SourceFileCollection sourceFileCollection) {
			SourceFiles = sourceFileCollection.Select(x => new SourceFileSurrogate(x)).ToArray();
		}

		[DataMember]
		public SourceFileSurrogate[] SourceFiles { get; private set; }
	}
}
