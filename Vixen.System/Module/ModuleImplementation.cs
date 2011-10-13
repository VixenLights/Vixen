using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module {
	abstract class ModuleImplementation {
		protected ModuleImplementation(Type moduleInstanceType) {
			// Get the name from the ModuleTypeAttribute of the superclass.
			TypeOfModuleAttribute typeOfModule = (this.GetType().GetCustomAttributes(typeof(TypeOfModuleAttribute), true).FirstOrDefault() as TypeOfModuleAttribute);
			if(typeOfModule == null) throw new InvalidOperationException("Type " + moduleInstanceType + " is not a valid module type.");
			TypeOfModule = typeOfModule.Name;
			ModuleInstanceType = moduleInstanceType;
		}

		public string TypeOfModule { get; private set; } // i.e. "Output"
		public Type ModuleInstanceType { get; private set; } // i.e. "IOutputModuleInstance"
		public string Path { get; set; } // Path that holds modules of this type.

		abstract public IModuleRepository Repository { get; protected set; }
		abstract public IModuleManagement Management { get; protected set; }
	}

	abstract class ModuleImplementation<T> : ModuleImplementation
		where T : class, IModuleInstance {
		protected ModuleImplementation(IModuleManagement<T> moduleManagement, IModuleRepository<T> moduleRepository)
			: base(typeof(T)) {
			this.Management = moduleManagement;
			this.Repository = moduleRepository;
		}

		override public IModuleRepository Repository { get; protected set; }
		override public IModuleManagement Management { get; protected set; }
	}
}
