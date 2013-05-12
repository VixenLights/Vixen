using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VixenModules.Preview.VixenPreview.Shapes
{
    class PreviewSetElementString
    {
        string _stringName = "";
        List<PreviewPixel> _pixels = new List<PreviewPixel>();

        public string StringName
        {
            get { return _stringName; }
            set { _stringName = value; }
        }

        public List<PreviewPixel> Pixels
        {
            get { return _pixels; }
        }
    }
}
