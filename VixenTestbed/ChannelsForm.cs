using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommonElements;
using Vixen.Sys;

namespace VixenTestbed {
	public partial class ChannelsForm : Form {
		private bool _dragCapture = false;

		public ChannelsForm() {
			InitializeComponent();
		}

		private void ChannelsForm_Load(object sender, EventArgs e) {
			try {
				_LoadChannels();
				_LoadNodes();
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private Channel _SelectedChannel {
			get { return listBoxChannels.SelectedItem as Channel; }
		}

		private ChannelNode _SelectedNode {
			get {
				if(treeViewNodes.SelectedNodes.Count == 1) {
					return treeViewNodes.SelectedNode.Tag as ChannelNode;
				}
				return null;
			}
		}

		private IEnumerable<ChannelNode> _SelectedNodes {
			get { return treeViewNodes.SelectedNodes.Select(x => x.Tag as ChannelNode); }
		}

		private ChannelNode _ParentOf(ChannelNode node) {
			TreeNode treeNode = _Find(node, treeViewNodes.Nodes);
			if(treeNode != null && treeNode.Parent != null) {
				return treeNode.Parent.Tag as ChannelNode;
			}
			return null;
		}

		private TreeNode _Find(ChannelNode node, TreeNodeCollection treeNodes) {
			foreach(TreeNode treeNode in treeNodes) {
				if(treeNode.Tag == node) return treeNode;
				TreeNode childNode = _Find(node, treeNode.Nodes);
				if(childNode != null) return childNode;
			}
			return null;
		}

		private void _LoadChannels() {
			listBoxChannels.BeginUpdate();
			listBoxChannels.Items.Clear();
			foreach(Channel channel in VixenSystem.Channels) {
				_AddChannel(listBoxChannels.Items, channel);
			}
			listBoxChannels.EndUpdate();
		}

		private void _LoadNodes() {
			treeViewNodes.BeginUpdate();
			treeViewNodes.Nodes.Clear();
			TreeNode rootNode = treeViewNodes.Nodes.Add("Root");
			foreach(ChannelNode node in VixenSystem.Nodes.GetRootNodes()) {
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
			try {
				using(CommonElements.TextDialog textDialog = new CommonElements.TextDialog("New channel name")) {
					if(textDialog.ShowDialog() == DialogResult.OK) {
						Channel channel = VixenSystem.Channels.AddChannel(textDialog.Response);
						VixenSystem.Nodes.AddChannelLeaf(channel);
						_LoadChannels();
						_LoadNodes();
					}
				}
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void buttonRemoveChannel_Click(object sender, EventArgs e) {
			try {
				Channel channel = _SelectedChannel;
				if(channel != null) {
					if(MessageBox.Show("This will also remove any nodes that reference the channel.  Continue?", "Vixen Testbed", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
						VixenSystem.Channels.RemoveChannel(channel);
						_LoadChannels();
						_LoadNodes();
					}
				}
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void buttonProperties_Click(object sender, EventArgs e) {
			try {
				ChannelNode channelNode = _SelectedNode;
				using(NodePropertiesDialog nodePropertiesDialog = new NodePropertiesDialog(channelNode)) {
					nodePropertiesDialog.ShowDialog();
				}
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void treeViewNodes_AfterSelect(object sender, TreeViewEventArgs e) {
			buttonProperties.Enabled = _SelectedNode != null;
			buttonCreateGroup.Enabled = _SelectedNodes.Count() > 1;
			buttonRemoveNode.Enabled = _SelectedNodes.Count() > 0;
		}

		private void treeViewNodes_DragDrop(object sender, DragEventArgs e) {
		}
		//private void treeViewNodes_DragDrop(object sender, DragEventArgs e) {
		//    try {
		//        _dragCapture = false;

		//        ChannelNode draggingNode = e.Data.GetData(typeof(ChannelNode)) as ChannelNode;
		//        TreeNode treeNode = treeViewNodes.GetNodeAt(treeViewNodes.PointToClient(new Point(e.X, e.Y)));
		//        ChannelNode targetNode = treeNode.Tag as ChannelNode;

		//        if(e.Effect == DragDropEffects.Copy) {
		//            VixenSystem.Nodes.AddChildToParent(draggingNode, targetNode);
		//        } else if(e.AllowedEffect == DragDropEffects.Move) {
		//            //Vixen.Sys.Execution.Nodes.MoveNode(draggingNode, targetNode);
		//        }
		//    } catch(Exception ex) {
		//        MessageBox.Show(ex.Message);
		//    }
		//}

		private void treeViewNodes_DragOver(object sender, DragEventArgs e) {
		}
		//private void treeViewNodes_DragOver(object sender, DragEventArgs e) {
		//    try {
		//        // If the source is a leaf, copy it
		//        // If the source is not a leaf, move it
		//        ChannelNode draggingNode = e.Data.GetData(typeof(ChannelNode)) as ChannelNode;
		//        TreeNode treeNode = treeViewNodes.GetNodeAt(treeViewNodes.PointToClient(new Point(e.X, e.Y)));
		//        if(draggingNode != null && treeNode != null) {
		//            // Copy a leaf, move a branch.
		//            e.Effect = (draggingNode.IsLeaf || ((e.KeyState & 4) != 0)) ? DragDropEffects.Copy : DragDropEffects.Move;
		//        } else {
		//            e.Effect = DragDropEffects.None;
		//        }
		//    } catch(Exception ex) {
		//        MessageBox.Show(ex.Message);
		//    }
		//}

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

		private void buttonCreateGroup_Click(object sender, EventArgs e) {
			using(TextDialog textDialog = new TextDialog("Name for the new group", "Create Group")) {
				if(textDialog.ShowDialog() == DialogResult.OK) {
					ChannelNode groupNode = VixenSystem.Nodes.AddNode(textDialog.Response);
					foreach(ChannelNode childNode in _SelectedNodes) {
						VixenSystem.Nodes.AddChildToParent(childNode, groupNode);
					}
					_LoadNodes();
				}
			}
		}

		private void buttonRemoveNode_Click(object sender, EventArgs e) {
			//string message;
			//ChannelNode[] nodes = _SelectedNodes.Where(x => x.IsRemovable).ToArray();
			//if(nodes.Length == 0) {
			//    MessageBox.Show("There are no removable nodes selected", "Vixen Testbed", MessageBoxButtons.OK, MessageBoxIcon.Information);
			//    return;
			//} else if(nodes.Length == 1) {
			//    message = "Remove the node \"" + nodes[0].Name + "\"?";
			//} else {
			//    message = "There are " + nodes.Length + " removable nodes selected.  Remove them?";
			//}

			//if(MessageBox.Show(message, "Vixen Testbed", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
			//    foreach(ChannelNode node in nodes) {
			//        ChannelNode parentNode = _ParentOf(node);
			//        VixenSystem.Nodes.RemoveNode(node, parentNode, true);
			//    }
			//}
		}

		private void listBoxChannels_SelectedIndexChanged(object sender, EventArgs e) {
			buttonRemoveChannel.Enabled = _SelectedChannel != null;
		}
	}
}
