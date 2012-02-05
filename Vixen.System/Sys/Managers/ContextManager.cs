using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Vixen.Execution;
using Vixen.Module.Timing;
using Vixen.Sys.Instrumentation;

namespace Vixen.Sys.Managers {
	public class ContextManager : IEnumerable<Context> {
		private Dictionary<Guid, Context> _instances;
		private ContextUpdateTimeValue _contextUpdateAverageValue;
		private Stopwatch _stopwatch;

		public event EventHandler<ContextEventArgs> ContextCreated;
		public event EventHandler<ContextEventArgs> ContextReleased;

		public ContextManager() {
			_instances = new Dictionary<Guid, Context>();
			_contextUpdateAverageValue = new ContextUpdateTimeValue();
			VixenSystem.Instrumentation.AddValue(_contextUpdateAverageValue);
			_stopwatch = Stopwatch.StartNew();
		}

		public Context CreateContext(Program program) {
			ProgramContext context = new ProgramContext(program);
			_AddContext(context);

			return context;
		}

		public Context CreateContext(ISequence sequence, string contextName = null) {
			Program program = new Program(contextName ?? sequence.Name) { sequence };
			return CreateContext(program);
		}

		internal Context CreateContext(string name, IDataSource dataSource, ITiming timingSource, IEnumerable<ChannelNode> logicalNodes) {
			Context context = new Context(name, dataSource, timingSource);
			_AddContext(context);
			
			return context;
		}

		public void ReleaseContext(Context context) {
			if(_instances.ContainsKey(context.Id)) {
				_ReleaseContext(context);
			}
		}

		public void ReleaseContexts() {
			foreach(Context context in _instances.Values.ToArray()) {
				ReleaseContext(context);
			}
			_instances.Clear();
		}

		public void Update() {
			lock(_instances) {
				_stopwatch.Restart();
				
				Parallel.ForEach(_instances.Values, context => {
					// Get a snapshot time value for this update.
					TimeSpan contextTime = context.GetTimeSnapshot();
					// Broken up so that filtering can be skipped at some future point.
					IEnumerable<Guid> affectedChannels = context.UpdateChannelStates(contextTime);
					context.FilterChannelStates(affectedChannels, contextTime);
					//Could possibly return affectedChannels so only affected outputs
					//are updated.  The controller would have to maintain state so those
					//outputs could be updated and the whole state sent out.
				});

				_contextUpdateAverageValue.Set(_stopwatch.ElapsedMilliseconds);
			}
		}

		public IEnumerator<Context> GetEnumerator() {
			Context[] contexts;
			lock(_instances) {
				contexts = _instances.Values.ToArray();
			}
			return contexts.Cast<Context>().GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		protected virtual void OnContextCreated(ContextEventArgs e) {
			if(ContextCreated != null) {
				ContextCreated(this, e);
			}
		}

		protected virtual void OnContextReleased(ContextEventArgs e) {
			if(ContextReleased != null) {
				ContextReleased(this, e);
			}
		}

		private void _AddContext(Context context) {
			lock(_instances) {
				_instances[context.Id] = context;
			}
			OnContextCreated(new ContextEventArgs(context));
		}

		private void _ReleaseContext(Context context) {
			context.Stop();
			lock(_instances) {
				_instances.Remove(context.Id);
			}
			context.Dispose();
			OnContextReleased(new ContextEventArgs(context));
		}
	}
}
