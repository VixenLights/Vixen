﻿namespace Vixen.Sys.Output
{
	public interface IUpdatableOutputCount
	{
		int OutputCount { get; set; }

		int OutputLimit { get; }
	}
}