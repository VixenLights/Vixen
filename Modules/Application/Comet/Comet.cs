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
using Vixen.Module.Editor;


namespace Comet
{
	public partial class Comet : Form, IApplication
	{
		private Guid _guid = new Guid("7b903272-73d0-416c-94b1-6932758b1963");

		public Comet()
		{
			InitializeComponent();

			Vixen.Sys.VixenSystem.Start(this, false);

			initializeEditorTypes();
		}

		private void Comet_FormClosing(object sender, FormClosingEventArgs e)
		{
			Vixen.Sys.VixenSystem.Stop();
		}


		#region IApplication implemetation

		public AppCommand AppCommands { get; private set; }

		public Guid ApplicationId
		{
			get { return _guid; }
		}

		private IEditorModuleInstance _activeEditor = null;
		public IEditorModuleInstance ActiveEditor
		{
			get
			{
				// Don't want to clear our reference on Deactivate because
				// it may be deactivated due to the client getting focus.
				if ((_activeEditor as Form).IsDisposed)
				{
					_activeEditor = null;
				}
				return _activeEditor;
			}
		}

		private List<IEditorModuleInstance> _openEditors = new List<IEditorModuleInstance>();
		public IEditorModuleInstance[] AllEditors
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
					IEditorModuleInstance editor = ApplicationServices.GetEditor(editorModuleId);
					if (editor != null)
					{
						editor.NewSequence();
						_OpenEditor(editor);
					}
				};
				contextMenuStripNewSequence.Items.Add(item);
			}
		}

		private void _OpenEditor(IEditorModuleInstance editor)
		{
			_openEditors.Add(editor);
			Form editorForm = editor as Form;
			editorForm.FormClosing += (sender, e) =>
			{
				if (!_CloseEditor(sender as IEditorModuleInstance))
				{
					e.Cancel = true;
				}
			};
			editorForm.Activated += (sender, e) =>
			{
				_activeEditor = sender as IEditorModuleInstance;
			};
			editorForm.Show();
		}

		private bool _CloseEditor(IEditorModuleInstance editor)
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

	}
}
