﻿using System.Windows;
using Microsoft.Win32;

namespace VixenModules.Editor.EffectEditor.Editors
{
	public class FilePathEditor : PropertyEditor
	{
		public FilePathEditor()
		{
			InlineTemplate = EditorKeys.FilePathEditorKey;
		}

		public override Object ShowDialog(PropertyItem propertyItem, Object propertyValue, IInputElement commandSource)
		{
			OpenFileDialog ofd = new OpenFileDialog
			{
				//Filter = "Image Files (*.jpg, *.png, *.bmp)|*.jpg;*.png;*.bmp",
				Multiselect = false
			};

			if (ofd.ShowDialog() == true)
			{
				propertyValue = ofd.FileName;
			}

			return propertyValue;
		}
	}
}
