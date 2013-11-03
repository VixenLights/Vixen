using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Common.Resources.Properties;
using Vixen.Data.Flow;
using Vixen.Factory;
using Vixen.Module;
using Vixen.Module.Controller;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace VixenApplication.Setup
{
	public partial class SetupControllersSimple : UserControl, ISetupControllersControl
	{
		private static readonly NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		public SetupControllersSimple()
		{
			InitializeComponent();

			buttonAddController.BackgroundImage = Resources.add;
			buttonAddController.Text = "";
			buttonConfigureController.BackgroundImage = Resources.cog;
			buttonConfigureController.Text = "";
			buttonNumberChannelsController.BackgroundImage = Resources.attributes_display;
			buttonNumberChannelsController.Text = "";
			buttonRenameController.BackgroundImage = Resources.cog_edit;
			buttonRenameController.Text = "";
			buttonDeleteController.BackgroundImage = Resources.delete;
			buttonDeleteController.Text = "";
			buttonSelectSourceElements.BackgroundImage = Resources.table_select_row;
			buttonSelectSourceElements.Text = "";

			comboBoxNewControllerType.BeginUpdate();
			foreach (KeyValuePair<Guid, string> kvp in ApplicationServices.GetAvailableModules<IControllerModuleInstance>()) {
				ComboBoxItem item = new ComboBoxItem(kvp.Value, kvp.Key);
				comboBoxNewControllerType.Items.Add(item);
			}			
			comboBoxNewControllerType.EndUpdate();
			if (comboBoxNewControllerType.Items.Count > 0)
				comboBoxNewControllerType.SelectedIndex = 0;

			controllerTree.ControllerSelectionChanged += controllerTree_ControllerSelectionChanged;
			controllerTree.ControllersChanged += controllerTree_ControllersChanged;

			UpdateButtons();
		}

		void controllerTree_ControllerSelectionChanged(object sender, EventArgs e)
		{
			OnControllerSelectionChanged();
			UpdateButtons();
		}

		void controllerTree_ControllersChanged(object sender, EventArgs e)
		{
			OnControllersChanged();
			UpdateButtons();
		}

		void UpdateButtons()
		{
			buttonConfigureController.Enabled = controllerTree.SelectedControllers.Count() == 1;
			buttonNumberChannelsController.Enabled = controllerTree.SelectedControllers.Count() == 1;
			buttonRenameController.Enabled = controllerTree.SelectedControllers.Count() == 1;

			buttonDeleteController.Enabled = controllerTree.SelectedControllers.Count() >= 1;

			buttonAddController.Enabled = comboBoxNewControllerType.SelectedIndex >= 0;

			buttonSelectSourceElements.Enabled = controllerTree.SelectedTreeNodes.Count > 0;
		}

		public ControllersAndOutputsSet BuildSelectedControllersAndOutputs()
		{
			ControllersAndOutputsSet result = new ControllersAndOutputsSet();

			foreach (TreeNode node in controllerTree.SelectedTreeNodes) {
				IControllerDevice controller = node.Tag as IControllerDevice;
				int outputIndex = -1;

				if (controller == null) {
					if (node.Tag is int) {
						outputIndex = (int) node.Tag;
						controller = node.Parent.Tag as IControllerDevice;
						if (controller == null) {
							Logging.Error("node parent is not a controller: " + node.Name);
						}
					}
				}

				if (controller != null) {
					HashSet<int> selectedOutputs;
					result.TryGetValue(controller, out selectedOutputs);
					if (selectedOutputs == null) {
						selectedOutputs = new HashSet<int>();
						result[controller] = selectedOutputs;
					}

					// if there wasn't a specific output that this was about, add all outputs for the controller
					if (outputIndex < 0) {
						for (int i = 0; i < controller.OutputCount; i++) {
							selectedOutputs.Add(i);
						}
					} else {
						selectedOutputs.Add(outputIndex);
					}
				} else {
					Logging.Error("can't figure out what controller the node maps to: " + node.Name);
				}
			}

			return result;
		}

		public ControllersAndOutputsSet SelectedControllersAndOutputs
		{
			get { return BuildSelectedControllersAndOutputs(); }
			set
			{
				controllerTree.PopulateControllerTree(value);
			}
		}

		public Control SetupControllersControl
		{
			get { return this; }
		}

		public DisplaySetup MasterForm { get; set; }

		public void UpdatePatching()
		{
			controllerTree.PopulateControllerTree();
		}

		public event EventHandler<ControllerSelectionEventArgs> ControllerSelectionChanged;
		public void OnControllerSelectionChanged()
		{
			if (ControllerSelectionChanged == null)
				return;

			ControllerSelectionEventArgs e = new ControllerSelectionEventArgs(SelectedControllersAndOutputs);
			ControllerSelectionChanged(this, e);
		}

		public event EventHandler ControllersChanged;
		public void OnControllersChanged()
		{
			if (ControllersChanged == null)
				return;

			ControllersChanged(this, EventArgs.Empty);
		}



		private void buttonAddController_Click(object sender, EventArgs e)
		{
			ComboBoxItem item = (comboBoxNewControllerType.SelectedItem as ComboBoxItem);

			if (item != null) {
				controllerTree.AddNewControllerOfTypeWithPrompts((Guid) item.Value);
			}
		}

		private void buttonDeleteController_Click(object sender, EventArgs e)
		{
			controllerTree.DeleteControllersWithPrompt(controllerTree.SelectedControllers);
		}

		private void buttonConfigureController_Click(object sender, EventArgs e)
		{
			if (controllerTree.SelectedControllers.Count() > 0) {
				controllerTree.ConfigureController(controllerTree.SelectedControllers.First());
			}
		}

		private void buttonNumberChannelsController_Click(object sender, EventArgs e)
		{
			if (controllerTree.SelectedControllers.Count() > 0) {
				controllerTree.SetControllerOutputCount(controllerTree.SelectedControllers.First());
			}
		}

		private void buttonRenameController_Click(object sender, EventArgs e)
		{
			if (controllerTree.SelectedControllers.Count() > 0) {
				controllerTree.RenameControllerWithPrompt(controllerTree.SelectedControllers.First());
			}
		}

		private void buttonSelectSourceElements_Click(object sender, EventArgs e)
		{
			List<ElementNode> elementNodesToSelect = new List<ElementNode>();

			foreach (KeyValuePair<IControllerDevice, HashSet<int>> controllerAndOutputs in SelectedControllersAndOutputs) {
				OutputController oc = controllerAndOutputs.Key as OutputController;
				if (oc == null)
					continue;

				foreach (int i in controllerAndOutputs.Value) {
					IDataFlowComponent outputComponent = oc.GetDataFlowComponentForOutput(oc.Outputs[i]);
					IDataFlowComponent rootComponent = FindRootSourceOfDataComponent(outputComponent);

					if (rootComponent is ElementDataFlowAdapter) {
						Element element = (rootComponent as ElementDataFlowAdapter).Element;
						ElementNode elementNode = VixenSystem.Elements.GetElementNodeForElement(element);
						if (elementNode != null) {
							elementNodesToSelect.Add(elementNode);
						}
					}
				}
			}

			MasterForm.SelectElements(elementNodesToSelect);
		}

		private IDataFlowComponent FindRootSourceOfDataComponent(IDataFlowComponent component)
		{
			if (component == null)
				return null;
			
			if (component.Source == null || component.Source.Component == null)
				return component;

			return FindRootSourceOfDataComponent(component.Source.Component);
		}
	}
}
