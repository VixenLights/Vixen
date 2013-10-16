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

		#region Effect Drag/Drop

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
				MessageBox.Show("Currently, you must drag this item to the grid to place an effect.");
			}
		}

		#endregion

		private void treeEffects_AfterSelect(object sender, TreeViewEventArgs e)
		{

		}

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

	}
}
