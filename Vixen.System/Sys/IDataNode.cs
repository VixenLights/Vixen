using System;
using System.Collections.Generic;

namespace Vixen.Sys {
	public interface IDataNode : ITimeNode {
		// Currently used by classes that wrap data in a start time -- EffectNode, IntentNode, sequenceFilterNode.
		// Going to keep this around in case we need to differentiate from ITimeNode.
	}

	class DataNode {
		static public int Compare(IDataNode left, IDataNode right) {
			return DefaultComparer.Compare(left, right);
		}

		static public IComparer<IDataNode> DefaultComparer = new DataNodeComparer();

		class DataNodeComparer : IComparer<IDataNode> {
			public int Compare(IDataNode x, IDataNode y) {
				return x.StartTime.CompareTo(y.StartTime);
			}
		}
	}
}
