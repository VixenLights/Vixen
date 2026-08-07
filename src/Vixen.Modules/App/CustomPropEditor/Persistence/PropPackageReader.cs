using System.IO.Compression;
using System.IO;
using System.Text.Json;
using VixenModules.App.CustomPropEditor.Persistence.Documents;

namespace VixenModules.App.CustomPropEditor.Persistence;

internal sealed class PropPackageReader(IPropImageCodec imageCodec) : IPropFileReader
{
	private const int MaximumJsonBytes = 4 * 1024 * 1024;
	private const int MaximumImageBytes = 64 * 1024 * 1024;
	private const int MaximumCompressionRatio = 100;
	private static readonly byte[] ZipSignature = [0x50, 0x4b, 0x03, 0x04];
	private static readonly byte[] JpegSignature = [0xff, 0xd8, 0xff];

	public async Task<PropFileReadResult> ReadAsync(string path, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(path);
		await using var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
		var signature = new byte[ZipSignature.Length];
		if (await file.ReadAsync(signature, cancellationToken) != ZipSignature.Length || !signature.SequenceEqual(ZipSignature))
			throw new PropPersistenceException("The prop file is not a valid package.", "The file does not have a ZIP local-header signature.");
		file.Position = 0;
		try
		{
			using var archive = new ZipArchive(file, ZipArchiveMode.Read, leaveOpen: false);
			var entries = ValidateEntries(archive);
			var document = await ReadDocumentAsync(entries.Json, cancellationToken);
			PropDocumentValidator.Validate(document);
			var image = await ReadImageAsync(entries.Image, cancellationToken);
			return new PropFileReadResult(document, image, PropFileSourceFormat.Package);
		}
		catch (PropPersistenceException) { throw; }
		catch (InvalidDataException exception)
		{
			throw new PropPersistenceException("The prop package is invalid.", "The ZIP archive could not be read.", exception);
		}
	}

	private static (ZipArchiveEntry Json, ZipArchiveEntry Image) ValidateEntries(ZipArchive archive)
	{
		if (archive.Entries.Count != 2) Fail("The package must contain exactly two entries.");
		var names = new HashSet<string>(StringComparer.Ordinal);
		ZipArchiveEntry json = null;
		ZipArchiveEntry image = null;
		foreach (var entry in archive.Entries)
		{
			if (!names.Add(entry.FullName) || entry.FullName.Contains('/') || entry.FullName.Contains('\\') || entry.FullName is not ("prop.json" or "background.jpg"))
				Fail("The package contains an unsafe or unexpected entry name.");
			if (entry.Length < 1 || entry.CompressedLength < 1 || entry.Length > MaximumImageBytes || entry.Length / entry.CompressedLength > MaximumCompressionRatio)
				Fail("The package contains an entry with an invalid size or compression ratio.");
			if (entry.FullName == "prop.json") json = entry;
			else image = entry;
		}
		if (json == null || image == null || json.Length > MaximumJsonBytes) Fail("The package is missing a required entry or its manifest is too large.");
		return (json, image);
	}

	private static async Task<PropPackageDocument> ReadDocumentAsync(ZipArchiveEntry entry, CancellationToken cancellationToken)
	{
		await using var source = entry.Open();
		try
		{
			return await JsonSerializer.DeserializeAsync(source, CustomPropJsonSerializerContext.Default.PropPackageDocument, cancellationToken)
				?? throw new PropPersistenceException("The prop package is invalid.", "The manifest contains no document.");
		}
		catch (JsonException exception)
		{
			throw new PropPersistenceException("The prop package is invalid.", "The manifest is not valid JSON.", exception);
		}
	}

	private async Task<System.Windows.Media.Imaging.BitmapSource> ReadImageAsync(ZipArchiveEntry entry, CancellationToken cancellationToken)
	{
		await using var source = entry.Open();
		await using var imageBytes = new MemoryStream((int)entry.Length);
		await source.CopyToAsync(imageBytes, 81920, cancellationToken);
		if (imageBytes.Length < JpegSignature.Length || !imageBytes.GetBuffer().AsSpan(0, JpegSignature.Length).SequenceEqual(JpegSignature))
			throw new PropPersistenceException("The prop package image is invalid.", "The image does not have a JPEG signature.");
		imageBytes.Position = 0;
		try { return imageCodec.DecodeJpeg(imageBytes); }
		catch (PropPersistenceException) { throw; }
		catch (Exception exception) { throw new PropPersistenceException("The prop package image is invalid.", "The JPEG decoder rejected the image.", exception); }
	}

	private static void Fail(string diagnostic) => throw new PropPersistenceException("The prop package is invalid.", diagnostic);
}
