using System.IO.Compression;
using System.IO;
using System.Text.Json;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Persistence.Documents;

namespace VixenModules.App.CustomPropEditor.Persistence;

internal sealed class PropPackageWriter(IPropDocumentMapper mapper, IPropImageCodec imageCodec, IPropFileReader reader, AtomicPropFileWriter atomicWriter)
{
	internal Func<string, CancellationToken, Task> BeforePublishAsync { get; set; }

	public Task WriteAsync(Prop prop, string destinationPath, CancellationToken cancellationToken = default) => WriteAsync(prop, destinationPath, null, cancellationToken);

	internal async Task WriteAsync(Prop prop, string destinationPath, string legacyBackupPath, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(prop);
		ArgumentException.ThrowIfNullOrWhiteSpace(destinationPath);
		var document = mapper.ToDocument(prop);
		PropDocumentValidator.Validate(document);
		var destination = Path.GetFullPath(destinationPath);
		var directory = Path.GetDirectoryName(destination) ?? throw new PropPersistenceException("The prop cannot be saved.", "The destination has no directory.");
		Directory.CreateDirectory(directory);
		var temporaryPath = Path.Combine(directory, $".{Path.GetFileName(destination)}.{Guid.NewGuid():N}.tmp");
		try
		{
			await WriteTemporaryPackageAsync(temporaryPath, document, prop.Image, cancellationToken);
			await reader.ReadAsync(temporaryPath, cancellationToken);
			if (BeforePublishAsync != null) await BeforePublishAsync(temporaryPath, cancellationToken);
			cancellationToken.ThrowIfCancellationRequested();
			await atomicWriter.PublishAsync(temporaryPath, destination, legacyBackupPath, cancellationToken);
		}
		catch (PropPersistenceException) { throw; }
		catch (Exception exception) when (exception is not OperationCanceledException)
		{
			throw new PropPersistenceException("The prop could not be saved.", "Package creation or publication failed.", exception);
		}
		finally
		{
			if (File.Exists(temporaryPath)) File.Delete(temporaryPath);
		}
	}

	private async Task WriteTemporaryPackageAsync(string temporaryPath, PropPackageDocument document, System.Windows.Media.Imaging.BitmapSource image, CancellationToken cancellationToken)
	{
		await using var file = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, FileOptions.Asynchronous | FileOptions.SequentialScan);
		using var archive = new ZipArchive(file, ZipArchiveMode.Create, leaveOpen: true);
		var jsonEntry = archive.CreateEntry("prop.json", CompressionLevel.Optimal);
		await using (var json = jsonEntry.Open())
			await JsonSerializer.SerializeAsync(json, document, CustomPropJsonSerializerContext.Default.PropPackageDocument, cancellationToken);
		cancellationToken.ThrowIfCancellationRequested();
		var imageEntry = archive.CreateEntry("background.jpg", CompressionLevel.Optimal);
		await using (var imageStream = imageEntry.Open()) imageCodec.EncodeJpeg(image, imageStream);
	}
}
