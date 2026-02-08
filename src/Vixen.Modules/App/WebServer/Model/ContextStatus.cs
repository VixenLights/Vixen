namespace VixenModules.App.WebServer.Model
{
	public class ContextStatus:Status
	{
		public ContextStatus()
		{
			State = States.Stopped;
			Position = TimeSpan.Zero;
			Sequence = new Presentation()
			{
				Name = String.Empty,
				Info = String.Empty
			};

		}
		public States State { get; set; }
		public Presentation Sequence { get; set; }
		public TimeSpan Position { get; set; }

		public enum States
		{
			Stopped,
			Playing,
			Paused
		}
	}

	
}
