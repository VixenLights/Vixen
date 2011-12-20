using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Vixen.Commands;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Module.Input;
using Vixen.Sys;

namespace BasicInputManagement {
	public partial class SetupForm : Form {
		private List<InputEffectMap> _maps;
		private List<IInputModuleInstance> _inputModules;
		private IEffectModuleInstance _selectedEffect;

		public SetupForm(IEnumerable<InputEffectMap> maps, IEnumerable<IInputModuleInstance> inputModules) {
			InitializeComponent();
			_maps = new List<InputEffectMap>(maps);
			_inputModules = new List<IInputModuleInstance>(inputModules);
		}

		private void SetupForm_Load(object sender, EventArgs e) {
			var inputModuleDescriptors = ApplicationServices.GetModuleDescriptors<IInputModuleInstance>();
			foreach(IModuleDescriptor descriptor in inputModuleDescriptors) {
				ToolStripItem toolStripItem = contextMenuStripInputModules.Items.Add(descriptor.TypeName);
				toolStripItem.Click += toolStripItemAddInputModule_Click;
				toolStripItem.Tag = descriptor;
			}

			buttonAddInputModule.Enabled = contextMenuStripInputModules.Items.Count > 0;

			// Limit the effects to ones that have parameters that can be 
			// assignable from double.
			IEnumerable<IEffectModuleDescriptor> moduleDescriptors = ApplicationServices.GetModuleDescriptors<IEffectModuleInstance>().Cast<IEffectModuleDescriptor>();
			comboBoxEffect.DisplayMember = "TypeName";
			comboBoxEffect.ValueMember = "TypeId";
			comboBoxEffect.DataSource = moduleDescriptors.Where(x => x.Parameters.Any(y => _CanBeDouble(y.Type))).Cast<IModuleDescriptor>().ToArray();

			checkedListBoxNodes.DisplayMember = "Name";
			checkedListBoxNodes.ValueMember = "Id";
			checkedListBoxNodes.DataSource = VixenSystem.Nodes.GetRootNodes().ToArray();

			_RefreshInputModuleList();
		}

		private TreeNode _CreateInputModuleNode(IInputModuleInstance inputModule) {
			TreeNode treeNode = new TreeNode();
			_UpdateInputModuleNode(treeNode, inputModule);
			return treeNode;
		}

		private void _UpdateInputModuleNode(TreeNode treeNode, IInputModuleInstance inputModule) {
			treeNode.Text = string.IsNullOrWhiteSpace(inputModule.DeviceName) ? inputModule.Descriptor.TypeName : inputModule.DeviceName;
			treeNode.Tag = inputModule;
			treeNode.Nodes.Clear();
			foreach(IInputInput input in inputModule.Inputs) {
				TreeNode inputNode = treeNode.Nodes.Add(input.Name);
				inputNode.Tag = input;
			}
		}

		private ListViewItem _CreateInputEffectMapItem(InputEffectMap inputEffectMap) {
			string effectName = _EffectName(inputEffectMap.EffectModuleId);
			string parameterName = _EffectParameterName(inputEffectMap.EffectModuleId, inputEffectMap.InputValueParameterIndex);
			ListViewItem item = new ListViewItem(new[] { effectName, parameterName });
			item.Tag = inputEffectMap;

			return item;
		}

		private bool _ModuleNodeSelected {
			get { return treeViewInputs.SelectedNode != null && treeViewInputs.SelectedNode.Level == 0; }
		}

		private bool _InputNodeSelected {
			get { return treeViewInputs.SelectedNode != null && treeViewInputs.SelectedNode.Level == 1; }
		}

		private IInputModuleInstance _SelectedInputModule {
			get {
				IInputModuleInstance inputModule = null;
				if(_ModuleNodeSelected) {
					inputModule = treeViewInputs.SelectedNode.Tag as IInputModuleInstance;
				} else if(_InputNodeSelected) {
					inputModule = treeViewInputs.SelectedNode.Parent.Tag as IInputModuleInstance;
				}
				return inputModule;
			}
		}

		private IInputInput _SelectedInput {
			get {
				if(_InputNodeSelected) {
					return treeViewInputs.SelectedNode.Tag as IInputInput;
				}
				return null;
			}
		}

		private IEffectModuleDescriptor _SelectedEffectDescriptor {
			get { return comboBoxEffect.SelectedItem as IEffectModuleDescriptor; }
		}

		private IEffectModuleInstance _SelectedEffect {
			get { return _selectedEffect ?? (_selectedEffect = ApplicationServices.Get<IEffectModuleInstance>(_SelectedEffectDescriptor.TypeId)); }
			set {
				_selectedEffect = value;
				_AddEditEffect = _selectedEffect != null ? _selectedEffect.Descriptor.TypeId : Guid.Empty;
			}
		}

		public IEnumerable<IInputModuleInstance> InputModules {
			get { return _inputModules; }
		}

		public IEnumerable<InputEffectMap> Maps {
			get { return _maps; }
		}

		private bool _CanBeDouble(Type type) {
			return
				type.IsAssignableFrom(typeof(double)) ||
				type.GetMethod("op_Implicit", new[] { typeof(double) }) != null;
		}

		private Guid _AddEditEffect {
			get { return (Guid)comboBoxEffect.SelectedValue; }
			set { comboBoxEffect.SelectedValue = value; }
		}

		private int _AddEditParameterAffected {
			get {
				RadioButton radioButton = panelEffectParameters.Controls.OfType<RadioButton>().FirstOrDefault(x => x.Checked);
				if(radioButton != null) {
					return (int)radioButton.Tag;
				}
				return -1;
			}
			set {
				RadioButton radioButton = panelEffectParameters.Controls.OfType<RadioButton>().FirstOrDefault(x => (int)x.Tag == value);
				if(radioButton != null) {
					radioButton.Checked = true;
				}
			}
		}

		private Guid[] _AddEditNodes {
			get { return checkedListBoxNodes.CheckedItems.Cast<ChannelNode>().Select(x => x.Id).ToArray(); }
			set {
				for(int i=0; i<checkedListBoxNodes.Items.Count; i++) {
					ChannelNode channelNode = checkedListBoxNodes.Items[i] as ChannelNode;
					checkedListBoxNodes.SetItemChecked(i, value.Contains(channelNode.Id));
				}
			}
		}

		private InputEffectMap _EditingMap {
			get {
				return new InputEffectMap(
					_SelectedInputModule,
					_SelectedEffect,
					_SelectedInput,
					_AddEditParameterAffected,
					_AddEditNodes);
			}
			set {
				if(value != null) {
					IEffectModuleInstance effect = ApplicationServices.Get<IEffectModuleInstance>(value.EffectModuleId);
					effect.ParameterValues = value.EffectParameterValues;
					_SelectedEffect = effect;
					_AddEditParameterAffected = value.InputValueParameterIndex;
					_AddEditNodes = value.Nodes;
				}
			}
		}

		private IEnumerable<InputEffectMap> _GetInputEffectMaps(IInputModuleInstance inputModule, IInputInput input = null) {
			return (input != null) ? 
				_maps.Where(x => x.IsMappedTo(inputModule, input)) : 
				_maps.Where(x => x.IsMappedTo(inputModule));
		}

		private void _RefreshInputModuleList() {
			_PopulateInputModuleList(_inputModules);
		}

		private void _RefreshInputEffectList() {
			IEnumerable<InputEffectMap> inputEffectMaps = _GetInputEffectMaps(_SelectedInputModule, _SelectedInput);
			_PopulateInputEffectList(inputEffectMaps);
		}

		private void _PopulateInputModuleList(IEnumerable<IInputModuleInstance> inputModules) {
			treeViewInputs.Nodes.Clear();

			if(inputModules != null) {
				foreach(IInputModuleInstance inputModule in inputModules) {
					TreeNode treeNode = _CreateInputModuleNode(inputModule);
					treeViewInputs.Nodes.Add(treeNode);
				}
			}
		}

		private void _PopulateInputEffectList(IEnumerable<InputEffectMap> inputEffectMaps) {
			listViewInputEffectMap.Items.Clear();

			if(inputEffectMaps != null) {
				foreach(InputEffectMap inputEffectMap in inputEffectMaps) {
					ListViewItem item = _CreateInputEffectMapItem(inputEffectMap);
					listViewInputEffectMap.Items.Add(item);
				}
			}
		}

		private string _EffectName(Guid effectModuleId) {
			return ApplicationServices.GetModuleDescriptor(effectModuleId).TypeName;
		}

		private string _EffectParameterName(Guid effectModuleId, int parameterIndex) {
			IEffectModuleDescriptor descriptor = ApplicationServices.GetModuleDescriptor<IEffectModuleDescriptor>(effectModuleId);
			if(descriptor != null && parameterIndex < descriptor.Parameters.Count) {
				return descriptor.Parameters[parameterIndex].Name;
			}
			return null;
		}

		private void _RemoveInputEffectMaps(IEnumerable<InputEffectMap> inputEffectMaps) {
			foreach(InputEffectMap inputEffectMap in inputEffectMaps) {
				_maps.Remove(inputEffectMap);
			}
		}

		private bool _Validate() {
			Guid effectModuleId = _AddEditEffect;
			Guid inputModuleId = _SelectedInputModule.Descriptor.TypeId;
			string inputId = _SelectedInput.Name;
			int inputValueParameterIndex = _AddEditParameterAffected;
			Guid[] nodes = _AddEditNodes;

			List<string> failures = new List<string>();

			if(effectModuleId == Guid.Empty) failures.Add("Effect not selected.");
			if(inputModuleId == Guid.Empty) failures.Add("Input hardware not selected.");
			if(string.IsNullOrWhiteSpace(inputId)) failures.Add("Input not selected.");
			if(inputValueParameterIndex == -1) failures.Add("The input must affect a parameter of the effect.");
			if(nodes == null || nodes.Length == 0) failures.Add("No nodes selected.");

			if(failures.Count > 0) {
				string message = "Could not proceed because:" + Environment.NewLine;
				message += string.Join("\r", failures);
				MessageBox.Show(message, "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				return false;
			}

			return true;
		}

		private void toolStripItemAddInputModule_Click(object sender, EventArgs e) {
			try {
				ToolStripItem toolStripItem = sender as ToolStripItem;
				IModuleDescriptor descriptor = toolStripItem.Tag as IModuleDescriptor;
				IInputModuleInstance inputModule = ApplicationServices.Get<IInputModuleInstance>(descriptor.TypeId);

				_inputModules.Add(inputModule);

				_RefreshInputModuleList();

				_SetupInputDevice(inputModule);
			} catch(Exception ex) {
				MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
			}
		}

		private void treeViewInputs_AfterSelect(object sender, TreeViewEventArgs e) {
			IInputModuleInstance inputModule = _SelectedInputModule;
			if(inputModule != null) {
				buttonRemoveInputModule.Enabled = true;
				buttonSetup.Enabled = inputModule.HasSetup;
			} else {
				buttonRemoveInputModule.Enabled = false;
				buttonSetup.Enabled = false;
			}
			
			_RefreshInputEffectList();
			groupBoxInputDetail.Enabled = _SelectedInput != null;
		}

		private void buttonAddInputModule_Click(object sender, EventArgs e) {
			Point location = buttonAddInputModule.Parent.PointToScreen(new Point(buttonAddInputModule.Left, buttonAddInputModule.Bottom));
			contextMenuStripInputModules.Show(location);
		}

		private void buttonRemoveInputModule_Click(object sender, EventArgs e) {
			if(MessageBox.Show("Remove " + _SelectedInputModule.DeviceName + "?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
				InputEffectMap[] inputEffectMaps = _GetInputEffectMaps(_SelectedInputModule).ToArray();
				_RemoveInputEffectMaps(inputEffectMaps);
				_RefreshInputEffectList();

				_inputModules.Remove(_SelectedInputModule);
				_RefreshInputModuleList();
			}
		}

		private void buttonSetup_Click(object sender, EventArgs e) {
			_SetupInputDevice(_SelectedInputModule);
		}

		private void _SetupInputDevice(IInputModuleInstance inputModule) {
			if(inputModule.Setup()) {
				TreeNode node = treeViewInputs.Nodes.Cast<TreeNode>().First(x => x.Tag == inputModule);
				_UpdateInputModuleNode(node, inputModule);
			}
		}

		private void comboBoxEffect_SelectedIndexChanged(object sender, EventArgs e) {
			IEffectModuleDescriptor descriptor = _SelectedEffectDescriptor;
			
			buttonSetupEffect.Enabled = descriptor != null;

			panelEffectParameters.Controls.Clear();
			for(int i=0; i<descriptor.Parameters.Count; i++) {
				ParameterSpecification parameterSpecification = descriptor.Parameters[i];
				if(_CanBeDouble(parameterSpecification.Type)) {
					RadioButton radioButton = new RadioButton { Text = parameterSpecification.Name, Dock = DockStyle.Top, Tag = i };
					panelEffectParameters.Controls.Add(radioButton);
				}
			}
		}

		private void buttonSetupEffect_Click(object sender, EventArgs e) {
			using(EffectParameterSetup effectParameterSetup = new EffectParameterSetup(_SelectedEffectDescriptor, _SelectedEffect)) {
				if(effectParameterSetup.ShowDialog() == DialogResult.OK) {
					_SelectedEffect = effectParameterSetup.Effect;
				}
			}
		}

		private InputEffectMap _SelectedInputEffectMap {
			get {
				InputEffectMap map = null;
				if(listViewInputEffectMap.SelectedItems.Count == 1) {
					map = listViewInputEffectMap.SelectedItems[0].Tag as InputEffectMap;
				}
				return map;
			}
			set {
				buttonRemoveInputEffectMap.Enabled = value != null;
				groupBoxAddEdit.Enabled = value != null;
				_EditingMap = value;
			}
		}

		private void listViewInputEffectMap_SelectedIndexChanged(object sender, EventArgs e) {
			_SelectedInputEffectMap = listViewInputEffectMap.SelectedItems.Count == 1 ? listViewInputEffectMap.SelectedItems[0].Tag as InputEffectMap : null;
		}

		private void buttonAddInputEffect_Click(object sender, EventArgs e) {
			if(!_Validate()) return;

			try {
				InputEffectMap inputEffectMap = _EditingMap;
				_maps.Add(inputEffectMap);
				_RefreshInputEffectList();
			} catch(Exception ex) {
				MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
			}
		}

		private void buttonUpdateInputEffect_Click(object sender, EventArgs e) {
			if(!_Validate()) return;

			try {
				int index = _maps.IndexOf(_SelectedInputEffectMap);
				if(index != -1) {
					_maps[index] = _EditingMap;
					_RefreshInputEffectList();
				}
			} catch(Exception ex) {
				MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Stop);
			}
		}

		private void buttonRemoveInputEffectMap_Click(object sender, EventArgs e) {
			string message;
			if(listViewInputEffectMap.SelectedIndices.Count == 1) {
				ListViewItem item = listViewInputEffectMap.SelectedItems[0];
				message = "Remove mapping for " + item.SubItems[0].Text + ", " + item.SubItems[1].Text + "?";
			} else {
				message = "Remove " + listViewInputEffectMap.SelectedItems.Count + " mappings?";
			}

			if(MessageBox.Show(message, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
				InputEffectMap[] inputEffectMaps = listViewInputEffectMap.SelectedIndices.Cast<int>().Select(x => listViewInputEffectMap.Items[x].Tag as InputEffectMap).ToArray();
				_RemoveInputEffectMaps(inputEffectMaps);
				_RefreshInputEffectList();
			}
		}
	}
}
