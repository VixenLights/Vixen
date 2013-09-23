using System;
using System.Collections.Generic;
using System.Linq;

namespace Vixen.Sys
{
	public class InsertDataListenerStack
	{
		public delegate bool DataListener(IEffectNode effectNode);

		private LinkedList<DataListener> _listeners = new LinkedList<DataListener>();

		public void InsertData(IEffectNode effectNode)
		{
			bool cancel = false;
			using (IEnumerator<DataListener> enumerator = _listeners.GetEnumerator()) {
				while (!cancel && enumerator.MoveNext()) {
					cancel = enumerator.Current(effectNode);
				}
			}
		}

		public void InsertData(IEnumerable<IEffectNode> effectNodes)
		{
			bool cancel = false;
			using (IEnumerator<DataListener> enumerator = _listeners.GetEnumerator()) {
				while (!cancel && enumerator.MoveNext()) {
					foreach (IEffectNode effectNode in effectNodes) {
						cancel = enumerator.Current(effectNode);
					}
				}
			}
		}

		public static InsertDataListenerStack operator +(InsertDataListenerStack stack, DataListener listener)
		{
			stack._listeners.AddFirst(listener);
			return stack;
		}

		public static InsertDataListenerStack operator -(InsertDataListenerStack stack, DataListener listener)
		{
			if (stack == null)
				return null;

			DataListener value = stack._listeners.FirstOrDefault(x => x.Method.Name == listener.Method.Name);
			if (value != null) {
				stack._listeners.Remove(value);
			}
			return stack;
		}
	}
}