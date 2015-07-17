using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Resources.Properties;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class BeatMarkExportDialog : Form
	{
		public BeatMarkExportDialog()
		{
			InitializeComponent();
			buttonCancel.BackgroundImage = Resources.HeadingBackgroundImage;
			buttonOK.BackgroundImage = Resources.HeadingBackgroundImage;
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

			private void buttonBackground_MouseHover(object sender, EventArgs e)
			{
				var btn = (Button)sender;
				btn.BackgroundImage = Resources.HeadingBackgroundImageHover;
			}

			private void buttonBackground_MouseLeave(object sender, EventArgs e)
			{
				var btn = (Button)sender;
				btn.BackgroundImage = Resources.HeadingBackgroundImage;
			}
	}
}
