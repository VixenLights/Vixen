using System.Windows.Forms;
using ScriptSequence.Script;

namespace ScriptEditor {
	public partial class SourceFileTabPage : UserControl {
		private SourceFile _sourceFile;

		public SourceFileTabPage(SourceFile sourceFile) {
			InitializeComponent();
			SourceFile = sourceFile;
		}

		public SourceFile SourceFile {
			get {
				Commit();
				return _sourceFile; 
			}
			set {
				_sourceFile = value;
				richTextBox.Text = value.Contents;
			}
		}

		public void Commit() {
			_sourceFile.Contents = richTextBox.Text;
			richTextBox.Modified = false;
		}

		public bool IsModified {
			get { return richTextBox.Modified; }
		}

	}
}
