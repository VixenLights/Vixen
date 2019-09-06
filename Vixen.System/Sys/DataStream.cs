using System;
using System.Collections.Generic;
using Vixen.Extensions;
using Vixen.Sys.Enumerator;

namespace Vixen.Sys
{
	public class DataStream : IEnumerable<IDataNode>
	{
		private List<IDataNode> _data = new List<IDataNode>();

		public DataStream(string name)
		{
			Name = name;
			Id = Guid.NewGuid();
		}

		public string Name { get; set; }

		public Guid Id { get; private set; }

		public IEnumerator<IDataNode> GetEnumerator()
		{
			// We need an enumerator that is live and does not operate upon a snapshot of the data.
			// temporary workaround: sort the list of data before using it. We need to ensure the
			// data that is used is in order of start time, however the IDataNodes may have been
			// edited/moved, so resort them to ensure they're OK.
			_data.Sort(DataNode.DefaultComparer);
			return new LiveListEnumerator<IDataNode>(_data);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void AddData(IDataNode data)
		{
			_data.Add(data);
		}

		public bool RemoveData(IDataNode data)
		{
			return _data.Remove(data);
		}

		public void RemoveRangeData(IEnumerable<IDataNode> data)
		{
			_data.RemoveAll(data);
		}

		public void AddData(IEnumerable<IDataNode> data)
		{
			_data.AddRange(data);
		}

		public void Clear()
		{
			_data.Clear();
		}

		public override string ToString()
		{
			return Name;
		}
	}
}