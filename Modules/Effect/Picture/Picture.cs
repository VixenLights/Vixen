using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.Curves;
using VixenModules.Effect.Pixel;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Picture
{
	public class Picture:PixelEffectBase
	{
		private PictureData _data;

		private double _currentGifImageNum;
		private Image _image;
		private Bitmap _scaledImage;
		private int _frameCount;
		private FrameDimension _dimension;
		private FastPixel.FastPixel _fp;
		
		public Picture()
		{
			_data = new PictureData();
			UpdateAttributes();
		}

		#region Movement

		[Value]
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"Type")]
		[ProviderDescription(@"EffectType")]
		[PropertyOrder(0)]
		public EffectType Type
		{
			get { return _data.EffectType; }
			set
			{
				_data.EffectType = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"FitTime")]
		[ProviderDescription(@"FitTime")]
		[PropertyOrder(4)]
		public bool FitToTime
		{
			get { return _data.FitToTime; }
			set
			{
				_data.FitToTime = value;
				IsDirty = true;
				UpdateSpeedAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"XOffset")]
		[ProviderDescription(@"XOffset")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(-100, 100, 1)]
		[PropertyOrder(2)]
		public int XOffset
		{
			get { return _data.XOffset; }
			set
			{
				_data.XOffset = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"YOffset")]
		[ProviderDescription(@"YOffset")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(-100, 100, 1)]
		[PropertyOrder(3)]
		public int YOffset
		{
			get { return _data.YOffset; }
			set
			{
				_data.YOffset = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"GifSpeed")]
		[ProviderDescription(@"GifSpeed")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 20, 1)]
		[PropertyOrder(4)]
		public int GifSpeed
		{
			get { return _data.GifSpeed; }
			set
			{
				_data.GifSpeed = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion


		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"Orientation")]
		[ProviderDescription(@"Orientation")]
		[Browsable(false)]
		public StringOrientation Orientation
		{
			get { return _data.Orientation; }
			set
			{
				_data.Orientation = value;
				StringOrientation = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"Filename")]
		[ProviderDescription(@"Filename")]
		[PropertyEditor("ImagePathEditor")]
		[PropertyOrder(1)]
		public String FileName
		{
			get { return _data.FileName; }
			set
			{
				_data.FileName = ConvertPath(value);
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"Speed")]
		[ProviderDescription(@"Speed")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 20, 1)]
		[PropertyOrder(2)]
		public int Speed
		{
			get { return _data.Speed; }
			set
			{
				_data.Speed = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		

		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"ScaleToGrid")]
		[ProviderDescription(@"ScaleToGrid")]
		[PropertyOrder(4)]
		public bool ScaleToGrid
		{
			get { return _data.ScaleToGrid; }
			set
			{
				_data.ScaleToGrid = value;
				IsDirty = true;
				UpdateScaleAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"ScalePercent")]
		[ProviderDescription(@"ScalePercent")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[PropertyOrder(5)]
		public int ScalePercent
		{
			get { return _data.ScalePercent; }
			set
			{
				_data.ScalePercent = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#region String Setup properties

		[Value]
		public override StringOrientation StringOrientation
		{
			get { return _data.Orientation; }
			set
			{
				_data.Orientation = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Color

		

		#endregion

		#region Level properties

		[Value]
		[ProviderCategory(@"Brightness", 3)]
		[ProviderDisplayName(@"Brightness")]
		[ProviderDescription(@"Brightness")]
		public Curve LevelCurve
		{
			get { return _data.LevelCurve; }
			set
			{
				_data.LevelCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		private void UpdateAttributes()
		{
			UpdateSpeedAttribute(false);
			UpdateScaleAttribute(false);
			TypeDescriptor.Refresh(this);
		}

		private void UpdateSpeedAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{
				{"Speed", !FitToTime}
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		private void UpdateScaleAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{
				{"ScalePercent", !ScaleToGrid}
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as PictureData;
				UpdateAttributes();
				IsDirty = true;
			}
		}

		private string ConvertPath(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return path;
			}
			if (Path.IsPathRooted(path))
			{
				return CopyLocal(path);
			}
			
			return path;
		}

		private string CopyLocal(string path)
		{
			string name = Path.GetFileName(path);
			var destPath = Path.Combine(PictureDescriptor.ModulePath, name);
			File.Copy(path, destPath, true);
			return name;
		}
		
		protected override void SetupRender()
		{
			_currentGifImageNum = 0;
			_frameCount = 0;
			if (_scaledImage != null)
			{
				_scaledImage.Dispose();
			}
			
			if (!string.IsNullOrEmpty(FileName))
			{
				var filePath = Path.Combine(PictureDescriptor.ModulePath, FileName);
				if (File.Exists(filePath))
				{
					using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
					{
						var ms = new MemoryStream();
						fs.CopyTo(ms);
						ms.Position = 0;
						if (_image != null) _image.Dispose();
						_image = Image.FromStream(ms);
					}
					if (_image != null)
					{
						_dimension = new FrameDimension(_image.FrameDimensionsList[0]);
						// Number of frames
						_frameCount = _image.GetFrameCount(_dimension);
					}
				}

			}
		}

		protected override void CleanUpRender()
		{
			_dimension = null;
			if (_scaledImage != null)
			{
				_scaledImage.Dispose();
			}
			if (_image != null)
			{
				_image.Dispose();
			}
		}

		protected override void RenderEffect(int frame, ref PixelFrameBuffer frameBuffer)
		{
			if (_image == null) return;
			const int speedfactor = 4;
			double position = GetEffectTimeIntervalPosition(frame);
			double level = LevelCurve.GetValue(position * 100) / 100;

			if (_frameCount > 1)
			{
				if (FitToTime && GifSpeed==0)
				{
					CalculateImageNumberByPosition(position);
				}
				else
				{
					CalculateImageNumberBySpeed();
				}

				_image.SelectActiveFrame(_dimension,(int) _currentGifImageNum);
				
			}

			_scaledImage = ScaleToGrid ? ScaleImage(_image, BufferWi, BufferHt) : ScaleImage(_image, _image.Width * ScalePercent / 100, _image.Height * ScalePercent / 100);
			_fp = new FastPixel.FastPixel(_scaledImage);
			
			int imgwidth = _fp.Width;
			int imght = _fp.Height;
			int yoffset = (BufferHt + imght) / 2;
			int xoffset = (imgwidth - BufferWi) / 2;
			int state = Speed * frame;
			int xOffsetAdj = XOffset*BufferWi/100;
			int yOffsetAdj = YOffset*BufferHt/100;

			switch (Type)
			{
				case EffectType.RenderPicturePeekaboo0:
				case EffectType.RenderPicturePeekaboo180:
					//Peek a boo
					if (FitToTime)
					{
						yoffset = -(BufferHt) + (int)((BufferHt+5)*position*2);
						if (yoffset > 10)
						{
							yoffset = -yoffset + 10; //reverse direction
						}
						else if (yoffset >= -1)
						{
							yoffset = -1; //pause in middle
						}
					}
					else
					{
						yoffset = Convert.ToInt32(state / speedfactor - BufferHt);
						if (yoffset > 10)
						{
							yoffset = -yoffset + 10; //reverse direction
						}
						else if (yoffset > 0)
						{
							yoffset = 0; //pause in middle
						}
					}
					
					break;
				case EffectType.RenderPictureWiggle: 
					xoffset = Convert.ToInt32(state % (BufferWi / 4 * speedfactor));
					if (xoffset > BufferWi / 8 * speedfactor)
					{
						xoffset = BufferWi / 4 * speedfactor - xoffset; //reverse direction
					}
					xoffset -= BufferWi / 4; //* speedfactor; //center it on mid value
					xoffset += (imgwidth - BufferWi) / 2; //add in original xoffset from above	
					break;
				case EffectType.RenderPicturePeekaboo90: //peekaboo 90
				case EffectType.RenderPicturePeekaboo270: //peekaboo 270
					if (Orientation == StringOrientation.Vertical)
					{
						yoffset = (imght - BufferWi)/2; //adjust offsets for other axis
					}
					else
					{
						yoffset = (imgwidth - BufferHt) / 2; //adjust offsets for other axis	
					}

					if (FitToTime)
					{
						if (Orientation == StringOrientation.Vertical)
						{
							xoffset = -(BufferHt) + (int) ((BufferHt + 5)*position*2);
						}
						else
						{
							xoffset = -(BufferWi) + (int)((BufferWi + 5) * position * 2);
						}
						if (xoffset > 10)
						{
							xoffset = -xoffset + 10; //reverse direction
						}
						else if (xoffset >= -1)
						{
							xoffset = -1; //pause in middle
						}
					}
					else
					{
						xoffset = Convert.ToInt32(state/speedfactor - BufferHt); // * speedfactor; //draw_at = (state < BufferHt)? state
					
						if (xoffset > 10)
						{
							xoffset = -xoffset + 10; //reverse direction
						}
						else if (xoffset > 0)
						{
							xoffset = 0; //pause in middle
						}
					}
					break;
			}

			_fp.Lock();
			for (int x = 0; x < imgwidth; x++)
			{
				for (int y = 0; y < imght; y++)
				{
					//if (!image.IsTransparent(x,y))
					Color color = _fp.GetPixel(x, y);
					

					if (color != Color.Transparent)
					{
						var hsv = HSV.FromRGB(color);
						hsv.V = hsv.V * level;
						switch (Type)
						{
							case EffectType.RenderPictureLeft:
								// left
								int leftX;
								if (FitToTime)
								{
									
									leftX = x + (BufferWi - (int)(position * (imgwidth + BufferWi)));
								}
								else
								{
									leftX = Convert.ToInt32(x + BufferWi - (state%((imgwidth + BufferWi)*speedfactor))/speedfactor);
								}
								frameBuffer.SetPixel(leftX + xOffsetAdj, yoffset - y + yOffsetAdj, hsv);
								break;
							case EffectType.RenderPictureRight:
								// right
								int rightX;
								if (FitToTime)
								{
									rightX = x + -imgwidth + (int)(position * (imgwidth + BufferWi));
								}
								else
								{
									rightX = Convert.ToInt32(x + (state % ((imgwidth + BufferWi) * speedfactor)) / speedfactor - imgwidth);
								}

								frameBuffer.SetPixel(rightX + xOffsetAdj, yoffset - y + yOffsetAdj, hsv);
								break;
							case EffectType.RenderPictureUp:
								// up
								int upY;
								if (FitToTime)
								{
									upY = (int)((imght + BufferHt) *position) - y;
								}
								else
								{
									upY = Convert.ToInt32((state%((imght + BufferHt)*speedfactor))/speedfactor - y);
								}
								frameBuffer.SetPixel(x - xoffset + xOffsetAdj, upY + yOffsetAdj, hsv);
								break;
							case EffectType.RenderPictureDown:
								// down
								int downY;
								if (FitToTime)
								{
									downY = (BufferHt + imght-1)  - (int)((imght + BufferHt) * position) - y;
								}
								else
								{
									downY = Convert.ToInt32(BufferHt + imght - y - (state % ((imght + BufferHt) * speedfactor)) / speedfactor);
								}
								frameBuffer.SetPixel(x - xoffset + xOffsetAdj, downY+yOffsetAdj, hsv);
								break;
							case EffectType.RenderPictureUpleft:
								int upLeftY;
								if (FitToTime)
								{
									upLeftY = (int)((imght + BufferHt) * position) - y;
								}
								else
								{
									upLeftY = Convert.ToInt32((state % ((imght + BufferHt) * speedfactor)) / speedfactor - y);
								}
								frameBuffer.SetPixel(Convert.ToInt32(x + BufferWi - (state % ((imgwidth + BufferWi) * speedfactor)) / speedfactor) + xOffsetAdj, upLeftY+yOffsetAdj, hsv);
								break; // up-left
							case EffectType.RenderPictureDownleft:
								int downLeftY;
								if (FitToTime)
								{
									downLeftY = BufferHt + imght - (int)((imght + BufferHt) * position) - y;
								}
								else
								{
									downLeftY = Convert.ToInt32(BufferHt + imght - y - (state % ((imght + BufferHt) * speedfactor)) / speedfactor);
								}
								frameBuffer.SetPixel(Convert.ToInt32(x + BufferWi - (state % ((imgwidth + BufferWi) * speedfactor)) / speedfactor) + xOffsetAdj, downLeftY+yOffsetAdj, hsv);
								break; // down-left
							case EffectType.RenderPictureUpright:
								int upRightY;
								if (FitToTime)
								{
									upRightY = (int)((imght + BufferHt) * position) - y;
								}
								else
								{
									upRightY = Convert.ToInt32((state % ((imght + BufferHt) * speedfactor)) / speedfactor - y);
								}
								frameBuffer.SetPixel(Convert.ToInt32(x + (state % ((imgwidth + BufferWi) * speedfactor)) / speedfactor - imgwidth)+xOffsetAdj, upRightY+yOffsetAdj, hsv);
								break; // up-right
							case EffectType.RenderPictureDownright:
								int downRightY;
								if (FitToTime)
								{
									downRightY = BufferHt + imght - (int)((imght + BufferHt) * position) - y;
								}
								else
								{
									downRightY = Convert.ToInt32(BufferHt + imght - y - (state % ((imght + BufferHt) * speedfactor)) / speedfactor);
								}
								frameBuffer.SetPixel(Convert.ToInt32(x + (state % ((imgwidth + BufferWi) * speedfactor)) / speedfactor - imgwidth) + xOffsetAdj, downRightY+yOffsetAdj, hsv);
								break; // down-right
							case EffectType.RenderPicturePeekaboo0:
								//Peek a boo
								frameBuffer.SetPixel(x - xoffset + xOffsetAdj, BufferHt + yoffset - y+yOffsetAdj, hsv);
								break;
							case EffectType.RenderPictureWiggle:
								frameBuffer.SetPixel(x + xoffset + xOffsetAdj, yoffset - y + yOffsetAdj, hsv);
								break;
							case EffectType.RenderPicturePeekaboo90:
								frameBuffer.SetPixel(BufferWi + xoffset - y + xOffsetAdj, x - yoffset + yOffsetAdj, hsv);	
								break;
							case EffectType.RenderPicturePeekaboo180:
								frameBuffer.SetPixel(x - xoffset + xOffsetAdj, y - yoffset + yOffsetAdj, hsv);
								break;
							case EffectType.RenderPicturePeekaboo270:
								frameBuffer.SetPixel(y - xoffset + xOffsetAdj, BufferHt + yoffset - x + yOffsetAdj, hsv);
								break;
							default:
								// no movement - centered
								frameBuffer.SetPixel(x - xoffset + xOffsetAdj, yoffset - y + yOffsetAdj, hsv);
								break;
						}
					}
				}
			}
			_fp.Unlock(false);
			_fp.Dispose();
			_scaledImage.Dispose();
		}

		private void CalculateImageNumberByPosition(double position)
		{
			_currentGifImageNum = position*(_frameCount-1);
		}

		private void CalculateImageNumberBySpeed()
		{
			if (GifSpeed > 0)
			{
				_currentGifImageNum += GifSpeed*.05;
			}
			else
			{
				_currentGifImageNum++;
			}
			if (_currentGifImageNum >= _frameCount)
			{
				_currentGifImageNum = 0;
			}
		}

		public static Bitmap ScaleImage(Image image, int maxWidth, int maxHeight)
		{
			var ratioX = (double)maxWidth / image.Width;
			var ratioY = (double)maxHeight / image.Height;
			var ratio = Math.Min(ratioX, ratioY);

			var newWidth = (int)(image.Width * ratio);
			var newHeight = (int)(image.Height * ratio);

			var newImage = new Bitmap(newWidth, newHeight);
			Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
			return newImage;
		}

		
	}
}
