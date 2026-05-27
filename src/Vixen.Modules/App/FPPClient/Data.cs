using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.App.FPPClient;

/// <summary>
/// Serializable settings data for the FPP Client module.
/// </summary>
[DataContract]
public class Data : ModuleDataModelBase
{
	/// <inheritdoc />
	public override IModuleDataModel Clone() => (IModuleDataModel)MemberwiseClone();
}
