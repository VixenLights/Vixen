using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.NutcrackerEffectEditor
{
	public class NutcrackerTypeEditor : EffectEditorModuleInstanceBase
	{
		override public IEffectEditorControl CreateEditorControl()
		{
            Console.WriteLine("---->CreateEditorControl");
            return new NutcrackerTypeEditorControl();
		}
	}
}
