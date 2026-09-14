using Common.Controls.Timeline;
using Common.Controls.TimelineControl;
using Common.Controls.TimelineControl.LabeledMarks;
using System.ComponentModel;
using VixenModules.Media.Audio;

namespace VixenModules.Analysis.BeatsAndBars
{
	public partial class PreviewWaveform : UserControl
	{
		private Waveform _waveform;
		private readonly TimeInfo _info;
		private readonly Guid _instanceId = Guid.NewGuid();
		
		public PreviewWaveform(Audio audio)
		{
			InitializeComponent();

			_info = new TimeInfo
			{
				TotalTime = new TimeSpan(0, 0, 0, 1)
			};
			_waveform = new Waveform(_info, _instanceId);
			_waveform.WaveformStyle = WaveformStyle.Full;
			_waveform.BorderStyle = BorderStyle.FixedSingle;
			_waveform.Audio = audio;
			_waveform.BackColor = Color.LightGray;
			_waveform.Paint += PreviewWaveform_Paint;

			Controls.Add(_waveform);
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new int Width
		{
			get { return base.Width; }
			set
			{
				base.Width = value;
				_waveform.Width = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new int Height
		{
			get { return base.Height; }
			set
			{
				base.Height = value;
				_waveform.Height = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TimeSpan PreviewPeriod
		{
			get { return _info.TotalTime; }
			set
			{
				_info.TotalTime = value;
				_info.TimePerPixel = 
					new TimeSpan(value.Ticks / _waveform.Width);
				Invalidate();
			}
		}

		private List<TimeSpan> _intervalMarks;

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<TimeSpan> IntervalMarks
		{
			get
			{
				List<TimeSpan> retVal = new List<TimeSpan>(_intervalMarks);
				retVal.Reverse();
				return retVal;
			}

			set
			{
				_intervalMarks = new List<TimeSpan>(value);
				_intervalMarks.Reverse();
				Invalidate();
			}
		}

		private void PreviewWaveform_Paint(object sender, PaintEventArgs e)
		{
			var timeStack = new Stack<TimeSpan>(_intervalMarks);
			long tpp = _info.TimePerPixel.Ticks;
			Pen drawPen = new Pen(Color.Yellow, 2);
			Point x1 = new Point(0,0);
			Point x2 = new Point(0,Height);

			if (timeStack.Count > 0)
			{
				long compareVal = timeStack.Pop().Ticks;

				for (int j = 0; j < _waveform.Width; j++)
				{
					if (compareVal <= tpp * j)
					{
						x1.X = j;
						x2.X = j;

						e.Graphics.DrawLine(drawPen, x1, x2);
						if (timeStack.Count == 0)
						{
							break;
						}
						compareVal = timeStack.Pop().Ticks;
					}
				}
			}
		}

		private void _DisposePreviewWaveform()
		{
			TimeLineGlobalEventManager.CloseManager(_instanceId);
			TimeLineGlobalStateManager.CloseManager(_instanceId);
			if (_waveform != null)
			{
				_waveform.Paint += PreviewWaveform_Paint;
				_waveform.Dispose();
				_waveform = null;
			}
		}
	}
}
