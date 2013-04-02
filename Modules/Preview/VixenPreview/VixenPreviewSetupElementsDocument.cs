using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace VixenModules.Preview.VixenPreview
{
    public partial class VixenPreviewSetupElementsDocument : DockContent
    {
        public VixenPreviewSetupElementsDocument()
        {
            InitializeComponent();
        }

        private void VixenPreviewSetupElementsDocument_Load(object sender, EventArgs e)
        {
            Shapes.PreviewTools.PopulateElementTree(treeElements);
        }
    }
}
