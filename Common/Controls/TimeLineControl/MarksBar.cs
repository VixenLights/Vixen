using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Common.Controls.Theme;
using Common.Controls.Timeline;

namespace Common.Controls.TimelineControl
{
	public sealed class MarksBar:TimelineControlBase
	{

		private List<MarkRow> Rows;
		private bool _suppressInvalidate;

		/// <inheritdoc />
		public MarksBar(TimeInfo timeinfo) : base(timeinfo)
		{
			BackColor = Color.Gray;
			Rows = new List<MarkRow>();
		}

		public void AddMarks(LabeledMarkCollection labeledMarkCollection)
		{
			MarkRow row = new MarkRow(labeledMarkCollection);
			Rows.Add(row);
			if (!_suppressInvalidate)
			{
				CalculateHeight();
				Invalidate();
			}
		}

		public void ClearMarks()
		{
			Rows.Clear();
			if (!_suppressInvalidate) Invalidate();
		}

		protected override void OnVisibleTimeStartChanged(object sender, EventArgs e)
		{
			Invalidate();
		}

		private void CalculateHeight()
		{
			Height = Rows.Sum(x => x.Height);
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

			// Draw row separators
			using (Pen p = new Pen(ThemeColorTable.TimeLineGridColor))
			{
				foreach (var row in Rows)
				{
					//Point selectedTopLeft = new Point((-AutoScrollPosition.X), curY);
					curY += row.Height;
					Point lineLeft = new Point((-AutoScrollPosition.X), curY);
					Point lineRight = new Point((-AutoScrollPosition.X) + Width, curY);
					g.DrawLine(p, lineLeft.X, lineLeft.Y - 1, lineRight.X, lineRight.Y - 1);
				}
			}
		}

		private void DrawMarks(Graphics g)
		{
			int displaytop = 0;
			foreach (var row in Rows)
			{
				row.SetStackIndexes(VisibleTimeStart, VisibleTimeEnd);
				for (int i = 0; i < row.MarksCount; i++)
				{
					LabeledMark currentElement = row.GetMarkAtIndex(i);
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

			//Pen p;

			//// iterate through all snap points, and if it's visible, draw it
			//foreach (KeyValuePair<TimeSpan, List<SnapDetails>> kvp in StaticSnapPoints.ToArray())
			//{
			//	if (kvp.Key >= VisibleTimeEnd) break;

			//	if (kvp.Key >= VisibleTimeStart)
			//	{
			//		SnapDetails details = null;
			//		foreach (SnapDetails d in kvp.Value)
			//		{
			//			if (details == null || (d.SnapLevel > details.SnapLevel && d.SnapColor != Color.Empty))
			//				details = d;
			//		}

			//		int lineBold = 1;
			//		if (details.SnapBold)
			//			lineBold = 3;
			//		p = new Pen(details.SnapColor, lineBold);
			//		Single x = timeToPixels(kvp.Key);
			//		if (!details.SnapSolidLine)
			//			p.DashPattern = new float[] {details.SnapLevel, details.SnapLevel};
			//		if (selectedMarks.ContainsKey(kvp.Key))
			//		{
			//			p.Width = 3;
			//		}

			//		g.DrawLine(p, x, 0, x, Height);
			//		p.Dispose();



			//	}
			//}
		}

		private void DrawElement(Graphics g, MarkRow row, LabeledMark currentElement, int top)
		{
			int width;
			bool redBorder = false;

			//Sanity check - it is possible for .DisplayHeight to become zero if there are too many effects stacked.
			//We set the DisplayHeight to the row height for the currentElement, and change the border to red.		
			currentElement.DisplayHeight =
				(currentElement.StackCount != 0) ? ((row.Height - 1) / currentElement.StackCount) : row.Height - 1;

			currentElement.DisplayTop = top + (currentElement.DisplayHeight * currentElement.StackIndex);
			currentElement.RowTopOffset = currentElement.DisplayHeight * currentElement.StackIndex;

			if (currentElement.DisplayHeight == 0)
			{
				redBorder = true;
				currentElement.DisplayHeight = row.Height;
			}


			if (currentElement.StartTime >= VisibleTimeStart)
			{
				if (currentElement.EndTime < VisibleTimeEnd)
				{
					width = (int)timeToPixels(currentElement.Duration);
				}
				else
				{
					width = (int)(timeToPixels(VisibleTimeEnd) - timeToPixels(currentElement.StartTime));
				}
			}
			else
			{
				if (currentElement.EndTime <= VisibleTimeEnd)
				{
					width = (int)(timeToPixels(currentElement.EndTime) - timeToPixels(VisibleTimeStart));
				}
				else
				{
					width = (int)(timeToPixels(VisibleTimeEnd) - timeToPixels(VisibleTimeStart));
				}
			}
			if (width <= 0) return;
			Size size = new Size(width, currentElement.DisplayHeight);

			Bitmap elementImage = DrawPlaceholder(size, row.MarkColor);
			if (elementImage == null) return;
			Point finalDrawLocation = new Point((int)Math.Floor(timeToPixels(currentElement.StartTime > VisibleTimeStart ? currentElement.StartTime : VisibleTimeStart)), currentElement.DisplayTop);

			Rectangle destRect = new Rectangle(finalDrawLocation.X, finalDrawLocation.Y, size.Width, currentElement.DisplayHeight);
			currentElement.DisplayRect = destRect;
			g.DrawImage(elementImage, destRect);

		}

		public Bitmap DrawPlaceholder(Size imageSize, Color c)
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
