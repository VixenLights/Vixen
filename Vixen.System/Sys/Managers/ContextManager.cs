using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		//This was a ConcurrentDictionary for a while, but grabbing an instance enumerator can be costly as it makes a read only copy
		//General locking on a Dictionary is sufficient here and more performant for iterating
		private readonly Dictionary<Guid, IContext> _instances;
		//Used to help reduce the amount of object allocations. So the enumerator does not have to constantly make a copy.
		private ReadOnlyCollection<IContext> _contextInstances; 
		private MillisecondsValue _contextUpdateTimeValue = new MillisecondsValue("   Contexts update");
		private Stopwatch _stopwatch = Stopwatch.StartNew();
		private readonly HashSet<Guid> _affectedElements = new HashSet<Guid>(); 
		private PreCachingSequenceContext _preCachingSequenceContext;

		public event EventHandler<ContextEventArgs> ContextCreated;
		public event EventHandler<ContextEventArgs> ContextReleased;

		public ContextManager()
		{
			_instances = new Dictionary<Guid, IContext>();
			_contextInstances = _instances.Values.ToList().AsReadOnly();
			VixenSystem.Instrumentation.AddValue(_contextUpdateTimeValue);
		}

		public LiveContext CreateLiveContext(string name)
		{
			if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
			var context = new LiveContext(name);
			_AddContext(context);
			return context;
		}

		public PreCachingSequenceContext GetCacheCompileContext()
		{
			if (_preCachingSequenceContext == null)
			{
				_preCachingSequenceContext = new PreCachingSequenceContext("Compiler");
				_AddContext(_preCachingSequenceContext);
			}else if (!_instances.ContainsKey(_preCachingSequenceContext.Id))
			{
				_AddContext(_preCachingSequenceContext);
			}
			return _preCachingSequenceContext;
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
			VixenSystem.Instrumentation.AddValue(_contextUpdateTimeValue);
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
			_contextInstances = _instances.Values.ToList().AsReadOnly();
		}

		public HashSet<Guid> UpdateCacheCompileContext(TimeSpan time)
		{
			HashSet<Guid> elementsAffected = null;
			_stopwatch.Restart();
			try
			{
				elementsAffected =_preCachingSequenceContext.UpdateElementStates(time);
			} catch (Exception ee)
			{
				Logging.Error(ee.Message, ee);
			}
			_contextUpdateTimeValue.Set(_stopwatch.ElapsedMilliseconds);
			return elementsAffected;

		}

		public HashSet<Guid> Update()
		{
			_stopwatch.Restart();
			_affectedElements.Clear();
			
			foreach (var context in _contextInstances.Where(x => x.IsRunning))
			{
				try
				{
					// Get a snapshot time value for this update.
					TimeSpan contextTime = context.GetTimeSnapshot();
					var contextAffectedElements = context.UpdateElementStates(contextTime);
					if (contextAffectedElements != null)
					{
						//Aggregate the effected elements so we can do more selective work downstream
						_affectedElements.AddRange(contextAffectedElements);
					}

				}
				catch (Exception ee)
				{
					Logging.Error(ee.Message, ee);
				}
			}
			
			_contextUpdateTimeValue.Set(_stopwatch.ElapsedMilliseconds);
			return _affectedElements;
		}

		public IEnumerator<IContext> GetEnumerator()
		{
			return _contextInstances.GetEnumerator();
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
				Logging.Error(string.Format("Could not find a context for target type {0} with features {1}", contextTargetType, contextFeatures));
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
					x.GetCustomAttributes(typeof(ContextAttribute), false).Cast<ContextAttribute>().Any(
						y => y.TargetType == contextTargetType && y.Caching == contextFeatures.Caching));
		}

		private void _AddContext(IContext context)
		{
			lock (_instances) {
				_instances[context.Id] = context;
			}
			_contextInstances = _instances.Values.ToList().AsReadOnly();
			OnContextCreated(new ContextEventArgs(context));
		}

		private void _ReleaseContext(IContext context)
		{
			context.Stop();
			lock (_instances)
			{
				_instances.Remove(context.Id);	
			}
			_contextInstances = _instances.Values.ToList().AsReadOnly();
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