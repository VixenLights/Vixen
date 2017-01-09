using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls.Theme;

namespace VixenModules.App.Shows
{
	public partial class ShowTypeEditor : TypeEditorBase
	{
		public ShowItem _showItem;
		public Guid _currentShowID;
		public List<Show> _shows;

		public ShowTypeEditor(ShowItem item, Guid currentShowID)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			_showItem = item;
			_shows = ShowsData.ShowList;
			_currentShowID = currentShowID;
			PopulateShowList();
			checkBoxStopCurrentShow.Checked = _showItem.Show_StopCurrentShow;
		}

		public void PopulateShowList() 
		{
			foreach (Show show in _shows)
			{
				Common.Controls.ComboBoxItem item = new Common.Controls.ComboBoxItem(show.Name, show);
				if (show.ID != _currentShowID)
				{
					comboBoxShow.Items.Add(item);
					if (show.ID == _showItem.Show_ShowID)
					{
						comboBoxShow.SelectedItem = item;
					}
				}
			}
		}

		private void checkBoxStopCurrentShow_CheckedChanged(object sender, EventArgs e)
		{
			_showItem.Show_StopCurrentShow = checkBoxStopCurrentShow.Checked;
		}

		private void comboBoxShow_SelectedIndexChanged(object sender, EventArgs e)
		{
			Common.Controls.ComboBoxItem comboBoxItem = (sender as ComboBox).SelectedItem as Common.Controls.ComboBoxItem;
			Show item = comboBoxItem.Value as Show;
			_showItem.Show_ShowID = item.ID;
			_showItem.Name = "Start show: " + item.Name;
			FireChanged(_showItem.Name);
		}

		private void comboBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			ThemeComboBoxRenderer.DrawItem(sender, e);
		}
	}
}
