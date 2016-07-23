using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Curtain
{
	public class CurtainDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("71ce703a-daee-4873-bdf7-bd4d6614f7f5");

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
			get { return typeof(Curtain); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(CurtainData); }
		}

		public override string Author
		{
			get { return "Geoff Armstrong"; }
		}

		public override string Description
		{
			get { return "Applies a Curtain like effect to pixel elments"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Curtain"; }
		}
	}
}
