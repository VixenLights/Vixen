using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources;
using Common.Resources.Properties;

namespace VixenModules.App.LipSyncApp
{
	public partial class LipSyncTextConvertFailForm : BaseForm
	{

		public LipSyncTextConvertFailForm()
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;
		}

		private void LipSyncTextConvertFailForm_Load(object sender, EventArgs e)
		{
			aiButton.Image = Tools.GetIcon(Resources.AI, 45);
			eButton.Image = Tools.GetIcon(Resources.E, 45);
			etcButton.Image = Tools.GetIcon(Resources.etc, 45);
			fvButton.Image = Tools.GetIcon(Resources.FV, 45);
			lButton.Image = Tools.GetIcon(Resources.L, 45);
			mbpButton.Image = Tools.GetIcon(Resources.MBP, 45);
			oButton.Image = Tools.GetIcon(Resources.O, 45);
			restButton.Image = Tools.GetIcon(Resources.rest, 45);
			uButton.Image = Tools.GetIcon(Resources.U, 45);
			wqButton.Image = Tools.GetIcon(Resources.WQ, 45);

		}

		private void buttonClear_Click(object sender, EventArgs e)
		{
			phonemeTextBox.Clear();
		}

		private void aiButton_Click(object sender, EventArgs e)
		{
			phonemeTextBox.Text += "AI ";
		}

		private void eButton_Click(object sender, EventArgs e)
		{
			phonemeTextBox.Text += "E ";
		}

		private void etcButton_Click(object sender, EventArgs e)
		{
			phonemeTextBox.Text += "ETC ";
		}

		private void fvButton_Click(object sender, EventArgs e)
		{
			phonemeTextBox.Text += "FV ";
		}

		private void lButton_Click(object sender, EventArgs e)
		{
			phonemeTextBox.Text += "L ";
		}

		private void mbpButton_Click(object sender, EventArgs e)
		{
			phonemeTextBox.Text += "MBP ";
		}

		private void oButton_Click(object sender, EventArgs e)
		{
			phonemeTextBox.Text += "O ";
		}

		private void restButton_Click(object sender, EventArgs e)
		{
			phonemeTextBox.Text += "REST ";
		}

		private void uButton_Click(object sender, EventArgs e)
		{
			phonemeTextBox.Text += "U ";
		}

		private void wqButton_Click(object sender, EventArgs e)
		{
			phonemeTextBox.Text += "WQ ";
		}

		private void buttonOk_Click(object sender, EventArgs e)
		{
			TranslatedString = phonemeTextBox.Text;
		}

		public string TranslatedString
		{
			get 
			{
				return phonemeTextBox.Text;
			}

			set
			{
				phonemeTextBox.Text = value;
			}
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
}
