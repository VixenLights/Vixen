using Common.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using VixenModules.App.Polygon;

namespace VixenModules.Editor.EffectEditor.Editors
{
	public class PolygonTypeEditor : TypeEditor
	{
		public PolygonTypeEditor() : base(typeof(Polygon), EditorKeys.PolygonEditorKey)
		{

		}

		public override object ShowDialog(PropertyItem propertyItem, object value, IInputElement commandSource)
		{

			try
			{
				var newWindow = new PolygonEditor.PolygonEditorView();
				newWindow.Show();

				/*using (FontDialog ofd = new FontDialog())
				{
					ofd.ShowColor = false;
					if (value is Font)
					{
						ofd.Font = (Font)value;
					}

					if (ofd.ShowDialog() == DialogResult.OK)
					{
						value = ofd.Font;
					}
				}
				*/
			}
			catch (Exception e)
			{
				var messageBox = new MessageBoxForm("An error occurred loading the polygon editor.", "Polygon Error", MessageBoxButtons.OK, SystemIcons.Error);
				messageBox.ShowDialog();				
			}


			return value;
		}
	}
}
