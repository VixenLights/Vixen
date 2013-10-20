using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Common.Controls.ColorManagement.ColorModels;

namespace Common.Controls.ColorManagement.ColorPicker
{
	public abstract class ColorSelectionModuleRGB : ColorSelectionModule
	{
		#region variables

		protected Color _color;

		#endregion

		#region properties

		public override XYZ XYZ
		{
			get { return XYZ.FromRGB(_color); }
			set
			{
				Color newcolor = value.ToRGB();
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

	public class ColorSelectionModuleRGB_R : ColorSelectionModuleRGB
	{
		#region colorfader

		protected override void OnUpdateFaderImage(Bitmap bmp)
		{
			using (Graphics gr = Graphics.FromImage(bmp)) {
				using (LinearGradientBrush brs = new LinearGradientBrush(
					new Point(0, 0), new Point(bmp.Width),
					Color.FromArgb(0, _color.G, _color.B),
					Color.FromArgb(255, _color.G, _color.B))) {
					gr.FillRectangle(brs, new Rectangle(Point.Empty, bmp.Size));
				}
			}
		}

		protected override void OnUpdateFaderPosition(ColorSelectionFader fader)
		{
			fader.Position = (double) (_color.R)/255.0;
		}

		protected override void OnFaderScroll(ColorSelectionFader fader)
		{
			int newred = Math.Max(0, Math.Min(255, (int) (fader.Position*255.0)));
			if (newred == _color.R) return;
			_color = Color.FromArgb(newred, _color.G, _color.B);
			UpdatePlaneImage();
			RaiseColorChanged();
		}

		#endregion

		#region colorplane

		protected override void OnUpdatePlaneImage(Bitmap bmp)
		{
			double green = 255.0,
			       delta_green = -255.0/(double) bmp.Height;
			Color[] lincols = new Color[2];
			using (Graphics gr = Graphics.FromImage(bmp)) {
				using (LinearGradientBrush brs = new LinearGradientBrush(
					new Point(0, 0), new Point(bmp.Width, 0),
					Color.White, Color.White)) {
					for (int y = 0; y < bmp.Height; y++, green += delta_green) {
						int g = Math.Max(0, Math.Min(255, (int) green));
						lincols[0] = Color.FromArgb(_color.R, g, 0);
						lincols[1] = Color.FromArgb(_color.R, g, 255);

						brs.LinearColors = lincols;
						gr.FillRectangle(brs, 0, y, bmp.Width, 1);
					}
				}
			}
		}

		protected override void OnUpdatePlanePosition(ColorSelectionPlane plane)
		{
			plane.SetPosition((double) (_color.B)/255.0,
			                  1.0 - (double) (_color.G)/255.0);
		}

		protected override void OnPlaneScroll(ColorSelectionPlane plane)
		{
			int newgreen = Math.Max(0, Math.Min(255, (int) (255.0 - plane.PositionY*255.0))),
			    newblue = Math.Max(0, Math.Min(255, (int) (plane.PositionX*255.0)));
			if (_color.G == newgreen &&
			    _color.B == newblue) return;
			_color = Color.FromArgb(_color.R, newgreen, newblue);
			UpdateFaderImage();
			RaiseColorChanged();
		}

		#endregion
	}

	public class ColorSelectionModuleRGB_G : ColorSelectionModuleRGB
	{
		#region colorfader

		protected override void OnUpdateFaderImage(Bitmap bmp)
		{
			using (Graphics gr = Graphics.FromImage(bmp)) {
				using (LinearGradientBrush brs = new LinearGradientBrush(
					new Point(0, 0), new Point(bmp.Width),
					Color.FromArgb(_color.R, 0, _color.B),
					Color.FromArgb(_color.R, 255, _color.B))) {
					gr.FillRectangle(brs, new Rectangle(Point.Empty, bmp.Size));
				}
			}
		}

		protected override void OnUpdateFaderPosition(ColorSelectionFader fader)
		{
			fader.Position = (double) (_color.G)/255.0;
		}

		protected override void OnFaderScroll(ColorSelectionFader fader)
		{
			int newgreen = Math.Max(0, Math.Min(255, (int) (fader.Position*255.0)));
			if (newgreen == _color.G) return;
			_color = Color.FromArgb(_color.R, newgreen, _color.B);
			UpdatePlaneImage();
			RaiseColorChanged();
		}

		#endregion

		#region colorplane

		protected override void OnUpdatePlaneImage(Bitmap bmp)
		{
			double red = 255.0,
			       delta_red = -255.0/(double) bmp.Height;
			Color[] lincols = new Color[2];
			using (Graphics gr = Graphics.FromImage(bmp)) {
				using (LinearGradientBrush brs = new LinearGradientBrush(
					new Point(0, 0), new Point(bmp.Width, 0),
					Color.White, Color.White)) {
					for (int y = 0; y < bmp.Height; y++, red += delta_red) {
						int r = Math.Max(0, Math.Min(255, (int) red));
						lincols[0] = Color.FromArgb(r, _color.G, 0);
						lincols[1] = Color.FromArgb(r, _color.G, 255);

						brs.LinearColors = lincols;
						gr.FillRectangle(brs, 0, y, bmp.Width, 1);
					}
				}
			}
		}

		protected override void OnUpdatePlanePosition(ColorSelectionPlane plane)
		{
			plane.SetPosition((double) (_color.B)/255.0,
			                  1.0 - (double) (_color.R)/255.0);
		}

		protected override void OnPlaneScroll(ColorSelectionPlane plane)
		{
			int newred = Math.Max(0, Math.Min(255, (int) (255.0 - plane.PositionY*255.0))),
			    newblue = Math.Max(0, Math.Min(255, (int) (plane.PositionX*255.0)));
			if (_color.R == newred &&
			    _color.B == newblue) return;
			_color = Color.FromArgb(newred, _color.G, newblue);
			UpdateFaderImage();
			RaiseColorChanged();
		}

		#endregion
	}

	public class ColorSelectionModuleRGB_B : ColorSelectionModuleRGB
	{
		#region colorfader

		protected override void OnUpdateFaderImage(Bitmap bmp)
		{
			using (Graphics gr = Graphics.FromImage(bmp)) {
				using (LinearGradientBrush brs = new LinearGradientBrush(
					new Point(0, 0), new Point(bmp.Width),
					Color.FromArgb(_color.R, _color.G, 0),
					Color.FromArgb(_color.R, _color.G, 255))) {
					gr.FillRectangle(brs, new Rectangle(Point.Empty, bmp.Size));
				}
			}
		}

		protected override void OnUpdateFaderPosition(ColorSelectionFader fader)
		{
			fader.Position = (double) (_color.B)/255.0;
		}

		protected override void OnFaderScroll(ColorSelectionFader fader)
		{
			int newblue = Math.Max(0, Math.Min(255, (int) (fader.Position*255.0)));
			if (newblue == _color.B) return;
			_color = Color.FromArgb(_color.R, _color.G, newblue);
			UpdatePlaneImage();
			RaiseColorChanged();
		}

		#endregion

		#region colorplane

		protected override void OnUpdatePlaneImage(Bitmap bmp)
		{
			double green = 255.0,
			       delta_green = -255.0/(double) bmp.Height;
			Color[] lincols = new Color[2];
			using (Graphics gr = Graphics.FromImage(bmp)) {
				using (LinearGradientBrush brs = new LinearGradientBrush(
					new Point(0, 0), new Point(bmp.Width, 0),
					Color.White, Color.White)) {
					for (int y = 0; y < bmp.Height; y++, green += delta_green) {
						int g = Math.Max(0, Math.Min(255, (int) green));
						lincols[0] = Color.FromArgb(0, g, _color.B);
						lincols[1] = Color.FromArgb(255, g, _color.B);

						brs.LinearColors = lincols;
						gr.FillRectangle(brs, 0, y, bmp.Width, 1);
					}
				}
			}
		}

		protected override void OnUpdatePlanePosition(ColorSelectionPlane plane)
		{
			plane.SetPosition((double) (_color.R)/255.0,
			                  1.0 - (double) (_color.G)/255.0);
		}

		protected override void OnPlaneScroll(ColorSelectionPlane plane)
		{
			int newgreen = Math.Max(0, Math.Min(255, (int) (255.0 - plane.PositionY*255.0))),
			    newred = Math.Max(0, Math.Min(255, (int) (plane.PositionX*255.0)));
			if (_color.R == newred &&
			    _color.G == newgreen) return;
			_color = Color.FromArgb(newred, newgreen, _color.B);
			UpdateFaderImage();
			RaiseColorChanged();
		}

		#endregion
	}
}