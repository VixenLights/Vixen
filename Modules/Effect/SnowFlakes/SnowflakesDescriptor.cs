using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Snowflakes
{
	public class SnowflakesDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("c3353b3b-4e04-415e-86fa-1fe1641c4c31");

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
			get { return typeof(Snowflakes); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(SnowflakesData); }
		}

		public override string Author
		{
			get { return "Jeff Uchitjil"; }
		}

		public override string Description
		{
			get { return "Applies a Snowflakes effect to pixel elements"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Snowflakes"; }
		}
	}
}
