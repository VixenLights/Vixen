using System;
using System.Drawing;
using System.Collections.Generic;
using Common.Controls.ControlsEx;
using Vixen.Module.Editor;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace Common.Controls.Timeline
{
	[Serializable]
	public class Element : IComparable<Element>, ITimePeriod, IDisposable
	{
		private TimeSpan m_startTime;
		private TimeSpan m_duration;
		private Color m_backColor = Color.White;
		private Color m_borderColor = Color.Black;
		private object m_tag = null;
		private bool m_selected = false;
		private static System.Object drawLock = new System.Object();
		//private bool m_redraw = false;
		//private bool m_rendered = false;

		public Element()
		{
		}

		/// <summary>
		/// Copy constructor. Creates a shallow copy of other.
		/// </summary>
		/// <param name="other">The element to copy.</param>
		public Element(Element other)
		{
			m_startTime = other.m_startTime;
			m_duration = other.m_duration;
			m_backColor = other.m_backColor;
			m_tag = other.m_tag;
			m_selected = other.m_selected;
		}

		#region Begin/End update

		private TimeSpan m_origStartTime, m_origDuration;

		///<summary>Suspends raising events until EndUpdate is called.</summary>
		public void BeginUpdate()
		{
			m_origStartTime = this.StartTime;
			m_origDuration = this.Duration;
		}

		public void EndUpdate()
		{
			if ((StartTime != m_origStartTime) || (Duration != m_origDuration)) {
				OnTimeChanged();
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Display top for the last version of this element. Not reliable when the element is part of multiple rows.
		/// I.E. grouping. Use Row.DisplayTop for the containing row and RowTopOffset.
		/// </summary>
		public int DisplayTop { get; set; }
		public int RowTopOffset { get; set; }
		public int DisplayHeight { get; set; }
		public Rectangle DisplayRect { get; set; }
		public bool MouseCaptured { get; set; }

		[NonSerializedAttribute]
		public EffectNode _effectNode;

		public EffectNode EffectNode
		{
			get { return _effectNode; }
			set { _effectNode = value; }
		}

		public Row Row { get; set; }

		/// <summary>
		/// Gets or sets the starting time of this element (left side).
		/// </summary>
		public TimeSpan StartTime
		{
			get { return m_startTime; }
			set
			{
				if (value < TimeSpan.Zero)
					value = TimeSpan.Zero;

				if (m_startTime == value)
					return;

				m_startTime = value;
				OnTimeChanged();
			}
		}

		/// <summary>
		/// Gets or sets the time duration of this element (width).
		/// </summary>
		public TimeSpan Duration
		{
			get { return m_duration; }
			set
			{
				if (m_duration == value)
					return;

				m_duration = value;
				OnTimeChanged();
			}
		}

		/// <summary>
		/// Gets or sets the ending time of this element (right side).
		/// Changing this value adjusts the duration. The start time is unaffected.
		/// </summary>
		public TimeSpan EndTime
		{
			get { return StartTime + Duration; }
			set { Duration = (value - StartTime); }
		}


		public Color BackColor
		{
			get { return m_backColor; }
			set
			{
				m_backColor = value;
				CachedCanvasIsCurrent = false;
				Changed = true;
				OnContentChanged();
			}
		}

		public Color BorderColor
		{
			get { return m_borderColor; }
			set
			{
				m_borderColor = value;
				CachedCanvasIsCurrent = false;
				Changed = true;
				OnContentChanged();
			}
		}

		public object Tag
		{
			get { return m_tag; }
			set
			{
				m_tag = value;
				OnContentChanged();
			}
		}

		public bool Selected
		{
			get { return m_selected; }
			set
			{
				if (m_selected == value)
					return;

				m_selected = value;
				//m_redraw = true;
				//CachedCanvasIsCurrent = false;
				OnSelectedChanged();
			}
		}

		private bool _changed = true;

		public bool Changed
		{
			set
			{
				if (value) {
					//m_redraw = true;
					CachedCanvasIsCurrent = false;
				}
				//Console.WriteLine("Changed");
				_changed = value;
			}
			get { return _changed; }
		}

		#endregion

		#region Events

		/// <summary>
		/// Occurs when some of this element's other content changes.
		/// </summary>
		public event EventHandler ContentChanged;

		/// <summary>
		/// Occurs when this element's Selected state changes.
		/// </summary>
		public event EventHandler SelectedChanged;

		/// <summary>
		/// Occurs when one of this element's time propeties changes.
		/// </summary>
		public event EventHandler TimeChanged;

		#endregion

		#region Virtual Methods

		/// <summary>
		/// Raises the ContentChanged event.
		/// </summary>
		protected virtual void OnContentChanged()
		{
			if (ContentChanged != null)
				ContentChanged(this, EventArgs.Empty);
		}

		/// <summary>
		/// Raises the SelectedChanged event.
		/// </summary>
		protected virtual void OnSelectedChanged()
		{
			if (SelectedChanged != null)
				SelectedChanged(this, EventArgs.Empty);
		}

		/// <summary>
		/// Raises the TimeChanged event.
		/// </summary>
		protected virtual void OnTimeChanged()
		{
			if (TimeChanged != null)
				TimeChanged(this, EventArgs.Empty);
		}

		#endregion

		#region Methods

		public int CompareTo(Element other)
		{
			int rv = StartTime.CompareTo(other.StartTime);
			if (rv != 0)
				return rv;
			else
				return EndTime.CompareTo(other.EndTime);
		}

		public void MoveStartTime(TimeSpan offset)
		{
			if (m_startTime + offset < TimeSpan.Zero)
				offset = -m_startTime;

			m_duration -= offset;
			StartTime += offset;
		}

		#endregion

		#region Drawing

		private Bitmap CachedElementCanvas { get; set; }

		public bool CachedCanvasIsCurrent { get; set; }

		protected virtual Bitmap SetupCanvas(Size imageSize)
		{
			Bitmap result = new Bitmap(imageSize.Width, imageSize.Height);
			using (Graphics g = Graphics.FromImage(result)) {
				using (Brush b = new SolidBrush(BackColor)) {
					g.FillRectangle(b, 0, 0, imageSize.Width, imageSize.Height);
				}
			}
			return result;	
		}

		protected virtual void AddSelectionOverlayToCanvas(Graphics g)
		{
			// Width - bold if selected
			int b_wd = Selected ? 3 : 1;

			// Adjust the rect such that the border is completely inside it.
			Rectangle b_rect = new Rectangle(
				(b_wd/2),
				(b_wd/2),
				(int) g.VisibleClipBounds.Width - b_wd,
				(int) g.VisibleClipBounds.Height - b_wd
				);

			// Draw it!
			using (Pen border = new Pen(BorderColor)) {
				border.Width = b_wd;
				g.DrawRectangle(border, b_rect);
			}
		}

		protected virtual void DrawCanvasContent(Graphics graphics)
		{
		}

		public virtual bool IsCanvasContentCurrent(Size imageSize)
		{
			// DB: removed below. We don't care if the height is current. We stretch the bitmap anyway.
			return (CachedCanvasIsCurrent || CachedElementCanvas.Width != imageSize.Width ||
					CachedElementCanvas.Height != imageSize.Height);
			//return (CachedCanvasIsCurrent || CachedElementCanvas.Width != imageSize.Width);
		}

		public Bitmap SetupCachedImage(Size imageSize)
		{
			lock (drawLock) {
				Bitmap bitmap = new Bitmap(imageSize.Width, imageSize.Height);
				using(Graphics g = Graphics.FromImage(bitmap)){
					DrawCanvasContent(g);
					AddSelectionOverlayToCanvas(g);
					CachedElementCanvas = bitmap;
				}
			}
			CachedCanvasIsCurrent = true;
			Changed = false;
			
			return CachedElementCanvas;
		}

		public void DrawPlaceholder(Graphics g)
		{
			g.FillRectangle(new SolidBrush(Color.FromArgb(122, 122, 122)),
			                new Rectangle((int) g.VisibleClipBounds.Left, (int) g.VisibleClipBounds.Top,
			                              (int) g.VisibleClipBounds.Width, (int) g.VisibleClipBounds.Height));
		}

		public void DrawInfo(Graphics g, Rectangle rect) 
		{
			const int margin = 2;
			if (MouseCaptured)
			{
				Color TextColor = Color.FromArgb(60, 60, 60);

				// add text describing the effect
				using (Font f = new Font("Arial", 7))
				using (Brush b = new SolidBrush(TextColor))
				{
					g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

					string s;

					s = EffectNode.Effect.EffectName + "\r\n" +
						string.Format("Start: {0}", EffectNode.StartTime.ToString(@"m\:ss\.fff")) + "\r\n" +
						string.Format("Length: {0}", EffectNode.TimeSpan.ToString(@"m\:ss\.fff"));

					SizeF textSize = g.MeasureString(s, f);
					Rectangle destRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
					if (rect.Y < destRect.Height)
					{
						// Display the text below the effect
						destRect.Y += rect.Height + margin - 8;
					}
					else
					{
						// Display the text above the effect
						destRect.Y -= (int)textSize.Height + margin - 4;
					}
					// Full size info box. Comment out next two lines to clip
					destRect.Width = (int)textSize.Width + margin;
					destRect.Height = (int)textSize.Height + margin;
					
					g.FillRectangle(Brushes.White, new Rectangle(destRect.Left, destRect.Top, (int)Math.Min(textSize.Width + margin, destRect.Width), (int)Math.Min(textSize.Height + margin, destRect.Height)));
					g.DrawString(s, f, b, new Rectangle(destRect.Left + margin/2, destRect.Top + margin/2, destRect.Width - margin, destRect.Height - margin));
				}
			}
		}

		public Bitmap Draw(Size imageSize, bool useImageSize)
		{
			if (CachedElementCanvas == null)
			{
				return Draw(imageSize);
			}
			else
			{
				Size size = new Size(CachedElementCanvas.Width, CachedElementCanvas.Height);
				return Draw(size);
			}
		}

		public Bitmap Draw(Size imageSize)
		{
			if (CachedElementCanvas == null || !IsCanvasContentCurrent(imageSize)) {
				Bitmap b = SetupCanvas(imageSize);
				using (Graphics g = Graphics.FromImage(b)) {
					DrawPlaceholder(g);
					AddSelectionOverlayToCanvas(g);
					if (!m_selected) {
						Changed = true;
					}
				}
				return b;
			}
			else if (m_selected) {
				Bitmap b = new Bitmap(CachedElementCanvas);
				using (Graphics g = Graphics.FromImage(b)) {
					AddSelectionOverlayToCanvas(g);
				}
				return b;
			}
			return CachedElementCanvas;
		}

		#endregion

		~Element()
		{
			Dispose(false);
		}

		protected void Dispose(bool disposing)
		{
			if (disposing) {
			}
			if (CachedElementCanvas != null)
				CachedElementCanvas.Dispose();
		}

		public void Dispose()
		{
			Dispose(true);
		}
	}


	public class ElementTimeInfo : ITimePeriod
	{
		public ElementTimeInfo(Element elem)
		{
			StartTime = elem.StartTime;
			Duration = elem.Duration;
		}

		public TimeSpan StartTime { get; set; }
		public TimeSpan Duration { get; set; }

		public TimeSpan EndTime
		{
			get { return StartTime + Duration; }
		}

		public static void SwapTimes(ITimePeriod lhs, ITimePeriod rhs)
		{
			TimeSpan temp;

			temp = lhs.StartTime;
			lhs.StartTime = rhs.StartTime;
			rhs.StartTime = temp;

			temp = lhs.Duration;
			lhs.Duration = rhs.Duration;
			rhs.Duration = temp;
		}
	}

	public interface ITimePeriod
	{
		TimeSpan StartTime { get; set; }
		TimeSpan Duration { get; set; }
	}
}