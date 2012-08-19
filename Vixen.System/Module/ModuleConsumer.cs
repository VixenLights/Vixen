using System;
using Vixen.Sys;

namespace Vixen.Module {
	class ModuleConsumer : IModuleConsumer {
		private IModuleDataRetriever _moduleDataRetriever;
		private IModuleInstance _module;

		public ModuleConsumer(Guid moduleId, IModuleDataRetriever moduleDataRetriever) {
			if(moduleDataRetriever == null) throw new ArgumentNullException("moduleDataRetriever");

			ModuleId = moduleId;
			_moduleDataRetriever = moduleDataRetriever;
		}

		public Guid ModuleId { get; private set; }

		virtual public IModuleInstance Module {
			get {
				if(_module == null) {
					_module = Modules.GetById(ModuleId);
					if(_module != null) {
						_moduleDataRetriever.AssignModuleData(_module);
					}
				}
				return _module;
			}
		}
	}
}
