using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using Vixen.Marks;
using VixenModules.App.Marks;
using VixenModules.Editor.TimedSequenceEditor.Forms.WPF.MarksDocker.Services;
using Xunit;

namespace Vixen.Tests.Sequencer;

public sealed class PangolinBeyondMarkImportTests
{
	[Fact]
	public void TryParse_ValidRows_ParsesTextsTimesAndBgrColors()
	{
		// Arrange
		const string csv = "#,Name,Start,Color\r\nM1,Intro,00:01.250,112233\r\nM2,Chorus,01:02:03.004,A0B0C0\r\n";

		// Act
		var result = PangolinBeyondMarkParser.TryParse(csv, out var marks, out var error);

		// Assert
		Assert.True(result);
		Assert.Empty(error);
		Assert.Equal(2, marks.Count);
		Assert.Equal("Intro", marks[0].Text);
		Assert.Equal(TimeSpan.FromSeconds(1.25), marks[0].StartTime);
		Assert.Equal(Color.FromArgb(0x33, 0x22, 0x11), marks[0].Color);
		Assert.Equal(TimeSpan.FromHours(1) + TimeSpan.FromMinutes(2) + TimeSpan.FromSeconds(3.004), marks[1].StartTime);
		Assert.Equal(Color.FromArgb(0xC0, 0xB0, 0xA0), marks[1].Color);
	}

	[Theory]
	[InlineData("invalid header", "Line 1")]
	[InlineData("#,Name,Start,Color\nM1,Intro,00:01.250", "Line 2")]
	[InlineData("#,Name,Start,Color\nM1,Intro,1.25,112233", "Line 2")]
	[InlineData("#,Name,Start,Color\nM1,Intro,00:01.250,GG2233", "Line 2")]
	public void TryParse_InvalidCsv_ReturnsLineSpecificError(string csv, string expectedError)
	{
		// Act
		var result = PangolinBeyondMarkParser.TryParse(csv, out var marks, out var error);

		// Assert
		Assert.False(result);
		Assert.Empty(marks);
		Assert.Contains(expectedError, error);
	}

	[Fact]
	public void CreateCollections_GroupByColor_PreservesFirstColorOrderAndDefaultMarkDuration()
	{
		// Arrange
		var firstColor = Color.FromArgb(0x33, 0x22, 0x11);
		var secondColor = Color.FromArgb(0xC0, 0xB0, 0xA0);
		IReadOnlyList<PangolinBeyondMarkRecord> records =
		[
			new("Intro", TimeSpan.FromSeconds(1), firstColor),
			new("Chorus", TimeSpan.FromSeconds(2), secondColor),
			new("Again", TimeSpan.FromSeconds(3), firstColor)
		];

		// Act
		var collections = PangolinBeyondMarkCollectionFactory.CreateCollections(records, PangolinBeyondImportMode.GroupByColor, Color.Empty);

		// Assert
		Assert.Equal(["Beyond Marks - #332211", "Beyond Marks - #C0B0A0"], collections.Select(collection => collection.Name));
		Assert.Equal(firstColor, collections[0].Decorator.Color);
		Assert.Equal(secondColor, collections[1].Decorator.Color);
		Assert.All(collections, collection => Assert.True(collection.ShowMarkBar));
		Assert.Equal(["Intro", "Again"], collections[0].Marks.Select(mark => mark.Text));
		Assert.All(collections[0].Marks, mark =>
		{
			Assert.Equal(TimeSpan.FromMilliseconds(450), mark.Duration);
			Assert.Same(collections[0], mark.Parent);
		});
	}

	[Fact]
	public void CreateCollections_SingleCollection_UsesReplacementColorForAllMarks()
	{
		// Arrange
		var replacementColor = Color.MediumPurple;
		IReadOnlyList<PangolinBeyondMarkRecord> records =
		[
			new("Intro", TimeSpan.FromSeconds(1), Color.Red),
			new("Chorus", TimeSpan.FromSeconds(2), Color.Blue)
		];

		// Act
		var collections = PangolinBeyondMarkCollectionFactory.CreateCollections(records, PangolinBeyondImportMode.SingleCollection, replacementColor);

		// Assert
		var collection = Assert.Single(collections);
		Assert.Equal("Beyond Marks", collection.Name);
		Assert.True(collection.ShowMarkBar);
		Assert.Equal(replacementColor, collection.Decorator.Color);
		Assert.Equal(["Intro", "Chorus"], collection.Marks.Select(mark => mark.Text));
		Assert.All(collection.Marks, mark => Assert.Equal(TimeSpan.FromMilliseconds(450), mark.Duration));
	}

	[Fact]
	public void MaterializePangolinBeyondMarks_Cancelled_ReturnsCancelledResult()
	{
		IReadOnlyList<PangolinBeyondMarkRecord> records = [new("Intro", TimeSpan.FromSeconds(1), Color.Red)];

		// Act
		var result = MarkImportExportService.MaterializePangolinBeyondMarks(records, null, Color.Empty);

		// Assert
		Assert.Equal(MarkCollectionImportStatus.Cancelled, result.Status);
		Assert.Empty(result.Collections);
	}

	[Theory]
	[InlineData(0, false)]
	[InlineData(1, false)]
	[InlineData(2, true)]
	public void RequiresPangolinBeyondColorChoice_ReturnsTrueOnlyForMultipleSourceColors(int colorCount, bool expected)
	{
		// Arrange
		IReadOnlyList<PangolinBeyondMarkRecord> records = Enumerable.Range(0, colorCount)
			.Select(index => new PangolinBeyondMarkRecord($"Mark {index}", TimeSpan.FromSeconds(index), Color.FromArgb(index, index, index)))
			.ToList();

		// Act
		var requiresChoice = MarkImportExportService.RequiresPangolinBeyondColorChoice(records);

		// Assert
		Assert.Equal(expected, requiresChoice);
	}

	[Fact]
	public void RequiresPangolinBeyondColorChoice_RepeatedSourceColor_ReturnsFalse()
	{
		// Arrange
		IReadOnlyList<PangolinBeyondMarkRecord> records =
		[
			new("Intro", TimeSpan.FromSeconds(1), Color.Red),
			new("Chorus", TimeSpan.FromSeconds(2), Color.Red)
		];

		// Act
		var requiresChoice = MarkImportExportService.RequiresPangolinBeyondColorChoice(records);

		// Assert
		Assert.False(requiresChoice);
	}

	[Fact]
	public void GetPangolinBeyondImportMode_LegacyYesChoice_ReturnsGroupByColor()
	{
		// Act
		var importMode = MarkImportExportService.GetPangolinBeyondImportMode(DialogResult.OK);

		// Assert
		Assert.Equal(PangolinBeyondImportMode.GroupByColor, importMode);
	}

	[Fact]
	public void GetPangolinBeyondImportMode_NoChoice_ReturnsSingleCollection()
	{
		// Act
		var importMode = MarkImportExportService.GetPangolinBeyondImportMode(DialogResult.No);

		// Assert
		Assert.Equal(PangolinBeyondImportMode.SingleCollection, importMode);
	}

	[Fact]
	public void GetPangolinBeyondImportMode_CancelledChoice_ReturnsNull()
	{
		// Act
		var importMode = MarkImportExportService.GetPangolinBeyondImportMode(DialogResult.Cancel);

		// Assert
		Assert.Null(importMode);
	}

	[Fact]
	public void MaterializePangolinBeyondMarks_Success_ReturnsDetachedSourceName()
	{
		IReadOnlyList<PangolinBeyondMarkRecord> records = [new("Intro", TimeSpan.FromSeconds(1), Color.Red)];

		// Act
		var result = MarkImportExportService.MaterializePangolinBeyondMarks(
			records,
			PangolinBeyondImportMode.SingleCollection,
			Color.MediumPurple);

		// Assert
		Assert.Equal(MarkCollectionImportStatus.Succeeded, result.Status);
		Assert.Equal("Beyond Marks", Assert.Single(result.Collections).Name);
	}

	[Fact]
	public void MaterializePangolinBeyondMarks_Success_DoesNotAssignDefault()
	{
		IReadOnlyList<PangolinBeyondMarkRecord> records = [new("Intro", TimeSpan.FromSeconds(1), Color.Red)];

		// Act
		var result = MarkImportExportService.MaterializePangolinBeyondMarks(
			records,
			PangolinBeyondImportMode.SingleCollection,
			Color.MediumPurple);

		// Assert
		Assert.False(Assert.Single(result.Collections).IsDefault);
	}

	[Fact]
	public void MaterializeXTimingTracks_LipSyncLayers_LinkOnlyToLocalCandidates()
	{
		// Arrange
		var xmlDocument = new XmlDocument();
		xmlDocument.LoadXml("<timing name=\"Voice\"><EffectLayer><Effect label=\"Phrase\" starttime=\"0\" endtime=\"100\" /></EffectLayer><EffectLayer><Effect label=\"Word\" starttime=\"100\" endtime=\"200\" /></EffectLayer><EffectLayer><Effect label=\"Phoneme\" starttime=\"200\" endtime=\"300\" /></EffectLayer></timing>");

		// Act
		var result = MarkImportExportService.MaterializeXTimingTracks(xmlDocument, MarkCollectionImportType.XTiming);

		// Assert
		Assert.Equal(MarkCollectionImportStatus.Succeeded, result.Status);
		Assert.Equal(["Voice - Phrase", "Voice - Word", "Voice - Phoneme"], result.Collections.Select(collection => collection.Name));
		Assert.Equal([MarkCollectionType.Phrase, MarkCollectionType.Word, MarkCollectionType.Phoneme], result.Collections.Select(collection => collection.CollectionType));
		Assert.Equal(result.Collections[0].Id, result.Collections[1].LinkedMarkCollectionId);
		Assert.Equal(result.Collections[1].Id, result.Collections[2].LinkedMarkCollectionId);
	}
}
