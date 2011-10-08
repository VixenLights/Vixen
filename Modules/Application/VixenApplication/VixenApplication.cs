using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;
using Vixen.Module.Editor;


namespace VixenApplication
{
	public partial class VixenApplication : Form, IApplication
	{
		private Guid _guid = new Guid("7b903272-73d0-416c-94b1-6932758b1963");

		public VixenApplication()
		{
			InitializeComponent();

			Vixen.Sys.VixenSystem.Start(this, false);

			initializeEditorTypes();
		}

		private void VixenApp_FormClosing(object sender, FormClosingEventArgs e)
		{
			Vixen.Sys.VixenSystem.Stop();
		}


		#region IApplication implemetation

		public AppCommand AppCommands { get; private set; }

		public Guid ApplicationId
		{
			get { return _guid; }
		}

		private IEditorUserInterface _activeEditor = null;
		public IEditorUserInterface ActiveEditor
		{
			get
			{
				// Don't want to clear our reference on Deactivate because
				// it may be deactivated due to the client getting focus.
				if (_activeEditor.IsDisposed)
				{
					_activeEditor = null;
				}
				return _activeEditor;
			}
		}

		private List<IEditorUserInterface> _openEditors = new List<IEditorUserInterface>();
		public IEditorUserInterface[] AllEditors
		{
			get { return _openEditors.ToArray(); }
		}
		#endregion

		#region Sequence Editor Type population & management
		private void initializeEditorTypes()
		{
			ToolStripMenuItem item;
			foreach (KeyValuePair<Guid, string> typeId_FileTypeName in ApplicationServices.GetAvailableModules<IEditorModuleInstance>())
			{
				item = new ToolStripMenuItem(typeId_FileTypeName.Value);
				item.Tag = typeId_FileTypeName.Key;
				item.Click += (sender, e) =>
				{
					ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
					Guid editorModuleId = (Guid)menuItem.Tag;
					IEditorUserInterface editor = ApplicationServices.GetEditor(editorModuleId);
					if(editor != null) {
						editor.NewSequence();
						_OpenEditor(editor);
					}
				};
				contextMenuStripNewSequence.Items.Add(item);
			}
		}

		private void _OpenEditor(IEditorUserInterface editorUI)
		{
			_openEditors.Add(editorUI);

			editorUI.Closing += (sender, e) => {
				if(!_CloseEditor(sender as IEditorUserInterface)) {
					e.Cancel = true;
				}
			};

			editorUI.Activated += (sender, e) => {
				_activeEditor = sender as IEditorUserInterface;
			};

			editorUI.Start();
		}

		private bool _CloseEditor(IEditorUserInterface editor)
		{
			if (_openEditors.Contains(editor))
			{
				_openEditors.Remove(editor);
				Form editorForm = editor as Form;
				editor.Dispose();
			}
			return true;
		}
		#endregion

		private void buttonNewSequence_Click(object sender, EventArgs e)
		{
			contextMenuStripNewSequence.Show(buttonNewSequence, new Point(0,buttonNewSequence.Height));
		}

		private void buttonOpenSequence_Click(object sender, EventArgs e)
		{

		}

		private void buttonSetupChannels_Click(object sender, EventArgs e)
		{
			ConfigChannels form = new ConfigChannels();
			form.ShowDialog();
		}

		private void buttonSetupOutputControllers_Click(object sender, EventArgs e)
		{
			ConfigControllers form = new ConfigControllers();
			form.ShowDialog();
		}

		private void buttonSetupPatches_Click(object sender, EventArgs e)
		{
			ConfigPatches form = new ConfigPatches();
			form.ShowDialog();
		}

		private void button1_Click_1(object sender, EventArgs e)
		{
			CommonElements.ColorManagement.ColorPicker.ColorPicker picker = new CommonElements.ColorManagement.ColorPicker.ColorPicker();
			picker.LockValue_V = true;
			picker.ShowDialog();
		}
	}
}
