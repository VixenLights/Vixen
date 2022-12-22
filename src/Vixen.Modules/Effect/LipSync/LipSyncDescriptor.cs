﻿using Vixen.Sys;
using Vixen.Module.Effect;

namespace VixenModules.Effect.LipSync
{
	public class LipSyncDescriptor : EffectModuleDescriptorBase
	{
		private static Guid _typeId = new Guid("{52F17F4B-2159-4820-8660-05CD9D1F47C1}");

		public override string EffectName
		{
			get { return "LipSync"; }
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
			get { return typeof(LipSync); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(LipSyncData); }
		}

		public override string Author
		{
			get { return "Ed Brady"; }
		}

		public override string TypeName
		{
			get { return EffectName; }
		}

		public override string Description
		{
			get { return "Incorporate Lipsync Files and Data"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override ParameterSignature Parameters => new ParameterSignature();
	}
}