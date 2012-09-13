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
using Dataweb.NShape.Advanced;


namespace Dataweb.NShape.GeneralShapes {

	public static class NShapeLibraryInitializer {

		public static void Initialize(IRegistrar registrar) {
			if (registrar == null) throw new ArgumentNullException("registrar");
			registrar.RegisterLibrary(namespaceName, preferredRepositoryVersion);

			// Register linear shapes
			registrar.RegisterShapeType(new ShapeType("Polyline", namespaceName, namespaceName,
				Polyline.CreateInstance, Polyline.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("RectangularLine", namespaceName, namespaceName,
				RectangularLine.CreateInstance, RectangularLine.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("CircularArc", namespaceName, namespaceName,
				"With only two points, it behaves like a straight line, with all three points, it behaves like a circular arc.",
				CircularArc.CreateInstance, CircularArc.GetPropertyDefinitions));
			// new Type(typeof(BezierLine).Name, BezierLine.CreateInstance, 
			// BezierLine.GetPropertyDefinitions(null)).Register(registrar);
			// Planar GeneralShapes

			// Register text shapes
			registrar.RegisterShapeType(new ShapeType("Text", namespaceName, namespaceName,
				"Supports automatic sizing to its text.",
				Text.CreateInstance, Text.GetPropertyDefinitions,
				Dataweb.NShape.GeneralShapes.Properties.Resources.ShaperReferenceQuadrangle));
			registrar.RegisterShapeType(new ShapeType("Label", namespaceName, namespaceName,
				"Supports autosizing to its text and connecting to other shapes. If the label's 'pin' is connected to a shape, the label will move with its partner shape.",
				Label.CreateInstance, Label.GetPropertyDefinitions,
				Dataweb.NShape.GeneralShapes.Properties.Resources.ShaperReferenceQuadrangle));

			registrar.RegisterShapeType(new ShapeType("RegularPolygone", namespaceName, namespaceName,
				RegularPolygone.CreateInstance, RegularPolygone.GetPropertyDefinitions));

			// Register triangle shapes
			registrar.RegisterShapeType(new ShapeType("FreeTriangle", namespaceName, namespaceName,
				FreeTriangle.CreateInstance, FreeTriangle.GetPropertyDefinitions,
				Dataweb.NShape.GeneralShapes.Properties.Resources.ShaperReferenceTriangle));
			registrar.RegisterShapeType(new ShapeType("IsoscelesTriangle", namespaceName, namespaceName,
				IsoscelesTriangle.CreateInstance, IsoscelesTriangle.GetPropertyDefinitions,
				Dataweb.NShape.GeneralShapes.Properties.Resources.ShaperReferenceTriangle));

			// Register round shapes
			registrar.RegisterShapeType(new ShapeType("Circle", namespaceName, namespaceName,
				Circle.CreateInstance, Circle.GetPropertyDefinitions,
				Dataweb.NShape.GeneralShapes.Properties.Resources.ShaperReferenceCircle));
			registrar.RegisterShapeType(new ShapeType("Ellipse", namespaceName, namespaceName,
				Ellipse.CreateInstance, Ellipse.GetPropertyDefinitions,
				Dataweb.NShape.GeneralShapes.Properties.Resources.ShaperReferenceCircle));

			// Register quadrangle shapes
			registrar.RegisterShapeType(new ShapeType("Square", namespaceName, namespaceName,
				Square.CreateInstance, Square.GetPropertyDefinitions,
				Dataweb.NShape.GeneralShapes.Properties.Resources.ShaperReferenceQuadrangle));
			registrar.RegisterShapeType(new ShapeType("Box", namespaceName, namespaceName,
				Box.CreateInstance, Box.GetPropertyDefinitions,
				Dataweb.NShape.GeneralShapes.Properties.Resources.ShaperReferenceQuadrangle));
			registrar.RegisterShapeType(new ShapeType("RoundedBox", namespaceName, namespaceName,
				RoundedBox.CreateInstance, RoundedBox.GetPropertyDefinitions,
				Dataweb.NShape.GeneralShapes.Properties.Resources.ShaperReferenceQuadrangle));
			registrar.RegisterShapeType(new ShapeType("Diamond", namespaceName, namespaceName,
				Diamond.CreateInstance, Diamond.GetPropertyDefinitions,
				Dataweb.NShape.GeneralShapes.Properties.Resources.ShaperReferenceDiamond));

			// Register other shapes
			registrar.RegisterShapeType(new ShapeType("ThickArrow", namespaceName, namespaceName,
				delegate(ShapeType shapeType, Template t) { return (Shape)new ThickArrow(shapeType, t); },
				ThickArrow.GetPropertyDefinitions));
			registrar.RegisterShapeType(new ShapeType("Picture", namespaceName, namespaceName,
				Picture.CreateInstance, Picture.GetPropertyDefinitions,
				Dataweb.NShape.GeneralShapes.Properties.Resources.ShaperReferenceQuadrangle));
		}


		#region Fields

		private const string namespaceName = "GeneralShapes";

		private const int preferredRepositoryVersion = 4;

		#endregion
	}

}
