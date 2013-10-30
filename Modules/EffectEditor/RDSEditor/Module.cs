using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.RDSEditor {
	public class Module: EffectEditorModuleInstanceBase
	{
		public override IEffectEditorControl CreateEditorControl()
		{
			return new RDSEditorControl();
		}
	}  
}
