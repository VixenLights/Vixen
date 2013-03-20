using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VixenModules.Media.Audio;
using System.ComponentModel;

namespace Common.Controls.Timeline
{
	/// <summary>
	/// Waveform visualizer class
	/// </summary>
	[System.ComponentModel.DesignerCategory("")]    // Prevent this from showing up in designer.
	public sealed class Waveform : TimelineControlBase
	{
		private double samplesPerPixel;
		private SampleAggregator samples = new SampleAggregator();
		private Audio audio;
		private BackgroundWorker bw;
		/// <summary>
		/// Creates a waveform view of the <code>Audio</code> that is associated scaled to the timeinfo.
		/// </summary>
		/// <param name="timeinfo"></param>
		public Waveform(TimeInfo timeinfo)
			:base(timeinfo)
		{
			BackColor = Color.Gray;
			Visible = false;
			
		}

		private void CreateWorker()
		{
			if (bw != null)
			{
				bw.DoWork -= new DoWorkEventHandler(bw_createScaleSamples);
				bw.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
			}
			bw = new BackgroundWorker();
			bw.WorkerSupportsCancellation = true;
			bw.DoWork += new DoWorkEventHandler(bw_createScaleSamples);
			bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
		}

		//Create samples to scale based on the current timeline ticks period.
		//Runs in background to keep the ui free.
		private void bw_createScaleSamples(object sender, DoWorkEventArgs args)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			if (audio == null) { return; }
			if (!audio.MediaLoaded)
			{
				audio.LoadMedia(TimeSpan.MinValue);
			}
			samplesPerPixel = (double)pixelsToTime(1).Ticks / TimeSpan.TicksPerMillisecond * audio.Frequency / 1000;
			int step = audio.BytesPerSample;
			samples.Clear();
			double samplesRead = 0;
			while (samplesRead < audio.NumberSamples)
			{
				if ((worker.CancellationPending == true))
				{
					args.Cancel = true;
					break;
				}
				int low = 0;
				int high = 0;
				//Might need a better way to dither the partial samples out. Casting to int rounds it out while
				//the counter tries to maintian some sanity. A few random samples might be off at some zoom levels,
				//but we are doing a fair amount of averaging to begin with. The farther in the zoom the more chances 
				//a artifact might be visible.
				byte[] waveData = audio.GetSamples((int)samplesRead, (int)samplesPerPixel);
				samplesRead += samplesPerPixel;
				if (waveData == null)
					break;

				for (int n = 0; n < waveData.Length; n += step)
				{
					//Allow for 16 or 32 bit data. Should be 16 most of the time.
					int sample = step == 2 ? BitConverter.ToInt16(waveData, n) : BitConverter.ToInt32(waveData, n);

					if (sample < low) low = sample;
					if (sample > high) high = sample;
				}
				samples.Add(new SampleAggregator.Sample { High = high, Low = low });

			}
		}

		private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			//invalidate the control after the samples are created
			this.Invalidate();	
		}

		/// <summary>
		/// sets the associated audio module to produce a waveform on
		/// </summary>
		public Audio Audio
		{
			set
			{
				//Clean up any existing audio. 
				if (audio != null)
				{
					audio.Dispose();
				}
				audio = value;
				if (audio != null)
				{
					if (bw != null && bw.IsBusy)
					{
						bw.CancelAsync();
					}
					CreateWorker();
					bw.RunWorkerAsync();
					Visible = true;// Make us visible if we have audio to display.
				}
				else
				{
					Visible = false;
				}

				this.Invalidate();
			}

			get { return audio; }
		}

		protected override Size DefaultSize
		{
			get { return new Size(400, 50); }
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
		}

		protected override void OnTimePerPixelChanged(object sender, EventArgs e)
		{
			if (bw != null && bw.IsBusy)
			{
				bw.CancelAsync();
			}
			base.OnTimePerPixelChanged(sender, e);
			
			CreateWorker();
			bw.RunWorkerAsync();
		}

		protected override void OnVisibleTimeStartChanged(object sender, EventArgs e)
		{
			this.Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (samples.Count > 0 && !bw.IsBusy)
			{
				e.Graphics.TranslateTransform(-timeToPixels(VisibleTimeStart), 0);
				float maxSample = Math.Max(Math.Abs(samples.Low), samples.High);
				int workingHeight = Height - (int)(Height * .1); //Leave a little margin
				float factor = (float)(workingHeight) / maxSample;

				float maxValue = 2 * maxSample * factor;
				float minValue = -maxSample * factor;
				int start = (int)timeToPixels(VisibleTimeStart);
				int end = (int)timeToPixels(VisibleTimeEnd);

				for (int x = start; x < end; x += 1)
				{
					float lowPercent = ((((float)samples[x].Low * factor) - minValue) / maxValue);
					float highPercent = ((((float)samples[x].High * factor) - minValue) / maxValue);
					e.Graphics.DrawLine(Pens.Black, x, workingHeight * lowPercent, x, workingHeight * highPercent);
				}
			}
			else
			{
				using (Font f = new Font(this.Font.FontFamily, 10f, FontStyle.Regular))
				{
					e.Graphics.DrawString("Building waveform.....", f, Brushes.Black, new Point((int)timeToPixels(VisibleTimeStart) + 15, (int)(Height - f.GetHeight(e.Graphics)) / 2 ), new StringFormat { Alignment = StringAlignment.Near });
				}
			}

			base.OnPaint(e);
		}

		private class SampleAggregator : IList<SampleAggregator.Sample>
		{
			private readonly List<Sample> samples = new List<Sample>();
			public struct Sample
			{
				public int Low;

				public int High;
			}

			public int Low { get; set; }

			public int High { get; set; }

			public void Add(Sample sample)
			{
				samples.Add(sample);
				if (sample.Low < Low)
				{
					Low = sample.Low;
				}
				if (sample.High > High)
				{
					High = sample.High;
				}
			}

			public void Clear()
			{
				samples.Clear();
				High = 0;
				Low = 0;
			}

			public bool Contains(Sample item)
			{
				return samples.Contains(item);
			}

			public void CopyTo(Sample[] array, int arrayIndex)
			{
				throw new NotImplementedException();
			}

			public bool Remove(Sample item)
			{
				throw new NotImplementedException();
			}

			public int Count
			{
				get { return samples.Count; }
			}

			public bool IsReadOnly
			{
				get { return false; }
			}

			public IEnumerator<Sample> GetEnumerator()
			{
				return samples.GetEnumerator();
			}

			public int IndexOf(Sample item)
			{
				return samples.IndexOf(item);
			}

			public void Insert(int index, Sample item)
			{
				throw new NotImplementedException();
			}

			public void RemoveAt(int index)
			{
				throw new NotImplementedException();
			}

			public Sample this[int index]
			{
				get { return samples[index]; }
				set { throw new NotImplementedException(); }
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}
	}
}
