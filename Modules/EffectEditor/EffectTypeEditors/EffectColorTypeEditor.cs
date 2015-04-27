using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
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

			if (context != null)
			{
				ColorTypeEditorControl control = new ColorTypeEditorControl();
				IEffect effect = null;
				if (context.Instance.GetType().IsArray)
				{
					IEffect[] effects = context.Instance as IEffect[];
					if (effects != null)
					{
						effect = effects.First();
					}
				}
				else
				{
					effect = context.Instance as IEffect;
				}
				if (effect != null)
				{
					control.TargetEffect = effect;
					if (value != null)
					{
						control.ColorValue = (Color)value;
					}
					DialogResult result = control.ShowEditor();
					if (result == DialogResult.OK)
					{
						value = control.ColorValue;	
					}
		
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
