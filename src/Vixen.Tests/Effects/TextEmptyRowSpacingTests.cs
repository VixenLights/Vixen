using System.Reflection;
using VixenModules.Effect.Text;
using Xunit;

namespace Vixen.Tests.Effects;

public sealed class TextEmptyRowSpacingTests
{
	[Fact]
	public void TextRendering_EmptyPlainTextRowBecomesSingleSpace()
	{
		var effect = new Text { TextLines = ["First", String.Empty, "Second"] };

		var preparedRows = PrepareTextLinesForRendering(effect);

		Assert.Equal(["First", " ", "Second"], preparedRows);
		Assert.Equal(String.Empty, effect.TextLines[1]);
	}

	[Fact]
	public void TextRendering_AllBlankRepresentationsBecomeSingleSpace()
	{
		var effect = new Text { TextLines = [null!, String.Empty, " ", "   ", "\t"] };

		var preparedRows = PrepareTextLinesForRendering(effect);

		Assert.All(preparedRows, row => Assert.Equal(" ", row));
		Assert.Null(effect.TextLines[0]);
		Assert.Equal("   ", effect.TextLines[3]);
	}

	[Fact]
	public void TextRendering_BlankRowCanonicalizationDoesNotMutateTextLines()
	{
		var effect = new Text { TextLines = [String.Empty, "  ", "Visible "] };

		_ = PrepareTextLinesForRendering(effect);

		Assert.Equal([String.Empty, "  ", "Visible "], effect.TextLines);
	}

	[Fact]
	public void TextRendering_ConsecutiveBlankRowsRemainDistinct()
	{
		var effect = new Text { TextLines = ["First", String.Empty, "\t", "Second"] };

		var preparedRows = PrepareTextLinesForRendering(effect);

		Assert.Equal(["First", " ", " ", "Second"], preparedRows);
	}

	[Fact]
	public void TextRendering_LeadingAndTrailingBlankRowsArePreserved()
	{
		var effect = new Text { TextLines = [" ", "Visible", String.Empty] };

		var preparedRows = PrepareTextLinesForRendering(effect);

		Assert.Equal([" ", "Visible", " "], preparedRows);
	}

	[Fact]
	public void TextRendering_VisibleTextWhitespaceIsUnchanged()
	{
		var effect = new Text { TextLines = ["  Visible text  "] };

		var preparedRows = PrepareTextLinesForRendering(effect);

		Assert.Equal("  Visible text  ", Assert.Single(preparedRows));
	}

	[Fact]
	public void TextRendering_EmptyCollectionRemainsEmpty()
	{
		var effect = new Text { TextLines = [] };

		var preparedRows = PrepareTextLinesForRendering(effect);

		Assert.Empty(preparedRows);
	}

	[Fact]
	public void TextRendering_MarkCollectionRetainsExistingEmptyEntryBehavior()
	{
		var effect = new Text
		{
			TextSource = TextSource.MarkCollection,
			TextLines = [String.Empty, " ", "Visible"]
		};

		var preparedRows = PrepareTextLinesForRendering(effect);

		Assert.Equal([" ", "Visible"], preparedRows);
	}

	[Fact]
	public void TextRendering_NonNormalModeRetainsExistingSplitBehavior()
	{
		var effect = new Text
		{
			TextMode = TextMode.Rotated,
			TextLines = ["Hi", String.Empty]
		};

		var preparedRows = PrepareTextLinesForRendering(effect);

		Assert.Equal(["H", "i", Environment.NewLine], preparedRows);
	}

	private static List<string> PrepareTextLinesForRendering(Text effect)
	{
		var method = typeof(Text).GetMethod("PrepareTextLinesForRendering", BindingFlags.Instance | BindingFlags.NonPublic);

		Assert.NotNull(method);
		return (List<string>)method.Invoke(effect, [])!;
	}
}
