using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Common.Controls.ColorManagement.ColorModels;

namespace Common.Controls.ColorManagement.ColorPicker
{
	public abstract class ColorSelectionModuleHSV : ColorSelectionModule
	{
		#region variables

		protected HSV _color;

		#endregion

		protected static ColorBlend GetHueBlend(double s, double v)
		{
			ColorBlend ret = new ColorBlend();
			ret.Colors = new Color[]
			             	{
			             		new HSV(0.0, s, v).ToRGB(), new HSV(0.1666, s, v).ToRGB(), new HSV(0.3333, s, v).ToRGB(),
			             		new HSV(0.5, s, v).ToRGB(), new HSV(0.6666, s, v).ToRGB(), new HSV(0.8333, s, v).ToRGB(),
			             		new HSV(1.0, s, v).ToRGB()
			             	};
			ret.Positions = new float[]
			                	{
			                		0f, 0.1666f, 0.3333f, 0.5f, 0.6666f, 0.8333f, 1f
			                	};
			return ret;
		}

		#region properties

		public override XYZ XYZ
		{
			get { return XYZ.FromRGB(_color.ToRGB()); }
			set
			{
				HSV newcolor = HSV.FromRGB(value.ToRGB());
				if (newcolor == _color) return;
				_color = newcolor;
				//update controls
				UpdatePlaneImage();
				UpdatePlanePosition();
				UpdateFaderImage();
				UpdateFaderPosition();
			}
		}

		#endregion
	}

	public class ColorSelectionModuleHSV_H : ColorSelectionModuleHSV
	{
		#region colorfader

		protected override void OnUpdateFaderImage(Bitmap bmp)
		{
			using (Graphics gr = Graphics.FromImage(bmp)) {
				using (LinearGradientBrush brs = new LinearGradientBrush(
					new Point(0, 0), new Point(bmp.Width),
					Color.Black, Color.White)) {
					brs.InterpolationColors = GetHueBlend(1.0, 1.0);
					gr.FillRectangle(brs, new Rectangle(Point.Empty, bmp.Size));
				}
			}
		}

		protected override void OnUpdateFaderPosition(ColorSelectionFader fader)
		{
			fader.Position = _color.H;
		}

		protected override void OnFaderScroll(ColorSelectionFader fader)
		{
			double newhue = fader.Position;
			if (newhue == _color.H) return;
			_color.H = newhue;
			UpdatePlaneImage();
			RaiseColorChanged();
		}

		#endregion

		#region colorplane

		protected override void OnUpdatePlaneImage(Bitmap bmp)
		{
			using (Graphics gr = Graphics.FromImage(bmp)) {
				using (LinearGradientBrush brs = new LinearGradientBrush(
					new Point(0, 0), new Point(1, 0),
					Color.White, new HSV(_color.H, 100, 100).ToRGB())) {
					//draw saturation
					brs.Transform = new Matrix((float) bmp.Width, 0f, 0f, 1f, 0f, 0f);
					gr.FillRectangle(brs, new Rectangle(Point.Empty, bmp.Size));
					//draw luminance
					brs.Transform = new Matrix(0f, (float) bmp.Height, 1f, 0f, 0f, 0f);
					brs.LinearColors = new Color[] {Color.FromArgb(0, 0, 0, 0), Color.Black};
					gr.FillRectangle(brs, new Rectangle(Point.Empty, bmp.Size));
				}
			}
		}

		protected override void OnUpdatePlanePosition(ColorSelectionPlane plane)
		{
			plane.SetPosition(_color.S, 1.0 - _color.V);
		}

		protected override void OnPlaneScroll(ColorSelectionPlane plane)
		{
			double newsaturation = XYZ.ClipValue(plane.PositionX, 0.0, 1.0),
			       newvalue = 1.0 - XYZ.ClipValue(plane.PositionY, 0.0, 1.0);
			if (newsaturation == _color.S &&
			    newvalue == _color.V) return;
			_color.S = newsaturation;
			_color.V = newvalue;
			RaiseColorChanged();
		}

		#endregion
	}

	public class ColorSelectionModuleHSV_S : ColorSelectionModuleHSV
	{
		#region colorfader

		protected override void OnUpdateFaderImage(Bitmap bmp)
		{
			using (Graphics gr = Graphics.FromImage(bmp)) {
				using (LinearGradientBrush brs = new LinearGradientBrush(
					new Point(0, 0), new Point(bmp.Width),
					new HSV(_color.H, 0.0, _color.V).ToRGB(),
					new HSV(_color.H, 1.0, _color.V).ToRGB())) {
					gr.FillRectangle(brs, new Rectangle(Point.Empty, bmp.Size));
				}
			}
		}

		protected override void OnUpdateFaderPosition(ColorSelectionFader fader)
		{
			fader.Position = _color.S;
		}

		protected override void OnFaderScroll(ColorSelectionFader fader)
		{
			double newsaturation = fader.Position;
			if (newsaturation == _color.S) return;
			_color.S = newsaturation;
			UpdatePlaneImage();
			RaiseColorChanged();
		}

		#endregion

		#region colorplane

		protected override void OnUpdatePlaneImage(Bitmap bmp)
		{
			using (Graphics gr = Graphics.FromImage(bmp)) {
				using (LinearGradientBrush brs = new LinearGradientBrush(
					new Point(0, 0), new Point(1, 0),
					Color.White, Color.White)) {
					//draw hue
					brs.Transform = new Matrix((float) bmp.Width, 0f, 0f, 1f, 0f, 0f);
					brs.InterpolationColors = GetHueBlend(_color.S, 1.0);
					gr.FillRectangle(brs, new Rectangle(Point.Empty, bmp.Size));
					//draw value
					brs.Transform = new Matrix(0f, (float) bmp.Height, 1f, 0f, 0f, 0f);
					ColorBlend blnd = new ColorBlend();
					blnd.Colors = new Color[] {Color.FromArgb(0, 0, 0, 0), Color.Black};
					blnd.Positions = new float[] {0f, 1f};
					brs.InterpolationColors = blnd;
					gr.FillRectangle(brs, new Rectangle(Point.Empty, bmp.Size));
				}
			}
		}

		protected override void OnUpdatePlanePosition(ColorSelectionPlane plane)
		{
			plane.SetPosition(_color.H, 1.0 - _color.V);
		}

		protected override void OnPlaneScroll(ColorSelectionPlane plane)
		{
			double newhue = XYZ.ClipValue(plane.PositionX, 0.0, 1.0),
			       newvalue = 1.0 - XYZ.ClipValue(plane.PositionY, 0.0, 1.0);
			if (newhue == _color.H &&
			    newvalue == _color.V) return;
			_color.H = newhue;
			_color.V = newvalue;
			UpdateFaderImage();
			RaiseColorChanged();
		}

		#endregion
	}

	public class ColorSelectionModuleHSV_V : ColorSelectionModuleHSV
	{
		#region colorfader

		protected override void OnUpdateFaderImage(Bitmap bmp)
		{
			using (Graphics gr = Graphics.FromImage(bmp)) {
				using (LinearGradientBrush brs = new LinearGradientBrush(
					new Point(0, 0), new Point(bmp.Width),
					Color.Black, new HSV(_color.H, _color.S, 1.0).ToRGB()
					)) {
					gr.FillRectangle(brs, new Rectangle(Point.Empty, bmp.Size));
				}
			}
		}

		protected override void OnUpdateFaderPosition(ColorSelectionFader fader)
		{
			fader.Position = _color.V;
		}

		protected override void OnFaderScroll(ColorSelectionFader fader)
		{
			double newvalue = fader.Position;
			if (newvalue == _color.V) return;
			_color.V = newvalue;
			UpdatePlaneImage();
			RaiseColorChanged();
		}

		#endregion

		#region colorplane

		protected override void OnUpdatePlaneImage(Bitmap bmp)
		{
			using (Graphics gr = Graphics.FromImage(bmp)) {
				using (LinearGradientBrush brs = new LinearGradientBrush(
					new Point(0, 0), new Point(1, 0),
					Color.White, Color.White)) {
					//draw hue
					brs.Transform = new Matrix((float) bmp.Width, 0f, 0f, 1f, 0f, 0f);
					brs.InterpolationColors = GetHueBlend(1.0, _color.V);
					gr.FillRectangle(brs, new Rectangle(Point.Empty, bmp.Size));
					//draw value
					brs.Transform = new Matrix(0f, (float) bmp.Height, 1f, 0f, 0f, 0f);
					ColorBlend blnd = new ColorBlend();
					Color zerosat = new HSV(0.0, 0.0, _color.V).ToRGB();
					blnd.Colors = new Color[] {Color.FromArgb(0, zerosat), zerosat};
					blnd.Positions = new float[] {0f, 1f};
					brs.InterpolationColors = blnd;
					gr.FillRectangle(brs, new Rectangle(Point.Empty, bmp.Size));
				}
			}
		}

		protected override void OnUpdatePlanePosition(ColorSelectionPlane plane)
		{
			plane.SetPosition(_color.H, 1.0 - _color.S);
		}

		protected override void OnPlaneScroll(ColorSelectionPlane plane)
		{
			double newhue = XYZ.ClipValue(plane.PositionX, 0.0, 1.0),
			       newsaturation = 1.0 - XYZ.ClipValue(plane.PositionY, 0.0, 1.0);
			if (newhue == _color.H &&
			    newsaturation == _color.S) return;
			_color.H = newhue;
			_color.S = newsaturation;
			UpdateFaderImage();
			RaiseColorChanged();
		}

		#endregion
	}
}