using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.Controls.Wizard;
using NLog;
using Vixen.Export;
using Vixen.Module.Media;
using Vixen.Services;
using Vixen.Sys;

namespace VixenModules.App.ExportWizard
{
	public partial class BulkExportSummaryStage : WizardStage
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
		private readonly BulkExportWizardData _data;
		private bool _cancelled;
		private AutoCompleteStringCollection namesCollection = new AutoCompleteStringCollection();
		private BindingList<ExportProfile> _profiles;

		public BulkExportSummaryStage(BulkExportWizardData data)
		{
			_data = data;
			InitializeComponent();
			taskProgress.Minimum = 0;
			taskProgress.Maximum = 100;
			overallProgress.Minimum = 0;
			overallProgress.Maximum = 100;
			//lblSummary.Font = ThemeUpdateControls.SizeAndStyleFont(lblSummary.Font, lblSummary.Font.Size + 4, FontStyle.Bold);
		}

		private void ConfigureSummary()
		{
			_data.ConfigureExport(_data.ActiveProfile);
			chkSaveConfig.Checked = false;
			comboConfigName.Visible = false;
			lblSequenceCount.Text = _data.ActiveProfile.SequenceFiles.Count().ToString();
			lblTimingValue.Text = string.Format("{0} ms", _data.ActiveProfile.Interval);
			lblFormatName.Text = _data.ActiveProfile.Format;
			lblOutputFolder.Text = _data.ActiveProfile.OutputFolder;
			string audioOption = "Not included.";
			lblAudioOutputFolder.Visible = lblAudioDestination.Visible = _data.ActiveProfile.IncludeAudio;
			if (_data.ActiveProfile.IncludeAudio)
			{
				audioOption = _data.ActiveProfile.RenameAudio ? "Rename to match sequence name." : "Include as is.";
				lblAudioOutputFolder.Text = _data.ActiveProfile.AudioOutputFolder;
			}
			
			lblAudioOption.Text = audioOption;
			if (_data.ActiveProfile.IsFalconFormat && _data.ActiveProfile.CreateUniverseFile)
			{
				lblUniverseFolder.Text = _data.ActiveProfile.FalconOutputFolder;
				lblUniverseFolder.Visible = lblUniverse.Visible = true;
			}
			else
			{
				lblUniverseFolder.Visible = lblUniverse.Visible = false;
			}
			if (!_data.Export.AllSelectedControllersSupportUniverses && _data.ActiveProfile.CreateUniverseFile)
				{
				lblUniverseFileWarning.Visible = true;
				lblUniverseFileWarning.Text = "Not all controllers selected for export support universes.\n" +
				                              "These controllers will not be included in the universes file.\n" +
				                              "Some manual FPP output configuration will be required.";
			}
			else
			{
				lblUniverseFileWarning.Visible = false;
			}
		}

		private void PopulateProfiles()
		{
			_profiles = new BindingList<ExportProfile>(_data.Profiles);
			comboConfigName.DataSource = new BindingSource { DataSource = _profiles };
			var index = _data.Profiles.FindIndex(x => x.Id == _data.ActiveProfile.Id);
			comboConfigName.SelectedIndex = index;
			if (index < 0)
			{
				comboConfigName.Text = _data.ActiveProfile.Name;
			}
		}

		public override void StageStart()
		{
			taskProgress.Visible = false;
			overallProgress.Visible = false;
			lblTaskProgress.Visible = false;
			lblOverallProgress.Visible = false;
			comboConfigName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
			comboConfigName.AutoCompleteSource = AutoCompleteSource.CustomSource;
			namesCollection = new AutoCompleteStringCollection();
			comboConfigName.AutoCompleteCustomSource = namesCollection;
			namesCollection.AddRange(_data.Profiles.Select(x => x.Name).ToArray());
			PopulateProfiles();
			ConfigureSummary();
		}

		public override async Task StageEnd()
		{
			if (chkSaveConfig.Checked)
			{
				SaveActiveConfig();
			}
			IProgress<ExportProgressStatus> progress = new Progress<ExportProgressStatus>(ReportProgress);
			taskProgress.Visible = true;
			overallProgress.Visible = true;
			lblTaskProgress.Visible = true;
			lblOverallProgress.Visible = true;
			await DoExport(progress);
			
			_data.ActiveProfile = null;
		}

		public override void StageCancelled()
		{
			_cancelled = true;
			_data.Export.Cancel();
		}

		private async void SaveActiveConfig()
		{
			if (comboConfigName.SelectedIndex >= 0)
			{
				//we are replacing an existing config.
				_data.ActiveProfile.Name = comboConfigName.Text;
				_data.ActiveProfile.Id = (comboConfigName.SelectedItem as ExportProfile).Id;
				_data.Profiles[comboConfigName.SelectedIndex] = _data.ActiveProfile;
			}
			else
			{
				//Save as new with text from combo as name
				_data.ActiveProfile.Name = comboConfigName.Text;
				_data.ActiveProfile.Id = Guid.NewGuid();
				_data.Profiles.Add(_data.ActiveProfile);
			}

			await VixenSystem.SaveModuleConfigAsync();
		}

		private async Task<bool> DoExport(IProgress<ExportProgressStatus> progress)
		{
			var updateIntervalHold = VixenSystem.DefaultUpdateInterval;
			VixenSystem.DefaultUpdateInterval = _data.ActiveProfile.Interval;
			var exportProgressStatus = new ExportProgressStatus();
			var overallProgressSteps = _data.ActiveProfile.SequenceFiles.Count * 2d; //There are basically 2 steps for each. Render and export.
			var overallProgressStep = 0;

			exportProgressStatus.OverallProgressMessage = "Overall Progress";
			progress.Report(exportProgressStatus);

			await Task.Run(async () =>
			{
				foreach (var sequenceFile in _data.ActiveProfile.SequenceFiles)
				{
					if (_cancelled)
					{
						break;
					}
					exportProgressStatus.TaskProgressMessage = string.Format("Loading {0}", Path.GetFileNameWithoutExtension(sequenceFile)); ;
					progress.Report(exportProgressStatus);

					//Load our sequence
					var sequence = SequenceService.Instance.Load(sequenceFile);
					exportProgressStatus.TaskProgressMessage = string.Format("Loading any media for {0}", sequence.Name);
					progress.Report(exportProgressStatus);
					//Load it's media
					LoadMedia(sequence);
					//Render it
					RenderSequence(sequence, progress);

					if (_cancelled)
					{
						break;
					}

					//Update over all progress with next step
					overallProgressStep++;
					exportProgressStatus.OverallProgressValue = (int)(overallProgressStep / overallProgressSteps * 100);
					exportProgressStatus.TaskProgressMessage = string.Format("Exporting {0}", sequence.Name);
					progress.Report(exportProgressStatus);

					//Begin export step.
					await Export(sequence, progress);

					overallProgressStep++;
					exportProgressStatus.OverallProgressValue = (int)(overallProgressStep / overallProgressSteps * 100);
					progress.Report(exportProgressStatus);
					
				}

				if (!_cancelled)
				{
					await CreateUniverseFile();
				}
				exportProgressStatus.TaskProgressMessage = "";
				exportProgressStatus.TaskProgressValue = 0;
				exportProgressStatus.OverallProgressMessage = "Completed";
				progress.Report(exportProgressStatus);

			});

			VixenSystem.DefaultUpdateInterval = updateIntervalHold;
			return true;
		}

		private async Task CreateUniverseFile()
		{
			if (_data.ActiveProfile.IsFalcon2xFormat)
			{
				var path = Path.Combine(_data.ActiveProfile.FalconOutputFolder, "config");
				if (!Directory.Exists(path))
				{
					CreateDirectory(path);
				}

				string fileName = Path.Combine(path, "co-universes.json");

				if (_data.ActiveProfile.BackupUniverseFile && File.Exists(fileName))
				{
					var now = DateTime.Now;
					var newFile = $"{fileName}_{now.Month}{now.Day}{now.Year}-{now.Hour}{now.Minute}{now.Second}";
					File.Move(fileName, newFile);
				}

				await _data.Export.Write2xUniverseFile(fileName);
			}
			
		}

		private bool RenderSequence(ISequence sequence, IProgress<ExportProgressStatus> progress)
		{
			double count = sequence.SequenceData.EffectData.Count();
			long index = 1;
			var p = new ExportProgressStatus(ExportProgressStatus.ProgressType.Task) {TaskProgressMessage = string.Format("Rendering {0}", sequence.Name)};
			foreach (var effectNode in sequence.SequenceData.EffectData.Cast<IEffectNode>())
			{
				RenderEffect(effectNode);
				p.TaskProgressValue = (int) (index / count * 100);
				progress.Report(p);
				index++;
			}
		
			return true;
		}

		private async Task Export(ISequence sequence, IProgress<ExportProgressStatus> progress)
		{

			IEnumerable<string> mediaFileNames =
			(from media in sequence.SequenceData.Media
				where media.GetType().ToString().Contains("Audio")
				where media.MediaFilePath.Length != 0
				select media.MediaFilePath);

			if (mediaFileNames.Any())
			{
				_data.Export.AudioFilename = mediaFileNames.First();
			}
			else
			{
				_data.Export.AudioFilename = String.Empty;
			}

			bool canOutput = true;
			if (_data.ActiveProfile.IncludeAudio && _data.Export.AudioFilename != string.Empty)
			{
				if (!Directory.Exists(_data.ActiveProfile.AudioOutputFolder))
				{
					canOutput = CreateDirectory(_data.ActiveProfile.AudioOutputFolder);
				}

				if (canOutput)
				{
					string audioOutputPath = Path.Combine(_data.ActiveProfile.AudioOutputFolder,
						_data.ActiveProfile.RenameAudio
							? _data.Export.FormatAudioFileName(sequence.Name)
							: Path.GetFileName(_data.Export.AudioFilename));
					File.Copy(_data.Export.AudioFilename, audioOutputPath, true);
				}
			}

			if (!Directory.Exists(_data.ActiveProfile.OutputFolder))
			{
				canOutput = CreateDirectory(_data.ActiveProfile.OutputFolder);
			}

			if (canOutput)
			{
				_data.Export.OutFileName = Path.Combine(_data.ActiveProfile.OutputFolder, sequence.Name + "." + _data.Export.ExportFileTypes[_data.ActiveProfile.Format]);
				await _data.Export.DoExport(sequence, _data.ActiveProfile.Format, _data.ActiveProfile.EnableCompression, progress, _data.ActiveProfile.RenameAudio);
			}
			
		}

		private bool CreateDirectory(string path)
		{
			bool success = false;
			try
			{
				Directory.CreateDirectory(path);
				success = true;
			}
			catch (Exception e)
			{
				Logging.Error(e, $"An error occurred trying to create the export directory structure {path}");
			}

			return success;
		}

		private void LoadMedia(ISequence sequence)
		{
			var sequenceMedia = sequence.GetAllMedia();
			if (sequenceMedia != null && sequenceMedia.Any())
				foreach (IMediaModuleInstance media in sequenceMedia)
				{
					media.LoadMedia(TimeSpan.Zero);
				}
		}

		private void RenderEffect(IEffectNode node)
		{
			if (node.Effect.IsDirty)
			{
				node.Effect.PreRender();
			}
		}

		private void ReportProgress(ExportProgressStatus progressStatus)
		{
			switch (progressStatus.StatusType)
			{
				case ExportProgressStatus.ProgressType.Both:
					UpdateTaskProgress(progressStatus);
					UpdateOverallProgress(progressStatus);
					break;
				case ExportProgressStatus.ProgressType.Task:
					UpdateTaskProgress(progressStatus);
					break;
				case ExportProgressStatus.ProgressType.Overall:
					UpdateOverallProgress(progressStatus);
					break;
			}
			
		}

		private void UpdateOverallProgress(ExportProgressStatus progressStatus)
		{
			overallProgress.Value = progressStatus.OverallProgressValue;
			lblOverallProgress.Text = progressStatus.OverallProgressMessage;
		}

		private void UpdateTaskProgress(ExportProgressStatus progressStatus)
		{
			taskProgress.Value = progressStatus.TaskProgressValue;
			lblTaskProgress.Text = progressStatus.TaskProgressMessage;
		}

		private void chkSaveConfig_CheckedChanged(object sender, EventArgs e)
		{
			comboConfigName.Visible = chkSaveConfig.Checked;
		}

		private void comboConfigName_TextChanged(object sender, EventArgs e)
		{
			//if(chkSaveConfig.Checked)
			//{
			//	_data.ActiveProfile.Name = comboConfigName.Text;
			//}
		}

		private void comboConfigName_TextUpdate(object sender, EventArgs e)
		{
			//Console.Out.WriteLine("TextUpdate");
		}

		private void mainLayoutPanel_Paint(object sender, PaintEventArgs e)
		{

		}

		private void lblUniverseFolder_Click(object sender, EventArgs e)
		{

		}
	}
}
