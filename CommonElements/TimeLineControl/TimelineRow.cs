using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CommonElements.Timeline
{
    /// <summary>
    /// Represents a row in a TimelineControl, which contains TimelineElements.
    /// </summary>
	public class TimelineRow : IEnumerable<TimelineElement>
	{
		// the elements contained in this row. Must be kept sorted; however, we can't use a SortedList
		// or similar, as the elements within the list may have their times updated by the grid, which
		// puts their order out.
		protected List<TimelineElement> m_elements = new List<TimelineElement>();

		// a list of the selected elements. This should always be a subset of the element list above,
		// and is kept just to save having to iterate through all elements when retrieving selected elements.
		protected List<TimelineElement> m_selectedElements = new List<TimelineElement>();

		public TimelineRow(TimelineRowLabel trl)
		{
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

		protected IEnumerable<TimelineElement> Elements
		{
			get { return m_elements; }
		}

		public IEnumerable<TimelineElement> SelectedElements
		{
			get { return m_selectedElements; }
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
					m_rowLabel.LabelClicked -= LabelClickedHandler;
				}

				m_rowLabel = value;
				m_rowLabel.ParentRow = this;
				m_rowLabel.TreeToggled += TreeToggledHandler;
				m_rowLabel.HeightChanged += HeightChangedHandler;
				m_rowLabel.LabelClicked += LabelClickedHandler;

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

		private bool m_selected;
		public bool Selected
		{
			get { return m_selected; }
			set
			{
				if (m_selected == value)
					return;

				// select all the child elements if we're being selected, or vice versa
				foreach (TimelineElement te in Elements) {
					te.Selected = value;
				}

				m_selected = value;
				_RowChanged();
			}
		}

		public int ElementCount { get { return m_elements.Count; } }

		#endregion


		#region Events

		public event EventHandler<ElementEventArgs> ElementAdded;
		public event EventHandler<ElementEventArgs> ElementRemoved;
		public static event EventHandler RowChanged;
		public static event EventHandler<ModifierKeysEventArgs> RowSelectedChanged;

		private void _ElementAdded(TimelineElement te) { if (ElementAdded != null) ElementAdded(this, new ElementEventArgs(te)); }
		private void _ElementRemoved(TimelineElement te) { if (ElementRemoved != null) ElementRemoved(this, new ElementEventArgs(te)); }
		private void _RowChanged() { if (RowChanged != null) RowChanged(this, EventArgs.Empty); }
		private void _RowSelectedChanged(Keys k) { if (RowSelectedChanged != null) RowSelectedChanged(this, new ModifierKeysEventArgs(k)); }

		#endregion


		#region Event Handlers

		protected void ElementContentChangedHandler(object sender, EventArgs e)
		{
			_RowChanged();
		}

		protected void ElementMovedHandler(object sender, EventArgs e)
		{
			m_elements.Sort();
			_RowChanged();
		}

		protected void ElementSelectedHandler(object sender, EventArgs e)
		{
			if ((sender as TimelineElement).Selected)
				m_selectedElements.Add(sender as TimelineElement);
			else
				m_selectedElements.Remove(sender as TimelineElement);

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

		protected void LabelClickedHandler(object sender, ModifierKeysEventArgs e)
		{
			if (e.ModifierKeys.HasFlag(Keys.Control)) {
				Selected = !Selected;
			} else {
				Selected = true;
			}

			_RowSelectedChanged(e.ModifierKeys);
		}

		#endregion


		#region Methods

		public void AddElement(TimelineElement element)
		{
			m_elements.Add(element);
			if (element.Selected)
				m_selectedElements.Add(element);
			element.ElementContentChanged += ElementContentChangedHandler;
			element.ElementMoved += ElementMovedHandler;
			element.ElementSelectedChanged += ElementSelectedHandler;
			m_elements.Sort();
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
			if (element.Selected)
				m_selectedElements.Remove(element);
			element.ElementContentChanged -= ElementContentChangedHandler;
			element.ElementMoved -= ElementMovedHandler;
			element.ElementSelectedChanged -= ElementSelectedHandler;
			m_elements.Sort();
			_ElementRemoved(element);
			_RowChanged();
		}

		public bool ContainsElement(TimelineElement element)
		{
			// TODO: improve this function. m_elements is now a sorted list,
			// but it doesn't know that it is, so will iterate through everything.
			// We can more intelligently search through it.
			return m_elements.Contains(element);
		}

		public int IndexOfElement(TimelineElement element)
		{
			return m_elements.IndexOf(element);
		}

		public TimelineElement GetElementAtIndex(int index)
		{
			if (index < 0 || index >= m_elements.Count)
				return null;

			return m_elements[index];
		}

		public void ClearRowElements()
		{
			foreach (TimelineElement element in m_elements.ToArray())
				RemoveElement(element);

			_RowChanged();
		}

		public void ClearAllElements()
		{
			foreach (TimelineRow child in ChildRows) {
				child.ClearAllElements();
			}
			ClearRowElements();
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
