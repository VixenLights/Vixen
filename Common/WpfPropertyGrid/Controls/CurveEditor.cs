using System.Windows.Forms;
using VixenModules.App.Curves;

namespace System.Windows.Controls.WpfPropertyGrid.Controls
{
	public class CurveEditor : TypeEditor
	{
		public CurveEditor()
		{
			EditedType = KnownTypes.Wpf.Curve;
			InlineTemplate = EditorKeys.CurveEditorKey;
			ExtendedTemplate = null;
		}

		public override void ShowDialog(PropertyItemValue propertyValue, IInputElement commandSource)
		{
			if (propertyValue == null) return;
			if (propertyValue.ParentProperty.IsReadOnly) return;

			Curve curveValue = propertyValue.Value as Curve;
			VixenModules.App.Curves.CurveEditor editor = new VixenModules.App.Curves.CurveEditor(curveValue ?? new Curve());
			if (editor.ShowDialog() == DialogResult.OK)
			{
				propertyValue.Value = editor.Curve;
			}
		}
	}
}