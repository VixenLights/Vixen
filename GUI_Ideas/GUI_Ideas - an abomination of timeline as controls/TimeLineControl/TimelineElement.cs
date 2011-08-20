using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Timeline
{
	public class TimelineElement : UserControl
    {
        public TimelineElement()
        {
			BackColor = Color.White;
        }

        #region Properties

		private DragState m_dragState = DragState.Normal;		// the current dragging state
		private Point m_lastMouseLocation;						// the location of the mouse at last draw; used to update the dragging
		private Rectangle m_ignoreDragArea;						// the area in which move movements should be ignored, before we start dragging

		public TimeSpan TimeOffset { get; set; }
		public TimeSpan Duration { get; set; }

        public TimelineRow ParentRow
		{
			get
			{
				if (Parent is TimelineRow)
					return Parent as TimelineRow;
				else
					return null;
			}
		}

		public Single timeToPixels(TimeSpan t) { return ParentRow.timeToPixels(t); }
		public TimeSpan pixelsToTime(int px) { return ParentRow.pixelsToTime(px); }

        /// <summary>
        /// Returns true if the element is selected in the parent TimelineControl.
        /// </summary>
        public bool Selected
        {
            get
            {
				if (ParentRow != null)
					return ParentRow.IsChildSelected(this);
				else
					return false;
            }
			set
			{
				if (ParentRow != null)
					if (value)
						ParentRow.SelectChild(this);
					else
						ParentRow.DeselectChild(this);
			}
        }

        #endregion

		#region Events

		public static event EventHandler ElementDoubleClicked;
		public static event EventHandler ElementMoved;

		#endregion


		protected override void OnPaint(PaintEventArgs e)
		{
            // Fill
            Brush b = new SolidBrush(BackColor);
            e.Graphics.FillRectangle(b, e.Graphics.VisibleClipBounds);

            // Width - bold if selected
            int b_wd = Selected ? 3 : 1;

			// Adjust the rect such that the border is completely inside it.
			RectangleF b_rect = new RectangleF(
				e.Graphics.VisibleClipBounds.Left + (b_wd / 2),
				e.Graphics.VisibleClipBounds.Top + (b_wd / 2),
				e.Graphics.VisibleClipBounds.Width - b_wd,
				e.Graphics.VisibleClipBounds.Height - b_wd
				);
			
			// Draw it!
            Pen border = new Pen(Color.Black);
            border.Width = b_wd;
			//border.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
			//graphics.DrawRectangle(border, rect);
			e.Graphics.DrawRectangle(border, b_rect.Left, b_rect.Top, b_rect.Width, b_rect.Height);
		}






		private bool CtrlPressed { get { return Form.ModifierKeys.HasFlag(Keys.Control); } }

		private void beginDrag()
		{
			m_dragState = DragState.Dragging;
			this.Cursor = Cursors.Hand;






			//// calculate all the snap points (in pixels) for all selected elements
			//// for every visible drag point (and a width either side, so they can snap
			//// to non-visible points that are close)
			//m_snapPixels = new SortedDictionary<int, SnapDetails>();

			//foreach (KeyValuePair<TimeSpan, int> kvp in m_snapPoints) {
			//    if ((kvp.Key >= VisibleTimeStart - VisibleTimeSpan) &&
			//        (kvp.Key <= VisibleTimeEnd + VisibleTimeSpan)) {

			//        int snapTimePixelCentre = (int)timeToPixels(kvp.Key);
			//        int snapRange = kvp.Value;
			//        int snapLevel = kvp.Value;

			//        foreach (TimelineElement element in m_selectedElements) {
			//            int elementPixelStart = (int)timeToPixels(element.Offset);
			//            int elementPixelEnd = (int)timeToPixels(element.Offset + element.Duration);

			//            // iterate through all pixels for this particular snap point, for this element
			//            for (int offset = -snapRange; offset <= snapRange; offset++) {

			//                // calculate the relative pixel (to the mouse location) for this point
			//                int rp = location.X + snapTimePixelCentre + offset - elementPixelStart;

			//                bool addNewSnapDetail = false;

			//                // if it doesn't have a Snap entry for this item, make one
			//                if (!m_snapPixels.ContainsKey(rp)) {
			//                    addNewSnapDetail = true;
			//                } else {
			//                    // if it does, we have to figure out an intelligent way to combine them. If it's
			//                    // going to snap to the same pixel, then just add it to the list. Also update
			//                    // the priority if needed.
			//                    if (m_snapPixels[rp].DestinationPixel == rp - offset) {
			//                        m_snapPixels[rp].SnapElements[element] = kvp.Key;
			//                        m_snapPixels[rp].SnapLevel = Math.Max(m_snapPixels[rp].SnapLevel, snapLevel);
			//                    }
			//                        // if it's not going to snap to the same pixel as the existing one, then only
			//                        // update it if the new one's of a higher priority.
			//                    else {
			//                        if (m_snapPixels[rp].SnapLevel < snapLevel) {
			//                            addNewSnapDetail = true;
			//                        }
			//                    }
			//                }

			//                // add the new one if needed
			//                if (addNewSnapDetail) {
			//                    SnapDetails sd = new SnapDetails();
			//                    sd.DestinationPixel = rp - offset;
			//                    sd.SnapLevel = snapLevel;
			//                    sd.SnapElements = new Dictionary<TimelineElement, TimeSpan>();
			//                    sd.SnapElements[element] = kvp.Key;
			//                    m_snapPixels[rp] = sd;
			//                }


			//                // do the same for the end of the element
			//                addNewSnapDetail = false;
			//                rp = location.X + snapTimePixelCentre + offset - elementPixelEnd;




			//                // if it doesn't have a Snap entry for this item, make one
			//                if (!m_snapPixels.ContainsKey(rp)) {
			//                    addNewSnapDetail = true;
			//                } else {
			//                    // if it does, we have to figure out an intelligent way to combine them. If it's
			//                    // going to snap to the same pixel, then just add it to the list. Also update
			//                    // the priority if needed.
			//                    if (m_snapPixels[rp].DestinationPixel == rp - offset) {
			//                        m_snapPixels[rp].SnapElements[element] = kvp.Key - element.Duration;
			//                        m_snapPixels[rp].SnapLevel = Math.Max(m_snapPixels[rp].SnapLevel, snapLevel);
			//                    }
			//                        // if it's not going to snap to the same pixel as the existing one, then only
			//                        // update it if the new one's of a higher priority.
			//                    else {
			//                        if (m_snapPixels[rp].SnapLevel < snapLevel) {
			//                            addNewSnapDetail = true;
			//                        }
			//                    }
			//                }

			//                // add the new one if needed
			//                if (addNewSnapDetail) {
			//                    SnapDetails sd = new SnapDetails();
			//                    sd.DestinationPixel = rp - offset;
			//                    sd.SnapLevel = snapLevel;
			//                    sd.SnapElements = new Dictionary<TimelineElement, TimeSpan>();
			//                    sd.SnapElements[element] = kvp.Key - element.Duration;
			//                    m_snapPixels[rp] = sd;
			//                }
			//            }
			//        }
			//    }
			//}






		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			switch (e.Button) {
				case MouseButtons.Left:
					// select or deselect as needed
					if (Selected) {
						if (CtrlPressed)
							Selected = false;
					} else {
						if (!CtrlPressed)
							ParentRow.DeselectAllChildren();
						Selected = true;
					}

					// begin the dragging process -- calculate a area outside which a drag starts
					m_dragState = DragState.Waiting;
					Size drag = SystemInformation.DragSize;
					m_ignoreDragArea = new Rectangle(new Point(e.X - drag.Width / 2, e.Y - drag.Height / 2), drag);
					m_lastMouseLocation = e.Location;
					break;

				default:
					break;
			}
		}


		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (m_dragState == DragState.Normal)
				return;

			if (m_dragState == DragState.Waiting) {
				if (!m_ignoreDragArea.Contains(e.Location)) {
					//begin the dragging process
					beginDrag();
				}
					
			}
			if (m_dragState == DragState.Dragging) {
				int updatedX = e.X;

				//// if the cursor position is in a snap location, change the position
				//// we update elements to be, to the snapped position
				//if (m_snapPixels.ContainsKey(e.X)) {
				//    updatedX = m_snapPixels[e.X].DestinationPixel;
				//}

				int dX = updatedX - m_lastMouseLocation.X;
				m_lastMouseLocation.X = updatedX;

				if (dX != 0) {
					TimeOffset += pixelsToTime(dX);
					//foreach (TimelineElement element in SelectedElements) {
					//    if (m_snapPixels.ContainsKey(e.X) && m_snapPixels[e.X].SnapElements.ContainsKey(element)) {
					//        element.Offset = m_snapPixels[e.X].SnapElements[element];
					//    } else {
					//        element.Offset += pixelsToTime(dX);
					//    }
					//}
				}

				if (ElementMoved != null) {
					ElementMoved(this, EventArgs.Empty);
				}
			}
		}


		//protected override void OnMouseDoubleClick(MouseEventArgs e)
		//{
		//    _translateMouseArgs(ref e);

		//    TimelineElement elem = elementAt(e.Location);

		//    if (elem != null) {
		//        if (ElementDoubleClicked != null)
		//            ElementDoubleClicked(this, new ElementEventArgs() { Element = elem });
		//    } else {
		//        // Raise the base class event, b/c the control was clicked, not an element in it.
		//        base.OnMouseDoubleClick(e);
		//    }
		//}








    }

	enum DragState
	{
		/// <summary>
		/// Not dragging, mouse is up.
		/// </summary>
		Normal = 0,

		/// <summary>
		/// Mouse down, but hasn't moved past threshold yet to be considered dragging
		/// </summary>
		Waiting,

		/// <summary>
		/// Actively dragging objects
		/// </summary>
		Dragging,
	}

	public class ElementEventArgs : EventArgs
	{
		public ElementEventArgs(TimelineElement te)
		{
			Element = te;
		}

		public TimelineElement Element { get; internal set; }
	}

	public class MultiElementEventArgs : EventArgs
	{
		public List<TimelineElement> Elements { get; internal set; }
	}
}