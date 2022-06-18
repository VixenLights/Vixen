using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Gobo
{
	/// <summary>
	/// Descriptor for the Gobo effect.
	/// </summary>
	public class GoboDescriptor : EffectModuleDescriptorBase
	{
		#region Fields

		/// <summary>
		/// Type ID for the effect.
		/// </summary>
		private Guid _typeId = new Guid("{EDC24546-4D8B-4062-9A62-10BCEDF225D0}");

		#endregion

		#region IModuleDescriptor

		/// <inheritdoc />
		public override string TypeName => "Gobo";

		/// <inheritdoc />
		public override Guid TypeId => _typeId;

		/// <inheritdoc />
		public override Type ModuleClass => typeof(GoboModule);

		/// <inheritdoc />
		public override Type ModuleDataClass => typeof(GoboData);

		/// <inheritdoc />
		public override string Author => "Vixen Team";

		/// <inheritdoc />
		public override string Description => "Selects a Gobo for an intelligent fixture";

		/// <inheritdoc />
		public override string Version => "1.0";

		#endregion

		#region Public Properites

		/// <summary>
		/// Gets the effect name.
		/// </summary>
		public override string EffectName => "Gobo";

		/// <inheritdoc />
		public override ParameterSignature Parameters { get; }

		/// <inheritdoc />
		public override EffectGroups EffectGroup => EffectGroups.IntelligentFixture;

		#endregion
	}
}