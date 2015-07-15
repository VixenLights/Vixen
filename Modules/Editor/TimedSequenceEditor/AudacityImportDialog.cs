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
	public partial class AudacityImportDialog : Form
	{
		public AudacityImportDialog()
		{
			InitializeComponent();
			btnCancel.BackgroundImage = Resources.HeadingBackgroundImage;
			btnOk.BackgroundImage = Resources.HeadingBackgroundImage;
		}

		public bool IsVampBeatSelection
		{
			get
			{
				return radioBeats.Checked;
			}
		}

		public bool IsVampBarSelection
		{
			get
			{
				return radioBars.Checked;
			}
		}

		public bool IsAudacityBeatSelection
		{
			get
			{
				return radioAudacityBeats.Checked;
			}
		}

		public bool IsVixen3BeatSelection
		{
			get
			{
				return radioVixen3Beats.Checked;
			}
		}

		private void btnOk_MouseHover(object sender, EventArgs e)
		{

			btnOk.BackgroundImage = Resources.HeadingBackgroundImageHover;
		}

		private void btnOk_MouseLeave(object sender, EventArgs e)
		{

			btnOk.BackgroundImage = Resources.HeadingBackgroundImage;
		}

		private void btnCancel_MouseHover(object sender, EventArgs e)
		{

			btnCancel.BackgroundImage = Resources.HeadingBackgroundImageHover;
		}

		private void btnCancel_MouseLeave(object sender, EventArgs e)
		{

			btnCancel.BackgroundImage = Resources.HeadingBackgroundImage;
		}
	}
}
