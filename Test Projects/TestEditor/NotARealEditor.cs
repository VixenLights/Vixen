using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.Editor;
using Vixen.Module.Sequence;
using Vixen.Module.Effect;
using Vixen.Module.Timing;
using Vixen.Execution;

namespace TestEditor {
	public partial class NotARealEditor : Form, IEditorUserInterface {
		//public event EventHandler<EditorClosingEventArgs> Closing;

		private Vixen.Sys.ISequence _sequence;
		private ProgramContext _context = null;
		//private NotARealEditorDataModel _moduleData;
		private ITiming _timingSource;

		public NotARealEditor() {
			InitializeComponent();
	
			Application.EnableVisualStyles(); // For ListView groups

			// Effects
			comboBoxEffects.DisplayMember = "EffectName";
			// Not setting ValueMember because it's defined on an interface other than the one
			// that the items are referenced as and therefore will not work.
			comboBoxEffects.ValueMember = "";
			// They have to be cast to IEffect because that's the interface that defines
			// the CommandName property that DisplayMember is bound to.
			comboBoxEffects.DataSource = ApplicationServices.GetAll<IEffectModuleInstance>().Cast<IEffect>().ToArray();

			//Nodes
			comboBoxChannels.DisplayMember = "Name";
			comboBoxChannels.ValueMember = "Id";
			comboBoxChannels.DataSource = VixenSystem.Nodes.ToArray();

			EditorValues = new EditorValues();
		}

		public bool IsModified {
			get { return true; }
		}

		private ChannelNode _SelectedNode {
			get { return comboBoxChannels.SelectedItem as ChannelNode; }
		}

		private void buttonAffectSelected_Click(object sender, EventArgs e) {
			int time;
			int.TryParse(textBoxStartTime.Text, out time);
			int length;
			int.TryParse(textBoxTimeSpan.Text, out length);

			ChannelNode node = _SelectedNode;
			if(node != null && comboBoxEffects.SelectedItem != null) {
				IEffectModuleInstance theEffect = comboBoxEffects.SelectedItem as IEffectModuleInstance;
				IEffectModuleInstance effect = ApplicationServices.Get<IEffectModuleInstance>(theEffect.Descriptor.TypeId);

				object[] parameterValues = null;
				using(EffectParameterContainer paramContainer = new EffectParameterContainer(effect.Descriptor.TypeId)) {
					if(paramContainer.HasParameters && paramContainer.ShowDialog() == DialogResult.OK) {
						parameterValues = paramContainer.Values;
					}
				}

				effect.TimeSpan = TimeSpan.FromMilliseconds(length);
				effect.ParameterValues = parameterValues;
				effect.TargetNodes = new[] { node };

				EffectNode effectNode = new EffectNode(effect, TimeSpan.FromMilliseconds(time));
				Sequence.InsertData(effectNode);
			}
		}

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

		private void buttonStart_Click(object sender, EventArgs e) {
			_context = Execution.CreateContext(this.Sequence);
			_context.SequenceStarted += _context_SequenceStarted;
			_context.ProgramEnded += _context_ProgramEnded;
			TimeSpan startTime = TimeSpan.FromMilliseconds(int.Parse(textBoxPlayStartTime.Text));
			TimeSpan endTime = TimeSpan.FromMilliseconds(int.Parse(textBoxPlayEndTime.Text));
			bool isPlaying;
			if(endTime == TimeSpan.Zero) {
				isPlaying = _context.Play();
			} else {
				isPlaying = _context.Play(startTime, endTime);
			}
			if(isPlaying) {
				timer.Start();
			}
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
			this.Sequence.Length = TimeSpan.FromMilliseconds((int)numericUpDownSequenceLength.Value * 1000);
		}

		private void timer_Tick(object sender, EventArgs e) {
			if(_timingSource != null) {
				long position = (long)_timingSource.Position.TotalMilliseconds;
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
					if(_sequence.Length > TimeSpan.Zero) {
						numericUpDownSequenceLength.Value = (decimal)_sequence.Length.TotalMilliseconds / 1000;
					}

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

					// Example of storing at the sequence level.
					//_sequence.ModuleDataSet.GetModuleTypeData(OwnerModule);
					// Example of storing at the application level.
					//VixenSystem.ModuleData.GetModuleTypeData(this);
					ModuleData.LastOpened = DateTime.Now;
				}
			}
		}

		public ISelection Selection {
			get { return null; }
		}

		//public void NewSequence() {
		//    this.Sequence = ApplicationServices.CreateSequence(".tim");
		//}

		public void Save(string filePath = null) {
			if(string.IsNullOrWhiteSpace(filePath)) {
				_sequence.Save();
			} else {
				_sequence.Save(filePath);
			}
		}

		public EditorValues EditorValues { get; private set; }

		//public Guid InstanceId { get; set; }

		//public IModuleDataModel ModuleData {
		//    get { return _moduleData; }
		//    set { _moduleData = value as NotARealEditorDataModel; }
		//}
		private NotARealEditorDataModel ModuleData {
			get { return OwnerModule.ModuleData as NotARealEditorDataModel; }
		}

		public IModuleDescriptor Descriptor { get; set; }

		private void buttonMedia_Click(object sender, EventArgs e) {
			if(Sequence != null) {
				using(MediaDialog mediaDialog = new MediaDialog(Sequence.Media)) {
					// The dialog is providing UI for module setup, so we're 
					// passing the sequence's actual media collection to provide the 
					// setup data, so no canceling.
					if(mediaDialog.ShowDialog() == DialogResult.OK) {
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
					listViewTimingSources.Items.Add(timingSource).Group = group;
				}
			}
		}

		private void listViewTimingSources_SelectedIndexChanged(object sender, EventArgs e) {
			_SetTimingSource();
		}

		public void Start() {
			Show();
		}

		public IEditorModuleInstance OwnerModule { get; set; }
	}
}
