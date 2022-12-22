﻿namespace Vixen.Sys
{
	public class SequenceEventArgs : EventArgs
	{
		public SequenceEventArgs(ISequence sequence)
		{
			Sequence = sequence;
		}

		public ISequence Sequence { get; private set; }
	}
}