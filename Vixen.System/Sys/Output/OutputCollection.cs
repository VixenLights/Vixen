//using System;

using System;
using System.Collections.Generic;
//using System.Linq;

namespace Vixen.Sys.Output {
	class OutputCollection<T> : IEnumerable<T>
		where T : Output, new() {
		private List<T> _outputs;
		private T[] _outputArray;
		//private HashSet<IOutputSourceCollection> _sourceCollections;
		//private Guid _controllerId;

		public event EventHandler<OutputCollectionEventArgs<T>> OutputAdded;
		public event EventHandler<OutputCollectionEventArgs<T>> OutputRemoved;

		//public OutputCollection(Guid owningControllerId) {
		public OutputCollection() {
			_outputs = new List<T>();
			_outputArray = new T[0];
			//_sourceCollections = new HashSet<IOutputSourceCollection>();
			//_controllerId = owningControllerId;
		}

		public int Count {
			get { return _outputs.Count; }
			set {
				// Adjust the outputs list.
				lock(_outputs) {
					if(value < _outputs.Count) {
						//_outputs.RemoveRange(value, _outputs.Count - value);
						while(_outputs.Count > value) {
							T output = _outputs[value];
							_outputs.RemoveAt(value);
							OnOutputRemoved(output);
						}
					} else {
						while(_outputs.Count < value) {
							// Create a new output.
							T output = new T();
							_outputs.Add(output);
							OnOutputAdded(output);
						}
					}
				}

				_outputArray = _outputs.ToArray();
			}
		}

		public T this[int index] {
			get { return _outputArray[index]; }
		}

		//public void AddSources(IOutputSourceCollection sourceCollection) {
		//    if(_sourceCollections.Add(sourceCollection)) {
		//        ReloadSources();
		//    }
		//}

		//public void RemoveSources(IOutputSourceCollection sourceCollection) {
		//    if(sourceCollection != null) {
		//        if(_sourceCollections.Remove(sourceCollection)) {
		//            ReloadSources();
		//        }
		//    }
		//}

		//public void ReloadSources() {
		//    for(int i = 0; i < Count; i++) {
		//        ReloadOutputSources(i);
		//    }
		//}

		//public void ReloadOutputSources(int outputIndex) {
		//    if(outputIndex < Count) {
		//        Output output = _outputs[outputIndex];
		//        IEnumerable<IOutputStateSource> outputSources = _GetAllOutputSources(outputIndex);
		//        output.ClearSources();
		//        output.AddSources(outputSources);
		//    }
		//}

		//public void ClearSources(int outputIndex) {
		//    if(outputIndex < Count) {
		//        _outputs[outputIndex].ClearSources();
		//    }
		//}

		public T[] AsArray {
			get { return _outputArray; }
		}

		protected virtual void OnOutputAdded(T output) {
			if(OutputAdded != null) {
				OutputAdded(this, new OutputCollectionEventArgs<T>(output));
			}
		}

		protected virtual void OnOutputRemoved(T output) {
			if(OutputRemoved != null) {
				OutputRemoved(this, new OutputCollectionEventArgs<T>(output));
			}
		}

		public IEnumerator<T> GetEnumerator() {
			return _outputs.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		//private IEnumerable<IOutputStateSource> _GetAllOutputSources(int outputIndex) {
		//    if(outputIndex < Count) {
		//        return _sourceCollections.SelectMany(x => x.GetOutputSources(_controllerId, outputIndex));
		//    }
		//    return Enumerable.Empty<IOutputStateSource>();
		//}
	}
}
