using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Vixen.Execution.DataSource;
using Vixen.Module.Timing;
using Vixen.Sys;
using Vixen.Sys.LayerMixing;

namespace Vixen.Execution.Context
{
	public class LiveContext : ContextBase
	{
		private static readonly NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private readonly string _name;
		private readonly LiveDataSource _dataSource;
		
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

		public void TerminateNodes(IEnumerable<Guid> targetNodes)
		{
			HashSet<IEffectNode> nodes = new HashSet<IEffectNode>();
			foreach (var targetNode in targetNodes)
			{
				 nodes.AddRange(CurrentEffects.Where(x => x.Effect.TargetNodes.Any(t => t.Id.Equals(targetNode))));
			}
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
			if (!IsRunning || IsPaused)
			{
				Logging.Error("Attempt to clear effects from a non running context");
				return;
			}
			CurrentEffects.Reset();
			if (waitForReset)
			{
				//wait for reset to occur, but time out if it does not happen
				var sw = Stopwatch.StartNew();
				while (CurrentEffects.Resetting())
				{
					if (sw.ElapsedMilliseconds > 1000)
					{
						Logging.Error("Attempt to clear current effects timed out after 1 second.");
						break;
					}
					
				}
			}
		}

		protected override ILayer GetLayerForNode(IEffectNode node)
		{
			return SequenceLayers.GetDefaultLayer();
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

		private class TargetNodesComparer:IEqualityComparer<IElementNode>
		{
			public bool Equals(IElementNode x, IElementNode y)
			{
				return x.Id.Equals(y.Id);
			}

			public int GetHashCode(IElementNode obj)
			{
				return obj.Id.GetHashCode();
			}
		}
	}
}