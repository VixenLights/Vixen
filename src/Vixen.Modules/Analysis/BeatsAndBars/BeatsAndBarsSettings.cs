using Common.Controls;
using Common.Controls.Theme;
using QMLibrary;
using System.ComponentModel;
using Vixen.Marks;
using VixenModules.Media.Audio;

namespace VixenModules.Analysis.BeatsAndBars
{
	
	public partial class BeatsAndBarsDialog : BaseForm
	{
		private static BeatBarSettingsData m_settingsData = null;
		private bool m_allowUpdates;
		private BeatBarPreviewData m_previewData;
		private PreviewWaveform m_previewWaveForm;


		public BeatsAndBarsDialog(Audio audio)
		{
			InitializeComponent();

			var excludes = new List<Control>();
			excludes.Add(BarsColorPanel);
			excludes.Add(BeatCountsColorPanel);
			excludes.Add(AllColorPanel);
			excludes.Add(BeatSplitsColorPanel);
			ThemeUpdateControls.UpdateControls(this, excludes);

			m_allowUpdates = false;

			var mToolTip = new ToolTip();
			mToolTip.AutoPopDelay = 5000;
			mToolTip.InitialDelay = 500;
			mToolTip.ReshowDelay = 500;
			mToolTip.ShowAlways = true;
			mToolTip.Active = true;

			mToolTip.SetToolTip(AllFeaturesCB, "Single Collection containing all features");
			mToolTip.SetToolTip(BarsCB, "Single Collection containing starting location of each measure/bar");
			mToolTip.SetToolTip(BeatCountsCB, "Generates a beat collection for each beat count");
			mToolTip.SetToolTip(BeatSplitsCB, "Generates a beat collection for each beat count and each beat count split");
			mToolTip.SetToolTip(BeatsNameTB, "Base name of each collection");
			mToolTip.SetToolTip(AllColorPanel, "Color of All Features Collection");
			mToolTip.SetToolTip(BarsColorPanel, "Color of Bars Collection");
			mToolTip.SetToolTip(BeatCountsColorPanel, "Color of Beat Counts Collection");
			mToolTip.SetToolTip(BeatSplitsColorPanel, "Color of Beat Splits Collection");

			m_settingsData = m_settingsData ?? new BeatBarSettingsData("Beats");

			BarsCB.Checked = true;
			AllFeaturesCB.Checked = true;
			BeatCountsCB.Checked = true;
			BeatSplitsCB.Checked = false;

			m_allowUpdates = true;
			SetBeatBarOutputSettings();

			musicStaff1.Width = grpDivisions.ClientSize.Width - 20;

			m_previewWaveForm = new PreviewWaveform(audio);
			m_previewWaveForm.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			PreviewGroupBox.Controls.Add(m_previewWaveForm);
			m_previewWaveForm.Width = PreviewGroupBox.ClientSize.Width-25;
			m_previewWaveForm.Height = PreviewGroupBox.ClientSize.Height/2;
			m_previewWaveForm.Location = new Point(musicStaff1.Location.X, PreviewGroupBox.ClientSize.Height/2 - m_previewWaveForm.Height/2);

			musicStaff1.SettingChanged += MusicStaffSettingsChanged;

			
		}

		public BeatBarSettingsData Settings
		{
			get
			{
				return m_settingsData;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<IMarkCollection> MarkCollectionList { private get; set; } 

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
				if (PreviewData.PreviewSplitCollection.Marks.Any())
				{
					m_previewWaveForm.PreviewPeriod = PreviewData.PreviewSplitCollection.Marks.Max(x => x.StartTime);
				}
	
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

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}
	}
}
