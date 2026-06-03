using VixenModules.App.CustomPropEditor.Import.XLights;
using Xunit;

namespace Vixen.Tests.App.CustomPropEditor.Import.XLights;

public class CustomModelParserTests
{
	[Fact]
	public void CustomModelAndCompressed_DecodeToEquivalentVixenState()
	{
		// Arrange
		const string customModel = "1,,;,,2;,,,,3";
		const string compressed = "1,0,0;2,1,2;3,2,4";

		// Act
		var customNodes = CustomModelParser.ParseCustomModel(customModel, 1);
		var compressedNodes = CustomModelParser.ParseCustomModelCompressed(compressed, 1);

		// Assert
		AssertEquivalentNodes(customNodes, compressedNodes);
	}

	[Fact]
	public void CustomModelAndCompressed_WithScale_DecodeToEquivalentVixenState()
	{
		// Arrange
		const string customModel = "1,,;,,2";
		const string compressed = "1,0,0;2,1,2";

		// Act
		var customNodes = CustomModelParser.ParseCustomModel(customModel, 4);
		var compressedNodes = CustomModelParser.ParseCustomModelCompressed(compressed, 4);

		// Assert
		AssertEquivalentNodes(customNodes, compressedNodes);
		Assert.Equal(9, compressedNodes[2].X);
		Assert.Equal(5, compressedNodes[2].Y);
	}

	[Fact]
	public void SantaWavingMinimalData_DecodesToEquivalentVixenState()
	{
		// Arrange
		var customModel = string.Join(
			";",
			CreateCustomModelRow((314, 0)),
			CreateCustomModelRow((315, 188)),
			CreateCustomModelRow((316, 195)));
		const string compressed = "314,0,0;315,1,188;316,2,195";

		// Act
		var customNodes = CustomModelParser.ParseCustomModel(customModel, 1);
		var compressedNodes = CustomModelParser.ParseCustomModelCompressed(compressed, 1);

		// Assert
		AssertEquivalentNodes(customNodes, compressedNodes);
	}

	[Fact]
	public void SnowmanMinimalData_DecodesToEquivalentVixenState()
	{
		// Arrange
		var customModel = string.Join(
			";",
			CreateCustomModelRow((243, 81), (242, 85), (241, 89), (240, 92)),
			CreateCustomModelRow((245, 73), (244, 77), (239, 96)));
		const string compressed = "243,0,81;242,0,85;241,0,89;240,0,92;245,1,73;244,1,77;239,1,96";

		// Act
		var customNodes = CustomModelParser.ParseCustomModel(customModel, 1);
		var compressedNodes = CustomModelParser.ParseCustomModelCompressed(compressed, 1);

		// Assert
		AssertEquivalentNodes(customNodes, compressedNodes);
	}

	[Fact]
	public void Resolver_PrefersCompressedWhenBothFormatsAreValid()
	{
		// Act
		var result = CustomModelSourceResolver.Resolve("1,0,0", "2", 1);

		// Assert
		Assert.True(result.Success);
		Assert.Equal(CustomModelSource.CustomModelCompressed, result.Source);
		Assert.True(result.ModelNodes.ContainsKey(1));
		Assert.False(result.ModelNodes.ContainsKey(2));
	}

	[Fact]
	public void Resolver_FallsBackToCustomModelWhenCompressedIsInvalid()
	{
		// Act
		var result = CustomModelSourceResolver.Resolve("bad", "2", 1);

		// Assert
		Assert.True(result.Success);
		Assert.Equal(CustomModelSource.CustomModel, result.Source);
		Assert.NotNull(result.CompressedException);
		Assert.True(result.ModelNodes.ContainsKey(2));
	}

	[Fact]
	public void Resolver_FailsWhenNeitherFormatIsValid()
	{
		// Act
		var result = CustomModelSourceResolver.Resolve("bad", "also-bad", 1);

		// Assert
		Assert.False(result.Success);
		Assert.Equal(CustomModelSource.None, result.Source);
		Assert.NotNull(result.CompressedException);
		Assert.NotNull(result.CustomModelException);
		Assert.Empty(result.ModelNodes);
	}

	private static void AssertEquivalentNodes(
		IReadOnlyDictionary<int, ModelNode> expected,
		IReadOnlyDictionary<int, ModelNode> actual)
	{
		Assert.Equal(expected.Keys.Order(), actual.Keys.Order());
		foreach (var order in expected.Keys)
		{
			Assert.Equal(expected[order].Order, actual[order].Order);
			Assert.Equal(expected[order].X, actual[order].X);
			Assert.Equal(expected[order].Y, actual[order].Y);
		}
	}

	private static string CreateCustomModelRow(params (int Order, int ZeroBasedX)[] nodes)
	{
		var maxX = nodes.Max(node => node.ZeroBasedX);
		var cells = Enumerable.Repeat(string.Empty, maxX + 1).ToArray();
		foreach (var node in nodes)
		{
			cells[node.ZeroBasedX] = node.Order.ToString();
		}

		return string.Join(",", cells);
	}
}
