using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.SpinColorWheel
{
	/// <summary>
	/// Descriptor for the Spin Color Wheel effect.
	/// </summary>
	public class SpinColorWheelDescriptor : EffectModuleDescriptorBase
	{
		#region Fields

		/// <summary>
		/// Type ID for the effect.
		/// </summary>
		private Guid _typeId = new Guid("{6E3623B6-258A-4CD7-8D0F-A9BAA575A5C4}");

		#endregion

		#region IModuleDescriptor

		/// <inheritdoc />
		public override string TypeName => "SpinColorWheel";

		/// <inheritdoc />
		public override Guid TypeId => _typeId;

		/// <inheritdoc />
		public override Type ModuleClass => typeof(SpinColorWheelModule);

		/// <inheritdoc />
		public override Type ModuleDataClass => typeof(SpinColorWheelData);

		/// <inheritdoc />
		public override string Author => "Vixen Team";

		/// <inheritdoc />
		public override string Description => "Spins the color wheel of an intelligent fixture";

		/// <inheritdoc />
		public override string Version => "1.0";

		#endregion

		#region Public Properites

		/// <summary>
		/// Gets the effect name.
		/// </summary>
		public override string EffectName => "Spin Color Wheel";

		/// <inheritdoc />
		public override ParameterSignature Parameters { get; }

		/// <inheritdoc />
		public override EffectGroups EffectGroup => EffectGroups.IntelligentFixture;

		#endregion
	}
}