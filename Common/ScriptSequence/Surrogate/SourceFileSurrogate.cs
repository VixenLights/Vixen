using System.Runtime.Serialization;
using Common.ScriptSequence.Script;

namespace Common.ScriptSequence.Surrogate
{
	[DataContract]
	internal class SourceFileSurrogate
	{
		public SourceFileSurrogate(SourceFile sourceFile)
		{
			Name = sourceFile.Name;
		}

		[DataMember]
		public string Name { get; private set; }
	}
}