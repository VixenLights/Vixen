using System;
using System.Collections.Concurrent;

namespace Vixen.Pool
{
	public class ObjectPool<T>
	{
		private static readonly NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		protected ConcurrentQueue<T> _objects;
		protected Func<T> _objectGenerator;

		public ObjectPool(Func<T> objectGenerator)
		{
			if (objectGenerator == null) throw new ArgumentNullException("objectGenerator");
			_objects = new ConcurrentQueue<T>();
			_objectGenerator = objectGenerator;
		}

		public virtual T GetObject()
		{
			T item;
			if (_objects.TryDequeue(out item)) return item;
			Logging.Warn("Allocated a state list! {0}", _objects.Count);
			return _objectGenerator();
		}

		public virtual void PutObject(T item)
		{
			_objects.Enqueue(item);
		}
	}
}
