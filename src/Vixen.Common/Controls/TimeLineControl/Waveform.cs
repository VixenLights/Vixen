using VixenModules.Media.Audio;
using System.Drawing.Drawing2D;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Common.Controls.TimelineControl;
using Common.Controls.TimelineControl.LabeledMarks;
using NLog;
using VixenModules.Media.Audio.SampleProviders;
using Font = System.Drawing.Font;
using FontStyle = System.Drawing.FontStyle;

namespace Common.Controls.Timeline
{

#pragma warning disable WFO1000  // This class is not a Designer type control so disabling to avoid all the noise of the hidden attribute
	/// <summary>
	/// Waveform visualizer class
	/// </summary>
	[System.ComponentModel.DesignerCategory("")] // Prevent this from showing up in designer.
	public sealed class Waveform : TimelineControlBase
	{
		private static Logger Logging = LogManager.GetCurrentClassLogger();
		private List<Sample> samples;
		private Audio audio;
		private bool _creatingSamples = false;
		private bool _showMarkAlignment;
		private IReadOnlyList<TimeSpan> _activeTimes = [];
		private const int MinimumHeight = 30;
		private readonly Subject<TimeSpan> _timePerPixelChangeSubject;
		private CancellationTokenSource _updateCancellationTokenSource;

		private readonly TimeLineGlobalEventManager _timeLineGlobalEventManager;
		private readonly TimeLineGlobalStateManager _timeLineGlobalStateManager;

		/// <summary>
		/// Gets or sets a value indicating whether interactive waveform height resizing is disabled.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if users cannot resize the waveform height with the mouse; otherwise,
		/// <see langword="false" />. The default is <see langword="false" />.
		/// </value>
		public bool LockWaveformHeight { get; set; }

		/// <summary>
		/// Creates a waveform view of the <code>Audio</code> that is associated scaled to the timeinfo.
		/// </summary>
		/// <param name="timeinfo"></param>
		public Waveform(TimeInfo timeinfo, Guid instanceId)
			: base(timeinfo)
		{
			samples = new List<Sample>();
			BackColor = Color.FromArgb(120,120,120);
			Visible = false;
			_timePerPixelChangeSubject = new Subject<TimeSpan>();
			_timePerPixelChangeSubject.Throttle(TimeSpan.FromMilliseconds(125)).Subscribe(x => CreateSamples());
			_timeLineGlobalStateManager = TimeLineGlobalStateManager.Manager(instanceId);
			_timeLineGlobalEventManager = TimeLineGlobalEventManager.Manager(instanceId);
			_timeLineGlobalEventManager.AlignmentActivity += WaveFormSelectedTimeLineGlobalMove;
			_timeLineGlobalEventManager.CursorMoved += CursorMoved;
		}

		private void CursorMoved(object sender, TimeSpanEventArgs e)
		{
			Invalidate();
		}

		private void WaveFormSelectedTimeLineGlobalMove(object sender, AlignmentEventArgs e)
		{
			var previousTimes = _activeTimes;
			var currentTimes = e.Active && e.Times != null ? e.Times.ToArray() : [];

			_showMarkAlignment = e.Active;
			_activeTimes = currentTimes;

			var invalidationRectangle = GetAlignmentInvalidationRectangle(previousTimes, currentTimes);
			if (!invalidationRectangle.IsEmpty)
			{
				Invalidate(invalidationRectangle);
			}
		}

		/// <summary>
		/// Gets the client area that must be repainted for an alignment guide.
		/// </summary>
		/// <param name="alignmentTime">The timeline time represented by the alignment guide.</param>
		/// <returns>A client-coordinate rectangle that contains the guide and its safety margin.</returns>
		internal Rectangle GetAlignmentInvalidationRectangle(TimeSpan alignmentTime)
		{
			var x = (int)MathF.Floor(timeToPixels(alignmentTime - VisibleTimeStart));
			var guideRectangle = new Rectangle(x - 2, 0, 5, ClientSize.Height);
			return Rectangle.Intersect(ClientRectangle, guideRectangle);
		}

		/// <summary>
		/// Gets the client area that must be repainted to remove previous alignment guides and draw current guides.
		/// </summary>
		/// <param name="previousTimes">The timeline times of the alignment guides previously displayed.</param>
		/// <param name="currentTimes">The timeline times of the alignment guides currently displayed.</param>
		/// <returns>A client-coordinate rectangle that contains every visible previous and current guide.</returns>
		internal Rectangle GetAlignmentInvalidationRectangle(IEnumerable<TimeSpan> previousTimes, IEnumerable<TimeSpan> currentTimes)
		{
			var invalidationRectangle = Rectangle.Empty;

			foreach (var alignmentTime in previousTimes.Concat(currentTimes))
			{
				var guideRectangle = GetAlignmentInvalidationRectangle(alignmentTime);
				if (guideRectangle.IsEmpty)
				{
					continue;
				}

				invalidationRectangle = invalidationRectangle.IsEmpty
					? guideRectangle
					: Rectangle.Union(invalidationRectangle, guideRectangle);
			}

			return invalidationRectangle;
		}

		/// <summary>
		/// Gets the bounded absolute waveform sample range that intersects a client paint clip.
		/// </summary>
		/// <param name="clipRectangle">The client-coordinate area being painted.</param>
		/// <returns>A half-open range of waveform sample indexes to draw.</returns>
		internal (int Start, int EndExclusive) GetVisibleSampleRange(Rectangle clipRectangle)
		{
			var visibleStartPixel = (int)MathF.Floor(timeToPixels(VisibleTimeStart));
			var mediaDurationPixel = audio == null ? 0 : (int)MathF.Floor(timeToPixels(audio.MediaDuration));
			var maximumSampleIndex = Math.Min(samples.Count, mediaDurationPixel);
			var start = Math.Clamp(visibleStartPixel + clipRectangle.Left - 1, 0, maximumSampleIndex);
			var endExclusive = Math.Clamp(visibleStartPixel + clipRectangle.Right + 1, start, maximumSampleIndex);

			return (start, endExclusive);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			//Adjusts WaveForm Height with a minimum of 40 pixels
			if (!LockWaveformHeight && e.Button == MouseButtons.Left && Cursor == Cursors.HSplit && e.Location.Y > MinimumHeight)
			{
				Height = e.Location.Y + 1;
			}
			else
			{
				Cursor = !LockWaveformHeight && e.Location.Y <= Height - 1 && e.Location.Y >= Height - 6 ? Cursors.HSplit : Cursors.Hand;
			}
		}

		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			base.OnMouseDoubleClick(e);

			//Resets WaveForm Height to default value of 50 when you double click the HSplit
			if (!LockWaveformHeight && Cursor == Cursors.HSplit)
			{
				Height = 50;
			}
		}

		//Create samples to scale based on the current timeline ticks period.
		//Runs in background to keep the ui free.
		private void CreateSamples()
		{
			if (_creatingSamples && _updateCancellationTokenSource != null)
			{
				_updateCancellationTokenSource?.Cancel();
				_updateCancellationTokenSource?.Dispose();
				_updateCancellationTokenSource = null;
			}
			_updateCancellationTokenSource = new CancellationTokenSource();
			var ct = _updateCancellationTokenSource.Token;

			var t = Task.Factory.StartNew(() =>
			{
				// Were we already canceled?
				ct.ThrowIfCancellationRequested();
			
				_creatingSamples = true;

				if (audio == null)
				{
					_creatingSamples = false;
					return;
				}

				if (!audio.MediaLoaded)
				{
					audio.LoadMedia(TimeSpan.Zero);
				}
			
				var totalPixels = timeToPixels(audio.MediaDuration);
				
				try
				{
					samples = audio.GetSamples((int)totalPixels, ct);
					_creatingSamples = false;

					if (InvokeRequired)
					{
						BeginInvoke((Action) FinishedSamples);
					}
				}
				catch (OperationCanceledException)
				{
					Logging.Info("Waveform create samples canceled.");
				}
				finally
				{
					_creatingSamples = false;
					_updateCancellationTokenSource?.Dispose();
					_updateCancellationTokenSource = null;
				}
			}, ct);
		}

		private void FinishedSamples()
		{
			//invalidate the control after the samples are created
			Invalidate();
		}

		/// <summary>
		/// sets the associated audio module to produce a waveform on
		/// </summary>
		public Audio Audio
		{
			set { SetAudio(value); }

			get { return audio; }
		}

		public WaveformStyle WaveformStyle { get; set; } = WaveformStyle.Half;

		private delegate void SetAudioDelegate(Audio value);

		private void SetAudio(Audio value)
		{
			if (InvokeRequired)
				Invoke(new SetAudioDelegate(SetAudio), value);
			else {
				//Clean up any existing audio. 
				if (audio != null) {
					audio.Dispose();
				}
				audio = value;
				if (audio != null)
				{
					_timePerPixelChangeSubject.OnNext(TimePerPixel);
					Visible = true;
					// Make us visible if we have audio to display.
				}
				else {
					Visible = false;
				}

				Invalidate();
			}
		}

		protected override Size DefaultSize
		{
			get { return new Size(400, 50); }
		}

		protected override void OnTimePerPixelChanged(object sender, EventArgs e)
		{
			_timePerPixelChangeSubject.OnNext(TimePerPixel);
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

		/// <inheritdoc />
		protected override void OnPaint(PaintEventArgs e)
		{
			if (VisibleTimeStart <= audio.MediaDuration)
			{
				if (samples.Count > 0 && !_creatingSamples)
				{
					//Draws the Mark alignment through the waveform if active mark is being moved.
					if (_showMarkAlignment)
					{
						Pen p;
						p = new Pen(Brushes.Yellow) { DashPattern = new float[] { 2, 2 } };

						foreach (var activeTime in _activeTimes)
						{
							var x1 = timeToPixels(activeTime - VisibleTimeStart);
							e.Graphics.DrawLine(p, x1, 0, x1, Height);
						}
							
						p.Dispose();
					}

					//Draws Waveform
					e.Graphics.TranslateTransform(-timeToPixels(VisibleTimeStart), 0);

					var drawBottom = WaveformStyle==WaveformStyle.Full?true:false;
					
					int workingHeight = Height - 2 - Height % 2; //Leave a little margin
					int topHeight = drawBottom?workingHeight/2:workingHeight;
					int bottomHeight = topHeight;
					int midPoint = topHeight;

					Pen bottomPen = null; 
					var topPen = CreatePen(topHeight);
					if (drawBottom)
					{
						bottomPen = CreatePen(bottomHeight,true);
					}

					var (start, endExclusive) = GetVisibleSampleRange(e.ClipRectangle);
					
					for (var x = start; x < endExclusive; x += 1)
					{
						if (samples.Count <= x) break;
						var lineHeight = topHeight * samples[x].High;
						e.Graphics.DrawLine(topPen, x, midPoint, x, midPoint - lineHeight);
						if (drawBottom)
						{
							lineHeight = bottomHeight * samples[x].Low;
							e.Graphics.DrawLine(bottomPen, x, midPoint, x, midPoint - lineHeight);
						}
					}

					topPen.Dispose();
					bottomPen?.Dispose();

					DrawCursor(e.Graphics);
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

		private static Pen CreatePen(int height, bool reverse = false)
		{
			var color1 = reverse ? Color.FromArgb(20, 20, 20) : Color.FromArgb(60, 60, 60);
			var color2 = reverse ? Color.FromArgb(60, 60, 60) : Color.FromArgb(20, 20, 20);
			var rect = new Rectangle(0, 0, 1, reverse?--height:height);
			var brush = new LinearGradientBrush(rect, color1, color2, LinearGradientMode.Vertical);
			return new Pen(brush);
		}

		private void DrawCursor(Graphics g)
		{
			using (Pen p = new Pen(Color.Blue, 1))
			{
				var curPos = timeToPixels(_timeLineGlobalStateManager.CursorPosition);
				g.DrawLine(p, curPos, 0, curPos, Height);
			}
		}
	}

	public enum WaveformStyle
	{
		Half,
		Full
	}
}
