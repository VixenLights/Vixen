using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;
using Vixen.Script;

namespace TestEditor {
	public partial class SourceFileTabPage : UserControl {
		private SourceFile _sourceFile;

		public SourceFileTabPage(SourceFile sourceFile) {
			InitializeComponent();
			this.SourceFile = sourceFile;
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
