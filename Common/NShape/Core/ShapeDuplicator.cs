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
using System.Diagnostics;

using Dataweb.NShape.Advanced;


namespace Dataweb.NShape {

	/// <summary>
	/// Helper class used for cloning shapes including model object(s), all children and their model object(s).
	/// </summary>
	public class ShapeDuplicator {

		static ShapeDuplicator() {
			modelObjectClones = new Dictionary<IModelObject, IModelObject>();
		}


		/// <summary>
		/// Creates a clone of both, shape and model object.
		/// </summary>
		public static Shape CloneShapeAndModelObject(Shape shape) {
			if (shape == null) throw new ArgumentNullException("shape");
			modelObjectClones.Clear();
			return DoCloneShape(shape, true);
		}


		/// <summary>
		/// Creates for each shape a clone of both, the shape and it's model object.
		/// </summary>
		public static IEnumerable<Shape> CloneShapeAndModelObject(IEnumerable<Shape> shapes) {
			if (shapes == null) throw new ArgumentNullException("shapes");
			modelObjectClones.Clear();
			foreach (Shape s in shapes)
				yield return DoCloneShape(s, true);
		}


		/// <summary>
		/// Creates a clone of the given shape. The clone references the source' model object.
		/// </summary>
		public static Shape CloneShapeOnly(Shape shape) {
			if (shape == null) throw new ArgumentNullException("shape");
			modelObjectClones.Clear();
			return DoCloneShape(shape, false);
		}


		/// <summary>
		/// Creates a clone of the given shapes. The clones reference their source' model object.
		/// </summary>
		public static IEnumerable<Shape> CloneShapesOnly(IEnumerable<Shape> shapes) {
			if (shapes == null) throw new ArgumentNullException("shapes");
			modelObjectClones.Clear();
			foreach (Shape s in shapes)
				yield return DoCloneShape(s, false);
		}


		/// <summary>
		/// Creates a clone of the given shape's model object.
		/// </summary>
		public static void CloneModelObjectOnly(Shape shape) {
			if (shape == null) throw new ArgumentNullException("shape");
			modelObjectClones.Clear();
			DoCloneShapeModelObject(shape);
		}


		/// <summary>
		/// Creates a clone of the given shape's model object.
		/// </summary>
		public static void CloneModelObjectsOnly(IEnumerable<Shape> shapes) {
			if (shapes == null) throw new ArgumentNullException("shapes");
			modelObjectClones.Clear();
			foreach (Shape shape in shapes)
				DoCloneShapeModelObject(shape);
		}


		private static Shape DoCloneShape(Shape shape, bool cloneModelObject) {
			Shape result = shape.Clone();
			if (cloneModelObject) DoCloneShapeModelObject(result);
			else {
				// ToDo: For now, we delete assigned model objects. Resolve this issue later ("Cannot delete modelObject if there is a copied shape referencing the model object")
				if (result.ModelObject != null) result.ModelObject = null;
			}
			return result;
		}
		
		
		private static void DoCloneShapeModelObject(Shape shape) {
			if (shape.Children.Count == 0 && shape.ModelObject != null) {
#if DEBUG_DIAGNOSTICS
				IModelObject clone = shape.ModelObject.Clone(); 
				Debug.Assert(clone.Parent == shape.ModelObject.Parent);
				shape.ModelObject = clone;
#else
				shape.ModelObject = shape.ModelObject.Clone();
#endif
			} else {
				CreateModelObjectClones(shape);
				if (modelObjectClones.Count > 0)
					AssignModelObjectClones(shape);
			}
		}


		private static void CreateModelObjectClones(Shape shape) {
			Debug.Assert(shape != null);
			if (shape.ModelObject != null) {
				modelObjectClones.Add(shape.ModelObject, shape.ModelObject.Clone());
				Debug.Assert(modelObjectClones[shape.ModelObject].Parent == shape.ModelObject.Parent);
				Debug.Assert(modelObjectClones[shape.ModelObject].Id == null);
			}
#if DEBUG_DIAGNOSTICS
			foreach (Shape childShape in shape.Children) {
				if (shape is IShapeGroup) {
					if (childShape.ModelObject != null && !modelObjectClones.ContainsKey(childShape.ModelObject))
						CreateModelObjectClones(childShape);
				} else {
					// Child shapes of aggregated shapes may not have model objects!
					Debug.Assert(childShape.ModelObject == null);
				}
			}
#endif
		}


		private static void AssignModelObjectClones(Shape shape) {
			Debug.Assert(shape != null);
			IModelObject mo = null;
			if (shape.ModelObject != null && modelObjectClones.TryGetValue(shape.ModelObject, out mo)) {
				if (shape.ModelObject.Parent != null 
					&& modelObjectClones.ContainsKey(shape.ModelObject.Parent)) {
					Debug.Print("Changing parent of '{0}' from '{1}' to '{2}'", mo.Name, shape.ModelObject.Parent.Name, modelObjectClones[shape.ModelObject.Parent].Name);
					mo.Parent = modelObjectClones[shape.ModelObject.Parent];
				}
				Debug.Print("Replacing shape's model object {0} with {1}", shape.ModelObject.Name, mo.Name);
				shape.ModelObject = mo;
			}
			if (shape is IShapeGroup) {
				foreach (Shape childShape in shape.Children)
					AssignModelObjectClones(childShape);
			}
		}


		private static Dictionary<IModelObject, IModelObject> modelObjectClones;
	}

}
