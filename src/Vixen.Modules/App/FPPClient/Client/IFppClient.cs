using VixenModules.App.FPPClient.Exceptions;
using VixenModules.App.FPPClient.Models;

namespace VixenModules.App.FPPClient.Client;

/// <summary>
/// Defines the contract for communicating with a Falcon Player (FPP) instance over its REST API.
/// </summary>
/// <remarks>
/// Obtain an instance via <see cref="IFppClientFactory.Create"/> or <see cref="FppClientFactory"/>.
/// The client holds an <c>HttpClient</c> internally and must be disposed when no longer needed.
/// </remarks>
public interface IFppClient : IAsyncDisposable
{
	/// <summary>
	/// Retrieves static information about the FPP host.
	/// </summary>
	/// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation.</param>
	/// <returns>An <see cref="FppSystemInfo"/> record describing the host's name, platform, version, and utilization.</returns>
	/// <exception cref="FppClientException">The HTTP response indicated a non-success status code.</exception>
	Task<FppSystemInfo> GetSystemInfoAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Retrieves the names of all FSEQ sequence files stored on the FPP instance.
	/// </summary>
	/// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation.</param>
	/// <returns>A read-only list of sequence names (without the <c>.fseq</c> extension).</returns>
	/// <exception cref="FppClientException">The HTTP response indicated a non-success status code.</exception>
	Task<IReadOnlyList<string>> GetSequencesAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Retrieves the names of all playlists stored on the FPP instance.
	/// </summary>
	/// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation.</param>
	/// <returns>A read-only list of playlist names.</returns>
	/// <exception cref="FppClientException">The HTTP response indicated a non-success status code.</exception>
	Task<IReadOnlyList<string>> GetPlaylistsAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Retrieves the full content of the named playlist.
	/// </summary>
	/// <param name="playlistName">The name of the playlist to retrieve.</param>
	/// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation.</param>
	/// <returns>An <see cref="FppPlaylist"/> describing the playlist's lead-in, main, and lead-out sections.</returns>
	/// <exception cref="FppClientException">The HTTP response indicated a non-success status code, including HTTP 404 if the playlist does not exist.</exception>
	Task<FppPlaylist> GetPlaylistAsync(string playlistName, CancellationToken cancellationToken = default);

	/// <summary>
	/// Retrieves the sequence names referenced by the named playlist.
	/// </summary>
	/// <param name="playlistName">The name of the playlist to inspect.</param>
	/// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation.</param>
	/// <returns>
	/// A read-only list of sequence names from entries whose type is <c>"sequence"</c> or <c>"both"</c>
	/// across the lead-in, main, and lead-out sections.
	/// </returns>
	/// <exception cref="FppClientException">The HTTP response indicated a non-success status code.</exception>
	Task<IReadOnlyList<string>> GetPlaylistSequencesAsync(string playlistName, CancellationToken cancellationToken = default);

	/// <summary>
	/// Uploads a file to the specified directory on the FPP instance.
	/// </summary>
	/// <param name="dirName">The target FPP media directory (e.g. <c>"sequences"</c>, <c>"music"</c>, <c>"videos"</c>).</param>
	/// <param name="filename">The destination filename on the FPP instance.</param>
	/// <param name="content">A stream containing the file bytes to upload.</param>
	/// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation.</param>
	/// <exception cref="ArgumentException"><paramref name="dirName"/> or <paramref name="filename"/> is <see langword="null"/> or whitespace.</exception>
	/// <exception cref="FppClientException">The HTTP response indicated a non-success status code.</exception>
	Task UploadFileAsync(string dirName, string filename, Stream content, CancellationToken cancellationToken = default);

	/// <summary>
	/// Uploads an FSEQ sequence file to the FPP instance.
	/// </summary>
	/// <param name="filename">The destination filename on the FPP instance.</param>
	/// <param name="content">A stream containing the FSEQ file bytes to upload.</param>
	/// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation.</param>
	/// <exception cref="FppClientException">The HTTP response indicated a non-success status code.</exception>
	Task UploadSequenceAsync(string filename, Stream content, CancellationToken cancellationToken = default);

	/// <summary>
	/// Uploads a music file to the FPP instance.
	/// </summary>
	/// <param name="filename">The destination filename on the FPP instance.</param>
	/// <param name="content">A stream containing the audio file bytes to upload.</param>
	/// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation.</param>
	/// <exception cref="FppClientException">The HTTP response indicated a non-success status code.</exception>
	Task UploadMusicAsync(string filename, Stream content, CancellationToken cancellationToken = default);

	/// <summary>
	/// Uploads a video file to the FPP instance.
	/// </summary>
	/// <param name="filename">The destination filename on the FPP instance.</param>
	/// <param name="content">A stream containing the video file bytes to upload.</param>
	/// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation.</param>
	/// <exception cref="FppClientException">The HTTP response indicated a non-success status code.</exception>
	Task UploadVideoAsync(string filename, Stream content, CancellationToken cancellationToken = default);

	/// <summary>
	/// Renames a file within a directory on the FPP instance.
	/// </summary>
	/// <param name="dirName">The FPP media directory containing the file (e.g. <c>"config"</c>).</param>
	/// <param name="source">The current filename (without path).</param>
	/// <param name="dest">The desired filename after rename (without path).</param>
	/// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation.</param>
	/// <returns>
	/// A <see cref="Task"/> that completes when the rename succeeds.
	/// If the file does not exist (HTTP 404), the method returns without error.
	/// </returns>
	/// <exception cref="ArgumentException">
	/// <paramref name="dirName"/>, <paramref name="source"/>, or <paramref name="dest"/> is
	/// <see langword="null"/> or whitespace.
	/// </exception>
	/// <exception cref="FppClientException">
	/// The HTTP response indicated a non-success status code other than 404.
	/// </exception>
	Task RenameFileAsync(string dirName, string source, string dest,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Restarts the FPPD daemon process on the FPP instance.
	/// </summary>
	/// <param name="quick">
	/// When <see langword="true"/>, passes <c>?quick=1</c> to the endpoint, which reloads
	/// some configuration without performing a full daemon restart.
	/// </param>
	/// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation.</param>
	/// <exception cref="FppClientException">The HTTP response indicated a non-success status code.</exception>
	Task RestartFppdAsync(bool quick = false, CancellationToken cancellationToken = default);
}
