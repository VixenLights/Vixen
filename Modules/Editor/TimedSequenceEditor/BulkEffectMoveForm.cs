using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class BulkEffectMoveForm : BaseForm
	{
		private const string TimeFormat = @"mm\:ss\.fff";
		public BulkEffectMoveForm():this(TimeSpan.Zero)
		{
			
		}

		public BulkEffectMoveForm(TimeSpan startTime)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;
			Start = startTime;
			End = startTime;
			Offset = TimeSpan.Zero;
		}

		private void BulkEffectMoveForm_Load(object sender, EventArgs e)
		{
			txtStartTime.Culture = txtEndTime.Culture = txtOffset.Culture = CultureInfo.InvariantCulture;
			txtStartTime.Mask = txtEndTime.Mask = txtOffset.Mask = @"00:00.000";
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

		public TimeSpan Offset
		{
			get
			{
				TimeSpan offset;
				TimeSpan.TryParseExact(txtOffset.Text, TimeFormat, CultureInfo.InvariantCulture, out offset);
				return offset;
			}
			set
			{
				txtOffset.Text = value.ToString(TimeFormat);
			}
		}

		public bool IsForward
		{
			get { return radioButtonForward.Checked; }
		}


		public bool ProcessVisibleRows
		{
			get { return checkBoxVisibleRows.Checked; }
		}
		
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
				toolTip.Show("You cannot enter any more data into the time field.", txtEndTime, 0, -20, 5000);
			}
			else if (e.Position == txtEndTime.Mask.Length)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot add extra number to the end of this time.", txtEndTime, 0, -20, 5000);
			}
			else
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You can only add numeric characters (0-9) into this time field.", txtEndTime, 0, -20, 5000);
			}
			btnOk.Enabled = false;
		}

		private void txtOffset_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
		{
			if (txtOffset.MaskFull)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot enter any more data into the time field.", txtStartTime, 0, -20, 5000);
			}
			else if (e.Position == txtOffset.Mask.Length)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot add extra numbers to the end of this time.", txtStartTime, 0, -20, 5000);
			}
			else
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You can only add numeric characters (0-9) into this time field.", txtStartTime, 0, -20, 5000);
			}
			btnOk.Enabled = false;

		}

		private void txtStartTime_KeyDown(object sender, KeyEventArgs e)
		{
			toolTip.Hide(txtStartTime);
			btnOk.Enabled = true;
		}

		private void txtStartTime_KeyUp(object sender, KeyEventArgs e)
		{
			TimeSpan start;
			if (TimeSpan.TryParseExact(txtStartTime.Text, TimeFormat, CultureInfo.InvariantCulture, out start))
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

		}

		private void txtEndTime_KeyUp(object sender, KeyEventArgs e)
		{
			TimeSpan duration;
			if (TimeSpan.TryParseExact(txtEndTime.Text, TimeFormat, CultureInfo.InvariantCulture, out duration) && End >= Start)
			{
				btnOk.Enabled = true;
			}
			else
			{
				btnOk.Enabled = false;
			}
		}

		private void txtOffset_KeyDown(object sender, KeyEventArgs e)
		{
			toolTip.Hide(txtEndTime);

		}

		private void txtOffset_KeyUp(object sender, KeyEventArgs e)
		{
			TimeSpan duration;
			if (TimeSpan.TryParseExact(txtOffset.Text, TimeFormat, CultureInfo.InvariantCulture, out duration))
			{
				btnOk.Enabled = true;
			}
			else
			{
				btnOk.Enabled = false;
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

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}
	}
}
