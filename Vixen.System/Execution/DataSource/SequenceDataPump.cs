using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Vixen.Sys;

namespace Vixen.Execution.DataSource
{
	internal class SequenceDataPump : IDataSource, IDisposable
	{
		private EffectNodeQueue _effectNodeQueue;
		private Thread _dataPumpThread;

		public SequenceDataPump()
		{
			_effectNodeQueue = new EffectNodeQueue();
		}

		public ISequence Sequence { get; set; }

		public void Start()
		{
			if (Sequence == null) throw new InvalidOperationException("Sequence has not been set in the data pump.");

			if (!IsRunning) {
				_StartThread();
				while (!IsRunning)
				{
					//wait until the thread is running
					Thread.Sleep(1);
				}
			}
		}

		public void Stop()
		{
			if (IsRunning) {
				_StopThread();
			}
		}

		public bool IsRunning { get; private set; }

		public IEnumerable<IEffectNode> GetDataAt(TimeSpan time)
		{
			if (IsRunning) {
				return _effectNodeQueue.Get(time);
			}
			return Enumerable.Empty<IEffectNode>();
		}

		private void _StartThread()
		{
			_effectNodeQueue = new EffectNodeQueue(Sequence.SequenceData.EffectData.Count());
			_dataPumpThread = new Thread(_DataPumpThread) {IsBackground = true, Name = string.Format("{0} data pump",Sequence.Name)};
			_dataPumpThread.Start();
		}

		private void _StopThread()
		{
			IsRunning = false;
			_dataPumpThread.Join(1000);
			_dataPumpThread = null;
			_effectNodeQueue = null;
		}

		private void _DataPumpThread()
		{
			IEnumerator<IDataNode> dataEnumerator = Sequence.SequenceData.EffectData.GetEnumerator();
			IsRunning = true;
			try {
				while (IsRunning && dataEnumerator.MoveNext()) {
					_effectNodeQueue.Add((IEffectNode) dataEnumerator.Current);
				}
			}
			finally {
				dataEnumerator.Dispose();
			}
		}

		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_dataPumpThread != null)
					_dataPumpThread.Abort();
				_dataPumpThread = null;
				Sequence = null;
				_effectNodeQueue = null;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}