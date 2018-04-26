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

		public event EventHandler<MarksMovedEventArgs> MarksMoved;
		public event EventHandler<MarksMovingEventArgs> MarksMoving;

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
			if (MarksMoved != null)
				MarksMoved(this, e);
		}

		private void OnMarksMoving(MarksMovingEventArgs e)
		{
			MarksMoving?.Invoke(this, e);
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
			// Draw row separators
			using (Pen p = new Pen(ThemeColorTable.TimeLineGridColor))
			{
				foreach (var row in _rows)
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
			foreach (var row in _rows)
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
