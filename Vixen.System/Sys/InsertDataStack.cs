using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys {
	public class InsertDataListenerStack {
		private LinkedList<Action<InsertDataParameters>> _listeners = new LinkedList<Action<InsertDataParameters>>();

		public void InsertData(OutputChannel[] channels, int startTime, int timeSpan, Command command) {
			InsertDataParameters parameters = new InsertDataParameters(channels, startTime, timeSpan, command);
			bool cancel = false;
			IEnumerator<Action<InsertDataParameters>> enumerator = _listeners.GetEnumerator();
			while(!cancel && enumerator.MoveNext()) {
				enumerator.Current(parameters);
				cancel = parameters.Cancel;
			}
		}

		static public InsertDataListenerStack operator +(InsertDataListenerStack stack, Action<InsertDataParameters> listener) {
			stack._listeners.AddFirst(listener);
			return stack;
		}

		static public InsertDataListenerStack operator -(InsertDataListenerStack stack, Action<InsertDataParameters> listener) {
			Action<InsertDataParameters> value = stack._listeners.FirstOrDefault(x => x.Method.Name == listener.Method.Name);
			if(value != null) {
				stack._listeners.Remove(value);
			}
			return stack;
		}
	}

	// Must be a class because we want to get Cancel out.
	public class InsertDataParameters {
		public InsertDataParameters(OutputChannel[] channels, int startTime, int timeSpan, Command command) {
			Channels = channels;
			StartTime = startTime;
			TimeSpan = timeSpan;
			this.Command = command;
			Cancel = false;
		}

		public readonly OutputChannel[] Channels;
		public readonly int StartTime;
		public readonly int TimeSpan;
		public readonly Command Command;
		public bool Cancel;
	}
}
