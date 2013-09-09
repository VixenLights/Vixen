using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Concurrent;

namespace FastPixel
{
	public class FastPixel : IDisposable
	{
		private byte[] rgbValues;
		private BitmapData bmpData;
		private IntPtr bmpPtr;
		public bool locked = false;

		private bool _isAlpha = false;
		private Bitmap _bitmap;
		private int _width;
		private int _height;

		public int Width
		{
			get { return this._width; }
		}

		public int Height
		{
			get { return this._height; }
		}

		public bool IsAlphaBitmap
		{
			get { return this._isAlpha; }
		}

		public Bitmap Bitmap
		{
			get { return _bitmap; }
		}

		public FastPixel(Bitmap bitmap)
		{
			if (bitmap.PixelFormat == (bitmap.PixelFormat | PixelFormat.Indexed))
				throw new Exception("Cannot lock an Indexed image.");

			this._bitmap = bitmap;
			this._isAlpha = (this.Bitmap.PixelFormat == (this.Bitmap.PixelFormat | PixelFormat.Alpha));
			this._width = bitmap.Width;
			this._height = bitmap.Height;
		}

		public void CloneBitmap(Bitmap bitmapToClone) 
		{
			if (bitmapToClone.Width != _bitmap.Width || bitmapToClone.Height != _bitmap.Height)
			{
				_bitmap = new Bitmap(bitmapToClone.Width, bitmapToClone.Height);
			}
			Graphics g = Graphics.FromImage(_bitmap);
			g.DrawImageUnscaled(bitmapToClone, 0, 0);
		}

		public FastPixel(int width, int height)
		{
			SetupBitmap(width, height);
		}

		// slow!
		public FastPixel(int width, int height, Color backgroundColor)
		{
			SetupBitmap(width, height);
			using (Graphics g = Graphics.FromImage(_bitmap)) {
				g.Clear(backgroundColor);
			}
		}

		public void SetupBitmap(int width, int height)
		{
			_bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
			if (_bitmap.PixelFormat == (_bitmap.PixelFormat | PixelFormat.Indexed))
				throw new Exception("Cannot lock an Indexed image.");

			this._isAlpha = (this.Bitmap.PixelFormat == (this.Bitmap.PixelFormat | PixelFormat.Alpha));
			this._width = _bitmap.Width;
			this._height = _bitmap.Height;
		}

		public void Lock()
		{
			if (this.locked)
				throw new Exception("Bitmap already locked.");

			Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
			this.bmpData = this.Bitmap.LockBits(rect, ImageLockMode.ReadWrite, this.Bitmap.PixelFormat);
			this.bmpPtr = this.bmpData.Scan0;

			if (this.IsAlphaBitmap) {
				int bytes = (this.Width*this.Height)*4;
				this.rgbValues = new byte[bytes];
				System.Runtime.InteropServices.Marshal.Copy(this.bmpPtr, rgbValues, 0, this.rgbValues.Length);
			}
			else {
				int bytes = (this.Width*this.Height)*3;
				this.rgbValues = new byte[bytes];
				System.Runtime.InteropServices.Marshal.Copy(this.bmpPtr, rgbValues, 0, this.rgbValues.Length);
			}
			this.locked = true;
		}

		public void Unlock(bool setPixels)
		{
			if (!this.locked)
				throw new Exception("Bitmap not locked.");

			// Copy the RGB values back to the bitmap;
			if (setPixels)
				System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, bmpPtr, rgbValues.Length);

			// Unlock the bits.;
			this.Bitmap.UnlockBits(bmpData);
			this.locked = false;
		}

		public void Clear(Color color)
		{
			if (!this.locked)
				throw new Exception("Bitmap not locked.");

			if (this.IsAlphaBitmap) {
				for (int index = 0; index < this.rgbValues.Length; index += 4) {
					this.rgbValues[index] = color.B;
					this.rgbValues[index + 1] = color.G;
					this.rgbValues[index + 2] = color.R;
					this.rgbValues[index + 3] = color.A;
				}
			}
			else {
				for (int index = 0; index < this.rgbValues.Length; index += 3) {
					this.rgbValues[index] = color.B;
					this.rgbValues[index + 1] = color.G;
					this.rgbValues[index + 2] = color.R;
				}
			}
		}

		public void SetPixel(Point location, Color color)
		{
			this.SetPixel(location.X, location.Y, color);
		}

		public void SetPixelAlpha(int x, int y, Color color)
		{
			if (!this.locked)
				throw new Exception("Bitmap not locked.");

			if (x >= 0 && x < _width && y >= 0 && y < _height) {
				if (this.IsAlphaBitmap) {
					//displayColor = sourceColor×alpha / 255 + backgroundColor×(255 – alpha) / 255 
					//New_R = CInt((255 - R) * (A / 255.0) + R)
					//New_G = CInt((255 - G) * (A / 255.0) + G)
					//New_B = CInt((255 - G) * (A / 255.0) + B)
					//Final Color = (Source Color x alpha / 255) + [Background Color x (255 - alpha) / 255]
					int index = ((y*this.Width + x)*4);

					float b = rgbValues[index];
					float g = rgbValues[index + 1];
					float r = rgbValues[index + 2];
					float a = rgbValues[index + 3];

					//float New_R = ((255f-color.R) * (color.A / 255f) + color.R);
					//float New_G = ((255f - color.G) * (color.A / 255f) + color.G);
					//float New_B = ((255f - color.B) * (color.A / 255f) + color.B);
					float New_R = color.R*color.A/255 + r*(255 - color.A)/255;
					float New_G = color.G*color.A/255 + g*(255 - color.A)/255;
					float New_B = color.B*color.A/255 + b*(255 - color.A)/255;
					rgbValues[index] = (byte) New_B;
					rgbValues[index + 1] = (byte) New_G;
					rgbValues[index + 2] = (byte) New_R;
					//this.rgbValues[index + 0] = (byte)(((float)color.B * ((float)color.A / 255f)) + ((float)rgbValues[index + 0] * (255f - (float)color.A) / 255f));
					//this.rgbValues[index + 1] = (byte)(((float)color.G * ((float)color.A / 255f)) + ((float)rgbValues[index + 1] * (255f - (float)color.A) / 255f));
					//this.rgbValues[index + 1] = (byte)(((float)color.R * ((float)color.A / 255f)) + ((float)rgbValues[index + 2] * (255f - (float)color.A) / 255f));
					//this.rgbValues[index+3] = (byte)((color.A * (color.A / 255)) + (rgbValues[index+3] * (255 - color.A) / 255));
				}
				else {
					int index = ((y*this.Width + x)*3);
					this.rgbValues[index] = color.B;
					this.rgbValues[index + 1] = color.G;
					this.rgbValues[index + 2] = color.R;
				}
			}
		}

		public void SetPixel(int x, int y, Color color)
		{
			SetPixelAlpha(x, y, color);
		}

		public Color GetPixel(Point location)
		{
			return this.GetPixel(location.X, location.Y);
		}

		public Color GetPixel(int x, int y)
		{
			if (!this.locked)
				throw new Exception("Bitmap not locked.");

			if (this.IsAlphaBitmap) {
				int index = ((y*this.Width + x)*4);
				int b = this.rgbValues[index];
				int g = this.rgbValues[index + 1];
				int r = this.rgbValues[index + 2];
				int a = this.rgbValues[index + 3];
				return Color.FromArgb(a, r, g, b);
			}
			else {
				int index = ((y*this.Width + x)*3);
				int b = this.rgbValues[index];
				int g = this.rgbValues[index + 1];
				int r = this.rgbValues[index + 2];
				return Color.FromArgb(r, g, b);
			}
		}

		public void DrawRectangle(Rectangle rect, Color color)
		{
			for (int x = 0; x < rect.Width; x++) {
				SetPixel(x + rect.Left, rect.Top, color);
				SetPixel(x + rect.Left, rect.Top + rect.Height, color);
			}

			for (int y = 0; y < rect.Height; y++) {
				SetPixel(rect.Left, y + rect.Top, color);
				SetPixel(rect.Left + rect.Width, y + rect.Top, color);
			}
		}

		//public static ConcurrentDictionary<int, FastPixel> circleCache = new ConcurrentDictionary<int, FastPixel>();

		public void DrawCircle(Rectangle rect, Color color)
		{
			if (rect.Width > 0 && rect.Height > 0) {
				// Default drawing tools don't draw circles that are either 1 or 2 pixels,
				// so we do it manually
				if (rect.Width == 1) {
					SetPixel(rect.Left, rect.Top, color);
				}
				else if (rect.Width == 2) {
					// Row 1
					SetPixel(rect.Left, rect.Top, color);
					// Row 2
					SetPixel(rect.Left, rect.Top + 1, color);
				}
				else if (rect.Width == 3) {
					// Row 1
					SetPixel(rect.Left, rect.Top, color);
					// Row 1
					SetPixel(rect.Left + 1, rect.Top, color);
					// Row 2
					SetPixel(rect.Left, rect.Top + 1, color);
				}
				else if (rect.Width == 4) {
					// Row 1
					SetPixel(rect.Left, rect.Top, color);
					// Row 1
					SetPixel(rect.Left + 1, rect.Top, color);
					// Row 2
					SetPixel(rect.Left, rect.Top + 1, color);
					// Row 2
					SetPixel(rect.Left + 1, rect.Top + 1, color);
				}
				else {
					
					FastPixel fp;
					//if (!FastPixel.circleCache.TryGetValue(rect.Width, out fp)) {
					using (var b = new Bitmap(rect.Width, rect.Height)) {
						using (Graphics g = Graphics.FromImage(b)) {
							g.FillEllipse(Brushes.White, new Rectangle(0, 0, rect.Width - 1, rect.Height - 1));
							fp = new FastPixel(b);
							// Lock the bitmap (loads pixels into memory buffer) now
							// and leave it that way because we'll never need to unlock it
							// to modify it -- it is just a circle after all
							fp.Lock();
							//FastPixel.circleCache.TryAdd(rect.Width, fp);
						}
					}
					//}
					for (int x = 0; x < rect.Width; x++) {
						for (int y = 0; y < rect.Height; y++) {
							Color newColor = fp.GetPixel(x, y);
							if (newColor.A != 0)
								SetPixel(rect.Left + x, rect.Top + y, color);
						}
					}

					fp.Dispose();
				}
			}
		}

		~FastPixel()
		{
			Dispose(false);
		}

		protected void Dispose(bool disposing)
		{
			if (disposing) {
				if (_bitmap != null)
					_bitmap.Dispose();
			}
			_bitmap = null;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}