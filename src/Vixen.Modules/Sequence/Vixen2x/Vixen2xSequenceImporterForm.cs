using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Common.Controls;
using Vixen.Sys;
using Common.Controls.Theme;
using Common.Resources.Properties;


namespace VixenModules.SequenceType.Vixen2x
{
	public partial class Vixen2xSequenceImporterForm : BaseForm
	{
		public ISequence Sequence { get; set; }

		private bool mapExists;
		private string vixen2ImportFile;
		private List<ChannelMapping> channelMappings;

		private Vixen2SequenceData parsedV2Sequence;
		private Vixen3SequenceCreator vixen3SequenceCreator;
		private Vixen2xSequenceStaticData StaticModuleData;

		public Vixen2xSequenceImporterForm(string Vixen2File, Vixen.Module.IModuleDataModel staticModuleData)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;

			channelMappings = new List<ChannelMapping>();

			//I think this was the correct way to implement this.
			StaticModuleData = (Vixen2xSequenceStaticData) staticModuleData;

			vixen2ImportFile = Vixen2File;

			//Add known information:
			vixen2SequenceTextBox.Text = vixen2ImportFile;


			//Go ahead and build the map for the sequence that we have.
			ParseV2SequenceData();

			//we parsed the sequence so go ahead and for now set our ChannelMappings to the parsed data
			//If the user selects one from the listbox we will make an adjustment.
			channelMappings = parsedV2Sequence.mappings;

			if (StaticModuleData.Vixen2xMappings.Count > 0) {
				LoadMaps();
			}
			else {
				mapExists = false;
				//use the profilename for now
				vixen2ToVixen3MappingTextBox.Text = parsedV2Sequence.ProfileName;
			}
		}

		private void LoadMaps()
		{
			mapExists = true;
			//disable the convertButton
			convertButton.Enabled = false;

			PopulateListBox();
		}

		private void PopulateListBox()
		{
			vixen2ToVixen3MappingListBox.Items.Clear();

			vixen2ToVixen3MappingTextBox.Text = string.Empty;

			//iterate over the dictionary to poplulate the listbox with the mappings
			foreach (KeyValuePair<string, List<ChannelMapping>> kvp in StaticModuleData.Vixen2xMappings) {
				vixen2ToVixen3MappingListBox.Items.Add(kvp.Key);
			}
		}

		private void ParseV2SequenceData()
		{
			parsedV2Sequence = new Vixen2SequenceData(vixen2ImportFile);

			if (!String.IsNullOrEmpty(parsedV2Sequence.ProfilePath)) {
				vixen2ProfileTextBox.Text = string.Format(@"{0}\{1}.pro", parsedV2Sequence.ProfilePath, parsedV2Sequence.ProfileName);
				vixen2ToVixen3MappingListBox.Text = parsedV2Sequence.ProfileName;
			}
			else {
				vixen2ProfileTextBox.Text = parsedV2Sequence.ProfileName;
			}
		}

		private void AddDictionaryEntry(string key, List<ChannelMapping> value)
		{
			if (StaticModuleData.Vixen2xMappings.ContainsKey(key)) {
				StaticModuleData.Vixen2xMappings[key] = value;
			}
			else {
				StaticModuleData.Vixen2xMappings.Add(key, value);
			}
		}

		private void createMapButton_Click(object sender, EventArgs e)
		{
			using (
				Vixen2xSequenceImporterChannelMapper mappingForm =
					new Vixen2xSequenceImporterChannelMapper(channelMappings, mapExists, vixen2ToVixen3MappingTextBox.Text)) {
				if (mappingForm.ShowDialog() == DialogResult.OK) {
					//add to or update the dictionary
					AddDictionaryEntry(mappingForm.MappingName, mappingForm.Mappings);

					//Clear out the text box and make the user re-select the mapping
					vixen2ToVixen3MappingTextBox.Text = string.Empty;
				}

				//User either created a new map or canceled out of the form so lets reload our
				//maps which will disable the convert button and clean out the Vixen 2 to Vixen 3 Map text box
				LoadMaps();
			}
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			Sequence = null;
		}

		private void convertButton_Click(object sender, EventArgs e)
		{
			//check to see if the mapping table is there.
			if (StaticModuleData.Vixen2xMappings.Count > 0) {
				vixen3SequenceCreator = new Vixen3SequenceCreator(parsedV2Sequence, StaticModuleData.Vixen2xMappings[vixen2ToVixen3MappingTextBox.Text]);

				Sequence = vixen3SequenceCreator.Sequence;

				if (Sequence.SequenceData != null) {
					//we got this baby converted so close it out and load up the Sequence
					DialogResult = System.Windows.Forms.DialogResult.OK;
					Close();
				}
			}
			else {
				MessageBox.Show("Mapping data is missing, please try again.", "No Mapping Data", MessageBoxButtons.OK,
				                MessageBoxIcon.Warning);
			}
		}

		private void vixen2ToVixen3MappingListBox_MouseClick(object sender, MouseEventArgs e)
		{
			if (vixen2ToVixen3MappingListBox.SelectedIndex < 0 ||
			    !vixen2ToVixen3MappingListBox.GetItemRectangle(vixen2ToVixen3MappingListBox.SelectedIndex).Contains(e.Location)) {
				vixen2ToVixen3MappingListBox.SelectedIndex = -1;
				vixen2ToVixen3MappingTextBox.Text = string.Empty;

				//do not use the static mapping use the parsed sequence mapping
				//cuase the user must want to start over.
				channelMappings = parsedV2Sequence.mappings;

				//disable the convert button cause we do not have a map selected
				convertButton.Enabled = false;
				createMapButton.Text = "Create New Map";
			}
			else {
				vixen2ToVixen3MappingTextBox.Text = vixen2ToVixen3MappingListBox.SelectedItem.ToString();

				//user selected a pre-existing mapping so use it now.
				channelMappings = StaticModuleData.Vixen2xMappings[vixen2ToVixen3MappingListBox.SelectedItem.ToString()];

				//user selected a map so enable the convert button
				convertButton.Enabled = true;
				createMapButton.Text = "Edit Selected Map";
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
	}
}