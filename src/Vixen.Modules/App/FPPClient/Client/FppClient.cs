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
		_httpClient.Timeout = Timeout.InfiniteTimeSpan;
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
	public Task UploadEspPixelStickSequenceAsync(
		string filename, Stream content, CancellationToken cancellationToken = default) =>
		UploadEspPixelStickFileAsync(filename, content, "sequence", cancellationToken);

	/// <inheritdoc/>
	public Task UploadEspPixelStickArchiveAsync(
		string filename, Stream content, CancellationToken cancellationToken = default) =>
		UploadEspPixelStickFileAsync(filename, content, "archive", cancellationToken);

	/// <inheritdoc/>
	public async Task RebootEspPixelStickAsync(CancellationToken cancellationToken = default)
	{
		const string url = "/X6";
		Log.Debug("Requesting ESPixelStick reboot");

		using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		cts.CancelAfter(_options.Timeout);

		using var response = await _httpClient.PostAsync(url, content: null, cts.Token)
			.ConfigureAwait(false);
		if (!response.IsSuccessStatusCode)
		{
			Log.Error("ESPixelStick reboot request failed with HTTP {StatusCode}", (int)response.StatusCode);
			throw new FppClientException(
				$"ESPixelStick reboot request failed with HTTP {(int)response.StatusCode}.",
				(int)response.StatusCode);
		}

		try
		{
			var body = await response.Content.ReadAsStringAsync(cts.Token).ConfigureAwait(false);
			using var acknowledgement = JsonDocument.Parse(body);
			if (acknowledgement.RootElement.ValueKind != JsonValueKind.Object ||
				!acknowledgement.RootElement.TryGetProperty("status", out var status) ||
				status.ValueKind != JsonValueKind.String ||
				!string.Equals(status.GetString(), "Rebooting", StringComparison.Ordinal))
			{
				Log.Error("ESPixelStick reboot request returned an unexpected acknowledgement");
				throw new FppClientException("ESPixelStick reboot request returned an unexpected acknowledgement.");
			}
		}
		catch (JsonException ex)
		{
			Log.Error(ex, "ESPixelStick reboot request returned invalid JSON");
			throw new FppClientException("ESPixelStick reboot request returned invalid JSON.", ex);
		}
	}

	private async Task UploadEspPixelStickFileAsync(
		string filename, Stream content, string fileType, CancellationToken cancellationToken)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filename);
		ArgumentNullException.ThrowIfNull(content);
		if (filename.Contains('/') || filename.Contains('\\'))
		{
			throw new ArgumentException("The filename must not contain path separators.", nameof(filename));
		}

		if (!content.CanRead)
		{
			throw new ArgumentException("The content stream must be readable.", nameof(content));
		}

		var url = $"/fpp?path=uploadFile&filename={Uri.EscapeDataString(filename)}";
		Log.Debug("Uploading ESPixelStick {FileType} {Filename}", fileType, filename);

		using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		cts.CancelAfter(_options.UploadTimeout);

		using var streamContent = new StreamContent(content);
		streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
		using var response = await _httpClient.PostAsync(url, streamContent, cts.Token).ConfigureAwait(false);

		if (!response.IsSuccessStatusCode)
		{
			Log.Error("ESPixelStick {FileType} upload of {Filename} failed with HTTP {StatusCode}",
				fileType, filename, (int)response.StatusCode);
			throw new FppClientException(
				$"ESPixelStick {fileType} upload of '{filename}' failed with HTTP {(int)response.StatusCode}.",
				(int)response.StatusCode);
		}

		try
		{
			await using var responseStream = await response.Content.ReadAsStreamAsync(cts.Token).ConfigureAwait(false);
			using var metadata = await JsonDocument.ParseAsync(responseStream, cancellationToken: cts.Token)
				.ConfigureAwait(false);
			if (metadata.RootElement.ValueKind != JsonValueKind.Object ||
				!metadata.RootElement.TryGetProperty("Name", out var name) ||
				name.ValueKind != JsonValueKind.String ||
				!string.Equals(name.GetString(), filename, StringComparison.Ordinal))
			{
				Log.Error("ESPixelStick {FileType} upload of {Filename} returned unexpected metadata",
					fileType, filename);
				throw new FppClientException(
					$"ESPixelStick {fileType} upload of '{filename}' returned unexpected file metadata.");
			}
		}
		catch (JsonException ex)
		{
			Log.Error(ex, "ESPixelStick {FileType} upload of {Filename} returned invalid JSON",
				fileType, filename);
			throw new FppClientException(
				$"ESPixelStick {fileType} upload of '{filename}' returned invalid JSON.", ex);
		}
	}

	/// <inheritdoc/>
	public Task UploadMusicAsync(string filename, Stream content, CancellationToken cancellationToken = default) =>
		UploadFileAsync("music", filename, content, cancellationToken);

	/// <inheritdoc/>
	public Task UploadVideoAsync(string filename, Stream content, CancellationToken cancellationToken = default) =>
		UploadFileAsync("videos", filename, content, cancellationToken);

	/// <inheritdoc/>
	public async Task RenameFileAsync(
		string dirName, string source, string dest, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(dirName);
		ArgumentException.ThrowIfNullOrWhiteSpace(source);
		ArgumentException.ThrowIfNullOrWhiteSpace(dest);

		var url = $"file/{Uri.EscapeDataString(dirName)}/rename"
		        + $"/{Uri.EscapeDataString(source)}/{Uri.EscapeDataString(dest)}";

		Log.Debug("Renaming {Source} to {Dest} in {DirName}", source, dest, dirName);

		using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		cts.CancelAfter(_options.Timeout);

		using var response = await _httpClient.PostAsync(url, content: null, cts.Token)
			.ConfigureAwait(false);

		if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
		{
			Log.Debug("Rename of {Source} in {DirName}: file not found (404), treating as no-op",
				source, dirName);
			return;
		}

		if (!response.IsSuccessStatusCode)
		{
			Log.Error("Rename of {Source} in {DirName} failed with HTTP {StatusCode}",
				source, dirName, (int)response.StatusCode);
			throw new FppClientException(
				$"Rename of '{source}' in '{dirName}' failed with HTTP {(int)response.StatusCode}.",
				(int)response.StatusCode);
		}
	}

	/// <inheritdoc/>
	public async Task RestartFppdAsync(bool quick = false, CancellationToken cancellationToken = default)
	{
		var url = quick ? "system/fppd/restart?quick=1" : "system/fppd/restart";

		Log.Debug("Restarting FPPD (quick={Quick})", quick);

		using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		cts.CancelAfter(_options.Timeout);

		using var response = await _httpClient.GetAsync(url, cts.Token)
			.ConfigureAwait(false);

		if (!response.IsSuccessStatusCode)
		{
			Log.Error("FPPD restart failed with HTTP {StatusCode}", (int)response.StatusCode);
			throw new FppClientException(
				$"FPPD restart failed with HTTP {(int)response.StatusCode}.",
				(int)response.StatusCode);
		}
	}

	/// <inheritdoc/>
	public ValueTask DisposeAsync()
	{
		_httpClient.Dispose();
		return ValueTask.CompletedTask;
	}

	private async Task<T> SendGetAsync<T>(string relativeUrl, CancellationToken ct)
	{
		Log.Debug("GET {RelativeUrl}", relativeUrl);

		using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
		cts.CancelAfter(_options.Timeout);

		using var response = await _httpClient.GetAsync(relativeUrl, cts.Token).ConfigureAwait(false);

		if (!response.IsSuccessStatusCode)
		{
			Log.Error("GET {RelativeUrl} failed with HTTP {StatusCode}", relativeUrl, (int)response.StatusCode);
			throw new FppClientException(
				$"Request to '{relativeUrl}' failed with HTTP {(int)response.StatusCode}.",
				(int)response.StatusCode);
		}

		var body = await response.Content.ReadAsStringAsync(cts.Token).ConfigureAwait(false);
		return JsonSerializer.Deserialize<T>(body, JsonOptions)!;
	}
}
