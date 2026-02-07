using Catel.Reflection;
using System.ComponentModel;
using System.Windows;
using VixenApplication.SetupDisplay.Wizards.ViewModels;
using VixenModules.App.Curves;
using ZedGraph;
using System.Windows.Controls;
using static VixenApplication.SetupDisplay.Wizards.Pages.DimmingWizardPage;

namespace VixenApplication.SetupDisplay.Wizards.Views
{
	public partial class DimmingWizardPageView : INotifyPropertyChanged
	{
		private CurveEditor _curveEditor;

		public DimmingWizardPageView()
		{
			InitializeComponent();

			SimpleDimming.Visibility = Visibility.Collapsed;
			CustomCurve.Visibility = Visibility.Collapsed;

			_curveEditor = new CurveEditor(zedGraphControl);
			Curve = new Curve();

			zedGraphControl.MouseDownEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(zedGraphControl_MouseDown);
			zedGraphControl.MouseUpEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(_curveEditor.zedGraphControl_MouseUpEvent);
			zedGraphControl.PreMouseMoveEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(zedGraphControl_PreMouseMove);
			zedGraphControl.PostMouseMoveEvent += new ZedGraph.ZedGraphControl.ZedMouseEventHandler(_curveEditor.zedGraphControl_PostMouseMoveEvent);

			zedGraphControl.IsAntiAlias = true;
			zedGraphControl.IsEnableHEdit = true;
			zedGraphControl.IsEnableHPan = false;
			zedGraphControl.IsEnableHZoom = false;
			zedGraphControl.IsEnableVEdit = true;
			zedGraphControl.IsEnableVPan = false;
			zedGraphControl.IsEnableVZoom = false;
			zedGraphControl.IsEnableWheelZoom = false;
			zedGraphControl.IsShowContextMenu = false;
			zedGraphControl.IsAntiAlias = true;
			zedGraphControl.IsEnableHEdit = true;
			zedGraphControl.IsEnableHPan = false;
			zedGraphControl.IsEnableHZoom = false;
			zedGraphControl.IsEnableVEdit = true;
			zedGraphControl.IsEnableVPan = false;
			zedGraphControl.IsEnableVZoom = false;
			zedGraphControl.IsEnableWheelZoom = false;
			zedGraphControl.IsShowContextMenu = false;
			zedGraphControl.LinkModifierKeys = System.Windows.Forms.Keys.None;
			zedGraphControl.PanModifierKeys = System.Windows.Forms.Keys.None;
			zedGraphControl.ScrollGrace = 0D;
			zedGraphControl.ScrollMaxX = 0D;
			zedGraphControl.ScrollMaxY = 0D;
			zedGraphControl.ScrollMaxY2 = 0D;
			zedGraphControl.ScrollMinX = 0D;
			zedGraphControl.ScrollMinY = 0D;
			zedGraphControl.ScrollMinY2 = 0D;
		}

		private DimmingType? DimmingTypeOption
		{
			get { return (ViewModel as DimmingWizardPageViewModel)?.DimmingTypeOption; }
			set {
				if (ViewModel is DimmingWizardPageViewModel viewModel)
				{
					viewModel.DimmingTypeOption = value.Value;
				}
			}
		}

		public Curve Curve
		{
			get { return _curveEditor?.Curve; }
			set 
			{
				if (_curveEditor != null)
				{
					_curveEditor.Curve = value;
					UpdateViewModel();
				}
			}
		}

		protected override void OnViewModelChanged()
		{
			base.OnViewModelChanged();

			if (DimmingTypeOption == DimmingType.Simple)
			{
				SimpleDimming.Visibility = Visibility.Visible;
				SimpleDimmingButton.IsChecked = true;
			}

			else if (DimmingTypeOption == DimmingType.Library)
			{
				CustomCurve.Visibility = Visibility.Visible;
				LibraryCurveButton.IsChecked = true;
			}

			else
			{
				SimpleDimming.Visibility = Visibility.Collapsed;
				CustomCurve.Visibility = Visibility.Collapsed;
				NoCurveButton.IsChecked = true;
			}
		}

		private void UpdateViewModel()
		{
			if (ViewModel is DimmingWizardPageViewModel viewModel)
			{
				viewModel.Curve = _curveEditor.Curve;
			}
		}

		private void NoCurveButtonClick(object sender, RoutedEventArgs e)
		{
			SimpleDimming.Visibility = Visibility.Collapsed;
			CustomCurve.Visibility = Visibility.Collapsed;
			DimmingTypeOption = DimmingType.NoCurve;
			_curveEditor.Clear();

			UpdateViewModel();
		}

		private void SimpleDimmingButtonClick(object sender, RoutedEventArgs e)
		{
			SimpleDimming.Visibility = Visibility.Visible;
			CustomCurve.Visibility = Visibility.Collapsed;
			DimmingTypeOption = DimmingType.Simple;
			_curveEditor.Clear();
		}

		private void SetupGammaGrid(System.Windows.Controls.Grid grid, int brightness, double gamma)
		{
			if (grid == null)
			{
				return;
			}

			grid.RowDefinitions.Clear();
			grid.ColumnDefinitions.Clear();
			grid.Children.Clear();

			grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) });
			grid.ShowGridLines = false;

			for (int i = 0; i < 10; i++)
			{
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(10, GridUnitType.Star) });

				double y = (Math.Pow((double)(i+1)/10, gamma) * brightness + 0.5) / 100 * 255;
				byte colorValue = (byte)Math.Clamp(y, 0, 255);
				var backgroundColor = System.Windows.Media.Color.FromRgb(colorValue, colorValue, colorValue);

				var cellBorder = new System.Windows.Controls.Border
				{
					BorderBrush = System.Windows.Media.Brushes.Gray,
					BorderThickness = new Thickness(i == 0 ? 1 : 0, 1, 1, 1), // Prevent double-thick internal borders
					Background = new System.Windows.Media.SolidColorBrush(backgroundColor),
					SnapsToDevicePixels = true
					};

				System.Windows.Controls.Grid.SetColumn(cellBorder, i);
				System.Windows.Controls.Grid.SetRow(cellBorder, 0);

				grid.Children.Add(cellBorder);
			}
		}

		private void BrightnessSlider_ValueChanged(object sender, RoutedEventArgs e)
		{
			if (BrightnessSlider != null && GammaSlider != null)
			{
				// Refresh the grid with new values
				SetupGammaGrid(GammaGrid, (int)BrightnessSlider.Value, GammaSlider.Value);
			}
		}

		private void GammaSlider_ValueChanged(object sender, RoutedEventArgs e)
		{
			if (BrightnessSlider != null && GammaSlider != null)
			{
				SetupGammaGrid(GammaGrid, (int)BrightnessSlider.Value, GammaSlider.Value);
			}
		}

		private void LibraryCurveButtonClick(object sender, RoutedEventArgs e)
		{
			SimpleDimming.Visibility = Visibility.Collapsed;
			CustomCurve.Visibility = Visibility.Visible;
			DimmingTypeOption = DimmingType.Library;
		}

		private void ReverseCurveButtonClick(object sender, RoutedEventArgs e)
		{
			_curveEditor.btnReverse_Click(sender, e);

			UpdateViewModel();
		}

		private void InvertCurveButtonClick(object sender, RoutedEventArgs e)
		{
			_curveEditor.btnInvert_Click(sender, e);

			UpdateViewModel();
		}

		private void DrawCurveButtonClick(object sender, RoutedEventArgs e)
		{
			_curveEditor.btnDraw_Click(sender, e);

			UpdateViewModel();
		}

		private void fxCurveButtonClick(object sender, RoutedEventArgs e)
		{
			_curveEditor.btnFunctionCurve_Click(sender, e);

			UpdateViewModel();
		}

		private void LoadCurveButtonClick(object sender, RoutedEventArgs e)
		{
			_curveEditor.buttonLoadCurveFromLibrary_Click(sender, e);
			if (_curveEditor.Curve.LibraryReferenceName != null)
			{
				if (_curveEditor.Curve.IsCurrentLibraryCurve == true)
				{
					CurveLibrary.Text = _curveEditor.LibraryCurveName == null ? "This curve is a library curve." : $"This curve is the library curve: {_curveEditor.LibraryCurveName}";
				}
				else
				{
					CurveLibrary.Text = _curveEditor.Curve.IsLibraryReference == true ? $"This curve is linked to the library curve: {_curveEditor.Curve.LibraryReferenceName}" : "This curve is not linked to any in the library.";
				}

				UnlinkCurveButton.IsEnabled = true;
				SaveCurveButton.IsEnabled = false;
				EditLibraryCurveButton.IsEnabled = true;
				ReverseCurveButton.IsEnabled = false;
				InvertCurveButton.IsEnabled = false;
				XCoord.IsEnabled = false;
				XCoord.Value = 0;
				YCoord.IsEnabled = false;
				YCoord.Value = 0;
			}

			UpdateViewModel();
		}

		private void SaveCurveButtonClick(object sender, RoutedEventArgs e)
		{
			_curveEditor.buttonSaveCurveToLibrary_Click(sender, e);
			if (_curveEditor.Curve.LibraryReferenceName != null)
			{
				if (_curveEditor.Curve.IsCurrentLibraryCurve == true)
				{
					CurveLibrary.Text = _curveEditor.LibraryCurveName == null ? "This curve is a library curve." : $"This curve is the library curve: {_curveEditor.LibraryCurveName}";
				}
				else
				{
					CurveLibrary.Text = _curveEditor.Curve.IsLibraryReference == true ? $"This curve is linked to the library curve: {_curveEditor.Curve.LibraryReferenceName}" : "This curve is not linked to any in the library.";
				}

				UnlinkCurveButton.IsEnabled = true;
			}

			UpdateViewModel();
		}

		private void UnlinkCurveButtonClick(object sender, RoutedEventArgs e)
		{
			_curveEditor.buttonUnlinkCurve_Click(sender, e);
			CurveLibrary.Text = "This curve is not linked to any in the library.";

			UnlinkCurveButton.IsEnabled = false;
			SaveCurveButton.IsEnabled = true;
			EditLibraryCurveButton.IsEnabled = false;
			ReverseCurveButton.IsEnabled = true;
			InvertCurveButton.IsEnabled = true;

			XCoord.IsEnabled = true;
			YCoord.IsEnabled = true;

			UpdateViewModel();
		}

		private void EditLibraryCurveButtonClick(object sender, RoutedEventArgs e)
		{
			_curveEditor.buttonEditLibraryCurve_Click(sender, e);
			if (_curveEditor.Curve.IsCurrentLibraryCurve == true)
			{
				CurveLibrary.Text = _curveEditor.LibraryCurveName == null ? "This curve is a library curve." : $"This curve is the library curve: {_curveEditor.LibraryCurveName}";
			}
			else
			{
				CurveLibrary.Text = _curveEditor.Curve.IsLibraryReference == true ? $"This curve is linked to the library curve: {_curveEditor.Curve.LibraryReferenceName}" : "This curve is not linked to any in the library.";
			}

			UpdateViewModel();
		}

		private void UpdateCoordinatesButton(object sender, RoutedEventArgs e)
		{
			if (zedGraphControl.DragEditingPair != null)
			{
				zedGraphControl.DragEditingPair.X = XCoord.Value != null ? (double)XCoord.Value : 0;
				zedGraphControl.DragEditingPair.Y = YCoord.Value != null ? (double)YCoord.Value : 0;
				zedGraphControl.Invalidate();
			}

			UpdateViewModel();
		}

		private void UpdateDrawIntervalButton(object sender, RoutedEventArgs e)
		{
			if (_curveEditor != null)
			{
				_curveEditor.Threshold = DrawInterval.Value != null ? (int)DrawInterval.Value : 1;
			}
		}

		public bool zedGraphControl_MouseDown(ZedGraphControl sender, System.Windows.Forms.MouseEventArgs e)
		{
			_curveEditor.zedGraphControl_MouseDownEvent(sender, e);
			if (sender.DragEditingPair != null)
			{
				XCoord.Value = sender.DragEditingPair.X.CastToInt16();
				YCoord.Value = sender.DragEditingPair.Y.CastToInt16();
			}

			UpdateViewModel();
			return false;
		}

		public bool zedGraphControl_PreMouseMove(ZedGraphControl sender, System.Windows.Forms.MouseEventArgs e)
		{
			_curveEditor.zedGraphControl_PreMouseMoveEvent(sender, e);
			if (sender.DragEditingPair != null)
			{
				XCoord.Value = sender.DragEditingPair.X.CastToInt16();
				YCoord.Value = sender.DragEditingPair.Y.CastToInt16();
			}

			UpdateViewModel();
			return false;
		}
	}
}
