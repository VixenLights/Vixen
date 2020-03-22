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

namespace VixenModules.App.SuperScheduler
{
	public partial class StatusForm : BaseForm
	{
		private SuperSchedulerData SchedulerData;
		private ScheduleExecutor Executor;

		public StatusForm(SuperSchedulerData data, ScheduleExecutor executor)
		{
			InitializeComponent();

			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			int iconSize = (int)(24 * ScalingTools.GetScaleFactor());
			buttonPauseShow.Image = Tools.GetIcon(Resources.control_pause, iconSize);
			buttonPauseShow.Text = "";
			buttonNextSong.Image = Tools.GetIcon(Resources.control_end, iconSize);
			buttonNextSong.Text = "";
			buttonStartScheduler.Image = Tools.GetIcon(Resources.control_play_blue, iconSize);
			buttonStartScheduler.Text = "";
			buttonStopNow.Image = Tools.GetIcon(Resources.control_stop_blue, iconSize);
			buttonStopNow.Text = "";
			buttonStopGracefully.Image = Tools.GetIcon(Resources.clock_stop, iconSize);
			buttonStopGracefully.Text = "";
			buttonViewLog.Image = Tools.GetIcon(Resources.document_notes, iconSize);
			buttonViewLog.Text = "";
			buttonPlayShowNow.Image = Tools.GetIcon(Resources.control_play, iconSize);
			buttonPlayShowNow.Text = "";
			buttonPlayShowGracefully.Image = Tools.GetIcon(Resources.clock_play, iconSize);
			buttonPlayShowGracefully.Text = "";

			ThemeUpdateControls.UpdateControls(this);

			ControlBox = false;
			SchedulerData = data;
			Executor = executor;
		}

		private void StatusForm_Load(object sender, EventArgs e)
		{
			RestoreScreenLocation(SchedulerData.StatusForm_Position);
			PopulateShowList();
			CheckButtons();
		}

		private void RestoreScreenLocation(Point location)
		{
			WindowState = FormWindowState.Normal;

			var desktopBounds = new Rectangle(location, Size);
			if (IsVisibleOnAnyScreen(desktopBounds))
			{
				StartPosition = FormStartPosition.Manual;
				DesktopBounds = desktopBounds;
			}
			else
			{
				// this resets the upper left corner of the window to windows standards
				StartPosition = FormStartPosition.WindowsDefaultLocation;
			}
		}

		private bool IsVisibleOnAnyScreen(Rectangle rect)
		{
			return Screen.AllScreens.Any(screen => screen.WorkingArea.Contains(rect.Location));
		}


		private void PopulateShowList()
		{
			List<Shows.Show> shows = Shows.ShowsData.ShowList;
			if (shows != null)
			{
				foreach (Shows.Show show in shows)
				{
					bool foundIt = false;
					foreach (Common.Controls.ComboBoxItem oldItem in comboBoxShows.Items)
					{
						Shows.Show comboBoxScheduleItem = oldItem.Value as Shows.Show;
						if (comboBoxScheduleItem.ID == show.ID)
						{
							oldItem.Text = show.Name;
							foundIt = true;
							break;
						}
					}

					if (!foundIt)
					{
						Common.Controls.ComboBoxItem newItem = new Common.Controls.ComboBoxItem(show.Name, show);
						comboBoxShows.Items.Add(newItem);
					}
				}
			}
		}

		public string Status
		{
			set
			{ 
				labelStatus.Text = value;
				CheckButtons();
			}
		}

		private void StatusForm_LocationChanged(object sender, EventArgs e)
		{
			SchedulerData.StatusForm_Position = Location;
		}

		private void StatusForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = (e.CloseReason == CloseReason.UserClosing);
		}

		int originalHeight = 0;
		private void buttonViewLog_Click(object sender, EventArgs e)
		{
			if (Height == 420)
			{
				Height = originalHeight;
			}
			else
			{
				originalHeight = Height;
				Height = 420;
			}
		}

		private delegate void AddLogEntryDelegate(string logEntry);
		public void AddLogEntry(string logEntry)
		{
			try
			{
				if (!IsDisposed)
				{
					if (this.InvokeRequired)
						this.Invoke(new AddLogEntryDelegate(AddLogEntry), logEntry);
					else
					{
						listBoxLog.Items.Insert(0, logEntry);
					}

					// If we've got lots of crap in the log, remove the last one.
					if (listBoxLog.Items.Count > 250)
					{
						listBoxLog.Items.Remove(listBoxLog.Items.Count - 1);
					}
				}
			}
			catch
			{
			}
		}

		private void buttonStopNow_Click(object sender, EventArgs e)
		{
			Executor.Stop(false);
			CheckButtons();
		}

		private void comboBoxShows_DropDown(object sender, EventArgs e)
		{
			PopulateShowList();
		}

		private void buttonStopGracefully_Click(object sender, EventArgs e)
		{
			Executor.Stop(true);
			CheckButtons();
		}

		private void buttonPlayShowNow_Click(object sender, EventArgs e)
		{
			Common.Controls.ComboBoxItem item = comboBoxShows.SelectedItem as Common.Controls.ComboBoxItem;
			if (item != null)
			{
				Shows.Show show = item.Value as Shows.Show;
				Executor.Stop(false);

				ScheduleItem scheduleItem = new ScheduleItem();
				scheduleItem.ShowID = show.ID;
				scheduleItem.Start(true);
			}
		}

		private void buttonStartScheduler_Click(object sender, EventArgs e)
		{
			Executor.Start();
			CheckButtons();
		}

		private void CheckButtons()
		{
			buttonStopNow.Enabled = !Executor.ManuallyDisabled;
			buttonStopGracefully.Enabled = !Executor.ManuallyDisabled;
			buttonStartScheduler.Enabled = Executor.ManuallyDisabled;

			//if (buttonStartScheduler.Enabled)
			//{
			//	buttonStartScheduler.BackgroundImage = imageButtons.Images["control_play.png"];
			//}
			//else
			//{
			//	buttonStartScheduler.BackgroundImage = imageButtons.Images["control_play_disabled.png"];
			//}

			//if (buttonStopNow.Enabled)
			//{
			//	buttonStopNow.BackgroundImage = imageButtons.Images["control_stop.png"];
			//}
			//else
			//{
			//	buttonStopNow.BackgroundImage = imageButtons.Images["control_stop_disabled.png"];
			//}

			//if (buttonStopGracefully.Enabled)
			//{
			//	buttonStopGracefully.BackgroundImage = imageButtons.Images["clock_stop.png"];
			//}
			//else
			//{
			//	buttonStopGracefully.BackgroundImage = imageButtons.Images["clock_stop_disabled.png"];
			//}
		}

		private void StatusForm_ResizeEnd(object sender, EventArgs e)
		{
		}

		private void StatusForm_Resize(object sender, EventArgs e)
		{
			groupBoxLog.Height = ClientSize.Height - groupBoxLog.Top - 10;
			listBoxLog.Height = groupBoxLog.Height - listBoxLog.Top - 10;
		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}
	}
}
