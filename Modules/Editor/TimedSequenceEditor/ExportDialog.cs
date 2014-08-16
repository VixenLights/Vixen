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

        #region Contructor
        public ExportDialog(ISequence sequence)
        {
            InitializeComponent();

            Icon = Resources.Icon_Vixen3;
            
            _sequence = sequence;
            _exportOps = new Export();
            _exportOps.SequenceNotify += SequenceNotify;
            
            _sequenceFileName = _sequence.FilePath;

            exportProgressBar.Visible = false;
            currentTimeLabel.Visible = false;

            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);


        }
        #endregion

        #region Background Thread
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            double percentComplete = 0;
            TimeSpan curPos;
            TimeSpan renderCheck = new TimeSpan(0, 0, 0, 0, 250);
            while (_doProgressUpdate)
            {
                curPos = _exportOps.Position;
                Thread.Sleep(25);
                currentTimeLabel.Text = string.Format("{0:D2}:{1:D2}.{2:D3}",
                                                        curPos.Minutes,
                                                        curPos.Seconds,
                                                        curPos.Milliseconds);

                percentComplete =
                    (curPos.TotalMilliseconds /
                    (double)_sequence.Length.TotalMilliseconds) * 100;

                backgroundWorker1.ReportProgress((int)percentComplete);                    
            }
            this.UseWaitCursor = false;
            backgroundWorker1.ReportProgress(0);
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

            outputFormatComboBox.SelectedIndex = 0;
            resolutionComboBox.SelectedIndex = 1;

            stopButton.Enabled = false;

            UpdateNetworkList();

        }

        private void startButton_Click(object sender, EventArgs e)
        {
            this.UseWaitCursor = true;

            //Get Sequence Names
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

            setWorkingState("Exporting: ", true);
            _exportOps.DoExport(_sequence, outputFormatComboBox.SelectedItem.ToString());


            _doProgressUpdate = true;
            backgroundWorker1.RunWorkerAsync();

        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
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
            List<ControllerExportInfo> exportInfo = _exportOps.ControllerExportData;

            networkListView.Items.Clear();
            int startChan = 1;

            foreach (ControllerExportInfo info in exportInfo)
            {
                ListViewItem item = new ListViewItem(info.Name);
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
			networkListView.Enabled = false;
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
                backgroundWorker1.CancelAsync();
            }

            setToolbarStatus(newStatus, isWorking);
        }
        #endregion

        #region Events

        private void SequenceNotify(Vixen.Export.ExportNotifyType notifyType)
        {
            switch(notifyType)
            {
                case ExportNotifyType.NETSAVE:
                {
                    break;
                }

                case ExportNotifyType.LOADING:
                {
                    break;
                }

                case ExportNotifyType.SAVING:
                {
                    SequenceSaving();
                    break;
                }

                case ExportNotifyType.EXPORTING:
                {
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
                setWorkingState("", false);
            }
        }
        #endregion
    }
}
