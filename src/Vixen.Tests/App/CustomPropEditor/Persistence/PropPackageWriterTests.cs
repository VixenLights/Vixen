using System.IO.Compression;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Persistence;

namespace Vixen.Tests.App.CustomPropEditor.Persistence;

public class PropPackageWriterTests
{
	[Fact]
	public async Task WriteAsync_CreatesValidatedTwoEntryPackage()
	{
		var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.prp");
		try
		{
			var reader = new PropPackageReader(new WpfPropImageCodec());
			var writer = new PropPackageWriter(new PropDocumentMapper(), new WpfPropImageCodec(), reader, new AtomicPropFileWriter());

			await writer.WriteAsync(new Prop("Package"), path);

			using var archive = ZipFile.OpenRead(path);
			Assert.Equal(["background.jpg", "prop.json"], archive.Entries.Select(entry => entry.FullName).OrderBy(name => name));
			Assert.Equal((byte)0xff, archive.GetEntry("background.jpg").Open().ReadByte());
			var result = await reader.ReadAsync(path);
			Assert.Equal(PropFileSourceFormat.Package, result.SourceFormat);
		}
		finally
		{
			if (File.Exists(path)) File.Delete(path);
		}
	}

	[Fact]
	public async Task ReadAsync_RejectsUnexpectedArchiveEntries()
	{
		var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.prp");
		try
		{
			using (var archive = ZipFile.Open(path, ZipArchiveMode.Create))
			{
				archive.CreateEntry("prop.json");
				archive.CreateEntry("background.jpg");
				archive.CreateEntry("../unexpected.txt");
			}

			await Assert.ThrowsAsync<PropPersistenceException>(() => new PropPackageReader(new WpfPropImageCodec()).ReadAsync(path));
		}
		finally
		{
			if (File.Exists(path)) File.Delete(path);
		}
	}

	[Fact]
	public async Task WriteAsync_FaultBeforePublish_PreservesExistingDestination()
	{
		var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.prp");
		await File.WriteAllTextAsync(path, "original");
		try
		{
			var reader = new PropPackageReader(new WpfPropImageCodec());
			var writer = new PropPackageWriter(new PropDocumentMapper(), new WpfPropImageCodec(), reader, new AtomicPropFileWriter())
			{
				BeforePublishAsync = static (_, _) => throw new InvalidOperationException("Injected failure")
			};

			await Assert.ThrowsAsync<PropPersistenceException>(() => writer.WriteAsync(new Prop("Package"), path));
			Assert.Equal("original", await File.ReadAllTextAsync(path));
			Assert.Empty(Directory.GetFiles(Path.GetDirectoryName(path), $".{Path.GetFileName(path)}.*.tmp"));
		}
		finally
		{
			if (File.Exists(path)) File.Delete(path);
		}
	}
}
