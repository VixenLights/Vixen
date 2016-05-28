using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using NLog;
using VixenModules.Media.Audio;
using System.ComponentModel;
using Font = System.Drawing.Font;
using FontStyle = System.Drawing.FontStyle;

namespace Common.Controls.Timeline
{
	/// <summary>
	/// Waveform visualizer class
	/// </summary>
	[System.ComponentModel.DesignerCategory("")] // Prevent this from showing up in designer.
	public sealed class Waveform : TimelineControlBase
	{
		private double samplesPerPixel;
		private SampleAggregator samples;
		private Audio audio;
		private BackgroundWorker bw;
		private bool _creatingSamples = false;
		private bool waveFormMark;
		private TimeSpan selectedTime;

		/// <summary>
		/// Creates a waveform view of the <code>Audio</code> that is associated scaled to the timeinfo.
		/// </summary>
		/// <param name="timeinfo"></param>
		public Waveform(TimeInfo timeinfo)
			: base(timeinfo)
		{
			samples = new SampleAggregator();
			BackColor = Color.Gray;
			Visible = false;
			Ruler.SelectedMarkMove += waveForm_SelectedMarkMove;
		}

		private void waveForm_SelectedMarkMove(object sender, SelectedMarkMoveEventArgs e)
		{
			waveFormMark = e.WaveFormMark;
			selectedTime = e.SelectedMark;
			Refresh();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			//Adjusts WaveForm Height with a minimum of 40 pixels
			if (e.Button == MouseButtons.Left & Cursor == Cursors.HSplit & e.Location.Y > 40)
			{
				Height = e.Location.Y + 1;
			}
			else
			{
				Cursor = e.Location.Y <= Height - 1 && e.Location.Y >= Height - 6 ? Cursors.HSplit : Cursors.Hand;
			}
		}

		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			base.OnMouseDoubleClick(e);

			//Resets WaveForm Height to default value of 50 when you double click the HSplit
			if (Cursor == Cursors.HSplit)
			{
				Height = 50;
			}
		}

		private void CreateWorker()
		{
			if (bw != null) {
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
			_creatingSamples = true;
			BackgroundWorker worker = sender as BackgroundWorker;
			if (audio == null)
			{
				_creatingSamples = false;
				return;
			}
			if (!audio.MediaLoaded) {
				audio.LoadMedia(TimeSpan.MinValue);
			}
			samplesPerPixel = (double) pixelsToTime(1).Ticks/TimeSpan.TicksPerMillisecond*audio.Frequency/1000;
			int step = audio.BytesPerSample;
			samples.Clear();
			double samplesRead = 0;
			while (samplesRead < audio.NumberSamples) {
				if ((worker.CancellationPending)) {
					args.Cancel = true;
					break;
				}
				int low = 0;
				int high = 0;
				//Might need a better way to dither the partial samples out. Casting to int rounds it out while
				//the counter tries to maintian some sanity. A few random samples might be off at some zoom levels,
				//but we are doing a fair amount of averaging to begin with. The farther in the zoom the more chances 
				//a artifact might be visible.
				byte[] waveData = audio.GetSamples((int) samplesRead, (int) samplesPerPixel);
				samplesRead += samplesPerPixel;
				if (waveData == null)
					break;

				for (int n = 0; n < waveData.Length; n += step) {
					//Allow for 16 or 32 bit data. Should be 16 most of the time.
					int sample = step == 2 ? BitConverter.ToInt16(waveData, n) : BitConverter.ToInt32(waveData, n);

					if (sample < low) low = sample;
					if (sample > high) high = sample;
				}
				samples.Add(new SampleAggregator.Sample {High = high, Low = low});
			}
			_creatingSamples = false;
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
			set { SetAudio(value); }

			get { return audio; }
		}

		private delegate void SetAudioDelegate(VixenModules.Media.Audio.Audio value);

		private void SetAudio(VixenModules.Media.Audio.Audio value)
		{
			if (this.InvokeRequired)
				this.Invoke(new SetAudioDelegate(SetAudio), value);
			else {
				//Clean up any existing audio. 
				if (audio != null) {
					audio.Dispose();
				}
				audio = value;
				if (audio != null) {
					if (bw != null && bw.IsBusy) {
						bw.CancelAsync();
					}
					CreateWorker();
					bw.RunWorkerAsync();
					Visible = true;
					// Make us visible if we have audio to display.
				}
				else {
					Visible = false;
				}

				this.Invalidate();
			}
		}

		protected override Size DefaultSize
		{
			get { return new Size(400, 50); }
		}

		protected override void OnTimePerPixelChanged(object sender, EventArgs e)
		{
			if (bw != null && bw.IsBusy) {
				bw.CancelAsync();
			}
			while (_creatingSamples)
			{
				Thread.Sleep(1);
			}
			CreateWorker();
			bw.RunWorkerAsync();
		}

		protected override void OnPlaybackStartTimeChanged(object sender, EventArgs e)
		{
			//Do nothing
		}

		protected override void OnPlaybackEndTimeChanged(object sender, EventArgs e)
		{
			//Do nothing
		}

		protected override void OnPlaybackCurrentTimeChanged(object sender, EventArgs e)
		{
			//Do nothing
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (VisibleTimeStart <= audio.MediaDuration)
			{
				if (samples.Count > 0 && !_creatingSamples)
				{
					//Draws the Mark throught the waveform if Mark is being moved.
					if (waveFormMark)
					{
						Pen p;
						p = new Pen(Brushes.Yellow) { DashPattern = new float[] { 2, 2 } };
						Single x1 = timeToPixels(selectedTime - VisibleTimeStart);
						e.Graphics.DrawLine(p, x1, 0, x1, Height);
						p.Dispose();
					}

					//Draws Waveform
					e.Graphics.TranslateTransform(-timeToPixels(VisibleTimeStart), 0);
					float maxSample = Math.Max(Math.Abs(samples.Low), samples.High);
					int workingHeight = Height - (int) (Height*.1); //Leave a little margin
					float factor = workingHeight/maxSample;

					float maxValue = 2*maxSample*factor;
					float minValue = -maxSample*factor;
					int start = (int) timeToPixels(VisibleTimeStart);
					int end = (int) timeToPixels(VisibleTimeEnd <= audio.MediaDuration ? VisibleTimeEnd : audio.MediaDuration);
					
					for (int x = start; x < end; x += 1)
					{
						if (samples.Count <= x) break;
						{
							float lowPercent = (((samples[x].Low*factor) - minValue)/maxValue);
							float highPercent = (((samples[x].High*factor) - minValue)/maxValue);
							e.Graphics.DrawLine(Pens.Black, x, workingHeight*lowPercent, x, workingHeight*highPercent);
						}
					}
					
				}
				else
				{
					using (Font f = new Font(Font.FontFamily, 10f, FontStyle.Regular))
					{
						e.Graphics.DrawString("Building waveform.....", f, Brushes.Black,
							new Point((int)timeToPixels(VisibleTimeStart) + 15,
								(int)(Height - f.GetHeight(e.Graphics)) / 2),
							new StringFormat { Alignment = StringAlignment.Near });
					}
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
				if (sample.Low < Low) {
					Low = sample.Low;
				}
				if (sample.High > High) {
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
		protected override void Dispose(bool disposing)
		{
			//Only delete the Audio if Dispose call is explicit.
			if ((audio != null) && (disposing == true)) 
			{
				audio.Dispose();
				audio= null;
			}

			if (samples != null) {
				samples.Clear();
				samples	 = null;
				//samples = new SampleAggregator();
			}
			base.Dispose(disposing);
		}

	}
}