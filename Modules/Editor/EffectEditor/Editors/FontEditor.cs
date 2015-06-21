using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace VixenModules.Editor.EffectEditor.Editors
{
	public class FontEditor:TypeEditor
	{
		public FontEditor(): base(typeof(Font), EditorKeys.FontEditorKey)
		{
			
		}

		public override object ShowDialog(object effect, object value, IInputElement commandSource)
		{
			FontDialog ofd = new FontDialog();
			ofd.ShowColor = false;
			if (value is Font)
			{
				ofd.Font = (Font) value;
			}
			
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				value = ofd.Font;
			}

			return value;
		}
	}
}
