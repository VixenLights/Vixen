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
using System.Diagnostics;
using System.Drawing;


namespace Dataweb.NShape.Advanced {

	/// <summary>
	/// Describes a stroke as sequence of coordinates.
	/// </summary>
	public class Stroke : IEnumerable<Point> {

		/// <ToBeCompleted></ToBeCompleted>
		public static Stroke CreateStroke(sbyte[,] pixels, sbyte color) {
			if (pixels == null) throw new ArgumentNullException("pixels");
			Stroke result = new Stroke();
			int x, y; // Current pixel
			// Startpunkt finden
			FindStartingPixel(pixels, color, out x, out y);
			// Pixels zurücksetzen
			for (int i = 0; i < pixels.GetLength(0); ++i)
				for (int j = 0; j < pixels.GetLength(1); ++j)
					if (pixels[i, j] < 0) pixels[i, j] = (sbyte)-pixels[i, j];
			// Alle Pixel in richtiger Reihenfolge eintragen
			do {
				result.Add(x, y);
			} while (FindNextPixel(pixels, color, ref x, ref y));
			// Die markierten Pixel wieder zurücknehmen
			// Pixels zurücksetzen
			for (int i = 0; i < pixels.GetLength(0); ++i)
				for (int j = 0; j < pixels.GetLength(1); ++j)
					if (pixels[i, j] < 0) pixels[i, j] = (sbyte)-pixels[i, j];
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public const sbyte jokerColor = 127;


		/// <ToBeCompleted></ToBeCompleted>
		public int Count {
			get { return pixels.Count; }
		}


		/// <summary>
		/// Fügt einen Punkt zum Zug hinzu. Auch alle Punkte auf der Verbindungslinie 
		/// zwischen dem letzten Punkt und dem neuen werden hinzugefügt.
		/// </summary>
		public void Add(int x, int y) {
			if (pixels.Count != 0) {
				// Add line from last point to new one using Bresenham
				int y0 = pixels[pixels.Count - 1].Y;
				int x0 = pixels[pixels.Count - 1].X;
				int dy = y - y0;
				int dx = x - x0;
				int stepx, stepy;
				if (dx < 0) {
					dx = -dx;
					stepx = -1;
				}
				else stepx = 1;
				dx <<= 1;
				if (dy < 0) {
					dy = -dy;
					stepy = -1;
				}
				else stepy = 1;
				dy <<= 1;
				//
				if (dx > dy) {
					int fraction = dy - (dx >> 1);
					while (x0 != x) {
						if (fraction >= 0) {
							y0 += stepy;
							fraction -= dx;
						}
						x0 += stepx;
						fraction += dy;
						pixels.Add(new Point(x0, y0));
					}
				}
				else {
					int fraction = dx - (dy >> 1);
					while (y0 != y) {
						if (fraction >= 0) {
							x0 += stepx;
							fraction -= dy;
						}
						y0 += stepy;
						fraction += dx;
						pixels.Add(new Point(x0, y0));
					}
				}
			}
			else
				pixels.Add(new Point(x, y));
		}


		/// <ToBeCompleted></ToBeCompleted>
		public Point this[int index] {
			get { return pixels[index]; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void Clear() {
			pixels.Clear();
		}


		#region IEnumerable Members

		/// <ToBeCompleted></ToBeCompleted>
		public IEnumerator<Point> GetEnumerator() {
			for (int i = 0; i < pixels.Count; ++i)
				yield return pixels[i];
		}

		/// <ToBeCompleted></ToBeCompleted>
		IEnumerator IEnumerable.GetEnumerator() {
			for (int i = 0; i < pixels.Count; ++i)
				yield return pixels[i];
		}

		#endregion


		private static void FindStartingPixel(sbyte[,] pixels, sbyte color, out int x, out int y) {
			// Suche belegtes Pixel
			y = 0;
			x = 1;
			while (x < pixels.GetLength(0) - 1) {
				y = 1;
				while (y < pixels.GetLength(1) - 1) {
					if (pixels[x, y] == color || pixels[x, y] == jokerColor) break;
					++y;
				}
				if (pixels[x, y] == color || pixels[x, y] == jokerColor) break;
				++x;
			}
			// Falls gefunden, von hier bis zu einem Anfangspunkt gehen
			if (pixels[x, y] == 0) throw new Exception("The image is empty");
			bool found = false;
			while (!found)
				found = FindNextPixel(pixels, color, ref x, ref y);
		}


		private static bool FindNextPixel(sbyte[,] pixels, sbyte color, ref int x, ref int y) {
			// Voraussetzung: x, y ist ein gültiges Pixel
			// Es gibt drei Arten von Anschlus: Gerade, schräg oder ums Eck
			return FindNextDiagonalPixel(pixels, color, ref x, ref y, -1, -1)
			|| FindNextDiagonalPixel(pixels, color, ref x, ref y, +1, -1)
			|| FindNextDiagonalPixel(pixels, color, ref x, ref y, +1, +1)
			|| FindNextDiagonalPixel(pixels, color, ref x, ref y, -1, +1)
			|| FindNextDirectPixel(pixels, color, ref x, ref y, 0, -1)
			|| FindNextDirectPixel(pixels, color, ref x, ref y, +1, 0)
			|| FindNextDirectPixel(pixels, color, ref x, ref y, 0, +1)
			|| FindNextDirectPixel(pixels, color, ref x, ref y, -1, 0);
		}


		// x und y liegen im inneren von pixels
		private static bool FindNextDiagonalPixel(sbyte[,] pixels, sbyte color, ref int x, ref int y, int dx, int dy) {
			bool result = false;
			if (pixels[x + dx, y + dy] == color || pixels[x + dx, y + dy] == jokerColor) {
				// Hier gehts weiter, markieren
				pixels[x + dx, y + dy] = (sbyte)-pixels[x + dx, y + dy];
				// Wie siehts mit den Ecken aus?
				if ((pixels[x + dx, y] == color || pixels[x + dx, y] == jokerColor) && IsIsolated(pixels, color, x + dx, y))
					pixels[x + dx, y] = (sbyte)-pixels[x + dx, y];
				if ((pixels[x, y + dy] == color || pixels[x, y + dy] == -1) && IsIsolated(pixels, color, x, y + dy))
					pixels[x, y + dy] = (sbyte)-pixels[x, y + dy];
				x += dx; y += dy;
				result = true;
			}
			return result;
		}


		private static bool FindNextDirectPixel(sbyte[,] pixels, sbyte color, ref int x, ref int y, int dx, int dy) {
			bool result = false;
			if (pixels[x + dx, y + dy] == color || pixels[x + dx, y + dy] == jokerColor) {
				pixels[x + dx, y + dy] = (sbyte)-pixels[x + dx, y + dy];
				x += dx; y += dy;
				result = true;
			}
			return result;
		}


		// Ein Pixel ist isoliert, wenn es von dort aus keinen Weg mehr gibt
		private static bool IsIsolated(sbyte[,] pixels, sbyte color, int x, int y) {
			return
				pixels[x - 1, y - 1] != color && pixels[x - 1, y - 1] != jokerColor
				&& pixels[x, y - 1] != color && pixels[x, y - 1] != jokerColor
				&& pixels[x + 1, y - 1] != color && pixels[x + 1, y - 1] != jokerColor
				&& pixels[x - 1, y] != color && pixels[x - 1, y] != jokerColor
				&& pixels[x + 1, y] != color && pixels[x + 1, y] != jokerColor
				&& pixels[x - 1, y + 1] != color && pixels[x - 1, y + 1] != jokerColor
				&& pixels[x, y + 1] != color && pixels[x, y + 1] != jokerColor
				&& pixels[x + 1, y + 1] != color && pixels[x + 1, y + 1] != jokerColor;
		}


		private List<Point> pixels = new List<Point>(1000);

	}


	/// <summary>
	/// Describes a drawing as sequence of strokes.
	/// </summary>
	public class StrokeSequence {

		/// <ToBeCompleted></ToBeCompleted>
		public void Add(Stroke stroke) {
			strokes.Add(stroke);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public Stroke this[int index] {
			get { return strokes[index]; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public int Count {
			get { return strokes.Count; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void Clear() {
			strokes.Clear();
		}


		/// <ToBeCompleted></ToBeCompleted>
		public IEnumerator<Stroke> GetEnumerator() {
			foreach (Stroke s in strokes)
				yield return s;
		}


		private List<Stroke> strokes = new List<Stroke>(10);
	}


	/// <ToBeCompleted></ToBeCompleted>
	public enum Bowing {
		/// <ToBeCompleted></ToBeCompleted>
		Left,
		/// <ToBeCompleted></ToBeCompleted>
		Straight,
		/// <ToBeCompleted></ToBeCompleted>
		Right,
		/// <ToBeCompleted></ToBeCompleted>
		Wave 
	};


	/// <summary>
	/// Describes a geometric primitive such as circle, rectangle, triangle or, in general, a path.
	/// </summary>
	public class FigureShape {

		/// <ToBeCompleted></ToBeCompleted>
		public FigureShape(string typeName) {
			if (typeName == null) throw new ArgumentNullException("typeName");
			this.typeName = typeName;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public string TypeName {
			get { return typeName; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public virtual string Description {
			get { return TypeName; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public byte Matches(FigureShape other) {
			if (other == null) throw new ArgumentNullException("other");
			return (byte)(other.TypeName == TypeName ? 100 : 0);
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected Point[] points;
		private string typeName;

	}


	/// <summary>
	/// Describes a sequence of coordinates with straight or bowed lines between.
	/// </summary>
	public class PathFigureShape : FigureShape {


		/// <ToBeCompleted></ToBeCompleted>
		public class Line {

			internal Line(Bowing bowing) {
				this.bowing = bowing;
			}


			internal Bowing bowing;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public class Node {

			internal Node(int x, int y) {
				this.x = x;
				this.y = y;
			}


			internal int X {
				get { return x; }
			}


			internal int Y {
				get { return y; }
			}


			internal Line LeftLine {
				get { return leftLine; }
				set { leftLine = value; }
			}


			internal Line RightLine {
				get { return rightLine; }
				set { rightLine = value; }
			}


			private int x;
			private int y;
			private Line leftLine;
			private Line rightLine;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public PathFigureShape()
			: base("PathShape") {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public PathFigureShape(PathFigureShape source)
			: base("PathShape") {
			if (source == null) throw new ArgumentNullException("source");
			nodes = new List<Node>(source.nodes);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public Node this[int index] {
			get { return nodes[index]; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void Clear() {
			nodes.Clear();
		}


		/// <ToBeCompleted></ToBeCompleted>
		public Node LastNode {
			get { return nodes.Count > 0 ? nodes[nodes.Count - 1] : null; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public bool IsClosed {
			get {
				bool result = false;
				if (LastNode != null)
					result = LastNode.RightLine != null;
				return result;
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public Point[] CreateEnclosingRectangle() {
			Point[] result = new Point[2];
			result[0].X = int.MaxValue;
			result[0].Y = int.MaxValue;
			result[1].X = int.MinValue;
			result[1].Y = int.MinValue;
			foreach (Node n in nodes) {
				if (n.X < result[0].X) result[0].X = n.X;
				if (n.Y < result[0].Y) result[0].Y = n.Y;
				if (n.X > result[1].X) result[1].X = n.X;
				if (n.X > result[1].Y) result[1].Y = n.Y;
			}
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public List<Node> nodes = new List<Node>(20);

	}


	/// <summary>
	/// Beschreibt ein Dreieck
	/// </summary>
	public class TriangleFigureShape : FigureShape {

		/// <ToBeCompleted></ToBeCompleted>
		public static TriangleFigureShape CreateFromPath(PathFigureShape path) {
			if (path == null) throw new ArgumentNullException("path");
			TriangleFigureShape result = null;
			if (path.IsClosed && path.nodes.Count == 3) {
				bool isTriangleShape = true;
				for (int i = 0; i < 3; ++i) {
					if (path.nodes[i].RightLine.bowing != Bowing.Straight) {
						isTriangleShape = false;
						break;
					}
				}
				if (isTriangleShape)
					result = new TriangleFigureShape(path[0].X, path[0].Y, path[1].X, path[1].Y, path[2].X, path[2].Y);
			}
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public TriangleFigureShape(int x1, int y1, int x2, int y2, int x3, int y3)
			: base("TriangleShape") {
			points = new Point[3];
			points[0].X = x1;
			points[0].Y = y1;
			points[1].X = x2;
			points[1].Y = y2;
			points[2].X = x3;
			points[2].Y = x3;
		}


		/// <summary>
		/// Stellt, fest ob das Dreieck gleichseitig ist.
		/// </summary>
		public byte IsEquilateral {
			get {
				// Alle Seiten ungefähr gleich lang
				return Fuzzy.And(AreEdgesAboutEqual(0, 1), AreEdgesAboutEqual(1, 2), AreEdgesAboutEqual(2, 0));
			}
		}


		/// <summary>
		/// Stellt fest, ob das Dreieck gleichschenklig ist.
		/// Wert 100 = sicher, Wert 0 = keinesfalls
		/// </summary>
		public byte IsIsosceles {
			get {
				return Fuzzy.Or(AreEdgesAboutEqual(0, 1), AreEdgesAboutEqual(1, 2), AreEdgesAboutEqual(2, 0));
			}
		}


		/// <summary>
		/// Stellt fest, of das Dreieck rechtwinklig ist
		/// Wert 100 = sicher, Wert 0 = keinesfalls
		/// </summary>
		public byte IsRight {
			get {
				return Fuzzy.Or(IsRightAngle(0, 1), IsRightAngle(1, 2), IsRightAngle(2, 0));
			}
		}


		/// <override></override>
		public override string Description {
			get {
				string result = "TriangleShape";
				if (IsEquilateral > 80) result = "Equilateral triangle";
				else {
					byte isIsosceles = IsIsosceles;
					byte isRight = IsRight;
					if (isIsosceles > 80 && isIsosceles > isRight) result = "Isosceles triangle";
					if (isRight > 80) result = "right triangle";
				}
				return result;
			}
		}


		private int GetEdgeLength(int edge) {
			int result;
			switch (edge) {
				case 0: result = (int)Geometry.DistancePointPoint(points[0], points[1]); break;
				case 1: result = (int)Geometry.DistancePointPoint(points[1], points[2]); break;
				case 2: result = (int)Geometry.DistancePointPoint(points[2], points[0]); break;
				default: throw new Exception("NotSupported edge in TriangleShape.CalcEdgeLength");
			}
			return result;
		}


		private byte AreEdgesAboutEqual(int edge1, int edge2) {
			int v = (int)((200.0 * Math.Abs(GetEdgeLength(edge1) - GetEdgeLength(edge2))) / (GetEdgeLength(edge1) + GetEdgeLength(edge2)));
			if (v >= 20) return 0;
			else return (byte)(100 * (20 - v) / 20);
		}


		private byte IsRightAngle(int edge1, int edge2) {
			// Berechnen, wie weit sich die tatsächliche Seitenlänge
			// von der Hypothenusenlänge unterscheidet
			int actualEdgeLength;
			int requiredEdgeLength;
			if (edge1 == 0 && edge2 == 1 || edge1 == 1 && edge2 == 0) {
				actualEdgeLength = GetEdgeLength(2);
				requiredEdgeLength = (int)Math.Sqrt((GetEdgeLength(0) * GetEdgeLength(0)) + (GetEdgeLength(1) * GetEdgeLength(1)));
			}
			else if (edge1 == 1 && edge2 == 2 || edge1 == 2 && edge2 == 1) {
				actualEdgeLength = GetEdgeLength(0);
				requiredEdgeLength = (int)Math.Sqrt((GetEdgeLength(1) * GetEdgeLength(1)) + (GetEdgeLength(2) * GetEdgeLength(1)));
			}
			else if (edge1 == 2 && edge2 == 0 || edge1 == 0 && edge2 == 2) {
				actualEdgeLength = GetEdgeLength(1);
				requiredEdgeLength = (int)Math.Sqrt((GetEdgeLength(2) * GetEdgeLength(2)) + (GetEdgeLength(0) * GetEdgeLength(0)));
			}
			else throw new Exception("NotSupported edgeds in TriangleShape.IsRightAngle");
			// Bei 30% geben wir 0 bei 0% 100.
			int v = 100 * Math.Abs(actualEdgeLength - requiredEdgeLength) / actualEdgeLength;
			if (v >= 30) return 0;
			else return (byte)(100 * (30 - v) / 30);
		}

	}


	/// <summary>
	/// Beschreibt ein Viereck
	/// </summary>
	public class QuadrangleFigureShape : FigureShape {

		/// <ToBeCompleted></ToBeCompleted>
		public static QuadrangleFigureShape CreateFromPath(PathFigureShape path) {
			if (path == null) throw new ArgumentNullException("path");
			QuadrangleFigureShape result = null;
			// Ein Viereckt hat vier Ecken mit geraden Seiten
			if (path.IsClosed && path.nodes.Count == 4) {
				bool isQuadrangleShape = true;
				for (int i = 0; i < 3; ++i) {
					if (path.nodes[i].RightLine.bowing != Bowing.Straight) {
						isQuadrangleShape = false;
						break;
					}
				}
				if (isQuadrangleShape)
					result = new QuadrangleFigureShape(path[0].X, path[0].Y, path[1].X, path[1].Y, path[2].X, path[2].Y, path[3].X, path[3].Y);
			}
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public QuadrangleFigureShape(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4)
			: base("QuadrangleShape") {
			points = new Point[4];
			points[0].X = x1;
			points[0].Y = y1;
			points[1].X = x2;
			points[1].Y = y2;
			points[2].Y = x3;
			points[2].X = y3;
			points[3].Y = x4;
			points[3].X = y4;
		}

	}


	/// <summary>
	/// Beschreibt eine Ellipse
	/// </summary>
	public class EllipseFigureShape : FigureShape {

		/// <ToBeCompleted></ToBeCompleted>
		public static EllipseFigureShape CreateFromPath(PathFigureShape path) {
			if (path == null) throw new ArgumentNullException("path");
			EllipseFigureShape result = null;
			if (path.IsClosed) {
				if (path.nodes.Count <= 2)
					result = new EllipseFigureShape(path.CreateEnclosingRectangle());
				else {
					bool isCircle = true;
					Bowing bowing = Bowing.Straight;
					// Wenn mehrere Ecken vorhanden sind aber alle Verbindungen in die selbe Richtung gekrümmt
					for (int i = 1; i < path.nodes.Count; ++i) {
						switch (path.nodes[i].RightLine.bowing) {
							case Bowing.Wave:
							case Bowing.Straight:
								isCircle = false;
								break;
							case Bowing.Left:
								if (bowing == Bowing.Right) isCircle = false;
								else bowing = Bowing.Left;
								break;
							case Bowing.Right:
								if (bowing == Bowing.Left) isCircle = false;
								else bowing = Bowing.Right;
								break;
							default: Debug.Fail("Unexpected bowing in Circle.CreateFromPath"); isCircle = false; break;
						}
						if (!isCircle) break;
					}
					if (isCircle) result = new EllipseFigureShape(path.CreateEnclosingRectangle());
				}
			}
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public EllipseFigureShape(int x1, int y1, int x2, int y2)
			: base("EllipseShape") {
			points = new Point[2];
			points[0].X = x1;
			points[0].Y = y1;
			points[1].X = x2;
			points[1].Y = y2;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public EllipseFigureShape(Point[] points)
			: base("EllipseShape") {
			this.points = new Point[points.Length];
			Array.Copy(points, this.points, this.points.Length);
		}

	}


	/// <summary>
	/// Beschreibt eine gerade Linie.
	/// </summary>
	public class LineFigureShape : FigureShape {

		/// <ToBeCompleted></ToBeCompleted>
		public static LineFigureShape CreateFromPath(PathFigureShape path) {
			if (path == null) throw new ArgumentNullException("path");
			LineFigureShape result = null;
			if (!path.IsClosed && path.nodes.Count == 2)
				result = new LineFigureShape(path[0].X, path[0].Y, path[1].X, path[1].Y, path[0].RightLine.bowing);
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public LineFigureShape(int x1, int y1, int x2, int y2, Bowing bowing)
			: base("LineShape") {
			points = new Point[2];
			points[0].X = x1;
			points[0].Y = y1;
			points[1].X = x2;
			points[1].Y = y2;
			this.bowing = bowing;
		}


		/// <override></override>
		public override string Description {
			get {
				string result;
				switch (bowing) {
					case Bowing.Straight: result = "Straight "; break;
					case Bowing.Left: result = "left bowed "; break;
					case Bowing.Right: result = "right bowed "; break;
					case Bowing.Wave: result = "Wave "; break;
					default: Debug.Fail("Unexpected bowing in Line.Description"); result = "ERROR"; break;
				}
				return result + "line";
			}
		}


		// Die Linien sind parallel, wenn der Abstand der Endpunkte von other ungefähr gleich ist
		/// <ToBeCompleted></ToBeCompleted>
		public byte IsParallelTo(LineFigureShape other) {
			if (other == null) throw new ArgumentNullException("other");
			float d0 = CalcDistance(other.points[0]);
			float d1 = CalcDistance(other.points[1]);
			return Fuzzy.MapToFuzzy(-Math.Abs((d1 - d0) / d0), -1, -0.1F);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public byte Crosses(LineFigureShape line) {
			if (line == null) throw new ArgumentNullException("line");
			// TODO: Implement
			return 0;
		}


		// Berechnet den Abstand des Punktes von der Linie
		/// <ToBeCompleted></ToBeCompleted>
		public float CalcDistance(Point point) {
			return Geometry.DistancePointLine(point, points[0], points[1], true);
		}


		private Bowing bowing;

	}


	/// <summary>
	/// Describes a multi segment line.
	/// </summary>
	public class MultiLineFigureShape : FigureShape {

		/// <ToBeCompleted></ToBeCompleted>
		public static MultiLineFigureShape CreateFromPath(PathFigureShape path) {
			if (path == null) throw new ArgumentNullException("path");
			MultiLineFigureShape result = null;
			if (!path.IsClosed && path.nodes.Count > 2) {
				Point[] points = new Point[path.nodes.Count];
				for (int i = 0; i < path.nodes.Count; ++i) {
					points[i].X = path[i].X;
					points[i].Y = path[i].Y;
				}
				result = new MultiLineFigureShape(points);
			}
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public MultiLineFigureShape(Point[] points)
			: base(String.Format("Multiline with {0} segments", (points != null) ? points.Length - 1 : 0)) {
			if (points == null) throw new ArgumentNullException("points");
			this.points = (Point[])points.Clone();
		}

	}


	/// <ToBeCompleted></ToBeCompleted>
	public enum FigureShapeRelationType {
		/// <ToBeCompleted></ToBeCompleted>
		Crosses,
		/// <ToBeCompleted></ToBeCompleted>
		ParallelTo,
		/// <ToBeCompleted></ToBeCompleted>
		InsideCenter,
		/// <ToBeCompleted></ToBeCompleted>
		Above,
		/// <ToBeCompleted></ToBeCompleted>
		Below,
		/// <ToBeCompleted></ToBeCompleted>
		Left,
		/// <ToBeCompleted></ToBeCompleted>
		Right,
		/// <ToBeCompleted></ToBeCompleted>
		Undefined
	};


	/// <summary>
	/// Describes the relation between two shapes.
	/// </summary>
	public class FigureShapeRelation {

		/// <ToBeCompleted></ToBeCompleted>
		public static FigureShapeRelation CreateFromShapes(FigureShape referenceShape, FigureShape relatedShape) {
			if (referenceShape == null) throw new ArgumentNullException("referenceShape");
			if (relatedShape == null) throw new ArgumentNullException("relatedShape");
			FigureShapeRelation result = null;
			if (IsValidRelation(referenceShape, relatedShape, FigureShapeRelationType.ParallelTo) > 70)
				result = new FigureShapeRelation(referenceShape, relatedShape, FigureShapeRelationType.ParallelTo);
			else if (IsValidRelation(referenceShape, relatedShape, FigureShapeRelationType.Crosses) > 70)
				result = new FigureShapeRelation(referenceShape, relatedShape, FigureShapeRelationType.Crosses);
			return result;
		}


		private static byte IsValidRelation(FigureShape referenceShape, FigureShape relatedShape, FigureShapeRelationType type) {
			byte result = 0;
			switch (type) {
				case FigureShapeRelationType.ParallelTo:
					if (referenceShape is LineFigureShape && relatedShape is LineFigureShape)
						result = ((LineFigureShape)relatedShape).IsParallelTo((LineFigureShape)referenceShape);
					break;
			}
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public FigureShapeRelation(FigureShape referenceShape, FigureShape relatedShape, FigureShapeRelationType type) {
			if (referenceShape == null) throw new ArgumentNullException("referenceShape");
			if (relatedShape == null) throw new ArgumentNullException("relatedShape");
			this.referenceShape = referenceShape;
			this.relatedShape = relatedShape;
			this.type = type;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public FigureShapeRelation CloneFor(FigureShape referenceShape, FigureShape relatedShape) {
			if (referenceShape == null) throw new ArgumentNullException("referenceShape");
			if (relatedShape == null) throw new ArgumentNullException("relatedShape");
			return new FigureShapeRelation(referenceShape, relatedShape, type);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public FigureShape ReferenceShape {
			get { return referenceShape; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public FigureShape RelatedShape {
			get { return relatedShape; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public FigureShapeRelationType Type {
			get { return type; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public byte IsValid() {
			return IsValidRelation(referenceShape, relatedShape, type);
		}


		private FigureShape referenceShape;
		private FigureShape relatedShape;
		private FigureShapeRelationType type;

	}


	/// <summary>
	/// Describes a figure as collection of related shapes
	/// </summary>
	public class Figure {

		/// <ToBeCompleted></ToBeCompleted>
		public IList<FigureShape> Shapes {
			get { return shapes; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public IList<FigureShapeRelation> Relations {
			get { return relations; }
		}


		internal void Add(FigureShape shape) {
			shapes.Add(shape);
		}


		internal void Add(FigureShapeRelation relation) {
			relations.Add(relation);
		}


		/// <summary>
		/// Bestimmt, ob die andere Figur dieser hier gleicht. Dabei ist diese
		/// die Referenzfigur und die andere die Testfigur.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		// Jeder Shape in der anderern Figur muss einem Shape in dieser Figur entsprechen
		// und es darf kein Shape übrigbleiben. Jede Relation in der Referenzfigur muss in
		// der Testfigur enthalten sein oder auch mehr.
		// Wenn es mehrere mögliche Shape-Mappings gibt, muss man alle testen und eines suchen,
		// wo die Relationen erfüllt sind, falls möglich.
		internal byte Matches(Figure other) {
			byte result = 0;
			if (other.shapes.Count != shapes.Count) return result;
			// Enthält an Index i die Figur aus other, die auf die eigene Figur mit dem Index i passt.
			List<FigureShape> shapeMapping = new List<FigureShape>();
			return Matches(shapeMapping, other);
		}


		internal byte Matches(List<FigureShape> shapeMapping, Figure other) {
			byte result;
			if (shapeMapping.Count >= shapes.Count) {
				// Falls schon alle selectedShapes gemappt sind, die Relationen testen
				result = 100;
				foreach (FigureShapeRelation sr in relations) {
					FigureShape otherReferenceShape = shapeMapping[shapes.IndexOf(sr.ReferenceShape)];
					FigureShape otherRelatedShape = shapeMapping[shapes.IndexOf(sr.RelatedShape)];
					FigureShapeRelation relation = sr.CloneFor(otherReferenceShape, otherRelatedShape);
					result = Fuzzy.And(result, relation.IsValid());
				}
			}
			else {
				// Den nächsten eigenen Shape abbilden
				result = 0;
				int i = shapeMapping.Count;
				shapeMapping.Add(null);
				for (int j = 0; j < other.shapes.Count; ++j) {
					if (shapeMapping.IndexOf(other.shapes[j]) < 0 && shapes[i].Matches(other.shapes[j]) > 80) {
						shapeMapping[i] = other.shapes[j];
						result = Matches(shapeMapping, other);
						if (result > 70) break;
					}
				}
				shapeMapping.RemoveAt(i);
			}
			return result;
		}


		private List<FigureShape> shapes = new List<FigureShape>(10);
		private List<FigureShapeRelation> relations = new List<FigureShapeRelation>(10);
	}


	/// <summary>
	/// Manages a list of registered figures and determines the most akin one.
	/// </summary>
	public class Shaper {

		/// <ToBeCompleted></ToBeCompleted>
		public void RegisterFigure(string name, Bitmap bitmap) {
			if (bitmap != null) {
				StrokeSequence strokeSet = CreateStrokeSetFromBitmap(bitmap);
				if (strokeSet.Count <= 0) throw new Exception(String.Format("The bitmap for figure '{0}' is invalid", name));
				figureList.Add(new NamedFigure(name, IdentifyShapes(strokeSet)));
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public bool IsRegistered(string name) {
			foreach (NamedFigure figure in figureList)
				if (string.Compare(figure.name, name, StringComparison.InvariantCultureIgnoreCase) == 0)
					return true;
			return false;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void RegisterShapeSet(string name, Figure figure) {
			if (figure == null) throw new ArgumentNullException("figure");
			figureList.Add(new NamedFigure(name, figure));
		}


		// Liefert die Figur aus der Liste der registrierten Figure, die figure entspricht.
		/// <ToBeCompleted></ToBeCompleted>
		public Figure FindFigure(Figure figure) {
			if (figure == null) throw new ArgumentNullException("figure");
			Figure result = null;
			byte bestFuzzyValue = 0;
			foreach (NamedFigure nf in figureList) {
				byte fuzzyValue = nf.figure.Matches(figure);
				if (fuzzyValue > 70 && fuzzyValue > bestFuzzyValue) {
					result = nf.figure;
					bestFuzzyValue = fuzzyValue;
				}
			}
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public IEnumerable<string> GetFigureNames(Figure figure) {
			if (figure == null) throw new ArgumentNullException("figure");
			for (int i = 0; i < figureList.Count; ++i) {
				if (figureList[i].figure == figure)
					yield return figureList[i].name;
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public Figure IdentifyShapes(StrokeSequence strokes) {
			if (strokes == null) throw new ArgumentNullException("strokes");
			Figure result = new Figure();
			PathFigureShape polygone = new PathFigureShape();
			foreach (Stroke s in strokes) {
				polygone.Clear();
				CreatePolygone(s, polygone);
				FigureShape figureShape = null;
				if (polygone.IsClosed) {
					if (figureShape == null) figureShape = EllipseFigureShape.CreateFromPath(polygone);
					if (figureShape == null) figureShape = TriangleFigureShape.CreateFromPath(polygone);
					if (figureShape == null) figureShape = QuadrangleFigureShape.CreateFromPath(polygone);
				}
				else {
					if (figureShape == null) figureShape = LineFigureShape.CreateFromPath(polygone);
					if (figureShape == null) figureShape = MultiLineFigureShape.CreateFromPath(polygone);
				}
				if (figureShape == null) figureShape = new PathFigureShape(polygone);
				result.Add(figureShape);
				//
				if (result.Shapes.Count > 1) {
					// Beziehung zwischen dem neuen Shape und den anderen herstellen
					for (int si = 0; si < result.Shapes.Count - 1; ++si) {
						FigureShapeRelation relation = FigureShapeRelation.CreateFromShapes(result.Shapes[si], figureShape);
						if (relation != null) result.Add(relation);
					}
				}
			}
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void CreatePolygone(Stroke stroke, PathFigureShape polygone) {
			if (stroke == null) throw new ArgumentNullException("stroke");
			if (polygone == null) throw new ArgumentNullException("polygone");
			int index = 0;
			if (stroke.Count > 0) {
				do {
					polygone.nodes.Add(new PathFigureShape.Node(stroke[index].X, stroke[index].Y));
					polygone.LastNode.RightLine = CreateLine(stroke, ref index);
				} while (index < stroke.Count - 1);
				// Falls der Polygonzug geschlossen ist, machen wir eine Figur daraus
				// Geschlossen heißt Endpunkt innerhalb von 10 Pixeln vom Ausgangspunkt
				if (Geometry.DistancePointPoint(stroke[index], stroke[0]) < 10) {
					// Geschlossene Figur
				}
				else {
					polygone.nodes.Add(new PathFigureShape.Node(stroke[index].X, stroke[index].Y));
				}
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public StrokeSequence CreateStrokeSetFromBitmap(Bitmap bitmap) {
			if (bitmap == null) throw new ArgumentNullException("bitmap");
			// Wir lassen einen leeren Rand
			sbyte[,] pixels = new sbyte[bitmap.Width + 2, bitmap.Height + 2];
			List<Color> colorsUsed = new List<Color>();
			for (int i = 0; i < bitmap.Width; ++i) {
				for (int j = 0; j < bitmap.Height; ++j) {
					Color color = bitmap.GetPixel(i, j);
					if (color.ToArgb() == Color.Black.ToArgb())
						pixels[i + 1, j + 1] = Stroke.jokerColor;
					else if (color.ToArgb() == Color.White.ToArgb())
						pixels[i + 1, j + 1] = 0;
					else {
						sbyte colorIndex = (sbyte)colorsUsed.FindIndex(delegate(Color c) { return c.ToArgb() == color.ToArgb(); });
						if (colorIndex < 0) {
							if (colorIndex + 1 >= Stroke.jokerColor) throw new Exception("Image must be restricted to 126 colors");
							colorIndex = (sbyte)colorsUsed.Count;
							colorsUsed.Add(color);
						}
						pixels[i + 1, j + 1] = (sbyte)(colorIndex + 1);
					}
				}
			}
			StrokeSequence result = new StrokeSequence();
			for (sbyte c = 1; c <= colorsUsed.Count; ++c)
				result.Add(Stroke.CreateStroke(pixels, c));
			return result;
		}


		/// <summary>
		/// Extrahiert die nächste Linie aus dem Pixeln
		/// </summary>
		/// <param name="stroke"></param>
		/// <param name="index">Index der Startecke für die Linie</param>
		/// <returns></returns>
		private PathFigureShape.Line CreateLine(Stroke stroke, ref int index) {
			const int backLength = 20; // Muss immer eine gerade Zahl sein
			const double maxAngleDiff = 2 * Math.PI / 9; // 40°
			PathFigureShape.Line result = null;
			int startIndex = index;
			int firstEdgeIndex = -1;
			while (index < stroke.Count - 1) {
				++index;
				if (index - startIndex >= backLength) {
					// Zuerst die Pixel-Positionen relativ zum Mittelpunkt
					int firstPixelX = stroke[index - backLength].X - stroke[index - backLength / 2].X;
					int firstPixelY = stroke[index - backLength].Y - stroke[index - backLength / 2].Y;
					int currPixelX = stroke[index].X - stroke[index - backLength / 2].X;
					int currPixelY = stroke[index].Y - stroke[index - backLength / 2].Y;
					// Angle ist der Winkel zwischen dem vordern Teil und der geraden Fortsetzung
					// D.h. wenns gerade geht 0°, rechtwinklig nach links 90° und rechtwinklig nach rechts -90°
					// Winkel in Laufrichtung, normalisiert
					double firstAngle = Math.Atan2(firstPixelY, firstPixelX) - Math.PI;
					if (firstAngle <= -Math.PI) firstAngle += 2 * Math.PI;
					if (firstAngle > Math.PI) firstAngle -= 2 * Math.PI;
					double currAngle = Math.Atan2(currPixelY, currPixelX);
					double angle = firstAngle - currAngle;
					if (angle <= -Math.PI) angle += 2 * Math.PI;
					if (angle > Math.PI) angle -= 2 * Math.PI;
					if (angle > maxAngleDiff || angle < -maxAngleDiff) {
						// Es ist eine Ecke
						if (firstEdgeIndex < 0) firstEdgeIndex = index - backLength / 2; // Index des ersten in Frage kommenden Eckpunktes
					}
					else if (firstEdgeIndex >= 0) {
						// Wir haben die Ecke gefunden, die Ecke ist in der Mitte zwischen firstEdgeOffset und dem aktuellen index - 3
						int edgeIndex = (firstEdgeIndex + index - 1 - backLength / 2) / 2;
						// Neuer Standort ist die Ecke
						index = edgeIndex;
						result = new PathFigureShape.Line(Bowing.Straight);
						break;
					}
				}
			}
			if (result == null) {
				// index == stroke.Count -1: Die letzte Linie
				result = new PathFigureShape.Line(Bowing.Straight);
			}
			// *** Die Krümmung der Linie ermitteln ***
			float mdl = 0; // Maximaler Abstand nach links
			float mdr = 0; // Maximaler Abstand nach rechts
			for (int i = startIndex + 1; i < index; ++i) {
				float d = Geometry.DistancePointLine(stroke[i], stroke[startIndex], stroke[index], false);
				if (d < mdr) mdr = d; else if (d > mdl) mdl = d;
			}
			// Wenn beide Abstände klein sind, Gerade
			float length = Geometry.DistancePointPoint(stroke[index], stroke[startIndex]);
			if (Math.Abs(mdr) / length < 0.1 && Math.Abs(mdl) / length < 0.1)
				result.bowing = Bowing.Straight;
			// Wenn einer mittel ist und der andere 0, leicht gebogen
			else if (Math.Abs(mdr) / length < 0.1 && Math.Abs(mdl) / length >= 0.1)
				result.bowing = Bowing.Left;
			else if (Math.Abs(mdl) / length < 0.1 && Math.Abs(mdr) / length >= 0.1)
				result.bowing = Bowing.Right;
			// Wenn beide mindestens mittel sind, Wellenlinie
			else result.bowing = Bowing.Wave;
			//
			return result;
		}


		/// <summary>
		/// Translates the bitmap into a pixel array.
		/// </summary>
		private struct NamedFigure : IEquatable<NamedFigure> {

			/// <ToBeCompleted></ToBeCompleted>
			public NamedFigure(string name, Figure figure) {
				this.name = name;
				this.figure = figure;
			}


			/// <ToBeCompleted></ToBeCompleted>
			public string name;

			/// <ToBeCompleted></ToBeCompleted>
			public Figure figure;


			public bool Equals(NamedFigure other) {
				return (other.figure == this.figure
					&& other.name == this.name);
			}
		}


		private List<NamedFigure> figureList = new List<NamedFigure>();
	}
}
