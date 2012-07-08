using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VixenModules.Sequence.Timed;
using Vixen.Execution;
using Vixen.Module.Timing;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class MarkManager : Form
	{
		private MarkCollection _displayedCollection = null;
		private bool _updatingListContents = false;
		private IExecutionControl _executionControl;
		private ITiming _timingSource;

		public MarkManager(List<MarkCollection> markCollections, IExecutionControl executionControl, ITiming timingSource)
		{
			InitializeComponent();
			MarkCollections = markCollections;
			_executionControl = executionControl;
			_timingSource = timingSource;
		}

		public List<MarkCollection> MarkCollections { get; set; }

		private void MarkManager_Load(object sender, EventArgs e)
		{
			PopulateMarkCollectionsList();
			PopulateFormWithMarkCollection(null, true);
		}

		private double GetGrayValueForColor(Color c)
		{
			return c.R * 0.299 + c.G * 0.587 + c.B * 0.114;
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
				item.BackColor = (mc.Enabled) ? mc.MarkColor : SystemColors.Window;
				item.ForeColor = (mc.Enabled) ? ((GetGrayValueForColor(mc.MarkColor) > 128) ? Color.Black : Color.White)  : SystemColors.InactiveCaptionText;
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
					item.BackColor = (collection.Enabled) ? collection.MarkColor : SystemColors.Window;
					item.ForeColor = (collection.Enabled) ? ((GetGrayValueForColor(collection.MarkColor) > 128) ? Color.Black : Color.White) : SystemColors.InactiveCaptionText;
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
				panelColor.BackColor = SystemColors.Control;
				checkBoxEnabled.Checked = false;
			} else {
				textBoxCollectionName.Text = collection.Name;
				numericUpDownWeight.Value = collection.Level;
				panelColor.BackColor = collection.MarkColor;
				checkBoxEnabled.Checked = collection.Enabled;
			}

			PopulateMarkListFromMarkCollection(collection);

			groupBoxSelectedMarkCollection.Enabled = (collection != null);
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
			} else {
				PopulateFormWithMarkCollection(null);
			}

			buttonRemoveCollection.Enabled = (listViewMarkCollections.SelectedItems.Count > 0);
		}

		private void listViewMarks_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listViewMarks.SelectedItems.Count == 1) {
				textBoxTime.Text = listViewMarks.SelectedItems[0].Text;
				buttonAddOrUpdateMark.Text = "Update";
			} else {
				textBoxTime.Text = "";
				buttonAddOrUpdateMark.Text = "Add";
			}
		}

		private void buttonAddCollection_Click(object sender, EventArgs e)
		{
			MarkCollection newCollection = new MarkCollection();
			newCollection.Name = "New Collection";
			MarkCollections.Add(newCollection);

			// populate the form with the new collection first, *then* update the collection list,
			// so that the displayed collection is remembered and selected in the list.
			PopulateFormWithMarkCollection(newCollection);
			PopulateMarkCollectionsList();
		}

		private void buttonRemoveCollection_Click(object sender, EventArgs e)
		{
			if (listViewMarkCollections.SelectedItems.Count > 0) {
				foreach (ListViewItem item in listViewMarkCollections.SelectedItems) {
					MarkCollection mc = item.Tag as MarkCollection;
					MarkCollections.Remove(mc);
				}
			}

			if (!MarkCollections.Contains(_displayedCollection)) {
				PopulateFormWithMarkCollection(null);
			}

			PopulateMarkCollectionsList();
		}

		private void buttonAddOrUpdateMark_Click(object sender, EventArgs e)
		{
			TimeSpan time;
			bool success = TimeSpan.TryParseExact(textBoxTime.Text, TimeFormats.Formats, null, out time);
			if (success) {
				if (buttonAddOrUpdateMark.Text == "Update") {
					// updating an existing item, find it, remove it, and add the new one
					if (listViewMarks.SelectedItems.Count != 1)
						Vixen.Sys.VixenSystem.Logging.Error("MarkManager: updating a mark, but there are " + listViewMarks.SelectedItems.Count + " items selected!");
					if (listViewMarks.SelectedItems.Count > 0)
					_displayedCollection.Marks.Remove((TimeSpan)listViewMarks.SelectedItems[0].Tag);
				}

				if (_displayedCollection.Marks.Contains(time)) {
					System.Media.SystemSounds.Asterisk.Play();
				} else {
					_displayedCollection.Marks.Add(time);
					_displayedCollection.Marks.Sort();
					PopulateMarkListFromMarkCollection(_displayedCollection);
					UpdateMarkCollectionInList(_displayedCollection);
				}
			} else {
				MessageBox.Show("Error parsing time: please use the format '<minutes>:<seconds>.<milliseconds>'", "Error parsing time");
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
				if (tapper.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
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
			CommonElements.TextDialog prompt = new CommonElements.TextDialog("Time to offset (in seconds):");
			if (prompt.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				TimeSpan time;

				if (TimeSpan.TryParseExact(prompt.Response, TimeFormats.Formats, null, out time)) {
					List<TimeSpan> newMarks = new List<TimeSpan>();
					foreach (ListViewItem item in listViewMarks.Items) {
						if (item.Selected) {
							newMarks.Add(((TimeSpan)item.Tag) + time);
						} else {
							newMarks.Add((TimeSpan)item.Tag);
						}
						newMarks.Sort();
						_displayedCollection.Marks = newMarks;
						PopulateMarkListFromMarkCollection(_displayedCollection);
					}
				} else {
					MessageBox.Show("Error parsing time: please use the format '<minutes>:<seconds>.<milliseconds>'", "Error parsing time");
				}
			}
		}

		private void buttonEvenlySpaceMarks_Click(object sender, EventArgs e)
		{
			if (listViewMarks.SelectedItems.Count < 3) {
				MessageBox.Show("Select at least three marks to space evenly.", "Need more marks");
				return;
			}

			List<TimeSpan> spacingTimes = new List<TimeSpan>();
			List<TimeSpan> newTimes = new List<TimeSpan>();

			foreach (ListViewItem item in listViewMarks.Items) {
				if (item.Selected)
					spacingTimes.Add((TimeSpan)item.Tag);
				else
					newTimes.Add((TimeSpan)item.Tag);
			}

			spacingTimes.Sort();

			TimeSpan totalInterval = spacingTimes.Last() - spacingTimes.First();
			TimeSpan integralInterval = TimeSpan.FromTicks(totalInterval.Ticks / (spacingTimes.Count - 1));
			for (int i = 0; i < spacingTimes.Count; i++) {
				TimeSpan newTime = TimeSpan.FromTicks(spacingTimes.First().Ticks + (i * integralInterval.Ticks));
				if (!newTimes.Contains(newTime))
					newTimes.Add(newTime);
			}

			newTimes.Sort();
			_displayedCollection.Marks = newTimes;
			PopulateMarkListFromMarkCollection(_displayedCollection);
		}

		private void buttonGenerateSubmarks_Click(object sender, EventArgs e)
		{
			if (listViewMarks.SelectedItems.Count < 2) {
				MessageBox.Show("Select at least two marks to generate times between.", "Need more marks");
				return;
			}

			CommonElements.TextDialog prompt = new CommonElements.TextDialog("Break each interval into how many equal segments?");
			if (prompt.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				int divisions;
				if (int.TryParse(prompt.Response, out divisions)) {

					DialogResult newCollectionResult = MessageBox.Show("Do you want to put the new marks into a different collection?", "Add to new collection?", MessageBoxButtons.YesNoCancel);
					if (newCollectionResult == System.Windows.Forms.DialogResult.Cancel) {
						return;
					}

					List<TimeSpan> sourceTimes = new List<TimeSpan>();
					foreach (ListViewItem item in listViewMarks.SelectedItems) {
						sourceTimes.Add((TimeSpan)item.Tag);
					}
					sourceTimes.Sort();

					List<TimeSpan> newTimes = new List<TimeSpan>();
					for (int i = 1; i < sourceTimes.Count; i++) {
						TimeSpan interval = TimeSpan.FromTicks((sourceTimes[i] - sourceTimes[i - 1]).Ticks / divisions);
						for (int j = 0; j < divisions; j++) {
							newTimes.Add(sourceTimes[i - 1] + TimeSpan.FromTicks(interval.Ticks * j));
						}
					}
					newTimes.Add(sourceTimes.Last());

					MarkCollection destination = _displayedCollection;

					if (newCollectionResult == System.Windows.Forms.DialogResult.Yes) {
						List<KeyValuePair<string, object>> options = new List<KeyValuePair<string, object>>();
						foreach (MarkCollection mc in MarkCollections) {
							options.Add(new KeyValuePair<string, object>(mc.Name, mc));
						}
						CommonElements.ListSelectDialog selector = new CommonElements.ListSelectDialog("Destination Mark Collection?", options);
						if (selector.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
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
				} else {
					MessageBox.Show("Error parsing number: please enter a whole number for the number of divisions.", "Error parsing number");
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
				_displayedCollection.Level = (int)numericUpDownWeight.Value;
				UpdateMarkCollectionInList(_displayedCollection);
			}
		}

		private void panelColor_Click(object sender, EventArgs e)
		{
			CommonElements.ColorManagement.ColorPicker.ColorPicker picker = new CommonElements.ColorManagement.ColorPicker.ColorPicker();

			DialogResult result = picker.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK) {
				_displayedCollection.MarkColor = picker.Color.ToRGB().ToArgb();
				panelColor.BackColor = _displayedCollection.MarkColor;
				UpdateMarkCollectionInList(_displayedCollection);
			}
		}

		private void checkBoxEnabled_CheckedChanged(object sender, EventArgs e)
		{
			if (_displayedCollection != null && _displayedCollection.Enabled != checkBoxEnabled.Checked) {
				_displayedCollection.Enabled = checkBoxEnabled.Checked;
				UpdateMarkCollectionInList(_displayedCollection);
			}
		}

		private void listViewMarks_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode) {
				case Keys.Delete:
				foreach (ListViewItem item in listViewMarks.SelectedItems) {
					_displayedCollection.Marks.Remove((TimeSpan)item.Tag);
				}
				PopulateMarkListFromMarkCollection(_displayedCollection);
				UpdateMarkCollectionInList(_displayedCollection);
				break;
			}
		}
	}
}
