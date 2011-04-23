using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Timing;

namespace Vixen.Execution {
	class TimingModuleTimingProvider : ITimingProvider {
		public string TimingProviderTypeName {
			get { return "Timing module"; }
		}

		public string[] GetAvailableTimingSources(ISequence sequence) {
			return Modules.GetModuleDescriptors<ITimingModuleInstance>().Select(x => x.TypeName).ToArray();
		}

		public ITiming GetTimingSource(ISequence sequence, string sourceName) {
			IModuleDescriptor moduleDescriptor = Modules.GetModuleDescriptors<ITimingModuleInstance>().FirstOrDefault(x => x.TypeName == sourceName);
			if(moduleDescriptor != null) {
				return VixenSystem.ModuleManagement.GetTiming(moduleDescriptor.TypeId);
			}
			return null;
		}
	}
}
