using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Services;
using Vixen.Sys;
using System.Media;
using Vixen.Module.Property;
using System.Reflection;
using Common.Controls;
using Vixen.Sys.Output;

namespace VixenApplication
{
	public partial class ConfigElements : Form
	{
		private ElementNode _displayedNode;
		private bool _changesMade;

		// sets of data to keep track of which items in the treeview are open, selected, visible etc.
		// so that when we reload the tree, we can keep it looking relatively consistent with what the
		// user had before.
		private HashSet<string> _expandedNodes;		// TreeNode paths that are expanded
		private HashSet<string> _selectedNodes;		// TreeNode paths that are selected
		private List<string> _topDisplayedNodes;	// TreeNode paths that are at the top of the view. Should only
													// need one, but will have multiple in case the top node is deleted.
		private ToolTip _tooltip;

		public ConfigElements()
		{
			InitializeComponent();
			_displayedNode = null;
			_tooltip = new ToolTip();

			multiSelectTreeviewElementsGroups.DragFinishing += MultiSelectTreeviewElementsGroupsDragFinishingHandler;
			multiSelectTreeviewElementsGroups.DragOverVerify += MultiSelectTreeviewElementsGroupsDragVerifyHandler;
			multiSelectTreeviewElementsGroups.Deselected += MultiSelectTreeviewElementsGroupsDeselectedHandler;
		}

		private void ConfigElements_Load(object sender, EventArgs e)
		{
			PopulateNodeTree();
			PopulateFormWithNode(null, true);
		}


		#region Node Tree population & display
		private void PopulateNodeTree()
		{
			// save metadata that is currently in the treeview
			_expandedNodes = new HashSet<string>();
			_selectedNodes = new HashSet<string>();
			_topDisplayedNodes = new List<string>();

			SaveTreeNodeState(multiSelectTreeviewElementsGroups.Nodes);
			SaveTreeNodeTopVisible();

			// clear the treeview, and repopulate it
			multiSelectTreeviewElementsGroups.BeginUpdate();
			multiSelectTreeviewElementsGroups.Nodes.Clear();

			foreach(ElementNode element in VixenSystem.Nodes.GetRootNodes()) {
				AddNodeToTree(multiSelectTreeviewElementsGroups.Nodes, element);
			}

			// go through all the data we saved, and try to update the treeview to look
			// like it used to (expanded nodes, selected nodes, node at the top)

			foreach (string node in _expandedNodes) {
				TreeNode resultNode = FindNodeInTreeAtPath(multiSelectTreeviewElementsGroups, node);

				if (resultNode != null) {
					resultNode.Expand();
				}
			}

			foreach (string node in _selectedNodes) {
				TreeNode resultNode = FindNodeInTreeAtPath(multiSelectTreeviewElementsGroups, node);

				if (resultNode != null) {
					multiSelectTreeviewElementsGroups.AddSelectedNode(resultNode);
				}
			}

			multiSelectTreeviewElementsGroups.EndUpdate();

			// see stackoverflow.com/questions/626315/winforms-listview-remembering-scrolled-location-on-reload .
			// we can only set the topNode after EndUpdate(). Also, it might throw an exception -- weird?
			foreach (string node in _topDisplayedNodes) {
				TreeNode resultNode = FindNodeInTreeAtPath(multiSelectTreeviewElementsGroups, node);

				if (resultNode != null) {
					try {
						multiSelectTreeviewElementsGroups.TopNode = resultNode;
					} catch (Exception) {
						VixenSystem.Logging.Warning("ConfigElements: exception caught trying to set TopNode.");
					}
					break;
				}
			}
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
				if (tn.IsExpanded) {
					_expandedNodes.Add(GenerateTreeNodeFullPath(tn, multiSelectTreeviewElementsGroups.PathSeparator));
				}

				if (multiSelectTreeviewElementsGroups.SelectedNodes.Contains(tn)) {
					_selectedNodes.Add(GenerateTreeNodeFullPath(tn, multiSelectTreeviewElementsGroups.PathSeparator));
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
			if (multiSelectTreeviewElementsGroups.Nodes.Count > 0) {
				TreeNode current = multiSelectTreeviewElementsGroups.TopNode;
				while (current != null) {
					_topDisplayedNodes.Add(GenerateTreeNodeFullPath(current, multiSelectTreeviewElementsGroups.PathSeparator));
					current = current.NextNode;
				}
			}
		}

		private void AddNodeToTree(TreeNodeCollection collection, ElementNode elementNode)
		{
			TreeNode addedNode = new TreeNode();
			addedNode.Name = elementNode.Id.ToString();
			addedNode.Text = elementNode.Name;
			addedNode.Tag = elementNode;

			if (!elementNode.Children.Any()) {
				if (elementNode.Element != null && VixenSystem.DataFlow.GetChildren(VixenSystem.Elements.GetDataFlowComponentForElement(elementNode.Element)).Any()) {
					if (elementNode.Element.Masked)
						addedNode.ImageKey = addedNode.SelectedImageKey = "RedBall";
					else
						addedNode.ImageKey = addedNode.SelectedImageKey = "GreenBall";
				} else
					addedNode.ImageKey = addedNode.SelectedImageKey = "WhiteBall";
			} else {
				addedNode.ImageKey = addedNode.SelectedImageKey = "Group";
			}

			collection.Add(addedNode);

			foreach (ElementNode childNode in elementNode.Children) {
				AddNodeToTree(addedNode.Nodes, childNode);
			}
		}
		#endregion


		#region Form controls population & display

		private void PopulateFormWithNode(ElementNode node, bool forceUpdate)
		{
			if (node == _displayedNode && !forceUpdate)
				return;

			_displayedNode = node;

			PopulateGeneralNodeInfo(node);
			PopulatePropertiesArea(node);

			groupBoxSelectedNode.Enabled = (node != null);

			buttonDeleteElement.Enabled = (multiSelectTreeviewElementsGroups.SelectedNodes.Count > 0) && (node != null);
			buttonCreateGroup.Enabled = (multiSelectTreeviewElementsGroups.SelectedNodes.Count > 0) && (node != null);
			buttonRename.Enabled = (multiSelectTreeviewElementsGroups.SelectedNodes.Count > 0) && (node != null);
		}

		private void PopulateGeneralNodeInfo(ElementNode node)
		{
			if (node == null) {
				labelParents.Text = "";
				_tooltip.SetToolTip(labelParents, null);
				textBoxName.Text = "";
			} else {
				// update the label with parent info about the node. Any good suggestions or improvements for this?
				int parentCount = GetNodeParentGroupCount(node);
				List<string> parents = GetNodeParentGroupNames(node);
				string labelString = "", tooltipString = "";
				labelString = String.Format("This element is in {0} group{1}{2}", parentCount, ((parentCount != 1) ? "s" : ""), ((parentCount == 0) ? "." : ": "));
				tooltipString = labelString + "\r\n\r\n";
				foreach (string p in parents)
				{
					labelString = String.Format("{0}{1}, ", labelString, p);
					tooltipString = String.Format("{0}{1}\r\n", tooltipString, p);
				}
				labelParents.Text = labelString.TrimEnd(new char[] { ' ', ',' });
				tooltipString = tooltipString.TrimEnd(new char[] { '\r', '\n' });
				if (labelString.Length > 100) {
					_tooltip.SetToolTip(labelParents, tooltipString);
				} else {
					_tooltip.SetToolTip(labelParents, null);
				}

				textBoxName.Text = node.Name;
			}
		}

		private void PopulatePropertiesArea(ElementNode node)
		{
			listViewProperties.BeginUpdate();
			listViewProperties.Items.Clear();
			if (node != null) {
				foreach (IPropertyModuleInstance property in node.Properties) {
					ListViewItem item = new ListViewItem();
					item.Text = property.Descriptor.TypeName;
					item.Tag = property;
					listViewProperties.Items.Add(item);
				}

				listViewProperties.SelectedItems.Clear();
			}
			listViewProperties.EndUpdate();

			PopulatePropertiesButtons();
		}

		private void PopulatePropertiesButtons()
		{
			buttonConfigureProperty.Enabled = (listViewProperties.SelectedItems.Count == 1);
			buttonDeleteProperty.Enabled = (listViewProperties.SelectedItems.Count > 0);
		}

		#endregion


		#region Form buttons

		private void buttonAddElement_Click(object sender, EventArgs e)
		{
			multiSelectTreeviewElementsGroups.ClearSelectedNodes();
			AddSingleNodeWithPrompt();
			_changesMade = true;
		}

		private void buttonAddMultipleElements_Click(object sender, EventArgs e)
		{
			multiSelectTreeviewElementsGroups.ClearSelectedNodes();
			AddMultipleNodesWithPrompt();
			_changesMade = true;
		}

		private void buttonDeleteElement_Click(object sender, EventArgs e)
		{
			if (multiSelectTreeviewElementsGroups.SelectedNodes.Count > 0)
			{
				string message, title;
				if (multiSelectTreeviewElementsGroups.SelectedNodes.Count > 1) {
					message = "Are you sure you want to delete the selected elements?";
					title = "Delete Items?";
				} else {
					message = "Are you sure you want to delete the selected element?";
					title = "Delete Item?";
				}
				if (MessageBox.Show(message, title, MessageBoxButtons.OKCancel) == DialogResult.OK) {
					foreach (TreeNode tn in multiSelectTreeviewElementsGroups.SelectedNodes) {
						DeleteNode(tn);
					}
					_changesMade = true;
				}
			}

			PopulateNodeTree();
			PopulateFormWithNode(null, true);
		}

		private void buttonCreateGroup_Click(object sender, EventArgs e)
		{
			CreateGroupFromSelectedNodes();
			_changesMade = true;
		}

		private void buttonRename_Click(object sender, EventArgs e)
		{
			RenameSelectedElements();
			_changesMade = true;
		}

		private void buttonAddProperty_Click(object sender, EventArgs e)
		{
			List<KeyValuePair<string, object>> properties = new List<KeyValuePair<string, object>>();
			foreach (KeyValuePair<Guid, string> kvp in ApplicationServices.GetAvailableModules<IPropertyModuleInstance>()) {
				properties.Add(new KeyValuePair<string, object>(kvp.Value, kvp.Key));
			}
			using (ListSelectDialog addForm = new ListSelectDialog("Add Property", (properties)))
			{
				if (addForm.ShowDialog() == DialogResult.OK)
				{
					_displayedNode.Properties.Add((Guid)addForm.SelectedItem);
					PopulatePropertiesArea(_displayedNode);

					_changesMade = true;
				}
			}
		}

		private void buttonConfigureProperty_Click(object sender, EventArgs e)
		{
			ConfigureSelectedProperty();
			_changesMade = true;
		}

		private void buttonDeleteProperty_Click(object sender, EventArgs e)
		{
			if (listViewProperties.SelectedItems.Count > 0) {
				string message, title;
				if (listViewProperties.SelectedItems.Count == 1) {
					message = "Are you sure you want to remove the selected property from the element?";
					title = "Remove Property?";
				} else {
					message = "Are you sure you want to remove the selected properties from the element?";
					title = "Remove Properties?";
				}
				if (MessageBox.Show(message, title, MessageBoxButtons.OKCancel) == DialogResult.OK) {
					foreach (ListViewItem item in listViewProperties.SelectedItems) {
						_displayedNode.Properties.Remove((item.Tag as IPropertyModuleInstance).Descriptor.TypeId);
					}

					PopulatePropertiesArea(_displayedNode);
					_changesMade = true;
				}
			}
		}

		#endregion


		#region Events

		private void multiSelectTreeviewElementsGroups_AfterSelect(object sender, TreeViewEventArgs e)
		{
			PopulateFormWithNode(multiSelectTreeviewElementsGroups.SelectedNode.Tag as ElementNode, false);
		}

		private void listViewProperties_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			ConfigureSelectedProperty();
		}

		private void listViewProperties_SelectedIndexChanged(object sender, EventArgs e)
		{
			PopulatePropertiesButtons();
		}

		private void MultiSelectTreeviewElementsGroupsDeselectedHandler(object sender, EventArgs e)
		{
			PopulateFormWithNode(null, false);
		}

		#endregion


		#region Drag 'n' Drop Handlers

		private void MultiSelectTreeviewElementsGroupsDragFinishingHandler(object sender, DragFinishingEventArgs e)
		{
			// we want to finish off the drag ourselves, and not have the treeview control move the nodes around.
			// (In fact, in a lame attempt at 'data binding', we're going to completely redraw the tree after
			// making all the required changes from this drag-drop.) So set the flag to indicate that.
			e.FinishDrag = false;

			// first determine the node that they will be moved to. This will depend on if we are dragging onto a node
			// directly, or above/below one to reorder.
			ElementNode newParentNode = null;						// the ElementNode that the selected items will move to
			TreeNode expandNode = null;								// if we need to expand a node once we've moved everything
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
					newParentNode = null;	// needs to go at the root level
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
			if (CheckIfNodeWillLosePatches(newParentNode))
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
						if (newParentNode == oldParentNode && index < currentIndex)
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
					VixenSystem.Logging.Warning("ConfigElements: Trying to deal with a drag that is an unknown type!");
				}
			}

			if (expandNode != null)
				expandNode.Expand();
			
			PopulateNodeTree();
			PopulateFormWithNode((multiSelectTreeviewElementsGroups.SelectedNode == null) ? null : (multiSelectTreeviewElementsGroups.SelectedNode.Tag as ElementNode), true);
		}

		private void MultiSelectTreeviewElementsGroupsDragVerifyHandler(object sender, DragVerifyEventArgs e)
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

			if (e.DragBetweenNodes == DragBetweenNodes.DragOnTargetNode || e.DragBetweenNodes == DragBetweenNodes.DragBelowTargetNode && e.TargetNode.IsExpanded) {
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

			if ((e.KeyState & 8) != 0) {		// the CTRL key
				e.DragMode = DragDropEffects.Copy;
				permittedNodesForTarget = new HashSet<ElementNode>();
			} else {
				e.DragMode = DragDropEffects.Move;
			}

			IEnumerable<ElementNode> invalidSourceNodes = invalidNodesForTarget.Intersect(nodes);
			if (invalidSourceNodes.Count() > 0) {
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

		private void ConfigureSelectedProperty()
		{
			if (listViewProperties.SelectedItems.Count == 1) {
				(listViewProperties.SelectedItems[0].Tag as IPropertyModuleInstance).Setup();
			}
		}

		private int GetNodeParentGroupCount(ElementNode node)
		{
			int count = node.Parents.Count();
			if (VixenSystem.Nodes.GetRootNodes().Contains(node))
				count--;
			return count;
		}

		private List<string> GetNodeParentGroupNames(ElementNode node, int maxNames = Int32.MaxValue)
		{
			List<string> result = new List<string>();
			foreach (ElementNode parent in node.Parents) {
				if (maxNames <= 0) {
					break;
				}
				if (parent.Name != "Root") {
					result.Add(parent.Name);
				}
				maxNames--;
			}

			return result;
		}

		private void DeleteNode(TreeNode tn)
		{
			ElementNode cn = tn.Tag as ElementNode;
			ElementNode parent = (tn.Parent != null) ? tn.Parent.Tag as ElementNode : null;
			VixenSystem.Nodes.RemoveNode(cn, parent, true);
			if (_displayedNode == cn) {
				_displayedNode = null;
			}
		}


		private IEnumerable<ElementNode> AddMultipleNodesWithPrompt(ElementNode parent = null)
		{
			List<ElementNode> result = new List<ElementNode>();

			// since we're adding multiple nodes, prompt with the name generation form (which also includes a counter on there).
			using (NameGenerator nameGenerator = new NameGenerator()) {
				if (nameGenerator.ShowDialog() == DialogResult.OK) {
					result.AddRange(nameGenerator.Names.Where(name => !string.IsNullOrEmpty(name)).Select(name => AddNewNode(name, false, parent, true)));
					PopulateNodeTree();
				}
			}

			return result;
		}

		private ElementNode AddSingleNodeWithPrompt(ElementNode parent = null)
		{
			// since we're only adding a single node, prompt with a single text dialog.
			using (TextDialog textDialog = new TextDialog("Element Name?")) {
				if (textDialog.ShowDialog() == DialogResult.OK) {
					string newName;
					if (textDialog.Response == "")
						newName = "New Element";
					else
						newName = textDialog.Response;

					return AddNewNode(newName, true, parent);
				}
			}

			return null;
		}


		private ElementNode AddNewNode(string nodeName, bool repopulateNodeTree = true, ElementNode parent = null, bool skipPatchCheck = false)
		{
			// prompt the user if it's going to make a patched leaf a group; if they abandon it, return null
			if (!skipPatchCheck && CheckIfNodeWillLosePatches(parent))
				return null;

			ElementNode newNode = ElementNodeService.Instance.CreateSingle(parent, nodeName, true);
			if (repopulateNodeTree)
				PopulateNodeTree();
			return newNode;
		}

		private void CreateGroupFromSelectedNodes()
		{
			ElementNode newGroup = AddSingleNodeWithPrompt();

			foreach (TreeNode tn in multiSelectTreeviewElementsGroups.SelectedNodes) {
				VixenSystem.Nodes.AddChildToParent(tn.Tag as ElementNode, newGroup);
			}

			PopulateNodeTree();
		}

		private bool CheckIfNodeWillLosePatches(ElementNode node)
		{
			if (node != null && node.Element != null) {
				if (VixenSystem.DataFlow.GetChildren(VixenSystem.Elements.GetDataFlowComponentForElement(node.Element)).Any()) {
					string message = "Adding items to this element will convert it into a Group, which will remove any " +
						"patches it may have. Are you sure you want to continue?";
					string title = "Convert Element to Group?";
					DialogResult result = MessageBox.Show(message, title, MessageBoxButtons.YesNoCancel);
					if (result != DialogResult.Yes) {
						return true;
					}
				}
			}

			return false;
		}

		private void RenameSelectedElements()
		{
			if (multiSelectTreeviewElementsGroups.SelectedNodes.Count > 0) {
				List<string> oldNames = new List<string>(multiSelectTreeviewElementsGroups.SelectedNodes.Select(x => x.Tag as ElementNode).Select(x => x.Name).ToArray());
				NameGenerator renamer = new NameGenerator(oldNames.ToArray());
				if (renamer.ShowDialog() == DialogResult.OK) {
					for (int i = 0; i < multiSelectTreeviewElementsGroups.SelectedNodes.Count; i++) {
						if (i >= renamer.Names.Count) {
							VixenSystem.Logging.Warning("ConfigElements: bulk renaming elements, and ran out of new names!");
							break;
						}
						(multiSelectTreeviewElementsGroups.SelectedNodes[i].Tag as ElementNode).Name = renamer.Names[i];
					}

					PopulateNodeTree();
					PopulateFormWithNode(_displayedNode, true);
				}
			}
		}

		#endregion


		#region Context Menus

		private void contextMenuStripTreeView_Opening(object sender, CancelEventArgs e)
		{
			cutNodesToolStripMenuItem.Enabled = (multiSelectTreeviewElementsGroups.SelectedNodes.Count > 0);
			copyNodesToolStripMenuItem.Enabled = (multiSelectTreeviewElementsGroups.SelectedNodes.Count > 0);
			pasteNodesToolStripMenuItem.Enabled = (_clipboardNodes != null);
			copyPropertiesToolStripMenuItem.Enabled = (multiSelectTreeviewElementsGroups.SelectedNodes.Count == 1);
			pastePropertiesToolStripMenuItem.Enabled = (multiSelectTreeviewElementsGroups.SelectedNodes.Count > 0) && (_clipboardProperties != null);
			nodePropertiesToolStripMenuItem.Enabled = (multiSelectTreeviewElementsGroups.SelectedNodes.Count > 0);
			addNewNodeToolStripMenuItem.Enabled = true;
			createGroupWithNodesToolStripMenuItem.Enabled = (multiSelectTreeviewElementsGroups.SelectedNodes.Count > 0);
			deleteNodesToolStripMenuItem.Enabled = (multiSelectTreeviewElementsGroups.SelectedNodes.Count > 0);
			renameNodesToolStripMenuItem.Enabled = (multiSelectTreeviewElementsGroups.SelectedNodes.Count > 0);
		}

		// TODO: use the system clipboard properly; I couldn't get it working in the sequencer, so I'm not
		// going to bother with it here. If someone feels like playing with it, go ahead. :-)
		private List<ElementNode> _clipboardNodes;
		private List<IPropertyModuleInstance> _clipboardProperties;


		private void cutNodesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<ElementNode> cutNodes = new List<ElementNode>();

			foreach (TreeNode treenode in multiSelectTreeviewElementsGroups.SelectedNodes) {
				cutNodes.Add(treenode.Tag as ElementNode);
				DeleteNode(treenode);
			}

			_clipboardNodes = cutNodes;

			PopulateNodeTree();
		}

		private void copyNodesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<ElementNode> copiedNodes = new List<ElementNode>();

			foreach (TreeNode treenode in multiSelectTreeviewElementsGroups.SelectedNodes) {
				copiedNodes.Add(treenode.Tag as ElementNode);
			}

			_clipboardNodes = copiedNodes;
		}

		private void pasteNodesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_clipboardNodes == null)
				return;

			ElementNode destinationNode = null;
			TreeNode selectedTreeNode = multiSelectTreeviewElementsGroups.SelectedNode;

			if (selectedTreeNode != null)
				destinationNode = selectedTreeNode.Tag as ElementNode;

			IEnumerable<ElementNode> invalidNodesForTarget;
			if (destinationNode == null)
				invalidNodesForTarget = VixenSystem.Nodes.InvalidRootNodes;
			else
				invalidNodesForTarget = destinationNode.InvalidChildren();

			IEnumerable<ElementNode> invalidSourceNodes = invalidNodesForTarget.Intersect(_clipboardNodes);
			if (invalidSourceNodes.Count() > 0) {
				SystemSounds.Asterisk.Play();
			} else {
				// Check to see if the new parent node would be 'losing' the Element (ie. becoming a
				// group instead of a leaf node with a element/patches). Prompt the user first.
				if (CheckIfNodeWillLosePatches(destinationNode))
					return;

				foreach (ElementNode cn in _clipboardNodes) {
					VixenSystem.Nodes.AddChildToParent(cn, destinationNode);
				}

				if (selectedTreeNode != null)
					selectedTreeNode.Expand();

				PopulateNodeTree();
				PopulateFormWithNode(destinationNode, true);
			}
		}

		private void copyPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (multiSelectTreeviewElementsGroups.SelectedNodes.Count != 1)
				return;

			ElementNode sourceNode = multiSelectTreeviewElementsGroups.SelectedNode.Tag as ElementNode;
			_clipboardProperties = new List<IPropertyModuleInstance>();
			foreach (IPropertyModuleInstance property in sourceNode.Properties) {
				_clipboardProperties.Add(property);
			}
		}

		private void pastePropertiesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_clipboardProperties == null)
				return;

			foreach (TreeNode tn in multiSelectTreeviewElementsGroups.SelectedNodes) {
				ElementNode element = tn.Tag as ElementNode;

				foreach (IPropertyModuleInstance sourceProperty in _clipboardProperties) {
					IPropertyModuleInstance destinationProperty;

					if (element.Properties.Contains(sourceProperty.Descriptor.TypeId)) {
						destinationProperty = element.Properties.Get(sourceProperty.Descriptor.TypeId);
					} else {
						destinationProperty = element.Properties.Add(sourceProperty.Descriptor.TypeId);
					}

					if (destinationProperty == null) {
						VixenSystem.Logging.Error("ConfigElements: pasting a property to a element, but can't make or find the instance!");
						continue;
					}

					// get the property to do its best to copy values from the property we're copying from.
					destinationProperty.CloneValues(sourceProperty);
				}
			}

			PopulateFormWithNode(_displayedNode, true);
		}

		private void addNewNodeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode selectedTreeNode = multiSelectTreeviewElementsGroups.SelectedNode;
			if (selectedTreeNode == null)
				AddSingleNodeWithPrompt();
			else
				AddSingleNodeWithPrompt(selectedTreeNode.Tag as ElementNode);

			PopulateFormWithNode(_displayedNode, true);
		}

		private void addMultipleNewNodesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TreeNode selectedTreeNode = multiSelectTreeviewElementsGroups.SelectedNode;
			if (selectedTreeNode == null)
				AddMultipleNodesWithPrompt();
			else
				AddMultipleNodesWithPrompt(selectedTreeNode.Tag as ElementNode);

			PopulateFormWithNode(_displayedNode, true);
		}

		private void deleteNodesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (multiSelectTreeviewElementsGroups.SelectedNodes.Count == 0)
				return;

			foreach (TreeNode tn in multiSelectTreeviewElementsGroups.SelectedNodes) {
				DeleteNode(tn);
			}

			PopulateNodeTree();
			PopulateFormWithNode(_displayedNode, true);
		}

		private void createGroupWithNodesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CreateGroupFromSelectedNodes();
			PopulateFormWithNode(_displayedNode, true);
		}

		private void renameNodesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (multiSelectTreeviewElementsGroups.SelectedNodes.Count == 0)
				return;
			else if (multiSelectTreeviewElementsGroups.SelectedNodes.Count == 1) {
				ElementNode cn = multiSelectTreeviewElementsGroups.SelectedNode.Tag as ElementNode;
				using (TextDialog dialog = new TextDialog("Item name?", "Rename item", (cn).Name, true))
				{
					if (dialog.ShowDialog() == DialogResult.OK)
					{
						if (dialog.Response != "" && dialog.Response != cn.Name)
							VixenSystem.Nodes.RenameNode(cn, dialog.Response);
					}
				}
			}
			else if (multiSelectTreeviewElementsGroups.SelectedNodes.Count > 1)
			{
				RenameSelectedElements();
			}

			PopulateNodeTree();
			PopulateFormWithNode(_displayedNode, true);
		}

		#endregion

		private void textBoxName_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter) {
				string newName = textBoxName.Text.Trim();
				if (newName != "" && newName != _displayedNode.Name) {
					VixenSystem.Nodes.RenameNode(_displayedNode, newName);
					PopulateNodeTree();
				}
			}
		}

		private void multiSelectTreeviewElementsGroups_KeyDown(object sender, KeyEventArgs e)
		{
			// do our own deleting of items here
			if (e.KeyCode == Keys.Delete) {
				if (multiSelectTreeviewElementsGroups.SelectedNodes.Count > 0) {
					if (MessageBox.Show("Delete selected items?", "Delete items", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Yes) {
						foreach (TreeNode tn in multiSelectTreeviewElementsGroups.SelectedNodes) {
							DeleteNode(tn);
						}

						PopulateNodeTree();
						PopulateFormWithNode(_displayedNode, true);
					}
				}
			}
		}

		private void ConfigElements_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (_changesMade)
			{
				if (DialogResult == DialogResult.Cancel)
				{
					switch (MessageBox.Show(this, "All changes will be lost if you continue, do you wish to continue?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
					{
						case DialogResult.No:
							e.Cancel = true;
							break;
						default:
							break;
					}
				}
				else if (DialogResult == DialogResult.OK)
				{
					e.Cancel = false;
				}
				else
				{
					switch (e.CloseReason)
					{
						case CloseReason.UserClosing:
							e.Cancel = true;
							break;
					}
				}
			}
		}

        private void megaTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //TreeNode selectedTreeNode = multiSelectTreeviewElementsGroups.SelectedNode;
            //if (selectedTreeNode == null)
            //    AddSingleNodeWithPrompt();
            //else
            //    AddSingleNodeWithPrompt(selectedTreeNode.Tag as ElementNode);


            TreeNode selectedTreeNode = multiSelectTreeviewElementsGroups.SelectedNode;
            ElementNode selectedNode = null;
            if (selectedTreeNode != null && selectedTreeNode.Tag as ElementNode != null)
                selectedNode = selectedTreeNode.Tag as ElementNode;
            
            ConfigureElements.AddMegatree f = new ConfigureElements.AddMegatree();
            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //if (selectedTreeNode != null && selectedTreeNode.Tag as ElementNode != null) 
                //{
                //    selectedNode = selectedTreeNode.Tag as ElementNode;
                //}
                //Console.WriteLine(selectedNode.Name);
                ElementNode treeParent = AddNewNode(f.TreeName, false, selectedNode, false);

                for (int stringNum = 0; stringNum < f.StringCount; stringNum++) {
                    ElementNode treeString = AddNewNode(f.TreeName + " String " + (stringNum+1).ToString(), false, treeParent, false);
                    for (int pixelNum = 0; pixelNum < f.PixelsPerString; pixelNum++)
                    {
                        AddNewNode(treeString.Name + "-" + (pixelNum+1).ToString(), false, treeString, false);
                    }
                }

                PopulateNodeTree();
            }
        }
	}

	public class ComboBoxControllerItem
	{
		public string Name { get; set; }
		public Guid Id { get; set; }
	}
}
