using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;

namespace TestClient {
	public partial class FixtureConfigurationDialog : Form {
		private Fixture _fixture = null;

		public FixtureConfigurationDialog() {
			InitializeComponent();
		}

		public FixtureConfigurationDialog(Fixture fixture)
			: this() {
			this.Fixture = fixture;
		}

		public Fixture Fixture {
			get { return _fixture; }
			private set {
				_fixture = value;
				_Name = _fixture.Name;
				_ChannelNames = _fixture.Channels.Select(x => x.Name).ToArray();
			}
		}

		private void textBoxChannelNames_TextChanged(object sender, EventArgs e) {
			labelChannelCount.Text = "(" + _ChannelNames.Length + ")";
		}

		private void buttonOK_Click(object sender, EventArgs e) {
			string[] errors = _GetValidationErrors();

			if(errors.Length > 0) {
				MessageBox.Show(string.Join(Environment.NewLine, errors), "Vixen", MessageBoxButtons.OK, MessageBoxIcon.Stop);
			} else {
				if(_fixture == null) {
					_fixture = new Fixture(_Name);
				} else {
					_fixture.Name = _Name;
				}

				_fixture.ClearChannels();
				foreach(string channelName in _ChannelNames) {
					_fixture.InsertChannel(new OutputChannel(channelName));
				}

				DialogResult = DialogResult.OK;
			}
		}

		private string[] _GetValidationErrors() {
			List<string> errors = new List<string>();

			if(string.IsNullOrWhiteSpace(textBoxName.Text)) {
				errors.Add("A fixture name is required.");
			}
			if(_ChannelNames.Length == 0) {
				errors.Add("A fixture requires at least one channel.");
			}
			
			return errors.ToArray();
		}

		private string _Name {
			get { return textBoxName.Text; }
			set { textBoxName.Text = value; }
		}

		private string[] _ChannelNames {
			get {
				return textBoxChannelNames.Lines.Where(x => x.Trim().Length > 0).ToArray();
			}
			set {
				textBoxChannelNames.Lines = value;
			}
		}
	}
}
