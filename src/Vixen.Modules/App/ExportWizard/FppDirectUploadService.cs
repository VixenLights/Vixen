using NLog;
using Vixen.Export;
using VixenModules.App.FPPClient.Client;

namespace VixenModules.App.ExportWizard;

/// <summary>
/// Encapsulates the FPP direct-upload operations for sequences, audio, and universe files.
/// Receives <see cref="IFppClient"/> via its primary constructor so that the upload
/// operations can be exercised in unit tests with a mocked client.
/// </summary>
internal sealed class FppDirectUploadService(IFppClient client, bool isEspPixelStick = false, bool supportsZipArchives = false)
{
	private static readonly Logger Log = LogManager.GetCurrentClassLogger();

	internal bool IsEspPixelStick => isEspPixelStick;
	internal bool SupportsZipArchives => isEspPixelStick && supportsZipArchives;
	internal bool SupportsFppExtras => !isEspPixelStick;

	internal static async Task<FppDirectUploadService> DetectAsync(IFppClient client, CancellationToken ct = default)
	{
		ArgumentNullException.ThrowIfNull(client);
		var info = await client.GetSystemInfoAsync(ct).ConfigureAwait(false);
		if (info == null)
		{
			throw new InvalidOperationException("The device did not provide valid system information.");
		}

		var isEspPixelStick = string.Equals(info.Platform, "ESPixelStick", StringComparison.Ordinal);
		return new FppDirectUploadService(client, isEspPixelStick, isEspPixelStick && info.Zip);
	}

	/// <summary>Uploads an already-written FSEQ file to the detected target's sequence storage.</summary>
	/// <param name="tempPath">Full path to the local temp file containing the fseq data.</param>
	/// <param name="fseqFileName">The destination filename on the device (e.g. <c>"MyShow.fseq"</c>).</param>
	/// <param name="progress">Optional progress sink; receives a task-level status message before and after the upload.</param>
	/// <param name="ct">Optional cancellation token.</param>
	internal async Task UploadSequenceFileAsync(
		string tempPath, string fseqFileName,
		IProgress<ExportProgressStatus> progress = null, CancellationToken ct = default)
	{
		Log.Debug("Uploading sequence '{FileName}' from '{TempPath}'", fseqFileName, tempPath);
		progress?.Report(new ExportProgressStatus(ExportProgressStatus.ProgressType.Task)
		{
			TaskProgressValue = 0,
			TaskProgressMessage = $"Uploading {fseqFileName}"
		});
		await using var stream = File.OpenRead(tempPath);
		if (isEspPixelStick)
		{
			await client.UploadEspPixelStickSequenceAsync(fseqFileName, stream, ct).ConfigureAwait(false);
		}
		else
		{
			await client.UploadSequenceAsync(fseqFileName, stream, ct).ConfigureAwait(false);
		}
		progress?.Report(new ExportProgressStatus(ExportProgressStatus.ProgressType.Task)
		{
			TaskProgressValue = 100,
			TaskProgressMessage = $"Uploaded {fseqFileName}"
		});
	}

	/// <summary>Uploads an archive file to a ZIP-capable ESPixelStick.</summary>
	/// <param name="tempPath">Full path to the local ZIP archive.</param>
	/// <param name="archiveFileName">The destination archive filename on the device.</param>
	/// <param name="ct">Optional cancellation token.</param>
	internal async Task UploadArchiveFileAsync(string tempPath, string archiveFileName, CancellationToken ct = default)
	{
		if (!SupportsZipArchives)
		{
			throw new InvalidOperationException("The detected device does not support ZIP archives.");
		}

		await using var stream = File.OpenRead(tempPath);
		await client.UploadEspPixelStickArchiveAsync(archiveFileName, stream, ct).ConfigureAwait(false);
	}

	/// <summary>Requests an ESPixelStick reboot.</summary>
	/// <param name="ct">Optional cancellation token.</param>
	internal async Task RebootEspPixelStickAsync(CancellationToken ct = default)
	{
		await client.RebootEspPixelStickAsync(ct).ConfigureAwait(false);
	}

	/// <summary>Uploads an audio file to the FPP music directory.</summary>
	/// <param name="sourcePath">Full path to the source audio file on the local machine.</param>
	/// <param name="destFileName">The destination filename on the FPP device.</param>
	/// <param name="progress">Optional progress sink; receives a task-level status message before and after the upload.</param>
	/// <param name="ct">Optional cancellation token.</param>
	internal async Task UploadAudioFileAsync(
		string sourcePath, string destFileName,
		IProgress<ExportProgressStatus> progress = null, CancellationToken ct = default)
	{
		Log.Debug("Uploading audio '{FileName}' from '{SourcePath}'", destFileName, sourcePath);
		progress?.Report(new ExportProgressStatus(ExportProgressStatus.ProgressType.Task)
		{
			TaskProgressValue = 0,
			TaskProgressMessage = $"Uploading {destFileName}"
		});
		await using var stream = File.OpenRead(sourcePath);
		await client.UploadMusicAsync(destFileName, stream, ct).ConfigureAwait(false);
		progress?.Report(new ExportProgressStatus(ExportProgressStatus.ProgressType.Task)
		{
			TaskProgressValue = 100,
			TaskProgressMessage = $"Uploaded {destFileName}"
		});
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

	/// <summary>
	/// Restarts the FPPD daemon
	/// </summary>
	internal async Task RestartFppdAsync(bool quick = false, CancellationToken ct = default)
	{
		Log.Debug("Restarting FPPD...");
		await client.RestartFppdAsync(quick, ct).ConfigureAwait(false);
	}
}
