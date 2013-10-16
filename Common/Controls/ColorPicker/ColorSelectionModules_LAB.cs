using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Common.Controls.ColorManagement.ColorModels;

namespace Common.Controls.ColorManagement.ColorPicker
{
	public abstract class ColorSelectionModuleLAB : ColorSelectionModule
	{
		#region variables

		protected LAB _color;

		#endregion

		#region properties

		public override XYZ XYZ
		{
			get { return _color.ToXYZ(); }
			set
			{
				LAB newcolor = LAB.FromXYZ(value);
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

	public class ColorSelectionModuleLAB_L : ColorSelectionModuleLAB
	{
		#region colorfader

		protected override unsafe void OnUpdateFaderImage(Bitmap bmp)
		{
			if (bmp.PixelFormat != PixelFormat.Format32bppArgb) return;
			BitmapData bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
			                             ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			ColorBgra* scan0 = (ColorBgra*) bd.Scan0;
			//draw fader image
			LAB curr = new LAB(0.0f, _color.a, _color.b);
			float delta_L = 100.0f/(float) bd.Width;
			for (int x = 0; x < bd.Width; x++, scan0++,curr.L += delta_L) {
				scan0[0] = ColorBgra.FromArgb(curr.ToXYZ().ToRGB());
			}
			//end
			bmp.UnlockBits(bd);
		}

		protected override void OnUpdateFaderPosition(ColorSelectionFader fader)
		{
			fader.Position = _color.L/100.0f;
		}

		protected override void OnFaderScroll(ColorSelectionFader fader)
		{
			float newL = fader.Position*100.0f;
			if (newL == _color.L) return;
			_color.L = newL;
			RaiseColorChanged();
			System.Windows.Forms.Application.DoEvents();
			UpdatePlaneImage();
		}

		#endregion

		#region colorplane

		protected override unsafe void OnUpdatePlaneImage(Bitmap bmp)
		{
			if (bmp.PixelFormat != PixelFormat.Format32bppArgb) return;
			BitmapData bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
			                             ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			ColorBgra* scan0 = (ColorBgra*) bd.Scan0;
			//draw plane image
			LAB curr = new LAB(_color.L, -128.0f, 127.0f);
			float
				delta_a = 255.0f/(float) bd.Width,
				delta_b = -255.0f/(float) bd.Height;
			for (int y = 0; y < bd.Height; y++, curr.a = -128.0f, curr.b += delta_b) {
				for (int x = 0; x < bd.Width; x++, scan0++,curr.a += delta_a) {
					scan0[0] = ColorBgra.FromArgb(curr.ToXYZ().ToRGB());
				}
			}
			//end
			bmp.UnlockBits(bd);
		}

		protected override void OnUpdatePlanePosition(ColorSelectionPlane plane)
		{
			plane.SetPosition((_color.a + 128.0f)/255.0f,
			                  (127.0f - _color.b)/255.0f);
		}

		protected override void OnPlaneScroll(ColorSelectionPlane plane)
		{
			float newa = plane.PositionX*255.0f - 128.0f,
			       newb = 127.0f - plane.PositionY*255.0f;
			if (newa == _color.a &&
			    newb == _color.b) return;
			_color.a = newa;
			_color.b = newb;
			UpdateFaderImage();
			RaiseColorChanged();
		}

		#endregion
	}

	public class ColorSelectionModuleLAB_a : ColorSelectionModuleLAB
	{
		#region colorfader

		protected override unsafe void OnUpdateFaderImage(Bitmap bmp)
		{
			if (bmp.PixelFormat != PixelFormat.Format32bppArgb) return;
			BitmapData bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
			                             ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			ColorBgra* scan0 = (ColorBgra*) bd.Scan0;
			//draw fader image
			LAB curr = new LAB(_color.L, -128.0f, _color.b);
			float delta_a = 255.0f/(float) bd.Width;
			for (int x = 0; x < bd.Width; x++, scan0++,curr.a += delta_a) {
				scan0[0] = ColorBgra.FromArgb(curr.ToXYZ().ToRGB());
			}
			//end
			bmp.UnlockBits(bd);
		}

		protected override void OnUpdateFaderPosition(ColorSelectionFader fader)
		{
			fader.Position = (_color.a + 128.0f)/255.0f;
		}

		protected override void OnFaderScroll(ColorSelectionFader fader)
		{
			float newa = fader.Position*255.0f - 128.0f;
			if (newa == _color.b) return;
			_color.a = newa;
			RaiseColorChanged();
			System.Windows.Forms.Application.DoEvents();
			UpdatePlaneImage();
		}

		#endregion

		#region colorplane

		protected override unsafe void OnUpdatePlaneImage(Bitmap bmp)
		{
			if (bmp.PixelFormat != PixelFormat.Format32bppArgb) return;
			BitmapData bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
			                             ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			ColorBgra* scan0 = (ColorBgra*) bd.Scan0;
			//draw plane image
			LAB curr = new LAB(100.0f, _color.a, -128.0f);
			float
				delta_L = -100.0f/(float) bd.Height,
				delta_b = 255.0f/(float) bd.Width;
			for (int y = 0; y < bd.Height; y++, curr.b = -128.0f, curr.L += delta_L) {
				for (int x = 0; x < bd.Width; x++, scan0++,curr.b += delta_b) {
					scan0[0] = ColorBgra.FromArgb(curr.ToXYZ().ToRGB());
				}
			}
			//end
			bmp.UnlockBits(bd);
		}

		protected override void OnUpdatePlanePosition(ColorSelectionPlane plane)
		{
			plane.SetPosition((_color.b + 128.0f)/255.0f,
			                  1.0f - _color.L/100.0f);
		}

		protected override void OnPlaneScroll(ColorSelectionPlane plane)
		{
			float newb = plane.PositionX*255.0f - 128.0f,
			       newL = (1.0f - plane.PositionY)*100.0f;
			if (newb == _color.b &&
			    newL == _color.L) return;
			_color.b = newb;
			_color.L = newL;
			UpdateFaderImage();
			RaiseColorChanged();
		}

		#endregion
	}

	public class ColorSelectionModuleLAB_b : ColorSelectionModuleLAB
	{
		#region colorfader

		protected override unsafe void OnUpdateFaderImage(Bitmap bmp)
		{
			if (bmp.PixelFormat != PixelFormat.Format32bppArgb) return;
			BitmapData bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
			                             ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			ColorBgra* scan0 = (ColorBgra*) bd.Scan0;
			//draw fader image
			LAB curr = new LAB(_color.L, _color.a, -128.0f);
			float delta_b = 255.0f/(float) bd.Width;
			for (int x = 0; x < bd.Width; x++, scan0++,curr.b += delta_b) {
				scan0[0] = ColorBgra.FromArgb(curr.ToXYZ().ToRGB());
			}
			//end
			bmp.UnlockBits(bd);
		}

		protected override void OnUpdateFaderPosition(ColorSelectionFader fader)
		{
			fader.Position = (_color.b + 128.0f)/255.0f;
		}

		protected override void OnFaderScroll(ColorSelectionFader fader)
		{
			float newb = fader.Position*255.0f - 128.0f;
			if (newb == _color.b) return;
			_color.b = newb;
			RaiseColorChanged();
			System.Windows.Forms.Application.DoEvents();
			UpdatePlaneImage();
		}

		#endregion

		#region colorplane

		protected override unsafe void OnUpdatePlaneImage(Bitmap bmp)
		{
			if (bmp.PixelFormat != PixelFormat.Format32bppArgb) return;
			BitmapData bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
			                             ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			ColorBgra* scan0 = (ColorBgra*) bd.Scan0;
			//draw plane image
			LAB curr = new LAB(100.0f, -128.0f, _color.b);
			float
				delta_L = -100.0f/(float) bd.Height,
				delta_a = 255.0f/(float) bd.Width;
			for (int y = 0; y < bd.Height; y++, curr.a = -128.0f, curr.L += delta_L) {
				for (int x = 0; x < bd.Width; x++, scan0++,curr.a += delta_a) {
					scan0[0] = ColorBgra.FromArgb(curr.ToXYZ().ToRGB());
				}
			}
			//end
			bmp.UnlockBits(bd);
		}

		protected override void OnUpdatePlanePosition(ColorSelectionPlane plane)
		{
			plane.SetPosition((_color.a + 128.0f)/255.0f,
			                  1.0f - _color.L/100.0f);
		}

		protected override void OnPlaneScroll(ColorSelectionPlane plane)
		{
			float newa = plane.PositionX*255.0f - 128.0f,
			       newL = (1.0f - plane.PositionY)*100.0f;
			if (newa == _color.a &&
			    newL == _color.L) return;
			_color.a = newa;
			_color.L = newL;
			UpdateFaderImage();
			RaiseColorChanged();
		}

		#endregion
	}
}