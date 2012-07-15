using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Module;
using Vixen.Sys;
using VixenApplication.Controls;

namespace VixenApplication {
	public partial class PostFilterTemplateForm : Form {
		private ModuleLocalDataSet _dataSet;

		public PostFilterTemplateForm() {
			InitializeComponent();
			_dataSet = new ModuleLocalDataSet();
		}

		private bool _Validate() {
			if(string.IsNullOrWhiteSpace(_TemplateName)) {
				MessageBox.Show("Name is required.");
				return false;
			}

			if(panelContainer.Controls.Count == 0) {
				MessageBox.Show("This template is empty; no outputs would be affected.");
				return false;
			}

			return true;
		}

		private string _TemplateName {
			get { return textBoxName.Text; }
			set { textBoxName.Text = value; }
		}

		private void buttonAdd_Click(object sender, EventArgs e) {
			OutputFilterTemplateControl control = new OutputFilterTemplateControl(_dataSet, "Output " + (panelContainer.Controls.Count + 1));
			control.Dock = DockStyle.Top;
			panelContainer.Controls.Add(control);
			panelContainer.Controls.SetChildIndex(control, 0);
		}

		private void buttonRemove_Click(object sender, EventArgs e) {
			//*** No way to select a template control yet
		}

		private void buttonSave_Click(object sender, EventArgs e) {
			if(_Validate()) {
				OutputFilterTemplate template = new OutputFilterTemplate();
				template.FilePath = _TemplateName;
				foreach(OutputFilterTemplateControl templateControl in panelContainer.Controls.Cast<OutputFilterTemplateControl>().Reverse()) {
					template.AddOutputFilters(new OutputFilterCollection(templateControl.Filters));
				}
				template.Save();
			}
		}
	}
}
