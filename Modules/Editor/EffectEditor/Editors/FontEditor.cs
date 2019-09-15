using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Common.Controls;

namespace VixenModules.Editor.EffectEditor.Editors
{
	public class FontEditor:TypeEditor
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		public FontEditor(): base(typeof(Font), EditorKeys.FontEditorKey)
		{
			
		}

		public override object ShowDialog(PropertyItem propertyItem, object value, IInputElement commandSource)
		{
			
			try
			{
				using (FontDialog ofd = new FontDialog())
				{
					ofd.ShowColor = false;
					if (value is Font)
					{
						ofd.Font = (Font) value;
					}

					if (ofd.ShowDialog() == DialogResult.OK)
					{
						value = ofd.Font;
					}
				}

			}
			catch (Exception e)
			{
				var messageBox = new MessageBoxForm("An error occurred loading the font.", "Font Error", MessageBoxButtons.OK, SystemIcons.Error);
				messageBox.ShowDialog();
				Logging.Error(e, "An error occurred loading the font.");
			}
			

			return value;
		}
	}
}
