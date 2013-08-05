using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using Vixen.Sys;
using System.ComponentModel;
using Common.Controls;

namespace VixenModules.SequenceType.Vixen2x
{
	public partial class Vixen2xSequenceImporterChannelMapper : Form
	{
		private MultiSelectTreeview treeview;
		private bool MapExists;
		private int startingIndex;

		public Vixen2xSequenceImporterChannelMapper(List<ChannelMapping> mappings, bool mapExists, string mappingName)
		{
			InitializeComponent();

			Mappings = mappings;
			MapExists = mapExists;
			mappingNameTextBox.Text = mappingName;
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
			// if the user drags a large number of items to start at a position that
			// doesn't have enough 'room' off the end for them all, this can go OOR
			if (listViewMapping.Items.Count <= startingIndex)
				return;

			ElementNode enode = (ElementNode) node.Tag;
			ListViewItem item = listViewMapping.Items[startingIndex];

			item.SubItems[4].Text = enode.Element.Name;

			item.SubItems[4].Tag = enode;

			//Not sure where to get a node color from Vixen 3 stuff so if we have one in Vixen 2 just use it
			item.SubItems[5].Text = item.SubItems[3].Text;
			item.SubItems[5].BackColor = item.SubItems[3].BackColor;

			startingIndex++;
		}

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

			foreach (ChannelMapping mapping in Mappings) {
				ListViewItem item = new ListViewItem(mapping.ChannelNumber) {UseItemStyleForSubItems = false};
				item.SubItems.Add(mapping.ChannelOutput);
				item.SubItems.Add(mapping.ChannelName);
				item.SubItems.Add(GetColorName(mapping.ChannelColor));
				//item.SubItems[3].BackColor = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(GetColorName(mapping.ChannelColor));
				item.SubItems[3].BackColor = mapping.ChannelColor;

				if (MapExists && mapping.ElementNodeId != Guid.Empty) {
					ElementNode targetNode = VixenSystem.Nodes.GetElementNode(mapping.ElementNodeId);
					item.SubItems.Add(targetNode.Element.Name);
					item.SubItems[4].Tag = targetNode;

					item.SubItems.Add(GetColorName(mapping.DestinationColor));
					//item.SubItems[5].BackColor = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(GetColorName(mapping.DestinationColor));
					item.SubItems[5].BackColor = mapping.DestinationColor;
				}
				else {
					item.SubItems.Add(string.Empty);
					item.SubItems.Add(string.Empty);
				}

				//set the v2 columns to readonly
				item.SubItems[0].BackColor = Color.LightGray;
				item.SubItems[1].BackColor = Color.LightGray;
				item.SubItems[2].BackColor = Color.LightGray;

				listViewMapping.Items.Add(item);
			}
			listViewMapping.EndUpdate();

			PopulateNodeTreeMultiSelect();
		}

		private void CreateV2toV3MappingTable()
		{
			//default these to white
			Color vixen2Color = Color.Empty;
			Color Vixen3Color = Color.Empty;

			Mappings = new List<ChannelMapping>();
			foreach (ListViewItem itemrow in listViewMapping.Items) {
				vixen2Color = itemrow.SubItems[3].BackColor;

				if (!String.IsNullOrEmpty(itemrow.SubItems[4].Text)) {
					ElementNode node = (ElementNode) itemrow.SubItems[4].Tag;
					Vixen3Color = itemrow.SubItems[5].BackColor;

					Mappings.Add(new ChannelMapping(itemrow.SubItems[2].Text,
					                                vixen2Color,
					                                itemrow.SubItems[0].Text,
					                                itemrow.SubItems[1].Text,
					                                node.Id,
					                                Vixen3Color));
				}
				else {
					//we are using this because we do not have a V3 map.
					Mappings.Add(new ChannelMapping(itemrow.SubItems[2].Text,
					                                vixen2Color,
					                                itemrow.SubItems[0].Text,
					                                itemrow.SubItems[1].Text,
					                                Guid.Empty,
					                                Vixen3Color));
				}
			}

			Mappings = Mappings;
			MappingName = mappingNameTextBox.Text;
		}

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
				if (!String.IsNullOrEmpty(dragToItem.SubItems[4].Text)) {
					DialogResult result = MessageBox.Show("You are about to over write existing items.  Do you wish to continue?",
					                                      "Continue", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
					if (result == System.Windows.Forms.DialogResult.OK) {
						startingIndex = dragToItem.Index;
						ParseNodes(treeview.SelectedNodes);
					}
				}
				else {
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
					itemrow.SubItems[5].Text = GetColorName(colorDlg.Color);
					itemrow.SubItems[5].BackColor = colorDlg.Color;
				}
			}
		}

		#endregion
	}
}