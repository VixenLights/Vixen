using System.Windows.Forms;
using Vixen.Module.Effect;
using Vixen.Module.EffectEditor;
using VixenModules.Effect.ImageGrid;

namespace VixenModules.EffectEditor.FilePathTypeEditor {
	public partial class FilePathEditorControl : UserControl, IEffectEditorControl {
		public FilePathEditorControl() {
			InitializeComponent();
		}

		public object[] EffectParameterValues {
			get { return new[] { new FilePath(textBoxFilePath.Text) }; }
			set { textBoxFilePath.Text = ((FilePath)value[0]).Value; }
		}

		public IEffect TargetEffect { get; set; }

		private void buttonSelect_Click(object sender, System.EventArgs e) {
			if(openFileDialog.ShowDialog() == DialogResult.OK) {
				textBoxFilePath.Text = openFileDialog.FileName;
			}
		}
	}
}
