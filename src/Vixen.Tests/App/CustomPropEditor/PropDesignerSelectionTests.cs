using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Services;
using VixenModules.App.CustomPropEditor;
using VixenModules.App.CustomPropEditor.Controls;
using VixenModules.App.CustomPropEditor.ViewModels;
using Xunit;

namespace Vixen.Tests.App.CustomPropEditor;

/// <summary>
/// Verifies that viewer selection notifications expose the final selected state immediately.
/// </summary>
[Collection("CustomPropEditor")]
public sealed class PropDesignerSelectionTests
{
	/// <summary>
	/// Verifies that clicking a viewer item immediately selects and highlights it.
	/// </summary>
	[Fact]
	public void ViewerClick_SelectsAndHighlightsFirstLightImmediately()
	{
		RunOnStaThread(() =>
		{
			var (designer, drawing, first, second, restoreColors) = CreateFixture();
			try
			{
				InvokeViewerClick(designer, first);

				Assert.Contains(first, drawing.SelectedItems);
				Assert.True(first.IsSelected);
				Assert.Equal(System.Drawing.Color.HotPink, first.DisplayColor);
				Assert.Equal(System.Drawing.Color.Blue, second.DisplayColor);
			}
			finally
			{
				restoreColors();
				TearDownDesigner(designer);
			}
		});
	}

	/// <summary>
	/// Verifies that replacing a viewer selection immediately restores the former item's normal color.
	/// </summary>
	[Fact]
	public void ViewerClick_ReplacesSelectionAndRestoresPreviousLightColor()
	{
		RunOnStaThread(() =>
		{
			var (designer, drawing, first, second, restoreColors) = CreateFixture();
			try
			{
				InvokeViewerClick(designer, first);
				InvokeViewerClick(designer, second);

				Assert.DoesNotContain(first, drawing.SelectedItems);
				Assert.Contains(second, drawing.SelectedItems);
				Assert.False(first.IsSelected);
				Assert.True(second.IsSelected);
				Assert.Equal(System.Drawing.Color.Blue, first.DisplayColor);
				Assert.Equal(System.Drawing.Color.HotPink, second.DisplayColor);
			}
			finally
			{
				restoreColors();
				TearDownDesigner(designer);
			}
		});
	}

	/// <summary>
	/// Verifies that clearing viewer selection resets item flags before notifying observers.
	/// </summary>
	[Fact]
	public void ViewerClearSelections_ClearsFlagsBeforeResetAndRestoresNormalColors()
	{
		RunOnStaThread(() =>
		{
			var (designer, drawing, first, second, restoreColors) = CreateFixture(preselectLights: true);
			var resetObservedFinalFlags = false;
			NotifyCollectionChangedEventHandler resetObserver = (_, args) =>
			{
				if (args.Action == NotifyCollectionChangedAction.Reset)
				{
					resetObservedFinalFlags = drawing.LightNodes.All(light => !light.IsSelected);
				}
			};
			drawing.SelectedItems.CollectionChanged += resetObserver;
			try
			{
				InvokePrivateMethod(designer, "ClearSelections");

				Assert.True(resetObservedFinalFlags);
				Assert.Empty(drawing.SelectedItems);
				Assert.False(first.IsSelected);
				Assert.False(second.IsSelected);
				Assert.Equal(System.Drawing.Color.Blue, first.DisplayColor);
				Assert.Equal(System.Drawing.Color.Blue, second.DisplayColor);
			}
			finally
			{
				drawing.SelectedItems.CollectionChanged -= resetObserver;
				restoreColors();
				TearDownDesigner(designer);
			}
		});
	}

	private static (PropDesigner Designer, DrawingPanelViewModel Drawing, LightViewModel First, LightViewModel Second, Action RestoreColors)
		CreateFixture(bool preselectLights = false)
	{
		var prop = PropModelServices.Instance().CreateProp("Selection Prop");
		var model = new ElementModel("Model", prop.RootNode);
		var firstLeaf = new ElementModel("Leaf 1", model);
		firstLeaf.Lights.Add(new Light(new Point(10, 10), ElementModel.DefaultLightSize, firstLeaf.Id));
		var secondLeaf = new ElementModel("Leaf 2", model);
		secondLeaf.Lights.Add(new Light(new Point(20, 20), ElementModel.DefaultLightSize, secondLeaf.Id));
		model.AddChild(firstLeaf);
		model.AddChild(secondLeaf);
		prop.RootNode.AddChild(model);

		var elementTree = new ElementTreeViewModel(prop);
		var configurationService = ConfigurationService.Instance();
		var configurationProperty = typeof(ConfigurationService).GetProperty(
			nameof(ConfigurationService.Config),
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;
		var originalConfiguration = configurationProperty.GetValue(configurationService);
		var configuration = new Configuration(new CustomPropEditorData())
		{
			LightColor = System.Drawing.Color.Blue,
			SelectedLightColor = System.Drawing.Color.HotPink
		};
		configurationProperty.SetValue(configurationService, configuration);
		var drawing = new DrawingPanelViewModel(elementTree)
		{
			IsDrawing = false
		};
		var first = drawing.LightNodes.Single(light => light.Light.ParentModelId == firstLeaf.Id);
		var second = drawing.LightNodes.Single(light => light.Light.ParentModelId == secondLeaf.Id);

		if (preselectLights)
		{
			first.IsSelected = true;
			second.IsSelected = true;
			drawing.SelectedItems.Add(first);
			drawing.SelectedItems.Add(second);
		}

		var designer = new PropDesigner
		{
			ItemsSource = drawing.LightNodes,
			SelectedModels = drawing.SelectedItems
		};
		var canvas = new Canvas();
		typeof(PropDesigner)
			.GetField("_drawingCanvas", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(designer, canvas);

		void RestoreColors()
		{
			configurationProperty.SetValue(configurationService, originalConfiguration);
		}

		return (designer, drawing, first, second, RestoreColors);
	}

	private static void InvokeViewerClick(PropDesigner designer, LightViewModel target)
	{
		var source = new System.Windows.Shapes.Path { DataContext = target };
		var eventArgs = new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left)
		{
			RoutedEvent = Mouse.PreviewMouseDownEvent
		};
		eventArgs.Source = source;
		InvokePrivateMethod(designer, "_drawingCanvas_MouseLeftMouseDown", new Canvas(), eventArgs);
	}

	private static void InvokePrivateMethod(object instance, string methodName, params object[] arguments)
	{
		var method = instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)!;
		method.Invoke(instance, arguments);
	}

	private static void TearDownDesigner(PropDesigner designer)
	{
		designer.ItemsSource = null;
		designer.SelectedModels = null;
	}

	private static void RunOnStaThread(Action action)
	{
		Exception? exception = null;
		var thread = new Thread(() =>
		{
			try
			{
				action();
			}
			catch (Exception ex)
			{
				exception = ex;
			}
		});

		thread.SetApartmentState(ApartmentState.STA);
		thread.Start();
		if (!thread.Join(TimeSpan.FromSeconds(30)))
		{
			throw new TimeoutException("The WPF selection test did not complete within 30 seconds.");
		}

		if (exception != null)
		{
			ExceptionDispatchInfo.Capture(exception).Throw();
		}
	}
}
