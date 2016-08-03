using System;
using Vixen.Module.Effect;
using Vixen.Sys;
using Vixen.Sys.Attribute;

namespace VixenModules.Effect.Video
{
	public class VideoDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("5f33435c-eb96-4321-ae56-07e8e14e9251");

		public VideoDescriptor()
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
			get { return typeof(Video); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(VideoData); }
		}

		public override string Author
		{
			get { return "Geoff Armstrong"; }
		}

		public override string Description
		{
			get { return "Applies an effect based on a picture to pixel elments"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Video"; }
		}
	}
}
