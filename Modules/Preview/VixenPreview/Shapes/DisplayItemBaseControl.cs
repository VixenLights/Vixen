using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VixenModules.Preview.VixenPreview.Shapes
{
    public partial class DisplayItemBaseControl : UserControl
    {
        public  DisplayItem _displayItem;
        private string _title;

        public DisplayItemBaseControl()
        {
            InitializeComponent();
        }

        public DisplayItemBaseControl(DisplayItem displayItem)
        {
            InitializeComponent();
        }

        public string Title
        {
            get {return _title;}
            set {_title = value; }
        }

        public DisplayItem DisplayItem
        {
            get { return _displayItem; }
            set { _displayItem = value; }
        }    }
}
