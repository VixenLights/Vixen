using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;
using Vixen.Common;
//using Vixen.Sequence;
using Vixen.Execution;
using Vixen.Module;
using Vixen.Module.Editor;
using Vixen.Module.EffectEditor;
using Vixen.Module.Effect;
//using Vixen.Module.Sequence;
using Vixen.Hardware;
using Vixen.Module.RuntimeBehavior;

namespace TestEditor {
	public partial class RecordingEditor : Form, IEditorModuleInstance {
		private ISequence _sequence;
		//private string _fileName;
		private Dictionary<Guid, IEffectEditorControl> _effectEditors = new Dictionary<Guid, IEffectEditorControl>();
		private ProgramContext _context;
		private OutputChannel[] _channels = null;
		private IEffectEditorControl _editorControl;
		private object[] _parameterValues;
		private IEffectModuleInstance _selectedCommand;

		public RecordingEditor() {
			InitializeComponent();

			// Get the commands available.
			comboBoxCommand.DisplayMember = "EffectName";
			comboBoxCommand.ValueMember = "";
			comboBoxCommand.DataSource = ApplicationServices.GetAll<IEffectModuleInstance>().Cast<IEffect>().ToArray();

			// Get the controllers for patching.
			comboBoxController.DisplayMember = "Name";
			comboBoxController.ValueMember = "Id";
			comboBoxController.DataSource = OutputController.GetAll().ToArray();

			// Get the data targets.
			comboBoxChannel.DataSource = null;
			comboBoxChannel.DisplayMember = "Name";
			comboBoxChannel.ValueMember = "Id";
			comboBoxChannel.DataSource = Vixen.Sys.Execution.Nodes.ToArray();
		}

		public ISequence Sequence {
			get { return _sequence; }
			set {
				if((_sequence = value) != null) {
					//// Get the sequence channels.
					//comboBoxChannel.DataSource = null;
					//comboBoxChannel.DisplayMember = "Name";
					//comboBoxChannel.ValueMember = "Id";
					//comboBoxChannel.DataSource = _sequence.OutputChannels.ToArray();
					// Get the actions for the recording behavior.
					// Normally, this is done generically for all behaviors and not handled
					// so hacky.
					IRuntimeBehaviorModuleInstance behavior = _sequence.RuntimeBehaviors.First(x => x.TypeName.Contains("Recording"));
					Button button;
					foreach(Tuple<string,Action> action in behavior.BehaviorActions) {
						// Capture the iteration variable.
						Tuple<string, Action> targetAction = action;
						button = new Button();
						button.Text = action.Item1;
						button.Click += (sender, e) => {
							targetAction.Item2();
							//Action targetAction = action.Item2; // capture
							//targetAction();
						};
						flowLayoutPanel.Controls.Add(button);
					}
				}
			}
		}

		public ISelection Selection { get; private set; }

		public void NewSequence() {
			//".rec" is the key for the module type we want to instantiate.
			ISequence sequence = ApplicationServices.CreateSequence(".rec");
			
			//// *** TESTING ***
			//Fixture fixture = new Fixture();
			//fixture.Name = "Fixture 1";
			////OutputChannel channel = new OutputChannel(true);
			//OutputChannel channel = new OutputChannel();
			//channel.Name = "Channel 1";
			//fixture.InsertChannel(channel);
			//sequence.InsertFixture(fixture);

			this.Sequence = sequence;
		}

		public void Save(string filePath = null) {
			if(string.IsNullOrWhiteSpace(filePath)) {
				_sequence.Save();
			} else {
				_sequence.Save(filePath);
			}
		}

		//public void LoadSequence(string fileName) {
		//    this.Sequence = Vixen.Sys.Sequence.Load(fileName);
		//    if(_sequence != null) {
		//        _fileName = fileName;
		//    }
		//}

		public Dictionary<string, string> EditorValues { get; private set; }

		public bool IsModified {
			get { return true; }
		}

		public Guid TypeId {
			get { return RecordingSequenceEditorModule._typeId; }
		}

		public Guid InstanceId { get; set; }

		public IModuleDataModel ModuleData { get; set; }

		public string TypeName { get; set; }





		private IEffectModuleInstance SelectedCommand {
			get { return comboBoxCommand.SelectedItem as IEffectModuleInstance; }
		}

		private OutputChannel SelectedChannel {
			get { return comboBoxChannel.SelectedItem as OutputChannel; }
		}
		
		private void buttonExecution_Click(object sender, EventArgs e) {
			if(buttonExecution.Text == "Start") {
				// Start

				//// Reset patching to the specified controller.
				//// We made the sequence and we know it only has one channel.
				//OutputController controller = comboBoxController.SelectedItem as OutputController;
				//OutputChannel channel = _sequence.OutputChannels.First();
				//channel.Patch.Clear();
				//channel.Patch.Add(controller.Id, 0);

				_Play();
				buttonExecution.Text = "Stop";
			} else {
				// Stop
				_Stop();
				buttonExecution.Text = "Start";
			}
		}

		private void _Play() {
			if(_context == null) {
				_context = Execution.CreateContext(this.Sequence);
				_context.ProgramStarted += new EventHandler(_context_ProgramStarted);
				_context.ProgramEnded += new EventHandler(_context_ProgramEnded);
				_context.Play();
			}
		}

		void _context_ProgramStarted(object sender, EventArgs e) {
			buttonInject.Enabled = true;
			buttonOff.Enabled = true;
			if(_editorControl != null) {
				_parameterValues = _editorControl.EffectParameterValues;
			}
			_selectedCommand = SelectedCommand;
		}

		void _context_ProgramEnded(object sender, EventArgs e) {
			buttonInject.Enabled = false;
			buttonOff.Enabled = false;
		}

		private void _Stop() {
			if(_context != null) {
				_context.Stop();
				_context.ProgramStarted -= _context_ProgramStarted;
				_context.ProgramEnded -= _context_ProgramEnded;
				_context = null;
			}
		}

		private void comboBoxChannel_SelectedIndexChanged(object sender, EventArgs e) {
			if(SelectedChannel != null) {
				_channels = new OutputChannel[] { SelectedChannel };
			} else {
				_channels = null;
			}
		}

		private void comboBoxCommand_SelectedIndexChanged(object sender, EventArgs e) {
			if(SelectedCommand != null) {
				_editorControl = _GetEffectEditor(SelectedCommand);
				panelEditorControlContainer.Controls.Clear();
				if(_editorControl != null) {
					panelEditorControlContainer.Controls.Add(_editorControl as Control);
				}
			}
		}

		private IEffectEditorControl _GetEffectEditor(IEffectModuleInstance effect) {
			IEffectEditorControl editorControl = null;
			if(effect != null && !_effectEditors.TryGetValue(effect.TypeId, out editorControl)) {
				_effectEditors[effect.TypeId] = editorControl = ApplicationServices.GetEffectEditorControl(effect.TypeId);
			}
			return editorControl;
		}

		private void buttonInject_Click(object sender, EventArgs e) {
			if(_parameterValues != null && _channels != null) {
				Sequence.InsertData(_channels, 0, _sequence.Data.TimingInterval * 10, new Command(_selectedCommand.TypeId, _parameterValues));
			}
		}

		private void buttonOff_Click(object sender, EventArgs e) {
			if(_parameterValues != null && _channels != null) {
				Sequence.InsertData(_channels, 0, _sequence.Data.TimingInterval, null);
			}
		}


	}
}
