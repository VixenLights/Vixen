using System.Drawing;
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
}
