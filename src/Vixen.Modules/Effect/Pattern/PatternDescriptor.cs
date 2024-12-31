using Vixen.Module.Effect;
using Vixen.Sys;

using VixenModules.Effect.Weave;

namespace VixenModules.Effect.Pattern
{
	/// <summary>
	/// Descriptor for the Pattern effect.
	/// </summary>
	public class PatternDescriptor : EffectModuleDescriptorBase
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
		public override Type ModuleClass => typeof(Pattern);

		/// <inheritdoc />
		public override Type ModuleDataClass => typeof(WeaveData);

		/// <inheritdoc />
		public override string Author => "Vixen Team";

		/// <inheritdoc />
		public override string Description => "Applies a Pattern effect to pixel elements";

		/// <inheritdoc />
		public override string Version => "1.0";

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the effect name.
		/// </summary>
		public override string EffectName => "Pattern";

		/// <inheritdoc />
		public override ParameterSignature Parameters { get; }

		/// <inheritdoc />
		public override EffectGroups EffectGroup => EffectGroups.Pixel;

		#endregion
	}
}
