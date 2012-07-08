using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Vixen.Commands;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenApplication {
	public partial class TestForm : Form {
		private IModuleDescriptor[] _effects;
		private ChannelNode[] _nodes;
		private ChannelData _channelData;
		private Tuple<Guid,CommandNode>[] _commands;
		private int[] _selectedNodeIndexes;
		private Guid _selectedEffect;
		private IEffectModuleInstance _effect;

		public TestForm() {
			InitializeComponent();

			_effects = ApplicationServices.GetModuleDescriptors<IEffectModuleInstance>();
			_nodes = VixenSystem.Nodes.ToArray();
		}

		private void TestForm_Load(object sender, EventArgs e) {
			foreach(IModuleDescriptor effectDescriptor in _effects) {
				comboBoxEffect.Items.Add(effectDescriptor.TypeName);
			}

			foreach(ChannelNode node in _nodes) {
				checkedListBox.Items.Add(node.Name);
			}

			try {
				if(!Execution.IsInTest) {
					if(Execution.IsOpen) {
						Execution.CloseExecution();
					}
					Execution.OpenTest();
					if(!Execution.IsInTest) {
						throw new Exception("Could not get into the test state.");
					}
				}
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
				DialogResult = DialogResult.Cancel;
			}
		}

		private void TestForm_FormClosing(object sender, FormClosingEventArgs e) {
			try {
				Execution.CloseTest();
				Execution.OpenExecution();
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private Guid _SelectedEffect {
			get { return _selectedEffect; }
			set {
				if(_selectedEffect != value) {
					_selectedEffect = value;
					_SetEffectParameters();
				}
			}
		}

		private IEffectModuleInstance _Effect {
			get { return _effect; }
			set {
				if(_effect != value) {
					_effect = value;
					_GenerateEffectData();
				}
			}
		}

		private ChannelNode[] _SelectedNodes {
			get { return checkedListBox.CheckedIndices.Cast<int>().Select(x => _nodes[x]).ToArray(); }
		}

		private void _SetEffectParameters() {
			using(EffectParametersForm effectParametersForm = new EffectParametersForm(_SelectedEffect)) {
				if(effectParametersForm.ShowDialog() == DialogResult.OK) {
					IEffectModuleInstance effect = ApplicationServices.Get<IEffectModuleInstance>(_SelectedEffect);
					effect.ParameterValues = effectParametersForm.EffectParameters;
					_Effect = effect;
				} else {
					_Effect = null;
				}
			}
		}

		private void _GenerateEffectData() {
			trackBar.Enabled = false;
			if(_Effect != null && _selectedNodeIndexes != null && _selectedNodeIndexes.Length > 0) {
			    backgroundWorker.RunWorkerAsync();
			}
		}

		private void comboBoxEffect_SelectedIndexChanged(object sender, EventArgs e) {
			_SelectedEffect = (comboBoxEffect.SelectedIndex != -1) ? _effects[comboBoxEffect.SelectedIndex].TypeId : Guid.Empty;
		}

		private void checkedListBox_ItemCheck(object sender, ItemCheckEventArgs e) {
			List<int> checkedIndices = new List<int>(checkedListBox.CheckedIndices.Cast<int>());
			if(e.NewValue == CheckState.Checked) {
				checkedIndices.Add(e.Index);
			} else {
				checkedIndices.Remove(e.Index);
			}
			_selectedNodeIndexes = checkedIndices.ToArray();
			_GenerateEffectData();
		}

		private void trackBar_Scroll(object sender, EventArgs e) {
			Guid channelId = _commands[trackBar.Value].Item1;
			CommandNode commandNode = _commands[trackBar.Value].Item2;
			//TODO
			//VixenSystem.Channels.GetChannel(channelId).AddData(commandNode);
		}

		private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e) {
			// Completely arbitrary value, but the more data, the smoother the transitions.
			// Would be unnecessary with dynamic evaluation.
			_Effect.TimeSpan = TimeSpan.FromSeconds(5);
			_Effect.TargetNodes = _SelectedNodes;
			//TODO
			//_channelData = _Effect.Render();
			//_commands = _channelData.Keys.SelectMany(x => _channelData[x].Select(y => Tuple.Create(x, y))).OrderBy(x => x.Item2.StartTime).ToArray();
		}

		private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			if(!e.Cancelled) {
				trackBar.Maximum = _commands.Length - 1;
				trackBar.TickFrequency = trackBar.Maximum;
				trackBar.Enabled = _commands.Length > 0;
			}
		}

		private void buttonSetEffectParameters_Click(object sender, EventArgs e) {
			_SetEffectParameters();
		}
	}
}
