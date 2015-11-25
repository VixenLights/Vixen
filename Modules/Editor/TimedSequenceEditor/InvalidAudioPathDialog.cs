using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class InvalidAudioPathDialog : BaseForm
	{
		public InvalidAudioDialogResult InvalidAudioDialogResult { get; set; }
		
		public InvalidAudioPathDialog(string audioPath)
		{
			InitializeComponent();
			Icon = Resources.Icon_Vixen3;
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			label1.Text = string.Format("The audio file {0} could not be found at the following location:",
				Path.GetFileName(audioPath));
			labelAudioPath.Text = Path.GetFullPath(audioPath);
		}

		private void buttonLocateAudio_Click(object sender, EventArgs e)
		{
			InvalidAudioDialogResult = InvalidAudioDialogResult.LocateAudio;
			Close();
		}

		private void buttonRemoveAudio_Click(object sender, EventArgs e)
		{
			InvalidAudioDialogResult = InvalidAudioDialogResult.RemoveAudio;
			Close();
		}

		private void buttonKeepAudio_Click(object sender, EventArgs e)
		{
			InvalidAudioDialogResult = InvalidAudioDialogResult.KeepAudio;
			Close();
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImage;

		}
	}

	public enum InvalidAudioDialogResult
	{
		LocateAudio = 0,
		RemoveAudio = 1,
		KeepAudio =2
	}
}
