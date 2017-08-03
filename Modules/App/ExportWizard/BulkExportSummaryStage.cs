using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.Controls.Theme;
using Common.Controls.Wizard;
using Vixen.Export;
using Vixen.Module.Media;
using Vixen.Services;
using Vixen.Sys;

namespace VixenModules.App.ExportWizard
{
	public partial class BulkExportSummaryStage : WizardStage
	{
		private readonly BulkExportWizardData _data;
		private bool _cancelled;
		public BulkExportSummaryStage(BulkExportWizardData data)
		{
			_data = data;
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this, new List<Control>(new [] {txtSummary}));
			txtSummary.BackColor = ThemeColorTable.BackgroundColor;
			txtSummary.ForeColor = ThemeColorTable.ForeColor;
			taskProgress.Minimum = 0;
			taskProgress.Maximum = 100;
			overallProgress.Minimum = 0;
			overallProgress.Maximum = 100;
			//lblSummary.Font = ThemeUpdateControls.SizeAndStyleFont(lblSummary.Font, lblSummary.Font.Size + 4, FontStyle.Bold);
		}

		private void ConfigureSummary()
		{
			StringBuilder text = new StringBuilder();
				
			text.Append(string.Format(
				"Summary:  The export will process {0} sequence(s) and output them in the {1} "+
				"format on an interval of {2} ms into the the following folder: {3} \r\n"+
				"  If there are audio files associated with the sequences, they will{4}be exported.", 
				_data.SequenceFiles.Count, _data.Format, _data.Interval, _data.OutputFolder, _data.IncludeAudio?" ":" not "));

			if (_data.IncludeAudio)
			{
				text.Append(string.Format(" The audio will{0}be renamed when they are exported to the following folder: {1}",
					_data.RenameAudio ? " " : " not ", _data.AudioOutputFolder));
			}

			txtSummary.Text = text.ToString();
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
			await DoExport(progress);
		}

		public override void StageCancelled()
		{
			_cancelled = true;
			_data.Export.Cancel();
		}

		private async Task<bool> DoExport(IProgress<ExportProgressStatus> progress)
		{
			
			_data.ConfigureExport();

			var exportProgressStatus = new ExportProgressStatus();
			var overallProgressSteps = _data.SequenceFiles.Count * 2d; //There are basically 2 steps for each. Render and export.
			var overallProgressStep = 0;

			exportProgressStatus.OverallProgressMessage = "Overall Progress";
			progress.Report(exportProgressStatus);

			await Task.Run(async () =>
			{
				foreach (var sequenceFile in _data.SequenceFiles)
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

				exportProgressStatus.TaskProgressMessage = "";
				exportProgressStatus.TaskProgressValue = 0;
				exportProgressStatus.OverallProgressMessage = "Completed";
				progress.Report(exportProgressStatus);

			});
			return true;
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

			if (_data.IncludeAudio && _data.Export.AudioFilename != String.Empty)
			{
				string audioOutputPath = Path.Combine(_data.AudioOutputFolder, 
					_data.RenameAudio? sequence.Name + Path.GetExtension(_data.Export.AudioFilename): Path.GetFileName(_data.Export.AudioFilename));
				File.Copy(_data.Export.AudioFilename, audioOutputPath, true);
			}

			_data.Export.OutFileName = Path.Combine(_data.OutputFolder,sequence.Name+"."+_data.Export.ExportFileTypes[_data.Format]);
			await _data.Export.DoExport(sequence, _data.Format, progress);
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
	}
}
