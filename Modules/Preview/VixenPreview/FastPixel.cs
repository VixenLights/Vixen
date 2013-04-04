using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.Collections.Generic;

namespace VixenModules.Preview.VixenPreview
{
    public class FastPixel
    {
        private byte[] rgbValues;
        private BitmapData bmpData;
        private IntPtr bmpPtr;
        public bool locked = false;

        private bool _isAlpha = false;
        private Bitmap _bitmap;
        //private Bitmap _emptyBitmap;
        private int _width;
        private int _height;

        public int Width
        {
            get
            {
                return this._width;
            }
        }

        public int Height
        {
            get
            {
                return this._height;
            }
        }

        public bool IsAlphaBitmap
        {
            get
            {
                return this._isAlpha;
            }
        }

        public Bitmap Bitmap
        {
            get
            {
                return _bitmap;
            }
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

        public FastPixel(int width, int height)
        {
            SetupBitmap(width, height);
        }

        public void SetupBitmap(int width, int height)
        {
            _bitmap = new Bitmap(width, height);
            //_emptyBitmap = new Bitmap(width, height);
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

            if (this.IsAlphaBitmap)
            {
                int bytes = (this.Width * this.Height) * 4;
                this.rgbValues = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(this.bmpPtr, rgbValues, 0, this.rgbValues.Length);
            }
            else
            {
                int bytes = (this.Width * this.Height) * 3;
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

        public void Clear(Color colour)
        {
            if (!this.locked)
                throw new Exception("Bitmap not locked.");

            if (this.IsAlphaBitmap)
            {
                for (int index = 0; index < this.rgbValues.Length; index += 4)
                {
                    this.rgbValues[index] = colour.B;
                    this.rgbValues[index + 1] = colour.G;
                    this.rgbValues[index + 2] = colour.R;
                    this.rgbValues[index + 3] = colour.A;
                }
            }
            else
            {
                for (int index = 0; index < this.rgbValues.Length; index += 3)
                {
                    this.rgbValues[index] = colour.B;
                    this.rgbValues[index + 1] = colour.G;
                    this.rgbValues[index + 2] = colour.R;
                }
            }
        }

        public void SetPixel(Point location, Color colour)
        {
            this.SetPixel(location.X, location.Y, colour);
        }

        public void SetPixel(int x, int y, Color colour)
        {
            if (!this.locked)
                throw new Exception("Bitmap not locked.");

            if (x >= 0 && x < _width && y >= 0 && y < _height)
            {
                if (this.IsAlphaBitmap)
                {
                    int index = ((y * this.Width + x) * 4);
                    this.rgbValues[index] = colour.B;
                    this.rgbValues[index + 1] = colour.G;
                    this.rgbValues[index + 2] = colour.R;
                    this.rgbValues[index + 3] = colour.A;
                }
                else
                {
                    int index = ((y * this.Width + x) * 3);
                    this.rgbValues[index] = colour.B;
                    this.rgbValues[index + 1] = colour.G;
                    this.rgbValues[index + 2] = colour.R;
                }
            }
        }

        public Color GetPixel(Point location)
        {
            return this.GetPixel(location.X, location.Y);
        }

        public Color GetPixel(int x, int y)
        {
            if (!this.locked)
                throw new Exception("Bitmap not locked.");

            if (this.IsAlphaBitmap)
            {
                int index = ((y * this.Width + x) * 4);
                int b = this.rgbValues[index];
                int g = this.rgbValues[index + 1];
                int r = this.rgbValues[index + 2];
                int a = this.rgbValues[index + 3];
                return Color.FromArgb(a, r, g, b);
            }
            else
            {
                int index = ((y * this.Width + x) * 3);
                int b = this.rgbValues[index];
                int g = this.rgbValues[index + 1];
                int r = this.rgbValues[index + 2];
                return Color.FromArgb(r, g, b);
            }
        }

        public void DrawRectangle(Rectangle rect, Color color)
        {
            for (int x = 0; x < rect.Width; x++)
            {
                SetPixel(x + rect.Left, rect.Top, color);
                SetPixel(x + rect.Left, rect.Top + rect.Height, color);
            }

            for (int y = 0; y < rect.Height; y++)
            {
                SetPixel(rect.Left, y + rect.Top, color);
                SetPixel(rect.Left + rect.Width, y + rect.Top, color);
            }
        }

        static public Dictionary<int, FastPixel> circleCache = new Dictionary<int, FastPixel>();
        public void DrawCircle(Rectangle rect, Color color)
        {
            lock (Shapes.PreviewTools.renderLock)
            {
                // Default drawing tools don't draw circles that are either 1 or 2 pixels,
                // so we do it manually
                if (rect.Width == 1)
                {
                    SetPixel(rect.Left, rect.Top, color);
                }
                else if (rect.Width == 2)
                {
                    // Row 1
                    SetPixel(rect.Left, rect.Top, color);
                    // Row 2
                    SetPixel(rect.Left, rect.Top + 1, color);
                }
                else
                {
                    Bitmap b;
                    FastPixel fp;
                    if (!FastPixel.circleCache.TryGetValue(rect.Width, out fp))
                    {
                        b = new Bitmap(rect.Width, rect.Height);
                        Graphics g = Graphics.FromImage(b);
                        //g.Clear(Color.Black);
                        g.Clear(Color.Transparent);
                        SolidBrush brush = new SolidBrush(color);
                        g.FillEllipse(brush, new Rectangle(0, 0, rect.Width - 1, rect.Height - 1));
                        //b = new Bitmap(rect.Width, rect.Height, g);
                        fp = new FastPixel(b);
                        FastPixel.circleCache.Add(rect.Width, fp);
                    }
                    fp.Lock();
                    for (int x = 0; x < rect.Width; x++)
                    {
                        for (int y = 0; y < rect.Height; y++)
                        {
                            Color newColor = fp.GetPixel(x, y);
                            if (newColor.A != 0)
                                SetPixel(new Point(rect.Left + x, rect.Top + y), color);
                        }
                    }
                    fp.Unlock(false);
                }
            }

        }
    }
}