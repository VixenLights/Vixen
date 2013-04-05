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
    public partial class PreviewSetElements : Form
    {
        List<PreviewSetElementString> _strings = new List<PreviewSetElementString>();
        List<PreviewBaseShape> _shapes;
        bool connectStandardStrings;

        public PreviewSetElements(List<PreviewBaseShape> shapes)
        {
            InitializeComponent();
            _shapes = shapes;
            connectStandardStrings = shapes[0].connectStandardStrings;
            int i = 1;
            foreach (PreviewBaseShape shape in _shapes)
            {
                PreviewSetElementString newString = new PreviewSetElementString();
                // If this is a Standard string, only set the first pixel of the string
                if (shape.StringType == PreviewBaseShape.StringTypes.Standard)
                {
                    PreviewPixel pixel = shape.Pixels[0]; ;
                    newString.Pixels.Add(pixel.Clone());
                }
                // If this is a pixel string, let them set every pixel
                else
                {
                    foreach (PreviewPixel pixel in shape.Pixels)
                    {
                        newString.Pixels.Add(pixel.Clone());
                    }
                }
                newString.StringName = "String " + i.ToString();
                _strings.Add(newString);
                i++;
            }
        }

        private void PreviewSetElements_Load(object sender, EventArgs e)
        {
            PreviewTools.PopulateElementTree(treeElements);
            PopulateStringList();
            UpdateListLinkedElements();
        }

        private void PopulateStringList()
        {
            if (connectStandardStrings && _shapes[0].StringType == PreviewBaseShape.StringTypes.Standard)
            {
                comboStrings.Items.Add(new ComboBoxItem(_strings[0].StringName, _strings[0]));
            }
            else
            {
                foreach (PreviewSetElementString lightString in _strings)
                {
                    comboStrings.Items.Add(new ComboBoxItem(lightString.StringName, lightString));
                }
            }
            if (_strings.Count > 0)
                comboStrings.SelectedIndex = 0;
        }

        public void AddString(string stringName, List<PreviewPixel> pixels)
        {
            PreviewSetElementString lightString = new PreviewSetElementString();
            lightString.StringName = stringName;
            for (int i = 0; i < pixels.Count; i++) 
            {
                lightString.Pixels.Add(pixels[i].Clone());
            }
            _strings.Add(lightString);
        }
        
        private void treeElements_ItemDrag(object sender, ItemDragEventArgs e)
        {
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

        private void UpdateListLinkedElements()
        {
            listLinkedElements.Items.Clear();
            ComboBoxItem comboBoxItem = comboStrings.SelectedItem as ComboBoxItem;
            if (comboBoxItem != null)
            {
                PreviewSetElementString elementString = comboBoxItem.Value as PreviewSetElementString;
                if (elementString != null)
                {
                    foreach (PreviewPixel pixel in elementString.Pixels)
                    {
                        ListViewItem item = new ListViewItem((listLinkedElements.Items.Count + 1).ToString());
                        item.Tag = pixel;
                        listLinkedElements.Items.Add(item);
                        SetLinkedElementItems(item, pixel.Node);
                    }
                }
                else
                {
                    Console.WriteLine("elementString==null");
                }
            }
        }

        private void comboStrings_SelectedValueChanged(object sender, EventArgs e)
        {
        }

        private void comboStrings_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Console.Write(comboStrings.SelectedIndex);
            UpdateListLinkedElements();
        }

        private void SetLinkedElementItems(ListViewItem item, ElementNode channelNode)
        {
            //ListViewItem item = listLinkedElements.Items[0];
            PreviewPixel pixel = item.Tag as PreviewPixel;

            if (channelNode != null)
            {
                // Is this node an element?
                if (channelNode.Element != null)
                {
                    pixel.Node = channelNode;
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
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (connectStandardStrings && _shapes[0].StringType == PreviewBaseShape.StringTypes.Standard)
            {
                PreviewBaseShape shape = _shapes[0];
                for (int i = 0; i < _shapes.Count; i++)
                {
                    foreach (PreviewPixel pixel in _shapes[i]._pixels)
                    {
                        pixel.Node = _strings[0].Pixels[0].Node;
                        pixel.NodeId = _strings[0].Pixels[0].NodeId;
                    }
                }
            }
            else
            {
                for (int i = 0; i < _shapes.Count; i++)
                {
                    ComboBoxItem item = comboStrings.Items[i] as ComboBoxItem;
                    PreviewSetElementString lightString = item.Value as PreviewSetElementString;
                    PreviewBaseShape shape = _shapes[i];
                    for (int pixelNum = 0; pixelNum < lightString.Pixels.Count; pixelNum++)
                    {
                        // If this is a standard light string, assing ALL pixels to the first node
                        if (shape.StringType == PreviewBaseShape.StringTypes.Standard)
                        {
                            foreach (PreviewPixel pixel in shape._pixels)
                            {
                                pixel.Node = lightString.Pixels[0].Node;
                                pixel.NodeId = lightString.Pixels[0].NodeId;
                            }
                        }
                        else
                        {
                            shape.Pixels[pixelNum] = lightString.Pixels[pixelNum];
                        }
                    }
                }
            }
            
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

    }
}
