﻿using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Dissolve
{
	public class DissolveDescriptor : EffectModuleDescriptorBase
	{
		private static Guid _typeId = new Guid("{66311b1a-3a04-4722-bdaf-87f3417ae50f}");
		private static Guid _ColorGradientId = new Guid("{64f4ab26-3ed4-49a3-a004-23656ed0424a}");


		public override string EffectName
		{
			get { return "Dissolve"; }
		}

		public override EffectGroups EffectGroup
		{
			get { return EffectGroups.Basic; }
		}

		#region Overrides of EffectModuleDescriptorBase

		/// <inheritdoc />
		public override bool SupportsMarks => true;

		#endregion

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof(Dissolve); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(DissolveData); }
		}

		public override string Author
		{
			get { return "Geoff Armstrong"; }
		}

		public override string TypeName
		{
			get { return EffectName; }
		}

		public override string Description
		{
			get { return "Sets the target elements to Dissolve element over time."; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override Guid[] Dependencies
		{
			get { return new Guid[] { _ColorGradientId }; }
		}

		public override ParameterSignature Parameters
		{
			get { return new ParameterSignature(); }
		}
	}
}