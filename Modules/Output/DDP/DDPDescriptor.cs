using System;
using Vixen.Module.Controller;

namespace VixenModules.Output.DDP
{
	internal class DDPDescriptor : ControllerModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{110B5FE9-CD4D-4E73-BB5B-67552A1AD6D1}");

		public override string Author
		{
			get { return "Jon Chuchla"; }
		}

		public override string Description
		{
			get { return "DDP Output Controller Module"; }
		}

		public override Type ModuleClass
		{
			get { return typeof (DDP); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof (DDPData); }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override string TypeName
		{
			get { return "DDP"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

	}
}