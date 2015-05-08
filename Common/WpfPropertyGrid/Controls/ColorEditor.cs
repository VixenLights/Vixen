using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Controls.WpfPropertyGrid.Controls
{
	public class ColorEditor : PropertyEditor
	{
		public ColorEditor()
		{
			InlineTemplate = EditorKeys.ColorEditorKey;
		}

		public override void ShowDialog(PropertyItemValue propertyValue, IInputElement commandSource)
		{
			if (propertyValue == null) return;
			if (propertyValue.ParentProperty.IsReadOnly) return;
			MessageBox.Show("edit");
		}
	}
}
