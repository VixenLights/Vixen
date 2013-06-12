using System;
using System.Collections.Generic;
using System.Linq;

// Data is only pulled from this at execution start.
// Data written during sequence execution is handled by runtime behaviors and is written
// to the system for immediate execution at the appropriate time.

namespace Vixen.Sys {
	public class DataStreams  {
		// Data in these streams is pulled only at execution start.
		// There will always be at least one stream.
		private List<DataStream> _dataStreams = new List<DataStream>();
		private DataStream _mainStream;
		//private DataNodeCollection _executingData;

		public DataStreams() {
			// The sequence will have at least one stream to hold effect data.
			_mainStream = new DataStream("Main");
			_dataStreams.Add(_mainStream);
			//_executingData = new DataNodeCollection();
		}

		public DataStreams(DataStreams original)
			: this() {
			_mainStream.AddData(original._mainStream);
			foreach(DataStream dataStream in original._dataStreams.Skip(1)) {
				DataStream newStream = new DataStream(dataStream.Name);
				newStream.AddData(dataStream);
				_dataStreams.Add(newStream);
			}
		}

		#region IDataNodes
		public IEnumerable<IDataNode> GetMainStreamData() {
			return _mainStream;
		}

		public IEnumerable<IDataNode> GetStreamData(Guid streamId) {
			return GetDataStream(streamId);
		}

		// During authoring, as data is written to the sequence, it is stored in the
		// sequence's main stream.
		public void AddData(IDataNode data) {
			_mainStream.AddData(data);
		}

		// Used to add to the pre-filter stream.
		public void AddData(Guid streamId, IDataNode data) {
			DataStream dataStream = GetDataStream(streamId);
			if(dataStream != null) {
				dataStream.AddData(data);
			}
		}

		// Used by the recording behavior.
		public void AddData(IEnumerable<IDataNode> data) {
			_mainStream.AddData(data);
		}

		// Used by the recording behavior.
		public void AddData(Guid streamId, IEnumerable<IDataNode> data) {
			DataStream dataStream = GetDataStream(streamId);
			if(dataStream != null) {
				dataStream.AddData(data);
			}
		}

		public bool RemoveData(IDataNode data)
		{
			return _mainStream.RemoveData(data);
		}

		public bool RemoveData(Guid streamId, IDataNode data)
		{
			DataStream stream = GetDataStream(streamId);
			return stream.RemoveData(data);
		}

		///// <summary>
		///// If a sequence is executing, data written to a sequence by way of this
		///// method will be considered by the enumerator.
		///// </summary>
		///// <param name="data"></param>
		//public void AddLive(IEnumerable<IDataNode> data) {
		//    if(_executingData != null) {
		//        _executingData.AddRange(data);
		//    }
		//}

		///// <summary>
		///// If a sequence is executing, data written to a sequence by way of this
		///// method will be considered by the enumerator.
		///// </summary>
		///// <param name="data"></param>
		//public void AddLive(IDataNode data) {
		//    if(_executingData != null) {
		//        _executingData.Add(data);
		//    }
		//}
		#endregion

		#region Streams
		//public Guid CreateStream(string name) {
		//    DataStream dataStream = new DataStream(name);
		//    _dataStreams.Add(dataStream);
		//    return dataStream.Id;
		//}
		public DataStream CreateStream(string name) {
			DataStream dataStream = new DataStream(name);
			_dataStreams.Add(dataStream);
			return dataStream;
		}

		public void ClearStream() {
			_mainStream.Clear();
		}

		public void ClearStream(Guid dataStreamId) {
			//*** dictionary!
			DataStream dataStream = GetDataStream(dataStreamId);
			if(dataStream != null) {
				dataStream.Clear();
			}
		}

		public DataStream GetDataStream(Guid? dataStreamId) {
			if(!dataStreamId.HasValue) return _mainStream;
			return _dataStreams.FirstOrDefault(x => x.Id == dataStreamId);
		}
		
		//public void RemoveStream(Guid dataStreamId) {
		//    DataStream dataStream = _GetDataStream(dataStreamId);
		//    if(dataStream != null) {
		//        _dataStreams.Remove(dataStream);
		//    }
		//}
		public void RemoveStream(DataStream dataStream) {
			_dataStreams.Remove(dataStream);
		}
		#endregion
		 
	}
}
