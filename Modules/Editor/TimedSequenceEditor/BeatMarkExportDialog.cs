using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class BeatMarkExportDialog : Form
	{
		public BeatMarkExportDialog()
		{
			InitializeComponent();
		}

		public bool IsVixen3Selection
		{
			get
			{
				return radioVixen3Format.Checked;
			}
		}

			public bool IsAudacitySelection
		{
			get 
			{
				return radioAudacityFormat.Checked;
			}
		}

			private void BeatMarkExportDialog_Load(object sender, EventArgs e)
			{
				radioVixen3Format.Checked = true;
			}
	}
}
