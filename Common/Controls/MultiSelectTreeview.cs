using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;
using Common.Controls.Theme;


namespace Common.Controls
{
	public class MultiSelectTreeview : TreeView
	{
		[DllImport("user32.dll")]
		private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, int lParam);

		#region events

		/// <summary>
		/// Occurs when an item is starting to be dragged. This
		/// event can be used to cancel dragging of particular items.
		/// </summary>
		public event DragStartEventHandler DragStart;

		/// <summary>
		/// Occurs when an item is dragged and dropped onto another.
		/// </summary>
		public event DragCompleteEventHandler DragComplete;

		/// <summary>
		/// Occurs when an item is dragged, and the drag is cancelled.
		/// </summary>
		public event DragItemEventHandler DragCancel;

		/// <summary>
		/// Occurs when one or more tree nodes is dragged over another tree node.
		/// It is designed to allow the ability to reject the drag target if it is
		/// inappropriate.
		/// </summary>
		public event DragVerifyEventHandler DragOverVerify;

		/// <summary>
		/// Occurs when the drag/drop is completing, and all source/targets have been
		/// verified. It provides feedback just before the movement of the nodes in
		/// the treeview, and contains a parameter to allow the handler to stop the
		/// dragging of nodes. If it stops the drag process, the handler must move all
		/// the nodes itself.
		/// </summary>
		public event DragFinishingEventHandler DragFinishing;

		/// <summary>
		/// occurs when the item(s) in the treeview are deselected, by clicking on an empty area.
		/// </summary>
		public event EventHandler Deselected;

		#endregion

		#region Private members

		private DragDropEffects _dragDefaultMode = DragDropEffects.Move;
		private Color _dragDestinationNodeForeColor = SystemColors.HighlightText;
		private Color _dragDestinationNodeBackColor = SystemColors.Highlight;
		private Color _dragSourceNodeForeColor = SystemColors.ControlText;
		private Color _dragSourceNodeBackColor = SystemColors.ControlLight;
		private bool _usingCustomDragCursor;
		private Cursor _customDragCursor = null;
		private TreeNode _dragDestinationNode;
		private DragBetweenNodes _dragBetweenState = DragBetweenNodes.DragOnTargetNode;
		private Point _dragBetweenRowsDrawLineStart;
		private Point _dragBetweenRowsDrawLineEnd;
		private int _dragLastLineDrawnY;
		// a bit hackey: tracks a 'state' marking if we should be sorting lists. Used for mass-selections, etc.,
		// to avoid sorting the list every time.
		private bool _delaySortingSelectedNodes = false;
		private bool _sortSelectedNodesWhenUpdateEnds = false;
		private bool _clickedNodeWasInBounds = false;
		private bool _selectedNodeWithControlKey = false;

		#endregion

		#region Drag & Drop properties

		/// <summary>
		/// The custom cursor to use when dragging an item, if UsingCustomDragCursor is set to true.
		/// </summary>
		[
			Description("The custom cursor to use when dragging an item, if UsingCustomDragCursor is set to true."),
			Category("Drag and drop")
		]
		public Cursor CustomDragCursor
		{
			get { return _customDragCursor; }
			set { _customDragCursor = value; }
		}

		/// <summary>
		/// If a custom cursor should be using while dragging.
		/// </summary>
		[
			Description("If a custom cursor should be using while dragging."),
			Category("Drag and drop")
		]
		public bool UsingCustomDragCursor
		{
			get { return _usingCustomDragCursor; }
			set { _usingCustomDragCursor = value; }
		}

		/// <summary>
		/// The background colour of the node being dragged over.
		/// </summary>
		[
			Description("The background colour of the node being dragged over."),
			Category("Drag and drop")
		]
		public Color DragDestinationNodeForeColor
		{
			get { return _dragDestinationNodeForeColor; }
			set { _dragDestinationNodeForeColor = value; }
		}

		/// <summary>
		/// The foreground colour of the node being dragged over.
		/// </summary>
		[
			Description("The foreground colour of the node being dragged over."),
			Category("Drag and drop")
		]
		public Color DragDestinationNodeBackColor
		{
			get { return _dragDestinationNodeBackColor; }
			set { _dragDestinationNodeBackColor = value; }
		}

		/// <summary>
		/// The background colour of the node(s) being dragged.
		/// </summary>
		[
			Description("The background colour of the node(s) being dragged."),
			Category("Drag and drop")
		]
		public Color DragSourceNodeForeColor
		{
			get { return _dragSourceNodeForeColor; }
			set { _dragSourceNodeForeColor = value; }
		}

		/// <summary>
		/// The foreground colour of the node(s) being dragged.
		/// </summary>
		[
			Description("The foreground colour of the node(s) being dragged."),
			Category("Drag and drop")
		]
		public Color DragSourceNodeBackColor
		{
			get { return _dragSourceNodeBackColor; }
			set { _dragSourceNodeBackColor = value; }
		}

		/// <summary>
		/// The drag mode (move,copy etc.)
		/// </summary>
		[
			Description("The drag mode (move,copy etc.)"),
			Category("Drag and drop")
		]
		public DragDropEffects DragDefaultMode
		{
			get { return _dragDefaultMode; }
			set { _dragDefaultMode = value; }
		}

		public bool DraggingBetweenRows
		{
			get
			{
				return _dragBetweenState == DragBetweenNodes.DragAboveTargetNode ||
				       _dragBetweenState == DragBetweenNodes.DragBelowTargetNode;
			}
		}

		#endregion

		#region Selected Node(s) Properties

		private List<TreeNode> m_SelectedNodes = null;

		public List<TreeNode> SelectedNodes
		{
			get { return m_SelectedNodes; }
			set
			{
				ClearSelectedNodes();
				if (value != null) {
					foreach (TreeNode node in value) {
						ToggleNode(node, true);
					}
				}
			}
		}

		// Note we use the new keyword to Hide the native treeview's SelectedNode property.
		private TreeNode m_SelectedNode;

		public new TreeNode SelectedNode
		{
			get { return m_SelectedNode; }
			set
			{
				ClearSelectedNodes();
				if (value != null) {
					SelectNode(value);
				}
			}
		}

		public void AddSelectedNode(TreeNode node)
		{
			//AddNodeToSelectedListIfNotInList(node);
			ToggleNode(node, true);
		}

		private void AddNodeToSelectedListIfNotInList(TreeNode node)
		{
			if (!m_SelectedNodes.Contains(node))
				m_SelectedNodes.Add(node);
			SortSelectedNodes();
		}

		private void SortSelectedNodes()
		{
			if (_delaySortingSelectedNodes) {
				_sortSelectedNodesWhenUpdateEnds = true;
			} else {
				m_SelectedNodes.Sort(new TreeNodeSorter(this));
			}
		}

		#endregion

		public bool CanReverseElements()
		{
			//CanReverseElements is true if two requirements are met, selected nodes must all have the same parent and must be a contiguous selection.
			TreeNode firstParent = null;
			int lastNodesIndex = -1;
			foreach (TreeNode node in m_SelectedNodes)
			{
				if (lastNodesIndex == -1)
				{
					firstParent = node.Parent;
					lastNodesIndex = node.Index;
				}
				else
				{
					if (node.Parent == firstParent)
					{
						if (lastNodesIndex != node.Index -1) 
						{
							return false;
						}
						else
						{
							lastNodesIndex = node.Index;							
						}
					}
					else
					{
						return false;
					}
				}
			}
			return true;
		}
		public new void BeginUpdate()
		{
			base.BeginUpdate();
			_delaySortingSelectedNodes = true;
		}

		public new void EndUpdate()
		{
			_delaySortingSelectedNodes = false;
			if (_sortSelectedNodesWhenUpdateEnds) {
				SortSelectedNodes();
			}
			base.EndUpdate();
		}


		public MultiSelectTreeview()
		{
			m_SelectedNodes = new List<TreeNode>();
			base.SelectedNode = null;

			base.SetStyle(ControlStyles.DoubleBuffer, true);
			//base.SetStyle(ControlStyles.UserPaint, true);
			//base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			AllowDrop = true;
			AutoSize = true;
			Dock = DockStyle.Fill;
		}

		#region Overridden Events

		protected override void OnMouseDown(MouseEventArgs e)
		{
			_clickedNodeWasInBounds = false;

			// If the user clicks on a node that was not previously selected, select it now.
			try {
				base.SelectedNode = null;

				TreeNode node = GetNodeAt(e.Location);
				if (node != null) {
					int leftBound = node.Bounds.X; // - 20; // Allow user to click on image
					int rightBound = node.Bounds.Right + 10; // Give a little extra room

					if (e.Location.X > leftBound && e.Location.X < rightBound) {
						// mark the clicked node as being 'in bounds', so that we know later (for drag, mouseup, etc.) that
						// the initial click was 'valid' (ie. wasn't subject to the dodgy treenode full-width issue)
						_clickedNodeWasInBounds = true;

						if ((ModifierKeys == Keys.None || ModifierKeys == Keys.Control) && (m_SelectedNodes.Contains(node))) {
							// Potential Drag Operation
							// Let Mouse Up do select
						}
						else {
							if (ModifierKeys == Keys.Control)
								_selectedNodeWithControlKey = true;
							SelectNode(node);
						}
					}
					// due to a weird issue in the TreeNode GetNodeAt() call, if we click off to the right of the nodes,
					// it still selects them. Detect that, and clear them if needed (only if no modifier keys are down).
					if ((e.Location.X < leftBound || e.Location.X > rightBound) && ModifierKeys == Keys.None) {
						ClearSelectedNodes();
						if (Deselected != null)
							Deselected(this, new EventArgs());
					}
				}
				else {
					ClearSelectedNodes();
					if (Deselected != null)
						Deselected(this, new EventArgs());
				}

				base.OnMouseDown(e);
			}
			catch (Exception ex) {
			    HandleException(ex);
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			// If the clicked on a node that WAS previously
			// selected then, reselect it now. This will clear
			// any other selected nodes. e.g. A B C D are selected
			// the user clicks on B, now A C & D are no longer selected.
			try {
				// Check to see if a node was clicked on 
				TreeNode node = GetNodeAt(e.Location);
				if (node != null) {
					if (ModifierKeys == Keys.None && !m_SelectedNodes.Contains(node) && e.Button != MouseButtons.Right &&
					    _clickedNodeWasInBounds)
						SelectNode(node);
					if (ModifierKeys == Keys.Control && !_selectedNodeWithControlKey)
						SelectNode(node);
				}

				base.OnMouseUp(e);
			} catch (Exception ex) {
				HandleException(ex);
			}
			_selectedNodeWithControlKey = false;
		}

		protected override void OnItemDrag(ItemDragEventArgs e)
		{
			// if the item wasn't even properly clicked on, don't start a drag
			if (!_clickedNodeWasInBounds)
				return;

			base.OnItemDrag(e);

			// Call dragstart event
			if (DragStart != null) {
				DragStartEventArgs ea = new DragStartEventArgs();
				ea.Nodes = SelectedNodes;
				DragStart(this, ea);

				if (ea.CancelDrag)
					return;
			}

			DrawSelectedNodesAsDragSource();

			// Start drag drop
			DoDragDrop(SelectedNodes, DragDropEffects.All);
		}

		protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
		{
			// Never allow base.SelectedNode to be set!
			try {
				base.SelectedNode = null;
				e.Cancel = true;

				base.OnBeforeSelect(e);
			} catch (Exception ex) {
				HandleException(ex);
			}
		}

		protected override void OnAfterSelect(TreeViewEventArgs e)
		{
			// Never allow base.SelectedNode to be set!
			try {
				base.OnAfterSelect(e);
				base.SelectedNode = null;
			} catch (Exception ex) {
				HandleException(ex);
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			// Handle all possible key strokes for the control.
			// including navigation, selection, etc.

			base.OnKeyDown(e);
			
			if (e.Control || e.Shift) return;

			//BeginUpdate();
			bool bShift = (ModifierKeys == Keys.Shift);

			try {
				// Nothing is selected in the tree, this isn't a good state
				// select the top node
				if (m_SelectedNode == null && TopNode != null) {
					ToggleNode(TopNode, true);
				}

				// Nothing is still selected in the tree, this isn't a good state, leave.
				if (m_SelectedNode == null) return;

				if (e.KeyCode == Keys.Left) {
					if (m_SelectedNode.IsExpanded && m_SelectedNode.Nodes.Count > 0) {
						// Collapse an expanded node that has children
						m_SelectedNode.Collapse();
					}
					else if (m_SelectedNode.Parent != null) {
						// Node is already collapsed, try to select its parent.
						SelectSingleNode(m_SelectedNode.Parent);
					}
				}
				else if (e.KeyCode == Keys.Right) {
					if (!m_SelectedNode.IsExpanded) {
						// Expand a collpased node's children
						m_SelectedNode.Expand();
					}
					else {
						// Node was already expanded, select the first child
						SelectSingleNode(m_SelectedNode.FirstNode);
					}
				}
				else if (e.KeyCode == Keys.Up) {
					// Select the previous node
					if (m_SelectedNode.PrevVisibleNode != null) {
						SelectNode(m_SelectedNode.PrevVisibleNode);
					}
				}
				else if (e.KeyCode == Keys.Down) {
					// Select the next node
					if (m_SelectedNode.NextVisibleNode != null) {
						SelectNode(m_SelectedNode.NextVisibleNode);
					}
				}
				else if (e.KeyCode == Keys.Home) {
					if (bShift) {
						// Select all of the root nodes up to this point 
						if (Nodes.Count > 0) {
							SelectNode(Nodes[0]);
						}
					}
					else {
						// Select this first node in the tree
						if (Nodes.Count > 0) {
							SelectSingleNode(Nodes[0]);
						}
					}
				}
				else if (e.KeyCode == Keys.End) {
					// Select the last node visible node in the tree.
					// Don't expand branches incase the tree is virtual
					TreeNode ndLast = Nodes[Nodes.Count - 1];
					while (ndLast.IsExpanded && (ndLast.LastNode != null)) {
						ndLast = ndLast.LastNode;
					}
					if (bShift) {
						if (Nodes.Count > 0) {
							SelectNode(ndLast);
						}
					}
					else {
						if (Nodes.Count > 0) {
							SelectSingleNode(ndLast);
						}
					}
				}
				else if (e.KeyCode == Keys.PageUp) {
					// Select the highest node in the display
					int nCount = VisibleCount;
					TreeNode ndCurrent = m_SelectedNode;
					while ((nCount) > 0 && (ndCurrent.PrevVisibleNode != null)) {
						ndCurrent = ndCurrent.PrevVisibleNode;
						nCount--;
					}
					SelectSingleNode(ndCurrent);
				}
				else if (e.KeyCode == Keys.PageDown) {
					// Select the lowest node in the display
					int nCount = VisibleCount;
					TreeNode ndCurrent = m_SelectedNode;
					while ((nCount) > 0 && (ndCurrent.NextVisibleNode != null)) {
						ndCurrent = ndCurrent.NextVisibleNode;
						nCount--;
					}
					SelectSingleNode(ndCurrent);
				}
				else {
					// Assume this is a search character a-z, A-Z, 0-9, etc.
					// Select the first node after the current node that 
					// starts with this character
					string sSearch = ((char) e.KeyValue).ToString();

					TreeNode ndCurrent = m_SelectedNode;
					while ((ndCurrent.NextVisibleNode != null)) {
						ndCurrent = ndCurrent.NextVisibleNode;
						if (ndCurrent.Text.StartsWith(sSearch)) {
							SelectSingleNode(ndCurrent);
							break;
						}
					}
				}
			} catch (Exception ex) {
				HandleException(ex);
			}
			finally {
				EndUpdate();
			}
		}

		protected override void WndProc(ref Message m)
		{
			// Stop erase background message
			if (m.Msg == (int) 0x0014) {
				m.Msg = (int) 0x0000; // Set to null
			}

			switch (m.Msg) {
				case 0x0F: // WM_PAINT
					CustomPaint(ref m);
					break;

				default:
					base.WndProc(ref m);
					break;
			}
		}

		protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
		{
			if (e.Effect == _dragDefaultMode) {
				if (UsingCustomDragCursor && CustomDragCursor != null) {
					e.UseDefaultCursors = false;
					Cursor = CustomDragCursor;
				}
			}
			else {
				e.UseDefaultCursors = true;
				Cursor = Cursors.Default;
			}
		}

		protected override void OnDragOver(DragEventArgs e)
		{
			// Change any previous node back
			if (_dragDestinationNode != null) {
				DrawNodeAsNormal(_dragDestinationNode);
			}

			// Get the node from the mouse position, colour it
			Point pt = PointToClient(new Point(e.X, e.Y));

			_dragDestinationNode = GetNodeAt(pt);

			if (_dragDestinationNode != null) {
				// try and figure out if we would be dragging 'between' nodes.
				if (_dragDestinationNode.Bounds.Contains(pt) && pt.Y - _dragDestinationNode.Bounds.Top <= 3) {
					_dragBetweenState = DragBetweenNodes.DragAboveTargetNode;
				}
				else if (_dragDestinationNode.Bounds.Contains(pt) && _dragDestinationNode.Bounds.Bottom - pt.Y <= 3) {
					_dragBetweenState = DragBetweenNodes.DragBelowTargetNode;
				}
				else {
					_dragBetweenState = DragBetweenNodes.DragOnTargetNode;
				}

				// figure out where to draw the dotted line to show where it would be moving to.
				if (DraggingBetweenRows) {
					if (_dragBetweenState == DragBetweenNodes.DragAboveTargetNode) {
						_dragBetweenRowsDrawLineStart = new Point(_dragDestinationNode.Bounds.Left, _dragDestinationNode.Bounds.Top);
						_dragBetweenRowsDrawLineEnd = new Point(_dragDestinationNode.Bounds.Right + 10, _dragDestinationNode.Bounds.Top);
					}
					else if (_dragBetweenState == DragBetweenNodes.DragBelowTargetNode) {
						_dragBetweenRowsDrawLineStart = new Point(_dragDestinationNode.Bounds.Left, _dragDestinationNode.Bounds.Bottom);
						_dragBetweenRowsDrawLineEnd = new Point(_dragDestinationNode.Bounds.Right + 10, _dragDestinationNode.Bounds.Bottom);
					}
					else {
						_dragBetweenRowsDrawLineStart = new Point(-1, -1);
						_dragBetweenRowsDrawLineEnd = new Point(-1, -1);
					}

					if (_dragLastLineDrawnY != _dragBetweenRowsDrawLineStart.Y) {
						Invalidate();
						_dragLastLineDrawnY = _dragBetweenRowsDrawLineStart.Y;
					}
				}
			}
			else {
			}

			// get the nodes that are being dragged from the drag data
			List<TreeNode> dragNodes = null;
			if (e.Data.GetDataPresent(typeof (List<TreeNode>))) {
				dragNodes = (List<TreeNode>) e.Data.GetData(typeof (List<TreeNode>));
			}

            if (dragNodes != null)
            {
                // if the target node is in the dragged nodes, it's not a valid point: don't select it
                if (_dragDestinationNode != null && dragNodes.Contains(_dragDestinationNode))
                {
                    _dragDestinationNode = null;
                }
            }

			if (dragNodes != null && _dragDestinationNode != null) {
				// if there's been a verification call setup, call it to check that the
				// target node is OK. otherwise, assume it is.
				if (DragOverVerify != null) {
					DragVerifyEventArgs ea = new DragVerifyEventArgs();
					ea.SourceNodes = dragNodes;
					ea.TargetNode = _dragDestinationNode;
					ea.DragBetweenNodes = _dragBetweenState;
					ea.KeyState = e.KeyState;
					ea.DragMode = _dragDefaultMode;
					DragOverVerify(this, ea);

					if (ea.ValidDragTarget) {
						e.Effect = ea.DragMode;
					}
					else {
						e.Effect = DragDropEffects.None;
						_dragDestinationNode = null;
					}
				}
				else {
					e.Effect = _dragDefaultMode;
				}

				if (_dragDestinationNode != null && !DraggingBetweenRows) {
					DrawNodeAsDragDestination(_dragDestinationNode);
				}
			}
			else {
				e.Effect = DragDropEffects.None;
			}

			// Scrolling down/up
			if (pt.Y + 10 > ClientSize.Height)
				SendMessage(Handle, 277, (IntPtr) 1, 0);
			else if (pt.Y < Top + 10)
				SendMessage(Handle, 277, (IntPtr) 0, 0);
		}

		protected override void OnDragLeave(EventArgs e)
		{
			CleanupDragVisuals();

			// Call cancel event
			if (DragCancel != null) {
				DragSourceEventArgs ea = new DragSourceEventArgs();
				ea.Nodes = SelectedNodes;
				DragCancel(this, ea);
			}
		}

		protected override void OnDragEnter(DragEventArgs e)
		{
			e.Effect = _dragDefaultMode;
			DrawSelectedNodesAsDragSource();
		}

		protected override void OnDragDrop(DragEventArgs e)
		{
			// Check it's a list of nodes being dragged

			if (e.Data.GetDataPresent(typeof (List<TreeNode>))) {
				List<TreeNode> dragNodes = (List<TreeNode>) e.Data.GetData(typeof (List<TreeNode>));

				// if there was no target, don't do anything
				if (_dragDestinationNode == null) {
					CleanupDragVisuals();
					return;
				}

				// if we're dragging onto one of the selected nodes, then don't do anything
				if (dragNodes.Contains(_dragDestinationNode)) {
					CleanupDragVisuals();
					return;
				}

				// if we're dragging onto one of our children, then don't do anything
				foreach (TreeNode node in dragNodes) {
					// there seems to be a weird bug where we can get multiple nodes as drag data; sometimes
					// 2 copies of the same node, except that one is part of the treeview, and one is not!
					// (I suspect it is if the client treeview may completely repopulate itself while dragging).
					// So, make sure the dragged nodes are part of this treeview first.
					if (node.TreeView != this)
						continue;

					if (_dragDestinationNode.FullPath.StartsWith(node.FullPath)) {
						CleanupDragVisuals();
						return;
					}
				}

				// before we actually do the dragging of nodes, raise the 'finishing' event. If the
				// handler wants us to stop, then don't complete the drag.
				if (DragFinishing != null) {
					DragFinishingEventArgs ea = new DragFinishingEventArgs();
					ea.SourceNodes = dragNodes;
					ea.TargetNode = _dragDestinationNode;
					ea.DragBetweenNodes = _dragBetweenState;
					ea.DragMode = e.Effect;
					DragFinishing(this, ea);
					if (!ea.FinishDrag) {
						CleanupDragVisuals();
						return;
					}
				}

				if (e.Effect == DragDropEffects.Move) {
					foreach (TreeNode node in dragNodes) {
						node.Remove();
					}
				}

				// this is pretty freakin' horrible. Needs to be refactored.
				int i;
				TreeNodeCollection target;
				switch (_dragBetweenState) {
					case DragBetweenNodes.DragAboveTargetNode:
						if (_dragDestinationNode.Parent == null)
							target = Nodes;
						else
							target = _dragDestinationNode.Parent.Nodes;

						i = _dragDestinationNode.Index;
						foreach (TreeNode node in dragNodes)
							target.Insert(i++, node);
						break;

					case DragBetweenNodes.DragBelowTargetNode:
						// if it's expanded, drop the items into the start of the node's nodes.
						if (_dragDestinationNode.IsExpanded) {
							i = 0;
							foreach (TreeNode node in dragNodes) {
								_dragDestinationNode.Nodes.Insert(i++, node);
							}
							_dragDestinationNode.Expand();
						}
						else {
							if (_dragDestinationNode.Parent == null)
								target = Nodes;
							else
								target = _dragDestinationNode.Parent.Nodes;

							i = _dragDestinationNode.Index + 1;
							foreach (TreeNode node in dragNodes)
								target.Insert(i++, node);
						}
						break;

					case DragBetweenNodes.DragOnTargetNode:
						_dragDestinationNode.Nodes.AddRange(dragNodes.ToArray());
						_dragDestinationNode.Expand();
						break;
				}

				// Call drag complete event
				if (DragComplete != null) {
					DragSourceDestinationEventArgs ea = new DragSourceDestinationEventArgs();
					ea.SourceNodes = dragNodes;
					ea.TargetNode = _dragDestinationNode;
					DragComplete(this, ea);
				}
			}

			CleanupDragVisuals();
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape) {
				DrawSelectedNodesAsNormal();

				if (_dragDestinationNode != null) {
					DrawNodeAsNormal(_dragDestinationNode);
				}

				Cursor = Cursors.Default;

				// Call cancel event
				if (DragCancel != null) {
					DragSourceEventArgs ea = new DragSourceEventArgs();
					ea.Nodes = SelectedNodes;

					DragCancel(this, ea);
				}
			}
		}

		protected void CustomPaint(ref Message m)
		{
			base.WndProc(ref m);
			using (Graphics g = this.CreateGraphics()) {
				if (DraggingBetweenRows) {
					Color c = Color.FromArgb((int) (0.5*byte.MaxValue), Color.Black);
					using (Pen p = new Pen(c, 2)) {
						p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
						if (_dragBetweenRowsDrawLineStart.Y >= 0 && _dragBetweenRowsDrawLineEnd.Y >= 0)
							g.DrawLine(p, _dragBetweenRowsDrawLineStart, _dragBetweenRowsDrawLineEnd);
					}
				}
			}
		}

		#endregion

		#region Helper Methods

		private void CleanupDragVisuals()
		{
			if (_dragDestinationNode != null)
				DrawNodeAsNormal(_dragDestinationNode);
			DrawSelectedNodesAsNormal();
			Cursor = Cursors.Default;

			_dragDestinationNode = null;
			_dragBetweenState = DragBetweenNodes.DragOnTargetNode;
			_dragBetweenRowsDrawLineStart = new Point(0, 0);
			_dragBetweenRowsDrawLineEnd = new Point(0, 0);
		}

		private void DrawNodeAsDragSource(TreeNode node)
		{
			node.ForeColor = DragSourceNodeForeColor;
			node.BackColor = DragSourceNodeBackColor;
		}

		private void DrawNodeAsDragDestination(TreeNode node)
		{
			node.ForeColor = DragDestinationNodeForeColor;
			node.BackColor = DragDestinationNodeBackColor;
		}

		private void DrawNodeAsNormal(TreeNode node)
		{
			if (SelectedNodes.Contains(node)) {
				node.ForeColor = SystemColors.HighlightText;
				node.BackColor = SystemColors.Highlight;
			}
			else {
				node.BackColor = ThemeColorTable.ListBoxBackColor;
				node.ForeColor = ThemeColorTable.ForeColor;
			}
		}

		private void DrawSelectedNodesAsDragSource()
		{
			foreach (TreeNode node in SelectedNodes)
				DrawNodeAsDragSource(node);
		}

		private void DrawSelectedNodesAsNormal()
		{
			foreach (TreeNode node in SelectedNodes)
				DrawNodeAsNormal(node);
		}


		private void SelectNode(TreeNode node)
		{
			try {
				BeginUpdate();

				if (m_SelectedNode == null || ModifierKeys == Keys.Control) {
					// Ctrl+Click selects an unselected node, or unselects a selected node.
					bool bIsSelected = m_SelectedNodes.Contains(node);
					ToggleNode(node, !bIsSelected);
				}
				else if (ModifierKeys == Keys.Shift) {
					// Shift+Click selects nodes between the selected node and here.
					TreeNode ndStart = m_SelectedNode;
					TreeNode ndEnd = node;

					if (ndStart.Parent == ndEnd.Parent) {
						// Selected node and clicked node have same parent, easy case.
						if (ndStart.Index < ndEnd.Index) {
							// If the selected node is beneath the clicked node walk down
							// selecting each Visible node until we reach the end.
							while (ndStart != ndEnd) {
								ndStart = ndStart.NextVisibleNode;
								if (ndStart == null) break;
								ToggleNode(ndStart, true);
							}
						}
						else if (ndStart.Index == ndEnd.Index) {
							// Clicked same node, do nothing
						}
						else {
							// If the selected node is above the clicked node walk up
							// selecting each Visible node until we reach the end.
							while (ndStart != ndEnd) {
								ndStart = ndStart.PrevVisibleNode;
								if (ndStart == null) break;
								ToggleNode(ndStart, true);
							}
						}
					}
					else {
						// Selected node and clicked node have same parent, hard case.
						// We need to find a common parent to determine if we need
						// to walk down selecting, or walk up selecting.

						TreeNode ndStartP = ndStart;
						TreeNode ndEndP = ndEnd;
						int startDepth = Math.Min(ndStartP.Level, ndEndP.Level);

						// Bring lower node up to common depth
						while (ndStartP.Level > startDepth) {
							ndStartP = ndStartP.Parent;
						}

						// Bring lower node up to common depth
						while (ndEndP.Level > startDepth) {
							ndEndP = ndEndP.Parent;
						}

						// Walk up the tree until we find the common parent
						while (ndStartP.Parent != ndEndP.Parent) {
							ndStartP = ndStartP.Parent;
							ndEndP = ndEndP.Parent;
						}

						// Select the node
						if (ndStartP.Index < ndEndP.Index) {
							// If the selected node is beneath the clicked node walk down
							// selecting each Visible node until we reach the end.
							while (ndStart != ndEnd) {
								ndStart = ndStart.NextVisibleNode;
								if (ndStart == null) break;
								ToggleNode(ndStart, true);
							}
						}
						else if (ndStartP.Index == ndEndP.Index) {
							if (ndStart.Level < ndEnd.Level) {
								while (ndStart != ndEnd) {
									ndStart = ndStart.NextVisibleNode;
									if (ndStart == null) break;
									ToggleNode(ndStart, true);
								}
							}
							else {
								while (ndStart != ndEnd) {
									ndStart = ndStart.PrevVisibleNode;
									if (ndStart == null) break;
									ToggleNode(ndStart, true);
								}
							}
						}
						else {
							// If the selected node is above the clicked node walk up
							// selecting each Visible node until we reach the end.
							while (ndStart != ndEnd) {
								ndStart = ndStart.PrevVisibleNode;
								if (ndStart == null) break;
								ToggleNode(ndStart, true);
							}
						}
					}

					SortSelectedNodes();
				}
				else {
					// Just clicked a node, select it
					SelectSingleNode(node, false);
				}

				OnAfterSelect(new TreeViewEventArgs(m_SelectedNode));
			}
			finally {
				EndUpdate();
			}
		}

		public void ClearSelectedNodes()
		{
			try {
				foreach (TreeNode node in m_SelectedNodes) {
					node.BackColor = this.BackColor;
					node.ForeColor = this.ForeColor;
				}
			}
			finally {
				m_SelectedNodes.Clear();
				m_SelectedNode = null;
			}
		}

		private void SelectSingleNode(TreeNode node, bool notify=true)
		{
			BeginUpdate();
			ClearSelectedNodes();

			if (node != null) {
				ToggleNode(node, true);
				node.EnsureVisible();
			}

			EndUpdate();

			if (notify)
			{
				OnAfterSelect(new TreeViewEventArgs(m_SelectedNode));
			}
		}

		private void ToggleNode(TreeNode node, bool bSelectNode)
		{
			if (bSelectNode) {
				m_SelectedNode = node;
				AddNodeToSelectedListIfNotInList(node);
				node.BackColor = SystemColors.Highlight;
				node.ForeColor = SystemColors.HighlightText;
			}
			else {
				m_SelectedNodes.Remove(node);
				node.BackColor = this.BackColor;
				node.ForeColor = this.ForeColor;
			}
		}

		private void HandleException(Exception ex)
		{
			// Perform some error handling here.
			// We don't want to bubble errors to the CLR. 
			//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
			MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
			var messageBox = new MessageBoxForm(ex.Message,
				"Error", false, false);
			messageBox.ShowDialog();
		}

		#endregion
	}

	public enum DragBetweenNodes
	{
		DragAboveTargetNode,
		DragBelowTargetNode,
		DragOnTargetNode
	}

	#region Event classes/delegates

	public delegate void DragCompleteEventHandler(object sender, DragSourceDestinationEventArgs e);

	public delegate void DragItemEventHandler(object sender, DragSourceEventArgs e);

	public delegate void DragStartEventHandler(object sender, DragStartEventArgs e);

	public delegate void DragVerifyEventHandler(object sender, DragVerifyEventArgs e);

	public delegate void DragFinishingEventHandler(object sender, DragFinishingEventArgs e);

	public class DragFinishingEventArgs : DragSourceDestinationEventArgs
	{
		/// <summary>
		/// settable by the event handler: If true, the drag/drop functionality
		/// should finish the drag process, moving the nodes in the tree view.
		/// If false, it will stop and the handler must finish it when desired.
		/// </summary>
		public bool FinishDrag
		{
			get { return _finishDrag; }
			set { _finishDrag = value; }
		}

		public DragBetweenNodes DragBetweenNodes
		{
			get { return _dragBetweenNodes; }
			set { _dragBetweenNodes = value; }
		}

		public DragDropEffects DragMode
		{
			get { return _dragMode; }
			set { _dragMode = value; }
		}

		private bool _finishDrag = true;
		private DragBetweenNodes _dragBetweenNodes;
		private DragDropEffects _dragMode;
	}

	public class DragVerifyEventArgs : DragSourceDestinationEventArgs
	{
		/// <summary>
		/// settable by the event handler: If true, the drag target
		/// (source/destination combo) is valid, and can proceed
		/// </summary>
		public bool ValidDragTarget
		{
			get { return _validDragTarget; }
			set { _validDragTarget = value; }
		}

		public DragBetweenNodes DragBetweenNodes
		{
			get { return _dragBetweenNodes; }
			set { _dragBetweenNodes = value; }
		}

		public int KeyState
		{
			get { return _keyState; }
			set { _keyState = value; }
		}

		public DragDropEffects DragMode
		{
			get { return _dragMode; }
			set { _dragMode = value; }
		}

		private DragBetweenNodes _dragBetweenNodes;
		private bool _validDragTarget = true;
		private int _keyState = 0;
		private DragDropEffects _dragMode;
	}

	public class DragSourceDestinationEventArgs : EventArgs
	{
		/// <summary>
		/// The nodes that were being dragged
		/// </summary>
		public List<TreeNode> SourceNodes
		{
			get { return _sourceNodes; }
			set { _sourceNodes = value; }
		}

		/// <summary>
		/// The node that the source node was dragged onto.
		/// </summary>
		public TreeNode TargetNode
		{
			get { return _targetNode; }
			set { _targetNode = value; }
		}

		private TreeNode _targetNode;
		private List<TreeNode> _sourceNodes;
	}

	public class DragSourceEventArgs : EventArgs
	{
		/// <summary>
		/// The nodes that were/are being dragged
		/// </summary>
		public List<TreeNode> Nodes
		{
			get { return _nodes; }
			set { _nodes = value; }
		}

		private List<TreeNode> _nodes;
	}

	public class DragStartEventArgs : EventArgs
	{
		public DragStartEventArgs()
		{
			CancelDrag = false;
		}

		/// <summary>
		/// The nodes that were/are being dragged
		/// </summary>
		public List<TreeNode> Nodes
		{
			get { return _nodes; }
			set { _nodes = value; }
		}

		private List<TreeNode> _nodes;

		public bool CancelDrag { get; set; }
	}

	#endregion

	public class TreeNodeSorter : IComparer<TreeNode>
	{
		private TreeView _treeView;

		public TreeNodeSorter(TreeView treeView)
		{
			_treeView = treeView;
		}

		public int Compare(TreeNode x, TreeNode y)
		{
			if (x == null && y != null)
				return -1;
			if (x != null && y == null)
				return 1;
			if (x == y)
				return 0;

			if (x.Parent == y.Parent)
			{
				if (x.Index > y.Index) return 1;
				return -1;
			}

			TreeNode first = FindFirstInCollection(_treeView.Nodes, x, y);

			if (first == x)
				return -1;

			if (first == y)
				return 1;

			return 0;
		}

		public TreeNode FindFirstInCollection(TreeNodeCollection coll, TreeNode x, TreeNode y)
		{
			if (coll == null)
				return null;

			foreach (TreeNode node in coll) {
				if (node == x) return x;
				if (node == y) return y;
				TreeNode result = FindFirstInCollection(node.Nodes, x, y);
				if (result != null)
					return result;
			}

			return null;
		}
	}
}
