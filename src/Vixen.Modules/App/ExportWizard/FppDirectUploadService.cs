using NLog;
using VixenModules.App.FPPClient.Client;

namespace VixenModules.App.ExportWizard;

/// <summary>
/// Encapsulates the FPP direct-upload operations for sequences, audio, and universe files.
/// Receives <see cref="IFppClient"/> via its primary constructor so that the upload
/// operations can be exercised in unit tests with a mocked client.
/// </summary>
internal sealed class FppDirectUploadService(IFppClient client)
{
	private static readonly Logger Log = LogManager.GetCurrentClassLogger();

	/// <summary>Uploads an already-written fseq file to the FPP sequences directory.</summary>
	/// <param name="tempPath">Full path to the local temp file containing the fseq data.</param>
	/// <param name="fseqFileName">The destination filename on the FPP device (e.g. <c>"MyShow.fseq"</c>).</param>
	/// <param name="ct">Optional cancellation token.</param>
	internal async Task UploadSequenceFileAsync(
		string tempPath, string fseqFileName, CancellationToken ct = default)
	{
		Log.Debug("Uploading sequence '{FileName}' from '{TempPath}'", fseqFileName, tempPath);
		await using var stream = File.OpenRead(tempPath);
		await client.UploadSequenceAsync(fseqFileName, stream, ct).ConfigureAwait(false);
	}

	/// <summary>Uploads an audio file to the FPP music directory.</summary>
	/// <param name="sourcePath">Full path to the source audio file on the local machine.</param>
	/// <param name="destFileName">The destination filename on the FPP device.</param>
	/// <param name="ct">Optional cancellation token.</param>
	internal async Task UploadAudioFileAsync(
		string sourcePath, string destFileName, CancellationToken ct = default)
	{
		Log.Debug("Uploading audio '{FileName}' from '{SourcePath}'", destFileName, sourcePath);
		await using var stream = File.OpenRead(sourcePath);
		await client.UploadMusicAsync(destFileName, stream, ct).ConfigureAwait(false);
	}

	/// <summary>
	/// Renames the existing <c>co-universes.json</c> on the FPP device to a timestamped backup name.
	/// Silently no-ops if the file does not yet exist on the device.
	/// </summary>
	/// <param name="backupName">The target backup filename (e.g. <c>"co-universes.json_1012025-101530"</c>).</param>
	/// <param name="ct">Optional cancellation token.</param>
	internal async Task BackupUniverseFileAsync(string backupName, CancellationToken ct = default)
	{
		Log.Debug("Backing up co-universes.json as '{BackupName}'", backupName);
		await client.RenameFileAsync("config", "co-universes.json", backupName, ct)
			.ConfigureAwait(false);
	}

	/// <summary>Uploads a universe JSON file to the FPP config directory as <c>co-universes.json</c>.</summary>
	/// <param name="tempPath">Full path to the local temp file containing the universe JSON.</param>
	/// <param name="ct">Optional cancellation token.</param>
	internal async Task UploadUniverseFileAsync(string tempPath, CancellationToken ct = default)
	{
		Log.Debug("Uploading universe file from '{TempPath}'", tempPath);
		await using var stream = File.OpenRead(tempPath);
		await client.UploadFileAsync("config", "co-universes.json", stream, ct).ConfigureAwait(false);
	}
}
