using System;
using System.Collections.Generic;
using Vixen.Module.Controller;

namespace VixenModules.Output.HelixController
{
	public class HelixDescriptor : ControllerModuleDescriptorBase
	{
		/// <summary>
		/// Unique Identifier for the controller
		/// </summary>
		private Guid _typeId = new Guid("{D409C5D6-725B-45F4-827B-AAEE49E2FFB6}");
	
		/// <summary>
		/// Put your name here if you wrote the controller
		/// </summary>
		public override string Author
		{
			get { return "Ed Brady"; }
		}

		/// <summary>
		/// put in a brief description of the controller module
		/// </summary>
		public override string Description
		{
			get { return "Helix File Export"; }
		}

		/// <summary>
		/// This returns the module class from your project
		/// </summary>
		public override Type ModuleClass
		{
			get { return typeof(HelixModule); }
		}

		/// <summary>
		/// This returns the data class for your project
		/// </summary>
		public override Type ModuleDataClass
		{
			get { return typeof(HelixData); }
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
			get { return "Helix File Export"; }
		}

		/// <summary>
		/// This is the version of the controller
		/// </summary>
		public override string Version
		{
			get { return "1.1"; }
		}
	}
}
