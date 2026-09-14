using Common.Controls;

namespace VixenModules.Analysis.BeatsAndBars
{
	public partial class BeatsAndBarsProgress : BaseForm
	{
		private int m_lastValue = -1;
		private bool m_isFinalizing;
		public BeatsAndBarsProgress()
		{
			InitializeComponent();
			progressBar1.Value = 0;
		}


		public void UpdateProgress(int value)
		{
			if (m_isFinalizing)
			{
				return;
			}

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

		internal void SetFinalizing()
		{
			m_isFinalizing = true;
			generateLabel.Text = "Finalizing beat analysis...";
			percentLabel.Visible = false;
			progressBar1.Style = ProgressBarStyle.Marquee;
		}

	}
}
