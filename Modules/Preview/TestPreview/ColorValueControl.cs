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
	public partial class ColorValueControl : UserControl {
		public ColorValueControl() {
			InitializeComponent();
		}

		public IIntentState<ColorValue> IntentState {
			set { pictureBox.BackColor = value.GetValue().Color; }
		}
	}
}
