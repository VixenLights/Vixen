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
			ResourceManager lipSyncRM = LipSyncResources.ResourceManager;
			aiButton.Image = new Bitmap((Image)lipSyncRM.GetObject("AI"), new Size(45, 45));
			eButton.Image = new Bitmap((Image)lipSyncRM.GetObject("E"), new Size(45, 45));
			etcButton.Image = new Bitmap((Image)lipSyncRM.GetObject("etc"), new Size(45, 45));
			fvButton.Image = new Bitmap((Image)lipSyncRM.GetObject("FV"), new Size(45, 45));
			lButton.Image = new Bitmap((Image)lipSyncRM.GetObject("L"), new Size(45, 45));
			mbpButton.Image = new Bitmap((Image)lipSyncRM.GetObject("MBP"), new Size(45, 45));
			oButton.Image = new Bitmap((Image)lipSyncRM.GetObject("O"), new Size(45, 45));
			restButton.Image = new Bitmap((Image)lipSyncRM.GetObject("rest"), new Size(45, 45));
			uButton.Image = new Bitmap((Image)lipSyncRM.GetObject("U"), new Size(45, 45));
			wqButton.Image = new Bitmap((Image)lipSyncRM.GetObject("WQ"), new Size(45, 45));

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
