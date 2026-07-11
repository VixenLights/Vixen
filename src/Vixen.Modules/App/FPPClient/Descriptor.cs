using Vixen.Module.App;

namespace VixenModules.App.FPPClient;

/// <summary>
/// Describes the FPP Client App module to the Vixen plugin system.
/// </summary>
public class Descriptor : AppModuleDescriptorBase
{
	private readonly Guid _typeId = new Guid("{C9EDA00C-E5AC-4880-8BB5-00230A426703}");

	/// <inheritdoc />
	public override string TypeName => "FPP Client";

	/// <inheritdoc />
	public override Guid TypeId => _typeId;

	/// <inheritdoc />
	public override string Author => "Vixen Team";

	/// <inheritdoc />
	public override string Description => "REST API client for Falcon Player (FPP) instances";

	/// <inheritdoc />
	public override string Version => "1.0";

	/// <inheritdoc />
	public override Type ModuleClass => typeof(Module);
}
