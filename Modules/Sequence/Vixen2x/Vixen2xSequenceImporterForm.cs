using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Vixen.Sys;


namespace VixenModules.SequenceType.Vixen2x {
	public partial class Vixen2xSequenceImporterForm : Form {
		
        public ISequence Sequence { get; set; }

        private string vixen2ImportFile;
		private Vixen2SequenceData parsedV2Sequence = null;
        private Vixen3SequenceCreator vixen3SequenceCreator = null;
        private Vixen2xSequenceStaticData StaticModuleData;

        public Vixen2xSequenceImporterForm(string Vixen2File, Vixen.Module.IModuleDataModel staticModuleData)
        {
			InitializeComponent();

            //I think this was the correct way to implement this.
            StaticModuleData = (Vixen2xSequenceStaticData)staticModuleData;
            
            vixen2ImportFile = Vixen2File;

            //Add known information:
            vixen2SequenceTextBox.Text = vixen2ImportFile;

            //Go ahead and build the map for the sequence that we have.
            ParseV2SequenceData();

            //not really used at this time.  May be used if we decide to clear an existing map and
            //create a new one.
            LoadMap();
		}

        private void ParseV2SequenceData()
        {
            parsedV2Sequence = new Vixen2SequenceData(vixen2ImportFile);

            vixen2ProfileTextBox.Text = parsedV2Sequence.ProfileName;
        }

        private void LoadMap()
        {
            //if (StaticModuleData.Vixen2xMappings.Count > 0)
            //{
            //    createMapButton.Enabled = false;
            //}

        }
    
        private void loadMapButton_Click(object sender, EventArgs e)
        {
            //not enabled at this time.
        }

        private void createMapButton_Click(object sender, EventArgs e)
        {
            List<ChannelMapping> mappings = new List<ChannelMapping>();

            //For now we are going to assume that the map has already been done and somebody just
            //wants to make a change.  We are not considering another profile from outside the system
            //at this time.
            if (StaticModuleData.Vixen2xMappings.Count > 0)
            {
                mappings = StaticModuleData.Vixen2xMappings;
            }
            else
            {
                mappings = parsedV2Sequence.mappings;
            }

            using (Vixen2xSequenceImporterChannelMapper mappingForm = new Vixen2xSequenceImporterChannelMapper(mappings))
            {
                if (mappingForm.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        //Create our Vixen 3 sequences.
                        StaticModuleData.Vixen2xMappings.Clear();
                        StaticModuleData.Vixen2xMappings = mappingForm.Mappings;
                    }
                    catch (Exception ex)
                    {
                        int x = 0;
                    }

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
        }
	}
}