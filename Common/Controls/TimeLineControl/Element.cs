using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Windows.Forms.VisualStyles;
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
		private ElementNode[] targetNodes;
		private static Color m_backColor = Color.FromArgb(0, 0, 0, 0);
		private static Color m_gray = Color.FromArgb(122, 122, 122);
		private static Color m_borderColor = Color.Black;
		private bool m_selected = false;
		private static Font m_textFont = new Font("Arial", 7);
		private static Color m_textColor = Color.FromArgb(255, 255, 255);
		private static Brush infoBrush = new SolidBrush(Color.FromArgb(128,0,0,0));
		//private static System.Object drawLock = new System.Object();
		protected internal bool suspendEvents = false;
		private Bitmap cachedImage;
		private TimeSpan elementVisibleStartTime;
		private TimeSpan elementVisibleEndTime;
		
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
			m_selected = other.m_selected;
			targetNodes = other.targetNodes;
		}

		#region Begin/End update

		private TimeSpan m_origStartTime, m_origDuration;
		private ElementNode[] origTargetNodes;

		///<summary>Suspends raising events until EndUpdate is called.</summary>
		public void BeginUpdate()
		{
			suspendEvents = true;
			m_origStartTime = StartTime;
			m_origDuration = Duration;
			origTargetNodes = targetNodes;
		}

		public void EndUpdate()
		{
			suspendEvents = false;
			if ((StartTime != m_origStartTime) || (Duration != m_origDuration)) {
				OnTimeChanged();
			}
			if (origTargetNodes != targetNodes)
			{
				EffectNode.Effect.TargetNodes = targetNodes;
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
		public int StackIndex { get; set; }
		public int StackCount { get; set; }


		[NonSerializedAttribute]
		public EffectNode _effectNode;

		public EffectNode EffectNode
		{
			get { return _effectNode; }
			set { _effectNode = value; }
		}

		/// <summary>
		/// This is the last row that this element was associated with. This element can be part of more than one row if it is part of multiple groups
		/// So do not trust it. Already leads to a issue if the rows I belong to are different heights.
		/// The element will not be drawn correctly because the cached image height is based on this
		/// </summary>
		public Row Row { get; set; }

		public ElementNode[] TargetNodes
		{
			protected get
			{
				return targetNodes;
			}
			set
			{		
				targetNodes = value;
			}
		}

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

		public bool Selected
		{
			get { return m_selected; }
			set
			{
				if (m_selected == value)
					return;

				m_selected = value;
				if (cachedImage != null)
				{
					cachedImage.Dispose();
					cachedImage = null;
				}
				OnSelectedChanged();
			}
		}

		public bool IsRendered
		{
			get { return !EffectNode.Effect.IsDirty; }
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

		/// <summary>
		/// Occurs when the Effects target nodes have changed.
		/// </summary>
		public event EventHandler TargetNodesChanged;

		
		#endregion

		#region Virtual Methods

		/// <summary>
		/// Raises the Target Nodes Changed event
		/// </summary>
		protected virtual void OnTargetNodesChanged()
		{
			EventHandler handler = TargetNodesChanged;
			if (!suspendEvents && handler != null) 
				handler(this, EventArgs.Empty);
		}


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
			if (!suspendEvents && TimeChanged != null)
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

		#endregion

		#region Drawing

		protected virtual void AddSelectionOverlayToCanvas(Graphics g, bool drawSelected, bool includeLeft, bool includeRight)
		{
			// Width - bold if selected
			int borderWidth = drawSelected ? 3 : 1;

			// Adjust the rect such that the border is completely inside it.
			Rectangle borderRectangle = new Rectangle(
				(int) g.VisibleClipBounds.Left, (int)g.VisibleClipBounds.Top,
				(int) g.VisibleClipBounds.Width, (int) g.VisibleClipBounds.Height
				);

			// Draw it!
			using (Pen border = new Pen(m_borderColor,borderWidth))
			{
				border.Alignment = PenAlignment.Inset;
				
				g.DrawLine(border, borderRectangle.Left, borderRectangle.Top, borderRectangle.Right, borderRectangle.Top);
				g.DrawLine(border, borderRectangle.Left, borderRectangle.Bottom, borderRectangle.Right, borderRectangle.Bottom);

				if (includeRight)
				{
					g.DrawLine(border, borderRectangle.Right, borderRectangle.Top, borderRectangle.Right, borderRectangle.Bottom);
				}
				if (includeLeft)
				{
					g.DrawLine(border, borderRectangle.Left, borderRectangle.Top, borderRectangle.Left, borderRectangle.Bottom);
				}	
			
			}
		}

		protected virtual void DrawCanvasContent(Graphics graphics, TimeSpan startTime, TimeSpan endTime, int overallWidth)
		{
		}

		public void RenderElement()
		{
			if (!IsRendered)
			{
				EffectNode.Effect.Render();
				if (cachedImage != null)
				{
					cachedImage.Dispose();
					cachedImage = null;
				}
				
			}
		}

		protected Bitmap DrawImage(Size imageSize, TimeSpan startTime, TimeSpan endTime, int overallWidth)
		{
			TimeSpan visibleStartOffset;
			TimeSpan visibleEndOffset;
			if (startTime > StartTime)
			{
				//We are starting somewhere in the middle of the effect
				visibleStartOffset = startTime - StartTime;
			} else
			{
				//The effect starts in our visible region
				visibleStartOffset = TimeSpan.Zero;
			}
			if (endTime < EndTime)
			{
				//The effect ends past our visible region
				visibleEndOffset = endTime - StartTime;
			} else
			{

				visibleEndOffset = EndTime;
			}

			if (cachedImage == null || visibleStartOffset != elementVisibleStartTime || visibleEndOffset != elementVisibleEndTime || cachedImage.Height != imageSize.Height || cachedImage.Width != imageSize.Width)
			{
				cachedImage = new Bitmap(imageSize.Width, imageSize.Height);
				using (Graphics g = Graphics.FromImage(cachedImage))
				{
					DrawCanvasContent(g, visibleStartOffset, visibleEndOffset, overallWidth);
					AddSelectionOverlayToCanvas(g, m_selected, startTime <= StartTime, endTime >= EndTime);
				}
				elementVisibleStartTime = visibleStartOffset;
				elementVisibleEndTime = visibleEndOffset;
			}
			
			return cachedImage;
		}

		public Bitmap DrawPlaceholder(Size imageSize)
		{
			Bitmap result = new Bitmap(imageSize.Width, imageSize.Height);
			using (Graphics g = Graphics.FromImage(result))
			{
				using (Brush b = new SolidBrush(m_gray))
				{
					g.FillRectangle(b,
							new Rectangle((int)g.VisibleClipBounds.Left, (int)g.VisibleClipBounds.Top,
										  (int)g.VisibleClipBounds.Width, (int)g.VisibleClipBounds.Height));	
				}

				AddSelectionOverlayToCanvas(g, m_selected, true, true);
			}
			cachedImage = result;
			return cachedImage;
		}

		public void DrawInfo(Graphics g, Rectangle rect) 
		{
			const int margin = 2;
			if (MouseCaptured)
			{
				// add text describing the effect
				using (Brush b = new SolidBrush(m_textColor))
				{
					g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

					string s = string.Format("{0} \r\n Start: {1} \r\n Length: {2}", 
						EffectNode.Effect.EffectName,
						StartTime.ToString(@"m\:ss\.fff"),
						Duration.ToString(@"m\:ss\.fff"));

					SizeF textSize = g.MeasureString(s, m_textFont);
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

					//Check to make sure we are on the screen. 
					if (g.VisibleClipBounds.X > destRect.X)
					{
						destRect.X = (int)g.VisibleClipBounds.X + 5;
					}

					// Full size info box. Comment out next two lines to clip
					destRect.Width = (int)textSize.Width + margin;
					destRect.Height = (int)textSize.Height + margin;
					
					g.FillRectangle(infoBrush, new Rectangle(destRect.Left, destRect.Top, (int)Math.Min(textSize.Width + margin, destRect.Width), (int)Math.Min(textSize.Height + margin, destRect.Height)));
					g.DrawString(s, m_textFont, b, new Rectangle(destRect.Left + margin/2, destRect.Top + margin/2, destRect.Width - margin, destRect.Height - margin));
				}
			}
		}

		public Bitmap Draw(Size imageSize, Graphics g, TimeSpan visibleStartTime, TimeSpan visibleEndTime, int overallWidth)
		{

			return IsRendered ? DrawImage(imageSize, visibleStartTime, visibleEndTime, overallWidth) : DrawPlaceholder(imageSize);
		
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