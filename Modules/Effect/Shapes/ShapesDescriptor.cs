using System;
using Vixen.Module.Effect;
using Vixen.Sys;
using Vixen.Sys.Attribute;

namespace VixenModules.Effect.Shapes
{
	public class ShapesDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("e3c7ba07-83f9-4784-bbd0-94449e169e21");

		public override ParameterSignature Parameters
		{
			get { return new ParameterSignature(); }
		}

		public ShapesDescriptor()
		{
			ModulePath = EffectName;
		}

		[ModuleDataPath]
		public static string ModulePath { get; set; }

		public override EffectGroups EffectGroup
		{
			get { return EffectGroups.Pixel; }
		}

		#region Overrides of EffectModuleDescriptorBase

		/// <inheritdoc />
		public override bool SupportsMarks => true;

		/// <inheritdoc />
		public override bool SupportsFiles => true;

		//Used when dragging files from Windows Explorer and will grab the appropiate file extensions to check.
		public override string[] SupportedFileExtensions => new[] { ".svg" };

		#endregion

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
			get { return typeof(Shapes); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(ShapesData); }
		}

		public override string Author
		{
			get { return "Geoff Armstrong"; }
		}

		public override string Description
		{
			get { return "Applies a Shape effect to pixel elments"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Shapes"; }
		}
	}
}
