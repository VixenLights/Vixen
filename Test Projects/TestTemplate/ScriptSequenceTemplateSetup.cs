using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.RuntimeBehavior;

namespace TestTemplate {
	public partial class ScriptSequenceTemplateSetup : Form {
		private ScriptSequenceTemplate _template;

		public ScriptSequenceTemplateSetup(ScriptSequenceTemplate template) {
			InitializeComponent();
			_template = template;

			foreach(IModuleDescriptor descriptor in ApplicationServices.GetModuleDescriptors("RuntimeBehavior")) {
				checkedListBoxRuntimeBehaviors.Items.Add(new DescriptorItem(descriptor), _template.EnabledBehaviors.Contains(descriptor.TypeId));
			}

			if(template.Length == Vixen.Sys.ScriptSequence.Forever) {
				checkBoxForever.Checked = true;
				textBoxLength.Text = "0";
			} else {
				textBoxLength.Text = template.Length.ToString();
			}
			textBoxTimingInterval.Text = template.TimingInterval.ToString();
		}

		private void buttonOK_Click(object sender, EventArgs e) {
			long length;
			int timingInterval;

			if(checkBoxForever.Checked) {
				length = Vixen.Sys.ScriptSequence.Forever;
			} else {
				if(!long.TryParse(textBoxLength.Text, out length)) {
					MessageBox.Show("Length is invalid.");
					return;
				}
			}

			if(!int.TryParse(textBoxTimingInterval.Text, out timingInterval)) {
				MessageBox.Show("Timing interval is invalid.");
				return;
			}

			_template.Length = length;
			_template.TimingInterval = timingInterval;
			_template.EnabledBehaviors = checkedListBoxRuntimeBehaviors.CheckedItems.Cast<DescriptorItem>().Select(x => x.Id).ToArray();
		}

		private void checkBoxForever_CheckedChanged(object sender, EventArgs e) {
			textBoxLength.Enabled = !checkBoxForever.Checked;
		}

		#region DescriptorItem
		class DescriptorItem {
			private IModuleDescriptor _descriptor;

			public DescriptorItem(IModuleDescriptor descriptor) {
				_descriptor = descriptor;
			}

			public Guid Id {
				get { return _descriptor.TypeId; }
			}

			public string Name {
				get { return _descriptor.TypeName; }
			}

			public override string ToString() {
				return Name;
			}
		}
		#endregion
	}
}
