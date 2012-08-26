using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Data.Value;
using Vixen.Sys;

namespace VixenModules.Preview.TestPreview {
	public partial class PositionValueControl : UserControl {
		public PositionValueControl() {
			InitializeComponent();
		}

		public IIntentState<PositionValue> IntentState {
			set { labelPosition.Text = value.GetValue().Position.ToString(); }
		}
	}
}
