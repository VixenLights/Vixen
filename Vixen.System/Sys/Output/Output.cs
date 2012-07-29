using System.Collections.Generic;
using System.Linq;

namespace Vixen.Sys.Output {
	abstract public class Output : IHasOutputSources {
		private HashSet<IOutputStateSource> _sources;

		internal Output() {
			Name = "Unnamed";
			_sources = new HashSet<IOutputStateSource>();
		}

		// Completely independent; nothing is current dependent upon this value.
		public string Name { get; set; }

		virtual public void UpdateState() {
			State = new OutputIntentStateList(GetOutputStateData());
		}

		public IIntentStates State { get; protected set; }

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

		protected IIntentState[] GetOutputStateData() {
			return _sources.SelectMany(_GetSourceData).NotNull().ToArray();
		}

		private IIntentStates _GetSourceData(IOutputStateSource source) {
			return source.State;
		}
	}
}
