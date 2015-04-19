using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using Vixen.Module.Effect;
using VixenModules.App.ColorGradients;
using VixenModules.EffectEditor.ColorGradientTypeEditor;

namespace VixenModules.EffectEditor.EffectTypeEditors
{
	public class EffectColorGradientTypeEditor: UITypeEditor
	{

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			// This tells it to show the [...] button which is clickable firing off EditValue below.
			return UITypeEditorEditStyle.Modal;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			ColorGradientTypeEditorControl control = new ColorGradientTypeEditorControl();
			if (context != null)
			{
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
					control.ColorGradientValue = (ColorGradient)value;
					DialogResult result = control.ShowEditor();
					if (result == DialogResult.OK)
					{
						value = control.ColorGradientValue;	
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
			if (e.Value is ColorGradient)
			{
				ColorGradient cg = (ColorGradient)e.Value;
				Bitmap bitmap = cg.GenerateColorGradientImage(e.Bounds.Size, false);
				e.Graphics.DrawImageUnscaled(bitmap,e.Bounds);
				bitmap.Dispose();
			}
		}
	}
}
