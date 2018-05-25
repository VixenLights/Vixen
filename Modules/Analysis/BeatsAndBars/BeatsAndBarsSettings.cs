using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using QMLibrary;
using Vixen.Marks;
using VixenModules.Analysis.BeatsAndBars.Properties;
using VixenModules.App.Marks;
using VixenModules.Media.Audio;

namespace VixenModules.Analysis.BeatsAndBars
{
	
	public partial class BeatsAndBarsDialog : BaseForm
	{
		private ToolTip m_toolTip;
		private static BeatBarSettingsData m_settingsData = null;
		private bool m_allowUpdates;
		private BeatBarPreviewData m_previewData;
		private PreviewWaveform m_previewWaveForm;


		public BeatsAndBarsDialog(Audio audio)
		{
			InitializeComponent();

			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			var excludes = new List<Control>();
			excludes.Add(BarsColorPanel);
			excludes.Add(BeatCountsColorPanel);
			excludes.Add(AllColorPanel);
			excludes.Add(BeatSplitsColorPanel);
			ThemeUpdateControls.UpdateControls(this, excludes);

			m_allowUpdates = false;

			m_toolTip = new ToolTip();
			m_toolTip.AutoPopDelay = 5000;
			m_toolTip.InitialDelay = 500;
			m_toolTip.ReshowDelay = 500;
			m_toolTip.ShowAlways = true;
			m_toolTip.Active = true;

			m_toolTip.SetToolTip(AllFeaturesCB, "Single Collection containing all features");
			m_toolTip.SetToolTip(BarsCB, "Single Collection containing starting location of each measure/bar");
			m_toolTip.SetToolTip(BeatCountsCB, "Generates a beat collection for each beat count");
			m_toolTip.SetToolTip(BeatSplitsCB, "Generates a beat collection for each beat count and each beat count split");
			m_toolTip.SetToolTip(BeatsNameTB, "Base name of each collection");
			m_toolTip.SetToolTip(AllColorPanel, "Color of All Features Collection");
			m_toolTip.SetToolTip(BarsColorPanel, "Color of Bars Collection");
			m_toolTip.SetToolTip(BeatCountsColorPanel, "Color of Beat Counts Collection");
			m_toolTip.SetToolTip(BeatSplitsColorPanel, "Color of Beat Splits Collection");

			m_settingsData = m_settingsData ?? new BeatBarSettingsData("Beats");

			BarsCB.Checked = true;
			AllFeaturesCB.Checked = true;
			BeatCountsCB.Checked = true;
			BeatSplitsCB.Checked = false;

			m_allowUpdates = true;
			SetBeatBarOutputSettings();

			m_previewWaveForm = new PreviewWaveform(audio);
			m_previewWaveForm.Width = musicStaff1.Width;
			m_previewWaveForm.Height = 75;
			m_previewWaveForm.Location = new Point(musicStaff1.Location.X, 25);

			musicStaff1.SettingChanged += MusicStaffSettingsChanged;

			PreviewGroupBox.Controls.Add(m_previewWaveForm);
		}

		public BeatBarSettingsData Settings
		{
			get
			{
				return m_settingsData;
			}
		}

		public BeatBarPreviewData PreviewData
		{
			get
			{
				return m_previewData;
			}

			set
			{
				m_previewData = value;
				musicStaff1.BeatPeriod = m_previewData.BeatPeriod;
				SetBeatBarOutputSettings();
			}
		}
		public void Parameters(ICollection<ManagedParameterDescriptor> parameterDescriptors) { }

		public List<IMarkCollection> MarkCollectionList { set; private get; } 

		private void SetBeatBarOutputSettings()
		{
			if (m_allowUpdates)
			{
				m_settingsData.AllFeaturesEnabled = AllFeaturesCB.Checked;
				m_settingsData.BarsEnabled = BarsCB.Checked;
				m_settingsData.BeatCollectionsEnabled = BeatCountsCB.Checked;
				m_settingsData.BeatSplitsEnabled = BeatSplitsCB.Checked;

				m_settingsData.CollectionBaseName = BeatsNameTB.Text;
				m_settingsData.Divisions = (musicStaff1.SplitBeats ? 2 : 1);

				m_settingsData.AllFeaturesColor = AllColorPanel.BackColor;
				m_settingsData.BarsColor = BarsColorPanel.BackColor;
				m_settingsData.BeatCountsColor = BeatCountsColorPanel.BackColor;
				m_settingsData.BeatSplitsColor = BeatSplitsColorPanel.BackColor;

				m_settingsData.BeatsPerBar = musicStaff1.BeatsPerBar;
				m_settingsData.NoteSize = musicStaff1.NoteSize;

				UpdatePreviewWaveform();

				GenerateButton.Enabled = false;
				if (AllFeaturesCB.Checked ||
					BarsCB.Checked ||
					BeatCountsCB.Checked ||
					BeatSplitsCB.Checked)
				{
					GenerateButton.Enabled = true;
				}
			}

		}

		private void ColorPanel_Click(object sender, EventArgs e)
		{
			Panel colorPanel = sender as Panel;
			Common.Controls.ColorManagement.ColorPicker.ColorPicker picker =
				new Common.Controls.ColorManagement.ColorPicker.ColorPicker();

			DialogResult result = picker.ShowDialog();
			if (result == DialogResult.OK)
			{
				colorPanel.BackColor = picker.Color.ToRGB().ToArgb();
			}
			SetBeatBarOutputSettings();
		}

		private void GoButton_Click(object sender, EventArgs e)
		{
			SetBeatBarOutputSettings();				
		}

		private void MusicStaffSettingsChanged(object sender, EventArgs e)
		{
			if (!musicStaff1.SplitBeats)
			{
				BeatSplitsCB.Checked = false;
			}

			BeatSplitsColorPanel.Enabled = musicStaff1.SplitBeats;
			BeatSplitsCB.Enabled = musicStaff1.SplitBeats;
			BeatSplitsCB.Checked = musicStaff1.SplitBeats;

			UpdatePreviewWaveform();
		}

		private void UpdatePreviewWaveform()
		{
			if (PreviewData != null)
			{
				m_previewWaveForm.IntervalMarks =
					(musicStaff1.SplitBeats) ?
					PreviewData.PreviewSplitCollection.Marks.Select(x => x.StartTime).ToList() :
					PreviewData.PreviewCollection.Marks.Select(x => x.StartTime).ToList();

				m_previewWaveForm.PreviewPeriod = PreviewData.PreviewSplitCollection.Marks.Max(x => x.StartTime);
	
				Refresh();
			}
		}

		private void musicStaff1_Paint(object sender, PaintEventArgs e)
		{

		}

		private void BarsCB_CheckedChanged(object sender, EventArgs e)
		{
			BarsColorPanel.Enabled = BarsCB.Checked;
			SetBeatBarOutputSettings();
		}

		private void BeatCountsCB_CheckedChanged(object sender, EventArgs e)
		{
			BeatCountsColorPanel.Enabled = BeatCountsCB.Checked;
			SetBeatBarOutputSettings();
		}

		private void BeatSplitsCB_CheckedChanged(object sender, EventArgs e)
		{
			BeatSplitsColorPanel.Enabled = BeatSplitsCB.Checked;
			SetBeatBarOutputSettings();
		}

		private void AllFeaturesCB_CheckedChanged(object sender, EventArgs e)
		{
			AllColorPanel.Enabled = AllFeaturesCB.Checked;
			SetBeatBarOutputSettings();
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.HeadingBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.HeadingBackgroundImage;
		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}
	}
}
