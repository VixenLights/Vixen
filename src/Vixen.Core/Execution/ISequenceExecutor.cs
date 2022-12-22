﻿using Vixen.Sys;

namespace Vixen.Execution
{
	public interface ISequenceExecutor : IExecutor, IDisposable
	{
		ISequence Sequence { get; set; }
		void Play(TimeSpan startTime, TimeSpan endTime);
		void PlayLoop(TimeSpan startTime, TimeSpan endTime);
	}
}