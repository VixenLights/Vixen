using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using VixenModules.EffectEditor.LevelTypeEditor;

namespace VixenModules.EffectEditor.EffectTypeEditors
{
	public class EffectLevelTypeEditor : UITypeEditor
	{
		private IWindowsFormsEditorService _service;
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (provider != null)
				_service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

			if (_service != null && context != null)
			{
				// This is the code you want to run when the [...] is clicked and after it has been verified.
				LevelTypeEditorControl control = new LevelTypeEditorControl {LevelValue = Convert.ToDouble(value ?? 1)};
				_service.DropDownControl(control);
				value = control.LevelValue;
			}

			return value;
		}

	}
}
