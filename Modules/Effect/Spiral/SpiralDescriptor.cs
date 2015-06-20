using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Spiral
{
	public class SpiralDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("3f629e8c-cd8d-467e-afab-1a3836b24342");

		public override ParameterSignature Parameters
		{
			get { return new ParameterSignature(); }
		}

		public override EffectGroups EffectGroup
		{
			get { return EffectGroups.Basic; }
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
			get { return typeof(Spiral); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(SpiralData); }
		}

		public override string Author
		{
			get { return "Jeff Uchitjil"; }
		}

		public override string Description
		{
			get { return "Applies a Spiral like effect to pixel elments"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Spiral"; }
		}
	}
}
