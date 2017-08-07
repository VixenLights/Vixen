using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Balls
{
	public class BallDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("59ba6e70-dc4d-4108-b287-72adf0fd4f7c");

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
			get { return typeof(Balls); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(BallData); }
		}

		public override string Author
		{
			get { return "Geoff Armstrong"; }
		}

		public override string Description
		{
			get { return "Adds Balls to the pixel elments"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Balls"; }
		}
	}
}
