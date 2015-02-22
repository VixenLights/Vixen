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

			m_toolTip.SetToolTip(m_beatsCB, "Enable/Disable Beat Mark Generation");
			m_toolTip.SetToolTip(m_beatsNameTB, "The Name of the Beat collection to generate");
			m_toolTip.SetToolTip(m_beatsSubMarks, "Number of evenly spaced submarks to generate per feature");

			m_toolTip.SetToolTip(m_barsCB, "Enable/Disable Bar Mark Generation");
			m_toolTip.SetToolTip(m_barsNameTB, "The Name of the Bar collection to generate");
			m_toolTip.SetToolTip(m_barsSubmarks, "Number of evenly spaced submarks to generate per feature");

			m_beatSettings = m_beatSettings ?? new BeatBarSettings("Beats");
			m_barSettings = m_barSettings ?? new BeatBarSettings("Bars");

			m_beatsCB.Checked = m_beatSettings.Enabled;
			m_barsCB.Checked = m_barSettings.Enabled;

			m_barsNameTB.Text = m_barSettings.CollectionName;
			m_beatsNameTB.Text = m_beatSettings.CollectionName;

			m_barsSubmarks.Text = m_barSettings.Divisions.ToString();
			m_beatsSubMarks.Text = m_beatSettings.Divisions.ToString();

			m_beatColorPanel.BackColor = m_beatSettings.Color;
			m_barColorPanel.BackColor = m_barSettings.Color;

			m_allowUpdates = true;
			SetBeatBarOutputControls();
		}

		void DoDialogSizings()
		{
			Size offsetSize = new Size(20, 20);
			this.m_paramsGroupBox.Size = m_vampParamCtrl.Size + offsetSize;

			if (m_paramsGroupBox.Size.Width > m_outputGroupBox.Size.Width)
			{
				m_outputGroupBox.Size = new Size(m_paramsGroupBox.Size.Width, m_outputGroupBox.Size.Height);
			}
			else
			{
				m_paramsGroupBox.Size = new Size(m_outputGroupBox.Size.Width, m_paramsGroupBox.Size.Height);
			}
			m_outputGroupBox.Location =
				new Point(m_paramsGroupBox.Location.X,
					m_paramsGroupBox.Location.X + m_paramsGroupBox.Height + offsetSize.Height);

			m_goButton.Location = new Point(m_goButton.Location.X, m_outputGroupBox.Location.Y + m_outputGroupBox.Height + offsetSize.Height);
			m_cancelButton.Location = new Point(m_cancelButton.Location.X, m_goButton.Location.Y);
			this.ClientSize =  new Size(m_outputGroupBox.Location.X *2  + m_outputGroupBox.Width, 
								 (m_paramsGroupBox.Location.Y) + m_goButton.Location.Y + m_goButton.Size.Height);
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
			this.m_vampParamCtrl.InitParamControls(parameterDescriptors);
			DoDialogSizings();

		}

		public List<MarkCollection> MarkCollectionList { set; private get; } 

		public float BeatsPerBar()
		{
			float retVal = 0;
			
			String strVal = m_vampParamCtrl.FindParamByIdentifier("bpb");
			if (strVal != null)
			{
				retVal = (float)Convert.ToDouble(strVal);
			}
			return retVal;
		}
		private void SetBeatBarOutputControls()
		{
			if (m_allowUpdates)
			{
				m_beatsNameTB.Enabled = m_beatsCB.Checked;
				m_beatsSubMarks.Enabled = m_beatsCB.Checked;
				m_barsNameTB.Enabled = m_barsCB.Checked;
				m_barsSubmarks.Enabled = m_barsCB.Checked;

				m_beatSettings.Enabled = m_beatsCB.Checked;
				m_beatSettings.CollectionName = m_beatsNameTB.Text;
				m_beatColorPanel.BackColor = m_beatsCB.Checked ? m_beatSettings.Color : SystemColors.Control;
				m_beatColorPanel.Enabled = m_beatsCB.Checked;
				m_beatSettings.Divisions = Convert.ToInt32(m_beatsSubMarks.Text);

				m_barSettings.Enabled = m_barsCB.Checked;
				m_barSettings.CollectionName = m_barsNameTB.Text;
				m_barColorPanel.BackColor = m_barsCB.Checked ? m_barSettings.Color : SystemColors.Control;
				m_barColorPanel.Enabled = m_barsCB.Checked;
				m_barSettings.Divisions = Convert.ToInt32(m_barsSubmarks.Text);
				
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

			if ((m_beatsCB.Checked) && 
				(MarkCollectionList.FindIndex(x => x.Name.Equals(m_beatsNameTB.Text)) != -1))
			{
				if (MessageBox.Show("A collection by the name of " +
									m_beatsNameTB.Text +
									" already exists\nOverwrite?",
									"Beat Settings",
									MessageBoxButtons.YesNo) == DialogResult.No)
				{
					e.Cancel = true;
					return;
				}
			}
			
			if ((m_barsCB.Checked) &&
				(MarkCollectionList.FindIndex(x => x.Name.Equals(m_barsNameTB.Text)) != -1))
			{
				if (MessageBox.Show("A collection by the name of " +
									m_barsNameTB.Text +
									" already exists\nOverwrite?",
									"Bar Settings",
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
