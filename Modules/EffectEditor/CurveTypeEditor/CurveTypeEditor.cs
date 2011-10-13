using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.CurveTypeEditor
{
	public class CurveTypeEditor : EffectEditorModuleInstanceBase
	{
		override public IEffectEditorControl CreateEditorControl()
		{
			return new CurveTypeEditorControl();
		}
	}
}
