using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Text
{
	public class TextDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("087fe899-33e7-4d56-9e85-047139fbd3f8");

		public override ParameterSignature Parameters
		{
			get { return new ParameterSignature(); }
		}

		public override EffectGroups EffectGroup
		{
			get { return EffectGroups.Pixel; }
		}

		#region Overrides of EffectModuleDescriptorBase

		/// <inheritdoc />
		public override bool SupportsMarks => true;

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
			get { return typeof(Text); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(TextData); }
		}

		public override string Author
		{
			get { return "Jeff Uchitjil"; }
		}

		public override string Description
		{
			get { return "Applies a Text effect to pixel elments"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Text"; }
		}
	}
}
