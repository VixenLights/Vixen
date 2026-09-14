using Common.Controls;

namespace VixenModules.Analysis.BeatsAndBars
{
	public sealed partial class BeatsAndBarsProgress : BaseForm
	{
		private int _lastValue = -1;
		private bool _isFinalizing;
		public BeatsAndBarsProgress()
		{
			InitializeComponent();
			progressBar1.Value = 0;
		}


		public void UpdateProgress(int value)
		{
			if (_isFinalizing)
			{
				return;
			}

			if (value != _lastValue)
			{
				progressBar1.Value = value;
				percentLabel.Text = value + @"%";

				foreach (Control ctrl in Controls)
				{
					ctrl.Refresh();
				}

				_lastValue = value;
			}
		}

		internal void SetFinalizing()
		{
			_isFinalizing = true;
			generateLabel.Text = "Finalizing beat analysis...";
			percentLabel.Visible = false;
			progressBar1.Style = ProgressBarStyle.Marquee;
		}

	}
}
