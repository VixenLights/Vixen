using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Wave
{
	/// <summary>
	/// Provides descriptor data describing the Wave effect.
	/// </summary>
	public class WaveDescriptor : EffectModuleDescriptorBase
	{
		#region Static Fields

		private static readonly Guid _typeId = new Guid("E53BBB2D-D415-4E64-8F88-8CCA96F2C8FF");

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

		public override bool SupportsMarks => true;

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
			get { return typeof(Wave); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(WaveData); }
		}

		public override string Author
		{
			get { return "John Baur"; }
		}

		public override string Description
		{
			get { return "Applies a wave effect to pixel elements"; } 
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Wave"; }
		}

		#endregion
	}
}
