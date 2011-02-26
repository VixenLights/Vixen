using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using CommandStandard;
using Vixen.Sys;

namespace Vixen.Module.CommandSpec {
	class CommandSpecModuleManagement : IModuleManagement<ICommandSpecModuleInstance> {
		public ICommandSpecModuleInstance Get(string commandName) {
			// Need the type-specific repository.
			CommandSpecModuleRepository repository = Server.Internal.GetModuleRepository<ICommandSpecModuleInstance, CommandSpecModuleRepository>();
			return repository.Get(commandName);
		}

		public ICommandSpecModuleInstance Get(Guid id) {
			return Server.ModuleRepository.GetCommandSpec(id);
		}

		public ICommandSpecModuleInstance[] GetAll() {
			return Server.ModuleRepository.GetAllCommandSpec();
		}

		public ICommandSpecModuleInstance Clone(ICommandSpecModuleInstance instance) {
			// These are singletons.
			return null;
		}

		object IModuleManagement.Get(Guid id) {
			return Get(id);
		}

		object[] IModuleManagement.GetAll() {
			return GetAll();
		}

		public object Clone(object instance) {
			return Clone(instance as ICommandSpecModuleInstance);
		}
	}
}
