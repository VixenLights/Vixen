using System.Net;
using Vixen.Tests.FPPClient.Helpers;
using VixenModules.App.FPPClient.Exceptions;
using Xunit;

namespace Vixen.Tests.FPPClient;

public class FppClientUploadTests
{
	private static HttpResponseMessage OkUploadResponse() =>
		new(HttpStatusCode.OK)
		{
			Content = new StringContent("""{"status":"OK","file":"test","dir":"sequences"}""")
		};

	[Fact]
	public async Task UploadSequenceAsync_Success_PostsToCorrectUrl()
	{
		// Arrange
		HttpRequestMessage? captured = null;
		await using var client = MockHttpMessageHandler.CreateClient(req =>
		{
			captured = req;
			return OkUploadResponse();
		});

		// Act
		await client.UploadSequenceAsync("test.fseq", new MemoryStream([1, 2, 3]),
			TestContext.Current.CancellationToken);

		// Assert
		Assert.NotNull(captured);
		Assert.Equal(HttpMethod.Post, captured!.Method);
		Assert.Contains("file/sequences/test.fseq", captured.RequestUri!.PathAndQuery);
	}

	[Fact]
	public async Task UploadMusicAsync_Success_PostsToCorrectUrl()
	{
		// Arrange
		HttpRequestMessage? captured = null;
		await using var client = MockHttpMessageHandler.CreateClient(req =>
		{
			captured = req;
			return OkUploadResponse();
		});

		// Act
		await client.UploadMusicAsync("song.mp3", new MemoryStream([1, 2, 3]),
			TestContext.Current.CancellationToken);

		// Assert
		Assert.NotNull(captured);
		Assert.Equal(HttpMethod.Post, captured!.Method);
		Assert.Contains("file/music/song.mp3", captured.RequestUri!.PathAndQuery);
	}

	[Fact]
	public async Task UploadVideoAsync_Success_PostsToCorrectUrl()
	{
		// Arrange
		HttpRequestMessage? captured = null;
		await using var client = MockHttpMessageHandler.CreateClient(req =>
		{
			captured = req;
			return OkUploadResponse();
		});

		// Act
		await client.UploadVideoAsync("show.mp4", new MemoryStream([1, 2, 3]),
			TestContext.Current.CancellationToken);

		// Assert
		Assert.NotNull(captured);
		Assert.Equal(HttpMethod.Post, captured!.Method);
		Assert.Contains("file/videos/show.mp4", captured.RequestUri!.PathAndQuery);
	}

	[Fact]
	public async Task UploadFileAsync_NullDirName_ThrowsArgumentException()
	{
		// Arrange
		await using var client = MockHttpMessageHandler.CreateClient(_ => OkUploadResponse());

		// Act & Assert — null triggers ArgumentNullException (subtype of ArgumentException)
		await Assert.ThrowsAsync<ArgumentNullException>(
			() => client.UploadFileAsync(null!, "file.fseq", new MemoryStream(),
				TestContext.Current.CancellationToken));
	}

	[Fact]
	public async Task UploadFileAsync_NullFilename_ThrowsArgumentException()
	{
		// Arrange
		await using var client = MockHttpMessageHandler.CreateClient(_ => OkUploadResponse());

		// Act & Assert — null triggers ArgumentNullException (subtype of ArgumentException)
		await Assert.ThrowsAsync<ArgumentNullException>(
			() => client.UploadFileAsync("sequences", null!, new MemoryStream(),
				TestContext.Current.CancellationToken));
	}

	[Fact]
	public async Task UploadFileAsync_HttpError_ThrowsFppClientException()
	{
		// Arrange
		await using var client = MockHttpMessageHandler.CreateClient(_ =>
			new HttpResponseMessage(HttpStatusCode.InternalServerError));

		// Act & Assert
		var ex = await Assert.ThrowsAsync<FppClientException>(
			() => client.UploadFileAsync("sequences", "test.fseq", new MemoryStream([1, 2, 3]),
				TestContext.Current.CancellationToken));
		Assert.Equal(500, ex.HttpStatusCode);
	}
}
