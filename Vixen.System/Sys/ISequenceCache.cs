using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ProtoBuf;

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
		/// <summary>
		/// Output map in order that the data will be added.
		/// </summary>
		List<Guid> Outputs { get; set; }
		/// <summary>
		/// Data slice to each output in order to be added to the sequence cache
		/// </summary>
		/// <param name="dataSlice"></param>
		void AppendData(List<byte> dataSlice);
		void ClearData();
		List<byte> GetDataAtInterval(int interval);
		ReadOnlyCollection<List<byte>> RetrieveData();
	}
}
