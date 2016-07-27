using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Circles
{
	public class CirclesDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("ca94f43e-2c48-410e-b3ba-678d705f44ca");

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
			get { return typeof(Circles); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(CirclesData); }
		}

		public override string Author
		{
			get { return "Geoff Armstrong"; }
		}

		public override string Description
		{
			get { return "Applies Circle like effects to pixel elments"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Circles"; }
		}
	}
}
