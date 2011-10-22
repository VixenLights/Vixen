using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;
using Vixen.Module.Property;
using System.Reflection;
using CommonElements;

namespace VixenApplication
{
	public partial class ConfigChannels : Form
	{
		private ChannelNode _displayedNode;
		private bool _displayedNodeInitialized;

		// sets of data to keep track of which items in the treeview are open, selected, visible etc.
		// so that when we reload the tree, we can keep it looking relatively consistent with what the
		// user had before.
		private HashSet<string> _expandedNodes;		// TreeNode paths that are expanded
		private HashSet<string> _selectedNodes;		// TreeNode paths that are selected
		private List<string> _topDisplayedNodes;	// TreeNode paths that are at the top of the view. Should only
													// need one, but will have multiple in case the top node is deleted.
		private ToolTip _tooltip;

		public ConfigChannels()
		{
			InitializeComponent();
			_displayedNode = null;
			_displayedNodeInitialized = false;
			_tooltip = new ToolTip();

			multiSelectTreeviewChannelsGroups.DragFinishing += multiSelectTreeviewChannelsGroupsDragFinishingHandler;
			multiSelectTreeviewChannelsGroups.DragOverVerify += multiSelectTreeviewChannelsGroupsDragVerifyHandler;

			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		}

		private void ConfigChannels_Load(object sender, EventArgs e)
		{
			PopulateNodeTree();
		}


		#region Node Tree population & display
		private void PopulateNodeTree()
		{
			// save metadata that is currently in the treeview
			_expandedNodes = new HashSet<string>();
			_selectedNodes = new HashSet<string>();
			_topDisplayedNodes = new List<string>();

			SaveTreeNodeState(multiSelectTreeviewChannelsGroups.Nodes);
			SaveTreeNodeTopVisible();

			// clear the treeview, and repopulate it
			multiSelectTreeviewChannelsGroups.BeginUpdate();
			multiSelectTreeviewChannelsGroups.Nodes.Clear();

			foreach(ChannelNode channel in VixenSystem.Nodes.RootNodes) {
				AddNodeToTree(multiSelectTreeviewChannelsGroups.Nodes, channel);
			}

			// go through all the data we saved, and try to update the treeview to look
			// like it used to (expanded nodes, selected nodes, node at the top)

			foreach (string node in _expandedNodes) {
				TreeNode resultNode = FindNodeInTreeAtPath(multiSelectTreeviewChannelsGroups, node);

				if (resultNode != null) {
					resultNode.Expand();
				}
			}

			foreach (string node in _selectedNodes) {
				TreeNode resultNode = FindNodeInTreeAtPath(multiSelectTreeviewChannelsGroups, node);

				if (resultNode != null) {
					multiSelectTreeviewChannelsGroups.AddSelectedNode(resultNode);
				}
			}

			multiSelectTreeviewChannelsGroups.EndUpdate();

			// see stackoverflow.com/questions/626315/winforms-listview-remembering-scrolled-location-on-reload .
			// we can only set the topNode after EndUpdate(). Also, it might throw an exception -- weird?
			foreach (string node in _topDisplayedNodes) {
				TreeNode resultNode = FindNodeInTreeAtPath(multiSelectTreeviewChannelsGroups, node);

				if (resultNode != null) {
					try {
						multiSelectTreeviewChannelsGroups.TopNode = resultNode;
					} catch (Exception) {
						VixenSystem.Logging.Warn("ConfigChannels: exception caught trying to set TopNode.");
					}
					break;
				}
			}

			PopulateFormWithNode(_displayedNode);
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
					_expandedNodes.Add(GenerateTreeNodeFullPath(tn, multiSelectTreeviewChannelsGroups.PathSeparator));
				}

				if (multiSelectTreeviewChannelsGroups.SelectedNodes.Contains(tn)) {
					_selectedNodes.Add(GenerateTreeNodeFullPath(tn, multiSelectTreeviewChannelsGroups.PathSeparator));
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
			if (multiSelectTreeviewChannelsGroups.Nodes.Count > 0) {
				TreeNode current = multiSelectTreeviewChannelsGroups.TopNode;
				while (current != null) {
					_topDisplayedNodes.Add(GenerateTreeNodeFullPath(current, multiSelectTreeviewChannelsGroups.PathSeparator));
					current = current.NextNode;
				}
			}
		}

		private void AddNodeToTree(TreeNodeCollection collection, ChannelNode channel)
		{
			TreeNode addedNode = new TreeNode();
			addedNode.Name = channel.Id.ToString();
			addedNode.Text = channel.Name;
			addedNode.Tag = channel;

			if (channel.Children.Count() <= 0) {
				if (channel.Channel != null && channel.Channel.Patch.Count() > 0)
					addedNode.ImageKey = addedNode.SelectedImageKey = "ChannelPatched";
				else
					addedNode.ImageKey = addedNode.SelectedImageKey = "ChannelUnPatched";
			} else {
				addedNode.ImageKey = addedNode.SelectedImageKey = "Group";
			}

			collection.Add(addedNode);

			foreach (ChannelNode childNode in channel.Children) {
				AddNodeToTree(addedNode.Nodes, childNode);
			}
		}
		#endregion


		#region Form controls population & display
		private void PopulateFormWithNode(ChannelNode node)
		{
			if (node == _displayedNode && _displayedNodeInitialized)
				return;

			_displayedNode = node;
			_displayedNodeInitialized = true;

			buttonAddToGroup.Enabled = (node != null);
			buttonRemoveFromGroup.Enabled = (node != null);
			buttonAddProperty.Enabled = (node != null);
			buttonConfigureProperty.Enabled = (node != null);
			buttonRemoveProperty.Enabled = (node != null);
			buttonRenameItem.Enabled = (node != null);

			if (node == null) {
				labelParents.Text = "";
				_tooltip.SetToolTip(labelParents, null);
				listViewNodeContents.Items.Clear();
				listViewAddToNode.Items.Clear();
				listViewProperties.Items.Clear();
				return;
			} 

			// update the label with parent info about the node. Any good suggestions or improvements for this?
			int parentCount = GetNodeParentGroupCount(node);
			List<string> parents = GetNodeParentGroupNames(node);
			string labelString = "", tooltipString = "";
			labelString = "This item is in " + parentCount + " group" + ((parentCount != 1) ? "s" : "") + ((parentCount == 0) ? "." : ": ");
			tooltipString = labelString + "\r\n\r\n";
			foreach (string p in parents) {
				labelString = labelString + p + ", ";
				tooltipString = tooltipString + p + "\r\n";
			}
			labelParents.Text = labelString.TrimEnd(new char[] { ' ', ',' });
			tooltipString = tooltipString.TrimEnd(new char[] { '\r', '\n' });
			if (labelString.Length > 100) {
				_tooltip.SetToolTip(labelParents, tooltipString);
			} else {
				_tooltip.SetToolTip(labelParents, null);
			}

			textBoxName.Text = node.Name;
			PopulateContentsList(node);
			PopulateAddList(node);
			PopulatePropertiesList(node);
		}

		private void PopulatePropertiesList(ChannelNode node)
		{
			if (node == null)
				return;

			listViewProperties.BeginUpdate();
			listViewProperties.Items.Clear();

			foreach (IPropertyModuleInstance property in node.Properties) {
				ListViewItem item = new ListViewItem();
				item.Text = property.Descriptor.TypeName;
				item.Tag = property;
				listViewProperties.Items.Add(item);
			}

			listViewProperties.EndUpdate();
		}

		private void PopulateContentsList(ChannelNode node)
		{
			if (node == null)
				return;

			listViewNodeContents.BeginUpdate();
			listViewNodeContents.Items.Clear();

			ListViewGroup channelsGroup = null;
			ListViewGroup groupsGroup = null;
			ListViewGroup patchesGroup = null;

			// find each ListViewGroup for each of the different types of headers, so we can 
			// add each item to the correct group later on.
			foreach (ListViewGroup group in listViewNodeContents.Groups) {
				switch (group.Header) {
					case "Channels":
						channelsGroup = group;
						break;

					case "Groups":
						groupsGroup = group;
						break;

					case "Patches":
						patchesGroup = group;
						break;
				}
			}

			// iterate through each child of the given node, and add them to the contents box.
			// separate them based on being a channel or group.
			foreach (ChannelNode child in node.Children) {
				ListViewItem item = new ListViewItem();
				if (child.IsLeaf) {
					item.Group = channelsGroup;
				} else {
					item.Group = groupsGroup;
				}
				item.Text = child.Name;
				item.Tag = child;
				listViewNodeContents.Items.Add(item);
			}

			if (node.Channel != null) {
				foreach (ControllerReference patch in node.Channel.Patch.ControllerReferences) {
					ListViewItem item = new ListViewItem();
					item.Group = patchesGroup;
					item.Text = patch.ToString();
					item.Tag = patch;
					//item.ForeColor = SystemColors.InactiveCaption; // they are active for removing now, don't need this anymore?
					listViewNodeContents.Items.Add(item);
				}
			}

			listViewNodeContents.EndUpdate();
		}

		private void PopulateAddList(ChannelNode node)
		{
			if (node == null)
				return;

			// if it's a group, it can't have patches. If it isn't a group (leaf node), then
			// it can only have patches if it already has some. If it doesn't have anything
			// in it at the moment, it can have anything at all. Also change the selected item
			// if it was on an invalid one.
			if (node.IsLeaf) {
				if (node.Channel != null && node.Channel.Patch.Count() > 0) {
					radioButtonChannels.Enabled = false;
					radioButtonGroups.Enabled = false;
					radioButtonPatches.Enabled = true;
					radioButtonPatches.Checked = true;
				} else {
					radioButtonChannels.Enabled = true;
					radioButtonGroups.Enabled = true;
					radioButtonPatches.Enabled = true;
				}
			} else {
				radioButtonChannels.Enabled = true;
				radioButtonGroups.Enabled = true;
				radioButtonPatches.Enabled = false;
				if (radioButtonPatches.Checked) {
					radioButtonChannels.Checked = true;
				}
			}

			listViewAddToNode.BeginUpdate();
			listViewAddToNode.Items.Clear();

			// depending on which radio button is checked, populate the "add to item" box with
			// either channels or groups. Don't show any that are invalid.
			if (radioButtonChannels.Checked || radioButtonGroups.Checked) {
				IEnumerable<ChannelNode> collection;
				IEnumerable<ChannelNode> invalid = node.InvalidChildren();
				List<ChannelNode> seen = new List<ChannelNode>();

				if (radioButtonChannels.Checked) {
					collection = VixenSystem.Nodes.GetLeafNodes();
				} else {
					collection = VixenSystem.Nodes.GetNonLeafNodes();
				}

				foreach (ChannelNode n in collection) {
					if (!node.Children.Contains(n) && !invalid.Contains(n) && !seen.Contains(n)) {
						ListViewItem item = new ListViewItem();
						item.Text = n.Name;
						item.Tag = n;
						listViewAddToNode.Items.Add(item);

						seen.Add(n);
					}
				}
			} else {
				foreach(OutputController oc in VixenSystem.Controllers) {
					for (int i = 0; i < oc.OutputCount; i++) {
						if (node.Channel != null && node.Channel.Patch.Contains(new ControllerReference(oc.Id, i))) {
							continue;
						}
						ListViewItem item = new ListViewItem();
						item.Text = oc.Name + " [" + (i + 1) + "]";
						item.Tag = new ControllerReference(oc.Id, i);
						listViewAddToNode.Items.Add(item);
					}
				}
			}

			listViewAddToNode.EndUpdate();
		}
		#endregion


		#region Form buttons
		private void buttonAddChannel_Click(object sender, EventArgs e)
		{
			using (CommonElements.TextDialog textDialog = new CommonElements.TextDialog("Channel Name?")) {
				if (textDialog.ShowDialog() == DialogResult.OK) {
					if (textDialog.Response == "")
						VixenSystem.Nodes.AddNewNode("Unnamed Channel");
					else
						VixenSystem.Nodes.AddNewNode(textDialog.Response);

					PopulateNodeTree();
				}
			}
		}

		private void buttonDeleteItem_Click(object sender, EventArgs e)
		{
			if (multiSelectTreeviewChannelsGroups.SelectedNodes.Count > 0)
			{
				if (MessageBox.Show("Are you sure you want to delete the selected item(s)?", "Delete Item(s)?", MessageBoxButtons.OKCancel) == DialogResult.OK)
				{
					foreach (TreeNode tn in multiSelectTreeviewChannelsGroups.SelectedNodes) {
						ChannelNode cn = tn.Tag as ChannelNode;
						ChannelNode parent = (tn.Parent != null) ? tn.Parent.Tag as ChannelNode : null;
						VixenSystem.Nodes.RemoveNode(cn, parent, true);
						if (_displayedNode == cn) {
							_displayedNode = null;
						}
					}
				}
			}

			PopulateNodeTree();
		}

		private void buttonCreateGroup_Click(object sender, EventArgs e)
		{
			if (multiSelectTreeviewChannelsGroups.SelectedNodes.Count > 0)
			{
				String groupName;
				using (CommonElements.TextDialog textDialog = new CommonElements.TextDialog("Group Name?")) {
					if (textDialog.ShowDialog() == DialogResult.OK) {
						if (textDialog.Response == "")
							groupName = "Unnamed Group";
						else
							groupName = textDialog.Response;

						ChannelNode newNode = VixenSystem.Nodes.AddNewNode(groupName);

						foreach (TreeNode tn in multiSelectTreeviewChannelsGroups.SelectedNodes) {
							newNode.AddChild(tn.Tag as ChannelNode);
						}

						PopulateNodeTree();
					}
				}
			}
		}

		private void buttonBulkRename_Click(object sender, EventArgs e)
		{
            if (multiSelectTreeviewChannelsGroups.SelectedNodes.Count > 0)
            {
                BulkChannelRename Formx = new BulkChannelRename();
                Formx.ShowDialog();
            }
		}

		private void buttonRenameItem_Click(object sender, EventArgs e)
		{
			if (textBoxName.Text.Trim() != "") {
				VixenSystem.Nodes.RenameNode(_displayedNode, textBoxName.Text.Trim());
			}
			PopulateNodeTree();
		}

		private void buttonRemoveFromGroup_Click(object sender, EventArgs e)
		{
			if (listViewNodeContents.SelectedItems.Count > 0) {
				string message, title;
				if (listViewNodeContents.SelectedItems.Count == 1) {
					message = "Are you sure you want to remove the selected item from the group?";
					title = "Remove Item?";
				} else {
					message = "Are you sure you want to remove the selected items from the group?";
					title = "Remove Items?";
				}
				if (MessageBox.Show(message, title, MessageBoxButtons.OKCancel) == DialogResult.OK) {
					foreach (ListViewItem item in listViewNodeContents.SelectedItems) {
						if (item.Tag is ChannelNode) {
							(item.Tag as ChannelNode).RemoveFromParent(_displayedNode, true);
						} else if (item.Tag is ControllerReference) {
							_displayedNode.Channel.Patch.Remove(item.Tag as ControllerReference);
						} else {
							throw new NotImplementedException("Can't figure out what to do to remove item!");
						}
					}

					PopulateNodeTree();
				}
			}
		}

		private void buttonAddToGroup_Click(object sender, EventArgs e)
		{
			if (listViewAddToNode.SelectedItems.Count <= 0) {
				return;
			}

			foreach (ListViewItem item in listViewAddToNode.SelectedItems) {
				if (item.Tag is ChannelNode) {
					_displayedNode.AddChild((ChannelNode)(item.Tag));
				} else if (item.Tag is ControllerReference) {
					if (_displayedNode.Channel == null) {
						_displayedNode.Channel = VixenSystem.Channels.AddChannel(_displayedNode.Name);
					}
					_displayedNode.Channel.Patch.Add((ControllerReference)item.Tag);
				} else {
					throw new NotImplementedException("Don't know how to add item to the node!");
				}
			}

			PopulateNodeTree();
		}

		private void buttonAddProperty_Click(object sender, EventArgs e)
		{
			List<KeyValuePair<string, object>> properties = new List<KeyValuePair<string, object>>();
			foreach (KeyValuePair<Guid, string> kvp in ApplicationServices.GetAvailableModules<IPropertyModuleInstance>()) {
				properties.Add(new KeyValuePair<string, object>(kvp.Value, kvp.Key));
			}
			CommonElements.ListSelectDialog addForm = new CommonElements.ListSelectDialog("Add Property", (properties));
			if (addForm.ShowDialog() == DialogResult.OK) {
				_displayedNode.Properties.Add((Guid)addForm.selectedItem);
				PopulatePropertiesList(_displayedNode);
			}
		}

		private void buttonConfigureProperty_Click(object sender, EventArgs e)
		{
			ConfigureSelectedProperty();
		}

		private void buttonRemoveProperty_Click(object sender, EventArgs e)
		{
			if (listViewProperties.SelectedItems.Count > 0) {
				string message, title;
				if (listViewProperties.SelectedItems.Count == 1) {
					message = "Are you sure you want to remove the selected property from the group?";
					title = "Remove Property?";
				} else {
					message = "Are you sure you want to remove the selected properties from the group?";
					title = "Remove Properties?";
				}
				if (MessageBox.Show(message, title, MessageBoxButtons.OKCancel) == DialogResult.OK) {
					foreach (ListViewItem item in listViewProperties.SelectedItems) {
						_displayedNode.Properties.Remove((item.Tag as IPropertyModuleInstance).Descriptor.TypeId);
					}

					PopulatePropertiesList(_displayedNode);
				}
			}
		}
		#endregion


		#region Events

		private void multiSelectTreeviewChannelsGroups_AfterSelect(object sender, TreeViewEventArgs e)
		{
			PopulateFormWithNode(multiSelectTreeviewChannelsGroups.SelectedNode.Tag as ChannelNode);
		}
		private void radioButtonChannels_CheckedChanged(object sender, EventArgs e)
		{
			PopulateAddList(_displayedNode);
		}

		private void radioButtonGroups_CheckedChanged(object sender, EventArgs e)
		{
			PopulateAddList(_displayedNode);
		}

		private void radioButtonPatches_CheckedChanged(object sender, EventArgs e)
		{
			PopulateAddList(_displayedNode);
		}

		private void listViewProperties_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			ConfigureSelectedProperty();
		}

		private void listViewProperties_SelectedIndexChanged(object sender, EventArgs e)
		{
			buttonConfigureProperty.Enabled = (listViewProperties.SelectedItems.Count == 1);
		}

		private void listViewNodeContents_SelectedIndexChanged(object sender, EventArgs e)
		{
			// TODO: enable/disable 'remove' button
		}

		private void listViewAddToNode_SelectedIndexChanged(object sender, EventArgs e)
		{
			// TODO: enable/disable 'add to group' button
		}

		private void multiSelectTreeviewChannelsGroupsDragFinishingHandler(object sender, DragFinishingEventArgs e)
		{
			// we want to finish off the drag ourselves, and not have the treeview control move the nodes around.
			// (In fact, in a lame attempt at 'data binding', we're going to completely redraw the tree after
			// making all the required changes from this drag-drop.) So set the flag to indicate that.
			e.FinishDrag = false;

			// first determine the node that they will be moved to. This will depend on if we are dragging onto a node
			// directly, or above/below one to reorder.
			ChannelNode newParentNode = null;						// the channelNode that the selected items will move to
			TreeNode expandNode = null;								// if we need to expand a node once we've moved everything
			int index = -1;

			if (e.DragBetweenNodes == DragBetweenNodes.DragOnTargetNode) {
				newParentNode = e.TargetNode.Tag as ChannelNode;
				expandNode = e.TargetNode;
			} else if (e.DragBetweenNodes == DragBetweenNodes.DragBelowTargetNode && e.TargetNode.IsExpanded) {
				newParentNode = e.TargetNode.Tag as ChannelNode;
				expandNode = e.TargetNode;
				index = 0;
			} else {
				if (e.TargetNode.Parent == null) {
					newParentNode = null;	// needs to go at the root level
				} else {
					newParentNode = e.TargetNode.Parent.Tag as ChannelNode;
				}

				if (e.DragBetweenNodes == DragBetweenNodes.DragAboveTargetNode) {
					index = e.TargetNode.Index;
				} else {
					index = e.TargetNode.Index + 1;
				}
			}

			// Check to see if the new parent node would be 'losing' the Channel (ie. becoming a
			// group instead of a leaf node with a channel/patches). Prompt the user first.
			if (newParentNode != null && newParentNode.Channel != null && newParentNode.Channel.Patch.Count() > 0) {
				string message = "Moving items into this Channel will convert it into a Group, which will remove any " +
					"patches it may have to outputs. Are you sure you want to continue?";
				string title = "Convert Channel to Group?";
				DialogResult result = MessageBox.Show(message, title, MessageBoxButtons.YesNoCancel);
				if (result != System.Windows.Forms.DialogResult.Yes) {
					return;
				}
			}

			// If moving channel nodes, we need to iterate through all selected treenodes, and remove them from
			// the parent in which they are selected (which we can determine from the treenode parent), and add them
			// to the target node. If copying, we need to just add them to the new parent node.
			foreach (TreeNode treeNode in e.SourceNodes) {
				ChannelNode sourceNode = treeNode.Tag as ChannelNode;
				ChannelNode oldParentNode = (treeNode.Parent != null) ? treeNode.Parent.Tag as ChannelNode : null;
				if (e.DragMode == DragDropEffects.Move) {
					if (index >= 0) {
						VixenSystem.Nodes.MoveNode(sourceNode, newParentNode, oldParentNode, index);
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
					VixenSystem.Logging.Warn("ConfigChannels: Trying to deal with a drag that is an unknown type!");
				}
			}

			if (expandNode != null)
				expandNode.Expand();
			
			PopulateNodeTree();
		}

		private void multiSelectTreeviewChannelsGroupsDragVerifyHandler(object sender, DragVerifyEventArgs e)
		{
			// we need to go through all nodes that would be moved (source nodes), and check if there's any
			// problem moving any of them to the target node (circular references, etc.), since it's possible
			// for a channel to exist multiple times in the treeview as different treenodes.

			List<ChannelNode> nodes = new List<ChannelNode>(e.SourceNodes.Select(x => x.Tag as ChannelNode));

			// now get a list of invalid children for this target node, and check all the remaining nodes against it.
			// If any of them fail, the entire operation should fail, as it would be an invalid move.

			// the target node will be the actual target node if it is directly on it, otherwise it would be the parent
			// of the target node if it would be dragged above/below the taget element (as it would be put alongside it).
			// also keep track of the 'permitted' nodes: this is used when dragging alongside another element: any children
			// of the new parent node are considered OK, as we might just be shuffling them around. Normally, this would
			// be A Bad Thing, since it would seem like we're adding a child to the group it's already in. (This is only
			// the case when moving; if copying, it should be disabled. That's checked later.)
			IEnumerable<ChannelNode> invalidNodesForTarget = null;
			IEnumerable<ChannelNode> permittedNodesForTarget = null;

			if (e.DragBetweenNodes == DragBetweenNodes.DragOnTargetNode || e.DragBetweenNodes == DragBetweenNodes.DragBelowTargetNode && e.TargetNode.IsExpanded) {
				invalidNodesForTarget = (e.TargetNode.Tag as ChannelNode).InvalidChildren();
				permittedNodesForTarget = new HashSet<ChannelNode>();
			} else {
				if (e.TargetNode.Parent == null) {
					invalidNodesForTarget = VixenSystem.Nodes.InvalidRootNodes;
					permittedNodesForTarget = VixenSystem.Nodes.RootNodes;
				} else {
					invalidNodesForTarget = (e.TargetNode.Parent.Tag as ChannelNode).InvalidChildren();
					permittedNodesForTarget = (e.TargetNode.Parent.Tag as ChannelNode).Children;
				}
			}

			if ((e.KeyState & 8) != 0) {		// the CTRL key
				e.DragMode = DragDropEffects.Copy;
				permittedNodesForTarget = new HashSet<ChannelNode>();
			} else {
				e.DragMode = DragDropEffects.Move;
			}

			IEnumerable<ChannelNode> invalidSourceNodes = invalidNodesForTarget.Intersect(nodes);
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

		private int GetNodeParentGroupCount(ChannelNode node)
		{
			int count = node.Parents.Count();
			if(VixenSystem.Nodes.RootNodes.Contains(node))
				count--;
			return count;
		}

		private List<string> GetNodeParentGroupNames(ChannelNode node, int maxNames = Int32.MaxValue)
		{
			List<string> result = new List<string>();
			foreach (ChannelNode parent in node.Parents) {
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



		#endregion

	}
}
