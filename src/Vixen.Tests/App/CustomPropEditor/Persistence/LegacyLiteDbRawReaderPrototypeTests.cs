using System.Security.Cryptography;
using VixenModules.App.CustomPropEditor.Persistence.Legacy.LiteDb5021;
using Xunit;

namespace Vixen.Tests.App.CustomPropEditor.Persistence;

[Collection("CustomPropEditor")]
public sealed class LegacyLiteDbRawReaderPrototypeTests
{
	[Theory]
	[InlineData("background.jpg")]
	[InlineData("background.png")]
	public void Read_ReadsV4PropAndBackgroundWithoutModifyingTheSource(string imageEntryName)
	{
		var path = LegacyLiteDbFixtureBuilder.Create(imageEntryName);
		try
		{
			var expectedHash = SHA256.HashData(File.ReadAllBytes(path));
			var expectedWriteTime = File.GetLastWriteTimeUtc(path);

			var result = new LegacyLiteDbRawReaderPrototype().Read(path);

			Assert.Equal(expectedHash, SHA256.HashData(File.ReadAllBytes(path)));
			Assert.Equal(expectedWriteTime, File.GetLastWriteTimeUtc(path));
			Assert.Equal(imageEntryName, result.ImageEntryName);
			Assert.Equal([0xFF, 0xD8, 0xFF, 0xD9], result.Image);
			var prop = LegacyBsonDocumentReader.Read(result.PropDocument);
			Assert.Equal("Legacy Custom Prop", prop["Name"]);
			Assert.NotNull(prop["StateDefinition"]);
			Assert.NotNull(prop["StateDefinitions"]);
		}
		finally
		{
			File.Delete(path);
		}
	}

	[Fact]
	public void Read_RejectsNestedTypeMetadataBeforeMaterialization()
	{
		var path = LegacyLiteDbFixtureBuilder.Create("background.jpg", includeNestedTypeMetadata: true);
		try
		{
			var exception = Assert.Throws<InvalidDataException>(() => new LegacyLiteDbRawReaderPrototype().Read(path));

			Assert.Contains("_type", exception.Message, StringComparison.Ordinal);
		}
		finally
		{
			File.Delete(path);
		}
	}

	[Fact]
	public void Read_ReadsV4DocumentLargerThanSixteenMiBWithoutModifyingTheSource()
	{
		const int payloadSize = 17 * 1024 * 1024;
		var path = LegacyLiteDbFixtureBuilder.Create("background.jpg", payloadSize: payloadSize);
		try
		{
			var expectedHash = SHA256.HashData(File.ReadAllBytes(path));

			var result = new LegacyLiteDbRawReaderPrototype().Read(path);

			Assert.Equal(expectedHash, SHA256.HashData(File.ReadAllBytes(path)));
			Assert.True(result.PropDocument.Length > 16 * 1024 * 1024, "The generated raw props document must exceed LiteDB 5's 16 MiB document limit.");
			var prop = LegacyBsonDocumentReader.Read(result.PropDocument);
			Assert.Equal(payloadSize, Assert.IsType<byte[]>(prop["LargePayload"]).Length);
		}
		finally
		{
			File.Delete(path);
		}
	}
}
