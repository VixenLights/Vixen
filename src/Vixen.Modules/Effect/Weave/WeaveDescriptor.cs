using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Weave
{
	/// <summary>
	/// Descriptor for the Weave effect.
	/// </summary>
	public class WeaveDescriptor : EffectModuleDescriptorBase
	{
		#region Fields

		/// <summary>
		/// Type ID for the effect.
		/// </summary>
		private static readonly Guid _typeId = new Guid("4633DE41-BB29-443F-A390-23EA0C464AB2");

		#endregion

		#region IModuleDescriptor

		/// <inheritdoc />
		public override string TypeName => EffectName;

		/// <inheritdoc />
		public override Guid TypeId => _typeId;

		/// <inheritdoc />
		public override Type ModuleClass => typeof(Weave);

		/// <inheritdoc />
		public override Type ModuleDataClass => typeof(WeaveData);

		/// <inheritdoc />
		public override string Author => "Vixen Team";

		/// <inheritdoc />
		public override string Description => "Applies a Weave effect to pixel elements";

		/// <inheritdoc />
		public override string Version => "1.0";

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the effect name.
		/// </summary>
		public override string EffectName => "Weave";

		/// <inheritdoc />
		public override ParameterSignature Parameters { get; }

		/// <inheritdoc />
		public override EffectGroups EffectGroup => EffectGroups.Pixel;

		#endregion
	}
}
