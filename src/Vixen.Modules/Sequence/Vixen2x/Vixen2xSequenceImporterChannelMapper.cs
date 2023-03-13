using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using Vixen.Sys;
using System.ComponentModel;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.SequenceType.Vixen2x
{
	public partial class Vixen2xSequenceImporterChannelMapper : BaseForm
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		private MultiSelectTreeview treeview;
		private bool MapExists;
		private int startingIndex;

		/// <summary>
		/// Mapping of Column headers to make code maintinance easier
		/// </summary>
		private enum mapperColumnId
		{
			v2ChannelId = 0,
			v2channelOutput,
			v2ChannelName,
			v2channelColor,
			v3Destination,
			importColor,
			rgbPixel
		};

		/// <summary>
		/// Fixed Channel offset to RGB Pixel color translation
		/// </summary>
		private Dictionary<string, List<Color>> m_defaultPixelColors = new Dictionary<string, List<Color>>(); //Color[] { Color.Red, Color.Green, Color.Blue };


		public Vixen2xSequenceImporterChannelMapper(List<ChannelMapping> mappings, bool mapExists, string mappingName)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;
			listViewMapping.ForeColor = SystemColors.WindowText;

			Mappings = mappings;
			MapExists = mapExists;
			mappingNameTextBox.Text = mappingName;

			checkBoxRGB.Enabled = true;
			comboBoxColorOrder.SelectedIndex = 0;
			comboBoxColorOrder.Enabled = false;

			List<Color> colorList = new List<Color>();
			colorList.Add(Color.Red);
			colorList.Add(Color.Green);
			colorList.Add(Color.Blue);
			m_defaultPixelColors.Add("RGB", new List<Color>(colorList));

			colorList.Clear();
			colorList.Add(Color.Red);
			colorList.Add(Color.Blue);
			colorList.Add(Color.Green);
			m_defaultPixelColors.Add("RBG", new List<Color>(colorList));

			colorList.Clear();
			colorList.Add(Color.Blue);
			colorList.Add(Color.Red);
			colorList.Add(Color.Green);
			m_defaultPixelColors.Add("BRG", new List<Color>(colorList));

			colorList.Clear();
			colorList.Add(Color.Blue);
			colorList.Add(Color.Green);
			colorList.Add(Color.Red);
			m_defaultPixelColors.Add("BGR", new List<Color>(colorList));

			colorList.Clear();
			colorList.Add(Color.Green);
			colorList.Add(Color.Red);
			colorList.Add(Color.Blue);
			m_defaultPixelColors.Add("GRB", new List<Color>(colorList));

			colorList.Clear();
			colorList.Add(Color.Green);
			colorList.Add(Color.Blue);
			colorList.Add(Color.Red);
			m_defaultPixelColors.Add("GBR", new List<Color>(colorList));
		}

		public List<ChannelMapping> Mappings { get; set; }
		public string MappingName { get; set; }


		private void PopulateNodeTreeMultiSelect()
		{
			multiSelectTreeview1.BeginUpdate();
			multiSelectTreeview1.Nodes.Clear();

			foreach (ElementNode element in VixenSystem.Nodes.GetRootNodes()) {
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

			foreach (ElementNode childNode in elementNode.Children) {
				AddNodeToTree(addedNode.Nodes, childNode);
			}
		}

		private void AddVixen3ElementToVixen2Channel(TreeNode node)
		{
			// this is a bit of a dodgy hack to allow elements to be repeated when dragged to the grid.
			// we just loop until we've repeated each element X times, in order.
			int repeatCount = decimal.ToInt32(numericUpDownRepeatElements.Value);
			if (repeatCount <= 0)
			{
				repeatCount = 1;
			}

			// Logging.Info("AddVixen3ElementToVixen2Channel: repeatCount = " + repeatCount + ". checkBoxRGB.Checked = " + checkBoxRGB.Checked + " listViewMapping.Items.Count = " + listViewMapping.Items.Count);

			for (int i = 0; i < repeatCount; i++) 
			{
				// if the user drags a large number of items to start at a position that
				// doesn't have enough 'room' off the end for them all, this can go OOR
				if (listViewMapping.Items.Count <= startingIndex)
				{
					Logging.Error("AddVixen3ElementToVixen2Channel: Aborting because startingIndex " + startingIndex + " is greater than (or equal to) listViewMapping.Items.Count " + listViewMapping.Items.Count );
					break;
				}

				ElementNode enode = (ElementNode) node.Tag;
				ListViewItem item = listViewMapping.Items[startingIndex];

				item.SubItems[(int)mapperColumnId.v3Destination].Text = enode.Element.Name;

				item.SubItems[(int)mapperColumnId.v3Destination].Tag = enode;

				// are we processing an RGB Pixel?
				if(true == checkBoxRGB.Checked)
				{
					// use a fixed translation
					// MessageBox.Show("m_defaultPixelColors.Count: '" + m_defaultPixelColors.Count + "' comboBoxColorOrder.SelectedText: '" + comboBoxColorOrder.SelectedItem.ToString() + "' i: " + i + "'", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);					
					item.SubItems[(int)mapperColumnId.importColor].Text = m_defaultPixelColors[comboBoxColorOrder.SelectedItem.ToString()][i].Name;
					item.SubItems[(int)mapperColumnId.importColor].BackColor = m_defaultPixelColors[comboBoxColorOrder.SelectedItem.ToString()][i];
					item.SubItems[(int)mapperColumnId.rgbPixel].Text = "Yes";
				} // end process pixel
				else
				{
					//Not sure where to get a node color from Vixen 3 stuff so if we have one in Vixen 2 just use it
					item.SubItems[(int)mapperColumnId.importColor].Text = item.SubItems[(int)mapperColumnId.v2channelColor].Text;
					item.SubItems[(int)mapperColumnId.importColor].BackColor = item.SubItems[(int)mapperColumnId.v2channelColor].BackColor;
					item.SubItems[(int)mapperColumnId.rgbPixel].Text = string.Empty;
				} // end not a pixel

				startingIndex++;
			} // for each repeat element
		} // end AddVixen3ElementToVixen2Channel

		private void ParseNodes(List<TreeNode> nodes)
		{
			foreach (TreeNode node in nodes) {
				//if we have node nodes assocatied with the node then this is a child node
				//so lets get our information and add it.
				if (node.Nodes.Count == 0) {
					//We have a node with no children so let's add it to our listviewMapping
					AddVixen3ElementToVixen2Channel(node);
				}
				else {
					//lets parse it till we get to the child node
					ParseNode(node);
				}
			}
		}

		private void ParseNode(TreeNode node)
		{
			foreach (TreeNode tn in node.Nodes) {
				if (tn.Nodes.Count != 0) {
					ParseNode(tn);
				}
				else {
					AddVixen3ElementToVixen2Channel(tn);
				}
			}
		}

		private static String GetColorName(Color color)
		{
			var predefined = typeof (Color).GetProperties(BindingFlags.Public | BindingFlags.Static);
			var match = (from p in predefined
			             where ((Color) p.GetValue(null, null)).ToArgb() == color.ToArgb()
			             select (Color) p.GetValue(null, null));
			if (match.Any()) {
				return match.First().Name;
			}
			return String.Empty;
		}

		private void Vixen2xSequenceImporterChannelMapper_Load(object sender, EventArgs e)
		{
			listViewMapping.BeginUpdate();

			listViewMapping.Items.Clear();

			foreach (ChannelMapping mapping in Mappings) 
			{
				// create an empty row list
				ListViewItem item = new ListViewItem(mapping.ChannelNumber) {UseItemStyleForSubItems = false};

				// v2channelOutput
				item.SubItems.Add(mapping.ChannelOutput);

				// v2ChannelName
				item.SubItems.Add(mapping.ChannelName);

				// v2channelColor
				item.SubItems.Add(GetColorName(mapping.ChannelColor));
				// item.SubItems.Add(mapping.ChannelColor.Name);
				item.SubItems[(int)mapperColumnId.v2channelColor].BackColor = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(GetColorName(mapping.ChannelColor));
				// item.SubItems[(int)mapperColumnId.v2channelColor].BackColor = mapping.ChannelColor;

				// do we have an existing mapping?
				if (MapExists && mapping.ElementNodeId != Guid.Empty) 
				{
					// get access to the existing target node information
					ElementNode targetNode = VixenSystem.Nodes.GetElementNode(mapping.ElementNodeId);

					// v3Destination
					item.SubItems.Add(targetNode.Element.Name);
					item.SubItems[(int)mapperColumnId.v3Destination].Tag = targetNode;

					// importColor
					item.SubItems.Add(GetColorName(mapping.DestinationColor));
					// item.SubItems.Add(mapping.DestinationColor.Name);
					item.SubItems[(int)mapperColumnId.importColor].BackColor = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(GetColorName(mapping.DestinationColor));
					// item.SubItems[(int)mapperColumnId.importColor].BackColor = mapping.DestinationColor;
				}
				else 
				{
					// v3Destination
					item.SubItems.Add(string.Empty);

					// importColor
					item.SubItems.Add(string.Empty);
				}
				// rgbPixel
				if( true == mapping.RgbPixel )
				{
					item.SubItems.Add("Yes");
				}
				else
				{
					item.SubItems.Add(string.Empty);
				}

				//set the v2 columns to readonly
				item.SubItems[(int)mapperColumnId.v2ChannelId].BackColor = Color.LightGray;
				item.SubItems[(int)mapperColumnId.v2channelOutput].BackColor = Color.LightGray;
				item.SubItems[(int)mapperColumnId.v2ChannelName].BackColor = Color.LightGray;

				listViewMapping.Items.Add(item);
			}
			listViewMapping.EndUpdate();

			PopulateNodeTreeMultiSelect();
		}

		private void CreateV2toV3MappingTable()
		{
			//default these to white
			Color vixen2Color = Color.Empty;

			Mappings = new List<ChannelMapping>();
			foreach (ListViewItem itemrow in listViewMapping.Items) 
			{
				vixen2Color = itemrow.SubItems[(int)mapperColumnId.v2channelColor].BackColor;

				if (!String.IsNullOrEmpty(itemrow.SubItems[(int)mapperColumnId.v3Destination].Text)) 
				{
					ElementNode node = (ElementNode) itemrow.SubItems[(int)mapperColumnId.v3Destination].Tag;

					Mappings.Add(new ChannelMapping(itemrow.SubItems[(int)mapperColumnId.v2ChannelName].Text,
					                                vixen2Color,
					                                itemrow.SubItems[(int)mapperColumnId.v2ChannelId].Text,
					                                itemrow.SubItems[(int)mapperColumnId.v2channelOutput].Text,
					                                node.Id,
													itemrow.SubItems[(int)mapperColumnId.importColor].BackColor,
					                                (itemrow.SubItems[(int)mapperColumnId.rgbPixel].Text) == "Yes"));
				}
				else 
				{
					//we are using this because we do not have a V3 map.
					Mappings.Add(new ChannelMapping(itemrow.SubItems[(int)mapperColumnId.v2ChannelName].Text,
					                                vixen2Color,
					                                itemrow.SubItems[(int)mapperColumnId.v2ChannelId].Text,
					                                itemrow.SubItems[(int)mapperColumnId.v2channelOutput].Text,
					                                Guid.Empty,
													Color.Empty,
													false));
				}
			}

			Mappings = Mappings;
			MappingName = mappingNameTextBox.Text;
		}

		/// <summary>
		/// value of the RGB checkbox has changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void checkBoxRGB_CheckedChanged(object sender, EventArgs e)
		{
			if(true == checkBoxRGB.Checked)
			{
				// disable the repeat counter and enter pixel mode
				numericUpDownRepeatElements.Enabled = false;
				numericUpDownRepeatElements.Value = 3;
				comboBoxColorOrder.Enabled = true;
			}
			else
			{
				// enable the repeat counter and exit pixel mode
				numericUpDownRepeatElements.Enabled = true;
				numericUpDownRepeatElements.Value = 1;
				comboBoxColorOrder.Enabled = false;
			}
		} // checkBoxRGB_CheckedChanged

		#region Drag drop events

		private void listViewMapping_DragDrop(object sender, DragEventArgs e)
		{
			Point cp = listViewMapping.PointToClient(new Point(e.X, e.Y));

			if (listViewMapping.HitTest(cp).Location.ToString() == "None") {
				//probably need to do something here
			}
			else {
				ListViewItem dragToItem = listViewMapping.GetItemAt(cp.X, cp.Y);

				//let the user know if we have items already here and we are about to overwrite them
				if (!String.IsNullOrEmpty(dragToItem.SubItems[(int)mapperColumnId.v3Destination].Text)) 
				{
					DialogResult result = MessageBox.Show("You are about to over write existing items.  Do you wish to continue?",
					                                      "Continue", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
					if (result == System.Windows.Forms.DialogResult.OK) 
					{
						startingIndex = dragToItem.Index;
						ParseNodes(treeview.SelectedNodes);
					}
				}
				else 
				{
					startingIndex = dragToItem.Index;
					ParseNodes(treeview.SelectedNodes);
				}
			}
		}

		private void listViewMapping_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof (TreeNode)))
				e.Effect = DragDropEffects.Move | DragDropEffects.Copy;
		}

		private void multiSelectTreeview1_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.Move | DragDropEffects.Copy;
		}

		private void multiSelectTreeview1_ItemDrag(object sender, ItemDragEventArgs e)
		{
			treeview = (MultiSelectTreeview) sender;
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
			if (String.IsNullOrEmpty(mappingNameTextBox.Text)) {
				MessageBox.Show("Please enter name of Map.", "Missing Name", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop);
				DialogResult = System.Windows.Forms.DialogResult.None;
			}
			else {
				CreateV2toV3MappingTable();
			}
		}

		private void destinationColorButton_Click(object sender, EventArgs e)
		{
			ColorDialog colorDlg = new ColorDialog()
			                       	{
			                       		AllowFullOpen = true,
			                       		AnyColor = true,
			                       		SolidColorOnly = false,
			                       		Color = Color.Red
			                       	};

			if (colorDlg.ShowDialog() == DialogResult.OK) {
				foreach (ListViewItem itemrow in listViewMapping.SelectedItems) {
					itemrow.UseItemStyleForSubItems = false;
					itemrow.SubItems[(int)mapperColumnId.importColor].Text = GetColorName(colorDlg.Color);
					itemrow.SubItems[(int)mapperColumnId.importColor].BackColor = colorDlg.Color;
				}
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

		#endregion

	}
}