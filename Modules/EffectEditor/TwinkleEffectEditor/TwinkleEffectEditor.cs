using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.TwinkleEffectEditor
{
	internal class TwinkleEffectEditor : EffectEditorModuleInstanceBase
	{
		public override IEffectEditorControl CreateEditorControl()
		{
			return new TwinkleEffectEditorControl();
		}
	}
}