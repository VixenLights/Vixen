using Vixen.Module.App;

namespace VixenModules.App.LivePreview
{
	/// <summary>Module descriptor for the Live Preview App module.</summary>
	public class Descriptor : AppModuleDescriptorBase
	{
		private static readonly Guid _typeId = new("{6F857027-6CBC-4729-BD00-6769C326E2CC}");

		/// <inheritdoc/>
		public override string TypeName => "Live Preview";

		/// <inheritdoc/>
		public override Guid TypeId => _typeId;

		/// <inheritdoc/>
		public override string Author => "Vixen Team";

		/// <inheritdoc/>
		public override string Description => "Shared live element control service for broadcast-driven previews";

		/// <inheritdoc/>
		public override string Version => "1.0";

		/// <inheritdoc/>
		public override Type ModuleClass => typeof(Module);

		/// <inheritdoc/>
		public override Type ModuleStaticDataClass => typeof(LivePreviewData);
	}
}
