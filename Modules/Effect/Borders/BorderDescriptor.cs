using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Borders
{
	public class BorderDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("d477d61a-7b46-4b25-be12-be52211156aa");

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
			get { return typeof(Border); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(BorderData); }
		}

		public override string Author
		{
			get { return "Geoff Armstrong"; }
		}

		public override string Description
		{
			get { return "Applies a Border effect to pixel elements"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Border"; }
		}
	}
}
