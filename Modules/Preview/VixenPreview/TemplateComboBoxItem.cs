using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VixenModules.Preview.VixenPreview
{
    class TemplateComboBoxItem
    {
        string _caption = "";
        string _fileName = "";

        public TemplateComboBoxItem(string caption, string fileName)
        {
            Caption = caption;
            FileName = fileName;
        }

        public override string ToString()
        {
            return Caption;
        }

        public string Caption
        {
            get { return _caption; }
            set { _caption = value; }
        }

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }
    }
}
