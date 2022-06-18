using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Frost
{
	/// <summary>
	/// Descriptor for the prism effect.
	/// </summary>
	public class FrostDescriptor : EffectModuleDescriptorBase
	{
		#region Fields

		/// <summary>
		/// Type ID for the effect.
		/// </summary>
		private Guid _typeId = new Guid("{8E1AD8C8-B331-4519-A26B-0445AEA7240F}");

		#endregion

		#region IModuleDescriptor

		/// <inheritdoc />
		public override string TypeName => "Frost";

		/// <inheritdoc />
		public override Guid TypeId => _typeId;

		/// <inheritdoc />
		public override Type ModuleClass => typeof(FrostModule);

		/// <inheritdoc />
		public override Type ModuleDataClass => typeof(FrostData);

		/// <inheritdoc />
		public override string Author => "Vixen Team";

		/// <inheritdoc />
		public override string Description => "Configures the prism of an intelligent fixture";

		/// <inheritdoc />
		public override string Version => "1.0";

		#endregion

		#region Public Properites

		/// <summary>
		/// Gets the effect name.
		/// </summary>
		public override string EffectName => "Frost";

		/// <inheritdoc />
		public override ParameterSignature Parameters { get; }

		/// <inheritdoc />
		public override EffectGroups EffectGroup => EffectGroups.IntelligentFixture;

		#endregion
	}
}