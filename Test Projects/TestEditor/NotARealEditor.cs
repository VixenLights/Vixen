using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;
using CommandStandard;
using Vixen.Common;
using Vixen.Hardware;
using Vixen.Module;
using Vixen.Module.Editor;
using Vixen.Module.Sequence;
using Vixen.Module.Effect;
using Vixen.Module.Timing;
//using Vixen.Sequence;
using Vixen.Execution;

namespace TestEditor {
	public partial class NotARealEditor : Form, IEditorModuleInstance {
		//private Vixen.Sequence.ISequence _sequence;
		private Vixen.Sys.ISequence _sequence;
		private ProgramContext _context = null;

		public NotARealEditor() {
			InitializeComponent();
			Application.EnableVisualStyles(); // For ListView groups
			// Get all available commands.
			// What can be assumed is that there will be platforms, categories, commands.
			// What cannot be assumed is the presence of any particular instances of
			// any of those, except the custom command category for every platform.

			//List<CommandSignature> commands = new List<CommandSignature>();
			////commands.AddRange(ApplicationServices.Standard.GetCategoryCommands(Standard.Lighting.Value, Standard.Lighting.Monochrome.Value));
			////commands.AddRange(ApplicationServices.Standard.GetPlatformCustomCommands(Standard.Lighting.Value));
			//commands.AddRange(Standard.GetCommandSignatures(Standard.Lighting.Value, Standard.Lighting.Monochrome.Value));
			//commands.AddRange(Standard.GetCommandSignatures(Standard.Media.Value, Standard.Media.Audio.Value));
			////This won't do a damn thing!  It's the application that has custom commands, not the standard.
			////commands.AddRange(Standard.GetCommandSignatures(Standard.Lighting.Value, Standard.CustomCategory));
			//comboBoxCommands.DisplayMember = "Identifier";
			//comboBoxCommands.ValueMember = "Identifier";
			//comboBoxCommands.DataSource = commands;
			comboBoxEffects.DisplayMember = "EffectName";
			// Not setting ValueMember because it's defined on an interface other than the one
			// that the items are referenced as and therefore will not work.
			comboBoxEffects.ValueMember = "";
			// They have to be cast to IEffect because that's the interface that defines
			// the CommandName property that DisplayMember is bound to.
			comboBoxEffects.DataSource = ApplicationServices.GetAll<IEffectModuleInstance>().Cast<IEffect>().ToArray();
			//// Need to get a "none" wedged in there.
			//List<KeyValuePair<Guid, string>> timingSources = new List<KeyValuePair<Guid, string>>();
			////timingSources.Add(new KeyValuePair<Guid,string>(Guid.Empty, "(none)"));
			//timingSources.AddRange(ApplicationServices.GetTimingSources(Sequence).ToArray());
			//comboBoxTimingSource.DisplayMember = "Value"; // string - Name
			//comboBoxTimingSource.ValueMember = "Key"; // Guid - Type id
			//comboBoxTimingSource.DataSource = timingSources;

			//comboBoxFixtures.DataSource = Vixen.Sys.Execution.Channels.ToArray();
			comboBoxChannels.DisplayMember = "Name";
			comboBoxChannels.ValueMember = "Id";
			//Nodes
			comboBoxChannels.DataSource = Vixen.Sys.Execution.Nodes.ToArray();

			EditorValues = new Dictionary<string, string>();
		}

		public bool IsModified {
			get { return true; }
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

		//private void comboBoxFixtures_SelectedIndexChanged(object sender, EventArgs e) {
		//    comboBoxChannels.DisplayMember = "Name";
		//    comboBoxChannels.ValueMember = "Id";
		//    comboBoxChannels.DataSource = (comboBoxFixtures.SelectedItem as Fixture).Channels.ToArray();
		//}

		//private OutputChannel SelectedChannel {
		//    get { return comboBoxChannels.SelectedItem as OutputChannel; }
		//}
		private ChannelNode _SelectedNode {
			get { return comboBoxChannels.SelectedItem as ChannelNode; }
		}

		private void buttonAffectSelected_Click(object sender, EventArgs e) {
			int time;
			int.TryParse(textBoxStartTime.Text, out time);
			int length;
			int.TryParse(textBoxTimeSpan.Text, out length);
			//OutputChannel channel = SelectedChannel;
			ChannelNode node = _SelectedNode;
			if(node != null && comboBoxEffects.SelectedItem != null) {
				IEffectModuleInstance effect = comboBoxEffects.SelectedItem as IEffectModuleInstance;
				object[] parameterValues = null;
				using(EffectParameterContainer paramContainer = new EffectParameterContainer(effect.TypeId)) {
					if(paramContainer.HasParameters && paramContainer.ShowDialog() == DialogResult.OK) {
						parameterValues = paramContainer.Values;
					}
				}
				Sequence.InsertData(new[] { node }, time, length, new Command((comboBoxEffects.SelectedItem as IEffectModuleInstance).TypeId, parameterValues));
			}
		}

		//private void buttonAffectAllFixtureChannels_Click(object sender, EventArgs e) {
		//    int time;
		//    int.TryParse(textBoxStartTime.Text, out time);
		//    int length;
		//    int.TryParse(textBoxTimeSpan.Text, out length);
		//    if(comboBoxEffects.SelectedItem != null) {
		//        IEffectModuleInstance effect = comboBoxEffects.SelectedItem as IEffectModuleInstance;

		//        object[] parameterValues = null;
		//        using(EffectParameterContainer paramContainer = new EffectParameterContainer(effect.TypeId)) {
		//            if(paramContainer.HasParameters && paramContainer.ShowDialog() == DialogResult.OK) {
		//                parameterValues = paramContainer.Values;
		//            }
		//        }
		//        Cursor = Cursors.WaitCursor;
		//        try {
		//            Sequence.InsertData(comboBoxChannels.Items.Cast<OutputChannel>().ToArray(), time, length, new Command((comboBoxEffects.SelectedItem as IEffectModuleInstance).TypeId, parameterValues));
		//        } finally {
		//            Cursor = Cursors.Default;
		//        }
		//    }
		//}

		//private void buttonAffectAllChannels_Click(object sender, EventArgs e) {
		//    int time;
		//    int.TryParse(textBoxStartTime.Text, out time);
		//    int length;
		//    int.TryParse(textBoxTimeSpan.Text, out length);
		//    if(comboBoxEffects.SelectedItem != null) {
		//        //CommandSignature commandSignature = comboBoxCommands.SelectedItem as CommandSignature;
		//        IEffectModuleInstance effect = comboBoxEffects.SelectedItem as IEffectModuleInstance;

		//        object[] parameterValues = null;
		//        using(EffectParameterContainer paramContainer = new EffectParameterContainer(effect.TypeId)) {
		//            if(paramContainer.HasParameters && paramContainer.ShowDialog() == DialogResult.OK) {
		//                parameterValues = paramContainer.Values;
		//            }
		//        }
		//        Cursor = Cursors.WaitCursor;
		//        try {
		//            //foreach(Fixture fixture in comboBoxFixtures.Items) {
		//            //    foreach(Channel channel in fixture.Channels) {
		//            //        channel.InsertCommand(new CommandData(commandSignature.Identifier, parameterValues), time);
		//            //    }
		//            //}
		//            //Sequence.InsertData(Sequence.OutputChannels.ToArray(), time, length, new Command((comboBoxCommands.SelectedItem as IEffectModuleInstance).TypeId, parameterValues));
		//            Sequence.InsertData(Vixen.Sys.Execution.Channels.SelectMany(x => x.Channels).ToArray(), time, length, new Command((comboBoxEffects.SelectedItem as IEffectModuleInstance).TypeId, parameterValues));
		//        } finally {
		//            Cursor = Cursors.Default;
		//        }
		//    }
		//}

		//Needs updating
		private void buttonAffectSelectedOverTime_Click(object sender, EventArgs e) {
			//int time;
			//int.TryParse(textBoxStartTime.Text, out time);
			//int interval;
			//int.TryParse(textBoxInterval.Text, out interval);
			//int count;
			//int.TryParse(textBoxSeconds.Text, out count);
			//count = count * 1000 / interval;
			//CommandSignature commandSignature = comboBoxCommands.SelectedItem as CommandSignature;

			//if(commandSignature != null) {
			//    object[] parameterValues = null;
			//    using(CommandParameterContainer paramContainer = new CommandParameterContainer(commandSignature)) {
			//        if(paramContainer.HasParameters && paramContainer.ShowDialog() == DialogResult.OK) {
			//            parameterValues = paramContainer.Values;
			//        }
			//    }
			//    Cursor = Cursors.WaitCursor;
			//    try {
			//        for(int i = 0; i < count; i++) {
			//            SelectedChannel.InsertCommand(new CommandData(commandSignature.Identifier, parameterValues), time + i * interval);
			//        }
			//    } finally {
			//        Cursor = Cursors.Default;
			//    }
			//}
		}

		//private void buttonAffectAllChannelsOverTime_Click(object sender, EventArgs e) {
		//    //int time;
		//    //int.TryParse(textBoxStartTime.Text, out time);
		//    //int interval;
		//    //int.TryParse(textBoxInterval.Text, out interval);
		//    //int count;
		//    //int.TryParse(textBoxSeconds.Text, out count);
		//    //count = count * 1000 / interval;
		//    //CommandSignature commandSignature = comboBoxCommands.SelectedItem as CommandSignature;

		//    //if(commandSignature != null) {
		//    //    //ParameterValue[] parameterValues = null;
		//    //    object[] parameterValues = null;
		//    //    using(CommandParameterContainer paramContainer = new CommandParameterContainer(commandSignature)) {
		//    //        if(paramContainer.HasParameters && paramContainer.ShowDialog() == DialogResult.OK) {
		//    //            parameterValues = paramContainer.Values;
		//    //        }
		//    //    }
		//    //    Cursor = Cursors.WaitCursor;
		//    //    try {
		//    //        foreach(Fixture fixture in comboBoxFixtures.Items) {
		//    //            foreach(Channel channel in fixture.Channels) {
		//    //                for(int i = 0; i < count; i++) {
		//    //                    channel.InsertCommand(new CommandData(commandSignature.Identifier, parameterValues), time + i * interval);
		//    //                }
		//    //            }
		//    //        }
		//    //    } finally {
		//    //        Cursor = Cursors.Default;
		//    //    }
		//    //}
		//}

		//private void buttonRefreshChannelData_Click(object sender, EventArgs e) {
		//    if(SelectedChannel != null) {
		//        listBoxChannelData.Items.Clear();
		//        foreach(CommandNode commandNode in this.Sequence.Data.GetCommands()) {
		//            listBoxChannelData.Items.Add(string.Format("[{0}] {1}  {2}-{3}", string.Join(",",commandNode.TargetChannels.Select(x => x.Name)), commandNode.Command.Effect.EffectName, commandNode.StartTime, commandNode.EndTime));
		//        }
		//    }
		//}

		private void buttonStart_Click(object sender, EventArgs e) {
			//if(SelectedExecutionController != null) {
				_context = Execution.CreateContext(this.Sequence);
				_context.SequenceStarted += _context_SequenceStarted;
				_context.ProgramEnded += _context_ProgramEnded;
				int startTime = int.Parse(textBoxPlayStartTime.Text);
				int endTime = int.Parse(textBoxPlayEndTime.Text);
				bool isPlaying;
				if(endTime == 0) {
					isPlaying = _context.Play();
				} else {
					isPlaying = _context.Play(startTime, endTime);
				}
				if(isPlaying) {
					timer.Start();
				}
			//}
		}

		private void _SetTimingSource() {
			if(_sequence != null) {
				if(listViewTimingSources.SelectedItems.Count == 1) {
					ListViewItem item = listViewTimingSources.SelectedItems[0];
					_sequence.TimingProvider.SetSelectedSource(item.Group.Name, item.Text);
				} else {
					_sequence.TimingProvider.SetSelectedSource(null, null);
				}
			}
		}

		private ITiming _timingSource;
		//void _context_SequenceStarted(object sender, TimedSequenceStartedEventArgs e) {
		void _context_SequenceStarted(object sender, SequenceStartedEventArgs e) {
			//*** no context was created, so no event
			Vixen.Sys.ISequence sequence = sender as Vixen.Sys.ISequence;
			_timingSource = e.TimingSource;
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
			if(_timingSource != null) {
				long position = _timingSource.Position;
				long minutes = position / 60000;
				long seconds = position % 60000 / 1000;
				long milliseconds = position % 1000;
				labelTime.Text = string.Format("{0:D2}:{1:D2}.{2:D3}", minutes, seconds, milliseconds);
			}
		}

		private void comboBoxTimingSource_SelectedIndexChanged(object sender, EventArgs e) {
			//if(_sequence != null) {
			//    if(comboBoxTimingSource.SelectedValue != null) {
			//        _sequence.TimingSourceId = (Guid)comboBoxTimingSource.SelectedValue;
			//        _sequence.TimingProvider.SetSelectedSource(
			//    } else {
			//        _sequence.TimingSourceId = Guid.Empty;
			//    }
			//}
		}

		private void buttonQueue_Click(object sender, EventArgs e) {
			int count = Vixen.Sys.Execution.QueueSequence(this.Sequence, textBoxQueueContextName.Text);
			MessageBox.Show("You are number " + count);
		}

		public Vixen.Sys.ISequence Sequence {
			get { return _sequence as Vixen.Sys.ISequence; }
			set {
				if((_sequence = value) != null) {
					// Name
					labelSequenceName.Text = _sequence.Name;
					// Length
					if(_sequence.Length > 0) {
						numericUpDownSequenceLength.Value = _sequence.Length / 1000;
					}
					// Timing source
					//if(_sequence.TimingSourceId != Guid.Empty) {
					//    comboBoxTimingSource.SelectedValue = _sequence.TimingSourceId;
					//} else {
					//    comboBoxTimingSource.SelectedIndex = 0;
					//}
					_RefreshTimingSources();
					string providerType;
					string sourceName;
					_sequence.TimingProvider.GetSelectedSource(out providerType, out sourceName);
					ListViewGroup group = listViewTimingSources.Groups.Cast<ListViewGroup>().FirstOrDefault(x => x.Name == providerType);
					if(group != null) {
						ListViewItem item = listViewTimingSources.Items.Cast<ListViewItem>().FirstOrDefault(x => x.Text == sourceName);
						if(item != null && item.Group == group) {
							item.Selected = true;
						}
					}
					//listViewTimingSources.Items[0].Selected

					_sequence.ModuleDataSet.GetModuleTypeData(this);
					_moduleData.LastOpened = DateTime.Now;
				}
			}
		}

		public ISelection Selection {
			get { return null; }
		}

		public void NewSequence() {
			//this.Sequence = new TimedSequence();
			this.Sequence = ApplicationServices.CreateSequence(".tim");
		}

		public void Save(string filePath = null) {
			if(string.IsNullOrWhiteSpace(filePath)) {
				_sequence.Save();
			} else {
				_sequence.Save(filePath);
			}
		}

		//private string _fileName;
		//public void LoadSequence(string fileName) {
		//    //if((this.Sequence = ApplicationServices.LoadSequence(fileName)) != null) {
		//    if((this.Sequence = Vixen.Sys.Sequence.Load(fileName)) != null) {
		//        _fileName = fileName;
		//    }
		//}

		public Dictionary<string, string> EditorValues { get; private set; }

		public Guid TypeId {
			get { return Module._typeId; }
		}

		public Guid InstanceId { get; set; }

		public string TypeName { get; set; }

		private NotARealEditorDataModel _moduleData;
		public IModuleDataModel ModuleData {
			get { return _moduleData; }
			set { _moduleData = value as NotARealEditorDataModel; }
		}

		private void buttonSetInterval_Click(object sender, EventArgs e) {
			int interval;
			if(int.TryParse(textBoxSequenceInterval.Text, out interval)) {
				Cursor = Cursors.WaitCursor;
				//_sequence.IntervalTiming = interval;
				_sequence.Data.TimingInterval = interval;
				Cursor = Cursors.Default;
			} else {
				MessageBox.Show("Nope.");
			}
		}

		private void buttonEditInterval_Click(object sender, EventArgs e) {
			using(IntervalEditDialog intervalEditDialog = new IntervalEditDialog(this.Sequence)) {
				intervalEditDialog.ShowDialog();
			}
		}

		private void buttonMedia_Click(object sender, EventArgs e) {
			if(Sequence != null) {
				using(MediaDialog mediaDialog = new MediaDialog(Sequence.Media)) {
					// The dialog is providing UI for module setup, so we're 
					// passing the sequence's actual media collection to provide the 
					// setup data, so no canceling.
					if(mediaDialog.ShowDialog() == DialogResult.OK) {
						//Sequence.Media.RemoveAll();
						//foreach(string filePath in mediaDialog.Files) {
						//    Sequence.Media.Add(filePath);
						//}
						_RefreshTimingSources();
					}
				}
			}
		}

		private void comboBoxTimingSource_DropDown(object sender, EventArgs e) {
			//// Available timing sources
			//// Need to get a "none" wedged in there.
			//foreach(string providerType in Sequence.TimingProvider.GetTimingProviderTypes()) {
			//    ListViewGroup group = listViewTimingSources.Groups.Add(providerType, providerType);
			//    foreach(string timingSource in Sequence.TimingProvider.GetAvailableTimingSources(providerType)) {
			//        group.Items.Add(timingSource);
			//    }
			//}
			////List<KeyValuePair<Guid, string>> timingSources = new List<KeyValuePair<Guid, string>>();
			////timingSources.AddRange(ApplicationServices.GetTimingSources(_sequence).ToArray());
			////comboBoxTimingSource.DisplayMember = "Value"; // string - Name
			////comboBoxTimingSource.ValueMember = "Key"; // Guid - Type id
			////comboBoxTimingSource.DataSource = timingSources;
		}

		private void _RefreshTimingSources() {
			// Available timing sources
			// Need to get a "none" wedged in there.
			listViewTimingSources.Groups.Clear();
			listViewTimingSources.Items.Clear();
			foreach(string providerType in Sequence.TimingProvider.GetTimingProviderTypes()) {
				ListViewGroup group = listViewTimingSources.Groups.Add(providerType, providerType);
				foreach(string timingSource in Sequence.TimingProvider.GetAvailableTimingSources(providerType)) {
					// Doesn't show the items (and therefore not the group).
					//group.Items.Add(timingSource);
					listViewTimingSources.Items.Add(timingSource).Group = group;
				}
			}
		}

		private void listViewTimingSources_SelectedIndexChanged(object sender, EventArgs e) {
			_SetTimingSource();
		}
	}
}
