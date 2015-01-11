using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Vixen.Sys;

namespace Vixen.Execution.DataSource
{
	internal class EffectNodeDataPump : IDataSource, IDisposable
	{

		private EffectNodeQueue _effectNodeQueue;
		private Thread _dataPumpThread;
		private readonly IEnumerable<IDataNode> _effectNodeSource;
		private bool dataLoadStarted;

		public EffectNodeDataPump()
		{
			_effectNodeQueue = new EffectNodeQueue();
		}

		public EffectNodeDataPump(IEnumerable<IDataNode> value) : this()
		{
			_effectNodeSource = value;
		}

		public void Start()
		{
			if (_effectNodeSource == null) throw new InvalidOperationException("Effect nodes have not been set in the data pump.");
			if (!IsRunning) {
				_StartThread();
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
				return _effectNodeQueue.Get(time).ToArray();
			}
			return Enumerable.Empty<IEffectNode>();
		}

		private void _StartThread()
		{
			_effectNodeQueue = new EffectNodeQueue();
			_dataPumpThread = new Thread(_DataPumpThread) {IsBackground = true, Name = string.Format("{0} data pump", "Compile")};
			_dataPumpThread.Start();
			if (_effectNodeSource.Any())
			{
				while (!dataLoadStarted)
				{
					Thread.Sleep(1); //wait until data is available
				}	
			}
			
		}

		private void _StopThread()
		{
			IsRunning = false;
			_dataPumpThread.Join(1000);
			_dataPumpThread = null;
			_effectNodeQueue = null;
			dataLoadStarted = false;
		}

		private void _DataPumpThread()
		{
			IEnumerator<IDataNode> dataEnumerator = _effectNodeSource.GetEnumerator();
			IsRunning = true;
			try {
				while (IsRunning && dataEnumerator.MoveNext()) {
					_effectNodeQueue.Add((IEffectNode) dataEnumerator.Current);
					dataLoadStarted = true;
				}
			}
			finally {
				dataEnumerator.Dispose();
			}
		}

		protected void Dispose(bool disposing)
		{
			if (disposing) {
				if (_dataPumpThread != null)
					_dataPumpThread.Abort();
			}
			_dataPumpThread = null;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
