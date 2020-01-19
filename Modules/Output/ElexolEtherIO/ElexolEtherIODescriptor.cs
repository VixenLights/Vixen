using System;
using System.Collections.Generic;
using Vixen.Module.Controller;

namespace VixenModules.Output.ElexolEtherIO
{
	public class ElexolEtherIODescriptor : ControllerModuleDescriptorBase
	{
		/// <summary>
		/// Unique Identifier for the controller
		/// </summary>
		private Guid _typeId = new Guid("{2565E617-89DB-4761-BE8D-4D5574CAFBA9}");

		public override string Author
		{
			get { return "Tony Eberle"; }
		}

		public override string Description
		{
			get { return "Elexol Ether I/O 24"; }
		}

		/// <summary>
		/// This returns the module class from your project
		/// </summary>
		public override Type ModuleClass
		{
			get { return typeof(ElexolEtherIOModule); }
		}

		/// <summary>
		/// This returns the data class for your project
		/// </summary>
		public override Type ModuleDataClass
		{
			get { return typeof(ElexolEtherIOData); }
		}

		/// <summary>
		/// This returns the GUID for the controller
		/// </summary>
		public override Guid TypeId
		{
			get { return _typeId; }
		}

		/// <summary>
		/// This returns the name of the controller module that will be displayed
		/// in the add hardware dialog list
		/// </summary>
		public override string TypeName
		{
			get { return "Elexol Ethernet I/O 24"; }
		}

		/// <summary>
		/// This is the version of the controller
		/// </summary>
		public override string Version
		{
			get { return "1.0"; }
		}
	}
}