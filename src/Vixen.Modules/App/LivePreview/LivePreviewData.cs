using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.App.LivePreview
{
	/// <summary>Serializable configuration data for the Live Preview module.</summary>
	/// <remarks>The module has no user-configurable settings; this class exists to satisfy the module framework contract.</remarks>
	[DataContract]
	public class LivePreviewData : ModuleDataModelBase
	{
		/// <inheritdoc/>
		public override IModuleDataModel Clone() => (IModuleDataModel)MemberwiseClone();
	}
}
