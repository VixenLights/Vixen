using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Resources;
using Common.Resources.Properties;

namespace VixenModules.App.Shows
{
	public partial class ShowListForm : BaseForm
	{
		public ShowListForm(ShowsData data)
		{
			InitializeComponent();

			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			int iconSize = (int)(24 * ScalingTools.GetScaleFactor());
			buttonAdd.Image = Tools.GetIcon(Resources.add, iconSize);
			buttonAdd.Text = "";
			buttonDelete.Image = Tools.GetIcon(Resources.delete, iconSize);
			buttonDelete.Text = "";
			buttonEdit.Image = Tools.GetIcon(Resources.pencil, iconSize);
			buttonEdit.Text = "";
			buttonHelp.Image = Tools.GetIcon(Resources.help, iconSize);
			ThemeUpdateControls.UpdateControls(this);

			Data = data;
		}

		public ShowsData Data { get; set; }

		#region Form Events

		private void buttonHelp_Click(object sender, EventArgs e)
		{
			Common.VixenHelp.VixenHelp.ShowHelp(Common.VixenHelp.VixenHelp.HelpStrings.Show_Editor);
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}

		private void ShowListForm_Load(object sender, EventArgs e)
		{
			PopulateShowList(null);
		}

		private void buttonAdd_Click(object sender, EventArgs e)
		{
			Show show = Data.AddShow();
			PopulateShowList(show);
		}

		private void listViewShows_DoubleClick(object sender, EventArgs e)
		{
			Point pt = listViewShows.PointToClient(new Point(MousePosition.X, MousePosition.Y));
			ListViewItem lvItem = listViewShows.GetItemAt(pt.X, pt.Y);
			lvItem.Selected = true;
			EditSelectedShow();
		}

		private void buttonEdit_Click(object sender, EventArgs e)
		{
			EditSelectedShow();
		}
		
		private void buttonDelete_Click(object sender, EventArgs e)
		{
			DeleteSelectedShow();
		}

#endregion // Form Events

		protected void PopulateShowList(Show selectedShow)
		{
			listViewShows.Items.Clear();
			foreach (Show show in Data.Shows)
			{
				ListViewItem lvItem = new ListViewItem(show.Name);
				lvItem.Tag = show;
				listViewShows.Items.Add(lvItem);
				if (show == selectedShow)
				{
					lvItem.Selected = true;
				}
			}
		}

		private void EditSelectedShow()
		{
			if (listViewShows.SelectedItems.Count > 0)
			{
				ListViewItem lvItem = listViewShows.SelectedItems[0];

				if (lvItem != null)
				{
					Show show = (lvItem.Tag as Show).Clone() as Show;
					ShowEditorForm form = new ShowEditorForm(show);
					if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					{
						int showIndex = Data.Shows.FindIndex(oldShow => oldShow == lvItem.Tag);
						Data.Shows[showIndex] = form.ShowData;
						lvItem.Tag = form.ShowData;
						lvItem.Text = form.ShowData.Name;
					}
				}
			}
		}

		private void DeleteSelectedShow()
		{
			if (listViewShows.SelectedItems.Count > 0)
			{
				ListViewItem lvItem = listViewShows.SelectedItems[0];

				if (lvItem != null)
				{
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("Are you sure you want to delete the selected show?\r\n\r\n" + lvItem.Text + "\r\n\r\nYou CANNOT undo this!", "Delete Show", true, false);
					messageBox.ShowDialog();
					if (messageBox.DialogResult == DialogResult.OK)
					{
						Data.Shows.Remove(lvItem.Tag as Show);
						listViewShows.Items.Remove(lvItem);
					}
				}
			}
		}

		private void listViewShows_AfterLabelEdit(object sender, LabelEditEventArgs e)
		{
			if (e.Label != null && e.Label.Length > 0)
			{
				ListViewItem lvItem = listViewShows.Items[e.Item];

				if (lvItem != null)
				{
					(lvItem.Tag as Show).Name = e.Label;
				}
			}
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImage;
		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}
	}
}
