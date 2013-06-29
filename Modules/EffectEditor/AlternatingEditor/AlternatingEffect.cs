using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.AlternatingEditor
{
	internal class AlternatingEffect : EffectEditorModuleInstanceBase
	{
		public override IEffectEditorControl CreateEditorControl()
		{
			return new AlternatingEffectEditorControl();
		}
	}
}