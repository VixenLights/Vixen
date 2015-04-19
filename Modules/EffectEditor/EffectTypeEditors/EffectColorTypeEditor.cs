using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using Vixen.Module.Effect;
using VixenModules.EffectEditor.ColorTypeEditor;

namespace VixenModules.EffectEditor.EffectTypeEditors
{
	public class EffectColorTypeEditor : UITypeEditor
	{
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			// This tells it to show the [...] button which is clickable firing off EditValue below.
			return UITypeEditorEditStyle.Modal;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{

			if (value != null && context != null)
			{
				IEffect effect = context.Instance as IEffect;
				if (effect != null)
				{
					ColorTypeEditorControl control = new ColorTypeEditorControl
					{
						TargetEffect = effect,
						ColorValue = (Color)value
					};
					control.ShowEditor();
					value = control.ColorValue;
				}
			}
			
			return value;
		}

		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override void PaintValue(PaintValueEventArgs e)
		{
			if (e.Value is Color)
			{
				Color color = (Color)e.Value;
				SolidBrush b = new SolidBrush(color);
				e.Graphics.FillRectangle(b, e.Bounds);
				b.Dispose();
			}
		}
	}
}
