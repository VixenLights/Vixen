using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using CommandStandard;
using CommandStandard.Types;
using Vixen.Module;
using Vixen.Module.EffectEditor;

namespace TestCommandEditors {
	public class TestCommandEditor : EffectEditorModuleInstanceBase {
		override public IEffectEditorControl CreateEditorControl() {
			return new TestCommandEditorControl();
		}
	}
}
