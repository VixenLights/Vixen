using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;

namespace TestEditor {
	public partial class MediaDialog : Form {
		private MediaCollection _media;
		public MediaDialog(MediaCollection mediaCollection) {
			InitializeComponent();
			_media = mediaCollection;
			listBoxMedia.Items.AddRange(_media.Select(x => x.MediaFilePath).ToArray());
		}

		private void buttonAdd_Click(object sender, EventArgs e) {
			if(openFileDialog.ShowDialog() == DialogResult.OK) {
				try {
					_media.Add(openFileDialog.FileName);
					listBoxMedia.Items.Add(openFileDialog.FileName);
				} catch(NotImplementedException) {
					MessageBox.Show("The file type is not supported.");
				} catch(Exception ex) {
					MessageBox.Show(ex.Message);
				}
			}
		}

		private void buttonRemove_Click(object sender, EventArgs e) {
			if(listBoxMedia.SelectedIndex != -1) {
				_media.RemoveAt(listBoxMedia.SelectedIndex);
				listBoxMedia.Items.RemoveAt(listBoxMedia.SelectedIndex);
			}
		}

		private void buttonSetup_Click(object sender, EventArgs e) {
			if(listBoxMedia.SelectedIndex != -1) {
				_media[listBoxMedia.SelectedIndex].Setup();
			}
		}

		public IEnumerable<string> Files {
			get { return listBoxMedia.Items.Cast<string>(); }
		}
	}
}
