using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;
using Vixen.Execution.Context;
using Vixen.Module.Preview;
using Vixen.Data.Value;
using Vixen.Sys;
using VixenModules.Preview.VixenPreview.Shapes;


namespace VixenModules.Preview.VixenPreview.Shapes
{
    [DataContract]
    public class PreviewPixel
    {
        private Color color = Color.White;
        private Brush brush;
        private int x = 0;
        private int y = 0;
        private int size = 3;
        private Rectangle drawArea;
        private ElementNode _node = null;
        private Guid _nodeId;
        private int _maxAlpha = 255;

        //static Hashtable brushes = new Hashtable();
        //static Dictionary<Int32, Brush> brushes = new Dictionary<Int32, Brush>();

        //public static Hashtable IntentNodeToColor = new Hashtable();
        public static Dictionary<ElementNode, Color> IntentNodeToColor = new Dictionary<ElementNode, Color>();
        public static Dictionary<Guid, IIntentStates> intentStates = new Dictionary<Guid, IIntentStates>();

        public PreviewPixel(int xPosition, int yPositoin, int pixelSize)
        {
            x = xPosition;
            y = yPositoin;
            size = pixelSize;
            brush = new SolidBrush(Color.White);
            Resize();
        }

        public PreviewPixel Clone()
        {
            PreviewPixel p = new PreviewPixel(x, y, size);
            p.color = color;
            p.drawArea = new Rectangle(drawArea.X, drawArea.Y, drawArea.Width, drawArea.Height);
            p.Node = Node;
            p.MaxAlpha = _maxAlpha;

            return p;
        }

        [DataMember]
        public int MaxAlpha
        {
            get 
            {
                if (_maxAlpha == 0)
                    _maxAlpha = 255;
                return _maxAlpha; 
            }
            set { _maxAlpha = value; }
        }

        [DataMember]
        public Guid NodeId
        {
            get {
                return _nodeId; 
            }
            set {
                _nodeId = value;
            }
        }

        public ElementNode Node
        {
            get 
            {
                if (_node == null)
                {
                    _node = VixenSystem.Nodes.GetElementNode(NodeId);
                }
                return _node; 
            }
            set
            {
                if (value == null)
                    NodeId = Guid.Empty;
                else
                    NodeId = value.Id;
                _node = value;
            }
        }

        //public void SetGraphics(Graphics Graphics)
        //{
        //    g = Graphics;
        //}

        public void Resize()
        {
            drawArea = new Rectangle(x, y, size, size);
        }

        [DataMember]
        public int X
        {
            get { return x; }
            set { 
                x = value;
                Resize();
            }
        }

        [DataMember]
        public int Y
        {
            get { return y; }
            set { 
                y = value;
                Resize();
            }
        }

        [DataMember]
        public int PixelSize
        {
            get { return size; }
            set { 
                size = value;
                Resize();

            }
        }

        [DataMember]
        public Color PixelColor
        {
            get { return color; }
            set
            {
                color = value;
            }
        }

        public void Draw(Graphics graphics, Color c)
        {            
            graphics.FillEllipse(new SolidBrush(c), drawArea);
        }

        public void Draw(FastPixel fp)
        {
            if (Node != null)
            {
                //IIntentStates nodeIntentStates;
                //if (intentStates.TryGetValue(Node.Id, out nodeIntentStates))
                //{
                //    if (nodeIntentStates != null)
                //    {
                //        foreach (IIntentState<LightingValue> intentState in nodeIntentStates)
                //        {
                //            Draw(fp, intentState.GetValue().GetAlphaChannelIntensityAffectedColor());
                //        }
                //    }
                //}
                Color color;
                if (PreviewPixel.IntentNodeToColor.TryGetValue(Node, out color))
                {
                    //if (nodeIntentStates != null)
                    //{
                        //foreach (IIntentState<LightingValue> intentState in nodeIntentStates)
                        //{
                            Draw(fp, color);
                        //}
                    //}
                }
            }
        }

        public void Draw(FastPixel fp, Color newColor)
        {
            if (newColor.A > 0)
            {
                if (PixelSize == 1)
                {
                    fp.SetPixel(drawArea.Left, drawArea.Top, newColor);
                }
                else if (PixelSize == 2)
                {
                    // Row 1
                    fp.SetPixel(drawArea.Left, drawArea.Top, newColor);
                    //fp.SetPixel(drawArea.Left+1, drawArea.Top, newColor);
                    // Row 2
                    fp.SetPixel(drawArea.Left, drawArea.Top+1, newColor);
                    //fp.SetPixel(drawArea.Left+1, drawArea.Top+1, newColor);
                }
                else if (PixelSize == 3)
                {
                    // Row 1
                    //fp.SetPixel(drawArea.Left - 1, drawArea.Top - 1, newColor);
                    fp.SetPixel(drawArea.Left, drawArea.Top - 1, newColor);
                    //fp.SetPixel(drawArea.Left + 1, drawArea.Top - 1, newColor);
                    // Row 2
                    fp.SetPixel(drawArea.Left - 1, drawArea.Top, newColor);
                    fp.SetPixel(drawArea.Left, drawArea.Top, newColor);
                    fp.SetPixel(drawArea.Left + 1, drawArea.Top, newColor);
                    // Row 1
                    //fp.SetPixel(drawArea.Left - 1, drawArea.Top + 1, newColor);
                    fp.SetPixel(drawArea.Left, drawArea.Top + 1, newColor);
                    //fp.SetPixel(drawArea.Left + 1, drawArea.Top + 1, newColor);
                }
                else
                {
                    if (MaxAlpha != 255)
                    {
                        double newAlpha = ((double)newColor.A / 255) * (double)MaxAlpha;
                        Color outColor = Color.FromArgb((int)newAlpha, newColor.R, newColor.G, newColor.B);
                        fp.DrawCircle(drawArea, outColor);
                    }
                    else
                    {
                        fp.DrawCircle(drawArea, newColor);
                    }
                }
            }
        }
    }
}
