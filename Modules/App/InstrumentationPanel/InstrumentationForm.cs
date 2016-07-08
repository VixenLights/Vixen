using System;
using System.Linq;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using Vixen.Sys;

namespace VixenModules.App.InstrumentationPanel
{
	public partial class InstrumentationForm : BaseForm
	{
		public InstrumentationForm()
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			textBox1.ForeColor = ThemeColorTable.ForeColor;
			textBox1.BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;
		}

		private void InstrumentationForm_Load(object sender, EventArgs e)
		{
			timer.Start();
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			string[] lines = VixenSystem.Instrumentation.Values.Select(x => string.Format("{0}: {1}", x.Name , x.FormattedValue)).ToArray();
			textBox1.Lines = lines;
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btnReset_Click(object sender, EventArgs e)
		{
			foreach (var instrumentationValue in VixenSystem.Instrumentation.Values)
			{
				instrumentationValue.Reset();	
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