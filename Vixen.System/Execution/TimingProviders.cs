using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Vixen.Module.Timing;
using Vixen.Sys;

namespace Vixen.Execution {
	public class TimingProviders {
		//static private Dictionary<string,ITimingProvider> _providers;
		//private SelectedTimingProvider _selectedTimingProvider;

		//static TimingProviders() {
		//    _providers = typeof(ITimingProvider)
		//        .FindImplementationsWithin(Assembly.GetExecutingAssembly())
		//        .Select(Activator.CreateInstance)
		//        .Cast<ITimingProvider>()
		//        .ToDictionary(x => x.TimingProviderTypeName, x => x);
		//}

		public TimingProviders(ISequence owner) {
			Owner = owner;
		}

		//public TimingProviders(ISequence owner, TimingProviders original)
		//    : this(owner) {
		//        SelectedTimingProvider = original.SelectedTimingProvider;
		//}

		public ISequence Owner { get; private set; }

		//public SelectedTimingProvider SelectedTimingProvider {
		//    get { return _selectedTimingProvider; }
		//    set {
		//        if(!SelectedTimingProvider.IsValid(value)) {
		//            // Reset to having no explicit timing source.
		//            _selectedTimingProvider = null;
		//        } else {
		//            ITimingProvider provider;
		//            if(_providers.TryGetValue(value.ProviderType, out provider)) {
		//                if(provider.GetTimingSource(Owner, value.SourceName) != null) {
		//                    _selectedTimingProvider = value;
		//                    return;
		//                }
		//            }
		//            throw new InvalidOperationException("Timing source does not exist.");
		//        }
		//    }
		//}

		//public ITiming GetSelectedSource() {
		//    ITiming selectedSource = null;

		//    ITimingProvider provider;
		//    if(SelectedTimingProvider.IsValid(SelectedTimingProvider) && _providers.TryGetValue(SelectedTimingProvider.ProviderType, out provider)) {
		//        selectedSource = provider.GetTimingSource(Owner, SelectedTimingProvider.SourceName);
		//    }

		//    return selectedSource;
		//}

		public ITiming GetTimingSource(SelectedTimingProvider selectedProvider) {
			ITiming selectedSource = null;

			var providers = _GetProviders();

			ITimingProvider provider;
			if(SelectedTimingProvider.IsValid(selectedProvider) && providers.TryGetValue(selectedProvider.ProviderType, out provider)) {
				selectedSource = provider.GetTimingSource(Owner, selectedProvider.SourceName);
			}

			return selectedSource;
		}

		//public void SetSelectedSource(string providerType, string sourceName) {
		//    if(providerType == null || sourceName == null) {
		//        // Reset to having no explicit timing source.
		//        _selectedProviderType = null;
		//        _selectedSourceName = null;
		//    } else {
		//        ITimingProvider provider;
		//        if(_providers.TryGetValue(providerType, out provider)) {
		//            if(provider.GetTimingSource(Owner, sourceName) != null) {
		//                _selectedProviderType = providerType;
		//                _selectedSourceName = sourceName;
		//                return;
		//            }
		//        }
		//        throw new InvalidOperationException("Timing source does not exist.");
		//    }
		//}

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
