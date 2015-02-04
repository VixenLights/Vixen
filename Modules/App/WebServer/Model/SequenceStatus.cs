using System;

namespace VixenModules.App.WebServer.Model
{
	public class SequenceStatus:Status
	{
		public SequenceStatus()
		{
			SequenceState = State.Stopped;
			Position = TimeSpan.Zero;
			Name = String.Empty;
		}
		public State SequenceState { get; set; }
		public String Name { get; set; }
		public TimeSpan Position { get; set; }

		public enum State
		{
			Stopped,
			Playing,
			Paused
		}
	}

	
}
