using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Controls.Wizard;
using NLog;
using Vixen;
using Vixen.Export;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Extensions;
using VixenModules.App.TimedSequenceMapper.SequenceElementMapper.Models;
using VixenModules.App.TimedSequenceMapper.SequenceElementMapper.Services;
using VixenModules.Sequence.Timed;

namespace VixenModules.App.TimedSequenceMapper.SequencePackageImport
{
	public partial class SequencePackageImportSummaryStage : WizardStage
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
		private readonly ImportConfig _data;
		private bool _cancelled;
		
		public SequencePackageImportSummaryStage(ImportConfig data)
		{
			_data = data;
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			taskProgress.Minimum = 0;
			taskProgress.Maximum = 100;
			overallProgress.Minimum = 0;
			overallProgress.Maximum = 100;
			
		}

		private void ConfigureSummary()
		{
			lblSequenceCount.Text = _data.Sequences.Count(s => s.Value).ToString();
			lblOutputFolder.Text = SequenceService.SequenceDirectory;
			lblAudioOption.Text = @"Ask on duplicate name.";
		}

		
		public override void StageStart()
		{
			taskProgress.Visible = false;
			overallProgress.Visible = false;
			lblTaskProgress.Visible = false;
			lblOverallProgress.Visible = false;
			ConfigureSummary();
		}

		public override async Task StageEnd()
		{
			IProgress<ExportProgressStatus> progress = new Progress<ExportProgressStatus>(ReportProgress);
			taskProgress.Visible = true;
			overallProgress.Visible = true;
			lblTaskProgress.Visible = true;
			lblOverallProgress.Visible = true;
			await DoImport(progress);
			
			//_data.ActiveProfile = null;
		}

		public override void StageCancelled()
		{
			_cancelled = true;
		}

		private async Task<bool> DoImport(IProgress<ExportProgressStatus> progress)
		{
			var exportProgressStatus = new ExportProgressStatus();
			//Progress steps per sequence
			//1 Extract Sequence
			//2 Load Sequence
			//3 Remap Sequence
			//4 Save Sequence to sequence folder
			//5 Add media to Module Data folder
			//6 Add Audio to Media folder

			//Progress for Element Tree is one to create and one to archive
			var stepsPerSequence = 6d;
			var overallProgressSteps = (_data.Sequences.Count * stepsPerSequence);  
			var overallProgressStep = 0;

			exportProgressStatus.OverallProgressMessage = "Overall Progress";
			progress.Report(exportProgressStatus);

			await Task.Run(async () =>
			{
				ModelPersistenceService<ElementMap> ems = new ModelPersistenceService<ElementMap>();
				var map = await ems.LoadModelAsync(_data.MapFile);

				foreach (var sequenceFile in _data.Sequences.Where(s => s.Value).Select(x => x.Key))
				{
					if (_cancelled)
					{
						break;
					}

					//Progress step 1 Extract Sequence
					exportProgressStatus.TaskProgressValue = 0;
					exportProgressStatus.TaskProgressMessage = $"Extracting {sequenceFile} from package"; ;
					progress.Report(exportProgressStatus);

					//var n =  Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".tim";
					//var sequenceTemp = Path.Combine(Path.GetTempPath(), n);
					var sequenceDestination = Path.Combine(SequenceService.SequenceDirectory,
						sequenceFile + TimedSequence.Extension);
					if (File.Exists(sequenceDestination))
					{
						var overwrite = ShowSequenceExistsMessage(sequenceFile);
						if (!overwrite)
						{
							var target = GetFileName();
							if (!string.IsNullOrEmpty(target))
							{
								sequenceDestination = target;
							}
							else
							{
								continue;
							}
						}

					}

					ExtractSequence(_data.InputFile, sequenceFile, sequenceDestination);
					//End step 1

					//Progress step 2 Load Sequence
					exportProgressStatus.TaskProgressValue = (int)(1/stepsPerSequence * 100);
					overallProgressStep++;
					exportProgressStatus.OverallProgressValue = (int)(overallProgressStep / overallProgressSteps * 100);
					exportProgressStatus.TaskProgressMessage = $"Loading {sequenceFile}";
					progress.Report(exportProgressStatus);
					
					var sequence = await LoadSequence(sequenceDestination);

					//try to clean up any sequence migration files becasue they are not relevant anyway.
					var escapedFile = Regex.Escape(sequenceFile + TimedSequence.Extension);
					Regex filter = new Regex($@"^.*{escapedFile}\.\d*$");
					try
					{
						foreach (string f in Directory.EnumerateFiles(Path.GetDirectoryName(sequenceDestination), "*.*", SearchOption.TopDirectoryOnly)
							.Where(x => filter.IsMatch(x)))
						{
							File.Delete(f);
						}
					}
					catch (Exception e)
					{
						Logging.Error(e, "An error occured trying to remove sequence migration files.");
					}
					//End step 2

					//Progress step 3 Remap Sequence
					exportProgressStatus.TaskProgressValue = (int)(2/stepsPerSequence * 100);
					overallProgressStep++;
					exportProgressStatus.OverallProgressValue = (int)(overallProgressStep / overallProgressSteps * 100);
					exportProgressStatus.TaskProgressMessage = $"Remapping {sequenceFile}";
					progress.Report(exportProgressStatus);
					
					MapSequence(sequence, map);
					//End step 3

					exportProgressStatus.TaskProgressValue = (int)(3 / stepsPerSequence * 100);
					overallProgressStep++;
					exportProgressStatus.OverallProgressValue = (int)(overallProgressStep / overallProgressSteps * 100);
					exportProgressStatus.TaskProgressMessage = $"Saving mapped sequence {sequenceFile}";
					progress.Report(exportProgressStatus);
					
					//Progress step 4 Save Sequence to sequence folder
					
					await SaveSequence(sequence, Path.Combine(SequenceService.SequenceDirectory, sequenceDestination));
					//End step 4

					//Progress step 5 Add media to Module Data folder
					exportProgressStatus.TaskProgressValue = (int)(4 / stepsPerSequence * 100);
					overallProgressStep++;
					exportProgressStatus.OverallProgressValue = (int)(overallProgressStep / overallProgressSteps * 100);
					exportProgressStatus.TaskProgressMessage = $"Extracting module data for {sequenceFile}";
					progress.Report(exportProgressStatus);

					ExtractModuleData(_data.InputFile, sequenceFile);
					//End step 5

					//Progress step 6 Add Audio to Media folder
					exportProgressStatus.TaskProgressValue = (int)(5 / stepsPerSequence * 100);
					overallProgressStep++;
					exportProgressStatus.OverallProgressValue = (int)(overallProgressStep / overallProgressSteps * 100);
					exportProgressStatus.TaskProgressMessage = $"Extracting media/audio for {sequenceFile}";
					progress.Report(exportProgressStatus);

					ExtractMedia(_data.InputFile, sequenceFile);

					exportProgressStatus.TaskProgressValue = 100;
					overallProgressStep++;
					exportProgressStatus.OverallProgressValue = (int)(overallProgressStep / overallProgressSteps * 100);
					progress.Report(exportProgressStatus);
					
				}

				exportProgressStatus.TaskProgressMessage = "";
				exportProgressStatus.TaskProgressValue = 100;
				exportProgressStatus.OverallProgressValue = 100;
				exportProgressStatus.OverallProgressMessage = "Completed";
				progress.Report(exportProgressStatus);

			});
			return true;
		}

		private void MapSequence(ISequence sequence, ElementMap elementMap)
		{
			List<EffectNode> effectsToRemove = new List<EffectNode>();
			
			foreach (var unmappedEffect in sequence.SequenceData.EffectData.Cast<EffectNode>())
			{
				if (elementMap.GetBySourceId(unmappedEffect.Effect.TargetNodes.First().Id, out Guid targetId))
				{
					var node = VixenSystem.Nodes.GetElementNode(targetId);
					if (node != null)
					{
						unmappedEffect.Effect.TargetNodes = new[] { node };
					}
					else
					{
						effectsToRemove.Add(unmappedEffect);
					}
				}
				else
				{
					effectsToRemove.Add(unmappedEffect);
				}
			}
			
			sequence.SequenceData.EffectData.RemoveRangeData(effectsToRemove);
		}

		private async Task<ISequence> LoadSequence(string fileName)
		{
			return await Task.Run(() => SequenceService.Instance.Load(fileName));
		}

		private async Task SaveSequence(ISequence sequence, string fileName)
		{
			string extension = Path.GetExtension(fileName);

			// if the given extension isn't valid for this type, then keep the name intact and add an extension
			if (extension != sequence.FileExtension)
			{
				fileName = Path.GetFileNameWithoutExtension(fileName) + sequence.FileExtension;
				Logging.Info("Incorrect extension provided for timed sequence, appending one.");
			}
			await Task.Run(() => sequence.Save(fileName));
		}

		private bool ExtractSequence(string archivePath, string sequenceName, string destinationPath)
		{
			var success = false;
			try
			{
				using (ZipArchive archive = ZipFile.OpenRead(archivePath))
				{
					var entry = archive.Entries.First(x => x.FullName.EndsWith(sequenceName + ".tim"));
					entry.ExtractToFile(destinationPath, true);
				}
				success = true;
			}
			catch (Exception ex)
			{
				Logging.Error(ex, $"An error occurred extracting a sequence from the package. {sequenceName} to {destinationPath}");
				ShowErrorMessage($"An error occurred extracting a sequence from the package. {sequenceName}");
			}

			return success;
		}

		private bool ExtractModuleData(string archivePath, string sequenceName)
		{
			var success = false;
			var searchPath = Path.Combine(Path.Combine(Constants.PackageSequenceDataFolder,sequenceName), @"Module Data Files");

			try
			{
				ExtractToPath(archivePath, searchPath, Paths.ModuleDataFilesPath);
				success = true;
			}
			catch (Exception ex)
			{
				Logging.Error(ex, $"An error occurred extracting a sequence module files from the package. {sequenceName}");
				ShowErrorMessage($"An error occurred extracting a sequences module files from the package. {sequenceName}");
			}

			return success;
		}

		private bool ExtractMedia(string archivePath, string sequenceName)
		{
			var success = false;
			var searchPath = Path.Combine(Path.Combine(Constants.PackageSequenceDataFolder,sequenceName), @"Media");

			try
			{
				ExtractToPath(archivePath, searchPath, MediaService.MediaDirectory);
				success = true;
			}
			catch (Exception ex)
			{
				Logging.Error(ex, $"An error occurred extracting a sequence media files from the package. {sequenceName}");
				ShowErrorMessage($"An error occurred extracting a sequences media files from the package. {sequenceName}");
			}

			return success;
		}

		private static List<string> ExtractToPath(string archivePath, string searchPath, string destinationFolder)
		{
			var skipped = new List<string>();
			using (ZipArchive archive = ZipFile.OpenRead(archivePath))
			{
				var entries = archive.Entries.Where(x => x.FullName.StartsWith(searchPath));
				foreach (var file in entries)
				{
					var destinationPath = Path.Combine(destinationFolder, file.FullName.TrimStart(searchPath).TrimStart(Path.DirectorySeparatorChar));
					//if a same named file exists, skip overwriting it.
					if (!File.Exists(destinationPath))
					{
						var dir = Path.GetDirectoryName(destinationPath);
						if(!string.IsNullOrEmpty(dir))
						{
							Directory.CreateDirectory(dir);
							file.ExtractToFile(destinationPath, true);
						}
					}
					else
					{
						skipped.Add(file.FullName);
					}
				}
			}

			return skipped;
		}

		private void ShowErrorMessage(string message)
		{
			if (InvokeRequired)
			{
				Invoke(new Delegates.GenericVoidString(ShowErrorMessage),message);
			}
			else
			{
				MessageBoxForm mbf = new MessageBoxForm(message, "Error Importing Package", MessageBoxButtons.OK, SystemIcons.Error);
				mbf.ShowDialog(this);
			}
		}

		private delegate bool OkCancelDelegate(string val);

		private bool ShowSequenceExistsMessage(string sequenceName)
		{
			if (InvokeRequired)
			{
				return (bool)Invoke(new OkCancelDelegate(ShowSequenceExistsMessage), sequenceName);
			}
			else
			{
				MessageBoxForm mbf = new MessageBoxForm($"The sequence {sequenceName} already exists. Overwrite?", "Sequence exists.", MessageBoxButtons.YesNo, SystemIcons.Question);
				var result = mbf.ShowDialog(this);
				return result == DialogResult.OK;
			}
		}

		private string GetFileName()
		{
			if (InvokeRequired)
			{
				return (string) Invoke(new Delegates.GenericStringValue(GetFileName));
			}
			else
			{
				SaveFileDialog sfd = new SaveFileDialog();
				sfd.InitialDirectory = SequenceService.SequenceDirectory;
				sfd.OverwritePrompt = true;
				sfd.CheckPathExists = true;
				sfd.Filter = @"Timed Sequence (*.tim)|*.tim";

				var result = sfd.ShowDialog(this);
				if (result == DialogResult.OK)
				{
					return sfd.FileName;
				}

				return String.Empty;
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
	}
}
