using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls.Theme;
using Common.Resources;
using Common.Resources.Properties;

namespace Common.Controls
{
	public partial class MessageBoxForm : BaseForm
	{
		public static Icon msgIcon;

		public MessageBoxForm(string messageBoxData, string messageBoxTitle, bool buttonNoVisible, bool buttonCancelVisible)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			labelPrompt.Text = messageBoxData;
			this.Text = messageBoxTitle;
			buttonNo.Visible = buttonNoVisible;
			buttonCancel.Visible = buttonCancelVisible;
			if (!buttonCancelVisible & !buttonNoVisible)
			{
				buttonOk.Location = buttonCancel.Location;
			}
			else
			{
				buttonOk.Text = "YES";
			}
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.Properties.Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.Properties.Resources.ButtonBackgroundImage;

		}

		private void messageIcon_Paint(object sender, PaintEventArgs e)
		{
			if (msgIcon != null)
				e.Graphics.DrawIcon(msgIcon, 24, 24);
		}

		private void MessageBoxForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			msgIcon = null;
		}

		private void buttonOk_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}