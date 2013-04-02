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
    public partial class PreviewLineProperties : Form
    {
        private PreviewLine _line;

        public PreviewLineProperties(PreviewLine line)
        {
            InitializeComponent();
            _line = line;
        }

        private void PreviewLineProperties_Load(object sender, EventArgs e)
        {
            //PopulateElements();
            PreviewTools.PopulateElementTree(treeElements);

            numericLightCount.Value = _line.PixelCount;

            int itemNum = 0;
            foreach (PreviewPixel pixel in _line.Pixels)
            {
                SetLinkedElementItem(itemNum, pixel.Node);
                itemNum++;
            }
        }

        //// 
        //// Add the root nodes to the Display Element tree
        ////
        //private void PopulateElements()
        //{
        //    foreach (ElementNode channel in VixenSystem.Nodes.GetRootNodes())
        //    {
        //        AddNodeToElementTree(treeElements.Nodes, channel);
        //    }
        //}

        //// 
        //// Add each child Display Element or Display Element Group to the tree
        //// 
        //private void AddNodeToElementTree(TreeNodeCollection collection, ElementNode channelNode)
        //{
        //    TreeNode addedNode = new TreeNode();
        //    addedNode.Name = channelNode.Id.ToString();
        //    addedNode.Text = channelNode.Name;
        //    addedNode.Tag = channelNode;

        //    collection.Add(addedNode);

        //    foreach (ElementNode childNode in channelNode.Children)
        //    {
        //        AddNodeToElementTree(addedNode.Nodes, childNode);
        //    }
        //}

        private void treeElements_DoubleClick(object sender, EventArgs e)
        {
            if (treeElements.SelectedNode != null)
            {
                //listLinkedElements.Items.Add(treeElements.SelectedNode);
                TreeNode treeNode = treeElements.SelectedNode;
                //treeElements.Nodes.Remove(treeNode);s
                //treeLinkedElements.Nodes.Add(treeNode);

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
                    //_line.pixels[listLinkedElements.SelectedItems[0].Index].Node = channelNode;
                }
            }
        }

        private void numericLightCount_ValueChanged(object sender, EventArgs e)
        {
            UpdateListLinkedElements();
        }

        private void UpdateListLinkedElements()
        {
            int newLightCount = (int)numericLightCount.Value;
            if (radioMono.Checked)
                newLightCount = 1;

            while (listLinkedElements.Items.Count > newLightCount)
            {
                listLinkedElements.Items[listLinkedElements.Items.Count-1].Remove();
            }
            while (listLinkedElements.Items.Count < newLightCount)
            {
                ListViewItem item = new ListViewItem((listLinkedElements.Items.Count+1).ToString());
                listLinkedElements.Items.Add(item);
            }

        }

        private void treeElements_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void radioPixel_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void radioMono_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            SaveDisplayItem();
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void buttonLinkElements_Click(object sender, EventArgs e)
        {
            TreeNode treeNode = treeElements.SelectedNode;
            if (treeNode != null)
            {
                // Is this a group?
                if (treeNode.Nodes.Count > 0)
                {
                    int currentNodeNum = 0;
                    foreach (TreeNode childNode in treeNode.Nodes)
                    {
                        if (currentNodeNum < numericLightCount.Value)
                        {
                            // Make sure this is not a group
                            if (childNode.Nodes.Count == 0)
                            {
                                //Console.WriteLine(item.SubItems.Count.ToString());
                                //Console.WriteLine("Item Updated");
                                //ChannelNode channelNode = childNode.Tag as ChannelNode;
                                //ListViewItem item = listLinkedElements.Items[currentNodeNum];
                                //item.Tag = channelNode;
                                //item.SubItems[1].Text = channelNode.Name;

                                SetLinkedElementItem(currentNodeNum, childNode.Tag as ElementNode);
                            }
                        }
                        currentNodeNum++;
                    }
                }
            }
        }

        private void SetLinkedElementItem(int itemNum, ElementNode channelNode)
        {
            Console.WriteLine("c: " + listLinkedElements.Items.Count.ToString() + " a:" + itemNum.ToString());
            if (itemNum < listLinkedElements.Items.Count)
            {
                ListViewItem item = listLinkedElements.Items[itemNum];
                item.Tag = channelNode;
                //Console.WriteLine(item.SubItems.Count.ToString());
                if (item.SubItems.Count > 1)
                {
                    //Console.WriteLine("Item Updated");
                    item.SubItems[1].Text = channelNode.Name;
                }
                else
                {
                    //Console.WriteLine("Item Added");
                    if (channelNode != null)
                    {
                        item.SubItems.Add(channelNode.Name);
                    }
                }

                //_line.pixels[item.Index].Node = channelNode;
            }
        }

        private void SaveDisplayItem() {
            while (_line.Pixels.Count > numericLightCount.Value)
            {
                _line.Pixels.Remove(_line.Pixels.Last());
            }
            while (_line.Pixels.Count < numericLightCount.Value)
            {
                PreviewPixel pixel = _line.AddPixel(10, 10);
                pixel.PixelColor = Color.White;
            }

            int pixelNum = 0;
            foreach (ListViewItem item in listLinkedElements.Items)
            {
                //Pixels[pixelNum].Node
                _line.SetPixelNode(pixelNum, item.Tag as ElementNode);
                pixelNum++;
            }

            _line.Layout();
        }
    }
}
