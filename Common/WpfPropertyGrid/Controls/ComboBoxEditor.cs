using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Controls.WpfPropertyGrid.Controls
{
	public class ComboBoxEditor: PropertyEditor
	{
		public ComboBoxEditor()
		{
			InlineTemplate = EditorKeys.ComboBoxEditorKey;
		}
	}
}
