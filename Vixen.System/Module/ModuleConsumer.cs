using System;
using Vixen.Sys;

namespace Vixen.Module {
	class ModuleConsumer<T> : IModuleConsumer<T>
		where T : class, IModuleInstance {
		private IModuleDataRetriever _moduleDataRetriever;
		private IModuleInstance _module;

		public ModuleConsumer(Guid moduleId, IModuleDataRetriever moduleDataRetriever) {
			if(moduleDataRetriever == null) throw new ArgumentNullException("moduleDataRetriever");

			ModuleId = moduleId;
			_moduleDataRetriever = moduleDataRetriever;
		}

		public Guid ModuleId { get; private set; }

		virtual public T Module {
			get {
				if(_module == null) {
					IModuleManagement moduleTypeManager = Modules.GetManager<T>();
					_module = (T)moduleTypeManager.Get(ModuleId);
					if(_module != null) {
						_moduleDataRetriever.AssignModuleData(_module);
					}
				}
				return (T)_module;
			}
		}
	}
}
