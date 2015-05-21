using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Controls.WpfPropertyGrid.Controls
{
	public class SliderPercentageEditor : PropertyEditor
	{
		public SliderPercentageEditor()
		{
			InlineTemplate = EditorKeys.SliderPercentageEditorKey;
		}
	}
}