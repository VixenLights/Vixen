using System.ComponentModel;
using System.Text;
using Common.Controls;
using Common.Controls.Wizard;
using NLog;
using Vixen;
using Vixen.Export;
using Vixen.Module.Media;
using Vixen.Services;
using Vixen.Sys;
using VixenModules.App.FPPClient.Client;

namespace VixenModules.App.ExportWizard
{
	public partial class BulkExportSummaryStage : WizardStage
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
		private readonly BulkExportWizardData _data;
		private bool _cancelled;
		private bool _uploadFailed;
		private CancellationTokenSource _directUploadCancellation;
		private AutoCompleteStringCollection _namesCollection = new();
		private BindingList<ExportProfile> _profiles;

		public BulkExportSummaryStage(BulkExportWizardData data)
		{
			_data = data;
			InitializeComponent();
			lblUniverseFileWarning.MaximumSize = new System.Drawing.Size(
				mainLayoutPanel.ClientSize.Width - lblUniverseFileWarning.Left - mainLayoutPanel.Padding.Right, 0);
			mainLayoutPanel.SizeChanged += (_, _) => lblUniverseFileWarning.MaximumSize = new System.Drawing.Size(
				mainLayoutPanel.ClientSize.Width - lblUniverseFileWarning.Left - mainLayoutPanel.Padding.Right, 0);
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
			lblTimingValue.Text = $@"{_data.ActiveProfile.Interval} ms";
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
				lblUniverseFileWarning.Text = new StringBuilder()
					.Append("Not all controllers selected for export support universes.\n")
					.Append("These controllers will not be included in the universes file.\n")
					.Append("Some manual FPP output configuration will be required.")
					.ToString();
			}
			else
			{
				lblUniverseFileWarning.Visible = false;
			}

			// FPP device info — hidden by default; shown only when Direct Upload is active
			lblFppInfo.Text = "FPP Device Info";
			lblFppInfo.Visible = false;
			lblFppHostName.Visible = lblFppHostNameValue.Visible = false;
			lblFppDescription.Visible = lblFppDescriptionValue.Visible = false;
			lblFppPlatform.Visible = lblFppPlatformValue.Visible = false;
			lblFppVariant.Visible = lblFppVariantValue.Visible = false;
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

		public override async void StageStart()
		{
			try
			{
				_cancelled = false;
				_uploadFailed = false;
				taskProgress.Visible = false;
				overallProgress.Visible = false;
				lblTaskProgress.Visible = false;
				lblOverallProgress.Visible = false;
				comboConfigName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
				comboConfigName.AutoCompleteSource = AutoCompleteSource.CustomSource;
				_namesCollection = new AutoCompleteStringCollection();
				comboConfigName.AutoCompleteCustomSource = _namesCollection;
				_namesCollection.AddRange(_data.Profiles.Select(x => x.Name).ToArray());
				PopulateProfiles();
				ConfigureSummary();

				if (_data.ActiveProfile.IsFalcon2xFormat && _data.ActiveProfile.FppDirectUpload)
				{
					await PopulateFppInfoAsync();
				}
			}
			catch (Exception e)
			{
				Logging.Error(e, $"An error occured starting the summary stage");
			}
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
			_directUploadCancellation?.Cancel();
			_data.Export.Cancel();
		}

		private async void SaveActiveConfig()
		{
			try
			{
				if (comboConfigName.SelectedIndex >= 0)
				{
					//we are replacing an existing config.
					_data.ActiveProfile.Name = comboConfigName.Text;
					_data.ActiveProfile.Id = (comboConfigName.SelectedItem as ExportProfile)!.Id;
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
			catch (Exception e)
			{
				Logging.Error(e, $"An error occured saving the active config");
			}
		}

		private async Task<bool> DoExport(IProgress<ExportProgressStatus> progress)
		{
			var updateIntervalHold = VixenSystem.DefaultUpdateInterval;
			IFppClient directClient = null;
			FppDirectUploadService directService = null;
			var zipSequences = new List<EspPixelStickSequenceFile>();
			try
			{
				if (_data.ActiveProfile.FppDirectUpload && _data.ActiveProfile.IsFalcon2xFormat)
				{
					_directUploadCancellation = new CancellationTokenSource();
					var factory = new FppClientFactory();
					directClient = factory.Create(new FppClientOptions
					{
						BaseUrl = $"http://{_data.ActiveProfile.FppHostAddress}/"
					});
					directService = await FppDirectUploadService.DetectAsync(
						directClient, _directUploadCancellation.Token);
					if (directService.IsEspPixelStick)
					{
						UpdateEspPixelStickSummary(directService.SupportsZipArchives);
					}
				}

				VixenSystem.DefaultUpdateInterval = _data.ActiveProfile.Interval;
			var exportProgressStatus = new ExportProgressStatus();
			var sequenceCount = _data.ActiveProfile.SequenceFiles.Count;
			var useZipArchives = directService?.SupportsZipArchives == true;
			var plannedZipCount = (sequenceCount + EspPixelStickZipBatchUploader.BatchSize - 1) /
				EspPixelStickZipBatchUploader.BatchSize;
			var overallProgressSteps = Math.Max(1d, sequenceCount * 2d +
				(useZipArchives ? Math.Ceiling(sequenceCount / (double)EspPixelStickZipBatchUploader.BatchSize) * 2 +
					(sequenceCount > 0 ? 1 : 0) : 0));
			var overallProgressStep = 0;

			if (useZipArchives)
			{
				var duplicateName = _data.ActiveProfile.SequenceFiles
					.Select(file => Path.GetFileNameWithoutExtension(file) + ".fseq")
					.GroupBy(name => name, StringComparer.OrdinalIgnoreCase)
					.FirstOrDefault(group => group.Count() > 1)?.Key;
				if (duplicateName != null)
				{
					throw new InvalidOperationException($"Duplicate sequence filename '{duplicateName}' cannot be uploaded in a ZIP archive.");
				}
			}

			exportProgressStatus.OverallProgressMessage = "Overall Progress";
			progress.Report(exportProgressStatus);

			await Task.Run(async () =>
			{
				var zipBatchCount = 0;
				async Task FlushZipBatchAsync()
				{
					var batch = zipSequences.ToArray();
					var part = ++zipBatchCount;
					try
					{
						await EspPixelStickZipBatchUploader.UploadBatchAsync(batch, part, plannedZipCount,
							(path, fileName, ct) => directService.UploadArchiveFileAsync(path, fileName, ct), progress,
							() =>
							{
								overallProgressStep++;
								progress.Report(new ExportProgressStatus(ExportProgressStatus.ProgressType.Overall)
								{
									OverallProgressValue = (int)(overallProgressStep / overallProgressSteps * 100),
									OverallProgressMessage = "Overall Progress"
								});
							}, _directUploadCancellation.Token).ConfigureAwait(false);
					}
					catch (OperationCanceledException) when (_cancelled)
					{
					}
					catch (Exception ex)
					{
						_cancelled = true;
						_uploadFailed = true;
						Logging.Error(ex, "ZIP batch {Part} upload to '{Host}' failed", part,
							_data.ActiveProfile.FppHostAddress);
						ShowDirectUploadError(ex.Message);
					}
					finally
					{
						foreach (var sequenceFile in batch)
						{
							if (File.Exists(sequenceFile.Path)) File.Delete(sequenceFile.Path);
						}
						zipSequences.Clear();
					}
				}

				foreach (var sequenceFile in _data.ActiveProfile.SequenceFiles)
				{
					if (_cancelled)
					{
						break;
					}
					exportProgressStatus.TaskProgressMessage =
						$"Loading {Path.GetFileNameWithoutExtension(sequenceFile)}";
					progress.Report(exportProgressStatus);

					ISequence sequence = null;
					try
					{
						//Load our sequence
						sequence = SequenceService.Instance.Load(sequenceFile);
					}
					catch (Exception ex)
					{
						Logging.Error(ex, $"An error occured loading the sequence {sequenceFile}");
					}

					if (sequence == null)
					{
						var result = ShowSequenceLoadError(sequenceFile);
						if (result)
						{
							overallProgressStep += 2;
							exportProgressStatus.OverallProgressValue = (int)(overallProgressStep / overallProgressSteps * 100);
							progress.Report(exportProgressStatus);
							continue;
						}

						_cancelled = true;
						break;
					}
					
					exportProgressStatus.TaskProgressMessage = $"Loading any media for {sequence.Name}";
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
					exportProgressStatus.TaskProgressMessage = $"Exporting {sequence.Name}";
					progress.Report(exportProgressStatus);

					//Begin export step.
					await Export(sequence, progress, directService, zipSequences);

					overallProgressStep++;
					exportProgressStatus.OverallProgressValue = (int)(overallProgressStep / overallProgressSteps * 100);
					progress.Report(exportProgressStatus);
					if (useZipArchives && !_cancelled && !_uploadFailed &&
						zipSequences.Count == EspPixelStickZipBatchUploader.BatchSize)
					{
						await FlushZipBatchAsync();
					}
					
				}

				if (useZipArchives)
				{
					if (!_cancelled && !_uploadFailed && zipSequences.Count > 0)
					{
						await FlushZipBatchAsync();
					}

					if (!_cancelled && !_uploadFailed && zipBatchCount > 0)
					{
						var rebootService = directService ??
							throw new InvalidOperationException("The ESPixelStick upload service is unavailable.");
						overallProgressSteps = Math.Max(1d, overallProgressStep + 1d);
						try
						{
							await EspPixelStickZipBatchUploader.RequestRebootAsync(
								rebootService.RebootEspPixelStickAsync, progress,
								() =>
								{
									overallProgressStep++;
									progress.Report(new ExportProgressStatus(ExportProgressStatus.ProgressType.Overall)
									{
										OverallProgressValue = (int)(overallProgressStep / overallProgressSteps * 100),
										OverallProgressMessage = "Overall Progress"
									});
								}, _directUploadCancellation.Token).ConfigureAwait(false);
						}
						catch (OperationCanceledException) when (_cancelled)
						{
						}
						catch (Exception ex)
						{
							_cancelled = true;
							_uploadFailed = true;
							Logging.Error(ex, "ESPixelStick reboot request to '{Host}' failed",
								_data.ActiveProfile.FppHostAddress);
							ShowDirectUploadError(ex.Message);
						}
					}
				}
				else if (!_cancelled)
				{
					await CreateUniverseFile(directService);
				}

				var finalMessage = _uploadFailed ? "Failed" : _cancelled ? "Cancelled" : "Completed";
				if (useZipArchives)
				{
					var finalOverallValue = _cancelled || _uploadFailed
						? (int)(overallProgressStep / overallProgressSteps * 100)
						: 100;
					progress.Report(new ExportProgressStatus(ExportProgressStatus.ProgressType.Overall)
					{
						OverallProgressValue = finalOverallValue,
						OverallProgressMessage = finalMessage
					});
				}
				else
				{
					exportProgressStatus.TaskProgressMessage = "";
					exportProgressStatus.TaskProgressValue = 0;
					exportProgressStatus.OverallProgressMessage = finalMessage;
					progress.Report(exportProgressStatus);
				}

			});

				return !_cancelled;
			}
			catch (OperationCanceledException) when (_cancelled)
			{
				return false;
			}
			catch (Exception ex)
			{
				_cancelled = true;
				_uploadFailed = true;
				Logging.Error(ex, "Export for '{Host}' failed", _data.ActiveProfile.FppHostAddress);
				if (!_data.ActiveProfile.FppDirectUpload || !_data.ActiveProfile.IsFalcon2xFormat) throw;
				ShowDirectUploadError(ex.Message);
				return false;
			}
			finally
			{
				foreach (var sequenceFile in zipSequences)
				{
					if (File.Exists(sequenceFile.Path)) File.Delete(sequenceFile.Path);
				}
				if (directClient != null) await directClient.DisposeAsync();
				_directUploadCancellation?.Dispose();
				_directUploadCancellation = null;
				VixenSystem.DefaultUpdateInterval = updateIntervalHold;
			}
		}

		private bool ShowSequenceLoadError(string sequenceFile)
		{
			if (InvokeRequired)
			{
				return (bool)Invoke(new Delegates.GenericBoolString(ShowSequenceLoadError), sequenceFile);
			}
			else
			{
				var msgBox = new MessageBoxForm(
					$"An error occurred opening the sequence {sequenceFile}. \nDo you wish to continue with the export of the remaining ones?",
					"Error Loading Sequence", MessageBoxButtons.YesNo, SystemIcons.Error);
				var result = msgBox.ShowDialog(this);
				if (result == DialogResult.OK)
				{
					return true;
				}

				return false;
			}
			
		}

		private async Task CreateUniverseFile(FppDirectUploadService service)
		{
			if (!_data.ActiveProfile.IsFalcon2xFormat) return;

			if (_data.ActiveProfile.FppDirectUpload)
			{
				if (service.SupportsFppExtras) await CreateUniverseFileDirect(service);
			}
			else
			{
				await CreateUniverseFileToPath();
			}
		}

		/// <summary>Original file-path universe file code path — unchanged behaviour.</summary>
		private async Task CreateUniverseFileToPath()
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

		/// <summary>
		/// Direct-upload universe file code path: backs up the existing remote file (rename),
		/// writes the new file to a temp path, uploads it, then deletes the temp file.
		/// </summary>
		private async Task CreateUniverseFileDirect(FppDirectUploadService svc)
		{
			if (!_data.ActiveProfile.CreateUniverseFile && !_data.ActiveProfile.BackupUniverseFile)
				return;

			try
			{
				if (_data.ActiveProfile.BackupUniverseFile)
				{
					var now = DateTime.Now;
					var backupName = $"co-universes.json_{now.Month}{now.Day}{now.Year}"
					               + $"-{now.Hour}{now.Minute}{now.Second}";
					await svc.BackupUniverseFileAsync(backupName, _directUploadCancellation.Token)
						.ConfigureAwait(false);
				}

				if (_data.ActiveProfile.CreateUniverseFile)
				{
					var tempUniverse = Path.Combine(Path.GetTempPath(),
						Path.GetRandomFileName() + ".json");
					try
					{
						await _data.Export.Write2xUniverseFile(tempUniverse);
						await svc.UploadUniverseFileAsync(tempUniverse, _directUploadCancellation.Token)
							.ConfigureAwait(false);
						await svc.RestartFppdAsync(ct: _directUploadCancellation.Token).ConfigureAwait(false);
					}
					finally
					{
						if (File.Exists(tempUniverse)) File.Delete(tempUniverse);
					}
				}
			}
			catch (OperationCanceledException) when (_cancelled)
			{
			}
			catch (Exception ex)
			{
				_cancelled = true;
				_uploadFailed = true;
				Logging.Error(ex, "Direct upload of universe file to '{Host}' failed",
					_data.ActiveProfile.FppHostAddress);
				ShowDirectUploadError(ex.Message);
			}
		}

		private bool RenderSequence(ISequence sequence, IProgress<ExportProgressStatus> progress)
		{
			double count = sequence.SequenceData.EffectData.Count();
			long index = 1;
			var p = new ExportProgressStatus(ExportProgressStatus.ProgressType.Task) {TaskProgressMessage =
				$"Rendering {sequence.Name}"
			};
			foreach (var effectNode in sequence.SequenceData.EffectData.Cast<IEffectNode>())
			{
				RenderEffect(effectNode);
				p.TaskProgressValue = (int) (index / count * 100);
				progress.Report(p);
				index++;
			}
		
			return true;
		}

		private async Task Export(ISequence sequence, IProgress<ExportProgressStatus> progress,
			FppDirectUploadService directService, List<EspPixelStickSequenceFile> zipSequences)
		{
			// Resolve audio filename for this sequence regardless of export mode
			IEnumerable<string> mediaFileNames =
			(from media in sequence.SequenceData.Media
				where media.GetType().ToString().Contains("Audio")
				where media.MediaFilePath.Length != 0
				select media.MediaFilePath);

			_data.Export.AudioFilename = mediaFileNames.FirstOrDefault(string.Empty);
			
			if (_data.ActiveProfile.FppDirectUpload && _data.ActiveProfile.IsFalcon2xFormat)
			{
				await ExportDirect(sequence, progress, directService, zipSequences);
			}
			else
			{
				await ExportToFilePath(sequence, progress);
			}
		}

		/// <summary>Original file-path export code path — unchanged behaviour.</summary>
		private async Task ExportToFilePath(ISequence sequence, IProgress<ExportProgressStatus> progress)
		{
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
						(_data.ActiveProfile.RenameAudio
							? _data.Export.FormatAudioFileName(sequence.Name)
							: Path.GetFileName(_data.Export.AudioFilename)) ?? string.Empty);
					if (!string.IsNullOrEmpty(_data.Export.AudioFilename))
					{
						File.Copy(_data.Export.AudioFilename, audioOutputPath, true);
					}
				}
			}

			if (!Directory.Exists(_data.ActiveProfile.OutputFolder))
			{
				canOutput = CreateDirectory(_data.ActiveProfile.OutputFolder);
			}

			if (canOutput)
			{
				_data.Export.OutFileName = Path.Combine(_data.ActiveProfile.OutputFolder,
					sequence.Name + "." + _data.Export.ExportFileTypes[_data.ActiveProfile.Format]);
				await _data.Export.DoExport(sequence, _data.ActiveProfile.Format,
					_data.ActiveProfile.EnableCompression, progress, _data.ActiveProfile.RenameAudio);
			}
		}

		/// <summary>
		/// Direct-upload export code path: writes fseq/audio to temp files then pushes them
		/// to the FPP device via <see cref="FppDirectUploadService"/>.
		/// </summary>
		private async Task ExportDirect(ISequence sequence, IProgress<ExportProgressStatus> progress,
			FppDirectUploadService svc, List<EspPixelStickSequenceFile> zipSequences)
		{

			// Export fseq to a temp file, upload it, then delete the temp file.
			var tempFseq = Path.Combine(Path.GetTempPath(),
				Path.GetRandomFileName() + "." + _data.Export.ExportFileTypes[_data.ActiveProfile.Format]);
			var retainForArchive = false;
			try
			{
				_data.Export.OutFileName = tempFseq;
				await _data.Export.DoExport(sequence, _data.ActiveProfile.Format,
					_data.ActiveProfile.EnableCompression, progress, _data.ActiveProfile.RenameAudio).ConfigureAwait(false);

				var fseqFileName = sequence.Name + "."
					+ _data.Export.ExportFileTypes[_data.ActiveProfile.Format];
				if (svc.SupportsZipArchives)
				{
					zipSequences.Add(new EspPixelStickSequenceFile(tempFseq, fseqFileName));
					retainForArchive = true;
				}
				else
				{
					await svc.UploadSequenceFileAsync(tempFseq, fseqFileName, progress,
						_directUploadCancellation.Token).ConfigureAwait(false);
				}
			}
			catch (OperationCanceledException) when (_cancelled)
			{
				return;
			}
			catch (Exception ex)
			{
				_cancelled = true;
				_uploadFailed = true;
				Logging.Error(ex, "Direct upload of sequence '{Name}' failed", sequence.Name);
				ShowDirectUploadError(ex.Message);
			}
			finally
			{
				if (!retainForArchive && File.Exists(tempFseq)) File.Delete(tempFseq);
			}

			// Upload audio if included.
			if (svc.SupportsFppExtras && !_cancelled && _data.ActiveProfile.IncludeAudio
				&& !string.IsNullOrEmpty(_data.Export.AudioFilename))
			{
				try
				{
					var audioFileName = _data.ActiveProfile.RenameAudio
						? _data.Export.FormatAudioFileName(sequence.Name)
						: Path.GetFileName(_data.Export.AudioFilename);
					await svc.UploadAudioFileAsync(_data.Export.AudioFilename, audioFileName, progress,
						_directUploadCancellation.Token).ConfigureAwait(false);
				}
				catch (OperationCanceledException) when (_cancelled)
				{
				}
				catch (Exception ex)
				{
					_cancelled = true;
					_uploadFailed = true;
					Logging.Error(ex, "Direct upload of audio for '{Name}' failed", sequence.Name);
					ShowDirectUploadError(ex.Message);
				}
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
			if (sequenceMedia != null)
			{
				foreach (IMediaModuleInstance media in sequenceMedia)
				{
					media.LoadMedia(TimeSpan.Zero);
				}
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

		private void ShowDirectUploadError(string message)
		{
			if (InvokeRequired)
			{
				Invoke(new Action<string>(ShowDirectUploadError), message);
				return;
			}

			var msgBox = new MessageBoxForm(
				$"FPP direct upload failed:\n{message}",
				"FPP Upload Error", MessageBoxButtons.OK, SystemIcons.Error);
			msgBox.ShowDialog(this);
		}

		private void UpdateEspPixelStickSummary(bool supportsZip)
		{
			if (InvokeRequired)
			{
				Invoke(new Action<bool>(UpdateEspPixelStickSummary), supportsZip);
				return;
			}

			lblAudioOption.Text = "Not uploaded to ESPixelStick.";
			lblAudioOutputFolder.Visible = lblAudioDestination.Visible = false;
			lblUniverseFolder.Visible = lblUniverse.Visible =
				_data.ActiveProfile.CreateUniverseFile || _data.ActiveProfile.BackupUniverseFile;
			lblUniverseFolder.Text = "Not uploaded to ESPixelStick.";
			var route = supportsZip
				? "FSEQ sequences will be sent in ZIP archives of up to five files, followed by one reboot request."
				: "FSEQ sequences will be uploaded individually; no reboot is requested.";
			lblUniverseFileWarning.Text = $"ESPixelStick direct upload sends FSEQ sequences only; audio and universe settings are not uploaded. {route}";
			lblUniverseFileWarning.Visible = true;
		}

		private async Task PopulateFppInfoAsync()
		{
			var host = _data.ActiveProfile.FppHostAddress;
			if (string.IsNullOrWhiteSpace(host)) return;

			try
			{
				var factory = new FppClientFactory();
				await using var client = factory.Create(new FppClientOptions { BaseUrl = $"http://{host}/" });
				// ConfigureAwait(true) keeps the continuation on the UI thread so label
				// assignments do not require Invoke.
				var info = await client.GetSystemInfoAsync().ConfigureAwait(true);

				if (IsDisposed || !_data.ActiveProfile.FppDirectUpload
					|| !string.Equals(host, _data.ActiveProfile.FppHostAddress, StringComparison.Ordinal)) return;

				if (string.Equals(info.Platform, "ESPixelStick", StringComparison.Ordinal))
				{
					UpdateEspPixelStickSummary(info.Zip);
				}

				lblFppHostNameValue.Text = info.HostName;
				lblFppDescriptionValue.Text = info.HostDescription;
				lblFppPlatformValue.Text = info.Platform;
				lblFppVariantValue.Text = info.Variant;

				lblFppInfo.Visible = true;
				lblFppHostName.Visible = lblFppHostNameValue.Visible = true;
				lblFppDescription.Visible = lblFppDescriptionValue.Visible = true;
				lblFppPlatform.Visible = lblFppPlatformValue.Visible = true;
				lblFppVariant.Visible = lblFppVariantValue.Visible = true;
			}
			catch (Exception ex)
			{
				Logging.Warn(ex, "Could not retrieve FPP system info for host '{Host}'", host);
				if (!IsDisposed)
				{
					lblFppInfo.Text = @"FPP Device Info (unavailable — check host address)";
					lblFppInfo.Visible = true;
				}
			}
		}
	}
}
