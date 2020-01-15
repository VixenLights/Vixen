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
using Vixen.Sys;

namespace VixenModules.App.SuperScheduler
{
	public partial class SetupForm : BaseForm
	{
		private bool _isDirty;
		public SetupForm(SuperSchedulerData data)
		{
			InitializeComponent();

			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			int iconSize = (int)(24 * ScalingTools.GetScaleFactor());
			buttonAddSchedule.Image = Tools.GetIcon(Resources.add, iconSize);
			buttonAddSchedule.Text = "";
			buttonDeleteSchedule.Image = Tools.GetIcon(Resources.delete, iconSize);
			buttonDeleteSchedule.Text = "";
			buttonEditSchedule.Image = Tools.GetIcon(Resources.pencil, iconSize);
			buttonEditSchedule.Text = "";
			buttonEditShow.Image = Tools.GetIcon(Resources.table_edit, iconSize);
			buttonEditShow.Text = "";
			buttonHelp.Image = Tools.GetIcon(Resources.help, iconSize);

			ThemeUpdateControls.UpdateControls(this);
			
			Data = data;

			Closing += SetupForm_Closing;
		}

		private void SetupForm_Closing(object sender, CancelEventArgs e)
		{
			if (_isDirty)
			{
				VixenSystem.SaveModuleConfigAsync();
			}
		}

		public SuperSchedulerData Data { get; set; }

		private void buttonAddSchedule_Click(object sender, EventArgs e)
		{
			AddSchedule();
		}

		private void buttonHelp_Click(object sender, EventArgs e)
		{
			Common.VixenHelp.VixenHelp.ShowHelp(Common.VixenHelp.VixenHelp.HelpStrings.Scheduler_Main);
		}

		private void SetupForm_Load(object sender, EventArgs e)
		{
			_isDirty = false;
			PopulateListBox();
		}

		public void PopulateListBox()
		{
			listViewItems.Items.Clear();
			foreach (ScheduleItem item in Data.Items)
			{
				AddListItem(item);
			}
		}

		public string ScheduleString(ScheduleItem item)
		{
			string dateSt = item.StartDate.ToShortDateString() + " to " + item.EndDate.ToShortDateString();

			string daySt = String.Empty;
			if (item.Monday && item.Tuesday && item.Wednesday && item.Thursday && item.Friday && item.Saturday && item.Sunday)
				daySt = "Everyday";
			else if (item.Monday && item.Tuesday && item.Wednesday && item.Thursday && item.Friday && !(item.Saturday || item.Sunday))
				daySt = "Weekdays";
			else if (item.Saturday && item.Sunday && !(item.Monday || item.Tuesday || item.Wednesday || item.Thursday || item.Friday))
				daySt = "Weekends";
			else
			{
				if (item.Monday)
					daySt = daySt.Length > 0 ? daySt += ", Mon" : daySt = "Mon";
				if (item.Tuesday)
					daySt = daySt.Length > 0 ? daySt += ", Tue" : daySt = "Tue";
				if (item.Wednesday)
					daySt = daySt.Length > 0 ? daySt += ", Wed" : daySt = "Wed";
				if (item.Thursday)
					daySt = daySt.Length > 0 ? daySt += ", Thu" : daySt = "Thu";
				if (item.Friday)
					daySt = daySt.Length > 0 ? daySt += ", Fri" : daySt = "Fri";
				if (item.Saturday)
					daySt = daySt.Length > 0 ? daySt += ", Sat" : daySt = "Sat";
				if (item.Sunday)
					daySt = daySt.Length > 0 ? daySt += ", Sun" : daySt = "Sun";
			}
			if (daySt.Length == 0)
				daySt = "Never";

			string timeSt = item.StartTime.ToShortTimeString() + " - " + item.EndTime.ToShortTimeString();

			return dateSt + " @ " + timeSt + ", " + daySt;
		}

		private void buttonClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void UpdateListItem(ScheduleItem item)
		{
			foreach (ListViewItem listItem in listViewItems.Items)
			{
				if ((listItem.Tag as ScheduleItem) == item)
				{
					string status = item.Enabled ? "Enabled" : "Disabled";
					Shows.Show show = Shows.ShowsData.GetShow(item.ShowID);
					string showName = show != null ? show.Name : "None Selected";

					listItem.Text = showName;
					listItem.SubItems[1].Text = ScheduleString(item);
					listItem.SubItems[2].Text = status;
				}
			}
		}

		private void AddListItem(ScheduleItem item) 
		{
			string status = item.Enabled ? "Enabled" : "Disabled";
			Shows.Show show = Shows.ShowsData.GetShow(item.ShowID);
			string showName = show != null ? show.Name : "None Selected";

			ListViewItem lvItem = new ListViewItem(showName);
			lvItem.Tag = item;
			listViewItems.Items.Add(lvItem);
			lvItem.SubItems.Add(ScheduleString(item));
			lvItem.SubItems.Add(status);
		}

		private void buttonDeleteSchedule_Click(object sender, EventArgs e)
		{
			DeleteCurrentItem();
			_isDirty = true;
		}

		private void listViewItems_DoubleClick(object sender, EventArgs e)
		{
			EditCurrentItem();				
		}

		private void AddSchedule()
		{
			ScheduleItem item = new ScheduleItem();
			using (SetupScheduleForm f = new SetupScheduleForm(item))
			{
				if (f.ShowDialog() == DialogResult.OK)
				{
					Data.Items.Add(item);
					AddListItem(item);
					_isDirty = true;
				}
			}
		}

		private async void EditCurrentItem()
		{
			if (listViewItems.SelectedItems.Count == 1)
			{
				ListViewItem lvItem = listViewItems.SelectedItems[0];
				ScheduleItem scheduleItem = lvItem.Tag as ScheduleItem;
				using (SetupScheduleForm f = new SetupScheduleForm(scheduleItem))
				{
					if (f.ShowDialog() == DialogResult.OK)
					{
						UpdateListItem(scheduleItem);
						_isDirty = true;
					}
				}
			}
		}

		private async void DeleteCurrentItem()
		{
			if (listViewItems.SelectedItems.Count == 1)
			{
				ListViewItem lvItem = listViewItems.SelectedItems[0];
				ScheduleItem scheduleItem = lvItem.Tag as ScheduleItem;
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm("Are you sure you want to delete the selected schedule?", "Delete Schedule", true, false);
				messageBox.ShowDialog();
				if (messageBox.DialogResult == DialogResult.OK)
				{
					listViewItems.Items.Remove(lvItem);
					Data.Items.Remove(scheduleItem);
					_isDirty = true;
				}
			}
		}

		private void buttonEditSchedule_Click(object sender, EventArgs e)
		{
			EditCurrentItem();
		}

		private void buttonEditShow_Click(object sender, EventArgs e)
		{
			EditSelectedShow();
		}

		private void EditSelectedShow()
		{
			if (listViewItems.SelectedItems.Count == 1)
			{
				ListViewItem lvItem = listViewItems.SelectedItems[0];
				ScheduleItem scheduleItem = lvItem.Tag as ScheduleItem;
				Shows.Show show = scheduleItem.Show;
				if (show != null)
				{
					using (Shows.ShowEditorForm f = new Shows.ShowEditorForm(show))
					{
						if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
						{
							UpdateListItem(scheduleItem);
							_isDirty = true;
						}
					}
				}
				else
				{
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("The selected schedule does not have a show associated with it.", "Edit a Show", false, true);
					messageBox.ShowDialog();
				}
			}
		}

		private void addAScheduledShowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AddSchedule();
		}

		private void editTheSelectedScheduleToolStripMenuItem_Click(object sender, EventArgs e)
		{
			EditCurrentItem();
		}

		private void deleteTheSelectedScheduleToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DeleteCurrentItem();
		}

		private void editTheAssociatedShowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			EditSelectedShow();
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
