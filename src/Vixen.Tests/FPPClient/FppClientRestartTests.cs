using System.Net;
using Vixen.Tests.FPPClient.Helpers;
using VixenModules.App.FPPClient.Exceptions;
using Xunit;

namespace Vixen.Tests.FPPClient;

public class FppClientRestartTests
{
	private static HttpResponseMessage OkResponse() =>
		new(HttpStatusCode.OK)
		{
			Content = new StringContent("""{"status":"OK"}""")
		};

	[Fact]
	public async Task RestartFppdAsync_Success_GetsCorrectUrl()
	{
		// Arrange
		HttpRequestMessage? captured = null;
		await using var client = MockHttpMessageHandler.CreateClient(req =>
		{
			captured = req;
			return OkResponse();
		});

		// Act
		await client.RestartFppdAsync(quick: false, TestContext.Current.CancellationToken);

		// Assert
		Assert.NotNull(captured);
		Assert.Equal(HttpMethod.Get, captured!.Method);
		Assert.Contains("system/fppd/restart", captured.RequestUri!.PathAndQuery);
		Assert.DoesNotContain("quick", captured.RequestUri.PathAndQuery);
	}

	[Fact]
	public async Task RestartFppdAsync_Quick_AppendsQuickQueryParameter()
	{
		// Arrange
		HttpRequestMessage? captured = null;
		await using var client = MockHttpMessageHandler.CreateClient(req =>
		{
			captured = req;
			return OkResponse();
		});

		// Act
		await client.RestartFppdAsync(quick: true, TestContext.Current.CancellationToken);

		// Assert
		Assert.NotNull(captured);
		Assert.Equal(HttpMethod.Get, captured!.Method);
		Assert.Contains("system/fppd/restart", captured.RequestUri!.PathAndQuery);
		Assert.Contains("quick=1", captured.RequestUri!.Query);
	}

	[Fact]
	public async Task RestartFppdAsync_HttpError_ThrowsFppClientException()
	{
		// Arrange
		await using var client = MockHttpMessageHandler.CreateClient(_ =>
			new HttpResponseMessage(HttpStatusCode.InternalServerError));

		// Act & Assert
		var ex = await Assert.ThrowsAsync<FppClientException>(
			() => client.RestartFppdAsync(cancellationToken: TestContext.Current.CancellationToken));
		Assert.Equal(500, ex.HttpStatusCode);
	}

	[Fact]
	public async Task RestartFppdAsync_DefaultsToFullRestart()
	{
		// Arrange — calling with no arguments should not include quick=1
		HttpRequestMessage? captured = null;
		await using var client = MockHttpMessageHandler.CreateClient(req =>
		{
			captured = req;
			return OkResponse();
		});

		// Act
		await client.RestartFppdAsync(cancellationToken: TestContext.Current.CancellationToken);

		// Assert
		Assert.DoesNotContain("quick", captured!.RequestUri!.PathAndQuery);
	}
}
