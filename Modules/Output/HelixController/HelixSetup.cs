using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VixenModules.Output.HelixController
{
	public partial class HelixSetup : Form
	{
		public HelixSetup()
		{
			InitializeComponent();
		}

		public int EventData { get; set; }

		private void okButton_Click(object sender, EventArgs e)
		{
			EventData = int.Parse(eventDataTextBox.Text);
		}
	}
}
