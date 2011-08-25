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

		public TimelineRow(TimelineRowLabel trl)
		{
			// actually, we don't want to listen for this event in every row; since the 
			// whole grid gets redrawn anyway, we can just listen and handle the event
			// at the grid level instead.
			//TimelineElement.ElementChanged += new ElementChangedHandler;

			RowLabel = trl;
			ChildRows = new List<TimelineRow>();
		}

		public TimelineRow()
			: this(new TimelineRowLabel())
		{
		}

		#region Properties

		private int m_height;
		public int Height
		{
			get { return m_height; }
			set { RowLabel.Height = m_height = value; _RowChanged(); }
		}

		private object m_tag;
		public object Tag
		{
			get { return m_tag; }
			set { m_tag = value; _RowChanged(); }
		}

		public string Name
		{
			get { return RowLabel.Name; }
			set { RowLabel.Name = value; _RowChanged(); }
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

		private TimelineRowLabel m_rowLabel;
		public TimelineRowLabel RowLabel
		{
			get { return m_rowLabel; }
			set
			{
				if (m_rowLabel != null) {
					m_rowLabel.ParentRow = null;
					m_rowLabel.TreeToggled -= TreeToggledHandler;
					m_rowLabel.HeightChanged -= HeightChangedHandler;
				}

				m_rowLabel = value;
				m_rowLabel.ParentRow = this;
				m_rowLabel.TreeToggled += TreeToggledHandler;
				m_rowLabel.HeightChanged += HeightChangedHandler;

				_RowChanged();
			}
		}

		private TimelineRow m_parentRow;
		public TimelineRow ParentRow
		{
			get { return m_parentRow; }
			set { m_parentRow = value; }
		}

		public int ParentDepth
		{
			get
			{
				if (ParentRow == null)
					return 0;
				else
					return ParentRow.ParentDepth + 1;
			}
		}

		public List<TimelineRow> ChildRows { get; set; }

		private bool m_treeOpen;
		public bool TreeOpen
		{
			get { return m_treeOpen; }
			set
			{
				if (m_treeOpen == value)
					return;

				// if we're opening a tree, show all our children, and vice versa.
				// the Visible property will take care of the rest.
				foreach (TimelineRow row in ChildRows)
					row.Visible = value;

				m_treeOpen = value;
				_RowChanged();
			}
		}

		public bool Visible
		{
			get { return RowLabel.Visible; }
			set
			{
				// if we're being told to show or not (ie. a tree is being closed
				// or opened), then show or hide all our children. However, only
				// show them if our tree is currently open as well.
				foreach (TimelineRow row in ChildRows)
				    row.Visible = value && TreeOpen;

				RowLabel.Visible = value;
				_RowChanged();
			}
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
			if (ContainsElement(sender as TimelineElement))
				_RowChanged();
		}

		protected void TreeToggledHandler(object sender, EventArgs e)
		{
			TreeOpen = !TreeOpen;
		}

		protected void HeightChangedHandler(object sender, RowHeightChangedEventArgs e)
		{
			Height = Height + e.HeightChange;
		}

		#endregion


		#region Methods

		public TimelineElement this[int index]
		{
			get { return m_elements[index]; }
		}

		public void AddElement(TimelineElement element)
		{
			m_elements.Add(element);

			_ElementAdded(element);
			_RowChanged();
		}

		public bool AddUniqueElement(TimelineElement element)
		{
			if (m_elements.Contains(element))
				return false;

			AddElement(element);
			return true;
		}

		public void RemoveElement(TimelineElement element)
		{
			m_elements.Remove(element);
			_ElementRemoved(element);
			_RowChanged();
		}

		public bool ContainsElement(TimelineElement element)
		{
			return m_elements.Contains(element);
		}

		public void ClearElements()
		{
			m_elements.Clear();
			_RowChanged();
		}

		public void AddChildRow(TimelineRow row)
		{
			ChildRows.Add(row);
			row.ParentRow = this;
			row.Visible = TreeOpen;
		}

		public void RemoveChildRow(TimelineRow row)
		{
			ChildRows.Remove(row);
			row.ParentRow = null;
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
