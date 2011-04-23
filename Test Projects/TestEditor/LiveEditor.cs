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

namespace TestEditor {
	public partial class LiveEditor : Form, IEditorModuleInstance {
		//private LiveSequence _sequence;
		private ISequence _sequence;
		//private string _fileName;
		//private ICommandSpec[] _commands;
		private Dictionary<Guid, IEffectEditorControl> _effectEditors = new Dictionary<Guid, IEffectEditorControl>();
		private ProgramContext _context;
		private OutputChannel[] _channels = null;
		private IEffectEditorControl _editorControl;

		public LiveEditor() {
			InitializeComponent();

			// Get the commands available.
			comboBoxCommand.DisplayMember = "CommandName";
			comboBoxCommand.ValueMember = "";
			comboBoxCommand.DataSource = ApplicationServices.GetAll<IEffectModuleInstance>().Cast<IEffect>().ToArray();

			// Get the controllers for patching.
			comboBoxController.DisplayMember = "Name";
			comboBoxController.ValueMember = "Id";
			comboBoxController.DataSource = OutputController.GetAll().ToArray();

			// Get the system channels.
			comboBoxChannel.DataSource = null;
			comboBoxChannel.DisplayMember = "Name";
			comboBoxChannel.ValueMember = "Id";
			comboBoxChannel.DataSource = Vixen.Sys.Execution.Channels.SelectMany(x => x.Channels).ToArray();
		}

		public ISequence Sequence {
			get { return _sequence; }
			set {
				_sequence = value;
				//if((_sequence = value) != null) {
				//    // Get the sequence channels.
				//    comboBoxChannel.DataSource = null;
				//    comboBoxChannel.DisplayMember = "Name";
				//    comboBoxChannel.ValueMember = "Id";
				//    comboBoxChannel.DataSource = _sequence.OutputChannels.ToArray();
				//}
			}
		}

		public ISelection Selection { get; private set; }

		public void NewSequence() {
			// Create a dummy sequence with a single channel
			//LiveSequence sequence = new LiveSequence();
			ISequence sequence = ApplicationServices.CreateSequence(".liv");

			//No length for a LiveSequence.
			//_sequence.Length = 10 * 1000;
			
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
			get { return LiveSequenceEditorModule._typeId; }
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

		private void buttonCommandParameters_Click(object sender, EventArgs e) {
			//if(SelectedCommand != null) {
			//    ICommandEditorControl editorControl = ApplicationServices.GetCommandEditorControl(SelectedCommand);
			//    //***
			//}
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

		//private bool _isRunning = false;
		private IEffectModuleInstance _selectedCommand;
		void _context_ProgramStarted(object sender, EventArgs e) {
			buttonInject.Enabled = true;
			buttonOff.Enabled = true;
			if(_editorControl != null) {
				_parameterValues = _editorControl.EffectParameterValues;
			}
			_selectedCommand = SelectedCommand;
			//System.Threading.Thread thread = new System.Threading.Thread(_DataPump);
			//thread.Start();
		}

		void _context_ProgramEnded(object sender, EventArgs e) {
			//_isRunning = false;
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

		private void comboBoxChannel_SelectedIndexChanged(object sender, EventArgs e)
		{
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

		//No, not the ideal implementation, has many holes, but it will do.
		//private bool _buttonDown = false;
		private object[] _parameterValues;
		private void buttonInject_MouseDown(object sender, MouseEventArgs e) {
			//_buttonDown = true;

			//if(_buttonDown && _parameterValues != null) {
			//    if(_channels != null) {
			//        //Sequence.InsertData(_channels, null, new Command(_selectedCommand, _parameterValues));
			//        Sequence.InsertData(_channels, 0, _sequence.TimingInterval, new Command(_selectedCommand, _parameterValues));
			//    }
			//}
		}

		private void buttonInject_MouseUp(object sender, MouseEventArgs e) {
			//_buttonDown = false;
		}

		////Dangerous implementation because the responsiveness is dependent upon how much
		////data is dumped into the stream.  Too much and it won't respond immediately; too
		////little and the state will not be stable.
		//private void _DataPump() {
		//    _isRunning = true;

		//    while(_isRunning) {
		//        if(_buttonDown && _parameterValues != null) {
		//            if(_channels != null) {
		//                //Sequence.InsertData(_channels, null, new Command(_selectedCommand, _parameterValues));
		//                Sequence.InsertData(_channels, 0, _sequence.TimingInterval, new Command(_selectedCommand, _parameterValues));
		//            }
		//        }
		//        System.Threading.Thread.Sleep(10);
		//    }
		//}

	}
}
