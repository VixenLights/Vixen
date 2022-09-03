using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Serialization;
using Vixen.Sys;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	[DataContract]
	public abstract class PreviewBaseShape : ICloneable, IDisposable
	{
		public string _name;
		protected  bool _selected = false;
		
		[XmlIgnore] public List<PreviewPoint> _selectPoints = null;
		
		public PreviewPoint _selectedPoint;
		public delegate void OnPropertiesChangedHandler(object sender, PreviewBaseShape shape);
		public event OnPropertiesChangedHandler OnPropertiesChanged;
		
		internal virtual void Reconfigure(ElementNode node)
		{
		}

		[Browsable(false)]
		public virtual int BackgroundAlpha { get; set; }

		public const int SelectPointSize = 6;

		[DataMember(EmitDefaultValue = false),
		 Category("Settings"),
		 Description("The name of this string. Used in templates to distinguish various strings."),
		 DisplayName("Name")]
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = string.IsNullOrEmpty(value) ? VixenPreviewSetup3.DrawShape : value;
				FireOnPropertiesChanged(this, this);
			}
		}
		
		/// <summary>
		/// Display name for the type of shape.
		/// </summary>
		[Browsable(false)]
		public virtual string TypeName => @"Shape";

		/// <summary>
		/// Top most pixel location
		/// </summary>
		[Browsable(false)]
        public abstract int Top { get; set; }

		/// <summary>
		/// Bottom most pixel location
		/// </summary>
        [Browsable(false)]
        public abstract int Bottom { get; }

		/// <summary>
		/// Left most pixel location
		/// </summary>
        [Browsable(false)]
        public abstract int Left { get; set; }

		/// <summary>
		/// Right most pixel location
		/// </summary>
        [Browsable(false)]
        public abstract int Right { get; }

		public abstract void Match(PreviewBaseShape matchShape);

		public abstract void Layout();
        
		[Browsable(false)]
        public PreviewBaseShape Parent { get; set; }

		private double _zoomLevel = 1;

		[Browsable(false)]
		public virtual double ZoomLevel
		{
			get
			{
				if (_zoomLevel <= 0)
					_zoomLevel = 1;
				return _zoomLevel;
			}
			set
			{
				_zoomLevel = value;				
				Layout();
			}
		}
		[Browsable(false)]
		public bool Selected
		{
			get { return _selected; }
			set { _selected = value; }
		}

		public virtual void Select(bool selectDragPoints)
		{
			Selected = true;
			if (selectDragPoints)
				SelectDragPoints();
		}

		public abstract void SelectDragPoints();

		public virtual void Deselect()
		{
			_selected = false;
			if (_selectPoints != null)
				_selectPoints.Clear();
		}

		public void SetSelectPoints(List<PreviewPoint> selectPoints, List<PreviewPoint> skewPoints)
		{
			_selectPoints = selectPoints;
			foreach (PreviewPoint p in _selectPoints)
				if (p != null)
					p.PointType = PreviewPoint.PointTypes.Size;
		}
		protected static Rect GetCombinedBounds(IEnumerable<Rect> recs)
		{
			if(!recs.Any()) return new Rect(0,0,0,0);
			double xMin = recs.Min(s => s.X);
			double yMin = recs.Min(s => s.Y);
			double xMax = recs.Max(s => s.X + s.Width);
			double yMax = recs.Max(s => s.Y + s.Height);
			var rect = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
			return rect;
		}

		public virtual void Draw(Bitmap b, bool editMode, HashSet<Guid> highlightedElements)
		{
			throw new NotImplementedException();
		}
		public virtual void Draw(FastPixel.FastPixel fp, bool editMode, HashSet<Guid> highlightedElements, bool selected,
		                         bool forceDraw, double zoomLevel)
		{
			

			DrawSelectPoints(fp);
		}
		public virtual void DrawSelectPoints(FastPixel.FastPixel fp)
		{
			if (_selectPoints != null) {
				foreach (PreviewPoint point in _selectPoints) {
					if (point != null) {
						if (point.PointType == PreviewPoint.PointTypes.Size) {
                            int x = Convert.ToInt32((point.X) * ZoomLevel) - (SelectPointSize / 2);
                            int y = Convert.ToInt32(point.Y * ZoomLevel) - (SelectPointSize / 2);
                            fp.DrawRectangle(
								new Rectangle(x, y, SelectPointSize, SelectPointSize),
								Color.White);
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
		public virtual void MouseUp(object sender, MouseEventArgs e)
		{
			FireOnPropertiesChanged(this, this);
		}
		public abstract bool PointInShape(PreviewPoint point);

		public abstract bool ShapeInRect(Rectangle rect);
		
        public virtual bool ShapeAllInRect(Rectangle rect)
        {
            PreviewPoint p1 = PointToZoomPoint(new PreviewPoint(rect.X, rect.Y));
            PreviewPoint p2 = PointToZoomPoint(new PreviewPoint(rect.X + rect.Width, rect.Y + rect.Height));
            int X1 = Math.Min(p1.X, p2.X);
            int X2 = Math.Max(p1.X, p2.X);
            int Y1 = Math.Min(p1.Y, p2.Y);
            int Y2 = Math.Max(p1.Y, p2.Y);
            //Console.WriteLine(Top + ":" + Y1 + "  " + Bottom + ":" + Y2 + "  " + Left + ":" + X1 + "  " + Right + ":" + X2);
            return (Top >= Y1 && Bottom <= Y2 && Left >= X1 && Right <= X2);
        }

		public virtual PreviewPoint PointInSelectPoint(PreviewPoint point)
		{
			if (_selectPoints != null) {
				foreach (PreviewPoint selectPoint in _selectPoints) {
					if (selectPoint != null) {
						int selectPointX = Convert.ToInt32(selectPoint.X * ZoomLevel);
						int selectPointY = Convert.ToInt32(selectPoint.Y * ZoomLevel);
						if (point.X >= selectPointX - (SelectPointSize / 2) &&
						    point.Y >= selectPointY - (SelectPointSize/2) &&
						    point.X <= selectPointX + (SelectPointSize/2) &&
						    point.Y <= selectPointY + (SelectPointSize/2)) {
							return selectPoint;
						}
					}
				}
			}
			return null;
		}
		public PreviewPoint PointToZoomPoint(PreviewPoint p)
		{
			PreviewPoint newPoint = new PreviewPoint(p.X, p.Y);
			PointToZoomPointRef(newPoint);
			return newPoint;
		}

		public void PointToZoomPointRef(PreviewPoint p)
		{
			int xDif = p.X - (int)(p.X / ZoomLevel);
			int yDif = p.Y - (int)(p.Y / ZoomLevel);
			p.X = p.X - xDif;
			p.Y = p.Y - yDif;
		}
		public PreviewPoint PointToZoomPointAdd(PreviewPoint p)
		{
			return new PreviewPoint(Convert.ToInt32(p.X * ZoomLevel), Convert.ToInt32(p.Y * ZoomLevel));
		}
		public abstract void SetSelectPoint(PreviewPoint point = null);
		public abstract void SelectDefaultSelectPoint();
		public virtual object Clone()
		{
			var shape = (PreviewBaseShape)this.MemberwiseClone();

			return shape;
		}
		public abstract void MoveTo(int x, int y);
		public virtual void Nudge(int x, int y)
		{
			MoveTo(Left + x, Top + y);
		}
		public abstract void Resize(double aspect);
		public abstract void ResizeFromOriginal(double aspect);

		/// <summary>
		/// This will be true if the shape is being created. Only used in multi-point placement objects
		/// </summary>
		[Browsable(false)]
		public virtual bool Creating { get; set; }

		public DisplayItemBaseControl GetSetupControl()
		{
			Shapes.DisplayItemBaseControl setupControl = null;

			if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewSingle") {
				setupControl = new Shapes.PreviewShapeBaseSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewLine") {
				setupControl = new Shapes.PreviewShapeBaseSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewRectangle") {
				setupControl = new Shapes.PreviewShapeBaseSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewEllipse") {
				setupControl = new Shapes.PreviewShapeBaseSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewArch") {
				setupControl = new Shapes.PreviewArchSetupControl((PreviewLightBaseShape)this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewMegaTree") {
				setupControl = new Shapes.PreviewShapeBaseSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewTriangle") {
				setupControl = new Shapes.PreviewShapeBaseSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewFlood") {
				setupControl = new Shapes.PreviewShapeBaseSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewCane") {
				setupControl = new Shapes.PreviewCaneSetupControl((PreviewLightBaseShape)this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewStar") {
				setupControl = new Shapes.PreviewStarSetupControl((PreviewLightBaseShape)this);
			}
            else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewStarBurst")
            {
                setupControl = new Shapes.PreviewShapeBaseSetupControl(this);
            }
            else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewNet")
            {
				setupControl = new Shapes.PreviewNetSetupControl((PreviewLightBaseShape)this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewCustom") {
				setupControl = new Shapes.PreviewCustomSetupControl((PreviewLightBaseShape)this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewPixelGrid") {
				setupControl = new PreviewShapeBaseSetupControl(this);
			}
            else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewIcicle")
            {
                setupControl = new Shapes.PreviewIcicleSetupControl((PreviewLightBaseShape)this);
            }
            else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewPolyLine")
            {
                setupControl = new Shapes.PreviewShapeBaseSetupControl((PreviewLightBaseShape)this);
            }
            else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewMultiString")
            {
                setupControl = new Shapes.PreviewShapeBaseSetupControl(this);
            }
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewCustomProp")
			{
				setupControl = new PreviewShapeBaseSetupControl(this);
			}
			else if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewMovingHead")
			{
				setupControl = new PreviewShapeBaseSetupControl(this);
			}

			return setupControl;
		}
		/// <summary>
		/// Gives derived shapes the opportunity to configure and adjust the shape after an Add New using the mouse.
		/// </summary>
		public virtual void EndAddNew()
		{			
		}
		/// <summary>
		/// Gives derived shapes the opportunity to adjust the shape after a point has been moved. 
		/// </summary>
		public virtual void OnMovePoint()
		{
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing) 
			{
				if (_selectPoints != null)
					_selectPoints.Clear();
				_selectPoints = null;				
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}				
	}
}
