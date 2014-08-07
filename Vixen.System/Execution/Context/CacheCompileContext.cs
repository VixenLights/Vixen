using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Execution.DataSource;
using Vixen.Module.Timing;
using Vixen.Sys;

namespace Vixen.Execution.Context
{
	public class CacheCompileContext: ContextBase
	{

		private string _name;
		private EffectNodeDataPump _dataSource;
		private ISequence _sequence;

		public CacheCompileContext(string name)
		{
			_name = name;
		}

		public ISequence Sequence
		{
			get { return _sequence; }
			set
			{
				_sequence = value;
				_dataSource = new EffectNodeDataPump(_sequence.SequenceData.EffectData);
			}
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
			get { return Sys.Execution.SystemTime; }
		}

		public override IExecutor Executor
		{
			set { ; }
		}
	}
}
