using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys.Attribute;

namespace Vixen.Module.EffectEditor
{
	[TypeOfModule("EffectEditor")]
	internal class EffectEditorModuleImplementation : ModuleImplementation<IEffectEditorModuleInstance>
	{
		public EffectEditorModuleImplementation()
			: base(new EffectEditorModuleManagement(), new EffectEditorModuleRepository())
		{
		}
	}
}