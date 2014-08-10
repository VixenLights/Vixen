using System;
using Vixen.Sys;

namespace Vixen.Cache.Sequence
{
	public interface ISequenceCache
	{
		string Name { get; }
		TimeSpan Length { get; set; }
		int Interval { get; set; }
		void Save();
		string CacheFileExtension { get; }
		string CacheFilePath { get; }
		string SequenceFilePath { get; set; }
		OutputStateListAggregator OutputStateListAggregator { get; set; }
	}
}
