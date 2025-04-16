using System.Globalization;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class EffectTimeEditor : BaseForm
	{
		public EffectTimeEditor(TimeSpan start, TimeSpan duration, TimeSpan sequenceLength)
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			Start = start;
			Duration = duration;
			SequenceLength = sequenceLength;
			End = Start + Duration;
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
				txtStartTime.Maximum = value;
				txtEndTime.Maximum = value;
				txtDuration.Maximum = value;
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
	}
}
