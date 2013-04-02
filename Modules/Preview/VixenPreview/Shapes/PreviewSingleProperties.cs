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
    public partial class PreviewSingleProperties : Form
    {
        private PreviewSingle _single;

        public PreviewSingleProperties(PreviewSingle singleLight)
        {
            _single = singleLight;
            InitializeComponent();
        }

        private void PreviewSingleProperties_Load(object sender, EventArgs e)
        {
            PreviewTools.PopulateElementTree(treeElements);
            UpdateListLinkedElements();
            SetLinkedElementItem(_single._pixels[0].Node);
            numericUpDownPixelSize.Value = _single.PixelSize;
            trackBarAlpha.Value = _single.MaxAlpha;
        }

        private void treeElements_DoubleClick(object sender, EventArgs e)
        {
            CopySelectedNodeToOutput();
        }

        private void CopySelectedNodeToOutput()
        {
            if (treeElements.SelectedNode != null)
            {
                TreeNode treeNode = treeElements.SelectedNode;

                if (listLinkedElements.SelectedItems.Count > 0)
                {
                    ElementNode channelNode = treeNode.Tag as ElementNode;
                    ListViewItem item = listLinkedElements.SelectedItems[0];
                    item.Tag = channelNode;
                    Console.WriteLine(item.SubItems.Count.ToString());
                    if (item.SubItems.Count > 1)
                    {
                        Console.WriteLine("Item Updated");
                        item.SubItems[1].Text = channelNode.Name;
                    }
                    else
                    {
                        Console.WriteLine("Item Added");
                        item.SubItems.Add(channelNode.Name);
                    }
                }
            }
        }

        private void buttonLinkElements_Click(object sender, EventArgs e)
        {
            CopySelectedNodeToOutput();
        }

        private void SetLinkedElementItem(ElementNode channelNode)
        {
            ListViewItem item = listLinkedElements.Items[0];
            item.Tag = channelNode;
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

        private void UpdateListLinkedElements()
        {
            int newLightCount = 1;
            while (listLinkedElements.Items.Count < newLightCount)
            {
                ListViewItem item = new ListViewItem((listLinkedElements.Items.Count + 1).ToString());
                listLinkedElements.Items.Add(item);
            }

        }

        private void treeElements_ItemDrag(object sender, ItemDragEventArgs e)
        {
            Console.WriteLine(e.Item.GetType().ToString());
            treeElements.DoDragDrop(e.Item, DragDropEffects.Copy);
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
            ListViewItem item = listLinkedElements.GetItemAt(targetPoint.X, targetPoint.Y);
            if (item != null)
                item.Selected = true;
        }

        private void listLinkedElements_DragDrop(object sender, DragEventArgs e)
        {
            TreeNode treeNode = (TreeNode)e.Data.GetData(typeof(TreeNode)); ;
            if (treeNode != null)
            {
                ElementNode channelNode = treeNode.Tag as ElementNode;

                SetLinkedElementItem(channelNode);
            } else 
            {
                MessageBox.Show("treeNode==null!");
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            _single.PixelSize = (int)numericUpDownPixelSize.Value;
            _single.Pixels[0].Node = listLinkedElements.Items[0].Tag as ElementNode;
            _single.MaxAlpha = trackBarAlpha.Value;
            Close();
        }

        private void numericUpDownPixelSize_ValueChanged(object sender, EventArgs e)
        {
            ShowPreview();
        }

        public void ShowPreview()
        {
            Rectangle rect = new Rectangle();
            rect.X = (int)Math.Round((pictureSample.Width / 2) - (numericUpDownPixelSize.Value / 2));
            rect.Width = (int)numericUpDownPixelSize.Value;
            rect.Y = (int)Math.Round((pictureSample.Height / 2) - (numericUpDownPixelSize.Value / 2));
            rect.Height = (int)numericUpDownPixelSize.Value;

            pictureSample.Image = new Bitmap(pictureSample.Width, pictureSample.Height);
            Graphics g = Graphics.FromImage(pictureSample.Image);
            SolidBrush brush = new SolidBrush(Color.FromArgb(trackBarAlpha.Value, Color.White));
            g.FillEllipse(brush, rect);
        }

        private void trackBarAlpha_Scroll(object sender, EventArgs e)
        {
            //ShowPreview();
        }

        private void trackBarAlpha_ValueChanged(object sender, EventArgs e)
        {
            ShowPreview();
        }

        private void treeElements_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
    }
}
