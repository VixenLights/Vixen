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
			get { return listBoxMarkCollections.CheckedItems; }
		}

		public int EffectCount
		{
			get { return int.Parse(txtEffectCount.Text); }
			set { txtEffectCount.Text = value.ToString(); }
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
				return txtStartTime.TimeSpan;
			}
			set { txtStartTime.TimeSpan = value; }
		}

		public TimeSpan EndTime
		{
			get
			{
				return txtEndTime.TimeSpan;
			}
			set { txtEndTime.TimeSpan = value; }
		}

		public TimeSpan Duration
		{
			get
			{
				return txtDuration.TimeSpan;
			}
			set { txtDuration.TimeSpan = value; }
		}

		public TimeSpan DurationBetween
		{
			get
			{
				return txtDurationBetween.TimeSpan;
			}
			set { txtDurationBetween.TimeSpan = value; }
		}

		public bool AlignToBeatMarks { get { return checkBoxAlignToBeatMarks.Checked; } }
		public bool FillDuration { get { return checkBoxFillDuration.Checked; } }
		public bool SelectEffects { get { return checkBoxSelectEffects.Checked; } }
		public bool SkipEOBeat { get { return checkBoxSkipEOBeat.Checked; } }

		public bool AlignToMarkStartEnd => chkUseMarkStartEnd.Checked;

		#endregion

		#region Initialization

		public Form_AddMultipleEffects(TimeSpan sequenceLength)
		{
			InitializeComponent();
			btnShowBeatMarkOptions.Image = Resources.bullet_toggle_plus;
			btnHideBeatMarkOptions.Image = Resources.bullet_toggle_minus;
			listBoxMarkCollections.Visible = false;
			txtStartTime.Maximum = sequenceLength;
			txtDuration.Minimum = TimeSpan.FromMilliseconds(10);
			txtDuration.Maximum = sequenceLength;
			txtDurationBetween.Maximum = sequenceLength;
			txtEndTime.Maximum = sequenceLength;
			txtStartTime.ValueChanged += TxtTime_ValueChanged;
			txtDuration.ValueChanged += TxtTime_ValueChanged;
			txtDurationBetween.ValueChanged += TxtTime_ValueChanged;
			txtEndTime.ValueChanged += TxtTime_ValueChanged;
			ThemeUpdateControls.UpdateControls(this);
			panelBeatAlignment.Visible = false;
		}

		private void TxtTime_ValueChanged(object sender, EventArgs e)
		{
			CalculatePossibleEffects();
		}

		private void Form_AddMultipleEffects_Load(object sender, EventArgs e)
		{
			SetCheckboxStates();
			PopulateMarksList();
			CalculatePossibleEffects();
			//SetCheckboxStates();
		}

		#endregion

		#region Event Handlers

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
			if (!TimeExistsForAddition() && !checkBoxAlignToBeatMarks.Checked && !checkBoxFillDuration.Checked)
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
			SetCheckboxStates();
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

		private void SetCheckboxStates()
		{
			listBoxMarkCollections.Enabled = checkBoxFillDuration.AutoCheck =
				checkBoxSkipEOBeat.AutoCheck = checkBoxAlignToBeatMarks.Checked;
			chkUseMarkStartEnd.AutoCheck = checkBoxAlignToBeatMarks.Checked;
			checkBoxSkipEOBeat.ForeColor = checkBoxAlignToBeatMarks.Checked
				? ThemeColorTable.ForeColor
				: ThemeColorTable.ForeColorDisabled;
			checkBoxFillDuration.ForeColor = checkBoxAlignToBeatMarks.Checked
				? ThemeColorTable.ForeColor
				: ThemeColorTable.ForeColorDisabled;
			chkUseMarkStartEnd.ForeColor = checkBoxAlignToBeatMarks.Checked
				? ThemeColorTable.ForeColor
				: ThemeColorTable.ForeColorDisabled;
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
			if (checkBoxFillDuration.Checked)
			{
				chkUseMarkStartEnd.AutoCheck = false;
				chkUseMarkStartEnd.ForeColor = ThemeColorTable.ForeColorDisabled;
			}
			else
			{
				chkUseMarkStartEnd.AutoCheck = true;
				chkUseMarkStartEnd.ForeColor = ThemeColorTable.ForeColor;
			}

			txtDuration.Enabled = (checkBoxFillDuration.Checked ? false : true);
			CalculatePossibleEffectsByBeatMarks();
		}

		private void chkUseMarkStartEnd_CheckStateChanged(object sender, EventArgs e)
		{
			if (chkUseMarkStartEnd.Checked)
			{
				checkBoxFillDuration.AutoCheck = false;
				checkBoxFillDuration.ForeColor = ThemeColorTable.ForeColorDisabled;

				txtDuration.Enabled = false;
			}
			else
			{
				checkBoxFillDuration.AutoCheck = true;
				checkBoxFillDuration.ForeColor = ThemeColorTable.ForeColor;
			}
			CalculatePossibleEffectsByBeatMarks();
		}

		private void btnShowBeatMarkOptions_Click(object sender, EventArgs e)
		{
			btnShowBeatMarkOptions.Visible = false;
			btnHideBeatMarkOptions.Visible = true;
			panelBeatAlignment.Visible = true;
		}

		private void btnHideBeatMarkOptions_Click(object sender, EventArgs e)
		{
			btnHideBeatMarkOptions.Visible = false;
			btnShowBeatMarkOptions.Visible = true;
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
			TimeSpan LastEffectEndTime = StartTime + (TimeSpan.FromTicks(Duration.Ticks * EffectCount) + TimeSpan.FromTicks(DurationBetween.Ticks * (EffectCount - 1)));
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

			int i = 1;
			//This gives us the available amount of time to fill.
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
			txtEffectCount.Maximum = (int)(checkBoxSkipEOBeat.Checked ? Math.Ceiling(possibleEffects / 2d) : possibleEffects);
			lblPossibleEffects.Text = $"{txtEffectCount.Maximum} effects possible.";
			btnOK.Enabled = (Times.Count() == 0 ? false : true);
		}

		#endregion

	}
}
