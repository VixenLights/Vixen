using System;
using System.Collections.Generic;

namespace Vixen.Sys
{
	public interface IDataNode : ITimeNode
	{
		// Currently used by classes that wrap data in a start time -- EffectNode, IntentNode, sequenceFilterNode.
		// Going to keep this around in case we need to differentiate from ITimeNode.
	}

	internal class DataNode
	{
		public static int Compare(IDataNode left, IDataNode right)
		{
			return DefaultComparer.Compare(left, right);
		}

		public static IComparer<IDataNode> DefaultComparer = new DataNodeComparer();

		private class DataNodeComparer : IComparer<IDataNode>
		{
			public int Compare(IDataNode x, IDataNode y)
			{
				return x.StartTime.CompareTo(y.StartTime);
			}
		}
	}
}