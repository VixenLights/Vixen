using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Controls.WpfPropertyGrid.Controls
{
	public class SliderEditor : PropertyEditor
	{
		public SliderEditor()
		{
			InlineTemplate = EditorKeys.SliderEditorKey;
		}
	}
}
