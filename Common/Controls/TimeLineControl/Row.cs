using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Common.Controls.Timeline
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
			trl.ActiveIndicator = () => !IsEmpty;
			trl.ChildActiveIndicator = IsTreeActive;
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
			set
			{
				// cap the height to a minimum of 10 pixels
				m_height = Math.Max(value, 10);
				if (m_visible)
				{
					RowLabel.Height = m_height;
					_RowHeightChanged();
				}
			}
		}

		public int DisplayTop { get; set; }

		private object m_tag;

		public object Tag
		{
			get { return m_tag; }
			set
			{
				m_tag = value;
				_RowChanged();
			}
		}

		public string Name
		{
			get { return RowLabel.Name; }
			set
			{
				RowLabel.Name = value;
				_RowChanged();
			}
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

		public bool IsTreeActive()
		{
			return ChildRows.Any(x => !x.IsEmpty || x.IsTreeActive());
		}

		private RowLabel m_rowLabel;

		public RowLabel RowLabel
		{
			get { return m_rowLabel; }
			set
			{
				if (m_rowLabel != null)
				{
					m_rowLabel.ParentRow = null;
					m_rowLabel.TreeToggled -= TreeToggledHandler;
					m_rowLabel.HeightChanged -= HeightChangedHandler;
					m_rowLabel.LabelClicked -= LabelClickedHandler;
					m_rowLabel.HeightResized -= HeightResizedHandler;
					m_rowLabel.RowContextMenuSelect -= RowContextMenuSelectHandler;
				}

				m_rowLabel = value;
				m_rowLabel.ParentRow = this;
				m_rowLabel.TreeToggled += TreeToggledHandler;
				m_rowLabel.HeightChanged += HeightChangedHandler;
				m_rowLabel.LabelClicked += LabelClickedHandler;
				m_rowLabel.HeightResized += HeightResizedHandler;
				m_rowLabel.RowContextMenuSelect += RowContextMenuSelectHandler;

				_RowChanged();
			}
		}

		private static string CalculateMd5Hash(string input)
		{
			string hash;
			using (MD5 md5 = MD5.Create())
			{
				hash = BitConverter.ToString(
					md5.ComputeHash(Encoding.UTF8.GetBytes(input))
				).Replace("-", String.Empty);
			}

			return hash;
		}

		/// <summary>
		/// This is a hash id based on its position in the tree. It is based on the hash of it's name and all it's parents. 
		/// This should provide a pretty unique id that is fairly static for restoring row settings. This can change if the user 
		/// restructures the group, but then the settings are probably not valid anyway.
		/// </summary>
		/// <returns></returns>
		public string TreeId()
		{
			return CalculateMd5Hash(TreePathName());
		}

		public string TreePathName()
		{
			if (ParentRow == null)
			{
				return Name;
			}
			
			return Name + ParentRow.TreePathName();
		}

		public Row ParentRow { get; set; }

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
				RowLabel.Invalidate();
				_RowToggled();
				_RowChanged();
			}
		}

		public void ToggleTree(bool open = true)
		{
			TreeOpen = open;
			foreach (Row row in ChildRows)
			{
				row.ToggleTree(open);	
			}
			RowLabel.Invalidate();
			_RowToggled();
			_RowChanged();
		}

		private bool m_visible;

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
				if (m_visible && RowLabel.Height != Height)
				{
					RowLabel.Height = Height;
				}
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

				m_selected = value;
				_RowChanged();
			}
		}

		/// <summary>
		/// Identifies a row that has been chosen in the grid
		/// It is not the same as a selected row(s). There should only be one active row.
		/// </summary>
		public bool Active { get; set; }

		public int ElementCount
		{
			get { return m_elements.Count; }
		}

		internal void InvalidateRowLabel()
		{
			if (Visible)
			{
				RowLabel.Invalidate();
			}
			ParentRow?.InvalidateRowLabel();
		}

		#endregion

		#region Events

		public event EventHandler<ElementEventArgs> ElementAdded;
		public event EventHandler<ElementEventArgs> ElementRemoved;
		public static event EventHandler RowToggled;
		public static event EventHandler RowChanged;
		public static event EventHandler RowHeightChanged;
		public static event EventHandler RowHeightResized;
		public static event EventHandler RowLabelContextMenuSelect;
		public static event EventHandler<ModifierKeysEventArgs> RowSelectedChanged;

		private void _ElementAdded(Element te)
		{
			if (ElementAdded != null) ElementAdded(this, new ElementEventArgs(te));
		}

		private void _ElementRemoved(Element te)
		{
			if (ElementRemoved != null) ElementRemoved(this, new ElementEventArgs(te));
		}

		private void _RowToggled()
		{
			if (RowToggled != null) RowToggled(this, EventArgs.Empty);
		}

		private void _RowChanged()
		{
			if (RowChanged != null) RowChanged(this, EventArgs.Empty);
		}

		private void _RowHeightChanged()
		{
			if (RowHeightChanged != null) RowHeightChanged(this, EventArgs.Empty);
		}

		public void _RowHeightResized()
		{
			if (RowHeightResized != null) RowHeightResized(this, EventArgs.Empty);
		}

		private void _RowLabelContextMenuSelect()
		{
			if (RowLabelContextMenuSelect != null) RowLabelContextMenuSelect(this, EventArgs.Empty);
		}

		private void _RowSelectedChanged(Keys k)
		{
			if (RowSelectedChanged != null) RowSelectedChanged(this, new ModifierKeysEventArgs(k));
		}

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
			Height += e.HeightChange;
		}

		protected void HeightResizedHandler(object sender, EventArgs e)
		{
			_RowHeightResized();
		}

		protected void RowContextMenuSelectHandler(object sender, EventArgs e)
		{
			_RowLabelContextMenuSelect();
		}

		protected void LabelClickedHandler(object sender, ModifierKeysEventArgs e)
		{
			if (e.ModifierKeys.HasFlag(Keys.Control)) {
				Selected = !Selected;
			}
			else {
				Selected = true;
			}

			_RowSelectedChanged(e.ModifierKeys);
		}

		#endregion

		#region Methods

		public IEnumerable<Row> Descendants()
		{
			var nodes = new Stack<Row>(new[] { this });
			while (nodes.Any())
			{
				Row node = nodes.Pop();
				yield return node;
				foreach (var n in node.ChildRows) nodes.Push(n);
			}
		}

		private readonly List<List<Element>> _stack = new List<List<Element>>();
		/// <summary>
		/// Set the stacking indexes for overlapping elements in the specific time range.
		/// </summary>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		public void SetStackIndexes(TimeSpan startTime, TimeSpan endTime)
		{
			for (int i = 0; i < m_elements.Count; i++)
			{
				if (m_elements[i].EndTime < startTime) continue;
				if (m_elements[i].StartTime > endTime) break;
				List<Element> overlappingElements = GetOverlappingElements(m_elements[i]);
				if (overlappingElements.Count > 1)
				{
					DetermineElementStack(overlappingElements);
					int x = 0;
					foreach (var elementGroup in _stack)
					{
						foreach (var element in elementGroup)
						{
							element.StackIndex = x;
							element.StackCount = _stack.Count;
						}
						x++;
					}
				}
				else
				{
					m_elements[i].StackCount = 1;
					m_elements[i].StackIndex = 0;
				}
				i += overlappingElements.Count - overlappingElements.IndexOf(m_elements[i]) - 1;

			}
		}

		private void DetermineElementStack(List<Element> elements)
		{
			_stack.Clear();
			_stack.Add(new List<Element> { elements[0] });
			for (int i = 1; i < elements.Count; i++)
			{
				bool add = true;
				for (int x = 0; x < _stack.Count; x++)
				{
					if (elements[i].StartTime >= _stack[x].Last().EndTime)
					{
						_stack[x].Add(elements[i]);
						add = false;
						break;
					}
				}
				if (add) _stack.Add(new List<Element> { elements[i] });
			}
		}

		public List<Element> GetOverlappingElements(Element elementMaster)
		{
			List<Element> elements = new List<Element>();
			elements.Add(elementMaster); //add our reference
			int startingIndex = IndexOfElement(elementMaster);
			TimeSpan startTime = elementMaster.StartTime;
			TimeSpan endTime = elementMaster.EndTime;

			//we start here and look backward and forward until no more overlap
			//Look forward.
			for (int i = startingIndex + 1; i < ElementCount; i++)
			{
				Element element = GetElementAtIndex(i);
				if (element.StartTime < endTime)
				{
					elements.Add(element);
					endTime = element.EndTime > endTime ? element.EndTime : endTime;
				} else
				{
					break;
				}

			}

			//Look backward.
			for (int i = startingIndex - 1; i >= 0; i--)
			{
				Element element = GetElementAtIndex(i);
				if (element.EndTime > startTime)
				{
					elements.Insert(0, element);
					startTime = element.StartTime < startTime ? element.StartTime : startTime;
				}

			}

			return elements;
		}

		/// <summary>
		/// For adding elements in bulk. Sorting is delayed until all elements are added.
		/// </summary>
		/// <param name="elements"></param>
		public void AddBulkElements(List<Element> elements)
		{
			foreach (Element element in elements)
			{
				m_elements.Add(element);
				if (element.Selected)
					m_selectedElements.Add(element);
				element.Row = this;
				element.ContentChanged += ElementContentChangedHandler;
				element.TimeChanged += ElementMovedHandler;
				element.SelectedChanged += ElementSelectedHandler;
				_ElementAdded(element);
			}
			m_elements.Sort();
			_RowChanged();
		}

		public void AddElement(Element element)
		{
			m_elements.Add(element);
			if (element.Selected)
				m_selectedElements.Add(element);
			element.Row = this;
			element.ContentChanged += ElementContentChangedHandler;
			element.TimeChanged += ElementMovedHandler;
			element.SelectedChanged += ElementSelectedHandler;
			m_elements.Sort();
			_ElementAdded(element);
			InvalidateRowLabel();
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
			if (m_elements.Remove(element))
			{
				if (element.Selected)
					m_selectedElements.Remove(element);
				element.ContentChanged -= ElementContentChangedHandler;
				element.TimeChanged -= ElementMovedHandler;
				element.SelectedChanged -= ElementSelectedHandler;
				m_elements.Sort();
				_ElementRemoved(element);
				InvalidateRowLabel();
				_RowChanged();
			}
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

		public void SelectAllElements(bool deselect = false)
		{
			// select all the child elements if we're being selected, or vice versa
			foreach (Element te in Elements) {
				te.Selected = !deselect;
			}
		}

		public void DeselectAllElements()
		{
			SelectAllElements(true);
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