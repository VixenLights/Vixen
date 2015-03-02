using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QMLibrary;
using VixenModules.Sequence.Timed;

namespace VixenModules.Analysis.BeatsAndBars
{
	
	public partial class BeatsAndBarsDialog : Form
	{

		private ToolTip m_toolTip;
		private static BeatBarSettingsData m_settingsData = null;
		private bool m_allowUpdates;
		private bool m_allowClose;
		private ManagedPlugin m_plugin;

		public BeatsAndBarsDialog(ManagedPlugin plugin)
		{
			InitializeComponent();

			m_allowUpdates = false;
			m_allowClose = false;

			m_toolTip = new ToolTip();
			m_toolTip.AutoPopDelay = 5000;
			m_toolTip.InitialDelay = 500;
			m_toolTip.ReshowDelay = 500;
			m_toolTip.ShowAlways = true;
			m_toolTip.Active = true;

			m_settingsData = m_settingsData ?? new BeatBarSettingsData("Beats");

			m_plugin = plugin;

			BaseColorPanel.BackColor = m_settingsData.Color;

			m_allowUpdates = true;
			SetBeatBarOutputSettings();
		}

		public BeatBarSettingsData Settings
		{
			get
			{
				return m_settingsData;
			}
		}


		public void Parameters(ICollection<ManagedParameterDescriptor> parameterDescriptors)
		{
			//this.m_vampParamCtrl.InitParamControls(parameterDescriptors);
		}

		public List<MarkCollection> MarkCollectionList { set; private get; } 

		public float BeatsPerBar
		{
			get
			{
				return musicStaff1.BeatsPerBar;	
			}
		}

		private void SetBeatBarOutputSettings()
		{
			if (m_allowUpdates)
			{
				m_settingsData.AllFeaturesEnabled = AllFeaturesCB.Checked;
				m_settingsData.BarsEnabled = BarsCB.Checked;
				m_settingsData.BeatCollectionsEnabled = BeatCountsCB.Checked;
				m_settingsData.BeatSplitsEnabled = BeatSplitsCB.Checked;

				m_settingsData.CollectionBaseName = BeatsNameTB.Text;
				m_settingsData.Color = BaseColorPanel.BackColor;
				m_settingsData.Divisions = (musicStaff1.SplitBeats ? 2 : 1);

				m_settingsData.BeatsPerBar = musicStaff1.BeatsPerBar;
				m_settingsData.NoteSize = musicStaff1.NoteSize;
			}
		}


		private void BeatColorPanel_Click(object sender, EventArgs e)
		{
			Common.Controls.ColorManagement.ColorPicker.ColorPicker picker =
				new Common.Controls.ColorManagement.ColorPicker.ColorPicker();

			DialogResult result = picker.ShowDialog();
			if (result == DialogResult.OK)
			{
				m_settingsData.Color = picker.Color.ToRGB().ToArgb();
			}
		}

		private void GoButton_Click(object sender, EventArgs e)
		{
			m_plugin.SetParameter("bpb", BeatsPerBar);

			m_plugin.Initialise(1,
				(uint)m_plugin.GetPreferredStepSize(),
				(uint)m_plugin.GetPreferredBlockSize());

			SetBeatBarOutputSettings();

		}


		private void BeatsAndBarsDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (m_allowClose)
			{
				return;
			}

			if (MarkCollectionList.FindIndex(x => x.Name.Equals(BeatsNameTB.Text)) != -1)
			{
				if (MessageBox.Show("A collection by the name of " +
									BeatsNameTB.Text +
									" already exists\nOverwrite?",
									"Beat Settings",
									MessageBoxButtons.YesNo) == DialogResult.No)
				{
					e.Cancel = true;
					return;
				}
			}
		}

		private void CancelButton_Click(object sender, EventArgs e)
		{
			m_allowClose = true;
		}

		private void musicStaff1_Paint(object sender, PaintEventArgs e)
		{
			BeatSplitsCB.Enabled = musicStaff1.SplitBeats;
		}
	}

	public class BeatBarSettingsData
	{
		public bool BarsEnabled { get; set; }
		public bool BeatCollectionsEnabled { get; set; }
		public bool BeatSplitsEnabled { get; set; }
		public bool AllFeaturesEnabled { get; set; }
		public String CollectionBaseName { get; set; }

		public Color Color { get; set; }
		public int Divisions { get; set; }

		public int BeatsPerBar { get; set; }
		public int NoteSize { get; set; }

		public String AllCollectionName
		{
			get { return CollectionBaseName + " - All"; }
		}

		public String BarsCollectionName
		{
			get { return CollectionBaseName + " - Bars";  }
		}

		public String[] BeatCollectionNames(bool addDivisions)
		{
			int collections = BeatsPerBar * ((addDivisions) ? Divisions : 1);
			int actualNoteSize = NoteSize * ((addDivisions) ? Divisions : 1);
			
			String[] retVal = new string[collections];
			
			for (int j = 0; j < collections; j++)
			{
				retVal[j] = CollectionBaseName + " 1/" + actualNoteSize + " Note - " + j;
			}

			return retVal;
		}

		public BeatBarSettingsData(String collectionBaseName)
		{
			BarsEnabled = false;
			BeatCollectionsEnabled = false;
			BeatSplitsEnabled = false;
			AllFeaturesEnabled = false;

			CollectionBaseName = collectionBaseName;
			Color = Color.White;
			Divisions = 1;
		}

	}
}
