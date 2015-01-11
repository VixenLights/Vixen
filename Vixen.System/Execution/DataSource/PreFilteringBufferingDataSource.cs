using System;
using System.Collections.Generic;
using Vixen.Sys;

namespace Vixen.Execution.DataSource
{
	internal class PreFilteringBufferingDataSource : IMutableDataSource, IDisposable
	{
		private IntentPreFilter _intentPreFilter;
		private EffectNodeBuffer _intentBuffer;
		private bool _persistPreFilterCache;

		public PreFilteringBufferingDataSource(bool persistPreFilterCache)
		{
			_persistPreFilterCache = persistPreFilterCache;
			_intentPreFilter = new IntentPreFilter();
			_intentBuffer = new EffectNodeBuffer(_intentPreFilter);
		}

		public void SetSequence(ISequence sequence)
		{
			_UseSequenceData(sequence);
			_UseSequenceFilters(sequence);
		}

		private void _UseSequenceData(ISequence sequence)
		{
			if (!_persistPreFilterCache) {
				_intentPreFilter.ClearCache();
			}
			_intentPreFilter.Data = sequence.SequenceData.EffectData;
		}

		private void _UseSequenceFilters(ISequence sequence)
		{
			_intentPreFilter.Filters = sequence.GetAllSequenceFilters();
		}

		public string ContextName
		{
			set { _intentPreFilter.ContextName = value; }
		}

		public void Start()
		{
			_intentBuffer.Start();
		}

		public void Stop()
		{
			_intentBuffer.Stop();
		}

		public IEnumerable<IEffectNode> GetDataAt(TimeSpan time)
		{
			return _intentBuffer.GetDataAt(time);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_intentPreFilter != null)
				{
					_intentPreFilter.Dispose();
					_intentPreFilter = null;
				}
				_intentBuffer = null;
			}
		}


		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}