using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Vixen.Cache.Sequence;
using Vixen.Services;

namespace Common.BaseSequence
{
	public abstract class SequenceCache : ISequenceCache
	{
		private readonly List<List<byte>> _dataSlices;

		protected SequenceCache()
		{
			_dataSlices = new List<List<byte>>();
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

		public string CacheFilePath
		{
			get { return Path.ChangeExtension(SequenceFilePath, CacheFileExtension); }
		}

		public List<Guid> Outputs { get; set; }	

		public void AppendData(List<byte> dataSlice)
		{
			_dataSlices.Add(dataSlice);
		}

		public void ClearData()
		{
			_dataSlices.Clear();
		}

		public List<byte> GetDataAtInterval(int interval)
		{
			List<byte> data = null;
			if (interval < _dataSlices.Count)
			{
				data = _dataSlices[interval];	
			}
			return data;
		}

		public ReadOnlyCollection<List<byte>> RetrieveData()
		{
			return _dataSlices.AsReadOnly();
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
