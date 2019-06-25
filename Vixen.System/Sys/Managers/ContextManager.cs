using System;
using System.Collections;
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
		private List<IContext> _contextInstances; 
		private MillisecondsValue _contextUpdateTimeValue = new MillisecondsValue("   Contexts update");
		private Stopwatch _stopwatch = Stopwatch.StartNew();
		
		public event EventHandler<ContextEventArgs> ContextCreated;
		public event EventHandler<ContextEventArgs> ContextReleased;

		public ContextManager()
		{
			_instances = new Dictionary<Guid, IContext>();
			_contextInstances = _instances.Values.ToList();
			VixenSystem.Instrumentation.AddValue(_contextUpdateTimeValue);
		}

		public LiveContext CreateLiveContext(string name)
		{
			if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
			var context = new LiveContext(name);
			_AddContext(context);
			return context;
		}

		public PreviewContext CreatePreviewContext(string name)
		{
			if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
			var context = new PreviewContext(name);
			_AddContext(context);
			return context;
		}

		public PreCachingSequenceContext GetCacheCompileContext()
		{
			var	preCachingSequenceContext = new PreCachingSequenceContext("Compiler");
			_AddContext(preCachingSequenceContext);
			
			return preCachingSequenceContext;
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
			_contextInstances = _instances.Values.ToList();
		}

		public bool UpdateCacheCompileContext(TimeSpan time, IContext context)
		{
			bool elementsAffected = false;
			_stopwatch.Restart();
			try
			{
				elementsAffected = context.UpdateElementStates(time);
			}
			catch (Exception ee)
			{
				Logging.Error(ee, ee.Message);
			}
			_contextUpdateTimeValue.Set(_stopwatch.ElapsedMilliseconds);
			return elementsAffected;
		}

		public bool Update()
		{
			_stopwatch.Restart();
			bool elementsAffected = false;
			foreach (var context in _contextInstances)
			{
				if (context.IsRunning)
				{
					try
					{
						// Get a snapshot time value for this update.
						TimeSpan contextTime = context.GetTimeSnapshot();
						var contextAffectedElements = context.UpdateElementStates(contextTime);
						if (contextAffectedElements)
						{
							elementsAffected = true;
						}

					}
					catch (Exception ee)
					{
						Logging.Error(ee,ee.Message);
					}
				}
			}

			_contextUpdateTimeValue.Set(_stopwatch.ElapsedMilliseconds);
			return elementsAffected;
		}

		//public IEnumerator<IContext> GetEnumerator()
		//{
		//	return _contextInstances.GetEnumerator();
		//}

		IEnumerator<IContext> IEnumerable<IContext>.GetEnumerator()
		{
			return new Enumerator(_contextInstances);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(_contextInstances);
		}

		public Enumerator GetEnumerator()
		{
			return new Enumerator(_contextInstances);
		}

		//System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		//{
		//	return GetEnumerator();
		//}

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
			_contextInstances = _instances.Values.ToList();
			OnContextCreated(new ContextEventArgs(context));
		}

		private void _ReleaseContext(IContext context)
		{
			context.Stop();
			lock (_instances)
			{
				_instances.Remove(context.Id);	
			}
			_contextInstances = _instances.Values.ToList();
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

		public struct Enumerator : IEnumerator<IContext>
	{
		int nIndex;
		readonly List<IContext> _collection;
		public Enumerator(List<IContext> coll)
		{
			_collection = coll;
			nIndex = -1;
		}

		public void Reset()
		{
			nIndex = -1;
		}

		public bool MoveNext()
		{
			nIndex++;
			return (nIndex < _collection.Count);
		}

		public IContext Current
		{
			get
			{
				return (_collection[nIndex]);
			}
		}

		// The current property on the IEnumerator interface:
		object IEnumerator.Current
		{
			get
			{
				return (Current);
			}
		}

		public void Dispose()
		{
			
		}
	}
	}

	
}