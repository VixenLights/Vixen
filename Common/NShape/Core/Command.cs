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
using System.Drawing;
using System.Reflection;

using Dataweb.NShape.Advanced;


namespace Dataweb.NShape.Commands {

	/// <summary>
	/// Encapsulates a command.
	/// </summary>
	public interface ICommand {

		/// <summary>
		/// Executes the command.
		/// </summary>
		void Execute();

		/// <summary>
		/// Reverts the command.
		/// </summary>
		void Revert();

		/// <summary>
		/// Tests whether the required permissions for the command are granted.
		/// </summary>
		bool IsAllowed(ISecurityManager securityManager);

		/// <summary>
		/// Tests whether the required permissions for the command are granted.
		/// </summary>
		/// <returns>
		/// Returns a <see cref="T:Dataweb.NShape.NShapeSecurityException"/> if the required permission is not granted.
		/// The exception can be thrown or used for retrieving a detailed description.
		/// </returns>
		Exception CheckAllowed(ISecurityManager securityManager);

		/// <summary>
		/// Specifies the cache on which the command will be executed.
		/// </summary>
		IRepository Repository { get; set; }

		/// <summary>
		/// Describes the purpose of the command.
		/// </summary>
		string Description { get; }
	}


	#region Base Classes

	/// <summary>
	/// Base class for all commands.
	/// </summary>
	public abstract class Command : ICommand {

		/// <override></override>
		protected Command() {
			// nothing to do here
		}


		/// <override></override>
		protected Command(IRepository repository)
			: this() {
			this.repository = repository;
		}


		/// <override></override>
		public abstract void Execute();


		/// <override></override>
		public abstract void Revert();


		/// <override></override>
		public abstract Permission RequiredPermission { get; }


		/// <override></override>
		public bool IsAllowed(ISecurityManager securityManager) {
			Exception result = null;
			return CheckAllowedCore(securityManager, false, out result);
		}


		/// <override></override>
		public Exception CheckAllowed(ISecurityManager securityManager) {
			Exception result = null;
			CheckAllowedCore(securityManager, true, out result);
			return result;
		}


		/// <override></override>
		public IRepository Repository {
			get { return repository; }
			set { repository = value; }
		}


		/// <override></override>
		public virtual string Description {
			get { return description; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected abstract bool CheckAllowedCore(ISecurityManager securityManager, bool createException, out Exception exception);


		/// <summary>
		/// Checks whether the given entity has to be undeleted instead of being inserted
		/// </summary>
		protected bool CanUndeleteEntity(IEntity entity) {
			if (entity == null) throw new ArgumentNullException();
			return entity.Id != null;
		}


		/// <summary>
		/// Checks whether the given entity has to be undeleted instead of being inserted
		/// </summary>
		protected bool CanInsertEntity(IEntity entity) {
			if (entity == null) throw new ArgumentNullException();
			return entity.Id == null;
		}


		/// <summary>
		/// Checks whether the given entity has to be undeleted instead of being inserted
		/// </summary>
		protected bool CanUndeleteEntities<TEntity>(IEnumerable<TEntity> entities) where TEntity : IEntity {
			if (entities == null) throw new ArgumentNullException();
			foreach (IEntity e in entities) return CanUndeleteEntity(e);
			return false;
		}


		#region Fields

		/// <ToBeCompleted></ToBeCompleted>
		protected string description;
		private IRepository repository;

		#endregion
	}


	/// <summary>
	/// Base class for shape specific commands
	/// </summary>
	public abstract class ShapeCommand : Command {

		/// <ToBeCompleted></ToBeCompleted>
		protected ShapeCommand(Shape shape)
			: this(null, shape) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected ShapeCommand(IRepository repository, Shape shape)
			: base(repository) {
			if (shape == null) throw new ArgumentNullException("shape");
			this.shape = shape;
		}


		/// <override></override>
		protected override bool CheckAllowedCore(ISecurityManager securityManager, bool createException, out Exception exception) {
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			bool isGranted = securityManager.IsGranted(RequiredPermission, SecurityAccess.Modify, shape);
			exception = (!isGranted && createException) ? new NShapeSecurityException(this) : null;
			return isGranted;
		}

		/// <ToBeCompleted></ToBeCompleted>
		protected Shape shape;
	}


	/// <summary>
	/// Base class for shape specific commands
	/// </summary>
	public abstract class ShapesCommand : Command {

		/// <ToBeCompleted></ToBeCompleted>
		protected ShapesCommand(IEnumerable<Shape> shapes)
			: this(null, shapes) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected ShapesCommand(IRepository repository, IEnumerable<Shape> shapes)
			: base(repository) {
			if (shapes == null) throw new ArgumentNullException("shapes");
			this.shapes = new List<Shape>(shapes);
		}


		/// <override></override>
		protected override bool CheckAllowedCore(ISecurityManager securityManager, bool createException, out Exception exception) {
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			bool isGranted = securityManager.IsGranted(RequiredPermission, shapes);
			exception = (!isGranted && createException) ? exception = new NShapeSecurityException(this) : null;
			return isGranted;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected List<Shape> shapes;
	}


	/// <summary>
	/// Base class for template specific commands
	/// </summary>
	public abstract class TemplateCommand : Command {

		/// <ToBeCompleted></ToBeCompleted>
		protected TemplateCommand(IRepository repository)
			: base(repository) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected TemplateCommand(Template template)
			: this(null, template) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected TemplateCommand(IRepository repository, Template template)
			: base(repository) {
			if (template == null) throw new ArgumentNullException("template");
			this.template = template;
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Templates; }
		}


		/// <override></override>
		protected override bool CheckAllowedCore(ISecurityManager securityManager, bool createException, out Exception exception) {
			if (securityManager == null) throw new ArgumentNullException("template");
			bool isGranted = securityManager.IsGranted(RequiredPermission);
			exception = (!isGranted && createException) ? new NShapeSecurityException(this) : null;
			return isGranted;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected Template template = null;
	}


	/// <summary>
	/// Base class for diagram specific commands
	/// </summary>
	public abstract class DiagramCommand : Command {

		/// <ToBeCompleted></ToBeCompleted>
		protected DiagramCommand(Diagram diagram)
			: this(null, diagram) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected DiagramCommand(IRepository repository, Diagram diagram)
			: base(repository) {
			if (diagram == null) throw new ArgumentNullException("Diagram");
			this.diagram = diagram;
		}


		/// <override></override>
		protected override bool CheckAllowedCore(ISecurityManager securityManager, bool createException, out Exception exception) {
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			bool isGranted = securityManager.IsGranted(RequiredPermission, diagram.SecurityDomainName);
			exception = (!isGranted && createException) ? new NShapeSecurityException(this) : null;
			return isGranted;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected Diagram diagram;
	}


	/// <summary>
	/// Base class for design specific commands
	/// </summary>
	public abstract class DesignCommand : Command {

		/// <ToBeCompleted></ToBeCompleted>
		protected DesignCommand(Design design)
			: this(null, design) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected DesignCommand(IRepository repository, Design design)
			: base(repository) {
			if (design == null) throw new ArgumentNullException("design");
			this.design = design;
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Designs; }
		}


		/// <override></override>
		protected override bool CheckAllowedCore(ISecurityManager securityManager, bool createException, out Exception exception) {
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			bool isGranted = securityManager.IsGranted(RequiredPermission);
			exception = (!isGranted && createException) ? new NShapeSecurityException(this) : null;
			return isGranted;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected Design design;
	}


	/// <summary>
	/// Base class for commands that need to disconnect connected shapes before the action may executed,
	/// e.g. a DeleteCommand has to disconnect the deleted shape before deleting it.
	/// </summary>
	public abstract class AutoDisconnectShapesCommand : Command {

		/// <ToBeCompleted></ToBeCompleted>
		protected AutoDisconnectShapesCommand()
			: this(null) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected AutoDisconnectShapesCommand(IRepository repository)
			: base(repository) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void Disconnect(IList<Shape> shapes) {
			for (int i = shapes.Count - 1; i >= 0; --i)
				Disconnect(shapes[i]);
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void Disconnect(Shape shape) {
			if (!connections.ContainsKey(shape))
				connections.Add(shape, new List<ShapeConnectionInfo>(shape.GetConnectionInfos(ControlPointId.Any, null)));
			foreach (ShapeConnectionInfo sci in connections[shape]) {
				if (shape.HasControlPointCapability(sci.OwnPointId, ControlPointCapabilities.Glue)) {
					shape.Disconnect(sci.OwnPointId);
					Repository.DeleteConnection(shape, sci.OwnPointId, sci.OtherShape, sci.OtherPointId);
				} else {
					sci.OtherShape.Disconnect(sci.OtherPointId);
					Repository.DeleteConnection(sci.OtherShape, sci.OtherPointId, shape, sci.OwnPointId);
				}
				sci.OtherShape.Invalidate();
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void Reconnect(IList<Shape> shapes) {
			// restore connections
			int cnt = shapes.Count;
			for (int i = 0; i < cnt; ++i)
				Reconnect(shapes[i]);
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void Reconnect(Shape shape) {
			// restore connections
			if (connections.ContainsKey(shape)) {
				foreach (ShapeConnectionInfo sci in connections[shape]) {
					if (shape is ILinearShape) {
						shape.Connect(sci.OwnPointId, sci.OtherShape, sci.OtherPointId);
						if (Repository != null) Repository.InsertConnection(shape, sci.OwnPointId, sci.OtherShape, sci.OtherPointId);
					} else {
						sci.OtherShape.Connect(sci.OtherPointId, shape, sci.OwnPointId);
						if (Repository != null) Repository.InsertConnection(sci.OtherShape, sci.OtherPointId, shape, sci.OwnPointId);
					}
				}
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected Dictionary<Shape, List<ShapeConnectionInfo>> ShapeConnections { get { return connections; } }


		private Dictionary<Shape, List<ShapeConnectionInfo>> connections = new Dictionary<Shape, List<ShapeConnectionInfo>>();
	}


	///// <summary>
	///// Base class for inserting and removing shapes to a diagram and a cache
	///// </summary>
	//public abstract class InsertOrRemoveShapeCommand : AutoDisconnectShapesCommand {

	//   protected InsertOrRemoveShapeCommand(Diagram diagram)
	//      : base() {
	//      if (diagram == null) throw new ArgumentNullException("diagram");
	//      this.diagram = diagram;
	//   }


	//   protected void InsertShapes(LayerIds activeLayers) {
	//      DoInsertShapes(false, activeLayers);
	//   }


	//   protected void InsertShapes() {
	//      DoInsertShapes(true, LayerIds.None);
	//   }


	//   protected void RemoveShapes() {
	//      if (Shapes.Count == 0) throw new NShapeInternalException("No shapes set. Call SetShapes() before.");

	//      // disconnect all selectedShapes connected to the deleted shape(s)
	//      Disconnect(Shapes);

	//      if (Shapes.Count > 1) {
	//         diagram.Shapes.RemoveRange(Shapes);
	//         if (Repository != null) Repository.DeleteShapes(Shapes);
	//      } else {
	//         diagram.Shapes.Remove(Shapes[0]);
	//         if (Repository != null) Repository.DeleteShape(Shapes[0]);
	//      }
	//   }


	//   protected void SetShapes(Shape shape) {
	//      if (shape == null) throw new ArgumentNullException("shape");
	//      if (shapeLayers == null) shapeLayers = new List<LayerIds>(1);
	//      else this.shapeLayers.Clear();
	//      this.Shapes.Clear();

	//      if (!this.Shapes.Contains(shape)) {
	//         this.Shapes.Add(shape);
	//         this.shapeLayers.Add(shape.Layers);
	//      }
	//   }


	//   protected void SetShapes(IEnumerable<Shape> shapes) {
	//      SetShapes(shapes, true);
	//   }


	//   protected void SetShapes(IEnumerable<Shape> shapes, bool invertSortOrder) {
	//      if (shapes == null) throw new ArgumentNullException("shapes");
	//      this.Shapes.Clear();
	//      if (shapeLayers == null) shapeLayers = new List<LayerIds>();
	//      else this.shapeLayers.Clear();

	//      foreach (Shape shape in shapes) {
	//         if (!this.Shapes.Contains(shape)) {
	//            if (invertSortOrder) {
	//               this.Shapes.Insert(0, shape);
	//               this.shapeLayers.Insert(0, shape.Layers);
	//            } else {
	//               this.Shapes.Add(shape);
	//               this.shapeLayers.Add(shape.Layers);
	//            }
	//         }
	//      }
	//   }


	//   protected List<Shape> Shapes = new List<Shape>();


	//   protected static string DeleteDescription = "Delete {0} shape{1}";


	//   protected static string CreateDescription = "Create {0} shape{1}";


	//   private void DoInsertShapes(bool useOriginalLayers, LayerIds activeLayers) {
	//      int startIdx = Shapes.Count - 1;
	//      if (startIdx < 0) throw new NShapeInternalException("No shapes set. Call SetShapes() before.");

	//      if (Repository == null) throw new ArgumentNullException("Repository"); 
	//      for (int i = startIdx; i >= 0; --i) {
	//         //if (Shapes[i].ZOrder == 0) 
	//            Shapes[i].ZOrder = Repository.ObtainNewTopZOrder(diagram);
	//         diagram.Shapes.Add(Shapes[i]);
	//         diagram.AddShapeToLayers(Shapes[i], useOriginalLayers ? shapeLayers[i] : activeLayers);
	//      }
	//      if (startIdx == 0)
	//         Repository.InsertShape(Shapes[0], diagram);
	//      else
	//         Repository.InsertShapes(Shapes, diagram);

	//      // connect all selectedShapes that were previously connected to the shape(s)
	//      Reconnect(Shapes);
	//   }


	//   private Diagram diagram = null;
	//   private List<LayerIds> shapeLayers;
	//}


	/// <ToBeCompleted></ToBeCompleted>
	public abstract class InsertOrRemoveModelObjectsCommand : Command {

		/// <override></override>
		protected InsertOrRemoveModelObjectsCommand()
			: this(null) {
		}


		/// <override></override>
		protected InsertOrRemoveModelObjectsCommand(IRepository repository)
			: base(repository) {
		}


		/// <override></override>
		protected override bool CheckAllowedCore(ISecurityManager securityManager, bool createException, out Exception exception) {
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			bool isGranted = true;
			foreach (KeyValuePair<IModelObject, AttachedObjects> pair in ModelObjects) {
				if (!IsAllowed(securityManager, pair.Value)) {
					isGranted = false;
					break;
				}
			}
			exception = (!isGranted && createException) ? new NShapeSecurityException(this) : null;
			return isGranted;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void SetModelObjects(IModelObject modelObject) {
			if (modelObject == null) throw new ArgumentNullException("modelObject");
			Debug.Assert(this.ModelObjects.Count == 0);
			ModelObjects.Add(modelObject, new AttachedObjects(modelObject, Repository));
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void SetModelObjects(IEnumerable<IModelObject> modelObjects) {
			if (modelObjects == null) throw new ArgumentNullException("modelObjects");
			Debug.Assert(this.ModelObjects.Count == 0);
			foreach (IModelObject modelObject in modelObjects)
				ModelObjects.Add(modelObject, new AttachedObjects(modelObject, Repository));
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void InsertModelObjects(bool insertShapes) {
			int cnt = ModelObjects.Count;
			if (cnt == 0) throw new NShapeInternalException("No ModelObjects set. Call SetModelObjects() before.");
			if (Repository != null) {
				if (CanUndeleteEntities(ModelObjects.Keys))
					Repository.Undelete(ModelObjects.Keys);
				else Repository.Insert(ModelObjects.Keys);
				foreach (KeyValuePair<IModelObject, AttachedObjects> item in ModelObjects)
					InsertAndAttachObjects(item.Key, item.Value, Repository, insertShapes);
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void RemoveModelObjects(bool deleteShapes) {
			if (ModelObjects.Count == 0) throw new NShapeInternalException("No ModelObjects set. Call SetModelObjects() before.");
			if (Repository != null) {
				foreach (KeyValuePair<IModelObject, AttachedObjects> item in ModelObjects)
					DetachAndDeleteObjects(item.Value, Repository, deleteShapes);
				Repository.Delete(ModelObjects.Keys);
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		internal Dictionary<IModelObject, AttachedObjects> ModelObjects = new Dictionary<IModelObject, AttachedObjects>();


		private void DetachAndDeleteObjects(AttachedObjects attachedObjects, IRepository repository, bool deleteShapes) {
			// Delete or detach shapes
			if (attachedObjects.Shapes.Count > 0) {
				if (deleteShapes) repository.DeleteAll(attachedObjects.Shapes);
				else {
					for (int sIdx = attachedObjects.Shapes.Count - 1; sIdx >= 0; --sIdx) {
						attachedObjects.Shapes[sIdx].ModelObject = null;
					}
					repository.Update(attachedObjects.Shapes);
				}
			}
			// Process children
			foreach (KeyValuePair<IModelObject, AttachedObjects> child in attachedObjects.Children)
				DetachAndDeleteObjects(child.Value, repository, deleteShapes);
			// Delete model object
			repository.Delete(attachedObjects.Children.Keys);
		}


		private void InsertAndAttachObjects(IModelObject modelObject, AttachedObjects attachedObjects, IRepository repository, bool insertShapes) {
			// Insert model objects
			if (CanUndeleteEntities(attachedObjects.Children.Keys))
				repository.Undelete(attachedObjects.Children.Keys);
			else repository.Insert(attachedObjects.Children.Keys);
			foreach (KeyValuePair<IModelObject, AttachedObjects> child in attachedObjects.Children) {
				InsertAndAttachObjects(child.Key, child.Value, repository, insertShapes);
				child.Key.Parent = modelObject;
			}
			repository.Update(attachedObjects.Children.Keys);
			// insert shapes
			if (attachedObjects.Shapes.Count > 0) {
				for (int sIdx = attachedObjects.Shapes.Count - 1; sIdx >= 0; --sIdx)
					attachedObjects.Shapes[sIdx].ModelObject = modelObject;
				if (insertShapes)
					throw new NotImplementedException();
				else repository.Update(attachedObjects.Shapes);
			}
		}


		private bool IsAllowed(ISecurityManager securityManager, AttachedObjects attachedObjects) {
			if (attachedObjects == null) return true;
			foreach (AttachedObjects obj in attachedObjects.Children.Values) {
				if (!IsAllowed(securityManager, obj))
					return false;
			}
			return securityManager.IsGranted(RequiredPermission, attachedObjects.Shapes);
		}

	}


	/// <ToBeCompleted></ToBeCompleted>
	internal class AttachedObjects {

		/// <ToBeCompleted></ToBeCompleted>
		public AttachedObjects(IModelObject modelObject, IRepository repository) {
			shapes = new List<Shape>();
			children = new Dictionary<IModelObject, AttachedObjects>();
			Add(modelObject, repository);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public List<Shape> Shapes {
			get { return shapes; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public Dictionary<IModelObject, AttachedObjects> Children {
			get { return children; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void Add(IModelObject modelObject, IRepository repository) {
			DoAdd(this, modelObject, repository);
		}


		private void DoAdd(AttachedObjects attachedObjects, IModelObject modelObject, IRepository repository) {
			attachedObjects.Shapes.AddRange(modelObject.Shapes);
			foreach (IModelObject child in repository.GetModelObjects(modelObject))
				attachedObjects.Children.Add(child, new AttachedObjects(child, repository));
		}


		private List<Shape> shapes;
		private Dictionary<IModelObject, AttachedObjects> children;
	}


	/// <summary>
	/// Base class for inserting and removing shapes along with their model objects to a diagram and a cache
	/// </summary>
	public abstract class InsertOrRemoveShapeCommand : AutoDisconnectShapesCommand {


		/// <ToBeCompleted></ToBeCompleted>
		protected InsertOrRemoveShapeCommand(Diagram diagram)
			: this(null, diagram) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected InsertOrRemoveShapeCommand(IRepository repository, Diagram diagram)
			: base(repository) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			this.diagram = diagram;
		}


		/// <override></override>
		protected override bool CheckAllowedCore(ISecurityManager securityManager, bool createException, out Exception exception) {
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			bool isGranted = securityManager.IsGranted(RequiredPermission, shapes);
			exception = (!isGranted && createException) ? new NShapeSecurityException(this) : null;
			return isGranted;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void InsertShapesAndModels(LayerIds activeLayers) {
			DoInsertShapesAndModels(false, activeLayers);
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void InsertShapesAndModels() {
			DoInsertShapesAndModels(true, LayerIds.None);
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void DeleteShapesAndModels() {
			if (Repository != null && ModelObjects != null && ModelObjects.Count > 0) {
				// Disconnect shapes as long as the model objects still exist.
				// If there are no model objects, the shapes will be disconnected in Remove()
				Disconnect(Shapes);
				if (modelsAndObjects == null) {
					modelsAndObjects = new Dictionary<IModelObject, AttachedObjects>();
					foreach (IModelObject modelObject in ModelObjects)
						modelsAndObjects.Add(modelObject, new AttachedObjects(modelObject, Repository));
				}
				foreach (KeyValuePair<IModelObject, AttachedObjects> item in modelsAndObjects)
					DetachAndDeleteObjects(item.Value, Repository);
				Repository.Delete(modelsAndObjects.Keys);
			}
			RemoveShapes();
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void InsertShapes(LayerIds activeLayers) {
			DoInsertShapes(false, activeLayers);
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void InsertShapes() {
			DoInsertShapes(true, LayerIds.None);
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void RemoveShapes() {
			if (Shapes.Count == 0) throw new NShapeInternalException("No shapes set. Call SetShapes() before.");
			// Disconnect all shapes connected to the deleted shape(s)
			Disconnect(Shapes);
			// Remove shapes
			diagram.Shapes.RemoveRange(Shapes);
			if (Repository != null) Repository.DeleteAll(Shapes);
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void SetShape(Shape shape, bool withModelObject) {
			if (shape == null) throw new ArgumentNullException("shape");
			if (shapeLayers == null) shapeLayers = new List<LayerIds>(1);
			else this.shapeLayers.Clear();
			this.Shapes.Clear();

			if (!this.Shapes.Contains(shape)) {
				this.Shapes.Add(shape);
				this.shapeLayers.Add(shape.Layers);

				if (shape.ModelObject != null && withModelObject)
					SetModelObject(shape.ModelObject);
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void SetShapes(IEnumerable<Shape> shapes, bool withModelObjects) {
			SetShapes(shapes, withModelObjects, false);
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void SetShapes(IEnumerable<Shape> shapes, bool withModelObjects, bool invertSortOrder) {
			if (shapes == null) throw new ArgumentNullException("shapes");
			this.Shapes.Clear();
			if (shapeLayers == null) shapeLayers = new List<LayerIds>();
			else this.shapeLayers.Clear();

			foreach (Shape shape in shapes) {
				if (!this.Shapes.Contains(shape)) {
					if (invertSortOrder) {
						this.Shapes.Insert(0, shape);
						this.shapeLayers.Insert(0, shape.Layers);
					} else {
						this.Shapes.Add(shape);
						this.shapeLayers.Add(shape.Layers);
					}
					if (shape.ModelObject != null && withModelObjects)
						SetModelObject(shape.ModelObject);
				}
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void SetModelObject(IModelObject modelObject) {
			if (modelObject == null) throw new ArgumentNullException("modelObject");
			if (this.modelObjects == null) this.modelObjects = new List<IModelObject>();
			if (!this.modelObjects.Contains(modelObject)) this.modelObjects.Add(modelObject);
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void SetModelObjects(IEnumerable<IModelObject> modelObjects) {
			if (modelObjects == null) throw new ArgumentNullException("modelObjects");
			if (this.modelObjects == null) this.modelObjects = new List<IModelObject>();
			foreach (IModelObject modelObject in modelObjects) {
				if (!this.modelObjects.Contains(modelObject))
					this.modelObjects.Add(modelObject);
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void SetModelObjects(IEnumerable<Shape> shapes) {
			if (shapes == null) throw new ArgumentNullException("shapes");
			foreach (Shape shape in shapes)
				if (shape.ModelObject != null) SetModelObject(shape.ModelObject);
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected List<Shape> Shapes {
			get { return shapes; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected List<IModelObject> ModelObjects {
			get { return modelObjects; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected string GetDescription(DescriptionType descType, Shape shape, bool withModelObject) {
			string modelString = string.Empty;
			if (withModelObject && shape.ModelObject != null)
				modelString = string.Format(WithModelsFormatStr, string.Empty, " " + shape.ModelObject.Type.Name);
			return string.Format(DescriptionFormatStr,
				GetDescTypeText(descType),
				shape.Type.Name,
				string.Empty,
				modelString);
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected string GetDescription(DescriptionType descType, IEnumerable<Shape> shapes, bool withModelObjects) {
			int shapeCnt = 0, modelCnt = 0;
			foreach (Shape s in shapes) {
				++shapeCnt;
				if (s.ModelObject != null) ++modelCnt;
			}
			if (shapeCnt == 1)
				foreach (Shape s in shapes)
					return GetDescription(descType, s, withModelObjects);

			string modelString = string.Empty;
			if (withModelObjects && modelCnt > 0)
				modelString = string.Format(WithModelsFormatStr, modelCnt.ToString() + " ", modelCnt > 1 ? "s" : string.Empty);
			return string.Format(DescriptionFormatStr,
				GetDescTypeText(descType),
				shapeCnt,
				"s",
				modelString);
		}


		private string GetDescTypeText(DescriptionType descType) {
			switch (descType) {
				case DescriptionType.Insert: return "Create";
				case DescriptionType.Delete: return "Delete";
				case DescriptionType.Cut: return "Cut";
				case DescriptionType.Paste: return "Paste";
				default:
					Debug.Fail("Unhandled switch case!");
					return descType.ToString();
			}
		}


		// Insert shapes only
		private void DoInsertShapes(bool useOriginalLayers, LayerIds activeLayers) {
			if (Shapes.Count == 0) throw new NShapeInternalException("No shapes set. Call SetShapes() before.");
			for (int i = Shapes.Count - 1; i >= 0; --i) {
				//Shapes[i].ZOrder = Repository.ObtainNewTopZOrder(diagram);
				diagram.Shapes.Add(Shapes[i]);
				diagram.AddShapeToLayers(Shapes[i], useOriginalLayers ? shapeLayers[i] : activeLayers);
			}
			if (Repository != null) {
				//// Insert ModelObjects
				//for (int i = modelObjects.Count - 1; i >= 0; --i) {
				//    if (CanUndeleteEntity(modelObjects[i])) Repository.Undelete(modelObjects[i]);
				//    else Repository.Insert(modelObjects[i]);
				//}
				// Insert shapes
				Repository.UndeleteAll(GetEntities<Shape>(Shapes, CanUndeleteEntity), diagram);
				Repository.InsertAll(GetEntities<Shape>(Shapes, CanInsertEntity), diagram);
			}
			// connect all selectedShapes that were previously connected to the shape(s)
			Reconnect(Shapes);
		}


		private IEnumerable<TEntity> GetEntities<TEntity>(IEnumerable<TEntity> entities, Predicate<TEntity> predicate) where TEntity : IEntity {
			foreach (TEntity entity in entities)
				if (predicate(entity)) yield return entity;
		}


		private void DoInsertShapesAndModels(bool useOriginalLayers, LayerIds activeLayers) {
			// Insert model objects first
			if (Repository != null && ModelObjects != null && ModelObjects.Count > 0) {
				if (modelsAndObjects == null) {
					modelsAndObjects = new Dictionary<IModelObject, AttachedObjects>();
					foreach (IModelObject modelObject in ModelObjects)
						modelsAndObjects.Add(modelObject, new AttachedObjects(modelObject, Repository));
				}
				if (CanUndeleteEntities(modelsAndObjects.Keys))
					Repository.Undelete(modelsAndObjects.Keys);
				else Repository.Insert(modelsAndObjects.Keys);
			}
			// Insert shapes afterwards
			if (useOriginalLayers) InsertShapes();
			else InsertShapes(activeLayers);
			// Attach model obejcts to shapes finally
			if (Repository != null && modelsAndObjects != null) {
				foreach (KeyValuePair<IModelObject, AttachedObjects> item in modelsAndObjects)
					InsertAndAttachObjects(item.Key, item.Value, Repository);
			}
		}


		private void DetachAndDeleteObjects(AttachedObjects attachedObjects, IRepository repository) {
			for (int sIdx = attachedObjects.Shapes.Count - 1; sIdx >= 0; --sIdx)
				attachedObjects.Shapes[sIdx].ModelObject = null;
			repository.Update(attachedObjects.Shapes);
			foreach (KeyValuePair<IModelObject, AttachedObjects> child in attachedObjects.Children)
				DetachAndDeleteObjects(child.Value, repository);
			repository.Delete(attachedObjects.Children.Keys);
		}


		private void InsertAndAttachObjects(IModelObject modelObject, AttachedObjects attachedObjects, IRepository repository) {
			if (CanUndeleteEntities(attachedObjects.Children.Keys))
				repository.Undelete(attachedObjects.Children.Keys);
			else repository.Insert(attachedObjects.Children.Keys);
			foreach (KeyValuePair<IModelObject, AttachedObjects> child in attachedObjects.Children) {
				InsertAndAttachObjects(child.Key, child.Value, repository);
				child.Key.Parent = modelObject;
			}
			repository.Update(attachedObjects.Children.Keys);
			for (int sIdx = attachedObjects.Shapes.Count - 1; sIdx >= 0; --sIdx)
				attachedObjects.Shapes[sIdx].ModelObject = modelObject;
			repository.Update(attachedObjects.Shapes);
			repository.Update(modelObject);
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected enum DescriptionType {
			/// <ToBeCompleted></ToBeCompleted>
			Insert,
			/// <ToBeCompleted></ToBeCompleted>
			Delete,
			/// <ToBeCompleted></ToBeCompleted>
			Cut,
			/// <ToBeCompleted></ToBeCompleted>
			Paste
		};


		private const string DescriptionFormatStr = "{0} {1} shape{2}{3}";
		private const string WithModelsFormatStr = " with {0}model{1}";

		private Diagram diagram = null;
		private List<Shape> shapes = new List<Shape>();
		private List<LayerIds> shapeLayers;
		private List<IModelObject> modelObjects = null;
		private Dictionary<IModelObject, AttachedObjects> modelsAndObjects = null;
	}


	/// <summary>
	/// Base class for Connecting and disconnecting two shapes
	/// </summary>
	public abstract class ConnectionCommand : Command {

		/// <ToBeCompleted></ToBeCompleted>
		protected ConnectionCommand(Shape connectorShape, ControlPointId gluePointId, Shape targetShape, ControlPointId targetPointId)
			: this(null, connectorShape, gluePointId, targetShape, targetPointId) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected ConnectionCommand(IRepository repository, Shape connectorShape, ControlPointId gluePointId, Shape targetShape, ControlPointId targetPointId)
			: base(repository) {
			if (connectorShape == null) throw new ArgumentNullException("connectorShape");
			if (targetShape == null) throw new ArgumentNullException("targetShape");
			this.connectorShape = connectorShape;
			this.gluePointId = gluePointId;
			this.targetShape = targetShape;
			this.targetPointId = targetPointId;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected ConnectionCommand(Shape connectorShape, ControlPointId gluePointId)
			: this(null, connectorShape, gluePointId) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected ConnectionCommand(IRepository repository, Shape connectorShape, ControlPointId gluePointId)
			: base(repository) {
			if (connectorShape == null) throw new ArgumentNullException("connectorShape");
			this.connectorShape = connectorShape;
			this.gluePointId = gluePointId;
			this.targetShape = null;
			this.targetPointId = ControlPointId.None;

			ShapeConnectionInfo sci = connectorShape.GetConnectionInfo(gluePointId, null);
			if (sci.IsEmpty) throw new NShapeException("GluePoint {0} is not connected.", gluePointId);
			this.targetShape = sci.OtherShape;
			this.targetPointId = sci.OtherPointId;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void Connect() {
			connectorShape.Connect(gluePointId, targetShape, targetPointId);
			if (Repository != null)
				Repository.InsertConnection(connectorShape, gluePointId, targetShape, targetPointId);
			connectorShape.Invalidate();
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void Disconnect() {
			connectorShape.Disconnect(gluePointId);
			if (Repository != null)
				Repository.DeleteConnection(connectorShape, gluePointId, targetShape, targetPointId);
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Connect; }
		}


		/// <override></override>
		protected override bool CheckAllowedCore(ISecurityManager securityManager, bool createException, out Exception exception) {
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			bool isGranted = securityManager.IsGranted(RequiredPermission, connectorShape);
			exception = (!isGranted && createException) ? new NShapeSecurityException(this) : null;
			return isGranted;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected Shape connectorShape;
		/// <ToBeCompleted></ToBeCompleted>
		protected Shape targetShape;
		/// <ToBeCompleted></ToBeCompleted>
		protected ControlPointId gluePointId;
		/// <ToBeCompleted></ToBeCompleted>
		protected ControlPointId targetPointId;
	}


	/// <summary>
	/// Base class for (un)aggregating shapes in shape aggregations.
	/// </summary>
	public abstract class ShapeAggregationCommand : AutoDisconnectShapesCommand {

		/// <ToBeCompleted></ToBeCompleted>
		protected ShapeAggregationCommand(Diagram diagram, Shape aggregationShape, IEnumerable<Shape> shapes)
			: this(null, diagram, aggregationShape, shapes) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected ShapeAggregationCommand(IRepository repository, Diagram diagram, Shape aggregationShape, IEnumerable<Shape> shapes)
			: base(repository) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (aggregationShape == null) throw new ArgumentNullException("aggregationShape");
			if (shapes == null) throw new ArgumentNullException("shapes");
			this.diagram = diagram;
			this.aggregationShape = aggregationShape;
			this.aggregationShapeOwnedByDiagram = diagram.Shapes.Contains(aggregationShape);
			this.shapes = new List<Shape>(shapes);
			aggregationLayerIds = LayerIds.None;
			for (int i = 0; i < this.shapes.Count; ++i)
				aggregationLayerIds |= this.shapes[i].Layers;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void CreateShapeAggregation(bool maintainZOrders) {
			// Add aggregation shape to diagram
			if (!aggregationShapeOwnedByDiagram) {
				diagram.Shapes.Add(aggregationShape);
				diagram.AddShapeToLayers(aggregationShape, aggregationLayerIds);
			}
			// Insert aggregation shape to repository (if necessary)
			if (Repository != null) {
				if (!aggregationShapeOwnedByDiagram) {
					if (CanUndeleteEntity(aggregationShape))
						Repository.UndeleteAll(aggregationShape, diagram);
					else Repository.InsertAll(aggregationShape, diagram);
				}
			}

			// Remove shapes from diagram
			diagram.Shapes.RemoveRange(shapes);
			// Add Shapes to aggregation shape
			if (maintainZOrders) {
				int cnt = shapes.Count;
				for (int i = 0; i < cnt; ++i)
					aggregationShape.Children.Add(shapes[i], shapes[i].ZOrder);
			} else aggregationShape.Children.AddRange(shapes);

			// Finally, update the child shape's owner
			if (Repository != null) {
				foreach (Shape childShape in aggregationShape.Children)
					Repository.UpdateOwner(childShape, aggregationShape);
				if (aggregationShapeOwnedByDiagram)
					Repository.Update(aggregationShape);
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void DeleteShapeAggregation() {
			// Update the child shape's owner
			if (Repository != null) {
				foreach (Shape childShape in aggregationShape.Children)
					Repository.UpdateOwner(childShape, diagram);
			}

			// Move the shapes to their initial owner
			aggregationShape.Children.RemoveRange(shapes);
			foreach (Shape childShape in aggregationShape.Children)
				aggregationShape.Children.Remove(childShape);

			if (!aggregationShapeOwnedByDiagram)
				// If the aggregation shape was not initialy part of the diagram, remove it.
				diagram.Shapes.Remove(aggregationShape);
			if (zOrders != null) {
				int cnt = shapes.Count;
				for (int i = 0; i < cnt; ++i)
					diagram.Shapes.Add(shapes[i], zOrders[i]);
			} else diagram.Shapes.AddRange(shapes);

			// Delete shapes from repository
			if (Repository != null) {
				// If the aggregation shape was not initialy part of the diagram, remove it.
				if (!aggregationShapeOwnedByDiagram)
					// Shape aggregations are deleted with all their children
					Repository.DeleteAll(aggregationShape);
				else Repository.Update(aggregationShape);
			}
		}


		/// <override></override>
		protected override bool CheckAllowedCore(ISecurityManager securityManager, bool createException, out Exception exception) {
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			bool isGranted = (securityManager.IsGranted(Permission.Insert | Permission.Delete, shapes)
							&& securityManager.IsGranted(RequiredPermission, aggregationShape));
			exception = (!isGranted && createException) ? new NShapeSecurityException(this) : null;
			return isGranted;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected Diagram diagram;
		/// <ToBeCompleted></ToBeCompleted>
		protected List<Shape> shapes;
		/// <ToBeCompleted></ToBeCompleted>
		protected List<int> zOrders;
		/// <ToBeCompleted></ToBeCompleted>
		protected LayerIds aggregationLayerIds;
		/// <ToBeCompleted></ToBeCompleted>
		protected Shape aggregationShape;
		// Specifies if the aggreagtion shape initialy was owned by the diagram
		/// <ToBeCompleted></ToBeCompleted>
		protected bool aggregationShapeOwnedByDiagram;
	}


	/// <summary>
	/// Executes a list of commands
	/// The Label of this command is created by concatenating the labels of each command.
	/// </summary>
	public class AggregatedCommand : Command {

		/// <ToBeCompleted></ToBeCompleted>
		public AggregatedCommand()
			: this((IRepository)null) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public AggregatedCommand(IRepository repository)
			: base(repository) {
			commands = new List<ICommand>();
			description = string.Empty;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public AggregatedCommand(IEnumerable<ICommand> commands)
			: this(null, commands) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public AggregatedCommand(IRepository repository, IEnumerable<ICommand> commands)
			: base(repository) {
			if (commands == null) throw new ArgumentNullException("commands");
			commands = new List<ICommand>(commands);
			CreateLabelString();
		}


		/// <override></override>
		public override string Description {
			get {
				if (string.IsNullOrEmpty(description))
					CreateLabelString();
				return base.Description;
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public int CommandCount { get { return commands.Count; } }


		/// <ToBeCompleted></ToBeCompleted>
		public void Add(ICommand command) {
			if (command == null) throw new ArgumentNullException("command");
			command.Repository = Repository;
			commands.Add(command);
			description = string.Empty;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void Insert(int index, ICommand command) {
			if (command == null) throw new ArgumentNullException("command");
			command.Repository = Repository;
			commands.Add(command);
			description = string.Empty;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void Remove(ICommand command) {
			if (command == null) throw new ArgumentNullException("command");
			RemoveAt(commands.IndexOf(command));
			description = string.Empty;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void RemoveAt(int index) {
			commands.RemoveAt(index);
			description = string.Empty;
		}


		/// <override></override>
		public override void Execute() {
			for (int i = 0; i < commands.Count; ++i) {
				if (commands[i].Repository != Repository)
					commands[i].Repository = Repository;
				commands[i].Execute();
			}
		}


		/// <override></override>
		public override void Revert() {
			for (int i = commands.Count - 1; i >= 0; --i) {
				if (commands[i].Repository != Repository)
					commands[i].Repository = Repository;
				commands[i].Revert();
			}
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get {
				Permission requiredPermission = Permission.None;
				for (int i = 0; i < commands.Count; ++i) {
					if (commands[i] is Command)
						requiredPermission = ((Command)commands[i]).RequiredPermission;
				}
				return requiredPermission;
			}
		}


		/// <override></override>
		protected override bool CheckAllowedCore(ISecurityManager securityManager, bool createException, out Exception exception) {
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			exception = null;
			bool isGranted = true;
			for (int i = 0; i < commands.Count; ++i) {
				if (!commands[i].IsAllowed(securityManager)) {
					if (createException) exception = CheckAllowed(securityManager);
					isGranted = false;
					break;
				}
			}
			if (!isGranted && createException && exception == null) exception = new NShapeSecurityException(this);
			return isGranted;
		}


		private void CreateLabelString() {
			description = string.Empty;
			if (commands.Count > 0) {
				string newLine = commands.Count > 3 ? Environment.NewLine : string.Empty;
				description = commands[0].Description;
				int lastIdx = commands.Count - 1;
				for (int i = 1; i <= lastIdx; ++i) {
					description += (i < lastIdx) ? ", " : " and ";
					description += string.Format("{0}{1}{2}", newLine, commands[i].Description.Substring(0, 1).ToLowerInvariant(), commands[i].Description.Substring(1));
				}
			}
		}


		List<ICommand> commands;
	}

	#endregion


	#region Commands for Shapes

	/// <summary>
	/// Inserts the given shape(s) into diagram and repository.
	/// </summary>
	[Obsolete("Use CreateShapesCommand instead")]
	public class InsertShapeCommand : CreateShapesCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public InsertShapeCommand(Diagram diagram, LayerIds layerIds, Shape shape, bool withModelObjects, bool keepZOrder)
			: base(diagram, layerIds, shape, withModelObjects, keepZOrder) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public InsertShapeCommand(IRepository repository, Diagram diagram, LayerIds layerIds, Shape shape, bool withModelObjects, bool keepZOrder)
			: base(repository, diagram, layerIds, shape, withModelObjects, keepZOrder) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public InsertShapeCommand(Diagram diagram, LayerIds layerIds, Shape shape, bool withModelObjects, bool keepZOrder, int toX, int toY)
			: base(diagram, layerIds, shape, withModelObjects, keepZOrder, toX, toY) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public InsertShapeCommand(IRepository repository, Diagram diagram, LayerIds layerIds, Shape shape, bool withModelObjects, bool keepZOrder, int toX, int toY)
			: base(repository, diagram, layerIds, shape, withModelObjects, keepZOrder, toX, toY) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public InsertShapeCommand(Diagram diagram, LayerIds layerIds, IEnumerable<Shape> shapes, bool withModelObjects, bool keepZOrder, int deltaX, int deltaY)
			: base(diagram, layerIds, shapes, withModelObjects, keepZOrder, deltaX, deltaY) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public InsertShapeCommand(IRepository repository, Diagram diagram, LayerIds layerIds, IEnumerable<Shape> shapes, bool withModelObjects, bool keepZOrder, int deltaX, int deltaY)
			: base(repository, diagram, layerIds, shapes, withModelObjects, keepZOrder, deltaX, deltaY) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public InsertShapeCommand(Diagram diagram, LayerIds layerIds, IEnumerable<Shape> shapes, bool withModelObjects, bool keepZOrder)
			: base(diagram, layerIds, shapes, withModelObjects, keepZOrder) {
		}
	}


	/// <summary>
	/// Inserts the given shape(s) into diagram and repository.
	/// </summary>
	public class CreateShapesCommand : InsertOrRemoveShapeCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public CreateShapesCommand(Diagram diagram, LayerIds layerIds, Shape shape, bool withModelObjects, bool keepZOrder)
			: this(null, diagram, layerIds, shape, withModelObjects, keepZOrder) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public CreateShapesCommand(IRepository repository, Diagram diagram, LayerIds layerIds, Shape shape, bool withModelObjects, bool keepZOrder)
			: base(repository, diagram) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (shape == null) throw new ArgumentNullException("shape");
			Construct(layerIds, shape, 0, 0, keepZOrder, withModelObjects);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public CreateShapesCommand(Diagram diagram, LayerIds layerIds, Shape shape, bool withModelObjects, bool keepZOrder, int toX, int toY)
			: this(null, diagram, layerIds, shape, withModelObjects, keepZOrder, toX, toY) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public CreateShapesCommand(IRepository repository, Diagram diagram, LayerIds layerIds, Shape shape, bool withModelObjects, bool keepZOrder, int toX, int toY)
			: base(repository, diagram) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (shape == null) throw new ArgumentNullException("shape");
			Construct(layerIds, shape, toX - shape.X, toY - shape.Y, keepZOrder, withModelObjects);
			this.description += string.Format(" at {0}", new Point(toX, toY));
		}


		/// <ToBeCompleted></ToBeCompleted>
		public CreateShapesCommand(Diagram diagram, LayerIds layerIds, IEnumerable<Shape> shapes, bool withModelObjects, bool keepZOrder, int deltaX, int deltaY)
			: this(null, diagram, layerIds, shapes, withModelObjects, keepZOrder, deltaX, deltaY) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public CreateShapesCommand(IRepository repository, Diagram diagram, LayerIds layerIds, IEnumerable<Shape> shapes, bool withModelObjects, bool keepZOrder, int deltaX, int deltaY)
			: base(repository, diagram) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (shapes == null) throw new ArgumentNullException("shapes");
			Construct(layerIds, shapes, deltaX, deltaY, keepZOrder, withModelObjects);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public CreateShapesCommand(Diagram diagram, LayerIds layerIds, IEnumerable<Shape> shapes, bool withModelObjects, bool keepZOrder)
			: base(diagram) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (shapes == null) throw new ArgumentNullException("shapes");
			Construct(layerIds, shapes, 0, 0, keepZOrder, withModelObjects);
		}


		/// <override></override>
		public override void Execute() {
			//InsertShapes(layerIds);
			InsertShapesAndModels(layerIds);
			if (connectionsToRestore != null) {
				foreach (KeyValuePair<Shape, List<ShapeConnectionInfo>> item in connectionsToRestore) {
					for (int i = item.Value.Count - 1; i >= 0; --i)
						item.Key.Connect(item.Value[i].OwnPointId, item.Value[i].OtherShape, item.Value[i].OtherPointId);
				}
			}
		}


		/// <override></override>
		public override void Revert() {
			//RemoveShapes();
			if (connectionsToRestore != null) {
				foreach (KeyValuePair<Shape, List<ShapeConnectionInfo>> item in connectionsToRestore) {
					for (int i = item.Value.Count - 1; i >= 0; --i)
						item.Key.Disconnect(item.Value[i].OwnPointId);
				}
			}
			DeleteShapesAndModels();
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Insert; }
		}


		/// <override></override>
		protected override bool CheckAllowedCore(ISecurityManager securityManager, bool createException, out Exception exception) {
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			bool isGranted = securityManager.IsGranted(RequiredPermission, Shapes);
			exception = (!isGranted && createException) ? new NShapeSecurityException(this) : null;
			return isGranted;
		}


		private void Construct(LayerIds layerIds, Shape shape, int offsetX, int offsetY, bool keepZOrder, bool withModelObjects) {
			this.layerIds = layerIds;
			PrepareShape(shape, offsetX, offsetY, keepZOrder);
			SetShape(shape, withModelObjects);
			description = GetDescription(DescriptionType.Insert, shape, withModelObjects);
		}


		private void Construct(LayerIds layerIds, IEnumerable<Shape> shapes, int offsetX, int offsetY, bool keepZOrder, bool withModelObjects) {
			this.layerIds = layerIds;
			foreach (Shape shape in shapes)
				PrepareShape(shape, offsetX, offsetY, keepZOrder);
			SetShapes(shapes, withModelObjects);
			description = GetDescription(DescriptionType.Insert, shapes, withModelObjects);
		}


		/// <summary>
		/// Reset shape's ZOrder to 'Unassigned' and offset shape's position if neccessary
		/// </summary>
		private void PrepareShape(Shape shape, int offsetX, int offsetY, bool keepZOrder) {
			if (!keepZOrder) shape.ZOrder = 0;
			if (offsetX != 0 || offsetY != 0) {
				// Check if the shape has glue points. 
				// If it has, store all connections of connected glue points and disconnect the shapes
				List<ShapeConnectionInfo> connInfos = null;
				foreach (ShapeConnectionInfo sci in shape.GetConnectionInfos(ControlPointId.Any, null)) {
					if (!shape.HasControlPointCapability(sci.OwnPointId, ControlPointCapabilities.Glue))
						continue;
					if (sci.IsEmpty) continue;
					if (connInfos == null) connInfos = new List<ShapeConnectionInfo>();
					connInfos.Add(sci);
					shape.Disconnect(sci.OwnPointId);
				}
				if (connInfos != null) {
					if (connectionsToRestore == null) connectionsToRestore = new Dictionary<Shape, List<ShapeConnectionInfo>>();
					connectionsToRestore.Add(shape, connInfos);
				}
				shape.MoveBy(offsetX, offsetY);
			}
		}


		#region Fields
		private LayerIds layerIds = LayerIds.None;
		private Dictionary<Shape, List<ShapeConnectionInfo>> connectionsToRestore;
		#endregion
	}


	/// <summary>
	/// Remove the given shapes and their model objects from diagram and repository.
	/// </summary>
	[Obsolete("Use DeleteShapesCommand instead.")]
	public class DeleteShapeCommand : DeleteShapesCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public DeleteShapeCommand(Diagram diagram, Shape shape, bool withModelObjects)
			: base(diagram, shape, withModelObjects) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DeleteShapeCommand(IRepository repository, Diagram diagram, Shape shape, bool withModelObjects)
			: base(repository, diagram, shape, withModelObjects) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DeleteShapeCommand(Diagram diagram, IEnumerable<Shape> shapes, bool withModelObjects)
			: base(diagram, shapes, withModelObjects) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DeleteShapeCommand(IRepository repository, Diagram diagram, IEnumerable<Shape> shapes, bool withModelObjects)
			: base(repository, diagram, shapes, withModelObjects) {
		}
	}
	
	
	/// <summary>
	/// Remove the given shapes and their model objects from diagram and repository.
	/// </summary>
	public class DeleteShapesCommand : InsertOrRemoveShapeCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public DeleteShapesCommand(Diagram diagram, Shape shape, bool withModelObjects)
			: this(null, diagram, shape, withModelObjects) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DeleteShapesCommand(IRepository repository, Diagram diagram, Shape shape, bool withModelObjects)
			: this(repository, diagram, SingleInstanceEnumerator<Shape>.Create(shape), withModelObjects) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DeleteShapesCommand(Diagram diagram, IEnumerable<Shape> shapes, bool withModelObjects)
			: this(null, diagram, shapes, withModelObjects) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DeleteShapesCommand(IRepository repository, Diagram diagram, IEnumerable<Shape> shapes, bool withModelObjects)
			: base(repository, diagram) {
			SetShapes(shapes, withModelObjects);
			this.description = GetDescription(DescriptionType.Delete, shapes, withModelObjects);
			// Store "shape to model object" assignments for undo
			foreach (Shape s in shapes) {
				if (s.ModelObject != null) {
					if (modelObjectAssignments == null) modelObjectAssignments = new Dictionary<Shape, IModelObject>();
					modelObjectAssignments.Add(s, s.ModelObject);
				}
			}
		}



		/// <override></override>
		public override void Execute() {
			DeleteShapesAndModels();
		}


		/// <override></override>
		public override void Revert() {
			InsertShapesAndModels();
			if (modelObjectAssignments != null) {
				foreach (KeyValuePair<Shape, IModelObject> item in modelObjectAssignments)
					item.Key.ModelObject = item.Value;
			}
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Delete; }
		}


		private Dictionary<Shape, IModelObject> modelObjectAssignments;
	}


	/// <summary>
	/// Moves a set of shapes by the same displacement.
	/// Used mainly for interactive moving of multiple selected shapes.
	/// </summary>
	public class MoveShapeByCommand : ShapesCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public MoveShapeByCommand(Shape shape, int dX, int dY)
			: this(null, shape, dX, dY) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public MoveShapeByCommand(IRepository repository, Shape shape, int dX, int dY)
			: base(repository, SingleInstanceEnumerator<Shape>.Create(shape)) {
			if (shape == null) throw new ArgumentNullException("shape");
			this.description = string.Format("Move {0}", shape.Type.Name);
			this.dX = dX;
			this.dY = dY;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public MoveShapeByCommand(IEnumerable<Shape> shapes, int dX, int dY)
			: this(null, shapes, dX, dY) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public MoveShapeByCommand(IRepository repository, IEnumerable<Shape> shapes, int dX, int dY)
			: base(repository, EmptyEnumerator<Shape>.Empty) {
			if (shapes == null) throw new ArgumentNullException("shapes");
			//// This approach is too simple - it will not work satisfactory because connected shapes are moved twice 
			//// (the shape is moved AND will follow its partner shape).
			//// Label shapes will re-calculate their relative position in this case and therefore they are not moved as expected
			//this.shapes = new List<Shape>(shapes);

			// Move only shapes without connected glue points
			foreach (Shape shape in shapes) {
				// Do not move shapes that are connected with all their glue points
				if (shape is ILinearShape) {
					if (!CanMoveShape((ILinearShape)shape)) continue;
				} else if (shape is LabelBase) {
					if (!CanMoveShape((LabelBase)shape, shapes)) continue;
				}
				this.shapes.Add(shape);
			}
			// Collect connections to remove temporarily
			for (int i = 0; i < this.shapes.Count; ++i) {
				if (!IsConnectedToNonSelectedShapes(this.shapes[i])) {
					foreach (ControlPointId gluePointId in this.shapes[i].GetControlPointIds(ControlPointCapabilities.Glue)) {
						ShapeConnectionInfo gluePointConnectionInfo = this.shapes[i].GetConnectionInfo(gluePointId, null);
						if (!gluePointConnectionInfo.IsEmpty) {
							ConnectionInfoBuffer connInfoBuffer;
							connInfoBuffer.shape = this.shapes[i];
							connInfoBuffer.connectionInfo = gluePointConnectionInfo;
							if (connectionsBuffer == null)
								connectionsBuffer = new List<ConnectionInfoBuffer>();
							connectionsBuffer.Add(connInfoBuffer);
						}
					}
				}
			}
			this.dX = dX;
			this.dY = dY;
			this.description = string.Format("Move {0} shape", this.shapes.Count);
			if (this.shapes.Count > 1) this.description += "s";
		}


		/// <override></override>
		public override void Execute() {
			// Remove connections temporarily so we can move connected lines as well.
			if (connectionsBuffer != null) {
				for (int i = 0; i < connectionsBuffer.Count; ++i)
					connectionsBuffer[i].shape.Disconnect(connectionsBuffer[i].connectionInfo.OwnPointId);
			}
			// move shapes
			int cnt = shapes.Count;
			for (int i = 0; i < cnt; ++i)
				shapes[i].MoveBy(dX, dY);
			// restore temporarily removed connections between selected shapes
			if (connectionsBuffer != null) {
				for (int i = 0; i < connectionsBuffer.Count; ++i)
					connectionsBuffer[i].shape.Connect(connectionsBuffer[i].connectionInfo.OwnPointId, connectionsBuffer[i].connectionInfo.OtherShape, connectionsBuffer[i].connectionInfo.OtherPointId);
			}
			// update cache
			if (Repository != null) Repository.Update(shapes);
		}


		/// <override></override>
		public override void Revert() {
			// remove connections temporarily
			if (connectionsBuffer != null) {
				for (int i = 0; i < connectionsBuffer.Count; ++i)
					connectionsBuffer[i].shape.Disconnect(connectionsBuffer[i].connectionInfo.OwnPointId);
			}

			// move shapes
			for (int i = 0; i < shapes.Count; ++i)
				shapes[i].MoveBy(-dX, -dY);

			// restore temporarily removed connections between selected shapes
			if (connectionsBuffer != null) {
				for (int i = 0; i < connectionsBuffer.Count; ++i)
					connectionsBuffer[i].shape.Connect(connectionsBuffer[i].connectionInfo.OwnPointId, connectionsBuffer[i].connectionInfo.OtherShape, connectionsBuffer[i].connectionInfo.OtherPointId);
			}

			if (Repository != null) Repository.Update(shapes);
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Layout; }
		}



		private bool CanMoveShape(ILinearShape shape) {
			int gluePointCnt = 0, connectedCnt = 0;
			foreach (ControlPointId gluePointId in ((Shape)shape).GetControlPointIds(ControlPointCapabilities.Glue)) {
				++gluePointCnt;
				ShapeConnectionInfo sci = ((Shape)shape).GetConnectionInfo(gluePointId, null);
				if (!sci.IsEmpty) ++connectedCnt;
			}
			return (gluePointCnt != connectedCnt);
		}


		private bool CanMoveShape(LabelBase shape, IEnumerable<Shape> selectedShapes) {
			foreach (ControlPointId gluePointId in shape.GetControlPointIds(ControlPointCapabilities.Glue)) {
				ShapeConnectionInfo sci = shape.GetConnectionInfo(gluePointId, null);
				if (!sci.IsEmpty) {
					foreach (Shape s in selectedShapes) {
						if (s == sci.OtherShape)
							return false;
					}
				}
			}
			return true;
		}


		private bool IsConnectedToNonSelectedShapes(Shape shape) {
			if (shape == null) throw new ArgumentNullException("shape");
			foreach (ControlPointId gluePointId in shape.GetControlPointIds(ControlPointCapabilities.Glue)) {
				ShapeConnectionInfo sci = shape.GetConnectionInfo(gluePointId, null);
				if (!sci.IsEmpty && shapes.IndexOf(sci.OtherShape) < 0)
					return true;
			}
			return false;
		}


		private int dX, dY;
		private struct ConnectionInfoBuffer : IEquatable<ConnectionInfoBuffer> {
			internal Shape shape;
			internal ShapeConnectionInfo connectionInfo;
			public bool Equals(ConnectionInfoBuffer other) {
				return other.shape == this.shape && other.connectionInfo == this.connectionInfo;
			}
		}
		private List<ConnectionInfoBuffer> connectionsBuffer;	// buffer used for storing connections that are temporarily disconnected for moving shapes
	}


	/// <summary>
	/// Moves multiple shapes by individual distances.
	/// </summary>
	public class MoveShapesCommand : Command {

		/// <ToBeCompleted></ToBeCompleted>
		public MoveShapesCommand()
			: this(null) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public MoveShapesCommand(IRepository repository)
			: base(repository) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void AddMove(Shape shape, int dx, int dy) {
			if (shape == null) throw new ArgumentNullException("shape");
			ShapeMove sm;
			sm.shape = shape;
			sm.dx = dx;
			sm.dy = dy;
			shapeMoves.Add(sm);
		}


		#region Base Class Implementation

		/// <override></override>
		public override string Description {
			get { return string.Format("Move {0} shape{1}", shapeMoves.Count, (shapeMoves.Count > 1) ? "s" : ""); }
		}


		/// <override></override>
		public override void Execute() {
			foreach (ShapeMove sm in shapeMoves)
				sm.shape.MoveControlPointBy(ControlPointId.Reference, sm.dx, sm.dy, ResizeModifiers.None);
		}


		/// <override></override>
		public override void Revert() {
			foreach (ShapeMove sm in shapeMoves)
				sm.shape.MoveControlPointBy(ControlPointId.Reference, -sm.dx, -sm.dy, ResizeModifiers.None);
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Layout; }
		}


		/// <override></override>
		protected override bool CheckAllowedCore(ISecurityManager securityManager, bool createException, out Exception exception) {
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			bool isGranted = true;
			for (int i = shapeMoves.Count - 1; i >= 0; --i) {
				if (!securityManager.IsGranted(RequiredPermission, shapeMoves[i].shape)) {
					isGranted = false;
					break;
				}
			}
			exception = (!isGranted && createException) ? new NShapeSecurityException(this) : null;
			return isGranted;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected struct ShapeMove : IEquatable<ShapeMove> {
			/// <ToBeCompleted></ToBeCompleted>
			public Shape shape;
			/// <ToBeCompleted></ToBeCompleted>
			public int dx;
			/// <ToBeCompleted></ToBeCompleted>
			public int dy;
			/// <ToBeCompleted></ToBeCompleted>
			public bool Equals(ShapeMove other) {
				return (other.shape == this.shape
					&& other.dx == this.dx
					&& other.dy == this.dy);
			}
		}


		private List<ShapeMove> shapeMoves = new List<ShapeMove>();

		#endregion
	}


	/// <summary>
	/// Moves multiple shapes to individual destination points.
	/// </summary>
	public class MoveShapesToCommand : Command {

		/// <ToBeCompleted></ToBeCompleted>
		public MoveShapesToCommand()
			: this(null) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public MoveShapesToCommand(IRepository repository)
			: base(repository) {
		}


		/// <summary>
		/// Adds a move for the shape the shape will be moved to the given coordinates. 
		/// The given shape has to be either at the original position or at the target position.
		/// </summary>
		public void AddMoveTo(Shape shape, int x0, int y0, int x1, int y1) {
			if (shape == null) throw new ArgumentNullException("shape");
			Debug.Assert((shape.X == x0 || shape.X == x1) && (shape.Y == y0 || shape.Y == y1));
			ShapeMove sm;
			sm.shape = shape;
			sm.origX = x0;
			sm.origY = y0;
			sm.destX = x1;
			sm.destY = y1;
			shapeMoves.Add(sm);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void AddMoveBy(Shape shape, int dx, int dy) {
			if (shape == null) throw new ArgumentNullException("shape");
			ShapeMove sm;
			sm.shape = shape;
			sm.origX = shape.X;
			sm.origY = shape.Y;
			sm.destX = shape.X + dx;
			sm.destY = shape.Y + dy;
			shapeMoves.Add(sm);
		}


		#region Base Class Implementation

		/// <override></override>
		public override string Description {
			get { return string.Format("Move {0} shape{1}", shapeMoves.Count, (shapeMoves.Count > 1) ? "s" : ""); }
		}


		/// <override></override>
		public override void Execute() {
			foreach (ShapeMove sm in shapeMoves)
				sm.shape.MoveTo(sm.destX, sm.destY);
		}


		/// <override></override>
		public override void Revert() {
			foreach (ShapeMove sm in shapeMoves)
				sm.shape.MoveTo(sm.origX, sm.origY);
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Layout; }
		}


		/// <override></override>
		protected override bool CheckAllowedCore(ISecurityManager securityManager, bool createException, out Exception exception) {
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			bool isGranted = true;
			for (int i = shapeMoves.Count - 1; i >= 0; --i) {
				if (!securityManager.IsGranted(RequiredPermission, shapeMoves[i].shape)) {
					isGranted = false;
					break;
				}
			}
			exception = (!isGranted && createException) ? new NShapeSecurityException(this) : null;
			return isGranted;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected struct ShapeMove : IEquatable<ShapeMove> {
			/// <ToBeCompleted></ToBeCompleted>
			public Shape shape;
			/// <ToBeCompleted></ToBeCompleted>
			public int origX;
			/// <ToBeCompleted></ToBeCompleted>
			public int origY;
			/// <ToBeCompleted></ToBeCompleted>
			public int destX;
			/// <ToBeCompleted></ToBeCompleted>
			public int destY;
			/// <ToBeCompleted></ToBeCompleted>
			public bool Equals(ShapeMove other) {
				return (other.destX == this.destX
					&& other.destY == this.destY
					&& other.origX == this.origX
					&& other.origY == this.origY
					&& other.shape == this.shape);
			}
		}


		private List<ShapeMove> shapeMoves = new List<ShapeMove>();

		#endregion

	}


	/// <ToBeCompleted></ToBeCompleted>
	public class MoveControlPointCommand : ShapesCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public MoveControlPointCommand(Shape shape, int controlPointId, int dX, int dY, ResizeModifiers modifiers)
			: this(null, shape, controlPointId, dX, dY, modifiers) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public MoveControlPointCommand(IRepository repository, Shape shape, int controlPointId, int dX, int dY, ResizeModifiers modifiers)
			: this(repository, SingleInstanceEnumerator<Shape>.Create(shape), controlPointId, dX, dY, modifiers) {
			this.description = string.Format("Move control point of {0}", shape.Type.Name);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public MoveControlPointCommand(IEnumerable<Shape> shapes, int controlPointId, int dX, int dY, ResizeModifiers modifiers)
			: this(null, shapes, controlPointId, dX, dY, modifiers) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public MoveControlPointCommand(IRepository repository, IEnumerable<Shape> shapes, int controlPointId, int dX, int dY, ResizeModifiers modifiers)
			: base(repository, shapes) {
			this.description = string.Format("Move {0} control point{1}", this.shapes.Count, this.shapes.Count > 1 ? "s" : "");
			int cnt = this.shapes.Count;
			moveInfos = new List<PointMoveInfo>(cnt);
			for (int i = 0; i < cnt; ++i)
				moveInfos.Add(new PointMoveInfo(this.shapes[i], controlPointId, dX, dY, modifiers));
		}


		/// <override></override>
		public override void Execute() {
			for (int i = 0; i < shapes.Count; ++i)
				shapes[i].MoveControlPointTo(moveInfos[i].PointId, moveInfos[i].To.X, moveInfos[i].To.Y, moveInfos[i].Modifiers);

			if (Repository != null) Repository.Update(shapes);
		}


		/// <override></override>
		public override void Revert() {
			for (int i = 0; i < shapes.Count; ++i)
				//shapes[i].MoveControlPointBy(controlPointId, -dX, -dY, modifiers);
				shapes[i].MoveControlPointTo(moveInfos[i].PointId, moveInfos[i].From.X, moveInfos[i].From.Y, moveInfos[i].Modifiers);

			if (Repository != null) Repository.Update(shapes);
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Layout; }
		}


		private class PointMoveInfo {

			public PointMoveInfo(Shape shape, ControlPointId id, int dx, int dy, ResizeModifiers mod)
				: this(id, shape.GetControlPointPosition(id), Point.Empty, mod) {
				To.Offset(From.X + dx, From.Y + dy);
			}

			public PointMoveInfo(ControlPointId id, Point from, int dx, int dy, ResizeModifiers mod)
				: this(id, from, from, mod) {
				To.Offset(dx, dy);
			}

			public PointMoveInfo(ControlPointId id, Point from, Point to, ResizeModifiers mod) {
				PointId = id;
				From = from;
				To = to;
				Modifiers = mod;
			}

			public ControlPointId PointId;
			public Point From;
			public Point To;
			public ResizeModifiers Modifiers;
		}

		private List<PointMoveInfo> moveInfos;
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class MoveGluePointCommand : ShapeCommand {

		/// <summary>
		/// Constructs a glue point moving command.
		/// </summary>
		public MoveGluePointCommand(Shape shape, ControlPointId gluePointId, Shape targetShape, int dX, int dY, ResizeModifiers modifiers)
			: this(null, shape, gluePointId, targetShape, dX, dY, modifiers) {
		}


		/// <summary>
		/// Constructs a glue point moving command.
		/// </summary>
		public MoveGluePointCommand(IRepository repository, Shape shape, ControlPointId gluePointId, Shape targetShape, int dX, int dY, ResizeModifiers modifiers)
			: base(repository, shape) {
			// Find target point
			ControlPointId targetPointId = ControlPointId.None;
			if (targetShape != null) {
				targetPointId = targetShape.FindNearestControlPoint(gluePointPosition.X + dX, gluePointPosition.Y + dY, 0, ControlPointCapabilities.Connect);
				if (targetPointId == ControlPointId.None && targetShape.ContainsPoint(gluePointPosition.X + dX, gluePointPosition.Y + dY))
					targetPointId = ControlPointId.Reference;
			}
			BaseConstruct(gluePointId, targetShape, targetPointId, dX, dY, modifiers);
		}


		/// <summary>
		/// Constructs a glue point moving command.
		/// </summary>
		public MoveGluePointCommand(Shape shape, ControlPointId gluePointId, Shape targetShape, ControlPointId targetPointId, int dX, int dY, ResizeModifiers modifiers)
			: this(null, shape, gluePointId, targetShape, targetPointId, dX, dY, modifiers) {
		}


		/// <summary>
		/// Constructs a glue point moving command.
		/// </summary>
		public MoveGluePointCommand(IRepository repository, Shape shape, ControlPointId gluePointId, Shape targetShape, ControlPointId targetPointId, int dX, int dY, ResizeModifiers modifiers)
			: base(repository, shape) {
			BaseConstruct(gluePointId, targetShape, targetPointId, dX, dY, modifiers);
		}


		/// <override></override>
		public override void Execute() {
			// DetachGluePointFromConnectionPoint existing connection
			if (!existingConnection.IsEmpty) {
				shape.Disconnect(gluePointId);
				if (Repository != null) Repository.DeleteConnection(shape, gluePointId, existingConnection.OtherShape, existingConnection.OtherPointId);
			}
			// Move point
			shape.MoveControlPointBy(gluePointId, dX, dY, modifiers);
			// Establish new connection
			if (!newConnection.IsEmpty) {
				shape.Connect(gluePointId, newConnection.OtherShape, newConnection.OtherPointId);
				if (Repository != null) Repository.InsertConnection(shape, gluePointId, newConnection.OtherShape, newConnection.OtherPointId);
			}
			if (Repository != null) Repository.Update(shape);
		}


		/// <override></override>
		public override void Revert() {
			// DetachGluePointFromConnectionPoint new connection
			if (!newConnection.IsEmpty) {
				shape.Disconnect(gluePointId);
				if (Repository != null) Repository.DeleteConnection(shape, gluePointId, newConnection.OtherShape, newConnection.OtherPointId);
			}
			// Move point
			shape.MoveControlPointTo(gluePointId, gluePointPosition.X, gluePointPosition.Y, modifiers);
			// Restore previous connection
			if (!existingConnection.IsEmpty) {
				shape.Connect(gluePointId, existingConnection.OtherShape, existingConnection.OtherPointId);
				if (Repository != null) Repository.InsertConnection(shape, gluePointId, existingConnection.OtherShape, existingConnection.OtherPointId);
			}
			if (Repository != null) Repository.Update(shape);
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Layout | Permission.Connect; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void BaseConstruct(int gluePointId, Shape targetShape, ControlPointId targetPointId, int dX, int dY, ResizeModifiers modifiers) {
			this.gluePointId = gluePointId;
			this.targetShape = targetShape;
			this.targetPointId = targetPointId;
			this.dX = dX;
			this.dY = dY;
			this.modifiers = modifiers;
			// store original position of gluePoint (cannot be restored with dX/dY in case of PointToShape connections)
			gluePointPosition = shape.GetControlPointPosition(gluePointId);
			// store existing connection
			existingConnection = shape.GetConnectionInfo(this.gluePointId, null);

			// create new ConnectionInfo
			if (targetShape != null && targetPointId != ControlPointId.None)
				this.newConnection = new ShapeConnectionInfo(this.gluePointId, targetShape, targetPointId);
			// set description
			if (!existingConnection.IsEmpty) {
				this.description = string.Format("Disconnect {0} from {1}", shape.Type.Name, existingConnection.OtherShape.Type.Name);
				if (!newConnection.IsEmpty)
					this.description += string.Format(" and connect to {0}", newConnection.OtherShape.Type.Name);
			} else {
				if (!newConnection.IsEmpty)
					this.description += string.Format("Connect {0} to {1}", shape.Type.Name, newConnection.OtherShape.Type.Name);
				else
					this.description = string.Format("Move glue point {0} of {1}", gluePointId, shape.Type.Name);
			}
		}


		#region Fields
		private Point gluePointPosition;
		private ControlPointId gluePointId;
		private Shape targetShape;
		ControlPointId targetPointId;
		private int dX;
		private int dY;
		private ResizeModifiers modifiers;
		private ShapeConnectionInfo existingConnection;
		private ShapeConnectionInfo newConnection = ShapeConnectionInfo.Empty;
		#endregion
	}


	/// <summary>
	/// Rotates all members of a set of shapes by the same angle.
	/// </summary>
	public class RotateShapesCommand : Command {

		/// <ToBeCompleted></ToBeCompleted>
		public RotateShapesCommand(IEnumerable<Shape> shapes, int tenthsOfDegree)
			: this(null, shapes, tenthsOfDegree) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public RotateShapesCommand(IRepository repository, IEnumerable<Shape> shapes, int tenthsOfDegree)
			: base(repository) {
			if (shapes == null) throw new ArgumentNullException("shapes");
			this.shapes = new List<Shape>(shapes);
			this.angle = tenthsOfDegree;
			if (this.shapes.Count == 1)
				this.description = string.Format("Rotate {0} by {1}°", this.shapes[0].Type.Name, tenthsOfDegree / 10f);
			else
				this.description = string.Format("Rotate {0} shapes by {1}°", this.shapes.Count, tenthsOfDegree / 10f);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public RotateShapesCommand(IEnumerable<Shape> shapes, int tenthsOfDegree, int rotateCenterX, int rotateCenterY)
			: this(null, shapes, tenthsOfDegree, rotateCenterX, rotateCenterY) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public RotateShapesCommand(IRepository repository, IEnumerable<Shape> shapes, int tenthsOfDegree, int rotateCenterX, int rotateCenterY)
			: base(repository) {
			if (shapes == null) throw new ArgumentNullException("shapes");
			this.shapes = new List<Shape>(shapes);
			for (int i = 0; i < this.shapes.Count; ++i) {
				if (this.shapes[i] is ILinearShape) {
					List<Point> points = new List<Point>();
					foreach (ControlPointId id in this.shapes[i].GetControlPointIds(ControlPointCapabilities.Resize)) {
						Point p = this.shapes[i].GetControlPointPosition(id);
						points.Add(p);
					}
					unrotatedLinePoints.Add((ILinearShape)this.shapes[i], points);
				}
			}
			this.angle = tenthsOfDegree;
			this.rotateCenterX = rotateCenterX;
			this.rotateCenterY = rotateCenterY;
			if (this.shapes.Count == 1)
				this.description = string.Format("Rotate {0} by {1}° at ({2}|{3}", this.shapes[0].Type.Name, tenthsOfDegree / 10f, rotateCenterX, rotateCenterY);
			else
				this.description = string.Format("Rotate {0} shapes by {1}° at ({2}|{3}", this.shapes.Count, tenthsOfDegree / 10f, rotateCenterX, rotateCenterY);
		}


		/// <override></override>
		public override void Execute() {
			foreach (Shape shape in shapes) {
				if (!Geometry.IsValid(rotateCenterX, rotateCenterY))
					shape.Rotate(angle, shape.X, shape.Y);
				else shape.Rotate(angle, rotateCenterX, rotateCenterY);
			}
			if (Repository != null) Repository.Update(shapes);
		}


		/// <override></override>
		public override void Revert() {
			foreach (Shape shape in shapes) {
				if (!Geometry.IsValid(rotateCenterX, rotateCenterY))
					shape.Rotate(-angle, shape.X, shape.Y);
				else shape.Rotate(-angle, rotateCenterX, rotateCenterY);
			}
			if (Repository != null) Repository.Update(shapes);
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Layout; }
		}


		/// <override></override>
		protected override bool CheckAllowedCore(ISecurityManager securityManager, bool createException, out Exception exception) {
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			bool isGranted = securityManager.IsGranted(RequiredPermission, shapes);
			exception = (!isGranted && createException) ? new NShapeSecurityException(this) : null;
			return isGranted;
		}


		private int angle;
		private List<Shape> shapes;
		private int rotateCenterX = Geometry.InvalidPoint.X;
		private int rotateCenterY = Geometry.InvalidPoint.Y;
		private Dictionary<ILinearShape, List<Point>> unrotatedLinePoints = new Dictionary<ILinearShape, List<Point>>();
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class SetCaptionTextCommand : Command {

		/// <ToBeCompleted></ToBeCompleted>
		public SetCaptionTextCommand(ICaptionedShape shape, int captionIndex, string oldValue, string newValue)
			: this(null, shape, captionIndex, oldValue, newValue) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public SetCaptionTextCommand(IRepository repository, ICaptionedShape shape, int captionIndex, string oldValue, string newValue)
			: base(repository) {
			if (shape == null) throw new ArgumentNullException("shape");
			if (!(shape is Shape)) throw new NShapeException("{0} is not of type {1}.", shape.GetType().Name, typeof(Shape).Name);
			this.modifiedLabeledShapes = new List<ICaptionedShape>(1);
			this.modifiedLabeledShapes.Add(shape);
			this.labelIndex = captionIndex;
			this.oldValue = oldValue;
			this.newValue = newValue;
			this.description = string.Format("Change text of {0} from '{1}' to '{2}", ((Shape)shape).Type.Name, this.oldValue, this.newValue);
		}


		/// <override></override>
		public override void Execute() {
			for (int i = modifiedLabeledShapes.Count - 1; i >= 0; --i) {
				modifiedLabeledShapes[i].SetCaptionText(labelIndex, newValue);
				if (Repository != null) Repository.Update((Shape)modifiedLabeledShapes[i]);
			}
		}


		/// <override></override>
		public override void Revert() {
			for (int i = modifiedLabeledShapes.Count - 1; i >= 0; --i) {
				modifiedLabeledShapes[i].SetCaptionText(labelIndex, oldValue);
				if (Repository != null) Repository.Update((Shape)modifiedLabeledShapes[i]);
			}
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Data; }
		}


		/// <override></override>
		protected override bool CheckAllowedCore(ISecurityManager securityManager, bool createException, out Exception exception) {
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			bool isGranted = true;
			for (int i = modifiedLabeledShapes.Count - 1; i >= 0; --i) {
				if (!securityManager.IsGranted(RequiredPermission, (Shape)modifiedLabeledShapes[i])) {
					isGranted = false;
					break;
				}
			}
			exception = (!isGranted && createException) ? new NShapeSecurityException(this) : null;
			return isGranted;
		}


		#region Fields
		private int labelIndex;
		private string oldValue;
		private string newValue;
		private List<ICaptionedShape> modifiedLabeledShapes;
		#endregion
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class LiftShapeCommand : Command {

		/// <ToBeCompleted></ToBeCompleted>
		public LiftShapeCommand(Diagram diagram, IEnumerable<Shape> shapes, ZOrderDestination liftMode)
			: this(null, diagram, shapes, liftMode) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public LiftShapeCommand(IRepository repository, Diagram diagram, IEnumerable<Shape> shapes, ZOrderDestination liftMode)
			: base(repository) {
			Construct(diagram, shapes, liftMode);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public LiftShapeCommand(Diagram diagram, Shape shape, ZOrderDestination liftMode)
			: this(diagram, SingleInstanceEnumerator<Shape>.Create(shape), liftMode) {
		}


		/// <override></override>
		public override void Execute() {
			// store current and new ZOrders
			if (zOrderInfos == null || zOrderInfos.Count == 0)
				ObtainZOrders();

			// process topDown/bottomUp to avoid ZOrder-sorting inside the diagram's ShapeCollection
			switch (liftMode) {
				case ZOrderDestination.ToBottom:
					foreach (Shape shape in shapes.TopDown) {
						ZOrderInfo info = zOrderInfos[shape];
						PerformZOrderChange(shape, info.newZOrder, info.layerIds);
					}
					break;
				case ZOrderDestination.ToTop:
					foreach (Shape shape in shapes.BottomUp) {
						ZOrderInfo info = zOrderInfos[shape];
						PerformZOrderChange(shape, info.newZOrder, info.layerIds);
					}
					break;
				default:
					throw new NShapeUnsupportedValueException(liftMode);
			}
			if (Repository != null) Repository.Update(shapes);
		}


		/// <override></override>
		public override void Revert() {
			Debug.Assert(zOrderInfos != null);
			foreach (Shape shape in shapes.BottomUp) {
				ZOrderInfo info = zOrderInfos[shape];
				PerformZOrderChange(shape, info.origZOrder, info.layerIds);
			}
			if (Repository != null) Repository.Update(shapes);
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Layout; }
		}


		/// <override></override>
		protected override bool CheckAllowedCore(ISecurityManager securityManager, bool createException, out Exception exception) {
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			bool isGranted = securityManager.IsGranted(RequiredPermission, shapes);
			exception = (!isGranted && createException) ? new NShapeSecurityException(this) : null;
			return isGranted;
		}


		private void Construct(Diagram diagram, IEnumerable<Shape> shapes, ZOrderDestination liftMode) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (shapes == null) throw new ArgumentNullException("shapes");
			this.shapes = new ShapeCollection();
			this.shapes.AddRange(shapes);
			this.diagram = diagram;
			this.liftMode = liftMode;
			string formatStr = string.Empty;
			switch (liftMode) {
				case ZOrderDestination.ToTop: formatStr = "Bring {0} shape{1} on top"; break;
				case ZOrderDestination.ToBottom: formatStr = "Send {0} shape{1} to bottom"; break;
				default: throw new NShapeUnsupportedValueException(liftMode);
			}
			if (this.shapes.Count == 1)
				this.description = string.Format(formatStr, this.shapes.TopMost.Type.Name, string.Empty);
			else this.description = string.Format(formatStr, this.shapes.Count, "s");
		}


		private void ObtainZOrders() {
			zOrderInfos = new Dictionary<Shape, ZOrderInfo>(shapes.Count);
			switch (liftMode) {
				case ZOrderDestination.ToBottom:
					if (Repository != null) {
						foreach (Shape shape in shapes.TopDown)
							zOrderInfos.Add(shape, new ZOrderInfo(shape.ZOrder, Repository.ObtainNewBottomZOrder(diagram), shape.Layers));
					} else {
						foreach (Shape shape in shapes.TopDown)
							zOrderInfos.Add(shape, new ZOrderInfo(shape.ZOrder, diagram.Shapes.MinZOrder, shape.Layers));
					}
					break;
				case ZOrderDestination.ToTop:
					if (Repository != null) {
						foreach (Shape shape in shapes.BottomUp)
							zOrderInfos.Add(shape, new ZOrderInfo(shape.ZOrder, Repository.ObtainNewTopZOrder(diagram), shape.Layers));
					} else {
						foreach (Shape shape in shapes.BottomUp)
							zOrderInfos.Add(shape, new ZOrderInfo(shape.ZOrder, diagram.Shapes.MaxZOrder, shape.Layers));
					}
					break;
				default:
					throw new NShapeUnsupportedValueException(liftMode);
			}
		}


		private void PerformZOrderChange(Shape shape, int zOrder, LayerIds layerIds) {
			diagram.Shapes.SetZOrder(shape, zOrder);

			//// remove shape from Diagram
			//diagram.Shapes.Remove(shape);
			//// restore the original ZOrder value
			//shape.ZOrder = zOrder;
			//// re-insert the shape on its previous position
			//diagram.Shapes.Add(shape);
			//diagram.AddShapeToLayers(shape, layerIds);
		}


		private class ZOrderInfo {
			public ZOrderInfo(int origZOrder, int newZOrder, LayerIds layerIds) {
				this.origZOrder = origZOrder;
				this.newZOrder = newZOrder;
				this.layerIds = layerIds;
			}
			public int origZOrder;
			public int newZOrder;
			public LayerIds layerIds;
		}

		#region Fields
		private Diagram diagram;
		private ShapeCollection shapes;
		private ZOrderDestination liftMode;
		private Dictionary<Shape, ZOrderInfo> zOrderInfos;
		#endregion
	}

	#endregion


	# region Commands for (dis)connecting shapes

	/// <summary>
	/// Command for connecting a shape's GluePoint to an other shape's GluePoint
	/// </summary>
	public class ConnectCommand : ConnectionCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public ConnectCommand(Shape connectorShape, int gluePointId, Shape targetShape, int targetPointId)
			: this(null, connectorShape, gluePointId, targetShape, targetPointId) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public ConnectCommand(IRepository repository, Shape connectorShape, int gluePointId, Shape targetShape, int targetPointId)
			: base(repository, connectorShape, gluePointId, targetShape, targetPointId) {
			this.description = string.Format("Connect {0} to {1}", connectorShape.Type.Name, targetShape.Type.Name);
		}


		/// <override></override>
		public override void Execute() {
			Connect();
		}


		/// <override></override>
		public override void Revert() {
			Disconnect();
		}

	}


	/// <ToBeCompleted></ToBeCompleted>
	public class DisconnectCommand : ConnectionCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public DisconnectCommand(Shape connectorShape, int gluePointId)
			: this(null, connectorShape, gluePointId) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DisconnectCommand(IRepository repository, Shape connectorShape, int gluePointId)
			: base(repository, connectorShape, gluePointId) {
			this.description = string.Format("Disconnect {0}", connectorShape.Type.Name);
			this.connectorShape = connectorShape;
			this.gluePointId = gluePointId;

			this.connectionInfo = connectorShape.GetConnectionInfo(gluePointId, null);
			if (this.connectionInfo.IsEmpty)
				throw new NShapeInternalException(string.Format("There is no connection for Point {0} of shape {1}.", gluePointId, connectorShape));
		}


		/// <override></override>
		public override void Execute() {
			connectorShape.Disconnect(gluePointId);
			Repository.DeleteConnection(connectorShape, gluePointId, connectionInfo.OtherShape, connectionInfo.OtherPointId);
		}


		/// <override></override>
		public override void Revert() {
			connectorShape.Connect(gluePointId, connectionInfo.OtherShape, connectionInfo.OtherPointId);
			Repository.InsertConnection(connectorShape, gluePointId, connectionInfo.OtherShape, connectionInfo.OtherPointId);
		}


		private ShapeConnectionInfo connectionInfo;
	}

	#endregion


	#region Commands for editing vertices of LinearShapes

	/// <ToBeCompleted></ToBeCompleted>
	public class AddVertexCommand : ShapeCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public AddVertexCommand(Shape shape, int x, int y)
			: this(null, shape, x, y) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public AddVertexCommand(IRepository repository, Shape shape, int x, int y)
			: base(repository, shape) {
			if (!(shape is ILinearShape)) throw new ArgumentException(string.Format("Shape does not implement required interface {0}.", typeof(ILinearShape).Name));
			this.description = string.Format("Add Point to {0} at {1}|{2}", shape, x, y);
			this.x = x;
			this.y = y;
		}


		/// <override></override>
		public override void Execute() {
			insertedPointId = ((ILinearShape)shape).AddVertex(x, y);
			if (Repository != null) Repository.Update(shape);
		}


		/// <override></override>
		public override void Revert() {
			((ILinearShape)shape).RemoveVertex(insertedPointId);
			if (Repository != null) Repository.Update(shape);
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Layout; }
		}


		#region Fields
		private int x;
		private int y;
		private ControlPointId insertedPointId = ControlPointId.None;
		#endregion
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class InsertVertexCommand : ShapeCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public InsertVertexCommand(Shape shape, ControlPointId beforePointId, int x, int y)
			: this(null, shape, beforePointId, x, y) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public InsertVertexCommand(IRepository repository, Shape shape, ControlPointId beforePointId, int x, int y)
			: base(repository, shape) {
			if (!(shape is ILinearShape)) throw new ArgumentException(string.Format("Shape does not implement required interface {0}.", typeof(ILinearShape).Name));
			if (beforePointId == ControlPointId.None || beforePointId == ControlPointId.Any || beforePointId == ControlPointId.Reference || beforePointId == ControlPointId.FirstVertex)
				throw new ArgumentException("beforePointId");
			this.description = string.Format("Add Point to {0} at {1}|{2}", shape, x, y);
			this.beforePointId = beforePointId;
			this.x = x;
			this.y = y;
		}


		/// <override></override>
		public override void Execute() {
			if (insertedPointId != ControlPointId.None && shape is LineShapeBase)
				insertedPointId = ((LineShapeBase)shape).InsertVertex(beforePointId, insertedPointId, x, y);
			else insertedPointId = ((ILinearShape)shape).InsertVertex(beforePointId, x, y);
			if (Repository != null) Repository.Update(shape);
		}


		/// <override></override>
		public override void Revert() {
			((ILinearShape)shape).RemoveVertex(insertedPointId);
			if (Repository != null) Repository.Update(shape);
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Layout; }
		}


		#region Fields
		private ControlPointId beforePointId = ControlPointId.None;
		private int x;
		private int y;
		private ControlPointId insertedPointId = ControlPointId.None;
		#endregion
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class RemoveVertexCommand : ShapeCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public RemoveVertexCommand(Shape shape, ControlPointId vertexId)
			: this(null, shape, vertexId) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public RemoveVertexCommand(IRepository repository, Shape shape, ControlPointId vertexId)
			: base(repository, shape) {
			if (!(shape is ILinearShape)) throw new ArgumentException(string.Format("Shape does not implement required interface {0}.", typeof(ILinearShape).Name));
			// MenuItemDefs always create their commands, regardless if there is a valid vertexId that can be removed or not.
			// So we have to handle this case instead of throwing an exception.
			if (vertexId != ControlPointId.None)
				this.p = shape.GetControlPointPosition(vertexId);
			ctrlPointPositions = new Dictionary<ControlPointId, Point>();
			foreach (ControlPointId id in shape.GetControlPointIds(ControlPointCapabilities.Resize | ControlPointCapabilities.Connect | ControlPointCapabilities.Movable)) {
				if (id == vertexId) continue;
				ctrlPointPositions.Add(id, shape.GetControlPointPosition(id));
			}
			this.removedPointId = vertexId;
			this.nextPointId = ((ILinearShape)shape).GetNextVertexId(vertexId);

			// Do not find point position here because if controlPointId is not valid, an exception would be thrown
			this.description = string.Format("Remove point at {0}|{1} from {2} ", p.X, p.Y, shape);
		}


		/// <override></override>
		public override void Execute() {
			if (removedPointId == ControlPointId.None) throw new NShapeException("ControlPointId.None is not a valid vertex to remove.");
			// Store point position if not done yet
			if (!Geometry.IsValid(p)) p = shape.GetControlPointPosition(removedPointId);
			((ILinearShape)shape).RemoveVertex(removedPointId);
			if (Repository != null) Repository.Update(shape);
		}


		/// <override></override>
		public override void Revert() {
			if (removedPointId == ControlPointId.None) throw new NShapeException("ControlPointId.None is not a valid vertex to add.");
			ControlPointId id = ControlPointId.None;
			if (shape is LineShapeBase) {
				id = ((LineShapeBase)shape).InsertVertex(nextPointId, removedPointId, p.X, p.Y);
				Debug.Assert(id == removedPointId);
			} else id = ((ILinearShape)shape).InsertVertex(nextPointId, p.X, p.Y);
			foreach (KeyValuePair<ControlPointId, Point> item in ctrlPointPositions) {
				if (item.Key == removedPointId || item.Key == id) continue;
				shape.MoveControlPointTo(item.Key, item.Value.X, item.Value.Y, ResizeModifiers.None);
			}
			if (Repository != null) Repository.Update(shape);
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Layout; }
		}


		#region Fields
		private Point p = Geometry.InvalidPoint;
		private ControlPointId removedPointId = ControlPointId.None;
		private ControlPointId nextPointId = ControlPointId.None;
		private Dictionary<ControlPointId, Point> ctrlPointPositions = null;
		#endregion
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class AddConnectionPointCommand : ShapeCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public AddConnectionPointCommand(Shape shape, int x, int y)
			: this(null, shape, x, y) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public AddConnectionPointCommand(IRepository repository, Shape shape, int x, int y)
			: base(repository, shape) {
			if (!(shape is ILinearShape)) throw new ArgumentException(string.Format("Shape does not implement required interface {0}.", typeof(ILinearShape).Name));
			this.description = string.Format("Add Connection Point to {0} at {1}|{2}", shape, x, y);
			this.x = x;
			this.y = y;
		}


		/// <override></override>
		public override void Execute() {
			insertedPointId = ((ILinearShape)shape).AddConnectionPoint(x, y);
			shape.Invalidate();
			if (Repository != null) Repository.Update(shape);
		}


		/// <override></override>
		public override void Revert() {
			((ILinearShape)shape).RemoveConnectionPoint(insertedPointId);
			shape.Invalidate();
			if (Repository != null) Repository.Update(shape);
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Connect; }
		}


		#region Fields
		private int x;
		private int y;
		private ControlPointId insertedPointId = ControlPointId.None;
		#endregion
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class RemoveConnectionPointCommand : ShapeCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public RemoveConnectionPointCommand(Shape shape, ControlPointId removedPointId)
			: this(null, shape, removedPointId) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public RemoveConnectionPointCommand(IRepository repository, Shape shape, ControlPointId removedPointId)
			: base(repository, shape) {
			if (!(shape is ILinearShape)) throw new ArgumentException(string.Format("Shape does not implement required interface {0}.", typeof(ILinearShape).Name));
			this.removedPointId = removedPointId;
			// MenuItemDefs always create their commands, so we have to handle this
			// case instead of throwing an exception.
			if (removedPointId != ControlPointId.None)
				this.p = shape.GetControlPointPosition(removedPointId);
			this.description = string.Format("Remove connection point from {0} at {1}|{2}", shape, p.X, p.Y);
		}


		/// <override></override>
		public override void Execute() {
			((ILinearShape)shape).RemoveConnectionPoint(removedPointId);
			shape.Invalidate();
			if (Repository != null) Repository.Update(shape);
		}


		/// <override></override>
		public override void Revert() {
			ControlPointId ptId = ((ILinearShape)shape).AddConnectionPoint(p.X, p.Y);
			if (ptId != removedPointId) {
				Debug.Fail("PointId should not change when reverting a command!");
				removedPointId = ptId;
			}
			shape.Invalidate();
			if (Repository != null) Repository.Update(shape);
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Connect; }
		}


		#region Fields
		private Point p;
		private ControlPointId removedPointId = ControlPointId.None;
		#endregion
	}

	#endregion


	#region Commands for inserting/deleting/editing ModelObjects

	/// <ToBeCompleted></ToBeCompleted>
	[Obsolete("Use CreateModelObjectsCommand instead.")]
	public class InsertModelObjectsCommand : CreateModelObjectsCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public InsertModelObjectsCommand(IModelObject modelObject)
			: base(modelObject) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public InsertModelObjectsCommand(IRepository repository, IModelObject modelObject)
			: base(repository, modelObject) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public InsertModelObjectsCommand(IEnumerable<IModelObject> modelObjects)
			: base(null, modelObjects) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public InsertModelObjectsCommand(IRepository repository, IEnumerable<IModelObject> modelObjects)
			: base(repository, modelObjects) {
		}

	}


	/// <ToBeCompleted></ToBeCompleted>
	public class CreateModelObjectsCommand : InsertOrRemoveModelObjectsCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public CreateModelObjectsCommand(IModelObject modelObject)
			: this(SingleInstanceEnumerator<IModelObject>.Create(modelObject)) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public CreateModelObjectsCommand(IRepository repository, IModelObject modelObject)
			: this(repository, SingleInstanceEnumerator<IModelObject>.Create(modelObject)) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public CreateModelObjectsCommand(IEnumerable<IModelObject> modelObjects)
			: this(null, modelObjects) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public CreateModelObjectsCommand(IRepository repository, IEnumerable<IModelObject> modelObjects)
			: base(repository) {
			modelObjectBuffer = new List<IModelObject>(modelObjects);
		}


		/// <override></override>
		public override void Execute() {
			if (ModelObjects.Count == 0) SetModelObjects(modelObjectBuffer);
			InsertModelObjects(true);
		}


		/// <override></override>
		public override void Revert() {
			RemoveModelObjects(true);
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Insert; }
		}


		// ToDO: Remove this buffer as soon as the ModelObject gets a Children Property...
		private List<IModelObject> modelObjectBuffer;
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class DeleteModelObjectsCommand : InsertOrRemoveModelObjectsCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public DeleteModelObjectsCommand(IModelObject modelObject)
			: this(null, SingleInstanceEnumerator<IModelObject>.Create(modelObject)) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DeleteModelObjectsCommand(IRepository repository, IModelObject modelObject)
			: this(repository, SingleInstanceEnumerator<IModelObject>.Create(modelObject)) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DeleteModelObjectsCommand(IEnumerable<IModelObject> modelObjects)
			: this(null, modelObjects) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DeleteModelObjectsCommand(IRepository repository, IEnumerable<IModelObject> modelObjects)
			: base(repository) {
			modelObjectBuffer = new List<IModelObject>(modelObjects);
		}


		/// <override></override>
		public override void Execute() {
			if (ModelObjects.Count == 0) SetModelObjects(modelObjectBuffer);
			RemoveModelObjects(true);
		}


		/// <override></override>
		public override void Revert() {
			InsertModelObjects(true);
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Delete; }
		}


		// ToDO: Remove this buffer as soon as the ModelObject gets a Children Property...
		private List<IModelObject> modelObjectBuffer;
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class SetModelObjectParentCommand : Command {

		/// <ToBeCompleted></ToBeCompleted>
		public SetModelObjectParentCommand(IModelObject modelObject, IModelObject parent)
			: this(null, modelObject, parent) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public SetModelObjectParentCommand(IRepository repository, IModelObject modelObject, IModelObject parent)
			: base(repository) {
			if (modelObject == null) throw new ArgumentNullException("modelObject");
			if (modelObject.Parent != null && parent == null)
				this.description = string.Format(
					"Remove {0} '{1}' from hierarchical position under {2} '{3}'.",
					modelObject.Type.Name, modelObject.Name,
					modelObject.Parent.Type.Name, modelObject.Parent.Name);
			else if (modelObject.Parent == null && parent != null)
				this.description = string.Format(
					"Move {0} '{1}' to hierarchical position under {2} '{3}'.",
					modelObject.Type.Name, modelObject.Name,
					parent.Type.Name, parent.Name);
			else if (modelObject.Parent != null && parent != null)
				this.description = string.Format(
					"Change hierarchical position of {0} '{1}' from {2} '{3}' to {4} '{5}'.",
					modelObject.Type.Name, modelObject.Name,
					modelObject.Parent.Type.Name, modelObject.Parent.Name,
					parent.Type.Name, parent.Name);
			else this.description = string.Format("Move {0} '{1}'.", modelObject.Type.Name, modelObject.Name);

			this.modelObject = modelObject;
			this.oldParent = modelObject.Parent;
			this.newParent = parent;
		}


		/// <override></override>
		public override void Execute() {
			modelObject.Parent = newParent;
			if (Repository != null) Repository.UpdateOwner(modelObject, newParent);
		}


		/// <override></override>
		public override void Revert() {
			modelObject.Parent = oldParent;
			if (Repository != null) Repository.UpdateOwner(modelObject, oldParent);
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Data; }
		}


		/// <override></override>
		protected override bool CheckAllowedCore(ISecurityManager securityManager, bool createException, out Exception exception) {
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			bool isGranted = securityManager.IsGranted(RequiredPermission, newParent.Shapes);
			exception = (!isGranted && createException) ? new NShapeSecurityException(this) : null;
			return isGranted;
		}


		#region Fields
		private IModelObject modelObject;
		private IModelObject oldParent;
		private IModelObject newParent;
		#endregion
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class AssignModelObjectCommand : Command {

		/// <ToBeCompleted></ToBeCompleted>
		public AssignModelObjectCommand(Shape shape, IModelObject modelObject)
			: this(null, shape, modelObject) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public AssignModelObjectCommand(IRepository repository, Shape shape, IModelObject modelObject)
			: base(repository) {
			if (shape == null) throw new ArgumentNullException("shape");
			if (modelObject == null) throw new ArgumentNullException("modelObject");

			if (shape.ModelObject == null)
				this.description = string.Format("Assign {0} '{1}' to {2}.", modelObject.Type.Name, modelObject.Name, shape.Type.Name);
			else
				this.description = string.Format("Replace {0} '{1}' of {2} with {3} '{4}'.", shape.ModelObject.Type.Name, shape.ModelObject.Name, shape.Type.Name, modelObject.Type.Name, modelObject.Name);

			this.shape = shape;
			this.oldModelObject = shape.ModelObject;
			this.newModelObject = modelObject;
		}


		/// <override></override>
		public override void Execute() {
			shape.ModelObject = newModelObject;
			if (Repository != null) Repository.Update(shape);
		}


		/// <override></override>
		public override void Revert() {
			shape.ModelObject = oldModelObject;
			if (Repository != null) Repository.Update(shape);
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Data; }
		}


		/// <override></override>
		protected override bool CheckAllowedCore(ISecurityManager securityManager, bool createException, out Exception exception) {
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			bool isGranted = securityManager.IsGranted(RequiredPermission, shape);
			exception = (!isGranted && createException) ? new NShapeSecurityException(this) : null;
			return isGranted;
		}


		#region Fields
		private Shape shape;
		private IModelObject oldModelObject;
		private IModelObject newModelObject;
		#endregion
	}

	#endregion


	#region Commands for editing Templates

	/// <ToBeCompleted></ToBeCompleted>
	public class CreateTemplateCommand : TemplateCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public CreateTemplateCommand(Template template)
			: this(null, template) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public CreateTemplateCommand(IRepository repository, Template template)
			: base(repository, template) {
			this.description = string.Format("Create new tempate '{0}' based on '{1}'", template.Title, template.Shape.Type.Name);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public CreateTemplateCommand(string templateName, Shape baseShape)
			: this(null, templateName, baseShape) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public CreateTemplateCommand(IRepository repository, string templateName, Shape baseShape)
			: base(repository) {
			if (string.IsNullOrEmpty(templateName)) throw new ArgumentNullException("templateName");
			if (baseShape == null) throw new ArgumentNullException("baseShape");
			this.description = string.Format("Create new tempate '{0}' based on '{1}'", templateName, baseShape.Type.Name);
			this.templateName = templateName;
			this.baseShape = baseShape;
		}


		/// <override></override>
		public override void Execute() {
			if (template == null) {
				Shape templateShape = baseShape.Type.CreateInstance();
				foreach (Shape childShape in baseShape.Children.BottomUp)
					templateShape.Children.Add(childShape.Type.CreateInstance(), childShape.ZOrder);
				// ToDo: 
				// The template of the other shape will be copied although it is not really wanted that the new template bases on an other template!
				// The child shapes will be copied including their template, too. We should check if it is better to 
				// copy the shapes manually here and change the CopyFrom method of the ShapeCollection so it checks for 
				// existing shapes before starting to clone the souce's child shapes...
				templateShape.CopyFrom(baseShape);	// Copies shape and its child shapes (all including template)
				template = new Template(templateName, templateShape);
			}
			if (Repository != null) {
				if (CanUndeleteEntity(template))
					Repository.UndeleteAll(template);
				else Repository.InsertAll(template);
			}
		}


		/// <override></override>
		public override void Revert() {
			if (Repository != null) {
				Repository.Delete(template.GetPropertyMappings());
				Repository.DeleteAll(template);
			}
		}


		#region Fields
		private string templateName;
		private Shape baseShape;
		#endregion
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class DeleteTemplateCommand : TemplateCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public DeleteTemplateCommand(Template template)
			: this(null, template) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DeleteTemplateCommand(IRepository repository, Template template)
			: base(repository, template) {
			this.description = string.Format("Delete tempate '{0}' based on '{1}'", template.Title, template.Shape.Type.Name);
		}


		/// <override></override>
		public override void Execute() {
			if (Repository != null) {
				Repository.Delete(template.GetPropertyMappings());
				Repository.DeleteAll(template);
			}
		}


		/// <override></override>
		public override void Revert() {
			if (Repository != null) {
				if (CanUndeleteEntity(template))
					Repository.UndeleteAll(template);
				else Repository.InsertAll(template);
			}
		}

	}


	/// <ToBeCompleted></ToBeCompleted>
	[Obsolete("Use CopyTemplateFromTemplateCommand instead.")]
	public class ExchangeTemplateCommand : CopyTemplateFromTemplateCommand {
		
		/// <ToBeCompleted></ToBeCompleted>
		public ExchangeTemplateCommand(Template originalTemplate, Template changedTemplate)
			: base(originalTemplate, changedTemplate) {
		}

		/// <ToBeCompleted></ToBeCompleted>
		public ExchangeTemplateCommand(IRepository repository, Template originalTemplate, Template changedTemplate)
			: base(repository, originalTemplate, changedTemplate) {
		}

	}


	/// <summary>
	/// Copies all properties and objects of the changed template into the original template.
	/// </summary>
	public class CopyTemplateFromTemplateCommand : Command {

		/// <ToBeCompleted></ToBeCompleted>
		public CopyTemplateFromTemplateCommand(Template originalTemplate, Template changedTemplate)
			: this(null, originalTemplate, changedTemplate) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public CopyTemplateFromTemplateCommand(IRepository repository, Template originalTemplate, Template changedTemplate)
			: base(repository) {
			if (originalTemplate == null) throw new ArgumentNullException("originalTemplate");
			if (changedTemplate == null) throw new ArgumentNullException("changedTemplate");
			this.description = string.Format("Change tempate '{0}'", originalTemplate.Title);
			this.originalTemplate = originalTemplate;
			this.oldTemplate = new Template(originalTemplate.Name, originalTemplate.Shape.Clone());
			this.oldTemplate.CopyFrom(originalTemplate);
			this.newTemplate = new Template(changedTemplate.Name, changedTemplate.Shape.Clone());
			this.newTemplate.CopyFrom(changedTemplate);
		}


		/// <override></override>
		public override void Execute() {
			DoExchangeTemplates(originalTemplate, oldTemplate, newTemplate);
		}


		/// <override></override>
		public override void Revert() {
			DoExchangeTemplates(originalTemplate, newTemplate, oldTemplate);
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Templates; }
		}


		/// <override></override>
		protected override bool CheckAllowedCore(ISecurityManager securityManager, bool createException, out Exception exception) {
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			bool isGranted = securityManager.IsGranted(RequiredPermission);
			exception = (!isGranted && createException) ? new NShapeSecurityException(this) : null;
			return isGranted;
		}


		private void DoExchangeTemplates(Template originalTemplate, Template oldTemplate, Template newTemplate) {
			if (Repository != null) {
				// Delete all existing model mappings
				Repository.Delete(originalTemplate.GetPropertyMappings());
				IModelObject originalModelObject = originalTemplate.Shape.ModelObject;
				// We delete the whole shape here in order to make sure that all deleted 
				// content of the shape is deleted from the repository.
				// Afterwards, the shape and its existing/new content will be undeleted/inserted.
				originalTemplate.Shape.ModelObject = null;		// Detach shape from model object				
				Repository.DeleteAll(originalTemplate.Shape);
				// Always delete the current model object if it has been changed
				if (originalModelObject != null && oldTemplate.Shape.ModelObject != newTemplate.Shape.ModelObject)
					Repository.Delete(originalModelObject);
			}
			// ToDo: Handle exchanging shapes of different ShapeTypes
			originalTemplate.CopyFrom(newTemplate);

			// Update template (does NOT include inserting/undeleting/updating the shape and model object)
			if (Repository != null) {
				// Insert/ update / undelete model object
				if (originalTemplate.Shape.ModelObject != null) {
					if (oldTemplate.Shape.ModelObject == newTemplate.Shape.ModelObject) {
						// Update model object 
						Repository.UpdateOwner(originalTemplate.Shape.ModelObject, originalTemplate);
						Repository.Update(originalTemplate.Shape.ModelObject);
					} else {
						// Insert / undelete model object if has been replaced
						if (CanUndeleteEntity(originalTemplate.Shape.ModelObject))
							Repository.Undelete(originalTemplate.Shape.ModelObject);
						else Repository.Insert(originalTemplate.Shape.ModelObject, originalTemplate);
					}
				}
				// Insert/undelete template shape
				if (CanUndeleteEntity(originalTemplate.Shape))
					Repository.Undelete(originalTemplate.Shape, originalTemplate);
				else Repository.Insert(originalTemplate.Shape, originalTemplate);
				// Insert template shape's children (CopyFrom() creates new children)
				if (originalTemplate.Shape.Children.Count > 0)
					Repository.InsertAll(originalTemplate.Shape.Children, originalTemplate.Shape);
				// Update template
				Repository.Update(originalTemplate);
				// Insert/undelete model mappings
				foreach (IModelMapping modelMapping in originalTemplate.GetPropertyMappings()) {
					if (CanUndeleteEntity(modelMapping))
						Repository.Undelete(modelMapping, originalTemplate);
					else Repository.Insert(modelMapping, originalTemplate);
				}
			}
			InvalidateTemplateShapes();
		}


		private void DisposeShape(Shape shape) {
			if (shape.Children.Count > 0) {
				foreach (Shape childShape in shape.Children)
					DisposeShape(childShape);
			}
			shape.Dispose();
		}


		private void InvalidateTemplateShapes() {
			// Invalidate all changed selectedShapes
			if (Repository != null) {
				foreach (Diagram diagram in Repository.GetDiagrams()) {
					foreach (Shape shape in diagram.Shapes) {
						if (originalTemplate == shape.Template)
							shape.NotifyStyleChanged(null);
					}
				}
			}
		}


		#region Fields
		private Template originalTemplate;
		private Template oldTemplate;
		private Template newTemplate;

		private List<ShapeConnectionInfo> shapeConnectionInfos = new List<ShapeConnectionInfo>();
		#endregion
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class ExchangeTemplateShapeCommand : Command {

		/// <ToBeCompleted></ToBeCompleted>
		public ExchangeTemplateShapeCommand(Template originalTemplate, Template changedTemplate)
			: this(null, originalTemplate, changedTemplate) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public ExchangeTemplateShapeCommand(IRepository repository, Template originalTemplate, Template changedTemplate)
			: base(repository) {
			if (originalTemplate == null) throw new ArgumentNullException("originalTemplate");
			if (changedTemplate == null) throw new ArgumentNullException("changedTemplate");
			this.description = string.Format("Change shape of tempate  '{0}' from '{1}' to '{2}'", originalTemplate.Title, originalTemplate.Shape.Type.Name, changedTemplate.Shape.Type.Name);
			this.originalTemplate = originalTemplate;
			this.oldTemplate = originalTemplate.Clone();
			this.newTemplate = changedTemplate;

			this.oldTemplateShape = originalTemplate.Shape;
			this.newTemplateShape = changedTemplate.Shape;
			this.newTemplateShape.DisplayService = oldTemplateShape.DisplayService;
		}


		/// <override></override>
		public override void Execute() {
			if (Repository != null && shapesFromTemplate == null)
				shapesFromTemplate = new List<ReplaceShapesBuffer>(GetShapesToReplace());

			// Copy all Properties of the new template to the reference of the original Template
			originalTemplate.Shape = null;
			originalTemplate.CopyFrom(newTemplate);
			originalTemplate.Shape = newTemplateShape;
			// exchange oldShapes with newShapes
			int cnt = shapesFromTemplate.Count;
			for (int i = 0; i < cnt; ++i) {
				ReplaceShapes(shapesFromTemplate[i].diagram,
									shapesFromTemplate[i].oldShape,
									shapesFromTemplate[i].newShape,
									shapesFromTemplate[i].oldConnections,
									shapesFromTemplate[i].newConnections);
			}
			if (Repository != null)
				Repository.ReplaceTemplateShape(originalTemplate, oldTemplateShape, newTemplateShape);
		}


		/// <override></override>
		public override void Revert() {
			originalTemplate.Shape = null;
			originalTemplate.CopyFrom(oldTemplate);
			originalTemplate.Shape = oldTemplateShape;
			// exchange old shape with new Shape
			int cnt = shapesFromTemplate.Count;
			for (int i = 0; i < cnt; ++i)
				ReplaceShapes(shapesFromTemplate[i].diagram,
									shapesFromTemplate[i].newShape,
									shapesFromTemplate[i].oldShape,
									shapesFromTemplate[i].newConnections,
									shapesFromTemplate[i].oldConnections);

			if (Repository != null)
				Repository.ReplaceTemplateShape(originalTemplate, newTemplateShape, oldTemplateShape);
		}


		private IEnumerable<ReplaceShapesBuffer> GetShapesToReplace() {
			// ToDo: In future versions, the cache should handle this by exchanging the loaded shapes at once and the unloaded shapes next time they are loaded
			// For now, find all Shapes in the Cache created from the changed Template and change their shape's type
			foreach (Diagram diagram in Repository.GetDiagrams()) {
				foreach (Shape shape in diagram.Shapes) {
					if (shape.Template == originalTemplate) {
						// copy as much properties as possible from the old shape into the new shape
						ReplaceShapesBuffer buffer = new ReplaceShapesBuffer();
						buffer.diagram = diagram;
						buffer.oldShape = shape;
						// Create a new shape instance refering to the original template
						buffer.newShape = newTemplate.Shape.Type.CreateInstance(originalTemplate);
						buffer.newShape.CopyFrom(newTemplate.Shape);	// Copy all properties of the new shape (Template will not be copied)
						buffer.oldConnections = new List<ShapeConnectionInfo>(shape.GetConnectionInfos(ControlPointId.Any, null));
						buffer.newConnections = new List<ShapeConnectionInfo>(buffer.oldConnections.Count);
						foreach (ShapeConnectionInfo sci in buffer.oldConnections) {
							// find a matching connection point...
							ControlPointId ptId = ControlPointId.None;
							if (sci.OwnPointId == ControlPointId.Reference)
								ptId = ControlPointId.Reference;	// Point-To-Shape connections are always possible
							else {
								// try to find a connection point with the same point id
								foreach (ControlPointId id in buffer.newShape.GetControlPointIds(ControlPointCapabilities.Connect)) {
									if (id == sci.OwnPointId) {
										ptId = id;
										break;
									}
								}
							}
							// if the desired point is not a valid ConnectionPoint, create a Point-To-Shape connection
							if (ptId == ControlPointId.None)
								ptId = ControlPointId.Reference;
							// store the new connectionInfo
							buffer.newConnections.Add(new ShapeConnectionInfo(ptId, sci.OtherShape, sci.OtherPointId));
						}

						// Make the new shape about the size of the original one
						Rectangle oldBounds = buffer.oldShape.GetBoundingRectangle(true);
						Rectangle newBounds = buffer.newShape.GetBoundingRectangle(true);
						if (newBounds != oldBounds)
							buffer.newShape.Fit(oldBounds.X, oldBounds.Y, oldBounds.Width, oldBounds.Height);

						yield return buffer;
					}
				}
			}
		}


		private void ReplaceShapes(Diagram diagram, Shape oldShape, Shape newShape, IEnumerable<ShapeConnectionInfo> oldConnections, IEnumerable<ShapeConnectionInfo> newConnections) {
			oldShape.Invalidate();
			// Disconnect all connections to the old shape
			foreach (ShapeConnectionInfo sci in oldConnections) {
				Debug.Assert(oldShape.IsConnected(ControlPointId.Any, null) != ControlPointId.None);
				if (sci.OtherShape.HasControlPointCapability(sci.OtherPointId, ControlPointCapabilities.Glue)) {
					sci.OtherShape.Disconnect(sci.OtherPointId);
					if (Repository != null) Repository.DeleteConnection(sci.OtherShape, sci.OtherPointId, oldShape, sci.OwnPointId);
				} else {
					oldShape.Disconnect(sci.OwnPointId);
					if (Repository != null) Repository.DeleteConnection(oldShape, sci.OwnPointId, sci.OtherShape, sci.OtherPointId);
				}
			}
			// Delete children of old template shape because the children will be discarded in CopyFrom()
			if (Repository != null) {
				if (oldShape.Children.Count > 0)
					Repository.DeleteAll(oldShape.Children);
			}
			// Exchange the shapes before deleting the old shape from the repository because it 
			// might be removed from the diagram in a "ShapeDeleted" event handler
			newShape.CopyFrom(oldShape);	// Template will not be copied
			diagram.Shapes.Replace(oldShape, newShape);
			if (Repository != null) {
				Repository.Delete(oldShape);
				if (CanUndeleteEntity(newShape))
					Repository.Undelete(newShape, diagram);
				else Repository.Insert(newShape, diagram);
				if (newShape.Children.Count > 0)
					Repository.InsertAll(newShape.Children, newShape);
			}
			//
			// Restore all connections to the new shape
			foreach (ShapeConnectionInfo sci in newConnections) {
				Debug.Assert(newShape.HasControlPointCapability(sci.OwnPointId, ControlPointCapabilities.Connect) ||
							newShape.HasControlPointCapability(sci.OwnPointId, ControlPointCapabilities.Glue));
				if (newShape.HasControlPointCapability(sci.OwnPointId, ControlPointCapabilities.Glue)) {
					newShape.Connect(sci.OwnPointId, sci.OtherShape, sci.OtherPointId);
					if (Repository != null) Repository.InsertConnection(newShape, sci.OwnPointId, sci.OtherShape, sci.OtherPointId);
				} else {
					sci.OtherShape.Connect(sci.OtherPointId, newShape, sci.OwnPointId);
					if (Repository != null) Repository.InsertConnection(sci.OtherShape, sci.OtherPointId, newShape, sci.OwnPointId);
				}
			}
			newShape.Invalidate();
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get {
				//return Permission.ModifyData | Permission.Present | Permission.Connect | Permission.Templates; 
				return Permission.Templates;
			}
		}


		/// <override></override>
		protected override bool CheckAllowedCore(ISecurityManager securityManager, bool createException, out Exception exception) {
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			bool isGranted = securityManager.IsGranted(RequiredPermission);
			exception = (!isGranted && createException) ? new NShapeSecurityException(this) : null;
			return isGranted;
		}


		#region Fields

		private struct ReplaceShapesBuffer : IEquatable<ReplaceShapesBuffer> {
			public Diagram diagram;
			public Shape oldShape;
			public Shape newShape;
			public List<ShapeConnectionInfo> oldConnections;
			public List<ShapeConnectionInfo> newConnections;
			public bool Equals(ReplaceShapesBuffer other) {
				return (other.diagram == this.diagram
					&& other.newConnections == this.newConnections
					&& other.newShape == this.newShape
					&& other.oldConnections == this.oldConnections
					&& other.oldShape == this.oldShape);
			}
		}

		private Template originalTemplate;	// reference on the (original) Template which has to be changed
		private Template oldTemplate;			// a clone of the original Template (needed for reverting the command)
		private Template newTemplate;			// a clone of the new Template
		private Shape oldTemplateShape;		// the original template shape
		private Shape newTemplateShape;		// the new template shape
		private List<ReplaceShapesBuffer> shapesFromTemplate = null;
		#endregion
	}

	#endregion


	#region Commands for PropertyController

	/// <ToBeCompleted></ToBeCompleted>
	public abstract class PropertySetCommand<T> : Command {

		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.PropertySetCommand`1" />.
		/// </summary>
		public PropertySetCommand(T modifiedObject, PropertyInfo propertyInfo, object oldValue, object newValue)
			: this(null, modifiedObject, propertyInfo, oldValue, newValue) {
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.PropertySetCommand`1" />.
		/// </summary>
		public PropertySetCommand(IRepository repository, T modifiedObject, PropertyInfo propertyInfo, object oldValue, object newValue)
			: this(null, SingleInstanceEnumerator<T>.Create(modifiedObject), propertyInfo, SingleInstanceEnumerator<object>.Create(oldValue), SingleInstanceEnumerator<object>.Create(newValue)) {
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.PropertySetCommand`1" />.
		/// </summary>
		public PropertySetCommand(IEnumerable<T> modifiedObjects, PropertyInfo propertyInfo, IEnumerable<object> oldValues, object newValue)
			: this(null, modifiedObjects, propertyInfo, oldValues, newValue) {
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.PropertySetCommand`1" />.
		/// </summary>
		public PropertySetCommand(IRepository repository, IEnumerable<T> modifiedObjects, PropertyInfo propertyInfo, IEnumerable<object> oldValues, object newValue)
			: this(repository, modifiedObjects, propertyInfo, oldValues, SingleInstanceEnumerator<object>.Create(newValue)) {
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.PropertySetCommand`1" />.
		/// </summary>
		public PropertySetCommand(IEnumerable<T> modifiedObjects, PropertyInfo propertyInfo, IEnumerable<object> oldValues, IEnumerable<object> newValues)
			: this(null, modifiedObjects, propertyInfo, oldValues, newValues) {
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.PropertySetCommand`1" />.
		/// </summary>
		public PropertySetCommand(IRepository repository, IEnumerable<T> modifiedObjects, PropertyInfo propertyInfo, IEnumerable<object> oldValues, IEnumerable<object> newValues)
			: base() {
			if (modifiedObjects == null) throw new ArgumentNullException("modifiedObjects");
			Construct(propertyInfo);
			this.modifiedObjects = new List<T>(modifiedObjects);
			this.oldValues = new List<object>(oldValues);
			this.newValues = new List<object>(newValues);
		}


		/// <override></override>
		public override void Execute() {
			int valCnt = newValues.Count;
			int objCnt = modifiedObjects.Count;
			for (int i = 0; i < objCnt; ++i) {
				object newValue = newValues[(valCnt == objCnt) ? i : 0];
				object currValue = propertyInfo.GetValue(modifiedObjects[i], null);
				// Check if the new value has been set already (e.g. by the PropertyGrid). If the new value is 
				// already set, skip setting the new value (again).
				// This check is necessary because if the value is a value that is exclusive-or'ed when set 
				// (e.g. a FontStyle), the change would be undone when setting the value again
				if (currValue == null && newValue != null
					|| currValue != null && newValue == null
					|| (currValue != null && !currValue.Equals(newValue))) {
					propertyInfo.SetValue(modifiedObjects[i], newValue, null);
				}
			}
		}


		/// <override></override>
		public override void Revert() {
			int valCnt = oldValues.Count;
			int objCnt = modifiedObjects.Count;
			for (int i = 0; i < objCnt; ++i) {
				object oldValue = oldValues[(valCnt == objCnt) ? i : 0];
				object currValue = propertyInfo.GetValue(modifiedObjects[i], null);
				if (currValue == null && oldValue != null
					|| (currValue != null && !currValue.Equals(oldValue)))
					propertyInfo.SetValue(modifiedObjects[i], oldValue, null);
			}
		}


		/// <override></override>
		public override string Description {
			get {
				if (string.IsNullOrEmpty(description)) {
					if (modifiedObjects.Count == 1)
						description = string.Format("Change property '{0}' of {1} from '{2}' to '{3}'",
							propertyInfo.Name, modifiedObjects[0].GetType().Name, oldValues[0], newValues[0]);
					else {
						if (oldValues.Count == 1 && newValues.Count == 1) {
							description = string.Format("Change property '{0}' of {1} {2}{3} from '{4}' to '{5}'",
								propertyInfo.Name,
								this.modifiedObjects.Count,
								this.modifiedObjects[0].GetType().Name,
								this.modifiedObjects.Count > 1 ? "s" : string.Empty,
								oldValues[0],
								newValues[0]);
						} else if (oldValues.Count > 1 && newValues.Count == 1) {
							this.description = string.Format("Change property '{0}' of {1} {2}{3} to '{4}'",
								propertyInfo.Name,
								this.modifiedObjects.Count,
								this.modifiedObjects[0].GetType().Name,
								this.modifiedObjects.Count > 1 ? "s" : string.Empty,
								newValues[0]);
						} else {
							description = string.Format("Change property '{0}' of {1} {2}{3}",
								propertyInfo.Name, this.modifiedObjects.Count, typeof(T).Name,
								modifiedObjects.Count > 1 ? "s" : string.Empty);
						}
					}
				}
				return base.Description;
			}
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return requiredPermissions; }
		}


		/// <override></override>
		protected override bool CheckAllowedCore(ISecurityManager securityManager, bool createException, out Exception exception) {
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			bool isGranted = true;
			for (int i = modifiedObjects.Count - 1; i >= 0; --i) {
				if (modifiedObjects[i] is ISecurityDomainObject) {
					if (!securityManager.IsGranted(RequiredPermission, ((ISecurityDomainObject)modifiedObjects[i]).SecurityDomainName))
						isGranted = false;
				} else if (!securityManager.IsGranted(RequiredPermission))
					isGranted = false;
				if (!isGranted) break;
			}
			exception = (!isGranted && createException) ? new NShapeSecurityException(this) : null;
			return isGranted;
		}


		private void Construct(PropertyInfo propertyInfo) {
			if (propertyInfo == null) throw new ArgumentNullException("propertyInfo");
			this.propertyInfo = propertyInfo;
			// Retrieve required permissions
			requiredPermissions = Permission.None;
			RequiredPermissionAttribute attr = Attribute.GetCustomAttribute(propertyInfo, typeof(RequiredPermissionAttribute)) as RequiredPermissionAttribute;
			requiredPermissions = (attr != null) ? attr.Permission : Permission.None;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected PropertyInfo propertyInfo;
		/// <ToBeCompleted></ToBeCompleted>
		protected List<object> oldValues;
		/// <ToBeCompleted></ToBeCompleted>
		protected List<object> newValues;
		/// <ToBeCompleted></ToBeCompleted>
		protected List<T> modifiedObjects;
		/// <ToBeCompleted></ToBeCompleted>
		protected Permission requiredPermissions;
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class ShapePropertySetCommand : PropertySetCommand<Shape> {

		/// <ToBeCompleted></ToBeCompleted>
		public ShapePropertySetCommand(IEnumerable<Shape> modifiedShapes, PropertyInfo propertyInfo, IEnumerable<object> oldValues, IEnumerable<object> newValues)
			: base(modifiedShapes, propertyInfo, oldValues, newValues) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public ShapePropertySetCommand(IRepository repository, IEnumerable<Shape> modifiedShapes, PropertyInfo propertyInfo, IEnumerable<object> oldValues, IEnumerable<object> newValues)
			: base(repository, modifiedShapes, propertyInfo, oldValues, newValues) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public ShapePropertySetCommand(IEnumerable<Shape> modifiedShapes, PropertyInfo propertyInfo, IEnumerable<object> oldValues, object newValue)
			: base(modifiedShapes, propertyInfo, oldValues, newValue) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public ShapePropertySetCommand(IRepository repository, IEnumerable<Shape> modifiedShapes, PropertyInfo propertyInfo, IEnumerable<object> oldValues, object newValue)
			: base(repository, modifiedShapes, propertyInfo, oldValues, newValue) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public ShapePropertySetCommand(Shape modifiedShape, PropertyInfo propertyInfo, object oldValue, object newValue)
			: base(modifiedShape, propertyInfo, oldValue, newValue) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public ShapePropertySetCommand(IRepository repository, Shape modifiedShape, PropertyInfo propertyInfo, object oldValue, object newValue)
			: base(repository, modifiedShape, propertyInfo, oldValue, newValue) {
		}


		/// <override></override>
		public override void Execute() {
			base.Execute();
			if (Repository != null) {
				if (modifiedObjects.Count == 1) Repository.Update(modifiedObjects[0]);
				else Repository.Update(modifiedObjects);
			}
		}


		/// <override></override>
		public override void Revert() {
			base.Revert();
			if (Repository != null) {
				if (modifiedObjects.Count == 1) Repository.Update(modifiedObjects[0]);
				else Repository.Update(modifiedObjects);
			}
		}


		///// <override></override>
		//public override Permission RequiredPermission {
		//    get { return Permission.Present | Permission.ModifyData | Permission.Layout; }
		//}


		/// <override></override>
		protected override bool CheckAllowedCore(ISecurityManager securityManager, bool createException, out Exception exception) {
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			bool isGranted = securityManager.IsGranted(RequiredPermission, modifiedObjects);
			exception = (!isGranted && createException) ? new NShapeSecurityException(this) : null;
			return isGranted;
		}

	}


	/// <ToBeCompleted></ToBeCompleted>
	public class DiagramPropertySetCommand : PropertySetCommand<Diagram> {

		/// <ToBeCompleted></ToBeCompleted>
		public DiagramPropertySetCommand(IEnumerable<Diagram> modifiedDiagrams, PropertyInfo propertyInfo, IEnumerable<object> oldValues, IEnumerable<object> newValues)
			: base(modifiedDiagrams, propertyInfo, oldValues, newValues) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DiagramPropertySetCommand(IRepository repository, IEnumerable<Diagram> modifiedDiagrams, PropertyInfo propertyInfo, IEnumerable<object> oldValues, IEnumerable<object> newValues)
			: base(repository, modifiedDiagrams, propertyInfo, oldValues, newValues) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DiagramPropertySetCommand(IEnumerable<Diagram> modifiedDiagrams, PropertyInfo propertyInfo, IEnumerable<object> oldValues, object newValue)
			: base(modifiedDiagrams, propertyInfo, oldValues, newValue) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DiagramPropertySetCommand(IRepository repository, IEnumerable<Diagram> modifiedDiagrams, PropertyInfo propertyInfo, IEnumerable<object> oldValues, object newValue)
			: base(repository, modifiedDiagrams, propertyInfo, oldValues, newValue) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DiagramPropertySetCommand(Diagram modifiedDiagram, PropertyInfo propertyInfo, object oldValue, object newValue)
			: base(modifiedDiagram, propertyInfo, oldValue, newValue) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DiagramPropertySetCommand(IRepository repository, Diagram modifiedDiagram, PropertyInfo propertyInfo, object oldValue, object newValue)
			: base(repository, modifiedDiagram, propertyInfo, oldValue, newValue) {
		}


		/// <override></override>
		public override void Execute() {
			base.Execute();
			if (Repository != null) {
				for (int i = modifiedObjects.Count - 1; i >= 0; --i)
					Repository.Update(modifiedObjects[i]);
			}
		}


		/// <override></override>
		public override void Revert() {
			base.Revert();
			if (Repository != null) {
				for (int i = modifiedObjects.Count - 1; i >= 0; --i)
					Repository.Update(modifiedObjects[i]);
			}
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return (requiredPermissions != Permission.None) ? requiredPermissions : (Permission.Data | Permission.Present); }
		}


		//public override bool IsAllowed(ISecurityManager securityManager) {
		//    if (securityManager == null) throw new ArgumentNullException("securityManager");
		//    for (int i = modifiedObjects.Count - 1; i >= 0; --i) {
		//        if (!securityManager.IsGranted(RequiredPermission, modifiedObjects[i].SecurityDomainName))
		//            return false;
		//    }
		//    return true;
		//}

	}


	/// <ToBeCompleted></ToBeCompleted>
	public class DesignPropertySetCommand : PropertySetCommand<Design> {

		/// <ToBeCompleted></ToBeCompleted>
		public DesignPropertySetCommand(IEnumerable<Design> modifiedDesigns, PropertyInfo propertyInfo, IEnumerable<object> oldValues, IEnumerable<object> newValues)
			: base(modifiedDesigns, propertyInfo, oldValues, newValues) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DesignPropertySetCommand(IRepository repository, IEnumerable<Design> modifiedDesigns, PropertyInfo propertyInfo, IEnumerable<object> oldValues, IEnumerable<object> newValues)
			: base(repository, modifiedDesigns, propertyInfo, oldValues, newValues) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DesignPropertySetCommand(IEnumerable<Design> modifiedDesigns, PropertyInfo propertyInfo, IEnumerable<object> oldValues, object newValue)
			: base(modifiedDesigns, propertyInfo, oldValues, newValue) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DesignPropertySetCommand(IRepository repository, IEnumerable<Design> modifiedDesigns, PropertyInfo propertyInfo, IEnumerable<object> oldValues, object newValue)
			: base(repository, modifiedDesigns, propertyInfo, oldValues, newValue) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DesignPropertySetCommand(Design modifiedDesign, PropertyInfo propertyInfo, object oldValue, object newValue)
			: base(modifiedDesign, propertyInfo, oldValue, newValue) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DesignPropertySetCommand(IRepository repository, Design modifiedDesign, PropertyInfo propertyInfo, object oldValue, object newValue)
			: base(repository, modifiedDesign, propertyInfo, oldValue, newValue) {
		}


		/// <override></override>
		public override void Execute() {
			base.Execute();
			if (Repository != null) {
				for (int i = modifiedObjects.Count - 1; i >= 0; --i)
					Repository.Update(modifiedObjects[i]);
			}
		}


		/// <override></override>
		public override void Revert() {
			base.Revert();
			if (Repository != null) {
				for (int i = modifiedObjects.Count - 1; i >= 0; --i)
					Repository.Update(modifiedObjects[i]);
			}
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Designs; }
		}


		//public override bool IsAllowed(ISecurityManager securityManager) {
		//    if (securityManager == null) throw new ArgumentNullException("securityManager");
		//    return securityManager.IsGranted(RequiredPermission);
		//}

	}


	/// <ToBeCompleted></ToBeCompleted>
	public class LayerPropertySetCommand : PropertySetCommand<Layer> {

		/// <ToBeCompleted></ToBeCompleted>
		public LayerPropertySetCommand(Diagram diagram, IEnumerable<Layer> modifiedLayers, PropertyInfo propertyInfo, IEnumerable<object> oldValues, IEnumerable<object> newValues)
			: base(modifiedLayers, propertyInfo, oldValues, newValues) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			this.diagram = diagram;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public LayerPropertySetCommand(IRepository repository, Diagram diagram, IEnumerable<Layer> modifiedLayers, PropertyInfo propertyInfo, IEnumerable<object> oldValues, IEnumerable<object> newValues)
			: base(repository, modifiedLayers, propertyInfo, oldValues, newValues) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			this.diagram = diagram;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public LayerPropertySetCommand(Diagram diagram, IEnumerable<Layer> modifiedLayers, PropertyInfo propertyInfo, IEnumerable<object> oldValues, object newValue)
			: base(modifiedLayers, propertyInfo, oldValues, newValue) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			this.diagram = diagram;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public LayerPropertySetCommand(IRepository repository, Diagram diagram, IEnumerable<Layer> modifiedLayers, PropertyInfo propertyInfo, IEnumerable<object> oldValues, object newValue)
			: base(repository, modifiedLayers, propertyInfo, oldValues, newValue) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			this.diagram = diagram;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public LayerPropertySetCommand(Diagram diagram, Layer modifiedLayer, PropertyInfo propertyInfo, object oldValue, object newValue)
			: base(modifiedLayer, propertyInfo, oldValue, newValue) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			this.diagram = diagram;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public LayerPropertySetCommand(IRepository repository, Diagram diagram, Layer modifiedLayer, PropertyInfo propertyInfo, object oldValue, object newValue)
			: base(repository, modifiedLayer, propertyInfo, oldValue, newValue) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			this.diagram = diagram;
		}


		/// <override></override>
		public override void Execute() {
			base.Execute();
			if (Repository != null) Repository.Update(diagram);
		}


		/// <override></override>
		public override void Revert() {
			base.Revert();
			if (Repository != null) Repository.Update(diagram);
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return (requiredPermissions != Permission.None) ? requiredPermissions : Permission.Layout; }
		}


		/// <override></override>
		protected override bool CheckAllowedCore(ISecurityManager securityManager, bool createException, out Exception exception) {
			if (securityManager == null) throw new ArgumentNullException("securityManager");
			bool isGranted = securityManager.IsGranted(RequiredPermission, diagram.SecurityDomainName);
			exception = (!isGranted && createException) ? new NShapeSecurityException(this) : null;
			return isGranted;
		}


		#region Fields
		private Diagram diagram;
		#endregion
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class StylePropertySetCommand : PropertySetCommand<Style> {

		/// <ToBeCompleted></ToBeCompleted>
		public StylePropertySetCommand(Design design, IEnumerable<Style> modifiedStyles, PropertyInfo propertyInfo, IEnumerable<object> oldValues, IEnumerable<object> newValues)
			: base(modifiedStyles, propertyInfo, oldValues, newValues) {
			if (design == null) throw new ArgumentNullException("design");
			this.design = design;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public StylePropertySetCommand(IRepository repository, Design design, IEnumerable<Style> modifiedStyles, PropertyInfo propertyInfo, IEnumerable<object> oldValues, IEnumerable<object> newValues)
			: base(repository, modifiedStyles, propertyInfo, oldValues, newValues) {
			if (design == null) throw new ArgumentNullException("design");
			this.design = design;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public StylePropertySetCommand(Design design, IEnumerable<Style> modifiedStyles, PropertyInfo propertyInfo, IEnumerable<object> oldValues, object newValue)
			: base(modifiedStyles, propertyInfo, oldValues, newValue) {
			if (design == null) throw new ArgumentNullException("design");
			this.design = design;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public StylePropertySetCommand(IRepository repository, Design design, IEnumerable<Style> modifiedStyles, PropertyInfo propertyInfo, IEnumerable<object> oldValues, object newValue)
			: base(repository, modifiedStyles, propertyInfo, oldValues, newValue) {
			if (design == null) throw new ArgumentNullException("design");
			this.design = design;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public StylePropertySetCommand(Design design, Style modifiedStyle, PropertyInfo propertyInfo, object oldValue, object newValue)
			: base(modifiedStyle, propertyInfo, oldValue, newValue) {
			if (design == null) throw new ArgumentNullException("design");
			this.design = design;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public StylePropertySetCommand(IRepository repository, Design design, Style modifiedStyle, PropertyInfo propertyInfo, object oldValue, object newValue)
			: base(repository, modifiedStyle, propertyInfo, oldValue, newValue) {
			if (design == null) throw new ArgumentNullException("design");
			this.design = design;
		}


		/// <override></override>
		public override void Execute() {
			bool styleRenamed = propertyInfo.Name == "Name";
			if (styleRenamed) RemoveModifiedStyles();
			base.Execute();
			UpdateDesignAndRepository(styleRenamed);
		}


		/// <override></override>
		public override void Revert() {
			bool styleRenamed = propertyInfo.Name == "Name";
			if (styleRenamed) RemoveModifiedStyles();
			base.Revert();
			UpdateDesignAndRepository(styleRenamed);
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Designs; }
		}


		//public override bool IsAllowed(ISecurityManager securityManager) {
		//    if (securityManager == null) throw new ArgumentNullException("securityManager");
		//    return securityManager.IsGranted(RequiredPermission);
		//}


		private void UpdateDesignAndRepository(bool styleRenamed) {
			for (int i = modifiedObjects.Count - 1; i >= 0; --i) {
				if (styleRenamed) design.AddStyle(modifiedObjects[i]);
				if (Repository != null) Repository.Update(modifiedObjects[i]);
			}
			if (Repository != null) Repository.Update(design);
		}


		private void RemoveModifiedStyles() {
			for (int i = modifiedObjects.Count - 1; i >= 0; --i)
				design.RemoveStyle(modifiedObjects[i].Name, modifiedObjects[i].GetType());
		}


		#region Fields
		private Design design;
		#endregion
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class ModelObjectPropertySetCommand : PropertySetCommand<IModelObject> {

		/// <ToBeCompleted></ToBeCompleted>
		public ModelObjectPropertySetCommand(IEnumerable<IModelObject> modifiedModelObjects, PropertyInfo propertyInfo, IEnumerable<object> oldValues, IEnumerable<object> newValues)
			: base(modifiedModelObjects, propertyInfo, oldValues, newValues) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public ModelObjectPropertySetCommand(IRepository repository, IEnumerable<IModelObject> modifiedModelObjects, PropertyInfo propertyInfo, IEnumerable<object> oldValues, IEnumerable<object> newValues)
			: base(repository, modifiedModelObjects, propertyInfo, oldValues, newValues) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public ModelObjectPropertySetCommand(IEnumerable<IModelObject> modifiedModelObjects, PropertyInfo propertyInfo, IEnumerable<object> oldValues, object newValue)
			: base(modifiedModelObjects, propertyInfo, oldValues, newValue) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public ModelObjectPropertySetCommand(IRepository repository, IEnumerable<IModelObject> modifiedModelObjects, PropertyInfo propertyInfo, IEnumerable<object> oldValues, object newValue)
			: base(repository, modifiedModelObjects, propertyInfo, oldValues, newValue) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public ModelObjectPropertySetCommand(IModelObject modifiedModelObject, PropertyInfo propertyInfo, object oldValue, object newValue)
			: base(modifiedModelObject, propertyInfo, oldValue, newValue) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public ModelObjectPropertySetCommand(IRepository repository, IModelObject modifiedModelObject, PropertyInfo propertyInfo, object oldValue, object newValue)
			: base(repository, modifiedModelObject, propertyInfo, oldValue, newValue) {
		}


		/// <override></override>
		public override void Execute() {
			base.Execute();
			if (Repository != null) {
				if (modifiedObjects.Count == 1) Repository.Update(modifiedObjects[0]);
				else Repository.Update(modifiedObjects);
			}
		}


		/// <override></override>
		public override void Revert() {
			base.Revert();
			if (Repository != null) {
				if (modifiedObjects.Count == 1) Repository.Update(modifiedObjects[0]);
				else Repository.Update(modifiedObjects);
			}
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return (requiredPermissions != Permission.None) ? requiredPermissions : Permission.Data; }
		}


		//public override bool IsAllowed(ISecurityManager securityManager) {
		//    if (securityManager == null) throw new ArgumentNullException("securityManager");
		//    for (int i = modifiedObjects.Count - 1; i >= 0; --i) {
		//        if (!securityManager.IsGranted(RequiredPermission, modifiedObjects[i].Shapes))
		//            return false;
		//    }
		//    return true;
		//}

	}

	#endregion


	#region Commands for Designs and Styles

	/// <ToBeCompleted></ToBeCompleted>
	public class CreateDesignCommand : DesignCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public CreateDesignCommand(Design design)
			: this(null, design) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public CreateDesignCommand(IRepository repository, Design design)
			: base(repository, design) {
			description = string.Format("Create {0} '{1}'", design.GetType().Name, design.Name);
			this.design = design;
		}


		/// <override></override>
		public override void Execute() {
			if (Repository != null) {
				if (CanUndeleteEntity(design))
					Repository.UndeleteAll(design);
				else Repository.InsertAll(design);
			}
		}


		/// <override></override>
		public override void Revert() {
			if (Repository != null) Repository.DeleteAll(design);
		}

	}


	/// <ToBeCompleted></ToBeCompleted>
	public class DeleteDesignCommand : DesignCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public DeleteDesignCommand(Design design)
			: this(null, design) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DeleteDesignCommand(IRepository repository, Design design)
			: base(repository, design) {
			description = string.Format("Delete {0} '{1}'", design.GetType().Name, design.Name);
			this.design = design;
		}


		/// <override></override>
		public override void Execute() {
			if (Repository != null) Repository.DeleteAll(design);
		}


		/// <override></override>
		public override void Revert() {
			if (Repository != null) {
				if (CanUndeleteEntity(design))
					Repository.UndeleteAll(design);
				else Repository.InsertAll(design);
			}
		}

	}


	/// <ToBeCompleted></ToBeCompleted>
	public class CreateStyleCommand : DesignCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public CreateStyleCommand(Design design, Style style)
			: this(null, design, style) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public CreateStyleCommand(IRepository repository, Design design, Style style)
			: base(repository, design) {
			if (style == null) throw new ArgumentNullException("style");
			description = string.Format("Create {0} '{1}'", style.GetType().Name, style.Title);
			this.design = design;
			this.style = style;
		}


		/// <override></override>
		public override void Execute() {
			Design d = design ?? Repository.GetDesign(null);
			d.AddStyle(style);
			if (Repository != null) {
				if (CanUndeleteEntity(style))
					Repository.Undelete(d, style);
				if (CanUndeleteEntity(style))
					Repository.Undelete(d, style);
				else Repository.Insert(d, style);
				Repository.Update(d);
			}
		}


		/// <override></override>
		public override void Revert() {
			Design d = design ?? Repository.GetDesign(null);
			d.RemoveStyle(style);
			if (Repository != null) {
				Repository.Delete(style);
				Repository.Update(d);
			}
		}


		private Style style;
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class DeleteStyleCommand : DesignCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public DeleteStyleCommand(Design design, Style style)
			: this(null, design, style) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public DeleteStyleCommand(IRepository repository, Design design, Style style)
			: base(repository, design) {
			if (style == null) throw new ArgumentNullException("style");
			description = string.Format("Delete {0} '{1}'", style.GetType().Name, style.Title);
			this.design = design;
			this.style = style;
		}


		/// <override></override>
		public override void Execute() {
			Design d = design ?? Repository.GetDesign(null);
			d.RemoveStyle(style);
			if (Repository != null) {
				Repository.Delete(style);
				Repository.Update(d);
			}
		}


		/// <override></override>
		public override void Revert() {
			Design d = design ?? Repository.GetDesign(null);
			d.AddStyle(style);
			if (Repository != null) {
				if (CanUndeleteEntity(style))
					Repository.Undelete(d, style);
				else Repository.Insert(d, style);
				Repository.Update(d);
			}
		}


		private Style style;
	}

	#endregion


	#region Commands for Diagrams and Layers

	/// <ToBeCompleted></ToBeCompleted>
	[Obsolete("Use CreateDiagramCommand instead")]
	public class InsertDiagramCommand : CreateDiagramCommand {
		/// <ToBeCompleted></ToBeCompleted>
		public InsertDiagramCommand(Diagram diagram)
			: base(diagram) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public InsertDiagramCommand(IRepository repository, Diagram diagram)
			: base(repository, diagram) {
		}

	}


	/// <ToBeCompleted></ToBeCompleted>
	public class CreateDiagramCommand : DiagramCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public CreateDiagramCommand(Diagram diagram)
			: this(null, diagram) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public CreateDiagramCommand(IRepository repository, Diagram diagram)
			: base(repository, diagram) {
			description = string.Format("Create diagram '{0}'.", diagram.Title);
		}


		/// <override></override>
		public override void Execute() {
			if (Repository != null) {
				if (CanUndeleteEntity(diagram))
					Repository.UndeleteAll(diagram);
				else Repository.InsertAll(diagram);
			}
		}


		/// <override></override>
		public override void Revert() {
			if (Repository != null) Repository.DeleteAll(diagram);
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Insert; }
		}

	}


	/// <ToBeCompleted></ToBeCompleted>
	public class DeleteDiagramCommand : DiagramCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public DeleteDiagramCommand(Diagram diagram)
			: this(null, diagram) {
		}

		/// <ToBeCompleted></ToBeCompleted>
		public DeleteDiagramCommand(IRepository repository, Diagram diagram)
			: base(repository, diagram) {
			this.shapes = new ShapeCollection(diagram.Shapes);
		}


		/// <override></override>
		public override void Execute() {
			if (Repository != null) Repository.DeleteAll(diagram);
		}


		/// <override></override>
		public override void Revert() {
			if (Repository != null) {
				if (diagram.Shapes.Count == 0)
					diagram.Shapes.AddRange(shapes);
				if (CanUndeleteEntity(diagram))
					Repository.UndeleteAll(diagram);
				else Repository.InsertAll(diagram);
			}
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Delete; }
		}


		private ShapeCollection shapes;
	}


	/// <ToBeCompleted></ToBeCompleted>
	public abstract class InsertOrRemoveLayerCommand : DiagramCommand {

		/// <ToBeCompleted></ToBeCompleted>
		protected InsertOrRemoveLayerCommand(Diagram diagram, string layerName)
			: this(null, diagram, layerName) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected InsertOrRemoveLayerCommand(IRepository repository, Diagram diagram, string layerName)
			: base(repository, diagram) {
			if (layerName == null) throw new ArgumentNullException("layerName");
			//this.layerName = layerName;
			layers = new List<Layer>(1);
			Layer l = this.diagram.Layers.FindLayer(layerName);
			if (l == null) l = new Layer(layerName);
			layers.Add(l);
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected InsertOrRemoveLayerCommand(Diagram diagram, Layer layer)
			: this(null, diagram, layer) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected InsertOrRemoveLayerCommand(IRepository repository, Diagram diagram, Layer layer)
			: this(repository, diagram, SingleInstanceEnumerator<Layer>.Create(layer)) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected InsertOrRemoveLayerCommand(Diagram diagram, IEnumerable<Layer> layers)
			: this(null, diagram, layers) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected InsertOrRemoveLayerCommand(IRepository repository, Diagram diagram, IEnumerable<Layer> layers)
			: base(repository, diagram) {
			this.layers = new List<Layer>(layers);
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Layout; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void AddLayers() {
			for (int i = 0; i < layers.Count; ++i)
				diagram.Layers.Add(layers[i]);
			if (Repository != null) Repository.Update(diagram);
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void RemoveLayers() {
			for (int i = 0; i < layers.Count; ++i)
				diagram.Layers.Remove(layers[i]);
			if (Repository != null) Repository.Update(diagram);
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected List<Layer> layers = null;
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class AddLayerCommand : InsertOrRemoveLayerCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public AddLayerCommand(Diagram diagram, string layerName)
			: this(null, diagram, layerName) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public AddLayerCommand(IRepository repository, Diagram diagram, string layerName)
			: base(repository, diagram, layerName) {
			this.description = string.Format("Add layer '{0}'", layerName);
		}


		/// <override></override>
		public override void Execute() {
			AddLayers();
		}

		/// <override></override>
		public override void Revert() {
			RemoveLayers();
		}
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class RemoveLayerCommand : InsertOrRemoveLayerCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public RemoveLayerCommand(Diagram diagram, Layer layer)
			: this(null, diagram, layer) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public RemoveLayerCommand(IRepository repository, Diagram diagram, Layer layer)
			: base(repository, diagram, layer) {
			Construct();
			this.description = string.Format("Remove layer '{0}' from diagram '{1}'", layer.Title, diagram.Title);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public RemoveLayerCommand(Diagram diagram, IEnumerable<Layer> layers)
			: this(null, diagram, layers) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public RemoveLayerCommand(IRepository repository, Diagram diagram, IEnumerable<Layer> layers)
			: base(repository, diagram, layers) {
			Construct();
			this.description = string.Format("Remove {0} layers from diagram '{1}'", this.layers.Count, diagram.Title);
		}


		/// <override></override>
		public override void Execute() {
			diagram.RemoveShapesFromLayers(affectedShapes, layerIds);
			RemoveLayers();
		}


		/// <override></override>
		public override void Revert() {
			AddLayers();
			RestoreShapeLayers();
		}


		private void Construct() {
			layerIds = LayerIds.None;
			for (int i = 0; i < layers.Count; ++i)
				layerIds |= layers[i].Id;

			affectedShapes = new List<Shape>();
			originalLayers = new List<LayerIds>();
			foreach (Shape shape in diagram.Shapes) {
				if ((shape.Layers & layerIds) != 0) {
					affectedShapes.Add(shape);
					originalLayers.Add(shape.Layers);
				}
			}
		}


		private void RestoreShapeLayers() {
			int cnt = affectedShapes.Count;
			for (int i = 0; i < cnt; ++i)
				diagram.AddShapeToLayers(affectedShapes[i], originalLayers[i]);
			if (Repository != null) Repository.Update(diagram);
		}


		private struct OriginalShapeLayer : IEquatable<OriginalShapeLayer> {
			static OriginalShapeLayer() {
				Empty.layerIds = LayerIds.None;
				Empty.shape = null;
			}
			public static readonly OriginalShapeLayer Empty;
			public Shape shape;
			public LayerIds layerIds;
			public bool Equals(OriginalShapeLayer other) {
				return (other.layerIds == this.layerIds
					&& other.shape == this.shape);
			}
		}

		private LayerIds layerIds;
		private List<Shape> affectedShapes;
		private List<LayerIds> originalLayers;
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class EditLayerCommand : DiagramCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public EditLayerCommand(Diagram diagram, Layer layer, ChangedProperty property, string oldValue, string newValue)
			: this(null, diagram, layer, property, oldValue, newValue) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public EditLayerCommand(IRepository repository, Diagram diagram, Layer layer, ChangedProperty property, string oldValue, string newValue)
			: base(repository, diagram) {
			if (newValue == null) throw new ArgumentNullException("newValue");
			if (property == ChangedProperty.LowerZoomThreshold || property == ChangedProperty.UpperZoomThreshold)
				throw new ArgumentException("property");
			this.oldStrValue = oldValue;
			this.newStrValue = newValue;
			Construct(layer, property);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public EditLayerCommand(Diagram diagram, Layer layer, ChangedProperty property, int oldValue, int newValue)
			: this(null, diagram, layer, property, oldValue, newValue) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public EditLayerCommand(IRepository repository, Diagram diagram, Layer layer, ChangedProperty property, int oldValue, int newValue)
			: base(repository, diagram) {
			if (property == ChangedProperty.Name || property == ChangedProperty.Title)
				throw new ArgumentException("property");
			this.oldIntValue = oldValue;
			this.newIntValue = newValue;
			Construct(layer, property);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public enum ChangedProperty {
			/// <ToBeCompleted></ToBeCompleted>
			Name,
			/// <ToBeCompleted></ToBeCompleted>
			Title,
			/// <ToBeCompleted></ToBeCompleted>
			LowerZoomThreshold,
			/// <ToBeCompleted></ToBeCompleted>
			UpperZoomThreshold
		}


		/// <ToBeCompleted></ToBeCompleted>
		public override void Execute() {
			switch (changedProperty) {
				case ChangedProperty.Name:
					layer.Name = newStrValue;
					break;
				case ChangedProperty.Title:
					layer.Title = newStrValue;
					break;
				case ChangedProperty.LowerZoomThreshold:
					layer.LowerZoomThreshold = newIntValue;
					break;
				case ChangedProperty.UpperZoomThreshold:
					layer.UpperZoomThreshold = newIntValue;
					break;
				default:
					Debug.Fail("Unhandled switch case!");
					break;
			}
			if (Repository != null) Repository.Update(diagram);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public override void Revert() {
			switch (changedProperty) {
				case ChangedProperty.Name:
					layer.Name = oldStrValue;
					break;
				case ChangedProperty.Title:
					layer.Title = oldStrValue;
					break;
				case ChangedProperty.LowerZoomThreshold:
					layer.LowerZoomThreshold = oldIntValue;
					break;
				case ChangedProperty.UpperZoomThreshold:
					layer.UpperZoomThreshold = oldIntValue;
					break;
				default:
					Debug.Fail("Unhandled switch case!");
					break;
			}
			if (Repository != null) Repository.Update(diagram);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public override Permission RequiredPermission {
			get { return Permission.Layout; }
		}


		private void Construct(Layer layer, ChangedProperty property) {
			this.layer = layer;
			this.changedProperty = property;
			string quotes, oldVal, newVal;
			if (property == ChangedProperty.Name || property == ChangedProperty.Title) {
				quotes = "'";
				oldVal = oldStrValue;
				newVal = newStrValue;
			} else {
				quotes = "";
				oldVal = oldIntValue.ToString();
				newVal = newIntValue.ToString();
			}
			description = string.Format("Change layer property '{0}' from {1}{2}{1} to {1}{3}{1}", property, quotes, oldVal, newVal);
		}


		private Layer layer = null;
		private ChangedProperty changedProperty;
		private string oldStrValue, newStrValue;
		private int oldIntValue, newIntValue;
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class AddShapesToLayersCommand : DiagramCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public AddShapesToLayersCommand(Diagram diagram, Shape shape, LayerIds layerIds)
			: this(null, diagram, SingleInstanceEnumerator<Shape>.Create(shape), layerIds) {
		}

		/// <ToBeCompleted></ToBeCompleted>
		public AddShapesToLayersCommand(IRepository repository, Diagram diagram, Shape shape, LayerIds layerIds)
			: this(repository, diagram, SingleInstanceEnumerator<Shape>.Create(shape), layerIds) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public AddShapesToLayersCommand(Diagram diagram, IEnumerable<Shape> shapes, LayerIds layerIds)
			: this(null, diagram, shapes, layerIds) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public AddShapesToLayersCommand(IRepository repository, Diagram diagram, IEnumerable<Shape> shapes, LayerIds layerIds)
			: base(repository, diagram) {
			if (shapes == null) throw new ArgumentNullException("shapes");
			this.newLayerIds = layerIds;
			this.shapes = new List<Shape>(shapes);
		}


		/// <override></override>
		public override void Execute() {
			diagram.AddShapesToLayers(shapes, newLayerIds);
			if (Repository != null) Repository.Update(shapes);
		}


		/// <override></override>
		public override void Revert() {
			diagram.RemoveShapesFromLayers(shapes, newLayerIds);
			if (Repository != null) Repository.Update(shapes);
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Layout; }
		}


		private LayerIds newLayerIds;
		private List<Shape> shapes;
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class AssignShapesToLayersCommand : ShapesCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public AssignShapesToLayersCommand(Diagram diagram, Shape shape, LayerIds layerIds)
			: this(null, diagram, shape, layerIds) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public AssignShapesToLayersCommand(IRepository repository, Diagram diagram, Shape shape, LayerIds layerIds)
			: this(repository, diagram, SingleInstanceEnumerator<Shape>.Create(shape), layerIds) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public AssignShapesToLayersCommand(Diagram diagram, IEnumerable<Shape> shapes, LayerIds layerIds)
			: this(null, diagram, shapes, layerIds) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public AssignShapesToLayersCommand(IRepository repository, Diagram diagram, IEnumerable<Shape> shapes, LayerIds layerIds)
			: base(repository, shapes) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			this.diagram = diagram;
			this.newLayerIds = layerIds;
			this.oldLayerIds = new List<LayerIds>(this.shapes.Count);
			int cnt = this.shapes.Count;
			foreach (Shape shape in shapes)
				oldLayerIds.Add(shape.Layers);
		}


		/// <override></override>
		public override void Execute() {
			if (shapes.Count > 0) {
				foreach (Shape shape in shapes) {
					diagram.RemoveShapeFromLayers(shape, LayerIds.All);
					diagram.AddShapeToLayers(shape, newLayerIds);
				}
				if (Repository != null) Repository.Update(shapes);
			}
		}


		/// <override></override>
		public override void Revert() {
			if (shapes.Count > 0) {
				int i = -1;
				foreach (Shape shape in shapes) {
					diagram.RemoveShapeFromLayers(shape, LayerIds.All);
					diagram.AddShapeToLayers(shape, oldLayerIds[++i]);
				}
				if (Repository != null) Repository.Update(shapes);
			}
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Layout; }
		}


		private Diagram diagram;
		private LayerIds newLayerIds;
		private List<LayerIds> oldLayerIds;
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class RemoveShapesFromLayersCommand : ShapesCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public RemoveShapesFromLayersCommand(Diagram diagram, Shape shape)
			: this(null, diagram, SingleInstanceEnumerator<Shape>.Create(shape)) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public RemoveShapesFromLayersCommand(IRepository repository, Diagram diagram, Shape shape)
			: this(repository, diagram, SingleInstanceEnumerator<Shape>.Create(shape)) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public RemoveShapesFromLayersCommand(Diagram diagram, IEnumerable<Shape> shapes)
			: this(null, diagram, shapes, LayerIds.All) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public RemoveShapesFromLayersCommand(IRepository repository, Diagram diagram, IEnumerable<Shape> shapes)
			: this(repository, diagram, shapes, LayerIds.All) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public RemoveShapesFromLayersCommand(Diagram diagram, Shape shape, LayerIds removeFromLayerIds)
			: this(null, diagram, shape, removeFromLayerIds) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public RemoveShapesFromLayersCommand(IRepository repository, Diagram diagram, Shape shape, LayerIds removeFromLayerIds)
			: this(repository, diagram, SingleInstanceEnumerator<Shape>.Create(shape), removeFromLayerIds) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public RemoveShapesFromLayersCommand(Diagram diagram, IEnumerable<Shape> shapes, LayerIds removeFromLayerIds)
			: this(null, diagram, shapes, removeFromLayerIds) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public RemoveShapesFromLayersCommand(IRepository repository, Diagram diagram, IEnumerable<Shape> shapes, LayerIds removeFromLayerIds)
			: base(repository, shapes) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			this.diagram = diagram;
			this.removeLayerIds = removeFromLayerIds;
			this.originalLayerIds = new List<LayerIds>(this.shapes.Count);
			int cnt = this.shapes.Count;
			foreach (Shape shape in shapes)
				originalLayerIds.Add(shape.Layers);
		}


		/// <override></override>
		public override void Execute() {
			if (shapes.Count > 0) {
				foreach (Shape shape in shapes)
					diagram.RemoveShapeFromLayers(shape, removeLayerIds);
				if (Repository != null) Repository.Update(shapes);
			}
		}


		/// <override></override>
		public override void Revert() {
			if (shapes.Count > 0) {
				int i = -1;
				foreach (Shape shape in shapes)
					diagram.AddShapeToLayers(shape, originalLayerIds[++i]);
				if (Repository != null) Repository.Update(shapes);
			}
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.Layout; }
		}


		private Diagram diagram;
		private LayerIds removeLayerIds;
		private List<LayerIds> originalLayerIds;
	}

	#endregion


	#region Commands for aggregating/grouping shapes

	/// <summary>
	/// Create clones of the given shapes and insert them into diagram and cache
	/// </summary>
	public class GroupShapesCommand : ShapeAggregationCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public GroupShapesCommand(Diagram diagram, LayerIds layerIds, Shape shapeGroup, IEnumerable<Shape> childShapes)
			: this(null, diagram, layerIds, shapeGroup, childShapes) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public GroupShapesCommand(IRepository repository, Diagram diagram, LayerIds layerIds, Shape shapeGroup, IEnumerable<Shape> childShapes)
			: base(repository, diagram, shapeGroup, childShapes) {
			if (!(aggregationShape is IShapeGroup)) throw new ArgumentException("Shape must be a shape group.");
			this.description = string.Format("Aggregate {0} shapes to a '{1}'", base.shapes.Count, ((Shape)base.aggregationShape).Type.Name);

			// Calculate boundingRectangle of the children and
			// move aggregationShape to children's center
			if (aggregationShape.X == 0 && aggregationShape.Y == 0) {
				Rectangle r = Rectangle.Empty;
				foreach (Shape shape in childShapes) {
					if (r.IsEmpty) r = shape.GetBoundingRectangle(true);
					else r = Geometry.UniteRectangles(r, shape.GetBoundingRectangle(true));
				}
				aggregationShape.MoveTo(r.X + (r.Width / 2), r.Y + (r.Height / 2));
			}
			aggregationShapeOwnedByDiagram = false;
		}


		/// <override></override>
		public override void Execute() {
			// insert aggregationShape into diagram (and Cache)
			if (Repository != null) aggregationShape.ZOrder = Repository.ObtainNewTopZOrder(diagram);
			CreateShapeAggregation(false);
		}


		/// <override></override>
		public override void Revert() {
			DeleteShapeAggregation();
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.None; }
		}
	}


	/// <summary>
	/// Create clones of the given shapes and insert them into diagram and cache
	/// </summary>
	public class UngroupShapesCommand : ShapeAggregationCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public UngroupShapesCommand(Diagram diagram, Shape shapeGroup)
			: this(null, diagram, shapeGroup) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public UngroupShapesCommand(IRepository repository, Diagram diagram, Shape shapeGroup)
			: base(repository, diagram, shapeGroup, (shapeGroup != null) ? shapeGroup.Children : null) {
			if (!(shapeGroup is IShapeGroup)) throw new ArgumentException("Shape must support IShapeGroup.");
			this.description = string.Format("Release {0} shapes from {1}'s aggregation", base.shapes.Count, base.aggregationShape.Type.Name);
			aggregationShapeOwnedByDiagram = false;
		}


		/// <override></override>
		public override void Execute() {
			DeleteShapeAggregation();
		}


		/// <override></override>
		public override void Revert() {
			CreateShapeAggregation(false);
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get { return Permission.None; }
		}

	}


	/// <ToBeCompleted></ToBeCompleted>
	public class AggregateCompositeShapeCommand : ShapeAggregationCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public AggregateCompositeShapeCommand(Diagram diagram, LayerIds layerIds, Shape parentShape, IEnumerable<Shape> childShapes)
			: this(null, diagram, layerIds, parentShape, childShapes) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public AggregateCompositeShapeCommand(IRepository repository, Diagram diagram, LayerIds layerIds, Shape parentShape, IEnumerable<Shape> childShapes)
			: base(diagram, parentShape, childShapes) {
			this.description = string.Format("Aggregate {0} shapes to a composite shape", base.shapes.Count);
		}


		/// <override></override>
		public override void Execute() {
			CreateShapeAggregation(true);
		}


		/// <override></override>
		public override void Revert() {
			DeleteShapeAggregation();
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get {
				// As the aggregated shape changes its visual appearance when (un)aggregating child 
				// shapes, we have to check presentation permission
				return Permission.Present;
			}
		}
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class SplitCompositeShapeCommand : ShapeAggregationCommand {

		/// <ToBeCompleted></ToBeCompleted>
		public SplitCompositeShapeCommand(Diagram diagram, LayerIds layerIds, Shape parentShape)
			: this(null, diagram, layerIds, parentShape) {
		}


		/// <ToBeCompleted></ToBeCompleted>
		public SplitCompositeShapeCommand(IRepository repository, Diagram diagram, LayerIds layerIds, Shape parentShape)
			: base(repository, diagram, parentShape, (parentShape != null) ? parentShape.Children : null) {
			this.description = string.Format("Split composite shape into {0} single shapes", base.shapes.Count);
		}


		/// <override></override>
		public override void Execute() {
			DeleteShapeAggregation();
		}


		/// <override></override>
		public override void Revert() {
			CreateShapeAggregation(true);
		}


		/// <override></override>
		public override Permission RequiredPermission {
			get {
				// As the aggregated shape changes its visual appearance when (un)aggregating child 
				// shapes, we have to check presentation permission
				return Permission.Present;
			}
		}
	}

	#endregion

}