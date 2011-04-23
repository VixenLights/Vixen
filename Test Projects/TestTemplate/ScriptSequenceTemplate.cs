using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.FileTemplate;
using Vixen.Module.RuntimeBehavior;
using Vixen.Module.Sequence;
using Vixen.Sys;
using Vixen.Common;

namespace TestTemplate {
	public class ScriptSequenceTemplate : IFileTemplateModuleInstance {
		private ScriptSequenceTemplateData _data;

		public Guid[] EnabledBehaviors {
			get { return _data.Behaviors.ToArray(); }
			set {
				_data.Behaviors.Clear();
				_data.Behaviors.AddRange(value ?? new Guid[] { });
			}
		}

		public long Length {
			get { return _data.Length; }
			set { _data.Length = value; }
		}

		public int TimingInterval {
			get { return _data.TimingInterval; }
			set { _data.TimingInterval = value; }
		}

		public void Project(object target) {
			ScriptSequence scriptSequence = target as ScriptSequence;

			scriptSequence.Length = Length;
			scriptSequence.Data.TimingInterval = TimingInterval;
			foreach(IRuntimeBehaviorModuleInstance runtimeBehavior in scriptSequence.RuntimeBehaviors) {
				runtimeBehavior.Enabled = EnabledBehaviors.Contains(runtimeBehavior.TypeId);
			}
		}

		public void Setup() {
			using(ScriptSequenceTemplateSetup dialog = new ScriptSequenceTemplateSetup(this)) {
				if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
					ApplicationServices.CommitTemplate(this);
				}
			}
		}

		public Guid TypeId {
			get { return ScriptSequenceTemplateModule._typeId; }
		}

		public Guid InstanceId { get; set; }

		public IModuleDataModel ModuleData {
			get { return _data; }
			set {
				_data = value as ScriptSequenceTemplateData;
			}
		}

		public string TypeName { get; set; }

		public void Dispose() {
		}
	}
}
