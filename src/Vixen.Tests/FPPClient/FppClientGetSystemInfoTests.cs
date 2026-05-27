using System.Net;
using System.Text.Json;
using Vixen.Tests.FPPClient.Helpers;
using VixenModules.App.FPPClient.Exceptions;
using Xunit;

namespace Vixen.Tests.FPPClient;

public class FppClientGetSystemInfoTests
{
	private const string ValidJson = """
		{
			"HostName": "fpp2",
			"Platform": "Raspberry Pi",
			"Variant": "Pi 4",
			"Version": "7.5",
			"Mode": "player",
			"IPs": ["192.168.1.100", "10.0.0.1"]
		}
		""";

	[Fact]
	public async Task GetSystemInfoAsync_Success_ReturnsSystemInfo()
	{
		// Arrange
		await using var client = MockHttpMessageHandler.CreateClient(_ =>
			new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent(ValidJson)
			});

		// Act
		var info = await client.GetSystemInfoAsync(TestContext.Current.CancellationToken);

		// Assert
		Assert.Equal("fpp2", info.HostName);
		Assert.Equal("Raspberry Pi", info.Platform);
		Assert.Equal("7.5", info.Version);
		Assert.Equal(2, info.IPs.Length);
		Assert.Contains("192.168.1.100", info.IPs);
	}

	[Fact]
	public async Task GetSystemInfoAsync_HttpError_ThrowsFppClientException()
	{
		// Arrange
		await using var client = MockHttpMessageHandler.CreateClient(_ =>
			new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));

		// Act & Assert
		var ex = await Assert.ThrowsAsync<FppClientException>(
			() => client.GetSystemInfoAsync(TestContext.Current.CancellationToken));
		Assert.Equal(503, ex.HttpStatusCode);
	}

	[Fact]
	public async Task GetSystemInfoAsync_MalformedJson_ThrowsException()
	{
		// Arrange
		await using var client = MockHttpMessageHandler.CreateClient(_ =>
			new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent("not json")
			});

		// Act & Assert
		await Assert.ThrowsAsync<JsonException>(
			() => client.GetSystemInfoAsync(TestContext.Current.CancellationToken));
	}
}
