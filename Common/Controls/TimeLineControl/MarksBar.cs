using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Common.Controls.Theme;
using Common.Controls.Timeline;
using Common.Controls.TimelineControl.LabeledMarks;
using Vixen.Sys.Marks;

namespace Common.Controls.TimelineControl
{
	public sealed class MarksBar:TimelineControlBase
	{

		private readonly List<MarkRow> _rows;
		private bool _suppressInvalidate;
		private Point _mouseDownLocation, _mouseUpLocation;
		private Mark _mouseDownMark;

		public event EventHandler<MarksMovedEventArgs> MarksMoved;
		public event EventHandler<MarksMovingEventArgs> MarksMoving;
		public event EventHandler<MarksDeletedEventArgs> DeleteMark;

		/// <inheritdoc />
		public MarksBar(TimeInfo timeinfo) : base(timeinfo)
		{
			BackColor = Color.Gray;
			_rows = new List<MarkRow>();
		}

		public void AddMarks(MarkCollection labeledMarkCollection)
		{
			MarkRow row = new MarkRow(labeledMarkCollection);
			_rows.Add(row);
			if (!_suppressInvalidate)
			{
				CalculateHeight();
				Invalidate();
			}
		}

		public void ClearMarks()
		{
			_rows.Clear();
			if (!_suppressInvalidate) Invalidate();
		}

		#region Overrides of Control

		#region Overrides of UserControl

		/// <inheritdoc />
		protected override void OnMouseDown(MouseEventArgs e)
		{
			_mouseDownMark = null;
			Point location = _mouseDownLocation = TranslateLocation(e.Location);
			
			if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
			{
				_mouseDownMark = MarkAt(location);
			}

		}

		#endregion

		/// <inheritdoc />
		protected override void OnMouseUp(MouseEventArgs e)
		{

			Point gridLocation = _mouseUpLocation = TranslateLocation(e.Location);

			if (e.Button == MouseButtons.Right)
			{
				if (_mouseDownLocation == _mouseUpLocation && _mouseDownMark != null)
				{
					ContextMenuStrip c = new ContextMenuStrip();
					c.Renderer = new ThemeToolStripRenderer();
					var delete = c.Items.Add("&Delete");
					delete.Click += DeleteMark_Click;
					var rename = c.Items.Add("&Rename");
					rename.Click += Rename_Click;
					c.Show(this, new Point(e.X, e.Y));
				}

			}

			
		}

		private void Rename_Click(object sender, EventArgs e)
		{
			TextDialog td = new TextDialog("Enter the new name.", "Rename Mark");
			var result = td.ShowDialog(this);
			if (result == DialogResult.OK)
			{
				_mouseDownMark.Text = td.Response;
				Invalidate();
			}
		}

		private void DeleteMark_Click(object sender, EventArgs e)
		{
			OnDeleteMark(new MarksDeletedEventArgs(new List<Mark>(new []{_mouseDownMark})));
		}

		#endregion

		/// <summary>
		/// Returns all elements located at the given point in client coordinates
		/// </summary>
		/// <param name="p">Client coordinates.</param>
		/// <returns>Elements at given point, or null if none exists.</returns>
		private Mark MarkAt(Point p)
		{
			
			// First figure out which row we are in
			MarkRow containingRow = RowAt(p);

			if (containingRow == null)
				return null;

			// Now figure out which element we are on
			foreach (Mark mark in containingRow)
			{
				Single x = timeToPixels(mark.StartTime);
				if (x > p.X) break; //The rest of them are beyond our point.
				Single width = timeToPixels(mark.Duration);
				MarkRow.MarkStack ms = containingRow.GetStackForMark(mark);
				var displayHeight = containingRow.Height / ms.StackCount;
				var rowTopOffset = displayHeight * ms.StackIndex;
				if (p.X >= x &&
				    p.X <= x + width &&
				    p.Y >= containingRow.DisplayTop + rowTopOffset &&
				    p.Y < containingRow.DisplayTop + rowTopOffset + displayHeight)
				{
					return mark;
				}
			}

			return null;
		}

		/// <summary>
		/// Returns the row located at the current point in client coordinates
		/// </summary>
		/// <param name="p">Client coordinates.</param>
		/// <returns>Row at given point, or null if none exists.</returns>
		private MarkRow RowAt(Point p)
		{
			MarkRow containingRow = null;
			int curheight = 0;
			foreach (MarkRow row in _rows)
			{
				if (p.Y < curheight + row.Height)
				{
					containingRow = row;
					break;
				}
				curheight += row.Height;
			}

			return containingRow;
		}

		private void CalculateRowDisplayTops(bool visibleRowsOnly = true)
		{
			int top = 0;
			var processRows = visibleRowsOnly ? _rows.Where(x => x.Visible) : _rows;
			foreach (var row in processRows)
			{
				row.DisplayTop = top;
				top += row.Height;
			}
		}

		/// <summary>
		/// Translates a location (Point) so that its coordinates represent the coordinates on the underlying timeline, taking into account scroll position.
		/// </summary>
		/// <param name="originalLocation"></param>
		public Point TranslateLocation(Point originalLocation)
		{
			// Translate this location based on the auto scroll position.
			Point p = originalLocation;
			var xOffset = (int) timeToPixels(VisibleTimeStart);
			p.Offset(xOffset, 0);
			return p;
		}

		protected override void OnVisibleTimeStartChanged(object sender, EventArgs e)
		{
			Invalidate();
		}

		private void CalculateHeight()
		{
			Height = _rows.Sum(x => x.Height);
		}

		public void BeginDraw()
		{
			_suppressInvalidate = true;
		}

		public void EndDraw()
		{
			_suppressInvalidate = false;
			CalculateHeight();
			Invalidate();
		}

		private void OnMarkMoved(MarksMovedEventArgs e)
		{
			MarksMoved?.Invoke(this, e);
		}

		private void OnMarksMoving(MarksMovingEventArgs e)
		{
			MarksMoving?.Invoke(this, e);
		}

		private void OnDeleteMark(MarksDeletedEventArgs e)
		{
			DeleteMark?.Invoke(this, e);
		}


		protected override void OnPaint(PaintEventArgs e)
		{
			try
			{
				// Translate the graphics to work the same way the timeline grid does
				// (ie. Drawing coordinates take into account where we start at in time)
				e.Graphics.TranslateTransform(-timeToPixels(VisibleTimeStart), 0);

				DrawRows(e.Graphics);
				DrawMarks(e.Graphics);

			}
			catch (Exception ex)
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Exception in Timeline.MarksBar.OnPaint():\n\n\t" + ex.Message + "\n\nBacktrace:\n\n\t" + ex.StackTrace,
					@"Error", false, false);
				messageBox.ShowDialog();
			}
		}

		private static readonly Color SelectionColor = Color.FromArgb(100, 40, 100, 160);
		

		private void DrawRows(Graphics g)
		{
			int curY = 0;
			var start = (int)timeToPixels(VisibleTimeStart);
			var end = (int)timeToPixels(VisibleTimeEnd);
			CalculateRowDisplayTops();
			// Draw row separators
			using (Pen p = new Pen(ThemeColorTable.TimeLineGridColor))
			{
				foreach (var row in _rows.Where(x => x.Visible))
				{
					curY += row.Height;
					Point lineLeft = new Point(start, curY);
					Point lineRight = new Point(end, curY);
					g.DrawLine(p, lineLeft.X, lineLeft.Y - 1, lineRight.X, lineRight.Y - 1);
				}
			}
		}

		private void DrawMarks(Graphics g)
		{
			int displaytop = 0;
			foreach (var row in _rows.Where(x => x.Visible))
			{
				row.SetStackIndexes(VisibleTimeStart, VisibleTimeEnd);
				for (int i = 0; i < row.MarksCount; i++)
				{
					Mark currentElement = row.GetMarkAtIndex(i);
					if (currentElement.EndTime < VisibleTimeStart)
						continue;

					if (currentElement.StartTime > VisibleTimeEnd)
					{
						break;
					}

					DrawElement(g, row, currentElement, displaytop);
				}

				displaytop += row.Height;
			}

		}

		private void DrawElement(Graphics g, MarkRow row, Mark mark, int top)
		{
			int width;
			
			//Sanity check - it is possible for .DisplayHeight to become zero if there are too many marks stacked.
			//We set the DisplayHeight to the row height for the mark, and change the border to red.	
			var markStack = row.GetStackForMark(mark);
			var displayHeight =
				(markStack.StackCount != 0) ? ((row.Height - 1) / markStack.StackCount) : row.Height - 1;

			var displayTop = top + displayHeight * markStack.StackIndex;
			
			if (displayHeight == 0)
			{
				displayHeight = row.Height;
			}

			if (mark.StartTime >= VisibleTimeStart)
			{
				if (mark.EndTime < VisibleTimeEnd)
				{
					width = (int)timeToPixels(mark.Duration);
				}
				else
				{
					width = (int)(timeToPixels(VisibleTimeEnd) - timeToPixels(mark.StartTime));
				}
			}
			else
			{
				if (mark.EndTime <= VisibleTimeEnd)
				{
					width = (int)timeToPixels(mark.Duration);// (int)(timeToPixels(mark.EndTime) - timeToPixels(VisibleTimeStart));
				}
				else
				{
					width = (int)(timeToPixels(VisibleTimeEnd) - timeToPixels(VisibleTimeStart));
				}
			}
			if (width <= 0) return;
			Size size = new Size(width, displayHeight);

			Bitmap labelImage = DrawMarkLabel(size, row.MarkDecorator.Color);
			if (labelImage == null) return;
			Point finalDrawLocation = new Point((int)Math.Floor(timeToPixels(mark.StartTime)), displayTop);

			Rectangle destRect = new Rectangle(finalDrawLocation.X, finalDrawLocation.Y, size.Width, displayHeight);
			g.DrawImage(labelImage, destRect);

			//Draw the text

			SolidBrush drawBrush = new SolidBrush(Color.Black);
			StringFormat drawFormat = new StringFormat();
			g.DrawString(mark.Text, SystemFonts.MessageBoxFont, drawBrush, destRect, drawFormat);
		}

		public Bitmap DrawMarkLabel(Size imageSize, Color c)
		{
			Bitmap result = new Bitmap(imageSize.Width, imageSize.Height);
			using (Graphics g = Graphics.FromImage(result))
			{
				using (Brush b = new SolidBrush(c))
				{
					g.FillRectangle(b,
						new Rectangle((int)g.VisibleClipBounds.Left, (int)g.VisibleClipBounds.Top,
							(int)g.VisibleClipBounds.Width, (int)g.VisibleClipBounds.Height));
				}

				
			}

			return result;
		}


	}
}
