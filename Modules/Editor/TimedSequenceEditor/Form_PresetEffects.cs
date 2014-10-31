using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
using Common.Controls.Timeline;
using Vixen.Module.App;
using Vixen.Module.Effect;
using Vixen.Services;
using Vixen.Sys;
using VixenModules.App.PresetEffects;
using WeifenLuo.WinFormsUI.Docking;
using Common.Resources.Properties;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class Form_PresetEffects : DockContent
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private string _lastFolder;
		private TreeNode _mNode;
		private PresetEffectsLibrary _presetEffectsLibrary;
		private bool _beginDragDrop;
		public event EventHandler EscapeDrawMode;
		public TimelineControl TimelineControl { get; set; }
		private ContextMenuStrip contextMenuStrip = new ContextMenuStrip();

		public Form_PresetEffects(TimelineControl timelineControl)
		{
			InitializeComponent();
			TimelineControl = timelineControl;

			toolStripButtonExport.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonExport.Image = Resources.disk;

			toolStripButtonImport.DisplayStyle = ToolStripItemDisplayStyle.Image;
			toolStripButtonImport.Image = Resources.folder_open;
		}

		private void Form_PresetEffects_Load(object sender, EventArgs e)
		{
			_presetEffectsLibrary = ApplicationServices.Get<IAppModuleInstance>(PresetEffectsLibraryDescriptor.ModuleID) as PresetEffectsLibrary;
			if (_presetEffectsLibrary != null)
			{
				_presetEffectsLibrary.PresetEffectsChanged += PresetEffectsLibrary_Changed;
			}

			treeEffects.Sorted = true;
			LoadAvailableEffectImages();
			LoadCustomEffects();
		}

		private void Form_PresetEffects_FormClosing(object sender, FormClosingEventArgs e)
		{
			_presetEffectsLibrary.PresetEffectsChanged -= PresetEffectsLibrary_Changed;
		}

		#region Effect Loading

		private void LoadAvailableEffectImages()
		{
			foreach (
				IEffectModuleDescriptor effectDesriptor in
					ApplicationServices.GetModuleDescriptors<IEffectModuleInstance>().Cast<IEffectModuleDescriptor>())
			{
				Image image = effectDesriptor.GetRepresentativeImage(48, 48);
				if (image != null)
				{
					effectTreeImages.Images.Add(effectDesriptor.EffectName, image);
				}
			}
		}

		private void LoadCustomEffects()
		{
			treeEffects.Nodes[0].Nodes.Clear();

			foreach (KeyValuePair<Guid, PresetEffect> presetEffect in _presetEffectsLibrary)
			{
				TreeNode node = new TreeNode(presetEffect.Value.EffectTypeName + ": " + presetEffect.Value.Name) {Tag = presetEffect.Key};
				treeEffects.Nodes["treePreset"].Nodes.Add(node);
				SetNodeImage(node, presetEffect.Value.EffectTypeName.Contains("LipSync") ? "LipSync" : presetEffect.Value.EffectTypeName);
			}

			if (treeEffects.Nodes["treePreset"].Nodes.Count > 0)
			{
				treeEffects.Nodes[0].Expand();
			}

		}

		#endregion Effect Loading

		#region TreeView Event Handlers

		private void treeEffects_MouseDown(object sender, MouseEventArgs e)
		{
			_mNode = treeEffects.GetNodeAt(e.X, e.Y);

			_beginDragDrop = 
				(_mNode != null && _mNode.Nodes.Count == 0) &&
				(e.Button == MouseButtons.Left && e.Clicks == 1);
		}

		private void treeEffects_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left && _beginDragDrop)
			{
				_beginDragDrop = false;
				DataObject data = new DataObject(DataFormats.Serializable, _mNode.Tag);
				DoDragDrop(data, DragDropEffects.Copy);
			}
		}

		private void treeEffects_MouseClick(object sender, MouseEventArgs e)
		{
			if (_mNode == null)
				return;

			if (e.Button == MouseButtons.Right && _mNode.Nodes.Count == 0)
			{
				if (_mNode.Parent == null || _mNode.Parent.Name != "treePreset") return;

				contextMenuStrip.Items.Clear();

				ToolStripMenuItem toolStripMenuItemDelete = new ToolStripMenuItem("Delete " + _mNode.Text);
				toolStripMenuItemDelete.Click += (mySender, myE) =>
				{
					var dr = MessageBox.Show(@"Are you sure you want to delete this preset effect?", @"Delete preset effect",
						MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
					if (dr != DialogResult.Yes) return;
					DeselectAllNodes();
					_presetEffectsLibrary.RemovePresetEffect((Guid)_mNode.Tag);
				};
				contextMenuStrip.Items.Add(toolStripMenuItemDelete);
				contextMenuStrip.Show(treeEffects,e.X,e.Y);
			}

			//From here is all about the left mouse button, if thats not what we have just return
			if (e.Button != MouseButtons.Left) return;

			//If this is a group, expand or collapse it
			if (_mNode.Nodes.Count > 0)
			{
				if (_mNode.IsExpanded)
				{
					_mNode.Collapse();
				}
				else
				{
					_mNode.Expand();
				}
			}

			else //This isn't a group or the group has no children
			{
				//Make sure this isn't just an empty group
				if (_mNode.Level != 0)
				{
					SelectNodeForDrawing();
				}
			}
		}

		private void treeEffects_AfterExpand(object sender, TreeViewEventArgs e)
		{
			SetNodeImage(e.Node, "bullet_arrow_down.png");
		}

		private void treeEffects_AfterCollapse(object sender, TreeViewEventArgs e)
		{
			SetNodeImage(e.Node, "bullet_arrow_Right.png");
		}

		private void treeEffects_AfterSelect(object sender, TreeViewEventArgs e)
		{
			TreeView treeView = (TreeView)sender;
			treeView.SelectedNode = null;
		}

		#endregion TreeView Event Handlers

		public void DeselectAllNodes()
		{
			TimelineControl.grid.SelectedEffect = Guid.Empty;
			TimelineControl.grid.Cursor = Cursors.Default;

			int i = 0;
			while (i < treeEffects.Nodes.Count)
			{
				foreach (TreeNode node in treeEffects.Nodes[i].Nodes)
				{
					node.BackColor = Color.Empty;
				}
				i++;
			}
		}

		private void SelectNodeForDrawing()
		{
			if (_mNode.BackColor == Color.Blue)
			{
				TimelineControl.grid.SelectedEffect = Guid.Empty;
				_mNode.BackColor = Color.Empty;
			}
			else
			{
				TimelineControl.grid.SelectedEffect = (Guid)_mNode.Tag;

				int i = 0;
				while (i < treeEffects.Nodes.Count)
				{
					foreach (TreeNode node in treeEffects.Nodes[i].Nodes)
					{
						node.BackColor = Color.Empty;
					}
					i++;
				}

				_mNode.BackColor = Color.Blue;
			}
		}

		private void SetNodeImage(TreeNode node, string image)
		{
			node.ImageKey = image;
			node.SelectedImageKey = node.ImageKey;
		}
		
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			//This prevents the treeview from handling keystrokes while it still has focus
			//Was causing an annoying ding when the user had not drawn anything
			if (keyData == Keys.Escape)
				EscapeDrawMode(this,EventArgs.Empty);
			base.ProcessCmdKey(ref msg, keyData);
			return true;
		}
		
		private void PresetEffectsLibrary_Changed(object sender, EventArgs e)
		{
			LoadCustomEffects();
		}


		private void toolStripButtonExport_Click(object sender, EventArgs e)
		{
			ExportPresetEffects();
		}

		private void toolStripButtonImport_Click(object sender, EventArgs e)
		{
			ImportPresetEffects();
		}

		private void ExportPresetEffects()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				DefaultExt = ".vpc",
				Filter = @"Vixen 3 Preset Effects (*.vpc)|*.vpc|All Files (*.*)|*.*"
			};

			if (_lastFolder != string.Empty) saveFileDialog.InitialDirectory = _lastFolder;
			if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
			_lastFolder = Path.GetDirectoryName(saveFileDialog.FileName);

			var xmlsettings = new XmlWriterSettings
			{
				Indent = true,
				IndentChars = "\t",
			};

			DataContractSerializer ser = new DataContractSerializer(typeof(PresetEffectsLibrary));
			var writer = XmlWriter.Create(saveFileDialog.FileName, xmlsettings);
			ser.WriteObject(writer, _presetEffectsLibrary);
			writer.Close();

			/*
			try
			{
				DataContractSerializer ser = new DataContractSerializer(typeof(KeyValuePair<Guid, PresetEffect>));
				var writer = XmlWriter.Create(saveFileDialog.FileName, xmlsettings);
				ser.WriteObject(writer, _presetEffectsLibrary);
				writer.Close();
			}
			catch (Exception ex)
			{
				Logging.Error("While exporting Preset Effects: " + saveFileDialog.FileName + " " + ex.InnerException);
				MessageBox.Show(@"Unable to export data, please check the error log for details", @"Unable to export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			 */
		}

		private void ImportPresetEffects()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				DefaultExt = ".vpc",
				Filter = @"Vixen 3 Preset Effects (*.vpc)|*.vpc|All Files (*.*)|*.*",
				FilterIndex = 0
			};

			if (_lastFolder != string.Empty) openFileDialog.InitialDirectory = _lastFolder;
			if (openFileDialog.ShowDialog() != DialogResult.OK) return;
			_lastFolder = Path.GetDirectoryName(openFileDialog.FileName);

			try
			{
				using (FileStream reader = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
				{
					DataContractSerializer ser = new DataContractSerializer(typeof(KeyValuePair<Guid, PresetEffect>));
					foreach (KeyValuePair<Guid, PresetEffect> presetEffect in (PresetEffectsLibrary)ser.ReadObject(reader))
					{
						if (_presetEffectsLibrary.Contains(presetEffect.Key))
						{
							Logging.Error("Duplicate GUID found while importing Preset Effects collection: " +
								presetEffect.Key + " " + presetEffect.Value.EffectTypeName + ": " + presetEffect.Value.Name);
							continue;
						}
							
						_presetEffectsLibrary.AddPresetEffect(presetEffect.Key, presetEffect.Value);
					}

				}

			}
			catch (Exception ex)
			{
				Logging.Error("Invalid file while importing Preset Effects: " + openFileDialog.FileName + " " + ex.InnerException);
				MessageBox.Show(@"Sorry, we didn't reconize the data in that file as valid Preset Effect data.", @"Invalid file", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}
	}
}
