using System;
using Vixen.Sys;

namespace Vixen.Module
{
	internal class ModuleConsumer<T> : IModuleConsumer<T>
		where T : class, IModuleInstance
	{
		private IModuleDataRetriever _moduleDataRetriever;
		private IModuleInstance _module;

		public ModuleConsumer(Guid moduleId, Guid moduleInstanceId, IModuleDataRetriever moduleDataRetriever)
		{
			if (moduleDataRetriever == null) throw new ArgumentNullException("moduleDataRetriever");

			ModuleId = moduleId;
			ModuleInstanceId = moduleInstanceId;
			_moduleDataRetriever = moduleDataRetriever;
		}

		public Guid ModuleId { get; private set; }

		public Guid ModuleInstanceId { get; private set; }

		public virtual T Module
		{
			get
			{
				if (_module == null) {
					IModuleManagement moduleTypeManager = Modules.GetManager<T>();
					_module = (T) moduleTypeManager.Get(ModuleId);
					_module.InstanceId = ModuleInstanceId;
					if (_module != null) {
						_moduleDataRetriever.AssignModuleData(_module);
					}
				}
				return (T) _module;
			}
		}
	}
}