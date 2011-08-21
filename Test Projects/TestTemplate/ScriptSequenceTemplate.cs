using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.ModuleTemplate;
using Vixen.Module.RuntimeBehavior;
using Vixen.Module.Sequence;
using Vixen.Sys;

namespace TestTemplate {
	public class ScriptSequenceTemplate : ModuleTemplateModuleInstanceBase {
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

		override public void Project(IModuleInstance target) {
			ScriptSequence scriptSequence = target as ScriptSequence;

			scriptSequence.Length = Length;
			foreach(IRuntimeBehaviorModuleInstance runtimeBehavior in scriptSequence.RuntimeBehaviors) {
				runtimeBehavior.Enabled = EnabledBehaviors.Contains(runtimeBehavior.Descriptor.TypeId);
			}
		}

		//override public void Project(object target) {
		//    ScriptSequence scriptSequence = target as ScriptSequence;

		//    scriptSequence.Length = Length;
		//    foreach(IRuntimeBehaviorModuleInstance runtimeBehavior in scriptSequence.RuntimeBehaviors) {
		//        runtimeBehavior.Enabled = EnabledBehaviors.Contains(runtimeBehavior.Descriptor.TypeId);
		//    }
		//}

		override public void Setup() {
			using(ScriptSequenceTemplateSetup dialog = new ScriptSequenceTemplateSetup(this)) {
				if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
					//ApplicationServices.CommitTemplate(this);
				}
			}
		}

		override public IModuleDataModel ModuleData {
			get { return _data; }
			set { _data = value as ScriptSequenceTemplateData; }
		}
	}
}
