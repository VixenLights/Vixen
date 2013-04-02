using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VixenModules.Preview.VixenPreview.Shapes;
using Vixen.Sys;
using System.Runtime.Serialization;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Module;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace VixenModules.Preview.VixenPreview
{
    #region "EventArg Classes"
    public class DisplayItemEventArgs : EventArgs
    {
        public DisplayItem DisplayItem { get; internal set; }
        public DisplayItemEventArgs(DisplayItem displayItem)
        {
            DisplayItem = displayItem;
        }
    }
    #endregion

    public partial class VixenPreviewControl : UserControl
    {
        #region "Variables"
        private BackgroundWorker renderBackgroundWorker = new BackgroundWorker();
        private BufferedGraphicsContext context;
        private BufferedGraphics bufferedGraphics;
        public static double averageUpdateTime = 0;
        public static double updateCount = 0;
        public static double totalUpdateTime = 0;
        public static double lastUpdateTime = 0;
        public double lastRenderUpdateTime = 0;

        private Tools _currentTool = Tools.Select;
        public enum Tools
        {
            Select,
            String,
            Arch,
            Rectangle,
            Ellipse,
            Single,
            MegaTree
        }

        private Random random = new Random();

        private List<DisplayItem> selectedDisplayItems = new List<DisplayItem>();
        private Point dragStart;
        private Point dragCurrent;
        private int changeX;
        private int changeY;
        private DisplayItem _selectedDisplayItem = null;
        private bool _editMode = false;

        private Image _background;
        private Bitmap _alphaBackground;
        //private int _backgroundAlpha = 255;

        private VixenPreviewData _data;

        Stopwatch renderTimer = new Stopwatch();
        private System.Object renderLock = new System.Object();
        #endregion

        #region "Events"
        public delegate void SelectDisplayItemEventHandler(object sender, DisplayItem displayItem);
        public event SelectDisplayItemEventHandler OnSelectDisplayItem;

        public delegate void DeSelectDisplayItemEventHandler(object sender, DisplayItem displayItem);
        public event DeSelectDisplayItemEventHandler OnDeSelectDisplayItem;
        //public event EventHandler SelectDisplayItemEventHandler;
        //public void OnSelectDisplayItem(DisplayItem displayItem)
        //{
        //    EventHandler handler = SelectDisplayItemEventHandler;
        //    if (handler != null) handler(this, new DisplayItemEventArgs(displayItem));
        //}

        //public event EventHandler DeSelectDisplayItemEventHandler;
        //public void OnDeSelectDisplayItem(DisplayItem displayItem)
        //{
        //    EventHandler handler = DeSelectDisplayItemEventHandler;
        //    if (handler != null) handler(this, new DisplayItemEventArgs(displayItem));
        //}
        #endregion

        public int BackgroundAlpha
        {
            get { return Data.BackgroundAlpha; }
            set
            {
                if (Data != null)
                {
                    Data.BackgroundAlpha = value;
                    SetupBackgroundAlphaImage();
                }
            }
        }

        public bool EditMode
        {
            set 
            { 
                _editMode = value;
                _selectedDisplayItem = null;
                if (DisplayItems != null)
                {
                    foreach (DisplayItem item in DisplayItems)
                    {
                        item.Shape.Deselect();
                    }
                }
            }
            get { return _editMode; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public VixenPreviewData Data
        {
            get
            {
                if (DesignMode)
                    _data = new VixenPreviewData();
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        private List<DisplayItem> DisplayItems
        {
            get {
                if (Data != null)
                {
                    return Data.DisplayItems;
                }
                else
                {
                    return null;
                }
            }
        }

        public int PixelCount
        {
            get
            {
                int count = 0;
                foreach (DisplayItem displayItem in DisplayItems)
                {
                    count += displayItem.Shape.Pixels.Count;
                }
                return count;
            }
        }

        public VixenPreviewControl() : base()
        {
            InitializeComponent();

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);

            context = BufferedGraphicsManager.Current;
            AllocateGraphicsBuffer();

            renderBackgroundWorker.DoWork += new DoWorkEventHandler(DoRenderWork);
            renderBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RenderWorkCompleted);
        }

        private void VixenPreviewControl_Load(object sender, EventArgs e)
        {
            InitializeGraphics();
            //StartRefresh();
        }

        public void LoadBackground(string fileName)
        {
            lock (renderLock)
            {
                if (System.IO.File.Exists(fileName))
                {
                    try
                    {
                        _background = Image.FromFile(fileName);
                        Console.WriteLine(fileName);
                    }
                    catch (Exception ex)
                    {
                        _background = new Bitmap(640, 480);
                        MessageBox.Show("There was an error loading the background image: " + ex.Message, "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    _background = new Bitmap(640, 480);
                }
            }

            SetupBackgroundAlphaImage();
        }

        private void SetupBackgroundAlphaImage()
        {
            if (_background != null)
            {
                lock (renderLock)
                {
                    _alphaBackground = new Bitmap(_background.Width, _background.Height);
                    using (Graphics gfx = Graphics.FromImage(_alphaBackground))

                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(255 - BackgroundAlpha, 0, 0, 0)))
                    {
                        gfx.DrawImage(_background, 0, 0, _background.Width, _background.Height);
                        gfx.FillRectangle(brush, 0, 0, _alphaBackground.Width, _alphaBackground.Height);
                    }
                }
            }
        }

        private void InitializeGraphics()
        {
            context = BufferedGraphicsManager.Current;
            AllocateGraphicsBuffer();
        }

        private void AllocateGraphicsBuffer()
        {
            lock (renderLock)
            {

                if (context != null)
                {
                    context.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);

                    if (bufferedGraphics != null)
                    {
                        lock (bufferedGraphics)
                        {
                            bufferedGraphics.Dispose();
                            bufferedGraphics = null;
                            bufferedGraphics = context.Allocate(this.CreateGraphics(),
                                new Rectangle(0, 0, this.Width + 1, this.Height + 1));
                        }
                    }
                    else
                    {
                        bufferedGraphics = context.Allocate(this.CreateGraphics(),
                        new Rectangle(0, 0, this.Width + 1, this.Height + 1));
                    }
                }
            }
        }

        public void AddDisplayItem(DisplayItem displayItem)
        {
            DisplayItems.Add(displayItem);
        }

        private bool _mouseCaptured = false;
        private void VixenPreviewControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (_editMode)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    PreviewPoint point = new PreviewPoint(e.X, e.Y);

                    if (_currentTool == Tools.Select)
                    {
                        if (_selectedDisplayItem != null)
                        {
                            // Lets see if we've got a drag point.
                            PreviewPoint selectedPoint = _selectedDisplayItem.Shape.PointInSelectPoint(point);
                            PreviewPoint skewPoint = _selectedDisplayItem.Shape.PointInSkewPoint(point);
                            if (selectedPoint != null)
                            {
                                dragStart.X = e.X;
                                dragStart.Y = e.Y;
                                _selectedDisplayItem.Shape.SetSelectPoint(selectedPoint);
                                Capture = true;
                                _mouseCaptured = true;
                            }
                            else if (skewPoint != null)
                            {
                                dragStart.X = e.X;
                                dragStart.Y = e.Y;
                                _selectedDisplayItem.Shape.SetSelectPoint(skewPoint);
                                Capture = true;
                                _mouseCaptured = true;
                            }
                            // If we're not resizing, see if we're moving
                            else if (_selectedDisplayItem.Shape.PointInShape(point))
                            {
                                dragStart.X = e.X;
                                dragStart.Y = e.Y;
                                _selectedDisplayItem.Shape.SetSelectPoint(null);
                                Capture = true;
                                _mouseCaptured = true;
                            }
                            // If we get this far, we're off the shape, deselect it!
                            else
                            {
                                OnDeSelectDisplayItem(this, _selectedDisplayItem);
                                _selectedDisplayItem.Shape.Deselect();
                                _selectedDisplayItem = null;
                            }
                        }

                        if (!_mouseCaptured)
                        {
                            _selectedDisplayItem = DisplayItemAtPoint(point);
                            if (_selectedDisplayItem != null)
                            {
                                _selectedDisplayItem.Shape.Select();
                                OnSelectDisplayItem(this, _selectedDisplayItem);
                            }
                        }
                    }
                    // If we're not Selecting items, we're drawing them
                    else
                    {
                        DisplayItem newDisplayItem = null;
                        if (_currentTool == Tools.String)
                        {
                            newDisplayItem = new DisplayItem();
                            newDisplayItem.Shape = new PreviewLine(new PreviewPoint(e.X, e.Y), new PreviewPoint(e.X, e.Y), 50);
                        }
                        else if (_currentTool == Tools.Arch)
                        {
                            newDisplayItem = new DisplayItem();
                            newDisplayItem.Shape = new PreviewArch(new PreviewPoint(e.X, e.Y));
                        }
                        else if (_currentTool == Tools.Rectangle)
                        {
                            newDisplayItem = new DisplayItem();
                            newDisplayItem.Shape = new PreviewRectangle(new PreviewPoint(e.X, e.Y));
                        }
                        else if (_currentTool == Tools.Single)
                        {
                            newDisplayItem = new DisplayItem();
                            newDisplayItem.Shape = new PreviewSingle(new PreviewPoint(e.X, e.Y));
                        }
                        else if (_currentTool == Tools.Ellipse)
                        {
                            newDisplayItem = new DisplayItem();
                            newDisplayItem.Shape = new PreviewEllipse(new PreviewPoint(e.X, e.Y), 50);
                        }
                        else if (_currentTool == Tools.MegaTree)
                        {
                            newDisplayItem = new DisplayItem();
                            newDisplayItem.Shape = new PreviewMegaTree(new PreviewPoint(e.X, e.Y));
                        }

                        // Now add the newely created display item to the screen.
                        if (newDisplayItem != null)
                        {
                            AddDisplayItem(newDisplayItem);
                            _selectedDisplayItem = newDisplayItem;
                            _selectedDisplayItem.Shape.PixelSize = 3;
                            _selectedDisplayItem.Shape.Select();
                            _selectedDisplayItem.Shape.SelectDefaultSelectPoint();
                            dragStart.X = e.X;
                            dragStart.Y = e.Y;
                            Capture = true;
                            _mouseCaptured = true;
                        }
                    }
                }
            }
        }

        private void VixenPreviewControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (_editMode)
            {
                PreviewPoint point = new PreviewPoint(e.X, e.Y);

                if (_mouseCaptured)
                {
                    dragCurrent.X = e.X;
                    dragCurrent.Y = e.Y;
                    changeX = e.X - dragStart.X;
                    changeY = e.Y - dragStart.Y;

                    _selectedDisplayItem.Shape.MouseMove(dragCurrent.X, dragCurrent.Y, changeX, changeY);
                }
                else
                {
                    if (_selectedDisplayItem != null)
                    {
                        PreviewPoint selectPoint = _selectedDisplayItem.Shape.PointInSelectPoint(point);
                        PreviewPoint skewPoint = _selectedDisplayItem.Shape.PointInSkewPoint(point);
                        if (selectPoint != null)
                        {
                            Cursor.Current = Cursors.Cross;
                        }
                        else if (skewPoint != null)
                        {
                            if (skewPoint.PointType == PreviewPoint.PointTypes.SkewNS)
                            {
                                Cursor.Current = Cursors.SizeNS;
                            }
                            else if (skewPoint.PointType == PreviewPoint.PointTypes.SkewWE)
                            {
                                Cursor.Current = Cursors.SizeWE;
                            }

                        }
                        else if (_selectedDisplayItem.Shape.PointInShape(point))
                        {
                            Cursor.Current = Cursors.SizeAll;
                        }
                        else
                        {
                            Cursor.Current = Cursors.Default;
                        }
                    }
                }
            }
        }

        private void VixenPreviewControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (_mouseCaptured)
            {
                Capture = false;
                _mouseCaptured = false;
                if (_currentTool != Tools.Select)
                {
                    // If control is pressed, deselect the shape and immediately allow drawing another shape
                    if ((Control.ModifierKeys & Keys.Control) != 0)
                    {
                        OnDeSelectDisplayItem(this, _selectedDisplayItem);
                        _selectedDisplayItem.Shape.MouseUp(sender, e);
                        _selectedDisplayItem.Shape.Deselect();
                        _selectedDisplayItem = null;
                    }
                    else
                    {
                        _currentTool = Tools.Select;
                    }
                }

                if (_selectedDisplayItem != null)
                {
                    _selectedDisplayItem.Shape.MouseUp(sender, e);
                    OnSelectDisplayItem(this, _selectedDisplayItem);
                }
            }
            //else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            //{
            //    PreviewPoint point = new PreviewPoint(e.X, e.Y);

            //    DisplayItem displayItem = DisplayItemAtPoint(point);
            //    if (displayItem != null)
            //    {
            //        displayItem.Shape.PropertyDialog();
            //    }
            //}
            
        }

        public DisplayItem DisplayItemAtPoint(PreviewPoint point) 
        {
            foreach (DisplayItem displayItem in DisplayItems)
            {
                if (displayItem.Shape.PointInShape(point))
                {
                    return displayItem;
                }
            }
            return null;
        }

        public Tools CurrentTool
        {
            get { return _currentTool; }
            set
            {
                _currentTool = value;
                if (_selectedDisplayItem != null)
                {
                    OnDeSelectDisplayItem(this, _selectedDisplayItem);
                    _selectedDisplayItem.Shape.Deselect();
                    _selectedDisplayItem = null;
                }
            }
        }

        private void VixenPreviewControl_Resize(object sender, EventArgs e)
        {
            AllocateGraphicsBuffer();
        }

        private void VixenPreviewControl_SizeChanged(object sender, EventArgs e)
        {
        }

        private void VixenPreviewControl_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void VixenPreviewControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (_selectedDisplayItem != null)
                {
                    DisplayItems.Remove(_selectedDisplayItem);
                }
            }
        }

        public void DrawDisplayItemsInBackground()
        {
            //Thread drawThread = new Thread(() => DrawDisplayItems(bufferedGraphics.Graphics));
            //drawThread.Start();
        }

        public void UpdateColors(ElementNode node, Color newColor)
        {
            List<PreviewPixel> pixels;
            if (PreviewBaseShape.NodeToPixel.TryGetValue(node, out pixels))
            {
                foreach (PreviewPixel pixel in pixels)
                {
                    pixel.PixelColor = newColor;
                }
            }
        }

        public void ResetColors()
        {
            foreach (List<PreviewPixel> pixels in PreviewBaseShape.NodeToPixel.Values)
            {
                foreach (PreviewPixel pixel in pixels)
                {
                    if (_editMode)
                    {
                        pixel.PixelColor = Color.White;
                    }
                    else
                    {
                        pixel.PixelColor = Color.Transparent;
                    }
                }
            }
        }

        public void RenderInBackground()
        {
            if (!DesignMode)
            {
                if (renderBackgroundWorker.IsBusy != true)
                {
                    // Start the asynchronous operation.
                    renderTimer.Reset();
                    renderTimer.Start();

                    renderBackgroundWorker.RunWorkerAsync();
                }
            }
        }

        private void DoRenderWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            if (_background != null)
            {
                FastPixel fp = new FastPixel(_background.Width, _background.Height);
                fp.Lock();
                foreach (DisplayItem displayItem in DisplayItems)
                {
                    if (_editMode)
                        displayItem.Draw(fp, Color.White);
                    else
                        displayItem.Draw(fp);
                }
                fp.Unlock(true);

                lock (renderLock)
                {
                    // First, draw our background image opaque
                    bufferedGraphics.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                    //bufferedGraphics.Graphics.DrawImage(_background, 0, 0, _background.Width, _background.Height);
                    bufferedGraphics.Graphics.DrawImage(_alphaBackground, 0, 0, fp.Width, fp.Height);

                    bufferedGraphics.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                    // Now, draw our "pixel" image using alpha blending
                    bufferedGraphics.Graphics.DrawImage(fp.Bitmap, 0, 0, fp.Width, fp.Height);
                }
            }
        }

        private void RenderWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
            }

            else if (!(e.Error == null))
            {
            }

            else
            {
                if (bufferedGraphics != null && !this.Disposing)
                    bufferedGraphics.Render(Graphics.FromHwnd(this.Handle));
            }

            renderTimer.Stop();
            lastRenderUpdateTime = renderTimer.ElapsedMilliseconds;
        }

        public void RenderInForeground()
        {
            renderTimer.Reset();
            renderTimer.Start();
            if (_background != null)
            {
                FastPixel fp = new FastPixel(_background.Width, _background.Height);
                fp.Lock();
                foreach (DisplayItem displayItem in DisplayItems)
                {
                    if (_editMode)
                        displayItem.Draw(fp, Color.White);
                    else
                        displayItem.Draw(fp);
                }
                fp.Unlock(true);

                //lock (renderLock)
                //{
                // First, draw our background image opaque
                bufferedGraphics.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                //bufferedGraphics.Graphics.DrawImage(_background, 0, 0, _background.Width, _background.Height);
                bufferedGraphics.Graphics.DrawImage(_alphaBackground, 0, 0, fp.Width, fp.Height);

                bufferedGraphics.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                // Now, draw our "pixel" image using alpha blending
                bufferedGraphics.Graphics.DrawImage(fp.Bitmap, 0, 0, fp.Width, fp.Height);
                //}
            }

            bufferedGraphics.Render(Graphics.FromHwnd(this.Handle));
            renderTimer.Stop();
            lastRenderUpdateTime = renderTimer.ElapsedMilliseconds;
        }

    }
}
