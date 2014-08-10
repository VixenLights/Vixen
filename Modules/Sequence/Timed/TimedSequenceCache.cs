using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.BaseSequence;

namespace VixenModules.Sequence.Timed
{
	[Serializable]
	public class TimedSequenceCache : SequenceCache
	{
		public static string Extension = ".tcf";

		public override string CacheFileExtension
		{
			get { return Extension; }
		}
	}
}
