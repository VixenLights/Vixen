using System.Globalization;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class EffectTimeEditor : BaseForm
	{
		public EffectTimeEditor(TimeSpan start, TimeSpan duration, TimeSpan sequenceLength, TimeSpan minimumDuration)
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			Start = start;
			Duration = duration;
			SequenceLength = sequenceLength;
			End = Start + Duration;

			txtDuration.Minimum = minimumDuration;
			txtStartTime.Maximum = sequenceLength - minimumDuration;
			txtEndTime.Minimum = Start + minimumDuration;

			txtStartTime.ValueChanged += Time_Changed;
			txtEndTime.ValueChanged += Time_Changed;
			txtDuration.ValueChanged += Time_Changed;
		}


		public TimeSpan Duration
		{
			get
			{
				return txtDuration.TimeSpan;
			}
			set
			{
				txtDuration.TimeSpan = value;
			}
		}
		public TimeSpan Start
		{
			get 
			{
				return txtStartTime.TimeSpan;	
			}
			set
			{
				txtStartTime.TimeSpan = value;
			}
		}
		public TimeSpan End
		{
			get
			{
				return txtEndTime.TimeSpan;
			}
			set
			{
				txtEndTime.TimeSpan = value;
			}
		}

        private TimeSpan _sequenceLength;
        private TimeSpan SequenceLength
        {
            get => _sequenceLength;
            set
            {
                _sequenceLength = value;
                txtStartTime.Maximum = value;
                txtEndTime.Maximum = value;
                txtDuration.Maximum = _sequenceLength.Subtract(txtStartTime.TimeSpan);
            }
        }

        /// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}

			base.Dispose(disposing);
		}

		private void btnSetFullSequence_Click(object sender, EventArgs e)
		{
			Start = TimeSpan.Zero;
			End = SequenceLength;
			Duration = SequenceLength;
		}

		private void Time_Changed(object sender, EventArgs e)
		{
			// Turn off event handlers to prevent multiple firings while we update values.
			txtStartTime.ValueChanged -= Time_Changed;
			txtEndTime.ValueChanged -= Time_Changed;
			txtDuration.ValueChanged -= Time_Changed;

			// We're updating the Start Time, so validate the End and Duration times
			if (sender == txtStartTime)
			{
				End = Start.Add(Duration);
				if (Start + Duration > _sequenceLength)
				{
					Duration = _sequenceLength.Subtract(Start);
				}
				txtDuration.Maximum = _sequenceLength.Subtract(Start);
				txtEndTime.Minimum = Start + txtDuration.Minimum;
			}

			// We're updating the Duration Time, so validate the Start Time
			else if (sender == txtDuration)
			{
				End = Start.Add(Duration);
			}

			// We're updating the End Time, so validate the Duration Time
			else if (sender == txtEndTime)
			{
				Duration = End.Subtract(Start);
			}

			// Turn event handlers back on.
			txtStartTime.ValueChanged += Time_Changed;
			txtEndTime.ValueChanged += Time_Changed;
			txtDuration.ValueChanged += Time_Changed;
		}
	}
}
