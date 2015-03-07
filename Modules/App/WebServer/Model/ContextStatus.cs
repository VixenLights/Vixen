using System;

namespace VixenModules.App.WebServer.Model
{
	public class ContextStatus:Status
	{
		public ContextStatus()
		{
			State = States.Stopped;
			Position = TimeSpan.Zero;
			Sequence = new Sequence()
			{
				Name = String.Empty,
				FileName = String.Empty
			};

		}
		public States State { get; set; }
		public Sequence Sequence { get; set; }
		public TimeSpan Position { get; set; }

		public enum States
		{
			Stopped,
			Playing,
			Paused
		}
	}

	
}
