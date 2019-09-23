using Common.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Resources;
using Common.Resources.Properties;
using Vixen.Sys;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	public partial class PreviewSetElements : BaseForm
    {
        private List<PreviewSetElementString> _strings = new List<PreviewSetElementString>();
        private List<PreviewBaseShape> _shapes;
        private bool connectStandardStrings;
        private const string VirtualNodeName = @"VIRT";

        public PreviewSetElements(List<PreviewBaseShape> shapes)
        {
			InitializeComponent();
			Icon = Resources.Icon_Vixen3;
			ThemeUpdateControls.UpdateControls(this);
	        contextMenuLinkedElements.Renderer = new ThemeToolStripRenderer();
	        int imageSize = (int)(16 * ScalingTools.GetScaleFactor());
	        contextMenuLinkedElements.ImageScalingSize = new Size(imageSize, imageSize);
	        buttonHelp.Image = Tools.GetIcon(Resources.help, imageSize);
			_shapes = shapes;
            connectStandardStrings = shapes[0].connectStandardStrings;
            int i = 1;
            foreach (PreviewBaseShape shape in _shapes)
            {
                if (shape.Pixels.Count == 0)
                    continue;
                var newString = new PreviewSetElementString();
                // If this is a Standard string, only set the first pixel of the string
                if (shape.StringType == PreviewBaseShape.StringTypes.Standard)
                {
                    //Console.WriteLine("Standard String");
                    PreviewPixel pixel = shape.Pixels[0];
                    //Console.WriteLine(shape.Pixels[0].Node.Name.ToString());
                    newString.Pixels.Add(pixel.Clone());
                }
                // If this is a pixel string, let them set every pixel
                else if (shape.StringType == PreviewBaseShape.StringTypes.Pixel)
                {
                    foreach (PreviewPixel pixel in shape.Pixels)
                    {
                        newString.Pixels.Add(pixel.Clone());
                    }
                }else if (shape.StringType == PreviewBaseShape.StringTypes.Custom)
                {
	                foreach (var pixel in shape.Pixels)
	                {
						newString.Pixels.Add(pixel.Clone());
					}
                }

                newString.StringName = "String " + i.ToString();
                _strings.Add(newString);
                i++;
            }

            if (_shapes[0].Parent != null)
            {
	            var shapeType = _shapes[0].Parent.GetType().ToString();
	            if ((shapeType.Contains("Icicle") && _shapes[0].StringType != PreviewBaseShape.StringTypes.Standard) || shapeType.Contains("MultiString") )
                {
                    tblLightCountControls.Visible = true;
                }
            }
        }

        private void PreviewSetElements_Load(object sender, EventArgs e)
        {
            PopulateElementTree(treeElements);
			treeElements.BeforeExpand += TreeElements_BeforeExpand;
            PopulateStringList();
            UpdateListLinkedElements();
        }

		private void TreeElements_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			if (e.Node.Tag is ElementNode elementNode)
			{
				if (elementNode.Children.Any() && e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Name.Equals(VirtualNodeName))
				{
					AddChildrenToTree(e.Node, elementNode);
				}
			}
		}

		// 
		// Add the root nodes to the Display Element tree
		//
		private static void PopulateElementTree(TreeView tree)
        {
	        foreach (ElementNode channel in VixenSystem.Nodes.GetRootNodes()) {
		        AddNodeToElementTree(tree.Nodes, channel);
	        }
        }

        // 
        // Add each child Display Element or Display Element Group to the tree
        // 
        private static void AddNodeToElementTree(TreeNodeCollection collection, ElementNode elementNode)
        {
	        TreeNode addedNode = new TreeNode();
	        addedNode.Name = elementNode.Id.ToString();
	        addedNode.Text = elementNode.Name;
	        addedNode.Tag = elementNode;
	        collection.Add(addedNode);

	        if(elementNode.Children.Any())
	        {
		        TreeNode virtNode = new TreeNode();
		        virtNode.Name = VirtualNodeName;
		        addedNode.Nodes.Add(virtNode);
	        }
        }

        private void AddChildrenToTree(TreeNode node, ElementNode elementNode)
        {
	        node.Nodes.Clear();
	        foreach (ElementNode childNode in elementNode.Children)
	        {
		        AddNodeToElementTree(node.Nodes, childNode);
	        }
        }

        private void PopulateStringList()
        {
            if (connectStandardStrings && _shapes[0].StringType == PreviewBaseShape.StringTypes.Standard)
            {
                comboStrings.Items.Add(new Common.Controls.ComboBoxItem(_strings[0].StringName, _strings[0]));
            }
            else
            {
                foreach (PreviewSetElementString lightString in _strings)
                {
                    comboStrings.Items.Add(new Common.Controls.ComboBoxItem(lightString.StringName, lightString));
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
            treeElements.DoDragDrop(treeElements.SelectedNodes, DragDropEffects.Link);
        }

        private void listLinkedElements_DragDrop(object sender, DragEventArgs e)
        {
            List<TreeNode> nodes = (List<TreeNode>)e.Data.GetData(typeof(List<TreeNode>));

            // Retrieve the client coordinates of the mouse position.
            Point targetPoint = listLinkedElements.PointToClient(new Point(e.X, e.Y));
            // Select the node at the mouse position.
            ListViewItem item = listLinkedElements.GetItemAt(targetPoint.X, targetPoint.Y);
            if (item == null)
            {
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Elements must be dropped on a target.  Please try again.", "Error", false, false);
				messageBox.ShowDialog();
                return;
            }

            foreach (TreeNode treeNode in nodes)
            {
                if (treeNode != null)
                {
                    ElementNode channelNode = treeNode.Tag as ElementNode;

                    SetLinkedElementItems(item, channelNode);

                    if (item.Index == listLinkedElements.Items.Count - 1)
                    {
                        break;
                    }
                   
	                item = listLinkedElements.Items[item.Index + 1];
                }
                else
                {
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("treeNode==null!", "Error", false, false);
					messageBox.ShowDialog();
                }
            }
            AdjustColumnWidths();
        }

        private void listLinkedElements_DragEnter(object sender, DragEventArgs e)
        {

	        if(e.Data.GetDataPresent(typeof(List<TreeNode>)))
	        {
		        Point targetPoint = listLinkedElements.PointToClient(new Point(e.X, e.Y));
		        ListViewItem itemToSelect = listLinkedElements.GetItemAt(targetPoint.X, targetPoint.Y);
		        e.Effect = itemToSelect != null ? e.AllowedEffect : DragDropEffects.None;
			}else
	        {
		        e.Effect = DragDropEffects.None;
	        }

        }

        private void listLinkedElements_DragOver(object sender, DragEventArgs e)
        {
	        if(e.Data.GetDataPresent(typeof(List<TreeNode>)))
	        {
		        // Retrieve the client coordinates of the mouse position.
		        Point targetPoint = listLinkedElements.PointToClient(new Point(e.X, e.Y));
		        // Select the node at the mouse position.
		        ListViewItem itemToSelect = listLinkedElements.GetItemAt(targetPoint.X, targetPoint.Y);
		        listLinkedElements.SelectedItems.Clear();
		        if (itemToSelect != null)
		        {
			        e.Effect = e.AllowedEffect;
			        itemToSelect.Selected = true;
		        }
		        else
		        {
			        e.Effect = DragDropEffects.None;
		        }
	        }
	        else
	        {
		        e.Effect = DragDropEffects.None;
	        }
           
        }

        private void UpdateListLinkedElements()
        {
            listLinkedElements.Items.Clear();
            Common.Controls.ComboBoxItem comboBoxItem = comboStrings.SelectedItem as Common.Controls.ComboBoxItem;
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

	            var count = elementString.Pixels.Count();

				numericUpDownLightCount.Value = count>0?count:1;

	            AdjustColumnWidths();
            }
        }

	    private void AdjustColumnWidths()
	    {
		    if (listLinkedElements.Columns.Count > 1)
		    {
			    listLinkedElements.Columns[0].Width = -1;
			    listLinkedElements.Columns[1].Width = -1;
		    }
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
            if ((connectStandardStrings || _shapes.Count() == 1) && _shapes[0].StringType == PreviewBaseShape.StringTypes.Standard)
            {
                //var shape = _shapes[0];
                for (int i = 0; i < _shapes.Count; i++)
                {
                    foreach (var pixel in _shapes[i]._pixels)
                    {
                        pixel.Node = _strings[0].Pixels[0].Node;
                        pixel.NodeId = _strings[0].Pixels[0].NodeId;
                    }
                }
            }
            else
            {
                // shapes with count==0 don't show up in combo box so keep separate index
                var comboidx = -1;
                for (var i = 0; i < _shapes.Count; i++)
                {                    
                    if (_shapes[i].Pixels.Count == 0)
                        continue;
                    comboidx++;
                    var item = comboStrings.Items[comboidx] as Common.Controls.ComboBoxItem;
                    var lightString = item.Value as PreviewSetElementString;
                    var shape = _shapes[i];

                    if (shape.StringType == PreviewBaseShape.StringTypes.Pixel) { 
                        while (shape.Pixels.Count > lightString.Pixels.Count)
                        {
                            shape.Pixels.RemoveAt(shape.Pixels.Count - 1);
                        }
                        while (shape.Pixels.Count < lightString.Pixels.Count)
                        {
                            var pixel = new PreviewPixel();
                            shape.Pixels.Add(pixel);
                        }
                    }

                    for (int pixelNum = 0; pixelNum < lightString.Pixels.Count; pixelNum++)
                    {
                        //Console.WriteLine("   pixelNum=" + pixelNum.ToString());
                        // If this is a standard light string, assing ALL pixels to the first node
                        if (shape.StringType == PreviewBaseShape.StringTypes.Standard)
                        {
                            foreach (var pixel in shape._pixels)
                            {
                                //Console.WriteLine("       pixel:" + lightString.Pixels[0].Node.Id.ToString());
                                pixel.Node = _strings[i].Pixels[0].Node;
                                pixel.NodeId = _strings[i].Pixels[0].NodeId;
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

        private void treeElements_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (treeElements.SelectedNode != null && listLinkedElements.FocusedItem != null)
            {
                foreach (ListViewItem item in listLinkedElements.SelectedItems)
                {
                    SetLinkedElementItems(item, treeElements.SelectedNode.Tag as ElementNode);
                }
	            AdjustColumnWidths();
			}
        }

        private void copyToAllElementsAllStringsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem item = listLinkedElements.FocusedItem;
            if (item != null)
            {
                ElementNode node = (item.Tag as PreviewPixel).Node;
                for (int i = 0; i < _strings.Count; i++)
                {
                    foreach (PreviewPixel pixel in _strings[i].Pixels)
                    {
                        pixel.Node = node;
                    }
                }
                UpdateListLinkedElements();
            }
        }

        private void buttonHelp_Click(object sender, EventArgs e)
        {
            Common.VixenHelp.VixenHelp.ShowHelp(Common.VixenHelp.VixenHelp.HelpStrings.Preview_LinkElements);
        }

        private void copyToAllElementsInThisStringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem selectedItem = listLinkedElements.FocusedItem;
            if (selectedItem != null)
            {
                ElementNode node = (selectedItem.Tag as PreviewPixel).Node;
                for (int i = 0; i < _strings.Count; i++)
                {
                    foreach (ListViewItem item in listLinkedElements.Items)
                    {
                        (item.Tag as PreviewPixel).Node = (selectedItem.Tag as PreviewPixel).Node;
                    }
                }

                UpdateListLinkedElements();
            }
        }

        private void buttonSetLightCount_Click(object sender, EventArgs e)
        {
            Common.Controls.ComboBoxItem comboBoxItem = comboStrings.SelectedItem as Common.Controls.ComboBoxItem;
            if (comboBoxItem != null)
            {
                PreviewSetElementString elementString = comboBoxItem.Value as PreviewSetElementString;
                if (elementString != null)
                {
                    while (elementString.Pixels.Count > numericUpDownLightCount.Value)
                    {
                        elementString.Pixels.RemoveAt(elementString.Pixels.Count - 1);
                    }
                    while (elementString.Pixels.Count < numericUpDownLightCount.Value)
                    {
                        PreviewPixel pixel = new PreviewPixel();
                        elementString.Pixels.Add(pixel);
                    }
                }
                UpdateListLinkedElements();
            }
        }

        private void reverseElementLinkingInThisStringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ComboBoxItem comboBoxItem = comboStrings.SelectedItem as Common.Controls.ComboBoxItem;
            if (comboBoxItem != null)
            {
                PreviewSetElementString elementString = comboBoxItem.Value as PreviewSetElementString;
                if (elementString != null)
                {
                    elementString.Pixels.Reverse();
                }
                else
                {
                    Console.WriteLine("elementString==null");
                }
                numericUpDownLightCount.Value = elementString.Pixels.Count();
                UpdateListLinkedElements();
            }
        }

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImage;

		}

		private void comboBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			ThemeComboBoxRenderer.DrawItem(sender, e);
		}
    }
}