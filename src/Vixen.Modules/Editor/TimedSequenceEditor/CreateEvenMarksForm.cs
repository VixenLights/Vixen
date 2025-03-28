using System.Globalization;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class CreateEvenMarksForm : BaseForm
	{
		private const string TimeFormat = @"mm\:ss\.fff";
		public CreateEvenMarksForm(TimeSpan? startTime, TimeSpan? endTime)
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			Start = startTime == null ? TimeSpan.Zero : (TimeSpan)startTime;
			End = endTime == null ? TimeSpan.Zero : (TimeSpan)endTime;
			if (Start == End)
				btnOk.Enabled = false;
		}

		private void CreateEvenMarksForm_Load(object sender, EventArgs e)
		{
			txtStartTime.Culture = txtEndTime.Culture = CultureInfo.InvariantCulture;
			txtStartTime.Mask = txtEndTime.Mask = @"00:00.000";
		}

		public TimeSpan End
		{
			get
			{
				TimeSpan endTime;
				TimeSpan.TryParseExact(txtEndTime.Text, TimeFormat, CultureInfo.InvariantCulture, out endTime);
				return endTime;
			}
			set
			{
				txtEndTime.Text = value.ToString(TimeFormat);
			}
		}
		public TimeSpan Start
		{
			set
			{
				txtStartTime.Text = value.ToString(TimeFormat);
			}
			get
			{
				TimeSpan start;
				TimeSpan.TryParseExact(txtStartTime.Text, TimeFormat, CultureInfo.InvariantCulture, out start);
				return start;
			}
		}

		public int Divisions => (int)updownDivide.Value;

		private void txtStartTime_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
		{
			if (txtStartTime.MaskFull)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot enter any more data into the time field.", txtStartTime, 0, -20, 5000);
			}
			else if (e.Position == txtStartTime.Mask.Length)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot add extra number to the end of this time.", txtStartTime, 0, -20, 5000);
			}
			else
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You can only add numeric characters (0-9) into this time field.", txtStartTime, 0, -20, 5000);
			}
			btnOk.Enabled = false;

		}

		private void txtEndTime_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
		{
			if (txtEndTime.MaskFull)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot enter any more data into the time field.", txtEndTime, 0, -20, 3000);
			}
			else if (e.Position == txtEndTime.Mask.Length)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot add extra number to the end of this time.", txtEndTime, 0, -20, 3000);
			}
			else
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You can only add numeric characters (0-9) into this time field.", txtEndTime, 0, -20, 3000);
			}
			btnOk.Enabled = false;
		}

		private void txtStartTime_KeyDown(object sender, KeyEventArgs e)
		{
			toolTip.Hide(txtStartTime);
			if (Start != End)
				btnOk.Enabled = true;
		}

		private void txtStartTime_KeyUp(object sender, KeyEventArgs e)
		{
			TimeSpan start;
			if (TimeSpan.TryParseExact(txtStartTime.Text, TimeFormat, CultureInfo.InvariantCulture, out start) && Start < End)
			{
				btnOk.Enabled = true;
			}
			else
			{
				btnOk.Enabled = false;
			}

		}

		private void txtEndTime_KeyDown(object sender, KeyEventArgs e)
		{
			toolTip.Hide(txtEndTime);
			if (Start != End)
				btnOk.Enabled = true;
		}

		private void txtEndTime_KeyUp(object sender, KeyEventArgs e)
		{
			TimeSpan duration;
			if (TimeSpan.TryParseExact(txtEndTime.Text, TimeFormat, CultureInfo.InvariantCulture, out duration) && End > Start)
			{
				btnOk.Enabled = true;
			}
			else
			{
				btnOk.Enabled = false;
			}
		}
	}
}
