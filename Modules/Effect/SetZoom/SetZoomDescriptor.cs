using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.SetZoom
{
	/// <summary>
	/// Descriptor for the Set Zoom effect.
	/// </summary>
	public class SetZoomDescriptor : EffectModuleDescriptorBase
	{
		#region Fields

		/// <summary>
		/// Type ID for the effect.
		/// </summary>
		private Guid _typeId = new Guid("{66FC831B-18C8-4EA5-99BE-6B57E5B92522}");

		#endregion

		#region IModuleDescriptor

		/// <inheritdoc />
		public override string TypeName => "Set Zoom";

		/// <inheritdoc />
		public override Guid TypeId => _typeId;

		/// <inheritdoc />
		public override Type ModuleClass => typeof(SetZoomModule);

		/// <inheritdoc />
		public override Type ModuleDataClass => typeof(SetZoomData);

		/// <inheritdoc />
		public override string Author => "Vixen Team";

		/// <inheritdoc />
		public override string Description => "Sets the zoom of an intelligent fixture";

		/// <inheritdoc />
		public override string Version => "1.0";

		#endregion

		#region Public Properites

		/// <summary>
		/// Gets the effect name.
		/// </summary>
		public override string EffectName => "Set Zoom";

		/// <inheritdoc />
		public override ParameterSignature Parameters { get; }

		/// <inheritdoc />
		public override EffectGroups EffectGroup => EffectGroups.IntelligentFixture;

		#endregion
	}
}