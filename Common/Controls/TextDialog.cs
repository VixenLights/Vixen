using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CommonElements {
	public partial class TextDialog : Form {
		public TextDialog(string prompt) {
			InitializeComponent();
			labelPrompt.Text = prompt;
		}

		public TextDialog(string prompt, string title)
			: this(prompt)
		{
			this.Text = title;
		}

		public TextDialog(string prompt, string title, string initialText, bool selectInitialText = false)
			: this(prompt, title)
		{
			textBoxResponse.Text = initialText;
			if (selectInitialText)
				textBoxResponse.SelectAll();
		}

		private void TextDialog_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Escape) DialogResult = DialogResult.Cancel;
			else if(e.KeyCode == Keys.Enter) DialogResult = DialogResult.OK;
		}

		public string Response {
			get { return textBoxResponse.Text; }
		}
	}
}
