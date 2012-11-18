using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vixen.Module.Timing;
using Vixen.Sys;

namespace Vixen.Execution {
	public class TimingProviders {
		public TimingProviders(ISequence owner) {
			Owner = owner;
		}

		public ISequence Owner { get; private set; }

		public ITiming GetTimingSource(SelectedTimingProvider selectedProvider) {
			ITiming selectedSource = null;

			var providers = _GetProviders();

			ITimingProvider provider;
			if(SelectedTimingProvider.IsValid(selectedProvider) && providers.TryGetValue(selectedProvider.ProviderType, out provider)) {
				selectedSource = provider.GetTimingSource(Owner, selectedProvider.SourceName);
			}

			return selectedSource;
		}

		public string[] GetTimingProviderTypes() {
			var providers = _GetProviders();
			return providers.Keys.ToArray();
		}

		public string[] GetAvailableTimingSources(string providerType) {
			ITimingProvider provider;

			var providers = _GetProviders();
			if(providers.TryGetValue(providerType, out provider)) {
				return provider.GetAvailableTimingSources(Owner);
			}

			throw new InvalidOperationException("Invalid provider type.");
		}

		private Dictionary<string, ITimingProvider> _GetProviders() {
			return typeof(ITimingProvider)
				.FindImplementationsWithin(Assembly.GetExecutingAssembly())
				.Select(Activator.CreateInstance)
				.Cast<ITimingProvider>()
				.ToDictionary(x => x.TimingProviderTypeName);
		}
	}
}
