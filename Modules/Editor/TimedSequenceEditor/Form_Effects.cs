using System;
using System.Linq;
using System.Windows.Forms;
using Common.Controls.Timeline;
using Vixen.Module.Effect;
using Vixen.Services;
using WeifenLuo.WinFormsUI.Docking;

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
			LoadAvailableEffects();
		}

		public TimelineControl TimelineControl { get; set; }

		private void LoadAvailableEffects()
		{
			foreach (
				IEffectModuleDescriptor effectDesriptor in
					ApplicationServices.GetModuleDescriptors<IEffectModuleInstance>().Cast<IEffectModuleDescriptor>())
			{
				// Add a button to the tool strip
				ToolStripItem tsItem = new ToolStripButton(effectDesriptor.EffectName);
				tsItem.Tag = effectDesriptor.TypeId;
				tsItem.MouseDown += toolStripEffects_Item_MouseDown;
				tsItem.MouseMove += toolStripEffects_Item_MouseMove;
				tsItem.Click += toolStripEffects_Item_Click;
				tsItem.Image = effectDesriptor.GetRepresentativeImage(48, 48);

				toolStripEffects.Items.Add(tsItem);

				// Add the effects to the tree
				TreeNode parentNode = treeEffects.Nodes["treeBasic"];
				TreeNode node = new TreeNode(effectDesriptor.EffectName);
				parentNode.Nodes.Add(node);
				// Set the image
				System.Drawing.Image image = effectDesriptor.GetRepresentativeImage(48, 48);
				if (image != null)
				{
					effectTreeImages.Images.Add(effectDesriptor.EffectName, image);
					node.ImageIndex = effectTreeImages.Images.Count - 1;
					node.SelectedImageIndex = node.ImageIndex;
					//node.StateImageKey = node.ImageKey;
				}
				else
				{
					SetNodeImage(node, "blank.png");
				}
			}
		}

		#region Effect Drag/Drop

		// http://sagistech.blogspot.com/2010/03/dodragdrop-prevent-doubleclick-event.html
		private bool _beginDragDrop;

		private void toolStripEffects_Item_MouseDown(object sender, MouseEventArgs e)
		{
			if ((e.Button == MouseButtons.Left) && (e.Clicks == 1))
				_beginDragDrop = true;
			else
				_beginDragDrop = false;
		}

		private void toolStripEffects_Item_MouseMove(object sender, MouseEventArgs e)
		{
			if ((e.Button == MouseButtons.Left) && _beginDragDrop)
			{
				_beginDragDrop = false;
				ToolStripItem item = sender as ToolStripItem;
				DataObject data = new DataObject(DataFormats.Serializable, item.Tag);
				item.GetCurrentParent().DoDragDrop(data, DragDropEffects.Copy);
			}
		}

		private void toolStripEffects_Item_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Currently, you must drag this item to the grid to place an effect.");
		}

		#endregion

		private void treeEffects_AfterSelect(object sender, TreeViewEventArgs e)
		{

		}

		private void treeEffects_MouseDown(object sender, MouseEventArgs e)
		{
			TreeNode node = treeEffects.GetNodeAt(e.X, e.Y);
			if (node.Nodes.Count > 0)
			{
				if (node.IsExpanded)
				{
					node.Collapse();
				}
				else
				{
					node.Expand();
				}
			}
		}

		private void treeEffects_AfterExpand(object sender, TreeViewEventArgs e)
		{
			Console.WriteLine("After1");
			SetNodeImage(e.Node, "bullet_arrow_down.png");
		}

		private void SetNodeImage(TreeNode node, string image) 
		{
			node.ImageKey = image;
			node.SelectedImageKey = node.ImageKey;
			//node.StateImageKey = node.ImageKey;
		}

		private void treeEffects_AfterCollapse(object sender, TreeViewEventArgs e)
		{
			Console.WriteLine("After");
			SetNodeImage(e.Node, "bullet_arrow_Right.png");
		}
	}
}
