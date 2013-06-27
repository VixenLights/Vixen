/******************************************************************************
  Copyright 2009-2012 dataweb GmbH
  This file is part of the NShape framework.
  NShape is free software: you can redistribute it and/or modify it under the 
  terms of the GNU General Public License as published by the Free Software 
  Foundation, either version 3 of the License, or (at your option) any later 
  version.
  NShape is distributed in the hope that it will be useful, but WITHOUT ANY
  WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
  A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
  You should have received a copy of the GNU General Public License along with 
  NShape. If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using Dataweb.NShape.Commands;


namespace Dataweb.NShape.Advanced
{
	/// <summary>
	/// One-dimensional shape with optional caps on both ends defined by a sequence of vertices
	/// </summary>
	/// <remarks>RequiredPermissions set</remarks>
	public abstract class LineShapeBase : ShapeBase, ILinearShape
	{
		#region [Public] Shape Members

		/// <override></override>
		public override void CopyFrom(Shape source)
		{
			base.CopyFrom(source);
			if (source is LineShapeBase) {
				LineShapeBase sourceLine = (LineShapeBase) source;
				// Copy templated properties
				ICapStyle capStyle;
				capStyle = sourceLine.StartCapStyleInternal;
				privateStartCapStyle = (Template != null && capStyle == ((LineShapeBase) Template.Shape).StartCapStyleInternal)
				                       	? null
				                       	: capStyle;

				capStyle = sourceLine.EndCapStyleInternal;
				privateEndCapStyle = (Template != null && capStyle == ((LineShapeBase) Template.Shape).EndCapStyleInternal)
				                     	? null
				                     	: capStyle;

				// Copy Control points (exactly)
				CopyControlPoints(sourceLine);
			}
			else if (source is ILinearShape) {
				// Copy line control points with "best effort" approach
				CopyControlPoints(source);
			}
		}


		/// <override></override>
		public override void MakePreview(IStyleSet styleSet)
		{
			base.MakePreview(styleSet);
			if (StartCapStyleInternal == null)
				privateStartCapStyle = styleSet.GetPreviewStyle(styleSet.CapStyles.None);
			else privateStartCapStyle = styleSet.GetPreviewStyle(StartCapStyleInternal);
			if (EndCapStyleInternal == null)
				privateEndCapStyle = styleSet.GetPreviewStyle(styleSet.CapStyles.None);
			else privateEndCapStyle = styleSet.GetPreviewStyle(EndCapStyleInternal);
		}


		///// <override></override>
		//public abstract Point CalculateConnectionFoot(int x1, int y1, int x2, int y2);


		/// <override></override>
		public override void Fit(int x, int y, int width, int height)
		{
			Rectangle bounds = GetBoundingRectangle(true);
			if (bounds.Width == 0) bounds.Width = 1;
			if (bounds.Height == 0) bounds.Height = 1;
			foreach (ControlPointId id in GetControlPointIds(ControlPointCapabilities.Resize)) {
				// Calculate relative position
				Point p = GetControlPointPosition(id);
				p.Offset(-bounds.X, -bounds.Y);
				float tX = (float) p.X/bounds.Width;
				float tY = (float) p.Y/bounds.Height;
				MoveControlPointTo(id, x + (int) Math.Round(width*tX), y + (int) Math.Round(height*tY), ResizeModifiers.None);
			}

			//float scaleFactor = Geometry.CalcScaleFactor(bounds.Width, bounds.Height, width, height);
			//Matrix.Reset();
			//Matrix.Scale(scaleFactor, scaleFactor);
			//Matrix.TransformPoints(points);
			//... hier stimmt was nicht...
			//i = -1;
			//foreach (ControlPointId id in GetControlPointIds(ControlPointCapabilities.Resize)) {
			//    ++i;
			//    MoveControlPointTo(id, points[i].X, points[i].Y, ResizeModifiers.None);
			//}
			InvalidateDrawCache();
		}


		/// <override></override>
		public override void Connect(ControlPointId ownPointId, Shape otherShape, ControlPointId otherPointId)
		{
			if (otherShape == null) throw new ArgumentNullException("otherShape");
			if (otherShape.IsConnected(ControlPointId.Any, this) == ControlPointId.Reference)
				throw new InvalidOperationException(
					string.Format("The specified {0} is already connected to this {1} via Point-To-Shape connection.",
					              otherShape.Type.Name, this.Type.Name));
			base.Connect(ownPointId, otherShape, otherPointId);
		}


		/// <override></override>
		public override IEnumerable<ControlPointId> GetControlPointIds(ControlPointCapabilities controlPointCapability)
		{
			return Enumerator.Create(this, controlPointCapability);
		}


		/// <override></override>
		public override bool HasStyle(IStyle style)
		{
			if (IsStyleAffected(StartCapStyleInternal, style) || IsStyleAffected(EndCapStyleInternal, style))
				return true;
			else return base.HasStyle(style);
		}


		/// <override></override>
		public override void Invalidate()
		{
			base.Invalidate();
			if (DisplayService != null) {
				if (IsShapedLineCap(StartCapStyleInternal) && Geometry.IsValid(StartCapBounds))
					DisplayService.Invalidate(StartCapBounds);
				if (IsShapedLineCap(EndCapStyleInternal) && Geometry.IsValid(EndCapBounds))
					DisplayService.Invalidate(EndCapBounds);
			}
		}


		/// <override></override>
		public override Point GetControlPointPosition(ControlPointId controlPointId)
		{
			if (controlPointId == ControlPointId.None || controlPointId == ControlPointId.Any)
				throw new InvalidOperationException(string.Format("{0} is not a valid value for this operation", controlPointId));
			else if (controlPointId == ControlPointId.Reference) {
				Point p = Point.Empty;
				p.Offset(X, Y);
				return p;
			}
			else {
				int idx = GetControlPointIndex(controlPointId);
				if (idx >= 0)
					return controlPoints[idx].GetPosition();
				else {
					Debug.Fail("ControlPointId not found!");
					return Geometry.InvalidPoint;
				}
			}
		}


		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId,
		                                               ControlPointCapabilities controlPointCapability)
		{
			if (controlPointId == ControlPointId.None || controlPointId == ControlPointId.Any)
				return base.HasControlPointCapability(controlPointId, controlPointCapability);
			else if (controlPointId == ControlPointId.Reference) {
				return (((controlPointCapability & ControlPointCapabilities.Reference) > 0)
				        ||
				        (controlPointCapability & ControlPointCapabilities.Connect) != 0 && IsConnectionPointEnabled(controlPointId));
			}
			else if (IsFirstVertex(controlPointId)) {
				return ((controlPointCapability & ControlPointCapabilities.Glue) != 0
				        || (controlPointCapability & ControlPointCapabilities.Reference) != 0
				        || (controlPointCapability & ControlPointCapabilities.Resize) != 0);
			}
			else if (IsLastVertex(controlPointId)) {
				return ((controlPointCapability & ControlPointCapabilities.Glue) != 0
				        || (controlPointCapability & ControlPointCapabilities.Resize) != 0);
			}
			else {
				int pointIdx = GetControlPointIndex(controlPointId);
				if ((controlPointCapability & ControlPointCapabilities.Connect) != 0 && IsConnectionPointEnabled(controlPointId))
					return true;
				if (pointIdx >= 0 && pointIdx < ControlPointCount) {
					if (controlPoints[pointIdx] is VertexControlPoint)
						return ((controlPointCapability & ControlPointCapabilities.Resize) != 0);
					else if (controlPoints[pointIdx] is DynamicConnectionPoint)
						return ((controlPointCapability & ControlPointCapabilities.Movable) != 0);
				}
			}
			return false;
		}


		/// <override></override>
		public override IEnumerable<MenuItemDef> GetMenuItemDefs(int mouseX, int mouseY, int range)
		{
			// return actions of base class
			IEnumerator<MenuItemDef> enumerator = GetBaseMenuItemDefs(mouseX, mouseY, range);
			while (enumerator.MoveNext()) yield return enumerator.Current;

			// return own actions
			bool isFeasible;
			string description;
			ControlPointId clickedPointId = HitTest(mouseX, mouseY, ControlPointCapabilities.All, range);

			isFeasible = ContainsPoint(mouseX, mouseY) &&
			             (clickedPointId == ControlPointId.None || clickedPointId == ControlPointId.Reference);
			description = "You have to click on the line in order to insert new points";
			if (VertexCount >= MaxVertexCount) {
				isFeasible = false;
				description = "The line already has the maximum number of vertices";
			}
			yield return new CommandMenuItemDef("Insert Vertex", null, description, isFeasible,
			                                    new AddVertexCommand(this, mouseX, mouseY));

			isFeasible = ContainsPoint(mouseX, mouseY) &&
			             (clickedPointId == ControlPointId.None || clickedPointId == ControlPointId.Reference);
			description = "You have to click on the line in order to insert new points";
			yield return new CommandMenuItemDef("Insert Connection Point", null, description, isFeasible,
			                                    new AddConnectionPointCommand(this, mouseX, mouseY));

			isFeasible = false;
			if (HasControlPointCapability(clickedPointId, ControlPointCapabilities.Resize)) {
				if (!HasControlPointCapability(clickedPointId, ControlPointCapabilities.Glue)) {
					if ((clickedPointId != ControlPointId.None && IsConnected(clickedPointId, null) == ControlPointId.None)) {
						if (VertexCount > MinVertexCount)
							isFeasible = true;
						else description = "Minimum vertex count reached";
					}
					else description = "Control point is connected";
				}
				else description = "Glue control points may not be removed";
			}
			else description = "No resize point was clicked";
			yield return new CommandMenuItemDef("Remove Vertex", null, description, isFeasible,
			                                    new RemoveVertexCommand(this, clickedPointId));

			isFeasible = false;
			if (HasControlPointCapability(clickedPointId, ControlPointCapabilities.Connect | ControlPointCapabilities.Movable)
			    &&
			    !HasControlPointCapability(clickedPointId,
			                               ControlPointCapabilities.Glue | ControlPointCapabilities.Resize |
			                               ControlPointCapabilities.Reference)) {
				if ((clickedPointId != ControlPointId.None && IsConnected(clickedPointId, null) == ControlPointId.None))
					isFeasible = true;
				else description = "Connection point is connected";
			}
			else description = "No connection point was clicked";
			yield return new CommandMenuItemDef("Remove Connection Point", null, description, isFeasible,
			                                    new RemoveConnectionPointCommand(this, clickedPointId));
		}

		#endregion

		#region [Public] IEntity Members

		/// <summary>
		/// Retrieves the persistable properties of <see cref="T:Dataweb.NShape.Advanced.LineShapeBase" />.
		/// </summary>
		public new static IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version)
		{
			// Initialize definitions for inner objects
			foreach (EntityPropertyDefinition pi in ShapeBase.GetPropertyDefinitions(version))
				yield return pi;
			yield return new EntityFieldDefinition("StartCapStyle", typeof (object));
			yield return new EntityFieldDefinition("EndCapStyle", typeof (object));
			yield return new EntityInnerObjectsDefinition(attrNameVertices, "Core.Vertex", vertexAttrNames, vertexAttrTypes);
			if (version >= 4)
				yield return
					new EntityInnerObjectsDefinition(attrNameConnectionPoints, "Core.ConnectionPoint", connectionPointAttrNames,
					                                 connectionPointAttrTypes);
		}


		/// <override></override>
		protected override void SaveFieldsCore(IRepositoryWriter writer, int version)
		{
			base.SaveFieldsCore(writer, version);
			writer.WriteStyle(privateStartCapStyle);
			writer.WriteStyle(privateEndCapStyle);
		}


		/// <override></override>
		protected override void LoadFieldsCore(IRepositoryReader reader, int version)
		{
			base.LoadFieldsCore(reader, version);
			privateStartCapStyle = reader.ReadCapStyle();
			privateEndCapStyle = reader.ReadCapStyle();
		}


		/// <override></override>
		protected override void SaveInnerObjectsCore(string propertyName, IRepositoryWriter writer, int version)
		{
			if (propertyName == attrNameVertices) {
				// Save vertices
				writer.BeginWriteInnerObjects();
				for (int i = 0; i < ControlPointCount; ++i) {
					LineControlPoint linePt = controlPoints[i];
					// Dynamic connection points will be converted into vertices in versions < 4.
					if (controlPoints[i] is VertexControlPoint || version < 4) {
						Point p = linePt.GetPosition();

						writer.BeginWriteInnerObject();
						writer.WriteInt32(i);
						writer.WriteInt32(linePt.Id);
						writer.WriteInt32(p.X);
						writer.WriteInt32(p.Y);
						writer.EndWriteInnerObject();
					}
				}
				writer.EndWriteInnerObjects();
			}
			else if (propertyName == attrNameConnectionPoints) {
				// Save dynamic control points
				if (version >= 4) {
					writer.BeginWriteInnerObjects();
					for (int i = 0; i < ControlPointCount; ++i) {
						if (controlPoints[i] is DynamicConnectionPoint) {
							DynamicConnectionPoint linePt = (DynamicConnectionPoint) controlPoints[i];

							writer.BeginWriteInnerObject();
							writer.WriteInt32(i);
							writer.WriteInt32(linePt.Id);
							writer.WriteInt32(linePt.RelativePosition.A);
							writer.WriteInt32(linePt.RelativePosition.B);
							writer.WriteInt32(linePt.RelativePosition.C);
							writer.EndWriteInnerObject();
						}
					}
					writer.EndWriteInnerObjects();
				}
			}
			else base.SaveInnerObjectsCore(propertyName, writer, version);
		}


		/// <override></override>
		protected override void LoadInnerObjectsCore(string propertyName, IRepositoryReader reader, int version)
		{
			if (propertyName == attrNameVertices) {
				// Clear control points and vertex count
				controlPoints.Clear();
				vertexCount = 0;

				// Load vertices
				reader.BeginReadInnerObjects();
				while (reader.BeginReadInnerObject()) {
					// Read point definition
					int pointIdx = reader.ReadInt32();
					ControlPointId pointId = reader.ReadInt32();
					int x = reader.ReadInt32();
					int y = reader.ReadInt32();
					// Create point
					Point vertex = Point.Empty;
					vertex.Offset(x, y);
					InsertControlPoint(controlPoints.Count, CreateVertex(pointId, vertex));
					reader.EndReadInnerObject();
				}
				reader.EndReadInnerObjects();
			}
			else if (propertyName == attrNameConnectionPoints) {
				// Load dynamic connection points
				if (version >= 4) {
					reader.BeginReadInnerObjects();
					while (reader.BeginReadInnerObject()) {
						// Read point definition
						int pointIdx = reader.ReadInt32();
						ControlPointId pointId = reader.ReadInt32();
						RelativePosition pos = RelativePosition.Empty;
						pos.A = reader.ReadInt32();
						pos.B = reader.ReadInt32();
						pos.C = reader.ReadInt32();
						// Create point
						InsertControlPoint(pointIdx, new DynamicConnectionPoint(this, pointId, pos));
						reader.EndReadInnerObject();
					}
					reader.EndReadInnerObjects();
				}
			}
			else base.LoadInnerObjectsCore(propertyName, reader, version);
		}

		#endregion

		#region [Public] ILinearShape Members

		/// <override></override>
		public abstract ControlPointId InsertVertex(ControlPointId beforePointId, int x, int y);


		/// <summary>
		/// Inserts a new vertex accessible before the specified vertex at the given position x, y. The new vertex will have the specified control point id.
		/// </summary>
		/// <remarks>This method is not part of the ILinearShape interface. It applies only for linear shapes derived from LineShapeBase.</remarks>
		public abstract ControlPointId InsertVertex(ControlPointId beforePointId, ControlPointId newVertexId, int x, int y);


		/// <override></override>
		public abstract ControlPointId AddVertex(int x, int y);


		/// <override></override>
		public abstract void RemoveVertex(ControlPointId controlPointId);


		/// <override></override>
		public abstract ControlPointId AddConnectionPoint(int x, int y);


		/// <override></override>
		public virtual void RemoveConnectionPoint(ControlPointId pointId)
		{
			switch (pointId) {
				case ControlPointId.Any:
				case ControlPointId.None:
				case ControlPointId.FirstVertex:
				case ControlPointId.LastVertex:
				case ControlPointId.Reference:
					throw new ArgumentException("Invalid ControlPointId for this operation.");
			}
			int idx = GetControlPointIndex(pointId);
			if (idx < 0 || idx >= ControlPointCount)
				throw new ArgumentOutOfRangeException(string.Format("Line does not contain control point {0}", pointId));
			if (GetControlPoint(idx) is VertexControlPoint)
				throw new InvalidOperationException(string.Format("Control point {0} is a vertex.", pointId));
			RemoveControlPoint(idx);
		}


		/// <override></override>
		public abstract Point CalcNormalVector(Point p);


		/// <override></override>
		[Browsable(false)]
		public abstract int MinVertexCount { get; }


		/// <override></override>
		[Browsable(false)]
		public abstract int MaxVertexCount { get; }


		/// <override></override>
		[Browsable(false)]
		public virtual int VertexCount
		{
			get { return vertexCount; }
		}


		/// <summary>
		/// Retrieve the id of the next neighbor vertex of pointId in physical order "start to end"
		/// </summary>
		public ControlPointId GetNextVertexId(ControlPointId pointId)
		{
			return GetNextPointIdCore(pointId, ControlPointCapabilities.Resize, +1);
		}


		/// <summary>
		/// Retrieve the id of the previous neighbor vertex of pointId in physical order "end to start"
		/// </summary>
		public ControlPointId GetPreviousVertexId(ControlPointId pointId)
		{
			return GetNextPointIdCore(pointId, ControlPointCapabilities.Resize, -1);
		}


		/// <override></override>
		[Browsable(false)]
		public bool IsDirected
		{
			get
			{
				return (IsShapedLineCap(StartCapStyleInternal))
				       || (IsShapedLineCap(EndCapStyleInternal));
			}
		}

		#endregion

		#region [Protected] Properties

		/// <summary>
		/// Internal start CapStyle of the line. May be published by a decendant through a property
		/// </summary>
		protected ICapStyle StartCapStyleInternal
		{
			get { return privateStartCapStyle ?? ((LineShapeBase) Template.Shape).StartCapStyleInternal; }
			set
			{
				Invalidate();
				if (Owner != null) Owner.NotifyChildResizing(this);

				privateStartCapStyle = (Template != null && value == ((LineShapeBase) Template.Shape).StartCapStyleInternal)
				                       	? null
				                       	: value;
				InvalidateDrawCache();

				if (Owner != null) Owner.NotifyChildResized(this);
				Invalidate();
			}
		}


		/// <summary>
		/// Internal end CapStyle of the line. May be published by a decendant through a property
		/// </summary>
		protected ICapStyle EndCapStyleInternal
		{
			get { return privateEndCapStyle ?? ((LineShapeBase) Template.Shape).EndCapStyleInternal; }
			set
			{
				Invalidate();
				if (Owner != null) Owner.NotifyChildResizing(this);

				privateEndCapStyle = (Template != null && value == ((LineShapeBase) Template.Shape).EndCapStyleInternal)
				                     	? null
				                     	: value;
				InvalidateDrawCache();

				if (Owner != null) Owner.NotifyChildResized(this);
				Invalidate();
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected float StartCapAngle
		{
			get
			{
				if (float.IsNaN(startCapAngle))
					startCapAngle = CalcCapAngle(ControlPointId.FirstVertex);
				return startCapAngle;
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected float EndCapAngle
		{
			get
			{
				if (float.IsNaN(endCapAngle))
					endCapAngle = CalcCapAngle(ControlPointId.LastVertex);
				return endCapAngle;
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected Rectangle StartCapBounds
		{
			get
			{
				if (!Geometry.IsValid(startCapBounds) && !float.IsNaN(StartCapAngle)) {
					if (IsShapedLineCap(StartCapStyleInternal))
						startCapBounds = ToolCache.GetCapBounds(StartCapStyleInternal, LineStyle, StartCapAngle);
					else startCapBounds = Rectangle.Empty;
					startCapBounds.Offset(GetControlPointPosition(ControlPointId.FirstVertex));
				}
				return startCapBounds;
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected Rectangle EndCapBounds
		{
			get
			{
				if (!Geometry.IsValid(endCapBounds) && !float.IsNaN(EndCapAngle)) {
					if (IsShapedLineCap(EndCapStyleInternal))
						endCapBounds = ToolCache.GetCapBounds(EndCapStyleInternal, LineStyle, EndCapAngle);
					else endCapBounds = Rectangle.Empty;
					endCapBounds.Offset(GetControlPointPosition(ControlPointId.LastVertex));
				}
				return endCapBounds;
			}
		}

		#endregion

		#region [Protected Internal] Methods (Inherited)

		/// <summary>
		/// Protetced internal constructur. Should only be called by the <see cref="T:Dataweb.NShape.Advanced.ShapeType" />'s <see cref="T:Dataweb.NShape.Advanced.CreateShapeDelegate" />
		/// </summary>
		protected internal LineShapeBase(ShapeType shapeType, Template template)
			: base(shapeType, template)
		{
			Construct();
		}


		/// <summary>
		/// Protetced internal constructor. Should only be called by the <see cref="T:Dataweb.NShape.Advanced.ShapeType" />'s <see cref="T:Dataweb.NShape.Advanced.CreateShapeDelegate" />
		/// </summary>
		protected internal LineShapeBase(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet)
		{
			Construct();
		}


		/// <override></override>
		protected internal override void InitializeToDefault(IStyleSet styleSet)
		{
			base.InitializeToDefault(styleSet);

			privateStartCapStyle = styleSet.CapStyles.None;
			privateEndCapStyle = styleSet.CapStyles.None;

			controlPoints[0].SetPosition(Point.Empty);
			controlPoints[1].SetPosition(20, 20);
		}


		/// <override></override>
		protected internal override int ControlPointCount
		{
			get { return controlPoints.Count; }
		}


		/// <summary>
		/// Calculates the diagram cells this shape occupies.
		/// </summary>
		protected IEnumerable<Point> CalculateCommonCells(int cellSize)
		{
			// Calculate cells occupied by children
			if (ChildrenCollection != null) {
				foreach (Shape shape in ChildrenCollection.BottomUp)
					foreach (Point cell in shape.CalculateCells(cellSize))
						yield return cell;
			}
			// Calculate cells occupied by the line caps
			Rectangle startCapCells = Geometry.InvalidRectangle;
			if (IsShapedLineCap(StartCapStyleInternal)) {
				startCapCells.X = StartCapBounds.Left/cellSize;
				startCapCells.Y = StartCapBounds.Top/cellSize;
				startCapCells.Width = (StartCapBounds.Right/cellSize) - startCapCells.X;
				startCapCells.Height = (StartCapBounds.Bottom/cellSize) - startCapCells.Y;
				Point p = Point.Empty;
				for (p.X = startCapCells.Left; p.X <= startCapCells.Right; p.X += 1)
					for (p.Y = startCapCells.Top; p.Y <= startCapCells.Bottom; p.Y += 1)
						yield return p;
			}
			Rectangle endCapCells = Geometry.InvalidRectangle;
			if (IsShapedLineCap(EndCapStyleInternal)) {
				endCapCells.X = EndCapBounds.Left/cellSize;
				endCapCells.Y = EndCapBounds.Top/cellSize;
				endCapCells.Width = (EndCapBounds.Right/cellSize) - endCapCells.X;
				endCapCells.Height = (EndCapBounds.Bottom/cellSize) - endCapCells.Y;
				Point p = Point.Empty;
				for (p.X = endCapCells.Left; p.X <= endCapCells.Right; p.X += 1) {
					for (p.Y = endCapCells.Top; p.Y <= endCapCells.Bottom; p.Y += 1) {
						// Skip all cells occupied by the startCap's bounds
						if (Geometry.IsValid(startCapCells) && Geometry.RectangleContainsPoint(startCapCells, p))
							continue;
						yield return p;
					}
				}
			}
		}

		#endregion

		#region [Protected] Methods (Inherited)

		/// <override></override>
		protected override void ProcessExecModelPropertyChange(IModelMapping propertyMapping)
		{
			switch (propertyMapping.ShapePropertyId) {
				case PropertyIdStartCapStyle:
					privateStartCapStyle = (propertyMapping.GetStyle() as ICapStyle);
					InvalidateDrawCache();
					Invalidate();
					break;
				case PropertyIdEndCapStyle:
					privateEndCapStyle = (propertyMapping.GetStyle() as ICapStyle);
					InvalidateDrawCache();
					Invalidate();
					break;
				default:
					base.ProcessExecModelPropertyChange(propertyMapping);
					break;
			}
		}


		/// <override></override>
		protected override bool RotateCore(int angle, int x, int y)
		{
			bool result = false;
			if (IsConnected(1, null) != ControlPointId.None || IsConnected(2, null) != ControlPointId.None)
				result = false;
			else {
				Point rotationCenter = Point.Empty;
				// Prepare transformation matrix and transform vertices
				rotationCenter.Offset(x, y);
				Matrix.Reset();
				Matrix.RotateAt(Geometry.TenthsOfDegreeToDegrees(angle), rotationCenter);
				// Rotate vertices
				Point[] vertex = new Point[1];
				for (int i = controlPoints.Count - 1; i >= 0; --i) {
					if (controlPoints[i] is VertexControlPoint) {
						vertex[0] = controlPoints[i].GetPosition();
						Matrix.TransformPoints(vertex);
						controlPoints[i].SetPosition(vertex[0]);
					}
				}
				InvalidateDrawCache();
				result = true;
			}
			return result;
		}


		/// <override></override>
		protected override sealed Rectangle CalculateBoundingRectangle(bool tight)
		{
			// tight fitting and loose bounding rectangle are equal for lines
			Rectangle result = CalculateBoundingRectangleCore(tight);
			if (IsShapedLineCap(StartCapStyleInternal))
				result = Geometry.UniteRectangles(result, StartCapBounds);
			if (IsShapedLineCap(EndCapStyleInternal))
				result = Geometry.UniteRectangles(result, EndCapBounds);
			return result;
		}


		/// <summary>
		/// Calculates the X-axis aligned bounding rectangle of the line (without caps).
		/// </summary>
		protected abstract Rectangle CalculateBoundingRectangleCore(bool tight);


		/// <override></override>
		protected override bool MoveByCore(int deltaX, int deltaY)
		{
			if (IsConnected(ControlPointId.FirstVertex, null) != ControlPointId.None
			    || IsConnected(ControlPointId.LastVertex, null) != ControlPointId.None)
				return false;

			// Move vertices
			Point p = Point.Empty;
			for (int i = controlPoints.Count - 1; i >= 0; --i) {
				if (controlPoints[i] is DynamicConnectionPoint)
					continue;
				controlPoints[i].Offset(deltaX, deltaY);
			}
			// Move CapBounds (if calculated)
			if (Geometry.IsValid(startCapBounds)) startCapBounds.Offset(deltaX, deltaY);
			if (Geometry.IsValid(endCapBounds)) endCapBounds.Offset(deltaX, deltaY);
			TransformDrawCache(deltaX, deltaY, 0, X, Y);
			return true;
		}


		/// <override></override>
		protected override bool ContainsPointCore(int x, int y)
		{
			if (IsShapedLineCap(StartCapStyleInternal))
				if (StartCapContainsPoint(x, y)) return true;
			if (IsShapedLineCap(EndCapStyleInternal))
				if (EndCapContainsPoint(x, y)) return true;
			return false;
		}


		/// <summary>
		/// Retrieve ControlPoint's index in the list of point id's (in physical order)
		/// </summary>
		protected override int GetControlPointIndex(ControlPointId pointId)
		{
			if (pointId == ControlPointId.Reference)
				return -1;
			else if (IsFirstVertex(pointId))
				return firstVertexIdx;
			else if (IsLastVertex(pointId))
				return lastVertexIdx;
			else {
				// Find point id
				for (int i = ControlPointCount - 1; i >= 0; --i) {
					if (controlPoints[i].Id == pointId)
						return i;
				}
				return -1;
			}
		}


		///// <summary>
		///// Retrieve ControlPoint's index in the list of point id's (in physical order)
		///// </summary>
		//protected virtual int GetVertexIndex(ControlPointId pointId) {
		//    if (pointId == ControlPointId.Reference)
		//        return -1;
		//    else if (IsFirstVertex(pointId))
		//        return 0;
		//    else if (IsLastVertex(pointId))
		//        return VertexCount - 1;
		//    else return IndexOfPointId(pointId, ControlPointCapabilities.Resize);
		//}


		/// <summary>
		/// Retrieve physical point's ControlPointId
		/// </summary>
		protected override ControlPointId GetControlPointId(int pointIdx)
		{
			if (pointIdx == 0)
				return ControlPointId.FirstVertex;
			else if (pointIdx == lastVertexIdx)
				return ControlPointId.LastVertex;
			else return controlPoints[pointIdx].Id;
		}


		/// <override></override>
		protected override bool IsConnectionPointEnabled(ControlPointId pointId)
		{
			return base.IsConnectionPointEnabled(pointId);
		}


		/// <override></override>
		protected override void InvalidateDrawCache()
		{
			base.InvalidateDrawCache();
			// Do not delete shapePoints or cap buffers here for performance reasons
			startCapAngle = endCapAngle = float.NaN;
			startCapBounds = endCapBounds = Geometry.InvalidRectangle;
		}


		/// <override></override>
		protected override void UpdateDrawCache()
		{
			if (drawCacheIsInvalid) {
				Debug.Assert(shapePoints != null);
				// Calculate the line's shape points at the coordinate system's origin...
				RecalcDrawCache();
				// ... and transform to the current position
				TransformDrawCache(X, Y, 0, X, Y);
			}
		}


		/// <override></override>
		protected override void RecalcDrawCache()
		{
			if (IsShapedLineCap(StartCapStyleInternal))
				DoRecalcCapPoints(ControlPointId.FirstVertex, StartCapStyleInternal, StartCapAngle, ref startCapPointBuffer);
			if (IsShapedLineCap(EndCapStyleInternal))
				DoRecalcCapPoints(ControlPointId.LastVertex, EndCapStyleInternal, EndCapAngle, ref endCapPointBuffer);
			drawCacheIsInvalid = false;
		}


		/// <summary>
		/// Transforms all objects that need to be transformed, such as Point-Arrays, GraphicsPaths or Brushes
		/// </summary>
		/// <param name="deltaX">Translation on X axis</param>
		/// <param name="deltaY">Translation on Y axis</param>
		/// <param name="deltaAngle">Rotation shapeAngle in tenths of degrees</param>
		/// <param name="rotationCenterX">X coordinate of the rotation center</param>
		/// <param name="rotationCenterY">Y coordinate of the rotation center</param>
		protected override void TransformDrawCache(int deltaX, int deltaY, int deltaAngle, int rotationCenterX,
		                                           int rotationCenterY)
		{
			Matrix.Reset();
			if (!drawCacheIsInvalid) {
				if (deltaX != 0 || deltaY != 0 || deltaAngle != 0) {
					Matrix.Translate(deltaX, deltaY, MatrixOrder.Prepend);
					if (deltaAngle != 0) {
						PointF rotationCenter = PointF.Empty;
						rotationCenter.X = rotationCenterX;
						rotationCenter.Y = rotationCenterY;
						Matrix.RotateAt(Geometry.TenthsOfDegreeToDegrees(deltaAngle), rotationCenter, MatrixOrder.Append);
					}
					if (shapePoints != null) Matrix.TransformPoints(shapePoints);
					if (startCapPointBuffer != null) Matrix.TransformPoints(startCapPointBuffer);
					if (endCapPointBuffer != null) Matrix.TransformPoints(endCapPointBuffer);
				}
			}
		}

		#endregion

		#region [Protected] Methods

		/// <ToBeCompleted></ToBeCompleted>
		protected bool IsFirstVertex(ControlPointId pointId)
		{
			return (pointId == ControlPointId.FirstVertex || controlPoints[firstVertexIdx].Id == pointId);
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected bool IsLastVertex(ControlPointId pointId)
		{
			return (pointId == ControlPointId.LastVertex || controlPoints[lastVertexIdx].Id == pointId);
		}


		/// <summary>
		/// Returns true if the line cap is a shaped and sizable line cap.
		/// </summary>
		protected bool IsShapedLineCap(ICapStyle capStyle)
		{
			if (capStyle == null) return false;
			switch (capStyle.CapShape) {
				case CapShape.None:
				case CapShape.Flat:
				case CapShape.Peak:
				case CapShape.Round:
					return false;
				case CapShape.ClosedArrow:
				case CapShape.OpenArrow:
				case CapShape.CenteredCircle:
				case CapShape.CenteredHalfCircle:
				case CapShape.Circle:
				case CapShape.Diamond:
				case CapShape.Square:
				case CapShape.Triangle:
					return true;
				default:
					throw new NShapeUnsupportedValueException(capStyle.CapShape);
			}
		}


		/// <summary>
		/// Performs an intersection test on the LineCap
		/// </summary>
		protected bool StartCapIntersectsWith(Rectangle rectangle)
		{
			if (IsShapedLineCap(StartCapStyleInternal)) {
				if (Geometry.RectangleIntersectsWithRectangle(startCapBounds, rectangle)) {
					if (Geometry.PolygonIntersectsWithRectangle(startCapPointBuffer, rectangle))
						return true;
				}
			}
			return false;
		}


		/// <summary>
		/// Performs an intersection test on the LineCap
		/// </summary>
		protected bool EndCapIntersectsWith(Rectangle rectangle)
		{
			if (IsShapedLineCap(EndCapStyleInternal)) {
				if (Geometry.RectangleIntersectsWithRectangle(endCapBounds, rectangle)) {
					if (Geometry.PolygonIntersectsWithRectangle(endCapPointBuffer, rectangle))
						return true;
				}
			}
			return false;
		}


		/// <summary>
		/// Performs a hit test on the LineCap
		/// </summary>
		protected bool StartCapContainsPoint(int pointX, int pointY)
		{
			if (Geometry.RectangleContainsPoint(startCapBounds, pointX, pointY)) {
				if (startCapPointBuffer == null)
					CalcCapPoints(GetControlPointIndex(ControlPointId.FirstVertex), StartCapAngle, StartCapStyleInternal, LineStyle,
					              ref startCapBounds, ref startCapPointBuffer);
				if (Geometry.ConvexPolygonContainsPoint(startCapPointBuffer, pointX, pointY))
					return true;
			}
			return false;
		}


		/// <summary>
		/// Performs a hit test on the LineCap
		/// </summary>
		protected bool EndCapContainsPoint(int pointX, int pointY)
		{
			if (Geometry.RectangleContainsPoint(endCapBounds, pointX, pointY)) {
				if (endCapPointBuffer == null)
					CalcCapPoints(GetControlPointIndex(ControlPointId.LastVertex), endCapAngle, EndCapStyleInternal, LineStyle,
					              ref endCapBounds, ref endCapPointBuffer);
				if (Geometry.ConvexPolygonContainsPoint(endCapPointBuffer, pointX, pointY))
					return true;
			}
			return false;
		}


		/// <summary>
		/// Calculates the line cap angle for the given control point.
		/// </summary>
		/// <param name="pointId"> ControlPointId.FirstVertex or ControlPointId.LastVertex</param>
		/// <remarks>
		/// GDI+ uses the intercsection point of the line cap's border with the line itself to calculate the angle of the cap.
		/// E.g: If a polyline a vertex inside its cap that changes the direction, the intersection point of this segment with 
		/// the line cap's shape is used to calculate the angle. 
		/// Therefore we cannot use normal- or tangent vectors for cap angle calculation.
		/// </remarks>
		/// <returns>Line cap angle in degrees</returns>
		protected abstract float CalcCapAngle(ControlPointId pointId);


		/// <summary>
		/// Draws the line's StartCap
		/// </summary>
		protected void DrawStartCapBackground(Graphics graphics, int pointX, int pointY)
		{
			if (IsShapedLineCap(StartCapStyleInternal)) {
				Brush capBrush = ToolCache.GetBrush(StartCapStyleInternal.ColorStyle, LineStyle);
				// ToDo: Find a solution for round caps - perhaps transform the GraphicsPath itself?
				if (startCapPointBuffer != null && startCapPointBuffer.Length > 0)
					graphics.FillPolygon(capBrush, startCapPointBuffer, System.Drawing.Drawing2D.FillMode.Alternate);
			}
		}


		/// <summary>
		/// Draws the line's EndCap
		/// </summary>
		protected void DrawEndCapBackground(Graphics graphics, int pointX, int pointY)
		{
			if (IsShapedLineCap(EndCapStyleInternal)) {
				Brush capBrush = ToolCache.GetBrush(EndCapStyleInternal.ColorStyle, LineStyle);
				// ToDo: Find a solution for round caps - perhaps transform the GraphicsPath itself?
				if (endCapPointBuffer != null && endCapPointBuffer.Length > 0)
					graphics.FillPolygon(capBrush, endCapPointBuffer, System.Drawing.Drawing2D.FillMode.Alternate);
			}
		}


		/// <summary>
		/// Re-Calculates the position of a glue point when the control point next to the glue point moved.
		/// If the given glue point is not next to the moved point and/or if it is not connected via 
		/// Point-to-Shape connection, this method will do nothing. 
		/// Otherwise, the position of the given glue point will be re-calculated and the glue point will be moved.
		/// </summary>
		/// <param name="gluePointId">Glue point to be moved</param>
		/// <param name="movedPointId">The point id that</param>
		/// <returns></returns>
		protected bool MaintainGluePointPosition(ControlPointId gluePointId, ControlPointId movedPointId)
		{
			if ((GetNextVertexId(gluePointId) == movedPointId || GetPreviousVertexId(gluePointId) == movedPointId)
			    && HasControlPointCapability(gluePointId, ControlPointCapabilities.Glue)) {
				// Recalc glue point if it is connected to an other shape via point-to-shape connection
				ShapeConnectionInfo sci = GetConnectionInfo(gluePointId, null);
				if (!sci.IsEmpty && sci.OtherPointId == ControlPointId.Reference) {
					Point pos = CalcGluePoint(gluePointId, sci.OtherShape);
					if (Geometry.IsValid(pos)) {
						controlPoints[GetControlPointIndex(gluePointId)].SetPosition(pos);
						return true;
					}
				}
			}
			return false;
		}


		/// <summary>
		/// Constructs a new instance.
		/// </summary>
		protected virtual void Construct()
		{
			controlPoints = new List<LineControlPoint>(MinVertexCount);
			for (int i = MinVertexCount - 1; i >= 0; --i)
				InsertControlPoint(0, CreateVertex(i + 1, Point.Empty));
			shapePoints = new Point[MinVertexCount];
			InvalidateDrawCache();
		}


		/// <summary>
		/// Creates a vertex. Override to use a different vertex type.
		/// </summary>
		protected virtual LineControlPoint CreateVertex(ControlPointId id, Point position)
		{
			return new VertexControlPoint(id, position);
		}


		/// <summary>
		/// Retrieve a new PointId for a new point
		/// </summary>
		/// <returns></returns>
		protected virtual ControlPointId GetNewControlPointId()
		{
			for (int id = 1; id <= ControlPointCount; ++id) {
				if (GetControlPointIndex(id) < 0)
					return id;
			}
			return ControlPointCount + 1;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected IEnumerable<LineControlPoint> ControlPoints
		{
			get { return controlPoints; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected LineControlPoint GetControlPoint(int index)
		{
			return controlPoints[index];
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void SetControlPoint(int index, LineControlPoint controlPoint)
		{
			controlPoints[index] = controlPoint;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected virtual void InsertControlPoint(int index, LineControlPoint controlPoint)
		{
			// Maintain relative positions of dynamic control points:
			// Store all positions of dynamic control points and re-calculate 
			// them after removing the control point
			Dictionary<ControlPointId, Point> changedDynCtrlPoints = null;
			if (controlPoint is VertexControlPoint) {
				Invalidate();
				changedDynCtrlPoints = StoreDynControlPointPositions();
			}

			controlPoints.Insert(index, controlPoint);
			if (controlPoint is VertexControlPoint)
				++vertexCount;
			MaintainFirstVertexIndex();
			MaintainLastVertexIndex();

			if (controlPoint is VertexControlPoint) {
				InvalidateDrawCache();

				// Recalculate position of dynamic control points
				if (changedDynCtrlPoints != null)
					RestoreDynControlPointPositions(changedDynCtrlPoints);

				ControlPointsHaveMoved();
				Invalidate();
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected virtual void RemoveControlPoint(int index)
		{
			// Maintain relative positions of dynamic control points:
			// Store all positions of dynamic control points and re-calculate 
			// them after removing the control point
			Dictionary<ControlPointId, Point> changedDynCtrlPoints = null;
			bool isVertex = controlPoints[index] is VertexControlPoint;
			if (isVertex) {
				Invalidate();
				changedDynCtrlPoints = StoreDynControlPointPositions();
			}

			if (controlPoints[index] is VertexControlPoint)
				--vertexCount;
			controlPoints.RemoveAt(index);
			MaintainFirstVertexIndex();
			MaintainLastVertexIndex();

			if (isVertex) {
				InvalidateDrawCache();

				// Recalculate position of dynamic control points
				if (changedDynCtrlPoints != null)
					RestoreDynControlPointPositions(changedDynCtrlPoints);

				ControlPointsHaveMoved();
				Invalidate();
			}
		}

		#endregion

		#region [Protected] Types - LineControlPoint

		/// <summary>
		/// Defines the position of a dynamic connection point along on a linear shape.
		/// The position of the point is defined as tenths of percentage of the line's length.
		/// </summary>
		protected abstract class LineControlPoint
		{
			/// <ToBeCompleted></ToBeCompleted>
			public LineControlPoint(LineControlPoint source)
			{
				if (source == null) throw new ArgumentNullException("source");
				this.controlPointId = source.controlPointId;
				this.SetPosition(source.GetPosition());
			}

			/// <ToBeCompleted></ToBeCompleted>
			public LineControlPoint(ControlPointId controlPointId)
			{
				this.controlPointId = controlPointId;
			}

			/// <ToBeCompleted></ToBeCompleted>
			public abstract LineControlPoint Clone();

			/// <ToBeCompleted></ToBeCompleted>
			public abstract void CopyFrom(LineControlPoint source);

			/// <ToBeCompleted></ToBeCompleted>
			public abstract Point GetPosition();

			/// <ToBeCompleted></ToBeCompleted>
			public abstract void SetPosition(int x, int y);

			/// <ToBeCompleted></ToBeCompleted>
			public abstract void Offset(int deltaX, int deltaY);

			/// <ToBeCompleted></ToBeCompleted>
			public void SetPosition(Point position)
			{
				SetPosition(position.X, position.Y);
			}

			/// <ToBeCompleted></ToBeCompleted>
			public ControlPointId Id
			{
				get { return controlPointId; }
				set { controlPointId = value; }
			}

			/// <ToBeCompleted></ToBeCompleted>
			private ControlPointId controlPointId = ControlPointId.None;
		}


		/// <summary>
		/// Base class for control points based on absolute coordinates, e.g. vertices
		/// </summary>
		protected abstract class AbsoluteLineControlPoint : LineControlPoint
		{
			/// <ToBeCompleted></ToBeCompleted>
			public AbsoluteLineControlPoint(AbsoluteLineControlPoint source)
				: base(source)
			{
			}

			/// <ToBeCompleted></ToBeCompleted>
			public AbsoluteLineControlPoint(ControlPointId pointId, Point position)
				: this(pointId, position.X, position.Y)
			{
			}

			/// <ToBeCompleted></ToBeCompleted>
			public AbsoluteLineControlPoint(ControlPointId pointId, int x, int y)
				: base(pointId)
			{
				Debug.Assert(position == Point.Empty);
				this.position.Offset(x, y);
			}

			/// <ToBeCompleted></ToBeCompleted>
			public override Point GetPosition()
			{
				return position;
			}

			/// <ToBeCompleted></ToBeCompleted>
			public override void SetPosition(int x, int y)
			{
				this.position.X = x;
				this.position.Y = y;
			}

			/// <ToBeCompleted></ToBeCompleted>
			public override void Offset(int deltaX, int deltaY)
			{
				this.position.Offset(deltaX, deltaY);
			}

			/// <ToBeCompleted></ToBeCompleted>
			protected void CopyCore(AbsoluteLineControlPoint source)
			{
				if (source == null) throw new ArgumentNullException("source");
				this.Id = source.Id;
				this.position = source.position;
			}

			private Point position;
		}


		/// <summary>
		/// Base class for control points based on relative positions, e.g. dynamic connection points
		/// </summary>
		protected abstract class RelativeLineControlPoint : LineControlPoint
		{
			/// <ToBeCompleted></ToBeCompleted>
			public RelativeLineControlPoint(RelativeLineControlPoint source)
				: base(source.Id)
			{
				this.owner = source.owner;
				this.SetPosition(source.GetPosition());
			}

			/// <ToBeCompleted></ToBeCompleted>
			public RelativeLineControlPoint(LineShapeBase owner, ControlPointId pointId, RelativePosition position)
				: base(pointId)
			{
				this.owner = owner;
				this.relativePosition = position;
			}

			/// <ToBeCompleted></ToBeCompleted>
			public RelativePosition RelativePosition
			{
				get { return relativePosition; }
				set { relativePosition = value; }
			}

			/// <ToBeCompleted></ToBeCompleted>
			public override Point GetPosition()
			{
				return owner.CalculateAbsolutePosition(this.relativePosition);
			}

			/// <ToBeCompleted></ToBeCompleted>
			public override void SetPosition(int x, int y)
			{
				this.relativePosition = owner.CalculateRelativePosition(x, y);
			}

			/// <ToBeCompleted></ToBeCompleted>
			public override void Offset(int deltaX, int deltaY)
			{
				Point p = GetPosition();
				p.Offset(deltaX, deltaY);
				SetPosition(p.X, p.Y);
			}

			/// <ToBeCompleted></ToBeCompleted>
			protected void CopyCore(RelativeLineControlPoint source)
			{
				if (source == null) throw new ArgumentNullException("source");
				this.Id = source.Id;
				this.owner = source.owner;
				this.relativePosition = source.relativePosition;
			}

			private RelativePosition relativePosition;
			private LineShapeBase owner = null;
		}


		/// <summary>
		/// Class for vertex control points. 
		/// Implements type specific copy and clone methods.
		/// </summary>
		protected class VertexControlPoint : AbsoluteLineControlPoint
		{
			/// <ToBeCompleted></ToBeCompleted>
			public VertexControlPoint(AbsoluteLineControlPoint source) : base(source)
			{
			}

			/// <ToBeCompleted></ToBeCompleted>
			public VertexControlPoint(ControlPointId pointId, Point position) : base(pointId, position)
			{
			}

			/// <ToBeCompleted></ToBeCompleted>
			public VertexControlPoint(ControlPointId pointId, int x, int y) : base(pointId, x, y)
			{
			}

			/// <ToBeCompleted></ToBeCompleted>
			public override LineControlPoint Clone()
			{
				return new VertexControlPoint(this);
			}

			/// <ToBeCompleted></ToBeCompleted>
			public override void CopyFrom(LineControlPoint source)
			{
				if (source is AbsoluteLineControlPoint)
					// CopyCore does the same but should be faster
					base.CopyCore((AbsoluteLineControlPoint) source);
				else {
					this.Id = source.Id;
					this.SetPosition(source.GetPosition());
				}
			}
		}


		/// <summary>
		/// Class for dnamic control points.
		/// Implements type specific copy and clone methods.
		/// </summary>
		protected class DynamicConnectionPoint : RelativeLineControlPoint
		{
			/// <ToBeCompleted></ToBeCompleted>
			public DynamicConnectionPoint(RelativeLineControlPoint source)
				: base(source)
			{
			}

			/// <ToBeCompleted></ToBeCompleted>
			public DynamicConnectionPoint(LineShapeBase owner, ControlPointId pointId, RelativePosition position)
				: base(owner, pointId, position)
			{
			}

			/// <ToBeCompleted></ToBeCompleted>
			public override LineControlPoint Clone()
			{
				return new DynamicConnectionPoint(this);
			}

			/// <ToBeCompleted></ToBeCompleted>
			public override void CopyFrom(LineControlPoint source)
			{
				if (source is RelativeLineControlPoint)
					base.CopyCore((RelativeLineControlPoint) source);
				else {
					this.Id = source.Id;
					this.SetPosition(source.GetPosition());
				}
			}
		}

		#endregion

		#region [Private] Methods and Properties

		/// <summary>
		/// Specifies the tolerance when performing hit tests and intersection calculations
		/// </summary>
		private float CalcRange
		{
			get { return (int) Math.Ceiling(LineStyle.LineWidth/2f) + 1; }
		}


		private void DoRecalcCapPoints(ControlPointId pointId, ICapStyle capStyle, float capAngle, ref PointF[] pointBuffer)
		{
			int ptIdx = GetControlPointIndex(pointId);
			Point p = controlPoints[ptIdx].GetPosition();
			p.Offset(-X, -Y);

			//int vertexIdx = GetVertexIndex(pointId);
			//Point p = shapePoints[vertexIdx];

			// get untransfomed cap points and transform it to the start point (relative to origin of coordinates)
			ToolCache.GetCapPoints(capStyle, LineStyle, ref pointBuffer);
			TransformCapToOrigin(p.X, p.Y, capAngle, ref pointBuffer);
		}


		/// <summary>
		/// Calculate LineCap
		/// </summary>
		/// <param name="pointIndex"></param>
		/// <param name="capAngle"></param>
		/// <param name="capStyle"></param>
		/// <param name="lineStyle"></param>
		/// <param name="capBounds"></param>
		/// <param name="pointBuffer"></param>
		private void CalcCapPoints(int pointIndex, float capAngle, ICapStyle capStyle, ILineStyle lineStyle,
		                           ref Rectangle capBounds, ref PointF[] pointBuffer)
		{
			// get untransfomed shape points
			ToolCache.GetCapPoints(capStyle, lineStyle, ref pointBuffer);
			// translate, rotate and scale shapePoints
			Point p = GetControlPoint(pointIndex).GetPosition();
			TransformCapToOrigin(p.X, p.Y, capAngle, ref pointBuffer);
		}


		/// <summary>
		/// Transform CapPoints to the line end it belongs to, rotate it in the right direction and scale it according 
		/// to the LineStyle's line width (see note below)
		/// </summary>
		private void TransformCapToOrigin(int toX, int toY, float angleDeg, ref PointF[] capPoints)
		{
			Matrix.Reset();
			Matrix.Translate(toX, toY);
			Matrix.Rotate(angleDeg + 90);
			// Due to the fact that CustomCaps are automatically scaled with the LineWidth of a Pen
			// in GDI+, we scale down the the PointF arrays returned by the ToolCache in order to maintain
			// the cap size.
			// As a result of that, the CustomLineCap's PointF array have to be upscaled again for calculating
			// the bounds to invalidate and filling the interior
			Matrix.Scale(LineStyle.LineWidth, LineStyle.LineWidth);
			Matrix.TransformPoints(capPoints);
		}


		private IEnumerator<MenuItemDef> GetBaseMenuItemDefs(int mouseX, int mouseY, int range)
		{
			return base.GetMenuItemDefs(mouseX, mouseY, range).GetEnumerator();
		}


		private int GetVertexIndex(ControlPointId pointId)
		{
			int vertexIdx;
			int startIdx, stopIdx, step;
			if (IsLastVertex(pointId)) {
				startIdx = controlPoints.Count - 1;
				stopIdx = 0;
				step = -1;
				vertexIdx = VertexCount;
			}
			else {
				startIdx = 0;
				stopIdx = controlPoints.Count;
				step = 1;
				vertexIdx = -1;
			}

			for (int i = startIdx; i != stopIdx; i += step) {
				if (controlPoints[i] is VertexControlPoint) {
					vertexIdx += step;
					switch (pointId) {
						case ControlPointId.FirstVertex:
							if (IsFirstVertex(controlPoints[i].Id))
								return vertexIdx;
							break;
						case ControlPointId.LastVertex:
							if (IsLastVertex(controlPoints[i].Id))
								return vertexIdx;
							break;
						default:
							if (controlPoints[i].Id == pointId)
								return vertexIdx;
							break;
					}
				}
			}
			return -1;
		}


		private ControlPointId GetNextPointIdCore(ControlPointId pointId, ControlPointCapabilities capabilities, int step)
		{
			switch (pointId) {
				case ControlPointId.Any:
				case ControlPointId.None:
				case ControlPointId.Reference:
					return ControlPointId.None;
				default:
					int ptCnt = ControlPointCount;
					int ptIdx = GetControlPointIndex(pointId);
					do {
						ptIdx += step;
					} while (ptIdx > 0 && ptIdx < ptCnt && !HasControlPointCapability(controlPoints[ptIdx].Id, capabilities));

					if (ptIdx >= 0 && ptIdx < ptCnt)
						return GetControlPointId(ptIdx);
					else return ControlPointId.None;
			}
		}


		private void MaintainFirstVertexIndex()
		{
			for (int i = 0; i < controlPoints.Count; ++i) {
				if (controlPoints[i] is VertexControlPoint) {
					firstVertexIdx = i;
					break;
				}
			}
		}


		private void MaintainLastVertexIndex()
		{
			for (int i = controlPoints.Count - 1; i >= 0; --i) {
				if (controlPoints[i] is VertexControlPoint) {
					lastVertexIdx = i;
					break;
				}
			}
		}


		private void MaintainVertexCount()
		{
			int vtxCnt = 0;
			for (int i = controlPoints.Count - 1; i >= 0; --i)
				if (controlPoints[i] is VertexControlPoint)
					++vtxCnt;
			vertexCount = vtxCnt;
		}


		private void CopyControlPoints(Shape source)
		{
			ILinearShape srcLine = ((ILinearShape) source);
			// Maintain maximum number of points: Not more than MaxVertexCount and not more than the source has
			int maxPointCount = Math.Min(MaxVertexCount, srcLine.VertexCount);
			if (ControlPointCount > maxPointCount) {
				for (int i = ControlPointCount - 1; controlPoints.Count > srcLine.VertexCount; --i) {
					// Do not remove the first and the last vertex!
					if (i == firstVertexIdx || i == lastVertexIdx) continue;
					controlPoints.RemoveAt(i);
				}
				MaintainFirstVertexIndex();
				MaintainLastVertexIndex();
				// Set the vertex count equal to the point count as the copied points 
				// will all become vertices
				vertexCount = controlPoints.Count;
			}

			// Copy vertices
			if (srcLine.MinVertexCount <= MaxVertexCount) {
				int ownMaxVertexIdx = MaxVertexCount - 1;
				int vertexIdx = -1;
				// Try to copy all vertices of the source shape
				Point p = Point.Empty;
				for (ControlPointId vertexId = ControlPointId.FirstVertex;
				     vertexId != ControlPointId.None;
				     vertexId = srcLine.GetNextVertexId(vertexId)) {
					p = source.GetControlPointPosition(vertexId);
					++vertexIdx;
					if (vertexId == ControlPointId.FirstVertex) {
						Debug.Assert(controlPoints[vertexIdx] is VertexControlPoint);
						// Move start point
						Point vertexPos = GetControlPointPosition(ControlPointId.FirstVertex);
						MovePointByCore(ControlPointId.FirstVertex, p.X - vertexPos.X, p.Y - vertexPos.Y, ResizeModifiers.None);
					}
					else if (vertexId == ControlPointId.LastVertex) {
						Debug.Assert(controlPoints[vertexIdx] is VertexControlPoint);
						// Move end point
						Point vertexPos = GetControlPointPosition(ControlPointId.LastVertex);
						MovePointByCore(ControlPointId.LastVertex, p.X - vertexPos.X, p.Y - vertexPos.Y, ResizeModifiers.None);
					}
					else if (vertexIdx >= ownMaxVertexIdx) {
						// If the line's maximum vertex count is reached, skip all vertices but the source's last vertex
						continue;
					}
					else if (VertexCount < srcLine.VertexCount) {
						// If the destination shape has not enough vertices, insert a new vertex
						InsertControlPoint(vertexIdx, new VertexControlPoint(vertexId, p));
					}
					else {
						// If there destination already has enough vertices, replace the shape's vertex
						SetControlPoint(vertexIdx, new VertexControlPoint(vertexId, p));
					}
					Debug.Assert(controlPoints[vertexIdx] is VertexControlPoint);
				}
			}
			Debug.Assert(VertexCount == ControlPointCount);

			// Copy non-vertex connection points
			foreach (ControlPointId pointId in source.GetControlPointIds(ControlPointCapabilities.All)) {
				// Skip existing points.
				// If vertices were skipped before, we try to create dynamic connection points instead.
				if (GetControlPointIndex(pointId) >= 0) continue;

				// Get a valid control point position (best effort approach)
				Point srcPtPos = source.GetControlPointPosition(pointId);
				srcPtPos = GetValidConnectionPointPosition(srcPtPos);

				// As linear shapes place their dynamic connection points differently, let the shape do the work.
				// Afterwards, the point id has to be maintained.
				int idx = GetControlPointIndex(AddConnectionPoint(srcPtPos.X, srcPtPos.Y));
				GetControlPoint(idx).Id = pointId;
			}
		}


		private void CopyControlPoints(LineShapeBase source)
		{
			if (source == null) throw new ArgumentNullException("source");
			// Ensure that the number of control points matches the source
			if (controlPoints.Count > source.ControlPointCount)
				controlPoints.RemoveRange(source.ControlPointCount, controlPoints.Count - source.ControlPointCount);

			// Copy at least first and last vertex. Do not exceed MaxVertexCount!
			int verticesToCopy = MaxVertexCount;
			for (int i = 0; i < source.ControlPointCount; ++i) {
				// Copy point
				LineControlPoint dstPoint = null;
				CopyControlPoint(source, source.controlPoints[i], ref dstPoint, verticesToCopy > 0);
				if (dstPoint is VertexControlPoint) --verticesToCopy;
				// Assign copied point
				if (i < controlPoints.Count) controlPoints[i] = dstPoint;
				else controlPoints.Add(dstPoint);
			}
			firstVertexIdx = source.firstVertexIdx;
			lastVertexIdx = source.lastVertexIdx;
			vertexCount = Math.Min(MaxVertexCount, source.vertexCount);
#if DEBUG_DIAGNOSTICS
			int vtxCnt = VertexCount;
			MaintainVertexCount();
			Debug.Assert(VertexCount <= MaxVertexCount && vtxCnt == VertexCount);
#endif
		}


		private void CopyControlPoint(Shape source, LineControlPoint srcPoint, ref LineControlPoint dstPoint,
		                              bool copyVertexPoint)
		{
			if (srcPoint is VertexControlPoint) {
				if (copyVertexPoint) {
					if (dstPoint is VertexControlPoint)
						dstPoint.CopyFrom(srcPoint);
					else dstPoint = srcPoint.Clone();
				}
				else dstPoint = CreateDynamicConnectionPoint(srcPoint);
			}
			else if (srcPoint is DynamicConnectionPoint) {
				if (source.Type == this.Type) {
					if (dstPoint is DynamicConnectionPoint)
						((DynamicConnectionPoint) dstPoint).RelativePosition = ((DynamicConnectionPoint) srcPoint).RelativePosition;
					else
						dstPoint = new DynamicConnectionPoint(this, srcPoint.Id, ((DynamicConnectionPoint) srcPoint).RelativePosition);
				}
				else dstPoint = CreateDynamicConnectionPoint(srcPoint);
			}
			else {
				Debug.Fail("Unknown control point type!");
				dstPoint = CreateDynamicConnectionPoint(srcPoint);
			}
		}


		private DynamicConnectionPoint CreateDynamicConnectionPoint(LineControlPoint sourceControlPoint)
		{
			Point srcPtPos = sourceControlPoint.GetPosition();
			srcPtPos = GetValidConnectionPointPosition(srcPtPos);
			// Create a dynamic connection point from the calculated relative position
			RelativePosition relPos = CalculateRelativePosition(srcPtPos.X, srcPtPos.Y);
			return new DynamicConnectionPoint(this, sourceControlPoint.Id, relPos);
		}


		private Dictionary<ControlPointId, Point> StoreDynControlPointPositions()
		{
			// Store all positions of dynamic control points and re-calculate 
			// them after removing the control point
			Dictionary<ControlPointId, Point> dynCtrlPointPositions = null;
			for (int i = ControlPointCount - 1; i >= 0; --i) {
				LineControlPoint ctrlPoint = GetControlPoint(i);
				if (ctrlPoint is DynamicConnectionPoint) {
					if (dynCtrlPointPositions == null) dynCtrlPointPositions = new Dictionary<ControlPointId, Point>();
					dynCtrlPointPositions.Add(ctrlPoint.Id, ctrlPoint.GetPosition());
				}
			}
			return dynCtrlPointPositions;
		}


		private void RestoreDynControlPointPositions(Dictionary<ControlPointId, Point> dynControlPointPositions)
		{
			foreach (KeyValuePair<ControlPointId, Point> item in dynControlPointPositions) {
				// Ensure that the point's position is on the line
				Point newPos = CalculateConnectionFoot(item.Value.X, item.Value.Y);
				Debug.Assert(Geometry.IsValid(newPos));
				int idx = GetControlPointIndex(item.Key);
				controlPoints[idx].SetPosition(newPos);
			}
		}


		private Point GetValidConnectionPointPosition(Point p)
		{
			// If the source's control point is not on this shape's outline, 
			// try to do a best effort approach
			if (!ContainsPoint(p.X, p.Y))
				p = CalculateConnectionFoot(p.X, p.Y);
			// If the best effort failed too, use the shape's location in order to ensure that the 
			// control point can be created
			if (!ContainsPoint(p.X, p.Y))
				p.X = X;
			p.Y = Y;
			return p;
		}

		#endregion

		#region [Private] Types - ControlPointId Enumerator

		private struct Enumerator : IEnumerable<ControlPointId>, IEnumerator<ControlPointId>, IEnumerator
		{
			public static Enumerator Create(LineShapeBase shape, ControlPointCapabilities flags)
			{
				Debug.Assert(shape != null);
				Enumerator result;
				result.shape = shape;
				result.flags = flags;
				// We use Reference id as the start value: RefId -> FirstVertex -> Id's -> LastVertex -> None
				result.currentId = ControlPointId.Reference;
				result.ctrlPointCnt = shape.ControlPointCount;
				return result;
			}

			#region IEnumerable<ControlPointId> Members

			public IEnumerator<ControlPointId> GetEnumerator()
			{
				return this;
			}

			#endregion

			#region IEnumerable Members

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this;
			}

			#endregion

			#region IEnumerator<ControlPointId> Members

			public bool MoveNext()
			{
				bool result = false;
				do {
					if (currentId == ControlPointId.Reference)
						currentId = ControlPointId.FirstVertex;
					else currentId = shape.GetNextPointIdCore(currentId, ControlPointCapabilities.All, +1);

					if (currentId == ControlPointId.None)
						return false;
					if (shape.HasControlPointCapability(currentId, flags))
						result = true;
				} while (result == false);
				return result;
			}


			public void Reset()
			{
				ctrlPointCnt = shape.ControlPointCount;
				currentId = 1;
			}


			ControlPointId IEnumerator<ControlPointId>.Current
			{
				get
				{
					if (currentId == ControlPointId.None)
						throw new InvalidOperationException("ControlPointId.None is not a valid ControlPointId for iterating.");
					return currentId;
					//int result = currentId;
					//currentId = shape.GetNextPointIdCore(currentId, ControlPointCapabilities.All, +1);
					//return result;
				}
			}

			#endregion

			#region IEnumerator Members

			public object Current
			{
				get { return (IEnumerator<int>) this.Current; }
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				this.flags = 0;
				this.currentId = ControlPointId.None;
				this.ctrlPointCnt = 0;
				this.shape = null;
			}

			#endregion

			#region Fields

			private LineShapeBase shape;
			private ControlPointCapabilities flags;
			private ControlPointId currentId;
			private int ctrlPointCnt;

			#endregion
		}

		#endregion

		#region Fields

		/// <ToBeCompleted></ToBeCompleted>
		protected const int PropertyIdStartCapStyle = 7;

		/// <ToBeCompleted></ToBeCompleted>
		protected const int PropertyIdEndCapStyle = 8;

		/// <summary>Array of points used for drawing. Transformation (for drawing) in TransformDrawCache is handled in this class</summary>
		protected Point[] shapePoints = null;

		private static readonly string attrNameVertices = "Vertices";
		private static readonly string[] vertexAttrNames = new string[] {"PointIndex", "PointId", "X", "Y"};
		private static readonly Type[] vertexAttrTypes = new Type[] {typeof (int), typeof (int), typeof (int), typeof (int)};
		private static readonly string attrNameConnectionPoints = "ConnectionPoints";
		private static readonly string[] connectionPointAttrNames = new string[] {"PointIndex", "PointId", "A", "B", "C"};

		private static readonly Type[] connectionPointAttrTypes = new Type[]
		                                                          	{
		                                                          		typeof (int), typeof (int), typeof (int), typeof (int),
		                                                          		typeof (int)
		                                                          	};

		// List of control points (absolute and relative).
		private List<LineControlPoint> controlPoints = null;
		private int vertexCount = 0;
		private int firstVertexIdx = 0;
		private int lastVertexIdx = 1;

		// Styles
		private ICapStyle privateStartCapStyle = null;
		private ICapStyle privateEndCapStyle = null;

		// drawing stuff
		private float startCapAngle = float.NaN;
		private float endCapAngle = float.NaN;
		private Rectangle startCapBounds = Rectangle.Empty; // Rectangle for invalidating the line caps
		private Rectangle endCapBounds = Rectangle.Empty; // Rectangle for invalidating the line caps

		private PointF[] startCapPointBuffer = null;
		                 // buffer for the startCap - used for drawing and hit- / intersection testing

		private PointF[] endCapPointBuffer = null;
		                 // buffer for the startCap - used for drawing and hit- / intersection testing

		//private Matrix matrix = new Matrix();

		#endregion
	}
}