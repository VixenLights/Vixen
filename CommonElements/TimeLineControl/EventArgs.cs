using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CommonElements.Timeline
{
	public class ElementEventArgs : EventArgs
	{
		public ElementEventArgs(Element te)
		{
			Element = te;
		}

		public Element Element { get; private set; }
	}

	public class RowEventArgs : EventArgs
	{
		public RowEventArgs(Row r)
		{
			Row = r;
		}

		public Row Row { get; private set; }
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

	public class TimelineDropEventArgs : TimelineEventArgs
	{
		public TimelineDropEventArgs(Row row, TimeSpan time, IDataObject data)
			: base(row, time)
		{
			Data = data;
		}

		public IDataObject Data { get; private set; }
	}



    public class ElementsChangedTimesEventArgs : EventArgs
    {
        public ElementsChangedTimesEventArgs(ElementMoveInfo info, ElementMoveType type)
        {
            PreviousTimes = info.OriginalElements;
            Type = type;
        }

        public Dictionary<Element, ElementTimeInfo> PreviousTimes { get; private set; }
        public IEnumerable<Element> Elements { get { return PreviousTimes.Keys; } }
        public ElementMoveType Type { get; private set; }
    }
}