using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;
using Vixen.Common;
using Vixen.Hardware;
using Vixen.Module.Property;

namespace VixenApplication
{
	public partial class ConfigChannels : Form
	{
		private ChannelNode _displayedNode;

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
			_tooltip = new ToolTip();
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

			foreach (ChannelNode channel in Vixen.Sys.Execution.Nodes.RootNodes) {
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
						//TODO: log message here (check with KC on preferred logging methods)
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
			TreeNode addedNode;
			addedNode = collection.Add(channel.Id.ToString(), channel.Name);

			addedNode.Tag = channel;

			foreach (ChannelNode childNode in channel.Children) {
				AddNodeToTree(addedNode.Nodes, childNode);
			}
		}
		#endregion


		#region Form controls population & display
		private void PopulateFormWithNode(ChannelNode node)
		{
			_displayedNode = node;

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

			// What the hell. I mean, seriously. C# sucks; surely there's a better way to do this?!@*(#@!
			// (when someone else comes across this: PLEASE correct this WTF-worthy code.)
			// ******************************************************************
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
			// ******************************************************************

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

			listViewAddToNode.BeginUpdate();
			listViewAddToNode.Items.Clear();

			// depending on which radio button is checked, populate the "add to item" box with
			// either channels or groups. Don't show any that are invalid.
			if (radioButtonChannels.Checked || radioButtonGroups.Checked) {
				IEnumerable<ChannelNode> collection;
				List<ChannelNode> invalid = node.InvalidChildren();
				List<ChannelNode> seen = new List<ChannelNode>();

				if (radioButtonChannels.Checked) {
					collection = Vixen.Sys.Execution.Nodes.GetLeafNodes();
				} else {
					collection = Vixen.Sys.Execution.Nodes.GetNonLeafNodes();
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
				foreach (OutputController oc in OutputController.GetAll()) {
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
						Vixen.Sys.Execution.Nodes.AddNewNode("Unnamed Channel");
					else
						Vixen.Sys.Execution.Nodes.AddNewNode(textDialog.Response);

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
						Vixen.Sys.Execution.Nodes.RemoveNode(cn, parent);
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

						ChannelNode newNode = Vixen.Sys.Execution.Nodes.AddNewNode(groupName);

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
			MessageBox.Show("Not implemented yet.");
		}

		private void buttonRenameItem_Click(object sender, EventArgs e)
		{
			if (textBoxName.Text.Trim() != "") {
				Vixen.Sys.Execution.Nodes.RenameNode(_displayedNode, textBoxName.Text.Trim());
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
							(item.Tag as ChannelNode).RemoveFromParent(_displayedNode);
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
			foreach (ListViewItem item in listViewAddToNode.SelectedItems) {
				if (item.Tag is ChannelNode) {
					_displayedNode.AddChild((ChannelNode)(item.Tag));
				} else if (item.Tag is ControllerReference) {
					if (_displayedNode.Channel == null) {
						_displayedNode.Channel = Vixen.Sys.Execution.AddChannel(_displayedNode.Name);
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
			ListSelectDialog addForm = new ListSelectDialog("Add Property", (properties));
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
		private void multiSelectTreeviewChannelsGroups_AfterSelect(object sender, TreeViewEventArgs e) {
			PopulateFormWithNode(e.Node.Tag as ChannelNode);
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
			if (Vixen.Sys.Execution.Nodes.RootNodes.Contains(node))
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
