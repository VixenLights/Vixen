using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Shockwave
{
	public class ShockwaveDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("880280EA-C38B-442C-8F09-395DB4EB698C");

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
			get { return typeof(Shockwave); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(ShockwaveData); }
		}

		public override string Author
		{
			get { return "Jeff Uchitjil"; }
		}

		public override string Description
		{
			get { return "Applies a Shockwave type effect to pixel elements"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Shockwave"; }
		}
	}
}
