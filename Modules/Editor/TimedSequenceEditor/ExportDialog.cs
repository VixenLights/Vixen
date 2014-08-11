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
using Vixen.Module.Timing;
using Vixen.Services;
using Vixen.Export;
using Vixen.Sys;
using Vixen.Sys.Output;
using Vixen.Module.Controller;

namespace VixenModules.Editor.TimedSequenceEditor
{

    public partial class ExportDialog : Form
    {
        private string _exportDir;
        private string _outFileName;
        private ISequence _sequence;
        private Export _exportOps;
        private ITiming _timing;
        private bool _doProgressUpdate;
        private const int RENDER_TIME_DELTA = 250;
        private string _sequenceFileName = "";

        public ExportDialog(ISequence sequence)
        {
            InitializeComponent();
            _exportDir = Path.Combine(Paths.DataRootPath, "Export");
            _exportOps = new Export();

            _sequence = sequence;
            _sequenceFileName = sequence.FilePath;

            exportProgressBar.Visible = false;
            currentTimeLabel.Visible = false;

            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);
        }

        public ISequence Sequence { get; set; }
        public EventHandler StartButtonHandler;
        public EventHandler StopButtonHandler;

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            double percentComplete = 0;
            TimeSpan renderCheck = new TimeSpan(0, 0, 0, 0, 250);
            if (_timing != null)
            {
                while (_doProgressUpdate)
                {
                    Thread.Sleep(25);
                    currentTimeLabel.Text = string.Format("{0:D2}:{1:D2}.{2:D3}",
                                                            _timing.Position.Minutes,
                                                            _timing.Position.Seconds,
                                                            _timing.Position.Milliseconds);

                    percentComplete =
                        (_timing.Position.TotalMilliseconds /
                        _exportOps.SequenceLength) * 100;

                    backgroundWorker1.ReportProgress((int)percentComplete);                    
                }
                this.UseWaitCursor = false;
                backgroundWorker1.ReportProgress(0);

                ShowDestinationMB();
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs args)
        {
            try
            {
                exportProgressBar.Value = args.ProgressPercentage;
            }
            catch (Exception e) { }
            
        }

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

        private void ExportForm_Load(object sender, EventArgs e)
        {
			outputFormatComboBox.Items.Clear();
            outputFormatComboBox.Items.AddRange(_exportOps.FormatTypes);

            outputFormatComboBox.SelectedIndex = 0;
            resolutionComboBox.SelectedIndex = 1;

            stopButton.Enabled = false;

            UpdateNetworkList();

        }

        private bool checkExportdir()
        {
            if (!Directory.Exists(_exportDir))
            {
                try
                {
                    Directory.CreateDirectory(_exportDir);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return true;
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

            checkExportdir();

            _outFileName = _exportDir +
                Path.DirectorySeparatorChar +
                Path.GetFileNameWithoutExtension(_sequenceFileName) + "." +
                _exportOps.ExportFileTypes[outputFormatComboBox.SelectedItem.ToString()];

			_exportOps.OutFileName = _outFileName;
			_exportOps.UpdateInterval = Convert.ToInt32(resolutionComboBox.Text);            
            setWorkingState(true);
            
            if (StartButtonHandler != null)
            {
                StartButtonHandler(this, null);
            }
//            _timing = _exportOps.SequenceTiming;
            
//            _exportOps.DoExport(Sequence);

            _doProgressUpdate = true;
//            backgroundWorker1.RunWorkerAsync();

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

        private void setWorkingState(bool isWorking)
        {
            string newStatus = "";
            startButton.Enabled = !isWorking;
            stopButton.Enabled = isWorking;
            //networkListView.Enabled = !isWorking;
			networkListView.Enabled = false;
            outputFormatComboBox.Enabled = !isWorking;
            resolutionComboBox.Enabled = !isWorking;
            _doProgressUpdate = isWorking;
            exportProgressBar.Visible = isWorking;
            currentTimeLabel.Visible = isWorking;

            if (isWorking)
            {
                newStatus =
                    getAbbreviatedSequenceName("Exporting: ", "");
            }
            else
            {
                _sequenceFileName = "";
                backgroundWorker1.CancelAsync();
            }

            setToolbarStatus(newStatus, isWorking);
        }

        private void context_SequenceEnded(object sender, EventArgs e)
        {
            setWorkingState(false);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            setWorkingState(false);
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

        public void SequenceCacheStarted(object sender, Vixen.Cache.Event.CacheStartedEventArgs e)
        {
            //Timing source position will indicate progression......
        }

        public void SequenceCacheEnded(object sender, Vixen.Cache.Event.CacheEventArgs e)
        {

            _exportOps.WriteControllerInfo(_sequence);
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => { this.Close(); }));
            }
            else
            {
                this.Close();
            }

        }

    }
}
