using System.Net;

namespace Vixen.Tests.Application.Updates;

internal sealed class RecordingHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler) : HttpMessageHandler
{
	private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler = handler;

	internal List<Uri> RequestUris { get; } = [];

	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		RequestUris.Add(request.RequestUri!);
		return Task.FromResult(_handler(request));
	}
}
