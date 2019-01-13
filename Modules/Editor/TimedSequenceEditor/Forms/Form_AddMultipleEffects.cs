using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using Vixen.Marks;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class Form_AddMultipleEffects : BaseForm
	{
		#region Member Variables

		private const string timeFormat = @"m\:ss\.fff";
		public IEnumerable<IMarkCollection> MarkCollections { get; set; }

		public ListView.CheckedListViewItemCollection CheckedMarks
		{
			get	{ return listBoxMarkCollections.CheckedItems; }
		}

		public int EffectCount
		{
			get	{ return int.Parse(txtEffectCount.Text); }
			set	{ txtEffectCount.Text = value.ToString(); }
		}

		public string EffectName
		{
			set { this.Text = string.Format("Add {0} effects", value); }
		}

		public TimeSpan SequenceLength { get; set; }

		public TimeSpan StartTime
		{
			get
			{
				TimeSpan StartTime;
				TimeSpan.TryParseExact(txtStartTime.Text, timeFormat, CultureInfo.InvariantCulture, out StartTime);
				return StartTime;
			}
			set { txtStartTime.Text = value.ToString(timeFormat); }
		}

		public TimeSpan EndTime
		{
			get
			{
				TimeSpan EndTime;
				TimeSpan.TryParseExact(txtEndTime.Text, timeFormat, CultureInfo.InvariantCulture, out EndTime);
				return EndTime;
			}
			set { txtEndTime.Text = value.ToString(timeFormat); }
		}

		public TimeSpan Duration
		{
			get
			{
				TimeSpan Duration;
				TimeSpan.TryParseExact(txtDuration.Text, timeFormat, CultureInfo.InvariantCulture, out Duration);
				return Duration;
			}
			set { txtDuration.Text = value.ToString(timeFormat); }
		}

		public TimeSpan DurationBetween
		{
			get
			{
				TimeSpan DurationBetween;
				TimeSpan.TryParseExact(txtDurationBetween.Text, timeFormat, CultureInfo.InvariantCulture, out DurationBetween);
				return DurationBetween;
			}
			set { txtDurationBetween.Text = value.ToString(timeFormat); }
		}

		public bool AlignToBeatMarks { get { return checkBoxAlignToBeatMarks.Checked; } }
		public bool FillDuration { get { return checkBoxFillDuration.Checked; }	}
		public bool SelectEffects {	get { return checkBoxSelectEffects.Checked; } }
		public bool SkipEOBeat { get { return checkBoxSkipEOBeat.Checked; } }

		#endregion

		#region Initialization

		public Form_AddMultipleEffects()
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			listBoxMarkCollections.BackColor = ThemeColorTable.BackgroundColor;
			checkBoxSkipEOBeat.ForeColor = checkBoxAlignToBeatMarks.Checked ? ThemeColorTable.ForeColor : ThemeColorTable.ForeColorDisabled;
			checkBoxFillDuration.ForeColor = checkBoxAlignToBeatMarks.Checked ? ThemeColorTable.ForeColor : ThemeColorTable.ForeColorDisabled;
			btnShowBeatMarkOptions.Image = Resources.bullet_toggle_plus;
			btnShowBeatMarkOptions.Text = "";
			btnHideBeatMarkOptions.Image = Resources.bullet_toggle_minus;
			btnHideBeatMarkOptions.Text = "";
			ThemeUpdateControls.UpdateControls(this);
			panelBeatAlignment.Visible = false;
		}

		private void Form_AddMultipleEffects_Load(object sender, EventArgs e)
		{
			txtStartTime.Culture = txtEndTime.Culture =
				txtDuration.Culture = txtDurationBetween.Culture = CultureInfo.InvariantCulture;
			txtStartTime.Mask = txtEndTime.Mask = txtDuration.Mask = txtDurationBetween.Mask = "0:00.000";
			PopulateMarksList();
			CalculatePossibleEffects();
		}

		#endregion

		#region Event Handlers

		#region txtStartTime

		private void txtStartTime_KeyDown(object sender, KeyEventArgs e)
		{
			toolTip.Hide(txtStartTime);
			btnOK.Enabled = true;
		}

		private void txtStartTime_KeyUp(object sender, KeyEventArgs e)
		{
			TimeSpan StartTime;
			if (TimeSpan.TryParseExact(txtStartTime.Text, timeFormat, CultureInfo.InvariantCulture, out StartTime))
			{
				btnOK.Enabled = true;
				CalculatePossibleEffects();
			}
			else
			{
				btnOK.Enabled = false;
			}
		}

		private void txtStartTime_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
		{
			if (txtStartTime.MaskFull)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot enter any more data into the time field.", txtStartTime, 0, -40, 5000);
			}
			else if (e.Position == txtStartTime.Mask.Length)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot add extra number to the end of this time.", txtStartTime, 0, -40, 5000);
			}
			else
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You can only add numeric characters (0-9) into this time field.", txtStartTime, 0, -40, 5000);
			}
			btnOK.Enabled = false;
		}

		#endregion

		#region txtEndTime

		private void txtEndTime_KeyDown(object sender, KeyEventArgs e)
		{
			toolTip.Hide(txtEndTime);
			btnOK.Enabled = true;
		}

		private void txtEndTime_KeyUp(object sender, KeyEventArgs e)
		{
			TimeSpan EndTime;
			if (TimeSpan.TryParseExact(txtEndTime.Text, timeFormat, CultureInfo.InvariantCulture, out EndTime))
			{
				if (EndTime > SequenceLength)
				{
					toolTip.ToolTipTitle = "Invalid Time";
					toolTip.Show("The given time exceeds seuqence length.", txtEndTime, 0, -40, 5000);
					btnOK.Enabled = false;
				}
				else
				{
					btnOK.Enabled = true;
					CalculatePossibleEffects();
				}
			}
			else
			{
				btnOK.Enabled = false;
			}
		}

		private void txtEndTime_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
		{
			if (txtEndTime.MaskFull)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot enter any more data into the time field.", txtEndTime, 0, -40, 5000);
			}
			else if (e.Position == txtEndTime.Mask.Length)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot add extra number to the end of this time.", txtEndTime, 0, -40, 5000);
			}
			else
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You can only add numeric characters (0-9) into this time field.", txtEndTime, 0, -40, 5000);
			}
			btnOK.Enabled = false;
		}

		#endregion

		#region txtDuration

		private void txtDuration_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
		{
			if (txtDuration.MaskFull)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot enter any more data into the time field.", txtDuration, 0, -40, 5000);
			}
			else if (e.Position == txtDuration.Mask.Length)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot add extra number to the end of this time.", txtDuration, 0, -40, 5000);
			}
			else
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You can only add numeric characters (0-9) into this time field.", txtDuration, 0, -40, 5000);
			}
			btnOK.Enabled = false;
		}

		private void txtDuration_KeyDown(object sender, KeyEventArgs e)
		{
			toolTip.Hide(txtDuration);
			btnOK.Enabled = true;
		}

		private void txtDuration_KeyUp(object sender, KeyEventArgs e)
		{
			TimeSpan Duration;
			if (TimeSpan.TryParseExact(txtDuration.Text, timeFormat, CultureInfo.InvariantCulture, out Duration) && Duration.TotalSeconds > .009)
			{
				btnOK.Enabled = true;
				CalculatePossibleEffects();
			}
			else
			{
				if (TimeSpan.TryParseExact(txtDuration.Text, timeFormat, CultureInfo.InvariantCulture, out Duration) && Duration.TotalSeconds < .01)
				{
					toolTip.ToolTipTitle = "Invalid Time";
					toolTip.Show("Minimum effect duration is .01 seconds.", txtDuration, 0, -40, 5000);
				}
				btnOK.Enabled = false;
			}
		}

		#endregion

		#region txtDurationBetween

		private void txtDurationBetween_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
		{
			if (txtDurationBetween.MaskFull)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot enter any more data into the time field.", txtDurationBetween, 0, -40, 5000);
			}
			else if (e.Position == txtDurationBetween.Mask.Length)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot add extra number to the end of this time.", txtDurationBetween, 0, -40, 5000);
			}
			else
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You can only add numeric characters (0-9) into this time field.", txtDurationBetween, 0, -40, 5000);
			}
			btnOK.Enabled = false;
		}

		private void txtDurationBetween_KeyDown(object sender, KeyEventArgs e)
		{
			toolTip.Hide(txtDurationBetween);
			btnOK.Enabled = true;
		}

		private void txtDurationBetween_KeyUp(object sender, KeyEventArgs e)
		{
			TimeSpan DurationBetween;
			if (TimeSpan.TryParseExact(txtDurationBetween.Text, timeFormat, CultureInfo.InvariantCulture, out DurationBetween))
			{
				btnOK.Enabled = true;
				CalculatePossibleEffects();
			}
			else btnOK.Enabled = false;
		}

		#endregion

		#region Other Event Handlers

		private void lblPossibleEffects_DoubleClick(object sender, EventArgs e)
		{
			txtEffectCount.Value = txtEffectCount.Maximum;
		}

		private void txtEffectCount_KeyDown(object sender, KeyEventArgs e)
		{
			toolTip.Hide(txtEffectCount);
			btnOK.Enabled = true;
		}

		private void txtEffectCount_KeyUp(object sender, KeyEventArgs e)
		{
			int EffectCount;
			btnOK.Enabled = (int.TryParse(txtEffectCount.Text, out EffectCount) ? true : false);
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (txtEffectCount.Value == 0)
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Exclamation; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("OOPS! Your effect count is set to 0 (zero)", "Warning", false, false);
				messageBox.ShowDialog();
				DialogResult = DialogResult.None;
			}
			//Double check for calculations
			if (!TimeExistsForAddition() && !checkBoxAlignToBeatMarks.Checked && !checkBoxFillDuration.Checked )
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Exclamation; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("At least one effect would be placed beyond the sequence length, and will not be added.\n\nWould you like to proceed anyway?", "Warning", true, false);
				messageBox.ShowDialog();
				if (messageBox.DialogResult == DialogResult.No)
				{
					DialogResult = DialogResult.None;
				}
			}
		}

		private void checkBoxAlignToBeatMarks_CheckStateChanged(object sender, EventArgs e)
		{
			txtDurationBetween.Enabled = (checkBoxAlignToBeatMarks.Checked ? false : true);
			listBoxMarkCollections.Visible = !listBoxMarkCollections.Visible;
			listBoxMarkCollections.Enabled = checkBoxFillDuration.AutoCheck = checkBoxSkipEOBeat.AutoCheck = checkBoxAlignToBeatMarks.Checked;
			checkBoxSkipEOBeat.ForeColor = checkBoxAlignToBeatMarks.Checked ? ThemeColorTable.ForeColor : ThemeColorTable.ForeColorDisabled;
			checkBoxFillDuration.ForeColor = checkBoxAlignToBeatMarks.Checked ? ThemeColorTable.ForeColor : ThemeColorTable.ForeColorDisabled;
			if (checkBoxAlignToBeatMarks.Checked)
			{
				CalculatePossibleEffectsByBeatMarks();
			}
			else
			{
				CalculatePossibleEffects();
			}

			if (checkBoxAlignToBeatMarks.Checked)
			{
				var names = new HashSet<String>();
				foreach (var mc in MarkCollections)
				{
					if (!names.Add(mc.Name))
					{
						//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
						MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
						var messageBox = new MessageBoxForm("You have Beat Mark collections with duplicate names.\nBecause of this, your results may not be as expected.", "Duplicate Names", false, false);
						messageBox.ShowDialog();
						break;
					}
				}
				
			}
		}

		private void checkBoxSkipEOBeat_CheckedChanged(object sender, EventArgs e)
		{
			if (checkBoxAlignToBeatMarks.Checked)
			{
				CalculatePossibleEffectsByBeatMarks();
			}
			else
			{
				CalculatePossibleEffects();
			}
		}

		private void listBoxMarkCollections_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			if (!checkBoxAlignToBeatMarks.Checked) //Because this event is fired when the control is loaded
				return;
			if (checkBoxAlignToBeatMarks.Checked)
			{
				CalculatePossibleEffectsByBeatMarks();
			}
			else
			{
				CalculatePossibleEffects();
			}
		}

		private void checkBoxFillDuration_CheckStateChanged(object sender, EventArgs e)
		{
			txtDuration.Enabled = (checkBoxFillDuration.Checked ? false : true);
			CalculatePossibleEffectsByBeatMarks();
		}

		private void btnShowBeatMarkOptions_Click(object sender, EventArgs e)
		{
			btnShowBeatMarkOptions.Visible = false;
			btnHideBeatMarkOptions.Visible = true;
			panelBeatAlignment.Visible = true;
			panelBeatAlignment.Visible = true;
		}

		private void btnHideBeatMarkOptions_Click(object sender, EventArgs e)
		{
			btnHideBeatMarkOptions.Visible = false;
			btnShowBeatMarkOptions.Visible = true;
			panelBeatAlignment.Visible = false;
			panelBeatAlignment.Visible = false;
		}

		#endregion 

		#endregion

		#region Private Methods

		private void PopulateMarksList()
		{
			listBoxMarkCollections.Items.Clear();
			foreach (var mc in MarkCollections)
			{
				if (mc.Name != null)
				{
					var lvItems = new ListViewItem();
					lvItems.ForeColor = mc.Decorator.Color;
					lvItems.Text = mc.Name;
					listBoxMarkCollections.Items.Add(lvItems);
				}
			}
		}

		private bool TimeExistsForAddition()
		{
			TimeSpan LastEffectEndTime = StartTime + (TimeSpan.FromTicks(Duration.Ticks * EffectCount) + TimeSpan.FromTicks(DurationBetween.Ticks * (EffectCount -1)));
			//return (LastEffectEndTime > SequenceLength ? false : true);
			return (LastEffectEndTime > EndTime ? false : true);
		}

		private void CalculatePossibleEffects()
		{
			if (checkBoxAlignToBeatMarks.Checked)
			{
				CalculatePossibleEffectsByBeatMarks(); //Get an update when the Start Time is edited by the user
				return;
			}

			//This gives us the available amount of time to fill.
			int i = 1;
			//TimeSpan WorkingTime = SequenceLength - StartTime;
			TimeSpan WorkingTime = EndTime - StartTime;
			TimeSpan UsesTime = (TimeSpan.FromTicks(Duration.Ticks * i) + TimeSpan.FromTicks(DurationBetween.Ticks * (i - 1))); ;
			while (UsesTime < WorkingTime)
			{
				i++;
				UsesTime = (TimeSpan.FromTicks(Duration.Ticks * i) + TimeSpan.FromTicks(DurationBetween.Ticks * (i - 1)));
			}
			//Get rid of the straw that broke the camels back.
			i--;
			txtEffectCount.Maximum = i;
			lblPossibleEffects.Text = string.Format("{0} effects possible.", i);
			btnOK.Enabled = (i == 0 ? false : true);
		}

		private void CalculatePossibleEffectsByBeatMarks()
		{
			//Returns the number of possible effects based on the selected beat mark collection(s)

			List<TimeSpan> Times = new List<TimeSpan>();

			foreach (ListViewItem lvi in CheckedMarks)
			{
				foreach (var item in MarkCollections)
				{
					if (item.Name == lvi.Text)
					{
						foreach (var mark in item.Marks)
						{
							if (mark.StartTime >= StartTime && mark.StartTime < EndTime) Times.Add(mark.StartTime);
						}
					}
				}
			}
			var possibleEffects = (checkBoxFillDuration.Checked ? Times.Count() - 1 : Times.Count());
			if (possibleEffects < 0) possibleEffects = 0;
			txtEffectCount.Maximum = (checkBoxSkipEOBeat.Checked ? possibleEffects / 2 : possibleEffects);
			lblPossibleEffects.Text = string.Format("{0} effects possible.", (checkBoxSkipEOBeat.Checked ? possibleEffects / 2 : possibleEffects));
			btnOK.Enabled = (Times.Count() == 0 ? false : true);
		}

		#endregion

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
