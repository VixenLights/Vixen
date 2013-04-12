using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Vixen.Data.Flow;
using Vixen.Module;
using Vixen.Module.OutputFilter;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace VixenModules.Preview.VixenPreview
{
    public partial class VixenPreviewSetupElementsDocument : DockContent
    {
        private VixenPreviewControl _preview;

        public VixenPreviewSetupElementsDocument(VixenPreviewControl preview)
        {
            InitializeComponent();
            _preview = preview;
            _preview.OnSelectDisplayItem += OnSelectDisplayItem;
            _preview.OnDeSelectDisplayItem += OnDeSelectDisplayItem;
        }

        private void VixenPreviewSetupElementsDocument_Load(object sender, EventArgs e)
        {
            Shapes.PreviewTools.PopulateElementTree(treeElements);
        }

        private void OnDeSelectDisplayItem(object sender, Shapes.DisplayItem displayItem)
        {
            treeElements.SelectedNodes = null;
        }

        //Dictionary<ElementNode, TreeNode> ElementNodeToTreeNode = new Dictionary<ElementNode, TreeNode>();
        //private void CreateElementNodeToTreeNode()
        //{
        //    ElementNodeToTreeNode.Clear();
        //    AddElementNodesToTreeNode(treeElements.Nodes);
        //}
        //private void AddElementNodesToTreeNode(TreeNodeCollection treeNodes)
        //{
        //    foreach (TreeNode treeNode in treeNodes)
        //    {
        //        ElementNodeToTreeNode.Add(treeNode.Tag as ElementNode, treeNode);
        //        AddElementNodesToTreeNode(treeNode.Nodes);
        //    }
        //}

        //
        //
        // This is just very slow, so I've disabled it...
        //
        private void OnSelectDisplayItem(object sender, Shapes.DisplayItem displayItem)
        {
            treeElements.SelectedNodes = null;

            //CreateElementNodeToTreeNode();

            TreeNode visibleNode = null;
            foreach (Shapes.PreviewPixel pixel in displayItem.Shape.Pixels)
            {
                if (pixel.Node != null)
                {
                    //TreeNode treeNode;
                    //if (ElementNodeToTreeNode.TryGetValue(pixel.Node, out treeNode))
                    //{
                    //    treeElements.AddSelectedNode(treeNode);
                    //    visibleNode = treeNode;
                    //}
                    //TreeNode[] nodes = treeElements.Nodes.Find(pixel.Node.Id.ToString(), true);
                    //foreach (TreeNode node in nodes)
                    //{
                    //    visibleNode = node;
                    //    treeElements.AddSelectedNode(node);
                    //}

                    //TreeNode node = treeElements.Nodes[pixel.Node.Id.ToString()];
                    //if (node != null)
                    //{
                    //    visibleNode = node;
                    //    treeElements.AddSelectedNode(node);
                    //}
                }
            }
            if (visibleNode != null)
            {
                bool selectParent = true;
                foreach (TreeNode node in visibleNode.Parent.Nodes)
                {
                    selectParent = (selectParent && treeElements.SelectedNodes.Contains(node));
                }
                if (selectParent)
                {
                    treeElements.SelectedNodes = null;
                    treeElements.AddSelectedNode(visibleNode.Parent);
                    visibleNode.Parent.Collapse();
                    visibleNode.Parent.EnsureVisible();
                }
                else
                {
                    visibleNode.EnsureVisible();
                }
            }
        }

        private void HighlightNode(TreeNode node)
        {
            // Is this a group?
            if (node.Nodes.Count > 0)
            {
                // If so, iterate through children and highlight them
                foreach (TreeNode childNode in node.Nodes)
                {
                    HighlightNode(childNode);
                }
            }
            // Finally, highlight the node passed to us
            _preview.HighlightedElements.Add(node.Tag as ElementNode);
            _preview.DeSelectSelectedDisplayItemNoNotify();
        }

        private void treeElements_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _preview.HighlightedElements.Clear();

            foreach (TreeNode node in treeElements.SelectedNodes)
            {
                HighlightNode(node);
            }
        }

        private void treeElements_DragDrop(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
        }

        private void treeElements_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
        }

        private void treeElements_DragFinishing(object sender, Common.Controls.DragFinishingEventArgs e)
        {
            e.FinishDrag = false;
        }

        private void treeElements_MouseClick(object sender, MouseEventArgs e)
        {
            _preview.HighlightedElements.Clear();
        }

        public ElementNode SelectedNode {
            get 
            {
                if (treeElements.SelectedNode != null)
                {
                    return treeElements.SelectedNode.Tag as ElementNode;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
