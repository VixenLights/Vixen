using Vixen.Sys;

namespace Vixen.Pool
{
	public class IntentStateListPool : ObjectPool<IntentStateList>
	{
		private static IntentStateListPool _intentStateListPool;

		public static IntentStateListPool GetPool()
		{
			if (_intentStateListPool == null)
			{
				_intentStateListPool = new IntentStateListPool();
			}

			return _intentStateListPool;
		}
		private IntentStateListPool():base(() => new IntentStateList(4))
		{
			
		}

		public override IntentStateList GetObject()
		{
			var t = base.GetObject();
			t.Clear();
			return t;
		}

		public void Allocate(int num)
		{
			for (int i = 0; i < num; i++)
			{
				PutObject(_objectGenerator());
			}
		}

		public void DeAllocate(int num)
		{
			for (int i = 0; i < num; i++)
			{
				IntentStateList intentStateList;
				_objects.TryDequeue(out intentStateList);
			}
		}

		public int Count()
		{
			return _objects.Count;
		}
	}
}
