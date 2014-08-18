using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Common.Controls.Timeline;
using Vixen.Module.Effect;
using Vixen.Services;
using WeifenLuo.WinFormsUI.Docking;
using VixenModules.Sequence.Timed;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class Form_Effects : DockContent
	{
		private int SelectedNode;
		public event EventHandler EscapeDrawMode;

		public Form_Effects(TimelineControl timelineControl)
		{
			InitializeComponent();
			TimelineControl = timelineControl;
		}

		private void Form_Effects_Load(object sender, EventArgs e)
		{
			// Remove "Advanced Lighting" for now
			treeEffects.Nodes.RemoveAt(1);
			LoadAvailableEffects();
		}

		public TimelineControl TimelineControl { get; set; }

		private void LoadAvailableEffects()
		{
			foreach (
				IEffectModuleDescriptor effectDesriptor in
					ApplicationServices.GetModuleDescriptors<IEffectModuleInstance>().Cast<IEffectModuleDescriptor>())
			{
				// Add the effects to the tree
				TreeNode parentNode = null;
				switch (effectDesriptor.EffectGroup)
				{
					case EffectGroups.Basic:
						parentNode = treeEffects.Nodes["treeBasic"];
						break;
					case EffectGroups.Advanced:
						parentNode = treeEffects.Nodes["treeAdvanced"];
						break;
					case EffectGroups.Device:
						parentNode = treeEffects.Nodes["treeDevice"];
						break;
				}
				TreeNode node = new TreeNode(effectDesriptor.EffectName);
				node.Tag = effectDesriptor.TypeId;
				parentNode.Nodes.Add(node);
				// Set the image
				System.Drawing.Image image = effectDesriptor.GetRepresentativeImage(48, 48);
				if (image != null)
				{
					effectTreeImages.Images.Add(effectDesriptor.EffectName, image);
					node.ImageIndex = effectTreeImages.Images.Count - 1;
					node.SelectedImageIndex = node.ImageIndex;
				}
				else
				{
					SetNodeImage(node, "blank.png");
				}

				if (treeEffects.Nodes.Count > 0)
				{
					treeEffects.CollapseAll();
					treeEffects.Nodes[0].Expand();
					treeEffects.Nodes[1].Expand();
				}
			}
		}

		#region Effect Drag/Drop/Select

		private bool _beginDragDrop;

		TreeNode m_node = null;
		private void treeEffects_MouseDown(object sender, MouseEventArgs e)
		{
			m_node = treeEffects.GetNodeAt(e.X, e.Y);
			if (m_node != null)
			{
				// Is this a group?
				if (m_node.Nodes.Count > 0 && e.Clicks == 1)
				{
					if (m_node.IsExpanded)
					{
						m_node.Collapse();
					}
					else
					{
						m_node.Expand();
					}
				}
				else
				{
					if ((e.Button == MouseButtons.Left) && (e.Clicks == 1))
						_beginDragDrop = true;
					else
						_beginDragDrop = false;
				}
			}
		}

		public void DeselectAllNodes()
		{
			SelectedNode = 0;
			TimelineControl.grid.SelectedEffect = Guid.Empty;
			TimelineControl.grid.Cursor = Cursors.Default;
			foreach (TreeNode node in treeEffects.Nodes[0].Nodes)
			{
				node.BackColor = Color.Empty;
			}
		}

		public void MoveNodeSelection(string MoveDirection)
		{
			var nodeCount = treeEffects.Nodes[0].GetNodeCount(true) - 1;
			bool validMove = false;
			var targetNode = 0;

			switch (MoveDirection)
			{
				case "down":
					targetNode = SelectedNode + 1;
					if (TimelineControl.grid.SelectedEffect == Guid.Empty)
						targetNode = 0;
					if (targetNode <= nodeCount)
						validMove = true;
				break;
				
				case "up":
					targetNode = SelectedNode - 1;
					if (TimelineControl.grid.SelectedEffect == Guid.Empty)
						targetNode = nodeCount;
					if (targetNode >= 0)
						validMove = true;
				break;
			}
			
			if (validMove)
			{
				//MessageBox.Show(string.Format("Target Node: {0}", targetNode));
				foreach (TreeNode node in treeEffects.Nodes[0].Nodes)
				{
					if (node.Index == targetNode)
					{
						SelectedNode = node.Index;
						TimelineControl.grid.SelectedEffect = (Guid)node.Tag;
						node.BackColor = Color.Blue;
					}
					else
					{
						node.BackColor = Color.Empty;
					}
				}
			}
		}

		private void SelectNodeForDrawing()
		{
			SelectedNode = m_node.Index;
			//MessageBox.Show(string.Format("Selected node: {0}", SelectedNode.ToString()));

			if (m_node.BackColor == Color.Blue)
			{
				TimelineControl.grid.SelectedEffect = Guid.Empty;
				m_node.BackColor = Color.Empty;
			}
			else
			{
				TimelineControl.grid.SelectedEffect = (Guid)m_node.Tag;
				foreach (TreeNode node in treeEffects.Nodes[0].Nodes)
				{
					node.BackColor = Color.Empty;
				}
				m_node.BackColor = Color.Blue;
			}
		}

		private void treeEffects_MouseMove(object sender, MouseEventArgs e)
		{
			if ((e.Button == MouseButtons.Left) && _beginDragDrop)
			{
				_beginDragDrop = false;
				DataObject data = new DataObject(DataFormats.Serializable, m_node.Tag);
				DoDragDrop(data, DragDropEffects.Copy);
			}
		}

		private void treeEffects_MouseClick(object sender, MouseEventArgs e)
		{
			m_node = treeEffects.GetNodeAt(e.X, e.Y);
			// Is this a group?
			if (m_node != null && m_node.Nodes.Count == 0)
			{
				//We are not going to allow RDS or Launcher in draw mode, for now at least.
				if (m_node.Parent.Name == "treeDevice")
				{
					MessageBox.Show("Currently, you must drag this item to the grid to place it.","Effect Selection", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				else
				{
					//if (TimelineControl.grid.EnableDrawMode)
						SelectNodeForDrawing();
					//else
					//	MessageBox.Show("You must drag this item to the grid to place it when not in Drawing Mode", "Effect Selection", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
			}
		}

		#endregion

		private void treeEffects_AfterExpand(object sender, TreeViewEventArgs e)
		{
			SetNodeImage(e.Node, "bullet_arrow_down.png");
		}

		private void SetNodeImage(TreeNode node, string image)
		{
			node.ImageKey = image;
			node.SelectedImageKey = node.ImageKey;
		}

		private void treeEffects_AfterCollapse(object sender, TreeViewEventArgs e)
		{
			SetNodeImage(e.Node, "bullet_arrow_Right.png");
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			//This prevents the treeview from handling keystrokes while it still has focus
			//Was causing an annoying ding when the user had not drawn anything, and fumbling
			//the arrow key navigation
			if (keyData == Keys.Escape)
				EscapeDrawMode(this,EventArgs.Empty);
			base.ProcessCmdKey(ref msg, keyData);
			return true;
			
		}

		private void treeEffects_AfterSelect(object sender, TreeViewEventArgs e)
		{
			treeEffects.SelectedNode = null;
		}
	}
}
