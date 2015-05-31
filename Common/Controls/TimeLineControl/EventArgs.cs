using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Common.Controls.Timeline
{
	public class ElementEventArgs : EventArgs
	{
		public ElementEventArgs(Element te)
		{
			Element = te;
		}

		public Element Element { get; private set; }
	}

	public class DrawElementEventArgs: EventArgs
	{
		public DrawElementEventArgs(Guid guid, List<Row> rows, TimeSpan mouseDownTime, TimeSpan mouseUpTime)	
		{
			Guid = guid;
			Rows = rows;
			MouseDownTime = mouseDownTime;
			MouseUpTime = mouseUpTime;
		}

		public Guid Guid { get; private set; }
		public List<Row> Rows { get; set; }
		private TimeSpan MouseDownTime { get; set; }
		private TimeSpan MouseUpTime { get; set; }
		public TimeSpan StartTime
		{
			get
			{
				return (MouseUpTime < MouseDownTime ? MouseUpTime : MouseDownTime);
			}
		}
		public TimeSpan Duration
		{
			get
			{
				return (MouseUpTime > MouseDownTime ? MouseUpTime - MouseDownTime : MouseDownTime - MouseUpTime);
			}
		}
	}


	public class MultiElementEventArgs : EventArgs
	{
		public IEnumerable<Element> Elements { get; set; }
	}


	public class RowHeightChangedEventArgs : EventArgs
	{
		public RowHeightChangedEventArgs(int heightChange)
		{
			HeightChange = heightChange;
		}

		public int HeightChange { get; private set; }
	}


	public class TimeSpanEventArgs : EventArgs
	{
		public TimeSpanEventArgs(TimeSpan t)
		{
			Time = t;
		}

		public TimeSpan Time { get; private set; }
	}


	public class ModifierKeysEventArgs : EventArgs
	{
		public ModifierKeysEventArgs(Keys k)
		{
			ModifierKeys = k;
		}

		public Keys ModifierKeys { get; private set; }
	}

	public class ElementRowChangeEventArgs : EventArgs
	{
		public ElementRowChangeEventArgs(Element element, Row oldRow, Row newRow)
		{
			Element = element;
			OldRow = oldRow;
			NewRow = newRow;
		}

		public Element Element { get; private set; }
		public Row OldRow { get; private set; }
		public Row NewRow { get; private set; }
	}


	public class TimelineEventArgs : EventArgs
	{
		public TimelineEventArgs(Row row, TimeSpan time)
		{
			Row = row;
			Time = time;
		}

		public Row Row { get; private set; }
		public TimeSpan Time { get; private set; }
	}

	public class ElementsChangedTimesEventArgs : EventArgs
	{
		public ElementsChangedTimesEventArgs(ElementMoveInfo info, ElementMoveType type)
		{
			PreviousTimes = info.OriginalElements;
			Type = type;
		}

		public Dictionary<Element, ElementTimeInfo> PreviousTimes { get; private set; }

		public IEnumerable<Element> Elements
		{
			get { return PreviousTimes.Keys; }
		}

		public ElementMoveType Type { get; private set; }
	}

	public class ElementsSelectedEventArgs : EventArgs
	{
		public ElementsSelectedEventArgs(IEnumerable<Element> elements)
		{
			ElementsUnderCursor = elements;
			AutomaticallyHandleSelection = true;
		}

		public IEnumerable<Element> ElementsUnderCursor { get; private set; }
		public bool AutomaticallyHandleSelection { get; set; }
	}

	public class ContextSelectedEventArgs : EventArgs
	{
		public ContextSelectedEventArgs(IEnumerable<Element> elements, TimeSpan gridTime, Row row)
		{
			ElementsUnderCursor = elements;
			GridTime = gridTime;
			Row = row;
			AutomaticallyHandleSelection = true;
		}

		public IEnumerable<Element> ElementsUnderCursor { get; private set; }
		public bool AutomaticallyHandleSelection { get; set; }
		public TimeSpan GridTime { get; set; }
		public Row Row { get; set; }
	}

	public class RenderElementEventArgs : EventArgs
	{
		public RenderElementEventArgs(int percent)
		{
			Percent = percent;
		}

		public int Percent { get; set; }
	}
}