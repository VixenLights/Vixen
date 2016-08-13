using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.ColorWash
{
	public class ColorWashDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("9dc93585-fff6-4216-bc72-fbf2617954c5");

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
			get { return typeof(ColorWash); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(ColorWashData); }
		}

		public override string Author
		{
			get { return "Geoff Armstrong"; }
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
			get { return "Colorwash"; }
		}
	}
}
