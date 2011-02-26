using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Common;

namespace Vixen.Module.Output {
	class OutputModuleManagement : UnusedModuleManagement<IOutputModuleInstance> {
		public Guid[] GetAllTimingSources() {
			return Modules.GetModuleDescriptors<IOutputModuleInstance>().Where(x => x.ModuleClass.GetInterface(typeof(ITimingSourceFactory).Name) != null).Select(x => x.TypeId).ToArray();
		}

		public ITimingSource GetTimingSource(Guid moduleId) {
			ITimingSource timingSource = null;
			IEnumerable<IOutputModuleInstance> outputModules = GetAll();
			// Get the referenced module as a timing factory.
			// If it's referenced as a timing factory, this should be a valid cast.
			ITimingSourceFactory timingSourceFactory = GetAll().FirstOrDefault(x => x.TypeId == moduleId) as ITimingSourceFactory;
			if(timingSourceFactory != null) {
				// Get a timing source instance.
				timingSource = timingSourceFactory.CreateTimingSource();
			}
			return timingSource;
		}
	}
}
