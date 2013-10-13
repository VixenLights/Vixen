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
	public partial class AudacityImportDialog : Form
	{
		public AudacityImportDialog()
		{
			InitializeComponent();
		}

		public bool IsBeatSelection
		{
			get
			{
				return radioBeats.Checked;
			}
		}

		public bool IsBarSelection
		{
			get
			{
				return radioBars.Checked;
			}
		}
	}
}
