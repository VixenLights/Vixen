using System;
using Vixen.Module.Input;

namespace VixenModules.Input.DirectXJoystick
{
	public class DirectXJoystickDescriptor : InputModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{2E15154E-2EB4-4aa6-9F70-E432256BBAF2}");

		public override string TypeName
		{
			get { return "DirectX joystick"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (DirectXJoystickModule); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof (DirectXJoystickData); }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string Description
		{
			get { return "A single joystick, as reported by DirectX"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}
	}
}