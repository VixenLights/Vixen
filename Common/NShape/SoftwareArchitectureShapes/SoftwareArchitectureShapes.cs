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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Reflection;

using Dataweb.NShape.Advanced;
using Dataweb.NShape.Commands;


namespace Dataweb.NShape.SoftwareArchitectureShapes {

	#region Commands for editing EntitySymbol columns

	public abstract class EntityShapeColumnCommand : Command {

		protected EntityShapeColumnCommand(EntitySymbol shape, string columnText)
			: base() {
			this.shape = shape;
			this.columnText = columnText;
		}


		protected Shape Shape {
			get { return shape; }
		}
		
		protected string ColumnText {
			get { return columnText; }
		}


		public override Permission RequiredPermission {
			get { return Permission.Data; }
		}


		/// <override></override>
		protected override bool CheckAllowedCore(ISecurityManager securityManager, bool createException, out Exception exception) {
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			bool isGranted = securityManager.IsGranted(RequiredPermission, shape.SecurityDomainName);
			exception = (!isGranted && createException) ? new NShapeSecurityException(this) : null;
			return isGranted;
		}


		protected EntitySymbol shape;
		private string columnText;
	}


	public class AddColumnCommand : EntityShapeColumnCommand {

		public AddColumnCommand(EntitySymbol shape, string columnText)
			: base(shape, columnText) {
			base.description = string.Format("Add column to {0}", shape.Type.Name);
		}
		
		
		#region ICommand Members

		/// <override></override>
		public override void Execute() {
			shape.AddColumn(ColumnText);
			if (Repository != null) Repository.Update(Shape);
		}


		/// <override></override>
		public override void Revert() {
			shape.RemoveColumn(ColumnText);
			if (Repository != null) Repository.Update(Shape);
		}

		#endregion

	}


	public class InsertColumnCommand : EntityShapeColumnCommand {
		
		public InsertColumnCommand(EntitySymbol shape, int beforeColumnIndex, string columnText)
			: base(shape, columnText) {
			base.description = string.Format("Insert new column in {0}", shape.Type.Name);
			this.beforeIndex = beforeColumnIndex;
		}


		#region ICommand Members

		/// <override></override>
		public override void Execute() {
			shape.AddColumn(shape.GetCaptionText(shape.CaptionCount - 1));
			for (int i = shape.CaptionCount - 2; i > beforeIndex; --i)
				shape.SetCaptionText(i, shape.GetCaptionText(i-1));
			shape.SetCaptionText(beforeIndex, ColumnText);
			if (Repository != null) Repository.Update(Shape);
		}


		/// <override></override>
		public override void Revert() {
			for (int i = shape.CaptionCount - 1; i > beforeIndex; --i)
				shape.SetCaptionText(i - 1, shape.GetCaptionText(i));
			// The shape's Text does count as caption but not as column, that's why CaptionCount-2.
			shape.RemoveColumnAt(shape.CaptionCount - 2);
			if (Repository != null) Repository.Update(Shape);
		}

		#endregion


		private int beforeIndex;
	}


	public class EditColumnCommand : EntityShapeColumnCommand {

		public EditColumnCommand(EntitySymbol shape, int columnIndex, string columnText)
			: base(shape, columnText) {
			base.description = string.Format("Edit column '{0}' in {1}", columnText, shape.Type.Name);
			this.oldColumnText = shape.ColumnNames[columnIndex];
			this.columnIndex = columnIndex;
		}


		#region ICommand Members

		/// <override></override>
		public override void Execute() {
			string[] columns = new string[shape.ColumnNames.Length];
			Array.Copy(shape.ColumnNames, columns, shape.ColumnNames.Length);
			columns[columnIndex] = ColumnText;

			shape.ColumnNames = columns;
			if (Repository != null) Repository.Update(Shape);
		}


		/// <override></override>
		public override void Revert() {
			string[] columns = new string[shape.ColumnNames.Length];
			Array.Copy(shape.ColumnNames, columns, shape.ColumnNames.Length);
			columns[columnIndex] = oldColumnText;

			shape.ColumnNames = columns;
			if (Repository != null) Repository.Update(Shape);
		}

		#endregion


		private string oldColumnText;
		private int columnIndex;
	}


	public class RemoveColumnCommand : EntityShapeColumnCommand {

		public RemoveColumnCommand(EntitySymbol shape, int removeColumnIndex, string columnText)
			: base(shape, columnText) {
			base.description = string.Format("Remove column '{0}' from {1}", columnText, shape.Type.Name);
			this.removeIndex = removeColumnIndex;
		}


		#region ICommand Members

		/// <override></override>
		public override void Execute() {
			int maxCaptionIdx = shape.CaptionCount - 1;
			for (int i = removeIndex; i < maxCaptionIdx; ++i)
				shape.SetCaptionText(i, shape.GetCaptionText(i + 1));
			// The shape's Text does count as caption but not as column, that's why maxCaptionIdx - 1.
			shape.RemoveColumnAt(maxCaptionIdx - 1);
			if (Repository != null) Repository.Update(Shape);
		}


		/// <override></override>
		public override void Revert() {
			shape.AddColumn(shape.GetCaptionText(shape.CaptionCount - 1));
			for (int i = shape.CaptionCount - 2; i > removeIndex; --i)
				shape.SetCaptionText(i, shape.GetCaptionText(i - 1));
			shape.SetCaptionText(removeIndex, ColumnText);
			if (Repository != null) Repository.Update(Shape);
		}

		#endregion


		private int removeIndex;
	}

	#endregion


	public class EntitySymbol : RectangleBase {

		/// <override></override>
		public override void CopyFrom(Shape source) {
			base.CopyFrom(source);
			// Copy default styles for captions before copying the caption's properties
			if (source is EntitySymbol) {
				EntitySymbol src = (EntitySymbol)source;
				this.ColumnBackgroundColorStyle = src.ColumnBackgroundColorStyle;
				this.ColumnCharacterStyle = src.ColumnCharacterStyle;
				this.ColumnParagraphStyle = src.ColumnParagraphStyle;
			}
			// Copy captions via the ICaptionedShape interface
			if (source is ICaptionedShape) {
				ClearColumns();
				ICaptionedShape src = (ICaptionedShape)source;
				for (int i = 1; i < src.CaptionCount; ++i) {
					if (CaptionCount < src.CaptionCount)
						AddColumn(src.GetCaptionText(i));
					else SetCaptionText(i, src.GetCaptionText(i));
					SetCaptionCharacterStyle(i, src.GetCaptionCharacterStyle(i));
					SetCaptionParagraphStyle(i, src.GetCaptionParagraphStyle(i));
				}
			}
		}


		/// <override></override>
		public override Shape Clone() {
			Shape result = new EntitySymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		public override void MakePreview(IStyleSet styleSet) {
			base.MakePreview(styleSet);
			privateColumnCharacterStyle = styleSet.GetPreviewStyle(ColumnCharacterStyle);
			privateColumnParagraphStyle = styleSet.GetPreviewStyle(ColumnParagraphStyle);
			privateColumnBackgroundColorStyle = styleSet.GetPreviewStyle(ColumnBackgroundColorStyle);
		}


		/// <override></override>
		public override bool HasStyle(IStyle style) {
			if (IsStyleAffected(ColumnBackgroundColorStyle, style) 
				|| IsStyleAffected(ColumnCharacterStyle, style) 
				|| IsStyleAffected(ColumnParagraphStyle, style))
				return true;
			else return base.HasStyle(style);
		}


		#region IEntity

		/// <override></override>
		protected override void SaveFieldsCore(IRepositoryWriter writer, int version) {
			base.SaveFieldsCore(writer, version);
			writer.WriteStyle(ColumnBackgroundColorStyle);
			writer.WriteStyle(ColumnCharacterStyle);
			writer.WriteStyle(ColumnParagraphStyle);
			writer.WriteInt32(ColumnNames.Length);
		}


		/// <override></override>
		protected override void LoadFieldsCore(IRepositoryReader reader, int version) {
			base.LoadFieldsCore(reader, version);
			ColumnBackgroundColorStyle = reader.ReadColorStyle();
			ColumnCharacterStyle = reader.ReadCharacterStyle();
			ColumnParagraphStyle = reader.ReadParagraphStyle();
			int colCnt = reader.ReadInt32();
			if (columnNames == null) columnNames = new string[colCnt];
			else Array.Resize(ref columnNames, colCnt);
		}


		/// <override></override>
		protected override void SaveInnerObjectsCore(string propertyName, IRepositoryWriter writer, int version) {
			if (propertyName == attrNameColumns) {
				writer.BeginWriteInnerObjects();
				int cnt = CaptionCount;
				for (int i = 1; i < cnt; ++i) {	// Skip first caption (title)
					writer.BeginWriteInnerObject();
					writer.WriteInt32(i - 1);
					writer.WriteString(GetCaptionText(i));
					writer.EndWriteInnerObject();
				}
				writer.EndWriteInnerObjects();
			} else base.SaveInnerObjectsCore(propertyName, writer, version);
		}


		/// <override></override>
		protected override void LoadInnerObjectsCore(string propertyName, IRepositoryReader reader, int version) {
			if (propertyName == attrNameColumns) {
				reader.BeginReadInnerObjects();
				while (reader.BeginReadInnerObject()) {
					int colIdx = reader.ReadInt32();
					string colName = reader.ReadString();
					reader.EndReadInnerObject();
					InsertColumn(colIdx, colName);
				}
				reader.EndReadInnerObjects();
			} else base.LoadInnerObjectsCore(propertyName, reader, version);
		}


		/// <summary>
		/// Retrieves the persistable properties of <see cref="T:Dataweb.NShape.SoftwareArchitectureShapes.EntitySymbol" />.
		/// </summary>
		public static new IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version) {
			foreach (EntityPropertyDefinition pi in RectangleBase.GetPropertyDefinitions(version))
				yield return pi;
			yield return new EntityFieldDefinition("ColumnBackgroundColorStyle", typeof(object));
			yield return new EntityFieldDefinition("ColumnCharacterStyle", typeof(object));
			yield return new EntityFieldDefinition("ColumnParagraphStyle", typeof(object));
			yield return new EntityFieldDefinition("ColumnCount", typeof(int));
			yield return new EntityInnerObjectsDefinition(attrNameColumns, attrNameColumn, columnAttrNames, columnAttrTypes);
		}

		#endregion


		#region ICaptionedShape

		/// <override></override>
		public override int CaptionCount { get { return base.CaptionCount + columnCaptions.Count; } }


		/// <override></override>
		public override bool GetCaptionBounds(int index, out Point topLeft, out Point topRight, out Point bottomRight, out Point bottomLeft) {
			if (index < base.CaptionCount)
				return base.GetCaptionBounds(index, out topLeft, out topRight, out bottomRight, out bottomLeft);
			else {
				int idx = index - 1;
				Rectangle captionBounds;
				CalcCaptionBounds(index, out captionBounds);
				Geometry.TransformRectangle(Center, Angle, captionBounds, out topLeft, out topRight, out bottomRight, out bottomLeft);
				return (Geometry.ConvexPolygonContainsPoint(columnFrame, bottomLeft.X, bottomLeft.Y)
					&& Geometry.ConvexPolygonContainsPoint(columnFrame, bottomRight.X, bottomRight.Y));
			}
		}


		/// <override></override>
		public override bool GetCaptionTextBounds(int index, out Point topLeft, out Point topRight, out Point bottomRight, out Point bottomLeft) {
			if (index < base.CaptionCount) {
				return base.GetCaptionTextBounds(index, out topLeft, out topRight, out bottomRight, out bottomLeft);
			} else {
				int idx = index - 1;
				Rectangle bounds;
				CalcCaptionBounds(index, out bounds);
				bounds = columnCaptions[idx].CalculateTextBounds(bounds, ColumnCharacterStyle, ColumnParagraphStyle, DisplayService);
				Geometry.TransformRectangle(Center, Angle, bounds, out topLeft, out topRight, out bottomRight, out bottomLeft);
				return (Geometry.QuadrangleContainsPoint(columnFrame[0], columnFrame[1], columnFrame[2], columnFrame[3], topLeft.X, topLeft.Y)
					&& Geometry.QuadrangleContainsPoint(columnFrame[0], columnFrame[1], columnFrame[2], columnFrame[3], bottomRight.X, bottomRight.Y));
			}
		}


		/// <override></override>
		public override string GetCaptionText(int index) {
			if (index < base.CaptionCount)
				return base.GetCaptionText(index);
			else
				return columnCaptions[index - 1].Text;
		}


		/// <override></override>
		public override ICharacterStyle GetCaptionCharacterStyle(int index) {
			if (index < base.CaptionCount)
				return base.GetCaptionCharacterStyle(index);
			else return ColumnCharacterStyle;
		}


		/// <override></override>
		public override IParagraphStyle GetCaptionParagraphStyle(int index) {
			if (index < base.CaptionCount)
				return base.GetCaptionParagraphStyle(index);
			else return ColumnParagraphStyle;
		}


		/// <override></override>
		public override void SetCaptionText(int index, string text) {
			if (index < base.CaptionCount)
				base.SetCaptionText(index, text);
			else {
				Invalidate();
				columnCaptions[index - 1].Text = text;
				InvalidateDrawCache();
				Invalidate();
			}
		}


		/// <override></override>
		public override void SetCaptionCharacterStyle(int index, ICharacterStyle characterStyle) {
			if (index < base.CaptionCount)
				base.SetCaptionCharacterStyle(index, characterStyle);
			else {
				int idx = index - 1;
				// Create if needed
				if (columnCharacterStyles == null)
					columnCharacterStyles = new SortedList<int, ICharacterStyle>(1);
				// Set private style for a single caption
				if (characterStyle != ColumnCharacterStyle) {
					if (!columnCharacterStyles.ContainsKey(idx))
						columnCharacterStyles.Add(idx, characterStyle);
					else columnCharacterStyles[idx] = characterStyle;
				} else {
					if (columnCharacterStyles != null) {
						if (columnCharacterStyles.ContainsKey(idx))
							columnCharacterStyles.Remove(idx);
						// Delete if not needed any more
						if (columnCharacterStyles.Count == 0)
							columnCharacterStyles = null;
					}
				}
			}
		}


		/// <override></override>
		public override void SetCaptionParagraphStyle(int index, IParagraphStyle paragraphStyle) {
			if (index < base.CaptionCount)
				base.SetCaptionParagraphStyle(index, paragraphStyle);
			else {
				int idx = index - 1;
				// Create if needed
				if (columnParagraphStyles == null) 
					columnParagraphStyles = new SortedList<int, IParagraphStyle>(1);
				// Set private style for a single caption
				if (paragraphStyle != ColumnParagraphStyle) {
					if (!columnParagraphStyles.ContainsKey(idx))
						columnParagraphStyles.Add(idx, paragraphStyle);
					else columnParagraphStyles[idx] = paragraphStyle;
				} else {
					if (columnParagraphStyles != null) {
						if (columnParagraphStyles.ContainsKey(idx))
							columnParagraphStyles.Remove(idx);
						// Delete if not needed any longer
						if (columnParagraphStyles.Count == 0)
							columnParagraphStyles = null;
					}
				}
			}
		}

		#endregion


		#region Properties
		[Category("Appearance")]
		[Description("Defines the appearence of the shape's interior.\nUse the template editor to modify all shapes of a template.\nUse the design editor to modify and create styles.")]
		[PropertyMappingId(PropertyIdColumnBackgroundColorStyle)]
		[RequiredPermission(Permission.Present)]
		public virtual IColorStyle ColumnBackgroundColorStyle {
			get { return privateColumnBackgroundColorStyle ?? ((EntitySymbol)Template.Shape).ColumnBackgroundColorStyle; }
			set {
				privateColumnBackgroundColorStyle = (Template != null && Template.Shape is EntitySymbol && value == ((EntitySymbol)Template.Shape).ColumnBackgroundColorStyle) ? null : value;
				Invalidate();
			}
		}


		[Category("Text Appearance")]
		[Description("Determines the style of the shape's column names.\nUse the template editor to modify all shapes of a template.\nUse the design editor to modify and create styles.")]
		[PropertyMappingId(PropertyIdColumnCharacterStyle)]
		[RequiredPermission(Permission.Present)]
		public ICharacterStyle ColumnCharacterStyle {
			get { return privateColumnCharacterStyle ?? ((EntitySymbol)Template.Shape).ColumnCharacterStyle; }
			set {
				Invalidate();
				privateColumnCharacterStyle = (Template != null && Template.Shape is EntitySymbol && value == ((EntitySymbol)Template.Shape).ColumnCharacterStyle) ? null : value;
				InvalidateDrawCache();
				Invalidate();
			}
		}


		[Category("Text Appearance")]
		[Description("Determines the layout of the shape's column names.\nUse the template editor to modify all shapes of a template.\nUse the design editor to modify and create styles.")]
		[PropertyMappingId(PropertyIdColumnParagraphStyle)]
		[RequiredPermission(Permission.Present)]
		public IParagraphStyle ColumnParagraphStyle {
			get { return privateColumnParagraphStyle ?? ((EntitySymbol)Template.Shape).ColumnParagraphStyle;
			}
			set {
				Invalidate();
				privateColumnParagraphStyle = (Template != null && Template.Shape is EntitySymbol && value == ((EntitySymbol)Template.Shape).ColumnParagraphStyle) ? null : value;
				InvalidateDrawCache();
				Invalidate();
			}
		}


		[Category("Text Layout")]
		[Description("The column names of this table.")]
		[RequiredPermission(Permission.Present)]
		[TypeConverter("Dataweb.NShape.WinFormsUI.TextTypeConverter")]
		[Editor("Dataweb.NShape.WinFormsUI.TextUITypeEditor", typeof(UITypeEditor))]
		public string[] ColumnNames {
			get {
				if (columnNames == null || columnNames.Length != columnCaptions.Count)
					columnNames = new string[columnCaptions.Count];
				for (int i = columnCaptions.Count - 1; i >= 0; --i) {
					if (columnNames[i] != columnCaptions[i].Text)
						columnNames[i] = columnCaptions[i].Text;
				}
				return columnNames; 
			}
			set {
				if (value == null) throw new ArgumentNullException();
				Invalidate();

				// Remove columns that are no longer needed
				int valueCnt = value.Length;
				if (columnNames.Length > valueCnt) {
					for (int i = columnNames.Length - 1; i >= valueCnt; --i)
						RemoveColumnAt(i);
				}
				// Replace existing and add new columns
				for (int i = 0; i < valueCnt; ++i) {
					if (i < columnNames.Length)
						columnNames[i] = value[i];
					else AddColumn(value[i]);
				}

				InvalidateDrawCache();
				Invalidate();
			}
		}
		#endregion


		#region Caption objects stuff

		public void AddColumn(string columnName) {
			columnCaptions.Add(new Caption(columnName));
			Array.Resize(ref columnControlPoints, columnControlPoints.Length + 2);
			Array.Resize(ref columnNames, columnNames.Length + 1);
			columnNames[columnNames.Length - 1] = columnName;
			InvalidateDrawCache();
			Invalidate();
		}


		public void InsertColumn(int index, string columnName) {
			columnCaptions.Insert(index, new Caption(columnName));
			Array.Resize(ref columnControlPoints, columnControlPoints.Length + 2);
			Array.Resize(ref columnNames, columnCaptions.Count);
			for (int i = columnCaptions.Count - 1; i >= 0; --i)
				columnNames[i] = columnCaptions[i].Text;
			InvalidateDrawCache();
			Invalidate();
		}


		public void RemoveColumn(string columnName) {
			int idx = -1;
			for (int i = 0; i < columnCaptions.Count; ++i) {
				if (columnName.Equals(columnCaptions[i].Text, StringComparison.InvariantCulture)) {
					idx = i;
					break;
				}
			}
			RemoveColumnAt(idx);
			InvalidateDrawCache();
			Invalidate();
		}


		public void RemoveColumnAt(int index) {
			if (index < 0 || index > columnCaptions.Count)
				throw new IndexOutOfRangeException();

			ControlPointId id;
			id = GetControlPointId(base.ControlPointCount + (2 * index));
			foreach (ShapeConnectionInfo sci in GetConnectionInfos(id, null))
				DetachGluePointFromConnectionPoint(id, sci.OtherShape, sci.OtherPointId);
			id = GetControlPointId(base.ControlPointCount + (2 * index) + 1);
			foreach (ShapeConnectionInfo sci in GetConnectionInfos(id, null))
				DetachGluePointFromConnectionPoint(id, sci.OtherShape, sci.OtherPointId);

			// Remove caption
			columnCaptions.RemoveAt(index);
			if (index < columnControlPoints.Length - 2)
				Array.Copy(columnControlPoints, index + 2, columnControlPoints, index, columnControlPoints.Length - index - 2);
			Array.Resize(ref columnControlPoints, columnControlPoints.Length - 2);
			if (index < columnNames.Length - 1)
				Array.Copy(columnNames, index + 1, columnNames, index, columnNames.Length - index - 1);
			Array.Resize(ref columnNames, columnCaptions.Count);
			InvalidateDrawCache();
			Invalidate();
		}


		public void ClearColumns() {
			columnCaptions.Clear();
			Array.Resize<Point>(ref columnControlPoints, 0);
			Array.Resize(ref columnNames, 0);
			InvalidateDrawCache();
			Invalidate();
		}

		#endregion


		/// <override></override>
		public override IEnumerable<MenuItemDef> GetMenuItemDefs(int mouseX, int mouseY, int range) {
			// return actions of base class
			IEnumerator<MenuItemDef> enumerator = GetBaseActions(mouseX, mouseY, range);
			while (enumerator.MoveNext()) yield return enumerator.Current;
			// return own actions

			string newColumnTxt = string.Format("Column {0}", CaptionCount);
			int captionIdx = -1;
			if (ContainsPoint(mouseX, mouseY)) {
				Point tl, tr, bl, br;
				for (int i = columnCaptions.Count - 1; i >= 0; --i) {
					// +1 because Text Property is Caption '0'
					GetCaptionBounds(i + 1, out tl, out tr, out br, out bl);
					if (Geometry.QuadrangleContainsPoint(tl, tr, br, bl, mouseX, mouseY)) {
						// +1 because Text Property is Caption '0'
						captionIdx = i + 1;
						break;
					}
				}
			}

			yield return new CommandMenuItemDef("Append Column", null, string.Empty, true,
				new AddColumnCommand(this, newColumnTxt));

			bool isFeasible = captionIdx > 0;
			string description = "No caption clicked.";
			string columnName = (captionIdx > 0) ? GetCaptionText(captionIdx) : null;
			yield return new CommandMenuItemDef(string.Format("Insert Column{0}", (captionIdx > 0) ? string.Format(" before '{0}'", columnName) : ""),
				null, description, isFeasible, isFeasible ? new InsertColumnCommand(this, captionIdx, newColumnTxt) : null);
			yield return new CommandMenuItemDef(string.Format("Remove Column{0}", (captionIdx > 0) ? string.Format(" '{0}'", GetCaptionText(captionIdx)) : ""),
				null, description, isFeasible, isFeasible ? new RemoveColumnCommand(this, captionIdx, columnCaptions[captionIdx - 1].Text) : null);
		}


		/// <override></override>
		public override Point CalculateConnectionFoot(int startX, int startY) {
			// First, calculate intersection point with rectangle
			Point result = base.CalculateConnectionFoot(startX, startY);
			// Then, check result for intersection with one of the rounded corners
			if (Geometry.IsValid(result)) {
				// Check the top and bottom side (between the rounded corners:
				// If the line intersects with any of these sides, we need not calculate the rounded corner intersection
				int cornerRadius = CalcCornerRadius();
				float angleDeg = Geometry.TenthsOfDegreeToDegrees(Angle);
				
				// If there is no intersection with any of the straight sides, check the rounded corners:
				if (!Geometry.RectangleIntersectsWithLine(X, Y, Width - (2 * cornerRadius), Height, angleDeg, startX, startY, result.X, result.Y, true)
					&& !Geometry.RectangleIntersectsWithLine(X, Y, Width, Height - (2 * cornerRadius), angleDeg, startX, startY, result.X, result.Y, true)) {
					// Calculate all center points of all corner roundings
					PointF topLeft = PointF.Empty, topRight = PointF.Empty, bottomRight = PointF.Empty, bottomLeft = PointF.Empty;
					RectangleF rect = RectangleF.Empty;
					rect.X = X - (Width / 2f);
					rect.Y = Y - (Height / 2f);
					rect.Width = Width;
					rect.Height = Height;
					rect.Inflate(-cornerRadius, -cornerRadius);
					Geometry.RotateRectangle(rect, X, Y, angleDeg, out topLeft, out topRight, out bottomRight, out bottomLeft);
					// Check corner roundings for intersection with the calculated line
					PointF p = Geometry.InvalidPointF;
					if (Geometry.CircleIntersectsWithLine(topLeft.X, topLeft.Y, cornerRadius, startX, startY, X, Y, false)) {
						p = Geometry.IntersectCircleWithLine(topLeft.X, topLeft.Y, cornerRadius, startX, startY, X, Y, false);
						if (Geometry.IsValid(p)) result = Point.Round(p);
					} else if (Geometry.CircleIntersectsWithLine(topRight.X, topRight.Y, cornerRadius, startX, startY, X, Y, false)) {
						p = Geometry.IntersectCircleWithLine(topRight.X, topRight.Y, cornerRadius, startX, startY, X, Y, false);
						if (Geometry.IsValid(p)) result = Point.Round(p);
					} else if (Geometry.CircleIntersectsWithLine(bottomRight.X, bottomRight.Y, cornerRadius, startX, startY, X, Y, false)) {
						p = Geometry.IntersectCircleWithLine(bottomRight.X, bottomRight.Y, cornerRadius, startX, startY, X, Y, false);
						if (Geometry.IsValid(p)) result = Point.Round(p);
					} else if (Geometry.CircleIntersectsWithLine(bottomLeft.X, bottomLeft.Y, cornerRadius, startX, startY, X, Y, false)) {
						p = Geometry.IntersectCircleWithLine(bottomLeft.X, bottomLeft.Y, cornerRadius, startX, startY, X, Y, false);
						if (Geometry.IsValid(p)) result = Point.Round(p);
					}
				}
			} else result = Center;
			return result;
		}


		/// <override></override>
		protected override int ControlPointCount { 
			get { return base.ControlPointCount + columnControlPoints.Length; } 
		}


		/// <override></override>
		public override Point GetControlPointPosition(ControlPointId controlPointId) {
			if (controlPointId <= base.ControlPointCount)
				return base.GetControlPointPosition(controlPointId);
			else {
				UpdateDrawCache();
				int idx = controlPointId - base.ControlPointCount - 1;
				return columnControlPoints[idx];
			}
		}


		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
			if (controlPointId <= base.ControlPointCount) {
				switch (controlPointId) {
					case TopLeftControlPoint:
					case TopRightControlPoint:
					case MiddleLeftControlPoint:
					case MiddleRightControlPoint:
					case BottomLeftControlPoint:
					case BottomRightControlPoint:
						return (controlPointCapability & ControlPointCapabilities.Resize) != 0;
					case TopCenterControlPoint:
					case BottomCenterControlPoint:
						return ((controlPointCapability & ControlPointCapabilities.Resize) != 0 || (controlPointCapability & ControlPointCapabilities.Connect) != 0);
					case MiddleCenterControlPoint:
						return (controlPointCapability & ControlPointCapabilities.Rotate) != 0;
					default:
						return base.HasControlPointCapability(controlPointId, controlPointCapability);
				}
			}
			else
				return (controlPointCapability & ControlPointCapabilities.Connect) != 0;
		}


		/// <override></override>
		public override void Draw(Graphics graphics) {
			base.Draw(graphics);

			Pen pen = ToolCache.GetPen(LineStyle, null, null);
			Brush columnBrush = ToolCache.GetBrush(ColumnBackgroundColorStyle);
			int cornerRadius = CalcCornerRadius();
			int headerHeight = CalcHeaderHeight();
			int columnHeight = CalcColumnHeight();

			// fill column background
			if (Height > headerHeight) {
				graphics.FillPolygon(columnBrush, columnFrame);
				graphics.DrawPolygon(pen, columnFrame);

				// draw column names
				int top = (int)Math.Round(Y - (Height / 2f));
				int bottom = (int)Math.Round(Y + (Height / 2f));
				if (columnCaptions.Count > 0) {
					for (int i = 0; i < columnCaptions.Count; ++i) {
						// draw all captions that fit into the text area. 
						if (top + headerHeight + (i * columnHeight) + columnHeight <= bottom - cornerRadius) {
							// If there are private styles for a single caption, use these
							if (columnCharacterStyles != null || columnParagraphStyles != null) {
								ICharacterStyle characterStyle = null;
								if (columnCharacterStyles != null)
									columnCharacterStyles.TryGetValue(i, out characterStyle);
								IParagraphStyle paragraphStyle = null;
								if (columnParagraphStyles != null)
									columnParagraphStyles.TryGetValue(i, out paragraphStyle);
								columnCaptions[i].Draw(graphics, characterStyle ?? ColumnCharacterStyle, paragraphStyle ?? ColumnParagraphStyle);
							} else 
								columnCaptions[i].Draw(graphics, ColumnCharacterStyle, ColumnParagraphStyle);
						} else {
							// draw ellipsis indicators
							graphics.DrawLines(pen, upperScrollArrow);
							graphics.DrawLines(pen, lowerScrollArrow);
							break;
						}
					}
				}
			}
			graphics.DrawPath(pen, Path);
		}


		/// <override></override>
		protected override void InitializeToDefault(IStyleSet styleSet) {
			base.InitializeToDefault(styleSet);
			privateColumnBackgroundColorStyle = styleSet.ColorStyles.White;
			privateColumnCharacterStyle = styleSet.CharacterStyles.Caption;
			privateColumnParagraphStyle = styleSet.ParagraphStyles.Label;
			Width = 80;
			Height = 120;
			Text = "Table";
		}


		protected internal EntitySymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		/// <override></override>
		protected override bool IsConnectionPointEnabled(ControlPointId pointId) {
			if (pointId <= base.ControlPointCount)
				return base.IsConnectionPointEnabled(pointId);
			else
				return true;
		}


		/// <override></override>
		protected override bool MovePointByCore(ControlPointId pointId, float transformedDeltaX, float transformedDeltaY, float sin, float cos, ResizeModifiers modifiers) {
			bool result = base.MovePointByCore(pointId, transformedDeltaX, transformedDeltaY, sin, cos, modifiers);
			//for (int id = base.ControlPointCount + 1; id <= ControlPointCount; ++id) {
			//   ControlPointHasMoved(id);
			//}
			return result;
		}


		/// <override></override>
		protected override void CalcControlPoints() {
			base.CalcControlPoints();

			int left = (int)Math.Round(-Width / 2f);
			int top = (int)Math.Round(-Height / 2f);
			int right = left + Width;
			int bottom = top + Height;

			int headerHeight = CalcHeaderHeight();
			int ctrlPtCnt = base.ControlPointCount;
			if (ControlPointCount > ctrlPtCnt) {
				int halfColumnHeight = (int)Math.Round(CalcColumnHeight() / 2f);
				int y;
				for (int i = 0; i < columnControlPoints.Length; ++i) {
					if (i % 2 == 0) {
						y = top + headerHeight + (i * halfColumnHeight) + halfColumnHeight;
						if (y > bottom) y = bottom;
						columnControlPoints[i].X = left;
						columnControlPoints[i].Y = y;
					}
					else {
						y = top + headerHeight + ((i - 1) * halfColumnHeight) + halfColumnHeight;
						if (y > bottom) y = bottom;
						columnControlPoints[i].X = right;
						columnControlPoints[i].Y = y;
					}
				}
			}
		}


		/// <override></override>
		protected override void TransformDrawCache(int deltaX, int deltaY, int deltaAngle, int rotationCenterX, int rotationCenterY) {
			base.TransformDrawCache(deltaX, deltaY, deltaAngle, rotationCenterX, rotationCenterY);
			// transform ControlPoints
			if (columnControlPoints.Length > 0)
				Matrix.TransformPoints(columnControlPoints);
			// transform column frame
			Matrix.TransformPoints(columnFrame);
			// transform ellipsis indicator
			Matrix.TransformPoints(upperScrollArrow);
			Matrix.TransformPoints(lowerScrollArrow);
			// transform Column paths
			for (int i = 0; i < columnCaptions.Count; ++i)
				columnCaptions[i].TransformPath(Matrix);
		}


		/// <override></override>
		protected override void CalcCaptionBounds(int index, out Rectangle captionBounds) {
			int left = (int)Math.Round(-Width / 2f);
			int top = (int)Math.Round(-Height / 2f);
			int cornerRadius = CalcCornerRadius() + 1;
			int headerHeight = CalcHeaderHeight();
			captionBounds = Rectangle.Empty;
			captionBounds.X = left + cornerRadius;
			captionBounds.Width = Width - cornerRadius - cornerRadius;
			if (index == 0) {
				captionBounds.Y = top;
				captionBounds.Height = headerHeight;
			} else {
				int columnHeight = CalcColumnHeight();
				captionBounds.Y = top + headerHeight + ((index - 1) * columnHeight);
				captionBounds.Height = columnHeight;
			}
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				Path.Reset();
				// calculate main shape
				int left = (int)Math.Round(-Width / 2f);
				int top = (int)Math.Round(-Height / 2f);
				int right = left + Width;
				int bottom = top + Height;
				int cornerRadius = CalcCornerRadius();
				int headerHeight = CalcHeaderHeight();
				int columnHeight = CalcColumnHeight();

				rectBuffer.X = left;
				rectBuffer.Y = top;
				rectBuffer.Width = Width;
				rectBuffer.Height = Height;

				//Path.StartFigure();
				//Path.AddLine(left + cornerRadius, top, right - cornerRadius, top);
				//Path.AddArc(right - cornerRadius - cornerRadius, top, cornerRadius + cornerRadius, cornerRadius + cornerRadius, -90, 90);
				//Path.AddLine(right, top + cornerRadius, right, bottom - cornerRadius);
				//Path.AddArc(right - cornerRadius - cornerRadius, bottom - cornerRadius - cornerRadius, cornerRadius + cornerRadius, cornerRadius + cornerRadius, 0, 90);
				//Path.AddLine(right - cornerRadius, bottom, left + cornerRadius, bottom);
				//Path.AddArc(left, bottom - cornerRadius - cornerRadius, cornerRadius + cornerRadius, cornerRadius + cornerRadius, 90, 90);
				//Path.AddLine(left, bottom - cornerRadius, left, top + cornerRadius);
				//Path.AddArc(left, top, cornerRadius + cornerRadius, cornerRadius + cornerRadius, 180, 90);
				//Path.CloseFigure();
				AddRoundedRectangleFigure(Path, left, top, right, bottom, cornerRadius);

				if (Height > headerHeight) {
					int headerTop = top + headerHeight;
					int dblLineWidth = LineStyle.LineWidth + LineStyle.LineWidth;

					// column section frame lines
					columnFrame[0].X = left + cornerRadius;
					columnFrame[0].Y = headerTop;
					columnFrame[1].X = right - cornerRadius;
					columnFrame[1].Y = headerTop;
					columnFrame[2].X = right - cornerRadius;
					columnFrame[2].Y = bottom - cornerRadius;
					columnFrame[3].X = left + cornerRadius;
					columnFrame[3].Y = bottom - cornerRadius;

					Rectangle bounds = Rectangle.Empty;
					if (columnCaptions.Count > 0) {
						int colX, colY, colWidth, colHeight;
						colX = left + cornerRadius;
						colWidth = Width - cornerRadius - cornerRadius;
						colHeight = columnHeight;
						for (int i = 0; i < columnCaptions.Count; ++i) {
							// calc ColumnName text path
							colY = top + headerHeight + (i * columnHeight);
							// check if the column text is inside the column area
							if (colY + colHeight <= bottom - cornerRadius)
								columnCaptions[i].CalculatePath(colX, colY, colWidth, colHeight, ColumnCharacterStyle, ColumnParagraphStyle);
							else {
								// if not, draw an ellipsis symbol (double downward arrow)
								int offsetX = dblLineWidth + dblLineWidth + dblLineWidth;
								int offsetY = dblLineWidth + dblLineWidth;

								// calculate arrows indicating that not all columns can be drawn
								upperScrollArrow[0].X = right - cornerRadius - offsetX - offsetX;
								upperScrollArrow[0].Y = bottom - cornerRadius - offsetY - offsetY - offsetY;
								upperScrollArrow[1].X = right - cornerRadius - offsetX - (offsetX / 2);
								upperScrollArrow[1].Y = bottom - cornerRadius - offsetY - offsetY;
								upperScrollArrow[2].X = right - cornerRadius - offsetX;
								upperScrollArrow[2].Y = bottom - cornerRadius - offsetY - offsetY - offsetY;

								lowerScrollArrow[0].X = right - cornerRadius - offsetX - offsetX;
								lowerScrollArrow[0].Y = bottom - cornerRadius - offsetY - offsetY;
								lowerScrollArrow[1].X = right - cornerRadius - offsetX - (offsetX / 2);
								lowerScrollArrow[1].Y = bottom - cornerRadius - offsetY;
								lowerScrollArrow[2].X = right - cornerRadius - offsetX;
								lowerScrollArrow[2].Y = bottom - cornerRadius - offsetY - offsetY;
								break;
							}	// end of "is column in column area check"
						} // end of for loop (processing all columnName-captions)
					} // end of block (if (columnCaptions.Count > 0) )
				} // end of check "Height > ColumnHeight"
				return true;
			}
			return false;
		}


		/// <override></override>
		protected override void ProcessExecModelPropertyChange(IModelMapping propertyMapping) {
			switch (propertyMapping.ShapePropertyId) {
				case PropertyIdColumnBackgroundColorStyle:
					privateColumnBackgroundColorStyle = (propertyMapping.GetStyle() as IColorStyle);
					Invalidate();
					break;
				case PropertyIdColumnCharacterStyle:
					privateColumnCharacterStyle = (propertyMapping.GetStyle() as ICharacterStyle);
					InvalidateDrawCache();
					Invalidate();
					break;
				case PropertyIdColumnParagraphStyle:
					privateColumnParagraphStyle = (propertyMapping.GetStyle() as IParagraphStyle);
					InvalidateDrawCache();
					Invalidate();
					break;
				default:
					base.ProcessExecModelPropertyChange(propertyMapping);
					break;
			}
		}


		private IEnumerator<MenuItemDef> GetBaseActions(int mouseX, int mouseY, int range) {
			return base.GetMenuItemDefs(mouseX, mouseY, range).GetEnumerator();
		}


		private int CalcHeaderHeight() {
			int cornerRadius = CalcCornerRadius() + 1;
			Size size = Size.Empty;
			size.Width = Width - (2 * cornerRadius);
			size.Height = Height - (2 * cornerRadius);
			Size result = TextMeasurer.MeasureText(string.IsNullOrEmpty(Text) ? "Ig" : Text, ToolCache.GetFont(CharacterStyle), size, ParagraphStyle);
			result.Height += (cornerRadius + LineStyle.LineWidth + LineStyle.LineWidth);
			return result.Height;
		}


		private int CalcColumnHeight() {
			SizeF result = TextMeasurer.MeasureText("Ig", ToolCache.GetFont(ColumnCharacterStyle), Size.Empty, ColumnParagraphStyle);
			result.Height *= 1.5f;
			return (int)Math.Ceiling(result.Height);
		}


		private int CalcCornerRadius() {
			int cornerRadius = 10;
			if (Width <= 80)
				cornerRadius = (int)Math.Round((Width - 2) / 8f);
			else if (Height <= 80)
				cornerRadius = (int)Math.Round((Height - 2) / 8f);

			if (cornerRadius <= 0)
				cornerRadius = 1;
			return cornerRadius;
		}


		private void AddRoundedRectangleFigure(GraphicsPath path, int left, int top, int right, int bottom, int cornerRadius) {
			path.StartFigure();
			path.AddLine(left + cornerRadius, top, right - cornerRadius, top);
			path.AddArc(right - cornerRadius - cornerRadius, top, cornerRadius + cornerRadius, cornerRadius + cornerRadius, -90, 90);
			path.AddLine(right, top + cornerRadius, right, bottom - cornerRadius);
			path.AddArc(right - cornerRadius - cornerRadius, bottom - cornerRadius - cornerRadius, cornerRadius + cornerRadius, cornerRadius + cornerRadius, 0, 90);
			path.AddLine(right - cornerRadius, bottom, left + cornerRadius, bottom);
			path.AddArc(left, bottom - cornerRadius - cornerRadius, cornerRadius + cornerRadius, cornerRadius + cornerRadius, 90, 90);
			path.AddLine(left, bottom - cornerRadius, left, top + cornerRadius);
			path.AddArc(left, top, cornerRadius + cornerRadius, cornerRadius + cornerRadius, 180, 90);
			path.CloseFigure();
		}


		#region Fields

		protected const int PropertyIdColumnBackgroundColorStyle = 9;
		protected const int PropertyIdColumnCharacterStyle = 10;
		protected const int PropertyIdColumnParagraphStyle = 11;

		// ControlPoint Id Constants
		private const int TopLeftControlPoint = 1;
		private const int TopCenterControlPoint = 2;
		private const int TopRightControlPoint = 3;
		private const int MiddleLeftControlPoint = 4;
		private const int MiddleRightControlPoint = 5;
		private const int BottomLeftControlPoint = 6;
		private const int BottomCenterControlPoint = 7;
		private const int BottomRightControlPoint = 8;
		private const int MiddleCenterControlPoint = 9;

		private const string attrNameColumns = "TableColumns";
		private const string attrNameColumn = "Column";
		private static string[] columnAttrNames = new string[] { "ColumnIndex", "ColumnName" };
		private static Type[] columnAttrTypes = new Type[] { typeof(int), typeof(string) };

		private string[] columnNames = new string[0];
		private List<Caption> columnCaptions = new List<Caption>(0);
		private List<Rectangle> columnBounds = new List<Rectangle>(0);
		private SortedList<int, ICharacterStyle> columnCharacterStyles = null;
		private SortedList<int, IParagraphStyle> columnParagraphStyles = null;
		private Point[] columnControlPoints = new Point[0];
		private IColorStyle privateColumnBackgroundColorStyle = null;
		private ICharacterStyle privateColumnCharacterStyle = null;
		private IParagraphStyle privateColumnParagraphStyle = null;

		private Rectangle rectBuffer = Rectangle.Empty;
		//private Point[] pointBuffer = new Point[4];
		private Point[] columnFrame = new Point[4];
		private Point[] upperScrollArrow = new Point[3];
		private Point[] lowerScrollArrow = new Point[3];

		#endregion
	}


	public class AnnotationSymbol : RectangleBase {

		/// <override></override>
		protected override void InitializeToDefault(IStyleSet styleSet) {
			base.InitializeToDefault(styleSet);
			Width = 120;
			Height = 80;
			FillStyle = styleSet.FillStyles.Yellow;
		}


		/// <override></override>
		public override Shape Clone() {
			Shape result = new AnnotationSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
			switch (controlPointId) {
				case TopLeftControlPoint:
				case TopCenterControlPoint:
				case TopRightControlPoint:
				case MiddleLeftControlPoint:
				case MiddleRightControlPoint:
				case BottomLeftControlPoint:
				case BottomCenterControlPoint:
				case BottomRightControlPoint:
					return (controlPointCapability & ControlPointCapabilities.Resize) != 0;
				case MiddleCenterControlPoint:
				case ControlPointId.Reference:
					return ((controlPointCapability & ControlPointCapabilities.Rotate) != 0 
						|| (controlPointCapability & ControlPointCapabilities.Reference) != 0
						|| ((controlPointCapability & ControlPointCapabilities.Connect) != 0
							&& IsConnectionPointEnabled(controlPointId)));
				default:
					return false;
			}
		}


		/// <override></override>
		public override Point CalculateConnectionFoot(int startX, int startY) {
			Point result = Point.Empty;
			result.Offset(X, Y);

			// Calculate shape
			CalculateShapePoints();
			Matrix.Reset();
			Matrix.Translate(X, Y);
			Matrix.RotateAt(Geometry.TenthsOfDegreeToDegrees(Angle), Center);
			Matrix.TransformPoints(shapePoints);

			float currDist, dist = float.MaxValue;
			foreach (Point p in Geometry.IntersectPolygonLine(shapePoints, startX, startY, X, Y, true)) {
				currDist = Geometry.DistancePointPoint(p.X, p.Y, startX, startY);
				if (currDist < dist) {
					dist = currDist;
					result = p;
				}
			}
			return result;
		}


		/// <override></override>
		public override void Draw(Graphics graphics) {
			base.Draw(graphics);
			Pen pen = ToolCache.GetPen(LineStyle, null, null);
			graphics.DrawLines(pen, foldingPoints);
		}


		protected internal AnnotationSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		/// <override></override>
		protected override void TransformDrawCache(int deltaX, int deltaY, int deltaAngle, int rotationCenterX, int rotationCenterY) {
			base.TransformDrawCache(deltaX, deltaY, deltaAngle, rotationCenterX, rotationCenterY);
			Matrix.TransformPoints(foldingPoints);
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				CalculateShapePoints();

				Path.Reset();
				Path.StartFigure();
				Path.AddPolygon(shapePoints);
				Path.CloseFigure();
				return true;
			}
			return false;
		}


		private void CalculateShapePoints() {
			int foldingSize = Math.Min(Width / 8, Height / 8);
			int left = (int)Math.Round(-Width / 2f);
			int top = (int)Math.Round(-Height / 2f);
			int right = left + Width;
			int bottom = top + Height;

			shapePoints[0].X = left;
			shapePoints[0].Y = top;
			shapePoints[1].X = right - foldingSize;
			shapePoints[1].Y = top;
			shapePoints[2].X = right;
			shapePoints[2].Y = top + foldingSize;
			shapePoints[3].X = right;
			shapePoints[3].Y = bottom;
			shapePoints[4].X = left;
			shapePoints[4].Y = bottom;

			foldingPoints[0].X = right - foldingSize;
			foldingPoints[0].Y = top;
			foldingPoints[1].X = right - foldingSize;
			foldingPoints[1].Y = top + foldingSize;
			foldingPoints[2].X = right;
			foldingPoints[2].Y = top + foldingSize;
		}


		#region Fields

		// ControlPoint Id Constants
		private const int TopLeftControlPoint = 1;
		private const int TopCenterControlPoint = 2;
		private const int TopRightControlPoint = 3;
		private const int MiddleLeftControlPoint = 4;
		private const int MiddleRightControlPoint = 5;
		private const int BottomLeftControlPoint = 6;
		private const int BottomCenterControlPoint = 7;
		private const int BottomRightControlPoint = 8;
		private const int MiddleCenterControlPoint = 9;
		
		private Point[] shapePoints = new Point[5];
		private Point[] foldingPoints = new Point[3];
		
		#endregion
	}


	public class CloudSymbol : RectangleBase {

		/// <override></override>
		protected override void InitializeToDefault(IStyleSet styleSet) {
			base.InitializeToDefault(styleSet);
			Width = 120;
			Height = 80;
		}


		/// <override></override>
		public override Shape Clone() {
			Shape result = new CloudSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
			switch (controlPointId) {
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
				case 6:
				case 7:
				case 8:
					return ((controlPointCapability & ControlPointCapabilities.Resize) != 0);
				case 9:
					return ((controlPointCapability & ControlPointCapabilities.Reference) != 0
						|| (controlPointCapability & ControlPointCapabilities.Rotate) != 0);
				case 10:
				case 11:
				case 12:
				case 13:
				case 14:
				case 15:
					return ((controlPointCapability & ControlPointCapabilities.Connect) != 0);
				default:
					return base.HasControlPointCapability(controlPointId, controlPointCapability);
			}
		}


		/// <override></override>
		public override Point CalculateConnectionFoot(int startX, int startY) {
			UpdateDrawCache();
			Point result = Geometry.GetNearestPoint(startX, startY, Geometry.IntersectPolygonLine(Path.PathPoints, startX, startY, X, Y, true));
			if (!Geometry.IsValid(result)) result = Center;
			return result;
		}


		/// <override></override>
		public override void Draw(Graphics graphics) {
			base.Draw(graphics);

			//// draw debug info
			//graphics.DrawRectangles(Pens.DarkGreen, arcBounds);
			//for (int i = 0; i < shapePoints.Length; ++i) {
			//   graphics.DrawLine(Pens.Red, shapePoints[i].X - 2, shapePoints[i].Y, shapePoints[i].X + 2, shapePoints[i].Y);
			//   graphics.DrawLine(Pens.Red, shapePoints[i].X, shapePoints[i].Y - 2, shapePoints[i].X, shapePoints[i].Y + 2);
			//}
			//Path.Reset();	// call CalcPath every Frame
		}


		protected internal CloudSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		/// <override></override>
		protected override int ControlPointCount {
			get { return 15; }
		}


		/// <override></override>
		protected override void CalcControlPoints() {
			base.CalcControlPoints();
			float w = Width / 128f;
			float h = Height / 128f;
			ControlPoints[9].X = 0 - (int)Math.Round(56 * w);
			ControlPoints[9].Y = 0;
			ControlPoints[10].X = 0 - (int)Math.Round(32 * w);
			ControlPoints[10].Y = 0 - (int)Math.Round(48 * h);
			ControlPoints[11].X = 0 + (int)Math.Round(8 * w);
			ControlPoints[11].Y = 0 - (int)Math.Round(48 * h);
			ControlPoints[12].X = 0 + (int)Math.Round(56 * w);
			ControlPoints[12].Y = 0 - (int)Math.Round(16 * h);
			ControlPoints[13].X = 0 + (int)Math.Round(30 * w);
			ControlPoints[13].Y = 0 + (int)Math.Round(48 * h);
			ControlPoints[14].X = 0 - (int)Math.Round(16 * w);
			ControlPoints[14].Y = 0 + (int)Math.Round(48 * h);
		}


		/// <override></override>
		protected override void CalcCaptionBounds(int index, out Rectangle captionBounds) {
			if (index != 0) throw new IndexOutOfRangeException();
			int left, right, top, bottom;
			left = top = int.MaxValue;
			right = bottom = int.MinValue;
			for (int i = 0; i < ControlPoints.Length; ++i) {
				if (ControlPoints[i].X < left) left = ControlPoints[i].X;
				if (ControlPoints[i].X > right) right = ControlPoints[i].X;
				if (ControlPoints[i].Y < top) top = ControlPoints[i].Y;
				if (ControlPoints[i].Y > bottom) bottom = ControlPoints[i].Y;
			}
			captionBounds = Rectangle.Empty;
			captionBounds.X = left;
			captionBounds.Y = top;
			captionBounds.Width = right - left;
			captionBounds.Height = bottom - top;
			if (captionBounds.X > (-Width / 2f))
				captionBounds.X -= X;
			if (captionBounds.Y > (-Height / 2f))
				captionBounds.Y -= Y;
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				Path.Reset();
				float left, top, width, height;
				float w = Width / 128f;
				float h = Height / 128f;

				Path.StartFigure();
				/*****************************************/
				left = -(64 * w);
				top = -(58 * h);
				width = 37 * w;
				height = 64 * h;
				AddArcToGraphicsPath(left, top, width, height, ControlPoints[9].X, ControlPoints[9].Y, ControlPoints[10].X, ControlPoints[10].Y);
				arcBounds[0] = RectangleF.FromLTRB(left, top, left + width, top + height);
				/*****************************************/
				left = -(35 * w);
				top = -(64 * h);
				width = 46 * w;
				height = 62 * h;
				AddArcToGraphicsPath(left, top, width, height, ControlPoints[10].X, ControlPoints[10].Y, ControlPoints[11].X, ControlPoints[11].Y);
				arcBounds[1] = RectangleF.FromLTRB(left, top, left + width, top + height);
				/*****************************************/
				left = (3 * w);
				top = -(64 * h);
				width = 54 * w;
				height = 78 * h;
				AddArcToGraphicsPath(left, top, width, height, ControlPoints[11].X, ControlPoints[11].Y, ControlPoints[12].X, ControlPoints[12].Y);
				arcBounds[2] = RectangleF.FromLTRB(left, top, left + width, top + height);
				/*****************************************/
				left = (21 * w);
				top = -(25 * h);
				width = 43 * w;
				height = 81 * h;
				AddArcToGraphicsPath(left, top, width, height, ControlPoints[12].X, ControlPoints[12].Y, ControlPoints[13].X, ControlPoints[13].Y);
				arcBounds[3] = RectangleF.FromLTRB(left, top, left + width, top + height);
				/*****************************************/
				left = -(20 * w);
				top = -(0 * h);
				width = 53 * w;
				height = 64 * h;
				AddArcToGraphicsPath(left, top, width, height, ControlPoints[13].X, ControlPoints[13].Y, ControlPoints[14].X, ControlPoints[14].Y);
				arcBounds[4] = RectangleF.FromLTRB(left, top, left + width, top + height);
				/*****************************************/
				left = -(64 * w);
				top = -(10 * h);
				width = 52 * w;
				height = (73 * h);
				AddArcToGraphicsPath(left, top, width, height, ControlPoints[14].X, ControlPoints[14].Y, ControlPoints[9].X, ControlPoints[9].Y);
				arcBounds[5] = RectangleF.FromLTRB(left, top, left + width, top + height);
				/*****************************************/
				Path.CloseFigure();
				return true;
			}
			return false;
		}


		private void AddArcToGraphicsPath(float left, float top, float width, float height, int fromX, int fromY, int toX, int toY) {
			int centerX = (int)Math.Round(left + (width / 2));
			int centerY = (int)Math.Round(top + (height / 2));

			float startAngle, sweepAngle;
			startAngle = Geometry.Angle(centerX, centerY, (int)Math.Round(left + width), centerY, fromX, fromY) / (float)(Math.PI / 180);
			sweepAngle = (360 + Geometry.Angle(centerX, centerY, fromX, fromY, toX, toY) / (float)(Math.PI / 180)) % 360;

			if (width == 0) width = 1;
			if (height == 0) height = 1;
			Path.AddArc(left, top, width, height, startAngle, sweepAngle);
		}


		/// <override></override>
		public override void DrawOutline(Graphics graphics, Pen pen) {
			base.DrawOutline(graphics, pen);

			//if (connections.Count == 1) {
			//   Point startPt = connections[0].PassiveShape.GetControlPointPosition(connections[0].ConnectionPointId == 1 ? 2 : 1);
			//   int startX = startPt.X;
			//   int startY = startPt.Y;
			//   Point result = Center;

			//   graphics.DrawPolygon(Pens.Red, Path.PathPoints);

			//   float distance, lowestDistance;
			//   lowestDistance = float.MaxValue;
			//   foreach (Point p in Geometry.IntersectPolygonLine(Path.PathPoints, startX, startY, X, Y)) {
			//      distance = Geometry.DistancePointPoint(p.X, p.Y, startX, startY);
			//      Geometry.DrawPoint(graphics, Pens.Yellow, p.X, p.Y, 3);
			//      if (distance < lowestDistance) {
			//         lowestDistance = distance;
			//         result = p;
			//      }
			//   }

			//   Geometry.DrawPoint(graphics, Pens.Red, result.X, result.Y, 3);
			//}
		}


		#region Fields
		private RectangleF[] arcBounds = new RectangleF[6];
		#endregion
	}


	public class DatabaseSymbol : RectangleBase {

		/// <override></override>
		protected override void InitializeToDefault(IStyleSet styleSet) {
			base.InitializeToDefault(styleSet);
			Width = 80;
			Height = 80;
		}


		/// <override></override>
		public override Shape Clone() {
			Shape result = new DatabaseSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		public override Point CalculateConnectionFoot(int startX, int startY) {
			// First, calculate intersection point with the shape aligned bounding box (the bounding box that is rotated with the shape)
			Point result = base.CalculateConnectionFoot(startX, startY);
			bool calcUpperEllipseIntersection = false;
			bool calcLowerEllipseIntersection = false;
			//
			// Then, check if the intersection point would intersect with any of the ellipsis parts of the shape
			// by checking if the calculated intersection point is on the (shape aligned) bounding box of the
			// upper/lower half-ellipse
			float angleDeg = Angle == 0 ? 0 : Geometry.TenthsOfDegreeToDegrees(Angle);
			if (Angle == 0) {
				// An unrotated shape simplifies the calculation
				Rectangle halfEllipseBounds = Rectangle.Empty;
				// check bounds of the upper half-ellipse
				halfEllipseBounds.X = X - (int)Math.Round(Width / 2f);
				halfEllipseBounds.Y = Y - (int)Math.Round(Height / 2f);
				halfEllipseBounds.Width = Width;
				halfEllipseBounds.Height = (int)Math.Round(EllipseHeight / 2);
				calcUpperEllipseIntersection = Geometry.RectangleContainsPoint(halfEllipseBounds.X, halfEllipseBounds.Y, halfEllipseBounds.Width, halfEllipseBounds.Height, result.X, result.Y, true);
				if (!calcUpperEllipseIntersection) {
					// check bounds of the lower half-ellipse
					halfEllipseBounds.Width = Width;
					halfEllipseBounds.Height = (int)Math.Round(EllipseHeight / 2);
					halfEllipseBounds.X = X - (int)Math.Round(Width / 2f);
					halfEllipseBounds.Y = Y + (int)Math.Round(Height / 2f) - halfEllipseBounds.Height;
					calcLowerEllipseIntersection = Geometry.RectangleContainsPoint(halfEllipseBounds.X, halfEllipseBounds.Y, halfEllipseBounds.Width, halfEllipseBounds.Height, result.X, result.Y, true);
				}
			} else {
				// check bounds of upper half ellipse
				int boxWidth = Width;
				int boxHeight = (int)Math.Round(EllipseHeight / 2);
				Point boxCenter = Point.Empty;
				boxCenter.X = X;
				boxCenter.Y = (int)Math.Round(Y - (Height / 2f) + (EllipseHeight / 4f));
				boxCenter = Geometry.RotatePoint(Center, angleDeg, boxCenter);
				calcUpperEllipseIntersection = Geometry.RectangleIntersectsWithLine(boxCenter.X, boxCenter.Y, boxWidth, boxHeight, angleDeg, startX, startY, X, Y, true);
				// check bounds of the lower half-ellipse
				if (!calcUpperEllipseIntersection) {
					boxCenter.X = X;
					boxCenter.Y = (int)Math.Round(Y + (Height / 2f) - (EllipseHeight / 4f));
					boxCenter = Geometry.RotatePoint(Center, angleDeg, boxCenter);
					calcLowerEllipseIntersection = Geometry.RectangleIntersectsWithLine(boxCenter.X, boxCenter.Y, boxWidth, boxHeight, angleDeg, startX, startY, X, Y, true);
				}
			}

			// Calculate intersection point with ellipse if neccessary
			if (calcUpperEllipseIntersection || calcLowerEllipseIntersection) {
				Point ellipseCenter = Point.Empty;
				if (calcUpperEllipseIntersection)
					ellipseCenter.Y = (int)Math.Round(Y - (Height / 2f) + (EllipseHeight / 2f));
				else
					ellipseCenter.Y = (int)Math.Round(Y + (Height / 2f) - (EllipseHeight / 2f));
				ellipseCenter.X = X;
				if (Angle != 0) ellipseCenter = Geometry.RotatePoint(Center, angleDeg, ellipseCenter);
				PointF p = Geometry.GetNearestPoint(startX, startY, Geometry.IntersectEllipseLine(ellipseCenter.X, ellipseCenter.Y, Width, EllipseHeight, angleDeg, startX, startY, X, Y, false));
				if (Geometry.IsValid(p)) result = Point.Round(p);
			}
			return result;
		}
		
		
		protected internal DatabaseSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		/// <override></override>
		protected override void CalcCaptionBounds(int index, out Rectangle captionBounds) {
			if (index != 0) throw new IndexOutOfRangeException();
			int left = (int)Math.Round(-Width / 2f);
			int top = (int)Math.Round(-Height / 2f);
			captionBounds = Rectangle.Empty;
			captionBounds.X = left;
			captionBounds.Y = top + (int)Math.Ceiling(EllipseHeight);
			captionBounds.Width = Width;
			captionBounds.Height = Height - (int)Math.Ceiling(EllipseHeight);
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				Path.Reset();
				int left = (int)Math.Round(-Width / 2f);
				int top = (int)Math.Round(-Height / 2f);
				int bottom = top + Height;
				float arcWidth = Math.Max(Width, 0.00001f);
				float arcHeight = Math.Max(EllipseHeight, 0.00001f);

				Path.StartFigure();
				Path.AddEllipse(left, top, Width, EllipseHeight);
				Path.CloseFigure();
				Path.StartFigure();
				Path.AddArc(left, top, arcWidth, arcHeight, 0, 180);
				Path.AddLine(left, top + (EllipseHeight / 2), left, bottom - (EllipseHeight / 2));
				Path.AddArc(left, bottom - EllipseHeight, arcWidth, arcHeight, 180, -180);
				Path.AddLine(left + Width, bottom - (EllipseHeight / 2), left + Width, top + (EllipseHeight / 2));
				Path.CloseAllFigures();
				return true;
			}
			return false;
		}


		private float EllipseHeight {
			get { return Math.Max(Width / zFactor, 1); }
		}


		#region Fields
		private const float zFactor = 6;
		#endregion
	}


	public class ClassSymbol : EntitySymbol {
		
		protected internal ClassSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		/// <override></override>
		public override Shape Clone() {
			Shape result = new ClassSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		public override IEnumerable<MenuItemDef> GetMenuItemDefs(int mouseX, int mouseY, int range) {
			return base.GetMenuItemDefs(mouseX, mouseY, range);
		}


		/// <override></override>
		protected override void InitializeToDefault(IStyleSet styleSet) {
			base.InitializeToDefault(styleSet);
			Text = "Class";
		}
	}


	public class ComponentSymbol : RectangleBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new ComponentSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		protected override int ControlPointCount {
			get { return 34; }
		}


		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
			switch (controlPointId) {
				case 1:
				case 4:
				case 6:
					return ((controlPointCapability & ControlPointCapabilities.Resize) != 0);
				case 2:
				case 3:
				case 5:
				case 7:
				case 8:
					return ((controlPointCapability & ControlPointCapabilities.Connect) != 0
						|| (controlPointCapability & ControlPointCapabilities.Resize) != 0);
				case 9:
					return ((controlPointCapability & ControlPointCapabilities.Reference) != 0
						|| (controlPointCapability & ControlPointCapabilities.Rotate) != 0);
				case 10:
				case 11:
				case 12:
				case 13:
				case 14:
				case 15:
				case 16:
				case 17:
				case 18:
				case 19:
				case 20:
				case 21:
				case 22:
				case 23:
				case 24:
				case 25:
				case 26:
				case 27:
				case 28:
				case 29:
				case 30:
				case 31:
				case 32:
				case 33:
				case 34:
					return ((controlPointCapability & ControlPointCapabilities.Connect) != 0);
				default:
					return base.HasControlPointCapability(controlPointId, controlPointCapability);
			}
		}


		protected internal ComponentSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		/// <override></override>
		protected override void CalcControlPoints() {
			// calculate positions of standard drag- and rotate handles
			base.CalcControlPoints();

			int left = (int)Math.Round(-Width / 2f);
			int top = (int)Math.Round(-Height / 2f);
			int right = left + Width;
			int bottom = top + Height;

			// add specific connectionpoints for "Lollies"
			int h, w;
			w = (int)Math.Round((float)Width / 8);
			h = (int)Math.Round((float)Height / 8);
			ControlPoints[9].X = -(w + w);
			ControlPoints[9].Y = top;
			ControlPoints[10].X = (w + w);
			ControlPoints[10].Y = top;

			ControlPoints[11].X = -(w + w);
			ControlPoints[11].Y = bottom;
			ControlPoints[12].X = (w + w);
			ControlPoints[12].Y = bottom;

			ControlPoints[13].X = left;
			ControlPoints[13].Y = -(h + h);
			ControlPoints[14].X = left;
			ControlPoints[14].Y = (h + h);

			ControlPoints[15].X = right;
			ControlPoints[15].Y = -(h + h);
			ControlPoints[16].X = right;
			ControlPoints[16].Y = (h + h);

			ControlPoints[17].X = left + w;
			ControlPoints[17].Y = 0;

			ControlPoints[18].X = 0 - w;
			ControlPoints[18].Y = top;
			ControlPoints[19].X = w;
			ControlPoints[19].Y = top;
			ControlPoints[20].X = (w + w + w);
			ControlPoints[20].Y = top;

			ControlPoints[21].X = -w;
			ControlPoints[21].Y = bottom;
			ControlPoints[22].X = +w;
			ControlPoints[22].Y = bottom;
			ControlPoints[23].X = (w + w + w);
			ControlPoints[23].Y = bottom;

			ControlPoints[24].X = left;
			ControlPoints[24].Y = -(h + h + h);
			ControlPoints[25].X = left;
			ControlPoints[25].Y = -h;
			ControlPoints[26].X = left;
			ControlPoints[26].Y = +h;
			ControlPoints[27].X = left;
			ControlPoints[27].Y = (h + h + h);

			ControlPoints[28].X = right;
			ControlPoints[28].Y = -(h + h + h);
			ControlPoints[29].X = right;
			ControlPoints[29].Y = -h;
			ControlPoints[30].X = right;
			ControlPoints[30].Y = +h;
			ControlPoints[31].X = right;
			ControlPoints[31].Y = (h + h + h);

			ControlPoints[32].X = left + w;
			ControlPoints[32].Y = top;
			ControlPoints[33].X = left + w;
			ControlPoints[33].Y = bottom;
		}


		/// <override></override>
		protected override void CalcCaptionBounds(int index, out Rectangle captionBounds) {
			if (index != 0) throw new IndexOutOfRangeException();
			int left = (int)Math.Round(-Width / 2f);
			int top = (int)Math.Round(-Height / 2f);
			int w = (int)Math.Round((float)Width / 8);
			captionBounds = Rectangle.Empty;
			captionBounds.X = left + (w + w);
			captionBounds.Y = top;
			captionBounds.Width = Width - (w + w);
			captionBounds.Height = Height;
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				Path.Reset();
				Path.FillMode = System.Drawing.Drawing2D.FillMode.Winding;

				int h, w;
				int left = (int)Math.Round(-Width / 2f);
				int top = (int)Math.Round(-Height / 2f);
				int right = left + Width;
				int bottom = top + Height;
				w = (int)Math.Round((float)Width / 8);
				h = (int)Math.Round((float)Height / 8);

				shapePoints[0].X = left + w;
				shapePoints[0].Y = top + h;
				shapePoints[1].X = left + w;
				shapePoints[1].Y = top;
				shapePoints[2].X = right;
				shapePoints[2].Y = top;
				shapePoints[3].X = right;
				shapePoints[3].Y = bottom;
				shapePoints[4].X = left + w;
				shapePoints[4].Y = bottom;
				shapePoints[5].X = left + w;
				shapePoints[5].Y = bottom - h;


				Rectangle rect1Buffer = Rectangle.Empty;
				rect1Buffer.X = left;
				rect1Buffer.Y = h;
				rect1Buffer.Width = w + w;
				rect1Buffer.Height = h + h;

				Rectangle rect2Buffer = Rectangle.Empty;
				rect2Buffer.X = left;
				rect2Buffer.Y = top + h;
				rect2Buffer.Width = w + w;
				rect2Buffer.Height = h + h;

				Path.StartFigure();
				Path.AddLines(shapePoints);
				Path.AddRectangle(rect1Buffer);
				Path.AddLine(left + w, h, left + w, -h);
				Path.AddRectangle(rect2Buffer);
				Path.CloseFigure();
				return true;
			}
			return false;
		}


		#region Fields
		private Point[] shapePoints = new Point[6];
		#endregion
	}


	/// <summary>
	/// Displays a symbol for a document.
	/// </summary>
	public class DocumentSymbol : RectangleBase {

		/// <override></override>
		protected override void InitializeToDefault(IStyleSet styleSet) {
			base.InitializeToDefault(styleSet);
			Height = 60;
			Width = (int)(Height / Math.Sqrt(2));
		}


		/// <override></override>
		public override Shape Clone() {
			Shape result = new DocumentSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		public override bool HasControlPointCapability(ControlPointId controlPointId, ControlPointCapabilities controlPointCapability) {
			switch (controlPointId) {
				case TopLeftControlPoint:
				case TopRightControlPoint:
				case BottomLeftControlPoint:
				case BottomCenterControlPoint:
				case BottomRightControlPoint:
					return (controlPointCapability != ControlPointCapabilities.Connect 
						&& base.HasControlPointCapability(controlPointId, controlPointCapability));
				default: return base.HasControlPointCapability(controlPointId, controlPointCapability);
			}
		}


		protected internal DocumentSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				Path.Reset();
				Path.FillMode = System.Drawing.Drawing2D.FillMode.Winding;
				int size = Width > 40 && Height > 40 ? 20 : Math.Min(Width / 2, Height / 2);
				// Rechteck zeichnen
				Point[] points = new Point[5];
				points[0].X = -Width / 2;
				points[0].Y = -Height / 2;
				points[1].X = -Width / 2 + Width - size;
				points[1].Y = -Height / 2;
				points[2].X = -Width / 2 + Width;
				points[2].Y = -Height / 2 + size;
				points[3].X = -Width / 2 + Width;
				points[3].Y = -Height / 2 + Height;
				points[4].X = -Width / 2;
				points[4].Y = -Height / 2 + Height;
				Path.StartFigure();
				Path.AddLines(points);
				Path.CloseFigure();
				// Ecke hinzufügen
				points = new Point[3];
				points[0].X = -Width / 2 + Width - size;
				points[0].Y = -Height / 2;
				points[1].X = -Width / 2 + Width - size;
				points[1].Y = -Height / 2 + size;
				points[2].X = -Width / 2 + Width;
				points[2].Y = -Height / 2 + size;
				Path.StartFigure();
				Path.AddLines(points);
				for (int i = 1; i < Height / 8; ++i) {
					int right = -Width / 2 + Width - 10;
					if (8 * i < size) right -= size;
					Path.StartFigure();
					Path.AddLine(-Width / 2 + 10, -Height / 2 + i * 8, right, -Height / 2 + i * 8);
				}
				return true;
			}
			return false;
		}


		// ControlPoint Id Constants
		private const int TopLeftControlPoint = 1;
		private const int TopCenterControlPoint = 2;
		private const int TopRightControlPoint = 3;
		private const int MiddleLeftControlPoint = 4;
		private const int MiddleRightControlPoint = 5;
		private const int BottomLeftControlPoint = 6;
		private const int BottomCenterControlPoint = 7;
		private const int BottomRightControlPoint = 8;
		private const int MiddleCenterControlPoint = 9;
		
		private Point[] shapePoints = new Point[6];
	}


	public class DataFlowArrow : PolylineBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new DataFlowArrow(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		[Category("Appearance")]
		[Description("Defines the line cap appearance of the line's beginning.\nUse the template editor to modify all shapes of a template.\nUse the design editor to modify and create styles.")]
		[PropertyMappingId(PropertyIdStartCapStyle)]
		[RequiredPermission(Permission.Present)]
		public ICapStyle StartCapStyle {
			get {
				if (StartCapStyleInternal == null && Template == null) throw new NShapeException("Property StartCapStyle is not set.");
				return StartCapStyleInternal == null ? ((DataFlowArrow)Template.Shape).StartCapStyle : StartCapStyleInternal;
			}
			set {
				StartCapStyleInternal = (Template != null && value == ((DataFlowArrow)Template.Shape).StartCapStyle) ? null : value;
			}
		}


		[Category("Appearance")]
		[Description("Defines the line cap appearance of the line's ending.\nUse the template editor to modify all shapes of a template.\nUse the design editor to modify and create styles.")]
		[PropertyMappingId(PropertyIdEndCapStyle)]
		[RequiredPermission(Permission.Present)]
		public ICapStyle EndCapStyle {
			get {
				if (EndCapStyleInternal == null && Template == null) throw new NShapeException("Property EndCapStyle is not set.");
				return EndCapStyleInternal == null ? ((DataFlowArrow)Template.Shape).EndCapStyle : EndCapStyleInternal;
			}
			set {
				EndCapStyleInternal = (Template != null && value == ((DataFlowArrow)Template.Shape).EndCapStyle) ? null : value;
			}
		}


		protected internal DataFlowArrow(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}
	}


	public class DependencyArrow : PolylineBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new DependencyArrow(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		[Category("Appearance")]
		[Description("Defines the line cap appearance of the line's beginning.\nUse the template editor to modify all shapes of a template.\nUse the design editor to modify and create styles.")]
		[PropertyMappingId(PropertyIdStartCapStyle)]
		[RequiredPermission(Permission.Present)]
		public ICapStyle StartCapStyle {
			get {
				if (StartCapStyleInternal == null && Template == null) throw new NShapeException("Property StartCapStyle is not set.");
				return StartCapStyleInternal == null ? ((DependencyArrow)Template.Shape).StartCapStyle : StartCapStyleInternal;
			}
			set {
				StartCapStyleInternal = (Template != null && value == ((DependencyArrow)Template.Shape).StartCapStyle) ? null : value;
			}
		}


		[Category("Appearance")]
		[Description("Defines the line cap appearance of the line's ending.\nUse the template editor to modify all shapes of a template.\nUse the design editor to modify and create styles.")]
		[PropertyMappingId(PropertyIdEndCapStyle)]
		[RequiredPermission(Permission.Present)]
		public ICapStyle EndCapStyle {
			get {
				if (EndCapStyleInternal == null && Template == null) throw new NShapeException("Property EndCapStyle is not set.");
				return EndCapStyleInternal == null ? ((DependencyArrow)Template.Shape).EndCapStyle : EndCapStyleInternal;
			}
			set {
				EndCapStyleInternal = (Template != null && value == ((DependencyArrow)Template.Shape).EndCapStyle) ? null : value;
			}
		}


		protected internal DependencyArrow(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}

	}


	public class InterfaceUsageSymbol : PolylineBase {

		/// <override></override>
		public override Shape Clone() {
			InterfaceUsageSymbol result = new InterfaceUsageSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		protected override void InitializeToDefault(IStyleSet styleSet) {
			base.InitializeToDefault(styleSet);
			if (Template == null) {
				//EndCapStyle = styleSet.GetCapStyle("Fork Cap");
				// @@Kurt
				// Den Style "Fork Cap" gibt es nicht in den Standard-Designs. Da müssen wir uns erst noch was überlegen wie 
				// man das Design um benötigte Styles erweitern kann (am Besten biim Registrieren der GeneralShapes)
				EndCapStyleInternal = styleSet.CapStyles.Special2;
			}
		}
		

		protected internal InterfaceUsageSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}
	}


	public class InterfaceSymbol : PolylineBase {

		/// <override></override>
		public override Shape Clone() {
			Shape result = new InterfaceSymbol(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		/// <override></override>
		protected override void InitializeToDefault(IStyleSet styleSet) {
			base.InitializeToDefault(styleSet);
			if (Template == null) {
				//StartEndCapStyle = styleSet.GetCapStyle("Circle Cap");
				// @@Kurt
				// Den Style "Circle Cap" gibt es nicht in den Standard-Designs. Da müssen wir uns erst noch was überlegen wie 
				// man das Design um benötigte Styles erweitern kann (am Besten beim Registrieren der GeneralShapes)
				EndCapStyleInternal = styleSet.CapStyles.Special1;
			}
		}


		protected internal InterfaceSymbol(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}

	}


	public class VectorImage : CustomizableMetaFile {
		internal static VectorImage CreateInstance(ShapeType shapeType, Template template, 
			string resourceBasename, Assembly resourceAssembly) {
			return new VectorImage(shapeType, template, resourceBasename, resourceAssembly);
		}


		/// <override></override>
		public override Shape Clone() {
			Shape result = new VectorImage(Type, (Template)null, resourceName, resourceAssembly);
			result.CopyFrom(this);
			return result;
		}


		protected internal VectorImage(ShapeType shapeType, IStyleSet styleSet, string resourceBaseName, Assembly resourceAssembly)
			: base(shapeType, styleSet, resourceBaseName, resourceAssembly) {
		}


		protected internal VectorImage(ShapeType shapeType, Template template, string resourceBaseName, Assembly resourceAssembly)
			: base(shapeType, template, resourceBaseName, resourceAssembly) {
		}
	}


	public static class NShapeLibraryInitializer {

		public static void Initialize(IRegistrar registrar) {
			registrar.RegisterLibrary(namespaceName, preferredRepositoryVersion);
			registrar.RegisterShapeType(new ShapeType("DataFlow", namespaceName, namespaceName, 
				delegate(ShapeType shapeType, Template t) { return new DataFlowArrow(shapeType, t); }, 
				DataFlowArrow.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Dependency", namespaceName, namespaceName, 
				delegate(ShapeType shapeType, Template t) { return new DependencyArrow(shapeType, t); }, 
				DependencyArrow.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Database", namespaceName, namespaceName, 
				delegate(ShapeType shapeType, Template t) { return new DatabaseSymbol(shapeType, t); }, 
				DatabaseSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Entity", namespaceName, namespaceName, 
				delegate(ShapeType shapeType, Template t) { return new EntitySymbol(shapeType, t); }, 
				EntitySymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Annotation", namespaceName, namespaceName, 
				delegate(ShapeType shapeType, Template t) { return new AnnotationSymbol(shapeType, t); }, 
				AnnotationSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Cloud", namespaceName, namespaceName, 
				delegate(ShapeType shapeType, Template t) { return new CloudSymbol(shapeType, t); }, 
				CloudSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Class", namespaceName, namespaceName, 
				delegate(ShapeType shapeType, Template t) { return new ClassSymbol(shapeType, t); }, 
				ClassSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Component", namespaceName, namespaceName, 
				delegate(ShapeType shapeType, Template t) { return new ComponentSymbol(shapeType, t); }, 
				ComponentSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Document", namespaceName, namespaceName, 
				delegate(ShapeType shapeType, Template t) { return new DocumentSymbol(shapeType, t); }, 
				DocumentSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Interface", namespaceName, namespaceName, 
				delegate(ShapeType shapeType, Template t) { return new InterfaceSymbol(shapeType, t); }, 
				InterfaceSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("InterfaceUsage", namespaceName, namespaceName, 
				delegate(ShapeType shapeType, Template t) { return new InterfaceUsageSymbol(shapeType, t); }, 
				InterfaceUsageSymbol.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Server", namespaceName, namespaceName, 
				delegate(ShapeType shapeType, Template t) {
				VectorImage result = VectorImage.CreateInstance(shapeType, t,
					"Dataweb.NShape.SoftwareArchitectureShapes.Resources.Tower.emf", Assembly.GetExecutingAssembly());
				result.Text = "Server";
				return result;
				}, VectorImage.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("RTU", namespaceName, namespaceName, 
				delegate(ShapeType shapeType, Template t) {
				VectorImage result = VectorImage.CreateInstance(shapeType, t,
					"Dataweb.NShape.SoftwareArchitectureShapes.Resources.RTU.emf", Assembly.GetExecutingAssembly());
				result.Text = "RTU";
				return result;
			}, VectorImage.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Actor", namespaceName, namespaceName, 
				delegate(ShapeType shapeType, Template t) {
				VectorImage result = VectorImage.CreateInstance(shapeType, t,
					"Dataweb.NShape.SoftwareArchitectureShapes.Resources.Actor.emf", Assembly.GetExecutingAssembly());
				result.Text = "Actor";
				return result;
			}, VectorImage.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Monitor", namespaceName, namespaceName, 
				delegate(ShapeType shapeType, Template t) {
				VectorImage result = VectorImage.CreateInstance(shapeType, t,
					"Dataweb.NShape.SoftwareArchitectureShapes.Resources.Monitor.emf", Assembly.GetExecutingAssembly());
				result.Text = "Monitor";
				return result;
			}, VectorImage.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("PC", namespaceName, namespaceName, 
				delegate(ShapeType shapeType, Template t) {
				VectorImage result = VectorImage.CreateInstance(shapeType, t,
					"Dataweb.NShape.SoftwareArchitectureShapes.Resources.Desktop.emf", Assembly.GetExecutingAssembly());
				result.Text = "PC";
				return result;
			}, VectorImage.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Tower", namespaceName, namespaceName, 
				delegate(ShapeType shapeType, Template t) {
				VectorImage result = VectorImage.CreateInstance(shapeType, t,
					"Dataweb.NShape.SoftwareArchitectureShapes.Resources.Tower.emf", Assembly.GetExecutingAssembly());
				result.Text = "Tower";
				return result;
			}, VectorImage.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Laptop", namespaceName, namespaceName,
				delegate(ShapeType shapeType, Template t) {
					VectorImage result = VectorImage.CreateInstance(shapeType, t,
						"Dataweb.NShape.SoftwareArchitectureShapes.Resources.Laptop.emf", Assembly.GetExecutingAssembly());
					result.Text = "Notebook";
					return result;
				}, VectorImage.GetPropertyDefinitions));
		}


		private const string namespaceName = "SoftwareArchitectureShapes";
		private const int preferredRepositoryVersion = 3;
	}
}