using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;
using Vixen.Interface;
using CommandStandard;
using Vixen.Common;
using Vixen.Hardware;

namespace TestClient {
    public partial class NotARealEditor : Form, IEditor {
        private TimedSequence _sequence;
        private ExecutionContext _context = null;

        public NotARealEditor() {
            InitializeComponent();
            // Get all available commands.
            // What can be assumed is that there will be platforms, categories, commands.
            // What cannot be assumed is the presence of any particular instances of
            // any of those, except the custom command category for every platform.
            List<CommandSignature> commands = new List<CommandSignature>();
            //commands.AddRange(ApplicationServices.Standard.GetCategoryCommands(Standard.Lighting.Value, Standard.Lighting.Monochrome.Value));
            //commands.AddRange(ApplicationServices.Standard.GetPlatformCustomCommands(Standard.Lighting.Value));
            commands.AddRange(Standard.GetCommandSignatures(Standard.Lighting.Value, Standard.Lighting.Monochrome.Value));
            commands.AddRange(Standard.GetCommandSignatures(Standard.Media.Value, Standard.Media.Audio.Value));
			//*** This won't do a damn thing!  It's the application that has this.
            commands.AddRange(Standard.GetCommandSignatures(Standard.Lighting.Value, Standard.CustomCategory));
            comboBoxCommands.DisplayMember = "Identifier";
            comboBoxCommands.ValueMember = "Identifier";
            comboBoxCommands.DataSource = commands;
            List<OutputController> controllers = new List<OutputController>(OutputController.GetAll());
            comboBoxExecutionController.DisplayMember = "Name";
            comboBoxExecutionController.ValueMember = "Id";
            comboBoxExecutionController.DataSource = controllers;
            // Need to get a "none" wedged in there.
            List<KeyValuePair<Guid, string>> timingSources = new List<KeyValuePair<Guid, string>>();
            //timingSources.Add(new KeyValuePair<Guid,string>(Guid.Empty, "(none)"));
            timingSources.AddRange(ApplicationServices.GetTimingSources().ToArray());
            comboBoxTimingSource.DisplayMember = "Value"; // string - Name
            comboBoxTimingSource.ValueMember = "Key"; // Guid - Type id
            comboBoxTimingSource.DataSource = timingSources;

			EditorValues = new Dictionary<string, string>();
        }

		//public TimedSequence Sequence {
		//    get { return _sequence; }
		//    set {
		//        if((_sequence = value) != null) {
		//            labelSequenceName.Text = _sequence.Name;
		//            comboBoxFixtures.DataSource = _sequence.Fixtures.ToArray();
		//            if(this.Sequence.Length > 0) {
		//                numericUpDownSequenceLength.Value = this.Sequence.Length / 1000;
		//            }
		//            //Channel[] channels = 
		//            //    (from fixture in _sequence.Fixtures
		//            //     from channel in fixture.Channels
		//            //     select channel).ToArray();
		//            //comboBoxTimingSource.Items.AddRange(channels);
		//            if(_sequence.TimingSourceId != Guid.Empty) {
		//                comboBoxTimingSource.SelectedValue = _sequence.TimingSourceId;
		//            } else {
		//                comboBoxTimingSource.SelectedIndex = 0;
		//            }
		//            comboBoxControllerSetup.Items.Clear();
		//            comboBoxControllerSetup.DisplayMember = "Name";
		//            comboBoxControllerSetup.ValueMember = "ControllerInstance";
		//            comboBoxControllerSetup.DataSource =
		//                (from controller in ApplicationServices.GetConfiguredControllers(_sequence)
		//                 select new { Name = controller.Name, ControllerInstance = controller }).ToList(); 
		//        }
		//    }
		//}

        private void comboBoxFixtures_SelectedIndexChanged(object sender, EventArgs e) {
            comboBoxChannels.DisplayMember = "Name";
            comboBoxChannels.ValueMember = "Id";
            comboBoxChannels.DataSource = (comboBoxFixtures.SelectedItem as Fixture).Channels.ToArray();
        }

        private void comboBoxChannels_SelectedIndexChanged(object sender, EventArgs e) {
            //// Get all commands that exist for the selected channel type.
            //IModuleInstance channelInstance = comboBoxChannels.SelectedItem as IModuleInstance;
            //comboBoxCommands.DataSource = Vixen.Sys.ApplicationServices.GetModuleDescriptors<ICommand>().Cast<ICommandModuleDescriptor>().Where(x => x.ChannelTypeId == channelInstance.TypeId).ToList();
        }

        private void comboBoxCommands_SelectedIndexChanged(object sender, EventArgs e) {
        }

        private Channel SelectedChannel {
            get { return comboBoxChannels.SelectedItem as Channel; }
        }

        private void buttonAffectSelected_Click(object sender, EventArgs e) {
            int time;
            int.TryParse(textBoxStartTime.Text, out time);
            CommandSignature commandSignature = comboBoxCommands.SelectedItem as CommandSignature;
            Channel channel = SelectedChannel;
            if(channel != null && commandSignature != null) {
				//ParameterValue[] parameterValues = null;
				object[] parameterValues = null;
                using(CommandParameterContainer paramContainer = new CommandParameterContainer(commandSignature)) {
                    if(paramContainer.HasParameters && paramContainer.ShowDialog() == DialogResult.OK) {
                        parameterValues = paramContainer.Values;
                    }
                }
                channel.InsertData(time, commandSignature.Identifier, parameterValues);
            }
        }

        private void buttonAffectAllFixtureChannels_Click(object sender, EventArgs e) {
            int time;
            int.TryParse(textBoxStartTime.Text, out time);
            CommandSignature commandSignature = comboBoxCommands.SelectedItem as CommandSignature;

            if(commandSignature != null) {
				//ParameterValue[] parameterValues = null;
				object[] parameterValues = null;
                using(CommandParameterContainer paramContainer = new CommandParameterContainer(commandSignature)) {
                    if(paramContainer.HasParameters && paramContainer.ShowDialog() == DialogResult.OK) {
                        parameterValues = paramContainer.Values;
                    }
                }
                Cursor = Cursors.WaitCursor;
                try {
                    foreach(Channel channel in comboBoxChannels.Items) {
                        channel.InsertData(time, commandSignature.Identifier, parameterValues);
                    }
                } finally {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void buttonAffectAllChannels_Click(object sender, EventArgs e) {
            int time;
            int.TryParse(textBoxStartTime.Text, out time);
            CommandSignature commandSignature = comboBoxCommands.SelectedItem as CommandSignature;

            if(commandSignature != null) {
				//ParameterValue[] parameterValues = null;
				object[] parameterValues = null;
                using(CommandParameterContainer paramContainer = new CommandParameterContainer(commandSignature)) {
                    if(paramContainer.HasParameters && paramContainer.ShowDialog() == DialogResult.OK) {
                        parameterValues = paramContainer.Values;
                    }
                }
                Cursor = Cursors.WaitCursor;
                try {
                    foreach(Fixture fixture in comboBoxFixtures.Items) {
                        foreach(Channel channel in fixture.Channels) {
                            channel.InsertData(time, commandSignature.Identifier, parameterValues);
                        }
                    }
                } finally {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void buttonAffectSelectedOverTime_Click(object sender, EventArgs e) {
            int time;
            int.TryParse(textBoxStartTime.Text, out time);
            int interval;
            int.TryParse(textBoxInterval.Text, out interval);
            int count;
            int.TryParse(textBoxSeconds.Text, out count);
            count = count * 1000 / interval;
            CommandSignature commandSignature = comboBoxCommands.SelectedItem as CommandSignature;

            if(commandSignature != null) {
				//ParameterValue[] parameterValues = null;
				object[] parameterValues = null;
                using(CommandParameterContainer paramContainer = new CommandParameterContainer(commandSignature)) {
                    if(paramContainer.HasParameters && paramContainer.ShowDialog() == DialogResult.OK) {
                        parameterValues = paramContainer.Values;
                    }
                }
                Cursor = Cursors.WaitCursor;
                try {
                    for(int i = 0; i < count; i++) {
                        SelectedChannel.InsertData(time + i * interval, commandSignature.Identifier, parameterValues);
                    }
                } finally {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void buttonAffectAllChannelsOverTime_Click(object sender, EventArgs e) {
            int time;
            int.TryParse(textBoxStartTime.Text, out time);
            int interval;
            int.TryParse(textBoxInterval.Text, out interval);
            int count;
            int.TryParse(textBoxSeconds.Text, out count);
            count = count * 1000 / interval;
            CommandSignature commandSignature = comboBoxCommands.SelectedItem as CommandSignature;

            if(commandSignature != null) {
				//ParameterValue[] parameterValues = null;
				object[] parameterValues = null;
                using(CommandParameterContainer paramContainer = new CommandParameterContainer(commandSignature)) {
                    if(paramContainer.HasParameters && paramContainer.ShowDialog() == DialogResult.OK) {
                        parameterValues = paramContainer.Values;
                    }
                }
                Cursor = Cursors.WaitCursor;
                try {
                    foreach(Fixture fixture in comboBoxFixtures.Items) {
                        foreach(Channel channel in fixture.Channels) {
                            for(int i = 0; i < count; i++) {
                                channel.InsertData(time + i * interval, commandSignature.Identifier, parameterValues);
                            }
                        }
                    }
                } finally {
                    Cursor = Cursors.Default;
                }
            }
        }

        private void buttonRefreshChannelData_Click(object sender, EventArgs e) {
            if(SelectedChannel != null) {
                listBoxChannelData.Items.Clear();
                foreach(Vixen.Common.TimedData timedData in SelectedChannel) {
                    listBoxChannelData.Items.Add(string.Format("{0}: {1} {2}", timedData.Time, timedData.Data.CommandIdentifier, string.Join(" ", timedData.Data.ParameterValues.Select<object,string>(x => x.ToString()).ToArray())));
                }
            }
        }

        private OutputController SelectedExecutionController {
            get { return comboBoxExecutionController.SelectedItem as OutputController; }
        }

        private void buttonExecutionPatch_Click(object sender, EventArgs e) {
            if(SelectedExecutionController != null) {
                // Patch channels to outputs in order.
                Channel[] channels = this.Sequence.Fixtures.SelectMany<Fixture,Channel>(x => x.Channels).ToArray();
                int count = Math.Min(SelectedExecutionController.OutputCount, channels.Length);
                Guid controllerId = SelectedExecutionController.Id;
                for(int i = 0; i < count; i++) {
                    channels[i].Patch.Clear();
                    channels[i].Patch.Add(controllerId, i);
                }
            }
        }

        private void buttonStart_Click(object sender, EventArgs e) {
            if(SelectedExecutionController != null) {
                _context = Execution.CreateContext(this.Sequence);
                _context.SequenceStarted += _context_SequenceStarted;
                _context.ProgramEnded += _context_ProgramEnded;
                int startTime = int.Parse(textBoxPlayStartTime.Text);
                int endTime = int.Parse(textBoxPlayEndTime.Text);
                if(endTime == 0) {
                    _context.Play();
                } else {
                    _context.Play(startTime, endTime);
                }
                timer.Start();
            }
        }

        private ITimingSource _timingSource;
		//void _context_SequenceStarted(object sender, TimedSequenceStartedEventArgs e) {
        void _context_SequenceStarted(object sender, EventArgs e) {
            TimedSequence sequence = sender as TimedSequence;
			if(e is TimedSequenceStartedEventArgs)
			{
				_timingSource = (e as TimedSequenceStartedEventArgs).TimingSource;
			}
        }

        void _context_ProgramEnded(object sender, EventArgs e) {
            timer.Stop();
            _context.SequenceStarted -= _context_SequenceStarted;
            _context.ProgramEnded -= _context_ProgramEnded;
            Execution.ReleaseContext(_context);
            _context = null;
        }

        private void buttonPause_Click(object sender, EventArgs e) {
            if(_context != null) {
                _context.Pause();
            }
        }

        private void buttonResume_Click(object sender, EventArgs e) {
            if(_context != null) {
                _context.Play();
            }
        }

        private void buttonStop_Click(object sender, EventArgs e) {
            if(_context != null) {
                _context.Stop();
            }
        }

        private void numericUpDownSequenceLength_ValueChanged(object sender, EventArgs e) {
            this.Sequence.Length = (int)numericUpDownSequenceLength.Value * 1000;
        }

        private void timer_Tick(object sender, EventArgs e) {
            int position = _timingSource.Position;
            int minutes = position / 60000;
            int seconds = position % 60000 / 1000;
            int milliseconds = position % 1000;
            labelTime.Text = string.Format("{0:D2}:{1:D2}.{2:D3}", minutes, seconds, milliseconds);
        }

        private void buttonManualPatch_Click(object sender, EventArgs e) {
            using(ManualPatch manualPatch = new ManualPatch(OutputController.GetAll().ToArray(), Sequence.Fixtures.ToArray())) {
                manualPatch.ShowDialog();
            }
        }

        private void comboBoxTimingSource_SelectedIndexChanged(object sender, EventArgs e) {
            if(_sequence != null) {
                if(comboBoxTimingSource.SelectedValue != null) {
                    _sequence.TimingSourceId = (Guid)comboBoxTimingSource.SelectedValue;
                } else {
                    _sequence.TimingSourceId = Guid.Empty;
                }
            }
        }

        private void buttonControllerSetup_Click(object sender, EventArgs e) {
            if(comboBoxControllerSetup.SelectedItem != null) {
                Controller controller = comboBoxControllerSetup.SelectedValue as Controller;
                if(controller.Singleton) {
                    MessageBox.Show("System controller");
                } else {
                    if(!controller.Setup()) {
                        MessageBox.Show("No setup.");
                    }
                }
            }
        }

        private Masking _masking = null;
        private void buttonMasking_Click(object sender, EventArgs e) {
            if(_masking == null) {
                _masking = new Masking(_sequence);
            }
            _masking.Show();
        }

		private void buttonQueue_Click(object sender, EventArgs e) {
			Vixen.Sys.Execution.QueueSequence(this.Sequence, textBoxQueueContextName.Text);
		}

		public IExecutable Sequence {
			get { return _sequence as IExecutable; }
			set {
				if((_sequence = value as TimedSequence) != null) {
					labelSequenceName.Text = _sequence.Name;
					comboBoxFixtures.DataSource = _sequence.Fixtures.ToArray();
					if(_sequence.Length > 0) {
						numericUpDownSequenceLength.Value = _sequence.Length / 1000;
					}
					if(_sequence.TimingSourceId != Guid.Empty) {
						comboBoxTimingSource.SelectedValue = _sequence.TimingSourceId;
					} else {
						comboBoxTimingSource.SelectedIndex = 0;
					}
					comboBoxControllerSetup.Items.Clear();
					comboBoxControllerSetup.DisplayMember = "Name";
					comboBoxControllerSetup.ValueMember = "ControllerInstance";
					comboBoxControllerSetup.DataSource =
						(from controller in ApplicationServices.GetConfiguredControllers(_sequence)
						 select new { Name = controller.Name, ControllerInstance = controller }).ToList();
				}
			}
		}

		public ISelection Selection {
			get { return null; }
		}

		public void NewSequence() {
			this.Sequence = new TimedSequence();
		}

		public void SaveSequence() {
			this.Sequence.Save(_fileName);
		}

		private string _fileName;
		public void LoadSequence(string fileName) {
			this.Sequence = Vixen.Sys.Sequence.Load(fileName);
			if(_sequence != null) {
				_fileName = fileName;
			}
		}

		public Dictionary<string, string> EditorValues { get; private set; }
	}
}
