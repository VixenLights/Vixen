using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows.Forms;
using Common.Controls.NameGeneration;
using Common.Controls.Theme;
using Vixen.Module.Property;
using Vixen.Services;
using Vixen.Sys;

namespace Common.Controls
{
	public partial class ElementTree : UserControl
	{
		// sets of data to keep track of which items in the treeview are open, selected, visible etc., so that
		// when we reload the tree, we can keep it looking relatively consistent with what the user had before.
		private readonly HashSet<string> _expandedNodes = new HashSet<string>(); // TreeNode paths that are expanded
		private HashSet<string> _selectedNodes; // TreeNode paths that are selected
		private List<string> _topDisplayedNodes; // TreeNode paths that are at the top of the view. Should only
		// need one, but will have multiple in case the top node is deleted.
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private const string VirtualNodeName = @"VIRT";

		public ElementTree()
		{
			InitializeComponent();
			AutoSize = true;
			treeview.Dock = DockStyle.Fill;
			contextMenuStripTreeView.Renderer = new ThemeToolStripRenderer();
			treeview.DragFinishing += treeviewDragFinishingHandler;
			treeview.DragOverVerify += treeviewDragVerifyHandler;
			treeview.DragStart += treeview_DragStart;

			treeview.BeforeExpand += Treeview_BeforeExpand;
			treeview.AfterCollapse += TreeviewOnAfterCollapse;
			AllowDragging = true;
			AllowPropertyEdit = true;
			AllowWireExport = true;
		}

		private void TreeviewOnAfterCollapse(object sender, TreeViewEventArgs e)
		{
			_expandedNodes.Remove(GenerateTreeNodeFullPath(e.Node, treeview.PathSeparator));
		}

		private void Treeview_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			if (e.Node.Tag is ElementNode elementNode)
			{
				if (elementNode.Children.Any() && e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Name.Equals(VirtualNodeName))
				{
					AddChildrenToTree(e.Node, elementNode);
				}

				_expandedNodes.Add(GenerateTreeNodeFullPath(e.Node, treeview.PathSeparator));
			}
		}

		private void ElementTree_Load(object sender, EventArgs e)
		{
			if (!(DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)) {
				PopulateNodeTree();
			}
		}



		#region Control features

		// this contains all the 'features' of the control, as they may need to be customized to allow
		// different things in different places.  For example, dragging to reorder elements might not be
		// desired, or icons might not be wanted, or only groups displayed, etc., etc.

		public bool AllowDragging { get; set; }

		public bool AllowPropertyEdit { get; set; }

		public bool AllowWireExport { get; set; }


		#endregion



		#region Tree view population


		public void PopulateNodeTree(IEnumerable<ElementNode> elementsToSelect)
		{
			List<string> treeNodes = new List<string>();
			foreach (ElementNode elementNode in elementsToSelect) {
				treeNodes.Add(GenerateEquivalentTreeNodeFullPathFromElement(elementNode, treeview.PathSeparator));
			}			
			_PopulateNodeTree(treeNodes);
		}

		public void PopulateNodeTree(ElementNode elementToSelect)
		{
			List<string> treeNodes = new List<string>();
			treeNodes.Add(GenerateEquivalentTreeNodeFullPathFromElement(elementToSelect, treeview.PathSeparator));
			_PopulateNodeTree(treeNodes);
		}

		public void PopulateNodeTree()
		{
			_PopulateNodeTree();
		}

		public void UpdateScrollPosition()
		{
			if (treeview.SelectedNodes.Count > 0)
				treeview.TopNode = treeview.SelectedNodes[0];
		}

		private void _PopulateNodeTree(IEnumerable<string> elementTreeNodesToSelect = null)
		{
			// save metadata that is currently in the treeview
			_selectedNodes = new HashSet<string>();
			_topDisplayedNodes = new List<string>();

			SaveTreeNodeState(treeview.Nodes);
			SaveTreeNodeTopVisible();

			// clear the treeview, and repopulate it
			treeview.BeginUpdate();
			treeview.SelectedNode = null;
			treeview.SelectedNodes.Clear();
			treeview.Nodes.Clear();

			foreach (ElementNode element in VixenSystem.Nodes.GetRootNodes())
			{
				AddNodeToTree(treeview.Nodes, element, false);
			}

			// go through all the data we saved, and try to update the treeview to look
			// like it used to (expanded nodes, selected nodes, node at the top)

			foreach (string node in _expandedNodes)
			{
				TreeNode resultNode = FindNodeInTreeAtPath(treeview, node);
				resultNode?.Expand();
			}

			// if a new element has been passed in to select, select it instead.
			if (elementTreeNodesToSelect != null) {
				_selectedNodes = new HashSet<string>(elementTreeNodesToSelect);
			}
			foreach (string node in _selectedNodes)
			{
				SelectNodeAtPath(node);
			}

			treeview.EndUpdate();

			// see stackoverflow.com/questions/626315/winforms-listview-remembering-scrolled-location-on-reload .
			// we can only set the topNode after EndUpdate(). Also, it might throw an exception -- weird?
			foreach (string node in _topDisplayedNodes) {
				TreeNode resultNode = FindNodeInTreeAtPath(treeview, node);

				if (resultNode != null) {
					try {
						treeview.TopNode = resultNode;
					} catch (Exception) {
						 Logging.Warn("ConfigElements: exception caught trying to set TopNode.");
					}
					break;
				}
			}

			// finally, if we were selecting another element, make sure we raise the selection changed event
			if (elementTreeNodesToSelect != null)
			{
				// TODO: oops, we just pass the selection changed event through to the control; oh well,
				// an "elements have changed" event will do for now. Fix this sometime.
				OnElementsChanged();
			}
		}

		private void SelectNodeAtPath(string nodePath)
		{
			var paths = nodePath.Split(treeview.PathSeparator.ToCharArray());
			TreeNode resultNode = null;
			for (int i = 1; i <= paths.Length; i++)
			{
				var path = string.Join(treeview.PathSeparator, paths, 0, i);
				resultNode = FindNodeInTreeAtPath(treeview, path);
				if (i != paths.Length)
				{
					resultNode?.Expand();
				}
			}

			if (resultNode != null)
			{
				treeview.AddSelectedNode(resultNode);
			}
		}

		public void SelectElementNode(ElementNode node)
		{
			ClearSelectedNodes();
			var path = GenerateEquivalentTreeNodeFullPathFromElement(node, treeview.PathSeparator);
			SelectNodeAtPath(path);
		}

		private string GenerateTreeNodeFullPath(TreeNode node, string separator)
		{
			string result = node.Name;
			TreeNode parent = node.Parent;
			while (parent != null) {
				result = parent.Name + separator + result;
				parent = parent.Parent;
			}

			return result;
		}

		private string GenerateEquivalentTreeNodeFullPathFromElement(ElementNode element, string separator)
		{
			string result = element.Id.ToString();
			ElementNode parent = element.Parents.FirstOrDefault();
			while (parent != null && parent != VixenSystem.Nodes.RootNode) {
				result = parent.Id.ToString() + separator + result;
				parent = parent.Parents.FirstOrDefault();
			}

			return result;
		}


		private TreeNode FindNodeInTreeAtPath(TreeView tree, string path)
		{
			string[] subnodes = path.Split(new string[] { tree.PathSeparator }, StringSplitOptions.None);
			TreeNodeCollection searchNodes = tree.Nodes;
			TreeNode currentNode = null;
			foreach (string search in subnodes) {
				bool found = false;
				foreach (TreeNode tn in searchNodes) {
					if (tn.Name == search) {
						found = true;
						currentNode = tn;
						searchNodes = tn.Nodes;
						break;
					}
				}
				if (!found) {
					currentNode = null;
					break;
				}
			}

			return currentNode;
		}

		private void SaveTreeNodeState(TreeNodeCollection collection)
		{
			foreach (TreeNode tn in collection) {
				if (treeview.SelectedNodes.Contains(tn)) {
					_selectedNodes.Add(GenerateTreeNodeFullPath(tn, treeview.PathSeparator));
				}

				SaveTreeNodeState(tn.Nodes);
			}
		}

		private void SaveTreeNodeTopVisible()
		{
			// this will iterate through all root nodes -- starting with the topmost visible
			// node -- adding their path to a list in order. Later on, when refreshing the tree,
			// we can try them in order to place at the top of the display. We should only
			// need a single node, but in case the top node gets deleted (or the top few),
			// we keep a list of 'preferred' nodes.
			if (treeview.Nodes.Count > 0) {
				TreeNode current = treeview.TopNode;
				while (current != null) {
					_topDisplayedNodes.Add(GenerateTreeNodeFullPath(current, treeview.PathSeparator));
					current = current.NextNode;
				}
			}
		}

		private TreeNode AddNodeToTree(TreeNodeCollection collection, ElementNode elementNode, bool addChildren = true)
		{
			TreeNode addedNode = new TreeNode();
			addedNode.Name = elementNode.Id.ToString();
			addedNode.Text = elementNode.Name;
			addedNode.Tag = elementNode;
			
			UpdateTreeNodeImage(addedNode, elementNode);

			collection.Add(addedNode);

			if(addChildren)
			{
				foreach (ElementNode childNode in elementNode.Children)
				{
					AddNodeToTree(addedNode.Nodes, childNode);
				}
			}
			else if(elementNode.Children.Any())
			{
				TreeNode virtNode = new TreeNode();
				virtNode.Name = VirtualNodeName;
				addedNode.Nodes.Add(virtNode);
			}

			return addedNode;
		}

		private void AddChildrenToTree(TreeNode node, ElementNode elementNode)
		{
			node.Nodes.Clear();
			var nodesToExpand = new List<TreeNode>();
			foreach (ElementNode childNode in elementNode.Children)
			{
				var addedNode = AddNodeToTree(node.Nodes, childNode, false);
				var path = GenerateTreeNodeFullPath(addedNode, treeview.PathSeparator);
				if (_expandedNodes.Contains(path))
				{
					nodesToExpand.Add(addedNode);
				}
			}
			nodesToExpand.ForEach(x => x.Expand());
		}

		public void RefreshElementTreeStatus()
		{
			treeview.BeginUpdate();
			foreach (TreeNode node in treeview.Nodes)
			{
				RefreshElementTreeNode(node);
			}

			treeview.EndUpdate();
		}

		private void RefreshElementTreeNode(TreeNode node)
		{
			if (node.Tag is ElementNode elementNode)
			{
				UpdateTreeNodeImage(node, elementNode);

				foreach (TreeNode childNode in node.Nodes)
				{
					RefreshElementTreeNode(childNode);
				}
			}
		}

		private static void UpdateTreeNodeImage(TreeNode node, ElementNode elementNode)
		{
			if (!elementNode.Children.Any())
			{
				if (elementNode.Element != null &&
				    VixenSystem.DataFlow
					    .GetDestinationsOfComponent(VixenSystem.Elements.GetDataFlowComponentForElement(elementNode.Element))
					    .Any())
				{
					if (elementNode.Element.Masked)
						node.ImageKey = node.SelectedImageKey = @"RedBall";
					else
						node.ImageKey = node.SelectedImageKey = @"GreenBall";
				}
				else
					node.ImageKey = node.SelectedImageKey = @"WhiteBall";
			}
			else
			{
				node.ImageKey = node.SelectedImageKey = @"Group";
			}
		}

		#endregion



		#region Events

		public ElementNode SelectedNode
		{
			get
			{
				if (SelectedTreeNodes.Count <= 0)
					return null;
				return treeview.SelectedNode.Tag as ElementNode;
			}
		}

		public List<TreeNode> SelectedTreeNodes
		{
			get { return treeview.SelectedNodes; }
		}

		public IEnumerable<ElementNode> SelectedElementNodes
		{
			get { return treeview.SelectedNodes.Select(x => x.Tag as ElementNode); }
		}



		public event EventHandler treeviewDeselected
		{
			add { treeview.Deselected += value; }
			remove { treeview.Deselected -= value; }
		}

		public event TreeViewEventHandler treeviewAfterSelect
		{
			add { treeview.AfterSelect += value; }
			remove { treeview.AfterSelect -= value; }
		}

		public event EventHandler DragFinished;
		public event EventHandler ElementsChanged;


		public void OnDragFinished(EventArgs e = null)
		{
			if (e == null)
				e = EventArgs.Empty;
			EventHandler handler = DragFinished;
			if (handler != null) handler(this, e);
		}

		public void OnElementsChanged(EventArgs e = null)
		{
			if (e == null)
				e = EventArgs.Empty;
			EventHandler handler = ElementsChanged;
			if (handler != null) handler(this, e);
		}

		#endregion



		#region Drag handlers

		void treeview_DragStart(object sender, DragStartEventArgs e)
		{
			e.CancelDrag = !AllowDragging;
		}

		private void treeviewDragFinishingHandler(object sender, DragFinishingEventArgs e)
		{
			// we want to finish off the drag ourselves, and not have the treeview control move the nodes around.
			// (In fact, in a lame attempt at 'data binding', we're going to completely redraw the tree after
			// making all the required changes from this drag-drop.) So set the flag to indicate that.
			e.FinishDrag = false;

			// first determine the node that they will be moved to. This will depend on if we are dragging onto a node
			// directly, or above/below one to reorder.
			ElementNode newParentNode = null; // the ElementNode that the selected items will move to
			TreeNode expandNode = null; // if we need to expand a node once we've moved everything
			int index = -1;

			if (e.DragBetweenNodes == DragBetweenNodes.DragOnTargetNode) {
				newParentNode = e.TargetNode.Tag as ElementNode;
				expandNode = e.TargetNode;
			} else if (e.DragBetweenNodes == DragBetweenNodes.DragBelowTargetNode && e.TargetNode.IsExpanded) {
				newParentNode = e.TargetNode.Tag as ElementNode;
				expandNode = e.TargetNode;
				index = 0;
			} else {
				if (e.TargetNode.Parent == null) {
					newParentNode = null; // needs to go at the root level
				} else {
					newParentNode = e.TargetNode.Parent.Tag as ElementNode;
				}

				if (e.DragBetweenNodes == DragBetweenNodes.DragAboveTargetNode) {
					index = e.TargetNode.Index;
				} else {
					index = e.TargetNode.Index + 1;
				}
			}

			// Check to see if the new parent node would be 'losing' the Element (ie. becoming a
			// group instead of a leaf node with a element/patches). Prompt the user first.
			if (CheckAndPromptIfNodeWillLosePatches(newParentNode))
				return;

			// If moving element nodes, we need to iterate through all selected treenodes, and remove them from
			// the parent in which they are selected (which we can determine from the treenode parent), and add them
			// to the target node. If copying, we need to just add them to the new parent node.
			foreach (TreeNode treeNode in e.SourceNodes) {
				ElementNode sourceNode = treeNode.Tag as ElementNode;
				ElementNode oldParentNode = (treeNode.Parent != null) ? treeNode.Parent.Tag as ElementNode : null;
				int currentIndex = treeNode.Index;
				if (e.DragMode == DragDropEffects.Move) {
					if (index >= 0) {
						VixenSystem.Nodes.MoveNode(sourceNode, newParentNode, oldParentNode, index);

						// if we're moving nodes within the same group, but earlier in the group, then increment the target position each time.
						// This is because when the target is AFTER the current position, the shuffling offsets the nodes so that the target
						// index can stay the same. This isn't the case for the reverse case.
						if ((newParentNode != oldParentNode) ||
							(newParentNode == oldParentNode && index < currentIndex))
							index++;
					} else {
						VixenSystem.Nodes.MoveNode(sourceNode, newParentNode, oldParentNode);
					}
				} else if (e.DragMode == DragDropEffects.Copy) {
					if (index >= 0) {
						// increment the index after every move, so the items are inserted in the correct order (if not, they would be reversed)
						VixenSystem.Nodes.AddChildToParent(sourceNode, newParentNode, index++);
					} else {
						VixenSystem.Nodes.AddChildToParent(sourceNode, newParentNode);
					}
				} else {
					Logging.Warn("ConfigElements: Trying to deal with a drag that is an unknown type!");
				}
			}

			if (expandNode != null)
				expandNode.Expand();

			PopulateNodeTree();
			OnDragFinished();
		}

		private void treeviewDragVerifyHandler(object sender, DragVerifyEventArgs e)
		{
			// we need to go through all nodes that would be moved (source nodes), and check if there's any
			// problem moving any of them to the target node (circular references, etc.), since it's possible
			// for a element to exist multiple times in the treeview as different treenodes.

			List<ElementNode> nodes = new List<ElementNode>(e.SourceNodes.Select(x => x.Tag as ElementNode));

			// now get a list of invalid children for this target node, and check all the remaining nodes against it.
			// If any of them fail, the entire operation should fail, as it would be an invalid move.

			// the target node will be the actual target node if it is directly on it, otherwise it would be the parent
			// of the target node if it would be dragged above/below the taget element (as it would be put alongside it).
			// also keep track of the 'permitted' nodes: this is used when dragging alongside another element: any children
			// of the new parent node are considered OK, as we might just be shuffling them around. Normally, this would
			// be A Bad Thing, since it would seem like we're adding a child to the group it's already in. (This is only
			// the case when moving; if copying, it should be disabled. That's checked later.)
			IEnumerable<ElementNode> invalidNodesForTarget = null;
			IEnumerable<ElementNode> permittedNodesForTarget = null;

			if (e.DragBetweenNodes == DragBetweenNodes.DragOnTargetNode ||
				e.DragBetweenNodes == DragBetweenNodes.DragBelowTargetNode && e.TargetNode.IsExpanded) {
				invalidNodesForTarget = (e.TargetNode.Tag as ElementNode).InvalidChildren();
				permittedNodesForTarget = new HashSet<ElementNode>();
			} else {
				if (e.TargetNode.Parent == null) {
					invalidNodesForTarget = VixenSystem.Nodes.InvalidRootNodes;
					permittedNodesForTarget = VixenSystem.Nodes.GetRootNodes();
				} else {
					invalidNodesForTarget = (e.TargetNode.Parent.Tag as ElementNode).InvalidChildren();
					permittedNodesForTarget = (e.TargetNode.Parent.Tag as ElementNode).Children;
				}
			}

			if ((e.KeyState & 8) != 0) {
				// the CTRL key
				e.DragMode = DragDropEffects.Copy;
				permittedNodesForTarget = new HashSet<ElementNode>();
			} else {
				e.DragMode = DragDropEffects.Move;
			}

			IEnumerable<ElementNode> invalidSourceNodes = invalidNodesForTarget.Intersect(nodes);
            if (invalidSourceNodes.Any())
            {
				if (invalidSourceNodes.Intersect(permittedNodesForTarget).Count() == invalidSourceNodes.Count())
					e.ValidDragTarget = true;
				else
					e.ValidDragTarget = false;
			} else {
				e.ValidDragTarget = true;
			}
		}

		#endregion



		#region Helper functions

		public delegate void ExportDiagramDelegate(ElementNode node);

		public ExportDiagramDelegate ExportDiagram { get; set; }

		public void DeleteNode(TreeNode tn)
		{
			ElementNode cn = tn.Tag as ElementNode;
			ElementNode parent = (tn.Parent != null) ? tn.Parent.Tag as ElementNode : null;
			VixenSystem.Nodes.RemoveNode(cn, parent, true);
		}

		public IEnumerable<ElementNode> AddMultipleNodesWithPrompt(ElementNode parent = null)
		{
			List<ElementNode> result = new List<ElementNode>();

			// since we're adding multiple nodes, prompt with the name generation form (which also includes a counter on there).

			string newMultiName = "NewName";
			if (treeview.SelectedNode != null)
				newMultiName = treeview.SelectedNode.Text;
			
				using (NameGenerator nameGenerator = new NameGenerator(newMultiName))
				{
					if (nameGenerator.ShowDialog() == DialogResult.OK)
					{
						result.AddRange(
							nameGenerator.Names.Where(name => !string.IsNullOrEmpty(name)).Select(
								name => AddNewNode(name, false, parent, true)));
						if (!result.Any())
						{
							//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
							MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
							var messageBox = new MessageBoxForm("Could not create elements.  Ensure you use a valid name and try again.", "",
								false, false);
							messageBox.ShowDialog();
							return result;
						}
						AddNodePathToTree(result);
					}
				}

			return result;
		}

		public ElementNode AddSingleNodeWithPrompt(ElementNode parent = null)
		{
			// since we're only adding a single node, prompt with a single text dialog.
			using (TextDialog textDialog = new TextDialog("Element Name?")) {
				if (textDialog.ShowDialog() == DialogResult.OK) {
					string newName;
					if (textDialog.Response == string.Empty)
						newName = "New Element";
					else
						newName = textDialog.Response;

					ElementNode en = AddNewNode(newName, false, parent);
					AddNodePathToTree(new []{en});
					return en;
				}
			}

			return null;
		}


		public ElementNode AddNewNode(string nodeName, bool repopulateNodeTree = true, ElementNode parent = null,
		                               bool skipPatchCheck = false)
		{
			// prompt the user if it's going to make a patched leaf a group; if they abandon it, return null
			if (!skipPatchCheck && CheckAndPromptIfNodeWillLosePatches(parent))
				return null;

			ElementNode newNode = ElementNodeService.Instance.CreateSingle(parent, nodeName, true);
			if (repopulateNodeTree)
				PopulateNodeTree(newNode);
			return newNode;
		}

		public void AddNodePathToTree(IEnumerable<ElementNode> elementNodes)
		{
			_selectedNodes.Clear();
			
			treeview.BeginUpdate();
			ClearSelectedNodes();
			TreeNode resultNode = null;
			foreach (var elementNode in elementNodes)
			{
				if (elementNode.Parents.Any(x => x.Name!=@"Root"))
				{
					foreach (var nodeParent in elementNode.Parents)
					{
						var parentTreeNode = FindNodeInTreeAtPath(treeview, GenerateEquivalentTreeNodeFullPathFromElement(nodeParent, treeview.PathSeparator));
						if (parentTreeNode != null)
						{
							parentTreeNode.Nodes.Clear();
							AddChildrenToTree(parentTreeNode, nodeParent);
							_expandedNodes.Add(GenerateTreeNodeFullPath(parentTreeNode, treeview.PathSeparator));
							resultNode = FindNodeInTreeAtPath(treeview, GenerateEquivalentTreeNodeFullPathFromElement(elementNode, treeview.PathSeparator));
							_selectedNodes.Add(GenerateEquivalentTreeNodeFullPathFromElement(elementNode, treeview.PathSeparator));
							parentTreeNode.Expand();
						}
					}
				}
				else
				{
					resultNode = AddNodeToTree(treeview.Nodes, elementNode, false);
					_selectedNodes.Add(GenerateEquivalentTreeNodeFullPathFromElement(elementNode, treeview.PathSeparator));
				}
				
				if (resultNode != null)
				{
					treeview.AddSelectedNode(resultNode);
				}
			}
			
			treeview.EndUpdate();
			resultNode?.EnsureVisible();
		}

		public bool CreateGroupFromSelectedNodes()
		{
			// save this because AddSingle changes the selection to the new node
			var originalSelection = SelectedElementNodes.ToList();

			ElementNode newGroup = AddSingleNodeWithPrompt();
			if (newGroup == null)
				return false;

			foreach (ElementNode en in originalSelection) {
				VixenSystem.Nodes.AddChildToParent(en, newGroup);
			}

			PopulateNodeTree(newGroup);
			return true;
		}

		public bool CheckAndPromptIfNodeWillLosePatches(ElementNode node)
		{
			if (node != null && node.Element != null) {
				if (VixenSystem.DataFlow.GetDestinationsOfComponent(VixenSystem.Elements.GetDataFlowComponentForElement(node.Element)).Any()) {
					string message = "Adding items to this element will convert it into a Group, which will remove any " +
					                 "patches it may have. Are you sure you want to continue?";
					string title = "Convert Element to Group?";
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Question; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm(message, title, true, true);
					messageBox.ShowDialog();
					if (messageBox.DialogResult != DialogResult.OK)
					{
						return true;
					}
				}
			}

			return false;
		}

		public bool PatternRenameSelectedGroup()
		{
			if (SelectedTreeNodes.Count == 0)
				return false;

			List<string> oldNames = new List<string>(treeview.SelectedNodes.Select(x => x.Tag as ElementNode).Select(x => x.Name).ToArray());
			SubstitutionRenamer renamer = new SubstitutionRenamer(oldNames);
			if (renamer.ShowDialog() == DialogResult.OK)
			{
				for (int i = 0; i < treeview.SelectedNodes.Count; i++)
				{
					if (i >= renamer.Names.Count)
					{
						Logging.Warn("ConfigElements: bulk renaming elements, and ran out of new names!");
						break;
					}
					VixenSystem.Nodes.RenameNode((treeview.SelectedNodes[i].Tag as ElementNode), renamer.Names[i]);
				}

				PopulateNodeTree();

				return true;
			}

			return false;
		}

		public bool RenameSelectedElements()
		{
			if (SelectedTreeNodes.Count == 0)
				return false;

			if (SelectedTreeNodes.Count == 1) {
				using (TextDialog dialog = new TextDialog("Item name?", "Rename item", (SelectedNode).Name, true)) {
					if (dialog.ShowDialog() == DialogResult.OK) {
						if (dialog.Response != string.Empty && dialog.Response != SelectedNode.Name) {
							VixenSystem.Nodes.RenameNode(SelectedNode, dialog.Response);
							PopulateNodeTree();

							return true;
						}
					}
				}
			} else if (SelectedTreeNodes.Count > 1) {
				List<string> oldNames = new List<string>(treeview.SelectedNodes.Select(x => x.Tag as ElementNode).Select(x => x.Name).ToArray());
				NameGenerator renamer = new NameGenerator(oldNames.ToArray());
				if (renamer.ShowDialog() == DialogResult.OK) {
					for (int i = 0; i < treeview.SelectedNodes.Count; i++) {
						if (i >= renamer.Names.Count) {
							Logging.Warn("ConfigElements: bulk renaming elements, and ran out of new names!");
							break;
						}
						VixenSystem.Nodes.RenameNode((treeview.SelectedNodes[i].Tag as ElementNode), renamer.Names[i]);
					}

					PopulateNodeTree();

					return true;
				}
			}

			return false;
		}


		public void ClearSelectedNodes()
		{
			treeview.ClearSelectedNodes();
		}


		#endregion



		#region Context Menus

		private void contextMenuStripTreeView_Opening(object sender, CancelEventArgs e)
		{
			// temporarily disable Cut function till we can keep the underlying Elements around
			//When this gets fixed enable the keydown event handler as well
			cutNodesToolStripMenuItem.Enabled = false; // (SelectedTreeNodes.Count > 0);
			copyNodesToolStripMenuItem.Enabled = (SelectedTreeNodes.Count > 0);
			pasteNodesToolStripMenuItem.Enabled = (_clipboardNodes != null);
			pasteAsNewToolStripMenuItem.Enabled = (_clipboardNodes != null);
			nodePropertiesToolStripMenuItem.Visible = AllowPropertyEdit;
			copyPropertiesToolStripMenuItem.Enabled = (SelectedTreeNodes.Count == 1);
			pastePropertiesToolStripMenuItem.Enabled = (SelectedTreeNodes.Count > 0) && (_clipboardProperties != null);
			nodePropertiesToolStripMenuItem.Enabled = (SelectedTreeNodes.Count > 0);
			addNewNodeToolStripMenuItem.Enabled = true;
			createGroupWithNodesToolStripMenuItem.Enabled = (SelectedTreeNodes.Count > 0);
			deleteNodesToolStripMenuItem.Enabled = (SelectedTreeNodes.Count > 0);
			renameNodesToolStripMenuItem.Enabled = (SelectedTreeNodes.Count > 0);
			patternRenameToolStripMenuItem.Enabled = (SelectedTreeNodes.Count > 0);
			reverseElementsToolStripMenuItem.Enabled = (SelectedTreeNodes.Count > 1) && (treeview.CanReverseElements());
			sortToolStripMenuItem.Enabled = CanSortSelected();
			exportWireDiagramToolStripMenuItem.Visible = AllowWireExport;
			exportWireDiagramToolStripMenuItem.Enabled = CanExportDiagram();
			exportElementTreeToolStripMenuItem.Enabled = treeview.Nodes.Count > 0;
		}

		private bool CanExportDiagram()
		{
			var canExport = false;
			if (AllowWireExport)
			{
				if (SelectedTreeNodes.Count == 1)
				{
					if (SelectedElementNodes.Any(x => x.Children.Any()))
					{
						canExport = true;
					}
				}
			}
			
			return canExport;
		}

		// TODO: use the system clipboard properly; I couldn't get it working in the sequencer, so I'm not
		// going to bother with it here. If someone feels like playing with it, go ahead. :-)
		private List<ElementNode> _clipboardNodes;
		private List<IPropertyModuleInstance> _clipboardProperties;


		private void cutNodesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CutNodesToClipboard();
		}

		private void CutNodesToClipboard()
		{
			List<ElementNode> cutNodes = new List<ElementNode>();

			foreach (TreeNode treenode in SelectedTreeNodes)
			{
				cutNodes.Add(treenode.Tag as ElementNode);
				DeleteNode(treenode);
			}

			_clipboardNodes = cutNodes;

			OnElementsChanged();
			PopulateNodeTree();
		}

		private void copyNodesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CopyNodesToClipboard();
		}

		private void CopyNodesToClipboard()
		{
			List<ElementNode> copiedNodes = new List<ElementNode>();

			foreach (TreeNode treenode in SelectedTreeNodes)
			{
				copiedNodes.Add(treenode.Tag as ElementNode);
			}

			_clipboardNodes = copiedNodes;
		}

		private void pasteNodesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PasteNodes();
		}

		private void PasteNodes(bool pasteAsNew = false)
		{
			if (_clipboardNodes == null)
				return;

			ElementNode destinationNode = null;
			TreeNode selectedTreeNode = treeview.SelectedNode;

			if (selectedTreeNode != null)
				destinationNode = selectedTreeNode.Tag as ElementNode;
			if (!pasteAsNew)
			{
				IEnumerable<ElementNode> invalidNodesForTarget;
				if (destinationNode == null)
				{
					invalidNodesForTarget = VixenSystem.Nodes.InvalidRootNodes;
				}
				else
				{
					invalidNodesForTarget = destinationNode.InvalidChildren();
				}
				IEnumerable<ElementNode> invalidSourceNodes = invalidNodesForTarget.Intersect(_clipboardNodes);
				if (invalidSourceNodes.Any())
				{
					SystemSounds.Hand.Play();
					return;
				}
			}
			else
			{
				if (destinationNode != null && destinationNode.IsLeaf)
				{
					SystemSounds.Hand.Play();
					return;
				}
			}
			
			// Check to see if the new parent node would be 'losing' the Element (ie. becoming a
			// group instead of a leaf node with a element/patches). Prompt the user first.
			if (CheckAndPromptIfNodeWillLosePatches(destinationNode))
				return;

			foreach (ElementNode cn in _clipboardNodes)
			{
				if (pasteAsNew)
				{
					DuplicateNodes(cn, destinationNode);
				}
				else
				{
					VixenSystem.Nodes.AddChildToParent(cn, destinationNode);
				}
			}

			if (selectedTreeNode != null)
				selectedTreeNode.Expand();

			PopulateNodeTree();
			OnElementsChanged();
			
		}

		private void pasteNodesAsNewToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PasteNodes(true);

		}

		public void DuplicateNodes(ElementNode node, ElementNode parent = null)
		{
			//We need to make a map to ensure we can map all existing nodes back.
			Dictionary<Guid, ElementNode> leafNodeMap = new Dictionary<Guid, ElementNode>();

			//Create a new top level node
			ElementNode newNode = ElementNodeService.Instance.CreateSingle(parent, node.Name,node.IsLeaf);
			if (!node.IsLeaf)
			{
				DuplicateChildNodes(node, newNode, leafNodeMap);
			}

		}

		private void DuplicateChildNodes(ElementNode node, ElementNode newNode, Dictionary<Guid, ElementNode> leafNodeMap)
		{
			foreach (ElementNode childNode in node.Children)
			{
				if (childNode.IsLeaf)
				{
					if (leafNodeMap.ContainsKey(childNode.Id))
					{
						VixenSystem.Nodes.AddChildToParent(leafNodeMap[childNode.Id],newNode);
					}
					else
					{
						ElementNode newChild = ElementNodeService.Instance.CreateSingle(newNode, childNode.Name);
						leafNodeMap.Add(childNode.Id, newChild);
					}
				}
				else
				{
					ElementNode newChild = ElementNodeService.Instance.CreateSingle(newNode, childNode.Name, false);
					if (childNode.Children.Any())
					{
						DuplicateChildNodes(childNode, newChild, leafNodeMap);
					}
				}
			}
		}

		private void copyPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (SelectedTreeNodes.Count != 1)
				return;

			ElementNode sourceNode = SelectedNode;
			_clipboardProperties = new List<IPropertyModuleInstance>();
			foreach (IPropertyModuleInstance property in sourceNode.Properties) {
				_clipboardProperties.Add(property);
			}
		}

		private void pastePropertiesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_clipboardProperties == null)
				return;

			foreach (ElementNode en in SelectedElementNodes) {
				foreach (IPropertyModuleInstance sourceProperty in _clipboardProperties) {
					IPropertyModuleInstance destinationProperty;

					if (en.Properties.Contains(sourceProperty.Descriptor.TypeId)) {
						destinationProperty = en.Properties.Get(sourceProperty.Descriptor.TypeId);
					}
					else {
						destinationProperty = en.Properties.Add(sourceProperty.Descriptor.TypeId);
					}

					if (destinationProperty == null) {
						Logging.Error("ConfigElements: pasting a property to a element, but can't make or find the instance!");
						continue;
					}

					// get the property to do its best to copy values from the property we're copying from.
					destinationProperty.CloneValues(sourceProperty);
				}
			}

			OnElementsChanged();
		}

		private void addNewNodeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var added = AddSingleNodeWithPrompt(SelectedNode);
			if( added != null)
				OnElementsChanged();
		}

		private void addMultipleNewNodesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var added = AddMultipleNodesWithPrompt(SelectedNode);
			if( added != null)
				OnElementsChanged();
		}

		private void deleteNodesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (SelectedTreeNodes.Count == 0)
				return;

			foreach (TreeNode tn in SelectedTreeNodes) {
				DeleteNode(tn);
			}

			PopulateNodeTree();
			OnElementsChanged();
		}

		private void createGroupWithNodesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool bChanged = CreateGroupFromSelectedNodes();
			if( bChanged)
				OnElementsChanged();
		}

		private void renameNodesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (RenameSelectedElements()) {
				OnElementsChanged();
			}
		}

		private void patternRenameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PatternRenameSelectedGroup();
		}

		private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			treeview.CollapseAll();
		}

		private void reverseElementsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			
			for (int i = 1; i < SelectedTreeNodes.Count; i++)
			{
				ElementNode sourceNode = SelectedTreeNodes[i].Tag as ElementNode;
				ElementNode parentNode = (SelectedTreeNodes[i].Parent != null) ? SelectedTreeNodes[i].Parent.Tag as ElementNode : null;
				VixenSystem.Nodes.MoveNode(sourceNode, parentNode, parentNode, SelectedTreeNodes[0].Index);
			}
						
			PopulateNodeTree();
		}

		private void sortToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SortNodes();
		}
		private void exportWireDiagramToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ExportDiagram?.Invoke(SelectedElementNodes.FirstOrDefault());
		}

		private async void ExportElementTreeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var saveFileDialog = new SaveFileDialog())
			{
				saveFileDialog.OverwritePrompt = true;
				saveFileDialog.DefaultExt = ".v3m";
				saveFileDialog.Filter = @"Vixen 3 Element Nodes (*.v3e)|*.v3e";
				saveFileDialog.InitialDirectory = SequenceService.SequenceDirectory;
				var result = saveFileDialog.ShowDialog(Parent);
				if (result == DialogResult.OK)
				{
					VixenSystem.Nodes.ExportElementNodeProxy(saveFileDialog.FileName);
				}
			}
		}

		private bool CanSortSelected()
		{
			if (SelectedTreeNodes.Count != 1) return false;
			var sourceNode = SelectedTreeNodes[0].Tag as ElementNode;
			if (sourceNode != null && !sourceNode.IsLeaf) return true;

			return false;
		}

		private void SortNodes()
		{
			if (SelectedTreeNodes.Count != 1) return;
			var sourceNode = SelectedTreeNodes[0].Tag as ElementNode;
			if (sourceNode != null && !sourceNode.IsLeaf)
			{
				var nodes = sourceNode.Children.OrderBy(n => n.Name);
				var index = 0;
				foreach (var node in nodes)
				{
					VixenSystem.Nodes.MoveNode(node, sourceNode, sourceNode, index);
					index++;
				}

				PopulateNodeTree();
			}
		}


		#endregion



		private void treeview_KeyDown(object sender, KeyEventArgs e)
		{
			// do our own deleting of items here
			if (e.KeyCode == Keys.Delete)
			{
				if (SelectedTreeNodes.Count > 0)
				{
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Exclamation;
						//this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("Delete selected items?",
						"Delete items", true, false);
					messageBox.ShowDialog();
					if (messageBox.DialogResult == DialogResult.OK)
					{
						foreach (TreeNode tn in SelectedTreeNodes)
						{
							DeleteNode(tn);
						}

						PopulateNodeTree();
						OnElementsChanged();
					}
				}
			}
			else if(e.KeyCode == Keys.C && e.Control)
			{
				CopyNodesToClipboard();
				e.SuppressKeyPress = true;
			}
			else if (e.KeyCode == Keys.V && e.Control && e.Shift)
			{
				PasteNodes(true);
				e.SuppressKeyPress = true;
			}
			else if (e.KeyCode == Keys.V && e.Control)
			{
				PasteNodes();
				e.SuppressKeyPress = true;
			}
			// temporarily disable Cut function till we can keep the underlying Elements around
			//else if (e.KeyCode == Keys.X && e.Control)
			//{
			//	CutNodesToClipboard();
			//	e.SuppressKeyPress = true;
			//}
		}

	}
}
