using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows;
using System.Xml.Serialization;
using Vixen.Sys;
using VixenModules.Preview.VixenPreview.Converter;
using Point = System.Drawing.Point;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	[DataContract]
	[TypeConverter(typeof(PropertySorter))]
	public abstract class PreviewBaseShape : ICloneable, IDisposable
	{
		public string _name;
		protected bool _selected = false;
		protected PreviewPoint rotateHandle;
		private bool _showRotation = true;
		private int _rotationAngle = 0;

		[Browsable(false)]
		public PreviewPoint RotationAxis { get; set; }

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

		[DataMember(EmitDefaultValue = false),
		Category("Location"),
		Description("Uses a common location for all lights based on the center of the prop."),
		DisplayName("Common Location")]
		public bool UseCommonLocation { get; set; }

		[DataMember(EmitDefaultValue = false),
		Category("Control"),
		Description("Locks out access to the Prop. Use Shift key in combination with the mouse to override."), //ToDo: add text
		DisplayName("Locked")]
		public bool Locked { get; set; }

		/// <summary>
		/// Display name for the type of shape.
		/// </summary>
		[Browsable(false)]
		public virtual string TypeName => @"Shape";

		/// <summary>
		/// Initialize non-saved data members as the serializer unspools the Prop 
		/// </summary>
		/// <remarks>The XML Serializer does not call the standard object constructors, but does call all methods with the attribute
		/// [OnDeserialized]. Note, that if there is more than one method with the attribute [OnDeserialized], the order they are 
		/// called in is not deterministic. You can use the attribute [OnDeserializing] to execute work done before deserialization 
		/// instead of after.</remarks>
		/// <param name="context"></param>
		[OnDeserialized]
		private void InitializeNonDataMembers(StreamingContext context)
		{
			RotationAxis = new PreviewPoint(Center);
			RotationAxis.PointType = PreviewPoint.PointTypes.RotationAxis;

			rotateHandle = new PreviewPoint(Center.X, this.Top - 10);
			rotateHandle.PointType = PreviewPoint.PointTypes.RotateHandle;

			ShowRotation = true;
		}

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

		/// <summary>
		/// Center most pixel location
		/// </summary>
		[Browsable(false)]
		public Point Center {
			get
			{
				var x = Left + (Right - Left) / 2;
				var y = Top + (Bottom - Top) / 2;
				return new Point(x, y);
			}
		}

		[Browsable(false)]
		public bool ShowRotation
		{
			get { return _showRotation; }
			set { _showRotation = value; }
		}

		[DataMember(EmitDefaultValue = false),
		Browsable(true),
		PropertyOrder(99),   // 99 so it goes to the end of the list
		DisplayName("Rotation Angle"),
		DescriptionAttribute("Rotates the prop by the specified degrees. " +
			                 "A positive value rotates in a clockwise direction and a negative value rotates in a counter-clockwise direction."),
		Category("Position")]
		public int RotationAngle
		{
			get { return _rotationAngle; }
			set
			{
				if (value < -360 || value > 360) return;

				// Check for a special case since Custom Props do their own rotation
				if (GetType().ToString() == "VixenModules.Preview.VixenPreview.Shapes.PreviewCustomProp")
				{
					_rotationAngle = value;
					Layout();
				}
				else
				{
					// Save off the currently selected point, if any
					PreviewPoint holdSelectedPoint = this._selectedPoint;

					// We need to force the rotation axis to reset to the center of the object
					if (RotationAxis != null)
					{
						this._selectedPoint = rotateHandle;
						PreviewTools.TransformPreviewPoint(this, new PreviewPoint(0, 0));
					}

					// Set the actual desired rotation angle
					_rotationAngle = value;

					FireOnPropertiesChanged(this, this);
					Layout();

					// Reset the selected point back to the original, if temporarily changed above
					this._selectedPoint = holdSelectedPoint;
				}
			}
		}

		public virtual void Match(PreviewBaseShape matchShape)
		{
			_rotationAngle = matchShape._rotationAngle;
		}

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
			_selectPoints = null;
		}

		public void SetSelectPoints(List<PreviewPoint> selectPoints, List<PreviewPoint> skewPoints)
		{
			// Set all the sizing points
			_selectPoints = selectPoints;

			if (rotateHandle == null)
			{
				rotateHandle = new PreviewPoint(Center.X, this.Top - 10);
				rotateHandle.PointType = PreviewPoint.PointTypes.RotateHandle;
			}

			if (RotationAxis == null)
			{
				RotationAxis = new PreviewPoint(Center);
				RotationAxis.PointType = PreviewPoint.PointTypes.RotationAxis;
			}

			_selectPoints.Add(rotateHandle);

			// Insert the rotation point as the first item.  This makes subsequent operations more efficient
			_selectPoints.Insert(0, RotationAxis);
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
		                         bool forceDraw, bool locked, double zoomLevel)
		{
			

			DrawSelectPoints(fp);
		}
		public virtual void DrawSelectPoints(FastPixel.FastPixel fp)
		{
			if (_selectPoints != null && _selectPoints.Count > 0) 
			{
				if (ShowRotation == true)
				{
					// Set the X position of the Rotation Handle
					rotateHandle.X = Center.X;
					// If the Object is normally oriented, set the Y position to -10 of the top. If inverted, set to 
					// -10 of the bottom, but it will still show at the top of the object, on the screen.
					int _top = _selectPoints.Find(x => x.PointType == PreviewPoint.PointTypes.SizeTopLeft).Y;
					int _bottom = int.MaxValue;
					var _bottomRight = _selectPoints.Find(x => x.PointType == PreviewPoint.PointTypes.SizeBottomRight);
					if (_bottomRight != null)
						_bottom = _bottomRight.Y;
					if (_top > _bottom)
						rotateHandle.Y = Bottom + 10;
					else
						rotateHandle.Y = Top - 10;
				}

				foreach (PreviewPoint point in _selectPoints) 
				{
					var transformedPoint = PreviewTools.TransformPreviewPoint(this, point, ZoomLevel);

					if (point?.PointType == PreviewPoint.PointTypes.Size ||
						point?.PointType == PreviewPoint.PointTypes.SizeTopLeft ||
						point?.PointType == PreviewPoint.PointTypes.SizeTopRight ||
						point?.PointType == PreviewPoint.PointTypes.SizeBottomLeft ||
						point?.PointType == PreviewPoint.PointTypes.SizeBottomRight )
					{
						fp.DrawRectangle(transformedPoint.X, transformedPoint.Y, SelectPointSize, Color.White);
					}
					else if (point?.PointType == PreviewPoint.PointTypes.RotateHandle && ShowRotation == true)
					{
						fp.DrawCircle(transformedPoint.X, transformedPoint.Y, SelectPointSize, Color.White);
					}
				}
			}
		}
		public void FireOnPropertiesChanged(Object sender, PreviewBaseShape shape)
		{
			if (OnPropertiesChanged != null)
				OnPropertiesChanged(sender, shape);
		}

		/// <summary>
		/// Process mouse movements
		/// </summary>
		/// <param name="x">X position of the cursor</param>
		/// <param name="y">Y position of the cursor</param>
		/// <param name="changeX">The distance the cursor moved in the X direction since the last MouseMove was called</param>
		/// <param name="changeY">The distance the cursor moved in the Y direction since the last MouseMove was called</param>
		public virtual void MouseMove(int x, int y, int changeX, int changeY)
		{
			if (_selectedPoint != null)
			{
				switch (_selectedPoint.PointType)
				{
					case PreviewPoint.PointTypes.RotateHandle:
						RotationAngle = PreviewTools.CalculateRotation(this, new Point(x, y), ZoomLevel, Control.ModifierKeys == Keys.Control);
						break;
				}
			}
			else
			{
				this.RotationAxis.X += changeX;
				this.RotationAxis.Y += changeY;
			}
		}

		public virtual void MouseUp(object sender, MouseEventArgs e)
		{
			FireOnPropertiesChanged(this, this);
		}
		public abstract bool PointInShape(PreviewPoint point);

		/// <summary>
		/// Determines if the object is wholly or partly contained within the rectangle
		/// </summary>
		/// <param name="rect">Defines the rectangle to evaluate</param>
		/// <param name="allIn">Optional: True - Object must be fully contained.
		///                               False - Only part of the object is within the rectangle
		/// </param>
		/// <returns>True or False</returns>
		public abstract bool ShapeInRect(Rectangle rect, bool allIn = false);
		
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
			if (_selectPoints != null)
			{
				foreach (PreviewPoint selectPoint in _selectPoints) {
					// Disallow the rotation axis as a selectable point
					if (selectPoint.PointType == PreviewPoint.PointTypes.RotationAxis)
						continue;

					var pp = PreviewTools.TransformPreviewPoint(this, selectPoint, ZoomLevel);

					if (selectPoint != null) {
						if (point.X >= pp.X - (SelectPointSize / 2) &&
							point.Y >= pp.Y - (SelectPointSize/2) &&
							point.X <= pp.X + (SelectPointSize/2) &&
							point.Y <= pp.Y + (SelectPointSize/2)) {
							return selectPoint;
						}
					}
				}
			}
			return null;
		}
		public PreviewPoint PointToZoomPoint(PreviewPoint p)
		{
			PreviewPoint newPoint = new PreviewPoint(p);
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
		public virtual void SetSelectPoint(PreviewPoint point = null)
		{
			// If coordinate 0,0 is passed in, then this signifies a reset of the shape's position
			if (point?.X == 0 && point?.Y == 0)
			{
				RotationAxis.X = Center.X;
				RotationAxis.Y = Center.Y;
			}
		}

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
				setupControl = new PreviewMovingHeadSetupControl(this);
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
