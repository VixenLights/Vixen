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

		private Vixen2SequenceData parsedV2Sequence = null;
        private Vixen3SequenceCreator vixen3SequenceCreator = null;
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

			if (StaticModuleData.Vixen2xMappings.Count > 0)
			{
				LoadMap();
			}
		}

		private void LoadMap()
		{
			DialogResult result = MessageBox.Show("A mapping already exists, do you wish to keep it?", "Mapping Exists", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
			if (result == DialogResult.No)
			{
				//Send the vixen 2 parsed data and let the user create a new map.
				mapExists = false;
				channelMappings = parsedV2Sequence.mappings;
			}
			else
			{
				//we have a map so lets enable the convert button and set the channel mappings to
				//our mapped data.
				mapExists = true;
				covertButton.Enabled = true;
				channelMappings = StaticModuleData.Vixen2xMappings;
			}
		}

        private void ParseV2SequenceData()
        {
            parsedV2Sequence = new Vixen2SequenceData(vixen2ImportFile);

            vixen2ProfileTextBox.Text = parsedV2Sequence.ProfileName;
        }
    
        private void createMapButton_Click(object sender, EventArgs e)
        {
		   using (Vixen2xSequenceImporterChannelMapper mappingForm = new Vixen2xSequenceImporterChannelMapper(channelMappings,mapExists))
            {
				if (mappingForm.ShowDialog() == DialogResult.OK)
				{
					//Now that we have our mapping lets clear out what we have
					//and save it to our static module data
					StaticModuleData.Vixen2xMappings.Clear();
					StaticModuleData.Vixen2xMappings = mappingForm.Mappings;

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

        private void covertButton_Click(object sender, EventArgs e)
        {
            //check to see if the mapping table is there.
			if (StaticModuleData.Vixen2xMappings.Count > 0)
			{
				vixen3SequenceCreator = new Vixen3SequenceCreator(parsedV2Sequence, StaticModuleData.Vixen2xMappings);

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
	}
}