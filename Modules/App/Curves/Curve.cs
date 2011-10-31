using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Drawing;
using Vixen.Module;
using Vixen.Module.App;
using ZedGraph;

namespace VixenModules.App.Curves
{
	[Serializable]
	public class Curve
	{
		public static Color CurveGridColor = Color.RoyalBlue;
	
		public Curve(IPointList points)
		{
			Points = new PointPairList(points);
		}

		public Curve(Curve curve)
			: this(curve.Points)
		{
		}

		// default Curve constructor makes a ramp with x = y.
		public Curve()
			: this(new PointPairList(new double[] { 2.0, 98.0 }, new double[] { 2.0, 98.0 }))
		{
		}

		public PointPairList Points { get; internal set; }


		public double GetValue(double x)
		{
			if (x > 100.0) x = 100.0;
			if (x < 0.0) x = 0.0;

			double returnValue = Points.InterpolateX(x);

			if (returnValue > 100.0) returnValue = 100.0;
			if (returnValue < 0.0) returnValue = 0.0;

			return returnValue;
		}

		public double GetValue(int x)
		{
			return GetValue((double)x);
		}

		public int GetIntValue(double x)
		{
			return (int)Math.Round(GetValue(x), 0);
		}

		public int GetIntValue(int x)
		{
			return GetIntValue((double)x);
		}

		/// <summary>
		/// Opens up the curve editor for this particular curve, and lets the user modify it.
		/// </summary>
		/// <returns> true if the curve was modified, false if it was not.</returns>
		public bool EditCurve()
		{
			using (CurveEditor editor = new CurveEditor(Points)) {
				DialogResult result = editor.ShowDialog();
				if (result == DialogResult.OK && editor.Modified) {
					Points = editor.Points;
					return true;
				} else {
					return false;
				}
			}
		}

		public Bitmap GenerateCurveImage(Size size)
		{
			GraphPane pane = new GraphPane(new RectangleF(0, 0, size.Width, size.Height), "", "", "");
			Bitmap result = new Bitmap(size.Width, size.Height);

			pane.AddCurve("", Points, CurveGridColor);

			pane.XAxis.Scale.Min = 0;
			pane.XAxis.Scale.Max = 100;
			pane.YAxis.Scale.Min = 0;
			pane.YAxis.Scale.Max = 100;
			pane.XAxis.IsVisible = false;
			pane.YAxis.IsVisible = false;
			pane.Legend.IsVisible = false;
			pane.Title.IsVisible = false;
	
			pane.Chart.Fill.Color = SystemColors.Control;
			pane.Fill = new Fill(SystemColors.Control);
			pane.Border = new Border(SystemColors.Control, 0);

			using (Graphics g = Graphics.FromImage(result)) {
				pane.AxisChange(g);
				result = pane.GetImage(true);
			}

			return result;
		}
	}
}
