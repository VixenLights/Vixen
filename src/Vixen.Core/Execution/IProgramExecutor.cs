﻿using Vixen.Execution.DataSource;
using Vixen.Sys;

namespace Vixen.Execution
{
	internal interface IProgramExecutor : IExecutor, IDisposable
	{
		event EventHandler<ProgramEventArgs> ProgramStarted;
		event EventHandler<ProgramEventArgs> ProgramEnded;

		IProgram Program { get; set; }
		IMutableDataSource DataSource { set; }
		int Queue(ISequence sequence);
	}
}