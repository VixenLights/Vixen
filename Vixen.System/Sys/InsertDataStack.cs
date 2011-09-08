using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys {
	public class InsertDataListenerStack {
		public delegate bool DataListener(CommandNode commandNode);

		private LinkedList<DataListener> _listeners = new LinkedList<DataListener>();

		public void InsertData(CommandNode commandNode) {
			bool cancel = false;
			IEnumerator<DataListener> enumerator = _listeners.GetEnumerator();
			while(!cancel && enumerator.MoveNext()) {
				cancel = enumerator.Current(commandNode);
			}
		}

		static public InsertDataListenerStack operator +(InsertDataListenerStack stack, DataListener listener) {
			stack._listeners.AddFirst(listener);
			return stack;
		}

		static public InsertDataListenerStack operator -(InsertDataListenerStack stack, DataListener listener) {
			DataListener value = stack._listeners.FirstOrDefault(x => x.Method.Name == listener.Method.Name);
			if(value != null) {
				stack._listeners.Remove(value);
			}
			return stack;
		}
	}
}
