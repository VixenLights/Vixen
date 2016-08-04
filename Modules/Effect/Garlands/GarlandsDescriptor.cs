using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Garlands
{
	public class GarlandsDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("b81778a9-e50a-435f-8695-8c130cba8afc");

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
			get { return typeof(Garlands); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(GarlandsData); }
		}

		public override string Author
		{
			get { return "Geoff Armstrong"; }
		}

		public override string Description
		{
			get { return "Applies a Garland like effect to pixel elments"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Garlands"; }
		}
	}
}
