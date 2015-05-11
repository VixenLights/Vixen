using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Controls.WpfPropertyGrid.Controls
{
	public class CurveEditor:PropertyEditor
	{
		public CurveEditor()
		{
			InlineTemplate = EditorKeys.CurveEditorKey;
		}

		public override void ShowDialog(PropertyItemValue propertyValue, IInputElement commandSource)
		{
			if (propertyValue == null) return;
			if (propertyValue.ParentProperty.IsReadOnly) return;
			MessageBox.Show("edit");
		}
	}
}
