using VixenModules.App.FPPClient.Models;

namespace VixenModules.App.FPPClient.Client;

/// <summary>
/// Default implementation of <see cref="IFppClient"/> that communicates with an FPP device over HTTP.
/// </summary>
internal sealed class FppClient : IFppClient
{
	private readonly HttpClient _httpClient;
	private readonly FppClientOptions _options;

	/// <summary>
	/// Initializes a new instance of the <see cref="FppClient"/> class.
	/// </summary>
	/// <param name="httpClient">The HTTP client used to send requests to the FPP device.</param>
	/// <param name="options">The connection options for the target FPP instance.</param>
	public FppClient(HttpClient httpClient, FppClientOptions options)
	{
		_httpClient = httpClient;
		_options = options;
	}

	/// <inheritdoc/>
	public Task<FppSystemInfo> GetSystemInfoAsync(CancellationToken cancellationToken = default) =>
		throw new NotImplementedException();

	/// <inheritdoc/>
	public Task<IReadOnlyList<string>> GetSequencesAsync(CancellationToken cancellationToken = default) =>
		throw new NotImplementedException();

	/// <inheritdoc/>
	public Task<IReadOnlyList<string>> GetPlaylistsAsync(CancellationToken cancellationToken = default) =>
		throw new NotImplementedException();

	/// <inheritdoc/>
	public Task<FppPlaylist> GetPlaylistAsync(string playlistName, CancellationToken cancellationToken = default) =>
		throw new NotImplementedException();

	/// <inheritdoc/>
	public Task<IReadOnlyList<string>> GetPlaylistSequencesAsync(string playlistName, CancellationToken cancellationToken = default) =>
		throw new NotImplementedException();

	/// <inheritdoc/>
	public Task UploadFileAsync(string dirName, string filename, Stream content, CancellationToken cancellationToken = default) =>
		throw new NotImplementedException();

	/// <inheritdoc/>
	public Task UploadSequenceAsync(string filename, Stream content, CancellationToken cancellationToken = default) =>
		throw new NotImplementedException();

	/// <inheritdoc/>
	public Task UploadMusicAsync(string filename, Stream content, CancellationToken cancellationToken = default) =>
		throw new NotImplementedException();

	/// <inheritdoc/>
	public Task UploadVideoAsync(string filename, Stream content, CancellationToken cancellationToken = default) =>
		throw new NotImplementedException();

	/// <inheritdoc/>
	public ValueTask DisposeAsync()
	{
		_httpClient.Dispose();
		return ValueTask.CompletedTask;
	}
}
