using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using Vixen.IO;
using Vixen.Sys;
using Vixen.Hardware;
using Vixen.Module;
using Vixen.Module.Sequence;
using Vixen.Module.Editor;
using Vixen.Module.ModuleTemplate;
using Vixen.Module.Effect;
using Vixen.Module.EffectEditor;
using Vixen.Module.Output;

namespace TestClient
{
    public partial class Form1 : Form, IApplication
	{
        private Guid _applicationId = new Guid("{623EFEE5-3D39-4cee-8C1E-C0C43D8604F7}");
		private List<OutputController> _controllers = new List<OutputController>();
		//private ITypedDataMover[] _dataMovers;
		private IEffectEditorControl _editorControl;
		private List<IEditorUserInterface> _openEditors = new List<IEditorUserInterface>();
		private IEditorUserInterface _activeEditor = null;
		private bool _dragCapture = false;

		public Form1()
		{
			InitializeComponent();

            Control.CheckForIllegalCrossThreadCalls = true;

            // This must be done BEFORE the system is started because application modules
            // may immediately access the AOM.
            //**The implementation of this is nasty.**
            this.AppCommands = new AppCommand(this);
            AppCommand fileMenu = new AppCommand("FileMenu", "File");
            fileMenu.Add(new AppCommand("Open", "Open"));
            fileMenu.Add(new AppCommand("Save", "Save"));
            this.AppCommands.Add(fileMenu);

			Logging.ItemLogged += _ItemLogged;
			Vixen.Sys.VixenSystem.Start(this);

            // Output modules
			comboBoxOutputModule.DisplayMember = "Value";
			comboBoxOutputModule.ValueMember = "Key";
			comboBoxOutputModule.DataSource = ApplicationServices.GetAvailableModules<IOutputModuleInstance>().ToArray();

            // Controllers
			_LoadControllers();

			// Editors
			ToolStripMenuItem item;
			foreach(KeyValuePair<Guid,string> typeId_FileTypeName in ApplicationServices.GetAvailableModules<IEditorModuleInstance>()) {
				item = new ToolStripMenuItem(typeId_FileTypeName.Value);
				item.Tag = typeId_FileTypeName.Key;
				item.Click += (sender,e) => {
					ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
					Guid editorModuleId = (Guid)menuItem.Tag;
					IEditorUserInterface editor = ApplicationServices.GetEditor(editorModuleId);
					if(editor != null) {
						editor.NewSequence();
						if(editor.Sequence != null) { // Null if canceled
							SequenceName = editor.Sequence.Name;
							_OpenEditor(editor);
						}
					}
				};
				fileToolStripMenuItem.DropDownItems.Add(item);
			}

			// Data movers
			//_dataMovers = DataMover.GetAllMovers().ToArray();

			// File templates
			_LoadFileTemplates(comboBoxFileTemplates);

			// Loaded modules
			_DisplayLoadedModules();

			// Effects
			_LoadEffects();

			// Nodes
			_LoadNodes();
			Vixen.Sys.Execution.Nodes.NodesChanged += (sender, e) => _LoadNodes();

			// Node templates
			_LoadNodeTemplates();
        }

		private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
			//*** using editor.IsModified, give them the chance to save any changes,
			//    not save and close, or cancel the close
			Logging.ItemLogged -= _ItemLogged;
			Vixen.Sys.VixenSystem.Stop();
		}
		
		private void _LoadControllers() {
			listViewControllers.Items.Clear();
			_controllers.Clear();

			_controllers.AddRange(OutputController.GetAll());
			foreach(OutputController controller in _controllers) {
				_AddControllerToView(controller);
			}
			
			comboBoxLinkedTo.DisplayMember = "Name";
			comboBoxLinkedTo.ValueMember = "Id";
			comboBoxLinkedTo.DataSource = _controllers;

			comboBoxControllerOutputsControllers.DisplayMember = "Name";
			comboBoxControllerOutputsControllers.ValueMember = "Id";
			comboBoxControllerOutputsControllers.DataSource = _controllers;

			string[] combinationOperations = Enum.GetNames(typeof(CommandStandard.Standard.CombinationOperation));
			comboBoxCombinationStrategy.DataSource = combinationOperations;
		}

		private void _LoadNodeTemplates() {
			ChannelNodeDefinition[] definitions =
				ChannelNodeDefinition.GetAllFileNames().Select(x => ChannelNodeDefinition.Load(x)).ToArray();
			comboBoxNodeTemplates.DisplayMember = "Name";
			comboBoxNodeTemplates.ValueMember = "Name";
			comboBoxNodeTemplates.DataSource = definitions;
		}

		private void _LoadNodes() {
			IEnumerable<ChannelNode> nodes = Vixen.Sys.Execution.Nodes;

			treeViewSystemChannels.BeginUpdate();
			treeViewNodes.BeginUpdate();
			treeViewSystemChannels.Nodes.Clear();
			treeViewNodes.Nodes.Clear();
			TreeNode rootNode = treeViewNodes.Nodes.Add("Root");
			foreach(ChannelNode node in Vixen.Sys.Execution.Nodes.RootNodes) {
				_AddNode(treeViewSystemChannels.Nodes, node);
				_AddNode(rootNode.Nodes, node);
			}
			treeViewNodes.ExpandAll();
			treeViewNodes.EndUpdate();
			treeViewSystemChannels.EndUpdate();
		}

		private void _LoadEffects() {
			comboBoxEffects.DisplayMember = "EffectName";
			// Not setting ValueMember because it (TypeId) is defined on an interface other than
			// the one that the items are referenced as and therefore will not work.
			comboBoxEffects.ValueMember = "";
			// They have to be cast to IEffect because that's the interface that defines
			// the EffectName property that DisplayMember is bound to.
			comboBoxEffects.DataSource = ApplicationServices.GetAll<IEffectModuleInstance>().Cast<IEffect>().ToArray();
		}

		private void _AddNode(TreeNodeCollection collection, ChannelNode node) {
			TreeNode addedNode;
			addedNode = collection.Add(node.Name);

			addedNode.Tag = node;
			addedNode.Checked = node.Masked;

			foreach(ChannelNode childNode in node.Children) {
				_AddNode(addedNode.Nodes, childNode);
			}
		}

		private void _DisplayLoadedModules() {
			treeViewLoadedModules.Nodes.Clear();

			TreeNode typeNode;
			TreeNode instanceNode;
			foreach(string moduleType in ApplicationServices.GetTypesOfModules()) {
				typeNode = new TreeNode();
				treeViewLoadedModules.Nodes.Add(typeNode);
				typeNode.Text = moduleType;
				foreach(IModuleDescriptor descriptor in ApplicationServices.GetModuleDescriptors(moduleType)) {
					instanceNode = new TreeNode();
					typeNode.Nodes.Add(instanceNode);
					instanceNode.Tag = descriptor;
					instanceNode.Text = descriptor.TypeName;
				}
			}
		}

		private void _ItemLogged(object sender, LogEventArgs e) {
			//This would be a non-blocking UI indicator.
			MessageBox.Show(e.Text, e.LogName);
		}

        public List<T> CreateList<T>(T instance) {
            // Receive return value as a var.
            return new List<T>();
        }

        public Guid ApplicationId {
            get { return _applicationId; }
        }

		private void _LoadFileTemplates(ComboBox comboBox) {
			comboBox.DataSource = null;
			comboBox.DisplayMember = "Value";
			comboBox.ValueMember = "Key";
			comboBox.DataSource = ApplicationServices.GetAvailableModules<IModuleTemplateModuleInstance>().ToArray();
		}

        private string SequenceName {
            set {
				textBoxSequenceName.Text = value;
            }
        }

        private void buttonFindFile_Click(object sender, EventArgs e) {
			_LoadSequenceFileName(textBoxSequenceFileName);
        }

		private bool _LoadSequenceFileName(TextBox textBox) {
			if(openFileDialog.ShowDialog() == DialogResult.OK) {
				textBox.Text = openFileDialog.FileName;
				return true;
			}
			return false;
		}

        private void buttonWriteSequence_Click(object sender, EventArgs e) {
			if(ActiveEditor != null) {
				Cursor = Cursors.WaitCursor;
				try {
					ISequenceModuleInstance sequence = this.ActiveEditor.Sequence as ISequenceModuleInstance;
					if(sequence != null) {
						string fileName = textBoxSequenceName.Text;
						if(!fileName.EndsWith(sequence.FileExtension)) {
							fileName += sequence.FileExtension;
						}
						// Call the save of the editor, not the save of the sequence, so that it can
						// get its state committed.
						ActiveEditor.Save(fileName);

						textBoxSequenceName.Text = ActiveEditor.Sequence.Name;
						MessageBox.Show("Success");
					}
				} catch(Exception ex) {
					MessageBox.Show(ex.Message);
				} finally {
					Cursor = Cursors.Default;
				}
			}
        }

        private void buttonReadSequence_Click(object sender, EventArgs e) {
            Cursor = Cursors.WaitCursor;
            try {
				// Go through the editor
				Sequence sequence = Vixen.Sys.Sequence.Load(textBoxSequenceFileName.Text);
				IEditorUserInterface editor = _GetEditor(textBoxSequenceFileName.Text);
				if(editor != null) {
					if(sequence != null) {
						editor.Sequence = sequence;
						SequenceName = editor.Sequence.Name;
						_OpenEditor(editor);
					} else {
						MessageBox.Show("Could not load sequence.");
					}
				} else {
					MessageBox.Show("Could not load editor.");
				}
			} catch(Exception ex) {
                MessageBox.Show(ex.Message);
            } finally {
                Cursor = Cursors.Default;
            }
        }

        private void _AddControllerToView(OutputController controller) {
            ListViewItem item = new ListViewItem(new string[] {
                controller.Name,
				controller.OutputCount.ToString(),
				ApplicationServices.GetModuleDescriptor<IOutputModuleDescriptor>(controller.OutputModuleId).TypeName
            });
            item.Tag = controller;
            listViewControllers.Items.Add(item);
        }

		private OutputController _SelectedController {
			get {
				if(listViewControllers.SelectedItems.Count > 0) {
					return listViewControllers.SelectedItems[0].Tag as OutputController;
				}
				return null;
			}
		}

		private CommandStandard.Standard.CombinationOperation _CombinationStrategy {
			get {
				return (CommandStandard.Standard.CombinationOperation)Enum.Parse(typeof(CommandStandard.Standard.CombinationOperation), comboBoxCombinationStrategy.SelectedItem as string);
			}
			set {
				comboBoxCombinationStrategy.SelectedItem = value.ToString();
			}
		}

		private void buttonAddController_Click(object sender, EventArgs e) {
            string controllerName = textBoxControllerName.Text.Trim();
            if(controllerName.Length != 0) {
				OutputController controller = new OutputController(controllerName, (int)numericUpDownOutputCount.Value, (Guid)comboBoxOutputModule.SelectedValue, _CombinationStrategy);
				controller.Save();
				_AddControllerToView(controller);
				_controllers.Add(controller);
				comboBoxLinkedTo.DataSource = null;
				comboBoxLinkedTo.DisplayMember = "Name";
				comboBoxLinkedTo.ValueMember = "Id";
				comboBoxLinkedTo.DataSource = _controllers;
            }
        }

		public IEditorUserInterface ActiveEditor {
			get {
				// Don't want to clear our reference on Deactivate because
				// it may be deactivated due to the client getting focus.
				if(_activeEditor != null && _activeEditor.IsDisposed) {
					_activeEditor = null;
					labelActiveEditor.Text = "(none)";
				}
				return _activeEditor; 
			}
        }

		public IEditorUserInterface[] AllEditors {
			get { return _openEditors.ToArray(); }
		}

        public AppCommand AppCommands { get; private set; }

		private IEditorUserInterface _GetEditor(string sequenceFileName) {
			return ApplicationServices.GetEditor(sequenceFileName);
		}

		private IEditorUserInterface _GetEditor(Guid editorModuleId) {
			return ApplicationServices.GetEditor(editorModuleId);
		}

		private void buttonUpdateController_Click(object sender, EventArgs e) {
			OutputController controller = _SelectedController;
			if(controller != null) {
				controller.CombinationStrategy = _CombinationStrategy;
				controller.OutputCount = (int)numericUpDownOutputCount.Value;
				controller.OutputModuleId = (Guid)comboBoxOutputModule.SelectedValue;
				controller.Save(controller.FilePath);
			}
		}

		private void listViewControllers_SelectedIndexChanged(object sender, EventArgs e) {
			OutputController controller = _SelectedController;
			if(controller != null) {
				textBoxControllerName.Text = controller.Name;
				numericUpDownOutputCount.Value = controller.OutputCount;
				comboBoxOutputModule.SelectedValue = controller.OutputModuleId;
				_CombinationStrategy = controller.CombinationStrategy;
				_UpdateLinkCombo();
			}
		}

		private void buttonRemoveControllerLink_Click(object sender, EventArgs e) {
			_SelectedController.LinkTo(null);
			_UpdateLinkCombo();
		}

		private void _UpdateLinkCombo() {
			OutputController controller = _SelectedController;
			comboBoxLinkedTo.SelectedValue = (controller.Prior == null) ? Guid.Empty : controller.Prior.Id;
		}

		private void buttonLinkController_Click(object sender, EventArgs e) {
            if (comboBoxLinkedTo.SelectedValue != null) {
                _SelectedController.LinkTo(_controllers.First(x => x.Id == (Guid)comboBoxLinkedTo.SelectedValue));
            }
		}

		private void buttonFileTemplateSetup_Click(object sender, EventArgs e) {
			if(comboBoxFileTemplates.SelectedItem != null) {
				Guid id = (Guid)comboBoxFileTemplates.SelectedValue;
				// Get the template module instance.
				IModuleTemplate template = ApplicationServices.Get<IModuleTemplateModuleInstance>(id) as IModuleTemplate;
				// Setup!
				template.Setup();
			}
		}

		private void buttonUnloadModule_Click(object sender, EventArgs e) {
			//if(treeViewLoadedModules.SelectedNode != null && treeViewLoadedModules.SelectedNode.Level == 1) {
			//    IModuleDescriptor descriptor = treeViewLoadedModules.SelectedNode.Tag as IModuleDescriptor;
			//    string moduleType = treeViewLoadedModules.SelectedNode.Parent.Text;
			//    ApplicationServices.UnloadModule(descriptor.TypeId, moduleType);
			//    treeViewLoadedModules.Nodes.Remove(treeViewLoadedModules.SelectedNode);
			//}
		}

		private void buttonRefreshLoadedModules_Click(object sender, EventArgs e) {
			_DisplayLoadedModules();
		}

		private void treeViewSystemFixtures_AfterSelect(object sender, TreeViewEventArgs e) {
			// Must be on a channel to remove a channel.
			buttonRemoveSystemFixtureChannel.Enabled =
				treeViewSystemChannels.SelectedNode != null && treeViewSystemChannels.SelectedNode.Level == 1;
		}

		private void buttonAddSystemFixtureChannel_Click(object sender, EventArgs e) {
			using(CommonElements.TextDialog textDialog = new CommonElements.TextDialog("New channel name")) {
				if(textDialog.ShowDialog() == DialogResult.OK) {
					OutputChannel channel = Vixen.Sys.Execution.AddChannel(textDialog.Response);
					Vixen.Sys.Execution.Nodes.AddChannelLeaf(channel);
				}
			}
		}

		private void buttonRemoveSystemFixtureChannel_Click(object sender, EventArgs e) {
			ChannelNode node = treeViewSystemChannels.SelectedNode.Tag as ChannelNode;
			if(node != null) {
				Vixen.Sys.Execution.RemoveChannel(node.Channel);
				Vixen.Sys.Execution.Nodes.RemoveChannelLeaf(node.Channel);
			}
		}

		private void buttonPatchSystemChannels_Click(object sender, EventArgs e) {
			using(ManualPatchDialog patchDialog = new ManualPatchDialog(OutputController.GetAll().ToArray())) {
				patchDialog.ShowDialog();
			}
		}

		private void buttonFireEffect_Click(object sender, EventArgs e) {
			if(comboBoxEffects.SelectedItem != null && treeViewSystemChannels.SelectedNode != null) {
				IEffectModuleInstance effect = comboBoxEffects.SelectedItem as IEffectModuleInstance;
				object[] parameterValues = null;
				if(_editorControl != null) {
					parameterValues = _editorControl.EffectParameterValues;
				}
				Command command = new Command(effect.Descriptor.TypeId, parameterValues);
				ChannelNode node = treeViewSystemChannels.SelectedNode.Tag as ChannelNode;
				try {
					CommandNode commandNode = new CommandNode(command, new[] { node }, 0, (long)numericUpDownEffectLength.Value);
					Vixen.Sys.Execution.Write(new[] { commandNode });
				} catch(Exception ex) {
					MessageBox.Show(ex.Message);
				}
			}
		}

		private void comboBoxEffects_SelectedIndexChanged(object sender, EventArgs e) {
			if(_SelectedCommand != null) {
				_editorControl = _GetEffectEditor(_SelectedCommand);
				panelEditorControlContainer.Controls.Clear();
				if(_editorControl != null) {
					panelEditorControlContainer.Controls.Add(_editorControl as Control);
				}
			}
		}

		private IEffectEditorControl _GetEffectEditor(IEffectModuleInstance effect) {
			IEffectEditorControl editorControl = null;
			if(effect != null) {
				editorControl = ApplicationServices.GetEffectEditorControl(effect.Descriptor.TypeId);
			}
			return editorControl;
		}

		private IEffectModuleInstance _SelectedCommand {
			get { return comboBoxEffects.SelectedItem as IEffectModuleInstance; }
		}

		private void buttonControllerSetup_Click(object sender, EventArgs e) {
			if(listViewControllers.SelectedItems.Count == 1) {
				OutputController controller = listViewControllers.SelectedItems[0].Tag as OutputController;
				if(!controller.Setup()) {
					MessageBox.Show("No setup.");
				}
			}
		}

		private void treeViewSystemFixtures_AfterCheck(object sender, TreeViewEventArgs e) {
			if((e.Action & (TreeViewAction.ByKeyboard | TreeViewAction.ByMouse)) != 0) {
				ChannelNode node = e.Node.Tag as ChannelNode;
				node.Masked = e.Node.Checked;
			}
		}

		private void _OpenEditor(IEditorUserInterface editorUI) {
			_openEditors.Add(editorUI);

			editorUI.Closing += (sender, e) => {
				if(!_CloseEditor(sender as IEditorUserInterface)) {
					e.Cancel = true;
				}
			};

			editorUI.Activated += (sender, e) => {
				_activeEditor = sender as IEditorUserInterface;
				labelActiveEditor.Text = _activeEditor.Sequence.Name;
			};
			
			editorUI.Start();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="editor"></param>
		/// <returns>True if the editor is to close.</returns>
		private bool _CloseEditor(IEditorUserInterface editor) {
			if(_openEditors.Contains(editor)) {
				if(editor.IsModified) {
					//*** give them the change to save, not save, or cancel the close
				}
				_openEditors.Remove(editor);
				editor.Dispose();
			}
			return true;
		}

		private void treeViewNodes_DragOver(object sender, DragEventArgs e) {
			// If the source is a leaf, copy it
			// If the source is not a leaf, move it
			ChannelNode draggingNode = e.Data.GetData(typeof(ChannelNode)) as ChannelNode;
			TreeNode treeNode = treeViewNodes.GetNodeAt(treeViewNodes.PointToClient(new Point(e.X, e.Y)));
			if(draggingNode != null && treeNode != null) {
				// Copy a leaf, move a branch.
				e.Effect = (draggingNode.IsLeaf || ((e.KeyState & 4) != 0) ) ? DragDropEffects.Copy : DragDropEffects.Move;
			} else {
				e.Effect = DragDropEffects.None;
			}
		}

		private void treeViewNodes_DragDrop(object sender, DragEventArgs e) {
			_dragCapture = false;
			ChannelNode draggingNode = e.Data.GetData(typeof(ChannelNode)) as ChannelNode;
			TreeNode treeNode = treeViewNodes.GetNodeAt(treeViewNodes.PointToClient(new Point(e.X, e.Y)));
			ChannelNode targetNode = treeNode.Tag as ChannelNode;

			if(e.Effect == DragDropEffects.Copy) {
				Vixen.Sys.Execution.Nodes.CopyNode(draggingNode, targetNode);
			} else if(e.AllowedEffect == DragDropEffects.Move) {
				//Vixen.Sys.Execution.Nodes.MoveNode(draggingNode, targetNode);
			}
		}

		private void treeViewNodes_MouseMove(object sender, MouseEventArgs e) {
			// Node is selected?
			// Left mouse button is down?
			// Mouse is captured?
			// Root is not selected?
			// Then start dragging.
			//For debugging ease...
			TreeNode selectedNode = treeViewNodes.GetNodeAt(e.Location);
			bool nodeSelected = selectedNode != null;
			bool leftButtonDown = (MouseButtons & MouseButtons.Left) != 0;
			bool captured = _dragCapture;
			bool notRoot = nodeSelected && selectedNode.Level > 0;
			if(nodeSelected && 
				leftButtonDown && 
				captured && 
				notRoot) {
				ChannelNode node = selectedNode.Tag as ChannelNode;
				// MUST call DoDragDrop on the control, not the form.
				// If it's called on the form, QueryContinueDrag and
				// GiveFeedback will not be raised.
				treeViewNodes.DoDragDrop(node, DragDropEffects.Copy | DragDropEffects.Move);
			}
		}

		private void treeViewNodes_GiveFeedback(object sender, GiveFeedbackEventArgs e) {
			//The GiveFeedback event is raised when a drag-and-drop operation is started. 
			//With the GiveFeedback event, the source of a drag event can modify the 
			//appearance of the mouse pointer in order to give the user visual feedback 
			//during a drag-and-drop operation.
			//
			//The DragOver and GiveFeedback events are paired so that as the mouse moves 
			//across the drop target, the user is given the most up-to-date feedback on 
			//the mouse's position.
		}

		private void treeViewNodes_QueryContinueDrag(object sender, QueryContinueDragEventArgs e) {
			//If there is a change in the keyboard or mouse button state, 
			//the QueryContinueDrag event is raised
			if(e.EscapePressed) {
				// Cancel the drag
				_dragCapture = false;
			}
		}

		private void treeViewNodes_MouseDown(object sender, MouseEventArgs e) {
			// Need to do this here so that a mouse down is required to start a drag.
			// Otherwise, canceling one with ESC and moving the mouse would qualify
			// as a drag.
			TreeNode selectedNode = treeViewNodes.GetNodeAt(e.Location);
			_dragCapture = selectedNode != null;
		}

		private void buttonCreateNodeTemplate_Click(object sender, EventArgs e) {
			ChannelNode node = treeViewNodes.SelectedNode.Tag as ChannelNode;
			if(!node.IsLeaf) {
				using(CommonElements.TextDialog textDialog = new CommonElements.TextDialog("Template name")) {
					if(textDialog.ShowDialog() == DialogResult.OK) {
						ChannelNodeDefinition template = new ChannelNodeDefinition(textDialog.Response, node);
						template.Save();
						MessageBox.Show("Template created.");
					}
				}
			}
		}

		private void buttonImportNodeTemplate_Click(object sender, EventArgs e) {
			if(comboBoxNodeTemplates.SelectedItem != null) {
				ChannelNodeDefinition template = comboBoxNodeTemplates.SelectedItem as ChannelNodeDefinition;
				if(template != null) {
					using(CommonElements.TextDialog textDialog = new CommonElements.TextDialog("Name")) {
						if(textDialog.ShowDialog() == DialogResult.OK) {
							template.Import(textDialog.Response);
							MessageBox.Show("Imported.");
						}
					}
				}
			}
		}

		private void buttonDeleteController_Click(object sender, EventArgs e) {
			if(_SelectedController != null) {
				_SelectedController.Delete();
				_LoadControllers();
			}
		}

		private void comboBoxControllerOutputsControllers_SelectedIndexChanged(object sender, EventArgs e) {
			_ListControllerOutputs();
		}

		private void _ListControllerOutputs() {
			OutputController controller = comboBoxControllerOutputsControllers.SelectedItem as OutputController;

			if(controller != null) {
				listBoxControllerOutputs.DisplayMember = "Name";
				listBoxControllerOutputs.ValueMember = "Name";
				listBoxControllerOutputs.DataSource = controller.Outputs;
			} else {
				listBoxControllerOutputs.DataSource = null;
			}
		}

		private void listBoxControllerOutputs_SelectedIndexChanged(object sender, EventArgs e) {
			OutputController.Output output = listBoxControllerOutputs.SelectedItem as OutputController.Output;
			if(output != null) {
				textBoxControllerOutputName.Text = output.Name;
			} else {
				textBoxControllerOutputName.Text = string.Empty;
			}
		}

		private void buttonUpdateControllerOutputName_Click(object sender, EventArgs e) {
			OutputController.Output output = listBoxControllerOutputs.SelectedItem as OutputController.Output;
			if(output != null) {
				output.Name = textBoxControllerOutputName.Text;
				_ListControllerOutputs();
			}
		}

		private void buttonCommitControllerOutputChanges_Click(object sender, EventArgs e) {
			OutputController controller = comboBoxControllerOutputsControllers.SelectedItem as OutputController;
			if(controller != null) {
				controller.Save();
				MessageBox.Show("Committed.");
			}
		}

		private void buttonNodeProperties_Click(object sender, EventArgs e) {
			ChannelNode channelNode = treeViewNodes.SelectedNode.Tag as ChannelNode;
			using(NodePropertiesDialog nodePropertiesDialog = new NodePropertiesDialog(channelNode)) {
				nodePropertiesDialog.ShowDialog();
			}
		}

		private void treeViewNodes_AfterSelect(object sender, TreeViewEventArgs e) {
			buttonNodeProperties.Enabled = treeViewNodes.SelectedNode != null;
		}

		private void buttonGetContextFileName_Click(object sender, EventArgs e) {
			if(openFileDialogContext.ShowDialog() == DialogResult.OK) {
				textBoxTargetPackageFile.Text = openFileDialogContext.FileName;
			}
		}

		private void buttonPackageContext_Click(object sender, EventArgs e) {
			Cursor = Cursors.WaitCursor;
			try {
				ApplicationServices.PackageSystemContext(textBoxTargetPackageFile.Text);
				MessageBox.Show("Package built.");
			} catch(Exception ex) {
				MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
			} finally {
				Cursor = Cursors.Default;
			}
		}

		private void buttonUnpackageContext_Click(object sender, EventArgs e) {
			Cursor = Cursors.WaitCursor;
			try {
				string rootPath = ApplicationServices.UnpackageSystemContext(textBoxTargetPackageFile.Text);
				MessageBox.Show("Package unwrapped at " + rootPath);
			} catch(Exception ex) {
				MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
			} finally {
				Cursor = Cursors.Default;
			}
		}

    }

}
