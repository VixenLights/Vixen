using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Morph
{
	/// <summary>
	/// Provides descriptor data describing the morph effect.
	/// </summary>
	public class Descriptor : EffectModuleDescriptorBase
	{
		#region Private Static Fields

		private static readonly Guid _typeId = new Guid("461FB199-437F-4EBA-9C54-6AB126342379");

		#endregion 

		#region Public Properties

		public override ParameterSignature Parameters
		{
			get { return new ParameterSignature(); }
		}

		public override EffectGroups EffectGroup
		{
			get { return EffectGroups.Pixel; }
		}

		public override bool SupportsMedia
		{
			get { return true; }
		}

		public override bool SupportsMarks => false;

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
			get { return typeof(Morph); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(MorphData); }
		}

		public override string Author
		{
			get { return "John Baur"; }
		}

		public override string Description
		{
			get { return "Applies a Morph effect to pixel elements"; } 
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Morph"; }
		}

		#endregion
	}
}
