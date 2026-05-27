namespace VixenModules.App.FPPClient.Client;

/// <summary>
/// Default implementation of <see cref="IFppClientFactory"/> that creates <see cref="FppClient"/> instances.
/// </summary>
public sealed class FppClientFactory : IFppClientFactory
{
	/// <inheritdoc/>
	public IFppClient Create(FppClientOptions options) =>
		new FppClient(new HttpClient(), options);
}
