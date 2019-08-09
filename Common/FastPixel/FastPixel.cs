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
		Rectangle rect;

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
			SetupBitmap();
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

		private void SetupBitmap(int width, int height)
		{
			_bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
			SetupBitmap();
		}

		private void SetupBitmap()
		{
			if (_bitmap.PixelFormat == (_bitmap.PixelFormat | PixelFormat.Indexed))
				throw new Exception("Cannot lock an Indexed image.");

			_isAlpha = (_bitmap.PixelFormat == (_bitmap.PixelFormat | PixelFormat.Alpha));
			_width = _bitmap.Width;
			_height = _bitmap.Height;
			rect = new Rectangle(0, 0, Width, Height);
			int offset = IsAlphaBitmap ? 4 : 3;
			int bytes = (_width * _height) * offset;
			rgbValues = new byte[bytes];
		}

		/// <summary>
		/// Copy the values from the passed bitmap into the buffer and locks it for editing.
		/// If the bitmap is of different size, the reference bitmap will be updated to the new size
		/// </summary>
		/// <param name="bitmapToClone"></param>
		public void CloneToBuffer(Bitmap bitmapToClone)
		{
			if (this.locked)
				throw new Exception("Bitmap already locked.");
			locked = true;

			if (bitmapToClone.Width != Width || bitmapToClone.Height != Height)
			{
				if(_bitmap!=null) _bitmap.Dispose();
				_bitmap = new Bitmap(bitmapToClone.Width, bitmapToClone.Height);
				SetupBitmap();
			}
			
			BitmapData bitmapData = bitmapToClone.LockBits(rect, ImageLockMode.ReadOnly, bitmapToClone.PixelFormat);
			IntPtr bmpPtr = bitmapData.Scan0;

			System.Runtime.InteropServices.Marshal.Copy(bmpPtr, rgbValues, 0, rgbValues.Length);
			
			bitmapToClone.UnlockBits(bitmapData);
			bitmapData = null;
		
		}

		/// <summary>
		/// Copies the values from the buffer back into the bitmap and unlocks it from editing mode
		/// </summary>
		public void UnlockFromBuffer()
		{
			if (!this.locked)
				throw new Exception("Bitmap not locked.");

			BitmapData bmpData = Bitmap.LockBits(rect, ImageLockMode.WriteOnly, this.Bitmap.PixelFormat);
			IntPtr bmpPtr = bmpData.Scan0;

			System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, bmpPtr, rgbValues.Length);

			// Unlock the bits.;
			Bitmap.UnlockBits(bmpData);
			bmpData = null;
			locked = false;
		}

		/// <summary>
		/// Locks the current bitmap into editing state
		/// </summary>
		public void Lock()
		{
			if (this.locked)
				throw new Exception("Bitmap already locked.");

			this.bmpData = this.Bitmap.LockBits(rect, ImageLockMode.ReadWrite, this.Bitmap.PixelFormat);
			this.bmpPtr = this.bmpData.Scan0;

			System.Runtime.InteropServices.Marshal.Copy(this.bmpPtr, rgbValues, 0, this.rgbValues.Length);
			
			this.locked = true;
		}

		/// <summary>
		/// Unlocks the bitmap from editing state
		/// </summary>
		/// <param name="setPixels">If true copies the edited values back into the bitmap</param>
		public void Unlock(bool setPixels)
		{
			if (!this.locked)
				throw new Exception("Bitmap not locked.");

			// Copy the RGB values back to the bitmap;
			if (setPixels)
				System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, bmpPtr, rgbValues.Length);

			// Unlock the bits.;
			this.Bitmap.UnlockBits(bmpData);
			this.bmpData = null;
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

		private void SetPixelAlpha(int x, int y, Color color)
		{
			if (!locked)
				throw new Exception("Bitmap not locked.");

			if (x >= 0 && x < _width && y >= 0 && y < _height) {
				if (IsAlphaBitmap) {
					
					int index = ((y*Width + x)*4);

					float b = rgbValues[index];
					float g = rgbValues[index + 1];
					float r = rgbValues[index + 2];
					float alphaOffset = 255 - color.A;
					float alphaPercent = color.A / 255f;
					rgbValues[index + 3] = byte.MaxValue; //Ensure alpha is full because we are adjusting the color for it.
					rgbValues[index + 2] = (byte) (color.R* alphaPercent + r*(alphaOffset) /255);
					rgbValues[index + 1] = (byte)(color.G* alphaPercent + g*(alphaOffset) /255);
					rgbValues[index] = (byte) (color.B* alphaPercent + b*(alphaOffset) /255);
					
				}
				else {
					int index = ((y*Width + x)*3);
					rgbValues[index] = color.B;
					rgbValues[index + 1] = color.G;
					rgbValues[index + 2] = color.R;
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

		public void DrawCircle(Rectangle rectangle, Color color)
		{
			if (rectangle.Width > 0 && rectangle.Height > 0) {
				// Default drawing tools don't draw circles that are either 1 or 2 pixels,
				// so we do it manually
				if (rectangle.Width == 1) {
					SetPixel(rectangle.Left, rectangle.Top, color);
				}
				else {
					int radius = rectangle.Width/2; // radius
					for (int y = -radius; y <= radius; y++)
					{
						for (int x = -radius; x <= radius; x++)
						{
							if (x*x + y*y <= radius*radius)
								SetPixel(rectangle.X + x, rectangle.Y + y, color);
						}
					}
				}
			}
		}

		protected void Dispose(bool disposing)
		{
			if (disposing) {
				if (_bitmap != null)
					_bitmap.Dispose();
			}
			_bitmap = null;
			rgbValues = null;
			bmpData = null;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}