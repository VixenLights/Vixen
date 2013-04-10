using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Vixen.Sys;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Xml.Serialization;

namespace VixenModules.Preview.VixenPreview.Shapes
{
    [DataContract]
    public abstract class PreviewBaseShape: ICloneable
    {
        //private int top = 0;
        //private int left = 0;
        //private int width = 50;
        //private int height = 50;
        //private double aspect = 1;

        public bool connectStandardStrings = false;
        private StringTypes _stringType = StringTypes.Standard;
        public enum StringTypes
        {
            Standard,
            Pixel
        }

        [XmlIgnore]
        public static Dictionary<ElementNode, List<PreviewPixel>> NodeToPixel = new Dictionary<ElementNode, List<PreviewPixel>>();
        //Hashtable NodeToPixel = new Hashtable();
        //KeyValuePair<ChannelNode, PreviewPixel> NodeToPixel;

        //public Graphics g;

        private bool _selected = false;
        [XmlIgnore]
        public List<PreviewPoint> _selectPoints = null;
        //private List<PreviewPoint> _skewPoints = null;
        public const int SelectPointSize = 6;

        private Color _pixelColor = Color.White;
        private int _pixelSize = 2;

        public List<PreviewPixel> _pixels = new List<PreviewPixel>();
        //public List<PreviewPixel> _skewedPixels = new List<PreviewPixel>();

        [DataMember]
        public List<PreviewBaseShape> _strings;


        public PreviewPoint _selectedPoint;
        //public PreviewPoint _selectedSkewPoint;

        //private int _skewX = 0;
        //private int _skewY = 0;

        public event ResizeEvent DoResize;
        public delegate void ResizeEvent(EventArgs e);

        public delegate void OnPropertiesChangedHandler(object sender, PreviewBaseShape shape);
        public event OnPropertiesChangedHandler OnPropertiesChanged;

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            ResizePixels();
        }

        /// <summary>
        /// Need to override if this is anywhere other than the top left in _pixels
        /// </summary>
        public virtual int Top
        {
            get
            {
                int y = int.MaxValue;
                foreach (PreviewPixel pixel in Pixels)
                {
                    y = Math.Min(y, pixel.Y);
                }
                return y;
            }
            set
            {
                int y = int.MaxValue;
                foreach (PreviewPixel pixel in Pixels)
                {
                    y = Math.Min(y, pixel.Y);
                }
                int delta = value - y;
                foreach (PreviewPixel pixel in Pixels)
                {
                    pixel.Y += delta;
                }

            }
        }

        /// <summary>
        /// Need to override if this is anywhere other than the top left in _pixels
        /// </summary>
        public virtual int Left
        {
            get
            {
                int x = int.MaxValue;
                foreach (PreviewPixel pixel in Pixels)
                {
                    x = Math.Min(x, pixel.X);
                }
                return x;
            }
            set
            {
                int x = int.MaxValue;
                foreach (PreviewPixel pixel in Pixels)
                {
                    x = Math.Min(x, pixel.X);
                }
                int delta = value - x;
                foreach (PreviewPixel pixel in Pixels)
                {
                    pixel.X += delta;
                }
            }
        }

        public abstract void Layout();

        [DataMember,
        CategoryAttribute("Settings"),
        DisplayName("String Type")]
        public StringTypes StringType
        {
            get { return _stringType; }
            set
            {
                _stringType = value;
                if (_strings != null) {
                    foreach (PreviewBaseShape line in _strings) 
                    {
                        line.StringType = _stringType;
                    }
                }
            }
        }

        [DataMember,
        Browsable(false)]
        public virtual List<PreviewPixel> Pixels
        {
            get 
            {
                if (_strings != null && _strings.Count > 0)
                {
                    List<PreviewPixel> outPixels = new List<PreviewPixel>();
                    foreach (PreviewBaseShape line in _strings)
                    {
                        foreach (PreviewPixel pixel in line.Pixels)
                        {
                            outPixels.Add(pixel);
                        }
                    }
                    return outPixels;
                }
                else
                {
                    return _pixels;
                }
            }
            set
            {
                _pixels = value;
                ResetNodeToPixelDictionary();
            }
        }

        [Editor(typeof(PreviewSetElementsUIEditor), typeof(UITypeEditor)),
        CategoryAttribute("Settings"),
        DisplayName("Linked Elements")]
        public virtual List<PreviewBaseShape> Strings 
        {
            get {
                if (_strings != null)
                {
                    // set all the sub-strings to match the connection state for elements
                    foreach (PreviewBaseShape line in _strings)
                        line.connectStandardStrings = this.connectStandardStrings;

                    // Set all the StringTypes in the substrings
                    if (_strings != null)
                    {
                        foreach (PreviewBaseShape line in _strings)
                        {
                            line.StringType = _stringType;
                        }
                    }
                }

                List<PreviewBaseShape> stringsResult = _strings;
                if (stringsResult == null) 
                {
                    stringsResult = new List<PreviewBaseShape>();
                    stringsResult.Add(this);
                }
                return stringsResult; 
            }
            set { }
        }

        //[DataMember]
        //public int SkewX
        //{
        //    get { return _skewX; }
        //    set { _skewX = value; }
        //}

        //[DataMember]
        //public int SkewY
        //{
        //    get { return _skewY; }
        //    set { _skewY = value; }
        //}

        public void ResetNodeToPixelDictionary()
        {
            if (PreviewBaseShape.NodeToPixel == null)
            {
                PreviewBaseShape.NodeToPixel = new Dictionary<ElementNode, List<PreviewPixel>>();
                //NodeToPixel = new Hashtable();
            }
            else
            {
                //PreviewBaseShape.NodeToPixel.Clear();
            }
            foreach (PreviewPixel pixel in _pixels)
            {
                if (pixel.Node != null)
                {
                    List<PreviewPixel> pixels;
                    if (PreviewBaseShape.NodeToPixel.TryGetValue(pixel.Node, out pixels))
                    {
                        if (!pixels.Contains(pixel))
                        {
                            pixels.Add(pixel);
                            //PreviewBaseShape.NodeToPixel.Add(pixel.Node, pixels);
                        }
                    }
                    else
                    {
                        pixels = new List<PreviewPixel>();
                        pixels.Add(pixel);
                        PreviewBaseShape.NodeToPixel.Add(pixel.Node, pixels);
                    }
                }
            }
        }

        [Browsable(false)]
        public Color PixelColor
        {
            get { return _pixelColor; }
            set { 
                _pixelColor = value;
                foreach (PreviewPixel pixel in _pixels)
                {
                    pixel.PixelColor = _pixelColor;
                }
            }
        }

        //public void SetGraphics(Graphics graphics) {
        //    g = graphics;
        //    foreach (PreviewPixel pixel in Pixels) 
        //        pixel.SetGraphics(g);
        //}

        [DataMember,
        CategoryAttribute("Settings"),
        DescriptionAttribute("The size of the light point on the preview."),
        DisplayName("Light Size")]
        public int PixelSize
        {
            get { return _pixelSize; }
            set {
                _pixelSize = value;
                ResizePixels();
            }
        }

        public void SetPixelNode(int pixelNum, ElementNode node) 
        {
            Pixels[pixelNum].Node = node;
            ResetNodeToPixelDictionary();
        }

        public void SetPixelColor(int pixelNum, Color color) 
        {
            Pixels[pixelNum].PixelColor = color;
        }

        public void SetColor(Color pixelColor)
        {
            foreach (PreviewPixel pixel in Pixels)
                pixel.PixelColor = pixelColor;
        }

        [Browsable(false)]
        public bool Selected 
        {
            get {return _selected;}
        }

        public virtual void Select()
        {
            _selected = true;
        }

        public virtual void Deselect()
        {
            _selected = false;
            if (_selectPoints != null) 
                _selectPoints.Clear();
            //if (_skewPoints != null)
            //    _skewPoints.Clear();
        }

        public void SetSelectPoints(List<PreviewPoint> selectPoints, List<PreviewPoint> skewPoints)
        {
            _selectPoints = selectPoints;
            //_skewPoints = skewPoints;

            foreach (PreviewPoint p in _selectPoints)
                if (p != null)
                    p.PointType = PreviewPoint.PointTypes.Size;
        }

        // Add a pxiel at a specific location
        public PreviewPixel AddPixel(int x, int y)
        {
            PreviewPixel pixel = new PreviewPixel(x, y, PixelSize);
            pixel.PixelColor = PixelColor;
            Pixels.Add(pixel);
            ResetNodeToPixelDictionary();
            return pixel;
        }

        public void ResizePixels()
        {
            if (Pixels != null)
            {
                foreach (PreviewPixel pixel in Pixels)
                {
                    pixel.PixelSize = PixelSize;
                }
            }

            if (_strings != null && _strings.Count > 0)
            {
                foreach (PreviewBaseShape shape in _strings)
                {
                    shape.PixelSize = PixelSize;
                }
            } 
            else 
            {
                if (Pixels != null)
                {
                    foreach (PreviewPixel pixel in Pixels)
                    {
                        pixel.PixelSize = PixelSize;
                        pixel.Resize();
                    }
                }
            }
        }

        //public void Draw(Graphics graphics)
        //{
        //    foreach (PreviewPixel pixel in Pixels)
        //    {
        //        pixel.Draw(graphics);
        //    }

        //    if (_selectPoints != null)
        //    {
        //        foreach (PreviewPoint point in _selectPoints)
        //        {
        //            Pen pen = new Pen(Color.White, 1);
        //            graphics.DrawRectangle(pen, new Rectangle(point.X - (SelectPointSize / 2), point.Y - (SelectPointSize / 2), SelectPointSize, SelectPointSize));
        //        }
        //    }
        //}

        public virtual void Draw(FastPixel fp, Color color)
        {
            foreach (PreviewPixel pixel in Pixels)
            {
                pixel.Draw(fp, color);
            }

            DrawSelectPoints(fp);
        }
        
        public virtual void Draw(FastPixel fp)
        {
            foreach (PreviewPixel pixel in Pixels)
            {
                pixel.Draw(fp);
            }

            DrawSelectPoints(fp);
        }

        public virtual void Draw(Graphics graphics, Color color)
        {
            foreach (PreviewPixel pixel in Pixels)
            {
                pixel.Draw(graphics, color);
            }

            //DrawSelectPoints(graphics);
        }

        public virtual void Draw(FastPixel fp, bool editMode, List<ElementNode> highlightedElements)
        {
            foreach (PreviewPixel pixel in Pixels)
            {
                if (highlightedElements.Contains(pixel.Node))
                    pixel.Draw(fp, Color.HotPink);
                else
                    pixel.Draw(fp, Color.White);
            }

            DrawSelectPoints(fp);
        }

        private void DrawSelectPoints(FastPixel fp)
        {
            if (_selectPoints != null)
            {
                foreach (PreviewPoint point in _selectPoints)
                {
                    if (point != null)
                    {
                        if (point.PointType == PreviewPoint.PointTypes.Size)
                        {
                            fp.DrawRectangle(new Rectangle(point.X - (SelectPointSize / 2), point.Y - (SelectPointSize / 2), SelectPointSize, SelectPointSize), Color.White);
                        }
                    }
                }
            }
        }

        public void FireOnPropertiesChanged(Object sender, PreviewBaseShape shape)
        {
            if (OnPropertiesChanged != null)
                OnPropertiesChanged(sender, shape);
        }

        public abstract void MouseMove(int x, int y, int changeX, int changeY);

        public void MouseUp(object sender, MouseEventArgs e)
        {
            FireOnPropertiesChanged(this, this);
        }

        public abstract bool PointInShape(PreviewPoint point);

        public PreviewPoint PointInSelectPoint(PreviewPoint point)
        {
            if (_selectPoints != null)
            {
                foreach (PreviewPoint selectPoint in _selectPoints)
                {
                    if (selectPoint != null)
                    {
                        if (point.X >= selectPoint.X - (SelectPointSize / 2) &&
                            point.Y >= selectPoint.Y - (SelectPointSize / 2) &&
                            point.X <= selectPoint.X + (SelectPointSize / 2) &&
                            point.Y <= selectPoint.Y + (SelectPointSize / 2))
                        {
                            return selectPoint;
                        }
                    }
                }
            }
            return null;
        }

        public PreviewPoint PointInSkewPoint(PreviewPoint point)
        {
            //if (_skewPoints != null)
            //{
            //    foreach (PreviewPoint skewPoint in _skewPoints)
            //    {
            //        if (point.X >= skewPoint.X - (SelectPointSize / 2) &&
            //            point.Y >= skewPoint.Y - (SelectPointSize / 2) &&
            //            point.X <= skewPoint.X + (SelectPointSize / 2) &&
            //            point.Y <= skewPoint.Y + (SelectPointSize / 2))
            //        {
            //            return skewPoint;
            //        }
            //    }
            //}
            return null;
        }

        public abstract void SetSelectPoint(PreviewPoint point = null);

        public abstract void SelectDefaultSelectPoint();

        public void UpdateColors(ElementNode node, Color newColor)
        {
            //if (NodeToPixel.ContainsKey(node))
            //{
            //    PreviewPixel pixel = NodeToPixel[node];
            //    if (pixel != null)
            //        pixel.PixelColor = newColor;
            //}

            PreviewPixel pixel;
            //if (PreviewBaseShape.NodeToPixel.TryGetValue(node, out pixel))
            //    pixel.PixelColor = newColor;

            //PreviewPixel pixel = NodeToPixel[node] as PreviewPixel;
            //if (pixel != null)
            //    pixel.PixelColor = newColor;
            //_pixels[0].PixelColor = newColor;
        }

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
