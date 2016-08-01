using System;
using System.Windows;
using Microsoft.Win32;

namespace VixenModules.Editor.EffectEditor.Editors
{
	public class GledPathEditor : PropertyEditor
	{
		public GledPathEditor()
		{
			InlineTemplate = EditorKeys.FilePathEditorKey;
		}

		public override Object ShowDialog(PropertyItem propertyItem, Object propertyValue, IInputElement commandSource)
		{
			OpenFileDialog ofd = new OpenFileDialog
			{
				Filter = "Glediator Files (*.gled)|*.gled",
				Multiselect = false
			};

			ofd.Title = propertyItem.DisplayName;

			if (ofd.ShowDialog() == true)
			{
				propertyValue = ofd.FileName;
			}

			return propertyValue;
		}
	}
}
