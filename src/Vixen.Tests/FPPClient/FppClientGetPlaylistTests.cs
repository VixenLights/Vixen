using System.Net;
using Vixen.Tests.FPPClient.Helpers;
using VixenModules.App.FPPClient.Exceptions;
using Xunit;

namespace Vixen.Tests.FPPClient;

public class FppClientGetPlaylistTests
{
	private const string FullPlaylistJson = """
		{
			"name": "HolidayShow",
			"leadIn": [
				{ "type": "pause", "enabled": 1, "playOnce": 0 }
			],
			"mainPlaylist": [
				{ "type": "sequence", "enabled": 1, "playOnce": 0, "sequenceName": "GreatestShow.fseq" }
			],
			"leadOut": [
				{ "type": "both", "enabled": 1, "playOnce": 0, "sequenceName": "Closing.fseq", "mediaName": "song.mp3" }
			],
			"playlistInfo": { "total_duration": 120.5, "total_items": 3 }
		}
		""";

	[Fact]
	public async Task GetPlaylistAsync_Success_MapsAllSections()
	{
		// Arrange
		await using var client = MockHttpMessageHandler.CreateClient(_ =>
			new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent(FullPlaylistJson)
			});

		// Act
		var playlist = await client.GetPlaylistAsync("HolidayShow", TestContext.Current.CancellationToken);

		// Assert
		Assert.Equal("HolidayShow", playlist.Name);
		Assert.Single(playlist.LeadIn);
		Assert.Equal("pause", playlist.LeadIn[0].Type);
		Assert.Single(playlist.MainPlaylist);
		Assert.Equal("sequence", playlist.MainPlaylist[0].Type);
		Assert.Equal("GreatestShow.fseq", playlist.MainPlaylist[0].SequenceName);
		Assert.Single(playlist.LeadOut);
		Assert.Equal("both", playlist.LeadOut[0].Type);
		Assert.NotNull(playlist.PlaylistInfo);
		Assert.Equal(120.5, playlist.PlaylistInfo!.TotalDuration);
		Assert.Equal(3, playlist.PlaylistInfo.TotalItems);
	}

	[Fact]
	public async Task GetPlaylistSequencesAsync_ReturnsOnlySequenceEntries()
	{
		// Arrange — playlist has a "sequence" entry, a "both" entry, and a "pause" entry
		const string json = """
			{
				"name": "Mixed",
				"leadIn": [],
				"mainPlaylist": [
					{ "type": "sequence", "enabled": 1, "playOnce": 0, "sequenceName": "Show1.fseq" },
					{ "type": "pause",    "enabled": 1, "playOnce": 0 },
					{ "type": "both",     "enabled": 1, "playOnce": 0, "sequenceName": "Show2.fseq", "mediaName": "bg.mp3" }
				],
				"leadOut": [],
				"playlistInfo": { "total_duration": 60, "total_items": 3 }
			}
			""";

		await using var client = MockHttpMessageHandler.CreateClient(_ =>
			new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent(json)
			});

		// Act
		var sequences = await client.GetPlaylistSequencesAsync("Mixed", TestContext.Current.CancellationToken);

		// Assert — only the "sequence" and "both" entries should be included
		Assert.Equal(2, sequences.Count);
		Assert.Contains("Show1.fseq", sequences);
		Assert.Contains("Show2.fseq", sequences);
	}

	[Fact]
	public async Task GetPlaylistSequencesAsync_EmptyPlaylist_ReturnsEmpty()
	{
		// Arrange
		const string json = """
			{
				"name": "Empty",
				"leadIn": [],
				"mainPlaylist": [],
				"leadOut": [],
				"playlistInfo": { "total_duration": 0, "total_items": 0 }
			}
			""";

		await using var client = MockHttpMessageHandler.CreateClient(_ =>
			new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new StringContent(json)
			});

		// Act
		var sequences = await client.GetPlaylistSequencesAsync("Empty", TestContext.Current.CancellationToken);

		// Assert
		Assert.Empty(sequences);
	}

	[Fact]
	public async Task GetPlaylistAsync_NotFound_ThrowsFppClientException()
	{
		// Arrange
		await using var client = MockHttpMessageHandler.CreateClient(_ =>
			new HttpResponseMessage(HttpStatusCode.NotFound));

		// Act & Assert
		var ex = await Assert.ThrowsAsync<FppClientException>(
			() => client.GetPlaylistAsync("NonExistent", TestContext.Current.CancellationToken));
		Assert.Equal(404, ex.HttpStatusCode);
	}
}
