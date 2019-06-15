using Common.Controls.Theme;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VixenModules.App.LipSyncApp
{
	public partial class LipSyncMultiPicSelect : Form
	{
		private Dictionary<string, ComboBox> comboBoxes;
		public LipSyncMultiPicSelect()
		{
			InitializeComponent();
			comboBoxes = new Dictionary<string, ComboBox>(); 
			comboBoxes.Add("AI", ai_comboBox);
			comboBoxes.Add("E",e_comboBox);
			comboBoxes.Add("ETC",etc_comboBox);
			comboBoxes.Add("FV",fv_comboBox);
			comboBoxes.Add("L",l_comboBox);
			comboBoxes.Add("MBP",mbp_comboBox);
			comboBoxes.Add("O",o_comboBox);
			comboBoxes.Add("REST",rest_comboBox);
			comboBoxes.Add("U",u_comboBox);
			comboBoxes.Add("WQ",wq_comboBox);
		}

		private void LipSyncMultiPicSelect_Load(object sender, EventArgs e)
		{
			infoLabel.Text = "Multiple images detected, "
				+ Environment.NewLine
				+ "Please select the desired mapping";

			this.ForeColor = ThemeColorTable.ForeColor;
			this.BackColor = ThemeColorTable.BackgroundColor;


			foreach (KeyValuePair<string, ComboBox> kvp in comboBoxes)
			{
				kvp.Value.Items.Add("<Unmapped>");
				kvp.Value.Items.Add("<Mapped>");
			}
			
			foreach (string fileName in DropFileNames)
			{
				foreach (KeyValuePair<string,ComboBox> kvp in comboBoxes)
				{
					kvp.Value.Items.Add(Path.GetFileName(fileName));
				}
			}

			//Check the mapping list supplied before the call.
			//If no mapping exists and the CurrentPhonemeIndex is more than the index of ComboBox, mark "Unmapped"
			//If mapping exists and the CurrentPhonemeIndex is more than the index of ComboBox, mark "Mapped"
			//In either case, if the ComboBoxIndex is equal to CurrentPhonemeIndex (+2?) then use the FileDrop

			int dropIndex = 0;

			SortedSet<string> phonemeTypes = new SortedSet<string>(); 
			foreach(PhonemeType tmpPhoneme in Enum.GetValues(typeof(PhonemeType)))
			{
				string phonemeString = tmpPhoneme.ToString();
				if (string.Compare(phonemeString, "Unknown") != 0)
				{
					phonemeTypes.Add(tmpPhoneme.ToString());
				}
			}
			
			foreach (string searchPhoneme in phonemeTypes)
			{
				Bitmap searchValue;
				if (string.Compare(CurrentPhonemeString,searchPhoneme) == 0)
				{
					comboBoxes[searchPhoneme].SelectedIndex = dropIndex + 2;
					dropIndex++;
				}
				else if ((dropIndex > 0) && (dropIndex < DropFileNames.Length))
				{
					comboBoxes[searchPhoneme].SelectedIndex = dropIndex + 2;
					dropIndex++;
				}
				else
				{
					if (CurrentMappings.TryGetValue(searchPhoneme,out searchValue) == true)
					{
						comboBoxes[searchPhoneme].SelectedIndex = 1;
					}
					else
					{
						comboBoxes[searchPhoneme].SelectedIndex = 0;
					}
				}
			}
		}

		private void filterSelections(string key, int selectedIndex)
		{
			if ((0 != selectedIndex) && (1 != selectedIndex))
			{
				PicMappings[key] = DropFileNames[selectedIndex - 2];
			}
			else if (0 == selectedIndex)
			{
				PicMappings[key] = "!";
			}
		}

		private void acceptButton_Click(object sender, EventArgs e)
		{
			PicMappings = new Dictionary<string, string>();

			filterSelections("AI", ai_comboBox.SelectedIndex);
			filterSelections("E",e_comboBox.SelectedIndex);
			filterSelections("ETC",etc_comboBox.SelectedIndex);
			filterSelections("FV",fv_comboBox.SelectedIndex);
			filterSelections("L",l_comboBox.SelectedIndex);
			filterSelections("MBP",mbp_comboBox.SelectedIndex);
			filterSelections("O",o_comboBox.SelectedIndex);
			filterSelections("REST",rest_comboBox.SelectedIndex);
			filterSelections("U",u_comboBox.SelectedIndex);
			filterSelections("WQ",wq_comboBox.SelectedIndex);

			this.DialogResult = DialogResult.OK;
		}

		public string[] DropFileNames { get; set; }
		public Dictionary<string,string> PicMappings { get; set; }
		public string CurrentPhonemeString { get; set; }
		public Dictionary<string,Bitmap> CurrentMappings { get; set; }
	}
}
