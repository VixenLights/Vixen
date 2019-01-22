using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Common.Controls.Theme;
using Vixen.Sys;
using Vixen.Sys.LayerMixing;

namespace Common.Controls.Timeline
{
	[Serializable]
	public class Element : IComparable<Element>, ITimeRowLocation
	{
		private TimeSpan _startTime;
		private TimeSpan _duration;
		private ElementNode[] _targetNodes;
		private static readonly Color Gray = Color.FromArgb(122, 122, 122);
		private static readonly Color BorderColor = Color.Black;
		private bool _selected;
		private static readonly Font TextFont = new Font(SystemFonts.MessageBoxFont.FontFamily, 7);
		private static readonly Color TextColor = Color.FromArgb(255, 255, 255);
		private static readonly Brush InfoBrush = new SolidBrush(Color.FromArgb(128,0,0,0));
		protected internal bool SuspendEvents = false;
		private Bitmap _cachedImage;
		private Size _cachedImageSize= new Size(0,0);
		private TimeSpan _elementVisibleStartTime;
		private TimeSpan _elementVisibleEndTime;
		
		public Element()
		{
			
		}

		/// <summary>
		/// Copy constructor. Creates a shallow copy of other.
		/// </summary>
		/// <param name="other">The element to copy.</param>
		public Element(Element other)
		{
			_startTime = other._startTime;
			_duration = other._duration;
			_selected = other._selected;
			_targetNodes = other._targetNodes;
		}

		#region Begin/End update

		private TimeSpan _origStartTime, _origDuration;
		private ElementNode[] _origTargetNodes;

		///<summary>Suspends raising events until EndUpdate is called.</summary>
		public void BeginUpdate()
		{
			SuspendEvents = true;
			_origStartTime = StartTime;
			_origDuration = Duration;
			_origTargetNodes = _targetNodes;
		}

		public void EndUpdate()
		{
			SuspendEvents = false;
			if ((StartTime != _origStartTime) || (Duration != _origDuration)) {
				OnTimeChanged();
			}
			if (_origTargetNodes != _targetNodes)
			{
				EffectNode.Effect.TargetNodes = _targetNodes;
			}
		}

		/// <summary>
		/// Triggers the time changed event to occur in the middle of updating if the time has changed.
		/// For use in controlling When TimeChanged events occur while updating so they can be raised at optimal times.
		/// </summary>
		public void UpdateNotifyTimeChanged()
		{
			if (SuspendEvents && TimeChanged != null)
			{
				if ((StartTime != _origStartTime) || (Duration != _origDuration))
				{
					TimeChanged(this, EventArgs.Empty);
				}
			}
				
		}

		public void  UpdateNotifyContentChanged()
		{
			OnContentChanged();
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
		private EffectNode _effectNode;

		public EffectNode EffectNode
		{
			get { return _effectNode; }
			set { _effectNode = value; }
		}

		/// <summary>
		/// This is the last row that this element was associated with. This element can be part of more than one row if it is part of multiple groups
		/// So do not trust it. 
		/// </summary>
		public Row Row { get; set; }

		public ElementNode[] TargetNodes
		{
			protected get
			{
				return _targetNodes;
			}
			set
			{
				if (_targetNodes == value)
				{
					return;
				}
				_targetNodes = value;
				OnTargetNodesChanged();
			}
		}

		/// <summary>
		/// Gets or sets the starting time of this element (left side).
		/// </summary>
		public TimeSpan StartTime
		{
			get { return _startTime; }
			set
			{
				if (value < TimeSpan.Zero)
					value = TimeSpan.Zero;

				if (_startTime == value)
					return;

				_startTime = value;
				OnTimeChanged();
			}
		}

		/// <summary>
		/// Gets or sets the time duration of this element (width).
		/// </summary>
		public TimeSpan Duration
		{
			get { return _duration; }
			set
			{
				if (_duration == value)
					return;

				_duration = value;
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
			get { return _selected; }
			set
			{
				if (_selected == value)
					return;

				_selected = value;
				if (_cachedImage != null)
				{
					_cachedImage.Dispose();
					_cachedImage = null;
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
			if (!SuspendEvents && handler != null) 
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
			if (!SuspendEvents && TimeChanged != null)
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

		protected virtual void AddSelectionOverlayToCanvas(Graphics g, bool drawSelected, bool includeLeft, bool includeRight, bool redBorder)
		{
			// Width - bold if selected
			int borderWidth = drawSelected ? 3 : 1;

			// Adjust the rect such that the border is completely inside it.
			Rectangle borderRectangle = new Rectangle(
				(int) g.VisibleClipBounds.Left, (int)g.VisibleClipBounds.Top,
				(int) g.VisibleClipBounds.Width-1, (int) g.VisibleClipBounds.Height-1
				);

			// Draw it!
			using (Pen border = new Pen(redBorder ? Color.Red : BorderColor,borderWidth))
			{	
				DrawBorder(g, includeLeft, includeRight, border, borderRectangle);
				if (drawSelected)
				{
					//border.DashCap = DashCap.Flat;
					//border.DashStyle = DashStyle.Dot;
					border.Width = 1;
					border.DashPattern = new float[] { 1.0F, 2.0F};
					border.Color = ThemeColorTable.ForeColor;
					DrawBorder(g, includeLeft, includeRight, border, borderRectangle);
				}
			}
		}

		private static void DrawBorder(Graphics g, bool includeLeft, bool includeRight, Pen border, Rectangle borderRectangle)
		{
			g.DrawLine(border, borderRectangle.Left, borderRectangle.Top, borderRectangle.Right, borderRectangle.Top);
			g.DrawLine(border, borderRectangle.Left, borderRectangle.Bottom, borderRectangle.Right, borderRectangle.Bottom);

			if (includeRight)
			{
				g.DrawLine(border, borderRectangle.Right, borderRectangle.Top, borderRectangle.Right, borderRectangle.Bottom + 1);
			}
			if (includeLeft)
			{
				g.DrawLine(border, borderRectangle.Left, borderRectangle.Top, borderRectangle.Left, borderRectangle.Bottom);
			}
		}

		protected virtual void DrawCanvasContent(Graphics graphics, TimeSpan startTime, TimeSpan endTime, int overallWidth, bool redBorder)
		{
		}

		public void RenderElement()
		{
			if (!IsRendered)
			{
				EffectNode.Effect.PreRender();
				if (_cachedImage != null)
				{
					_cachedImage.Dispose();
					_cachedImage = null;
				}
				
			}
		}

		protected Bitmap DrawImage(Size imageSize, TimeSpan startTime, TimeSpan endTime, int overallWidth, bool redBorder)
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

			if (SuspendEvents)
			{
				double factor = _origDuration.TotalMilliseconds / Duration.TotalMilliseconds;
				visibleStartOffset = TimeSpan.FromMilliseconds(visibleStartOffset.TotalMilliseconds * factor);
				visibleEndOffset = TimeSpan.FromMilliseconds(visibleEndOffset.TotalMilliseconds * factor);
			}

			if (_cachedImage == null || visibleStartOffset != _elementVisibleStartTime || 
				visibleEndOffset != _elementVisibleEndTime || _cachedImageSize.Height != imageSize.Height || 
				_cachedImageSize.Width != imageSize.Width)
			{
				var image = new Bitmap(imageSize.Width, imageSize.Height);
				using (Graphics g = Graphics.FromImage(image))
				{
					DrawCanvasContent(g, visibleStartOffset, visibleEndOffset, overallWidth, redBorder);
					AddSelectionOverlayToCanvas(g, _selected, startTime <= StartTime, endTime >= EndTime, redBorder);
				}
				_elementVisibleStartTime = visibleStartOffset;
				_elementVisibleEndTime = visibleEndOffset;
				_cachedImageSize = image.Size;
				_cachedImage = image;
			}
			
			return _cachedImage;
		}

		public Bitmap DrawPlaceholder(Size imageSize, bool redBorder)
		{
			Bitmap result = new Bitmap(imageSize.Width, imageSize.Height);
			using (Graphics g = Graphics.FromImage(result))
			{
				using (Brush b = new SolidBrush(Gray))
				{
					g.FillRectangle(b,
							new Rectangle((int)g.VisibleClipBounds.Left, (int)g.VisibleClipBounds.Top,
										  (int)g.VisibleClipBounds.Width, (int)g.VisibleClipBounds.Height));	
				}

				AddSelectionOverlayToCanvas(g, _selected, true, true, redBorder);
			}
			
			return result;
		}

		public void DrawInfo(Graphics g, Rectangle rect, ILayer layer) 
		{
			const int margin = 2;
			if (MouseCaptured)
			{
				// add text describing the effect
				using (Brush b = new SolidBrush(TextColor))
				{
					g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

					string s = string.Format("{0} \r\n Layer: {4} \r\n Start: {1} \r\n End: {2} \r\n Length: {3}",
						EffectNode.Effect.EffectName,
						StartTime.ToString(@"m\:ss\.fff"),
						EndTime.ToString(@"m\:ss\.fff"),
						Duration.ToString(@"m\:ss\.fff"),
						layer.LayerName);
						
						

					SizeF textSize = g.MeasureString(s, TextFont);
					Rectangle destRect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);

					destRect.Width = (int)textSize.Width + margin;
					destRect.Height = (int)textSize.Height + margin;
					
					if (rect.Y - destRect.Height < g.VisibleClipBounds.Y)
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

					g.FillRectangle(InfoBrush, new Rectangle(destRect.Left + margin / 2, destRect.Top + margin / 2, destRect.Width + margin / 2, destRect.Height + margin / 2));
					g.DrawString(s, TextFont, b, new Rectangle(destRect.Left , destRect.Top , destRect.Width, destRect.Height));
				}
			}
		}

		public Bitmap Draw(Size imageSize, Graphics g, TimeSpan visibleStartTime, TimeSpan visibleEndTime, int overallWidth, bool redBorder)
		{

			return IsRendered ? DrawImage(imageSize, visibleStartTime, visibleEndTime, overallWidth, redBorder) : DrawPlaceholder(imageSize, redBorder);
		
		}

		#endregion

	}


	public class ElementTimeInfo : ITimeRowLocation
	{
		public ElementTimeInfo(Element elem)
		{
			StartTime = elem.StartTime;
			Duration = elem.Duration;
			Row = elem.Row;
		}

		public TimeSpan StartTime { get; set; }
		public TimeSpan Duration { get; set; }

		public TimeSpan EndTime
		{
			get { return StartTime + Duration; }
		}

		public Row Row { get; set; }

		public static void SwapPlaces(ITimeRowLocation lhs, ITimeRowLocation rhs)
		{
			TimeSpan temp = lhs.StartTime;
			lhs.StartTime = rhs.StartTime;
			rhs.StartTime = temp;

			temp = lhs.Duration;
			lhs.Duration = rhs.Duration;
			rhs.Duration = temp;

			Row tempRow = lhs.Row;
			lhs.Row = rhs.Row;
			rhs.Row = tempRow;
		}

	}

	public interface ITimeRowLocation
	{
		TimeSpan StartTime { get; set; }
		TimeSpan Duration { get; set; }
		Row Row { get; set; }
	}
}