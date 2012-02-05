using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;
using Vixen.Module.Output;
using Vixen.Sys.Output;

namespace VixenTestbed {
	public partial class ControllersForm : Form {
		private List<OutputController> _controllers = new List<OutputController>();

		public ControllersForm() {
			InitializeComponent();
		}

		private void ControllersForm_Load(object sender, EventArgs e) {
			try {
				_LoadOutputModules();
				_LoadControllers();
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void _LoadOutputModules() {
			comboBoxOutputModule.DisplayMember = "Value";
			comboBoxOutputModule.ValueMember = "Key";
			comboBoxOutputModule.DataSource = ApplicationServices.GetAvailableModules<IOutputModuleInstance>().ToArray();
		}

		private void _LoadControllers() {
			listViewControllers.Items.Clear();
			_controllers.Clear();

			_controllers.AddRange(VixenSystem.Controllers.Cast<OutputController>());
			foreach(OutputController controller in _controllers) {
				_AddControllerToView(controller);
			}

			_ResetLinkedToCombo();
		}

		private void _AddControllerToView(OutputController controller) {
			ListViewItem item = new ListViewItem(new string[] {
                controller.Name,
				controller.OutputCount.ToString(),
				ApplicationServices.GetModuleDescriptor<IOutputModuleDescriptor>(controller.OutputModuleId).TypeName
            });
			item.Tag = controller;
			listViewControllers.Items.Add(item);
		}

		private OutputController _SelectedController {
			get {
				if(listViewControllers.SelectedItems.Count > 0) {
					return listViewControllers.SelectedItems[0].Tag as OutputController;
				}
				return null;
			}
		}

		private string _ControllerName {
			get { return textBoxControllerName.Text.Trim(); }
			set { textBoxControllerName.Text = value; }
		}

		private int _OutputCount {
			get { return (int)numericUpDownOutputCount.Value; }
			set { numericUpDownOutputCount.Value = value; }
		}

		private Guid _OutputModule {
			get {
				if(comboBoxOutputModule.SelectedValue != null) {
					return (Guid)comboBoxOutputModule.SelectedValue;
				}
				return Guid.Empty;
			}
			set { comboBoxOutputModule.SelectedValue = value; }
		}

		private Guid? _LinkedTo {
			get {
				if(comboBoxLinkedTo.SelectedValue != null) {
					return (Guid)comboBoxLinkedTo.SelectedValue;
				}
				return null;
			}
			set { comboBoxLinkedTo.SelectedValue = value; }
		}

		private void _UpdateLinkCombo() {
			OutputController controller = _SelectedController;
			_LinkedTo = (controller != null) ? VixenSystem.ControllerLinking.GetPrior(controller.Id) : null;
		}

		private void _ResetLinkedToCombo() {
			comboBoxLinkedTo.DataSource = null;
			comboBoxLinkedTo.DisplayMember = "Name";
			comboBoxLinkedTo.ValueMember = "Id";
			comboBoxLinkedTo.DataSource = _controllers;
		}

		private void _DisplayControllerDetails(OutputController controller) {
			if(controller != null) {
				_ControllerName = controller.Name;
				_OutputCount = controller.OutputCount;
				_OutputModule = controller.OutputModuleId;
				buttonControllerSetup.Enabled = controller.HasSetup;
				buttonUpdateController.Enabled = true;
				buttonDeleteController.Enabled = true;
			} else {
				_ControllerName = null;
				_OutputCount = 1;
				_OutputModule = Guid.Empty;
				buttonControllerSetup.Enabled = false;
				buttonUpdateController.Enabled = false;
				buttonDeleteController.Enabled = false;
			}
			_UpdateLinkCombo();
		}

		private bool _Validate() {
			if(_ControllerName.Length == 0) {
				MessageBox.Show("Controller name is necessary.");
				return false;
			}
			
			if(comboBoxOutputModule.SelectedValue == null) {
				MessageBox.Show("Output module is necessary.");
				return false;
			}

			return true;
		}

		private void buttonControllerSetup_Click(object sender, EventArgs e) {
			try {
				OutputController controller = _SelectedController;
				if(controller != null) {
					controller.Setup();
				}
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void buttonAddController_Click(object sender, EventArgs e) {
			try {
				if(_Validate()) {
					OutputController controller = new OutputController(_ControllerName, _OutputCount, _OutputModule);
					VixenSystem.Controllers.Add(controller);
					_AddControllerToView(controller);
					_controllers.Add(controller);
					_ResetLinkedToCombo();
				}
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void buttonUpdateController_Click(object sender, EventArgs e) {
			try {
				if(_Validate()) {
					OutputController controller = _SelectedController;
					if(controller != null) {
						controller.OutputCount = _OutputCount;
						controller.OutputModuleId = _OutputModule;
					} else {
						MessageBox.Show("Controller must be selected.");
					}
				}
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void buttonDeleteController_Click(object sender, EventArgs e) {
			try {
				if(_SelectedController != null) {
					VixenSystem.Controllers.Remove(_SelectedController);
					_LoadControllers();
					_DisplayControllerDetails(null);
				}
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void listViewControllers_SelectedIndexChanged(object sender, EventArgs e) {
			_DisplayControllerDetails(_SelectedController);
		}

		private void buttonLinkController_Click(object sender, EventArgs e) {
			try {
				if(_LinkedTo != null && _SelectedController != null) {
					Guid parentControllerId = _controllers.First(x => x.Id == _LinkedTo).Id;
					VixenSystem.ControllerLinking.LinkController(_SelectedController.Id, parentControllerId);
				}
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void buttonRemoveControllerLink_Click(object sender, EventArgs e) {
			try {
				if(_SelectedController != null) {
					VixenSystem.ControllerLinking.RemoveControllerFromParent(_SelectedController.Id);
					_UpdateLinkCombo();
				}
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}
	}
}
