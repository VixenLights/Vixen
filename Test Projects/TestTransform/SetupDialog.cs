using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;

// By having the setup dialog take no parameters and only have properties set to the edited
// objects' values, the dialog is able to work with more than an assumed type of object.

namespace TestTransform {
	public partial class SetupDialog : Form {
		public SetupDialog() {
			InitializeComponent();
			comboBoxDimmingCurve.DisplayMember = "Name";
			comboBoxDimmingCurve.ValueMember = "Name";
			comboBoxDimmingCurve.DataSource = DimmingCurve.GetAll();
		}

		public string DimmingCurveName {
			get { return comboBoxDimmingCurve.SelectedValue as string; }
			set {
				if(value != null) {
					comboBoxDimmingCurve.SelectedValue = value;
				}
			}
		}
	}
}
