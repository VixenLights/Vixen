using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.Controls;
using Common.Resources;
using VixenModules.Sequence.Timed;
using Vixen.Execution;
using Vixen.Module.Timing;
using VixenModules.Media.Audio;
using System.Collections.Concurrent;
using System.IO;
using Common.Resources.Properties;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using Common.Controls.Scaling;
using Common.Controls.Theme;


namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class MarkManager : BaseForm
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		private MarkCollection _displayedCollection = null;
		private bool _updatingListContents = false;
		private IExecutionControl _executionControl;
		private ITiming _timingSource;
		private TimedSequenceEditorForm _timedSequenceEditorForm;
		private TimeSpan _currentPlayPosition = TimeSpan.Zero;
		private float _timingChangeDelta = .25f;
		private TimeSpan _lastMarkHit;
		private bool _playbackStarted = false;
		private bool _sequencePlaySelected = false;
		private List<TimeSpan> _newTappedMarks = new List<TimeSpan>();
		private Audio _audio = null;
		private decimal delayStartValue;
		private int delayCounter;

		public MarkManager(List<MarkCollection> markCollections, IExecutionControl executionControl, ITiming timingSource,
						   TimedSequenceEditorForm timedSequenceEditorForm)
		{
			InitializeComponent();
			Icon = Resources.Icon_Vixen3;
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			
			listViewMarkCollections.BackColor = ThemeColorTable.BackgroundColor;
			listViewMarks.BackColor = ThemeColorTable.BackgroundColor;
			int iconSize = (int)(24 * ScalingTools.GetScaleFactor());
			buttonPlay.Image = Tools.GetIcon(Resources.control_play_blue, iconSize);
			buttonPlay.Text = "";
			buttonStop.Image = Tools.GetIcon(Resources.control_stop_blue, iconSize);
			buttonStop.Text = "";
			buttonStop.Enabled = false;
			buttonRestartPlay.Text = "";
			buttonRestartPlay.Image = Tools.GetIcon(Resources.control_start_blue,iconSize);
			buttonIncreasePlaybackSpeed.Image = Tools.GetIcon(Resources.add, iconSize);
			buttonIncreasePlaybackSpeed.Text = "";
			buttonDecreasePlaySpeed.Image = Tools.GetIcon(Resources.minus, iconSize);
			buttonDecreasePlaySpeed.Text = "";
			buttonIncreaseSelectedMarks.Image = Tools.GetIcon(Resources.add, iconSize);
			buttonIncreaseSelectedMarks.Text = "";
			buttonDecreaseSelectedMarks.Image = Tools.GetIcon(Resources.minus, iconSize);
			buttonDecreaseSelectedMarks.Text = "";
			buttonRemoveCollection.ForeColor = buttonRemoveCollection.Enabled ? ThemeColorTable.ForeColor : ThemeColorTable.ForeColorDisabled;
			ThemeUpdateControls.UpdateControls(this);
			labelTapperInstructions.Visible = false;

			MarkCollections = markCollections;
			_executionControl = executionControl;
			_timingSource = timingSource;
			_timedSequenceEditorForm = timedSequenceEditorForm;
			textBoxPosition.Text = TimeSpan.FromMilliseconds(trackBarPlayBack.Value).ToString(@"m\:ss\.fff");
		}

		public List<MarkCollection> MarkCollections { get; set; }

		private void MarkManager_Load(object sender, EventArgs e)
		{
			PopulateMarkCollectionsList();
			PopulateFormWithMarkCollection(null, true);
			updateTimingSpeedTextbox();
			trackBarPlayBack.SetRange(0, (int) _timedSequenceEditorForm.Sequence.Length.TotalMilliseconds);
			if (_timedSequenceEditorForm.Sequence.SequenceData.Media.Count > 0)
			{
				btnAutoDetectionSettings.Enabled = true;
				btnCreateCollections.Enabled = true;
				ChkAutoTapper.AutoCheck = true;
				radioAll.AutoCheck = true;
				radioSelected.AutoCheck = true;
				chkHighPass.AutoCheck = true;
				chkLowPass.AutoCheck = true;
				numHighPass.Enabled = true;
				numLowPass.Enabled = true;
				radioButtonTapper.AutoCheck = true;
				radioButtonPlayback.AutoCheck = true;
				ChkAutoTapper.ForeColor = ThemeColorTable.ForeColor;
				radioAll.ForeColor = ThemeColorTable.ForeColor;
				radioSelected.ForeColor = ThemeColorTable.ForeColor;
				chkHighPass.ForeColor = ThemeColorTable.ForeColor;
				chkLowPass.ForeColor = ThemeColorTable.ForeColor;
				numHighPass.ForeColor = ThemeColorTable.ForeColor;
				numLowPass.ForeColor = ThemeColorTable.ForeColor;
				radioButtonTapper.ForeColor = ThemeColorTable.ForeColor;
				radioButtonPlayback.ForeColor = ThemeColorTable.ForeColor;
			}
			else
			{
				btnAutoDetectionSettings.Enabled = false;
				btnCreateCollections.Enabled = false;
				ChkAutoTapper.AutoCheck = false;
				radioAll.AutoCheck = false;
				radioSelected.AutoCheck = false;
				chkHighPass.AutoCheck = false;
				chkLowPass.AutoCheck = false;
				numHighPass.Enabled = false;
				numLowPass.Enabled = false;
				radioButtonTapper.AutoCheck = false;
				radioButtonPlayback.AutoCheck = false;
				ChkAutoTapper.ForeColor = ThemeColorTable.ForeColorDisabled;
				radioAll.ForeColor = ThemeColorTable.ForeColorDisabled;
				radioSelected.ForeColor = ThemeColorTable.ForeColorDisabled;
				chkHighPass.ForeColor = ThemeColorTable.ForeColorDisabled;
				chkLowPass.ForeColor = ThemeColorTable.ForeColorDisabled;
				numHighPass.ForeColor = ThemeColorTable.ForeColorDisabled;
				numLowPass.ForeColor = ThemeColorTable.ForeColorDisabled;
				radioButtonTapper.ForeColor = ThemeColorTable.ForeColorDisabled;
				radioButtonPlayback.ForeColor = ThemeColorTable.ForeColorDisabled;
			}
		}

		private double GetGrayValueForColor(Color c)
		{
			return c.R*0.299 + c.G*0.587 + c.B*0.114;
		}

		private void PopulateMarkCollectionsList()
		{
			_updatingListContents = true;
			listViewMarkCollections.BeginUpdate();
			listViewMarkCollections.Items.Clear();

			foreach (MarkCollection mc in MarkCollections) {
				ListViewItem item = new ListViewItem();
				item.Text = mc.Name;
				item.SubItems.Add(mc.Level.ToString());
				item.SubItems.Add(mc.MarkCount.ToString());
				item.ForeColor = mc.MarkColor;
				item.Tag = mc;

				if (mc == _displayedCollection)
					item.Selected = true;
				listViewMarkCollections.Items.Add(item);
			}

			listViewMarkCollections.EndUpdate();
			_updatingListContents = false;
		}

		private void UpdateMarkCollectionInList(MarkCollection collection)
		{
			foreach (ListViewItem item in listViewMarkCollections.Items) {
				if (item.Tag as MarkCollection == collection) {
					item.SubItems[0].Text = collection.Name;
					item.SubItems[1].Text = collection.Level.ToString();
					item.SubItems[2].Text = collection.Marks.Count.ToString();
					item.ForeColor = collection.MarkColor;
				}
			}
		}

		private void PopulateFormWithMarkCollection(MarkCollection collection, bool forceUpdate = false)
		{
			if (_displayedCollection == collection && !forceUpdate)
				return;

			_displayedCollection = collection;

			if (collection == null) {
				textBoxCollectionName.Text = "";
				numericUpDownWeight.Value = 1;
				checkBoxEnabled.Checked = false;
				checkBoxBold.Checked = false;
				checkBoxSolidLine.Checked = false;
			}
			else {
				textBoxCollectionName.Text = collection.Name;
				numericUpDownWeight.Value = collection.Level;
				checkBoxEnabled.Checked = collection.Enabled;
				checkBoxBold.Checked = collection.Bold;
				checkBoxSolidLine.Checked = collection.SolidLine;
			}

			PopulateMarkListFromMarkCollection(collection);

			checkBoxEnabled.AutoCheck = (collection != null);
			checkBoxBold.AutoCheck = (collection != null);
			checkBoxSolidLine.AutoCheck = (collection != null);
			textBoxCollectionName.Enabled = (collection != null);
			numericUpDownWeight.Enabled = (collection != null);
			panelColor.Enabled = (collection != null);
			groupBoxOperations.Enabled = (collection != null);
			buttonSelectAllMarks.Enabled = (collection != null);
			buttonAddOrUpdateMark.Enabled = (collection != null);
			textBoxTime.Enabled = (collection != null);
		}

		private void PopulateMarkListFromMarkCollection(MarkCollection collection)
		{
			listViewMarks.BeginUpdate();
			listViewMarks.Items.Clear();

			string timeFormat = @"m\:ss\.fff";

			if (collection != null) {
				collection.Marks.Sort();
				foreach (TimeSpan time in collection.Marks) {
					ListViewItem item = new ListViewItem(time.ToString(timeFormat));
					item.Tag = time;
					listViewMarks.Items.Add(item);
				}
			}

			textBoxTime.Text = "";
			buttonAddOrUpdateMark.Text = "Add";

			listViewMarks.EndUpdate();
		}

		private void listViewMarkCollections_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_updatingListContents)
				return;

			if (listViewMarkCollections.SelectedItems.Count > 0) {
				PopulateFormWithMarkCollection(listViewMarkCollections.SelectedItems[0].Tag as MarkCollection);
				panelColor.BackColor = listViewMarkCollections.SelectedItems[0].ForeColor;
			}
			else
			{
				panelColor.BackColor = ThemeColorTable.BackgroundColor;
				PopulateFormWithMarkCollection(null);
			}

			buttonRemoveCollection.Enabled = (listViewMarkCollections.SelectedItems.Count > 0);
		}

		private void listViewMarks_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewMarks.SelectedItems.Count == 1) {
				textBoxTime.Text = listViewMarks.SelectedItems[0].Text;
				buttonAddOrUpdateMark.Text = "Update";
			}
			else {
				textBoxTime.Text = "";
				buttonAddOrUpdateMark.Text = "Add";
			}
		}

		private void buttonAddCollection_Click(object sender, EventArgs e)
		{
			AddNewCollection(ThemeColorTable.ForeColor);
	//		Refresh();
			//MarkCollection newCollection = new MarkCollection();
			//newCollection.Name = "New Collection";
			//MarkCollections.Add(newCollection);

			//// populate the form with the new collection first, *then* update the collection list,
			//// so that the displayed collection is remembered and selected in the list.
			//PopulateFormWithMarkCollection(newCollection);
			//PopulateMarkCollectionsList();

			////enable Tapper as long as a collection is selected.
			//radioButtonTapper.Enabled = (listViewMarkCollections.SelectedItems.Count > 0);
			//radioButtonPlayback.Checked = true;
		}

		private void buttonRemoveCollection_Click(object sender, EventArgs e)
		{
			DeleteSelectedMarkCollections();
		}

		private void DeleteSelectedMarkCollections()
		{
			if (listViewMarkCollections.SelectedItems.Count > 0)
			{
				foreach (ListViewItem item in listViewMarkCollections.SelectedItems)
				{
					MarkCollection mc = item.Tag as MarkCollection;
					MarkCollections.Remove(mc);
				}
			}

			if (!MarkCollections.Contains(_displayedCollection))
			{
				PopulateFormWithMarkCollection(null);
			}

			PopulateMarkCollectionsList();
			if (listViewMarkCollections.Items.Count > 0)
			{
				listViewMarkCollections.Items[0].Selected = true;
				listViewMarkCollections.Items[0].Focused = true;
			}
		}

		private void listViewMarkCollections_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
			{
				DeleteSelectedMarkCollections();
			}
		}

		private void buttonAddOrUpdateMark_Click(object sender, EventArgs e)
		{
			TimeSpan time;
			bool success = TimeSpan.TryParseExact(textBoxTime.Text, TimeFormats.PositiveFormats, null, out time);
			if (success) {
				if (buttonAddOrUpdateMark.Text == "Update") {
					// updating an existing item, find it, remove it, and add the new one
					if (listViewMarks.SelectedItems.Count != 1)
						Logging.Error(string.Format("MarkManager: updating a mark, but there are {0} items selected!", listViewMarks.SelectedItems.Count));

					if (listViewMarks.SelectedItems.Count > 0)
						_displayedCollection.Marks.Remove((TimeSpan) listViewMarks.SelectedItems[0].Tag);
				}

				if (_displayedCollection.Marks.Contains(time)) {
					System.Media.SystemSounds.Asterisk.Play();
				}
				else {
					_displayedCollection.Marks.Add(time);
					_displayedCollection.Marks.Sort();
					PopulateMarkListFromMarkCollection(_displayedCollection);
					UpdateMarkCollectionInList(_displayedCollection);
				}
			}
			else
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Error parsing time: please use the format '<minutes>:<seconds>.<milliseconds>'", "Error parsing time", false, false);
				messageBox.ShowDialog();
			}
		}

		private void buttonSelectAllMarks_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem item in listViewMarks.Items) {
				item.Selected = true;
			}
		}

		private void buttonTapNewMarks_Click(object sender, EventArgs e)
		{
			using (MarkTapper tapper = new MarkTapper(_executionControl, _timingSource)) {
				if (tapper.ShowDialog() == DialogResult.OK) {
					foreach (TimeSpan time in tapper.Results) {
						if (!_displayedCollection.Marks.Contains(time))
							_displayedCollection.Marks.Add(time);
					}
					PopulateMarkListFromMarkCollection(_displayedCollection);
					UpdateMarkCollectionInList(_displayedCollection);
				}
			}
		}

		private void buttonOffsetMarks_Click(object sender, EventArgs e)
		{
			Common.Controls.TextDialog prompt = new Common.Controls.TextDialog("Time to offset (in seconds):");
			if (prompt.ShowDialog() == DialogResult.OK) {
				TimeSpan time;

				if (TimeSpan.TryParseExact(prompt.Response, TimeFormats.AllFormats, null, out time)) {
					// this is hackey as shit.
					if (prompt.Response.ToCharArray()[0] == '-')
						time = -time;

					List<TimeSpan> newMarks = new List<TimeSpan>();
					foreach (ListViewItem item in listViewMarks.Items) {
						if (item.Selected) {
							newMarks.Add(((TimeSpan) item.Tag) + time);
						}
						else {
							newMarks.Add((TimeSpan) item.Tag);
						}
						newMarks.Sort();
						_displayedCollection.Marks = newMarks;
						PopulateMarkListFromMarkCollection(_displayedCollection);
					}
				}
				else
				{
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("Error parsing time: please use the format '<minutes>:<seconds>.<milliseconds>'", "Error parsing time", false, false);
					messageBox.ShowDialog();
				}
			}
		}

		private void buttonEvenlySpaceMarks_Click(object sender, EventArgs e)
		{
			if (listViewMarks.SelectedItems.Count < 3)
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Select at least three marks to space evenly.", "Select more marks", false, false);
				messageBox.ShowDialog();
				return;
			}

			List<TimeSpan> spacingTimes = new List<TimeSpan>();
			List<TimeSpan> newTimes = new List<TimeSpan>();

			foreach (ListViewItem item in listViewMarks.Items) {
				if (item.Selected)
					spacingTimes.Add((TimeSpan) item.Tag);
				else
					newTimes.Add((TimeSpan) item.Tag);
			}

			spacingTimes.Sort();

			TimeSpan totalInterval = spacingTimes.Last() - spacingTimes.First();
			TimeSpan integralInterval = TimeSpan.FromTicks(totalInterval.Ticks/(spacingTimes.Count - 1));
			for (int i = 0; i < spacingTimes.Count; i++) {
				TimeSpan newTime = TimeSpan.FromTicks(spacingTimes.First().Ticks + (i*integralInterval.Ticks));
				if (!newTimes.Contains(newTime))
					newTimes.Add(newTime);
			}

			newTimes.Sort();
			_displayedCollection.Marks = newTimes;
			PopulateMarkListFromMarkCollection(_displayedCollection);
		}

		private void buttonGenerateSubmarks_Click(object sender, EventArgs e)
		{
			if (listViewMarks.SelectedItems.Count < 2)
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Select at least two marks to generate times between.", "Select more marks", false, false);
				messageBox.ShowDialog();
				return;
			}

			Common.Controls.TextDialog prompt =
				new Common.Controls.TextDialog("Break each interval into how many equal segments?");
			if (prompt.ShowDialog() == DialogResult.OK) {
				int divisions;
				if (int.TryParse(prompt.Response, out divisions))
				{
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Question; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("Do you want to put the new marks into a different collection?", "Add to new collection?", true, true);
					messageBox.ShowDialog();
					if (messageBox.DialogResult == DialogResult.Cancel)
					{
						return;
					}

					List<TimeSpan> sourceTimes = new List<TimeSpan>();
					foreach (ListViewItem item in listViewMarks.SelectedItems) {
						sourceTimes.Add((TimeSpan) item.Tag);
					}
					sourceTimes.Sort();

					List<TimeSpan> newTimes = new List<TimeSpan>();
					for (int i = 1; i < sourceTimes.Count; i++) {
						TimeSpan interval = TimeSpan.FromTicks((sourceTimes[i] - sourceTimes[i - 1]).Ticks/divisions);
						for (int j = 0; j < divisions; j++) {
							newTimes.Add(sourceTimes[i - 1] + TimeSpan.FromTicks(interval.Ticks*j));
						}
					}
					newTimes.Add(sourceTimes.Last());

					MarkCollection destination = _displayedCollection;

					if (messageBox.DialogResult == DialogResult.OK)
					{
						List<KeyValuePair<string, object>> options = new List<KeyValuePair<string, object>>();
						foreach (MarkCollection mc in MarkCollections)
						{
							options.Add(new KeyValuePair<string, object>(mc.Name, mc));
						}
						ListSelectDialog selector = new ListSelectDialog("Destination Mark Collection?", options);
						if (selector.ShowDialog() == DialogResult.OK)
						{
							destination = selector.SelectedItem as MarkCollection;
						}
					}
					foreach (TimeSpan time in newTimes) {
						if (!destination.Marks.Contains(time))
							destination.Marks.Add(time);
					}

					destination.Marks.Sort();

					if (destination == _displayedCollection) {
						PopulateMarkListFromMarkCollection(_displayedCollection);
					}
					UpdateMarkCollectionInList(destination);
				}
				else
				{
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("Error parsing number: please enter a whole number for the number of divisions.", "Error parsing number", false, false);
					messageBox.ShowDialog();
				}
			}
		}

		private void textBoxCollectionName_TextChanged(object sender, EventArgs e)
		{
			if (_displayedCollection != null && _displayedCollection.Name != textBoxCollectionName.Text) {
				_displayedCollection.Name = textBoxCollectionName.Text;
				UpdateMarkCollectionInList(_displayedCollection);
			}
		}

		private void numericUpDownWeight_ValueChanged(object sender, EventArgs e)
		{
			if (_displayedCollection != null && _displayedCollection.Level != numericUpDownWeight.Value) {
				_displayedCollection.Level = (int) numericUpDownWeight.Value;
				UpdateMarkCollectionInList(_displayedCollection);
			}
		}

		private void panelColor_Click(object sender, EventArgs e)
		{
			Common.Controls.ColorManagement.ColorPicker.ColorPicker picker =
				new Common.Controls.ColorManagement.ColorPicker.ColorPicker();

			DialogResult result = picker.ShowDialog();
			if (result == DialogResult.OK) {
				_displayedCollection.MarkColor = picker.Color.ToRGB().ToArgb();
				panelColor.BackColor = _displayedCollection.MarkColor;
				UpdateMarkCollectionInList(_displayedCollection);
			}
		}

		private void checkBoxEnabled_CheckedChanged(object sender, EventArgs e)
		{
			if (_displayedCollection != null && _displayedCollection.Enabled != checkBoxEnabled.Checked)
			{
				_displayedCollection.Enabled = checkBoxEnabled.Checked;
				UpdateMarkCollectionInList(_displayedCollection);
			}
		}

		private void listViewMarks_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode) {
				case Keys.Delete:
					foreach (ListViewItem item in listViewMarks.SelectedItems) {
						_displayedCollection.Marks.Remove((TimeSpan) item.Tag);
					}
					PopulateMarkListFromMarkCollection(_displayedCollection);
					UpdateMarkCollectionInList(_displayedCollection);
					break;

				case Keys.Up:
					if (listViewMarks.SelectedIndices.Count > 0)
						increaseSelectedMarks();
					break;

				case Keys.Down:
					if (listViewMarks.SelectedIndices.Count > 0) {
						decreaseSelectedMarks();
						e.Handled = true;
					}
					break;
			}
		}

		private void buttonPasteEffectsToMarks_Click(object sender, EventArgs e)
		{
			if (listViewMarks.SelectedItems.Count < 1)
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Select at least one mark to paste effects to.", "Select more marks", false, false);
				messageBox.ShowDialog();
				return;
			}

			int count = 0;
			foreach (ListViewItem item in listViewMarks.SelectedItems) {
				int totalPasted = _timedSequenceEditorForm.ClipboardPaste((TimeSpan) item.Tag);
				if (totalPasted <= 0)
				{
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("Copy an effect to paste to the clipboard in the sequence editor.", "Need an effect", false, false);
					messageBox.ShowDialog();
					return;
				}
				count += totalPasted;
			}
			//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
			MessageBoxForm.msgIcon = SystemIcons.Information; //this is used if you want to add a system icon to the message form.
			var messageBox2 = new MessageBoxForm(string.Format("{0} effects pasted.", count), "Effects Pasted", false, false);
			messageBox2.ShowDialog();
		}

		private void buttonCopyAndOffsetMarks_Click(object sender, EventArgs e)
		{
			if (listViewMarks.SelectedItems.Count < 1)
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Select at least one mark duplicate and offset.", "Select more marks", false, false);
				messageBox.ShowDialog();
				return;
			}

			Common.Controls.TextDialog prompt = new Common.Controls.TextDialog("Start time for copied marks (in seconds):");
			if (prompt.ShowDialog() == DialogResult.OK) {
				TimeSpan offsetTime;

				if (TimeSpan.TryParseExact(prompt.Response, TimeFormats.PositiveFormats, null, out offsetTime)) {
					TimeSpan earliestTime = TimeSpan.MaxValue;
					foreach (ListViewItem item in listViewMarks.SelectedItems) {
						if ((TimeSpan) item.Tag < earliestTime)
							earliestTime = (TimeSpan) item.Tag;
					}

					foreach (ListViewItem item in listViewMarks.SelectedItems) {
						_displayedCollection.Marks.Add((TimeSpan) item.Tag + offsetTime - earliestTime);
					}

					_displayedCollection.Marks.Sort();
					PopulateMarkListFromMarkCollection(_displayedCollection);
					UpdateMarkCollectionInList(_displayedCollection);
				}
				else
				{
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("Error parsing time: please use the format '<minutes>:<seconds>.<milliseconds>'", "Error parsing time", false, false);
					messageBox.ShowDialog();
				}
			}
		}

		private void buttonGenerateBeatMarks_Click(object sender, EventArgs e)
		{
			//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
			MessageBoxForm.msgIcon = SystemIcons.Question; //this is used if you want to add a system icon to the message form.
			var messageBox = new MessageBoxForm("This operation will determine the average beat from the selected marks, and apply them for the rest of the song. Do you want to continue?", "Information", true, false);
			if (messageBox.ShowDialog() == DialogResult.No)
				return;

			if (listViewMarks.SelectedItems.Count < 2)
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				messageBox = new MessageBoxForm("Select at least two marks to be able to determine an average time interval.", "Select more marks", false, false);
				messageBox.ShowDialog();
				return;
			}

			Common.Controls.TextDialog prompt =
				new Common.Controls.TextDialog(
					"How long should the beats be generated for, in seconds? Leave blank to go to the end.");
			// the default prompt isn't enough to hold all the above text. Oops.
			prompt.Size = new Size(550, prompt.Size.Height);
			if (prompt.ShowDialog() == DialogResult.OK) {
				TimeSpan duration;
				bool conversionSuccess = TimeSpan.TryParseExact(prompt.Response, TimeFormats.PositiveFormats, null, out duration);
				if (!conversionSuccess && prompt.Response.Length == 0) {
					conversionSuccess = true;
					duration = TimeSpan.MaxValue;
				}

				if (conversionSuccess) {
					TimeSpan earlistMark = TimeSpan.MaxValue;
					TimeSpan latestMark = TimeSpan.MinValue;

					foreach (ListViewItem item in listViewMarks.SelectedItems) {
						if ((TimeSpan) item.Tag < earlistMark) {
							earlistMark = (TimeSpan) item.Tag;
						}
						if ((TimeSpan) item.Tag > latestMark) {
							latestMark = (TimeSpan) item.Tag;
						}
					}

					int sourcesCount = listViewMarks.SelectedItems.Count;
					TimeSpan interval = TimeSpan.FromTicks((latestMark - earlistMark).Ticks/(sourcesCount - 1));
					double bpm = 60/interval.TotalSeconds;

					TimeSpan maxPossibleDuration = _timedSequenceEditorForm.Sequence.Length - latestMark;
					if (duration > maxPossibleDuration)
						duration = maxPossibleDuration;

					int generatedMarks = (int) (duration.Ticks/interval.Ticks) - 1;

					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Question; //this is used if you want to add a system icon to the message form.
					messageBox = new MessageBoxForm(string.Format("From the selected marks, a beat interval of {0:%s\\.ff} seconds was detected ({1:0.00} bpm). This will generate {2} marks. Do you want to continue?", interval,
										 bpm, generatedMarks), "Confirmation", true, false);
					if (messageBox.ShowDialog() == DialogResult.No)
						return;

					TimeSpan currentTime = latestMark + interval;
					TimeSpan endTime = latestMark + duration;
					while (currentTime <= endTime) {
						_displayedCollection.Marks.Add(currentTime);
						currentTime += interval;
					}

					_displayedCollection.Marks.Sort();
					PopulateMarkListFromMarkCollection(_displayedCollection);
					UpdateMarkCollectionInList(_displayedCollection);
				}
				else
				{
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					messageBox = new MessageBoxForm("Error parsing time: please use the format '<minutes>:<seconds>.<milliseconds>', or leave empty", "Error parsing time", false, false);
					messageBox.ShowDialog();
				}
			}
		}

		private void buttonGenerateGrid_Click(object sender, EventArgs e)
		{
			Common.Controls.TextDialog prompt =
				new Common.Controls.TextDialog("How often (in seconds) should the marks be generated?", "Mark Period", "0:00.050");
			if (prompt.ShowDialog() == DialogResult.OK) {
				TimeSpan interval;
				bool conversionSuccess = TimeSpan.TryParseExact(prompt.Response, TimeFormats.PositiveFormats, null, out interval);
				if (conversionSuccess) {
					TimeSpan currentTime = interval;
					TimeSpan endTime = _timedSequenceEditorForm.Sequence.Length;
					while (currentTime <= endTime) {
						_displayedCollection.Marks.Add(currentTime);
						currentTime += interval;
					}

					if (_displayedCollection.Level < 8) {
						_displayedCollection.Level = 8;
					}

					_displayedCollection.Marks.Sort();
					PopulateMarkListFromMarkCollection(_displayedCollection);
					UpdateMarkCollectionInList(_displayedCollection);
				}
				else
				{
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("Error parsing time: please use the format '<minutes>:<seconds>.<milliseconds>'", "Error parsing time", false, false);
					messageBox.ShowDialog();
				}
			}
		}

		#region Playback/Tapper

		private void sequencePlay(TimeSpan StartTime)
		{
			if (radioButtonTapper.Checked) {
				_newTappedMarks.Clear();
			}
			updatePositionControls(StartTime);
			_currentPlayPosition = StartTime;
			_timedSequenceEditorForm.PlaySequenceFrom(StartTime);
			timerPlayback.Start();
		}

		private void sequenceStop()
		{
			timerPlayback.Stop();
			_executionControl.Stop();
			_playbackStarted = false;
			delayStart.Enabled = true;
			if (radioButtonTapper.Checked && _newTappedMarks.Count > 0)
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Question; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Accept the new marks?", "", true, false);
				messageBox.ShowDialog();
				if (messageBox.DialogResult == DialogResult.OK)
				{
					foreach (TimeSpan time in _newTappedMarks)
					{
						if (!_displayedCollection.Marks.Contains(time))
							_displayedCollection.Marks.Add(time);
					}
					PopulateMarkListFromMarkCollection(_displayedCollection);
					UpdateMarkCollectionInList(_displayedCollection);
					_newTappedMarks.Clear();
				}
				else
				{
					if (listViewMarks.Items.Count == 0)
					{
						//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
						MessageBoxForm.msgIcon = SystemIcons.Question; //this is used if you want to add a system icon to the message form.
						messageBox = new MessageBoxForm("The Mark Collection is empty, would you like to remove it?", "", true, false);
						messageBox.ShowDialog();
						if (messageBox.DialogResult == DialogResult.OK)
						{
							MarkCollections.RemoveAt(listViewMarkCollections.SelectedIndices[0]);
							listViewMarkCollections.Items.Remove(listViewMarkCollections.SelectedItems[0]);
						}
					}
					else
					{
						_newTappedMarks.Clear();
					}
				}
			}
		}

		private void updateControlsforPlaying()
		{
			checkBoxEnabled.AutoCheck = false;
			radioButtonTapper.AutoCheck = false;
			radioButtonPlayback.AutoCheck = false;
			radioButtonTapper.ForeColor = radioButtonTapper.AutoCheck ? ThemeColorTable.ForeColor : ThemeColorTable.ForeColorDisabled;
			radioButtonPlayback.ForeColor = radioButtonTapper.AutoCheck ? ThemeColorTable.ForeColor : ThemeColorTable.ForeColorDisabled;
			textBoxCollectionName.Enabled = false;
			numericUpDownWeight.Enabled = false;
			panelColor.Enabled = false;
			groupBoxOperations.Enabled = false;
			buttonSelectAllMarks.Enabled = false;
			buttonAddOrUpdateMark.Enabled = false;
			textBoxTime.Enabled = false;
			buttonPlay.Enabled = false;
			buttonStop.Enabled = true;
			buttonRestartPlay.Enabled = false;
			textBoxCurrentMark.Text = "";
			panelMarkCollectionsButtons.Enabled = false;
			try {
				if (_audio != null)
					_audio.FrequencyDetected -= _audio_FrequencyDetected;

				var media = _timedSequenceEditorForm.Sequence.GetAllMedia().First();
				// IMediaModuleInstance media = _sequence.GetAllMedia().First();
				_audio = media as Audio;

				_audio.FrequencyDetected += _audio_FrequencyDetected;
			}
			catch (Exception) {
			}
		}

		private void updateControlsForStopped()
		{
			checkBoxEnabled.AutoCheck = true;
			radioButtonTapper.AutoCheck = true;
			radioButtonPlayback.AutoCheck = true;
			radioButtonTapper.ForeColor = radioButtonTapper.AutoCheck ? ThemeColorTable.ForeColor : ThemeColorTable.ForeColorDisabled;
			radioButtonPlayback.ForeColor = radioButtonTapper.AutoCheck ? ThemeColorTable.ForeColor : ThemeColorTable.ForeColorDisabled;
			textBoxCollectionName.Enabled = true;
			numericUpDownWeight.Enabled = true;
			panelColor.Enabled = true;
			groupBoxOperations.Enabled = true;
			buttonSelectAllMarks.Enabled = true;
			buttonAddOrUpdateMark.Enabled = true;
			textBoxTime.Enabled = true;
			buttonPlay.Enabled = true;
			buttonStop.Enabled = false;
			buttonRestartPlay.Enabled = true;
			panelMarkCollectionsButtons.Enabled = true;
		}

		private async void buttonPlay_Click(object sender, EventArgs e)
		{
			detectedActions.Clear();
			delayCounter = Convert.ToInt16(delayStart.Value);
			delayStartValue = delayStart.Value;
			delayStart.Enabled = false;
			timerDelayStart.Start();
			await Task.Delay(Convert.ToInt16(delayStart.Value) * 1000);
			sequencePlay(TimeSpan.FromMilliseconds(trackBarPlayBack.Value));
			_sequencePlaySelected = true;
			updateControlsforPlaying();
		}

		private void buttonStop_Click(object sender, EventArgs e)
		{
			sequenceStop();
			_playbackStarted = false;
			_sequencePlaySelected = false;
			updateControlsForStopped();
		}

		private void timerMarkHit_Tick(object sender, EventArgs e)
		{
			panelMarkView.BackColor = ThemeColorTable.BackgroundColor;
			timerMarkHit.Stop();
		}

		private void timerPlayback_Tick(object sender, EventArgs e)
		{
			//Handle delay until playback actually starts
			if (!_playbackStarted) {
				_playbackStarted = _timedSequenceEditorForm.PositionHasValue;
			}
			else {
				//detect if sequence reached the end
				if ((!_timedSequenceEditorForm.PositionHasValue) || 
					(_timingSource.Position.TotalMilliseconds > _timedSequenceEditorForm.Sequence.Length.TotalMilliseconds)) 
				{
					sequenceStop();
					updatePositionControls(TimeSpan.Zero); //reset to beginning
					updateControlsForStopped();
				}
				else 
				{
					textBoxPosition.Text = _timingSource.Position.ToString();
					updatePositionControls(_timingSource.Position);

					//handle playback mode
					if (_displayedCollection != null && radioButtonPlayback.Checked) 
					{
						handlePlaybackModeTick();
					}
				}
			}
		}

		private void handlePlaybackModeTick()
		{
			if (_timingSource.Position.CompareTo(_currentPlayPosition) > 0) {
				int newMarkIndex = _displayedCollection.Marks.FindIndex(x => x <= _timingSource.Position && x > _currentPlayPosition);
				_currentPlayPosition = _timingSource.Position;

				if (newMarkIndex != -1) {
					panelMarkView.BackColor = _displayedCollection.MarkColor;
					_lastMarkHit = _displayedCollection.Marks[newMarkIndex].Duration();
					textBoxCurrentMark.Text = _lastMarkHit.ToString(@"m\:ss\.fff");
					timerMarkHit.Start();
				}
			}
		}

		private void MarkManager_FormClosing(object sender, FormClosingEventArgs e)
		{
			_executionControl.Stop();
			timerMarkHit.Dispose();
			timerPlayback.Dispose();
		}

		private void buttonIncreasePlaySpeed_Click(object sender, EventArgs e)
		{
			_timingSource.Speed += _timingChangeDelta;
			updateTimingSpeedTextbox();
		}

		private void buttonDecreasePlaySpeed_Click(object sender, EventArgs e)
		{
			_timingSource.Speed -= _timingChangeDelta;
			updateTimingSpeedTextbox();
		}

		private void updateTimingSpeedTextbox()
		{
			textBoxTimingSpeed.Text = string.Format("{0}%", Math.Round((_timingSource.Speed*100), 0));
			buttonDecreasePlaySpeed.Enabled = _timingSource.Speed > _timingChangeDelta;
		}

		private void trackBarPlayBack_Scroll(object sender, EventArgs e)
		{
			//_executionControl.Stop();
			textBoxPosition.Text = TimeSpan.FromMilliseconds(trackBarPlayBack.Value).ToString(@"m\:ss\.fff");
		}

		private void trackBarPlayBack_MouseUp(object sender, MouseEventArgs e)
		{
			//if sequence was previously playing resume
			if (_sequencePlaySelected) {
				sequencePlay(TimeSpan.FromMilliseconds(trackBarPlayBack.Value));
			}
		}

		private void updatePositionControls(TimeSpan Position)
		{
			trackBarPlayBack.Value = (int) Position.TotalMilliseconds;
			textBoxPosition.Text = Position.ToString(@"m\:ss\.fff");
		}

		private void _triggerResult()
		{
			if (_playbackStarted) {
				if (listViewMarkCollections.SelectedItems.Count == 0)
					AddNewCollection(ThemeColorTable.ForeColor);
				// round the tapped time to the nearest millisecond
				_newTappedMarks.Add(TimeSpan.FromMilliseconds(Math.Round(_timingSource.Position.TotalMilliseconds)));
				panelMarkView.BackColor = _displayedCollection.MarkColor;
				timerMarkHit.Start();
			}
		}

		private void panelMarkView_MouseDown(object sender, MouseEventArgs e)
		{
			if (radioButtonTapper.Checked)
				_triggerResult();
		}

		private void MarkManager_KeyDown(object sender, KeyEventArgs e)
		{
			if (radioButtonTapper.Checked) {
				if (e.KeyCode == Keys.Space) {
					_triggerResult();
					e.Handled = true;
				}
			}
		}

		private void radioButtonTapper_CheckedChanged(object sender, EventArgs e)
		{
			labelTapperInstructions.Visible = radioButtonTapper.Checked;
		}

		#endregion

		#region Increase/Decrease selected Marks

		private void buttonIncreaseSelectedMarks_Click(object sender, EventArgs e)
		{
			increaseSelectedMarks();
		}

		private void buttonDecreaseSelectedMarks_Click(object sender, EventArgs e)
		{
			decreaseSelectedMarks();
		}

		private void increaseSelectedMarks()
		{
			int incrementValue = Convert.ToInt32(textBoxTimeIncrement.Text);
			TimeSpan incrementSpan = TimeSpan.FromMilliseconds(incrementValue);
			List<TimeSpan> selectedMarks = new List<TimeSpan>();
			foreach (int itemIndex in listViewMarks.SelectedIndices) {
				TimeSpan newTimeSpan = _displayedCollection.Marks[itemIndex].Add(incrementSpan);
				if (newTimeSpan > _timedSequenceEditorForm.Sequence.Length) {
					newTimeSpan = _timedSequenceEditorForm.Sequence.Length;
				}
				updateMark(itemIndex, newTimeSpan);
				selectedMarks.Add(newTimeSpan); //save new time so we can reselect the list items at the end.
			}
			selectMarks(selectedMarks);
		}

		private void decreaseSelectedMarks()
		{
			int incrementValue = Convert.ToInt32(textBoxTimeIncrement.Text);
			TimeSpan incrementSpan = TimeSpan.FromMilliseconds(incrementValue);
			List<TimeSpan> selectedMarks = new List<TimeSpan>();
			foreach (int itemIndex in listViewMarks.SelectedIndices) {
				TimeSpan newTimeSpan = _displayedCollection.Marks[itemIndex].Subtract(incrementSpan);
				if (newTimeSpan.CompareTo(TimeSpan.Zero) < 0) //Make sure theres never a negative timespan
				{
					newTimeSpan = TimeSpan.Zero;
				}
				updateMark(itemIndex, newTimeSpan);
				selectedMarks.Add(newTimeSpan);
			}
			selectMarks(selectedMarks);
		}

		private void selectMarks(List<TimeSpan> marks)
		{
			foreach (TimeSpan tSpan in marks) {
				int newIndex = _displayedCollection.IndexOf(tSpan);
				if (newIndex != -1) {
					listViewMarks.Items[newIndex].Selected = true;
				}
			}
			listViewMarks.Focus();
		}

		private void updateMark(int index, TimeSpan ts)
		{
			//test if the new mark already exists, if not remove it and add the new one
			if (_displayedCollection.Marks.Contains(ts)) {
				System.Media.SystemSounds.Asterisk.Play();
			}
			else {
				_displayedCollection.Marks.RemoveAt(index);
				_displayedCollection.Marks.Add(ts);
				_displayedCollection.Marks.Sort();
				PopulateMarkListFromMarkCollection(_displayedCollection);
				UpdateMarkCollectionInList(_displayedCollection);
			}
		}

		#endregion

		private void chkHighPass_CheckedChanged(object sender, EventArgs e)
		{
			var media = _timedSequenceEditorForm.Sequence.GetAllMedia().First();
			// IMediaModuleInstance media = _sequence.GetAllMedia().First();
			_audio = media as Audio;
			_audio.FrequencyDetected += _audio_FrequencyDetected;

			_audio.HighPassFilterEnabled = this.chkHighPass.Checked;
			_audio.HighPassFilterValue = (float) this.numHighPass.Value;
		}

		private void chkLowPass_CheckedChanged(object sender, EventArgs e)
		{
			var media = _timedSequenceEditorForm.Sequence.GetAllMedia().First();
			// IMediaModuleInstance media = _sequence.GetAllMedia().First();
			_audio = media as Audio;
			_audio.FrequencyDetected += _audio_FrequencyDetected;

			_audio.LowPassFilterEnabled = this.chkLowPass.Checked;
			_audio.LowPassFilterValue = (float) this.numLowPass.Value;
		}

		private void numLowPass_ValueChanged(object sender, EventArgs e)
		{
			var media = _timedSequenceEditorForm.Sequence.GetAllMedia().First();
			// IMediaModuleInstance media = _sequence.GetAllMedia().First();
			_audio = media as Audio;
			_audio.FrequencyDetected += _audio_FrequencyDetected;

			_audio.LowPassFilterValue = (float) this.numLowPass.Value;
		}

		private void numHighPass_ValueChanged(object sender, EventArgs e)
		{
			var media = _timedSequenceEditorForm.Sequence.GetAllMedia().First();
			// IMediaModuleInstance media = _sequence.GetAllMedia().First();
			_audio = media as Audio;
			_audio.FrequencyDetected += _audio_FrequencyDetected;

			_audio.HighPassFilterValue = (float) this.numHighPass.Value;
		}

		private void ChkAutoTapper_CheckedChanged(object sender, EventArgs e)
		{
			var media = _timedSequenceEditorForm.Sequence.GetAllMedia().First();
			// IMediaModuleInstance media = _sequence.GetAllMedia().First();
			_audio = media as Audio;
			_audio.FrequencyDetected += _audio_FrequencyDetected;

			_audio.DetectFrequeniesEnabled = ChkAutoTapper.Checked;
		}

		private void _audio_FrequencyDetectedSetText(object args)
		{
			var e = args as FrequencyEventArgs;
			if (args != null) {
				this.label8.Text = string.Format("Note {0} at {1}", e.Note, e.Time);
			}
		}

		private void _audio_FrequencyDetected(object sender, FrequencyEventArgs e)
		{
			if (!detectedActions.ContainsKey(e.Index))
				detectedActions.TryAdd(e.Index, new ConcurrentBag<TimeSpan>() {e.Time});
			else {
				ConcurrentBag<TimeSpan> spans = new ConcurrentBag<TimeSpan>();

				detectedActions.TryGetValue(e.Index, out spans);
				ConcurrentBag<TimeSpan> spans2 = spans;

				spans.Add(e.Time);
				detectedActions.TryUpdate(e.Index, spans, spans2);
			}

			if (this.InvokeRequired)
				this.Invoke(new Vixen.Delegates.GenericValue(_audio_FrequencyDetectedSetText), e);
			else
				_audio_FrequencyDetectedSetText(e);
		}

		private ConcurrentDictionary<int, ConcurrentBag<TimeSpan>> detectedActions =
			new ConcurrentDictionary<int, ConcurrentBag<TimeSpan>>();

		private List<int> detectionIndexes = new List<int>();
		private AutomaticMusicDetection audioDetectionSettings = null;
		private Properties.Settings settings = new Properties.Settings();

		private void btnAutoDetectionSettings_Click(object sender, EventArgs e)
		{
			if (_audio == null) {
				var media = _timedSequenceEditorForm.Sequence.GetAllMedia().First();
				// IMediaModuleInstance media = _sequence.GetAllMedia().First();
				_audio = media as Audio;
			}

			if (audioDetectionSettings == null)
				audioDetectionSettings = new AutomaticMusicDetection(_audio);
			audioDetectionSettings.Indexes = detectionIndexes;

			var result = audioDetectionSettings.ShowDialog();
			if (result == DialogResult.OK) {
				detectionIndexes = audioDetectionSettings.Indexes;
			}
		}

		private void btnCreateCollections_Click(object sender, EventArgs e)
		{
			List<int> indexes = new List<int>();
			if (radioAll.Checked) {
				for (int i = 0; i < 120; i++) {
					indexes.Add(i);
				}
			}
			else {
				indexes = detectionIndexes;
			}
			// indexes.AsParallel().ForAll(i => GenerateCollection(i));
			indexes.ForEach(i => GenerateCollection(i));
			PopulateMarkCollectionsList();
		}

		private void GenerateCollection(int index)
		{
			ConcurrentBag<TimeSpan> values = new ConcurrentBag<TimeSpan>();
			if (detectedActions.TryGetValue(index, out values)) {
				MarkCollection collection = new MarkCollection();
				collection.MarkColor = Color.Blue;
				collection.Name = string.Format("Note {0} Freq. {1}", _audio.DetectionNotes[index], _audio.DetectionNoteFreq[index]);

				TimeSpan lastValue = TimeSpan.FromMilliseconds(0);

				values.OrderBy(a => a).ToList().ForEach(v =>
															{
																var xtime = v - TimeSpan.FromMilliseconds(settings.MusicAccuracy);
																if (xtime > lastValue) {
																	collection.Marks.Add(v);
																	lastValue = v;
																}
															});

				lock (MarkCollections) {
					MarkCollections.RemoveAll(r => r.Name == collection.Name);
					MarkCollections.Add(collection);
				}
			}
		}

		static string lastFolder = "";
		private void buttonImportAudacity_Click(object sender, EventArgs e)
		{
			var aDialog = new AudacityImportDialog();

			if (aDialog.ShowDialog() == DialogResult.OK)
			{
				if (aDialog.IsVixen3BeatSelection)
					ImportVixen3Beats();
				if (aDialog.IsVampBarSelection || aDialog.IsAudacityBeatSelection)
					LoadBarLabels();
				if (aDialog.IsVampBeatSelection)
					LoadBeatLabels();		
			}
		}

		private void LoadBeatLabels()
		{
			openFileDialog.DefaultExt = ".txt";
			openFileDialog.Filter = @"Audacity Beat Labels|*.txt|All Files|*.*";
			openFileDialog.FilterIndex = 0;
			openFileDialog.InitialDirectory = lastFolder;
			openFileDialog.FileName = "";
			var colors = new List<Color>
			{
				Color.Yellow,Color.Gold, Color.Goldenrod, Color.SaddleBrown,Color.CadetBlue,Color.BlueViolet 
			};
			
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				lastFolder = Path.GetDirectoryName(openFileDialog.FileName);
				try
				{
					String file;
					using (var sr = new StreamReader(openFileDialog.FileName))
					{
						file = sr.ReadToEnd();
					}
					if (file.Any())
					{
						const string pattern = @"(\d*\.\d*)\s(\d*\.\d*)\s(\d)";
						MatchCollection matches = Regex.Matches(file, pattern);
						int numBeats =  Convert.ToInt32( matches.Cast<Match>().Max(x => x.Groups[3].Value) );
						var marks = new List<MarkCollection>(numBeats);
						for (int i = 0; i < numBeats; i++)
						{
							marks.Add(AddNewCollection(colors[i], string.Format("Audacity Beat {0} Marks", i + 1)));	
						}
						
						foreach (Match match in matches)
						{
							TimeSpan time = TimeSpan.FromSeconds(Convert.ToDouble(match.Groups[1].Value));
							int beatNumber = Convert.ToInt32(match.Groups[3].Value);
							marks[beatNumber-1].Marks.Add(time);
						}
						foreach (MarkCollection mark in marks)
						{
							UpdateMarkListBox(mark);	
						}
						
					}
				} catch (Exception ex)
				{
					string msg = "There was an error importing the Audacity beat marks: " + ex.Message;
					Logging.Error(msg);
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm(msg, "Audacity Import Error", false, false);
					messageBox.ShowDialog();
				}
			}
		}

		private void LoadBarLabels()
		{
			openFileDialog.DefaultExt = ".txt";
			openFileDialog.Filter = @"Audacity Bar Labels|*.txt|All Files|*.*";
			openFileDialog.FilterIndex = 0;
			openFileDialog.InitialDirectory = lastFolder;
			if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				lastFolder = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
				try
				{
					String everything;
					using (StreamReader sr = new StreamReader(openFileDialog.FileName))
					{
						everything = sr.ReadToEnd();
					}
					// Remove the \r so we're just left with a \n (allows importing of Sean's Audacity beat marks
					everything = everything.Replace("\r", "");
					string[] lines = everything.Split(new string[] {"\n"}, StringSplitOptions.RemoveEmptyEntries);
					if (lines.Any())
					{
						AddNewCollection(Color.Yellow, "Audacity Marks");
						foreach (string line in lines)
						{
							string mark;
							if (line.IndexOf("\t") > 0)
							{
								mark = line.Split('\t')[0].Trim();
							}
							else
							{
								mark = line.Trim().Split(' ')[0].Trim();
							}
						
							TimeSpan time = TimeSpan.FromSeconds(Convert.ToDouble(mark));
							_displayedCollection.Marks.Add(time);
						}
						UpdateMarkListBox();
					}
				}
				catch (Exception ex)
				{
					string msg = "There was an error importing the Audacity bar marks: " + ex.Message;
					Logging.Error(msg);
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm(msg, "Audacity Import Error", false, false);
					messageBox.ShowDialog();
				}
			}
		}

		private MarkCollection AddNewCollection(Color color, string name = "New Collection")
		{
			MarkCollection newCollection = new MarkCollection();
			newCollection.Name = name;
			newCollection.MarkColor = color;
			MarkCollections.Add(newCollection);

			// populate the form with the new collection first, *then* update the collection list,
			// so that the displayed collection is remembered and selected in the list.
			PopulateFormWithMarkCollection(newCollection);
			PopulateMarkCollectionsList();

			panelColor.BackColor = color;
			return newCollection;
		}

		private void AddMark(string markText)
		{
			TimeSpan time;
			bool success = TimeSpan.TryParseExact(markText, TimeFormats.PositiveFormats, null, out time);
			if (success) {
				_displayedCollection.Marks.Add(time);
			}
			else
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Error parsing time: please use the format '<minutes>:<seconds>.<milliseconds>'", "Error parsing time", false, false);
				messageBox.ShowDialog();
			}
		}

		private void UpdateMarkListBox()
		{
			_displayedCollection.Marks.Sort();
			PopulateMarkListFromMarkCollection(_displayedCollection);
			UpdateMarkCollectionInList(_displayedCollection);
		}

		private void UpdateMarkListBox(MarkCollection marks)
		{
			marks.Marks.Sort();
			PopulateMarkListFromMarkCollection(marks);
			UpdateMarkCollectionInList(marks);
		}

		private void buttonExportBeatMarks_Click(object sender, EventArgs e)
		{
			if (MarkCollections.Count == 0)
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Unable to find marks collection for export", "Warning", false, false);
				messageBox.ShowDialog();
				return;
			}

			var bDialog = new BeatMarkExportDialog();

			if (bDialog.ShowDialog() == DialogResult.OK)
			{
				if (bDialog.IsVixen3Selection)
					ExportMarkCollections(null, "vixen3");
				if (bDialog.IsAudacitySelection)
					ExportMarkCollections(null, "audacity");
				if (!bDialog.IsVixen3Selection && !bDialog.IsAudacitySelection)
				{ 
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("No export type selected", "Warning", false, false);
					messageBox.ShowDialog();
				}
			}
		}

		//Vixen 3 Beat Mark Collection Import routine 2-7-2014 JMB
		private void ImportVixen3Beats()
		{
			openFileDialog.DefaultExt = ".v3m";
			openFileDialog.Filter = "Vixen 3 Mark Collection (*.v3m)|*.v3m|All Files (*.*)|*.*";
			openFileDialog.FilterIndex = 0;
			openFileDialog.InitialDirectory = lastFolder;
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				using (FileStream reader = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
				{
					DataContractSerializer ser = new DataContractSerializer(typeof(List<MarkCollection>));
					MarkCollections = (List<MarkCollection>)ser.ReadObject(reader);
				}
				PopulateMarkCollectionsList();
				PopulateFormWithMarkCollection(null, true);
			}
		}

		//Beat Mark Collection Export routine 2-7-2014 JMB
		//In the audacity section, if the MarkCollections.Count = 1 then we assume the collection is bars and iMarkCollection++
		//Other wise its beats, at least from the information I have studied, and we do not iMarkCollection++ to keep the collections together properly.
		private void ExportMarkCollections(MarkCollection collection, string exportType)
		{
			if (exportType == "vixen3")
			{
				saveFileDialog.DefaultExt = ".v3m";
				saveFileDialog.Filter = "Vixen 3 Mark Collection (*.v3m)|*.v3m|All Files (*.*)|*.*";
				saveFileDialog.InitialDirectory = lastFolder;
				if (saveFileDialog.ShowDialog() == DialogResult.OK)
				{
					var xmlsettings = new XmlWriterSettings()
					{
						Indent = true,
						IndentChars = "\t",
					};

					DataContractSerializer ser = new DataContractSerializer(typeof(List<MarkCollection>));
					var writer = XmlWriter.Create(saveFileDialog.FileName, xmlsettings);
					ser.WriteObject(writer, MarkCollections);
					writer.Close();
				}
			}

			if (exportType == "audacity")
			{
				int iMarkCollection = 0;
				List<string> BeatMarks = new List<string>();
				foreach (MarkCollection mc in MarkCollections)
				{
					iMarkCollection++;
					foreach (TimeSpan time in mc.Marks)
					{
						BeatMarks.Add(time.TotalSeconds.ToString("0000.000") + "\t" + time.TotalSeconds.ToString("0000.000") + "\t" + iMarkCollection);
						if (MarkCollections.Count == 1)
							iMarkCollection++;
					}
				}
				
				saveFileDialog.DefaultExt = ".txt";
				saveFileDialog.Filter = "Audacity Marks (*.txt)|*.txt|All Files (*.*)|*.*";
				if (saveFileDialog.ShowDialog() == DialogResult.OK)
				{
					string name = saveFileDialog.FileName;

					using (System.IO.StreamWriter file = new System.IO.StreamWriter(name))
					{
						foreach (string bm in BeatMarks.OrderBy(x => x))
						{
							file.WriteLine(bm);
						}
					}
				}
			}
		}

		private void panelMarkCollectionsButtons_EnabledChanged(object sender, EventArgs e)
		{
			buttonExportBeatMarks.ForeColor = buttonExportBeatMarks.Enabled ? ThemeColorTable.ForeColor : ThemeColorTable.ForeColorDisabled;
			buttonImportAudacity.ForeColor = buttonImportAudacity.Enabled ? ThemeColorTable.ForeColor : ThemeColorTable.ForeColorDisabled;
			buttonAddCollection.ForeColor = buttonAddCollection.Enabled ? ThemeColorTable.ForeColor : ThemeColorTable.ForeColorDisabled;
		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
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

		private void groupBoxOperations_EnabledChanged(object sender, EventArgs e)
		{
			checkBoxEnabled.ForeColor = groupBoxOperations.Enabled ? ThemeColorTable.ForeColor : ThemeColorTable.ForeColorDisabled;
			label1.ForeColor = groupBoxOperations.Enabled ? ThemeColorTable.ForeColor : ThemeColorTable.ForeColorDisabled;
			label2.ForeColor = groupBoxOperations.Enabled ? ThemeColorTable.ForeColor : ThemeColorTable.ForeColorDisabled;
			label3.ForeColor = groupBoxOperations.Enabled ? ThemeColorTable.ForeColor : ThemeColorTable.ForeColorDisabled;
			checkBoxBold.ForeColor = groupBoxOperations.Enabled ? ThemeColorTable.ForeColor : ThemeColorTable.ForeColorDisabled;
			checkBoxSolidLine.ForeColor = groupBoxOperations.Enabled ? ThemeColorTable.ForeColor : ThemeColorTable.ForeColorDisabled;
		}

		private void buttonRestartPlay_Click(object sender, EventArgs e)
		{
			trackBarPlayBack.Value = 0;
			textBoxPosition.Text = TimeSpan.FromMilliseconds(trackBarPlayBack.Value).ToString(@"m\:ss\.fff");
		}

		private void timerDelayStart_Tick(object sender, EventArgs e)
		{
			
			delayCounter--;
			if (delayCounter < 0)
			{
				delayStart.Value = delayStartValue;
				timerDelayStart.Stop();
			}
			else
			{
				delayStart.Value--;
			}
		}

		private void checkBoxBold_CheckedChanged(object sender, EventArgs e)
		{
			if (_displayedCollection != null && _displayedCollection.Bold != checkBoxBold.Checked)
			{
				_displayedCollection.Bold = checkBoxBold.Checked;
				UpdateMarkCollectionInList(_displayedCollection);
			}
		}

		private void checkBoxSolidLine_CheckedChanged(object sender, EventArgs e)
		{
			if (_displayedCollection != null && _displayedCollection.SolidLine != checkBoxSolidLine.Checked)
			{
				_displayedCollection.SolidLine = checkBoxSolidLine.Checked;
				UpdateMarkCollectionInList(_displayedCollection);
			}
		}

	}
}