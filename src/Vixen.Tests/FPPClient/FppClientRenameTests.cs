using System.Net;
using Vixen.Tests.FPPClient.Helpers;
using VixenModules.App.FPPClient.Exceptions;
using Xunit;

namespace Vixen.Tests.FPPClient;

public class FppClientRenameTests
{
	private static HttpResponseMessage RenameSuccessResponse() =>
		new(HttpStatusCode.OK)
		{
			Content = new StringContent(
				"""{"status":"success","original":"co-universes.json","new":"co-universes.json_1012025-101530"}""")
		};

	[Fact]
	public async Task RenameFileAsync_Success_PostsToCorrectUrl()
	{
		// Arrange
		HttpRequestMessage? captured = null;
		await using var client = MockHttpMessageHandler.CreateClient(req =>
		{
			captured = req;
			return RenameSuccessResponse();
		});

		// Act
		await client.RenameFileAsync(
			"config", "co-universes.json", "co-universes.json_1012025-101530",
			TestContext.Current.CancellationToken);

		// Assert
		Assert.NotNull(captured);
		Assert.Equal(HttpMethod.Post, captured!.Method);
		Assert.Contains(
			"file/config/rename/co-universes.json/co-universes.json_1012025-101530",
			captured.RequestUri!.PathAndQuery);
	}

	[Fact]
	public async Task RenameFileAsync_FileNotFound_DoesNotThrow()
	{
		// Arrange
		await using var client = MockHttpMessageHandler.CreateClient(_ =>
			new HttpResponseMessage(HttpStatusCode.NotFound));

		// Act & Assert — 404 must be treated as a no-op, not an exception
		await client.RenameFileAsync(
			"config", "co-universes.json", "co-universes.json_1012025-101530",
			TestContext.Current.CancellationToken);
	}

	[Fact]
	public async Task RenameFileAsync_HttpError_ThrowsFppClientException()
	{
		// Arrange
		await using var client = MockHttpMessageHandler.CreateClient(_ =>
			new HttpResponseMessage(HttpStatusCode.InternalServerError));

		// Act & Assert
		var ex = await Assert.ThrowsAsync<FppClientException>(
			() => client.RenameFileAsync(
				"config", "co-universes.json", "co-universes.json_backup",
				TestContext.Current.CancellationToken));
		Assert.Equal(500, ex.HttpStatusCode);
	}

	[Fact]
	public async Task RenameFileAsync_NullDirName_ThrowsArgumentNullException()
	{
		// Arrange
		await using var client = MockHttpMessageHandler.CreateClient(_ => RenameSuccessResponse());

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(
			() => client.RenameFileAsync(
				null!, "co-universes.json", "co-universes.json_backup",
				TestContext.Current.CancellationToken));
	}
}
