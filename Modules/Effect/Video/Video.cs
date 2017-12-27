using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Video
{
	[BrowsableCategory(@"Advanced Settings", true, false)]
	public class Video : PixelEffectBase
	{
		private VideoData _data;
		private List<string> _moviePicturesFileList;
		private double _currentMovieImageNum;
		private readonly string _videoPath = VideoDescriptor.ModulePath;
		private readonly string _tempPath = Path.Combine(VideoDescriptor.ModulePath, "Temp");
		private bool _processVideo = true;

		public Video()
		{
			_data = new VideoData();
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
				_processVideo = false;
				UpdateIterationsAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 2)]
		[ProviderDisplayName(@"Iterations")]
		[ProviderDescription(@"Iterations")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 20, 1)]
		[PropertyOrder(1)]
		public int Speed
		{
			get { return _data.Speed; }
			set
			{
				_data.Speed = value;
				IsDirty = true;
				_processVideo = false;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 2)]
		[ProviderDisplayName(@"PlayBackSpeed")]
		[ProviderDescription(@"PlayBackSpeed")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(-100, 200, 1)]
		[PropertyOrder(2)]
		public int PlayBackSpeed
		{
			get { return _data.PlayBackSpeed; }
			set
			{
				_data.PlayBackSpeed = value;
				IsDirty = true;
				_processVideo = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"XOffset")]
		[ProviderDescription(@"XOffset")]
		//[NumberRange(-100, 100, 1)]
		[PropertyOrder(3)]
		public Curve XOffsetCurve
		{
			get { return _data.XOffsetCurve; }
			set
			{
				_data.XOffsetCurve = value;
				IsDirty = true;
				_processVideo = false;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"YOffset")]
		[ProviderDescription(@"YOffset")]
		//[NumberRange(-100, 100, 1)]
		[PropertyOrder(3)]
		public Curve YOffsetCurve
		{
			get { return _data.YOffsetCurve; }
			set
			{
				_data.YOffsetCurve = value;
				IsDirty = true;
				_processVideo = false;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Video Configuration

		[Value]
		[ProviderCategory(@"Video Configuration", 2)]
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
				_processVideo = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Video Configuration", 2)]
		[ProviderDisplayName(@"Filename")]
		[ProviderDescription(@"Filename")]
		[PropertyEditor("VideoPathEditor")]
		[PropertyOrder(0)]
		public String FileName
		{
			get { return _data.FileName; }
			set
			{
				_data.FileName = CopyLocal(value);
				IsDirty = true;
				_processVideo = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Video Configuration", 2)]
		[ProviderDisplayName(@"Color Type")]
		[ProviderDescription(@"EffectColorType")]
		[PropertyOrder(1)]
		public EffectColorType EffectColorType
		{
			get { return _data.EffectColorType; }
			set
			{
				_data.EffectColorType = value;
				IsDirty = true;
				_processVideo = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Video Configuration", 2)]
		[ProviderDisplayName(@"ScaleToGrid")]
		[ProviderDescription(@"ScaleToGrid")]
		[PropertyOrder(2)]
		public bool ScaleToGrid
		{
			get { return _data.ScaleToGrid; }
			set
			{
				_data.ScaleToGrid = value;
				IsDirty = true;
				_processVideo = true;
				UpdateScaleAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Video Configuration", 2)]
		[ProviderDisplayName(@"ScalePercent")]
		[ProviderDescription(@"ScalePercent")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(-50, 100, 1)]
		[PropertyOrder(3)]
		public int ScalePercent
		{
			get { return _data.ScalePercent; }
			set
			{
				_data.ScalePercent = value;
				IsDirty = true;
				_processVideo = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Video Configuration", 2)]
		[ProviderDisplayName(@"Brightness")]
		[ProviderDescription(@"Brightness")]
		[PropertyOrder(4)]
		public Curve LevelCurve
		{
			get { return _data.LevelCurve; }
			set
			{
				_data.LevelCurve = value;
				IsDirty = true;
				_processVideo = false;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Advance Settings

		[ReadOnly(true)]
		[ProviderCategory(@"Advanced Settings", 3)]
		[ProviderDisplayName(@"Video Length (sec)")]
		[ProviderDescription(@"Video Length")]
		[PropertyEditor("Label")]
		[PropertyOrder(1)]
		public int VideoLength
		{
			get { return _data.VideoLength; }
			private set
			{
				_data.VideoLength = value;
				IsDirty = true;
				_processVideo = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Advanced Settings", 3)]
		[ProviderDisplayName(@"Start Time (sec)")]
		[ProviderDescription(@"Start position of Video File")]
		[PropertyEditor("DoubleSliderEditor")]
		[NumberRange(0, 10000, 1)]
		[PropertyOrder(2)]
		public double StartTimeSeconds
		{
			get { return _data.StartTime; }
			set
			{
				_data.StartTime = value;
				IsDirty = true;
				_processVideo = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Advanced Settings", 3)]
		[ProviderDisplayName(@"Maintain Aspect")]
		[ProviderDescription(@"Maintain Grid Aspect")]
		[PropertyOrder(5)]
		public bool MaintainAspect
		{
			get { return _data.MaintainAspect; }
			set
			{
				_data.MaintainAspect = value;
				IsDirty = true;
				_processVideo = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Advanced Settings", 3)]
		[ProviderDisplayName(@"Rotate Video")]
		[ProviderDescription(@"Rotate Video")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 360, 1)]
		[PropertyOrder(6)]
		public int RotateVideo
		{
			get { return _data.RotateVideo; }
			set
			{
				_data.RotateVideo = value;
				IsDirty = true;
				_processVideo = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Advanced Settings", 3)]
		[ProviderDisplayName(@"Increase Brightness")]
		[ProviderDescription(@"Increase Brightness")]
		//[NumberRange(10, 100, 1)]
		[PropertyOrder(7)]
		public Curve IncreaseBrightnessCurve
		{
			get { return _data.IncreaseBrightnessCurve; }
			set
			{
				_data.IncreaseBrightnessCurve = value;
				IsDirty = true;
				_processVideo = false;
				OnPropertyChanged();
			}
		}

		#endregion

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

		private void UpdateAttributes()
		{
			UpdateScaleAttribute(false);
			UpdateIterationsAttribute(false);
			TypeDescriptor.Refresh(this);
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

		private void UpdateIterationsAttribute(bool refresh = true)
		{
			bool showIterations = Type != EffectType.RenderPictureNone;
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{
				{"Speed", showIterations}
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
				_data = value as VideoData;
				UpdateAttributes();
				IsDirty = true;
			}
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		#region Information

		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/video/"; }
		}

		#endregion

		private string CopyLocal(string path)
		{
			string name = Path.GetFileName(path);
			string destPathFilename = Path.Combine(_videoPath, name);
			if (path != destPathFilename)
			{
				File.Copy(path, destPathFilename, true);
			}
			return name;
		}

		protected override void SetupRender()
		{
			if (_processVideo && _data.FileName != "")
				ProcessMovie(_data.Video_DataPath);
			_currentMovieImageNum = 0;
		}

		protected override void CleanUpRender()
		{
			//Nothing to clean up
		}

		private void ProcessMovie(string folder)
		{
			//Delete old path and create new path for processed video
			if (System.IO.Directory.Exists(_data.Video_DataPath))
			{
				Directory.Delete(folder, true);
			}
			_data.Video_DataPath = Path.Combine(_tempPath, Guid.NewGuid().ToString());
			System.IO.Directory.CreateDirectory(_data.Video_DataPath);
			_moviePicturesFileList = null;

			//Setup scale size to process.
			int renderHeight = BufferHt;
			int renderWidth = BufferWi;
			if (!ScaleToGrid)
			{
				renderWidth = (int)(BufferWi * ((double)ScalePercent / 100 + 1));
				renderHeight = (int)(BufferHt * ((double)ScalePercent / 100 + 1));
				if (renderWidth % 2 != 0)
				{
					renderWidth += 1;
				}
			}

			string colorType = EffectColorType == EffectColorType.RenderGreyScale ? " -pix_fmt gray" : ""; //Effcet type will be Color or Gray scale
			string frameRate = " -r " + 20; //Video Frame rate is set to 20 to matach Vixen

			string videoFilename = Path.Combine(_videoPath, _data.FileName);
			try
			{
				//Gets Video length and will continue if users start position is less then the video length.
				ffmpeg.ffmpeg videoLengthInfo = new ffmpeg.ffmpeg(videoFilename);
				string result = videoLengthInfo.MakeThumbnails(_data.Video_DataPath);
				int index = result.IndexOf("Duration: ");
				string videoInfo = result.Substring(index + 10, 8);
				string[] words = videoInfo.Split(':');
				TimeSpan videoTimeSpan = new TimeSpan(Int32.Parse(words[0]), Int32.Parse(words[1]), Int32.Parse(words[2]));
				VideoLength = (int)videoTimeSpan.TotalSeconds;

				//Gets selected video if Video length is longer then the entered start time.
				if (VideoLength > StartTimeSeconds + (TimeSpan.TotalSeconds*((double) PlayBackSpeed/100 + 1)))
				{
					ffmpeg.ffmpeg converter = new ffmpeg.ffmpeg(videoFilename);
					converter.MakeThumbnails(_data.Video_DataPath, StartTimeSeconds, ((TimeSpan.TotalSeconds*((double) PlayBackSpeed/100 + 1))),
						renderWidth, renderHeight, MaintainAspect, frameRate, colorType, RotateVideo);
					_moviePicturesFileList = Directory.GetFiles(_data.Video_DataPath).OrderBy(f => f).ToList();
					_currentMovieImageNum = 0;
				}
				else
				{
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("Entered Start Time plus Effect length is greater than the Video Length of " + _data.FileName,
						"Invalid Start Time. Decrease the Start Time", MessageBoxButtons.OK, SystemIcons.Error);
					messageBox.ShowDialog();
				}
			}
			catch (Exception ex)
			{
				var messageBox = new MessageBoxForm("There was a problem converting " + videoFilename + ": " + ex.Message,
					"Error Converting Video", MessageBoxButtons.OK, SystemIcons.Error);
				messageBox.ShowDialog();
			}
		}

		#region Render Video Effect
		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			var intervalPos = GetEffectTimeIntervalPosition(frame);
			var intervalPosFactor = intervalPos * 100;
			double position = (intervalPos * Speed) % 1;
			double level = LevelCurve.GetValue(intervalPosFactor) / 100;
			double adjustBrightness = CalculateIncreaseBrightness(intervalPosFactor);


			// If we don't have any pictures, do nothing!
			if (_moviePicturesFileList == null || !_moviePicturesFileList.Any())
				return;

			int pictureCount = _moviePicturesFileList.Count;
			
			if (PlayBackSpeed > 0)
			{
				_currentMovieImageNum += ((PlayBackSpeed * .01) + 1);
			}
			else if (PlayBackSpeed < 0)
			{
				_currentMovieImageNum += (100 + PlayBackSpeed) * .01;
			}
			else
			{
				_currentMovieImageNum ++;
			}

			int currentImage = Convert.ToInt32(_currentMovieImageNum);
			if (currentImage >= pictureCount || currentImage < 0)
				_currentMovieImageNum = currentImage = 0;

			// copy image to buffer
			Bitmap resizeImage = new Bitmap(Image.FromFile(_moviePicturesFileList[currentImage]));
			FastPixel.FastPixel currentMovieImage = new FastPixel.FastPixel(new Bitmap(resizeImage));

			int imgwidth = currentMovieImage.Width;
			int imght = currentMovieImage.Height;
			int yoffset = (BufferHt + imght) / 2;
			int xoffset = (imgwidth - BufferWi) / 2;
			int xOffsetAdj = CalculateXOffset(intervalPosFactor) * BufferWi / 100;
			int yOffsetAdj = CalculateYOffset(intervalPosFactor) * BufferHt / 100;

			switch (Type)
			{
				case EffectType.RenderPicturePeekaboo0:
				case EffectType.RenderPicturePeekaboo180:
					//Peek a boo
					yoffset = -(BufferHt) + (int)((BufferHt + 5) * position * 2);
					if (yoffset > 10)
					{
						yoffset = -yoffset + 10; //reverse direction
					}
					else if (yoffset >= -1)
					{
						yoffset = -1; //pause in middle
					}

					break;
				case EffectType.RenderPictureWiggle:
					if (position >= 0.5)
					{
						xoffset += (int)(BufferWi * ((1.0 - position) * 2.0 - 0.5));
					}
					else
					{
						xoffset += (int)(BufferWi * (position * 2.0 - 0.5));
					}
					break;
				case EffectType.RenderPicturePeekaboo90: //peekaboo 90
				case EffectType.RenderPicturePeekaboo270: //peekaboo 270
					if (Orientation == StringOrientation.Vertical)
					{
						yoffset = (imght - BufferWi) / 2; //adjust offsets for other axis
						xoffset = -(BufferHt) + (int)((BufferHt + 5) * position * 2);
					}
					else
					{
						yoffset = (imgwidth - BufferHt) / 2; //adjust offsets for other axis
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

					break;
			}

			currentMovieImage.Lock();
			Color fpColor = new Color();
			for (int x = 0; x < imgwidth; x++)
			{
				for (int y = 0; y < imght; y++)
				{
					fpColor = currentMovieImage.GetPixel(x, y);
					if (fpColor != Color.Transparent && fpColor != Color.Black)
					{
						var hsv = HSV.FromRGB(fpColor);
						double tempV = hsv.V * level * (adjustBrightness / 10);
						if (tempV > 1)
							tempV = 1;
						hsv.V = tempV;
						int leftX, rightX, upY, downY;
						switch (_data.EffectType)
						{
							case EffectType.RenderPictureLeft:
								// left
								leftX = x + (BufferWi - (int)(position * (imgwidth + BufferWi)));

								frameBuffer.SetPixel(leftX + xOffsetAdj, yoffset - y + yOffsetAdj, hsv);
								break;
							case EffectType.RenderPictureRight:
								// right
								rightX = x + -imgwidth + (int)(position * (imgwidth + BufferWi));

								frameBuffer.SetPixel(rightX + xOffsetAdj, yoffset - y + yOffsetAdj, hsv);
								break;
							case EffectType.RenderPictureUp:
								// up
								upY = (int)((imght + BufferHt) * position) - y;

								frameBuffer.SetPixel(x - xoffset + xOffsetAdj, upY + yOffsetAdj, hsv);
								break;
							case EffectType.RenderPictureDown:
								// down
								downY = (BufferHt + imght - 1) - (int)((imght + BufferHt) * position) - y;

								frameBuffer.SetPixel(x - xoffset + xOffsetAdj, downY + yOffsetAdj, hsv);
								break;
							case EffectType.RenderPictureUpleft:
								// up-left
								leftX = x + (BufferWi - (int)(position * (imgwidth + BufferWi)));
								upY = (int)((imght + BufferHt) * position) - y;

								frameBuffer.SetPixel(leftX + xOffsetAdj, upY + yOffsetAdj, hsv);
								break;
							case EffectType.RenderPictureDownleft:
								// down-left
								leftX = x + (BufferWi - (int)(position * (imgwidth + BufferWi)));
								downY = (BufferHt + imght - 1) - (int)((imght + BufferHt) * position) - y;

								frameBuffer.SetPixel(leftX + xOffsetAdj, downY + yOffsetAdj, hsv);
								break;
							case EffectType.RenderPictureUpright:
								// up-right
								upY = (int)((imght + BufferHt) * position) - y; 
								rightX = x + -imgwidth + (int)(position * (imgwidth + BufferWi));
								
								frameBuffer.SetPixel(rightX + xOffsetAdj, upY + yOffsetAdj, hsv);
								break;
							case EffectType.RenderPictureDownright:
								// down-right
								downY = (BufferHt + imght - 1) - (int)((imght + BufferHt) * position) - y;
								rightX = x + -imgwidth + (int)(position * (imgwidth + BufferWi));

								frameBuffer.SetPixel(rightX + xOffsetAdj, downY + yOffsetAdj, hsv);
								break;
							case EffectType.RenderPicturePeekaboo0:
								// Peek a boo 0
								frameBuffer.SetPixel(x - xoffset + xOffsetAdj, BufferHt + yoffset - y + yOffsetAdj, hsv);
								break;
							case EffectType.RenderPictureWiggle:
								// Wiggle
								frameBuffer.SetPixel(x + xoffset + xOffsetAdj, yoffset - y + yOffsetAdj, hsv);
								break;
							case EffectType.RenderPicturePeekaboo90:
								// Peekaboo90
								frameBuffer.SetPixel(BufferWi + xoffset - y + xOffsetAdj, x - yoffset + yOffsetAdj, hsv);
								break;
							case EffectType.RenderPicturePeekaboo180:
								// Peekaboo180
								frameBuffer.SetPixel(x - xoffset + xOffsetAdj, y - yoffset + yOffsetAdj, hsv);
								break;
							case EffectType.RenderPicturePeekaboo270:
								// Peekaboo270
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
			currentMovieImage.Unlock(false);
			currentMovieImage.Dispose();
			resizeImage.Dispose();
		}
		#endregion

		private int CalculateXOffset(double intervalPos)
		{
			return (int)Math.Round(ScaleCurveToValue(XOffsetCurve.GetValue(intervalPos), 100, -100));
		}

		private int CalculateYOffset(double intervalPos)
		{
			return (int)Math.Round(ScaleCurveToValue(YOffsetCurve.GetValue(intervalPos), 100, -100));
		}

		private double CalculateIncreaseBrightness(double intervalPos)
		{
			return ScaleCurveToValue(IncreaseBrightnessCurve.GetValue(intervalPos), 100, 10);
		}
	}
}