using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.Controls.Timeline;
using VixenModules.Media.Audio;

namespace VixenModules.Analysis.BeatsAndBars
{
	public partial class PreviewWaveform : UserControl
	{
		private Waveform m_waveform;
		private TimeInfo m_info;
		
		public PreviewWaveform(Audio audio)
		{
			InitializeComponent();

			m_info = new TimeInfo();
			m_info.TotalTime = new TimeSpan(0, 0, 0, 1);
			m_waveform = new Waveform(m_info);
			m_waveform.BorderStyle = BorderStyle.FixedSingle;
			m_waveform.Audio = audio;
			m_waveform.BackColor = Color.LightGray;
			m_waveform.Paint += PreviewWaveform_Paint;

			Controls.Add(m_waveform);
		}

		public new int Width
		{
			get { return base.Width; }
			set
			{
				base.Width = value;
				m_waveform.Width = value;
			}
		}

		public new int Height
		{
			get { return base.Height; }
			set
			{
				base.Height = value;
				m_waveform.Height = value;
			}
		}

		public new Color BackColor
		{
			get { return m_waveform.BackColor; }
			set { m_waveform.BackColor = value; }
		}

		public TimeSpan PreviewPeriod
		{
			get { return m_info.TotalTime; }
			set
			{
				m_info.TotalTime = value;
				m_info.TimePerPixel = 
					new TimeSpan(value.Ticks / m_waveform.Width);
				Invalidate();
			}
		}

		private List<TimeSpan> m_intervalMarks;

		public List<TimeSpan> IntervalMarks
		{
			get
			{
				List<TimeSpan> retVal = new List<TimeSpan>(m_intervalMarks);
				retVal.Reverse();
				return retVal;
			}

			set
			{
				m_intervalMarks = new List<TimeSpan>(value);
				m_intervalMarks.Reverse();
				Invalidate();
			}
		}

		private void PreviewWaveform_Paint(object sender, PaintEventArgs e)
		{
			var timeStack = new Stack<TimeSpan>(m_intervalMarks);
			long tpp = m_info.TimePerPixel.Ticks;
			Pen drawPen = new Pen(Color.Yellow, 2);
			Point x1 = new Point(0,0);
			Point x2 = new Point(0,Height);

			if (timeStack.Count > 0)
			{
				long compareVal = timeStack.Pop().Ticks;

				for (int j = 0; j < m_waveform.Width; j++)
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
			if (m_waveform != null)
			{
				m_waveform.Paint += PreviewWaveform_Paint;
				m_waveform.Dispose();
				m_waveform = null;
			}
		}
	}
}
