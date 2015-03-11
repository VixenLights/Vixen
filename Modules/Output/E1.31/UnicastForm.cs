namespace VixenModules.Controller.E131
{
	using System.Windows.Forms;
    using System.Net;

	public partial class UnicastForm : Form
	{
		public UnicastForm()
		{
			this.InitializeComponent();
            ipTextBox.BringToFront();
		}

		public string IpAddrText
		{
            get { if (networkNameRadio.Checked) return this.networkNameTextBox.Text; else return this.ipTextBox.Text; }

            set {
                IPAddress temp; if (IPAddress.TryParse(value, out temp)) this.ipTextBox.Text = value; else { networkNameTextBox.Text = value; } updateChecked();
            }
		}

		private void UnicastForm_Load(object sender, System.EventArgs e)
		{
		}

		private void ipTextBox_TextChanged(object sender, System.EventArgs e)
		{
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void cancelButton_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

        private void updateChecked()
        {
            if (networkNameRadio.Checked)
            {
                networkNameTextBox.Enabled = true;
                ipTextBox.Enabled = false;
                ipRadio.Checked = false;
                networkNameTextBox.BringToFront();
            }
            else
            {
                networkNameTextBox.Enabled = false;
                ipTextBox.BringToFront();
                ipTextBox.Enabled = true;
                ipRadio.Checked = true;
            }

        }

        private void Radio_CheckedChanged(object sender, System.EventArgs e)
        {
            updateChecked();
        }
	}
}