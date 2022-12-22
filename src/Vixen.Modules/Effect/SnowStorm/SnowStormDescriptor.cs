﻿using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.SnowStorm
{
	public class SnowStormDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("93d60483-fdba-486f-8bc5-4e139f599b4b");

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
			get { return typeof(SnowStorm); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(SnowStormData); }
		}

		public override string Author
		{
			get { return "Geoff Armstrong"; }
		}

		public override string Description
		{
			get { return "Applies a SnowStorm like effect to pixel elments"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Snowstorm"; }
		}
	}
}
