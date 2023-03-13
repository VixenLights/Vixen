namespace Vixen.Sys
{
	public class ExecutorMessageEventArgs : EventArgs
	{
		public ExecutorMessageEventArgs(ISequence sequence, string value)
		{
			Sequence = sequence;
			Message = value;
		}

		public ISequence Sequence { get; private set; }
		public string Message { get; private set; }
	}
}