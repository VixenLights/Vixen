using System;
using System.Collections;
using System.Collections.Generic;

namespace Timeline
{
    /// <summary>
    /// Represents a row in a TimelineControl, which contains TimelineElements.
    /// </summary>
	public class TimelineRow : IEnumerable<TimelineElement>
	{
		protected List<TimelineElement> m_elements = new List<TimelineElement>();

		public TimelineRow()
		{
			// actually, we don't want to listen for this event in every row; since the 
			// whole grid gets redrawn anyway, we can just listen and handle the event
			// at the grid level instead.
			//TimelineElement.ElementChanged += new EventHandler(ElementChangedHandler);
		}

		#region Properties

		private int m_height;
		public int Height
		{
			get { return m_height; }
			set { m_height = value; _RowChanged(); }
		}

		private object m_tag;
		public object Tag
		{
			get { return m_tag; }
			set { m_tag = value; _RowChanged(); }
		}

		private string m_name;
		public string Name
		{
			get { return m_name; }
			set { m_name = value; _RowChanged(); }
		}

		public List<TimelineElement> Elements
		{
			get { return m_elements; }
		}

		public List<TimelineElement> SelectedElements
		{
			get
			{
				List<TimelineElement> result = new List<TimelineElement>();
				foreach (TimelineElement te in Elements) {
					if (te.Selected)
						result.Add(te);
				}
				return result;
			}
		}

		public bool IsEmpty
		{
			get { return (m_elements.Count == 0); }
		}

		#endregion


		#region Events

		internal event EventHandler<ElementEventArgs> ElementAdded;
		internal event EventHandler<ElementEventArgs> ElementRemoved;
		internal static event EventHandler RowChanged;

		private void _ElementAdded(TimelineElement te) { if (ElementAdded != null) ElementAdded(this, new ElementEventArgs(te)); }
		private void _ElementRemoved(TimelineElement te) { if (ElementRemoved != null) ElementRemoved(this, new ElementEventArgs(te)); }
		private void _RowChanged() { if (RowChanged != null) RowChanged(this, EventArgs.Empty); }

		#endregion


		#region Event Handlers

		protected void ElementChangedHandler(object sender, EventArgs e)
		{
			if (Contains(sender as TimelineElement))
				_RowChanged();
		}

		#endregion


		#region Methods

		public TimelineElement this[int index]
		{
			get { return m_elements[index]; }
		}

		public void Add(TimelineElement element)
		{
			m_elements.Add(element);

			_ElementAdded(element);
			_RowChanged();
		}

		public bool AddUnique(TimelineElement element)
		{
			if (m_elements.Contains(element))
				return false;

			Add(element);
			return true;
		}

		public void Remove(TimelineElement element)
		{
			m_elements.Remove(element);
			_ElementRemoved(element);
			_RowChanged();
		}

		public bool Contains(TimelineElement element)
		{
			return m_elements.Contains(element);
		}

		public void Clear()
		{
			m_elements.Clear();
			_RowChanged();
		}

		public IEnumerator<TimelineElement> GetEnumerator()
		{
			return m_elements.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_elements.GetEnumerator();
		}

		#endregion
	}
}
