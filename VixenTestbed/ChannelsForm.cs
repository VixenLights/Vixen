using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;

namespace VixenTestbed {
	public partial class ChannelsForm : Form {
		private bool _dragCapture = false;

		public ChannelsForm() {
			InitializeComponent();
		}

		private void ChannelsForm_Load(object sender, EventArgs e) {
			_LoadChannels();
			_LoadNodes();
		}

		private Channel _SelectedChannel {
			get { return listBoxChannels.SelectedItem as Channel; }
		}

		private ChannelNode _SelectedNode {
			get { return treeViewNodes.SelectedNode.Tag as ChannelNode; }
		}

		private void _LoadChannels() {
			listBoxChannels.BeginUpdate();
			listBoxChannels.Items.Clear();
			foreach(Channel channel in Vixen.Sys.Execution.Channels) {
				_AddChannel(listBoxChannels.Items, channel);
			}
			listBoxChannels.EndUpdate();
		}

		private void _LoadNodes() {
			treeViewNodes.BeginUpdate();
			treeViewNodes.Nodes.Clear();
			TreeNode rootNode = treeViewNodes.Nodes.Add("Root");
			foreach(ChannelNode node in Vixen.Sys.Execution.Nodes.RootNodes) {
				_AddNode(rootNode.Nodes, node);
			}
			treeViewNodes.ExpandAll();
			treeViewNodes.EndUpdate();
		}

		private void _AddChannel(ListBox.ObjectCollection collection, Channel channel) {
			collection.Add(channel);
		}

		private void _AddNode(TreeNodeCollection collection, ChannelNode node) {
			TreeNode addedNode;
			addedNode = collection.Add(node.Name);

			addedNode.Tag = node;
			addedNode.Checked = node.Masked;

			foreach(ChannelNode childNode in node.Children) {
				_AddNode(addedNode.Nodes, childNode);
			}
		}

		private void treeViewChannels_AfterSelect(object sender, TreeViewEventArgs e) {
			// Must be on a channel to remove a channel.
			buttonRemoveChannel.Enabled = _SelectedChannel != null;
		}

		private void buttonAddChannel_Click(object sender, EventArgs e) {
			using(CommonElements.TextDialog textDialog = new CommonElements.TextDialog("New channel name")) {
				if(textDialog.ShowDialog() == DialogResult.OK) {
					Channel channel = Vixen.Sys.Execution.AddChannel(textDialog.Response);
					Vixen.Sys.Execution.Nodes.AddChannelLeaf(channel);
				}
			}
		}

		private void buttonRemoveChannel_Click(object sender, EventArgs e) {
			Channel channel = _SelectedChannel;
			if(channel != null) {
				Vixen.Sys.Execution.RemoveChannel(channel);
				//Vixen.Sys.Execution.Nodes.RemoveChannelLeaf(node.Channel);
			}
		}

		private void buttonProperties_Click(object sender, EventArgs e) {
			ChannelNode channelNode = _SelectedNode;
			using(NodePropertiesDialog nodePropertiesDialog = new NodePropertiesDialog(channelNode)) {
				nodePropertiesDialog.ShowDialog();
			}
		}

		private void treeViewNodes_AfterSelect(object sender, TreeViewEventArgs e) {
			buttonProperties.Enabled = _SelectedNode != null;
		}

		private void treeViewNodes_DragDrop(object sender, DragEventArgs e) {
			_dragCapture = false;

			ChannelNode draggingNode = e.Data.GetData(typeof(ChannelNode)) as ChannelNode;
			TreeNode treeNode = treeViewNodes.GetNodeAt(treeViewNodes.PointToClient(new Point(e.X, e.Y)));
			ChannelNode targetNode = treeNode.Tag as ChannelNode;

			if(e.Effect == DragDropEffects.Copy) {
				Vixen.Sys.Execution.Nodes.CopyNode(draggingNode, targetNode);
			} else if(e.AllowedEffect == DragDropEffects.Move) {
				//Vixen.Sys.Execution.Nodes.MoveNode(draggingNode, targetNode);
			}
		}

		private void treeViewNodes_DragOver(object sender, DragEventArgs e) {
			// If the source is a leaf, copy it
			// If the source is not a leaf, move it
			ChannelNode draggingNode = e.Data.GetData(typeof(ChannelNode)) as ChannelNode;
			TreeNode treeNode = treeViewNodes.GetNodeAt(treeViewNodes.PointToClient(new Point(e.X, e.Y)));
			if(draggingNode != null && treeNode != null) {
				// Copy a leaf, move a branch.
				e.Effect = (draggingNode.IsLeaf || ((e.KeyState & 4) != 0)) ? DragDropEffects.Copy : DragDropEffects.Move;
			} else {
				e.Effect = DragDropEffects.None;
			}
		}

		private void treeViewNodes_MouseDown(object sender, MouseEventArgs e) {
			// Need to do this here so that a mouse down is required to start a drag.
			// Otherwise, canceling one with ESC and moving the mouse would qualify
			// as a drag.
			TreeNode selectedNode = treeViewNodes.GetNodeAt(e.Location);
			_dragCapture = selectedNode != null;
		}

		private void treeViewNodes_MouseMove(object sender, MouseEventArgs e) {
			// Node is selected?
			// Left mouse button is down?
			// Mouse is captured?
			// Root is not selected?
			// Then start dragging.
			//For debugging ease...
			TreeNode selectedNode = treeViewNodes.GetNodeAt(e.Location);
			bool nodeSelected = selectedNode != null;
			bool leftButtonDown = (MouseButtons & MouseButtons.Left) != 0;
			bool captured = _dragCapture;
			bool notRoot = nodeSelected && selectedNode.Level > 0;
			if(nodeSelected &&
				leftButtonDown &&
				captured &&
				notRoot) {
				ChannelNode node = selectedNode.Tag as ChannelNode;
				// MUST call DoDragDrop on the control, not the form.
				// If it's called on the form, QueryContinueDrag and
				// GiveFeedback will not be raised.
				treeViewNodes.DoDragDrop(node, DragDropEffects.Copy | DragDropEffects.Move);
			}
		}

		private void treeViewNodes_QueryContinueDrag(object sender, QueryContinueDragEventArgs e) {
			//If there is a change in the keyboard or mouse button state, 
			//the QueryContinueDrag event is raised
			if(e.EscapePressed) {
				// Cancel the drag
				_dragCapture = false;
			}
		}
	}
}
