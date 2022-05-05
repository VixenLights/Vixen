using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Collections.Generic;
using Common.Controls;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	internal class PreviewSetElementsUIEditor : UITypeEditor
	{
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			IWindowsFormsEditorService svc = (IWindowsFormsEditorService)
			                                 provider.GetService(typeof (IWindowsFormsEditorService));
			if (svc != null) {
				List<PreviewLightBaseShape> shapes = value as List<PreviewLightBaseShape>;
				if (shapes.Count < 1)
				{
					//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
					MessageBoxForm.msgIcon = SystemIcons.Error; //this is used if you want to add a system icon to the message form.
					var messageBox = new MessageBoxForm("Elements must have at least one shape. Remove the selected element.", "Error", false, false);
					messageBox.ShowDialog();
					if (value != null) return value;
				}
				PreviewSetElements elementsDialog = new PreviewSetElements(shapes);
				svc.ShowDialog(elementsDialog);
				// update etc
                if (shapes[0].Parent != null)
                {
                    shapes[0].Parent.Layout();
                }
			}
			return value;
		}
	}
}