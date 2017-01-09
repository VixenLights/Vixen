using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Common.Controls;
using Common.Resources.Properties;
using Vixen.Module.App;
using Vixen.Services;
using ZedGraph;
using Common.Controls.Theme;

namespace VixenModules.App.Curves
{
	public partial class CurveEditor : BaseForm
	{
		public CurveEditor()
		{
			InitializeComponent();
			Icon = Resources.Icon_Vixen3;

			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);

			zedGraphControl.GraphPane.XAxis.MajorGrid.IsVisible = true;
			zedGraphControl.GraphPane.XAxis.MajorGrid.Color = ThemeColorTable.GroupBoxBorderColor;
			zedGraphControl.GraphPane.XAxis.MajorGrid.DashOff = 4;
			zedGraphControl.GraphPane.XAxis.MajorGrid.DashOn = 2;

			zedGraphControl.GraphPane.YAxis.MajorGrid.IsVisible = true;
			zedGraphControl.GraphPane.YAxis.MajorGrid.Color = ThemeColorTable.GroupBoxBorderColor;
			zedGraphControl.GraphPane.YAxis.MajorGrid.DashOff = 4;
			zedGraphControl.GraphPane.YAxis.MajorGrid.DashOn = 2;

			zedGraphControl.EditModifierKeys = Keys.None;
			zedGraphControl.IsShowContextMenu = false;
			zedGraphControl.IsEnableSelection = false;
			zedGraphControl.EditButtons = MouseButtons.Left;
			zedGraphControl.GraphPane.XAxis.Scale.Min = 0;
			zedGraphControl.GraphPane.XAxis.Scale.Max = 100;
			zedGraphControl.GraphPane.YAxis.Scale.Min = 0;
			zedGraphControl.GraphPane.YAxis.Scale.Max = 100;
			zedGraphControl.GraphPane.XAxis.Title.IsVisible = false;
			zedGraphControl.GraphPane.YAxis.Title.IsVisible = false;
			zedGraphControl.GraphPane.Legend.IsVisible = false;
			zedGraphControl.GraphPane.Title.IsVisible = false;

			zedGraphControl.GraphPane.Fill = new Fill(ThemeColorTable.BackgroundColor);
			zedGraphControl.GraphPane.Border = new Border(ThemeColorTable.BackgroundColor, 0);

			zedGraphControl.GraphPane.AxisChange();
		}

		public CurveEditor(Curve curve)
			: this()
		{
			Curve = curve;
		}

		private Curve _curve;

		public Curve Curve
		{
			get { return _curve; }
			set
			{
				_curve = new Curve(value);
				PopulateFormWithCurve(_curve);
			}
		}

		private string _libraryCurveName;

		public string LibraryCurveName
		{
			get { return _libraryCurveName; }
			set
			{
				_libraryCurveName = value;
				PopulateFormWithCurve(Curve);
			}
		}

		private CurveLibrary _library;

		private CurveLibrary Library
		{
			get
			{
				if (_library == null)
					_library = ApplicationServices.Get<IAppModuleInstance>(CurveLibraryDescriptor.ModuleID) as CurveLibrary;

				return _library;
			}
		}

		public bool ReadonlyCurve { get; internal set; }

		private bool zedGraphControl_PreMouseMoveEvent(ZedGraphControl sender, MouseEventArgs e)
		{
			if (zedGraphControl.IsEditing) {
				PointPairList pointList = zedGraphControl.GraphPane.CurveList[0].Points as PointPairList;
				pointList.Sort();
			}
			return false;
		}

		private bool zedGraphControl_PostMouseMoveEvent(ZedGraphControl sender, MouseEventArgs e)
		{
			if (sender.DragEditingPair == null)
				return true;

			if (sender.DragEditingPair.X < 0)
				sender.DragEditingPair.X = 0;
			if (sender.DragEditingPair.X > 100)
				sender.DragEditingPair.X = 100;
			if (sender.DragEditingPair.Y < 0)
				sender.DragEditingPair.Y = 0;
			if (sender.DragEditingPair.Y > 100)
				sender.DragEditingPair.Y = 100;

			if (!Curve.IsLibraryReference && e.Button == MouseButtons.Left)
			{
				txtXValue.Text = sender.DragEditingPair.X.ToString();
				txtYValue.Text = sender.DragEditingPair.Y.ToString();
				txtXValue.Enabled = txtYValue.Enabled = btnUpdateCoordinates.Enabled = true;
			}
			// actually does nothing, just haven't changed the event handler definition
			return true;
		}

		private bool zedGraphControl_MouseDownEvent(ZedGraphControl sender, MouseEventArgs e)
		{
			if (ReadonlyCurve)
				return false;

			CurveItem curve;
			int dragPointIndex;

			// if CTRL is pressed, and we're not near a specific point, add a new point
			if (Control.ModifierKeys.HasFlag(Keys.Control) &&
			    !zedGraphControl.GraphPane.FindNearestPoint(e.Location, out curve, out dragPointIndex)) {
				// only add if we've actually clicked on the pane, so make sure the mouse is over it first
				if (zedGraphControl.MasterPane.FindPane(e.Location) != null) {
					PointPairList pointList = zedGraphControl.GraphPane.CurveList[0].Points as PointPairList;
					double newX, newY;
					zedGraphControl.GraphPane.ReverseTransform(e.Location, out newX, out newY);
					//Verify the point is in the usable bounds.
					if (newX > 100)
					{
						newX = 100;
					}
					else if(newX < 0)
					{
						newX = 0;
					}
					if (newY > 100)
					{
						newY = 100;
					}
					else if (newY < 0)
					{
						newY = 0;
					}
					pointList.Insert(0, newX, newY);
					pointList.Sort();
					zedGraphControl.Invalidate();
				}
			}
			// if the ALT key was pressed, and we're near a point, delete it -- but only if there would be at least two points left
			if (Control.ModifierKeys.HasFlag(Keys.Alt) &&
			    zedGraphControl.GraphPane.FindNearestPoint(e.Location, out curve, out dragPointIndex)) {
				PointPairList pointList = zedGraphControl.GraphPane.CurveList[0].Points as PointPairList;
				if (pointList.Count > 2) {
					pointList.RemoveAt(dragPointIndex);
					pointList.Sort();
					zedGraphControl.Invalidate();
				}
			}

			if (!Curve.IsLibraryReference && e.Button == MouseButtons.Left && sender.DragEditingPair != null)
			{
				txtXValue.Text = sender.DragEditingPair.X.ToString();
				txtYValue.Text = sender.DragEditingPair.Y.ToString();
				txtXValue.Enabled = txtYValue.Enabled = btnUpdateCoordinates.Enabled = true;
			}

			return false;
		}

		private bool zedGraphControl_MouseUpEvent(ZedGraphControl sender, MouseEventArgs e)
		{
			return false;
		}

		private void PopulateFormWithCurve(Curve curve)
		{
			zedGraphControl.GraphPane.Chart.Fill = new Fill(ThemeColorTable.BackgroundColor);
			zedGraphControl.GraphPane.Chart.Border.Color = ThemeColorTable.GroupBoxBorderColor;
			zedGraphControl.GraphPane.XAxis.MajorGrid.Color = ThemeColorTable.GroupBoxBorderColor;
			zedGraphControl.GraphPane.XAxis.MinorGrid.Color = ThemeColorTable.GroupBoxBorderColor;
			zedGraphControl.GraphPane.XAxis.MajorTic.Color = ThemeColorTable.GroupBoxBorderColor;
			zedGraphControl.GraphPane.XAxis.MinorTic.Color = ThemeColorTable.GroupBoxBorderColor;
			zedGraphControl.GraphPane.YAxis.MajorGrid.Color = ThemeColorTable.GroupBoxBorderColor;
			zedGraphControl.GraphPane.YAxis.MinorGrid.Color = ThemeColorTable.GroupBoxBorderColor;
			zedGraphControl.GraphPane.YAxis.MajorTic.Color = ThemeColorTable.GroupBoxBorderColor;
			zedGraphControl.GraphPane.YAxis.MinorTic.Color = ThemeColorTable.GroupBoxBorderColor;
			zedGraphControl.GraphPane.Title.FontSpec.FontColor = ThemeColorTable.ForeColor;
			zedGraphControl.GraphPane.XAxis.Scale.FontSpec.FontColor = ThemeColorTable.ForeColor;
			zedGraphControl.GraphPane.YAxis.Scale.FontSpec.FontColor = ThemeColorTable.ForeColor;

			// if we're editing a curve from the library, treat it special
			if (curve.IsCurrentLibraryCurve) {
				zedGraphControl.GraphPane.CurveList.Clear();
				zedGraphControl.DragEditingPair = null;
				zedGraphControl.GraphPane.AddCurve(string.Empty, curve.Points, Curve.ActiveCurveGridColor);
				if (LibraryCurveName == null) {
					labelCurve.Text = "This curve is a library curve.";
					Text = "Curve Editor: Library Curve";
				}
				else {
					labelCurve.Text = "This curve is the library curve: " + LibraryCurveName;
					Text = "Curve Editor: Library Curve " + LibraryCurveName;
				}

				zedGraphControl.IsEnableHEdit = true;
				zedGraphControl.IsEnableVEdit = true;
				ReadonlyCurve = false;
				buttonSaveCurveToLibrary.Enabled = false;
				buttonLoadCurveFromLibrary.Enabled = false;
				buttonUnlinkCurve.Enabled = false;
				buttonEditLibraryCurve.Enabled = false;
				btnInvert.Enabled = true;
				btnReverse.Enabled = true;
				labelInstructions1.Visible = true;
				labelInstructions2.Visible = true;
				txtXValue.Enabled = false;
				txtYValue.Enabled = false;
				txtXValue.Text = string.Empty;
				txtYValue.Text = string.Empty;
				btnUpdateCoordinates.Enabled = false;
				
			}
			else {
				if (curve.IsLibraryReference) {
					zedGraphControl.GraphPane.CurveList.Clear();
					LineItem item = zedGraphControl.GraphPane.AddCurve(string.Empty, curve.Points, Curve.InactiveCurveGridColor);
					item.Symbol.Fill = new Fill(Curve.InactiveCurveGridColor);
					labelCurve.Text = "This curve is linked to the library curve: " + curve.LibraryReferenceName;
				}
				else {
					zedGraphControl.GraphPane.CurveList.Clear();
					LineItem item = zedGraphControl.GraphPane.AddCurve(string.Empty, curve.Points, Curve.ActiveCurveGridColor);
					item.Symbol.Fill = new Fill(Curve.ActiveCurveGridColor);
					labelCurve.Text = "This curve is not linked to any in the library.";
				}
				zedGraphControl.DragEditingPair = null;
				zedGraphControl.IsEnableHEdit = !curve.IsLibraryReference;
				zedGraphControl.IsEnableVEdit = !curve.IsLibraryReference;
				ReadonlyCurve = curve.IsLibraryReference;
				buttonSaveCurveToLibrary.Enabled = !curve.IsLibraryReference;
				buttonLoadCurveFromLibrary.Enabled = true;
				buttonUnlinkCurve.Enabled = curve.IsLibraryReference;
				buttonEditLibraryCurve.Enabled = curve.IsLibraryReference;
				btnInvert.Enabled = !curve.IsLibraryReference;
				btnReverse.Enabled = !curve.IsLibraryReference;
				labelInstructions1.Visible = !curve.IsLibraryReference;
				labelInstructions2.Visible = !curve.IsLibraryReference;
				txtYValue.Enabled = !curve.IsLibraryReference;
				txtXValue.Enabled = !curve.IsLibraryReference;
				txtXValue.Text = string.Empty;
				txtYValue.Text = string.Empty;
				btnUpdateCoordinates.Enabled = false;
				
				Text = "Curve Editor";
			}

			zedGraphControl.Invalidate();
		}

		private void buttonLoadCurveFromLibrary_Click(object sender, EventArgs e)
		{
			CurveLibrarySelector selector = new CurveLibrarySelector();
			if (selector.ShowDialog() == DialogResult.OK && selector.SelectedItem != null) {
				// make a new curve that references the selected library curve, and set it to the current Curve
				Curve newCurve = new Curve(selector.SelectedItem.Item2);
				newCurve.LibraryReferenceName = selector.SelectedItem.Item1;
				newCurve.IsCurrentLibraryCurve = false;
				Curve = newCurve;
			}
		}

		private void buttonSaveCurveToLibrary_Click(object sender, EventArgs e)
		{
			Common.Controls.TextDialog dialog = new Common.Controls.TextDialog("Curve name?");

			while (dialog.ShowDialog() == DialogResult.OK) {
				if (dialog.Response == string.Empty) {
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("Please enter a name.", "Curve Name", false, false);
					messageBox.ShowDialog();
					continue;
				}

				if (Library.Contains(dialog.Response)) {
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Question; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("There is already a curve with that name. Do you want to overwrite it?", "Overwrite curve?", true, true);
					messageBox.ShowDialog();
					if (messageBox.DialogResult == DialogResult.OK) {
						Library.AddCurve(dialog.Response, new Curve(Curve));
						break;
					}
					if (messageBox.DialogResult == DialogResult.Cancel) {
						break;
					}
				}
				else {
					Library.AddCurve(dialog.Response, new Curve(Curve));
					break;
				}
			}
		}

		private void buttonUnlinkCurve_Click(object sender, EventArgs e)
		{
			Curve.UnlinkFromLibraryCurve();
			PopulateFormWithCurve(Curve);
		}

		private void buttonEditLibraryCurve_Click(object sender, EventArgs e)
		{
			string libraryName = Curve.LibraryReferenceName;

			Library.EditLibraryCurve(libraryName);

			PopulateFormWithCurve(Curve);
		}

		private void btnReverse_Click(object sender, EventArgs e)
		{

			foreach (var curveItem in zedGraphControl.GraphPane.CurveList)
			{
				for (int i = 0; i < curveItem.Points.Count; i++)
				{
					curveItem.Points[i].X = 100 - curveItem.Points[i].X;
				}
				var points = curveItem.Points as PointPairList;
				if (points != null)
				{
					points.Sort();
				}

			}

			zedGraphControl.Invalidate();
		}

		private void btnInvert_Click(object sender, EventArgs e)
		{
			foreach (var curveItem in zedGraphControl.GraphPane.CurveList)
			{
				for (int i = 0; i < curveItem.Points.Count; i++)
				{
					curveItem.Points[i].Y = 100 - curveItem.Points[i].Y;
				}
				
			}
			zedGraphControl.Invalidate();
		}

		private void btnUpdateCoordinates_Click(object sender, EventArgs e)
		{
			zedGraphControl.DragEditingPair.X = Convert.ToDouble(txtXValue.Text);
			zedGraphControl.DragEditingPair.Y = Convert.ToDouble(txtYValue.Text);
			zedGraphControl.Invalidate();
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImage;

		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}
	}
}