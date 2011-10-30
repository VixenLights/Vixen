using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Module.App;
using ZedGraph;

namespace VixenModules.App.Curves
{
	public partial class CurveEditor : Form
	{
		public CurveEditor(PointPairList points)
		{
			InitializeComponent();

			Points = points;

			zedGraphControl.EditModifierKeys = Keys.None;
			zedGraphControl.IsShowContextMenu = false;
			zedGraphControl.IsEnableSelection = false;
			zedGraphControl.EditButtons = System.Windows.Forms.MouseButtons.Left;
			zedGraphControl.GraphPane.XAxis.Scale.Min = 0;
			zedGraphControl.GraphPane.XAxis.Scale.Max = 100;
			zedGraphControl.GraphPane.YAxis.Scale.Min = 0;
			zedGraphControl.GraphPane.YAxis.Scale.Max = 100;
			zedGraphControl.GraphPane.XAxis.Title.IsVisible = false;
			zedGraphControl.GraphPane.YAxis.Title.IsVisible = false;
			zedGraphControl.GraphPane.Legend.IsVisible = false;
			zedGraphControl.GraphPane.Title.IsVisible = false;
			zedGraphControl.GraphPane.Chart.Fill.Color = SystemColors.Control;
			zedGraphControl.GraphPane.Fill = new Fill(SystemColors.Control);
			zedGraphControl.GraphPane.Border = new Border(SystemColors.Control, 0);
			zedGraphControl.GraphPane.AxisChange();
		}

		public CurveEditor(Curve curve)
			: this(curve.Points)
		{
		}

		public PointPairList Points
		{
			get
			{
				if (zedGraphControl.GraphPane.CurveList.Count > 0)
					return zedGraphControl.GraphPane.CurveList[0].Points as PointPairList;
				else
					return null;
			}
			set
			{
				zedGraphControl.GraphPane.CurveList.Clear();
				PointPairList ppl = value.Clone();
				ppl.Sort();
				zedGraphControl.GraphPane.AddCurve("", ppl, Color.Gray);
				zedGraphControl.Invalidate();
				Modified = false;
			}
		}

		public Curve Curve
		{
			get
			{
				return new Curve(Points);
			}
			set
			{
				Points = value.Points;
			}
		}

		public bool Modified { get; internal set; }

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

			// actually does nothing, just haven't changed the event handler definition
			return true;
		}

		private bool zedGraphControl_MouseDownEvent(ZedGraphControl sender, MouseEventArgs e)
		{
			CurveItem curve;
			int dragPointIndex;

			// if CTRL is pressed, and we're not near a specific point, add a new point
			if (Control.ModifierKeys.HasFlag(Keys.Control) && !zedGraphControl.GraphPane.FindNearestPoint(e.Location, out curve, out dragPointIndex)) {
				// only add if we've actually clicked on the pane, so make sure the mouse is over it first
				if (zedGraphControl.MasterPane.FindPane(e.Location) != null) {
					PointPairList pointList = zedGraphControl.GraphPane.CurveList[0].Points as PointPairList;
					double newX, newY;
					zedGraphControl.GraphPane.ReverseTransform(e.Location, out newX, out newY);
					pointList.Insert(0, newX, newY);
					pointList.Sort();
					zedGraphControl.Invalidate();
					Modified = true;
				}
			}
			// if the ALT key was pressed, and we're near a point, delete it -- but only if there would be at least two points left
			if (Control.ModifierKeys.HasFlag(Keys.Alt) && zedGraphControl.GraphPane.FindNearestPoint(e.Location, out curve, out dragPointIndex)) {
				PointPairList pointList = zedGraphControl.GraphPane.CurveList[0].Points as PointPairList;
				if (pointList.Count > 2) {
					pointList.RemoveAt(dragPointIndex);
					pointList.Sort();
					zedGraphControl.Invalidate();
					Modified = true;
				}
			}

			return false;
		}

		private bool zedGraphControl_MouseUpEvent(ZedGraphControl sender, MouseEventArgs e)
		{
			return false;
		}
	}
}
