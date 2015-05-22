using System;
using System.Windows;
using System.Windows.Controls.WpfPropertyGrid;
using System.Windows.Forms;
using Vixen.Module.EffectEditor;
using VixenModules.App.Curves;

namespace VixenModules.EffectEditor.EffectPropertyEditors
{
	public class CurveEditor : TypeEditor
	{
		public CurveEditor()
			: base(typeof(Curve), EditorKeys.CurveEditorKey, null)
		{
		}

		public override Object ShowDialog(Object effect, Object propertyValue, IInputElement commandSource)
		{
			if (propertyValue == null)
			{
				return propertyValue;
			}
			
			Curve curveValue = propertyValue as Curve;
			VixenModules.App.Curves.CurveEditor editor = new VixenModules.App.Curves.CurveEditor(curveValue ?? new Curve());
			if (editor.ShowDialog() == DialogResult.OK)
			{
				propertyValue = editor.Curve;
			}

			return propertyValue;
		}
	}
}