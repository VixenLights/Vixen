using System.Windows;
using VixenModules.App.CustomPropEditor.BackgroundImageScaling;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Services;
using VixenModules.App.CustomPropEditor.ViewModels;
using Xunit;

namespace Vixen.Tests.App.CustomPropEditor.BackgroundImageScaling;

[Collection("CustomPropEditor")]
public sealed class CoordinateScalingTests
{
	[Fact]
	public void ApplyBackgroundImageScale_ScalesEachUniqueLightCenterWithoutChangingSize()
	{
		var drawingPanel = CreateDrawingPanelWithDuplicatedLeaf(out var light);

		drawingPanel.ApplyBackgroundImageScale(new BackgroundImageScaleOptions(200, 150, true));

		Assert.Equal(200, drawingPanel.Width);
		Assert.Equal(150, drawingPanel.Height);
		Assert.Equal(20, light.X);
		Assert.Equal(15, light.Y);
		Assert.Equal(7, light.Size);
	}

	[Fact]
	public void ApplyBackgroundImageScale_PreservesCoordinatesWhenLightScalingIsDisabled()
	{
		var drawingPanel = CreateDrawingPanelWithDuplicatedLeaf(out var light);

		drawingPanel.ApplyBackgroundImageScale(new BackgroundImageScaleOptions(200, 150, false));

		Assert.Equal(10, light.X);
		Assert.Equal(10, light.Y);
		Assert.Equal(7, light.Size);
	}

	[Fact]
	public void ApplyBackgroundImageScale_DoesNotClampOutOfBoundsLightCoordinates()
	{
		var drawingPanel = CreateDrawingPanelWithDuplicatedLeaf(out var light);
		light.X = 150;
		light.Y = -10;

		drawingPanel.ApplyBackgroundImageScale(new BackgroundImageScaleOptions(200, 150, true));

		Assert.Equal(300, light.X);
		Assert.Equal(-15, light.Y);
	}

	private static DrawingPanelViewModel CreateDrawingPanelWithDuplicatedLeaf(out Light light)
	{
		var prop = PropModelServices.Instance().CreateProp("Scale test", 100, 100);
		var firstGroup = new ElementModel("First Group", prop.RootNode);
		var secondGroup = new ElementModel("Second Group", prop.RootNode);
		var leaf = new ElementModel("Shared Leaf", firstGroup);
		light = new Light(new Point(10, 10), 7, leaf.Id);
		leaf.Lights.Add(light);
		firstGroup.AddChild(leaf);
		secondGroup.AddChild(leaf);
		prop.RootNode.AddChild(firstGroup);
		prop.RootNode.AddChild(secondGroup);

		return new DrawingPanelViewModel(new ElementTreeViewModel(prop));
	}
}
