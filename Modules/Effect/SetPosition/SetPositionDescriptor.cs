using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.SetPosition
{
	/// <summary>
	/// Descriptor for the Set Position effect.
	/// </summary>
	public class SetPositionDescriptor : EffectModuleDescriptorBase
	{
		#region Fields

		/// <summary>
		/// Type ID for the effect.
		/// </summary>
		private Guid _typeId = new Guid("{9B6D85EC-F16B-41f2-8584-8E85211E02B8}");

		#endregion

		#region IModuleDescriptor

		/// <inheritdoc />
		public override string TypeName => "Set position";

		/// <inheritdoc />
		public override Guid TypeId => _typeId;

		/// <inheritdoc />
		public override Type ModuleClass => typeof (SetPositionModule);

		/// <inheritdoc />
		public override Type ModuleDataClass => typeof (SetPositionData);

		/// <inheritdoc />
		public override string Author => "Vixen Team";

		/// <inheritdoc />
		public override string Description => "Set the position of a positionable device";

		/// <inheritdoc />
		public override string Version => "1.0";

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the effect name.
		/// </summary>
		public override string EffectName => "Set Position";

		/// <inheritdoc />
		public override ParameterSignature Parameters { get; }

		/// <inheritdoc />
		public override EffectGroups EffectGroup => EffectGroups.IntelligentFixture;

		#endregion
	}
}