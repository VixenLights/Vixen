using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
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
		const string legacyJson = @"{""CycleCharacterColor"":true,""CycleColor"":false,""TextSource"":0}";

		// Act
		var migratedData = DeserializeJson(legacyJson);

		// Assert
		Assert.True(migratedData.CycleColor);
		Assert.Equal("Character", GetCycleColorModeValue(migratedData));
	}

	[Fact]
	public void TextData_MigratesLegacyMarkCycleToWordMode()
	{
		// Arrange
		const string legacyJson = @"{""CycleCharacterColor"":false,""CycleColor"":true,""TextSource"":1}";

		// Act
		var migratedData = DeserializeJson(legacyJson);

		// Assert
		Assert.True(migratedData.CycleColor);
		Assert.Equal("Word", GetCycleColorModeValue(migratedData));
	}

	[Fact]
	public void TextData_PreservesDisabledCycleColorWhenLegacyCharacterFlagRemainsTrue()
	{
		// Arrange
		var savedData = new TextData
		{
			CycleCharacterColor = true,
			CycleColor = false,
			TextSource = TextSource.None
		};
		SetCycleColorModeValue(savedData, "Character");

		// Act
		var reloadedData = RoundTrip(savedData);

		// Assert
		Assert.False(reloadedData.CycleColor);
		Assert.Equal("Character", GetCycleColorModeValue(reloadedData));
	}

	[Fact]
	public void TextProperties_DisablingCycleColorClearsLegacyCharacterFlag()
	{
		// Arrange
		var effect = new Text
		{
			CycleCharacterColor = true,
			CycleColor = true
		};

		// Act
		effect.CycleColor = false;

		// Assert
		Assert.False(effect.CycleColor);
		Assert.False(effect.CycleCharacterColor);
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

	[Fact]
	public void TextRendering_CharacterCycleVisualRepresentationAlignsWithNonCycle()
	{
		// Arrange
		var textLines = new List<string> { "One two" };
		using var font = new Font("Arial", 24);
		var nonCycleEffect = new Text
		{
			TextLines = textLines,
			Font = font,
			CycleColor = false
		};
		var characterCycleEffect = new Text
		{
			TextLines = textLines,
			Font = font,
			CycleColor = true
		};
		SetCycleColorModeValue(characterCycleEffect, "Character");

		// Act
		var nonCycleBounds = RenderVisualRepresentationBounds(nonCycleEffect);
		var characterCycleBounds = RenderVisualRepresentationBounds(characterCycleEffect);

		// Assert
		Assert.True(Math.Abs(nonCycleBounds.Left - characterCycleBounds.Left) <= 1);
	}

	private static TextData RoundTrip(TextData data)
	{
		var serializer = new DataContractJsonSerializer(typeof(TextData));
		using var stream = new MemoryStream();
		serializer.WriteObject(stream, data);
		var json = Encoding.UTF8.GetString(stream.ToArray());

		return DeserializeJson(json);
	}

	private static TextData DeserializeJson(string json)
	{
		var serializer = new DataContractJsonSerializer(typeof(TextData));
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

	private static Rectangle RenderVisualRepresentationBounds(Text effect)
	{
		using var bitmap = new Bitmap(300, 100, PixelFormat.Format32bppArgb);
		using (var graphics = Graphics.FromImage(bitmap))
		{
			graphics.Clear(Color.Transparent);
			effect.GenerateVisualRepresentation(graphics, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
		}

		var left = bitmap.Width;
		var top = bitmap.Height;
		var right = -1;
		var bottom = -1;
		for (var y = 0; y < bitmap.Height; y++)
		{
			for (var x = 0; x < bitmap.Width; x++)
			{
				if (bitmap.GetPixel(x, y).A == 0)
				{
					continue;
				}

				left = Math.Min(left, x);
				top = Math.Min(top, y);
				right = Math.Max(right, x);
				bottom = Math.Max(bottom, y);
			}
		}

		Assert.True(right >= left);
		Assert.True(bottom >= top);
		return Rectangle.FromLTRB(left, top, right + 1, bottom + 1);
	}

	private static PropertyInfo GetCycleColorModeProperty()
	{
		var property = typeof(TextData).GetProperty("CycleColorMode", BindingFlags.Instance | BindingFlags.Public);

		Assert.NotNull(property);
		return property;
	}
}
