using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Sys;

namespace Vixen.Execution.DataSource
{
	internal class LiveDataSource : IDataSource
	{
		private EffectNodeQueue _data;

		public LiveDataSource()
		{
			_data = new EffectNodeQueue();
		}

		public void AddData(EffectNode effectNode)
		{
			_Add(effectNode);
		}

		public void AddData(IEnumerable<EffectNode> effectNodes)
		{
			foreach (EffectNode effectNode in effectNodes) {
				_Add(effectNode);
			}
		}

		public void ClearData()
		{
			_data.Clear();			
		}

		public IEnumerable<IEffectNode> GetDataAt(TimeSpan time)
		{
			IEffectNode[] data = _data.Get(time).ToArray();
			return data;
		}

		private void _Add(EffectNode effectNode)
		{
			effectNode.StartTime += Sys.Execution.SystemTime.Position;
			_data.Add(effectNode);
		}
	}
}