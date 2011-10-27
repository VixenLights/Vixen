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
using Vixen.Module.Sequence;

namespace VixenTestbed {
	partial class Form1 {
		private IEditorModuleInstance _SelectedEditorModule {
			get { return moduleListEditor.GetSelectedModule<IEditorModuleInstance>(); }
		}

		private void moduleListEditor_SelectedModuleChanged(object sender, EventArgs e) {
			buttonLoadSequence.Enabled = _SelectedEditorModule != null;
			buttonShowEditor.Enabled = buttonLoadSequence.Enabled;
		}

		private void buttonLoadEditorSequence_Click(object sender, EventArgs e) {
			try {
				IEditorModuleDescriptor descriptor = _SelectedEditorModule.Descriptor as IEditorModuleDescriptor;
				openFileDialog.Filter = descriptor.TypeName + " files|" + string.Join(";", descriptor.FileExtensions.Select(x => "*" + x));
				openFileDialog.InitialDirectory = Vixen.Sys.Sequence.DefaultDirectory;

				if(openFileDialog.ShowDialog() == DialogResult.OK) {
					IEditorUserInterface editor = ApplicationServices.CreateEditor(openFileDialog.FileName);
					if(editor != null) {
						editor.Start();
					} else {
						MessageBox.Show("No editor supports the selected file.");
					}
				}
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void buttonShowEditor_Click(object sender, EventArgs e) {
			// Generally, the user will select a sequence type and receive the editor
			// that edits it.  But in this case, they are testing an editor so we're
			// changing the workflow a bit.
			try {
				// Get the editor's type id.
				Guid id = _SelectedEditorModule.Descriptor.TypeId;
				// Get its descriptor.
				IEditorModuleDescriptor descriptor = ApplicationServices.GetModuleDescriptor(id) as IEditorModuleDescriptor;
				// Get the first file type it edits.
				if(descriptor.FileExtensions.Length == 0) throw new Exception("Editor does not specify any file types.");
				string fileType = descriptor.FileExtensions.First();
				// Get the editor with a new sequence.
				IEditorUserInterface editor = ApplicationServices.CreateEditor(fileType);
				if(editor != null) {
					editor.Start();
				} else {
					MessageBox.Show("No editor supports the selected file.");
				}
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}
	}
}
