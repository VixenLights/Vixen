﻿namespace Vixen.Sys.Output
{
	internal class OutputCollection<T> : IHasOutputs<T>, IEnumerable<T>
		where T : Output
	{
		private readonly SortedSet<T> _outputs;
		private T[] _outputArray;

		public event EventHandler<OutputCollectionEventArgs<T>> OutputAdded;
		public event EventHandler<OutputCollectionEventArgs<T>> OutputRemoved;

		public OutputCollection()
		{
			_outputs = new SortedSet<T>(new ByIndex());
		}

		public int OutputCount
		{
			get { return _outputs.Count; }
		}

		public void AddOutput(T output)
		{
			if (output == null) throw new ArgumentNullException("output");

			_AddOutput(output);
		}

		void IHasOutputs.AddOutput(Output output)
		{
			AddOutput(output as T);
		}

		public void RemoveOutput(T output)
		{
			if (output == null) throw new ArgumentNullException("output");

			_RemoveOutput(output);
		}

		void IHasOutputs.RemoveOutput(Output output)
		{
			RemoveOutput(output as T);
		}

		public T this[int index]
		{
			get { return _outputArray[index]; }
		}

		public T[] Outputs
		{
			get
			{
				if (_outputArray == null) {
					_outputArray = _outputs.ToArray();
				}
				return _outputArray;
			}
		}

		Output[] IHasOutputs.Outputs
		{
			get { return Outputs; }
		}

		protected virtual void OnOutputAdded(T output)
		{
			if (OutputAdded != null) {
				OutputAdded(this, new OutputCollectionEventArgs<T>(output));
			}
		}

		protected virtual void OnOutputRemoved(T output)
		{
			if (OutputRemoved != null) {
				OutputRemoved(this, new OutputCollectionEventArgs<T>(output));
			}
		}

		private void _AddOutput(T output)
		{
			if (!_outputs.Add(output)) throw new InvalidOperationException("Output is already present in the collection.");
			_ResetOutputArray();
			OnOutputAdded(output);
		}

		private void _RemoveOutput(T output)
		{
			if (_outputs.Remove(output)) {
				_ResetOutputArray();
				OnOutputRemoved(output);
			}
		}

		private void _ResetOutputArray()
		{
			_outputArray = null;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _outputs.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private class ByIndex : IComparer<Output>
		{
			#region Implementation of IComparer<in Output>

			/// <inheritdoc />
			public int Compare(Output x, Output y)
			{
				if(x.Index == y.Index) return 0;
				if (x.Index < y.Index) return -1;
				return 1;
			}

			#endregion
		}
	}
}