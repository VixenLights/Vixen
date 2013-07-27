using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Vixen.Execution;
using Vixen.Execution.Context;
using Vixen.Sys.Attribute;
using Vixen.Sys.Instrumentation;

namespace Vixen.Sys.Managers
{
	public class ContextManager : IEnumerable<IContext>
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private Dictionary<Guid, IContext> _instances;
		private ContextUpdateTimeValue _contextUpdateTimeValue;
		private Stopwatch _stopwatch;
		private LiveContext _systemLiveContext;

		public event EventHandler<ContextEventArgs> ContextCreated;
		public event EventHandler<ContextEventArgs> ContextReleased;

		public ContextManager()
		{
			_instances = new Dictionary<Guid, IContext>();
			_SetupInstrumentation();
		}

		public LiveContext GetSystemLiveContext()
		{
			if (_systemLiveContext == null) {
				_systemLiveContext = new LiveContext("System");
				_AddContext(_systemLiveContext);
			}
			return _systemLiveContext;
		}

		public IProgramContext CreateProgramContext(ContextFeatures contextFeatures, IProgram program)
		{
			return CreateProgramContext(contextFeatures, program, new ProgramExecutor(program));
		}

		public IProgramContext CreateProgramContext(ContextFeatures contextFeatures, IProgram program, IExecutor executor)
		{
			if (contextFeatures == null) throw new ArgumentNullException("contextFeatures");
			if (executor == null) throw new ArgumentNullException("executor");

			IProgramContext context = (IProgramContext) _CreateContext(ContextTargetType.Program, contextFeatures);
			if (context != null) {
				context.Executor = executor;
				context.Program = program;
				_AddContext(context);
			}
			return context;
		}

		public ISequenceContext CreateSequenceContext(ContextFeatures contextFeatures, ISequence sequence)
		{
			ISequenceExecutor executor = Vixen.Services.SequenceTypeService.Instance.CreateSequenceExecutor(sequence);
			ISequenceContext context = (ISequenceContext) _CreateContext(ContextTargetType.Sequence, contextFeatures);
			if (executor != null && context != null) {
				context.Executor = executor;
				context.Sequence = sequence;
				_AddContext(context);
			}
			return context;
		}

		public void ReleaseContext(IContext context)
		{
			if (_instances.ContainsKey(context.Id)) {
				_ReleaseContext(context);
			}
		}

		public void ReleaseContexts()
		{
			foreach (IContext context in _instances.Values.ToArray()) {
				ReleaseContext(context);
			}
			lock (_instances) {
				_instances.Clear();
			}
		}

		public void Update()
		{
			lock (_instances) {
				_stopwatch.Restart();

				_instances.Values.AsParallel().ForAll(context =>
				                                      	{
				                                      		// Get a snapshot time value for this update.
				                                      		TimeSpan contextTime = context.GetTimeSnapshot();
				                                      		IEnumerable<Guid> affectedElements = context.UpdateElementStates(contextTime);
				                                      		//Could possibly return affectedElements so only affected outputs
				                                      		//are updated.  The controller would have to maintain state so those
				                                      		//outputs could be updated and the whole state sent out.
				                                      	});

				_contextUpdateTimeValue.Set(_stopwatch.ElapsedMilliseconds);
			}
		}

		public IEnumerator<IContext> GetEnumerator()
		{
			IContext[] contexts;
			lock (_instances) {
				contexts = _instances.Values.ToArray();
			}
			return contexts.Cast<IContext>().GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private IContext _CreateContext(ContextTargetType contextTargetType, ContextFeatures contextFeatures)
		{
			if (contextFeatures == null) throw new ArgumentNullException("contextFeatures");
			Type contextType = _FindContextWithFeatures(contextTargetType, contextFeatures);
			if (contextType == null) {
				Logging.Error("Could not find a context for target type " + contextTargetType + " with features " +
				                          contextFeatures);
				return null;
			}
			return (ContextBase) Activator.CreateInstance(contextType);
		}

		private Type _FindContextWithFeatures(ContextTargetType contextTargetType, ContextFeatures contextFeatures)
		{
			IEnumerable<Type> contextTypes = Assembly.GetExecutingAssembly().GetAttributedTypes(typeof (ContextAttribute));
			return
				contextTypes.FirstOrDefault(
					x =>
					x.GetCustomAttributes(typeof (ContextAttribute), false).Cast<ContextAttribute>().Any(
						y => y.TargetType == contextTargetType && y.Caching == contextFeatures.Caching));
		}

		private void _SetupInstrumentation()
		{
			_contextUpdateTimeValue = new ContextUpdateTimeValue();
			VixenSystem.Instrumentation.AddValue(_contextUpdateTimeValue);
			_stopwatch = Stopwatch.StartNew();
		}

		private void _AddContext(IContext context)
		{
			lock (_instances) {
				_instances[context.Id] = context;
			}
			OnContextCreated(new ContextEventArgs(context));
		}

		private void _ReleaseContext(IContext context)
		{
			context.Stop();
			lock (_instances) {
				_instances.Remove(context.Id);
			}
			context.Dispose();
			OnContextReleased(new ContextEventArgs(context));
		}

		protected virtual void OnContextCreated(ContextEventArgs e)
		{
			if (ContextCreated != null) {
				ContextCreated(this, e);
			}
		}

		protected virtual void OnContextReleased(ContextEventArgs e)
		{
			if (ContextReleased != null) {
				ContextReleased(this, e);
			}
		}
	}
}