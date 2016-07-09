using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
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
        private bool _doProgressUpdate;
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
	        textBox1.Font = SystemFonts.MessageBoxFont;
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

            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);

            _profile = new XMLProfileSettings();
        }
        #endregion

        #region Background Thread
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
           while (_doProgressUpdate)
            {
                Thread.Sleep(25); 
                switch (_currentState)
                {
                   
                    case ExportNotifyType.SAVING:
                    {
                        backgroundWorker1_Saving(sender, e);
                        break;
                    }

                    default:
                    {
                        break;
                    }
                }
            }
            UseWaitCursor = false;
			backgroundWorker1.ReportProgress(0);
        }

        private void backgroundWorker1_Saving(object sender, DoWorkEventArgs e)
        {
	        try
	        {
		        //currentTimeLabel.Text = string.Format("{0}%", _exportOps.SavePosition);
		        backgroundWorker1.ReportProgress((int) _exportOps.SavePosition);
	        }
	        catch (Exception ex)
	        {
		        Logging.Error("An error occured while updating the progress in the export.", ex);
	        }
            
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs args)
        {
	        try
	        {
		        exportProgressBar.Value = args.ProgressPercentage;
				currentTimeLabel.Text = string.Format("{0}%", args.ProgressPercentage);
	        }
	        catch (Exception e)
	        {
				Logging.Error("An error occured while updating the progress percentage in the export.", e);
	        }
            
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

            buttonStop.Enabled = false;
			networkListView.DragDrop += networkListView_DragDrop;
            //networkListView.Enabled = false;

			UpdateNetworkList();

        }

		void networkListView_DragDrop(object sender, DragEventArgs e)
		{
			
			int startChan = 1;
			int index = 0;
			foreach (ListViewItem item in networkListView.Items)
			{
				var info = item.Tag as ControllerExportInfo;
				if(info != null) info.Index = index;
				int channels = Convert.ToInt32(item.SubItems[1].Text);  //.Add(info.Channels.ToString());
				item.SubItems[2].Text = string.Format("Channels {0} to {1}", startChan, startChan + channels - 1);
				startChan += channels;
				index++;
			}

		}

        private void buttonStart_Click(object sender, EventArgs e)
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
			_doProgressUpdate = true;
			backgroundWorker1.RunWorkerAsync();

            _outFileName = saveDialog.FileName;
            _exportOps.OutFileName = _outFileName;
            _exportOps.UpdateInterval = Convert.ToInt32(resolutionComboBox.Text);
            _exportOps.DoExport(_sequence, outputFormatComboBox.SelectedItem.ToString());
            _exportOps.AudioFilename = _audioFileName;


           
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
            List<ControllerExportInfo> exportInfo = _exportOps.ControllerExportInfo;

            networkListView.Items.Clear();
            int startChan = 1;

            foreach (ControllerExportInfo info in exportInfo)
            {
                ListViewItem item = new ListViewItem(info.Name);
	            item.Tag = info;
                item.SubItems.Add(info.Channels.ToString());
                item.SubItems.Add(string.Format("Channels {0} to {1}", startChan, startChan + info.Channels - 1));

                networkListView.Items.Add(item);
                
                startChan += info.Channels;
            }

	        networkListView.ColumnAutoSize();
			networkListView.SetLastColumnWidth();
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
            _doProgressUpdate = isWorking;
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
            backgroundWorker1.DoWork -= backgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged -= backgroundWorker1_ProgressChanged;

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
        }

        private void resolutionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            ComboBox comboBox = (ComboBox)sender;
            _profile.PutSetting(XMLProfileSettings.SettingType.AppSettings, string.Format("{0}/ExportResolution", Name), (int)comboBox.SelectedIndex);
        }
	}
}
