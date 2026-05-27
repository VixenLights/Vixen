using Moq;
using VixenModules.App.ExportWizard;
using VixenModules.App.FPPClient.Client;
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
			await svc.UploadSequenceFileAsync(tempPath, "test.fseq", ct);

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
			await svc.UploadAudioFileAsync(tempPath, "song.mp3", ct);

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
