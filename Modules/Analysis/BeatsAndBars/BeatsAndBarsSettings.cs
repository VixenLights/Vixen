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

namespace VixenModules.Analysis.BeatsAndBars
{
	
	public partial class BeatsAndBarsDialog : Form
	{

		private ToolTip m_toolTip;
		private BeatBarSettings m_beatSettings;
		private BeatBarSettings m_barSettings;

		public BeatsAndBarsDialog()
		{
			InitializeComponent();
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

			m_beatsCB.Checked = false;
			m_barsCB.Checked = false;

			m_beatSettings = new BeatBarSettings();
			m_barSettings = new BeatBarSettings();

			m_beatColorPanel.BackColor = m_beatSettings.Color;
			m_barColorPanel.BackColor = m_barSettings.Color;

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

		public void Outputs(ICollection<ManagedOutputDescriptor> outputDescriptors)
		{
			DoDialogSizings();
		}

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
			m_beatsNameTB.Enabled = m_beatsCB.Checked;
			m_beatsSubMarks.Enabled = m_beatsCB.Checked;
			m_barsNameTB.Enabled = m_barsCB.Checked;
			m_barsSubmarks.Enabled = m_barsCB.Checked;

			m_beatSettings.Enabled = m_beatsCB.Checked;
			m_beatSettings.BeatCollectionName = m_beatsNameTB.Text;
			m_beatColorPanel.BackColor = m_beatsCB.Checked ? m_beatSettings.Color : SystemColors.Control;
			m_beatColorPanel.Enabled = m_beatsCB.Checked;

			m_barSettings.Enabled = m_barsCB.Checked;
			m_barSettings.BeatCollectionName = m_barsNameTB.Text;
			m_barColorPanel.BackColor = m_barsCB.Checked ? m_barSettings.Color : SystemColors.Control;
			m_barColorPanel.Enabled = m_barsCB.Checked;
		}

		private void m_beatsCB_CheckedChanged(object sender, EventArgs e)
		{
			SetBeatBarOutputControls();
		}

		private void m_barsCB_CheckedChanged(object sender, EventArgs e)
		{
			SetBeatBarOutputControls();
		}

		private void m_beatsNameTB_TextChanged(object sender, EventArgs e)
		{
			SetBeatBarOutputControls();
		}

		private void m_barsNameTB_TextChanged(object sender, EventArgs e)
		{
			SetBeatBarOutputControls();
		}

		private void m_beatColorPanel_Click(object sender, EventArgs e)
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

		private void m_barColorPanel_Click(object sender, EventArgs e)
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
	}

	public class BeatBarSettings
	{
		public bool Enabled { get; set; }
		public String BeatCollectionName { get; set; }
		public Color Color { get; set; }

		public BeatBarSettings()
		{
			Enabled = false;
			BeatCollectionName = "";
			Color = Color.White;
		}

	}
}
