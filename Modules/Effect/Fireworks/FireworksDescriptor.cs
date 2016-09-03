using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Fireworks
{
	public class FireworksDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("c2bae192-d884-454f-84e0-61b7d25b59d5");

		public override ParameterSignature Parameters
		{
			get { return new ParameterSignature(); }
		}

		public override bool SupportsMedia
		{
			get { return true; }
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
			get { return typeof(Fireworks); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(FireworksData); }
		}

		public override string Author
		{
			get { return "Jeff Uchitjil"; }
		}

		public override string Description
		{
			get { return "Applies a Fireworks like effect to pixel elments"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Fireworks"; }
		}
	}
}
