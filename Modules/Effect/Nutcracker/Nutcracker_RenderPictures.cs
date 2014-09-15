using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace VixenModules.Effect.Nutcracker
{
	partial class NutcrackerEffects
	{
		private string _pictureName = string.Empty;
		private FastPixel.FastPixel _fp;
		private double _currentGifImageNum;
		private Image _pictureImage;

		public void RenderPictures(int dir, string newPictureName, int gifSpeed)
		{
			const int speedfactor = 4;

			if (newPictureName != _pictureName)
			{
				_pictureImage = Image.FromFile(newPictureName);
				_pictureName = newPictureName;
			}

			var dimension = new FrameDimension(_pictureImage.FrameDimensionsList[0]);
			// Number of frames
			int frameCount = _pictureImage.GetFrameCount(dimension);

			if (frameCount > 1)
			{
				if (gifSpeed > 0)
				{
					_currentGifImageNum += ((gifSpeed * .01));
				}
				else
				{
					_currentGifImageNum++;
				}
				if (Convert.ToInt32(_currentGifImageNum) >= frameCount)
				{
					_currentGifImageNum = 0;
				}
				_pictureImage.SelectActiveFrame(dimension, Convert.ToInt32(_currentGifImageNum));

			}

			Image image = ScaleImage(_pictureImage, BufferWi, BufferHt);

			_fp = new FastPixel.FastPixel(new Bitmap(image));

			if (_fp != null)
			{
				int imgwidth = _fp.Width;
				int imght = _fp.Height;
				int yoffset = (BufferHt + imght) / 2;
				int xoffset = (imgwidth - BufferWi) / 2;
				int limit = (dir < 2) ? imgwidth + BufferWi : imght + BufferHt;
				int movement = Convert.ToInt32((State % (limit * speedfactor)) / speedfactor);

				// copy image to buffer
				_fp.Lock();
				for (int x = 0; x < imgwidth; x++)
				{
					for (int y = 0; y < imght; y++)
					{
						//if (!image.IsTransparent(x,y))
						Color fpColor = _fp.GetPixel(x, y);
						if (fpColor != Color.Transparent)
						{
							switch (dir)
							{
								case 0:
									// left
									SetPixel(x + BufferWi - movement, yoffset - y - 1, fpColor);
									break;
								case 1:
									// right
									SetPixel(x + movement - imgwidth, yoffset - y, fpColor);
									break;
								case 2:
									// up
									SetPixel(x - xoffset, movement - y, fpColor);
									break;
								case 3:
									// down
									SetPixel(x - xoffset, BufferHt + imght - y - movement, fpColor);
									break;
								default:
									// no movement - centered
									SetPixel(x - xoffset, yoffset - y, fpColor);
									break;
							}
						}
					}
				}
				_fp.Unlock(false);
			}
			if (image != null)
				image.Dispose();
		}

	}
}
