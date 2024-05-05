using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Whirlpool
{
	/// <summary>
	/// Descriptor for the Whirlpool effect.
	/// </summary>
	public class WhirlpoolDescriptor : EffectModuleDescriptorBase
	{
		#region Fields

		/// <summary>
		/// Type ID for the effect.
		/// </summary>
		private static readonly Guid _typeId = new Guid("c2c2ced8-a2a8-46f5-a4d4-43441e68c2bc");

		#endregion

		#region IModuleDescriptor

		/// <inheritdoc />
		public override string TypeName => EffectName;

		/// <inheritdoc />
		public override Guid TypeId => _typeId;

		/// <inheritdoc />
		public override Type ModuleClass => typeof(Whirlpool);

		/// <inheritdoc />
		public override Type ModuleDataClass => typeof(WhirlpoolData);

		/// <inheritdoc />
		public override string Author => "Vixen Team";

		/// <inheritdoc />
		public override string Description => "Applies a Whirlpool like effect to pixel display elements";

		/// <inheritdoc />
		public override string Version => "1.0";

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the effect name.
		/// </summary>
		public override string EffectName => "Whirlpool";

		/// <inheritdoc />
		public override ParameterSignature Parameters => new ParameterSignature();

		/// <inheritdoc />
		public override EffectGroups EffectGroup => EffectGroups.Pixel;

		#endregion
	}
}
