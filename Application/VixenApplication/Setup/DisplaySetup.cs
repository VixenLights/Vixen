using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Rule;
using Vixen.Sys;
using VixenApplication.Setup;
using VixenApplication.Setup.ElementTemplates;

namespace VixenApplication
{
	public partial class DisplaySetup : Form
	{
		private SetupElementsTree _setupElementsTree;
		private SetupElementsPreview _setupElementsPreview;

		private SetupPatchingSimple _setupPatchingSimple;
		private SetupPatchingGraphical _setupPatchingGraphical;

		private SetupControllersSimple _setupControllersSimple;

		private ISetupElementsControl _currentElementControl;
		private ISetupPatchingControl _currentPatchingControl;
		private ISetupControllersControl _currentControllersControl;

		private IElementTemplate[] _elementTemplates;
		private IElementSetupHelper[] _elementSetupHelpers;

		public DisplaySetup()
		{
			InitializeComponent();

			Icon = Common.Resources.Properties.Resources.Icon_Vixen3;

			_elementTemplates = Vixen.Services.ApplicationServices.GetAllElementTemplates();
			_elementSetupHelpers = Vixen.Services.ApplicationServices.GetAllElementSetupHelpers();
		}

		private void DisplaySetup_Load(object sender, EventArgs e)
		{
			_setupElementsTree = new SetupElementsTree(_elementTemplates, _elementSetupHelpers);
			_setupElementsTree.Dock = DockStyle.Fill;
			_setupElementsTree.MasterForm = this;
			_setupElementsPreview = new SetupElementsPreview();
			_setupElementsPreview.Dock = DockStyle.Fill;
			_setupElementsPreview.MasterForm = this;

			_setupPatchingSimple = new SetupPatchingSimple();
			_setupPatchingSimple.Dock = DockStyle.Fill;
			_setupPatchingSimple.MasterForm = this;
			_setupPatchingGraphical = new SetupPatchingGraphical();
			_setupPatchingGraphical.Dock = DockStyle.Fill;
			_setupPatchingGraphical.MasterForm = this;

			_setupControllersSimple = new SetupControllersSimple();
			_setupControllersSimple.Dock = DockStyle.Fill;
			_setupControllersSimple.MasterForm = this;

			radioButtonElementTree.Checked = true;
			radioButtonPatchingSimple.Checked = true;
			radioButtonControllersStandard.Checked = true;

			buttonHelp.Image = new Bitmap(Common.Resources.Properties.Resources.help, new Size(16, 16));
		}


		private void activateElementControl(ISetupElementsControl control)
		{
			if (_currentElementControl != null) {
				_currentElementControl.ElementSelectionChanged -=  control_ElementSelectionChanged;
				_currentElementControl.ElementsChanged -= control_ElementsChanged;
			}

			_currentElementControl = control;

			control.ElementSelectionChanged +=  control_ElementSelectionChanged;
			control.ElementsChanged += control_ElementsChanged;

			tableLayoutPanelElementSetup.Controls.Clear();
			tableLayoutPanelElementSetup.Controls.Add(control.SetupElementsControl);

			control.UpdatePatching();
		}

		void control_ElementsChanged(object sender, EventArgs e)
		{
			if (_currentPatchingControl != null) {
				_currentPatchingControl.UpdateElementDetails(_currentElementControl.SelectedElements);
			}
		}

		void control_ElementSelectionChanged(object sender, ElementNodesEventArgs e)
		{
			if (_currentPatchingControl != null) {
				_currentPatchingControl.UpdateElementSelection(e.ElementNodes);
			}
		}



		private void activatePatchingControl(ISetupPatchingControl control)
		{
			if (_currentPatchingControl != null) {
				_currentPatchingControl.FiltersAdded-=  control_FiltersAdded;
				_currentPatchingControl.PatchingUpdated -= control_PatchingUpdated;
			}

			_currentPatchingControl = control;

			control.FiltersAdded += control_FiltersAdded;
			control.PatchingUpdated += control_PatchingUpdated;

			tableLayoutPanelPatchingSetup.Controls.Clear();
			tableLayoutPanelPatchingSetup.Controls.Add(control.SetupPatchingControl);


			if (_currentControllersControl == null) {
				control.UpdateControllerSelection(new ControllersAndOutputsSet());
			} else {
				control.UpdateControllerSelection(_currentControllersControl.SelectedControllersAndOutputs);				
			}
			if (_currentElementControl == null) {
				control.UpdateElementSelection(Enumerable.Empty<Vixen.Sys.ElementNode>());
			} else {
				control.UpdateElementSelection(_currentElementControl.SelectedElements);				
			}
		}

		void control_PatchingUpdated(object sender, EventArgs e)
		{
			if (_currentElementControl != null) {
				_currentElementControl.UpdatePatching();
			}

			if (_currentControllersControl != null) {
				_currentControllersControl.UpdatePatching();
			}
		}

		void control_FiltersAdded(object sender, FiltersEventArgs e)
		{
		}



		private void activateControllersControl(ISetupControllersControl control)
		{
			if (_currentControllersControl != null) {
				_currentControllersControl.ControllerSelectionChanged -=  control_ControllerSelectionChanged;
				_currentControllersControl.ControllersChanged -= control_ControllersChanged;
			}

			_currentControllersControl = control;

			control.ControllerSelectionChanged += control_ControllerSelectionChanged;
			control.ControllersChanged += control_ControllersChanged;

			tableLayoutPanelControllerSetup.Controls.Clear();
			tableLayoutPanelControllerSetup.Controls.Add(control.SetupControllersControl);

			control.UpdatePatching();
		}

		void control_ControllersChanged(object sender, EventArgs e)
		{
			if (_currentPatchingControl != null) {
				_currentPatchingControl.UpdateControllerDetails(_currentControllersControl.SelectedControllersAndOutputs);
			}
		}

		void control_ControllerSelectionChanged(object sender, ControllerSelectionEventArgs e)
		{
			if (_currentPatchingControl != null) {
				_currentPatchingControl.UpdateControllerSelection(e.ControllersAndOutputs);
			}
		}






		private void radioButtonElementTree_CheckedChanged(object sender, EventArgs e)
		{
			if ((sender as RadioButton).Checked)
				activateElementControl(_setupElementsTree);
		}

		private void radioButtonElementPreview_CheckedChanged(object sender, EventArgs e)
		{
			if ((sender as RadioButton).Checked)
				activateElementControl(_setupElementsPreview);
		}

		private void radioButtonPatchingSimple_CheckedChanged(object sender, EventArgs e)
		{
			if ((sender as RadioButton).Checked)
				activatePatchingControl(_setupPatchingSimple);
		}

		private void radioButtonPatchingGraphical_CheckedChanged(object sender, EventArgs e)
		{
			if ((sender as RadioButton).Checked)
				activatePatchingControl(_setupPatchingGraphical);
		}

		private void radioButtonControllersStandard_CheckedChanged(object sender, EventArgs e)
		{
			if ((sender as RadioButton).Checked)
				activateControllersControl(_setupControllersSimple);
		}

		private void buttonHelp_Click(object sender, EventArgs e)
		{
			Common.VixenHelp.VixenHelp.ShowHelp(Common.VixenHelp.VixenHelp.HelpStrings.Setup_Main);
		}



		public void SelectElements(IEnumerable<ElementNode> elements)
		{
			_currentElementControl.SelectedElements = elements;
		}

		public void SelectControllersAndOutputs(ControllersAndOutputsSet controllersAndOutputs)
		{
			_currentControllersControl.SelectedControllersAndOutputs = controllersAndOutputs;
		}
	}
}
