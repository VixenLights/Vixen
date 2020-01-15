using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Vixen.Commands;
using Vixen.Sys;

namespace VixenModules.Output.DebugController
{
	public partial class DebugControllerOutputForm : BaseForm
	{
		private Stopwatch _timer;
		private long _lastUpdateMs;

		public bool Verbose { get; set; }

		public bool AppendText { get; set; }

		public int MsPerUpdate { get; set; }


		public DebugControllerOutputForm()
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			chkVerbose.Checked = Verbose = false;
			AppendText = false;

			_timer = new Stopwatch();
			//MsPerUpdate = VixenSystem.DefaultUpdateInterval;
			_lastUpdateMs = int.MinValue;
			_timer.Start();
		}

		public void UpdateState(ICommand[] outputStates)
		{
			//if (_timer.ElapsedMilliseconds < _lastUpdateMs + MsPerUpdate)
			//	return;

			_lastUpdateMs = _timer.ElapsedMilliseconds;

			string text = string.Empty;

			string time = "[" + Vixen.Sys.Execution.CurrentExecutionTimeString + "]";

			int i = 0;
			foreach (ICommand command in outputStates) {
				bool showIfQuiet = true;

				string line = time + " [" + (i+1) + "]: ";

				if (command == null) {
					line += "null";
					showIfQuiet = false;
				} else if (command is _8BitCommand) {
					line += "8-bit: " + (command as _8BitCommand).CommandValue;
				} else if (command is _16BitCommand) {
					line += "16-bit: " + (command as _16BitCommand).CommandValue;
				} else if (command is _32BitCommand) {
					line += "32-bit: " + (command as _32BitCommand).CommandValue;
				} else if (command is _64BitCommand) {
					line += "64-bit: " + (command as _64BitCommand).CommandValue;
				} else if (command is ColorCommand) {
					line += "Color: " + (command as ColorCommand).CommandValue;
				} else if (command is StringCommand) {
					line += "String: " + (command as StringCommand).CommandValue;
				} else {
					line += "Unknown command type";
				}

				if (Verbose || showIfQuiet) {
					text += line;
					text += Environment.NewLine;
				}

				i++;
			}

			if (text.Length > 0) {
				text += Environment.NewLine;
				BeginInvoke(new Action<string>(UpdateTextBox), text);
			}
		}

		public void UpdateTextBox(string text)
		{
			if (AppendText)
				textBoxOutput.AppendText(text);
			else
				textBoxOutput.Text = text;
		}

		private void chkVerbose_CheckedChanged(object sender, EventArgs e)
		{
			Verbose = chkVerbose.Checked;
		}
	}
}
