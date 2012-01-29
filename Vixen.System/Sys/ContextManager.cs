using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vixen.Execution;
using Vixen.Module.Timing;

namespace Vixen.Sys {
	public class ContextManager : IEnumerable<Context> {
		private Dictionary<Guid, Context> _instances;

		public event EventHandler<ContextEventArgs> ContextCreated;
		public event EventHandler<ContextEventArgs> ContextReleased;

		public ContextManager() {
			_instances = new Dictionary<Guid, Context>();
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
			Context context = new Context(name, dataSource, timingSource, logicalNodes);
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
				Parallel.ForEach(_instances.Values, context => {
					// Broken up so that filtering can be skipped at some future point.
					IEnumerable<Guid> affectedChannels = context.UpdateChannelStates();
					context.FilterChannelStates(affectedChannels);
					//Could possibly return affectedChannel so only affected outputs
					//are updated.  The controller would have to maintain state so those
					//outputs could be updated and the whole state sent out.
				});
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
