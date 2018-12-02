using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls.Theme;
using Common.Resources;
using Common.Resources.Properties;
using Common.Controls;
using Common.Controls.Scaling;
using Vixen.Data.Flow;
using Vixen.Module;
using Vixen.Module.Property;
using Vixen.Rule;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Sys.Output;
using VixenModules.App.Modeling;

namespace VixenApplication.Setup
{
	public partial class SetupElementsTree : UserControl, ISetupElementsControl
	{
		public SetupElementsTree(IEnumerable<IElementTemplate> elementTemplates, IEnumerable<IElementSetupHelper> elementSetupHelpers)
		{
			InitializeComponent();
			int iconSize = (int)(24 * ScalingTools.GetScaleFactor());
			buttonAddTemplate.Image = Tools.GetIcon(Resources.add, iconSize);
			buttonAddTemplate.Text = "";
			buttonRunHelperSetup.Image = Tools.GetIcon(Resources.cog_go, iconSize);
			buttonRunHelperSetup.Text = "";
			buttonAddProperty.Image = Tools.GetIcon(Resources.add, iconSize);
			buttonAddProperty.Text = "";
			buttonRemoveProperty.Image = Tools.GetIcon(Resources.delete, iconSize);
			buttonRemoveProperty.Text = "";
			buttonConfigureProperty.Image = Tools.GetIcon(Resources.cog, iconSize);
			buttonConfigureProperty.Text = "";
			buttonDeleteElements.Image = Tools.GetIcon(Resources.delete, iconSize);
			buttonDeleteElements.Text = "";
			buttonRenameElements.Image = Tools.GetIcon(Resources.pencil, iconSize);
			buttonRenameElements.Text = "";
			buttonSelectDestinationOutputs.Image = Tools.GetIcon(Resources.table_select_row, iconSize);
			buttonSelectDestinationOutputs.Text = "";
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
	//		comboBoxNewItemType.BackColor = ThemeColorTable.BackgroundColor;
	//		comboBoxSetupHelperType.BackColor = ThemeColorTable.BackgroundColor;

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

			UpdateFormWithNode(null);

			elementTree.ExportDiagram = ExportWireDiagram;
		}

		private void ExportWireDiagram(ElementNode node)
		{
			ElementModeling.ElementsToSvg(node);
		}

		public event EventHandler<ElementNodesEventArgs> ElementSelectionChanged;
		public void OnElementSelectionChanged(ElementNodesEventArgs e)
		{
			if (ElementSelectionChanged == null)
				return;

			ElementSelectionChanged(this, e);
		}

		public event EventHandler ElementsChanged;
		public void OnElementsChanged()
		{
			if (ElementsChanged == null)
				return;

			ElementsChanged(this, EventArgs.Empty);
		}


		public IEnumerable<ElementNode> SelectedElements
		{
			get { return elementTree.SelectedElementNodes; }
			set
			{
				elementTree.PopulateNodeTree(value);
			}
		}

		public Control SetupElementsControl
		{
			get { return this; }
		}

		public DisplaySetup MasterForm { get; set; }

		public void UpdatePatching()
		{
			elementTree.PopulateNodeTree();
		}

		public void UpdateScrollPosition()
		{
			elementTree.UpdateScrollPosition();
		}


		private void buttonRunSetupHelper_Click(object sender, EventArgs e)
		{
			if (comboBoxSetupHelperType.SelectedIndex < 0)
				return;

			ComboBoxItem item = (comboBoxSetupHelperType.SelectedItem as ComboBoxItem);

			if (item != null) {
				IElementSetupHelper helper = item.Value as IElementSetupHelper;
				helper.Perform(elementTree.SelectedElementNodes);
				elementTree.PopulateNodeTree();

				UpdateFormWithNode();
				OnElementsChanged();
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

					UpdateFormWithNode();
					OnElementsChanged();
				}
			}
		}

		private void buttonRemoveProperty_Click(object sender, EventArgs e)
		{
			if (listViewProperties.SelectedItems.Count > 0) {
				string message, title;
				if (listViewProperties.SelectedItems.Count == 1) {
					message = "Are you sure you want to remove the selected property from the element?";
					title = "Remove Property?";
				} else {
					message = "Are you sure you want to remove the selected properties from the element?";
					title = "Remove Properties?";
				}
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm(message, title, false, true);
				messageBox.ShowDialog();
				if (messageBox.DialogResult == DialogResult.OK)
				{
					foreach (ListViewItem item in listViewProperties.SelectedItems) {
						foreach (ElementNode elementNode in SelectedElements) {
							elementNode.Properties.Remove((item.Tag as IPropertyModuleInstance).Descriptor.TypeId);
						}
					}

					UpdateFormWithNode();
					OnElementsChanged();
				}
			}

		}

		private void buttonConfigureProperty_Click(object sender, EventArgs e)
		{
			ConfigureSelectedProperties();
		}

		private bool ConfigureSelectedProperties()
		{
			bool result = false;

			if (listViewProperties.SelectedItems.Count == 1) {
				var property = listViewProperties.SelectedItems[0].Tag as IPropertyModuleInstance;
				if (property != null) {

					if (property.HasSetup)
					{
						result = property.Setup();
						if (result)
						{
							// try and 'clone' the property data to any other selected element with this property data
							foreach (ElementNode elementNode in SelectedElements)
							{
								IPropertyModuleInstance p = elementNode.Properties.Get(property.TypeId);
								if (p != null && p.ModuleData != property.ModuleData)
								{
									p.CloneValues(property);
								}
							}

							OnElementsChanged();
						}
					}
					else if (property.HasElementSetupHelper)
					{
						result = property.SetupElements(SelectedElements);
						if (result)
						{
							OnElementsChanged();
						}
					}
				}
			}

			return result;
		}


		private void buttonAddTemplate_Click(object sender, EventArgs e)
		{
			ComboBoxItem item = (comboBoxNewItemType.SelectedItem as ComboBoxItem);

			if (item != null) {
				IElementTemplate template = item.Value as IElementTemplate;
				bool act = template.SetupTemplate(elementTree.SelectedElementNodes);
				if (act) {
					IEnumerable<ElementNode> createdElements = template.GenerateElements(elementTree.SelectedElementNodes);
					if (createdElements == null || createdElements.Count() == 0) {
						//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
						MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
						var messageBox = new MessageBoxForm("Could not create elements.  Ensure you use a valid name and try again.", "", false, false);
						messageBox.ShowDialog();
						return;
					}
					elementTree.PopulateNodeTree(createdElements.FirstOrDefault());
					OnElementsChanged();
					UpdateScrollPosition();
				}
			}
		}

		private void UpdateFormWithNode()
		{
			UpdateFormWithNode(elementTree.SelectedNode);
		}


		private void UpdateFormWithNode(ElementNode selectedNode)
		{
			// Properties
			// TODO: we should really go through the selected elements, and only show properties they all have
			// TODO: or even better, show normally if they ALL have them, and show in italics if SOME have the property... then don't let the partial ones be configured
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
			ColumnAutoSize();
			UpdateButtons();
		}

		public void ColumnAutoSize()
		{
			listViewProperties.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			ListView.ColumnHeaderCollection cc = listViewProperties.Columns;
			for (int i = 0; i < cc.Count; i++)
			{
				if (cc[i].Width > listViewProperties.Width)
				{
					cc[i].Width = listViewProperties.Width - (int)(listViewProperties.Width * .06d);
				}
			}
		}

		private void UpdateButtons()
		{
			buttonRunHelperSetup.Enabled = comboBoxSetupHelperType.SelectedIndex >= 0;
			buttonAddTemplate.Enabled = comboBoxNewItemType.SelectedIndex >= 0;

			List<ElementNode> elementList = SelectedElements.ToList();
			buttonRunHelperSetup.Enabled = elementList.Any();
			buttonAddProperty.Enabled = elementList.Any();
			buttonRemoveProperty.Enabled = listViewProperties.Items.Count > 0 && listViewProperties.SelectedItems.Count > 0;
			buttonConfigureProperty.Enabled = listViewProperties.Items.Count > 0 && listViewProperties.SelectedItems.Count == 1;
			buttonDeleteElements.Enabled = elementList.Any();
			buttonRenameElements.Enabled = elementList.Any();
			buttonSelectDestinationOutputs.Enabled = elementList.Any();
		}

		private void elementTree_ElementsChanged(object sender, EventArgs e)
		{
			UpdateFormWithNode();
			OnElementsChanged();
		}

		private void elementTree_treeviewAfterSelect(object sender, TreeViewEventArgs e)
		{
			UpdateFormWithNode();
			OnElementSelectionChanged(new ElementNodesEventArgs(elementTree.SelectedElementNodes));
		}

		private void elementTree_treeviewDeselected(object sender, EventArgs e)
		{
			UpdateFormWithNode();
			OnElementSelectionChanged(new ElementNodesEventArgs(elementTree.SelectedElementNodes));
		}

		private void listViewProperties_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			ConfigureSelectedProperties();
		}

		private void comboBoxNewItemType_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateButtons();
		}

		private void comboBoxSetupHelperType_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateButtons();
		}

		private void listViewProperties_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateButtons();
		}

		private void buttonSelectDestinationOutputs_Click(object sender, EventArgs e)
		{
			ControllersAndOutputsSet controllersAndOutputs = new ControllersAndOutputsSet();

			foreach (ElementNode selectedElement in SelectedElements) {
				foreach (ElementNode leafElementNode in selectedElement.GetLeafEnumerator()) {
					if (leafElementNode == null || leafElementNode.Element == null)
						continue;

					IDataFlowComponent component = VixenSystem.DataFlow.GetComponent(leafElementNode.Element.Id);
					if (component == null)
						continue;

					IEnumerable<IDataFlowComponent> outputComponents = _findComponentsOfTypeInTreeFromComponent(component, typeof (CommandOutputDataFlowAdapter));

					foreach (IDataFlowComponent outputComponent in outputComponents) {
						IControllerDevice controller;
						int outputIndex;
						VixenSystem.OutputControllers.getOutputDetailsForDataFlowComponent(outputComponent, out controller, out outputIndex);

						if (controller == null)
							continue;

						if (!controllersAndOutputs.ContainsKey(controller))
							controllersAndOutputs[controller] = new HashSet<int>();

						controllersAndOutputs[controller].Add(outputIndex);
					}
				}
			}

			MasterForm.SelectControllersAndOutputs(controllersAndOutputs, true);
		}

		private IEnumerable<IDataFlowComponent> _findComponentsOfTypeInTreeFromComponent(IDataFlowComponent dataFlowComponent, Type dfctype)
		{
			return VixenSystem.DataFlow.GetDestinationsOfComponent(dataFlowComponent)
				.SelectMany(x => _findComponentsOfTypeInTreeFromComponent(x, dfctype))
				.Concat(new[] { dataFlowComponent })
				.Where(dfc => dfctype.IsAssignableFrom(dfc.GetType()))
				;
		}

		private void buttonDeleteElements_Click(object sender, EventArgs e)
		{
			// TODO: need to consider the filters attached to a element. Hmm.

			//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
			MessageBoxForm.msgIcon = SystemIcons.Question; //this is used if you want to add a system icon to the message form.
			var messageBox = new MessageBoxForm("Are you sure you want to delete these element(s)?", "Delete Elements?", true, false);
			messageBox.ShowDialog();

			if (messageBox.DialogResult != DialogResult.OK)
				return;

			// can't delete by ElementNode, as one element can be in multiple places :-(
			foreach (TreeNode tn in elementTree.SelectedTreeNodes) {
				elementTree.DeleteNode(tn);
			}

			elementTree.PopulateNodeTree();
			OnElementsChanged();
		}

		private void buttonRenameElements_Click(object sender, EventArgs e)
		{
			if (elementTree.RenameSelectedElements()) {
				OnElementsChanged();
			}
		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}

		private void comboBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			ThemeComboBoxRenderer.DrawItem(sender, e);
		}
	}
}
