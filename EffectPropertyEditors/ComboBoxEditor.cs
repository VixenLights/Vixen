using System;
using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.EffectPropertyEditors
{
	public class ComboBoxEditor : PropertyEditor
	{
		public ComboBoxEditor(object inlineTemplate)
		{
			InlineTemplate = inlineTemplate;
		}

		public ComboBoxEditor(Type declaringType, string propertyName, object inlineTemplate):base(declaringType,propertyName, inlineTemplate)
		{
			
		}
	}
}