using Common.Controls;
using Common.Controls.Theme;
using QMLibrary;
using System.ComponentModel;
using VixenModules.Media.Audio;

namespace VixenModules.Analysis.BeatsAndBars
{
	
	public sealed partial class BeatsAndBarsDialog : BaseForm
	{
		private static BeatBarSettingsData _settingsData;
		private readonly bool _allowUpdates;
		private readonly PreviewWaveform _previewWaveForm;


		public BeatsAndBarsDialog(Audio audio)
		{
			InitializeComponent();

			var excludes = new List<Control>
			{
				BarsColorPanel,
				BeatCountsColorPanel,
				AllColorPanel,
				BeatSplitsColorPanel
			};
			ThemeUpdateControls.UpdateControls(this, excludes);

			_allowUpdates = false;

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

			_settingsData ??= new BeatBarSettingsData("Beats");

			BarsCB.Checked = true;
			AllFeaturesCB.Checked = true;
			BeatCountsCB.Checked = true;
			BeatSplitsCB.Checked = false;

			_allowUpdates = true;
			SetBeatBarOutputSettings();

			musicStaff1.Width = grpDivisions.ClientSize.Width - 20;

			_previewWaveForm = new PreviewWaveform(audio);
			_previewWaveForm.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			PreviewGroupBox.Controls.Add(_previewWaveForm);
			_previewWaveForm.Width = PreviewGroupBox.ClientSize.Width-25;
			_previewWaveForm.Height = PreviewGroupBox.ClientSize.Height/2;
			_previewWaveForm.Location = new Point(musicStaff1.Location.X, PreviewGroupBox.ClientSize.Height/2 - _previewWaveForm.Height/2);

			musicStaff1.SettingChanged += MusicStaffSettingsChanged;

			
		}

		public BeatBarSettingsData Settings
		{
			get
			{
				return _settingsData;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BeatBarPreviewData PreviewData
		{
			get;

			set
			{
				field = value;
				musicStaff1.BeatPeriod = field.BeatPeriod;
				SetBeatBarOutputSettings();
			}
		}

		public void Parameters(ICollection<ManagedParameterDescriptor> parameterDescriptors) { }

		private void SetBeatBarOutputSettings()
		{
			if (_allowUpdates)
			{
				_settingsData.AllFeaturesEnabled = AllFeaturesCB.Checked;
				_settingsData.BarsEnabled = BarsCB.Checked;
				_settingsData.BeatCollectionsEnabled = BeatCountsCB.Checked;
				_settingsData.BeatSplitsEnabled = BeatSplitsCB.Checked;

				_settingsData.CollectionBaseName = BeatsNameTB.Text;
				_settingsData.Divisions = (musicStaff1.SplitBeats ? 2 : 1);

				_settingsData.AllFeaturesColor = AllColorPanel.BackColor;
				_settingsData.BarsColor = BarsColorPanel.BackColor;
				_settingsData.BeatCountsColor = BeatCountsColorPanel.BackColor;
				_settingsData.BeatSplitsColor = BeatSplitsColorPanel.BackColor;

				_settingsData.BeatsPerBar = musicStaff1.BeatsPerBar;
				_settingsData.NoteSize = musicStaff1.NoteSize;

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
				colorPanel?.BackColor = picker.Color.ToRGB().ToArgb();
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
				_previewWaveForm.IntervalMarks =
					(musicStaff1.SplitBeats) ?
					PreviewData.PreviewSplitCollection.Marks.Select(x => x.StartTime).ToList() :
					PreviewData.PreviewCollection.Marks.Select(x => x.StartTime).ToList();
				if (PreviewData.PreviewSplitCollection.Marks.Any())
				{
					_previewWaveForm.PreviewPeriod = PreviewData.PreviewSplitCollection.Marks.Max(x => x.StartTime);
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
