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
		private static BeatBarSettings m_beatSettings = null;
		private static BeatBarSettings m_barSettings = null;
		private bool m_allowUpdates;
		private bool m_allowClose;

		public BeatsAndBarsDialog()
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

			m_beatSettings = m_beatSettings ?? new BeatBarSettings("Beats");
			m_barSettings = m_barSettings ?? new BeatBarSettings("Bars");


			BeatColorPanel.BackColor = m_beatSettings.Color;

			m_allowUpdates = true;
			SetBeatBarOutputControls();
		}

		void DoDialogSizings()
		{

		}

		public BeatBarSettings BeatSettings
		{
			get
			{
				return m_beatSettings;
			}
		}

		public BeatBarSettings BarSettings
		{
			get
			{
				return m_barSettings;
			}
		}

		public void Parameters(ICollection<ManagedParameterDescriptor> parameterDescriptors)
		{
			//this.m_vampParamCtrl.InitParamControls(parameterDescriptors);
			DoDialogSizings();

		}

		public List<MarkCollection> MarkCollectionList { set; private get; } 

		public float BeatsPerBar()
		{
			return musicStaff1.BeatsPerBar;
		}
		private void SetBeatBarOutputControls()
		{
			if (m_allowUpdates)
			{
			}
		}

		private void BeatsCB_CheckedChanged(object sender, EventArgs e)
		{
			SetBeatBarOutputControls();
		}

		private void BarsCB_CheckedChanged(object sender, EventArgs e)
		{
			SetBeatBarOutputControls();
		}

		private void BeatColorPanel_Click(object sender, EventArgs e)
		{
			Common.Controls.ColorManagement.ColorPicker.ColorPicker picker =
				new Common.Controls.ColorManagement.ColorPicker.ColorPicker();

			DialogResult result = picker.ShowDialog();
			if (result == DialogResult.OK)
			{
				m_beatSettings.Color = picker.Color.ToRGB().ToArgb();
			}
			SetBeatBarOutputControls();
		}

		private void BarColorPanel_Click(object sender, EventArgs e)
		{
			Common.Controls.ColorManagement.ColorPicker.ColorPicker picker =
				new Common.Controls.ColorManagement.ColorPicker.ColorPicker();

			DialogResult result = picker.ShowDialog();
			if (result == DialogResult.OK)
			{
				m_barSettings.Color = picker.Color.ToRGB().ToArgb();
			}
			SetBeatBarOutputControls();
		}

		private void GoButton_Click(object sender, EventArgs e)
		{
			SetBeatBarOutputControls();
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

		private void m_cancelButton_Click(object sender, EventArgs e)
		{
			m_allowClose = true;
		}

		private void musicStaff1_Paint(object sender, PaintEventArgs e)
		{
			BeatSplitsCB.Enabled = musicStaff1.SplitBeats;
		}
	}

	public class BeatBarSettings
	{
		public bool Enabled { get; set; }
		public String CollectionName { get; set; }
		public Color Color { get; set; }
		public int Divisions { get; set; }

		public BeatBarSettings(String collectionName)
		{
			Enabled = false;
			CollectionName = collectionName;
			Color = Color.White;
			Divisions = 0;
		}

	}
}
