using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Bars
{
	public class BarDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("24decea6-78ab-4f42-acee-79afe4ffc015");

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
			get { return typeof(Bars); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(BarsData); }
		}

		public override string Author
		{
			get { return "Jeff Uchitjil"; }
		}

		public override string Description
		{
			get { return "Applies a Bar like effect to pixel elments"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Bars"; }
		}
	}
}
