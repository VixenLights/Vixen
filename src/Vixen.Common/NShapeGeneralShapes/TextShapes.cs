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

using Dataweb.NShape.Advanced;


namespace Dataweb.NShape.GeneralShapes
{
	public class Text : TextBase
	{
		internal static Shape CreateInstance(ShapeType shapeType, Template template)
		{
			return new Text(shapeType, template);
		}


		/// <override></override>
		public override Shape Clone()
		{
			Shape result = new Text(Type, (Template) null);
			result.CopyFrom(this);
			return result;
		}


		protected internal Text(ShapeType shapeType, Template template)
			: base(shapeType, template)
		{
		}
	}


	public class Label : LabelBase
	{
		internal static Label CreateInstance(ShapeType shapeType, Template template)
		{
			return new Label(shapeType, template);
		}


		/// <override></override>
		public override Shape Clone()
		{
			Shape result = new Label(Type, (Template) null);
			result.CopyFrom(this);
			return result;
		}


		protected internal Label(ShapeType shapeType, Template template)
			: base(shapeType, template)
		{
		}
	}
}