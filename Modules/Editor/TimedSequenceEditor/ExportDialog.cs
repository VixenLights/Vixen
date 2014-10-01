using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.Resources.Properties;
using Vixen.Module.Timing;
using Vixen.Services;
using Vixen.Export;
using Vixen.Sys;
using Vixen.Cache.Sequence;
using Vixen.Sys.Output;
using Vixen.Module.Controller;

namespace VixenModules.Editor.TimedSequenceEditor
{

    public partial class ExportDialog : Form
    {
        private string _outFileName;
        private ISequence _sequence;
        private Export _exportOps;
        private bool _doProgressUpdate;
        private const int RENDER_TIME_DELTA = 250;
        private string _sequenceFileName = "";
        private string _audioFileName = "";
        private ExportNotifyType _currentState;
        private double _percentComplete = 0;
        private TimeSpan _curPos;
        private bool _cancelled;

        #region Contructor
        public ExportDialog(ISequence sequence)
        {
            InitializeComponent();

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
            if (mediaFileNames.Count() > 0)
            {
                _audioFileName = mediaFileNames.First();
            }

            exportProgressBar.Visible = false;
            currentTimeLabel.Visible = false;

            _cancelled = false;

            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);


        }
        #endregion

        #region Background Thread
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            TimeSpan renderCheck = new TimeSpan(0, 0, 0, 0, 250);
            while (_doProgressUpdate)
            {
                Thread.Sleep(25); 
                switch (_currentState)
                {
                    case ExportNotifyType.EXPORTING:
                    {
                        backgroundWorker1_Exporting(sender,e);
                        break;
                    }

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
            this.UseWaitCursor = false;
            backgroundWorker1.ReportProgress(0);
        }

        private void backgroundWorker1_Exporting(object sender, DoWorkEventArgs e)
        {
            _curPos = _exportOps.ExportPosition;
            currentTimeLabel.Text = string.Format("{0:D2}:{1:D2}.{2:D3}",
                                                    _curPos.Minutes,
                                                    _curPos.Seconds,
                                                    _curPos.Milliseconds);
            _percentComplete =
                (_curPos.TotalMilliseconds /
                (double)_sequence.Length.TotalMilliseconds) * 100;

            backgroundWorker1.ReportProgress((int)_percentComplete);    
        }

        private void backgroundWorker1_Saving(object sender, DoWorkEventArgs e)
        {
            try
            {
                currentTimeLabel.Text = string.Format("{0}%", _exportOps.SavePosition);
                backgroundWorker1.ReportProgress((int)_exportOps.SavePosition);
            }
            catch (Exception ex) { }
            
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs args)
        {
            try
            {
                exportProgressBar.Value = args.ProgressPercentage;
            }
            catch (Exception e) { }
            
        }
        #endregion

        #region Form Events

        private void ExportForm_Load(object sender, EventArgs e)
        {
            outputFormatComboBox.Items.Clear();
            outputFormatComboBox.Items.AddRange(_exportOps.FormatTypes);
            outputFormatComboBox.Sorted = true;

            outputFormatComboBox.SelectedIndex = 0;
            resolutionComboBox.SelectedIndex = 1;

            stopButton.Enabled = false;
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

        private void startButton_Click(object sender, EventArgs e)
        {
            this.UseWaitCursor = true;
            _cancelled = false;

            if (string.IsNullOrWhiteSpace(_sequenceFileName))
            {
                this.UseWaitCursor = false;
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
                this.UseWaitCursor = false;
                return;
            }

            _outFileName = saveDialog.FileName;
            _exportOps.OutFileName = _outFileName;
            _exportOps.UpdateInterval = Convert.ToInt32(resolutionComboBox.Text);
            _exportOps.DoExport(_sequence, outputFormatComboBox.SelectedItem.ToString());
            _exportOps.AudioFilename = _audioFileName;


            _doProgressUpdate = true;
            backgroundWorker1.RunWorkerAsync();

        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            _cancelled = true;
            _exportOps.Cancel();
        }

        private void ExportForm_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void stopButton_MouseEnter(object sender, EventArgs e)
        {
            this.UseWaitCursor = false;
        }

        private void stopButton_MouseLeave(object sender, EventArgs e)
        {
            this.UseWaitCursor = _doProgressUpdate;
        }

        #endregion

        #region Operational
        public void ShowDestinationMB()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(this.ShowDestinationMB));
                return;
            }

            startButton.Enabled = false;
            MessageBox.Show("File saved to " + _outFileName);
            startButton.Enabled = true;
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
        }

        private string setToolbarStatus(string progressText, bool showLiveProgress)
		{
			string prevVal = progressLabel.Text;
			progressLabel.Text = progressText;
			exportProgressBar.Visible = showLiveProgress;
			currentTimeLabel.Visible = showLiveProgress;

			return prevVal;
		}

        private string getAbbreviatedSequenceName(string prefix, string suffix)
        {
            return prefix  +
                Path.GetFileNameWithoutExtension(_sequenceFileName) +
                suffix;
        }

        private void setWorkingState(string message, bool isWorking)
        {
            setWorkingState(message, isWorking, isWorking);
        }

        private void setWorkingState(string message, bool isWorking, bool allowStop)
        {
            string newStatus = "";
            startButton.Enabled = !isWorking;
            stopButton.Enabled = allowStop;
            outputFormatComboBox.Enabled = !isWorking;
            resolutionComboBox.Enabled = !isWorking;
            _doProgressUpdate = isWorking;
            exportProgressBar.Visible = isWorking;
            currentTimeLabel.Visible = isWorking;

            if (isWorking)
            {
                newStatus =
                    getAbbreviatedSequenceName(message, "");
            }
            else
            {
                newStatus = message;
                backgroundWorker1.CancelAsync();
            }

            setToolbarStatus(newStatus, isWorking);
        }
        #endregion

        #region Events

        private void SequenceNotify(Vixen.Export.ExportNotifyType notifyType)
        {
            _currentState = notifyType;
            switch(notifyType)
            {
                case ExportNotifyType.NETSAVE:
                {
                    SequenceNetSave();
                    break;
                }

                case ExportNotifyType.LOADING:
                {
                    SequenceLoading();
                    break;
                }

                case ExportNotifyType.SAVING:
                {
                    SequenceSaving();
                    break;
                }

                case ExportNotifyType.EXPORTING:
                {
                    SequenceExporting();
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

        private void SequenceLoading()
        {
            //PlaceHolder Stub
        }

        private void SequenceNetSave()
        {
            //Placeholder Stub
        }

        private void SequenceExporting()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(SequenceExporting));
                return;
            }
            else
            {
                setWorkingState("Exporting: ", true);
            }

        }

        private void SequenceSaving()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(SequenceSaving));
                return;
            }
            else
            {
                setWorkingState("Saving: ", true, false);
            }
        }

        private void SequenceEnded()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(SequenceEnded));
                return;
            }
            else
            {
                if (_cancelled == true)
                {
                    setWorkingState("Export Canceled", false);
                }
                else
                {
                    setWorkingState("Export Complete", false);
                } 
            }
        }

        private void ExportDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            _exportOps.SequenceNotify -= SequenceNotify;
            backgroundWorker1.DoWork -= backgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged -= backgroundWorker1_ProgressChanged;

        }

        #endregion


    }
}
