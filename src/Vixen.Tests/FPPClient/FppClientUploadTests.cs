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
		Assert.Equal("http://fpp.test/api/file/sequences/test.fseq", captured.RequestUri!.AbsoluteUri);
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

	[Fact]
	public async Task UploadEspPixelStickArchiveAsync_Success_PostsEscapedArchiveAndDisposesStream()
	{
		// Arrange
		const string filename = "vixen export & one.zip";
		byte[] expected = [9, 8, 7, 6];
		var content = new MemoryStream(expected);
		HttpMethod? method = null;
		string? pathAndQuery = null;
		string? contentType = null;
		byte[]? uploaded = null;
		await using var client = MockHttpMessageHandler.CreateClient(async (request, token) =>
		{
			method = request.Method;
			pathAndQuery = request.RequestUri!.PathAndQuery;
			contentType = request.Content?.Headers.ContentType?.MediaType;
			uploaded = await request.Content!.ReadAsByteArrayAsync(token);
			return new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent("{\"Name\":\"" + filename + "\"}")
			};
		});

		// Act
		await client.UploadEspPixelStickArchiveAsync(filename, content, TestContext.Current.CancellationToken);

		// Assert
		Assert.Equal(HttpMethod.Post, method);
		Assert.Equal("/fpp?path=uploadFile&filename=vixen%20export%20%26%20one.zip", pathAndQuery);
		Assert.Equal("application/octet-stream", contentType);
		Assert.Equal(expected, uploaded);
		Assert.False(content.CanRead);
	}

	[Fact]
	public async Task UploadEspPixelStickArchiveAsync_UsesUploadTimeoutInsteadOfRequestTimeout()
	{
		// Arrange
		const string filename = "test.zip";
		var options = new VixenModules.App.FPPClient.Client.FppClientOptions
		{
			BaseUrl = "http://fpp.test/",
			Timeout = TimeSpan.FromMilliseconds(20),
			UploadTimeout = TimeSpan.FromSeconds(2)
		};
		await using var client = MockHttpMessageHandler.CreateClient(async (_, token) =>
		{
			await Task.Delay(TimeSpan.FromMilliseconds(100), token);
			return new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent("{\"Name\":\"test.zip\"}")
			};
		}, options);

		// Act
		await client.UploadEspPixelStickArchiveAsync(filename, new MemoryStream([1]),
			TestContext.Current.CancellationToken);
	}

	[Fact]
	public async Task UploadEspPixelStickArchiveAsync_HttpError_ThrowsFppClientException()
	{
		// Arrange
		await using var client = MockHttpMessageHandler.CreateClient(_ =>
			new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));

		// Act
		var ex = await Assert.ThrowsAsync<FppClientException>(() =>
			client.UploadEspPixelStickArchiveAsync("test.zip", new MemoryStream([1, 2]),
				TestContext.Current.CancellationToken));

		// Assert
		Assert.Equal(503, ex.HttpStatusCode);
		Assert.Contains("test.zip", ex.Message);
	}

	[Theory]
	[InlineData("")]
	[InlineData("not json")]
	[InlineData("{}")]
	[InlineData("{\"Name\":\"other.zip\"}")]
	[InlineData("{\"Name\":null}")]
	public async Task UploadEspPixelStickArchiveAsync_InvalidMetadata_ThrowsFppClientException(string responseBody)
	{
		// Arrange
		await using var client = MockHttpMessageHandler.CreateClient(_ =>
			new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(responseBody) });

		// Act & Assert
		var ex = await Assert.ThrowsAsync<FppClientException>(() =>
			client.UploadEspPixelStickArchiveAsync("test.zip", new MemoryStream([1]),
				TestContext.Current.CancellationToken));
		Assert.Contains("test.zip", ex.Message);
	}

	[Fact]
	public async Task UploadEspPixelStickArchiveAsync_InvalidArguments_ThrowBeforeSending()
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
			client.UploadEspPixelStickArchiveAsync(null!, new MemoryStream(), TestContext.Current.CancellationToken));
		await Assert.ThrowsAsync<ArgumentException>(() =>
			client.UploadEspPixelStickArchiveAsync("folder/test.zip", new MemoryStream(), TestContext.Current.CancellationToken));
		await Assert.ThrowsAsync<ArgumentNullException>(() =>
			client.UploadEspPixelStickArchiveAsync("test.zip", null!, TestContext.Current.CancellationToken));
		await Assert.ThrowsAsync<ArgumentException>(() =>
			client.UploadEspPixelStickArchiveAsync("test.zip", unreadable, TestContext.Current.CancellationToken));
		Assert.False(sent);
	}

	[Fact]
	public async Task UploadEspPixelStickArchiveAsync_Cancelled_PropagatesCancellation()
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
			client.UploadEspPixelStickArchiveAsync("test.zip", new MemoryStream([1]), cts.Token));
	}

	[Fact]
	public async Task RebootEspPixelStickAsync_Success_PostsToHostRootWithoutBody()
	{
		// Arrange
		HttpMethod? method = null;
		string? requestUri = null;
		HttpContent? requestContent = null;
		await using var client = MockHttpMessageHandler.CreateClient(request =>
		{
			method = request.Method;
			requestUri = request.RequestUri!.AbsoluteUri;
			requestContent = request.Content;
			return new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent("{\"status\":\"Rebooting\"}")
			};
		});

		// Act
		await client.RebootEspPixelStickAsync(TestContext.Current.CancellationToken);

		// Assert
		Assert.Equal(HttpMethod.Post, method);
		Assert.Equal("http://fpp.test/X6", requestUri);
		Assert.Null(requestContent);
	}

	[Fact]
	public async Task RebootEspPixelStickAsync_HttpError_ThrowsFppClientException()
	{
		// Arrange
		await using var client = MockHttpMessageHandler.CreateClient(_ =>
			new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));

		// Act
		var ex = await Assert.ThrowsAsync<FppClientException>(() =>
			client.RebootEspPixelStickAsync(TestContext.Current.CancellationToken));

		// Assert
		Assert.Equal(503, ex.HttpStatusCode);
	}

	[Fact]
	public async Task RebootEspPixelStickAsync_UsesRequestTimeout()
	{
		// Arrange
		var options = new VixenModules.App.FPPClient.Client.FppClientOptions
		{
			BaseUrl = "http://fpp.test/",
			Timeout = TimeSpan.FromMilliseconds(20),
			UploadTimeout = TimeSpan.FromSeconds(2)
		};
		await using var client = MockHttpMessageHandler.CreateClient(async (_, token) =>
		{
			await Task.Delay(Timeout.InfiniteTimeSpan, token);
			return new HttpResponseMessage(HttpStatusCode.OK);
		}, options);

		// Act & Assert
		await Assert.ThrowsAnyAsync<OperationCanceledException>(
			() => client.RebootEspPixelStickAsync(TestContext.Current.CancellationToken));
	}

	[Theory]
	[InlineData("")]
	[InlineData("not json")]
	[InlineData("{}")]
	[InlineData("{\"status\":\"Rebooted\"}")]
	[InlineData("{\"status\":\"rebooting\"}")]
	[InlineData("{\"status\":null}")]
	[InlineData("{\"status\":true}")]
	public async Task RebootEspPixelStickAsync_InvalidAcknowledgement_ThrowsFppClientException(string responseBody)
	{
		// Arrange
		await using var client = MockHttpMessageHandler.CreateClient(_ =>
			new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(responseBody) });

		// Act & Assert
		await Assert.ThrowsAsync<FppClientException>(
			() => client.RebootEspPixelStickAsync(TestContext.Current.CancellationToken));
	}

	[Fact]
	public async Task RebootEspPixelStickAsync_Cancelled_PropagatesCancellation()
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
			client.RebootEspPixelStickAsync(cts.Token));
	}
}
