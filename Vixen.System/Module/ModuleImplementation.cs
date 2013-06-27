using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys.Attribute;

namespace Vixen.Module
{
	internal abstract class ModuleImplementation
	{
		protected ModuleImplementation(Type moduleInstanceType)
		{
			// Get the name from the ModuleTypeAttribute of the superclass.
			TypeOfModuleAttribute typeOfModule =
				(this.GetType().GetCustomAttributes(typeof (TypeOfModuleAttribute), true).FirstOrDefault() as TypeOfModuleAttribute);
			if (typeOfModule == null)
				throw new InvalidOperationException(string.Format("Type {0} is not a valid module type.", moduleInstanceType));
			TypeOfModule = typeOfModule.Name;
			ModuleInstanceType = moduleInstanceType;
		}

		public string TypeOfModule { get; private set; } // i.e. "Controller"
		public Type ModuleInstanceType { get; private set; } // i.e. "IControllerModuleInstance"
		public string Path { get; set; } // Path that holds modules of this type.

		public abstract IModuleRepository Repository { get; protected set; }
		public abstract IModuleManagement Management { get; protected set; }
	}

	internal abstract class ModuleImplementation<T> : ModuleImplementation
		where T : class, IModuleInstance
	{
		protected ModuleImplementation(IModuleManagement<T> moduleManagement, IModuleRepository<T> moduleRepository)
			: base(typeof (T))
		{
			this.Management = moduleManagement;
			this.Repository = moduleRepository;
		}

		public override IModuleRepository Repository { get; protected set; }
		public override IModuleManagement Management { get; protected set; }
	}
}