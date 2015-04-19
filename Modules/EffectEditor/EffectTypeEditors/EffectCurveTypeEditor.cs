using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
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

			if (value != null && context != null)
			{
				IEffect effect = context.Instance as IEffect;
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
				Bitmap bitmap = curve.GenerateCurveImage(e.Bounds.Size);
				e.Graphics.DrawImageUnscaled(bitmap, e.Bounds);
				bitmap.Dispose();
			}
		}
	}
}
