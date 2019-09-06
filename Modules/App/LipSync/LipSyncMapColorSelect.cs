using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.ColorManagement.ColorPicker;
using Common.Controls.Theme;
using Common.Resources.Properties;
using Vixen.Module.Effect;
using VixenModules.Property.Color;
using Vixen.Sys;

namespace VixenModules.App.LipSyncApp
{
	public partial class LipSyncMapColorSelect : BaseForm
	{

		public LipSyncMapColorSelect()
		{
			Location = ActiveForm != null ? new Point(ActiveForm.Location.X + 250, ActiveForm.Location.Y + 100) : new Point(500, 200);
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;
		}


		public List<IElementNode> ChosenNodes
		{
			set
			{
				lipSyncMapColorCtrl1.ChosenNodes = value;
			}
		}

		public RGB RGBColor
		{
			get
			{
				return lipSyncMapColorCtrl1.RGBColor;
			}

			set
			{
				lipSyncMapColorCtrl1.RGBColor = value;
			}
		}

		public HSV HSVColor
		{
			get
			{
				return lipSyncMapColorCtrl1.HSVColor;
			}

			set
			{
				lipSyncMapColorCtrl1.HSVColor = value;
			}
		}

		public Color Color
		{
			get 
			{ 
				return lipSyncMapColorCtrl1.Color; 
			}

			set
			{
				lipSyncMapColorCtrl1.Color = value;
			}
		}   

		public double Intensity
		{
			get
			{
				return lipSyncMapColorCtrl1.Intensity;
			}

			set
			{
				lipSyncMapColorCtrl1.Intensity = value;
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
