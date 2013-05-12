using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Collections.Generic;

namespace VixenModules.Preview.VixenPreview.Shapes
{
    class PreviewSetElementsUIEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = (IWindowsFormsEditorService)
                provider.GetService(typeof(IWindowsFormsEditorService));
            if (svc != null)
            {
                List<PreviewBaseShape> shapes = value as List<PreviewBaseShape>;
                PreviewSetElements elementsDialog = new PreviewSetElements(shapes);
                svc.ShowDialog(elementsDialog);
                // update etc
            }
            return value;
        }
    }
}
