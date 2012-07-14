using System.Linq;
using System.Runtime.Serialization;
using Common.ScriptSequence.Script;

namespace Common.ScriptSequence.Surrogate {
	[DataContract]
	class SourceFileCollectionSurrogate {
		public SourceFileCollectionSurrogate(SourceFileCollection sourceFileCollection) {
			SourceFiles = sourceFileCollection.Select(x => new SourceFileSurrogate(x)).ToArray();
		}

		[DataMember]
		public SourceFileSurrogate[] SourceFiles { get; private set; }
	}
}
