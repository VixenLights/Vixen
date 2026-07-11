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
	/// <inheritdoc />
	public override IApplication Application { set { } }

	/// <inheritdoc />
	public override void Loading() { }

	/// <inheritdoc />
	public override void Unloading() { }
}
