using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Cache.Sequence;
using Vixen.Execution.DataSource;
using Vixen.Module.Timing;
using Vixen.Sys;
using Vixen.Sys.LayerMixing;

namespace Vixen.Execution.Context
{
	/// <summary>
	/// This context is designed to be used for pre caching an entire sequence commands 
	/// </summary>
	public class PreCachingSequenceContext: ContextBase
	{

		private readonly string _name;
		private SequenceDataPump _dataSource; //Maybe use  
		private ISequence _sequence;
		
		public PreCachingSequenceContext(string name)
		{
			_name = name;
		}

		public ISequence Sequence
		{
			get { return _sequence; }
			set
			{
				_sequence = value;
				_dataSource = new SequenceDataPump {Sequence = _sequence};
			}
		}

		protected override ILayer GetLayerForNode(IEffectNode node)
		{
			return Sequence.SequenceData.SequenceLayers.GetLayer(node);
		}

		protected override void _OnStart()
		{
			_dataSource.Start();	
		}

		protected override void _OnStop()
		{
			_dataSource.Stop();
		}

		public override bool IsRunning
		{
			get { return _dataSource.IsRunning; }
		}

		public override string Name
		{
			get { return _name; }
		}

		protected override IDataSource _DataSource
		{
			get { return _dataSource; }
		}

		protected override ITiming _SequenceTiming
		{
			get { return null; }
		}

		public override IExecutor Executor
		{
			set { }
		}
	}
}
