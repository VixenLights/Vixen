using System.IO;
using System.Runtime.CompilerServices;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Persistence;
using VixenModules.App.CustomPropEditor.Persistence.Legacy.LiteDb5021;

namespace VixenModules.App.CustomPropEditor.Services;

internal sealed class PropModelPersistenceService : IPropModelPersistenceService
{
	private readonly IPropDocumentMapper _mapper = new PropDocumentMapper();
	private readonly IPropImageCodec _imageCodec = new WpfPropImageCodec();
	private readonly ConditionalWeakTable<Prop, LegacySource> _legacySources = new();

	public async Task<Prop> LoadAsync(string path, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(path);
		var header = new byte[53];
		await using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, header.Length, FileOptions.Asynchronous | FileOptions.SequentialScan))
		{
			_ = await stream.ReadAsync(header, cancellationToken);
		}
		if (header.AsSpan().StartsWith(new byte[] { 0x50, 0x4b, 0x03, 0x04 }))
		{
			var result = await new PropPackageReader(_imageCodec).ReadAsync(path, cancellationToken);
			return _mapper.ToModel(result.Document, result.Image);
		}
		if (!LegacyLiteDbRawReaderPrototype.IsV4Header(header))
			throw new PropPersistenceException("The prop file format is not supported.", "The file is neither a schema-1 package nor a LiteDB v4 data file.");
		var legacy = await new LegacyLiteDbPropReader().ReadAsync(path, cancellationToken);
		_legacySources.Add(legacy.Prop, new LegacySource(Path.GetFullPath(path)));
		return legacy.Prop;
	}

	public async Task SaveAsync(Prop prop, string path, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(prop);
		ArgumentException.ThrowIfNullOrWhiteSpace(path);
		var writer = new PropPackageWriter(_mapper, _imageCodec, new PropPackageReader(_imageCodec), new AtomicPropFileWriter());
		if (_legacySources.TryGetValue(prop, out var legacy) && string.Equals(legacy.Path, Path.GetFullPath(path), StringComparison.OrdinalIgnoreCase))
		{
			var backup = AtomicPropFileWriter.GetLegacyBackupPath(path);
			if (File.Exists(backup))
			{
				if (!IsLegacyV4(backup)) throw new PropPersistenceException("The prop could not be saved.", "The existing legacy backup is not a LiteDB v4 file and will not be overwritten.");
				await writer.WriteAsync(prop, path, cancellationToken);
			}
			else
			{
				await writer.WriteAsync(prop, path, backup, cancellationToken);
			}
			_legacySources.Remove(prop);
			return;
		}
		await writer.WriteAsync(prop, path, cancellationToken);
	}

	private static bool IsLegacyV4(string path)
	{
		using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
		var header = new byte[53];
		_ = stream.Read(header, 0, header.Length);
		return LegacyLiteDbRawReaderPrototype.IsV4Header(header);
	}

	private sealed record LegacySource(string Path);
}
