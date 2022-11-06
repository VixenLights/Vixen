﻿using System;
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
using VixenModules.OutputFilter.DimmingCurve;
using VixenModules.Property.Color;

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
			ThemeUpdateControls.UpdateControls(this);
	
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

		public event EventHandler<ElementsChangedEventArgs> ElementsChanged;
		public void OnElementsChanged(ElementsChangedEventArgs e)
		{
			ElementsChanged?.Invoke(this, e);
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
			elementTree.RefreshElementTreeStatus();
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
				elementTree.RefreshElementTreeStatus();

				UpdateFormWithNode();
				OnElementsChanged(new ElementsChangedEventArgs(ElementsChangedEventArgs.ElementsChangedAction.Edit));
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
					OnElementsChanged(new ElementsChangedEventArgs(ElementsChangedEventArgs.ElementsChangedAction.Edit));
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
					OnElementsChanged(new ElementsChangedEventArgs(ElementsChangedEventArgs.ElementsChangedAction.Edit));
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

							OnElementsChanged(new ElementsChangedEventArgs(ElementsChangedEventArgs.ElementsChangedAction.Edit));
						}
					}
					else if (property.HasElementSetupHelper)
					{
						result = property.SetupElements(SelectedElements);
						if (result)
						{
							OnElementsChanged(new ElementsChangedEventArgs(ElementsChangedEventArgs.ElementsChangedAction.Edit));
						}
					}
				}
			}

			return result;
		}


		private async void buttonAddTemplate_Click(object sender, EventArgs e)
		{
			ComboBoxItem item = (comboBoxNewItemType.SelectedItem as ComboBoxItem);

			if (item != null) {
				IElementTemplate template = item.Value as IElementTemplate;
				bool act = template.SetupTemplate(elementTree.SelectedElementNodes);
				if (act) {
					IEnumerable<ElementNode> createdElements = await template.GenerateElements(elementTree.SelectedElementNodes);

					// If the user has not cancelled the template then...
					if (!template.Cancelled)
					{
						if (createdElements == null || !createdElements.Any())
						{
							//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
							MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
							var messageBox = new MessageBoxForm("Could not create elements.  Ensure you use a valid name and try again.", "", false, false);
							messageBox.ShowDialog();
							return;
						}

						// If the elements created by the template require configuration of dimming curves then...
						if (template.ConfigureDimming)
						{
							var question = new MessageBoxForm("Would you like to configure a dimming curve for this Prop?",
								"Dimming Curve Setup", MessageBoxButtons.YesNo, SystemIcons.Question);
							var response = question.ShowDialog(this);
							if (response == DialogResult.OK)
							{
								DimmingCurveHelper dimmingHelper = new DimmingCurveHelper(true);
								dimmingHelper.Perform(createdElements);
							}
						}

						// If the elements created by the template require further configuration of a color property then...
						if (template.ConfigureColor)
						{
							ColorSetupHelper helper = new ColorSetupHelper();
							helper.SetColorType(ElementColorType.FullColor);
							helper.Perform(createdElements);
						}

						elementTree.AddNodePathToTree(new[] { createdElements.First() });
						OnElementsChanged(new ElementsChangedEventArgs(ElementsChangedEventArgs.ElementsChangedAction.Add));
						UpdateFormWithNode();
						UpdateScrollPosition();
						
						// Refresh the tree so that the delete logic below will be successful
						elementTree.PopulateNodeTree();

						// Loop over the nodes created by the template that are also
						// part of a group.  Since the nodes are part of a group they
						// don't need to exist as stand alone nodes and should be removed.
						foreach (ElementNode node in template.GetElementsToDelete())
						{
							// Select the node
							elementTree.SelectElementNode(node);

							// Delete the tree node
							foreach (TreeNode tn in elementTree.SelectedTreeNodes)
							{
								elementTree.DeleteNode(tn);
							}							
						}

						// Refresh the tree
						elementTree.PopulateNodeTree();
					}
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
			OnElementsChanged(new ElementsChangedEventArgs(ElementsChangedEventArgs.ElementsChangedAction.Edit));
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

			if (controllersAndOutputs.Count == 0)
			{
				var msg = new MessageBoxForm("No controller patch points found.", "Not Found", MessageBoxButtons.OK,
					SystemIcons.Information);
				msg.ShowDialog(this);
			}
			else
			{
				MasterForm.SelectControllersAndOutputs(controllersAndOutputs, true);
			}
			
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
			OnElementsChanged(new ElementsChangedEventArgs(ElementsChangedEventArgs.ElementsChangedAction.Remove));
		}

		private void buttonRenameElements_Click(object sender, EventArgs e)
		{
			if (elementTree.RenameSelectedElements()) {
				OnElementsChanged(new ElementsChangedEventArgs(ElementsChangedEventArgs.ElementsChangedAction.Rename));
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
