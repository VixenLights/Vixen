using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Liquid
{
	public class LiquidDescriptor : EffectModuleDescriptorBase
	{
		#region Static Fields

		private static readonly Guid _typeId = new Guid("46C4E660-7188-4B14-98C7-0CA65EB533C6");

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
			get { return typeof(Liquid); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(LiquidData); }
		}

		public override string Author
		{
			get { return "John Baur"; }
		}

		public override string Description
		{
			get { return "Applies a liquid like effect to pixel elements"; } 
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Liquid"; }
		}

		#endregion
	}
}
