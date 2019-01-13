using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using NLog;
using Vixen.Export;
using Vixen.Sys;

namespace VixenModules.Editor.TimedSequenceEditor
{

    public partial class ExportDialog : BaseForm
    {
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
        private string _outFileName;
        private readonly ISequence _sequence;
        private readonly Export _exportOps;
        private readonly string _sequenceFileName;
        private readonly string _audioFileName;
        private ExportNotifyType _currentState;
	    private bool _cancelled;

        public int exportTypeDefault = 0;
        public int exportResolutionDefault = 1;

        private XMLProfileSettings _profile;

        #region Constructor
        public ExportDialog(ISequence sequence)
        {
            InitializeComponent();

			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this, new List<Control>(new []{textBox1}));
	        textBox1.BackColor = ThemeColorTable.BackgroundColor;
	        textBox1.ForeColor = ThemeColorTable.ForeColor;
	        textBox1.Font = ThemeUpdateControls.StandardFont;
	        textBox1.AutoSize = true;
		
            Icon = Resources.Icon_Vixen3;
            
            _sequence = sequence;
	        _exportOps = new Export();
            _exportOps.SequenceNotify += SequenceNotify;
            
            _sequenceFileName = _sequence.FilePath;

            IEnumerable<string> mediaFileNames =
                (from media in _sequence.SequenceData.Media
                 where media.GetType().ToString().Contains("Audio")
                 where media.MediaFilePath.Length != 0
                 select media.MediaFilePath);

            _audioFileName = "";
            if (mediaFileNames.Any())
            {
                _audioFileName = mediaFileNames.First();
            }

            exportProgressBar.Visible = false;
            currentTimeLabel.Visible = false;

            _cancelled = false;

            _profile = new XMLProfileSettings();
        }
        #endregion

        #region Form Events

        private void ExportForm_Load(object sender, EventArgs e)
        {
            outputFormatComboBox.Items.Clear();
            outputFormatComboBox.Items.AddRange(_exportOps.FormatTypes);
            outputFormatComboBox.Sorted = true;

            outputFormatComboBox.SelectedIndex = _profile.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ExportFormat", Name), exportTypeDefault);
            resolutionComboBox.SelectedIndex = _profile.GetSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ExportResolution", Name), exportResolutionDefault);
			chkGenerateControllerInfo.Checked = _profile.GetSetting(XMLProfileSettings.SettingType.AppSettings, $"{Name}/GenerateUniverse", false);
			radio2x.Checked = _profile.GetSetting(XMLProfileSettings.SettingType.AppSettings, $"{Name}/Universe2x", true);
	        radio1x.Checked = !radio2x.Checked;

			buttonStop.Enabled = false;
	        UpdateNetworkList();

			networkListView.DragDrop += networkListView_DragDrop;
			networkListView.ItemChecked += NetworkListView_ItemChecked;
            
        }

		private void NetworkListView_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			ReIndexControllerChannels();
		}

		void networkListView_DragDrop(object sender, DragEventArgs e)
		{
			ReIndexControllerChannels();
			//int startChan = 1;
			//int index = 0;
			//foreach (ListViewItem item in networkListView.Items)
			//{
			//	var info = item.Tag as Controller;
			//	if(info != null) info.Index = index;
			//	int channels = Convert.ToInt32(item.SubItems[1].Text);  //.Add(info.Channels.ToString());
			//	item.SubItems[2].Text = string.Format("Channels {0} to {1}", startChan, startChan + channels - 1);
			//	startChan += channels;
			//	index++;
			//}

		}

        private async void buttonStart_Click(object sender, EventArgs e)
        {
            
            _cancelled = false;

            if (string.IsNullOrWhiteSpace(_sequenceFileName))
            {
                UseWaitCursor = false;
                return;
            }

            string fileExt = _exportOps.ExportFileTypes[outputFormatComboBox.SelectedItem.ToString()];
            _outFileName = _exportOps.ExportDir +
                Path.DirectorySeparatorChar +
                Path.GetFileNameWithoutExtension(_sequenceFileName) + "." +
                fileExt;

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = _exportOps.ExportDir;
            saveDialog.FileName = Path.GetFileName(_outFileName);
            saveDialog.Filter = outputFormatComboBox.SelectedItem.ToString() +
                "|*." + fileExt + "|All Files(*.*)|*.*";
            DialogResult dr = saveDialog.ShowDialog();
            if (dr != DialogResult.OK)
            {
                UseWaitCursor = false;
                return;
            }
			UseWaitCursor = true;
			
            _outFileName = saveDialog.FileName;
            _exportOps.OutFileName = _outFileName;
            _exportOps.UpdateInterval = Convert.ToInt32(resolutionComboBox.Text);

			var progress = new Progress<ExportProgressStatus>(ReportExportProgress);
	        _exportOps.AudioFilename = _audioFileName;
			await _exportOps.DoExport(_sequence, outputFormatComboBox.SelectedItem.ToString(), progress);

	        if (outputFormatComboBox.SelectedItem.ToString().Contains("Falcon"))
	        {
		        if (!_exportOps.AllSelectedControllersSupportUniverses)
		        {
			        var messageBox = new MessageBoxForm("Some of the selected controllers do not support universes\n" +
			                                            "These controllers will not be included in the universes file.\n" +
			                                            "Some manual FPP output configuration will be required.",
														"Warning", MessageBoxButtons.OK, SystemIcons.Warning);
			        messageBox.ShowDialog();
		        }

		        if (chkGenerateControllerInfo.Enabled && chkGenerateControllerInfo.Checked)
		        {
			        if (radio2x.Checked)
			        {
				        var fileName = Path.GetDirectoryName(_exportOps.OutFileName) +
				                       Path.DirectorySeparatorChar +
				                       Path.GetFileNameWithoutExtension(_exportOps.OutFileName) +
				                       "-co-universes.json";
				        await _exportOps.Write2xUniverseFile(fileName);
					}
			        else
			        {
				        var fileName = Path.GetDirectoryName(_exportOps.OutFileName) +
				                       Path.DirectorySeparatorChar +
				                       Path.GetFileNameWithoutExtension(_exportOps.OutFileName) +
				                       "-universes";
				        await _exportOps.WriteUniverseFile(fileName);
					}
		        }
		        
			}
	        else if(chkGenerateControllerInfo.Checked)
	        {
		        _exportOps.WriteControllerInfo(_sequence);
			}
	      
			UseWaitCursor = false;
		}

	    private void ReportExportProgress(ExportProgressStatus exportProgressStatus)
	    {
			exportProgressBar.Value = exportProgressStatus.TaskProgressValue;
		    currentTimeLabel.Text = string.Format("{0}%", exportProgressStatus.TaskProgressValue);
		}

	    private void buttonCancel_Click(object sender, EventArgs e)
        {
            _cancelled = true;
			_exportOps.Cancel();
        }

        private void stopButton_MouseEnter(object sender, EventArgs e)
        {
            UseWaitCursor = false;
        }

        #endregion

        #region Operational
        public void ShowDestinationMB()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(ShowDestinationMB));
                return;
            }

            buttonStart.Enabled = false;
			//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
			MessageBoxForm.msgIcon = SystemIcons.Information; //this is used if you want to add a system icon to the message form.
			var messageBox = new MessageBoxForm("File saved to " + _outFileName, "File Saved?", false, false);
			messageBox.ShowDialog();
            buttonStart.Enabled = true;
        }

        private void UpdateNetworkList()
        {
            List<Controller> exportInfo = _exportOps.ControllerExportInfo;

            networkListView.Items.Clear();
            int startChan = 1;

			//foreach (Controller info in exportInfo)
			//{
			//    ListViewItem item = new ListViewItem(info.Name);
			// item.Tag = info;
			//    item.SubItems.Add(info.Channels.ToString());
			//    item.SubItems.Add(string.Format("Channels {0} to {1}", startChan, startChan + info.Channels - 1));

			//    networkListView.Items.Add(item);

			//    startChan += info.Channels;
			//}

			foreach (Controller info in exportInfo.OrderBy(x => x.Index))
			{
				ListViewItem item = new ListViewItem(info.Name);
				item.Tag = info;
				item.SubItems.Add(info.Channels.ToString());

				if (info.IsActive)
				{
					item.Checked = info.IsActive;
					item.SubItems.Add(startChan.ToString());
					item.SubItems.Add((startChan + info.Channels - 1).ToString());
					startChan += info.Channels;
				}
				else
				{
					item.SubItems.Add(String.Empty);
					item.SubItems.Add(String.Empty);
				}

				networkListView.Items.Add(item);
			}

			networkListView.ColumnAutoSize();
			networkListView.SetLastColumnWidth();
        }

	    private void ReIndexControllerChannels()
	    {
		    int startChan = 1;
		    int index = 0;
		    foreach (ListViewItem item in networkListView.Items)
		    {
			    var info = item.Tag as Controller;
			    if (info == null)
			    {
				    continue; // This should not happen!
			    }
			    info.Index = index;
			    info.IsActive = item.Checked;

			    if (info.IsActive)
			    {
				    int channels = Convert.ToInt32(item.SubItems[1].Text);
				    item.SubItems[2].Text = startChan.ToString();
				    item.SubItems[3].Text = (startChan + info.Channels - 1).ToString();
				    startChan += channels;
			    }
			    else
			    {
				    item.SubItems[2].Text = String.Empty;
				    item.SubItems[3].Text = String.Empty;
			    }

			    index++;
		    }
	    }

		private string SetToolbarStatus(string progressText, bool showLiveProgress)
		{
			string prevVal = progressLabel.Text;
			progressLabel.Text = progressText;
			exportProgressBar.Visible = showLiveProgress;
			currentTimeLabel.Visible = showLiveProgress;

			return prevVal;
		}

        private string GetAbbreviatedSequenceName(string prefix, string suffix)
        {
            return prefix  +
                Path.GetFileNameWithoutExtension(_sequenceFileName) +
                suffix;
        }

        private void SetWorkingState(string message, bool isWorking)
        {
            SetWorkingState(message, isWorking, isWorking);
        }

        private void SetWorkingState(string message, bool isWorking, bool allowStop)
        {
            string newStatus = "";
            buttonStart.Enabled = !isWorking;
            buttonStop.Enabled = allowStop;
            outputFormatComboBox.Enabled = !isWorking;
            resolutionComboBox.Enabled = !isWorking;
            exportProgressBar.Visible = isWorking;
            currentTimeLabel.Visible = isWorking;

            if (isWorking)
            {
                newStatus =
                    GetAbbreviatedSequenceName(message, "");
            }
            else
            {
                newStatus = message;
                backgroundWorker1.CancelAsync();
            }

            SetToolbarStatus(newStatus, isWorking);
        }
        #endregion

        #region Events

        private void SequenceNotify(ExportNotifyType notifyType)
        {
            _currentState = notifyType;
            switch(notifyType)
            {
                case ExportNotifyType.SAVING:
                {
                    SequenceSaving();
                    break;
                }

				case ExportNotifyType.COMPLETE:
                {
                    SequenceEnded();
                    break;
                }

                default:
                {
                    break;
                }
            }
        }

        private void SequenceSaving()
        {
            if (InvokeRequired)
            {
               BeginInvoke(new Action(SequenceSaving));
            }
            else
            {
                SetWorkingState("Saving: ", true, true);
            }
        }

        private void SequenceEnded()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(SequenceEnded));
            }
            else
            {
	            SetWorkingState(_cancelled ? "Export Canceled" : "Export Complete", false);
            }
        }

        private void ExportDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            _exportOps.SequenceNotify -= SequenceNotify;
        }

        #endregion

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImage;
			UseWaitCursor = _currentState == ExportNotifyType.SAVING;
		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}

		private void comboBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			ThemeComboBoxRenderer.DrawItem(sender, e);
		}

		private void networkListView_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
		{
			networkListView.SetLastColumnWidth();
		}

        private void outputFormatComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            _profile.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ExportFormat", Name), (int)comboBox.SelectedIndex);

			SetUniverseVersionEnabled();
        }

	    private void SetUniverseVersionEnabled()
	    {
		    if (chkGenerateControllerInfo.Checked)
		    {
			    if (outputFormatComboBox.SelectedItem.ToString().StartsWith("Falcon")) //Ewww...
			    {
				    radio2x.Enabled = radio1x.Enabled = true;
			    }
			    else
			    {
				    radio2x.Enabled = radio1x.Enabled = false;
			    }
			}
		    else
		    {
			    radio2x.Enabled = radio1x.Enabled = false;
			}
		    
	    }

	    private void resolutionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            ComboBox comboBox = (ComboBox)sender;
            _profile.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ExportResolution", Name), (int)comboBox.SelectedIndex);
        }

		private void chkGenerateControllerInfo_CheckedChanged(object sender, EventArgs e)
		{
			_profile.PutSetting(XMLProfileSettings.SettingType.AppSettings, $"{Name}/GenerateUniverse", chkGenerateControllerInfo.Checked);
			SetUniverseVersionEnabled();
		}

		private void radio2x_CheckedChanged(object sender, EventArgs e)
		{
			_profile.PutSetting(XMLProfileSettings.SettingType.AppSettings, $"{Name}/Universe2x", radio2x.Checked);
		}
	}
}
