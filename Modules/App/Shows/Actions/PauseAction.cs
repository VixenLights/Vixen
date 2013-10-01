using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Timers;

namespace VixenModules.App.Shows
{
	public class PauseAction: Action
	{
		private Process process = null;
		private Timer timer;

		public PauseAction(ShowItem showItem)
			: base(showItem)
		{
			timer = new Timer(showItem.Pause_Seconds);
		}

		public override void Execute()
		{
			base.Execute();

			timer.Elapsed += Timer_Tick;
			timer.Start();
		}

		public override void Stop()
		{
			if (timer.Enabled)
			{
				timer.Stop();
				timer.Elapsed -= Timer_Tick;
				base.Complete();
			}
		}

		public void Timer_Tick(object sender, EventArgs e) 
		{
			Timer timer = (sender as Timer);
			timer.Stop(); 
			timer.Elapsed -= Timer_Tick;
			base.Complete();
		}
	}
}
