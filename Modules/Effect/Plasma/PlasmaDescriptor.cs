using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Plasma
{
	public class PlasmaDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("a4b7255d-0011-481f-8065-a7e5f14b39f9");

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
			get { return typeof(Plasma); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(PlasmaData); }
		}

		public override string Author
		{
			get { return "Geoff Armstrong"; }
		}

		public override string Description
		{
			get { return "Applies a Plasma like effect to pixel elments"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Plasma"; }
		}
	}
}
