using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Fan
{
	/// <summary>
	/// Descriptor for the Fan effect.
	/// </summary>
	public class FanDescriptor : EffectModuleDescriptorBase
	{
		#region Fields

		/// <summary>
		/// Type ID for the effect.
		/// </summary>
		private static readonly Guid _typeId = new Guid("E3DA54E8-4267-4E03-B010-5F2F0FE5B66B");

		#endregion

		#region IModuleDescriptor

		/// <inheritdoc />
		public override string TypeName => EffectName;

		/// <inheritdoc />
		public override Guid TypeId => _typeId;

		/// <inheritdoc />
		public override Type ModuleClass => typeof(FanModule);

		/// <inheritdoc />
		public override Type ModuleDataClass => typeof(FanData);

		/// <inheritdoc />
		public override string Author => "Vixen Team";

		/// <inheritdoc />
		public override string Description => "Applies a Fan effect to a group of Intelligent Fixtures";

		/// <inheritdoc />
		public override string Version => "1.0";

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the effect name.
		/// </summary>
		public override string EffectName => "Fan";

		/// <inheritdoc />
		public override ParameterSignature Parameters { get; }

		/// <inheritdoc />
		public override EffectGroups EffectGroup => EffectGroups.IntelligentFixture;

		#endregion
	}
}
