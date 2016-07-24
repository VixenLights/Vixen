using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Life
{
	public class LifeDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("9f1e3985-a490-4584-839c-2d0f42670811");

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
			get { return typeof(Life); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(LifeData); }
		}

		public override string Author
		{
			get { return "Geoff Armstrong"; }
		}

		public override string Description
		{
			get { return "Applies a Life like effect to pixel elments"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Life"; }
		}
	}
}
