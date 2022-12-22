namespace Vixen.Module.Trigger
{
	public class TriggerSetEventArgs : EventArgs
	{
		public TriggerSetEventArgs(ITriggerInput trigger)
		{
			Trigger = trigger;
		}

		public ITriggerInput Trigger { get; private set; }
	}
}