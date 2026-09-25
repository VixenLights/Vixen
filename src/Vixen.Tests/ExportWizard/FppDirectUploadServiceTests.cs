using Moq;
using Vixen.Export;
using VixenModules.App.ExportWizard;
using VixenModules.App.FPPClient.Client;
using VixenModules.App.FPPClient.Models;
using Xunit;

namespace Vixen.Tests.ExportWizard;

public class FppDirectUploadServiceTests
{
	[Fact]
	public async Task UploadSequenceFileAsync_CallsUploadSequenceAsync_WithCorrectFilename()
	{
		// Arrange
		var ct = TestContext.Current.CancellationToken;
		var mockClient = new Mock<IFppClient>();
		mockClient.Setup(c => c.UploadSequenceAsync(
				It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		var svc = new FppDirectUploadService(mockClient.Object);

		var tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".fseq");
		await File.WriteAllBytesAsync(tempPath, new byte[] { 1, 2, 3 }, ct);

		try
		{
			// Act
			await svc.UploadSequenceFileAsync(tempPath, "test.fseq", progress: null, ct);

			// Assert
			mockClient.Verify(c => c.UploadSequenceAsync(
				"test.fseq", It.IsAny<Stream>(), ct), Times.Once);
		}
		finally
		{
			if (File.Exists(tempPath)) File.Delete(tempPath);
		}
	}

	[Fact]
	public async Task DetectAsync_EspPixelStick_RoutesMultipleSequencesWithoutFppExtras()
	{
		var ct = TestContext.Current.CancellationToken;
		var mockClient = new Mock<IFppClient>();
		mockClient.Setup(c => c.GetSystemInfoAsync(ct))
			.ReturnsAsync(new FppSystemInfo { Platform = "ESPixelStick" });
		mockClient.Setup(c => c.UploadEspPixelStickSequenceAsync(
				It.IsAny<string>(), It.IsAny<Stream>(), ct)).Returns(Task.CompletedTask);
		var service = await FppDirectUploadService.DetectAsync(mockClient.Object, ct);
		var tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".fseq");
		await File.WriteAllBytesAsync(tempPath, [1, 2, 3], ct);

		try
		{
			await service.UploadSequenceFileAsync(tempPath, "first.fseq", ct: ct);
			await service.UploadSequenceFileAsync(tempPath, "second.fseq", ct: ct);

			Assert.True(service.IsEspPixelStick);
			Assert.False(service.SupportsFppExtras);
			Assert.False(service.SupportsZipArchives);
			mockClient.Verify(c => c.GetSystemInfoAsync(ct), Times.Once);
			mockClient.Verify(c => c.UploadEspPixelStickSequenceAsync(
					"first.fseq", It.IsAny<Stream>(), ct), Times.Once);
			mockClient.Verify(c => c.UploadEspPixelStickSequenceAsync(
					"second.fseq", It.IsAny<Stream>(), ct), Times.Once);
			mockClient.Verify(c => c.UploadSequenceAsync(
					It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Never);
			mockClient.Verify(c => c.UploadMusicAsync(
					It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Never);
			mockClient.Verify(c => c.RenameFileAsync(
					It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
			mockClient.Verify(c => c.UploadFileAsync(
					It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Never);
			mockClient.Verify(c => c.RestartFppdAsync(
					It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
			mockClient.Verify(c => c.UploadEspPixelStickArchiveAsync(
					It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Never);
			mockClient.Verify(c => c.RebootEspPixelStickAsync(
				It.IsAny<CancellationToken>()), Times.Never);
		}
		finally
		{
			if (File.Exists(tempPath)) File.Delete(tempPath);
		}
	}

	[Fact]
	public async Task DetectAsync_EspPixelStickZipFalse_UsesIndividualUploadWithoutReboot()
	{
		var ct = TestContext.Current.CancellationToken;
		var mockClient = new Mock<IFppClient>();
		mockClient.Setup(c => c.GetSystemInfoAsync(ct))
			.ReturnsAsync(new FppSystemInfo { Platform = "ESPixelStick", Zip = false });
		mockClient.Setup(c => c.UploadEspPixelStickSequenceAsync(
				It.IsAny<string>(), It.IsAny<Stream>(), ct)).Returns(Task.CompletedTask);
		mockClient.Setup(c => c.RebootEspPixelStickAsync(ct)).Returns(Task.CompletedTask);
		var service = await FppDirectUploadService.DetectAsync(mockClient.Object, ct);
		var sequencePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".fseq");
		await File.WriteAllBytesAsync(sequencePath, [1], ct);

		try
		{
			Assert.False(service.SupportsZipArchives);
			await service.UploadSequenceFileAsync(sequencePath, "sequence.fseq", ct: ct);
			await service.RebootEspPixelStickAsync(ct);
			mockClient.Verify(c => c.UploadEspPixelStickSequenceAsync(
				"sequence.fseq", It.IsAny<Stream>(), ct), Times.Once);
			mockClient.Verify(c => c.UploadEspPixelStickArchiveAsync(
				It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Never);
			mockClient.Verify(c => c.RebootEspPixelStickAsync(ct), Times.Once);
		}
		finally
		{
			if (File.Exists(sequencePath)) File.Delete(sequencePath);
		}
	}

	[Fact]
	public async Task DetectAsync_ZipCapableEspPixelStick_UploadsArchiveAndReboots()
	{
		var ct = TestContext.Current.CancellationToken;
		var mockClient = new Mock<IFppClient>();
		mockClient.Setup(c => c.GetSystemInfoAsync(ct))
			.ReturnsAsync(new FppSystemInfo { Platform = "ESPixelStick", Zip = true });
		mockClient.Setup(c => c.UploadEspPixelStickArchiveAsync(
				It.IsAny<string>(), It.IsAny<Stream>(), ct)).Returns(Task.CompletedTask);
		mockClient.Setup(c => c.RebootEspPixelStickAsync(ct)).Returns(Task.CompletedTask);
		var service = await FppDirectUploadService.DetectAsync(mockClient.Object, ct);
		var archivePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".zip");
		await File.WriteAllBytesAsync(archivePath, [1, 2, 3], ct);

		try
		{
			Assert.True(service.SupportsZipArchives);
			await service.UploadArchiveFileAsync(archivePath, "batch.zip", ct);
			await service.RebootEspPixelStickAsync(ct);
			mockClient.Verify(c => c.UploadEspPixelStickArchiveAsync(
				"batch.zip", It.IsAny<Stream>(), ct), Times.Once);
			mockClient.Verify(c => c.RebootEspPixelStickAsync(ct), Times.Once);
		}
		finally
		{
			if (File.Exists(archivePath)) File.Delete(archivePath);
		}
	}

	[Theory]
	[InlineData("Raspberry Pi")]
	[InlineData("")]
	public async Task DetectAsync_Fpp_KeepsExistingSequenceRoute(string platform)
	{
		var ct = TestContext.Current.CancellationToken;
		var mockClient = new Mock<IFppClient>();
		mockClient.Setup(c => c.GetSystemInfoAsync(ct))
			.ReturnsAsync(new FppSystemInfo { Platform = platform });
		mockClient.Setup(c => c.UploadSequenceAsync(
				It.IsAny<string>(), It.IsAny<Stream>(), ct)).Returns(Task.CompletedTask);
		var service = await FppDirectUploadService.DetectAsync(mockClient.Object, ct);
		var tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".fseq");
		await File.WriteAllBytesAsync(tempPath, [1, 2, 3], ct);

		try
		{
			await service.UploadSequenceFileAsync(tempPath, "test.fseq", ct: ct);

			Assert.False(service.IsEspPixelStick);
			Assert.True(service.SupportsFppExtras);
			Assert.False(service.SupportsZipArchives);
			mockClient.Verify(c => c.GetSystemInfoAsync(ct), Times.Once);
			mockClient.Verify(c => c.UploadSequenceAsync(
					"test.fseq", It.IsAny<Stream>(), ct), Times.Once);
			mockClient.Verify(c => c.UploadEspPixelStickSequenceAsync(
					It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Never);
			mockClient.Verify(c => c.UploadEspPixelStickArchiveAsync(
					It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Never);
			mockClient.Verify(c => c.RebootEspPixelStickAsync(
				It.IsAny<CancellationToken>()), Times.Never);
		}
		finally
		{
			if (File.Exists(tempPath)) File.Delete(tempPath);
		}
	}

	[Fact]
	public async Task DetectAsync_SystemInfoFailure_PreventsUploads()
	{
		var ct = TestContext.Current.CancellationToken;
		var mockClient = new Mock<IFppClient>();
		mockClient.Setup(c => c.GetSystemInfoAsync(ct))
			.ThrowsAsync(new InvalidOperationException("Device unavailable"));

		await Assert.ThrowsAsync<InvalidOperationException>(
			() => FppDirectUploadService.DetectAsync(mockClient.Object, ct));
		mockClient.Verify(c => c.UploadSequenceAsync(
			It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Never);
		mockClient.Verify(c => c.UploadEspPixelStickSequenceAsync(
			It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task UploadSequenceFileAsync_ReportsCompletionOnlyAfterSuccessfulUpload()
	{
		var ct = TestContext.Current.CancellationToken;
		var mockClient = new Mock<IFppClient>();
		mockClient.Setup(c => c.UploadEspPixelStickSequenceAsync(
			It.IsAny<string>(), It.IsAny<Stream>(), ct))
			.ThrowsAsync(new IOException("Upload failed"));
		var progressValues = new List<int>();
		var progress = new Mock<IProgress<ExportProgressStatus>>();
		progress.Setup(p => p.Report(It.IsAny<ExportProgressStatus>()))
			.Callback<ExportProgressStatus>(status => progressValues.Add(status.TaskProgressValue));
		var service = new FppDirectUploadService(mockClient.Object, isEspPixelStick: true);
		var tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".fseq");
		await File.WriteAllBytesAsync(tempPath, [1, 2, 3], ct);

		try
		{
			await Assert.ThrowsAsync<IOException>(
				() => service.UploadSequenceFileAsync(tempPath, "test.fseq", progress.Object, ct));
			Assert.Equal([0], progressValues);
		}
		finally
		{
			if (File.Exists(tempPath)) File.Delete(tempPath);
		}
	}

	[Fact]
	public async Task UploadAudioFileAsync_CallsUploadMusicAsync_WithCorrectFilename()
	{
		// Arrange
		var ct = TestContext.Current.CancellationToken;
		var mockClient = new Mock<IFppClient>();
		mockClient.Setup(c => c.UploadMusicAsync(
				It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		var svc = new FppDirectUploadService(mockClient.Object);

		var tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".mp3");
		await File.WriteAllBytesAsync(tempPath, new byte[] { 0xFF, 0xFB, 0x90 }, ct);

		try
		{
			// Act
			await svc.UploadAudioFileAsync(tempPath, "song.mp3", progress: null, ct);

			// Assert
			mockClient.Verify(c => c.UploadMusicAsync(
				"song.mp3", It.IsAny<Stream>(), ct), Times.Once);
		}
		finally
		{
			if (File.Exists(tempPath)) File.Delete(tempPath);
		}
	}

	[Fact]
	public async Task BackupUniverseFileAsync_CallsRenameFileAsync_WithConfigDir()
	{
		// Arrange
		var ct = TestContext.Current.CancellationToken;
		var mockClient = new Mock<IFppClient>();
		mockClient.Setup(c => c.RenameFileAsync(
				It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		var svc = new FppDirectUploadService(mockClient.Object);

		const string backupName = "co-universes.json_1012025-101530";

		// Act
		await svc.BackupUniverseFileAsync(backupName, ct);

		// Assert
		mockClient.Verify(c => c.RenameFileAsync(
			"config", "co-universes.json", backupName, ct), Times.Once);
	}

	[Fact]
	public async Task UploadUniverseFileAsync_CallsUploadFileAsync_WithConfigDir()
	{
		// Arrange
		var ct = TestContext.Current.CancellationToken;
		var mockClient = new Mock<IFppClient>();
		mockClient.Setup(c => c.UploadFileAsync(
				It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		var svc = new FppDirectUploadService(mockClient.Object);

		var tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".json");
		await File.WriteAllTextAsync(tempPath, """{"channelOutputs":[]}""", ct);

		try
		{
			// Act
			await svc.UploadUniverseFileAsync(tempPath, ct);

			// Assert
			mockClient.Verify(c => c.UploadFileAsync(
				"config", "co-universes.json", It.IsAny<Stream>(), ct), Times.Once);
		}
		finally
		{
			if (File.Exists(tempPath)) File.Delete(tempPath);
		}
	}
}
