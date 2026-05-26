using Vixen.Module.App;
using Vixen.Sys;

namespace VixenModules.App.FPPClient;

/// <summary>
/// Vixen App module entry point for the FPP Client.
/// The module has no UI or background service; it exists to register the
/// FPP client library with Vixen's plugin loader.
/// </summary>
public class Module : AppModuleInstanceBase
{
	private Data _data = new();

	/// <inheritdoc />
	public override IApplication Application { set { } }

	/// <inheritdoc />
	public override Vixen.Module.IModuleDataModel StaticModuleData
	{
		get => _data;
		set => _data = (Data)value;
	}

	/// <inheritdoc />
	public override void Loading() { }

	/// <inheritdoc />
	public override void Unloading() { }
}
