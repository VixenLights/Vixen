using System.IO.Compression;
using Vixen.Export;
using VixenModules.App.ExportWizard;
using Xunit;

namespace Vixen.Tests.ExportWizard;

public class EspPixelStickZipBatchUploaderTests
{
	[Theory]
	[InlineData(1, 1)]
	[InlineData(5, 1)]
	[InlineData(6, 2)]
	[InlineData(10, 2)]
	[InlineData(11, 3)]
	public async Task UploadAsync_PreservesNamesOrderAndBytesAndRebootsOnce(int sequenceCount, int expectedArchives)
	{
		var cancellationToken = TestContext.Current.CancellationToken;
		var tempDirectory = CreateTempDirectory();
		var sequences = await CreateSequenceFilesAsync(tempDirectory, sequenceCount, cancellationToken);
		var uploadedArchives = new List<(string Name, string[] Entries)>();
		var archivePaths = new List<string>();
		var taskEvents = new List<string>();
		var overallValues = new List<int>();
		var progress = new RecordingProgress(status =>
		{
			if (status.StatusType == ExportProgressStatus.ProgressType.Overall)
			{
				overallValues.Add(status.OverallProgressValue);
			}
			else if (status.TaskProgressValue == 0 || status.TaskProgressValue == 100)
			{
				taskEvents.Add($"{status.TaskProgressValue}:{status.TaskProgressMessage}");
			}
		});
		var phaseCount = 0;
		var rebootCount = 0;

		try
		{
			await EspPixelStickZipBatchUploader.UploadAsync(sequences,
				(path, archiveName, _) =>
				{
					Assert.True(File.Exists(path));
					archivePaths.Add(path);
					using var archive = ZipFile.OpenRead(path);
					var entries = archive.Entries.Select(entry => entry.FullName).ToArray();
					for (var index = 0; index < entries.Length; index++)
					{
						using var entry = archive.Entries[index].Open();
						Assert.Equal([(byte)(uploadedArchives.Sum(batch => batch.Entries.Length) + index)], ReadAllBytes(entry));
					}
					uploadedArchives.Add((archiveName, entries));
					return Task.CompletedTask;
				}, _ =>
				{
					Assert.Equal(expectedArchives, uploadedArchives.Count);
					Assert.All(uploadedArchives, item => Assert.EndsWith(".zip", item.Name));
					rebootCount++;
					return Task.CompletedTask;
				}, progress, () => phaseCount++, cancellationToken, overallBaseSteps: 0,
				overallTotalSteps: expectedArchives * 2 + 1);

			Assert.Equal(expectedArchives, uploadedArchives.Count);
			Assert.Equal(1, rebootCount);
			Assert.Equal(expectedArchives * 2 + 1, phaseCount);
			Assert.Equal(expectedArchives * 2 + 1, overallValues.Count);
			Assert.Equal(overallValues.Order(), overallValues);
			Assert.All(overallValues.Take(overallValues.Count - 1), value => Assert.True(value < 100));
			Assert.Equal(100, overallValues[^1]);
			var expectedTaskEvents = Enumerable.Range(1, expectedArchives)
				.SelectMany(part => new[]
				{
					$"0:Creating ZIP {part}/{expectedArchives}",
					$"100:Created ZIP {part}/{expectedArchives}",
					$"0:Uploading ZIP {part}/{expectedArchives}",
					$"100:Uploaded ZIP {part}/{expectedArchives}"
				})
				.Concat(["0:Requesting ESPixelStick reboot", "100:ESPixelStick reboot acknowledged"]);
			Assert.Equal(expectedTaskEvents, taskEvents);
			Assert.Equal(sequenceCount, uploadedArchives.Sum(item => item.Entries.Length));
			Assert.Equal(sequences.Select(item => item.FileName), uploadedArchives.SelectMany(item => item.Entries));
			Assert.Equal(expectedArchives, uploadedArchives.Select(item => item.Name).Distinct(StringComparer.Ordinal).Count());
			Assert.Contains(taskEvents, item => item == "0:Requesting ESPixelStick reboot");
			Assert.Contains(taskEvents, item => item == "100:ESPixelStick reboot acknowledged");
			Assert.All(archivePaths, path => Assert.False(File.Exists(path)));
		}
		finally
		{
			Directory.Delete(tempDirectory, recursive: true);
		}
	}

	[Fact]
	public async Task UploadAsync_DuplicateNamesFailsBeforeAnyUpload()
	{
		var cancellationToken = TestContext.Current.CancellationToken;
		var tempDirectory = CreateTempDirectory();
		var firstPath = Path.Combine(tempDirectory, "first.fseq");
		var secondPath = Path.Combine(tempDirectory, "second.fseq");
		await File.WriteAllBytesAsync(firstPath, [1], cancellationToken);
		await File.WriteAllBytesAsync(secondPath, [2], cancellationToken);
		var uploadCount = 0;
		var rebootCount = 0;

		try
		{
			await Assert.ThrowsAsync<ArgumentException>(() => EspPixelStickZipBatchUploader.UploadAsync(
				[
					new EspPixelStickSequenceFile(firstPath, "same.fseq"),
					new EspPixelStickSequenceFile(secondPath, "SAME.FSEQ")
				], (_, _, _) => { uploadCount++; return Task.CompletedTask; },
				_ => { rebootCount++; return Task.CompletedTask; }, null, () => { }, cancellationToken));

			Assert.Equal(0, uploadCount);
			Assert.Equal(0, rebootCount);
		}
		finally
		{
			Directory.Delete(tempDirectory, recursive: true);
		}
	}

	[Fact]
	public async Task UploadAsync_LaterBatchFailureStopsWithoutRebootAndCleansArchive()
	{
		var cancellationToken = TestContext.Current.CancellationToken;
		var tempDirectory = CreateTempDirectory();
		var sequences = await CreateSequenceFilesAsync(tempDirectory, 6, cancellationToken);
		var archivePaths = new List<string>();
		var uploadCount = 0;
		var rebootCount = 0;

		try
		{
			await Assert.ThrowsAsync<IOException>(() => EspPixelStickZipBatchUploader.UploadAsync(sequences,
				(path, _, _) =>
				{
					archivePaths.Add(path);
					if (++uploadCount == 2) throw new IOException("second batch rejected");
					return Task.CompletedTask;
				}, _ => { rebootCount++; return Task.CompletedTask; }, null, () => { }, cancellationToken));

			Assert.Equal(2, uploadCount);
			Assert.Equal(0, rebootCount);
			Assert.All(archivePaths, path => Assert.False(File.Exists(path)));
		}
		finally
		{
			Directory.Delete(tempDirectory, recursive: true);
		}
	}

	[Fact]
	public async Task UploadAsync_CancellationDoesNotRequestReboot()
	{
		var cancellationToken = TestContext.Current.CancellationToken;
		var tempDirectory = CreateTempDirectory();
		var sequences = await CreateSequenceFilesAsync(tempDirectory, 1, cancellationToken);
		using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		await cts.CancelAsync();
		var rebootCount = 0;

		try
		{
			await Assert.ThrowsAnyAsync<OperationCanceledException>(() => EspPixelStickZipBatchUploader.UploadAsync(
				sequences, (_, _, _) => Task.CompletedTask,
				_ => { rebootCount++; return Task.CompletedTask; }, null, () => { }, cts.Token));
			Assert.Equal(0, rebootCount);
		}
		finally
		{
			Directory.Delete(tempDirectory, recursive: true);
		}
	}

	[Fact]
	public async Task UploadAsync_EmptySetDoesNotUploadOrReboot()
	{
		var uploadCount = 0;
		var rebootCount = 0;
		await EspPixelStickZipBatchUploader.UploadAsync([], (_, _, _) => { uploadCount++; return Task.CompletedTask; },
			_ => { rebootCount++; return Task.CompletedTask; }, null, () => { },
			TestContext.Current.CancellationToken);
		Assert.Equal(0, uploadCount);
		Assert.Equal(0, rebootCount);
	}

	private static byte[] ReadAllBytes(Stream stream)
	{
		using var output = new MemoryStream();
		stream.CopyTo(output);
		return output.ToArray();
	}

	private static async Task<List<EspPixelStickSequenceFile>> CreateSequenceFilesAsync(
		string directory, int count, CancellationToken cancellationToken)
	{
		var files = new List<EspPixelStickSequenceFile>();
		for (var index = 0; index < count; index++)
		{
			var fileName = $"sequence-{index + 1:00}.fseq";
			var path = Path.Combine(directory, fileName);
			await File.WriteAllBytesAsync(path, [(byte)index], cancellationToken);
			files.Add(new EspPixelStickSequenceFile(path, fileName));
		}
		return files;
	}

	private static string CreateTempDirectory()
	{
		var directory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
		Directory.CreateDirectory(directory);
		return directory;
	}

	private sealed class RecordingProgress(Action<ExportProgressStatus> report) : IProgress<ExportProgressStatus>
	{
		public void Report(ExportProgressStatus value) => report(value);
	}
}
