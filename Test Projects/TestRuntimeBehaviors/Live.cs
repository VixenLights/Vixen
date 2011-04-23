using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Common;
using Vixen.Module;
using Vixen.Module.RuntimeBehavior;
using Vixen.Module.Sequence;

namespace TestRuntimeBehaviors {
	public class Live : IRuntimeBehaviorModuleInstance {
		private Tuple<string, Action>[] _actions;
		private ISequenceModuleInstance _sequence;
		private ITimingSource _timingSource;
		private LiveData _moduleData;

		public Live() {
			_actions = new Tuple<string, Action>[] {
				//Examples:
				//new Tuple<string,Action>("Clear buffer", Clear),
				//new Tuple<string,Action>("Commit", Commit)
			};
		}

		public void Startup(ISequenceModuleInstance sequence, ITimingSource timingSource) {
			_sequence = sequence;
			_timingSource = timingSource;
		}

		public void Shutdown() { }

		public IEnumerable<CommandNode> GenerateCommandNodes(InsertDataParameters parameters) {
			CommandNode commandNode;

			// Live data is at a disadvantage because the time is being set as now, but time
			// is going to pass before it's evaluated so the risk of expiration is high.
			// For that reason, we're going to give it an arbitrary amount of time to
			// get to where it's going.
			commandNode = new CommandNode(parameters.Command, parameters.Channels, _moduleData.HeadStart + _timingSource.Position, parameters.TimeSpan);
		
			//Experiment: Having a flag on CommandNode that doesn't allow frames to be skipped;
			//            explicitly implemented by this behavior.
			if(commandNode.Command == null) {
				commandNode.IsRequired = true;
			}

			yield return commandNode;
		}

		public bool Enabled {
			get { return _moduleData.Enabled; }
			set { _moduleData.Enabled = value; }
		}

		public int HeadStart {
			get { return _moduleData.HeadStart; }
			set { _moduleData.HeadStart = value; }
		}

		public Tuple<string, Action>[] BehaviorActions {
			get { return _actions; }
		}

		public Guid TypeId {
			get { return LiveModule._typeId; }
		}

		public Guid InstanceId { get; set; }

		public IModuleDataModel ModuleData {
			get { return _moduleData; }
			set { _moduleData = value as LiveData; }
		}

		public string TypeName { get; set; }

		public void Dispose() { }
	}
}
