using System;
using Vixen.Data.Flow;

namespace Vixen.Sys.Output {
	abstract public class Output {
		//private HashSet<IOutputStateSource> _sources;

		internal Output() {
			Name = "Unnamed";
			//_sources = new HashSet<IOutputStateSource>();
			Id = Guid.NewGuid();
		}

		// Completely independent; nothing is current dependent upon this value.
		public string Name { get; set; }

		public Guid Id { get; set; }

		//virtual public void UpdateState() {
		//    State = new OutputIntentStateList(GetOutputStateData());
		//}
		public void Update() {
			//State = Source.Component.Outputs[Source.OutputIndex].Data;
			if(Source != null) {
				State = Source.GetOutputState();
			}
		}

		//public IIntentStates State { get; protected set; }

		//public void AddSource(IOutputStateSource source) {
		//    _sources.Add(source);
		//}

		//public void AddSources(IEnumerable<IOutputStateSource> sources) {
		//    _sources.AddRange(sources);
		//}

		//public void RemoveSource(IOutputStateSource source) {
		//    _sources.Remove(source);
		//}

		//public void ClearSources() {
		//    _sources.Clear();
		//}

		public IDataFlowComponentReference Source { get; set; }

		public IDataFlowData State { get; private set; }

		//protected IIntentState[] GetOutputStateData() {
		//    return _sources.SelectMany(_GetSourceData).NotNull().ToArray();
		//}

		//private IIntentStates _GetSourceData(IOutputStateSource source) {
		//    return source.State;
		//}
	}
}
