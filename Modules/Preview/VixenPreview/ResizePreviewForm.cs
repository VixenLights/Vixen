using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VixenModules.Preview.VixenPreview
{
    public partial class ResizePreviewForm : Form
    {
        int _origWidth, _origHeight;
        int _newWidth, _newHeight;
        
        public ResizePreviewForm(int width, int height)
        {
            InitializeComponent();
            _origWidth = width;
            _origHeight = height;
        }

        public new int Width {
            get { return _newWidth; }
        }

        public new int Height
        {
            get { return _newHeight; }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            _newWidth = (int)numericWidth.Value;
            _newHeight = (int)numericHeight.Value;
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void ResizePreviewForm_Load(object sender, EventArgs e)
        {
            numericWidth.Value = _origWidth;
            numericHeight.Value = _origHeight;
            labelWidth.Text = _origWidth.ToString();
            labelHeight.Text = _origHeight.ToString();
        }

        private void numericWidth_Leave(object sender, EventArgs e)
        {
            double aspect = (double)numericWidth.Value / (double)_origWidth;
            numericHeight.Value = (int)((double)_origHeight * aspect);
        }


    }
}
