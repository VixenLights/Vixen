using System.IO.Compression;
using Vixen.Export;

namespace VixenModules.App.ExportWizard;

/// <summary>Creates ordered ESPixelStick ZIP batches and uploads them before requesting one reboot.</summary>
internal static class EspPixelStickZipBatchUploader
{
	internal const int BatchSize = 5;

	internal static async Task UploadAsync(
		IReadOnlyList<EspPixelStickSequenceFile> sequences,
		Func<string, string, CancellationToken, Task> uploadArchiveAsync,
		Func<CancellationToken, Task> rebootAsync,
		IProgress<ExportProgressStatus> progress,
		Action phaseCompleted,
		CancellationToken cancellationToken,
		int overallBaseSteps = 0,
		int overallTotalSteps = 0)
	{
		ArgumentNullException.ThrowIfNull(sequences);
		ArgumentNullException.ThrowIfNull(uploadArchiveAsync);
		ArgumentNullException.ThrowIfNull(rebootAsync);
		ArgumentNullException.ThrowIfNull(phaseCompleted);
		ValidateOverallProgress(overallBaseSteps, overallTotalSteps);
		ValidateSequenceFiles(sequences);
		if (sequences.Count == 0)
		{
			return;
		}

		var completedSteps = overallBaseSteps;
		void CompletePhase()
		{
			phaseCompleted();
			if (overallTotalSteps == 0) return;
			completedSteps++;
			ReportOverall(progress, completedSteps, overallTotalSteps);
		}

		var totalArchives = (sequences.Count + BatchSize - 1) / BatchSize;
		for (var offset = 0; offset < sequences.Count; offset += BatchSize)
		{
			var batch = sequences.Skip(offset).Take(BatchSize).ToArray();
			await UploadBatchAsync(batch, offset / BatchSize + 1, totalArchives,
				uploadArchiveAsync, progress, CompletePhase, cancellationToken).ConfigureAwait(false);
		}

		await RequestRebootAsync(rebootAsync, progress, CompletePhase, cancellationToken).ConfigureAwait(false);
	}

	internal static async Task UploadBatchAsync(
		IReadOnlyList<EspPixelStickSequenceFile> batch,
		int part,
		int totalArchives,
		Func<string, string, CancellationToken, Task> uploadArchiveAsync,
		IProgress<ExportProgressStatus> progress,
		Action phaseCompleted,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(batch);
		ArgumentNullException.ThrowIfNull(uploadArchiveAsync);
		ArgumentNullException.ThrowIfNull(phaseCompleted);
		if (batch.Count is < 1 or > BatchSize)
		{
			throw new ArgumentOutOfRangeException(nameof(batch), $"A ZIP batch must contain between one and {BatchSize} FSEQ files.");
		}
		if (part < 1 || totalArchives < part)
		{
			throw new ArgumentOutOfRangeException(nameof(part));
		}

		ValidateSequenceFiles(batch);
		var archiveName = $"vixen-export-{Guid.NewGuid():N}-part-{part:000}.zip";
		var archivePath = Path.Combine(Path.GetTempPath(), archiveName);
		try
		{
			await CreateArchiveAsync(archivePath, batch, part, totalArchives, progress, cancellationToken)
				.ConfigureAwait(false);
			phaseCompleted();

			ReportTask(progress, 0, $"Uploading ZIP {part}/{totalArchives}");
			await uploadArchiveAsync(archivePath, archiveName, cancellationToken).ConfigureAwait(false);
			ReportTask(progress, 100, $"Uploaded ZIP {part}/{totalArchives}");
			phaseCompleted();
		}
		finally
		{
			if (File.Exists(archivePath))
			{
				File.Delete(archivePath);
			}
		}
	}

	internal static async Task RequestRebootAsync(
		Func<CancellationToken, Task> rebootAsync,
		IProgress<ExportProgressStatus> progress,
		Action phaseCompleted,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(rebootAsync);
		ArgumentNullException.ThrowIfNull(phaseCompleted);
		cancellationToken.ThrowIfCancellationRequested();
		ReportTask(progress, 0, "Requesting ESPixelStick reboot");
		await rebootAsync(cancellationToken).ConfigureAwait(false);
		ReportTask(progress, 100, "ESPixelStick reboot acknowledged");
		phaseCompleted();
	}

	private static void ValidateOverallProgress(int overallBaseSteps, int overallTotalSteps)
	{
		if (overallBaseSteps < 0 || overallTotalSteps < 0 || overallBaseSteps > overallTotalSteps)
		{
			throw new ArgumentOutOfRangeException(nameof(overallBaseSteps));
		}
	}

	private static void ValidateSequenceFiles(IReadOnlyList<EspPixelStickSequenceFile> sequences)
	{
		var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (var sequence in sequences)
		{
			ArgumentNullException.ThrowIfNull(sequence);
			if (string.IsNullOrWhiteSpace(sequence.Path) || string.IsNullOrWhiteSpace(sequence.FileName))
			{
				throw new ArgumentException("Each ESPixelStick archive entry needs a source path and filename.", nameof(sequences));
			}

			if (!string.Equals(Path.GetFileName(sequence.FileName), sequence.FileName, StringComparison.Ordinal) ||
				!string.Equals(Path.GetExtension(sequence.FileName), ".fseq", StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException($"'{sequence.FileName}' is not a flat FSEQ filename.", nameof(sequences));
			}

			if (!names.Add(sequence.FileName))
			{
				throw new ArgumentException($"Duplicate FSEQ filename '{sequence.FileName}' cannot be archived.", nameof(sequences));
			}

			if (!File.Exists(sequence.Path))
			{
				throw new FileNotFoundException($"FSEQ file '{sequence.FileName}' is missing.", sequence.Path);
			}
		}
	}

	private static async Task CreateArchiveAsync(
		string archivePath,
		IReadOnlyList<EspPixelStickSequenceFile> batch,
		int part,
		int totalArchives,
		IProgress<ExportProgressStatus> progress,
		CancellationToken cancellationToken)
	{
		ReportTask(progress, 0, $"Creating ZIP {part}/{totalArchives}");
		await using (var archiveStream = new FileStream(archivePath, FileMode.CreateNew, FileAccess.ReadWrite,
			FileShare.None, 81920, useAsync: true))
		await using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, leaveOpen: true))
		{
			for (var index = 0; index < batch.Count; index++)
			{
				cancellationToken.ThrowIfCancellationRequested();
				var sequence = batch[index];
				var entry = archive.CreateEntry(sequence.FileName, CompressionLevel.Optimal);
				await using var source = new FileStream(sequence.Path, FileMode.Open, FileAccess.Read, FileShare.Read,
					81920, useAsync: true);
				await using var destination = await entry.OpenAsync(cancellationToken);
				await source.CopyToAsync(destination, 81920, cancellationToken).ConfigureAwait(false);
				var completedValue = Math.Min(99, (index + 1) * 100 / batch.Count);
				ReportTask(progress, completedValue, $"Creating ZIP {part}/{totalArchives}");
			}
		}

		ReportTask(progress, 100, $"Created ZIP {part}/{totalArchives}");
	}

	private static void ReportOverall(IProgress<ExportProgressStatus> progress, int completedSteps, int totalSteps)
	{
		progress?.Report(new ExportProgressStatus(ExportProgressStatus.ProgressType.Overall)
		{
			OverallProgressValue = (int)(completedSteps / (double)totalSteps * 100),
			OverallProgressMessage = "Overall Progress"
		});
	}

	private static void ReportTask(IProgress<ExportProgressStatus> progress, int value, string message)
	{
		progress?.Report(new ExportProgressStatus(ExportProgressStatus.ProgressType.Task)
		{
			TaskProgressValue = value,
			TaskProgressMessage = message
		});
	}
}
