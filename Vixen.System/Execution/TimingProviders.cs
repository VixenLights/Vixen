using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml.Linq;
using Vixen.Module.Timing;
using Vixen.Sys;

namespace Vixen.Execution {
	public class TimingProviders {
		static private Dictionary<string,ITimingProvider> _providers;
		private string _selectedProviderType;
		private string _selectedSourceName;

		static TimingProviders() {
			_providers = typeof(ITimingProvider)
				.FindImplementationsWithin(Assembly.GetExecutingAssembly())
				.Select(x => Activator.CreateInstance(x))
				.Cast<ITimingProvider>()
				.ToDictionary(x => x.TimingProviderTypeName, x => x);
		}

		public TimingProviders(ISequence owner) {
			Owner = owner;
		}

		public ISequence Owner { get; private set; }

		public void GetSelectedSource(out string providerType, out string sourceName) {
			providerType = _selectedProviderType;
			sourceName = _selectedSourceName;
		}

		public ITiming GetSelectedSource() {
			ITiming selectedSource = null;

			ITimingProvider provider;
			if(_selectedProviderType != null && _providers.TryGetValue(_selectedProviderType, out provider)) {
				selectedSource = provider.GetTimingSource(Owner, _selectedSourceName);
			}

			return selectedSource;
		}

		public void SetSelectedSource(string providerType, string sourceName) {
			if(providerType == null || sourceName == null) {
				// Reset to having no explicit timing source.
				_selectedProviderType = null;
				_selectedSourceName = null;
			} else {
				ITimingProvider provider;
				if(_providers.TryGetValue(providerType, out provider)) {
					if(provider.GetTimingSource(Owner, sourceName) != null) {
						_selectedProviderType = providerType;
						_selectedSourceName = sourceName;
						return;
					}
				}
				throw new InvalidOperationException("Timing source does not exist.");
			}
		}

		public string[] GetTimingProviderTypes() {
			return _providers.Keys.ToArray();
		}

		public string[] GetAvailableTimingSources(string providerType) {
			ITimingProvider provider;
	
			if(_providers.TryGetValue(providerType, out provider)) {
				return provider.GetAvailableTimingSources(Owner);
			}

			throw new InvalidOperationException("Invalid provider type.");
		}
	}
}
