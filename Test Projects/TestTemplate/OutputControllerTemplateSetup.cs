using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Module;
using Vixen.Module.Transform;
using Vixen.Services;
using Vixen.Sys;

namespace TestTemplate {
	public partial class OutputControllerTemplateSetup : Form {
		//Must use instances, they are going to be set up.
		private List<ITransformModuleInstance> _templateTransforms = new List<ITransformModuleInstance>();
		private Dictionary<Guid, string> _transformNames = new Dictionary<Guid, string>();
		private IModuleDataSet _transformDatum;

		public OutputControllerTemplateSetup(IModuleDataSet transformDatum) {
			InitializeComponent();
			_transformDatum = transformDatum;
			//_transformNames = ApplicationServices.GetAvailableModules("Transform");
			_transformNames = ApplicationServices.GetAvailableModules<ITransformModuleInstance>();
			comboBoxTransforms.DisplayMember = "Value";
			comboBoxTransforms.ValueMember = "Key";
			comboBoxTransforms.DataSource = _transformNames.ToArray();
		}

		private void _UpdateTransformList() {
			listBoxTransforms.DataSource = null;
			listBoxTransforms.DisplayMember = "Value";
			listBoxTransforms.ValueMember = "Key";
			listBoxTransforms.DataSource = 
				(from n in _transformNames
				 join t in _templateTransforms on n.Key equals t.Descriptor.TypeId
				 select n).ToArray();
		}

		private void buttonAddTransform_Click(object sender, EventArgs e) {
			if(comboBoxTransforms.SelectedItem != null) {
				ITransformModuleInstance instance = ApplicationServices.Get<ITransformModuleInstance>((Guid)comboBoxTransforms.SelectedValue);
				_transformDatum.AssignModuleInstanceData(instance);
				_templateTransforms.Add(instance);
				_UpdateTransformList();
			}
		}

		private ITransformModuleInstance _SelectedInstance {
			get { return _templateTransforms[listBoxTransforms.SelectedIndex]; }
		}

		private void buttonRemoveTransform_Click(object sender, EventArgs e) {
			if(listBoxTransforms.SelectedItem != null) {
				ITransformModuleInstance instance = _SelectedInstance;
				_transformDatum.RemoveModuleInstanceData(instance.Descriptor.TypeId, instance.InstanceId);
				_templateTransforms.RemoveAt(listBoxTransforms.SelectedIndex);
				_UpdateTransformList();
			}
		}

		public ITransformModuleInstance[] Transforms {
			get { return _templateTransforms.ToArray(); }
			set {
				_templateTransforms.Clear();
				_templateTransforms.AddRange(value);
				_UpdateTransformList();
			}
		}

		private void buttonTransformSetup_Click(object sender, EventArgs e) {
			_SelectedInstance.Setup();
		}
	}
}
