using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Common.Controls
{
	public partial class BulkRename : Form
	{
		private enum eRenameMode
		{
			LettersFirst,
			NumbersFirst
		}


		public BulkRename(string[] oldNames)
		{
			InitializeComponent();
			OldNames = oldNames;
		}

		private string[] _oldNames;
		public string[] OldNames
		{
			get { return _oldNames; }
			set { _oldNames = value; UpdateNamesList(); }
		}

		private string[] _newNames;
		public string[] NewNames
		{
			get { return _newNames; }
			set { _newNames = value; UpdateNamesList(); }
		}

		private eRenameMode RenameMode
		{
			get
			{
				if (radioButtonNumbersFirst.Checked)
					return eRenameMode.NumbersFirst;

				return eRenameMode.LettersFirst;
			}
			set
			{
				if (value == eRenameMode.LettersFirst)
					radioButtonLettersFirst.Checked = true;
				else
					radioButtonNumbersFirst.Checked = true;
			}
		}

		private void RenameAllItems()
		{
			int totalItems = OldNames.Count();
			int startNumber = (int)numericUpDownStartNumber.Value;
			char[] letters = textBoxLetters.Text.ToCharArray();
			string pattern = textBoxPattern.Text;
			int maxNumber = startNumber + (int)Math.Ceiling(totalItems / (double)(letters.Length != 0 ? letters.Length : 1)) - 1;

			int currentLetterIndex = 0;
			int currentNumber = startNumber;

			List<string> result = new List<string>();

			for (int i = 0; i < totalItems; i++) {
				string output = pattern.Replace("#", currentNumber.ToString());

				if (letters.Length > 0)
					output = output.Replace('%', letters[currentLetterIndex]);

				result.Add(output);

				if (RenameMode == eRenameMode.LettersFirst) {
					if (++currentLetterIndex >= letters.Length) {
						currentLetterIndex = 0;
						currentNumber++;
					}
				} else {
					if (++currentNumber > maxNumber) {
						currentNumber = startNumber;
						currentLetterIndex++;
					}
				}
			}

			NewNames = result.ToArray();
		}

		private void UpdateNamesList()
		{
			listViewNames.BeginUpdate();
			listViewNames.Items.Clear();

			for (int i = 0; i < OldNames.Length; i++) {
				ListViewItem item = new ListViewItem();
				item.Text = OldNames[i];
				if (NewNames != null && NewNames.Length > i)
					item.SubItems.Add(NewNames[i]);
				else
					item.SubItems.Add("-");

				listViewNames.Items.Add(item);
			}

			listViewNames.EndUpdate();
		}

		private void BulkRename_Load(object sender, EventArgs e)
		{
			ResizeListviewColumns();
			RenameMode = eRenameMode.LettersFirst;
			textBoxLetters.Text = "RGB";
			textBoxPattern.Text = "Sample-#-%";
			numericUpDownStartNumber.Value = 1;
			RenameAllItems();
		}

		private void ResizeListviewColumns()
		{
			int width = (listViewNames.Width - SystemInformation.VerticalScrollBarWidth) / 2;
			listViewNames.Columns[0].Width = width;
			listViewNames.Columns[1].Width = width;
		}

		private void listViewNames_Resize(object sender, EventArgs e)
		{
			ResizeListviewColumns();
		}

		private void textBoxPattern_TextChanged(object sender, EventArgs e)
		{
			RenameAllItems();
		}

		private void textBoxLetters_TextChanged(object sender, EventArgs e)
		{
			RenameAllItems();
		}

		private void numericUpDownStartNumber_ValueChanged(object sender, EventArgs e)
		{
			RenameAllItems();
		}

		private void radioButtonLettersFirst_CheckedChanged(object sender, EventArgs e)
		{
			RenameAllItems();
		}

		private void radioButtonNumbersFirst_CheckedChanged(object sender, EventArgs e)
		{
			RenameAllItems();
		}
	}
}
