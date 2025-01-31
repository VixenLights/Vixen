using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Output.DummyLighting
{
	public partial class DummyLightingSetup : BaseForm
	{
		public DummyLightingSetup(RenderStyle renderStyle, string formTitle)
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);

			RenderStyle = renderStyle;
			FormTitle = formTitle;
		}

		private RenderStyle _style;

		public RenderStyle RenderStyle
		{
			get { return _style; }
			set
			{
				_style = value;
				if (_style == RenderStyle.Monochrome)
					radioButtonMonochrome.Checked = true;
				else if (_style == RenderStyle.RGBMultiChannel)
					radioButtonMultiRGB.Checked = true;
				else if (_style == RenderStyle.RGBSingleChannel)
					radioButtonSingleRGB.Checked = true;
			}
		}

		public string FormTitle
		{
			get { return textBoxWindowTitle.Text; }
			set { textBoxWindowTitle.Text = value; }
		}

		private void radioButton_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButtonMonochrome.Checked)
				RenderStyle = RenderStyle.Monochrome;
			if (radioButtonMultiRGB.Checked)
				RenderStyle = RenderStyle.RGBMultiChannel;
			if (radioButtonSingleRGB.Checked)
				RenderStyle = RenderStyle.RGBSingleChannel;
		}
	}
}