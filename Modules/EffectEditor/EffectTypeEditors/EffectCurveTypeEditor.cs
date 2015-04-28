using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using Vixen.Module.Effect;
using VixenModules.App.Curves;
using VixenModules.EffectEditor.CurveTypeEditor;

namespace VixenModules.EffectEditor.EffectTypeEditors
{
	public class EffectCurveTypeEditor : UITypeEditor
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
				IEffect effect = null;
				if (context.Instance.GetType().IsArray)
				{
					IEffect[] effects = context.Instance as IEffect[];
					if (effects != null)
					{
						effect = effects.First();
						value = new Curve();
					}
				}
				else
				{
					effect = context.Instance as IEffect;
				}
				if (effect != null)
				{
					CurveTypeEditorControl control = new CurveTypeEditorControl();
					control.CurveValue = (Curve)value;
					control.ShowEditor();
					value = control.CurveValue;
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
			if (e.Value is Curve)
			{
				Curve curve = (Curve)e.Value;
				Bitmap bitmap = curve.GenerateGenericCurveImage(e.Bounds.Size);
				e.Graphics.DrawImage(bitmap, e.Bounds);
				bitmap.Dispose();
			}
		}
	}
}
