namespace VixenModules.App.FPPClient.Client;

/// <summary>
/// Creates <see cref="IFppClient"/> instances configured for a specific FPP host.
/// </summary>
public interface IFppClientFactory
{
	/// <summary>
	/// Creates a new <see cref="IFppClient"/> configured with the specified options.
	/// </summary>
	/// <param name="options">The connection options for the target FPP instance.</param>
	/// <returns>A new <see cref="IFppClient"/> instance. The caller is responsible for disposing it.</returns>
	IFppClient Create(FppClientOptions options);
}
