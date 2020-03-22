using System;
using System.Collections.Generic;
using Vixen.Module.Controller;

namespace VixenModules.Output.ElexolUSBIO
{
	public class ElexolUSBDescriptor : ControllerModuleDescriptorBase
	{
		/// <summary>
		/// Unique Identifier for the controller
		/// </summary>
		private Guid _typeId = new Guid("{6F281DCD-F047-4dd2-B57A-097E6AF1F7A8}");

		/// <summary>
		/// Put your name here if you wrote the controller
		/// </summary>
		public override string Author
		{
			get { return "Tony Eberle"; }
		}

		/// <summary>
		/// put in a brief description of the controller module
		/// </summary>
		public override string Description
		{
			get { return "Elexol USB I/O 24"; }
		}

		/// <summary>
		/// This returns the module class from your project
		/// </summary>
		public override Type ModuleClass
		{
			get { return typeof(ElexolUSBModule); }
		}

		/// <summary>
		/// This returns the data class for your project
		/// </summary>
		public override Type ModuleDataClass
		{
			get { return typeof(ElexolUSBData); }
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
            get { return "Elexol USB I/O 24"; }
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