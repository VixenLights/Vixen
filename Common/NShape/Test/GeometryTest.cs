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
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace NShapeTest {

	[TestClass]
	public class GeometryTests {

		[TestMethod]
		public void CalcDroppedPerpendicularFootTest() {
			int fX, fY;
			//
			Geometry.CalcDroppedPerpendicularFoot(0, 18, 5, 0, 12, 0, out fX, out fY);
			Assert.AreEqual(fX, 0);
			Assert.AreEqual(fY, 0);
			//
			Geometry.CalcDroppedPerpendicularFoot(-2, -2, 4, -17, 4, -16, out fX, out fY);
			Assert.AreEqual(fX, 4);
			Assert.AreEqual(fY, -2);
			//
			Geometry.CalcDroppedPerpendicularFoot(8, 2, -5, 1, 1, 7, out fX, out fY);
			Assert.AreEqual(fX, 2);
			Assert.AreEqual(fY, 8);
		}


		[TestMethod]
		public void CalcNormalVectorOfLineTest() {
			int pX, pY;
			//
			Geometry.CalcNormalVectorOfLine(-2, -4, 2, -4, 0, -4, 100, out pX, out pY);
			Assert.AreEqual(pX, 0);
			Assert.AreEqual(pY, -104);
			//
			Geometry.CalcNormalVectorOfLine(-5, -12, -5, -14, -5, 100, 100, out pX, out pY);
			Assert.AreEqual(pX, -105);
			Assert.AreEqual(pY, 100);
			//
			Geometry.CalcNormalVectorOfLine(0, 8, 6, 0, 3, 4, 100, out pX, out pY);
			Assert.AreEqual(pX, 83);
			Assert.AreEqual(pY, 64);
		}


		[TestMethod]
		public void DistancePointLineTest() {
			float d;

			d = Geometry.DistancePointLine(0, +3, -4, 0, +4, 0, false);
			Assert.AreEqual(d, 3.0, 0.01);
			//
			d = Geometry.DistancePointLine(0, +3, -4, 0, +4, 0, true);
			Assert.AreEqual(d, 3.0, 0.01);

			d = Geometry.DistancePointLine(-8, +3, -4, 0, +4, 0, false);
			Assert.AreEqual(d, 3.0, 0.01);
			//
			d = Geometry.DistancePointLine(-8, +3, -4, 0, +4, 0, true);
			Assert.AreEqual(d, 5.0, 0.01);
		}


		[TestMethod]
		public void CalcNearestPointOfLineSegmentTest() {
			int x, y;
			//
			Geometry.CalcNearestPointOfLineSegment(-4, 0, +4, 0, 0, 3, out x, out y);
			Assert.AreEqual(x, 0);
			Assert.AreEqual(y, 0);
			//
			Geometry.CalcNearestPointOfLineSegment(-4, 0, +4, 0, -8, 3, out x, out y);
			Assert.AreEqual(x, -4);
			Assert.AreEqual(y, 0);
			//
			Geometry.CalcNearestPointOfLineSegment(-4, 0, +4, 0, 8, 3, out x, out y);
			Assert.AreEqual(x, 4);
			Assert.AreEqual(y, 0);
			//
			Geometry.CalcNearestPointOfLineSegment(1, 1, 7, 7, 2, 8, out x, out y);
			Assert.AreEqual(x, 5);
			Assert.AreEqual(y, 5);
			//
			Geometry.CalcNearestPointOfLineSegment(1, 1, 7, 7, 2, 13, out x, out y);
			Assert.AreEqual(x, 7);
			Assert.AreEqual(y, 7);
			//
			Geometry.CalcNearestPointOfLineSegment(1, 1, 7, 7, 2, -1, out x, out y);
			Assert.AreEqual(x, 1);
			Assert.AreEqual(y, 1);

		}
	}

}