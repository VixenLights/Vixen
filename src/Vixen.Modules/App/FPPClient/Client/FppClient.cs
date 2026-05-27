using System.Net.Http.Headers;
using System.Text.Json;
using NLog;
using VixenModules.App.FPPClient.Exceptions;
using VixenModules.App.FPPClient.Models;

namespace VixenModules.App.FPPClient.Client;

/// <summary>
/// Default implementation of <see cref="IFppClient"/> that communicates with an FPP device over HTTP.
/// </summary>
internal sealed class FppClient : IFppClient
{
	private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		PropertyNameCaseInsensitive = true,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
	};

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

		var baseUrl = options.BaseUrl.EndsWith('/') ? options.BaseUrl : options.BaseUrl + '/';
		_httpClient.BaseAddress = new Uri(baseUrl + "api/");
		_httpClient.Timeout = options.Timeout;
	}

	/// <inheritdoc/>
	public async Task<FppSystemInfo> GetSystemInfoAsync(CancellationToken cancellationToken = default) =>
		await SendGetAsync<FppSystemInfo>("system/info", cancellationToken).ConfigureAwait(false);

	/// <inheritdoc/>
	public async Task<IReadOnlyList<string>> GetSequencesAsync(CancellationToken cancellationToken = default)
	{
		var result = await SendGetAsync<string[]>("sequence", cancellationToken).ConfigureAwait(false);
		return result ?? [];
	}

	/// <inheritdoc/>
	public async Task<IReadOnlyList<string>> GetPlaylistsAsync(CancellationToken cancellationToken = default)
	{
		var result = await SendGetAsync<string[]>("playlists", cancellationToken).ConfigureAwait(false);
		return result ?? [];
	}

	/// <inheritdoc/>
	public async Task<FppPlaylist> GetPlaylistAsync(string playlistName, CancellationToken cancellationToken = default) =>
		await SendGetAsync<FppPlaylist>(
			$"playlist/{Uri.EscapeDataString(playlistName)}", cancellationToken).ConfigureAwait(false);

	/// <inheritdoc/>
	public async Task<IReadOnlyList<string>> GetPlaylistSequencesAsync(
		string playlistName, CancellationToken cancellationToken = default)
	{
		var playlist = await GetPlaylistAsync(playlistName, cancellationToken).ConfigureAwait(false);

		return playlist.LeadIn
			.Concat(playlist.MainPlaylist)
			.Concat(playlist.LeadOut)
			.Where(e => e.Type is "sequence" or "both" && !string.IsNullOrWhiteSpace(e.SequenceName))
			.Select(e => e.SequenceName!)
			.ToList();
	}

	/// <inheritdoc/>
	public async Task UploadFileAsync(
		string dirName, string filename, Stream content, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(dirName);
		ArgumentException.ThrowIfNullOrWhiteSpace(filename);

		var url = $"file/{Uri.EscapeDataString(dirName)}/{Uri.EscapeDataString(filename)}";

		Log.Debug("Uploading file {Filename} to directory {DirName}", filename, dirName);

		using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		cts.CancelAfter(_options.UploadTimeout);

		var streamContent = new StreamContent(content);
		streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

		using var response = await _httpClient.PostAsync(url, streamContent, cts.Token).ConfigureAwait(false);

		if (!response.IsSuccessStatusCode)
		{
			Log.Error("Upload of {Filename} to {DirName} failed with HTTP {StatusCode}",
				filename, dirName, (int)response.StatusCode);
			throw new FppClientException(
				$"Upload of '{filename}' to '{dirName}' failed with HTTP {(int)response.StatusCode}.",
				(int)response.StatusCode);
		}

		try
		{
			var body = await response.Content.ReadAsStringAsync(cts.Token).ConfigureAwait(false);
			var result = JsonSerializer.Deserialize<FppUploadResult>(body, JsonOptions);
			if (result is { Status: not "OK" and not "" })
			{
				Log.Warn("Upload of {Filename} returned non-OK status: {Status}", filename, result.Status);
			}
		}
		catch (JsonException ex)
		{
			Log.Warn(ex, "Could not parse upload response body for {Filename}; treating as success", filename);
		}
	}

	/// <inheritdoc/>
	public Task UploadSequenceAsync(string filename, Stream content, CancellationToken cancellationToken = default) =>
		UploadFileAsync("sequences", filename, content, cancellationToken);

	/// <inheritdoc/>
	public Task UploadMusicAsync(string filename, Stream content, CancellationToken cancellationToken = default) =>
		UploadFileAsync("music", filename, content, cancellationToken);

	/// <inheritdoc/>
	public Task UploadVideoAsync(string filename, Stream content, CancellationToken cancellationToken = default) =>
		UploadFileAsync("videos", filename, content, cancellationToken);

	/// <inheritdoc/>
	public ValueTask DisposeAsync()
	{
		_httpClient.Dispose();
		return ValueTask.CompletedTask;
	}

	private async Task<T> SendGetAsync<T>(string relativeUrl, CancellationToken ct)
	{
		Log.Debug("GET {RelativeUrl}", relativeUrl);

		using var response = await _httpClient.GetAsync(relativeUrl, ct).ConfigureAwait(false);

		if (!response.IsSuccessStatusCode)
		{
			Log.Error("GET {RelativeUrl} failed with HTTP {StatusCode}", relativeUrl, (int)response.StatusCode);
			throw new FppClientException(
				$"Request to '{relativeUrl}' failed with HTTP {(int)response.StatusCode}.",
				(int)response.StatusCode);
		}

		var body = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
		return JsonSerializer.Deserialize<T>(body, JsonOptions)!;
	}
}
