using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VixenModules.Preview.DisplayPreview
{
    public partial class PreviewForm : Form
    {
        public PreviewForm()
        {
            InitializeComponent();
			Size=new Size(0,0);
        }

		private void PreviewFormVisibleChanged(object sender, EventArgs e)
		{
			//Hack to prevent form from showing
			Visible = false;
		}

    }
}
