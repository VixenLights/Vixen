using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Data.Flow;
using Vixen.Module;
using Vixen.Module.OutputFilter;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace VixenModules.Preview.VixenPreview.Shapes
{
    public partial class PreviewMegaTreeProperties : Form
    {
        private PreviewMegaTree _tree;

        public PreviewMegaTreeProperties(PreviewMegaTree tree)
        {
            InitializeComponent();
            _tree = tree;

            numericUpDownBaseHeight.Minimum = trackBarBaseHeight.Minimum;
            numericUpDownBaseHeight.Maximum = trackBarBaseHeight.Maximum;
            numericUpDownTopHeight.Minimum = trackBarTopHeight.Minimum;
            numericUpDownTopHeight.Maximum = trackBarTopHeight.Maximum;
            numericUpDownDegrees.Minimum = trackBarDegrees.Minimum;
            numericUpDownDegrees.Maximum = trackBarDegrees.Maximum;
            numericUpDownTopWidth.Minimum = trackBarTopWidth.Minimum;
            numericUpDownTopWidth.Maximum = trackBarTopWidth.Maximum;
            numericUpDownLightsPerString.Minimum = trackBarLightsPerString.Minimum;
            numericUpDownLightsPerString.Maximum = trackBarLightsPerString.Maximum;
            numericUpDownStringCount.Minimum = trackBarStringCount.Minimum;
            numericUpDownStringCount.Maximum = trackBarStringCount.Maximum;

            trackBarBaseHeight.Value = _tree.BaseHeight;
            numericUpDownBaseHeight.Value = _tree.BaseHeight;
            trackBarTopHeight.Value = _tree.TopHeight;
            numericUpDownTopHeight.Value = _tree.TopHeight;
            trackBarDegrees.Value = _tree.Degrees;
            numericUpDownDegrees.Value = _tree.Degrees;
            trackBarTopWidth.Value = _tree.TopWidth;
            numericUpDownTopWidth.Value = _tree.TopWidth;
            trackBarLightsPerString.Value = _tree.LightsPerString;
            numericUpDownLightsPerString.Value = _tree.LightsPerString;
            trackBarStringCount.Value = _tree.StringCount;
            numericUpDownStringCount.Value = _tree.StringCount;
        }

        public PreviewMegaTree Tree
        {
            get { return _tree; }
        }

        private void PreviewMegaTreeProperties_Load(object sender, EventArgs e)
        {
            PreviewTools.PopulateElementTree(treeElements);
            PopulateStringList();
            UpdateListLinkedElements();
        }

        private void PopulateStringList()
        {
            comboStrings.Items.Add("String 1");
            comboStrings.SelectedIndex = 0;
        }

        private void treeElements_DoubleClick(object sender, EventArgs e)
        {
            //CopySelectedNodeToOutput();
        }

        private void treeElements_ItemDrag(object sender, ItemDragEventArgs e)
        {
            //Console.WriteLine(e.Item.GetType().ToString());
            //treeElements.DoDragDrop(e.Item, DragDropEffects.Copy);

            //List<TreeNode> nodes = new List<TreeNode>();
            treeElements.DoDragDrop(treeElements.SelectedNodes, DragDropEffects.Copy);

        }

        private void listLinkedElements_DragDrop(object sender, DragEventArgs e)
        {
            List<TreeNode> nodes = (List<TreeNode>)e.Data.GetData(typeof(List<TreeNode>));

            // Retrieve the client coordinates of the mouse position.
            Point targetPoint = listLinkedElements.PointToClient(new Point(e.X, e.Y));
            // Select the node at the mouse position.
            ListViewItem item = listLinkedElements.GetItemAt(targetPoint.X, targetPoint.Y);

            foreach (TreeNode treeNode in nodes)
            {
                //TreeNode treeNode = (TreeNode)e.Data.GetData(typeof(TreeNode)); ;
                if (treeNode != null)
                {
                    ElementNode channelNode = treeNode.Tag as ElementNode;

                    SetLinkedElementItems(item, channelNode);

                    if (item.Index == listLinkedElements.Items.Count - 1)
                    {
                        return;
                    }
                    else
                    {
                        item = listLinkedElements.Items[item.Index + 1];
                    }
                }
                else
                {
                    MessageBox.Show("treeNode==null!");
                }
            }
        }

        private void listLinkedElements_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        private void listLinkedElements_DragOver(object sender, DragEventArgs e)
        {
            // Retrieve the client coordinates of the mouse position.
            Point targetPoint = listLinkedElements.PointToClient(new Point(e.X, e.Y));
            // Select the node at the mouse position.
            ListViewItem itemToSelect = listLinkedElements.GetItemAt(targetPoint.X, targetPoint.Y);
            foreach (ListViewItem item in listLinkedElements.Items)
            {
                if (itemToSelect == item)
                    item.Selected = true;
                else
                    item.Selected = false;
            }
        }

        //private void CopySelectedNodeToOutput()
        //{
        //    if (treeElements.SelectedNode != null)
        //    {
        //        TreeNode treeNode = treeElements.SelectedNode;

        //        if (listLinkedElements.SelectedItems.Count > 0)
        //        {
        //            ElementNode channelNode = treeNode.Tag as ElementNode;
        //            ListViewItem item = listLinkedElements.SelectedItems[0];
        //            item.Tag = channelNode;
        //            Console.WriteLine(item.SubItems.Count.ToString());
        //            if (item.SubItems.Count > 1)
        //            {
        //                Console.WriteLine("Item Updated");
        //                item.SubItems[1].Text = channelNode.Name;
        //            }
        //            else
        //            {
        //                Console.WriteLine("Item Added");
        //                item.SubItems.Add(channelNode.Name);
        //            }
        //        }
        //    }
        //}

        private void SetLinkedElementItems(ListViewItem item, ElementNode channelNode)
        {
            //ListViewItem item = listLinkedElements.Items[0];

            // Is this node an element?
            if (channelNode.Element != null)
            {
                item.Tag = channelNode;
                if (item != null)
                {
                    if (item.SubItems.Count > 1)
                    {
                        item.SubItems[1].Text = channelNode.Name;
                    }
                    else
                    {
                        if (channelNode != null)
                        {
                            item.SubItems.Add(channelNode.Name);
                        }
                    }
                }
            }
            else // This node is a group, iterate
            {
                ListViewItem nextItem = item;
                foreach (ElementNode child in channelNode.Children)
                {
                    if (item.Index < listLinkedElements.Items.Count && child.Element != null)
                    {
                        SetLinkedElementItems(nextItem, child);
                    //}
                    //else
                    //{
                    //    SetLinkedElementItems(nextItem, child);
                        if (nextItem.Index == listLinkedElements.Items.Count - 1)
                        {
                            return;
                        }
                        else
                        {
                            nextItem = listLinkedElements.Items[nextItem.Index + 1];
                        }
                    }
                }
            }
        }

        private void UpdateListLinkedElements()
        {
            foreach (PreviewBaseShape line in _tree._strings)
            {
                ListViewItem item = new ListViewItem((listLinkedElements.Items.Count + 1).ToString());
                listLinkedElements.Items.Add(item);
            }
            //while (listLinkedElements.Items.Count < newLightCount)
            //{
            //    ListViewItem item = new ListViewItem((listLinkedElements.Items.Count + 1).ToString());
            //    listLinkedElements.Items.Add(item);
            //}

        }
    
        private void pictureBoxSample_Paint(object sender, PaintEventArgs e)
        {
            const int TopMargin = 10;
            const int LeftMargin = 10;

            _tree.SetTopLeft(LeftMargin, TopMargin);
            _tree.SetBottomRight(pictureBoxSample.Width - (LeftMargin * 2), pictureBoxSample.Height - (TopMargin * 2));
            _tree.Layout();
            //_tree.Draw(e.Graphics, Color.White);
        }

        private void numericUpDownStringCount_ValueChanged(object sender, EventArgs e)
        {
            _tree.StringCount = (int)numericUpDownStringCount.Value;
            //_tree.Layout();
            pictureBoxSample.Invalidate();
        }

        private void numericUpDownTopHeight_ValueChanged(object sender, EventArgs e)
        {
            _tree.TopHeight = (int)numericUpDownTopHeight.Value;
            //_tree.Layout();
            pictureBoxSample.Invalidate();
        }

        private void numericUpDownTopWidth_ValueChanged(object sender, EventArgs e)
        {
            _tree.TopWidth = (int)numericUpDownTopWidth.Value;
            //_tree.Layout();
            pictureBoxSample.Invalidate();
        }

        private void numericUpDownBaseHeight_ValueChanged(object sender, EventArgs e)
        {
            _tree.BaseHeight = (int)numericUpDownBaseHeight.Value;
            //_tree.Layout();
            pictureBoxSample.Invalidate();
        }

        private void trackBarBaseHeight_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownBaseHeight.Value = trackBarBaseHeight.Value;
        }

        private void trackBarTopHeight_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownTopHeight.Value = trackBarTopHeight.Value;
        }

        private void trackBarTopWidth_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownTopWidth.Value = trackBarTopWidth.Value;
        }

        private void trackBarDegrees_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownDegrees.Value = trackBarDegrees.Value;
        }

        private void trackBarStringCount_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownStringCount.Value = trackBarStringCount.Value;
        }

        private void trackBarLightsPerString_ValueChanged(object sender, EventArgs e)
        {
            numericUpDownLightsPerString.Value = trackBarLightsPerString.Value;
        }

        private void numericUpDownDegrees_ValueChanged(object sender, EventArgs e)
        {
            _tree.Degrees = (int)numericUpDownDegrees.Value;
            //_tree.Layout();
            pictureBoxSample.Invalidate();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void treeElements_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void trackBarDegrees_Scroll(object sender, EventArgs e)
        {

        }


    }
}
