using System.Net;
using Vixen.Tests.FPPClient.Helpers;
using VixenModules.App.FPPClient.Exceptions;
using Xunit;

namespace Vixen.Tests.FPPClient;

public class FppClientGetPlaylistsTests
{
	[Fact]
	public async Task GetPlaylistsAsync_Success_ReturnsNames()
	{
		// Arrange
		await using var client = MockHttpMessageHandler.CreateClient(_ =>
			new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent("""["HolidayShow","Idle30","TestPlaylist"]""")
			});

		// Act
		var playlists = await client.GetPlaylistsAsync(TestContext.Current.CancellationToken);

		// Assert
		Assert.Equal(3, playlists.Count);
		Assert.Contains("HolidayShow", playlists);
		Assert.Contains("Idle30", playlists);
	}

	[Fact]
	public async Task GetPlaylistsAsync_EmptyArray_ReturnsEmpty()
	{
		// Arrange
		await using var client = MockHttpMessageHandler.CreateClient(_ =>
			new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent("[]")
			});

		// Act
		var playlists = await client.GetPlaylistsAsync(TestContext.Current.CancellationToken);

		// Assert
		Assert.Empty(playlists);
	}

	[Fact]
	public async Task GetPlaylistsAsync_HttpError_ThrowsFppClientException()
	{
		// Arrange
		await using var client = MockHttpMessageHandler.CreateClient(_ =>
			new HttpResponseMessage(HttpStatusCode.InternalServerError));

		// Act & Assert
		var ex = await Assert.ThrowsAsync<FppClientException>(
			() => client.GetPlaylistsAsync(TestContext.Current.CancellationToken));
		Assert.Equal(500, ex.HttpStatusCode);
	}
}
