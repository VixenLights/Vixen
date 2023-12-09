using Common.Controls;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Resources;
using Common.Resources.Properties;
using Utilities;
using WeifenLuo.WinFormsUI.Docking;
using Vixen.Rule;
using Vixen.Services;
using Vixen.Sys;
using VixenModules.App.Modeling;

namespace VixenModules.Preview.VixenPreview
{
	public partial class VixenPreviewSetupElementsDocument : DockContent
	{
		private VixenPreviewControl _preview;

		public VixenPreviewSetupElementsDocument(VixenPreviewControl preview)
		{
			InitializeComponent();
			int iconSize = (int)(24 * ScalingTools.GetScaleFactor());
			buttonAddTemplate.Image = Tools.GetIcon(Resources.add, iconSize);
			buttonAddTemplate.Text = "";
			var elementTemplates = ApplicationServices.GetAllElementTemplates();
			comboBoxNewItemType.BeginUpdate();
			foreach (IElementTemplate template in elementTemplates)
			{
				ComboBoxItem item = new ComboBoxItem(template.TemplateName, template);
				comboBoxNewItemType.Items.Add(item);
			}
			comboBoxNewItemType.EndUpdate();
			if (comboBoxNewItemType.Items.Count > 0)
				comboBoxNewItemType.SelectedIndex = 0;

			ThemeUpdateControls.UpdateControls(this);
			_preview = preview;
			treeElements.AllowPropertyEdit = false;
			treeElements.AllowWireExport = true;
			treeElements.ExportDiagram = ExportWireDiagram;
		}

		private void ExportWireDiagram(ElementNode node)
		{
			ElementModeling.ElementsToSvg(node);
		}

		private void VixenPreviewSetupElementsDocument_Load(object sender, EventArgs e)
		{
			treeElements.treeviewAfterSelect += treeElements_AfterSelect;
			treeElements.treeviewDeselected += TreeElementsOnTreeviewDeselected;
			treeElements.ElementsChanged += TreeElements_ElementsChanged;
		}

		private void TreeElements_ElementsChanged(object sender, EventArgs e)
		{
			if (e is ElementTree.ElementsChangedEventArgs ev)
			{
				if (ev.Action == ElementTree.ElementsChangedEventArgs.ElementsChangedAction.Remove)
				{
					foreach (var evAffectedNode in ev.AffectedNodes)
					{
						if (!VixenSystem.Nodes.ElementNodeExists(evAffectedNode.Id))
						{
							_preview.UnlinkNodesFromDisplayItem(evAffectedNode);
						}
					}
					UpdateSelectedDisplayItems();
				}
			}
		}

		private void treeElements_AfterSelect(object sender, TreeViewEventArgs e)
		{
			UpdateSelectedDisplayItems();
		}

		private void TreeElementsOnTreeviewDeselected(object sender, EventArgs e)
		{
			UpdateSelectedDisplayItems();
		}

		private void UpdateSelectedDisplayItems()
		{
			_preview.BeginUpdate();
			_preview.HighlightedElements.Clear();

			foreach (var node in treeElements.SelectedElementNodes)
			{
				_preview.HighlightNode(node);
			}

			if (!_preview.SelectedDisplayItems.Any())
			{
				_preview.propertiesForm.ClearSetupControl();
			}

			_preview.EndUpdate();
		}

		public ElementNode SelectedNode => treeElements.SelectedNode;

		public void SelectElementNode(ElementNode node)
		{
			treeElements.SelectElementNode(node);
		}

		private async void ButtonAddTemplate_Click(object sender, EventArgs e)
		{
			// Disable the button so that the user cannot accidentally double click
			buttonAddTemplate.Enabled = false;

			ComboBoxItem item = (comboBoxNewItemType.SelectedItem as ComboBoxItem);

			if (item != null)
			{
				IElementTemplate template = item.Value as IElementTemplate;
				await SetupTemplate(template);
			}

			// Re-enable the plus add button
			buttonAddTemplate.Enabled = true;
		}

		internal void ClearSelectedNodes()
		{
			treeElements.ClearSelectedNodes();
			UpdateSelectedDisplayItems();
		}

		internal async Task<bool> SetupTemplate(IElementTemplate template)
		{
			// Create the element template helper
			ElementTemplateHelper elementTemplateHelper = new ElementTemplateHelper();

			// Process the template for the selected node(s)
			IEnumerable<ElementNode> createdElements = await elementTemplateHelper.ProcessElementTemplate(
				treeElements.SelectedElementNodes,
				template,
				this,
				(node) => AddNodeToTree(node),
				treeElements);
			
			// Return whether nodes were created
			return (createdElements != null);
		}

		internal void AddNodeToTree(ElementNode node)
		{
			if (node == null) return;
			treeElements.AddNodePathToTree(new[] { node });
			treeElements.UpdateScrollPosition();
		}

		private void ComboBoxNewItemType_SelectedIndexChanged(object sender, EventArgs e)
		{
			buttonAddTemplate.Enabled = comboBoxNewItemType.SelectedIndex >= 0;
		}

		private void ComboBoxNewItemType_DrawItem(object sender, DrawItemEventArgs e)
		{
			ThemeComboBoxRenderer.DrawItem(sender, e);
		}
	}
}