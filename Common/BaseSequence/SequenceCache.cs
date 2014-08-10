using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Vixen.Cache;
using Vixen.Cache.Sequence;
using Vixen.Services;
using Vixen.Sys;

namespace Common.BaseSequence
{
	[Serializable]
	public abstract class SequenceCache : ISequenceCache
	{
		protected SequenceCache()
		{
			OutputStateListAggregator = new OutputStateListAggregator();
		}

		public TimeSpan Length { get; set; }
		public int Interval { get; set; }
		protected void Save(string filePath)
		{
			SequenceService.Instance.SaveCache(this, filePath);
		}

		public void Save()
		{
			Save(CacheFilePath);
		}

		public string Name
		{
			get { return Path.GetFileNameWithoutExtension(CacheFilePath); }
		}

		public abstract string CacheFileExtension { get;}


		public string SequenceFilePath { get; set; }

		public OutputStateListAggregator OutputStateListAggregator { get; set; }

		public string CacheFilePath
		{
			get { return Path.ChangeExtension(SequenceFilePath, CacheFileExtension); }
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
