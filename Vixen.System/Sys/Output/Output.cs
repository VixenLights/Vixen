using System.Collections.Generic;
using System.Linq;

namespace Vixen.Sys.Output {
	abstract public class Output : IHasOutputSources {
		private HashSet<IOutputStateSource> _sources;
		private OutputIntentStateList _state;

		internal Output() {
			Name = "Unnamed";
			_sources = new HashSet<IOutputStateSource>();
		}

		// Completely independent; nothing is current dependent upon this value.
		public string Name { get; set; }

		public void UpdateState() {
			_state = _GetOutputStateData();
		}

		public IIntentStateList State {
			get { return _state; }
		}

		public void AddSource(IOutputStateSource source) {
			_sources.Add(source);
		}

		public void AddSources(IEnumerable<IOutputStateSource> sources) {
			_sources.AddRange(sources);
		}

		public void RemoveSource(IOutputStateSource source) {
			_sources.Remove(source);
		}

		public void ClearSources() {
			_sources.Clear();
		}

		private OutputIntentStateList _GetOutputStateData() {
			IEnumerable<IIntentState> intentStates = _sources.SelectMany(_GetSourceData).NotNull();
			IIntentState[] states = intentStates.ToArray();
			return new OutputIntentStateList(states);
		}

		private IIntentStateList _GetSourceData(IOutputStateSource source) {
			return source.State;
		}
	}
}
