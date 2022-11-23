using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.FixtureStrobe
{
	/// <summary>
	/// Descriptor for the fixture strobe effect.
	/// </summary>
	public class FixtureStrobeDescriptor : EffectModuleDescriptorBase
	{
		#region Fields

		/// <summary>
		/// Type ID for the effect.
		/// </summary>
		private Guid _typeId = new Guid("{E27630FD-EC96-42DA-A95A-B776E39C2DDB}");

		#endregion

		#region IModuleDescriptor

		/// <inheritdoc />
		public override string TypeName => "Fixture Strobe";

		/// <inheritdoc />
		public override Guid TypeId => _typeId;

		/// <inheritdoc />
		public override Type ModuleClass => typeof(FixtureStrobeModule);

		/// <inheritdoc />
		public override Type ModuleDataClass => typeof(FixtureStrobeData);

		/// <inheritdoc />
		public override string Author => "Vixen Team";

		/// <inheritdoc />
		public override string Description => "Configures the strobe of an intelligent fixture";

		/// <inheritdoc />
		public override string Version => "1.0";

		#endregion

		#region Public Properites

		/// <summary>
		/// Gets the effect name.
		/// </summary>
		public override string EffectName => "Fixture Strobe";

		/// <inheritdoc />
		public override ParameterSignature Parameters { get; }

		/// <inheritdoc />
		public override EffectGroups EffectGroup => EffectGroups.IntelligentFixture;

		#endregion
	}
}