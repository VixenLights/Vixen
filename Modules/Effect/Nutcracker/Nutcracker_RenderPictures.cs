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
			if (String.IsNullOrEmpty(newPictureName)) return;
			const int speedfactor = 4;
			var effectType = (EffectType)dir;
			
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
					_currentGifImageNum += ((gifSpeed * .05));
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
				//int limit = (dir < 2) ? imgwidth + BufferWi : imght + BufferHt;
				//int movement = Convert.ToInt32((State % (limit * speedfactor)) / speedfactor);


				switch (effectType)
				{
					case EffectType.RenderPicturePeekaboo0:
					case EffectType.RenderPicturePeekaboo180:
						//Peek a boo
						yoffset = Convert.ToInt32(State/speedfactor - BufferHt); // * speedfactor; //draw_at = (state < BufferHt)? state
						if (yoffset > 10)
						{
							yoffset = -yoffset + 10; //reverse direction
						}else if (yoffset > 0) {
							yoffset = 0; //pause in middle
						}
						break;
					case EffectType.RenderPictureWiggle: //wiggle left-right -DJ
						xoffset = Convert.ToInt32(State % (BufferWi / 4 * speedfactor));
						if (xoffset > BufferWi / 8 * speedfactor) {
							xoffset = BufferWi / 4 * speedfactor - xoffset; //reverse direction
						}
						xoffset -= BufferWi / 4; //* speedfactor; //center it on mid value
						xoffset += (imgwidth - BufferWi) / 2; //add in original xoffset from above
						break;
					case EffectType.RenderPicturePeekaboo90: //peekaboo 90
					case EffectType.RenderPicturePeekaboo270: //peekaboo 270
						yoffset = (imght - BufferWi) / 2; //adjust offsets for other axis
						xoffset = Convert.ToInt32(State / speedfactor - BufferHt); // * speedfactor; //draw_at = (state < BufferHt)? state
						if (xoffset > 10) {
							xoffset = -xoffset + 10; //reverse direction
						}
						else if (xoffset > 0){
							xoffset = 0; //pause in middle
						}
						break;
					default:
						break;
				}

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
							switch (effectType)
							{
								case EffectType.RenderPictureLeft:
									// left
									SetPixel( Convert.ToInt32(x + BufferWi - (State % ((imgwidth + BufferWi) * speedfactor)) / speedfactor), yoffset - y, fpColor);
									break;
								case EffectType.RenderPictureRight:
									// right
									SetPixel(Convert.ToInt32(x + (State % ((imgwidth + BufferWi) * speedfactor)) / speedfactor - imgwidth), yoffset - y, fpColor);
									break;
								case EffectType.RenderPictureUp:
									// up
									SetPixel(x - xoffset,  Convert.ToInt32((State % ((imght + BufferHt) * speedfactor)) / speedfactor - y), fpColor);
									break;
								case EffectType.RenderPictureDown:
									// down
									SetPixel(x - xoffset, Convert.ToInt32(BufferHt + imght - y - (State % ((imght + BufferHt) * speedfactor)) / speedfactor), fpColor);
									break;
								case EffectType.RenderPictureUpleft:
									SetPixel(Convert.ToInt32(x + BufferWi - (State % ((imgwidth + BufferWi) * speedfactor)) / speedfactor), Convert.ToInt32((State % ((imght + BufferHt) * speedfactor)) / speedfactor - y), fpColor);
									break; // up-left
								case EffectType.RenderPictureDownleft: 
									SetPixel( Convert.ToInt32(x + BufferWi - (State % ((imgwidth + BufferWi) * speedfactor)) / speedfactor), Convert.ToInt32(BufferHt + imght - y - (State % ((imght + BufferHt) * speedfactor)) / speedfactor), fpColor);
									break; // down-left
								case EffectType.RenderPictureUpright:
									SetPixel(Convert.ToInt32(x + (State % ((imgwidth + BufferWi) * speedfactor)) / speedfactor - imgwidth), Convert.ToInt32((State % ((imght + BufferHt) * speedfactor)) / speedfactor - y), fpColor);
									break; // up-right
								case EffectType.RenderPictureDownright: 
									SetPixel(Convert.ToInt32(x + (State % ((imgwidth + BufferWi) * speedfactor)) / speedfactor - imgwidth), Convert.ToInt32(BufferHt + imght - y - (State % ((imght + BufferHt) * speedfactor)) / speedfactor), fpColor);
									break; // down-right
								case EffectType.RenderPicturePeekaboo0: 
									//Peek a boo
									SetPixel(x - xoffset, BufferHt + yoffset - y, fpColor); 
									break;
								case EffectType.RenderPictureWiggle: 
									SetPixel(x + xoffset, yoffset - y, fpColor);
									break;
								case EffectType.RenderPicturePeekaboo90:  
									SetPixel(BufferWi + xoffset - y, x - yoffset, fpColor);
									break;
								case EffectType.RenderPicturePeekaboo180:  
									SetPixel(x - xoffset, y - yoffset, fpColor);
									break;
								case EffectType.RenderPicturePeekaboo270: 
									SetPixel(y - xoffset, BufferHt + yoffset - x, fpColor);
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

		internal enum EffectType
		{
			RenderPictureLeft,  
			RenderPictureRight,
			RenderPictureUp,
			RenderPictureDown,
			RenderPictureNone,
			RenderPictureUpleft,
			RenderPictureDownleft,
			RenderPictureUpright,
			RenderPictureDownright,
			RenderPicturePeekaboo0,
			RenderPicturePeekaboo90,
			RenderPicturePeekaboo180,
			RenderPicturePeekaboo270,
			RenderPictureWiggle
		}

	}
}
