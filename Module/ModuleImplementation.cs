using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;

namespace Vixen.Module {
	abstract class ModuleImplementation {
		protected ModuleImplementation(Type moduleInstanceType) {
			// Get the name from the ModuleTypeAttribute of the superclass.
			this.ModuleTypeName = (this.GetType().GetCustomAttributes(typeof(ModuleTypeAttribute), true).First() as ModuleTypeAttribute).Name;
			ModuleInstanceType = moduleInstanceType;
		}

		public string ModuleTypeName { get; private set; } // i.e. "Output"
		public Type ModuleInstanceType { get; private set; } // i.e. "IOutputModuleInstance"
		public IModuleLoadNotification LoadNotifier { get; protected set; }

		abstract public IModuleRepository Repository { get; protected set; }
		abstract public IModuleManagement Management { get; protected set; }
	}

	abstract class ModuleImplementation<T> : ModuleImplementation
		where T : class {
		protected ModuleImplementation(IModuleLoadNotification moduleLoadNotifier, IModuleManagement<T> moduleManagement, IModuleRepository<T> moduleRepository)
			: base(typeof(T)) {
			this.LoadNotifier = moduleLoadNotifier;
			this.Management = moduleManagement;
			this.Repository = moduleRepository;
		}

		override public IModuleRepository Repository { get; protected set; }
		override public IModuleManagement Management { get; protected set; }
	}
}
