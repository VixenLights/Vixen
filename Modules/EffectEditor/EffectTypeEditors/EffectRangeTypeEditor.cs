using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms.Design;
using Vixen.TypeConverters;

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
				object myValue = context.PropertyDescriptor.Converter.ConvertTo(context, CultureInfo.CurrentCulture, value, typeof (string));
				
				RangeTypeEditorControl control = new RangeTypeEditorControl{Value = 0};
				AttributeCollection attributes = context.PropertyDescriptor.Attributes;
				RangeAttribute attribute = (RangeAttribute)attributes[typeof(RangeAttribute)];
				if (attribute != null)
				{
					control.LowerBounds = attribute.Lower;
					control.UpperBounds = attribute.Upper;

				}
				control.Value = Convert.ToInt32(myValue ?? 1);
				service.DropDownControl(control);
				object val = context.PropertyDescriptor.Converter.ConvertFrom(context, CultureInfo.CurrentCulture, control.Value);
				value = val;
			}

			return value;
		}
	}
}
