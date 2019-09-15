using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Glediator
{
	public class Glediator : PixelEffectBase
	{
		private GlediatorData _data;
		private double _speed = 1;
		private FileStream _f;
		private double _position;
		private int _seqNumChannels;
		private byte[] _glediatorFrameBuffer;

		public Glediator()
		{
			_data = new GlediatorData();
			UpdateAttributes();
		}

		#region Movement

		[Value]
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"Movement Type")]
		[ProviderDescription(@"Movement Type")]
		[PropertyOrder(0)]
		public MovementType MovementType
		{
			get { return _data.MovementType; }
			set
			{
				_data.MovementType = value;
				IsDirty = true;
				UpdateMovementTypeAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"Speed")]
		[ProviderDescription(@"Speed")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
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
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"Iterations")]
		[ProviderDescription(@"Iterations")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 20, 1)]
		[PropertyOrder(3)]
		public int Iterations
		{
			get { return _data.Iterations; }
			set
			{
				_data.Iterations = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Config

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
		[ProviderDescription(@"Recorder file from Glediator")]
		[PropertyEditor("GledPathEditor")]
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

		public override string Information
		{
			get { return "Download Glediator using the More Info link. Configure Glediator with the same Matrix size."; }
		}

		public override string InformationLink
		{
			get { return "http://www.solderlab.de/index.php/software/glediator"; }
		}

		private void UpdateAttributes()
		{
			UpdateMovementTypeAttribute(false);
			TypeDescriptor.Refresh(this);
		}

		private void UpdateMovementTypeAttribute(bool refresh = true)
		{
			bool movementType = MovementType == MovementType.Speed;
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1);
			propertyStates.Add("Speed", movementType);
			propertyStates.Add("Iterations", !movementType);
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
				_data = value as GlediatorData;
				UpdateAttributes();
				IsDirty = true;
			}
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		private string ConvertPath(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				Logging.Warn("Path is empty!.");
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
			var destPath = Path.Combine(GlediatorDescriptor.ModulePath, name);
			if (path != destPath)
			{
				File.Copy(path, destPath, true);
			}
			return name;
		}

		protected override void SetupRender()
		{
			if (string.IsNullOrEmpty(FileName)) return;
			var filePath = Path.Combine(GlediatorDescriptor.ModulePath, FileName);
			CleanupStream(); //enusre that we don't leave a stream laying around if for some reason it exists.
			if (File.Exists(filePath))
			{
				_f = new FileStream(filePath, FileMode.Open, FileAccess.Read);
			}
			_seqNumChannels = (BufferWi * 3 * BufferHt);
			_glediatorFrameBuffer = new byte[_seqNumChannels];
		}

		protected override void CleanUpRender()
		{
			_glediatorFrameBuffer = null;
			CleanupStream();
		}

		private void CleanupStream()
		{
			if (_f != null)
			{
				try
				{
					_f.Dispose();
					_f = null;
				}
				catch (Exception e)
				{
					Logging.Error(e, "Failed to dispose of the filestream properly.");
				}
			}
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			if (_f == null) return;
			long fileLength = _f.Length;
			double level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;

			int seqNumPeriods = (int) (fileLength/ _seqNumChannels);

			if (frame == 0)
			{
				if (MovementType == MovementType.Iterations)
					_position = seqNumPeriods/(TimeSpan.TotalMilliseconds/FrameTime)*Iterations; //Iterations
				else
					_position = Speed; //Speed
			}

			_speed += _position;
			int period = (int)_speed % seqNumPeriods;
			int offset = _seqNumChannels * period;
			_f.Seek(offset, SeekOrigin.Begin);
			long readcnt = _f.Read(_glediatorFrameBuffer, 0, _seqNumChannels);

			for (int j = 0; j < readcnt; j += 3)
			{
				// Loop thru all channels
				Color color = Color.FromArgb(255, _glediatorFrameBuffer[j], _glediatorFrameBuffer[j + 1], _glediatorFrameBuffer[j + 2]);
				int x = (j%(BufferWi*3))/3;
				int y = (BufferHt - 1) - (j/(BufferWi*3));
				if (level < 1)
				{
					var hsv = HSV.FromRGB(color);
					hsv.V *= level;
					color = hsv.ToRGB();
				}
				if (x < BufferWi && y < BufferHt && y >= 0)
				{
					frameBuffer.SetPixel(x, y, color);
				}
			}
		}
	}
}
