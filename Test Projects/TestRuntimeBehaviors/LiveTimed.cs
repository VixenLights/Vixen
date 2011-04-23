using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Common;
using Vixen.Module;
using Vixen.Module.RuntimeBehavior;
//using Vixen.Module.Sequence;
using Vixen.Module.Timing;

namespace TestRuntimeBehaviors {
	public class LiveTimed : IRuntimeBehaviorModuleInstance {
		private Tuple<string, Action>[] _actions;
		private ISequence _sequence;
		private ITiming _timingSource;
		private LiveTimedData _moduleData;

		public LiveTimed() {
			_actions = new Tuple<string, Action>[] {
				//Examples:
				//new Tuple<string,Action>("Clear buffer", Clear),
				//new Tuple<string,Action>("Commit", Commit)
			};
		}

		public void Startup(ISequence sequence, ITiming timingSource) {
			_sequence = sequence;
			_timingSource = timingSource;
		}

		public void Shutdown() { }

		//public IEnumerable<CommandNode> GenerateCommandNodes(InsertDataParameters parameters) {
		public void GenerateCommandNodes(InsertDataParameters parameters) {
			CommandNode commandNode;

			commandNode = new CommandNode(parameters.Command, parameters.Channels, parameters.StartTime, parameters.TimeSpan);

			////Experiment: Having a flag on CommandNode that doesn't allow frames to be skipped;
			////            explicitly implemented by this behavior.
			//if(commandNode.Command == null) {
			//    commandNode.IsRequired = true;
			//}

			//yield return commandNode;
		}

		public bool Enabled {
			get { return _moduleData.Enabled; }
			set { _moduleData.Enabled = value; }
		}

		public Tuple<string, Action>[] BehaviorActions {
			get { return _actions; }
		}

		public Guid TypeId {
			get { return LiveTimedModule._typeId; }
		}

		public Guid InstanceId { get; set; }

		public IModuleDataModel ModuleData {
			get { return _moduleData; }
			set { _moduleData = value as LiveTimedData; }
		}

		public string TypeName { get; set; }

		public void Dispose() { }
	}
}
