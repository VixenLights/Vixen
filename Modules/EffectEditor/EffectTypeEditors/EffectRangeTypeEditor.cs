using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace VixenModules.EffectEditor.EffectTypeEditors
{
	public class EffectRangeTypeEditor: UITypeEditor
	{
		private IWindowsFormsEditorService service;
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (provider != null)
				service = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

			if (service != null && context != null)
			{
				// This is the code you want to run when the [...] is clicked and after it has been verified.
				RangeTypeEditorControl control = new RangeTypeEditorControl {Value = Convert.ToInt32(value ?? 1)};
				service.DropDownControl(control);
				value = control.Value;
			}

			return value;
		}
	}
}
