using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using Vixen.Sys;
using Common.Controls;
using System.ComponentModel;

namespace VixenModules.SequenceType.Vixen2x
{
    public partial class Vixen2xSequenceImporterChannelMapper : Form
    {
        private MultiSelectTreeview treeview;


        public Vixen2xSequenceImporterChannelMapper(List<ChannelMapping> mappings)
        {
            InitializeComponent();

            Mappings = mappings;
        }

        public List<ChannelMapping> Mappings { get; set; }


        private void PopulateNodeTreeMultiSelect()
        {
            multiSelectTreeview1.BeginUpdate();
            multiSelectTreeview1.Nodes.Clear();

            foreach (ElementNode element in VixenSystem.Nodes.GetRootNodes())
            {
                AddNodeToTree(multiSelectTreeview1.Nodes, element);
            }

            multiSelectTreeview1.EndUpdate();
        }


        private void AddNodeToTree(TreeNodeCollection collection, ElementNode elementNode)
        {
            TreeNode addedNode = new TreeNode()
            {
                Name = elementNode.Id.ToString(),
                Text = elementNode.Name,
                Tag = elementNode
            };

            collection.Add(addedNode);

            foreach (ElementNode childNode in elementNode.Children)
            {
                AddNodeToTree(addedNode.Nodes, childNode);
            }
        }

        private void AddVixen3ElementToVixen2Channel(int startingIndex)
        {
            if (startingIndex + treeview.SelectedNodes.Count > listViewMapping.Items.Count)
            {
                //to many nodes were selected so we can't paste them into our control
                MessageBox.Show("Too many elements were selected, unable to paste into the allotted space.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                foreach (TreeNode node in treeview.SelectedNodes)
                {
                    ElementNode enode = (ElementNode)node.Tag;
                    ListViewItem item = listViewMapping.Items[startingIndex];

                    item.SubItems[4].Text = enode.Element.Name;

                    item.SubItems[4].Tag = enode;

                    startingIndex++;
                }
            }

        }

        private static String GetColorName(Color color)
        {
            var predefined = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static);
            var match = (from p in predefined where ((Color)p.GetValue(null, null)).ToArgb() == color.ToArgb() select (Color)p.GetValue(null, null));
            if (match.Any())
                return match.First().Name;
            return String.Empty;
        }

        private void Vixen2xSequenceImporterChannelMapper_Load(object sender, EventArgs e)
        {
            listViewMapping.BeginUpdate();

            listViewMapping.Items.Clear();

            foreach (ChannelMapping mapping in Mappings)
            {
                ListViewItem item = new ListViewItem(mapping.ChannelNumber) { UseItemStyleForSubItems = false };
                item.SubItems.Add(mapping.ChannelOutput);
                item.SubItems.Add(mapping.ChannelName);
                item.SubItems.Add(GetColorName(mapping.ChannelColor));
                item.SubItems[3].BackColor = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(GetColorName(mapping.ChannelColor));

                //just add empty items here so I don't have to mess with checking for them later
                item.SubItems.Add("");
                item.SubItems.Add("");

                listViewMapping.Items.Add(item);
            }
            listViewMapping.EndUpdate();

            PopulateNodeTreeMultiSelect();
        }

        #region Drag drop events
        private void listViewMapping_DragDrop(object sender, DragEventArgs e)
        {
            Point cp = listViewMapping.PointToClient(new Point(e.X, e.Y));
            int dropIndex = 0;

            if (listViewMapping.HitTest(cp).Location.ToString() == "None")
            {
                //probably need to do something here
            }
            else
            {
                ListViewItem dragToItem = listViewMapping.GetItemAt(cp.X, cp.Y);
                dropIndex = dragToItem.Index;
            }

            //Lets see how many nodes we brought over and if more than one let the user know they will
            //be added sequentially starting at the selected node.
            if (treeview.SelectedNodes.Count > 1)
            {
                DialogResult result = MessageBox.Show("You have selected more than one item to map.  Items will be mapped starting at the selected Vixen 2.x Channel", "Warning", MessageBoxButtons.OKCancel);
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    AddVixen3ElementToVixen2Channel(dropIndex);
                }
            }
            else
            {
                AddVixen3ElementToVixen2Channel(dropIndex);
            }

        }

        private void listViewMapping_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeNode)))
                e.Effect = DragDropEffects.Move | DragDropEffects.Copy;
        }

        private void multiSelectTreeview1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move | DragDropEffects.Copy;
        }

        private void multiSelectTreeview1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            treeview = (MultiSelectTreeview)sender;
            multiSelectTreeview1.DoDragDrop(e.Item, DragDropEffects.Move | DragDropEffects.Copy);
        }

        #endregion

        #region Button Events
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            //show message about cancelling

        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            //default these to white
            Color vixen2Color = Color.White;
            Color Vixen3Color = Color.White;

            Mappings = new List<ChannelMapping>();

            foreach (ListViewItem itemrow in listViewMapping.Items)
            {
                vixen2Color = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(itemrow.SubItems[3].Text);

                if (!String.IsNullOrEmpty(itemrow.SubItems[4].Text))
                {
                    ElementNode node = (ElementNode)itemrow.SubItems[4].Tag;

                    if (!String.IsNullOrEmpty(itemrow.SubItems[5].Text))
                    {
                        Vixen3Color = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(itemrow.SubItems[5].Text);
                    }

                    Mappings.Add(new ChannelMapping(itemrow.SubItems[2].Text,
                       vixen2Color,
                        itemrow.SubItems[0].Text,
                        itemrow.SubItems[1].Text,
                        node.Id,
                        Vixen3Color));

                }
                else
                {
                    Mappings.Add(new ChannelMapping(itemrow.SubItems[2].Text,
                      vixen2Color,
                       itemrow.SubItems[0].Text,
                       itemrow.SubItems[1].Text,
                       Guid.Empty,
                       Vixen3Color));
                }
            }
            Mappings = Mappings;
        }
        #endregion

        private void destinationColorButton_Click(object sender, EventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog();
            colorDlg.AllowFullOpen = true;
            colorDlg.AnyColor = true;
            colorDlg.SolidColorOnly = false;
            colorDlg.Color = Color.Red;
            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                foreach (ListViewItem itemrow in listViewMapping.SelectedItems)
                {
                    
                    itemrow.UseItemStyleForSubItems = false;
                    itemrow.SubItems[5].Text = GetColorName(colorDlg.Color);
                    itemrow.SubItems[5].BackColor = colorDlg.Color;

                }
            }
        }
    }
}
