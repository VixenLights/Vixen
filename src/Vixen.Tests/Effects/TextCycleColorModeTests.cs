using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using VixenModules.Effect.Text;
using Xunit;

namespace Vixen.Tests.Effects;

public sealed class TextCycleColorModeTests
{
	[Fact]
	public void TextData_DefaultsToCycleColorOffAndCharacterMode()
	{
		// Arrange
		var data = new TextData();

		// Act
		var mode = GetCycleColorModeValue(data);

		// Assert
		Assert.False(data.CycleColor);
		Assert.Equal("Character", mode);
	}

	[Fact]
	public void TextData_CloneCopiesCycleColorMode()
	{
		// Arrange
		var original = new TextData
		{
			CycleColor = true
		};
		SetCycleColorModeValue(original, "Word");

		// Act
		var clone = (TextData)original.Clone();

		// Assert
		Assert.True(clone.CycleColor);
		Assert.Equal("Word", GetCycleColorModeValue(clone));
	}

	[Fact]
	public void TextData_MigratesLegacyCharacterCycleToCharacterMode()
	{
		// Arrange
		var legacyData = new TextData
		{
			CycleCharacterColor = true,
			CycleColor = false,
			TextSource = TextSource.None
		};

		// Act
		var migratedData = RoundTrip(legacyData);

		// Assert
		Assert.True(migratedData.CycleColor);
		Assert.Equal("Character", GetCycleColorModeValue(migratedData));
	}

	[Fact]
	public void TextData_MigratesLegacyMarkCycleToWordMode()
	{
		// Arrange
		var legacyData = new TextData
		{
			CycleCharacterColor = false,
			CycleColor = true,
			TextSource = TextSource.MarkCollection
		};

		// Act
		var migratedData = RoundTrip(legacyData);

		// Assert
		Assert.True(migratedData.CycleColor);
		Assert.Equal("Word", GetCycleColorModeValue(migratedData));
	}

	[Fact]
	public void TextProperties_ShowCycleModeOnlyWhenCycleColorIsEnabled()
	{
		// Arrange
		var effect = new Text();

		// Act
		var propertiesBeforeCycleColor = TypeDescriptor.GetProperties(effect);
		var cycleCharacterProperty = propertiesBeforeCycleColor[nameof(Text.CycleCharacterColor)];
		var cycleModePropertyBeforeCycleColor = propertiesBeforeCycleColor["CycleColorMode"];
		effect.CycleColor = true;
		var cycleModePropertyAfterCycleColor = TypeDescriptor.GetProperties(effect)["CycleColorMode"];

		// Assert
		Assert.NotNull(cycleCharacterProperty);
		Assert.False(cycleCharacterProperty.IsBrowsable);
		Assert.NotNull(cycleModePropertyBeforeCycleColor);
		Assert.False(cycleModePropertyBeforeCycleColor.IsBrowsable);
		Assert.NotNull(cycleModePropertyAfterCycleColor);
		Assert.True(cycleModePropertyAfterCycleColor.IsBrowsable);
	}

	[Fact]
	public void TextRendering_WordCycleRunsPreserveLiteralSpaces()
	{
		// Arrange
		var splitMethod = typeof(Text).GetMethod("SplitTextIntoSpacePreservingWordRuns", BindingFlags.Static | BindingFlags.NonPublic);

		// Act
		var runs = (List<string>)splitMethod!.Invoke(null, ["One  two   three"])!;

		// Assert
		Assert.Equal(["One", "  ", "two", "   ", "three"], runs);
		Assert.Equal(3, runs.Count(run => !String.IsNullOrWhiteSpace(run)));
	}

	[Fact]
	public void TextRendering_CycleModeHelpersUseUnifiedCycleColor()
	{
		// Arrange
		var effect = new Text
		{
			CycleCharacterColor = true,
			CycleColor = true
		};
		SetCycleColorModeValue(effect, "Character");

		// Act
		var characterModeEnabled = InvokeBooleanHelper(effect, "IsCharacterCycleEnabled");
		var wordModeDisabled = InvokeBooleanHelper(effect, "IsWordCycleEnabled");
		SetCycleColorModeValue(effect, "Word");
		var characterModeDisabled = InvokeBooleanHelper(effect, "IsCharacterCycleEnabled");
		var wordModeEnabled = InvokeBooleanHelper(effect, "IsWordCycleEnabled");
		var markWordModeDisabled = InvokeBooleanHelper(effect, "IsMarkWordCycleEnabled");
		effect.TextSource = TextSource.MarkCollection;
		var markWordModeEnabled = InvokeBooleanHelper(effect, "IsMarkWordCycleEnabled");

		// Assert
		Assert.True(characterModeEnabled);
		Assert.False(wordModeDisabled);
		Assert.False(characterModeDisabled);
		Assert.True(wordModeEnabled);
		Assert.False(markWordModeDisabled);
		Assert.True(markWordModeEnabled);
	}

	[Fact]
	public void TextRendering_EmptyColorsDoNotThrowInVisualRepresentation()
	{
		// Arrange
		var effect = new Text
		{
			Colors = [],
			CycleColor = true,
			TextLines = ["One two"]
		};
		SetCycleColorModeValue(effect, "Word");
		using var bitmap = new Bitmap(200, 80);
		using var graphics = Graphics.FromImage(bitmap);

		// Act
		var exception = Record.Exception(() => effect.GenerateVisualRepresentation(graphics, new Rectangle(0, 0, 200, 80)));

		// Assert
		Assert.Null(exception);
	}

	private static TextData RoundTrip(TextData data)
	{
		var serializer = new DataContractJsonSerializer(typeof(TextData));
		using var stream = new MemoryStream();
		serializer.WriteObject(stream, data);
		var json = Encoding.UTF8.GetString(stream.ToArray());

		using var readStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
		return (TextData)serializer.ReadObject(readStream)!;
	}

	private static string GetCycleColorModeValue(TextData data)
	{
		var property = GetCycleColorModeProperty();
		var value = property.GetValue(data);

		Assert.NotNull(value);
		return value.ToString()!;
	}

	private static void SetCycleColorModeValue(TextData data, string modeName)
	{
		var property = GetCycleColorModeProperty();
		var modeValue = Enum.Parse(property.PropertyType, modeName);

		property.SetValue(data, modeValue);
	}

	private static void SetCycleColorModeValue(Text effect, string modeName)
	{
		var property = typeof(Text).GetProperty("CycleColorMode", BindingFlags.Instance | BindingFlags.Public);

		Assert.NotNull(property);
		var modeValue = Enum.Parse(property.PropertyType, modeName);
		property.SetValue(effect, modeValue);
	}

	private static bool InvokeBooleanHelper(Text effect, string methodName)
	{
		var method = typeof(Text).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);

		Assert.NotNull(method);
		return (bool)method.Invoke(effect, [])!;
	}

	private static PropertyInfo GetCycleColorModeProperty()
	{
		var property = typeof(TextData).GetProperty("CycleColorMode", BindingFlags.Instance | BindingFlags.Public);

		Assert.NotNull(property);
		return property;
	}
}
