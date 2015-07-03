using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Butterfly
{
	public class SnowflakesDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("e2ef89d5-c3b3-47e7-a776-f31b4193fc76");

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
			get { return typeof(Butterfly); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(ButterflyData); }
		}

		public override string Author
		{
			get { return "Jeff Uchitjil"; }
		}

		public override string Description
		{
			get { return "Applies a Butterfly effect to pixel elements"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Butterfly"; }
		}
	}
}
