namespace VixenModules.Output.DmxUsbPro
{
	using System;
	using Vixen.Module.Controller;

	public class Descriptor : ControllerModuleDescriptorBase
	{
		private readonly Guid _typeId = new Guid("8ABF27A7-3156-4F4C-9D43-ADA45CDD6D0C");

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string Description
		{
			get { return "DMX USB Pro hardware controller module"; }
		}

		public override Type ModuleClass
		{
			get { return typeof (Module); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof (Data); }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override string TypeName
		{
			get { return "DMX Pro"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}
	}
}