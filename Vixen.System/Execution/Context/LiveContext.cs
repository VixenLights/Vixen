using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Execution.DataSource;
using Vixen.Module.Timing;
using Vixen.Sys;

namespace Vixen.Execution.Context
{
	public class LiveContext : ContextBase
	{
		private string _name;
		private LiveDataSource _dataSource;

		//public LiveContext(string name)
		//    : base(name) {
		//    _dataSource = new LiveDataSource();
		//}
		public LiveContext(string name)
		{
			_name = name;
			_dataSource = new LiveDataSource();
		}

		public void TerminateNode(Guid targetNode)
		{
			IEnumerable<IEffectNode> nodes = _currentEffects.Where(x => x.Effect.TargetNodes.Any(t => t.Id.Equals(targetNode))).ToList();
			_currentEffects.RemoveEffects(nodes);
		}

		public override string Name
		{
			get { return _name; }
		}

		public void Execute(EffectNode data)
		{
			_dataSource.AddData(data);
		}

		public void Execute(EffectNode[] data)
		{
			_dataSource.AddData(data);
		}

		public void Execute(IEnumerable<EffectNode> data)
		{
			_dataSource.AddData(data);
		}

		public void Clear()
		{	
			_dataSource.ClearData();
			ClearCurrentEffects();	
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