using Catel.Data;
using Catel.Reflection;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using Vixen.Extensions;
using VixenApplication.SetupDisplay.Wizards.ViewModels;
using VixenModules.App.Curves;
using ZedGraph;

namespace VixenApplication.SetupDisplay.Wizards.Views
{
	public partial class DimmingWizardPageView : INotifyPropertyChanged
	{
		private CurveEditor _curveEditor;

		public DimmingWizardPageView()
		{
			InitializeComponent();


			SimpleCurve.Visibility = Visibility.Collapsed;
			CustomCurve.Visibility = Visibility.Collapsed;
			RampUpLabel.Content = CurveType.RampUp.GetEnumDescription();
			RampDownLabel.Content = CurveType.RampDown.GetEnumDescription();
			RampIncreaseCurveLabel.Content = CurveType.RampIncreaseCurve.GetEnumDescription();
			RampDecreaseCurveLabel.Content = CurveType.RampDecreaseCurve.GetEnumDescription();

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

		private void UpdateViewModel()
		{
			if (ViewModel is DimmingWizardPageViewModel viewModel)
			{
				viewModel.Curve = _curveEditor.Curve;
			}
		}

		private void NoCurveButtonClick(object sender, RoutedEventArgs e)
		{
			SimpleCurve.Visibility = Visibility.Collapsed;
			CustomCurve.Visibility = Visibility.Collapsed;
			_curveEditor.Clear();

			UpdateViewModel();
		}

		private void SimpleCurveButtonClick(object sender, RoutedEventArgs e)
		{
			SimpleCurve.Visibility = Visibility.Visible;
			CustomCurve.Visibility = Visibility.Collapsed;
			_curveEditor.Clear();
		}

		private void LibraryCurveButtonClick(object sender, RoutedEventArgs e)
		{
			SimpleCurve.Visibility = Visibility.Collapsed;
			CustomCurve.Visibility = Visibility.Visible;
		}

		private void RampUpButtonClick(object sender, RoutedEventArgs e)
		{
			_curveEditor.Curve = new Curve(CurveType.RampUp);
			SimpleDimmingCurveButton.Content = Regex.Replace(SimpleDimmingCurveButton.Content.ToString(), "-.*", $"- {CurveType.RampUp.GetEnumDescription()}");

			UpdateViewModel();
		}

		private void RampDownButtonClick(object sender, RoutedEventArgs e)
		{
			_curveEditor.Curve = new Curve(CurveType.RampDown);
			SimpleDimmingCurveButton.Content = Regex.Replace(SimpleDimmingCurveButton.Content.ToString(), "-.*", $"- {CurveType.RampDown.GetEnumDescription()}");

			UpdateViewModel();
		}

		private void RampIncreaseCurveButtonClick(object sender, RoutedEventArgs e)
		{
			_curveEditor.Curve = new Curve(CurveType.RampIncreaseCurve);
			SimpleDimmingCurveButton.Content = Regex.Replace(SimpleDimmingCurveButton.Content.ToString(), "-.*", $"- {CurveType.RampIncreaseCurve.GetEnumDescription()}");

			UpdateViewModel();
		}

		private void RampDecreaseCurveButtonClick(object sender, RoutedEventArgs e)
		{
			_curveEditor.Curve = new Curve(CurveType.RampDecreaseCurve);
			SimpleDimmingCurveButton.Content = Regex.Replace(SimpleDimmingCurveButton.Content.ToString(), "-.*", $"- {CurveType.RampDecreaseCurve.GetEnumDescription()}");

			UpdateViewModel();
		}

		private void GrowingDecreaseButtonClick(object sender, RoutedEventArgs e)
		{
			_curveEditor.Curve = new Curve(CurveType.RampDown);
			SimpleDimmingCurveButton.Content = Regex.Replace(SimpleDimmingCurveButton.Content.ToString(), "-.*", $"- {CurveType.RampDown.GetEnumDescription()}");
			_curveEditor.LibraryCurveName = CurveType.RampUp.GetEnumDescription();

			UpdateViewModel();
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
				XCoord.Text = string.Empty;
				YCoord.IsEnabled = false;
				YCoord.Text = string.Empty;
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
