/******************************************************************************
  Copyright 2009-2012 dataweb GmbH
  This file is part of the NShape framework.
  NShape is free software: you can redistribute it and/or modify it under the 
  terms of the GNU General Public License as published by the Free Software 
  Foundation, either version 3 of the License, or (at your option) any later 
  version.
  NShape is distributed in the hope that it will be useful, but WITHOUT ANY
  WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
  A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
  You should have received a copy of the GNU General Public License along with 
  NShape. If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataweb.NShape.Advanced;


namespace Dataweb.NShape.WinFormsUI
{
	/// <summary>
	/// A simple text editor dialog used by ui type editors for text properties.
	/// </summary>
	[ToolboxItem(false)]
	public partial class TextEditorDialog : Form
	{
		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.TextUITypeEditorDialog" />.
		/// </summary>
		public TextEditorDialog()
		{
			SetStyle(ControlStyles.ContainerControl
			         | ControlStyles.OptimizedDoubleBuffer
			         | ControlStyles.ResizeRedraw
			         | ControlStyles.SupportsTransparentBackColor
			         , true);
			UpdateStyles();
			InitializeComponent();
			Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

			textBox.PreviewKeyDown += textBox_PreviewKeyDown;
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.TextUITypeEditorDialog" />.
		/// </summary>
		public TextEditorDialog(string text)
			: this()
		{
			textBox.Text = text;
			wantReturn = false;
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.TextUITypeEditorDialog" />.
		/// </summary>
		public TextEditorDialog(IEnumerable<string> lines)
			: this()
		{
			if (lines == null) throw new ArgumentNullException("lines");
			foreach (string line in lines)
				textBox.Text += line + Environment.NewLine;
			wantReturn = true;
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.TextUITypeEditorDialog" />.
		/// </summary>
		public TextEditorDialog(string text, ICharacterStyle characterStyle)
			: this(text)
		{
			if (characterStyle == null) throw new ArgumentNullException("characterStyle");
			Font font = ToolCache.GetFont(characterStyle);
			textBox.Font = (Font) font.Clone();
			font = null;
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.TextUITypeEditorDialog" />.
		/// </summary>
		public TextEditorDialog(string text, string fontFamilyName, float fontSizeInPts, System.Drawing.FontStyle fontStyle)
			: this(text)
		{
			if (fontFamilyName == null) throw new ArgumentNullException("fontFamilyName");
			textBox.Font = new Font(fontFamilyName, fontSizeInPts, fontStyle, GraphicsUnit.Point);
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.TextUITypeEditorDialog" />.
		/// </summary>
		public TextEditorDialog(IEnumerable<string> lines, CharacterStyle characterStyle)
			: this(lines)
		{
			if (characterStyle == null) throw new ArgumentNullException("characterStyle");
			Font font = ToolCache.GetFont(characterStyle);
			textBox.Font = (Font) font.Clone();
			font = null;
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.TextUITypeEditorDialog" />.
		/// </summary>
		public TextEditorDialog(IEnumerable<string> lines, string fontFamilyName, float fontSizeInPts,
		                        System.Drawing.FontStyle fontStyle)
			: this(lines)
		{
			if (fontFamilyName == null) throw new ArgumentNullException("fontFamilyName");
			textBox.Font = new Font(fontFamilyName, fontSizeInPts, fontStyle, GraphicsUnit.Point);
		}

		#region [Public] Properties

		/// <summary>
		/// Indicates whether the displayed text should automatically wrap words to the beginning of the new line when necessary.
		/// </summary>
		public bool WordWrap
		{
			get { return textBox.WordWrap; }
			set { textBox.WordWrap = value; }
		}


		/// <summary>
		/// Sets the horizontal alignment of the displayed text.
		/// </summary>
		public HorizontalAlignment TextAlignment
		{
			get { return textBox.SelectionAlignment; }
			set
			{
				textBox.SuspendLayout();
				int s = textBox.SelectionStart;
				int l = textBox.SelectionLength;
				textBox.SelectAll();
				textBox.SelectionAlignment = value;
				if (l == 0)
					textBox.Select(textBox.TextLength, l);
				else
					textBox.Select(s, l);
				textBox.ResumeLayout();
			}
		}


		/// <summary>
		/// Gets or sets the current text to be edited.
		/// </summary>
		[TypeConverter(typeof (TextTypeConverter))]
		public string ResultText
		{
			get { return textBox.Text; }
			set { textBox.Text = value; }
		}


		/// <summary>
		/// Gets or sets the current multiline text to be edited.
		/// </summary>
		[TypeConverter(typeof (TextTypeConverter))]
		public string[] Lines
		{
			get { return textBox.Lines; }
			set { textBox.Lines = value; }
		}


		/// <summary>
		/// Specifies whether the user can insert tab characters into the text using the TAB key.
		/// </summary>
		public bool WantTab
		{
			get { return !textBox.TabStop; }
			set { textBox.TabStop = !value; }
		}


		/// <summary>
		/// Specifies whether the user can insert line breaks using the RETURN key.
		/// </summary>
		public bool WantReturn
		{
			get { return wantReturn; }
			set { wantReturn = value; }
		}

		#endregion

		#region [Private] Methods

		private void okButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
		}


		private void cancelButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}


		private void TextEditor_FormClosed(object sender, FormClosedEventArgs e)
		{
			textBox.PreviewKeyDown -= textBox_PreviewKeyDown;
		}


		private void textBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			switch (e.KeyCode) {
				case Keys.Return:
					if (wantReturn) {
						if ((e.Modifiers & Keys.Control) != 0)
							okButton_Click(this, null);
					}
					else {
						if (!((e.Modifiers & Keys.Control) != 0))
							okButton_Click(this, null);
					}
					break;

				case Keys.Escape:
					cancelButton_Click(this, null);
					break;

				default:
					// do nothing
					break;
			}
		}

		#endregion

		#region Fields

		private bool wantReturn = false;

		#endregion
	}


	[ToolboxItem(false)]
	internal class TextEditorTextBox : RichTextBox
	{
		public TextEditorTextBox()
		{
			SetControlStyles();
		}


		public TextEditorTextBox(string text, Font font)
		{
			if (font == null) throw new ArgumentNullException("font");
			SetControlStyles();
			Font = font;
			Text = text;
		}


		public TextEditorTextBox(IEnumerable<string> lines, Font font)
		{
			if (lines == null) throw new ArgumentNullException("lines");
			if (font == null) throw new ArgumentNullException("font");
			SetControlStyles();
			Font = font;
			List<string> textLines = new List<string>(lines);
			textLines.CopyTo(Lines);
		}


		public HorizontalAlignment TextAlignment
		{
			get { return SelectionAlignment; }
			set
			{
				SuspendLayout();
				int s = SelectionStart;
				int l = SelectionLength;
				SelectAll();
				SelectionAlignment = value;
				if (l == 0)
					Select(TextLength, l);
				else
					Select(s, l);
				ResumeLayout();
			}
		}


		private void SetControlStyles()
		{
			SetStyle(
				//ControlStyles.AllPaintingInWmPaint
				//| ControlStyles.UserPaint
				//| ControlStyles.FixedHeight
				//| ControlStyles.FixedWidth
				//| 
				ControlStyles.OptimizedDoubleBuffer
				| ControlStyles.ResizeRedraw
				| ControlStyles.Selectable
				| ControlStyles.SupportsTransparentBackColor
				, true);
			UpdateStyles();
		}
	}
}