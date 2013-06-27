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
using System.ComponentModel;
using Dataweb.NShape.Advanced;


namespace Dataweb.NShape.GeneralShapes
{
	/// <summary>
	/// Line consisting of multiple line segments.
	/// </summary>
	public class Polyline : PolylineBase
	{
		internal static Shape CreateInstance(ShapeType shapeType, Template template)
		{
			Shape result = new Polyline(shapeType, template);
			return result;
		}


		/// <override></override>
		public override Shape Clone()
		{
			Shape result = new Polyline(Type, (Template) null);
			result.CopyFrom(this);
			return result;
		}


		[Category("Appearance")]
		[Description(
			"Defines the line cap appearance of the line's beginning.\nUse the template editor to modify all shapes of a template.\nUse the design editor to modify and create styles."
			)]
		[PropertyMappingId(PropertyIdStartCapStyle)]
		[RequiredPermission(Permission.Present)]
		public ICapStyle StartCapStyle
		{
			get
			{
				if (StartCapStyleInternal == null && Template == null)
					throw new NShapeException("Property StartCapStyle is not set.");
				return StartCapStyleInternal == null ? ((Polyline) Template.Shape).StartCapStyle : StartCapStyleInternal;
			}
			set { StartCapStyleInternal = (Template != null && value == ((Polyline) Template.Shape).StartCapStyle) ? null : value; }
		}


		[Category("Appearance")]
		[Description(
			"Defines the line cap appearance of the line's ending.\nUse the template editor to modify all shapes of a template.\nUse the design editor to modify and create styles."
			)]
		[PropertyMappingId(PropertyIdEndCapStyle)]
		[RequiredPermission(Permission.Present)]
		public ICapStyle EndCapStyle
		{
			get
			{
				if (EndCapStyleInternal == null && Template == null) throw new NShapeException("Property EndCapStyle is not set.");
				return EndCapStyleInternal == null ? ((Polyline) Template.Shape).EndCapStyle : EndCapStyleInternal;
			}
			set { EndCapStyleInternal = (Template != null && value == ((Polyline) Template.Shape).EndCapStyle) ? null : value; }
		}


		protected internal Polyline(ShapeType shapeType, Template template)
			: base(shapeType, template)
		{
		}


		protected internal Polyline(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet)
		{
		}

		#region Fields

		private const string persistentTypeName = "MultiSegmentLine";

		#endregion
	}


	/// <summary>
	/// Rectangular line with public cap styles.
	/// </summary>
	public class RectangularLine : RectangularLineBase
	{
		internal static Shape CreateInstance(ShapeType shapeType, Template template)
		{
			Shape result = new RectangularLine(shapeType, template);
			return result;
		}


		/// <override></override>
		public override Shape Clone()
		{
			Shape result = new RectangularLine(Type, (Template) null);
			result.CopyFrom(this);
			return result;
		}


		[Category("Appearance")]
		[Description(
			"Defines the line cap appearance of the line's beginning.\nUse the template editor to modify all shapes of a template.\nUse the design editor to modify and create styles."
			)]
		[PropertyMappingId(PropertyIdStartCapStyle)]
		[RequiredPermission(Permission.Present)]
		public ICapStyle StartCapStyle
		{
			get
			{
				if (StartCapStyleInternal == null && Template == null)
					throw new NShapeException("Property StartCapStyle is not set.");
				return StartCapStyleInternal == null ? ((RectangularLine) Template.Shape).StartCapStyle : StartCapStyleInternal;
			}
			set
			{
				StartCapStyleInternal = (Template != null && value == ((RectangularLine) Template.Shape).StartCapStyle)
				                        	? null
				                        	: value;
			}
		}


		[Category("Appearance")]
		[Description(
			"Defines the line cap appearance of the line's ending.\nUse the template editor to modify all shapes of a template.\nUse the design editor to modify and create styles."
			)]
		[PropertyMappingId(PropertyIdEndCapStyle)]
		[RequiredPermission(Permission.Present)]
		public ICapStyle EndCapStyle
		{
			get
			{
				if (EndCapStyleInternal == null && Template == null) throw new NShapeException("Property EndCapStyle is not set.");
				return EndCapStyleInternal == null ? ((RectangularLine) Template.Shape).EndCapStyle : EndCapStyleInternal;
			}
			set { EndCapStyleInternal = (Template != null && value == ((RectangularLine) Template.Shape).EndCapStyle) ? null : value; }
		}


		protected internal RectangularLine(ShapeType shapeType, Template template)
			: base(shapeType, template)
		{
		}


		protected internal RectangularLine(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet)
		{
		}

		#region Fields

		private const string persistentTypeName = "RectangularLine";

		#endregion
	}


	/// <summary>
	/// Circular arc with public cap styles.
	/// </summary>
	public class CircularArc : CircularArcBase
	{
		internal static Shape CreateInstance(ShapeType shapeType, Template template)
		{
			if (shapeType == null) throw new ArgumentNullException("shapeType");
			Shape result = new CircularArc(shapeType, template);
			return result;
		}


		/// <override></override>
		public override Shape Clone()
		{
			Shape result = new CircularArc(Type, (Template) null);
			result.CopyFrom(this);
			return result;
		}


		[Category("Appearance")]
		[Description(
			"Defines the line cap appearance of the line's beginning.\nUse the template editor to modify all shapes of a template.\nUse the design editor to modify and create styles."
			)]
		[PropertyMappingId(PropertyIdStartCapStyle)]
		[RequiredPermission(Permission.Present)]
		public ICapStyle StartCapStyle
		{
			get
			{
				if (StartCapStyleInternal == null && Template == null) return null;
				return StartCapStyleInternal == null ? ((CircularArc) Template.Shape).StartCapStyle : StartCapStyleInternal;
			}
			set { StartCapStyleInternal = (Template != null && value == ((CircularArc) Template.Shape).StartCapStyle) ? null : value; }
		}


		[Category("Appearance")]
		[Description(
			"Defines the line cap appearance of the line's ending.\nUse the template editor to modify all shapes of a template.\nUse the design editor to modify and create styles."
			)]
		[PropertyMappingId(PropertyIdEndCapStyle)]
		[RequiredPermission(Permission.Present)]
		public ICapStyle EndCapStyle
		{
			get
			{
				if (EndCapStyleInternal == null && Template == null) return null;
				return EndCapStyleInternal == null ? ((CircularArc) Template.Shape).EndCapStyle : EndCapStyleInternal;
			}
			set { EndCapStyleInternal = (Template != null && value == ((CircularArc) Template.Shape).EndCapStyle) ? null : value; }
		}


		protected internal CircularArc(ShapeType shapeType, Template template)
			: base(shapeType, template)
		{
		}
	}
}