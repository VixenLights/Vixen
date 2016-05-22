using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Spirograph
{
	public class SpirographDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("14cdf70f-cc79-4daf-bb22-e6ed6a865ef5");

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
			get { return typeof(Spirograph); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(SpirographData); }
		}

		public override string Author
		{
			get { return "Geoff Armstrong"; }
		}

		public override string Description
		{
			get { return "Applies a Spirograph like effect to pixel elments"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Spirograph"; }
		}
	}
}
