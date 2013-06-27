namespace VixenModules.Controller.E131
{
	using System.Windows.Forms;

	public partial class UnicastForm : Form
	{
		public UnicastForm()
		{
			this.InitializeComponent();
		}

		public string IpAddrText
		{
			get { return this.ipTextBox.Text; }

			set { this.ipTextBox.Text = value; }
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
	}
}