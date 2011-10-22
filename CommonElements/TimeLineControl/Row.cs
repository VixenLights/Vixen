using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CommonElements.Timeline
{
    /// <summary>
    /// Represents a row in a TimelineControl, which contains TimelineElements.
    /// </summary>
	public class Row : IEnumerable<Element>
	{
		// the elements contained in this row. Must be kept sorted; however, we can't use a SortedList
		// or similar, as the elements within the list may have their times updated by the grid, which
		// puts their order out.
		protected List<Element> m_elements = new List<Element>();

		// a list of the selected elements. This should always be a subset of the element list above,
		// and is kept just to save having to iterate through all elements when retrieving selected elements.
		protected List<Element> m_selectedElements = new List<Element>();

		public Row(RowLabel trl)
		{
			RowLabel = trl;
			ChildRows = new List<Row>();
		}

		public Row()
			: this(new RowLabel())
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

		protected IEnumerable<Element> Elements
		{
			get { return m_elements; }
		}

		public IEnumerable<Element> SelectedElements
		{
			get { return m_selectedElements; }
		}

		public bool IsEmpty
		{
			get { return (m_elements.Count == 0); }
		}

		private RowLabel m_rowLabel;
		public RowLabel RowLabel
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

		private Row m_parentRow;
		public Row ParentRow
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

		public List<Row> ChildRows { get; set; }

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
				foreach (Row row in ChildRows)
					row.Visible = value;

				m_treeOpen = value;
				_RowChanged();
			}
		}

		private bool m_visible = true;
		public bool Visible
		{
			get { return m_visible; }
			set
			{
				// if we're being told to show or not (ie. a tree is being closed
				// or opened), then show or hide all our children. However, only
				// show them if our tree is currently open as well.
				foreach (Row row in ChildRows)
				    row.Visible = value && TreeOpen;

				RowLabel.Visible = value;
				m_visible = value;
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
				foreach (Element te in Elements) {
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
		public static event EventHandler RowToggled;
		public static event EventHandler RowChanged;
		public static event EventHandler RowHeightChanged;
		public static event EventHandler<ModifierKeysEventArgs> RowSelectedChanged;

		private void _ElementAdded(Element te) { if (ElementAdded != null) ElementAdded(this, new ElementEventArgs(te)); }
		private void _ElementRemoved(Element te) { if (ElementRemoved != null) ElementRemoved(this, new ElementEventArgs(te)); }
		private void _RowToggled() { if (RowToggled != null) RowToggled(this, EventArgs.Empty); }
		private void _RowChanged() { if (RowChanged != null) RowChanged(this, EventArgs.Empty); }
		private void _RowHeightChanged() { if (RowChanged != null) RowHeightChanged(this, EventArgs.Empty); }
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
			if ((sender as Element).Selected)
				m_selectedElements.Add(sender as Element);
			else
				m_selectedElements.Remove(sender as Element);

			_RowChanged();
		}

		protected void TreeToggledHandler(object sender, EventArgs e)
		{
			TreeOpen = !TreeOpen;
			_RowToggled();
		}

		protected void HeightChangedHandler(object sender, RowHeightChangedEventArgs e)
		{
			// cap the height to a minimum of 10 pixels.
			Height = Math.Max(Height + e.HeightChange, 10);
			_RowHeightChanged();
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

		public void AddElement(Element element)
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

		public bool AddUniqueElement(Element element)
		{
			if (m_elements.Contains(element))
				return false;

			AddElement(element);
			return true;
		}

		public void RemoveElement(Element element)
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

		public bool ContainsElement(Element element)
		{
			// TODO: improve this function. m_elements is now a sorted list,
			// but it doesn't know that it is, so will iterate through everything.
			// We can more intelligently search through it.
			return m_elements.Contains(element);
		}

		public int IndexOfElement(Element element)
		{
			return m_elements.IndexOf(element);
		}

		public Element GetElementAtIndex(int index)
		{
			if (index < 0 || index >= m_elements.Count)
				return null;

			return m_elements[index];
		}

		public void ClearRowElements()
		{
			foreach (Element element in m_elements.ToArray())
				RemoveElement(element);

			_RowChanged();
		}

		public void AddChildRow(Row row)
		{
			ChildRows.Add(row);
			row.ParentRow = this;
			row.Visible = TreeOpen;
		}

		public void RemoveChildRow(Row row)
		{
			ChildRows.Remove(row);
			row.ParentRow = null;
		}

		public IEnumerator<Element> GetEnumerator()
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
