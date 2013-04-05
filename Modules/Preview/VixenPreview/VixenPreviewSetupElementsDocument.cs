using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

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
            //Console.WriteLine("Deselect Item: " + displayItem.Shape.ToString());
            //foreach (Shapes.PreviewPixel pixel in displayItem.Shape.Pixels)
            //{
            //    pixel.editColor = Color.White;
            //}
        }

        private void OnSelectDisplayItem(object sender, Shapes.DisplayItem displayItem)
        {
            //Console.WriteLine("Select Item: " + displayItem.Shape.ToString());
            treeElements.SelectedNodes.Clear();
            foreach (Shapes.PreviewPixel pixel in displayItem.Shape.Pixels)
            {
                if (pixel.Node != null)
                {
                    //Console.WriteLine("Node: " + pixel.Node.Name);
                    TreeNode[] nodes = treeElements.Nodes.Find(pixel.Node.Id.ToString(), true);
                    TreeNode visibleNode = null;
                    foreach (TreeNode node in nodes)
                    {
                        visibleNode = node;
                        treeElements.AddSelectedNode(node);
                    }
                }
            }
        }

        private void treeElements_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
    }
}
