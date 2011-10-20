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


namespace CommonElements
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
		public event DragItemEventHandler DragStart;

		/// <summary>
		/// Occurs when an item is dragged and dropped onto another.
		/// </summary>
		public event DragCompleteEventHandler DragComplete;

		/// <summary>
		/// Occurs when an item is dragged, and the drag is cancelled.
		/// </summary>
		public event DragItemEventHandler DragCancel;

		#endregion


		#region Private members
		private int _dragImageIndex;
		private DragDropEffects _dragMode = DragDropEffects.Move;
		private Color _dragOverNodeForeColor = SystemColors.HighlightText;
		private Color _dragOverNodeBackColor = SystemColors.Highlight;
		private DragCursorType _dragCursorType;
		private Cursor _dragCursor = null;
		private TreeNode _previousNode;
		private TreeNode _selectedNode;
		private FormDrag _formDrag = new FormDrag();
		#endregion


		#region Public properties
		/// <summary>
		/// The imagelist control from which DragImage icons are taken.
		/// </summary>
		[
		Description("The imagelist control from which DragImage icons are taken."),
		Category("Drag and drop")
		]
		public ImageList DragImageList
		{
			get
			{
				return _formDrag.imageList1;
			}
			set
			{
				if (value == _formDrag.imageList1) {
					return;
				}

				_formDrag.imageList1 = value;

				// Change the picture box to use this image
				if (_formDrag.imageList1.Images.Count > 0 && _formDrag.imageList1.Images[_dragImageIndex] != null) {
					_formDrag.pictureBox1.Image = _formDrag.imageList1.Images[_dragImageIndex];
					_formDrag.Height = _formDrag.pictureBox1.Image.Height;
				}

				if (!base.IsHandleCreated) {
					return;
				}
				SendMessage((IntPtr)4361, 0, ((value == null) ? IntPtr.Zero : value.Handle), 0);
			}

		}

		/// <summary>
		/// The default image index for the DragImage icon.
		/// </summary>
		[
		Description("The default image index for the DragImage icon."),
		Category("Drag and drop"),
		TypeConverter(typeof(ImageIndexConverter)),
		Editor("System.Windows.Forms.Design.ImageIndexEditor", typeof(System.Drawing.Design.UITypeEditor))
		]
		public int DragImageIndex
		{
			get
			{
				if (_formDrag.imageList1 == null) {
					return -1;
				}

				if (_dragImageIndex >= _formDrag.imageList1.Images.Count) {
					return Math.Max(0, (_formDrag.imageList1.Images.Count - 1));
				} else

					return _dragImageIndex;
			}
			set
			{
				// Change the picture box to use this image
				if (_formDrag.imageList1.Images.Count > 0 && _formDrag.imageList1.Images[value] != null) {
					_formDrag.pictureBox1.Image = _formDrag.imageList1.Images[value];
					_formDrag.Size = new Size(_formDrag.Width, _formDrag.pictureBox1.Image.Height);
					_formDrag.labelText.Size = new Size(_formDrag.labelText.Width, _formDrag.pictureBox1.Image.Height);
				}

				_dragImageIndex = value;
			}
		}

		/// <summary>
		/// The custom cursor to use when dragging an item, if DragCursor is set to Custom.
		/// </summary>
		[
		Description("The custom cursor to use when dragging an item, if DragCursor is set to Custom."),
		Category("Drag and drop")
		]
		public Cursor DragCursor
		{
			get
			{
				return _dragCursor;
			}
			set
			{
				if (value == _dragCursor) {
					return;
				}

				_dragCursor = value;
				if (!base.IsHandleCreated) {
					return;
				}
			}
		}

		/// <summary>
		/// The cursor type to use when dragging - None uses the default drag and drop cursor, DragIcon uses an icon and label, Custom uses a custom cursor.
		/// </summary>
		[
		Description("The cursor type to use when dragging - None uses the default drag and drop cursor, DragIcon uses an icon and label, Custom uses a custom cursor."),
		Category("Drag and drop")
		]
		public DragCursorType DragCursorType
		{
			get
			{
				return _dragCursorType;
			}
			set
			{
				_dragCursorType = value;
			}
		}

		/// <summary>
		/// Sets the font for the dragged node (shown as ghosted text/icon).
		/// </summary>
		[
		Description("Sets the font for the dragged node (shown as ghosted text/icon)."),
		Category("Drag and drop")
		]
		public Font DragNodeFont
		{
			get
			{
				return _formDrag.labelText.Font;
			}
			set
			{
				_formDrag.labelText.Font = value;

				// Set the drag form height to the font height
				_formDrag.Size = new Size(_formDrag.Width, (int)_formDrag.labelText.Font.GetHeight());
				_formDrag.labelText.Size = new Size(_formDrag.labelText.Width, (int)_formDrag.labelText.Font.GetHeight());


			}
		}

		/// <summary>
		/// Sets the opacity for the dragged node (shown as ghosted text/icon).
		/// </summary>
		[
			Description("Sets the opacity for the dragged node (shown as ghosted text/icon)."),
			Category("Drag and drop"),
			TypeConverter(typeof(System.Windows.Forms.OpacityConverter))
		]
		public double DragNodeOpacity
		{
			get
			{
				return _formDrag.Opacity;
			}
			set
			{
				_formDrag.Opacity = value;
			}
		}

		/// <summary>
		/// The background colour of the node being dragged over.
		/// </summary>
		[
			Description("The background colour of the node being dragged over."),
			Category("Drag and drop")
		]
		public Color DragOverNodeBackColor
		{
			get
			{
				return _dragOverNodeBackColor;
			}
			set
			{
				_dragOverNodeBackColor = value;
			}
		}

		/// <summary>
		/// The foreground colour of the node being dragged over.
		/// </summary>
		[
			Description("The foreground colour of the node being dragged over."),
			Category("Drag and drop")
		]
		public Color DragOverNodeForeColor
		{
			get
			{
				return _dragOverNodeForeColor;
			}
			set
			{
				_dragOverNodeForeColor = value;
			}
		}

		/// <summary>
		/// The drag mode (move,copy etc.)
		/// </summary>
		[
			Description("The drag mode (move,copy etc.)"),
			Category("Drag and drop")
		]
		public DragDropEffects DragMode
		{
			get
			{
				return _dragMode;
			}
			set
			{
				_dragMode = value;
			}
		}
		#endregion


		#region Selected Node(s) Properties

		private List<TreeNode> m_SelectedNodes = null;		
		public List<TreeNode> SelectedNodes
		{
			get
			{
				return m_SelectedNodes;
			}
			set
			{
				ClearSelectedNodes();
				if( value != null )
				{
					foreach( TreeNode node in value )
					{
						ToggleNode( node, true );
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
				if( value != null )
				{
					SelectNode( value );
				}
			}
		}

		public void AddSelectedNode(TreeNode node)
		{
			m_SelectedNodes.Add(node);
			ToggleNode(node, true);
		}

		#endregion


		public MultiSelectTreeview()
		{
			m_SelectedNodes = new List<TreeNode>();
			base.SelectedNode = null;

			base.SetStyle(ControlStyles.DoubleBuffer, true);
			AllowDrop = true;

			// Set the drag form to have ambient properties
			_formDrag.labelText.Font = Font;
			_formDrag.BackColor = BackColor;

			// Custom cursor handling
			if (_dragCursorType == DragCursorType.Custom && _dragCursor != null) {
				DragCursor = _dragCursor;
			}

			_formDrag.Show();
			_formDrag.Visible = false;
		}


		#region Overridden Events

		protected override void OnGotFocus( EventArgs e )
		{
			// Make sure at least one node has a selection
			// this way we can tab to the ctrl and use the 
			// keyboard to select nodes
			try
			{
				if( m_SelectedNode == null && TopNode != null )
				{
					ToggleNode( TopNode, true );
				}

				base.OnGotFocus( e );
			}
			catch( Exception ex )
			{
				HandleException( ex );
			}
		}

		protected override void OnMouseDown( MouseEventArgs e )
		{
			// If the user clicks on a node that was not
			// previously selected, select it now.

			try
			{
				base.SelectedNode = null;

				TreeNode node = GetNodeAt( e.Location );
				if( node != null )
				{
					int leftBound = node.Bounds.X; // - 20; // Allow user to click on image
					int rightBound = node.Bounds.Right + 10; // Give a little extra room
					if( e.Location.X > leftBound && e.Location.X < rightBound )
					{
						if( ModifierKeys == Keys.None && ( m_SelectedNodes.Contains( node ) ) )
						{
							// Potential Drag Operation
							// Let Mouse Up do select
						}
						else
						{							
							SelectNode( node );
						}
					}
				}

				base.OnMouseDown( e );
			}
			catch( Exception ex )
			{
				HandleException( ex );
			}
		}

		protected override void OnMouseUp( MouseEventArgs e )
		{
			// If the clicked on a node that WAS previously
			// selected then, reselect it now. This will clear
			// any other selected nodes. e.g. A B C D are selected
			// the user clicks on B, now A C & D are no longer selected.
			try
			{
				// Check to see if a node was clicked on 
				TreeNode node = GetNodeAt( e.Location );
				if( node != null )
				{
					if( ModifierKeys == Keys.None && m_SelectedNodes.Contains( node ) )
					{
						int leftBound = node.Bounds.X; // -20; // Allow user to click on image
						int rightBound = node.Bounds.Right + 10; // Give a little extra room
						if( e.Location.X > leftBound && e.Location.X < rightBound )
						{

							SelectNode( node );
						}
					}
				}

				base.OnMouseUp( e );
			}
			catch( Exception ex )
			{
				HandleException( ex );
			}
		}

		protected override void OnItemDrag( ItemDragEventArgs e )
		{
			// If the user drags a node and the node being dragged is NOT
			// selected, then clear the active selection, select the
			// node being dragged and drag it. Otherwise if the node being
			// dragged is selected, drag the entire selection.
			try
			{
				TreeNode node = e.Item as TreeNode;

				if( node != null )
				{
					if( !m_SelectedNodes.Contains( node ) )
					{
						SelectSingleNode( node );
						ToggleNode( node, true );
					}
				}

				base.OnItemDrag( e );
			}
			catch( Exception ex )
			{
				HandleException( ex );
			}

			_selectedNode = (TreeNode)e.Item;

			// Call dragstart event
			if (DragStart != null) {
				DragItemEventArgs ea = new DragItemEventArgs();
				ea.Node = _selectedNode;

				DragStart(this, ea);
			}
			// Change any previous node back 
			if (_previousNode != null) {
				_previousNode.BackColor = SystemColors.HighlightText;
				_previousNode.ForeColor = SystemColors.ControlText;
			}

			// Move the form with the icon/label on it
			// A better width measurement algo for the form is needed here

			int width = _selectedNode.Text.Length * (int)_formDrag.labelText.Font.Size;
			if (_selectedNode.Text.Length < 5)
				width += 20;

			_formDrag.Size = new Size(width, _formDrag.Height);

			_formDrag.labelText.Size = new Size(width, _formDrag.labelText.Size.Height);
			_formDrag.labelText.Text = _selectedNode.Text;

			// Start drag drop
			DoDragDrop(e.Item, _dragMode);
		}

		protected override void OnBeforeSelect( TreeViewCancelEventArgs e )
		{
			// Never allow base.SelectedNode to be set!
			try
			{
				base.SelectedNode = null;
				e.Cancel = true;

				base.OnBeforeSelect( e );
			}
			catch( Exception ex )
			{
				HandleException( ex );
			}
		}

		protected override void OnAfterSelect( TreeViewEventArgs e )
		{
			// Never allow base.SelectedNode to be set!
			try
			{
				base.OnAfterSelect( e );
				base.SelectedNode = null;
			}
			catch( Exception ex )
			{
				HandleException( ex );
			}
		}

		protected override void OnKeyDown( KeyEventArgs e )
		{
			// Handle all possible key strokes for the control.
			// including navigation, selection, etc.

			base.OnKeyDown( e );

			if( e.KeyCode == Keys.ShiftKey ) return;

			//BeginUpdate();
			bool bShift = ( ModifierKeys == Keys.Shift );

			try
			{
				// Nothing is selected in the tree, this isn't a good state
				// select the top node
				if( m_SelectedNode == null && TopNode != null )
				{
					ToggleNode( TopNode, true );
				}

				// Nothing is still selected in the tree, this isn't a good state, leave.
				if( m_SelectedNode == null ) return;

				if( e.KeyCode == Keys.Left )
				{
					if( m_SelectedNode.IsExpanded && m_SelectedNode.Nodes.Count > 0 )
					{
						// Collapse an expanded node that has children
						m_SelectedNode.Collapse();
					}
					else if( m_SelectedNode.Parent != null )
					{
						// Node is already collapsed, try to select its parent.
						SelectSingleNode( m_SelectedNode.Parent );
					}
				}
				else if( e.KeyCode == Keys.Right )
				{
					if( !m_SelectedNode.IsExpanded )
					{
						// Expand a collpased node's children
						m_SelectedNode.Expand();
					}
					else
					{
						// Node was already expanded, select the first child
						SelectSingleNode( m_SelectedNode.FirstNode );
					}
				}
				else if( e.KeyCode == Keys.Up )
				{
					// Select the previous node
					if( m_SelectedNode.PrevVisibleNode != null )
					{
						SelectNode( m_SelectedNode.PrevVisibleNode );
					}
				}
				else if( e.KeyCode == Keys.Down )
				{
					// Select the next node
					if( m_SelectedNode.NextVisibleNode != null )
					{
						SelectNode( m_SelectedNode.NextVisibleNode );
					}
				}
				else if( e.KeyCode == Keys.Home )
				{
					if( bShift )
					{
						if( m_SelectedNode.Parent == null )
						{
							// Select all of the root nodes up to this point 
							if( Nodes.Count > 0 )
							{
								SelectNode( Nodes[0] );
							}
						}
						else
						{
							// Select all of the nodes up to this point under this nodes parent
							SelectNode( m_SelectedNode.Parent.FirstNode );
						}
					}
					else
					{
						// Select this first node in the tree
						if( Nodes.Count > 0 )
						{
							SelectSingleNode( Nodes[0] );
						}
					}
				}
				else if( e.KeyCode == Keys.End )
				{
					if( bShift )
					{
						if( m_SelectedNode.Parent == null )
						{
							// Select the last ROOT node in the tree
							if( Nodes.Count > 0 )
							{
								SelectNode( Nodes[Nodes.Count - 1] );
							}
						}
						else
						{
							// Select the last node in this branch
							SelectNode( m_SelectedNode.Parent.LastNode );
						}
					}
					else
					{
						if( Nodes.Count > 0 )
						{
							// Select the last node visible node in the tree.
							// Don't expand branches incase the tree is virtual
							TreeNode ndLast = Nodes[0].LastNode;
							while( ndLast.IsExpanded && ( ndLast.LastNode != null ) )
							{
								ndLast = ndLast.LastNode;
							}
							SelectSingleNode( ndLast );
						}
					}
				}
				else if( e.KeyCode == Keys.PageUp )
				{
					// Select the highest node in the display
					int nCount = VisibleCount;
					TreeNode ndCurrent = m_SelectedNode;
					while( ( nCount ) > 0 && ( ndCurrent.PrevVisibleNode != null ) )
					{
						ndCurrent = ndCurrent.PrevVisibleNode;
						nCount--;
					}
					SelectSingleNode( ndCurrent );
				}
				else if( e.KeyCode == Keys.PageDown )
				{
					// Select the lowest node in the display
					int nCount = VisibleCount;
					TreeNode ndCurrent = m_SelectedNode;
					while( ( nCount ) > 0 && ( ndCurrent.NextVisibleNode != null ) )
					{
						ndCurrent = ndCurrent.NextVisibleNode;
						nCount--;
					}
					SelectSingleNode( ndCurrent );
				}
				else
				{
					// Assume this is a search character a-z, A-Z, 0-9, etc.
					// Select the first node after the current node that 
					// starts with this character
					string sSearch = ( (char) e.KeyValue ).ToString();

					TreeNode ndCurrent = m_SelectedNode;
					while( ( ndCurrent.NextVisibleNode != null ) )
					{
						ndCurrent = ndCurrent.NextVisibleNode;
						if( ndCurrent.Text.StartsWith( sSearch ) )
						{
							SelectSingleNode( ndCurrent );
							break;
						}
					}
				}
			}
			catch( Exception ex )
			{
				HandleException( ex );
			}
			finally
			{
				EndUpdate();
			}
		}





		protected override void WndProc(ref Message m)
		{
			//System.Diagnostics.Debug.WriteLine(m);
			// Stop erase background message
			if (m.Msg == (int)0x0014) {
				m.Msg = (int)0x0000; // Set to null
			}

			base.WndProc(ref m);
		}

		protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
		{
			if (e.Effect == _dragMode) {
				e.UseDefaultCursors = false;

				if (_dragCursorType == DragCursorType.Custom && _dragCursor != null) {
					// Custom cursor
					Cursor = _dragCursor;
				} else if (_dragCursorType == DragCursorType.DragIcon) {
					// This removes the default drag + drop cursor
					Cursor = Cursors.Default;
				} else {
					e.UseDefaultCursors = true;
				}
			} else {
				e.UseDefaultCursors = true;
				Cursor = Cursors.Default;
			}
		}

		protected override void OnDragOver(DragEventArgs e)
		{
			// Change any previous node back
			if (_previousNode != null) {
				_previousNode.BackColor = SystemColors.HighlightText;
				_previousNode.ForeColor = SystemColors.ControlText;
			}

			// Get the node from the mouse position, colour it
			Point pt = ((TreeView)this).PointToClient(new Point(e.X, e.Y));
			TreeNode treeNode = GetNodeAt(pt);
			treeNode.BackColor = _dragOverNodeBackColor;
			treeNode.ForeColor = _dragOverNodeForeColor;

			// Move the icon form
			if (_dragCursorType == DragCursorType.DragIcon) {
				_formDrag.Location = new Point(e.X + 5, e.Y - 5);
				_formDrag.Visible = true;
			}

			// Scrolling down/up
			if (pt.Y + 10 > ClientSize.Height)
				SendMessage(Handle, 277, (IntPtr)1, 0);
			else if (pt.Y < Top + 10)
				SendMessage(Handle, 277, (IntPtr)0, 0);

			// Remember the target node, so we can set it back
			_previousNode = treeNode;
		}

		protected override void OnDragLeave(EventArgs e)
		{
			if (_selectedNode != null) {
				SelectedNode = _selectedNode;
			}

			if (_previousNode != null) {
				_previousNode.BackColor = _dragOverNodeBackColor;
				_previousNode.ForeColor = _dragOverNodeForeColor;
			}

			_formDrag.Visible = false;
			Cursor = Cursors.Default;

			// Call cancel event
			if (DragCancel != null) {
				DragItemEventArgs ea = new DragItemEventArgs();
				ea.Node = _selectedNode;

				DragCancel(this, ea);
			}
		}

		protected override void OnDragEnter(DragEventArgs e)
		{
			e.Effect = _dragMode;

			// Reset the previous node var
			_previousNode = null;
			_selectedNode = null;
			Debug.WriteLine(_formDrag.labelText.Size);
		}

		protected override void OnDragDrop(DragEventArgs e)
		{
			// Custom cursor handling
			if (_dragCursorType == DragCursorType.DragIcon) {
				Cursor = Cursors.Default;
			}

			_formDrag.Visible = false;

			// Check it's a treenode being dragged
			if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", false)) {
				TreeNode dragNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");

				// Get the target node from the mouse coords
				Point pt = ((TreeView)this).PointToClient(new Point(e.X, e.Y));
				TreeNode targetNode = GetNodeAt(pt);

				// De-color it
				targetNode.BackColor = SystemColors.HighlightText;
				targetNode.ForeColor = SystemColors.ControlText;

				// 1) Check we're not dragging onto ourself
				// 2) Check we're not dragging onto one of our children 
				// (this is the lazy way, will break if there are nodes with the same name,
				// but it's quicker than checking all nodes below is)
				// 3) Check we're not dragging onto our parent
				if (targetNode != dragNode && !targetNode.FullPath.StartsWith(dragNode.FullPath) && dragNode.Parent != targetNode) {
					// Copy the node, add as a child to the destination node
					TreeNode newTreeNode = (TreeNode)dragNode.Clone();
					targetNode.Nodes.Add(newTreeNode);
					targetNode.Expand();

					// Remove Original Node, set the dragged node as selected
					dragNode.Remove();
					SelectedNode = newTreeNode;

					Cursor = Cursors.Default;

					// Call drag complete event
					if (DragComplete != null) {
						DragCompleteEventArgs ea = new DragCompleteEventArgs();
						ea.SourceNode = dragNode;
						ea.TargetNode = targetNode;

						DragComplete(this, ea);
					}
				}
			}
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape) {
				if (_selectedNode != null) {
					SelectedNode = _selectedNode;
				}

				if (_previousNode != null) {
					_previousNode.BackColor = SystemColors.HighlightText;
					_previousNode.ForeColor = SystemColors.ControlText;
				}

				Cursor = Cursors.Default;
				_formDrag.Visible = false;

				// Call cancel event
				if (DragCancel != null) {
					DragItemEventArgs ea = new DragItemEventArgs();
					ea.Node = _selectedNode;

					DragCancel(this, ea);
				}
			}
		}


		#endregion


		#region Helper Methods

		private void SelectNode( TreeNode node )
		{
			try
			{
				this.BeginUpdate();

				if( m_SelectedNode == null || ModifierKeys == Keys.Control )
				{
					// Ctrl+Click selects an unselected node, or unselects a selected node.
					bool bIsSelected = m_SelectedNodes.Contains( node );
					ToggleNode( node, !bIsSelected );
				}
				else if( ModifierKeys == Keys.Shift )
				{
					// Shift+Click selects nodes between the selected node and here.
					TreeNode ndStart = m_SelectedNode;
					TreeNode ndEnd = node;

					if( ndStart.Parent == ndEnd.Parent )
					{
						// Selected node and clicked node have same parent, easy case.
						if( ndStart.Index < ndEnd.Index )
						{							
							// If the selected node is beneath the clicked node walk down
							// selecting each Visible node until we reach the end.
							while( ndStart != ndEnd )
							{
								ndStart = ndStart.NextVisibleNode;
								if( ndStart == null ) break;
								ToggleNode( ndStart, true );
							}
						}
						else if( ndStart.Index == ndEnd.Index )
						{
							// Clicked same node, do nothing
						}
						else
						{
							// If the selected node is above the clicked node walk up
							// selecting each Visible node until we reach the end.
							while( ndStart != ndEnd )
							{
								ndStart = ndStart.PrevVisibleNode;
								if( ndStart == null ) break;
								ToggleNode( ndStart, true );
							}
						}
					}
					else
					{
						// Selected node and clicked node have same parent, hard case.
						// We need to find a common parent to determine if we need
						// to walk down selecting, or walk up selecting.

						TreeNode ndStartP = ndStart;
						TreeNode ndEndP = ndEnd;
						int startDepth = Math.Min( ndStartP.Level, ndEndP.Level );

						// Bring lower node up to common depth
						while( ndStartP.Level > startDepth )
						{
							ndStartP = ndStartP.Parent;
						}

						// Bring lower node up to common depth
						while( ndEndP.Level > startDepth )
						{
							ndEndP = ndEndP.Parent;
						}

						// Walk up the tree until we find the common parent
						while( ndStartP.Parent != ndEndP.Parent )
						{
							ndStartP = ndStartP.Parent;
							ndEndP = ndEndP.Parent;
						}

						// Select the node
						if( ndStartP.Index < ndEndP.Index )
						{
							// If the selected node is beneath the clicked node walk down
							// selecting each Visible node until we reach the end.
							while( ndStart != ndEnd )
							{
								ndStart = ndStart.NextVisibleNode;
								if( ndStart == null ) break;
								ToggleNode( ndStart, true );
							}
						}
						else if( ndStartP.Index == ndEndP.Index )
						{
							if( ndStart.Level < ndEnd.Level )
							{
								while( ndStart != ndEnd )
								{
									ndStart = ndStart.NextVisibleNode;
									if( ndStart == null ) break;
									ToggleNode( ndStart, true );
								}
							}
							else
							{
								while( ndStart != ndEnd )
								{
									ndStart = ndStart.PrevVisibleNode;
									if( ndStart == null ) break;
									ToggleNode( ndStart, true );
								}
							}
						}
						else
						{
							// If the selected node is above the clicked node walk up
							// selecting each Visible node until we reach the end.
							while( ndStart != ndEnd )
							{
								ndStart = ndStart.PrevVisibleNode;
								if( ndStart == null ) break;
								ToggleNode( ndStart, true );
							}
						}
					}
				}
				else
				{
					// Just clicked a node, select it
					SelectSingleNode( node );
				}

				OnAfterSelect( new TreeViewEventArgs( m_SelectedNode ) );
			}
			finally
			{
				this.EndUpdate();
			}
		}

		private void ClearSelectedNodes()
		{
			try
			{
				foreach( TreeNode node in m_SelectedNodes )
				{
					node.BackColor = this.BackColor;
					node.ForeColor = this.ForeColor;
				}
			}
			finally
			{
				m_SelectedNodes.Clear();
				m_SelectedNode = null;
			}
		}

		private void SelectSingleNode( TreeNode node )
		{
			if( node == null )
			{
				return;
			}

			ClearSelectedNodes();
			ToggleNode( node, true );
			node.EnsureVisible();
		}

		private void ToggleNode( TreeNode node, bool bSelectNode )
		{
			if( bSelectNode )
			{
				m_SelectedNode = node;
				if( !m_SelectedNodes.Contains( node ) )
				{
					m_SelectedNodes.Add( node );
				}
				node.BackColor = SystemColors.Highlight;
				node.ForeColor = SystemColors.HighlightText;
			}
			else
			{
				m_SelectedNodes.Remove( node );
				node.BackColor = this.BackColor;
				node.ForeColor = this.ForeColor;
			}
		}

		private void HandleException( Exception ex )
		{
			// Perform some error handling here.
			// We don't want to bubble errors to the CLR. 
			MessageBox.Show( ex.Message );
		}

		#endregion


		#region FormDrag form
		internal class FormDrag : Form
		{
			#region Components
			public System.Windows.Forms.Label labelText;
			public System.Windows.Forms.PictureBox pictureBox1;
			public System.Windows.Forms.ImageList imageList1;
			private System.ComponentModel.Container components = null;
			#endregion

			#region Constructor, dispose
			public FormDrag()
			{
				InitializeComponent();
			}

			/// <summary>
			/// Clean up any resources being used.
			/// </summary>
			protected override void Dispose(bool disposing)
			{
				if (disposing) {
					if (components != null) {
						components.Dispose();
					}
				}
				base.Dispose(disposing);
			}
			#endregion

			#region Windows Form Designer generated code
			/// <summary>
			/// Required method for Designer support - do not modify
			/// the contents of this method with the code editor.
			/// </summary>
			private void InitializeComponent()
			{
				components = new System.ComponentModel.Container();
				labelText = new System.Windows.Forms.Label();
				pictureBox1 = new System.Windows.Forms.PictureBox();
				imageList1 = new System.Windows.Forms.ImageList(components);
				SuspendLayout();
				// 
				// labelText
				// 
				labelText.BackColor = System.Drawing.Color.Transparent;
				labelText.Location = new System.Drawing.Point(16, 2);
				labelText.Name = "labelText";
				labelText.Size = new System.Drawing.Size(100, 16);
				labelText.TabIndex = 0;
				// 
				// pictureBox1
				// 
				pictureBox1.Location = new System.Drawing.Point(0, 0);
				pictureBox1.Name = "pictureBox1";
				pictureBox1.Size = new System.Drawing.Size(16, 16);
				pictureBox1.TabIndex = 1;
				pictureBox1.TabStop = false;
				// 
				// Form2
				// 
				AutoScaleBaseSize = new System.Drawing.Size(5, 13);
				BackColor = System.Drawing.SystemColors.Control;
				ClientSize = new System.Drawing.Size(100, 16);
				Controls.Add(pictureBox1);
				Controls.Add(labelText);
				Size = new Size(300, 500);
				FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
				Opacity = 0.3;
				ShowInTaskbar = false;
				ResumeLayout(false);

			}
			#endregion
		}
		#endregion
	}

	#region DragCursorType enum
	[Serializable]
	public enum DragCursorType
	{
		None,
		DragIcon,
		Custom
	}
	#endregion

	#region Event classes/delegates
	public delegate void DragCompleteEventHandler(object sender, DragCompleteEventArgs e);
	public delegate void DragItemEventHandler(object sender, DragItemEventArgs e);

	public class DragCompleteEventArgs : EventArgs
	{
		/// <summary>
		/// The node that was being dragged
		/// </summary>
		public TreeNode SourceNode
		{
			get
			{
				return _sourceNode;
			}
			set
			{
				_sourceNode = value;
			}
		}

		/// <summary>
		/// The node that the source node was dragged onto.
		/// </summary>
		public TreeNode TargetNode
		{
			get
			{
				return _targetNode;
			}
			set
			{
				_targetNode = value;
			}
		}

		private TreeNode _targetNode;
		private TreeNode _sourceNode;
	}

	public class DragItemEventArgs : EventArgs
	{
		/// <summary>
		/// The ndoe that was being dragged
		/// </summary>
		public TreeNode Node
		{
			get
			{
				return _node;
			}
			set
			{
				_node = value;
			}
		}

		private TreeNode _node;
	}
	#endregion

}

