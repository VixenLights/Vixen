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

	[Theory]
	[InlineData("\"zip\":true", true)]
	[InlineData("\"zip\":false", false)]
	[InlineData("", false)]
	public async Task GetSystemInfoAsync_ZipCapability_DeserializesTrueFalseAndMissing(string zipProperty, bool expected)
	{
		// Arrange
		var responseJson = $"{{{zipProperty}}}";
		await using var client = MockHttpMessageHandler.CreateClient(_ =>
			new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(responseJson) });

		// Act
		var info = await client.GetSystemInfoAsync(TestContext.Current.CancellationToken);

		// Assert
		Assert.Equal(expected, info.Zip);
	}

	[Theory]
	[InlineData("\"2 days, 4:30\"", "2 days, 4:30")]
	[InlineData("188100000", "2.04:15:00")]
	public async Task GetSystemInfoAsync_UtilizationUptime_AcceptsStringOrMilliseconds(string uptimeJson, string expectedUptime)
	{
		// Arrange
		var responseJson = "{\"Utilization\":{\"CPU\":12.5,\"Memory\":42,\"Uptime\":" + uptimeJson + "}}";
		await using var client = MockHttpMessageHandler.CreateClient(_ =>
			new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent(responseJson)
			});

		// Act
		var info = await client.GetSystemInfoAsync(TestContext.Current.CancellationToken);

		// Assert
		Assert.NotNull(info.Utilization);
		Assert.Equal(12.5, info.Utilization.CPU);
		Assert.Equal(42, info.Utilization.Memory);
		Assert.Equal(expectedUptime, info.Utilization.Uptime);
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
