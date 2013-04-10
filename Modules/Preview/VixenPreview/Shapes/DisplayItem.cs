using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Data.Value;
using System.Runtime.Serialization;
using System.Drawing;
using Vixen.Data.Flow;
using Vixen.Module;
using Vixen.Module.OutputFilter;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace VixenModules.Preview.VixenPreview.Shapes
{
    [DataContract]
    [KnownType(typeof(PreviewLine))]
    [KnownType(typeof(PreviewEllipse))]
    [KnownType(typeof(PreviewArch))]
    [KnownType(typeof(PreviewRectangle))]
    [KnownType(typeof(PreviewSingle))]
    [KnownType(typeof(PreviewEllipse))]
    [KnownType(typeof(PreviewTriangle))]
    [KnownType(typeof(PreviewMegaTree))]
    public class DisplayItem
    {
        private PreviewBaseShape _shape;

        public DisplayItem()
        {
            _shape = new PreviewLine(new PreviewPoint(1, 1), new PreviewPoint(10, 10), 1);
        }

        [DataMember]
        public PreviewBaseShape Shape
        {
            get { return _shape; }
            set { _shape = value; }
        }

        public void Draw(FastPixel fp, Color color)
        {
            _shape.Draw(fp, color);
        }

        public void Draw(FastPixel fp)
        {
            _shape.Draw(fp);
        }

        //public void ResetColors(bool isRunning)
        //{
        //    Shape.ResetNodeToPixelDictionary();
        //}

        public void Draw(FastPixel fp, bool editMode, List<ElementNode> highlightedElements)
        {
            _shape.Draw(fp, editMode, highlightedElements);
        }
    }
}
