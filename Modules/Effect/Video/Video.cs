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
using VixenModules.Effect.Effect.Location;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Video
{
	[BrowsableCategory(@"Advanced Settings", true, false)]
	public class Video : PixelEffectBase
	{
		private const int SpeedFactor = 4;
		private const int MaxRenderWidth = 800;
		private const int MaxRenderHeight = 600;
		private VideoData _data;
		private double _currentMovieImageNum;
		private static readonly string VideoPath = VideoDescriptor.ModulePath;
		private static readonly string TempPath = Path.Combine(Path.GetTempPath(), "Vixen", "VideoEffect");
		private bool _processVideo;
		private double _position;
		private int _xOffsetAdj;
		private int _yOffsetAdj;
		private int _imageHt;
		private int _imageWi;
		private int _xoffset;
		private int _yoffset;
		private FastPixel.FastPixel _fp;
		private bool _videoFileDetected;
		private List<string> _moviePicturesFileList;
		private double _ratioWidth;
		private double _ratioHeight;
		private int _renderHeight;
		private int _renderWidth;
		private bool _getNewVideoInfo;
		private string _tempFilePath;

		public Video()
		{
			_data = new VideoData();
			PopulateTempPath();
			EnableTargetPositioning(true, true);
			_processVideo = true;
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
				UpdateMovementRateAttribute();
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

		[Value]
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"Movement Rate")]
		[ProviderDescription(@"MovementRate")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[PropertyOrder(6)]
		public int MovementRate
		{
			get { return _data.MovementRate; }
			set
			{
				_data.MovementRate = value;
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
				_processVideo = true;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Video Configuration", 2)]
		[ProviderDisplayName(@"Quality")]
		[ProviderDescription(@"Quality")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(10, 100, 1)]
		[PropertyOrder(0)]
		public int VideoQuality
		{
			get { return _data.VideoQuality; }
			set
			{
				_data.VideoQuality = value;
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
		[PropertyOrder(1)]
		public String FileName
		{
			get { return _data.FileName; }
			set
			{
				_data.FileName = CopyLocal(value);
				IsDirty = true;
				_getNewVideoInfo = true;
				_processVideo = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Video Configuration", 2)]
		[ProviderDisplayName(@"Color Type")]
		[ProviderDescription(@"EffectColorType")]
		[PropertyOrder(2)]
		public EffectColorType EffectColorType
		{
			get { return _data.EffectColorType; }
			set
			{
				_data.EffectColorType = value;
				IsDirty = true;
				_processVideo = false;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Video Configuration", 2)]
		[ProviderDisplayName(@"StretchToGrid")]
		[ProviderDescription(@"StretchToGrid")]
		[PropertyOrder(3)]
		public bool StretchToGrid
		{
			get { return _data.StretchToGrid; }
			set
			{
				_data.StretchToGrid = value;
				IsDirty = true;
				_processVideo = true;
				if (StretchToGrid) MaintainAspect = false;
				UpdateScaleAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Video Configuration", 2)]
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
		[PropertyOrder(5)]
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
		[PropertyOrder(6)]
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
				_processVideo = false;
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
				_processVideo = true;
				OnPropertyChanged();
			}
		}

		#endregion

		private void UpdateAttributes()
		{
			UpdateScaleAttribute(false);
			UpdateStringOrientationAttributes();
			UpdateMovementRateAttribute(false);
			UpdateIterationsAttribute(false);
			TypeDescriptor.Refresh(this);
		}

		private void UpdateScaleAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2)
			{
				{"ScalePercent", !ScaleToGrid && !StretchToGrid},

				{"ScaleToGrid", !StretchToGrid}
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

		private void UpdateMovementRateAttribute(bool refresh = true)
		{
			bool movementRate = Type == EffectType.RenderPictureDownleft || Type == EffectType.RenderPictureDownright ||
			                    Type == EffectType.RenderPictureUpleft || Type == EffectType.RenderPictureUpright;
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{
				{"MovementRate", movementRate}
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}
		private void UpdateQualityAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{
				{"VideoQuality", TargetPositioning == TargetPositioningType.Locations}
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
			string destPathFilename = Path.Combine(VideoPath, name);
			if (path != destPathFilename)
			{
				File.Copy(path, destPathFilename, true);
			}
			return name;
		}
		
		protected override void SetupRender()
		{
			UpdateQualityAttribute();
			if ( _data.FileName == "") return;
			
			if (_processVideo || !Directory.Exists(_tempFilePath)) ProcessMovie(); // Check if directory exist is needed for when an effect is cloned.
			if (_videoFileDetected)
			{
				_currentMovieImageNum = 0;
			}
		}

		protected override void CleanUpRender()
		{
			_fp?.Dispose();
			_fp = null;
			_processVideo = true;
		}

		private void ProcessMovie()
		{
			EstablishTempFolder();
			
			string videoFilename = Path.Combine(VideoPath, _data.FileName);
			try
			{
				if (VideoQuality == 0 || _getNewVideoInfo) GetVideoInformation();

				string cropVideo = "";

				if (StretchToGrid) // Will stretch the image to the grid size.
				{
					_renderWidth = BufferWi;
					_renderHeight = BufferHt;
				}
				else
				{
					// Will scale the image to the grid size.
					GetNewImageSize(out _renderWidth, out _renderHeight, BufferWi, BufferHt);
					if (!ScaleToGrid) // Scale and crop the image based on users scale setting
					{
						_renderWidth = (int)(_renderWidth * ((double)ScalePercent / 100 + 1));
						_renderHeight = (int)(_renderHeight * ((double)ScalePercent / 100 + 1));
					}
				}

				double videoQuality = TargetPositioning == TargetPositioningType.Locations ? (double)VideoQuality / 100 : 1;
				if (_renderWidth > MaxRenderWidth || _renderHeight > MaxRenderHeight)
				{
					_ratioWidth = (double)_renderWidth / MaxRenderWidth / videoQuality;
					_ratioHeight = (double)_renderHeight / MaxRenderHeight / videoQuality;
					_renderHeight = (int)(MaxRenderHeight * videoQuality);
					_renderWidth = (int)(MaxRenderWidth * videoQuality);
				}
				else
				{
					_ratioWidth = _ratioHeight = 1;
				}

				if (!StretchToGrid && !ScaleToGrid)
				{
					int cropWidth = _renderWidth > BufferWi ? BufferWi : _renderWidth;
					int cropHeight = _renderHeight > BufferHt ? BufferHt : _renderHeight;
					cropVideo = $", crop={cropWidth}:{cropHeight}:{(_renderWidth - cropWidth) / 2}:{(_renderHeight - cropHeight) / 2}";
				}

				// Will adjust the render size if element is below 10 as FFMPEG could refuse to scale.
				if (_renderHeight < 10 || _renderWidth < 10)
				{
					// I don't see any point continuing if the element is this small.
					if (_renderHeight <= 2 || _renderWidth <= 2)
					{
						_videoFileDetected = false;
						return;
					}
					GetNewImageSize(out _renderWidth, out _renderHeight, 50, (int) (50 * ((double)_renderWidth / _renderHeight)));
				}

				// Gets selected video if Video length is longer then the entered start time.
				if (VideoLength > StartTimeSeconds + (TimeSpan.TotalSeconds * ((double)PlayBackSpeed / 100 + 1)))
				{
					ffmpeg.ffmpeg converter = new ffmpeg.ffmpeg(videoFilename);
					_currentMovieImageNum = 0;
					// Height and Width needs to be evenly divisible to work or ffmpeg complains.
					if (_renderHeight % 2 != 0) _renderHeight++;
					if (_renderWidth % 2 != 0) _renderWidth++;
					converter.MakeScaledThumbNails(_tempFilePath, StartTimeSeconds, ((TimeSpan.TotalSeconds * ((double)PlayBackSpeed / 100 + 1))),
						_renderWidth, _renderHeight, MaintainAspect, RotateVideo, cropVideo, 1000 / FrameTime);
					_moviePicturesFileList = Directory.GetFiles(_tempFilePath).OrderBy(f => f).ToList();

					_videoFileDetected = true;
				}
				else
				{
					var messageBox = new MessageBoxForm("Entered Start Time plus Effect length is greater than the Video Length of " + _data.FileName,
						"Invalid Start Time. Decrease the Start Time", MessageBoxButtons.OK, SystemIcons.Error);
					messageBox.StartPosition = FormStartPosition.CenterScreen;
					messageBox.TopMost = true;
					messageBox.ShowDialog();
					_videoFileDetected = false;
				}
			}
			catch (Exception ex)
			{
				Logging.Error(ex, $"There was a problem converting {videoFilename}");
				var messageBox = new MessageBoxForm("There was a problem converting " + videoFilename + ": " + ex.Message,
					"Error Converting Video", MessageBoxButtons.OK, SystemIcons.Error);
				messageBox.StartPosition = FormStartPosition.CenterScreen;
				messageBox.TopMost = true;
				messageBox.ShowDialog();
				_videoFileDetected = false;
			}
		}

		private void PopulateTempPath()
		{
			_tempFilePath = TempPath + Path.PathSeparator + InstanceId;
		}

		private void EstablishTempFolder()
		{
			//Delete old path and create new path for processed video
			RemoveTempFiles();

			_tempFilePath = Path.Combine(TempPath, InstanceId.ToString());
			Directory.CreateDirectory(_tempFilePath);
		}

		private void RemoveTempFiles()
		{
			if (Directory.Exists(_tempFilePath))
			{
				try
				{
					Directory.Delete(_tempFilePath, true);
				}
				catch (Exception e)
				{
					Logging.Error(e, "Unable to delete all the video temp files.");
				}
			}
		}

		#region Render Video Effect

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			if (!_videoFileDetected) return;
			InitFrameData(frame, out double intervalPos, out double intervalPosFactor, out double level, out double adjustedBrightness);
			InitialRender(intervalPos, intervalPosFactor);
			if (_fp != null)
			{
				_fp.Lock();
				var bufferWi = BufferWi;
				var bufferHt = BufferHt;
				for (int x = 0; x < _imageWi; x++)
				{
					for (int y = 0; y < _imageHt; y++)
					{
						CalculatePixel(x, y, frameBuffer, frame, level, adjustedBrightness, ref bufferHt, ref bufferWi);
					}
				}
				_fp.Unlock(false);
			}
		}

		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			if (!_videoFileDetected) return;
			var bufferWi = BufferWi;
			var bufferHt = BufferHt;
			//these lookups are a bit expensive when called a lot of times
			var bufferWiOffset = BufferWiOffset; 
			var bufferHtOffset = BufferHtOffset;

			for (int frame = 0; frame < numFrames; frame++)
			{
				frameBuffer.CurrentFrame = frame;

				InitFrameData(frame, out double intervalPos, out double intervalPosFactor, out double level, out double adjustedBrightness);
				InitialRender(intervalPos, intervalPosFactor);
				if (_fp != null)
				{
					_fp.Lock();
					
					foreach (var elementLocation in frameBuffer.ElementLocations)
					{
						CalculatePixel(elementLocation.X, elementLocation.Y, frameBuffer, frame, level, adjustedBrightness, ref bufferHt, ref bufferWi, true, bufferHtOffset, bufferWiOffset);
					}
					_fp.Unlock(false);
				}
			}
		}

		private void InitFrameData(int frame, out double intervalPos, out double intervalPosFactor, out double level, out double adjustedBrightness)
		{
			intervalPos = GetEffectTimeIntervalPosition(frame);
			intervalPosFactor = intervalPos * 100;
			level = LevelCurve.GetValue(intervalPosFactor) / 100;
			adjustedBrightness = CalculateIncreaseBrightness(intervalPosFactor) / 10;
		}

		private void InitialRender(double intervalPos, double intervalPosFactor)
		{
			// If we don't have any pictures, do nothing!
			if (_moviePicturesFileList == null || !_moviePicturesFileList.Any())
				return;
			_position = (intervalPos * Speed) % 1;

			int pictureCount = _moviePicturesFileList.Count;

			int currentImage = (int)_currentMovieImageNum;
			if (currentImage >= pictureCount)
			{
				_currentMovieImageNum = currentImage = pictureCount-1;
			}
			else if(currentImage < 0)
			{
				_currentMovieImageNum = currentImage = 0;
			}
			var image = Image.FromFile(_moviePicturesFileList[currentImage]);
			// Convert to Grey scale if selected.
			_fp = EffectColorType == EffectColorType.RenderGreyScale ? new FastPixel.FastPixel(new Bitmap(ConvertToGrayScale(image))) : new FastPixel.FastPixel(new Bitmap(image, (int)(_renderWidth * _ratioWidth), (int)(_renderHeight * _ratioHeight)));
			image.Dispose();

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
				_currentMovieImageNum++;
			}
			
			_imageWi = _fp.Width;
			_imageHt = _fp.Height;
			_yoffset = (BufferHt + _imageHt) / 2;
			_xoffset = (_imageWi - BufferWi) / 2;
			_xOffsetAdj = CalculateXOffset(intervalPosFactor) * BufferWi / 100;
			_yOffsetAdj = CalculateYOffset(intervalPosFactor) * BufferHt / 100;

			switch (Type)
			{
				case EffectType.RenderPicturePeekaboo0:
				case EffectType.RenderPicturePeekaboo180:
					//Peek a boo
					_yoffset = -(BufferHt) + (int)((BufferHt + 5) * _position * 2);
					if (_yoffset > 10)
					{
						_yoffset = -_yoffset + 10; //reverse direction
					}
					else if (_yoffset >= -1)
					{
						_yoffset = -1; //pause in middle
					}
					break;
				case EffectType.RenderPictureWiggle:
					if (_position >= 0.5)
					{
						_xoffset += (int)(_imageWi * ((1.0 - _position) * 2.0 - 0.5));
					}
					else
					{
						_xoffset += (int)(_imageWi * (_position * 2.0 - 0.5));
					}
					break;
				case EffectType.RenderPicturePeekaboo90: //peekaboo 90
				case EffectType.RenderPicturePeekaboo270: //peekaboo 270
					if (Orientation == StringOrientation.Horizontal || TargetPositioning == TargetPositioningType.Locations)
					{
						_yoffset = (BufferHt - _imageWi) / 2; //adjust offsets for other axis	
						_xoffset = -(BufferWi) + (int)((BufferWi + 5) * _position * 2);
					}
					else
					{
						_yoffset = (BufferWi) / 2; //adjust offsets for other axis
						_xoffset = -(BufferHt) + (int)((BufferHt + 5) * _position * 2);
					}

					if (_xoffset > 10)
					{
						_xoffset = -_xoffset + 10; //reverse direction
					}
					else if (_xoffset >= -1)
					{
						_xoffset = -1; //pause in middle
					}
					break;
			}
		}
		
		private void CalculatePixel(int x, int y, IPixelFrameBuffer frameBuffer, int frame, double level, double adjustedBrightness, ref int bufferHt, ref int bufferWi, bool locations=false, int bufferHtOffset = 0, int bufferWiOffset=0)
		{
			int yCoord = y;
			int xCoord = x;
			int locationY = 0;
			int locationX = 0;

			Color fpColor = Color.Empty;

			if (locations)
			{
				//Flip me over and offset my coordinates so I can act like the string version
				y = Math.Abs((bufferHtOffset - y) + (bufferHt - 1 + bufferHtOffset));
				y = y - bufferHtOffset;
				x = x - bufferWiOffset;
			}
			else
			{
				fpColor = GetIntensity(level, _fp.GetPixel(x, y), adjustedBrightness);
			}

			switch (Type)
			{
				case EffectType.RenderPicturePeekaboo0:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = bufferHt + _yoffset - y + _yOffsetAdj;
						locationX = x + _xoffset - _xOffsetAdj;
						break;
					}
					frameBuffer.SetPixel(xCoord - _xoffset + _xOffsetAdj, bufferHt + _yoffset - yCoord + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureWiggle:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = _yoffset - y + _yOffsetAdj;
						locationX = x - _xoffset - _xOffsetAdj - bufferWi + _imageWi;
						break;
					}
					frameBuffer.SetPixel(xCoord + _xoffset + _xOffsetAdj + bufferWi - _imageWi, _yoffset - yCoord + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPicturePeekaboo90:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = _xoffset + bufferWi - x - _xOffsetAdj;
						locationX = bufferHt - y - _yOffsetAdj - _yoffset;
						break;
					}
					frameBuffer.SetPixel(bufferWi + _xoffset - yCoord + _xOffsetAdj, bufferHt - xCoord - _yoffset + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPicturePeekaboo180:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = y - _yOffsetAdj + bufferHt - _imageHt + _yoffset;
						locationX = (bufferWi - x) + _xoffset + _xOffsetAdj;
						break;
					}
					if (Orientation == StringOrientation.Horizontal)
					{
						frameBuffer.SetPixel((_imageWi - xCoord) - _xoffset + _xOffsetAdj - bufferWi + bufferWi, yCoord - _yoffset + _yOffsetAdj, fpColor);
					}
					else
					{
						frameBuffer.SetPixel((bufferWi - xCoord) - _xoffset + _xOffsetAdj - _imageWi + bufferWi, yCoord - _yoffset + _yOffsetAdj, fpColor);
					}
					return;
				case EffectType.RenderPicturePeekaboo270:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = _xoffset + x - _xOffsetAdj;
						locationX = y - _yOffsetAdj - _yoffset;
						break;
					}
					frameBuffer.SetPixel(yCoord - _xoffset + _xOffsetAdj, bufferHt + _yoffset - (bufferHt - xCoord) + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureLeft:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = _yoffset - y + _yOffsetAdj;
						locationX = ((x - bufferWi) + (int)(_position * (_imageWi + bufferWi))) + _xOffsetAdj;
						break;
					}
					int leftX = xCoord + (bufferWi - (int)(_position * (_imageWi + bufferWi)));
					frameBuffer.SetPixel(leftX + _xOffsetAdj, _yoffset - y + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureRight:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = _yoffset - y + _yOffsetAdj;
						locationX = ((x + _imageWi) - (int)(_position * (_imageWi + bufferWi))) + _xOffsetAdj;
						break;
					}
					int rightX = xCoord + -_imageWi + (int)(_position * (_imageWi + bufferWi));
					frameBuffer.SetPixel(rightX + _xOffsetAdj, _yoffset - yCoord + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureUp:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = ((int)((_imageHt + bufferHt) * _position) - y) + _yOffsetAdj;
						locationX = x + _xoffset - _xOffsetAdj;
						break;
					}
					int upY = (int)((_imageHt + bufferHt) * _position) - y;
					frameBuffer.SetPixel(xCoord - _xoffset + _xOffsetAdj, upY + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureDown:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = ((bufferHt + _imageHt - 1) - (int)((_imageHt + bufferHt) * _position) - y) + _yOffsetAdj;
						locationX = x + _xoffset - _xOffsetAdj;
						break;
					}
					int downY = (bufferHt + _imageHt - 1) - (int)((_imageHt + bufferHt) * _position) - yCoord;
					frameBuffer.SetPixel(x - _xoffset + _xOffsetAdj, downY + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureUpleft:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = ((int)((_imageHt + bufferHt) * _position) - y) + _yOffsetAdj;
						locationX = (x - bufferWi + (MovementRate * frame % ((_imageWi + bufferWi) * SpeedFactor)) / SpeedFactor) + _xOffsetAdj;
						break;
					}
					int upLeftY = (int)((_imageHt + bufferHt) * _position) - yCoord;
					frameBuffer.SetPixel(
						Convert.ToInt32(xCoord + bufferWi - (MovementRate * frame % ((_imageWi + bufferWi) * SpeedFactor)) / SpeedFactor) + _xOffsetAdj,
						upLeftY + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureDownleft:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = ((bufferHt + _imageHt - 1) - (int)((_imageHt + bufferHt) * _position) - y) + _yOffsetAdj;
						locationX = (x - bufferWi + (MovementRate * frame % ((_imageWi + bufferWi) * SpeedFactor)) / SpeedFactor) + _xOffsetAdj;
						break;
					}
					int downLeftY = bufferHt + _imageHt - (int)((_imageHt + bufferHt) * _position) - yCoord;
					frameBuffer.SetPixel(
						Convert.ToInt32(xCoord + bufferWi - (MovementRate * frame % ((_imageWi + bufferWi) * SpeedFactor)) / SpeedFactor) + _xOffsetAdj,
						downLeftY + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureUpright:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = ((int)((_imageHt + bufferHt) * _position) - y) + _yOffsetAdj;
						locationX = (x + _imageWi - (MovementRate * frame % ((_imageWi + bufferWi) * SpeedFactor)) / SpeedFactor) + _xOffsetAdj;
						break;
					}
					int upRightY = (int)((_imageHt + bufferHt) * _position) - yCoord;
					frameBuffer.SetPixel(
						Convert.ToInt32(xCoord + (MovementRate * frame % ((_imageWi + bufferWi) * SpeedFactor)) / SpeedFactor - _imageWi) + _xOffsetAdj,
						upRightY + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureDownright:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = ((bufferHt + _imageHt - 1) - (int)((_imageHt + bufferHt) * _position) - y) + _yOffsetAdj;
						locationX = (x + _imageWi - (MovementRate * frame % ((_imageWi + bufferWi) * SpeedFactor)) / SpeedFactor) + _xOffsetAdj;
						break;
					}
					int downRightY = bufferHt + _imageHt - (int)((_imageHt + bufferHt) * _position) - yCoord;
					frameBuffer.SetPixel(
						Convert.ToInt32(xCoord + (MovementRate * frame % ((_imageWi + bufferWi) * SpeedFactor)) / SpeedFactor - _imageWi) + _xOffsetAdj,
						downRightY + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureNone:
					if (locations)
					{
						locationY = _yoffset - y + _yOffsetAdj;
						locationX = x + _xoffset - _xOffsetAdj;
						break;
					}
					frameBuffer.SetPixel(xCoord - _xoffset + _xOffsetAdj, _yoffset - yCoord + _yOffsetAdj, fpColor);
					return;
			}

			//will only get to here if Location and/or Tiles is selected.
			if (locationX < _fp.Width && locationY < _fp.Height && locationX >= 0 && locationY >= 0)
			{
				fpColor = GetIntensity(level, _fp.GetPixel(locationX, locationY), adjustedBrightness);
				frameBuffer.SetPixel(xCoord, yCoord, fpColor);
			}
		}

		#endregion

		private Color GetIntensity(double level, Color fpColor, double adjustedBrightness)
		{
			if (level < 1 || adjustedBrightness > 1)
			{
				HSV hsv = HSV.FromRGB(fpColor);
				double tempV = hsv.V * level * adjustedBrightness;
				if (tempV > 1) tempV = 1;
				hsv.V = tempV;
				fpColor = hsv.ToRGB();
			}

			return fpColor;
		}

		private void GetNewImageSize(out int renderWidth, out int renderHeight, int maxWidth, int maxHeight)
		{
			var ratioX = (double) maxWidth / _data.VideoSize.Width;
			var ratioY = (double) maxHeight / _data.VideoSize.Height;
			var ratio = maxHeight > maxWidth && !MaintainAspect ? Math.Max(ratioX, ratioY) : Math.Min(ratioX, ratioY);
			renderWidth = (int) (_data.VideoSize.Width * ratio);
			renderHeight = (int) (_data.VideoSize.Height * ratio);
			if (renderHeight <= 0) renderHeight = 1;
			if (renderWidth <= 0) renderWidth = 1;
		}
		
		public Image ConvertToGrayScale(Image image)
		{
			Bitmap srce = new Bitmap(image, (int)(_renderWidth * _ratioWidth), (int)(_renderHeight * _ratioHeight));
			using (Graphics gr = Graphics.FromImage(srce))
			{
				var matrix = new float[][]
				{
					new float[] {0.299f, 0.299f, 0.299f, 0, 0},
					new float[] {0.587f, 0.587f, 0.587f, 0, 0},
					new float[] {0.114f, 0.114f, 0.114f, 0, 0},
					new float[] {0, 0, 0, 1, 0},
					new float[] {0, 0, 0, 0, 1}
				};
				var ia = new System.Drawing.Imaging.ImageAttributes();
				ia.SetColorMatrix(new System.Drawing.Imaging.ColorMatrix(matrix));
				var rc = new Rectangle(0, 0, srce.Width, srce.Height);
				gr.DrawImage(srce, rc, 0, 0, srce.Width, srce.Height, GraphicsUnit.Pixel, ia);
				return srce;
			}
		}

		private void GetVideoInformation()
		{
			// This is only done each time a Video file is changed.
			// No point doing this every time it needs to render.
			// So once a user adds a video file to the effect this code will no longer be used.
			string videoFilename = Path.Combine(VideoPath, _data.FileName);
			try
			{
				VideoQuality = 50; // Set quality to 50% when a new file is opened.
				// Delete old path and create new path for processed video
				EstablishTempFolder();
				// Gets Video length and Frame rate will continue if users start position is less then the video length.
				ffmpeg.ffmpeg videoLengthInfo = new ffmpeg.ffmpeg(videoFilename);
				string result = videoLengthInfo.GetVideoInfo(_tempFilePath);
				// Get Video Length
				int durationIndex = result.IndexOf("Duration: ");
				string videoInfo = result.Substring(durationIndex + 10, 8);
				string[] words = videoInfo.Split(':');
				TimeSpan videoTimeSpan = new TimeSpan(Int32.Parse(words[0]), Int32.Parse(words[1]), Int32.Parse(words[2]));
				VideoLength = (int) videoTimeSpan.TotalSeconds;
				
				// Saves one frame from video then grabs it to determine Video size.
				// This was to replace the way Accord did it as it makes sense to do the Video size
				// conversion when generating all the images. This can reduces each bitmap file size significantly.
				ffmpeg.ffmpeg videoSizeInfo = new ffmpeg.ffmpeg(videoFilename);
				videoSizeInfo.GetVideoSize(_tempFilePath + "\\Temp.bmp");
				var image = Image.FromFile(_tempFilePath + "\\Temp.bmp");
				
				// Saves the Video info to data store.
				_data.VideoSize = new Size(image.Width, image.Height);

				image.Dispose();
				_getNewVideoInfo = false;
			}
			catch (Exception ex)
			{
				var messageBox = new MessageBoxForm("There was a problem converting " + videoFilename + ": " + ex.Message,
					"Error Converting Video", MessageBoxButtons.OK, SystemIcons.Error);
				messageBox.ShowDialog();
				_videoFileDetected = false;
			}

		}

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

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				RemoveTempFiles();
			}

			base.Dispose(disposing);
		}
	}
}