using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace VixenModules.App.Shows
{
	public class LaunchAction: Action
	{
		private Process process = null;

		public LaunchAction(ShowItem showItem)
			: base(showItem)
		{
		}

		public override void Execute()
		{
			base.Execute();

			process = new Process();
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.FileName = ShowItem.Launch_ProgramName;
			process.StartInfo.Arguments = ShowItem.Launch_CommandLine;
			process.StartInfo.CreateNoWindow = !ShowItem.Launch_ShowCommandWindow;
			process.EnableRaisingEvents = true;

			ResultString = string.Empty;

			process.Exited += new EventHandler((sender, eventArgs) =>
			{
				ResultString = process.StandardOutput.ReadToEnd();
				if (ResultString == string.Empty)
					ResultString = process.StandardError.ReadToEnd();
				// If we waited until the process was complete to tell return control to the scheduler
				// tell it (by calling Complete) to continue on with life
				if (ShowItem.Launch_WaitForExit)
				{
					Complete();
				}
			});

			process.Start();

			// If we're not waiting for the exit to return control, signal that this process is coplete
			if (!ShowItem.Launch_WaitForExit)
			{
				Complete();
			}
		}

		public override void Stop()
		{
			if (process != null && !process.HasExited)
			{
				process.Kill();
				base.Stop();
			}
		}
	}
}
