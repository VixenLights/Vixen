using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Bars
{
	public class BarDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("E3317C40-E78B-4280-AA50-057ECB9B6738");

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
			get { return typeof(DMXFixture); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(DMXFixtureData); }
		}

		public override string Author
		{
			get { return "John Baur"; }
		}

		public override string Description
		{
			get { return "Test effect for DMX Fixtures"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "DMXFixture"; }
		}
	}
}
