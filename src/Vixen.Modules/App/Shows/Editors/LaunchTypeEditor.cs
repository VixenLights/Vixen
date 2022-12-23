﻿using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Resources;
using Common.Resources.Properties;

namespace VixenModules.App.Shows
{
	public partial class LaunchTypeEditor : TypeEditorBase
	{
		private readonly ShowItem _showItem;
		
		public LaunchTypeEditor(ShowItem item)
		{
			InitializeComponent();

			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			int iconSize = (int)(16 * ScalingTools.GetScaleFactor());
			buttonSelectProgram.Image = Tools.GetIcon(Resources.folder_explore, iconSize);
			buttonSelectProgram.Text = "";
			buttonTest.Image = Tools.GetIcon(Resources.cog_go, iconSize);
			buttonTest.Text = "";
			ThemeUpdateControls.UpdateControls(this);
			_showItem = item;
		}

		private void LaunchTypeEditor_Load(object sender, EventArgs e)
		{
			textBoxProgram.Text = _showItem.Launch_ProgramName;
			textBoxCommandLine.Text = _showItem.Launch_CommandLine;
			checkBoxShowCommandWindow.Checked = _showItem.Launch_ShowCommandWindow;
			checkBoxWaitForExit.Checked = _showItem.Launch_WaitForExit;
			UpdateItemName();
		}

		private void UpdateItemName()
		{
			_showItem.Name = Text = $@"Launch: {_showItem.Launch_ProgramName}";
			FireChanged(_showItem.Name);
		}

		private void buttonSelectProgram_Click(object sender, EventArgs e)
		{
			string filter = "Executable Files (*.exe)|*.exe|Command Files (*.com)|*.com|Batch Files (*.bat)|*.bat|Command Script (*.cmd)|*.cmd|All files (*.*)|*.*";

			openFileDialog.Filter = filter;

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				textBoxProgram.Text = openFileDialog.FileName;
			}
		}

		private void buttonTest_Click(object sender, EventArgs e)
		{
			LaunchTypeTester f = new LaunchTypeTester(_showItem);
			f.Show();
		}

		private void textBoxProgram_TextChanged(object sender, EventArgs e)
		{
			_showItem.Launch_ProgramName = textBoxProgram.Text;
			UpdateItemName();
		}

		private void textBoxCommandLine_TextChanged(object sender, EventArgs e)
		{
			_showItem.Launch_CommandLine = textBoxCommandLine.Text;
		}

		private void checkBoxShowCommandWindow_CheckedChanged(object sender, EventArgs e)
		{
			_showItem.Launch_ShowCommandWindow = checkBoxShowCommandWindow.Checked;
		}

		private void checkBoxWaitForExit_CheckedChanged(object sender, EventArgs e)
		{
			_showItem.Launch_WaitForExit = checkBoxWaitForExit.Checked;
		}
	}
}
