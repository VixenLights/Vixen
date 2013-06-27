using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.ChaseEffectEditor
{
	internal class ChaseEffectEditor : EffectEditorModuleInstanceBase
	{
		public override IEffectEditorControl CreateEditorControl()
		{
			return new ChaseEffectEditorControl();
		}
	}
}