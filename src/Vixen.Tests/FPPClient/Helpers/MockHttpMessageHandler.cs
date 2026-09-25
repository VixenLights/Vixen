using VixenModules.App.FPPClient.Client;

namespace Vixen.Tests.FPPClient.Helpers;

/// <summary>
/// A test <see cref="HttpMessageHandler"/> that delegates each request to a caller-supplied function,
/// allowing tests to inspect outgoing requests and return pre-configured responses without network access.
/// </summary>
internal sealed class MockHttpMessageHandler : HttpMessageHandler
{
	private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handler;

	internal MockHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler)
		=> _handler = handler;

	protected override Task<HttpResponseMessage> SendAsync(
		HttpRequestMessage request, CancellationToken cancellationToken)
		=> _handler(request, cancellationToken);

	/// <summary>
	/// Creates an <see cref="IFppClient"/> backed by the given handler function.
	/// </summary>
	internal static IFppClient CreateClient(Func<HttpRequestMessage, HttpResponseMessage> handler) =>
		CreateClient((request, _) => Task.FromResult(handler(request)));

	/// <summary>
	/// Creates an <see cref="IFppClient"/> backed by an asynchronous handler function.
	/// </summary>
	internal static IFppClient CreateClient(
		Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler)
		=> CreateClient(handler, new FppClientOptions { BaseUrl = "http://fpp.test/" });

	/// <summary>
	/// Creates an <see cref="IFppClient"/> backed by the given handler and connection options.
	/// </summary>
	internal static IFppClient CreateClient(
		Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler,
		FppClientOptions options)
	{
		var httpClient = new HttpClient(new MockHttpMessageHandler(handler));
		return new FppClient(httpClient, options);
	}
}
