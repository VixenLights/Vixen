using System.Runtime.Serialization;
using Vixen.Execution;

namespace Vixen.Module.SequenceType.Surrogate
{
	[DataContract(Namespace = "")]
	internal class SelectedTimingProviderSurrogate
	{
		public SelectedTimingProviderSurrogate(SelectedTimingProvider selectedTimingProvider)
		{
			if (selectedTimingProvider != null) {
				ProviderType = selectedTimingProvider.ProviderType;
				SourceName = selectedTimingProvider.SourceName;
			}
		}

		[DataMember]
		public string ProviderType { get; private set; }

		[DataMember]
		public string SourceName { get; private set; }

		public SelectedTimingProvider CreateSelectedTimingProvider()
		{
			return new SelectedTimingProvider(ProviderType, SourceName);
		}
	}
}