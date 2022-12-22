﻿using Common.Resources.Properties;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Module.Property;
using Common.Controls;

namespace VixenApplication
{
	public partial class ConfigElements : BaseForm
	{
		private ElementNode _displayedNode;
		private bool _changesMade;

		private ToolTip _tooltip;

		public ConfigElements()
		{
			InitializeComponent();
			Icon = Resources.Icon_Vixen3;
			_displayedNode = null;
			_tooltip = new ToolTip();
		}

		private void ConfigElements_Load(object sender, EventArgs e)
		{
			PopulateFormWithNode(null, true);
			elementTree.treeviewAfterSelect += elementTree_AfterSelect;
			elementTree.treeviewDeselected += elementTree_treeviewDeselected;
			elementTree.ElementsChanged += elementTree_ElementsChanged;
		}


		#region Form controls population & display

		private void PopulateFormWithNode(ElementNode node, bool forceUpdate)
		{
			if (node == _displayedNode && !forceUpdate)
				return;

			_displayedNode = node;

			PopulateGeneralNodeInfo(node);
			PopulatePropertiesArea(node);

			groupBoxSelectedNode.Enabled = (node != null);

			buttonDeleteElement.Enabled = (elementTree.SelectedTreeNodes.Count > 0) && (node != null);
			buttonCreateGroup.Enabled = (elementTree.SelectedTreeNodes.Count > 0) && (node != null);
			buttonRename.Enabled = (elementTree.SelectedTreeNodes.Count > 0) && (node != null);
		}

		private void PopulateGeneralNodeInfo(ElementNode node)
		{
			if (node == null) {
				labelParents.Text = string.Empty;
				_tooltip.SetToolTip(labelParents, null);
				textBoxName.Text = string.Empty;
			}
			else {
				// update the label with parent info about the node. Any good suggestions or improvements for this?
				int parentCount = GetNodeParentGroupCount(node);
				List<string> parents = GetNodeParentGroupNames(node);
				string labelString = string.Empty, tooltipString = string.Empty;
				labelString = string.Format("This element is in {0} group{1}{2}", parentCount, ((parentCount != 1) ? "s" : string.Empty),
				                            ((parentCount == 0) ? "." : ": "));
				tooltipString = labelString + "\r\n\r\n";
				foreach (string p in parents) {
					labelString = string.Format("{0}{1}, ", labelString, p);
					tooltipString = string.Format("{0}{1}\r\n", tooltipString, p);
				}
				labelParents.Text = labelString.TrimEnd(new char[] {' ', ','});
				tooltipString = tooltipString.TrimEnd(new char[] {'\r', '\n'});
				if (labelString.Length > 100) {
					_tooltip.SetToolTip(labelParents, tooltipString);
				}
				else {
					_tooltip.SetToolTip(labelParents, null);
				}

				textBoxName.Text = node.Name;
			}
		}

		private void PopulatePropertiesArea(ElementNode node)
		{
			listViewProperties.BeginUpdate();
			listViewProperties.Items.Clear();
			if (node != null) {
				foreach (IPropertyModuleInstance property in node.Properties) {
					ListViewItem item = new ListViewItem();
					item.Text = property.Descriptor.TypeName;
					item.Tag = property;
					listViewProperties.Items.Add(item);
				}

				listViewProperties.SelectedItems.Clear();
			}
			listViewProperties.EndUpdate();

			PopulatePropertiesButtons();
		}

		private void PopulatePropertiesButtons()
		{
			buttonConfigureProperty.Enabled = (listViewProperties.SelectedItems.Count == 1);
			buttonDeleteProperty.Enabled = (listViewProperties.SelectedItems.Count > 0);
		}

		#endregion

		#region Form buttons

		private void buttonAddElement_Click(object sender, EventArgs e)
		{
			elementTree.ClearSelectedNodes();
			elementTree.AddSingleNodeWithPrompt();
			_changesMade = true;
		}

		private void buttonAddMultipleElements_Click(object sender, EventArgs e)
		{
			elementTree.ClearSelectedNodes();
			elementTree.AddMultipleNodesWithPrompt();
			_changesMade = true;
		}

		private void buttonDeleteElement_Click(object sender, EventArgs e)
		{
			if (elementTree.SelectedTreeNodes.Count > 0) {
				string message, title;
				if (elementTree.SelectedTreeNodes.Count > 1) {
					message = "Are you sure you want to delete the selected elements?";
					title = "Delete Items?";
				}
				else {
					message = "Are you sure you want to delete the selected element?";
					title = "Delete Item?";
				}
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Question; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm(message, title, false, true);
				messageBox.ShowDialog();
				if (messageBox.DialogResult == DialogResult.OK)
				{
					foreach (TreeNode tn in elementTree.SelectedTreeNodes) {
						elementTree.DeleteNode(tn);
					}
					_changesMade = true;
				}
			}

			elementTree.PopulateNodeTree();
			PopulateFormWithNode(null, true);
		}

		private void buttonCreateGroup_Click(object sender, EventArgs e)
		{
			elementTree.CreateGroupFromSelectedNodes();
			_changesMade = true;
		}

		private void buttonRename_Click(object sender, EventArgs e)
		{
			elementTree.RenameSelectedElements();
			_changesMade = true;
		}

		private void buttonAddProperty_Click(object sender, EventArgs e)
		{
			List<KeyValuePair<string, object>> properties = new List<KeyValuePair<string, object>>();
			foreach (KeyValuePair<Guid, string> kvp in ApplicationServices.GetAvailableModules<IPropertyModuleInstance>()) {
				properties.Add(new KeyValuePair<string, object>(kvp.Value, kvp.Key));
			}
			using (ListSelectDialog addForm = new ListSelectDialog("Add Property", (properties))) {
				addForm.SelectionMode = SelectionMode.MultiExtended;
				if (addForm.ShowDialog() == DialogResult.OK) {
					foreach(KeyValuePair<string,object> item in addForm.SelectedItems){

						_displayedNode.Properties.Add((Guid) item.Value);	
					}

					PopulatePropertiesArea(_displayedNode);
					_changesMade = true;
				}
			}
		}

		private void buttonConfigureProperty_Click(object sender, EventArgs e)
		{
			ConfigureSelectedProperty();
			_changesMade = true;
		}

		private void buttonDeleteProperty_Click(object sender, EventArgs e)
		{
			if (listViewProperties.SelectedItems.Count > 0) {
				string message, title;
				if (listViewProperties.SelectedItems.Count == 1) {
					message = "Are you sure you want to remove the selected property from the element?";
					title = "Remove Property?";
				}
				else {
					message = "Are you sure you want to remove the selected properties from the element?";
					title = "Remove Properties?";
				}
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Question; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm(message, title, false, true);
				messageBox.ShowDialog();
				if (messageBox.DialogResult == DialogResult.OK)
				{
					foreach (ListViewItem item in listViewProperties.SelectedItems) {
						_displayedNode.Properties.Remove((item.Tag as IPropertyModuleInstance).Descriptor.TypeId);
					}

					PopulatePropertiesArea(_displayedNode);
					_changesMade = true;
				}
			}
		}

		#endregion



		#region Events

		private void elementTree_AfterSelect(object sender, TreeViewEventArgs e)
		{
			PopulateFormWithNode(elementTree.SelectedNode, false);
		}

		void elementTree_treeviewDeselected(object sender, EventArgs e)
		{
			PopulateFormWithNode(elementTree.SelectedNode, false);
		}

		void elementTree_ElementsChanged(object sender, EventArgs e)
		{
			PopulateFormWithNode(elementTree.SelectedNode, true);
		}

		private void listViewProperties_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			ConfigureSelectedProperty();
		}

		private void listViewProperties_SelectedIndexChanged(object sender, EventArgs e)
		{
			PopulatePropertiesButtons();
		}

		#endregion



		#region Helper functions

		private void ConfigureSelectedProperty()
		{
			if (listViewProperties.SelectedItems.Count == 1) {
				(listViewProperties.SelectedItems[0].Tag as IPropertyModuleInstance).Setup();
			}
		}

		private int GetNodeParentGroupCount(ElementNode node)
		{
			int count = node.Parents.Count();
			if (VixenSystem.Nodes.GetRootNodes().Contains(node))
				count--;
			return count;
		}

		private List<string> GetNodeParentGroupNames(ElementNode node, int maxNames = Int32.MaxValue)
		{
			List<string> result = new List<string>();
			foreach (ElementNode parent in node.Parents) {
				if (maxNames <= 0) {
					break;
				}
				if (parent.Name != "Root") {
					result.Add(parent.Name);
				}
				maxNames--;
			}

			return result;
		}



		#endregion

		private void textBoxName_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter) {
				string newName = textBoxName.Text.Trim();
				if (newName != string.Empty && newName != _displayedNode.Name) {
					VixenSystem.Nodes.RenameNode(_displayedNode, newName);
					elementTree.PopulateNodeTree();
				}
			}
		}

		private void ConfigElements_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (_changesMade) {
				if (DialogResult == DialogResult.Cancel) {
					switch (
						MessageBox.Show(this, "All changes will be lost if you continue, do you wish to continue?", "Are you sure?",
						                MessageBoxButtons.YesNo, MessageBoxIcon.Question)) {
						                	case DialogResult.No:
						                		e.Cancel = true;
						                		break;
						                	default:
						                		break;
					}
				}
				else if (DialogResult == DialogResult.OK) {
					e.Cancel = false;
				}
				else {
					switch (e.CloseReason) {
						case CloseReason.UserClosing:
							e.Cancel = true;
							break;
					}
				}
			}
		}
	}
}