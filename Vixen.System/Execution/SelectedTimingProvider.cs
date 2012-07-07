using System.Runtime.Serialization;

namespace Vixen.Execution {
	public class SelectedTimingProvider {
		public SelectedTimingProvider(string providerType, string sourceName) {
			ProviderType = providerType;
			SourceName = sourceName;
		}

		public string ProviderType { get; private set; }

		public string SourceName { get; private set; }

		static public bool IsValid(SelectedTimingProvider selectedTimingProvider) {
			return
				selectedTimingProvider != null &&
				!string.IsNullOrEmpty(selectedTimingProvider.ProviderType) &&
				!string.IsNullOrEmpty(selectedTimingProvider.SourceName);
		}
	}
}
