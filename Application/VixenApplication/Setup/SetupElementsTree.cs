using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Resources.Properties;
using Common.Controls;
using Vixen.Module.Property;
using Vixen.Rule;
using Vixen.Services;
using Vixen.Sys;

namespace VixenApplication.Setup
{
	public partial class SetupElementsTree : UserControl, ISetupElementsControl
	{
		public SetupElementsTree(IEnumerable<IElementTemplate> elementTemplates, IEnumerable<IElementSetupHelper> elementSetupHelpers)
		{
			InitializeComponent();

			buttonAddTemplate.BackgroundImage = Resources.add;
			buttonAddTemplate.Text = "";
			buttonRunHelperSetup.BackgroundImage = Resources.cog_go;
			buttonRunHelperSetup.Text = "";
			buttonAddProperty.BackgroundImage = Resources.add;
			buttonAddProperty.Text = "";

			comboBoxNewItemType.BeginUpdate();
			foreach (IElementTemplate template in elementTemplates) {
				ComboBoxItem item = new ComboBoxItem(template.TemplateName, template);
				comboBoxNewItemType.Items.Add(item);
			}
			comboBoxNewItemType.EndUpdate();
			if (comboBoxNewItemType.Items.Count > 0)
				comboBoxNewItemType.SelectedIndex = 0;

			comboBoxSetupHelperType.BeginUpdate();
			foreach (IElementSetupHelper helper in elementSetupHelpers) {
				ComboBoxItem item = new ComboBoxItem(helper.HelperName, helper);
				comboBoxSetupHelperType.Items.Add(item);
			}
			comboBoxSetupHelperType.EndUpdate();
			if (comboBoxSetupHelperType.Items.Count > 0)
				comboBoxSetupHelperType.SelectedIndex = 0;

			PopulateWithNode(null);
		}



		public event EventHandler<ElementNodesEventArgs> ElementSelectionChanged;
		public void OnElementSelectionChanged(ElementNodesEventArgs e)
		{
			if (ElementSelectionChanged == null)
				return;

			ElementSelectionChanged(this, e);
		}

		public event EventHandler ElementsChanged;
		public void OnElementChanged()
		{
			if (ElementsChanged == null)
				return;

			ElementsChanged(this, EventArgs.Empty);
		}


		public IEnumerable<ElementNode> SelectedElements
		{
			get { return elementTree.SelectedElementNodes; }
		}

		public Control SetupElementsControl
		{
			get { return this; }
		}

		public void UpdatePatching()
		{
			elementTree.PopulateNodeTree();
		}


		private void buttonRunSetupHelper_Click(object sender, EventArgs e)
		{
			ComboBoxItem item = (comboBoxSetupHelperType.SelectedItem as ComboBoxItem);

			if (item != null) {
				IElementSetupHelper helper = item.Value as IElementSetupHelper;
				helper.Perform(elementTree.SelectedElementNodes);
				elementTree.PopulateNodeTree();
			}

		}

		private void buttonAddProperty_Click(object sender, EventArgs e)
		{
			List<KeyValuePair<string, object>> properties = new List<KeyValuePair<string, object>>();
			foreach (KeyValuePair<Guid, string> kvp in ApplicationServices.GetAvailableModules<IPropertyModuleInstance>()) {
				properties.Add(new KeyValuePair<string, object>(kvp.Value, kvp.Key));
			}
			using (ListSelectDialog addForm = new ListSelectDialog("Add Property", (properties))) {
				addForm.SelectionMode = SelectionMode.One;
				if (addForm.ShowDialog() == DialogResult.OK) {

					// TODO: something smarter about picking subelements vs. applying it to the groups. For now, will just apply it to the actual selected items.

					foreach (ElementNode node in elementTree.SelectedElementNodes) {
						node.Properties.Add((Guid)addForm.SelectedItem);
					}

					PopulateWithNode();
				}
			}

		}

		private void buttonAddTemplate_Click(object sender, EventArgs e)
		{
			ComboBoxItem item = (comboBoxNewItemType.SelectedItem as ComboBoxItem);

			if (item != null) {
				IElementTemplate template = item.Value as IElementTemplate;
				bool act = template.SetupTemplate(elementTree.SelectedElementNodes);
				if (act) {
					template.GenerateElements(elementTree.SelectedElementNodes);
					elementTree.PopulateNodeTree();
				}
			}

		}

		private void PopulateWithNode()
		{
			PopulateWithNode(elementTree.SelectedNode);
		}


		private void PopulateWithNode(ElementNode selectedNode)
		{
			// Properties
			listViewProperties.BeginUpdate();
			listViewProperties.Items.Clear();
			if (selectedNode != null) {
				foreach (IPropertyModuleInstance property in selectedNode.Properties) {
					ListViewItem item = new ListViewItem();
					item.Text = property.Descriptor.TypeName;
					item.Tag = property;
					listViewProperties.Items.Add(item);
				}

				listViewProperties.SelectedItems.Clear();
			}
			listViewProperties.EndUpdate();
		}

		private void elementTree_ElementsChanged(object sender, EventArgs e)
		{
			PopulateWithNode();
			OnElementChanged();
		}

		private void elementTree_treeviewAfterSelect(object sender, TreeViewEventArgs e)
		{
			PopulateWithNode();
			OnElementSelectionChanged(new ElementNodesEventArgs(elementTree.SelectedElementNodes));
		}

		private void elementTree_treeviewDeselected(object sender, EventArgs e)
		{
			PopulateWithNode();
			OnElementSelectionChanged(new ElementNodesEventArgs(elementTree.SelectedElementNodes));
		}

		private void listViewProperties_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (listViewProperties.SelectedItems.Count == 1) {
				var property = listViewProperties.SelectedItems[0].Tag as IPropertyModuleInstance;
				if (property != null)
					property.Setup();
			}
		}

		private void comboBoxNewItemType_SelectedIndexChanged(object sender, EventArgs e)
		{
			buttonAddTemplate.Enabled = comboBoxNewItemType.SelectedIndex >= 0;
		}

		private void comboBoxSetupHelperType_SelectedIndexChanged(object sender, EventArgs e)
		{
			buttonRunHelperSetup.Enabled = comboBoxSetupHelperType.SelectedIndex >= 0;
		}

		private void listViewProperties_SelectedIndexChanged(object sender, EventArgs e)
		{
			buttonAddProperty.Enabled = listViewProperties.SelectedItems.Count > 0;
		}
	}
}
