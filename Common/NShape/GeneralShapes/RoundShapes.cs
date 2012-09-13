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

	public class Circle : CircleBase {

		internal static Shape CreateInstance(ShapeType shapeType, Template template) {
			return new Circle(shapeType, template);
		}


		/// <override></override>
		public override Shape Clone() {
			Shape result = new Circle(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		protected internal Circle(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal Circle(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				int left = (int)Math.Round(-Diameter / 2f);
				int top = (int)Math.Round(-Diameter / 2f);
				Path.Reset();
				Path.StartFigure();
				Path.AddEllipse(left, top, Diameter, Diameter);
				Path.CloseFigure();
				return true;
			} else return false;
		}
	}


	public class Ellipse : EllipseBase {

		internal static Shape CreateInstance(ShapeType shapeType, Template template) {
			return new Ellipse(shapeType, template);
		}


		/// <override></override>
		public override Shape Clone() {
			Shape result = new Ellipse(Type, (Template)null);
			result.CopyFrom(this);
			return result;
		}


		protected internal Ellipse(ShapeType shapeType, Template template)
			: base(shapeType, template) {
		}


		protected internal Ellipse(ShapeType shapeType, IStyleSet styleSet)
			: base(shapeType, styleSet) {
		}


		/// <override></override>
		protected override bool CalculatePath() {
			if (base.CalculatePath()) {
				int left = (int)Math.Round(-Width / 2f);
				int top = (int)Math.Round(-Height / 2f);

				Path.Reset();
				Path.StartFigure();
				Path.AddEllipse(left, top, Width, Height);
				Path.CloseFigure();
				return true;
			} else return false;
		}
	}

}
