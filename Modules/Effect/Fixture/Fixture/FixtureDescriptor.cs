using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Fixture
{
	/// <summary>
	/// Descriptor for the Fixture effect.
	/// </summary>
	public class SetPositionDescriptor : EffectModuleDescriptorBase
	{
		#region Fields

		/// <summary>
		/// Type ID for the effect.
		/// </summary>
		private Guid _typeId = new Guid("{E97BE031-05CF-46F2-ADD9-7E5209FEAB2E}");

		#endregion

		#region IModuleDescriptor

		/// <inheritdoc />
		public override string TypeName => "Fixture";

		/// <inheritdoc />
		public override Guid TypeId => _typeId;

		/// <inheritdoc />
		public override Type ModuleClass => typeof (FixtureModule);

		/// <inheritdoc />
		public override Type ModuleDataClass => typeof (FixtureData);

		/// <inheritdoc />
		public override string Author => "Vixen Team";

		/// <inheritdoc />
		public override string Description => "Controls settings of an intelligent fixture";

		/// <inheritdoc />
		public override string Version => "1.0";

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the effect name.
		/// </summary>
		public override string EffectName => "Fixture";

		/// <inheritdoc />
		public override ParameterSignature Parameters { get; }

		/// <inheritdoc />
		public override EffectGroups EffectGroup => EffectGroups.IntelligentFixture;

		#endregion
	}
}