using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Vixen.Sys;


namespace VixenModules.SequenceType.Vixen2x {
	public partial class Vixen2xSequenceImporterForm : Form {
		
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

			channelMappings = new List<ChannelMapping>();

            //I think this was the correct way to implement this.
            StaticModuleData = (Vixen2xSequenceStaticData)staticModuleData;
            
            vixen2ImportFile = Vixen2File;

            //Add known information:
            vixen2SequenceTextBox.Text = vixen2ImportFile;
			

            //Go ahead and build the map for the sequence that we have.
            ParseV2SequenceData();

			//we parsed the sequence so go ahead and for now set our ChannelMappings to the parsed data
			//If the user selects one from the listbox we will make an adjustment.
			channelMappings = parsedV2Sequence.mappings;

			if (StaticModuleData.Vixen2xMappings.Count > 0)
			{
				LoadMap();
			}
			else
			{
				mapExists = false;
				//use the profilename for now
				vixen2ToVixen3MappingTextBox.Text = parsedV2Sequence.ProfileName;
			}

		}

		private void LoadMap()
		{
			mapExists = true;
			convertButton.Enabled = true;

			//iterate over the dictionary to poplulate the listbox with the mappings
			foreach(KeyValuePair<string,List<ChannelMapping>> kvp in StaticModuleData.Vixen2xMappings)
			{
				vixen2ToVixen3MappingListBox.Items.Add(kvp.Key);

			}
		}

        private void ParseV2SequenceData()
        {
            parsedV2Sequence = new Vixen2SequenceData(vixen2ImportFile);

            vixen2ProfileTextBox.Text = String.Format(@"{0}\{1}.pro", parsedV2Sequence.ProfilePath, parsedV2Sequence.ProfileName);
			vixen2ToVixen3MappingListBox.Text = parsedV2Sequence.ProfileName;
        }
    
        private void createMapButton_Click(object sender, EventArgs e)
        {
		   using (Vixen2xSequenceImporterChannelMapper mappingForm = new Vixen2xSequenceImporterChannelMapper(channelMappings,mapExists,vixen2ToVixen3MappingTextBox.Text))
            {
				if (mappingForm.ShowDialog() == DialogResult.OK)
				{
					//add to or update the dictionary
					AddDictionaryEntry(mappingForm.MappingName, mappingForm.Mappings);
					convertButton.Enabled = true;
				}
				else
				{
					//The user cancelled out so leave our current map in place
					MessageBox.Show("No changes will be stored since you cancelled the operation", "Cancelled Form", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Sequence = null;
        }

        private void convertButton_Click(object sender, EventArgs e)
        {
            //check to see if the mapping table is there.
			if (StaticModuleData.Vixen2xMappings.Count > 0)
			{
				vixen3SequenceCreator = new Vixen3SequenceCreator(parsedV2Sequence, StaticModuleData.Vixen2xMappings[vixen2ToVixen3MappingTextBox.Text]);

				Sequence = vixen3SequenceCreator.Sequence;

				if (Sequence.SequenceData != null)
				{
					//we got this baby converted so close it out and load up the Sequence
					DialogResult = System.Windows.Forms.DialogResult.OK;
					Close();
				}
			}
			else
			{
				MessageBox.Show("Mapping data is missing, please try again...", "No Mapping Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
        }

		private void AddDictionaryEntry(string key, List<ChannelMapping> value)
		{
			if (StaticModuleData.Vixen2xMappings.ContainsKey(key))
			{
				StaticModuleData.Vixen2xMappings[key] = value;
			}
			else
			{
				StaticModuleData.Vixen2xMappings.Add(key, value);
			}
		}

		private void vixen2ToVixen3MappingListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			vixen2ToVixen3MappingTextBox.Text = vixen2ToVixen3MappingListBox.SelectedItem.ToString();
			
			//user selected a pre-existing mapping so use it now.
			channelMappings = StaticModuleData.Vixen2xMappings[vixen2ToVixen3MappingListBox.SelectedItem.ToString()];
		}
	}
}