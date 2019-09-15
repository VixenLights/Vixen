using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Resources.Properties;
using Common.Resources;
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
			int iconSize = (int)(24 * ScalingTools.GetScaleFactor());
			buttonAddController.Image = Tools.GetIcon(Resources.add, iconSize);
			buttonAddController.Text = "";
			buttonConfigureController.Image = Tools.GetIcon(Resources.cog, iconSize);
			buttonConfigureController.Text = "";
			buttonNumberChannelsController.Image = Tools.GetIcon(Resources.attributes_display, iconSize);
			buttonNumberChannelsController.Text = "";
			buttonRenameController.Image = Tools.GetIcon(Resources.pencil, iconSize);
			buttonRenameController.Text = "";
			buttonDeleteController.Image = Tools.GetIcon(Resources.delete, iconSize);
			buttonDeleteController.Text = "";
			buttonSelectSourceElements.Image = Tools.GetIcon(Resources.table_select_row, iconSize);
			buttonSelectSourceElements.Text = "";
			buttonStopController.Image = Tools.GetIcon(Resources.control_stop_blue, iconSize);
			buttonStopController.Text = "";
			buttonStartController.Image = Tools.GetIcon(Resources.control_play_blue, iconSize);
			buttonStartController.Text = "";
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);

			comboBoxNewControllerType.BeginUpdate();
			foreach (KeyValuePair<Guid, string> kvp in ApplicationServices.GetAvailableModules<IControllerModuleInstance>()) {
				ComboBoxItem item = new ComboBoxItem(kvp.Value, kvp.Key);
				comboBoxNewControllerType.Items.Add(item);
			}			
			comboBoxNewControllerType.EndUpdate();
            comboBoxNewControllerType.Sorted = true;
            if (comboBoxNewControllerType.Items.Count > 0)
				comboBoxNewControllerType.SelectedIndex = 0;

			controllerTree.ControllerSelectionChanged += controllerTree_ControllerSelectionChanged;
			controllerTree.ControllersChanged += controllerTree_ControllersChanged;

			UpdateForm();
		}

		void controllerTree_ControllerSelectionChanged(object sender, EventArgs e)
		{
			OnControllerSelectionChanged();
			UpdateForm();
		}

		void controllerTree_ControllersChanged(object sender, EventArgs e)
		{
			OnControllersChanged();
			UpdateForm();
		}

		void UpdateForm()
		{
			int selectedControllerCount = controllerTree.SelectedControllers.Count();
			buttonConfigureController.Enabled = selectedControllerCount == 1;
			buttonNumberChannelsController.Enabled = selectedControllerCount == 1;
			buttonRenameController.Enabled = selectedControllerCount == 1;

			buttonDeleteController.Enabled = selectedControllerCount >= 1;

			controllerTree.CheckIfSelectedControllersRunning();
			buttonStartController.Enabled = controllerTree.SomeSelectedControllersNotRunning;
			buttonStopController.Enabled = controllerTree.SomeSelectedControllersRunning;

			buttonAddController.Enabled = comboBoxNewControllerType.SelectedIndex >= 0;

			buttonSelectSourceElements.Enabled = controllerTree.SelectedTreeNodes.Count > 0;

			if (selectedControllerCount <= 0)
			{
				labelControllerType.Text = "";
				labelOutputCount.Text = "";
			} else if (selectedControllerCount == 1) {
				labelControllerType.Text = ApplicationServices.GetModuleDescriptor(controllerTree.SelectedControllers.First().ModuleId).TypeName;
				labelOutputCount.Text = controllerTree.SelectedControllers.First().OutputCount.ToString();
			} else {
				labelControllerType.Text = selectedControllerCount + " controllers selected";
				int count = 0;
				foreach (IControllerDevice controller in controllerTree.SelectedControllers) {
					count += controller.OutputCount;
				}
				labelOutputCount.Text = count.ToString();
			}
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
			controllerTree.RefreshControllerOutputStatus();
		}

		public void UpdateScrollPosition()
		{
			controllerTree.UpdateScrollPosition();
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
				UpdateScrollPosition();
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

			MasterForm.SelectElements(elementNodesToSelect, true);
		}

		private IDataFlowComponent FindRootSourceOfDataComponent(IDataFlowComponent component)
		{
			if (component == null)
				return null;
			
			if (component.Source == null || component.Source.Component == null)
				return component;

			return FindRootSourceOfDataComponent(component.Source.Component);
		}

		private void buttonStartController_Click(object sender, EventArgs e)
		{
			controllerTree.StartController();
			UpdateForm();
		}

		private void buttonStopController_Click(object sender, EventArgs e)
		{
			controllerTree.StopController();
			UpdateForm();
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
