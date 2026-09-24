using System.IO.Compression;
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

	[Fact]
	public async Task UploadEspPixelStickSequenceAsync_Success_PostsBytesToHostRoot()
	{
		// Arrange
		const string filename = "show & snow.fseq";
		byte[] expected = [1, 2, 3, 4];
		HttpMethod? method = null;
		Uri? requestUri = null;
		string? contentType = null;
		byte[]? uploaded = null;
		await using var client = MockHttpMessageHandler.CreateClient(async (request, token) =>
		{
			method = request.Method;
			requestUri = request.RequestUri;
			contentType = request.Content?.Headers.ContentType?.MediaType;
			uploaded = await request.Content!.ReadAsByteArrayAsync(token);
			return new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent("""{"Name":"show & snow.fseq","Version":"2.0"}""")
			};
		});

		// Act
		await client.UploadEspPixelStickSequenceAsync(filename, new MemoryStream(expected),
			TestContext.Current.CancellationToken);

		// Assert
		Assert.Equal(HttpMethod.Post, method);
		Assert.Equal("http://fpp.test/fpp?path=uploadFile&filename=show%20%26%20snow.fseq", requestUri?.AbsoluteUri);
		Assert.Equal("application/octet-stream", contentType);
		Assert.Equal(expected, uploaded);
	}

	[Fact]
	public async Task UploadEspPixelStickSequenceAsync_HttpError_ThrowsFppClientException()
	{
		// Arrange
		await using var client = MockHttpMessageHandler.CreateClient(_ =>
			new HttpResponseMessage(HttpStatusCode.NotFound));

		// Act
		var ex = await Assert.ThrowsAsync<FppClientException>(() =>
			client.UploadEspPixelStickSequenceAsync("test.fseq", new MemoryStream([1, 2, 3]),
				TestContext.Current.CancellationToken));

		// Assert
		Assert.Equal(404, ex.HttpStatusCode);
		Assert.Contains("test.fseq", ex.Message);
	}

	[Theory]
	[InlineData("")]
	[InlineData("not json")]
	[InlineData("{}")]
	[InlineData("{\"Name\":\"other.fseq\"}")]
	[InlineData("{\"Name\":null}")]
	public async Task UploadEspPixelStickSequenceAsync_InvalidMetadata_ThrowsFppClientException(string responseBody)
	{
		// Arrange
		await using var client = MockHttpMessageHandler.CreateClient(_ =>
			new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(responseBody) });

		// Act & Assert
		var ex = await Assert.ThrowsAsync<FppClientException>(() =>
			client.UploadEspPixelStickSequenceAsync("test.fseq", new MemoryStream([1, 2, 3]),
				TestContext.Current.CancellationToken));
		Assert.Contains("test.fseq", ex.Message);
	}

	[Fact]
	public async Task UploadEspPixelStickSequenceAsync_InvalidArguments_ThrowBeforeSending()
	{
		// Arrange
		var sent = false;
		await using var client = MockHttpMessageHandler.CreateClient(_ =>
		{
			sent = true;
			return new HttpResponseMessage(HttpStatusCode.OK);
		});
		await using var unreadable = new GZipStream(new MemoryStream(), CompressionMode.Compress);
		Assert.False(unreadable.CanRead);

		// Act & Assert
		await Assert.ThrowsAsync<ArgumentNullException>(() =>
			client.UploadEspPixelStickSequenceAsync(null!, new MemoryStream(), TestContext.Current.CancellationToken));
		await Assert.ThrowsAsync<ArgumentException>(() =>
			client.UploadEspPixelStickSequenceAsync("folder/test.fseq", new MemoryStream(),
				TestContext.Current.CancellationToken));
		await Assert.ThrowsAsync<ArgumentNullException>(() =>
			client.UploadEspPixelStickSequenceAsync("test.fseq", null!, TestContext.Current.CancellationToken));
		await Assert.ThrowsAsync<ArgumentException>(() =>
			client.UploadEspPixelStickSequenceAsync("test.fseq", unreadable, TestContext.Current.CancellationToken));
		Assert.False(sent);
	}

	[Fact]
	public async Task UploadEspPixelStickSequenceAsync_Cancelled_PropagatesCancellation()
	{
		// Arrange
		await using var client = MockHttpMessageHandler.CreateClient((_, token) =>
		{
			token.ThrowIfCancellationRequested();
			return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
		});
		using var cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
		await cts.CancelAsync();

		// Act & Assert
		await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
			client.UploadEspPixelStickSequenceAsync("test.fseq", new MemoryStream([1]), cts.Token));
	}
}
