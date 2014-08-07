using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Vixen.Sys;
using Vixen.Sys.Instrumentation;

namespace Vixen.Execution.DataSource
{
	internal class EffectNodeBuffer : IDataSource
	{
		private IEnumerable<IEffectNode> _effectNodeSource;
		private EffectNodeQueue _effectNodeQueue;
		private BufferSizeInSecondsValue _bufferSizeSecondsValue;
		private AutoResetEvent _bufferReadSignal;
		private Thread _bufferPopulationThread;
		private TimeSpan _lastBufferReadPoint;
		private TimeSpan _lastBufferWritePoint;

		public EffectNodeBuffer()
		{
			BufferSizeInSeconds = 10;
		}

		public EffectNodeBuffer(IEnumerable<IEffectNode> effectNodeSource)
			: this()
		{
			_effectNodeSource = effectNodeSource;
		}

		public void SetDataSource(IEnumerable<IEffectNode> value)
		{
			if (_effectNodeSource != value && !IsRunning) {
				_effectNodeSource = value;
				_CreateBuffer();
			}
		}

		public string ContextName { get; set; }

		public int BufferSizeInSeconds { get; set; }

		public bool IsBufferFull()
		{
			return !_IsBufferInadequate();
		}

		public void Start()
		{
			if (!IsRunning) {
				if (_effectNodeSource == null) throw new InvalidOperationException("Effect node source has not been provided.");

				_AddInstrumentationValues();
				_StartThread();
			}
		}

		public void Stop()
		{
			if (IsRunning) {
				_StopThread();
				_RemoveInstrumentationValues();
			}
		}

		public bool IsRunning { get; private set; }

		private void _CreateBuffer()
		{
			_effectNodeQueue = new EffectNodeQueue();
			_LastBufferWritePoint = TimeSpan.Zero;
			_LastBufferReadPoint = TimeSpan.Zero;
		}

		private void _ReleaseBuffer()
		{
			_effectNodeQueue = null;
		}

		private void _AddInstrumentationValues()
		{
			_bufferSizeSecondsValue = new BufferSizeInSecondsValue(ContextName);
			VixenSystem.Instrumentation.AddValue(_bufferSizeSecondsValue);
		}

		private void _RemoveInstrumentationValues()
		{
			VixenSystem.Instrumentation.RemoveValue(_bufferSizeSecondsValue);
		}

		private void _StartThread()
		{
			_bufferReadSignal = _CreateAutoResetEvent();
			_CreateBuffer();
			_bufferPopulationThread = _CreatePopulationThread();
			_bufferPopulationThread.Start();
		}

		private void _StopThread()
		{
			_bufferSizeSecondsValue.Set(0);

			IsRunning = false;
			_bufferReadSignal.Set();
			_bufferPopulationThread.Join(1000);
			_bufferPopulationThread = null;
			_CloseAutoResetEvent();
			_ReleaseBuffer();
		}

		private AutoResetEvent _CreateAutoResetEvent()
		{
			_CloseAutoResetEvent();
			return new AutoResetEvent(true);
		}

		private void _CloseAutoResetEvent()
		{
			if (_bufferReadSignal != null) {
				_bufferReadSignal.Dispose();
				_bufferReadSignal = null;
			}
		}

		private Thread _CreatePopulationThread()
		{
			return new Thread(_BufferPopulationThread) {IsBackground = true, Name = ContextName + " buffer"};
		}

		private void _BufferPopulationThread()
		{
			IsRunning = true;

			IEnumerator<IEffectNode> dataEnumerator = _effectNodeSource.GetEnumerator();
			try {
				while (IsRunning) {
					while (_IsBufferInadequate() && IsRunning && dataEnumerator.MoveNext() && _effectNodeQueue != null) {
						_AddToQueue(dataEnumerator.Current);
					}

					// Wait until the buffer is read from.
					if (_bufferReadSignal != null)
						_bufferReadSignal.WaitOne();
				}
			}
			finally {
				dataEnumerator.Dispose();
				if (_bufferReadSignal != null) {
					_bufferReadSignal.Close();
					_bufferReadSignal.Dispose();
				}
			}
		}

		private void _AddToQueue(IEffectNode effectNode)
		{
			_effectNodeQueue.Add(effectNode);
			_LastBufferWritePoint = effectNode.StartTime;
		}

		private bool _IsBufferInadequate()
		{
			return _SecondsBuffered() < BufferSizeInSeconds;
		}

		private double _SecondsBuffered()
		{
			return (_LastBufferWritePoint - _LastBufferReadPoint).TotalSeconds;
		}

		private TimeSpan _LastBufferWritePoint
		{
			get { return _lastBufferWritePoint; }
			set
			{
				_lastBufferWritePoint = value;
				_bufferSizeSecondsValue.Set(_SecondsBuffered());
			}
		}

		private TimeSpan _LastBufferReadPoint
		{
			get { return _lastBufferReadPoint; }
			set
			{
				_lastBufferReadPoint = value;
				_bufferReadSignal.Set();
				_bufferSizeSecondsValue.Set(_SecondsBuffered());
			}
		}

		#region IDataSource

		public IEnumerable<IEffectNode> GetDataAt(TimeSpan time)
		{
			if (IsRunning) {
				return _GetEffectNodesAt(time);
			}
			return Enumerable.Empty<IEffectNode>();
		}

		private IEnumerable<IEffectNode> _GetEffectNodesAt(TimeSpan time)
		{
			if (time > _lastBufferWritePoint)
			{
				
			}
			IEffectNode[] effectNodes = _effectNodeQueue.Get(time).ToArray();
			if (effectNodes.Length > 0) {
				_LastBufferReadPoint = effectNodes[effectNodes.Length - 1].StartTime;
			}
			return effectNodes;
		}

		#endregion
	}
}