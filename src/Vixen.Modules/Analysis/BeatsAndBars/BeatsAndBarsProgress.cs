using Common.Controls;

namespace VixenModules.Analysis.BeatsAndBars
{
	public partial class BeatsAndBarsProgress : BaseForm
	{
		private int m_lastValue = -1;
		public BeatsAndBarsProgress()
		{
			InitializeComponent();
			progressBar1.Value = 0;
		}


		public void UpdateProgress(int value)
		{
			if (value != m_lastValue)
			{
				progressBar1.Value = value;
				percentLabel.Text = value.ToString() + "%";

				foreach (Control ctrl in this.Controls)
				{
					ctrl.Refresh();
				}

				m_lastValue = value;
			}
		}

	}
}
