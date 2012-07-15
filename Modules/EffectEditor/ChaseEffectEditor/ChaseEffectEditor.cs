using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module;
using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.ChaseEffectEditor
{
	class ChaseEffectEditor : EffectEditorModuleInstanceBase
	{
		override public IEffectEditorControl CreateEditorControl()
		{
			return new ChaseEffectEditorControl();
		}
	}
}
