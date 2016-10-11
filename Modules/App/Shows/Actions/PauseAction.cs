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
		private readonly Timer _timer;

		public PauseAction(ShowItem showItem)
			: base(showItem)
		{
			_timer = new Timer(showItem.Pause_Seconds*1000);
		}

		public override void Execute()
		{
			base.Execute();

			_timer.Elapsed += Timer_Tick;
			_timer.Start();
		}

		public override void Stop()
		{
			if (_timer.Enabled)
			{
				_timer.Stop();
				_timer.Elapsed -= Timer_Tick;
				base.Complete();
			}
		}

		public void Timer_Tick(object sender, EventArgs e) 
		{
			//Timer timer = (sender as Timer);
			_timer.Stop(); 
			_timer.Elapsed -= Timer_Tick;
			base.Complete();
		}
	}
}
