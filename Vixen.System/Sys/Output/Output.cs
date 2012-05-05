using System.Collections.Generic;
using System.Linq;
//using Vixen.Commands;
//using Vixen.Data.Policy;
//using Vixen.Module.PostFilter;

namespace Vixen.Sys.Output {
	//abstract public class Output<ControllerValueType> : /*IHasPostFilters,*/ IHasOutputSources {
	abstract public class Output : /*IHasPostFilters,*/ IHasOutputSources {
		private HashSet<IOutputStateSource> _sources;
		//private PostFilterCollection _postFilters;
		private OutputIntentStateList _state;

		internal Output() {
			Name = "Unnamed";
			//_postFilters = new PostFilterCollection();
			_sources = new HashSet<IOutputStateSource>();
		}

		//*** Going to try to have the owning controller maintain a collection of states for each
		//    output since the output doesn't use it and can't even generate it without the
		//    controller's data policy
		//public virtual ControllerValueType ControllerValue { get; set; }

		// Completely independent; nothing is current dependent upon this value.
		public string Name { get; set; }

		//public void AddSources(IEnumerable<IOutputStateSource> sources) {
		//    foreach(IOutputStateSource source in sources) {
		//        AddSource(source);
		//    }
		//}

		//public void AddSource(IOutputStateSource source) {
		//    _sources.Add(source);
		//}

		//public void RemoveSources(IEnumerable<IOutputStateSource> sources) {
		//    foreach(IOutputStateSource source in sources) {
		//        _sources.Remove(source);
		//    }
		//}

		//public void RemoveSource(IOutputStateSource source) {
		//    _sources.Remove(source);
		//}

		//public void ClearSources() {
		//    _sources.Clear();
		//}

		//public void AddPostFilter(IPostFilterModuleInstance filter) {
		//    _postFilters.Add(filter);
		//}

		//public void InsertPostFilter(int index, IPostFilterModuleInstance filter) {
		//    _postFilters.Insert(index, filter);
		//}

		//public void RemovePostFilter(IPostFilterModuleInstance filter) {
		//    _postFilters.Remove(filter);
		//}

		//public void ClearPostFilters() {
		//    _postFilters.Clear();
		//}

		//public PostFilterCollection PostFilters {
		//    get { return _postFilters; }
		//}

		public void UpdateState() {
			_state = _GetOutputStateData();
		}

		//private void _FilterState() {
		//    if(VixenSystem.AllowFilterEvaluation && Command != null) {
		//        foreach(IPostFilterModuleInstance postFilter in PostFilters) {
		//            _command = postFilter.Affect(_command);
		//            if(_command == null) return;
		//        }
		//    }
		//}

		public IIntentStateList State {
			get { return _state; }
		}

		////*** Not yet any way to set this for an output.
		////    It is intended to allow an output to override the controller's data policy.
		//public OutputDataPolicy DataPolicy { get; set; }

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
