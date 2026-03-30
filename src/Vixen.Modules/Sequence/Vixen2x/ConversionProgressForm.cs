using System.ComponentModel;
using Common.Controls;
using Common.Controls.Theme;


namespace VixenModules.SequenceType.Vixen2x
{
	public partial class ConversionProgressForm : BaseForm
	{
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string StatusLineLabel
		{
			set { lblStatusLine.Text = value; }
		}


		public ConversionProgressForm()
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
		}

		public void UpdateProgressBar(int value)
		{
			pbImport.Value = value;
		}

		public void SetupProgressBar(int min, int max)
		{
			pbImport.Minimum = min;
			pbImport.Maximum = max;
			pbImport.Value = 0;
		}
	}
}