using System;
using Vixen.Module.Effect;
using Vixen.Sys;
using Vixen.Sys.Attribute;

namespace VixenModules.Effect.Glediator
{
	public class GlediatorDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("7182b3b8-7998-4812-8ab2-ba99ab09cccd");

		public GlediatorDescriptor()
		{
			ModulePath = EffectName;
		}

		[ModuleDataPath]
		public static string ModulePath { get; set; }

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
			get { return typeof(Glediator); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(GlediatorData); }
		}

		public override string Author
		{
			get { return "Geoff Armstrong"; }
		}

		public override string Description
		{
			get { return "Applies an effect based on a Glediator file to pixel elments"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Glediator"; }
		}
	}
}
