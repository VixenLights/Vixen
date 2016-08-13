using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.PinWheel
{
	public class PinWheelDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("089da093-a743-4eac-b962-7f29e4ba337d");

		public override ParameterSignature Parameters
		{
			get { return new ParameterSignature(); }
		}

		public override EffectGroups EffectGroup
		{
			get { return EffectGroups.Pixel; }
		}

		public override string TypeName
		{
			get { return EffectName; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof(PinWheel); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(PinWheelData); }
		}

		public override string Author
		{
			get { return "Geoff Armstrong"; }
		}

		public override string Description
		{
			get { return "Applies a PinWheel like effect to pixel elments"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Pinwheel"; }
		}
	}
}
