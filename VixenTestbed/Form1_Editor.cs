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
					IEditorUserInterface editor = ApplicationServices.GetEditor(openFileDialog.FileName);
					if(editor != null) {
						Sequence sequence = Sequence.Load(openFileDialog.FileName);
						if(sequence != null) {
							editor.Sequence = sequence;
							editor.Start();
						} else {
							MessageBox.Show("The sequence could not be loaded.");
						}
					} else {
						MessageBox.Show("No editor supports the selected file.");
					}
				}
			} catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void buttonShowEditor_Click(object sender, EventArgs e) {
			try {
				Guid id = _SelectedEditorModule.Descriptor.TypeId;
				IEditorUserInterface editor = ApplicationServices.GetEditor(id);
				if(editor != null) {
					editor.NewSequence();
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
