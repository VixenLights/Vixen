using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Execution.DataSource;
using Vixen.Module.Timing;
using Vixen.Sys;
using Vixen.Sys.LayerMixing;

namespace Vixen.Execution.Context
{
	public class LiveContext : ContextBase
	{
		private readonly string _name;
		private readonly LiveDataSource _dataSource;
		private readonly Layer _layer = new DefaultLayer();

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
			IEnumerable<IEffectNode> nodes = CurrentEffects.Where(x => x.Effect.TargetNodes.Any(t => t.Id.Equals(targetNode))).ToList();
			CurrentEffects.RemoveEffects(nodes);
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

		public void Clear(bool waitForReset = true)
		{	
			_dataSource.ClearData();
			CurrentEffects.Reset();
			if (waitForReset)
			{
				while (CurrentEffects.Resetting())
				{
					//wait for reset to occur.
				}
			}
		}

		protected override ILayer GetLayerForNode(IEffectNode node)
		{
			return _layer;
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