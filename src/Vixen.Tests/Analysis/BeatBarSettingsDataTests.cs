using VixenModules.Analysis.BeatsAndBars;
using Xunit;

namespace Vixen.Tests.Analysis;

public sealed class BeatBarSettingsDataTests
{
	[Fact]
	public void BeatCollectionNames_ReturnsExpectedNamesForStandardAndSplitBeats()
	{
		// Arrange
		var settings = new BeatBarSettingsData("Beats")
		{
			BeatsPerBar = 4,
			Divisions = 2,
			NoteSize = 4
		};

		// Act
		var standardCollectionNames = settings.BeatCollectionNames(false);
		var splitCollectionNames = settings.BeatCollectionNames(true);

		// Assert
		Assert.Equal(
			["Beats Beat #1", "Beats Beat #2", "Beats Beat #3", "Beats Beat #4"],
			standardCollectionNames);
		Assert.Equal(
			[
				"Beats Beat #1a", "Beats Beat #1b",
				"Beats Beat #2a", "Beats Beat #2b",
				"Beats Beat #3a", "Beats Beat #3b",
				"Beats Beat #4a", "Beats Beat #4b"
			],
			splitCollectionNames);
	}
}
