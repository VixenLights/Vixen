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
	public partial class CommandValueControl : UserControl {
		public CommandValueControl() {
			InitializeComponent();
		}

		public IIntentState<CommandValue> IntentState {
			set { labelCommandType.Text = value.GetValue().Command.ToString(); }
		}
	}
}
