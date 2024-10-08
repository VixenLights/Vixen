﻿using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.LineDance
{
	/// <summary>
	/// Descriptor for the Line Dance effect.
	/// </summary>
	public class LineDanceDescriptor : EffectModuleDescriptorBase
	{
		#region Fields

		/// <summary>
		/// Type ID for the effect.
		/// </summary>
		private static readonly Guid _typeId = new Guid("04C245D6-E8F8-4F45-AC17-2358280B135D");

		#endregion

		#region IModuleDescriptor

		/// <inheritdoc />
		public override string TypeName => EffectName;

		/// <inheritdoc />
		public override Guid TypeId => _typeId;

		/// <inheritdoc />
		public override Type ModuleClass => typeof(LineDanceModule);

		/// <inheritdoc />
		public override Type ModuleDataClass => typeof(LineDanceData);

		/// <inheritdoc />
		public override string Author => "Vixen Team";

		/// <inheritdoc />
		public override string Description => "Applies variaious dance like movements to a group of Intelligent Fixtures";

		/// <inheritdoc />
		public override string Version => "1.0";

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the effect name.
		/// </summary>
		public override string EffectName => "Line Dance";

		/// <inheritdoc />
		public override ParameterSignature Parameters { get; }

		/// <inheritdoc />
		public override EffectGroups EffectGroup => EffectGroups.IntelligentFixture;

		#endregion
	}
}
