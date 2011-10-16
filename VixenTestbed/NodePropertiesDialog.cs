using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;
using Vixen.Module.Property;

namespace VixenTestbed {
	public partial class NodePropertiesDialog : Form {
		private ChannelNode _node;
		private Dictionary<Guid, string> _availableProperties;

		public NodePropertiesDialog(ChannelNode node) {
			InitializeComponent();
			_node = node;
		}

		private void NodePropertiesDialog_Load(object sender, EventArgs e) {
			try {
				labelNode.Text = "Node: " + _node.Name;

				// All property modules.
				_availableProperties = ApplicationServices.GetAvailableModules<IPropertyModuleInstance>();
				// Properties already on the node.
				Dictionary<Guid, IPropertyModuleInstance> nodeProperties = _node.Properties.ToDictionary(x => x.Descriptor.TypeId);
				// Remove properties already on the node from the list of available modules.
				foreach(Guid typeId in nodeProperties.Keys) {
					_availableProperties.Remove(typeId);
				}
				_UpdateAvailableList();
				_UpdateUsedList();
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private Guid _SelectedAvailable {
			get {
				if(listBoxAvailable.SelectedValue != null) {
					return (Guid)listBoxAvailable.SelectedValue;
				}
				return Guid.Empty;
			}
		}

		private PropertyModuleWrapper _SelectedUsed {
			get { return listBoxUsed.SelectedItem as PropertyModuleWrapper; }
		}

		private void _UpdateAvailableList() {
			listBoxAvailable.DataSource = null;
			listBoxAvailable.DisplayMember = "Value"; // Type name
			listBoxAvailable.ValueMember = "Key";     // Type id
			listBoxAvailable.DataSource = _availableProperties.ToArray();
		}

		private void _UpdateUsedList() {
			listBoxUsed.DataSource = null;
			listBoxUsed.DisplayMember = "Name";
			listBoxUsed.ValueMember = "Id";
			listBoxUsed.DataSource = _node.Properties.Select(x => new PropertyModuleWrapper(x)).ToArray();
		}

		private void buttonSetup_Click(object sender, EventArgs e) {
			try {
				PropertyModuleWrapper wrapper = _SelectedUsed;
				wrapper.Instance.Setup();
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void buttonRemove_Click(object sender, EventArgs e) {
			try {
				if(MessageBox.Show("Removing this will remove any setup data created.  Continue?", "Vixen", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes) {
					PropertyModuleWrapper wrapper = _SelectedUsed;

					// Remove from the used list.
					_node.Properties.Remove(wrapper.TypeId);
					_UpdateUsedList();

					// Add to the available list.
					_availableProperties[wrapper.TypeId] = wrapper.TypeName;
					_UpdateAvailableList();
				}
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void buttonAdd_Click(object sender, EventArgs e) {
			try {
				Guid typeId = _SelectedAvailable;

				// Remove from the available list.
				_availableProperties.Remove(typeId);
				_UpdateAvailableList();

				// Add to the used list.
				IPropertyModuleInstance instance = _node.Properties.Add(typeId);
				_UpdateUsedList();
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void listBoxAvailable_SelectedIndexChanged(object sender, EventArgs e) {
			buttonAdd.Enabled = _SelectedAvailable != Guid.Empty;
		}

		private void listBoxUsed_SelectedIndexChanged(object sender, EventArgs e) {
			buttonRemove.Enabled = listBoxUsed.SelectedItem != null;
			buttonSetup.Enabled = listBoxUsed.SelectedItem != null;
		}
	}

	#region PropertyModuleWrapper
	class PropertyModuleWrapper {
		private IPropertyModuleInstance _instance;

		public PropertyModuleWrapper(IPropertyModuleInstance instance) {
			_instance = instance;
		}

		public Guid TypeId {
			get { return _instance.Descriptor.TypeId; }
		}

		public string TypeName {
			get { return _instance.Descriptor.TypeName; }
		}

		public IPropertyModuleInstance Instance {
			get { return _instance; }
		}

		public override string ToString() {
			return TypeName;
		}
	}
	#endregion
}
