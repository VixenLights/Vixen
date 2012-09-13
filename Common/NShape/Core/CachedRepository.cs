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
using System.IO;
using System.Text;
using System.Runtime.Serialization;


namespace Dataweb.NShape.Advanced {

	/// <summary>
	/// Caches modifications to the cache and commits them to the data store
	/// during SaveChanges.
	/// </summary>
	[ToolboxItem(true)]
	[ToolboxBitmap(typeof(CachedRepository), "CachedRepository.bmp")]
	public class CachedRepository : Component, IRepository, IStoreCache {

		/// <summary>
		/// The <see cref="T:Dataweb.NShape.Advanced.Store" /> containing the persistent data for this <see cref="T:Dataweb.NShape.Advanced.CachedRepository" />
		/// </summary>
		public Store Store {
			get { return store; }
			set {
				AssertClosed();
				if (store != null) store.ProjectName = string.Empty;
				store = value;
				if (store != null) store.ProjectName = projectName;
			}
		}


		/// <summary>
		/// Specifies the version of the assembly containing the component.
		/// </summary>
		[Category("NShape")]
		public string ProductVersion {
			get { return this.GetType().Assembly.GetName().Version.ToString(); }
		}


		/// <summary>
		/// Returns the XML representation of the data stored in the cache.
		/// </summary>
		/// <returns></returns>
		public string GetXml() {
			throw new NotImplementedException();
		}


		/// <summary>
		/// Reads XML data into the cache using the specified System.IO.Stream.
		/// </summary>
		/// <param name="stream"></param>
		public void ReadXml(Stream stream) {
			if (stream == null) throw new ArgumentNullException("stream");
			throw new NotImplementedException();
		}


		/// <summary>
		/// Writes the current cache content as XML data.
		/// </summary>
		/// <param name="stream"></param>
		public void WriteXml(Stream stream) {
			if (stream == null) throw new ArgumentNullException("stream");
			throw new NotImplementedException();
		}


#if DEBUG_DIAGNOSTICS
		/// <summary>
		/// Finds the owner of the given shape. For debugging purposes only!
		/// </summary>
		public IEntity FindOwner(Shape shape) {
			foreach (Diagram d in GetCachedEntities<Diagram>(diagrams, newDiagrams))
				if (d.Shapes.Contains(shape)) return d;
			foreach (Template t in GetCachedEntities<Template>(templates, newTemplates))
				if (t.Shape == shape) return t;
			foreach (Shape s in GetCachedEntities<Shape>(shapes, newShapes))
				if (s.Children.Contains(shape)) return s;
			return null;
		}
#endif


		#region IRepository Members


		/// <override></override>
		public int Version {
			get { return version; }
			set {
				// ToDo: Check on first/last supported save/load version
				version = value;
				if (store != null) store.Version = version;
			}
		}


		/// <override></override>
		public string ProjectName {
			get { return projectName; }
			set {
				projectName = value;
				if (store != null) store.ProjectName = projectName;
			}
		}


		/// <override></override>
		public bool IsModified {
			get { return isModified; }
		}


		/// <override></override>
		public void AddEntityType(IEntityType entityType) {
			if (entityType == null) throw new ArgumentNullException("entityType");
			if (entityTypes.ContainsKey(CalcElementName(entityType.FullName)))
				throw new NShapeException("The repository already contains an entity type called '{0}'.", entityType.FullName);
			foreach (KeyValuePair<string, IEntityType> item in entityTypes) {
				if (item.Value.FullName.Equals(entityType.FullName, StringComparison.InvariantCultureIgnoreCase))
					throw new NShapeException("The repository already contains an entity type called '{0}'.", entityType.FullName);
			}
			// Calculate the XML element names for all entity identifiers
			entityType.ElementName = CalcElementName(entityType.FullName);
			foreach (EntityPropertyDefinition pi in entityType.PropertyDefinitions) {
				pi.ElementName = CalcElementName(pi.Name);
				if (pi is EntityInnerObjectsDefinition) {
					foreach (EntityPropertyDefinition fi in ((EntityInnerObjectsDefinition)pi).PropertyDefinitions)
						fi.ElementName = CalcElementName(fi.Name);
				}
			}
			entityTypes.Add(entityType.ElementName, entityType);
		}


		/// <override></override>
		public void RemoveEntityType(string entityTypeName) {
			if (entityTypeName == null) throw new ArgumentNullException("entityTypeName");
			if (entityTypeName == string.Empty) throw new ArgumentException("Invalid entity type name.");
			entityTypes.Remove(CalcElementName(entityTypeName));
		}


		/// <override></override>
		public void RemoveAllEntityTypes() {
			entityTypes.Clear();
		}


		/// <override></override>
		public void ReadVersion() {
			if (store == null) throw new Exception("Property Store is not set.");
			else if (!store.Exists()) throw new Exception("Store does not exist.");
			else store.ReadVersion(this);
		}


		/// <override></override>
		public bool Exists() {
			return store != null && store.Exists();
		}


		/// <override></override>
		public virtual void Create() {
			AssertClosed();
			if (string.IsNullOrEmpty(projectName)) throw new NShapeException("No project name defined.");
			settings = new ProjectSettings();
			newProjects.Add(settings, projectOwner);
			projectDesign = new Design();
			projectDesign.CreateStandardStyles();
			DoInsertDesign(projectDesign, settings, true);
			if (store != null) {
				store.Version = version;
				store.Create(this);
			}
			isOpen = true;
		}


		/// <override></override>
		public void Open() {
			AssertClosed();
			if (string.IsNullOrEmpty(projectName)) throw new NShapeException("No project name defined.");
			if (store == null)
				throw new InvalidOperationException("Repository has no store attached. An in-memory repository must be created, not opened.");
			store.ProjectName = projectName;
			store.Open(this);
			// Load the project, must be exactly one.
			store.LoadProjects(this, FindEntityType(ProjectSettings.EntityTypeName, true));
			IEnumerator<EntityBucket<ProjectSettings>> projectEnumerator = loadedProjects.Values.GetEnumerator();
			if (!projectEnumerator.MoveNext())
				throw new NShapeException("Project '{0}' not found in repository.", projectName);
			settings = projectEnumerator.Current.ObjectRef;
			if (projectEnumerator.MoveNext())
				throw new NShapeException("Two projects named '{0}' found in repository.", projectName);
			// Load the design, there must be exactly one returned
			store.LoadDesigns(this, ((IEntity)settings).Id);
			IEnumerator<EntityBucket<Design>> designEnumerator = loadedDesigns.Values.GetEnumerator();
			if (!designEnumerator.MoveNext())
				throw new NShapeException("Project styles not found.");
			projectDesign = designEnumerator.Current.ObjectRef;
			if (designEnumerator.MoveNext()) {
				//throw new NShapeException("More than one project design found in repository.");
				// ToDo: Load addinional designs
			}
			isOpen = true;
			isModified = false;
		}


		/// <override></override>
		public virtual void Close() {
			if (store != null) store.Close(this);
			isOpen = false;
			ClearBuffers();
			isModified = false;
		}


		/// <override></override>
		public void Erase() {
			if (store == null) throw new InvalidOperationException("Repository has no store attached.");
			store.Erase();
		}


		/// <override></override>
		public void SaveChanges() {
			if (store == null) throw new Exception("Repository has no store attached.");
			store.SaveChanges(this);
			AcceptAll();
		}


		/// <override></override>
		[Browsable(false)]
		public bool IsOpen {
			get { return isOpen; }
		}


		/// <override></override>
		public int ObtainNewBottomZOrder(Diagram diagram) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			return diagram.Shapes.MinZOrder - 10;
		}


		/// <override></override>
		public int ObtainNewTopZOrder(Diagram diagram) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			return diagram.Shapes.MaxZOrder + 10;
		}


		/// <override></override>
		public bool IsShapeTypeInUse(ShapeType shapeType) {
			return IsShapeTypeInUse(SingleInstanceEnumerator<ShapeType>.Create(shapeType));
		}


		/// <override></override>
		public bool IsShapeTypeInUse(IEnumerable<ShapeType> shapeTypes) {
			if (shapeTypes == null) throw new ArgumentNullException("shapeTypes");
			AssertOpen();
			// Store shape types in a hash list for better performance when searching huge amounts of shapes
			Dictionary<ShapeType, object> shapeTypeDict = new Dictionary<ShapeType, object>();
			foreach (ShapeType shapeType in shapeTypes)
				shapeTypeDict.Add(shapeType, null);
			if (shapeTypeDict.Count == 0) return false;

			// Check all new shapes if one of the shape types is used
			foreach (KeyValuePair<Shape, IEntity> keyValuePair in newShapes) {
				// Skip template shapes as they are checked by the Project's RemoveLibrary method.
				if (keyValuePair.Value is Template) continue;
				if (shapeTypeDict.ContainsKey(keyValuePair.Key.Type))
					return true;
			}
			// Check all loaded shapes if one of the shape types is used
			foreach (EntityBucket<Shape> entityBucket in loadedShapes) {
				// Skip deleted shapes and template shapes as they are checked by the Project's RemoveLibrary method.
				if (entityBucket.State == ItemState.Deleted) continue;
				if (entityBucket.ObjectRef.Template == null) continue;
				if (shapeTypeDict.ContainsKey(entityBucket.ObjectRef.Type))
					return true;
			}
			// Check all diagrams that are not loaded at the moment
			foreach (KeyValuePair<ShapeType, object> item in shapeTypeDict) {
				if (Store.CheckShapeTypeInUse(this, item.Key.FullName))
					return true;
			}
			return false;
		}


		/// <override></override>
		public bool IsModelObjectTypeInUse(ModelObjectType modelObjectType) {
			return IsModelObjectTypeInUse(SingleInstanceEnumerator<ModelObjectType>.Create(modelObjectType));
		}


		/// <override></override>
		public bool IsModelObjectTypeInUse(IEnumerable<ModelObjectType> modelObjectTypes) {
			if (modelObjectTypes == null) throw new ArgumentNullException("modelObjectTypes");
			AssertOpen();
			// Store shape types in a hash list for better performance when searching huge amounts of shapes
			Dictionary<ModelObjectType, object> modelObjTypeDict = new Dictionary<ModelObjectType, object>();
			foreach (ModelObjectType modelObjectType in modelObjectTypes)
				modelObjTypeDict.Add(modelObjectType, null);
			if (modelObjTypeDict.Count == 0) return false;

			// Check all new model objects if one of the model object types is used
			foreach (KeyValuePair<IModelObject, IEntity> keyValuePair in newModelObjects) {
				if (modelObjTypeDict.ContainsKey(keyValuePair.Key.Type))
					return true;
			}
			// Check all loaded model objects if one of the model object types is used
			foreach (EntityBucket<IModelObject> entityBucket in loadedModelObjects) {
				if (entityBucket.State == ItemState.Deleted) continue;
				if (modelObjTypeDict.ContainsKey(entityBucket.ObjectRef.Type))
					return true;
			}
			// A check on the database is not necessary here because models are always loaded completly.
			return false;
		}


		#region Project

		/// <override></override>
		public ProjectSettings GetProject() {
			AssertOpen();
			return this.settings;
		}


		/// <override></override>
		public void Update() {
			AssertOpen();
			UpdateEntity<ProjectSettings>(loadedProjects, newProjects, settings);
			if (ProjectUpdated != null) ProjectUpdated(this, GetProjectEventArgs(settings));
		}


		/// <override></override>
		public void Delete() {
			AssertOpen();
			DeleteEntity<ProjectSettings>(loadedProjects, newProjects, settings);
			if (ProjectDeleted != null) ProjectDeleted(this, GetProjectEventArgs(settings));
		}


		/// <override></override>
		public event EventHandler<RepositoryProjectEventArgs> ProjectUpdated;

		/// <override></override>
		public event EventHandler<RepositoryProjectEventArgs> ProjectDeleted;

		#endregion


		#region Designs

		/// <override></override>
		public IEnumerable<Design> GetDesigns() {
			AssertOpen();
			if (store != null && ((IEntity)settings).Id != null && loadedDesigns.Count <= 0)
				store.LoadDesigns(this, null);
			return GetCachedEntities(loadedDesigns, newDesigns);
		}


		/// <override></override>
		public Design GetDesign(object id) {
			Design result = null;
			AssertOpen();
			if (id == null) {
				// Return the project design
				result = projectDesign;
			} else {
				EntityBucket<Design> designBucket;
				if (store != null && loadedDesigns.Count <= 0)
					store.LoadDesigns(this, null);
				if (!loadedDesigns.TryGetValue(id, out designBucket))
					throw new NShapeException("Design with id '{0}' not found in repository.", id);
				if (designBucket.State == ItemState.Deleted) throw new NShapeException("Design '{0}' was deleted.", designBucket.ObjectRef);
				result = designBucket.ObjectRef;
			}
			return result;
		}


		/// <override></override>
		public Design GetDesign(string name) {
			AssertOpen();
			if (string.IsNullOrEmpty(name)) 
				return projectDesign;
			else {
				if (store != null && loadedDesigns.Count <= 0)
					store.LoadDesigns(this, null);
				foreach (Design d in GetCachedEntities<Design>(loadedDesigns, newDesigns))
					if (d.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
						return d;
				throw new ArgumentException(string.Format("A design named '{0}' does not exist in the repository.", name));
			}
		}


		/// <override></override>
		public void Insert(Design design) {
			if (design == null) throw new ArgumentNullException("design");
			AssertOpen();
			DoInsertDesign(design, settings, false);
			if (DesignInserted != null) DesignInserted(this, GetDesignEventArgs(design));
		}


		/// <override></override>
		public void InsertAll(Design design) {
			if (design == null) throw new ArgumentNullException("design");
			AssertOpen();
			DoInsertDesign(design, settings, true);
			if (DesignInserted != null) DesignInserted(this, GetDesignEventArgs(design));
		}


		/// <override></override>
		public void Update(Design design) {
			if (design == null) throw new ArgumentNullException("design");
			AssertOpen();
			AssertCanUpdate(design);
			UpdateEntity<Design>(loadedDesigns, newDesigns, design);
			if (DesignUpdated != null) DesignUpdated(this, GetDesignEventArgs(design));
		}


		/// <override></override>
		public void Delete(Design design) {
			if (design == null) throw new ArgumentNullException("design");
			if (design == projectDesign) throw new InvalidOperationException("Current project design cannot be deleted.");
			AssertOpen();
			DoDeleteDesign(design, false);
			if (DesignDeleted != null) DesignDeleted(this, GetDesignEventArgs(design));
		}


		/// <override></override>
		public void DeleteAll(Design design) {
			if (design == null) throw new ArgumentNullException("design");
			if (design == projectDesign) throw new InvalidOperationException("Current project design cannot be deleted.");
			AssertOpen();
			DoDeleteDesign(design, true);
			if (DesignDeleted != null) DesignDeleted(this, GetDesignEventArgs(design));
		}


		/// <override></override>
		public void Undelete(Design design) {
			if (design == null) throw new ArgumentNullException("design");
			AssertOpen();
			DoUndeleteDesign(design, false);
			if (DesignInserted != null) DesignInserted(this, GetDesignEventArgs(design));
		}


		/// <override></override>
		public void UndeleteAll(Design design) {
			if (design == null) throw new ArgumentNullException("design");
			AssertOpen();
			DoUndeleteDesign(design, true);
			if (DesignInserted != null) DesignInserted(this, GetDesignEventArgs(design));
		}


		/// <override></override>
		public event EventHandler<RepositoryDesignEventArgs> DesignInserted;

		/// <override></override>
		public event EventHandler<RepositoryDesignEventArgs> DesignUpdated;

		/// <override></override>
		public event EventHandler<RepositoryDesignEventArgs> DesignDeleted;

		#endregion


		#region Styles

		/// <override></override>
		public bool IsStyleInUse(IStyle style) {
			return IsStyleInUse(style, EmptyEnumerator<IStyle>.Empty);
		}


		/// <override></override>
		public void Insert(Design design, IStyle style) {
			if (design == null) throw new ArgumentNullException("design");
			if (style == null) throw new ArgumentNullException("style");
			AssertOpen();
			AssertCanInsert(SingleInstanceEnumerator<IStyle>.Create(style));
			DoInsertStyle(design, style);
			if (StyleInserted != null) StyleInserted(this, GetStyleEventArgs(style));
		}


		/// <override></override>
		public void Update(IStyle style) {
			if (style == null) throw new ArgumentNullException("style");
			AssertOpen();
			AssertCanUpdate(style);
			DoUpdateStyle(style);
			if (StyleUpdated != null) StyleUpdated(this, GetStyleEventArgs(style));
		}


		/// <override></override>
		public void Delete(IStyle style) {
			if (style == null) throw new ArgumentNullException("style");
			AssertOpen();
			AssertCanDelete(SingleInstanceEnumerator<IStyle>.Create(style));
			DeleteEntity<IStyle>(loadedStyles, newStyles, style);
			if (StyleDeleted != null) StyleDeleted(this, GetStyleEventArgs(style));
		}


		/// <override></override>
		public void Undelete(Design design, IStyle style) {
			if (style == null) throw new ArgumentNullException("style");
			AssertOpen();
			AssertCanUndelete(SingleInstanceEnumerator<IStyle>.Create(style));
			DoUndeleteStyle(design, style);
			if (StyleInserted != null) StyleInserted(this, GetStyleEventArgs(style));
		}


		/// <override></override>
		public event EventHandler<RepositoryStyleEventArgs> StyleInserted;

		/// <override></override>
		public event EventHandler<RepositoryStyleEventArgs> StyleUpdated;

		/// <override></override>
		public event EventHandler<RepositoryStyleEventArgs> StyleDeleted;

		#endregion


		#region  Model

		/// <override></override>
		public Model GetModel() {
			Model model = null;
			if (!TryGetModel(out model))
				throw new NShapeException("A model does not exist in the repository.");
			return model;
		}


		/// <override></override>
		public void Insert(Model model) {
			if (model == null) throw new ArgumentNullException("model");
			AssertOpen();
			AssertCanInsert(model);
			Model m = null;
			if (TryGetModel(out m))
				throw new NShapeException("A model aleady exists. More than one model per project is not supported.");
			InsertEntity<Model>(newModels, model, GetProject());
			if (ModelInserted != null) ModelInserted(this, GetModelEventArgs(model));
		}


		/// <override></override>
		public void Update(Model model) {
			AssertOpen();
			AssertCanUpdate(model);
			UpdateEntity<Model>(loadedModels, newModels, model);
			if (ModelUpdated != null) ModelUpdated(this, GetModelEventArgs(model));
		}


		/// <override></override>
		public void Delete(Model model) {
			AssertOpen();
			AssertCanDelete(model);
			DeleteEntity<Model>(loadedModels, newModels, model);
			if (ModelDeleted != null) ModelDeleted(this, GetModelEventArgs(model));
		}


		/// <override></override>
		public void Undelete(Model model) {
			AssertOpen();
			AssertCanUndelete(model);
			UndeleteEntity<Model>(loadedModels, model);
			if (ModelInserted != null) ModelInserted(this, GetModelEventArgs(model));
		}


		/// <override></override>
		public event EventHandler<RepositoryModelEventArgs> ModelInserted;

		/// <override></override>
		public event EventHandler<RepositoryModelEventArgs> ModelUpdated;

		/// <override></override>
		public event EventHandler<RepositoryModelEventArgs> ModelDeleted;

		#endregion


		#region ModelObjects

		/// <summary>
		/// Checks whether the given model object is still referenced by any object.
		/// </summary>
		public bool IsModelObjectInUse(IModelObject modelObject) {
			return IsModelObjectInUse(modelObject, EmptyEnumerator<IModelObject>.Empty);
		}


		/// <summary>
		/// Checks whether the given model object is still referenced by any object.
		/// </summary>
		private bool IsModelObjectInUse(IModelObject modelObject, IEnumerable<IModelObject> modelObjectsToDelete) {
			if (modelObject == null) throw new ArgumentNullException("modelObject");
			// Check if there are child model objects
			foreach (IModelObject m in GetModelObjects(modelObject)) {
				if (!Contains(modelObjectsToDelete, modelObject))
					return true;
			}
			// Check if model object is used by shapes
			foreach (KeyValuePair<Shape, IEntity> item in newShapes)
				if (item.Key.ModelObject == modelObject)
					return true;
			foreach (EntityBucket<Shape> item in loadedShapes) {
				if (item.State == ItemState.Deleted) continue;
				if (item.ObjectRef.ModelObject == modelObject)
					return true;
			}
			// Check if model object is used by objects that are not loaded
			if (modelObject.Id != null)
				return store.CheckModelObjectInUse(this, modelObject.Id);
			return false;
		}


		// TODO 2: Should be similar to GetShape. Unify?
		/// <override></override>
		public IModelObject GetModelObject(object id) {
			if (id == null) throw new ArgumentNullException("id");
			AssertOpen();
			IModelObject result = null;
			EntityBucket<IModelObject> bucket;
			if (loadedModelObjects.TryGetValue(id, out bucket))
				result = bucket.ObjectRef;
			else {
				// Load ModelObjects and try again
				Model model = GetModel();
				if (store != null && model != null) store.LoadModelModelObjects(this, model.Id);
				if (!loadedModelObjects.TryGetValue(id, out bucket))
					throw new NShapeException("Model object with id '{0}' not found in repository.", id);
				if (bucket.State == ItemState.Deleted) throw new NShapeException("ModelObject with id '{0}' was deleted.", id);
				result = bucket.ObjectRef;
			}
			return result;
		}


		/// <override></override>
		public IEnumerable<IModelObject> GetModelObjects(IModelObject parent) {
			AssertOpen();
			if (store != null && ((IEntity)settings).Id != null) {
				if (parent == null) {
					if (loadedModels.Count == 0) {
						Model model;
						if (TryGetModel(out model))
							store.LoadModelModelObjects(this, model.Id);
					}
				} else if (parent.Id != null)
					store.LoadChildModelObjects(this, ((IEntity)parent).Id);
			}
			foreach (EntityBucket<IModelObject> mob in loadedModelObjects) {
				if (mob.State == ItemState.Deleted) continue;
				if (mob.ObjectRef.Parent == parent) {
					if ((parent != null) || (parent == null && mob.Owner is Model))
						yield return mob.ObjectRef;
				}
			}
			foreach (KeyValuePair<IModelObject, IEntity> item in newModelObjects) {
				if (item.Key.Parent == parent) {
					if ((parent != null) || (parent == null && item.Value is Model))
						yield return item.Key;
				}
			}
		}


		/// <override></override>
		public void Insert(IModelObject modelObject) {
			if (modelObject == null) throw new ArgumentNullException("modelObject");
			AssertOpen();
			RepositoryModelObjectsEventArgs e = GetModelObjectsEventArgs(modelObject);
			DoInsertModelObjects(SingleInstanceEnumerator<IModelObject>.Create(modelObject));
			if (ModelObjectsInserted != null) ModelObjectsInserted(this, e);
		}


		/// <override></override>
		public void Insert(IModelObject modelObject, Template template) {
			if (modelObject == null) throw new ArgumentNullException("modelObject");
			AssertOpen();
			RepositoryModelObjectsEventArgs e = GetModelObjectsEventArgs(modelObject);
			DoInsertModelObject(modelObject, template);
			if (ModelObjectsInserted != null) ModelObjectsInserted(this, e);
		}


		/// <override></override>
		public void Insert(IEnumerable<IModelObject> modelObjects) {
			if (modelObjects == null) throw new ArgumentNullException("modelObjects");
			AssertOpen();
			RepositoryModelObjectsEventArgs e = GetModelObjectsEventArgs(modelObjects);
			DoInsertModelObjects(modelObjects);
			if (ModelObjectsInserted != null) ModelObjectsInserted(this, e);
		}


		/// <override></override>
		public void UpdateOwner(IModelObject modelObject, IModelObject parent) {
			if (modelObject == null) throw new ArgumentNullException("modelObject");
			DoUpdateModelObjectOwner(modelObject, parent);
			if (ModelObjectsUpdated != null) ModelObjectsUpdated(this, GetModelObjectsEventArgs(modelObject));
		}


		/// <override></override>
		public void UpdateOwner(IModelObject modelObject, Template template) {
			if (modelObject == null) throw new ArgumentNullException("modelObject");
			if (template == null) throw new ArgumentNullException("template");
			DoUpdateModelObjectOwner(modelObject, template);
			if (ModelObjectsUpdated != null) ModelObjectsUpdated(this, GetModelObjectsEventArgs(modelObject));
		}


		/// <override></override>
		public void UpdateOwner(IModelObject modelObject, Model model) {
			if (modelObject == null) throw new ArgumentNullException("modelObject");
			DoUpdateModelObjectOwner(modelObject, model);
			if (ModelObjectsUpdated != null) ModelObjectsUpdated(this, GetModelObjectsEventArgs(modelObject));
		}


		/// <override></override>
		public void Update(IModelObject modelObject) {
			if (modelObject == null) throw new ArgumentNullException("modelObject");
			AssertOpen();
			RepositoryModelObjectsEventArgs e = GetModelObjectsEventArgs(modelObject);
			DoUpdateModelObject(modelObject);
			if (ModelObjectsUpdated != null) ModelObjectsUpdated(this, e);
		}


		/// <override></override>
		public void Update(IEnumerable<IModelObject> modelObjects) {
			if (modelObjects == null) throw new ArgumentNullException("modelObjects");
			AssertOpen();
			RepositoryModelObjectsEventArgs e = GetModelObjectsEventArgs(modelObjects);
			DoUpdateModelObjects(modelObjects);
			if (ModelObjectsUpdated != null) ModelObjectsUpdated(this, e);
		}


		/// <override></override>
		public void Delete(IModelObject modelObject) {
			if (modelObject == null) throw new ArgumentNullException("modelObject");
			AssertOpen();
			RepositoryModelObjectsEventArgs e = GetModelObjectsEventArgs(modelObject);
			DoDeleteModelObject(modelObject);
			if (ModelObjectsDeleted != null) ModelObjectsDeleted(this, e);
		}


		/// <override></override>
		public void Delete(IEnumerable<IModelObject> modelObjects) {
			if (modelObjects == null) throw new ArgumentNullException("modelObjects");
			AssertOpen();
			RepositoryModelObjectsEventArgs e = GetModelObjectsEventArgs(modelObjects);
			DoDeleteModelObjects(modelObjects);
			if (ModelObjectsDeleted != null) ModelObjectsDeleted(this, e);
		}


		/// <override></override>
		public void Undelete(IModelObject modelObject) {
			if (modelObject == null) throw new ArgumentNullException("modelObject");
			AssertOpen();
			RepositoryModelObjectsEventArgs e = GetModelObjectsEventArgs(modelObject);
			DoUndeleteModelObjects(SingleInstanceEnumerator<IModelObject>.Create(modelObject));
			if (ModelObjectsInserted != null) ModelObjectsInserted(this, e);
		}


		/// <override></override>
		public void Undelete(IEnumerable<IModelObject> modelObjects) {
			if (modelObjects == null) throw new ArgumentNullException("modelObjects");
			AssertOpen();
			RepositoryModelObjectsEventArgs e = GetModelObjectsEventArgs(modelObjects);
			DoUndeleteModelObjects(modelObjects);
			if (ModelObjectsInserted != null) ModelObjectsInserted(this, e);
		}


		/// <override></override>
		public void Unload(IEnumerable<IModelObject> modelObjects) {
			if (modelObjects == null) throw new ArgumentNullException("modelObjects");
			AssertOpen();
			foreach (IModelObject mo in modelObjects) {
				// TODO 2: Should we allow to remove from new model objects?
				if (mo.Id == null) newModelObjects.Remove(mo);
				else loadedModelObjects.Remove(mo.Id);
			}
		}


		/// <override></override>
		public event EventHandler<RepositoryModelObjectsEventArgs> ModelObjectsInserted;

		/// <override></override>
		public event EventHandler<RepositoryModelObjectsEventArgs> ModelObjectsUpdated;

		/// <override></override>
		public event EventHandler<RepositoryModelObjectsEventArgs> ModelObjectsDeleted;

		#endregion


		#region Templates

		/// <override></override>
		public bool IsTemplateInUse(Template template) {
			return IsTemplateInUse(SingleInstanceEnumerator<Template>.Create(template));
		}


		/// <override></override>
		public bool IsTemplateInUse(IEnumerable<Template> templatesToCheck) {
			if (templatesToCheck == null) throw new ArgumentNullException("templatesToCheck");
			AssertOpen();
			// Store shape types in a hash list for better performance when searching huge amounts of shapes
			Dictionary<Template, object> templateDict = new Dictionary<Template, object>();
			foreach (Template template in templatesToCheck)
				templateDict.Add(template, null);
			if (templateDict.Count == 0) return false;

			// Check all new shapes if one of the shape types is used
			foreach (KeyValuePair<Shape, IEntity> keyValuePair in newShapes) {
				// Skip template shapes (and their children) as they are checked by the Project's RemoveLibrary method.
				if (keyValuePair.Value is Template) continue;
				if (keyValuePair.Key.Template == null) continue;
				if (templateDict.ContainsKey(keyValuePair.Key.Template))
					return true;
			}
			// Check all loaded shapes if one of the shape types is used
			foreach (EntityBucket<Shape> entityBucket in loadedShapes) {
				// Skip deleted shapes and template shapes
				if (entityBucket.State == ItemState.Deleted) continue;
				if (entityBucket.ObjectRef.Template == null) continue;
				if (templateDict.ContainsKey(entityBucket.ObjectRef.Template))
					return true;
			}
			// Check all diagrams that are not loaded at the moment
			foreach (KeyValuePair<Template, object> item in templateDict) {
				if (item.Key.Id == null) continue;
				if (Store.CheckTemplateInUse(this, item.Key.Id))
					return true;
			}
			return false;
		}


		// We assume that this is only called once to load all existing templates.
		/// <override></override>
		public IEnumerable<Template> GetTemplates() {
			AssertOpen();
			if (store != null && ((IEntity)settings).Id != null && loadedTemplates.Count <= 0)
				store.LoadTemplates(this, ((IEntity)settings).Id);
			return GetCachedEntities(loadedTemplates, newTemplates);
		}


		/// <override></override>
		public Template GetTemplate(object id) {
			if (id == null) throw new ArgumentNullException("id");
			EntityBucket<Template> result = null;
			AssertOpen();
			if (!loadedTemplates.TryGetValue(id, out result)) {
				AssertStoreExists();
				store.LoadTemplates(this, ((IEntity)settings).Id);
				if (!loadedTemplates.TryGetValue(id, out result))
					throw new NShapeException("Template with id '{0}' not found in store.", id);
				if (result.State == ItemState.Deleted) throw new NShapeException("Template '{0}' was deleted.", result.ObjectRef);
			}
			return result.ObjectRef;
		}


		/// <override></override>
		public Template GetTemplate(string name) {
			if (name == null) throw new ArgumentNullException("name");
			AssertOpen();
			if (store != null && loadedTemplates.Count <= 0)
				store.LoadTemplates(this, ((IEntity)settings).Id);
			foreach (Template t in GetCachedEntities<Template>(loadedTemplates, newTemplates))
				if (t.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
					return t;
			throw new ArgumentException(string.Format("A template named '{0}' does not exist in the repository.", name));
		}


		/// <override></override>
		public void Insert(Template template) {
			if (template == null) throw new ArgumentNullException("template");
			AssertOpen();
			DoInsertTemplate(template, false);
			if (TemplateInserted != null) TemplateInserted(this, GetTemplateEventArgs(template));
		}


		/// <override></override>
		public void InsertAll(Template template) {
			if (template == null) throw new ArgumentNullException("template");
			AssertOpen();
			DoInsertTemplate(template, true);
			if (TemplateInserted != null) TemplateInserted(this, GetTemplateEventArgs(template));
		}


		/// <override></override>
		public void Update(Template template) {
			if (template == null) throw new ArgumentNullException("template");
			AssertOpen();
			UpdateEntity<Template>(loadedTemplates, newTemplates, template);
			if (TemplateUpdated != null) TemplateUpdated(this, GetTemplateEventArgs(template));
		}


		/// <override></override>
		public void ReplaceTemplateShape(Template template, Shape oldShape, Shape newShape) {
			if (template == null) throw new ArgumentNullException("template");
			if (oldShape == null) throw new ArgumentNullException("oldShape");
			if (newShape == null) throw new ArgumentNullException("newShape");
			AssertOpen();
			AssertCanUpdate(template);

			//DoInsertShape(newShape, template);
			UpdateEntity<Template>(loadedTemplates, newTemplates, template);
			// Insert/Undelete new shape
			DoUpdateTemplateModelObject(template);
			DoUpdateTemplateShape(template, true);
			// Delete old shape
			if (oldShape.ModelObject != null) oldShape.ModelObject.DetachShape(oldShape);
			DoDeleteShapes(SingleInstanceEnumerator<Shape>.Create(oldShape), true);

			if (TemplateShapeReplaced != null) TemplateShapeReplaced(this, GetTemplateShapeExchangedEventArgs(template, oldShape, newShape));
		}


		/// <override></override>
		public void Delete(Template template) {
			AssertOpen();
			DoDeleteTemplate(template, false);
			if (TemplateDeleted != null) TemplateDeleted(this, GetTemplateEventArgs(template));
		}


		/// <override></override>
		public void DeleteAll(Template template) {
			AssertOpen();
			DoDeleteTemplate(template, true);
			if (TemplateDeleted != null) TemplateDeleted(this, GetTemplateEventArgs(template));
		}


		/// <override></override>
		public void Undelete(Template template) {
			if (template == null) throw new ArgumentNullException("template");
			AssertOpen();
			DoUndeleteTemplate(template, false);
			if (TemplateInserted != null) TemplateInserted(this, GetTemplateEventArgs(template));
		}


		/// <override></override>
		public void UndeleteAll(Template template) {
			if (template == null) throw new ArgumentNullException("template");
			AssertOpen();
			DoUndeleteTemplate(template, true);
			if (TemplateInserted != null) TemplateInserted(this, GetTemplateEventArgs(template));
		}


		/// <override></override>
		public event EventHandler<RepositoryTemplateEventArgs> TemplateInserted;

		/// <override></override>
		public event EventHandler<RepositoryTemplateEventArgs> TemplateUpdated;

		/// <override></override>
		public event EventHandler<RepositoryTemplateShapeReplacedEventArgs> TemplateShapeReplaced;

		/// <override></override>
		public event EventHandler<RepositoryTemplateEventArgs> TemplateDeleted;

		#endregion


		#region ModelMappings

		/// <override></override>
		public void Insert(IModelMapping modelMapping, Template template) {
			if (modelMapping == null) throw new ArgumentNullException("modelMapping");
			if (template == null) throw new ArgumentNullException("template");
			AssertOpen();
			DoInsertModelMappings(SingleInstanceEnumerator<IModelMapping>.Create(modelMapping), template);
			if (ModelMappingsInserted != null) ModelMappingsInserted(this, GetTemplateEventArgs(template));
		}


		/// <override></override>
		public void Insert(IEnumerable<IModelMapping> modelMappings, Template template) {
			if (modelMappings == null) throw new ArgumentNullException("modelMappings");
			if (template == null) throw new ArgumentNullException("template");
			AssertOpen();
			DoInsertModelMappings(modelMappings, template);
			if (ModelMappingsInserted != null) ModelMappingsInserted(this, GetTemplateEventArgs(template));
		}


		/// <override></override>
		public void Update(IModelMapping modelMapping) {
			if (modelMapping == null) throw new ArgumentNullException("modelMapping");
			AssertOpen();
			DoUpdateModelMappings(SingleInstanceEnumerator<IModelMapping>.Create(modelMapping));
			if (ModelMappingsUpdated != null) ModelMappingsUpdated(this,
				GetTemplateEventArgs(GetModelMappingOwner(modelMapping)));
		}


		/// <override></override>
		public void Update(IEnumerable<IModelMapping> modelMappings) {
			if (modelMappings == null) throw new ArgumentNullException("modelMapping");
			AssertOpen();
			DoUpdateModelMappings(modelMappings);
			if (ModelMappingsUpdated != null) {
				foreach (IModelMapping modelMapping in modelMappings) {
					ModelMappingsUpdated(this, GetTemplateEventArgs(GetModelMappingOwner(modelMapping)));
					break;
				}
			}
		}


		/// <override></override>
		public void Delete(IModelMapping modelMapping) {
			if (modelMapping == null) throw new ArgumentNullException("modelMapping");
			AssertOpen();
			DoDeleteModelMappings(SingleInstanceEnumerator<IModelMapping>.Create(modelMapping));
			if (ModelMappingsDeleted != null) {
				Template owner = GetModelMappingOwner(modelMapping);
				ModelMappingsDeleted(this, GetTemplateEventArgs(owner));
			}
		}


		/// <override></override>
		public void Delete(IEnumerable<IModelMapping> modelMappings) {
			if (modelMappings == null) throw new ArgumentNullException("modelMapping");
			AssertOpen();
			DoDeleteModelMappings(modelMappings);
			if (ModelMappingsDeleted != null) {
				foreach (IModelMapping modelMapping in modelMappings) {
					ModelMappingsDeleted(this, GetTemplateEventArgs(GetModelMappingOwner(modelMapping)));
					break;
				}
			}
		}


		/// <override></override>
		public void Undelete(IModelMapping modelMapping, Template template) {
			if (modelMapping == null) throw new ArgumentNullException("modelMapping");
			AssertOpen();
			DoUndeleteModelMappings(SingleInstanceEnumerator<IModelMapping>.Create(modelMapping), template);
			if (ModelMappingsInserted != null) ModelMappingsInserted(this, GetTemplateEventArgs(template));
		}


		/// <override></override>
		public void Undelete(IEnumerable<IModelMapping> modelMappings, Template template) {
			if (modelMappings == null) throw new ArgumentNullException("modelMapping");
			AssertOpen();
			DoUndeleteModelMappings(modelMappings, template);
			if (ModelMappingsInserted != null) ModelMappingsInserted(this, GetTemplateEventArgs(template));
		}


		/// <override></override>
		public event EventHandler<RepositoryTemplateEventArgs> ModelMappingsInserted;

		/// <override></override>
		public event EventHandler<RepositoryTemplateEventArgs> ModelMappingsUpdated;

		/// <override></override>
		public event EventHandler<RepositoryTemplateEventArgs> ModelMappingsDeleted;

		#endregion


		#region Diagrams

		/// <override></override>
		public IEnumerable<Diagram> GetDiagrams() {
			AssertOpen();
			if (store != null && loadedDiagrams.Count <= 0 && ((IEntity)settings).Id != null)
				store.LoadDiagrams(this, ((IEntity)settings).Id);
			return GetCachedEntities(loadedDiagrams, newDiagrams);
		}


		/// <override></override>
		public Diagram GetDiagram(object id) {
			if (id == null) throw new ArgumentNullException("id");
			EntityBucket<Diagram> result = null;
			AssertOpen();
			if (!loadedDiagrams.TryGetValue(id, out result)) {
				AssertStoreExists();
				store.LoadDiagrams(this, ((IEntity)settings).Id);
				if (!loadedDiagrams.TryGetValue(id, out result))
					throw new NShapeException("Diagram with id '{0}' not found in repository.", id);
				if (result.State == ItemState.Deleted) throw new NShapeException("Diagram '{0}' was deleted.", result.ObjectRef);
			}
			// Do *NOT* load diagram shapes here. The diagramController is responsible for 
			// loading the diagram shapes. Otherwise partial (per diagram) loading does not work.
			//store.LoadDiagramShapes(this, result.ObjectRef);
			return result.ObjectRef;
		}


		/// <override></override>
		public Diagram GetDiagram(string name) {
			if (name == null) throw new ArgumentNullException("name");
			AssertOpen();
			// If there is a diagram, we assume we have already loaded them all.
			if (store != null && loadedDiagrams.Count <= 0 && ((IEntity)settings).Id != null)
				store.LoadDiagrams(this, ((IEntity)settings).Id);
			foreach (Diagram d in GetCachedEntities(loadedDiagrams, newDiagrams))
				if (d.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)) {
					// Do *NOT* load diagram shapes here. The diagramController is responsible for 
					// loading the diagram shapes. Otherwise partial (per diagram) loading does not work.
					//store.LoadDiagramShapes(this, d);
					return d;
				}
			throw new ArgumentException(string.Format("A diagram '{0}' does not exist in the repository.", name));
		}


		/// <override></override>
		public void Insert(Diagram diagram) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			AssertOpen();
			DoInsertDiagram(diagram, false);
			if (DiagramInserted != null) DiagramInserted(this, GetDiagramEventArgs(diagram));
		}


		/// <override></override>
		public void InsertAll(Diagram diagram) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			AssertOpen();
			DoInsertDiagram(diagram, true);
			if (DiagramInserted != null) DiagramInserted(this, GetDiagramEventArgs(diagram));
		}


		/// <override></override>
		public void Update(Diagram diagram) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			AssertOpen();
			AssertCanUpdate(diagram);
			UpdateEntity<Diagram>(loadedDiagrams, newDiagrams, diagram);
			if (DiagramUpdated != null) DiagramUpdated(this, GetDiagramEventArgs(diagram));
		}


		/// <override></override>
		public void Delete(Diagram diagram) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			AssertOpen();
			DoDeleteDiagram(diagram, false);
			if (DiagramDeleted != null) DiagramDeleted(this, GetDiagramEventArgs(diagram));
		}


		/// <override></override>
		public void DeleteAll(Diagram diagram) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			AssertOpen();
			DoDeleteDiagram(diagram, true);
			if (DiagramDeleted != null) DiagramDeleted(this, GetDiagramEventArgs(diagram));
		}


		/// <override></override>
		public void Undelete(Diagram diagram) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			AssertOpen();
			DoUndeleteDiagram(diagram, false);
			if (DiagramInserted != null) DiagramInserted(this, GetDiagramEventArgs(diagram));
		}


		/// <override></override>
		public void UndeleteAll(Diagram diagram) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			AssertOpen();
			DoUndeleteDiagram(diagram, true);
			if (DiagramInserted != null) DiagramInserted(this, GetDiagramEventArgs(diagram));
		}


		/// <override></override>
		public event EventHandler<RepositoryDiagramEventArgs> DiagramInserted;

		/// <override></override>
		public event EventHandler<RepositoryDiagramEventArgs> DiagramUpdated;

		/// <override></override>
		public event EventHandler<RepositoryDiagramEventArgs> DiagramDeleted;

		#endregion


		#region Shapes

		/// <override></override>
		public void GetDiagramShapes(Diagram diagram, params Rectangle[] rectangles) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (((IEntity)diagram).Id == null) return;
			AssertOpen();
			// For the time being a diagram is either loaded or not. No partial diagram loading yet.
			if (diagram.Shapes.Count > 0) return;
			// Load missing shapes
			if (store != null) store.LoadDiagramShapes(this, diagram);
		}


		/// <override></override>
		public void Insert(Shape shape, Diagram diagram) {
			if (shape == null) throw new ArgumentNullException("shape");
			AssertOpen();
			DoInsertShapes(SingleInstanceEnumerator<Shape>.Create(shape), diagram, false);
			if (ShapesInserted != null) ShapesInserted(this, GetShapesEventArgs(shape, diagram));
		}


		/// <override></override>
		public void Insert(Shape shape, Shape parentShape) {
			if (shape == null) throw new ArgumentNullException("shape");
			AssertOpen();
			DoInsertShapes(SingleInstanceEnumerator<Shape>.Create(shape), parentShape, false);
			if (ShapesInserted != null) ShapesInserted(this, GetShapesEventArgs(shape, null));
		}


		/// <override></override>
		public void Insert(Shape shape, Template template) {
			if (shape == null) throw new ArgumentNullException("shape");
			AssertOpen();
			DoInsertShapes(shape, template, false);
			if (ShapesInserted != null) ShapesInserted(this, GetShapesEventArgs(shape, null));
		}


		/// <override></override>
		public void Insert(IEnumerable<Shape> shapes, Diagram diagram) {
			if (shapes == null) throw new ArgumentNullException("shapes");
			if (diagram == null) throw new ArgumentNullException("diagram");
			AssertOpen();
			DoInsertShapes(shapes, diagram, false);
			if (ShapesInserted != null) ShapesInserted(this, GetShapesEventArgs(shapes, diagram));
		}


		/// <override></override>
		public void Insert(IEnumerable<Shape> shapes, Shape parentShape) {
			if (shapes == null) throw new ArgumentNullException("shapes");
			if (parentShape == null) throw new ArgumentNullException("parentShape");
			AssertOpen();
			DoInsertShapes(shapes, parentShape, false);
			if (ShapesInserted != null) ShapesInserted(this, GetShapesEventArgs(shapes, null));
		}


		/// <override></override>
		public void InsertAll(Shape shape, Diagram diagram) {
			if (shape == null) throw new ArgumentNullException("shape");
			if (diagram == null) throw new ArgumentNullException("diagram");
			AssertOpen();
			DoInsertShapes(SingleInstanceEnumerator<Shape>.Create(shape), diagram, true);
			if (ShapesInserted != null) ShapesInserted(this, GetShapesEventArgs(shape, diagram));
		}


		/// <override></override>
		public void InsertAll(Shape shape, Shape parentShape) {
			if (shape == null) throw new ArgumentNullException("shape");
			if (parentShape == null) throw new ArgumentNullException("parentShape");
			AssertOpen();
			DoInsertShapes(SingleInstanceEnumerator<Shape>.Create(shape), parentShape, true);
			if (ShapesInserted != null) ShapesInserted(this, GetShapesEventArgs(shape, null));
		}


		/// <override></override>
		public void InsertAll(Shape shape, Template template) {
			if (shape == null) throw new ArgumentNullException("shape");
			if (template == null) throw new ArgumentNullException("template");
			AssertOpen();
			DoInsertShapes(shape, template, true);
			if (ShapesInserted != null) ShapesInserted(this, GetShapesEventArgs(shape, null));
		}


		/// <override></override>
		public void InsertAll(IEnumerable<Shape> shapes, Diagram diagram) {
			if (shapes == null) throw new ArgumentNullException("shapes");
			if (diagram == null) throw new ArgumentNullException("diagram");
			AssertOpen();
			DoInsertShapes(shapes, diagram, true);
			if (ShapesInserted != null) ShapesInserted(this, GetShapesEventArgs(shapes, diagram));
		}


		/// <override></override>
		public void InsertAll(IEnumerable<Shape> shapes, Shape parentShape) {
			if (shapes == null) throw new ArgumentNullException("shapes");
			if (parentShape == null) throw new ArgumentNullException("parentShape");
			AssertOpen();
			DoInsertShapes(shapes, parentShape, true);
			if (ShapesInserted != null) ShapesInserted(this, GetShapesEventArgs(shapes, null));
		}


		/// <override></override>
		public void Update(Shape shape) {
			if (shape == null) throw new ArgumentNullException("shape");
			AssertOpen();
			DoUpdateShapes(SingleInstanceEnumerator<Shape>.Create(shape));
			if (ShapesUpdated != null) ShapesUpdated(this, GetShapesEventArgs(shape));
		}


		/// <override></override>
		public void Update(IEnumerable<Shape> shapes) {
			if (shapes == null) throw new ArgumentNullException("shapes");
			AssertOpen();
			DoUpdateShapes(shapes);
			if (ShapesUpdated != null) ShapesUpdated(this, GetShapesEventArgs(shapes));
		}


		/// <override></override>
		public void UpdateOwner(Shape shape, Diagram diagram) {
			if (shape == null) throw new ArgumentNullException("shape");
			if (diagram == null) throw new ArgumentNullException("diagram");
			AssertOpen();
			AssertCanUpdate(SingleInstanceEnumerator<Shape>.Create(shape), diagram);
			DoUpdateShapeOwner(shape, diagram);
			if (ShapesUpdated != null) ShapesUpdated(this, GetShapesEventArgs(shape, diagram));
		}


		/// <override></override>
		public void UpdateOwner(Shape shape, Shape parent) {
			if (shape == null) throw new ArgumentNullException("shape");
			if (parent == null) throw new ArgumentNullException("parent");
			AssertOpen();
			AssertCanUpdate(SingleInstanceEnumerator<Shape>.Create(shape), parent);
			DoUpdateShapeOwner(shape, parent);
			if (ShapesUpdated != null) ShapesUpdated(this, GetShapesEventArgs(shape, null));
		}


		/// <override></override>
		public void Delete(Shape shape) {
			if (shape == null) throw new ArgumentNullException("shape");
			// Get event args before deleting because the owner of new shapes is not accessible
			// after deleting ( == removing) new shapes from the repository.
			AssertOpen();
			AssertCanDelete(SingleInstanceEnumerator<Shape>.Create(shape));
			RepositoryShapesEventArgs e = GetShapesEventArgs(shape);
			DoDeleteShapes(SingleInstanceEnumerator<Shape>.Create(shape), false);
			if (ShapesDeleted != null) ShapesDeleted(this, e);
		}


		/// <override></override>
		public void DeleteAll(Shape shape) {
			if (shape == null) throw new ArgumentNullException("shape");
			AssertOpen();
			// Get event args before deleting because the owner of new shapes is not accessible
			// after deleting ( == removing) new shapes from the repository.
			RepositoryShapesEventArgs e = GetShapesEventArgs(shape);
			DoDeleteShapes(SingleInstanceEnumerator<Shape>.Create(shape), true);
			if (ShapesDeleted != null) ShapesDeleted(this, e);
		}


		/// <override></override>
		public void Delete(IEnumerable<Shape> shapes) {
			if (shapes == null) throw new ArgumentNullException("shapes");
			AssertOpen();
			DoDeleteShapes(shapes, false);
			if (ShapesDeleted != null) ShapesDeleted(this, GetShapesEventArgs(shapes));
		}


		/// <override></override>
		public void DeleteAll(IEnumerable<Shape> shapes) {
			if (shapes == null) throw new ArgumentNullException("shapes");
			AssertOpen();
			DoDeleteShapes(shapes, true);
			if (ShapesDeleted != null) ShapesDeleted(this, GetShapesEventArgs(shapes));
		}


		/// <override></override>
		public void Undelete(Shape shape, Diagram diagram) {
			if (shape == null) throw new ArgumentNullException("shape");
			AssertOpen();
			DoUndeleteShapes(shape, diagram, false);
			if (ShapesInserted != null) ShapesInserted(this, GetShapesEventArgs(shape));
		}


		/// <override></override>
		public void UndeleteAll(Shape shape, Diagram diagram) {
			if (shape == null) throw new ArgumentNullException("shape");
			AssertOpen();
			DoUndeleteShapes(shape, diagram, true);
			if (ShapesInserted != null) ShapesInserted(this, GetShapesEventArgs(shape));
		}


		///// <override></override>
		//private void UndeleteShape(Shape shape, Template template) {
		//    if (shape == null) throw new ArgumentNullException("shape");
		//    AssertOpen();
		//    DoUndeleteShapes(shape, template, false);
		//}


		///// <override></override>
		//private void UndeleteShapeWithContent(Shape shape, Template template) {
		//    if (shape == null) throw new ArgumentNullException("shape");
		//    AssertOpen();
		//    DoUndeleteShapes(shape, template, true);
		//}


		/// <override></override>
		public void Undelete(Shape shape, Shape parent) {
			if (shape == null) throw new ArgumentNullException("shape");
			if (parent == null) throw new ArgumentNullException("parentShape");
			AssertOpen();
			DoUndeleteShapes(shape, parent, false);
			if (ShapesInserted != null) ShapesInserted(this, GetShapesEventArgs(shape, null));
		}


		/// <override></override>
		public void UndeleteAll(Shape shape, Shape parent) {
			if (shape == null) throw new ArgumentNullException("shape");
			if (parent == null) throw new ArgumentNullException("parentShape");
			AssertOpen();
			DoUndeleteShapes(shape, parent, true);
			if (ShapesInserted != null) ShapesInserted(this, GetShapesEventArgs(shape, null));
		}


		/// <override></override>
		public void Undelete(Shape shape, Template template) {
			if (shape == null) throw new ArgumentNullException("shape");
			if (template == null) throw new ArgumentNullException("parentShape");
			AssertOpen();
			DoUndeleteShapes(shape, template, false);
			if (ShapesInserted != null) ShapesInserted(this, GetShapesEventArgs(shape, null));
		}


		/// <override></override>
		public void UndeleteAll(Shape shape, Template template) {
			if (shape == null) throw new ArgumentNullException("shape");
			if (template == null) throw new ArgumentNullException("parentShape");
			AssertOpen();
			DoUndeleteShapes(shape, template, true);
			if (ShapesInserted != null) ShapesInserted(this, GetShapesEventArgs(shape, null));
		}


		/// <override></override>
		public void Undelete(IEnumerable<Shape> shapes, Diagram diagram) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (shapes == null) throw new ArgumentNullException("shapes");
			AssertOpen();
			DoUndeleteShapes(shapes, diagram, false);
			if (ShapesInserted != null) ShapesInserted(this, GetShapesEventArgs(shapes, diagram));
		}


		/// <override></override>
		public void UndeleteAll(IEnumerable<Shape> shapes, Diagram diagram) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (shapes == null) throw new ArgumentNullException("shapes");
			AssertOpen();
			DoUndeleteShapes(shapes, diagram, true);
			if (ShapesInserted != null) ShapesInserted(this, GetShapesEventArgs(shapes, diagram));
		}


		/// <override></override>
		public void Undelete(IEnumerable<Shape> shapes, Shape parent) {
			if (parent == null) throw new ArgumentNullException("parent");
			if (shapes == null) throw new ArgumentNullException("shapes");
			AssertOpen();
			DoUndeleteShapes(shapes, parent, false);
			if (ShapesInserted != null) ShapesInserted(this, GetShapesEventArgs(shapes));
		}


		/// <override></override>
		public void UndeleteAll(IEnumerable<Shape> shapes, Shape parent) {
			if (parent == null) throw new ArgumentNullException("parent");
			if (shapes == null) throw new ArgumentNullException("shapes");
			AssertOpen();
			DoUndeleteShapes(shapes, parent, true);
			if (ShapesInserted != null) ShapesInserted(this, GetShapesEventArgs(shapes));
		}


		/// <override></override>
		public void Unload(Diagram diagram) {
			if (diagram == null) throw new ArgumentNullException("diagram");
			DoUnloadShapes(diagram.Shapes);
		}


		/// <override></override>
		public event EventHandler<RepositoryShapesEventArgs> ShapesInserted;

		/// <override></override>
		public event EventHandler<RepositoryShapesEventArgs> ShapesUpdated;

		/// <override></override>
		public event EventHandler<RepositoryShapesEventArgs> ShapesDeleted;

		#endregion


		#region ShapeConnections

		/// <override></override>
		public void InsertConnection(Shape activeShape, ControlPointId gluePointId, Shape passiveShape, ControlPointId connectionPointId) {
			if (activeShape == null) throw new ArgumentNullException("activeShape");
			if (passiveShape == null) throw new ArgumentNullException("passiveShape");
			AssertOpen();
			AssertCanInsert(activeShape, gluePointId, passiveShape, connectionPointId);
			ShapeConnection connection = DoInsertShapeConnection(activeShape, gluePointId, passiveShape, connectionPointId);
			if (ConnectionInserted != null) ConnectionInserted(this, GetShapeConnectionEventArgs(connection));
		}


		/// <override></override>
		public void DeleteConnection(Shape activeShape, ControlPointId gluePointId, Shape passiveShape, ControlPointId connectionPointId) {
			if (activeShape == null) throw new ArgumentNullException("activeShape");
			if (passiveShape == null) throw new ArgumentNullException("passiveShape");
			AssertOpen();
			AssertCanDelete(activeShape, gluePointId, passiveShape, connectionPointId);
			ShapeConnection connection = DoDeleteShapeConnection(activeShape, gluePointId, passiveShape, connectionPointId);
			if (ConnectionDeleted != null) ConnectionDeleted(this, GetShapeConnectionEventArgs(connection));
		}


		/// <override></override>
		public event EventHandler<RepositoryShapeConnectionEventArgs> ConnectionInserted;

		/// <override></override>
		public event EventHandler<RepositoryShapeConnectionEventArgs> ConnectionDeleted;

		#endregion

		#endregion


		#region [Protected] ProjectOwner Class

		/// <summary>
		/// Serves as a parent entity for the project info.
		/// </summary>
		protected class ProjectOwner : IEntity {

			/// <override></override>
			public object Id;

			#region IEntity Members

			object IEntity.Id {
				get { return Id; }
			}

			void IEntity.AssignId(object id) {
				throw new NotImplementedException();
			}

			void IEntity.LoadFields(IRepositoryReader reader, int version) {
				throw new NotImplementedException();
			}

			void IEntity.LoadInnerObjects(string propertyName, IRepositoryReader reader, int version) {
				throw new NotImplementedException();
			}

			void IEntity.SaveFields(IRepositoryWriter writer, int version) {
				throw new NotImplementedException();
			}

			void IEntity.SaveInnerObjects(string PropertyName, IRepositoryWriter writer, int version) {
				throw new NotImplementedException();
			}

			void IEntity.Delete(IRepositoryWriter writer, int version) {
				throw new NotImplementedException();
			}

			#endregion

		}

		#endregion


		#region [Explicit] IStoreCache Members Implementation


		ProjectSettings IStoreCache.Project {
			get { return settings; }
		}


		void IStoreCache.SetRepositoryBaseVersion(int version) {
			this.version = version;
		}


		void IStoreCache.SetProjectOwnerId(object id) {
			projectOwner.Id = id;
		}


		object IStoreCache.ProjectId {
			get { return ((IEntity)settings).Id; }
		}


		string IStoreCache.ProjectName {
			get { return projectName; }
		}


		int IStoreCache.RepositoryBaseVersion {
			get { return version; }
		}


		Design IStoreCache.ProjectDesign {
			get { return projectDesign; }
		}


		IEnumerable<IEntityType> IStoreCache.EntityTypes {
			get { return entityTypes.Values; }
		}


		IEntityType IStoreCache.FindEntityTypeByElementName(string elementName) {
			if (!entityTypes.ContainsKey(elementName)) throw new ArgumentException(string.Format("An entity type with element name '{0}' is not registered.", elementName));
			return entityTypes[elementName];
		}


		IEntityType IStoreCache.FindEntityTypeByName(string name) {
			return FindEntityType(name, true);
		}


		string IStoreCache.CalculateElementName(string entityTypeName) {
			return CachedRepository.CalcElementName(entityTypeName);

		}


		bool IStoreCache.ModelExists() {
			Model model;
			return TryGetModel(out model);
		}


		ICacheCollection<ProjectSettings> IStoreCache.LoadedProjects {
			get { return loadedProjects; }
		}


		IEnumerable<KeyValuePair<ProjectSettings, IEntity>> IStoreCache.NewProjects {
			get { return newProjects; }
		}


		ICacheCollection<Model> IStoreCache.LoadedModels {
			get { return loadedModels; }
		}


		IEnumerable<KeyValuePair<Model, IEntity>> IStoreCache.NewModels {
			get { return newModels; }
		}


		ICacheCollection<Design> IStoreCache.LoadedDesigns {
			get { return loadedDesigns; }
		}


		IEnumerable<KeyValuePair<Design, IEntity>> IStoreCache.NewDesigns {
			get { return newDesigns; }
		}


		ICacheCollection<Diagram> IStoreCache.LoadedDiagrams {
			get { return loadedDiagrams; }
		}


		IEnumerable<KeyValuePair<Diagram, IEntity>> IStoreCache.NewDiagrams {
			get { return newDiagrams; }
		}


		ICacheCollection<Shape> IStoreCache.LoadedShapes {
			get { return loadedShapes; }
		}


		IEnumerable<KeyValuePair<Shape, IEntity>> IStoreCache.NewShapes {
			get { return newShapes; }
		}


		ICacheCollection<IStyle> IStoreCache.LoadedStyles {
			get { return loadedStyles; }
		}


		IEnumerable<KeyValuePair<IStyle, IEntity>> IStoreCache.NewStyles {
			get { return newStyles; }
		}


		ICacheCollection<Template> IStoreCache.LoadedTemplates {
			get { return loadedTemplates; }
		}


		IEnumerable<KeyValuePair<Template, IEntity>> IStoreCache.NewTemplates {
			get { return newTemplates; }
		}


		ICacheCollection<IModelMapping> IStoreCache.LoadedModelMappings {
			get { return loadedModelMappings; }
		}


		IEnumerable<KeyValuePair<IModelMapping, IEntity>> IStoreCache.NewModelMappings {
			get { return newModelMappings; }
		}


		ICacheCollection<IModelObject> IStoreCache.LoadedModelObjects {
			get { return loadedModelObjects; }
		}


		IEnumerable<KeyValuePair<IModelObject, IEntity>> IStoreCache.NewModelObjects {
			get { return newModelObjects; }
		}


		IEnumerable<ShapeConnection> IStoreCache.NewShapeConnections {
			get { return newShapeConnections; }
		}


		IEnumerable<ShapeConnection> IStoreCache.DeletedShapeConnections {
			get { return deletedShapeConnections; }
		}


		IStyle IStoreCache.GetProjectStyle(object id) {
			return loadedStyles[id].ObjectRef;
		}


		Template IStoreCache.GetTemplate(object id) {
			return loadedTemplates[id].ObjectRef;
		}


		Diagram IStoreCache.GetDiagram(object id) {
			return loadedDiagrams[id].ObjectRef;
		}


		Shape IStoreCache.GetShape(object id) {
			return loadedShapes[id].ObjectRef;
		}


		IModelObject IStoreCache.GetModelObject(object id) {
			return loadedModelObjects[id].ObjectRef;
		}


		Design IStoreCache.GetDesign(object id) {
			return loadedDesigns[id].ObjectRef;
		}

		#endregion


		#region [Private] Implementation

		private void AcceptAll() {
			AcceptEntities<ProjectSettings>(loadedProjects, newProjects);
			AcceptEntities<Design>(loadedDesigns, newDesigns);
			AcceptEntities<IStyle>(loadedStyles, newStyles);
			AcceptEntities<Template>(loadedTemplates, newTemplates);
			AcceptEntities<IModelMapping>(loadedModelMappings, newModelMappings);
			AcceptEntities<IModelObject>(loadedModelObjects, newModelObjects);
			AcceptEntities<Model>(loadedModels, newModels);
			AcceptEntities<Diagram>(loadedDiagrams, newDiagrams);
			AcceptEntities<Shape>(loadedShapes, newShapes);
			newShapeConnections.Clear();
			deletedShapeConnections.Clear();
			isModified = false;
		}


		private void ClearBuffers() {
			projectDesign = null;

			newProjects.Clear();
			newDesigns.Clear();
			newStyles.Clear();
			newDiagrams.Clear();
			newTemplates.Clear();
			newShapes.Clear();
			newModels.Clear();
			newModelObjects.Clear();
			newModelMappings.Clear();

			loadedProjects.Clear();
			loadedStyles.Clear();
			loadedDesigns.Clear();
			loadedDiagrams.Clear();
			loadedTemplates.Clear();
			loadedShapes.Clear();
			loadedModels.Clear();
			loadedModelObjects.Clear();
			loadedModelMappings.Clear();
		}


		/// <summary>
		/// Calculates an XML tag name for the given entity name.
		/// </summary>
		static private string CalcElementName(string entityName) {
			string result;
			// We remove the prefixes Core. and GeneralShapes.
			if (entityName.StartsWith("Core.")) result = entityName.Substring(5);
			else if (entityName.StartsWith("GeneralShapes.")) result = entityName.Substring(14);
			else result = entityName;
			// ReplaceRange invalid characters
			result = result.Replace(' ', '_');
			result = result.Replace('/', '_');
			result = result.Replace('<', '_');
			result = result.Replace('>', '_');
			// We replace Camel casing with underscores
			stringBuilder.Length = 0;
			for (int i = 0; i < result.Length; ++i) {
				if (char.IsUpper(result[i])) {
					// Avoid multiple subsequent underscores
					if (i > 0 && stringBuilder.Length > 0 && stringBuilder[stringBuilder.Length - 1] != '_')
						stringBuilder.Append('_');
					stringBuilder.Append(char.ToLowerInvariant(result[i]));
				} else stringBuilder.Append(result[i]);
			}
			// We use namespace prefixes for the library names
			// Not yet, must use prefix plus name in order to do that
			// result = result.ReplaceRange('.', ':');
			return stringBuilder.ToString();
		}


		private IEntityType FindEntityType(string entityTypeName, bool mustExist) {
			IEntityType result;
			entityTypes.TryGetValue(CalcElementName(entityTypeName), out result);
			if (mustExist && result == null)
				throw new NShapeException("Entity type '{0}' does not exist in the repository.", entityTypeName);
			return result;
		}

		#endregion


		#region [Private] Implementation (Generic Methods)

		private IEnumerable<TEntity> GetCachedEntities<TEntity>(LoadedEntities<TEntity> loadedEntities,
			IDictionary<TEntity, IEntity> newEntities) where TEntity : IEntity {
			foreach (EntityBucket<TEntity> eb in loadedEntities) {
				if (eb.State != ItemState.Deleted)
					yield return eb.ObjectRef;
			}
			foreach (KeyValuePair<TEntity, IEntity> item in newEntities)
				yield return item.Key;
		}


		private IEnumerable<TEntity> GetCachedEntities<TEntity>(LoadedEntities<TEntity> loadedEntities,
			IDictionary<TEntity, IEntity> newEntities, IEntity owner) where TEntity : IEntity {
			foreach (EntityBucket<TEntity> eb in loadedEntities) {
				if (eb.Owner == owner && eb.State != ItemState.Deleted)
					yield return eb.ObjectRef;
			}
			foreach (KeyValuePair<TEntity, IEntity> item in newEntities)
				if (item.Value == owner) yield return item.Key;
		}


		/// <summary>
		/// Inserts an entity into the internal cache and marks it as new.
		/// </summary>
		private void InsertEntity<TEntity>(Dictionary<TEntity, IEntity> newEntities,
			TEntity entity, IEntity owner) where TEntity : IEntity {
			if (entity.Id != null)
				throw new ArgumentException("Entities with an id cannot be inserted into the repository.");
			newEntities.Add(entity, owner);
			isModified = true;
		}


		/// <summary>
		/// Updates an entity in the internal cache and marks it as modified.
		/// </summary>
		private void UpdateEntity<TEntity>(Dictionary<object, EntityBucket<TEntity>> loadedEntities,
			Dictionary<TEntity, IEntity> newEntities, TEntity entity) where TEntity : IEntity {
			if (entity.Id == null) {
				if (!newEntities.ContainsKey(entity))
					throw new NShapeException(string.Format("Entity not found in repository."));
			} else {
				EntityBucket<TEntity> item;
				if (!loadedEntities.TryGetValue(entity.Id, out item))
					throw new NShapeException("Entity not found in repository.");
				if (item.State == ItemState.Deleted) throw new NShapeException("Entity was deleted before. Undelete the entity before modifying it.");
				item.State = ItemState.Modified;
			}
			isModified = true;
		}


		/// <summary>
		/// Marks the entity for deletion from the data store. 
		/// Must be called after all children have been removed.
		/// </summary>
		private void DeleteEntity<TEntity>(Dictionary<object, EntityBucket<TEntity>> loadedEntities,
			Dictionary<TEntity, IEntity> newEntities, TEntity entity) where TEntity : IEntity {
			if (entity.Id == null) {
				if (!newEntities.ContainsKey(entity))
					throw new NShapeException(string.Format("Entity not found in repository.", entity.Id));
				newEntities.Remove(entity);
			} else {
				EntityBucket<TEntity> item;
				if (!loadedEntities.TryGetValue(entity.Id, out item))
					throw new NShapeException("Entity not found in repository.");
				if (item.State == ItemState.Deleted) throw new NShapeException("Entity is already deleted.");
				item.State = ItemState.Deleted;
			}
			isModified = true;
		}


		private void UndeleteEntity<TEntity>(Dictionary<object, EntityBucket<TEntity>> loadedEntities,
			TEntity entity) where TEntity : IEntity {
			if (entity.Id == null) throw new NShapeException(string.Format("An entity without id cannot be undeleted.", entity.Id));
			else {
				EntityBucket<TEntity> item;
				if (!loadedEntities.TryGetValue(entity.Id, out item))
					throw new NShapeException("Entity not found in repository.");
				if (item.State != ItemState.Deleted) throw new NShapeException("Entity was not deleted before. Onlydeleted entities can be undeleted.");
				item.State = ItemState.Modified;
			}
			isModified = true;
		}


		private void UndeleteEntity<TEntity>(Dictionary<object, EntityBucket<TEntity>> loadedEntities,
			TEntity entity, IEntity owner) where TEntity : IEntity {
			if (entity.Id == null) throw new NShapeException(string.Format("An entity without id cannot be undeleted.", entity.Id));
			else {
				EntityBucket<TEntity> item;
				if (!loadedEntities.TryGetValue(entity.Id, out item))
					loadedEntities.Add(entity.Id, new EntityBucket<TEntity>(entity, owner, ItemState.New));
				else {
					if (item.State != ItemState.Deleted) throw new NShapeException("Entity was not deleted before. Onlydeleted entities can be undeleted.");
					item.State = ItemState.Modified;
					Debug.Assert(item.Owner == owner);
				}
			}
			isModified = true;
		}


		private void AcceptEntities<EntityType>(Dictionary<object, EntityBucket<EntityType>> loadedEntities,
			Dictionary<EntityType, IEntity> newEntities) where EntityType : IEntity {
			// Remove deleted entities from loaded Entities
			List<object> deletedEntities = new List<object>(100);
			foreach (KeyValuePair<object, EntityBucket<EntityType>> ep in loadedEntities)
				if (ep.Value.State == ItemState.Deleted)
					deletedEntities.Add(ep.Key);
			foreach (object id in deletedEntities)
				loadedEntities.Remove(id);
			deletedEntities.Clear();
			deletedEntities = null;

			// Mark modified entities as original
			foreach (KeyValuePair<object, EntityBucket<EntityType>> item in loadedEntities) {
				if (item.Value.State != ItemState.Original) {
					Debug.Assert(loadedEntities[item.Key].State != ItemState.Deleted);
					loadedEntities[item.Key].State = ItemState.Original;
				}
			}

			// Move new entities from newEntities to loadedEntities
			foreach (KeyValuePair<EntityType, IEntity> ep in newEntities) {
				// Settings do not have a parent
				loadedEntities.Add(ep.Key.Id, new EntityBucket<EntityType>(ep.Key, ep.Value, ItemState.Original));
			}
			newEntities.Clear();
		}


		/// <summary>
		/// Defines a dictionary for loaded entity types.
		/// </summary>
		private class LoadedEntities<TEntity> : Dictionary<object, EntityBucket<TEntity>>,
			ICacheCollection<TEntity> where TEntity : IEntity {

			#region ICacheCollection<TEntity> Members

			public bool Contains(object id) {
				return ContainsKey(id);
			}


			public TEntity GetEntity(object id) {
				return this[id].ObjectRef;
			}


			public void Add(EntityBucket<TEntity> bucket) {
				Add(((IEntity)bucket.ObjectRef).Id, bucket);
			}

			#endregion


			#region IEnumerable<TEntity> Members

			// ToDo 3: Find a way to avoid the new keyword
			public new IEnumerator<EntityBucket<TEntity>> GetEnumerator() {
				foreach (EntityBucket<TEntity> eb in Values)
					yield return eb;
			}

			#endregion

		}


		private bool Contains<T>(IEnumerable<T> collection, T obj) {
			foreach (T item in collection)
				if (item != null && item.Equals(obj)) return true;
			return false;
		}

		#endregion


		#region [Private] Implemenation - Design and Styles

		private void DoInsertDesign(Design design, ProjectSettings projectData, bool withStyles) {
			AssertCanInsert(design);
			if (withStyles) AssertCanInsert(design.Styles);

			InsertEntity<Design>(newDesigns, design, projectData);
			if (withStyles) {
				foreach (IStyle s in design.Styles)
					DoInsertStyle(design, s);
			}
			isModified = true;
		}


		private void DoDeleteDesign(Design design, bool withStyles) {
			// First, delete all styles (ColorStyles least)
			if (withStyles) {
				AssertCanDelete(design.Styles);
				foreach (IStyle s in design.CapStyles)
					DoDeleteStyle(s);
				foreach (IStyle s in design.CharacterStyles)
					DoDeleteStyle(s);
				foreach (IStyle s in design.FillStyles)
					DoDeleteStyle(s);
				foreach (IStyle s in design.LineStyles)
					DoDeleteStyle(s);
				foreach (IStyle s in design.ParagraphStyles)
					DoDeleteStyle(s);
				foreach (IStyle s in design.ColorStyles)
					DoDeleteStyle(s);
			}
			AssertCanDelete(design);
			DeleteEntity<Design>(loadedDesigns, newDesigns, design);
		}


		private void DoUndeleteDesign(Design design, bool withStyles) {
			AssertCanUndelete(design);
			UndeleteEntity<Design>(loadedDesigns, design);
			if (withStyles) {
				AssertCanUndelete(design.Styles);
				// Undelete styles (ColorStyles first)
				foreach (IStyle s in design.ColorStyles)
					UndeleteEntity<IStyle>(loadedStyles, s, design);
				foreach (IStyle s in design.CapStyles)
					UndeleteEntity<IStyle>(loadedStyles, s, design);
				foreach (IStyle s in design.CharacterStyles)
					UndeleteEntity<IStyle>(loadedStyles, s, design);
				foreach (IStyle s in design.FillStyles)
					UndeleteEntity<IStyle>(loadedStyles, s, design);
				foreach (IStyle s in design.LineStyles)
					UndeleteEntity<IStyle>(loadedStyles, s, design);
				foreach (IStyle s in design.ParagraphStyles)
					UndeleteEntity<IStyle>(loadedStyles, s, design);
			}
		}


		private void DoInsertStyle(Design design, IStyle style) {
			InsertEntity<IStyle>(newStyles, style, design);
		}


		private void DoUpdateStyle(IStyle style) {
			UpdateEntity<IStyle>(loadedStyles, newStyles, style);
		}


		private void DoDeleteStyle(IStyle style) {
			DeleteEntity<IStyle>(loadedStyles, newStyles, style);
		}


		private void DoUndeleteStyle(Design design, IStyle style) {
			UndeleteEntity<IStyle>(loadedStyles, style, design);
		}


		/// <summary>
		/// Retrieves the indicated project style, which is always loaded when the project is open.
		/// </summary>
		private IStyle GetProjectStyle(object id) {
			EntityBucket<IStyle> styleItem;
			if (!loadedStyles.TryGetValue(id, out styleItem))
				throw new NShapeException("Style with id '{0}' does not exist.", id);
			if (styleItem.State == ItemState.Deleted) throw new NShapeException("Style '{0}' was deleted.", styleItem.ObjectRef);
			return styleItem.ObjectRef;
		}


		private bool UsesStyle(IStyle style, IStyle ownerStyle) {
			Debug.Assert(style != null);
			Debug.Assert(ownerStyle != null);
			if (ownerStyle is ICapStyle && ((ICapStyle)ownerStyle).ColorStyle == style)
				return true;
			else if (ownerStyle is ICharacterStyle && ((ICharacterStyle)ownerStyle).ColorStyle == style)
				return true;
			else if (ownerStyle is IFillStyle) {
				IFillStyle fillStyle = (IFillStyle)ownerStyle;
				if (fillStyle.AdditionalColorStyle == style) return true;
				if (fillStyle.BaseColorStyle == style) return true;
			} else if (ownerStyle is ILineStyle && ((ILineStyle)ownerStyle).ColorStyle == style)
				return true;
			return false;
		}


		private bool UsesStyle(IStyle style, IModelMapping modelMapping) {
			if (modelMapping is StyleModelMapping) {
				StyleModelMapping styleMapping = (StyleModelMapping)modelMapping;
				foreach (object range in styleMapping.ValueRanges) {
					if (range is int) {
						if (styleMapping[(int)range] == style) return true;
					} else if (range is float) {
						if (styleMapping[(float)range] == style) return true;
					}
				}
			}
			return false;
		}

		#endregion


		#region [Private] Implementation - Templates and ModelMappings


		private void DoInsertTemplate(Template template, bool withContent) {
			AssertCanInsert(template, withContent);
			InsertEntity<Template>(newTemplates, template, GetProject());
			if (withContent) {
				// Insert template model object
				if (template.Shape.ModelObject != null)
					DoInsertModelObject(template.Shape.ModelObject, template);
				// Insert template shape
				DoInsertShapes(template.Shape, template, withContent);
				// Insert model property mappings
				if (template.Shape.ModelObject != null)
					DoInsertModelMappings(template.GetPropertyMappings(), template);
			}
		}


		private void DoUpdateTemplateShape(Template template, bool withContent) {
			// Insert / undelete / update shape
			if (((IEntity)template.Shape).Id == null && !newShapes.ContainsKey(template.Shape))
				DoInsertShapes(template.Shape, template, withContent);
			else {
				if (CanUndeleteEntity<Shape>(template.Shape, loadedShapes))
					DoUndeleteShapes(template.Shape, template, withContent);
				DoUpdateTemplateShapeWithContent(template.Shape);
			}
		}


		private void DoUpdateTemplateShapeWithContent(Shape shape) {
			AssertCanUpdate(GetShapes(SingleInstanceEnumerator<Shape>.Create(shape), true));
			UpdateEntity<Shape>(loadedShapes, newShapes, shape);
			// Update connections automatically will not work due to the fact that loaded connections 
			// are not stored and therefore we don't know which connections are really new.
			//DoUpdateShapeConnections(shape);

			if (shape.Children.Count > 0) {
				// Delete removed child shapes
				List<Shape> shapesToDelete = new List<Shape>();
				foreach (Shape childShape in GetCachedEntities<Shape>(loadedShapes, newShapes, shape))
					if (!shape.Children.Contains(childShape)) shapesToDelete.Add(childShape);
				DoDeleteShapes(shapesToDelete, true);

				// Update/Insert all existing child shapes
				foreach (Shape childShape in shape.Children) {
					object childId = ((IEntity)childShape).Id;
					if (CanUndeleteEntity(childShape, loadedShapes))
						DoUndeleteShapes(childShape, shape, true);
					else if ((childId != null && loadedShapes.Contains(childId)) || newShapes.ContainsKey(childShape))
						DoUpdateTemplateShapeWithContent(childShape);
					else DoInsertShapes(SingleInstanceEnumerator<Shape>.Create(childShape), shape, true);
				}
			}
		}


		private void DoUpdateTemplateModelObject(Template template) {
			// Insert / undelete / update model object
			if (template.Shape.ModelObject != null) {
				IModelObject modelObject = template.Shape.ModelObject;
				if (modelObject.Id == null && !newModelObjects.ContainsKey(modelObject))
					DoInsertModelObject(template.Shape.ModelObject, template);
				else {
					if (CanUndeleteEntity<IModelObject>(modelObject, loadedModelObjects))
						DoUndeleteModelObject(modelObject);
					DoUpdateModelObject(template.Shape.ModelObject);
				}
			}
		}


		private void DoDeleteTemplate(Template template, bool withContent) {
			if (template == null) throw new ArgumentNullException("template");
			if (withContent) {
				// Delete template's model object
				IModelObject modelObject = template.Shape.ModelObject;
				if (modelObject != null) {
					// Detach shape and model object
					template.Shape.ModelObject = null;
					DoDeleteModelObject(modelObject);
				}
				// Delete the template's shape and model object
				//DoDeleteShapes(GetShapes(SingleInstanceEnumerator<Shape>.Create(template.Shape), true), withContent);
				DoDeleteShapes(SingleInstanceEnumerator<Shape>.Create(template.Shape), withContent);
				// Delete template's model mappings
				DoDeleteModelMappings(template.GetPropertyMappings());
			}
			// Delete the template
			AssertCanDelete(template);
			DeleteEntity<Template>(loadedTemplates, newTemplates, template);
			if (TemplateDeleted != null) TemplateDeleted(this, GetTemplateEventArgs(template));
		}


		private void DoUndeleteTemplate(Template template, bool withContent) {
			UndeleteEntity<Template>(loadedTemplates, template);
			if (withContent) {
				DoUndeleteShapes(template.Shape, template, withContent);

				if (template.Shape.ModelObject != null) {
					DoUndeleteModelObject(template.Shape.ModelObject);
					Undelete(template.GetPropertyMappings(), template);
				}
			}
		}


		private void DoInsertModelMappings(IEnumerable<IModelMapping> modelMappings, Template template) {
			AssertCanInsert(modelMappings);
			foreach (IModelMapping modelMapping in modelMappings)
				InsertEntity<IModelMapping>(newModelMappings, modelMapping, template);
		}


		private void DoUpdateModelMappings(IEnumerable<IModelMapping> modelMappings) {
			AssertCanUpdate(modelMappings);
			foreach (IModelMapping modelMapping in modelMappings)
				UpdateEntity<IModelMapping>(loadedModelMappings, newModelMappings, modelMapping);
		}


		private void DoDeleteModelMappings(IEnumerable<IModelMapping> modelMappings) {
			AssertCanDelete(modelMappings);
			foreach (IModelMapping modelMapping in modelMappings)
				DeleteEntity<IModelMapping>(loadedModelMappings, newModelMappings, modelMapping);
		}


		private void DoUndeleteModelMappings(IEnumerable<IModelMapping> modelMappings, Template owner) {
			AssertCanUndelete(modelMappings);
			foreach (IModelMapping modelMapping in modelMappings)
				UndeleteEntity<IModelMapping>(loadedModelMappings, modelMapping, owner);
		}


		private Template GetModelMappingOwner(IModelMapping modelMapping) {
			Template owner = null;
			if (modelMapping.Id == null) {
				Debug.Assert(newModelMappings.ContainsKey(modelMapping));
				Debug.Assert(newModelMappings[modelMapping] is Template);
				owner = (Template)newModelMappings[modelMapping];
			} else {
				Debug.Assert(loadedModelMappings[modelMapping.Id].Owner is Template);
				owner = (Template)loadedModelMappings[modelMapping.Id].Owner;
			}
			return owner;
		}

		#endregion


		#region [Private] Implementation - Diagrams, Shapes and ShapeConnections

		private void DoInsertDiagram(Diagram diagram, bool withContent) {
			AssertCanInsert(diagram);
			InsertEntity<Diagram>(newDiagrams, diagram, GetProject());
			if (withContent) {
				AssertCanInsert(diagram.Shapes, diagram);
				DoInsertShapes(diagram.Shapes, diagram, withContent);
			}
		}


		private void DoDeleteDiagram(Diagram diagram, bool withContent) {
			if (withContent) {
				AssertCanDelete(diagram.Shapes);
				// First, delete all shapes with their connections
				DoDeleteShapes(diagram.Shapes, withContent);
				if (ShapesDeleted != null) ShapesDeleted(this, GetShapesEventArgs(diagram.Shapes, diagram));
			}
			// Now we can delete the actual diagram
			AssertCanDelete(diagram);
			DeleteEntity<Diagram>(loadedDiagrams, newDiagrams, diagram);
		}


		private void DoUndeleteDiagram(Diagram diagram, bool withContent) {
			AssertCanUndelete(diagram);
			UndeleteEntity<Diagram>(loadedDiagrams, diagram);
			if (withContent) {
				AssertCanUndelete(diagram.Shapes, diagram);
				// First, delete all shapes with their connections
				DoUndeleteShapes(diagram.Shapes, diagram, withContent);
			}
		}


		private void DoInsertShapes(IEnumerable<Shape> shapes, Shape parentShape, bool withContent) {
			if (shapes == null) throw new ArgumentNullException("shapes");
			if (parentShape == null) throw new ArgumentNullException("parentShape");
			AssertCanInsert(shapes, parentShape);
			DoInsertShapesCore(shapes, parentShape, withContent);
		}

		
		private void DoInsertShapes(IEnumerable<Shape> shapes, Diagram diagram, bool withContent) {
			if (shapes == null) throw new ArgumentNullException("shapes");
			if (diagram == null) throw new ArgumentNullException("diagram");
			AssertCanInsert(shapes, diagram);
			DoInsertShapesCore(shapes, diagram, withContent);
		}


		private void DoInsertShapes(Shape shape, Template template, bool withContent) {
			if (shape == null) throw new ArgumentNullException("shape");
			if (template == null) throw new ArgumentNullException("template");
			AssertCanInsert(GetShapes(SingleInstanceEnumerator<Shape>.Create(shape), true), template);
			DoInsertShapesCore(SingleInstanceEnumerator<Shape>.Create(shape), template, withContent);
		}


		private void DoInsertShapesCore(IEnumerable<Shape> shapes, IEntity parentEntity, bool withContent) {
			foreach (Shape shape in shapes) {
				// This check will be done in CanInsert(Shape shape)
				if (shape.ModelObject != null)
					Debug.Assert(IsExistent(shape.ModelObject, newModelObjects, loadedModelObjects), "Shape has a model object that does not exist in the repository.");
				InsertEntity<Shape>(newShapes, shape, parentEntity);
				if (withContent)
					DoInsertShapesCore(shape.Children.BottomUp, shape, withContent);
			}
			// Insert connections only when inserting with content
			if (withContent) {
				foreach (Shape shape in shapes) {
					// Insert only connectons of the active shapes
					foreach (ShapeConnectionInfo sci in shape.GetConnectionInfos(ControlPointId.Any, null))
						if (shape.HasControlPointCapability(sci.OwnPointId, ControlPointCapabilities.Glue))
							DoInsertShapeConnection(shape, sci);
				}
			}
		}


		private void DoUpdateShapes(IEnumerable<Shape> shapes) {
			AssertCanUpdate(shapes);
			foreach (Shape shape in shapes)
				UpdateEntity<Shape>(loadedShapes, newShapes, shape);
		}


		private void DoUpdateShapeOwner(Shape shape, IEntity owner) {
			if (((IEntity)shape).Id == null) {
				newShapes.Remove(shape);
				newShapes.Add(shape, owner);
			} else {
				loadedShapes[((IEntity)shape).Id].Owner = owner;
				loadedShapes[((IEntity)shape).Id].State = ItemState.OwnerChanged;
			}
		}


		//private void DoDeleteShapes(Shape shape, bool withContent) {
		//    DoDeleteShapes(SingleInstanceEnumerator<Shape>.Create(shape), withContent);
		//}


		private void DoDeleteShapes(IEnumerable<Shape> shapes, bool withContent) {
			AssertCanDelete(GetShapes(shapes, withContent));
			DoDeleteShapesCore(shapes, withContent);
		}


		private void DoDeleteShapesCore(IEnumerable<Shape> shapes, bool withContent) {
			if (withContent) {
				// Delete the shape's children
				foreach (Shape shape in shapes) {
					DoDeleteShapesCore(shape.Children, withContent);
					if (withContent) {
						// Delete all connections of the deleted shapes
						foreach (ShapeConnectionInfo sci in shape.GetConnectionInfos(ControlPointId.Any, null))
							if (shape.HasControlPointCapability(sci.OwnPointId, ControlPointCapabilities.Glue))
								DoDeleteShapeConnection(shape, sci);
					}
				}
			}
			// Delete the shape itself
			foreach (Shape shape in shapes)
				DeleteEntity<Shape>(loadedShapes, newShapes, shape);
		}


		private void DoUndeleteShapes(Shape shape, IEntity parentEntity, bool withContent) {
			AssertOpen();
			AssertCanUndelete(GetShapes(SingleInstanceEnumerator<Shape>.Create(shape), withContent), parentEntity);
			DoUndeleteShapes(SingleInstanceEnumerator<Shape>.Create(shape), parentEntity, withContent);
		}


		private void DoUndeleteShapes(IEnumerable<Shape> shapes, IEntity parentEntity, bool withContent) {
			AssertOpen();
			foreach (Shape shape in shapes) {
				UndeleteEntity<Shape>(loadedShapes, shape);
				// Re-attach model object after undelete
				if (shape.ModelObject != null) {
					bool isAttached = false;
					foreach (Shape s in shape.ModelObject.Shapes)
						if (shape == s) {
							isAttached = true;
							break;
						}
					if (!isAttached) shape.ModelObject.AttachShape(shape);
				}
				if (withContent) {
					if (shape.Children.Count > 0)
						DoUndeleteShapes(shape.Children, shape, withContent);
					// Insert connections
					foreach (ShapeConnectionInfo sci in shape.GetConnectionInfos(ControlPointId.Any, null))
						if (shape.HasControlPointCapability(sci.OwnPointId, ControlPointCapabilities.Glue))
							DoInsertShapeConnection(shape, sci);
				}
			}
		}


		private void DoUnloadShapes(IEnumerable<Shape> shapes) {
			// First unload the children then the parent.
			// TODO 2: Should we allow to remove from new shapes?
			foreach (Shape s in shapes) {
				DoUnloadShapes(s.Children);
				if (((IEntity)s).Id == null) newShapes.Remove(s);
				else loadedShapes.Remove(((IEntity)s).Id);
			}
		}


		private ShapeConnection DoInsertShapeConnection(Shape shape, ShapeConnectionInfo connectionInfo) {
			if (shape.HasControlPointCapability(connectionInfo.OwnPointId, ControlPointCapabilities.Glue))
				return DoInsertShapeConnection(shape, connectionInfo.OwnPointId, connectionInfo.OtherShape, connectionInfo.OtherPointId);
			else return DoInsertShapeConnection(connectionInfo.OtherShape, connectionInfo.OtherPointId, shape, connectionInfo.OwnPointId);
		}


		private ShapeConnection DoInsertShapeConnection(Shape activeShape, ControlPointId gluePointId, Shape passiveShape, ControlPointId connectionPointId) {
			ShapeConnection connection;
			connection.ConnectorShape = activeShape;
			connection.GluePointId = gluePointId;
			connection.TargetShape = passiveShape;
			connection.TargetPointId = connectionPointId;

			// If the inserted connection is not in the list of deleted connections, it's a new 
			// connection and has to be added to the list of new connections.
			if (!deletedShapeConnections.Remove(connection)) {
				Debug.Assert(!newShapeConnections.Contains(connection), "Connection already inserted in repository.");
				newShapeConnections.Add(connection);
			}
			isModified = true;
			return connection;
		}


		private ShapeConnection DoDeleteShapeConnection(Shape shape, ShapeConnectionInfo connectionInfo) {
			if (shape.HasControlPointCapability(connectionInfo.OwnPointId, ControlPointCapabilities.Glue))
				return DoDeleteShapeConnection(shape, connectionInfo.OwnPointId, connectionInfo.OtherShape, connectionInfo.OtherPointId);
			else return DoDeleteShapeConnection(connectionInfo.OtherShape, connectionInfo.OtherPointId, shape, connectionInfo.OwnPointId);
		}


		private ShapeConnection DoDeleteShapeConnection(Shape activeShape, ControlPointId gluePointId, Shape passiveShape, ControlPointId connectionPointId) {
			ShapeConnection connection;
			connection.ConnectorShape = activeShape;
			connection.GluePointId = gluePointId;
			connection.TargetShape = passiveShape;
			connection.TargetPointId = connectionPointId;

			// If the deleted connection is not in the list of new connections, it's a loaded 
			// connection and has to be added to the list of deleted connections
			if (!newShapeConnections.Remove(connection)) {
				Debug.Assert(!deletedShapeConnections.Contains(connection), "Connection already deleted from repository.");
				deletedShapeConnections.Add(connection);
			}
			isModified = true;
			return connection;
		}


		private bool IsShapeInRepository(Shape shape) {
			Debug.Assert(shape != null);
			IEntity shapeEntity = (IEntity)shape;
			if (shapeEntity.Id == null) 
				return newShapes.ContainsKey(shape);
			else return loadedShapes.ContainsKey(shapeEntity.Id);
		}


		private bool IsShapeInCollection(Shape shape, IEnumerable<Shape> shapeCollection) {
			Debug.Assert(shape != null);
			foreach (Shape s in shapeCollection)
				if (s == shape) return true;
			return false;
		}


		/// <summary>
		/// Returns all shapes (child and parent shapes) in a 'flat' collection
		/// </summary>
		private IEnumerable<Shape> GetShapes(IEnumerable<Shape> shapes, bool withChildren) {
			foreach (Shape shape in shapes) {
				if (withChildren)
					foreach (Shape childShape in GetShapes(shape.Children, true))
						yield return childShape;
				yield return shape;
			}
		}

		#endregion


		#region [Private] Implementation - Model and ModelObjects

		private bool TryGetModel(out Model model) {
			AssertOpen();
			model = null;
			if (store != null && loadedModels.Count <= 0 && ((IEntity)settings).Id != null)
				store.LoadModel(this, ((IEntity)settings).Id);
			foreach (Model m in GetCachedEntities(loadedModels, newModels)) {
				model = m;
				return true;
			}
			return false;
		}


		private EntityBucket<IModelObject> GetModelObjectItem(object id) {
			if (id == null) throw new NShapeException("ModelObject has no identifier.");
			EntityBucket<IModelObject> item;
			if (!loadedModelObjects.TryGetValue(id, out item))
				throw new NShapeException(string.Format("ModelObject {0} not found.", id));
			return item;
		}


		private void DoInsertModelObjects(IEnumerable<IModelObject> modelObjects) {
			AssertCanInsert(modelObjects);
			IEntity owner;
			foreach (IModelObject modelObject in modelObjects) {
				if (modelObject.Parent != null) owner = modelObject.Parent;
				else owner = GetModel();
				InsertEntity<IModelObject>(newModelObjects, modelObject, owner);
			}
		}


		private void DoInsertModelObject(IModelObject modelObject, Template template) {
			DoInsertModelObjects(SingleInstanceEnumerator<IModelObject>.Create(modelObject), template);
		}


		private void DoInsertModelObjects(IEnumerable<IModelObject> modelObjects, Template template) {
			AssertCanInsert(modelObjects);
			foreach (IModelObject modelObject in modelObjects)
				InsertEntity<IModelObject>(newModelObjects, modelObject, template);
		}


		private void DoUpdateModelObjectOwner(IModelObject modelObject, IEntity owner) {
			Debug.Assert(owner == null || owner is Model || owner is Template || owner is IModelObject, "Invalid parent type.");
			if (((IEntity)modelObject).Id == null) {
				newModelObjects.Remove(modelObject);
				newModelObjects.Add(modelObject, owner ?? GetModel());
			} else {
				loadedModelObjects[((IEntity)modelObject).Id].Owner = owner ?? GetModel();
				loadedModelObjects[((IEntity)modelObject).Id].State = ItemState.OwnerChanged;
			}
		}


		private void DoUpdateModelObject(IModelObject modelObject) {
			DoUpdateModelObjects(SingleInstanceEnumerator<IModelObject>.Create(modelObject));
		}


		private void DoUpdateModelObjects(IEnumerable<IModelObject> modelObjects) {
			AssertCanUpdate(modelObjects);
			foreach (IModelObject modelObject in modelObjects)
				UpdateEntity<IModelObject>(loadedModelObjects, newModelObjects, modelObject);
		}


		private void DoDeleteModelObject(IModelObject modelObject) {
			DoDeleteModelObjects(SingleInstanceEnumerator<IModelObject>.Create(modelObject));
		}


		private void DoDeleteModelObjects(IEnumerable<IModelObject> modelObjects) {
			AssertCanDelete(modelObjects);
			foreach (IModelObject modelObject in modelObjects)
				DeleteEntity<IModelObject>(loadedModelObjects, newModelObjects, modelObject);
		}


		private void DoUndeleteModelObject(IModelObject modelObject) {
			DoUndeleteModelObjects(SingleInstanceEnumerator<IModelObject>.Create(modelObject));
		}


		private void DoUndeleteModelObjects(IEnumerable<IModelObject> modelObjects) {
			AssertCanUndelete(modelObjects);
			foreach (IModelObject modelObject in modelObjects)
				UndeleteEntity<IModelObject>(loadedModelObjects, modelObject);
		}

		#endregion


		#region [Private] Methods: Consistency checks

		#region Designs and Styles

		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanInsert(Design design) {
			if (!CanInsert(design, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanUpdate(Design design) {
			if (!CanUpdate(design, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanDelete(Design design) {
			if (!CanDelete(design, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanUndelete(Design design) {
			if (!CanUndeleteEntity(design, loadedDesigns, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanInsert(IEnumerable<IStyle> styles) {
			if (!CanInsert(styles, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanUpdate(IStyle style) {
			if (!CanUpdate(style, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanDelete(IEnumerable<IStyle> styles) {
			if (!CanDelete(styles, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanUndelete(IEnumerable<IStyle> styles) {
			foreach (IStyle style in styles)
				if (!CanUndeleteEntity(style, loadedStyles, out reasonText)) throw new CachedRepositoryException(reasonText);
		}

		#endregion


		#region Model and ModelObjects

		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanInsert(Model model) {
			if (!CanInsert(model, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanUpdate(Model model) {
			if (!CanUpdate(model, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanDelete(Model model) {
			if (!CanDelete(model, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanUndelete(Model model) {
			if (!CanUndeleteEntity(model, loadedModels, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanInsert(IEnumerable<IModelObject> modelObjects) {
			if (!CanInsert(modelObjects, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanUpdate(IEnumerable<IModelObject> modelObjects) {
			if (!CanUpdate(modelObjects, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanDelete(IEnumerable<IModelObject> modelObjects) {
			if (!CanDelete(modelObjects, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanUndelete(IEnumerable<IModelObject> modelObjects) {
			foreach (IModelObject modelObject in modelObjects)
				if (!CanUndeleteEntity(modelObject, loadedModelObjects, out reasonText)) throw new CachedRepositoryException(reasonText);
		}

		#endregion


		#region Templates and ModelMappings

		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanInsert(Template template, bool checkContentCanInsert) {
			if (!CanInsert(template, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanUpdate(Template template) {
			if (!CanUpdate(template, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanDelete(Template template) {
			if (!CanDelete(template, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanInsert(IEnumerable<IModelMapping> modelMappings) {
			if (!CanInsert(modelMappings, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanUpdate(IEnumerable<IModelMapping> modelMappings) {
			if (!CanUpdate(modelMappings, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanDelete(IEnumerable<IModelMapping> modelMappings) {
			if (!CanDelete(modelMappings, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanUndelete(IEnumerable<IModelMapping> modelMappings) {
			foreach (IModelMapping modelMapping in modelMappings)
				if (!CanUndeleteEntity(modelMapping, loadedModelMappings, out reasonText)) throw new CachedRepositoryException(reasonText);
		}

		#endregion


		#region Diagram, Shapes and Connections

		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanInsert(Diagram diagram) {
			if (!CanInsert(diagram, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanUpdate(Diagram diagram) {
			if (!CanUpdate(diagram, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanDelete(Diagram diagram) {
			if (!CanDelete(diagram, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanUndelete(Diagram diagram) {
			if (!CanUndeleteEntity(diagram, loadedDiagrams, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanInsert(IEnumerable<Shape> shapes, IEntity parent) {
			if (parent is Diagram) {
				if (!IsExistent((Diagram)parent, newDiagrams, loadedDiagrams)) throw new CachedRepositoryException("Diagram not found in repository.");
			} else if (parent is Template) {
				foreach (Shape s in shapes)
					if (!HasNoTemplate(s)) throw new CachedRepositoryException("Shape or one of its child shapes has a template. Template shapes may not refer to other templates.");
			} else if (parent is Shape) {
				if (!IsExistent((Shape)parent, newShapes, loadedShapes)) throw new CachedRepositoryException("Parent shape not found in repository.");
			}
			if (!CanInsert(shapes, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanUpdate(IEnumerable<Shape> shapes) {
			if (!CanUpdate(shapes, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanUpdate(IEnumerable<Shape> shapes, IEntity owner) {
			if (!CanUpdate(shapes, owner, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanDelete(IEnumerable<Shape> shapes) {
			if (!CanDelete(shapes, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanUndelete(IEnumerable<Shape> shapes, IEntity owner) {
			if (owner is Diagram) {
				if (!IsExistent((Diagram)owner, newDiagrams, loadedDiagrams)) throw new CachedRepositoryException("Diagram does not exist in the repository.");
			} else if (owner is Template) {
				if (!IsExistent((Template)owner, newTemplates, loadedTemplates)) throw new CachedRepositoryException("Template does not exist in the repository.");
			} else if (owner is Shape) {
				if (!IsExistent((Shape)owner, newShapes, loadedShapes)) throw new CachedRepositoryException("Parent shape does not exist in the repository.");
			}
			foreach (Shape shape in shapes) AssertCanUndelete(shape);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanUndelete(Shape shape) {
			if (!CanUndeleteEntity(shape, loadedShapes, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanInsert(Shape activeShape, ControlPointId gluePointId, Shape passiveShape, ControlPointId connectionPointId) {
			if (!CanInsert(activeShape, gluePointId, passiveShape, connectionPointId, out reasonText)) throw new CachedRepositoryException(reasonText);
		}


		[Conditional(REPOSITORY_CHECK_DEFINE)]
		//[Conditional(DEBUG_DEFINE)]
		private void AssertCanDelete(Shape activeShape, ControlPointId gluePointId, Shape passiveShape, ControlPointId connectionPointId) {
			if (!CanDelete(activeShape, gluePointId, passiveShape, connectionPointId, out reasonText)) throw new CachedRepositoryException(reasonText);
		}

		#endregion


		//[Conditional(CONSISTENCY_CHECK_DEFINE)]
		//private void AssertCan () {
		//    if (!Can (, out reasonText)) throw new CachedRepositoryException(reasonText);
		//}


		//[Conditional(CONSISTENCY_CHECK_DEFINE)]
		//private void AssertCan () {
		//    if (!Can (, out reasonText)) throw new CachedRepositoryException(reasonText);
		//}

		#endregion


		#region [Private] Methods: Consistency checks implementation

		private void AssertOpen() {
			if (!isOpen) throw new NShapeException("Repository is not open.");
			Debug.Assert(settings != null && projectDesign != null);
		}


		private void AssertClosed() {
			if (isOpen) throw new NShapeException("Repository is already open.");
		}


		private void AssertStoreExists() {
			if (store == null) throw new NShapeException("There is no store component connected to the repository.");
		}


		// Project
		// ToDo: Implement consistency check methods for project class


		#region Consistency check implementations for Design and Styles

		private bool CanInsert(Design design, out string reason) {
			reason = null;
			CanInsertEntity(design, newDesigns, loadedDesigns, out reason);
			return string.IsNullOrEmpty(reason);
		}


		private bool CanUpdate(Design design, out string reason) {
			reason = null;
			CanUpdateEntity(design, newDesigns, loadedDesigns, out reason);
			return string.IsNullOrEmpty(reason);
		}


		private bool CanDelete(Design design, out string reason) {
			reason = null;
			CanDeleteEntity(design, newDesigns, loadedDesigns, out reason);
			return string.IsNullOrEmpty(reason);
		}


		private bool CanInsert(IEnumerable<IStyle> styles, out string reason) {
			reason = null;
			foreach (IStyle style in styles)
				CanInsertEntity(style, newStyles, loadedStyles, out reason);
			return string.IsNullOrEmpty(reason);
		}


		private bool CanUpdate(IStyle style, out string reason) {
			reason = null;
			CanUpdateEntity(style, newStyles, loadedStyles, out reason);
			return string.IsNullOrEmpty(reason);
		}


		private bool CanDelete(IEnumerable<IStyle> styles, out string reason) {
			reason = null;
			foreach (IStyle style in styles) {
				if (IsStyleInUse(style, styles)) reason = "Style is still in use.";
				else if (!IsExistent(style, newStyles, loadedStyles)) reason = "Style does not exist in the repository.";
				if (!string.IsNullOrEmpty(reason)) break;
			}
			return string.IsNullOrEmpty(reason);
		}


		private bool IsStyleInUse(IStyle style, IEnumerable<IStyle> stylesToDelete) {
			if (style == null) throw new ArgumentNullException("style");
			AssertOpen();
			// First, check Styles and StyleModelMappings as they are always loaded completly,
			// then check new shapes and laoded shapes 
			// Finally, the store has to check all objects that are not loaded:
			//
			// Check Styles
			if (style is IColorStyle) {
				// Styles only have to checked if the given style is a ColorStyle
				foreach (KeyValuePair<IStyle, IEntity> keyValuePair in newStyles)
					if (UsesStyle(style, keyValuePair.Key)) {
						if (Contains(stylesToDelete, keyValuePair.Key)) continue;
						return true;
					}
				// Check all loaded styles if style is used
				foreach (EntityBucket<IStyle> entityBucket in loadedStyles) {
					if (entityBucket.State == ItemState.Deleted) continue;
					if (UsesStyle(style, entityBucket.ObjectRef)) {
						if (Contains(stylesToDelete, entityBucket.ObjectRef)) continue;
						return true;
					}
				}
			}
			// Check StyleModelMappings
			foreach (KeyValuePair<IModelMapping, IEntity> keyValuePair in newModelMappings)
				if (UsesStyle(style, keyValuePair.Key)) return true;
			foreach (EntityBucket<IModelMapping> entityBucket in loadedModelMappings) {
				if (entityBucket.State == ItemState.Deleted) continue;
				if (UsesStyle(style, entityBucket.ObjectRef)) return true;
			}
			// Check all new shapes if style is used
			foreach (KeyValuePair<Shape, IEntity> keyValuePair in newShapes)
				if (keyValuePair.Key.HasStyle(style)) return true;
			// Check all loaded shapes if style is used
			foreach (EntityBucket<Shape> entityBucket in loadedShapes) {
				if (entityBucket.State == ItemState.Deleted) continue;
				if (entityBucket.ObjectRef.HasStyle(style)) return true;
			}
			// If the given style is not a new style, check if it is used in 
			// currently not loaded objects
			if (style.Id != null)
				return store.CheckStyleInUse(this, style.Id);
			return false;
		}

		#endregion


		#region Consistency check implementations for Model and Model Objects

		private bool CanInsert(Model model, out string reason) {
			reason = null;
			CanInsertEntity(model, newModels, loadedModels, out reason);
			return string.IsNullOrEmpty(reason);
		}


		private bool CanUpdate(Model model, out string reason) {
			reason = null;
			CanUpdateEntity(model, newModels, loadedModels, out reason);
			return string.IsNullOrEmpty(reason);
		}


		private bool CanDelete(Model model, out string reason) {
			reason = null;
			CanDeleteEntity(model, newModels, loadedModels, out reason);
			return string.IsNullOrEmpty(reason);
		}


		private bool CanInsert(IEnumerable<IModelObject> modelObjects, out string reason) {
			reason = null;
			foreach (IModelObject modelObject in modelObjects)
				if (!CanInsertEntity(modelObject, newModelObjects, loadedModelObjects, out reason))
					break;
			return string.IsNullOrEmpty(reason);
		}


		private bool CanUpdate(IEnumerable<IModelObject> modelObjects, out string reason) {
			reason = null;
			foreach (IModelObject modelObject in modelObjects)
				CanUpdateEntity(modelObject, newModelObjects, loadedModelObjects, out reason);
			return string.IsNullOrEmpty(reason);
		}


		private bool CanDelete(IEnumerable<IModelObject> modelObjects, out string reason) {
			reason = null;
			foreach (IModelObject modelObject in modelObjects){
			if (CanDeleteEntity(modelObject, newModelObjects, loadedModelObjects, out reason)) {
				// Check if model object is used by shapes
				bool usedByShapes = false;
				foreach (KeyValuePair<Shape, IEntity> item in newShapes) {
					if (item.Key.ModelObject == modelObject) {
						usedByShapes = true;
						break;
					}
				}
				if (!usedByShapes) {
					foreach (EntityBucket<Shape> item in loadedShapes) {
						if (item.State == ItemState.Deleted) continue;
						if (item.ObjectRef.ModelObject == modelObject) {
							usedByShapes = true;
							break;
						}
					}
				}
				if (usedByShapes) reason = "Model object is still used by shapes.";

				if (string.IsNullOrEmpty(reason)) {
					bool usedByModelObjects = false;
					if (newModelObjects.ContainsValue(modelObject))
						usedByModelObjects = true;
					else {
						foreach (EntityBucket<IModelObject> item in loadedModelObjects) {
							if (item.Owner == modelObject && item.State != ItemState.Deleted) {
								usedByModelObjects = true;
								break;
							}
						}
					}
					if (usedByModelObjects) reason = "Model object is still used by model objects";
				}
				if (string.IsNullOrEmpty(reason))
					if (IsModelObjectInUse(modelObject)) reason = "Model object is in use.";
			}
			}
			return string.IsNullOrEmpty(reason);
		}

		#endregion


		#region Consistency check implementations for Templates and ModelMappings

		private bool CanInsert(Template template, out string reason) {
			reason = null;
			CanInsertEntity(template, newTemplates, loadedTemplates, out reason);
			return string.IsNullOrEmpty(reason);
		}
		
		
		private bool CanUpdate(Template template, out string reason) {
			reason = null;
			CanUpdateEntity(template, newTemplates, loadedTemplates, out reason);
			return string.IsNullOrEmpty(reason);
		}


		private bool CanDelete(Template template, out string reason) {
			reason = null;
			if (CanDeleteEntity(template, newTemplates, loadedTemplates, out reason)) {
				// Check if the template shape was deleted
				bool shapeDeleted = true;
				if (newShapes.ContainsValue(template))
					shapeDeleted = false;
				else {
					foreach (EntityBucket<Shape> item in loadedShapes) {
						if (item.Owner == template && item.State != ItemState.Deleted) {
							shapeDeleted = false;
							break;
						}
					}
				}
				if (!shapeDeleted) reason = "Template's shape was not deleted.";

				// Check if model mappings were deleted
				bool modelMappingsDeleted = true;
				if (newModelMappings.ContainsValue(template))
					modelMappingsDeleted = false;
				else {
					foreach (EntityBucket<IModelMapping> item in loadedModelMappings) {
						if (item.Owner == template && item.State != ItemState.Deleted) {
							modelMappingsDeleted = false;
							break;
						}
					}
				}
				if (!modelMappingsDeleted) reason = "Template's model mappings were not deleted.";

				// Check if template is still in use
				if (IsTemplateInUse(template)) reason = "Template is in use.";
			}
			return string.IsNullOrEmpty(reason);
		}


		private bool CanInsert(IEnumerable<IModelMapping> modelMappings, out string reason) {
			reason = null;
			foreach (IModelMapping modelMapping in modelMappings)
				if (!CanInsertEntity(modelMapping, newModelMappings, loadedModelMappings, out reason))
					break;
			return string.IsNullOrEmpty(reason);
		}


		private bool CanUpdate(IEnumerable<IModelMapping> modelMappings, out string reason) {
			reason = null;
			foreach (IModelMapping modelMapping in modelMappings)
				if (!CanUpdateEntity(modelMapping, newModelMappings, loadedModelMappings, out reason))
					break;
			if (string.IsNullOrEmpty(reason))
				OwnerIsValid(modelMappings, out reason);
			return string.IsNullOrEmpty(reason);
		}


		private bool CanDelete(IEnumerable<IModelMapping> modelMappings, out string reason) {
			reason = null;
			foreach (IModelMapping modelMapping in modelMappings)
				if (!CanDeleteEntity(modelMapping, newModelMappings, loadedModelMappings, out reason))
					break;
			if (string.IsNullOrEmpty(reason))
				OwnerIsValid(modelMappings, out reason);
			return string.IsNullOrEmpty(reason);
		}


		private bool OwnerIsValid(IEnumerable<IModelMapping> modelMappings, out string reason) {
			reason = null;
			Template owner = null;
			foreach (IModelMapping modelMapping in modelMappings) {
				if (owner == null) owner = GetModelMappingOwner(modelMapping);
				else if (owner != GetModelMappingOwner(modelMapping)) {
					reason = "Not all model mappings are part of the same template.";
					break;
				}
			}
			return string.IsNullOrEmpty(reason);
		}

		#endregion


		#region Consistency check implementations for Diagram, Shapes and Connections

		private bool CanInsert(Diagram diagram, out string reason) {
			reason = null;
			CanInsertEntity(diagram, newDiagrams, loadedDiagrams, out reason);
			return string.IsNullOrEmpty(reason);
		}
		

		private bool CanUpdate(Diagram diagram, out string reason) {
			reason = null;
			CanUpdateEntity(diagram, newDiagrams, loadedDiagrams, out reason);
			return string.IsNullOrEmpty(reason);
		}


		private bool CanDelete(Diagram diagram, out string reason) {
			reason = null;

			// Check if the diagram's shapes were deleted
			bool shapesDeleted = true;
			if (newShapes.ContainsValue(diagram))
				shapesDeleted = false;
			else {
				foreach (EntityBucket<Shape> item in loadedShapes) {
					if (item.Owner == diagram && item.State != ItemState.Deleted) {
						shapesDeleted = false;
						break;
					}
				}
			}
			if (!shapesDeleted) reason = "The diagram's shapes are not deleted.";

			//if (diagram.Shapes.Count > 0)
			//    reason = string.Format("Diagram still contains {0} shapes", diagram.Shapes.Count);
			return string.IsNullOrEmpty(reason);
		}


		private bool CanInsert(IEnumerable<Shape> shapes, out string reason) {
			reason = null;
			if (IsConnectedToOtherShapes(shapes, shapes, true))
				reason = "Shape is connected to shapes that are not part of the repository.";
			if (!string.IsNullOrEmpty(reason)) {
				foreach (Shape shape in shapes) {
					if (IsExistent(shape, newShapes, loadedShapes)) {
						reason = "Shape already exists in the repository.";
						break;
					}
					if (shape.ModelObject != null) {
						if (!IsExistent(shape.ModelObject, newModelObjects, loadedModelObjects)) {
							reason = "Shape has a model object that does not exist in the repository.";
							break;
						}
					}
				}
			}
			return string.IsNullOrEmpty(reason);
		}


		private bool CanUpdate(IEnumerable<Shape> shapes, out string reason) {
			reason = null;
			foreach (Shape shape in shapes)
				if (!CanUpdate(shape, out reason)) 
					break;
			return string.IsNullOrEmpty(reason);
		}


		private bool CanUpdate(IEnumerable<Shape> shapes, IEntity owner, out string reason) {
			reason = null;
			foreach (Shape shape in shapes) {
				if (CanUpdate(shape, out reason)) {
					if (IsOwner(shape, owner))
						reason = string.Format("Shape is already owned by {0}.", owner.GetType().Name);
					if (owner is Template) {
						if (!HasNoTemplate(shape))
							reason = "Shape or one of its child shapes has a template. Template shapes may not refer to other templates.";
					}
				}
				if (!string.IsNullOrEmpty(reason)) break;
			}
			return string.IsNullOrEmpty(reason);
		}


		private bool CanUpdate(Shape shape, out string reason) {
			reason = null;
			if (!IsExistent(shape, newShapes, loadedShapes))
				reason = "Shape does not exist in the repository or is already deleted.";
			if (shape.Children.Count > 0) {
				foreach (Shape childShape in GetShapes(shape.Children, true))
					if (!IsExistent(childShape, newShapes, loadedShapes)) {
						reason = "Shape's child shapes either do not exist, are deleted or assigned to the wrong parent in the repository.";
						break;
					}
			}
			return string.IsNullOrEmpty(reason);
		}


		private bool CanDelete(IEnumerable<Shape> shapes, out string reason) {
			reason = null;
			// ToDo: Optimize this
			IEnumerable<Shape> allShapes = GetShapes(shapes, true);
			// Check if shape has children that are not deleted
			if (HasOtherChildren(shapes, allShapes))
				reason = "Shape has still children that were not deleted.";
			// Check shape has connections to other shapes
			if (IsConnectedToOtherShapes(shapes, allShapes, false))
				reason = "Shape has still connections to other shapes.";
			foreach (Shape shape in shapes) {
				if (shape.ModelObject != null) {
					foreach (Shape s in shape.ModelObject.Shapes) {
						if (shape == s) {
							reason = "Shape is still attached to a model object.";
							break;
						}
					}
				}
			}

			//if (shape.Diagram != null) 
			//    reason = string.Format("Shape was not removed from diagram '{0}'.", shape.Diagram);
			//if (shape.Template != null)
			//    reason = string.Format("Shape was not removed from template '{0}'.", shape.Template);
			//if (shape.Parent != null)
			//    reason = "Shape was not removed from its parent.";
			//if (shape.Children.Count > 0)
			//    reason = string.Format("Shape has still {0} children.", shape.Children.Count);
			//if (shape.IsConnected(ControlPointId.Any, null) != ControlPointId.None)
			//    reason = "Shape is still connected to other shapes.";

			return string.IsNullOrEmpty(reason);
		}


		private bool IsOwner(Shape shape, IEntity owner) {
			bool isOwner = false;
			if (newShapes.ContainsKey(shape))
				isOwner = (newShapes[shape] == owner);
			else {
				object id = ((IEntity)shape).Id;
				if (loadedShapes[id].Owner == owner && loadedShapes[id].State != ItemState.Deleted)
					isOwner = true;
			}
			return isOwner;
		}


		private bool HasChildren(IEnumerable<Shape> shapesToCheck) {
			return HasOtherChildren(shapesToCheck, null);
		}


		private bool HasOtherChildren(IEnumerable<Shape> shapesToCheck, IEnumerable<Shape> allShapes) {
			// Check if shape has children that are not deleted
			bool hasChildren = false;
			foreach (Shape shape in shapesToCheck) {
				if (shape.Children.Count == 0) continue;
				foreach (Shape childShape in shape.Children) {
					if (allShapes != null && IsShapeInCollection(childShape, allShapes)) continue;
					if (IsExistent(childShape, newShapes, loadedShapes)) {
						hasChildren = true;
						break;
					}
				}
			}
			return hasChildren;
		}


		private bool IsConnectedToOtherShapes(IEnumerable<Shape> shapes, IEnumerable<Shape> allShapes, bool allowConnectionsToRepositoryMembers) {
			// Search for connections with shapes not contained in the given collection
			foreach (Shape shape in shapes) {
				foreach (ShapeConnectionInfo ci in shape.GetConnectionInfos(ControlPointId.Any, null)) {
					if (!shape.HasControlPointCapability(ci.OwnPointId, ControlPointCapabilities.Glue)) continue;
					if (allowConnectionsToRepositoryMembers && IsShapeInRepository(ci.OtherShape)) continue;
					if (IsShapeInCollection(ci.OtherShape, allShapes)) continue;
					return true;
				}
			}
			return false;
		}


		private bool HasNoTemplate(Shape shape) {
			Debug.Assert(shape != null);
			bool result = true;
			if (shape.Template != null) result = false;
			foreach (Shape childShape in shape.Children)
				if (!HasNoTemplate(childShape)) result = false;
			return result;
		}


		private bool CanInsert(Shape activeShape, ControlPointId gluePointId, Shape passiveShape, ControlPointId connectionPointId, out string reason) {
			reason = null;
			if (!IsExistent(activeShape, newShapes, loadedShapes)) reason = "Active shape is not in the repository.";
			if (!IsExistent(passiveShape, newShapes, loadedShapes)) reason = "Passive shape is not in the repository.";
			if (!activeShape.HasControlPointCapability(gluePointId, ControlPointCapabilities.Glue)) reason = string.Format("Active shape's point {0} is not a glue point.", gluePointId);
			if (!passiveShape.HasControlPointCapability(connectionPointId, ControlPointCapabilities.Connect)) reason = string.Format("Passive shape's point {0} is not a connection point.", connectionPointId);
			return string.IsNullOrEmpty(reason);
		}


		private bool CanDelete(Shape activeShape, ControlPointId gluePointId, Shape passiveShape, ControlPointId connectionPointId, out string reason) {
			reason = null;
			if (!IsExistent(activeShape, newShapes, loadedShapes)) reason = "Active shape is not in the repository.";
			if (!IsExistent(passiveShape, newShapes, loadedShapes)) reason = "Passive shape is not in the repository.";
			if (!activeShape.HasControlPointCapability(gluePointId, ControlPointCapabilities.Glue)) reason = string.Format("Active shape's point {0} is not a glue point.", gluePointId);
			if (!passiveShape.HasControlPointCapability(connectionPointId, ControlPointCapabilities.Connect)) reason = string.Format("Passive shape's point {0} is not a connection point.", connectionPointId);
			return string.IsNullOrEmpty(reason);
		}

		#endregion


		private bool CanInsertEntity<TEntity>(TEntity entity, IDictionary<TEntity, IEntity> newEntities, LoadedEntities<TEntity> loadedEntities, out string reason)
			where TEntity : IEntity {
			reason=null;
			if (entity.Id != null) 
				reason = string.Format("{0} '{1}' already has an Id assigned. Entities with Id cannot be inserted.", entity.GetType().Name, entity);
			else if (IsExistent(entity, newEntities, loadedEntities)) 
				reason = string.Format("{0} '{1}' already exists in the repository.", entity.GetType().Name, entity);
			return string.IsNullOrEmpty(reason);
		}


		private bool CanUpdateEntity<TEntity>(TEntity entity, IDictionary<TEntity, IEntity> newEntities, LoadedEntities<TEntity> loadedEntities, out string reason)
			where TEntity : IEntity {
			reason = null;
			if (!IsExistent(entity, newEntities, loadedEntities))
				reason = string.Format("{0} '{1}' does not exist in the repository or is already deleted.", entity.GetType().Name, entity);
			return string.IsNullOrEmpty(reason);
		}


		private bool CanDeleteEntity<TEntity>(TEntity entity, IDictionary<TEntity, IEntity> newEntities, LoadedEntities<TEntity> loadedEntities, out string reason)
			where TEntity : IEntity {
			reason = null;
			if (!IsExistent(entity, newEntities, loadedEntities))
				reason = string.Format("{0} '{1}' does not exist in the repository or is already deleted.", entity.GetType().Name, entity);
			return string.IsNullOrEmpty(reason);
		}


		private bool CanUndeleteEntity<TEntity>(TEntity entity, LoadedEntities<TEntity> loadedEntities, out string reason) where TEntity : IEntity {
			reason = null;
			if (!CanUndeleteEntity(entity, loadedEntities)) {
				reason = "Shape cannot be undeleted. It was not loaded from a store or it was not deleted.";
				return false;
			} else return true;
		}


		private bool CanUndeleteEntity<TEntity>(TEntity entity, LoadedEntities<TEntity> loadedEntities) where TEntity : IEntity {
			if (entity == null) throw new ArgumentNullException("entity");
			if (loadedEntities == null) throw new ArgumentNullException("loadedEntities");
			EntityBucket<TEntity> item = null;
			if (entity.Id != null && loadedEntities.TryGetValue(entity.Id, out item)) {
				return (item.State == ItemState.Deleted);
			} else return false;
		}

		
		private bool IsExistent<TEntity>(TEntity entity, IDictionary<TEntity, IEntity> newEntities, LoadedEntities<TEntity> loadedEntities) 
			where TEntity : IEntity {
			bool isExistent = false;
			if (entity.Id == null)
				isExistent = newEntities.ContainsKey(entity);
			else if (loadedEntities.ContainsKey(entity.Id))
				isExistent = (loadedEntities[entity.Id].State != ItemState.Deleted);
			return isExistent;
		}

		#endregion


		#region [Private] Methods for retrieving EventArgs

		private RepositoryProjectEventArgs GetProjectEventArgs(ProjectSettings projectData) {
			projectEventArgs.Project = projectData;
			return projectEventArgs;
		}


		private RepositoryModelEventArgs GetModelEventArgs(Model model) {
			modelEventArgs.Model = model;
			return modelEventArgs;
		}


		private RepositoryDesignEventArgs GetDesignEventArgs(Design design) {
			designEventArgs.Design = design;
			return designEventArgs;
		}


		private RepositoryStyleEventArgs GetStyleEventArgs(IStyle style) {
			styleEventArgs.Style = style;
			return styleEventArgs;
		}


		private RepositoryDiagramEventArgs GetDiagramEventArgs(Diagram diagram) {
			diagramEventArgs.Diagram = diagram;
			return diagramEventArgs;
		}


		private RepositoryTemplateEventArgs GetTemplateEventArgs(Template template) {
			templateEventArgs.Template = template;
			return templateEventArgs;
		}


		private RepositoryTemplateShapeReplacedEventArgs GetTemplateShapeExchangedEventArgs(Template template, Shape oldTemplateShape, Shape newTemplateShape) {
			templateShapeExchangedEventArgs.Template = template;
			templateShapeExchangedEventArgs.OldTemplateShape = oldTemplateShape;
			templateShapeExchangedEventArgs.NewTemplateShape = newTemplateShape;
			return templateShapeExchangedEventArgs;
		}


		private RepositoryShapesEventArgs GetShapesEventArgs(Shape shape) {
			Diagram diagram;
			if (((IEntity)shape).Id == null)
				diagram = newShapes[shape] as Diagram;
			else diagram = loadedShapes[((IEntity)shape).Id].Owner as Diagram;
			shapeEventArgs.SetShape(shape, diagram);
			return shapeEventArgs;
		}


		private RepositoryShapesEventArgs GetShapesEventArgs(Shape shape, Diagram diagram) {
			shapeEventArgs.SetShape(shape, diagram);
			return shapeEventArgs;
		}


		private RepositoryShapesEventArgs GetShapesEventArgs(IEnumerable<Shape> shapes, Diagram diagram) {
			shapeEventArgs.SetShapes(shapes, diagram);
			return shapeEventArgs;
		}


		private RepositoryShapesEventArgs GetShapesEventArgs(IEnumerable<Shape> shapes) {
			shapeEventArgs.Clear();
			foreach (Shape shape in shapes) {
				Diagram diagram = null;
				if (((IEntity)shape).Id == null) {
					if (newShapes.ContainsKey(shape))
						diagram = newShapes[shape] as Diagram;
				} else if (loadedShapes.ContainsKey(((IEntity)shape).Id))
					diagram = loadedShapes[((IEntity)shape).Id].Owner as Diagram;
				shapeEventArgs.AddShape(shape, diagram);
			}
			return shapeEventArgs;
		}


		private RepositoryModelObjectsEventArgs GetModelObjectsEventArgs(IModelObject modelObject) {
			modelObjectEventArgs.SetModelObject(modelObject);
			return modelObjectEventArgs;
		}


		private RepositoryModelObjectsEventArgs GetModelObjectsEventArgs(IEnumerable<IModelObject> modelObjects) {
			modelObjectEventArgs.SetModelObjects(modelObjects);
			return modelObjectEventArgs;
		}


		private RepositoryShapeConnectionEventArgs GetShapeConnectionEventArgs(ShapeConnection connection) {
			shapeConnectionEventArgs.SetShapeConnection(connection);
			return shapeConnectionEventArgs;
		}

		#endregion


		#region [Private] Constants and Fields

		private const string REPOSITORY_CHECK_DEFINE = "REPOSITORY_CHECK";
		private const string DEBUG_DEFINE = "DEBUG";

		// project info is an internal entity type
		private const string projectInfoEntityTypeName = "ProjectInfo";

		// Used to calculate the element entityTypeName
		static private StringBuilder stringBuilder = new StringBuilder();

		/// <summary>
		/// True, when modfications have been done to any part of the projects since
		/// Open or SaveChanges. 
		/// </summary>
		private bool isModified;

		// True, when Open was successfully called. Is identical to store.IsOpen if store 
		// is defined.
		private bool isOpen;

		/// <summary>
		/// Reference to the open project for easier access.
		/// </summary>
		private ProjectSettings settings;

		/// <summary>
		/// Indicates the pseudo design used to manage the styles of the project.
		/// This design is not entered in the designs or newDesigns dictionaries.
		/// </summary>
		private Design projectDesign;

		private int version;

		// Name of the project
		private string projectName = string.Empty;

		// Store for cache data. Is null, if no store is assigned to open
		// cache, i.e. the cache is in-memory.
		private Store store;

		// project needs an owner for the newProjects dictionary
		private ProjectOwner projectOwner = new ProjectOwner();

		// Buffers
		private string reasonText;

		// DirectoryName of registered entities
		private Dictionary<string, IEntityType> entityTypes = new Dictionary<string, IEntityType>();

		// Containers for loaded objects
		private LoadedEntities<ProjectSettings> loadedProjects = new LoadedEntities<ProjectSettings>();
		private LoadedEntities<Model> loadedModels = new LoadedEntities<Model>();
		private LoadedEntities<Design> loadedDesigns = new LoadedEntities<Design>();
		private LoadedEntities<IStyle> loadedStyles = new LoadedEntities<IStyle>();
		private LoadedEntities<Diagram> loadedDiagrams = new LoadedEntities<Diagram>();
		private LoadedEntities<Template> loadedTemplates = new LoadedEntities<Template>();
		private LoadedEntities<IModelMapping> loadedModelMappings = new LoadedEntities<IModelMapping>();
		private LoadedEntities<Shape> loadedShapes = new LoadedEntities<Shape>();
		private LoadedEntities<IModelObject> loadedModelObjects = new LoadedEntities<IModelObject>();

		// Containers for new entities
		// Stores the new entity as the key and its parent as the value.
		// (New objects do not yet have an id and are therefore not addressable in the dictionary.)
		private Dictionary<ProjectSettings, IEntity> newProjects = new Dictionary<ProjectSettings, IEntity>();
		private Dictionary<Model, IEntity> newModels = new Dictionary<Model, IEntity>();
		private Dictionary<Design, IEntity> newDesigns = new Dictionary<Design, IEntity>();
		private Dictionary<IStyle, IEntity> newStyles = new Dictionary<IStyle, IEntity>();
		private Dictionary<Diagram, IEntity> newDiagrams = new Dictionary<Diagram, IEntity>();
		private Dictionary<Template, IEntity> newTemplates = new Dictionary<Template, IEntity>();
		private Dictionary<IModelMapping, IEntity> newModelMappings = new Dictionary<IModelMapping, IEntity>();
		private Dictionary<Shape, IEntity> newShapes = new Dictionary<Shape, IEntity>();
		private Dictionary<IModelObject, IEntity> newModelObjects = new Dictionary<IModelObject, IEntity>();
		private List<ShapeConnection> newShapeConnections = new List<ShapeConnection>();
		private List<ShapeConnection> deletedShapeConnections = new List<ShapeConnection>();

		// EventArg Buffers
		private RepositoryProjectEventArgs projectEventArgs = new RepositoryProjectEventArgs();
		private RepositoryModelEventArgs modelEventArgs = new RepositoryModelEventArgs();
		private RepositoryDesignEventArgs designEventArgs = new RepositoryDesignEventArgs();
		private RepositoryStyleEventArgs styleEventArgs = new RepositoryStyleEventArgs();
		private RepositoryDiagramEventArgs diagramEventArgs = new RepositoryDiagramEventArgs();
		private RepositoryTemplateEventArgs templateEventArgs = new RepositoryTemplateEventArgs();
		private RepositoryTemplateShapeReplacedEventArgs templateShapeExchangedEventArgs = new RepositoryTemplateShapeReplacedEventArgs();
		private RepositoryShapesEventArgs shapeEventArgs = new RepositoryShapesEventArgs();
		private RepositoryShapeConnectionEventArgs shapeConnectionEventArgs = new RepositoryShapeConnectionEventArgs();
		private RepositoryModelObjectsEventArgs modelObjectEventArgs = new RepositoryModelObjectsEventArgs();

		#endregion
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class CachedRepositoryException : NShapeInternalException {

		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.NShapeInternalException" />.
		/// </summary>
		public CachedRepositoryException(string message)
			: base(message) {
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.NShapeInternalException" />.
		/// </summary>
		public CachedRepositoryException(string message, Exception innerException)
			: base(message, innerException) {
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.NShapeInternalException" />.
		/// </summary>
		public CachedRepositoryException(string format, params object[] args)
			: base(string.Format(format, args), (Exception)null) {
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.NShapeInternalException" />.
		/// </summary>
		public CachedRepositoryException(string format, Exception innerException, params object[] args)
			: base(string.Format(format, args), innerException) {
		}


		/// <summary>
		/// A constructor is needed for serialization when an exception propagates from a remoting server to the client. 
		/// </summary>
		protected CachedRepositoryException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
	}


	#region EntityBucket<TObject> Class

	/// <summary>
	/// Specifies the state of a persistent entity stored in a <see cref="T:Dataweb.NShape.Advanced.CachedRepository" />
	/// </summary>
	public enum ItemState {
		/// <summary>The entity was not modified.</summary>
		Original,
		/// <summary>The entity was modified and not yet saved.</summary>
		Modified,
		/// <summary>The owner of the entity changed.</summary>
		OwnerChanged,
		/// <summary>The entity was deleted from the repository but not yet saved.</summary>
		Deleted,
		/// <summary>The entity is new.</summary>
		New
	};


	// EntityBucket is a reference type, because it is entered into dictionaries.
	// Modifying a field of a value type in a dictionary is not possible during
	// an enumeration, but we have to modify at least the State.
	/// <summary>
	/// Stores a reference to a loaded object together with its state.
	/// </summary>
	public class EntityBucket<TObject> {

		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.EntityBucket`1" />
		/// </summary>
		public EntityBucket(TObject obj, IEntity owner, ItemState state) {
			this.ObjectRef = obj;
			this.Owner = owner;
			this.State = state;
		}

		/// <summary>
		/// Gets the object stored in the <see cref="T:Dataweb.NShape.Advanced.EntityBucket`1" />
		/// </summary>
		public TObject ObjectRef;

		/// <summary>
		/// Gets the owner <see cref="T:Dataweb.NShape.Advanced.IEntity" />.
		/// </summary>
		public IEntity Owner;

		/// <summary>
		/// Gets the <see cref="T:Dataweb.NShape.Advanced.ItemState" />.
		/// </summary>
		public ItemState State;
	}

	#endregion


	#region ShapeConnection Struct

	/// <ToBeCompleted></ToBeCompleted>
	public struct ShapeConnection : IEquatable<ShapeConnection> {

		/// <ToBeCompleted></ToBeCompleted>
		public static bool operator ==(ShapeConnection x, ShapeConnection y) {
			return (
				x.ConnectorShape == y.ConnectorShape
				&& x.TargetShape == y.TargetShape
				&& x.GluePointId == y.GluePointId
				&& x.TargetPointId == y.TargetPointId);
		}

		/// <ToBeCompleted></ToBeCompleted>
		public static bool operator !=(ShapeConnection x, ShapeConnection y) { return !(x == y); }

		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.ShapeConnection" />.
		/// </summary>
		public ShapeConnection(Diagram diagram, Shape connectorShape, ControlPointId gluePointId, Shape targetShape, ControlPointId targetPointId) {
			this.ConnectorShape = connectorShape;
			this.GluePointId = gluePointId;
			this.TargetShape = targetShape;
			this.TargetPointId = targetPointId;
		}

		/// <override></override>
		public override bool Equals(object obj) {
			return obj is ShapeConnection && this == (ShapeConnection)obj;
		}

		/// <override></override>
		public bool Equals(ShapeConnection other) {
			return other == this;
		}

		/// <override></override>
		public override int GetHashCode() {
			int result = GluePointId.GetHashCode() ^ TargetPointId.GetHashCode();
			if (ConnectorShape != null) result ^= ConnectorShape.GetHashCode();
			if (TargetShape != null) result ^= TargetShape.GetHashCode();
			return result;
		}

		/// <ToBeCompleted></ToBeCompleted>
		public static readonly ShapeConnection Empty;

		/// <ToBeCompleted></ToBeCompleted>
		public Shape ConnectorShape;

		/// <ToBeCompleted></ToBeCompleted>
		public ControlPointId GluePointId;

		/// <ToBeCompleted></ToBeCompleted>
		public Shape TargetShape;

		/// <ToBeCompleted></ToBeCompleted>
		public ControlPointId TargetPointId;


		static ShapeConnection() {
			Empty.ConnectorShape = null;
			Empty.GluePointId = ControlPointId.None;
			Empty.TargetShape = null;
			Empty.TargetPointId = ControlPointId.None;
		}
	}

	#endregion


	#region Delegates

	/// <summary>
	/// Defines a filter function for the loading methods.
	/// </summary>
	public delegate bool FilterDelegate<TEntity>(TEntity entity, IEntity owner);


	/// <summary>
	/// Retrieves the entity with the given id.
	/// </summary>
	/// <param name="pid"></param>
	/// <returns></returns>
	public delegate IEntity Resolver(object pid);

	#endregion


	#region RepositoryReader Class

	/// <summary>
	/// Cache reader for the cached cache.
	/// </summary>
	public abstract class RepositoryReader : IRepositoryReader, IDisposable {

		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.RepositoryReader" />.
		/// </summary>
		protected RepositoryReader(IStoreCache cache) {
			if (cache == null) throw new ArgumentNullException("cache");
			this.cache = cache;
		}


		#region [Public] IRepositoryReader Members

		/// <summary>
		/// Fetches the next set of inner objects and prepares them for reading.
		/// </summary>
		public abstract void BeginReadInnerObjects();


		/// <summary>
		/// Finishes reading the current set of inner objects.
		/// </summary>
		public abstract void EndReadInnerObjects();


		/// <summary>
		/// Fetches the next inner object in a set of inner object.
		/// </summary>
		public bool BeginReadInnerObject() {
			if (innerObjectsReader == null)
				return DoBeginObject();
			else return innerObjectsReader.BeginReadInnerObject();
		}


		/// <summary>
		/// Finishes reading an inner object.
		/// </summary>
		public abstract void EndReadInnerObject();


		/// <summary>
		/// Reads a boolean value from the data source.
		/// </summary>
		public bool ReadBool() {
			if (innerObjectsReader == null) {
				++PropertyIndex;
				ValidatePropertyIndex();
				return DoReadBool();
			} else return innerObjectsReader.ReadBool();
		}


		/// <summary>
		/// Reads a byte value from the data source.
		/// </summary>
		public byte ReadByte() {
			if (innerObjectsReader == null) {
				++PropertyIndex;
				ValidatePropertyIndex();
				return DoReadByte();
			} else return innerObjectsReader.ReadByte();
		}


		/// <summary>
		/// Reads a 16 bit integer value from the data source.
		/// </summary>
		/// <returns></returns>
		public short ReadInt16() {
			if (innerObjectsReader == null) {
				++PropertyIndex;
				ValidatePropertyIndex();
				return DoReadInt16();
			} else return innerObjectsReader.ReadInt16();
		}


		/// <summary>
		/// Reads a 32 bit integer value from the data source.
		/// </summary>
		/// <returns></returns>
		public int ReadInt32() {
			if (innerObjectsReader == null) {
				++PropertyIndex;
				ValidatePropertyIndex();
				return DoReadInt32();
			} else return innerObjectsReader.ReadInt32();
		}


		/// <summary>
		/// Reads a 64 bit integer value from the data source.
		/// </summary>
		/// <returns></returns>
		public long ReadInt64() {
			if (innerObjectsReader == null) {
				++PropertyIndex;
				ValidatePropertyIndex();
				return DoReadInt64();
			} else return innerObjectsReader.ReadInt64();
		}


		/// <summary>
		/// Reads a single precision floating point number from the data source.
		/// </summary>
		/// <returns></returns>
		public float ReadFloat() {
			if (innerObjectsReader == null) {
				++PropertyIndex;
				ValidatePropertyIndex();
				return DoReadFloat();
			} else return innerObjectsReader.ReadFloat();
		}


		/// <summary>
		/// Reads a double precision floating point number from the data source.
		/// </summary>
		/// <returns></returns>
		public double ReadDouble() {
			if (innerObjectsReader == null) {
				++PropertyIndex;
				ValidatePropertyIndex();
				return DoReadDouble();
			} else return innerObjectsReader.ReadDouble();
		}


		/// <summary>
		/// Reads a character value.
		/// </summary>
		public char ReadChar() {
			if (innerObjectsReader == null) {
				++PropertyIndex;
				ValidatePropertyIndex();
				return DoReadChar();
			} else return innerObjectsReader.ReadChar();
		}


		/// <summary>
		/// Reads a string value from the data source.
		/// </summary>
		public string ReadString() {
			if (innerObjectsReader == null) {
				++PropertyIndex;
				ValidatePropertyIndex();
				return DoReadString();
			} else return innerObjectsReader.ReadString();
		}


		/// <summary>
		/// Reads a date and time value from the data source.
		/// </summary>
		public DateTime ReadDate() {
			if (innerObjectsReader == null) {
				++PropertyIndex;
				ValidatePropertyIndex();
				return DoReadDate();
			} else return innerObjectsReader.ReadDate();
		}


		/// <summary>
		/// Reads an image value from the data source.
		/// </summary>
		public System.Drawing.Image ReadImage() {
			if (innerObjectsReader == null) {
				++PropertyIndex;
				ValidatePropertyIndex();
				return DoReadImage();
			} else return innerObjectsReader.ReadImage();
		}


		/// <summary>
		/// Reads a template from the data source.
		/// </summary>
		public Template ReadTemplate() {
			object id = ReadId();
			if (id == null) return null;
			else return Cache.GetTemplate(id);
		}


		/// <summary>
		/// Reads a shape from the data source.
		/// </summary>
		/// <returns></returns>
		public Shape ReadShape() {
			object id = ReadId();
			if (id == null) return null;
			else return Cache.GetShape(id);
		}


		/// <summary>
		/// Reads a model object from the data source.
		/// </summary>
		public IModelObject ReadModelObject() {
			object id = ReadId();
			if (id == null) return null;
			else return Cache.GetModelObject(id);
		}


		/// <summary>
		/// Reads a design from the data source.
		/// </summary>
		public Design ReadDesign() {
			object id = ReadId();
			if (id == null) return null;
			else return Cache.GetDesign(id);
		}


		/// <summary>
		/// Reads a cap style from the data source.
		/// </summary>
		public ICapStyle ReadCapStyle() {
			if (innerObjectsReader == null) {
				object id = ReadId();
				if (id == null) return null;
				return (ICapStyle)cache.GetProjectStyle(id);
			} else return innerObjectsReader.ReadCapStyle();
		}


		/// <summary>
		/// Reads a character style from the data source.
		/// </summary>
		public ICharacterStyle ReadCharacterStyle() {
			if (innerObjectsReader == null) {
				object id = ReadId();
				if (id == null) return null;
				return (ICharacterStyle)cache.GetProjectStyle(id);
			} else return innerObjectsReader.ReadCharacterStyle();
		}


		/// <summary>
		/// Reads a color style from the data source.
		/// </summary>
		public IColorStyle ReadColorStyle() {
			if (innerObjectsReader == null) {
				object id = ReadId();
				if (id == null) return null;
				return (IColorStyle)cache.GetProjectStyle(id);
			} else return innerObjectsReader.ReadColorStyle();
		}


		/// <summary>
		/// Reads a fill style from the data source.
		/// </summary>
		public IFillStyle ReadFillStyle() {
			if (innerObjectsReader == null) {
				object id = ReadId();
				if (id == null) return null;
				return (IFillStyle)cache.GetProjectStyle(id);
			} else return innerObjectsReader.ReadFillStyle();
		}


		/// <summary>
		/// Reads a line style from the data source.
		/// </summary>
		public ILineStyle ReadLineStyle() {
			if (innerObjectsReader == null) {
				object id = ReadId();
				if (id == null) return null;
				IStyle style = cache.GetProjectStyle(id);
				Debug.Assert(style is ILineStyle, string.Format("Style {0} is not a line style.", id));
				return (ILineStyle)style;
			} else return innerObjectsReader.ReadLineStyle();
		}


		/// <summary>
		/// Reads a paragraph stylefrom the data source.
		/// </summary>
		public IParagraphStyle ReadParagraphStyle() {
			if (innerObjectsReader == null) {
				object id = ReadId();
				if (id == null) return null;
				return (IParagraphStyle)cache.GetProjectStyle(id);
			} else return innerObjectsReader.ReadParagraphStyle();
		}

		#endregion


		#region [Public] IDisposable Members

		/// <summary>
		/// Releases all allocated unmanaged or persistent resources.
		/// </summary>
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion


		#region [Protected] Properties

		/// <summary>
		/// Indicates the current index in the list of property info of the entity type.
		/// </summary>
		protected internal int PropertyIndex {
			get { return propertyIndex; }
			set { propertyIndex = value; }
		}


		/// <summary>
		/// The IStoreCache that contains the data to read.
		/// </summary>
		protected IStoreCache Cache {
			get { return cache; }
		}


		/// <summary>
		/// A read only collection of property info of the entity type to read.
		/// </summary>
		protected IEnumerable<EntityPropertyDefinition> PropertyInfos {
			get { return propertyInfos; }
		}


		/// <summary>
		/// When reading inner objects, this property stores the owner entity of the inner objects. Otherwise, this property is null/Nothing.
		/// </summary>
		protected IEntity Object {
			get { return entity; }
			set { entity = value; }
		}

		#endregion


		#region [Protected] Methods: Implementation

		/// <summary>
		/// Implementation of reading an id value. Reads an id or null, if no id exists.
		/// </summary>
		protected internal abstract object ReadId();


		/// <summary>
		/// Resets the repositoryReader for a sequence of reads of entities of the same type.
		/// </summary>
		internal virtual void ResetFieldReading(IEnumerable<EntityPropertyDefinition> propertyInfos) {
			if (propertyInfos == null) throw new ArgumentNullException("propertyInfos");
			this.propertyInfos.Clear();
			this.propertyInfos.AddRange(propertyInfos);
			propertyIndex = int.MinValue;
		}


		/// <summary>
		/// Advances to the next object and prepares reading it.
		/// </summary>
		protected internal abstract bool DoBeginObject();


		/// <summary>
		/// Finishes reading an object.
		/// </summary>
		protected internal abstract void DoEndObject();


		/// <summary>
		/// Implementation of reading a boolean value.
		/// </summary>
		protected abstract bool DoReadBool();


		/// <summary>
		/// Implementation of reading a byte value.
		/// </summary>
		protected abstract byte DoReadByte();


		/// <summary>
		/// Implementation of reading a 16 bit integer number.
		/// </summary>
		protected abstract short DoReadInt16();


		/// <summary>
		/// Implementation of reading a 32 bit integer number.
		/// </summary>
		protected abstract int DoReadInt32();


		/// <summary>
		/// Implementation of reading a 64 bit integer number.
		/// </summary>
		protected abstract long DoReadInt64();


		/// <summary>
		/// Implementation of reading a single precision floating point number.
		/// </summary>
		protected abstract float DoReadFloat();


		/// <summary>
		/// Implementation of reading a double precision floating point number.
		/// </summary>
		protected abstract double DoReadDouble();


		/// <summary>
		/// Implementation of reading a character value.
		/// </summary>
		protected abstract char DoReadChar();


		/// <summary>
		/// Implementation of reading a string value.
		/// </summary>
		/// <returns></returns>
		protected abstract string DoReadString();


		/// <summary>
		/// Implementation of reading a date and time value.
		/// </summary>
		protected abstract DateTime DoReadDate();


		/// <summary>
		/// Implementation of reading an image.
		/// </summary>
		protected abstract System.Drawing.Image DoReadImage();


		/// <summary>
		/// Implementation of reading a template.
		/// </summary>
		protected Template DoReadTemplate() {
			object id = ReadId();
			if (id == null) return null;
			else return Cache.GetTemplate(id);
		}


		/// <summary>
		/// Implementation of reading a shape.
		/// </summary>
		protected Shape DoReadShape() {
			object id = ReadId();
			if (id == null) return null;
			else return Cache.GetShape(id);
		}


		/// <summary>
		/// Implementation of reading a model object.
		/// </summary>
		protected IModelObject DoReadModelObject() {
			object id = ReadId();
			if (id == null) return null;
			else return Cache.GetModelObject(id);
		}


		/// <summary>
		/// Implementation of reading a design.
		/// </summary>
		protected Design DoReadDesign() {
			object id = ReadId();
			if (id == null) return null;
			else return Cache.GetDesign(id);
		}


		/// <summary>
		/// Implementation of reading a cap style.
		/// </summary>
		protected ICapStyle DoReadCapStyle() {
			object id = ReadId();
			if (id == null) return null;
			else return (ICapStyle)cache.GetProjectStyle(id);
		}


		/// <summary>
		/// Implementation of reading a character style.
		/// </summary>
		protected ICharacterStyle DoReadCharacterStyle() {
			object id = ReadId();
			if (id == null) return null;
			else return (ICharacterStyle)cache.GetProjectStyle(id);
		}


		/// <summary>
		/// Implementation of reading a color style.
		/// </summary>
		protected IColorStyle DoReadColorStyle() {
			object id = ReadId();
			if (id == null) return null;
			else return (IColorStyle)cache.GetProjectStyle(id);
		}


		/// <summary>
		/// Implementation of reading a fill style.
		/// </summary>
		protected IFillStyle DoReadFillStyle() {
			object id = ReadId();
			if (id == null) return null;
			else return (IFillStyle)cache.GetProjectStyle(id);
		}


		/// <summary>
		/// Implementation of reading a line style.
		/// </summary>
		protected ILineStyle DoReadLineStyle() {
			object id = ReadId();
			if (id == null) return null;
			else return (ILineStyle)cache.GetProjectStyle(id);
		}


		/// <summary>
		/// Implementation of reading a paragraph style.
		/// </summary>
		protected IParagraphStyle DoReadParagraphStyle() {
			object id = ReadId();
			if (id == null) return null;
			else return (IParagraphStyle)cache.GetProjectStyle(id);
		}


		/// <summary>
		/// Checks whether the current property index refers to a valid entity field.
		/// </summary>
		protected virtual void ValidatePropertyIndex() {
			// We cannot check propertyIndex < 0 because some readers use PropertyIndex == -1 for the id.
			if (propertyIndex >= propertyInfos.Count)
				throw new NShapeException("An entity tries to read more properties from the repository than there are defined.");
		}


		/// <override></override>
		protected virtual void Dispose(bool disposing) {
			// Nothing to do
		}

		#endregion


		#region Fields

		/// <summary>
		/// A list of property info of the entity type to read.
		/// </summary>
		protected List<EntityPropertyDefinition> propertyInfos = new List<EntityPropertyDefinition>(20);

		/// <summary>
		/// When reading inner objects, this field holds the reader used for reading these inner objects.
		/// </summary>
		protected RepositoryReader innerObjectsReader;

		private IStoreCache cache;
		private int propertyIndex;
		// used for loading innerObjects
		private IEntity entity;

		#endregion
	}

	#endregion


	#region RepositoryWriter Class

	/// <summary>
	/// Offline RepositoryWriter
	/// </summary>
	public abstract class RepositoryWriter : IRepositoryWriter {

		/// <summary>
		/// Initializes a new Iinstance of RepositoryWriter
		/// </summary>
		protected RepositoryWriter(IStoreCache cache) {
			if (cache == null) throw new ArgumentNullException("cache");
			this.cache = cache;
		}


		#region [Public] IRepositoryWriter Members

		/// <summary>
		/// Fetches the next inner object in a set of inner object.
		/// </summary>
		public void BeginWriteInnerObject() {
			// Must be executed by the outer writer. Currently there is only one inner 
			// and one outer.
			DoBeginWriteInnerObject();
		}


		/// <summary>
		/// Fetches the next set of inner objects and prepares them for writing.
		/// </summary>
		public void BeginWriteInnerObjects() {
			if (innerObjectsWriter != null)
				throw new InvalidOperationException("Call EndWriteInnerObjects before a new call to BeginWriteInnerObjects.");
			DoBeginWriteInnerObjects();
		}


		/// <summary>
		/// Finishes writing an inner object.
		/// </summary>
		public void EndWriteInnerObject() {
			// Must be executed by the outer writer. Currently there is only one inner 
			// and one outer.
			DoEndWriteInnerObject();
		}


		/// <summary>
		/// Finishes writing the current set of inner objects.
		/// </summary>
		public void EndWriteInnerObjects() {
			if (innerObjectsWriter == null)
				throw new InvalidOperationException("BeginWriteInnerObjects has not been called.");
			DoEndWriteInnerObjects();
			innerObjectsWriter = null;
		}


		/// <summary>
		/// Deletes the current set of inner objects.
		/// </summary>
		public void DeleteInnerObjects() {
			BeginWriteInnerObjects();
			EndWriteInnerObjects();
		}


		/// <summary>
		/// Writes an IEntity.Id value.
		/// </summary>
		/// <param name="id"></param>
		public void WriteId(object id) {
			if (innerObjectsWriter == null) DoWriteId(id);
			else innerObjectsWriter.WriteId(id);
		}


		/// <summary>
		/// Writes a boolean value.
		/// </summary>
		public void WriteBool(bool value) {
			if (innerObjectsWriter == null) DoWriteBool(value);
			else innerObjectsWriter.WriteBool(value);
		}


		/// <summary>
		/// Writes a byte value.
		/// </summary>
		public void WriteByte(byte value) {
			if (innerObjectsWriter == null) DoWriteByte(value);
			else innerObjectsWriter.WriteByte(value);
		}


		/// <summary>
		/// Writes a 16 bit integer value.
		/// </summary>
		public void WriteInt16(short value) {
			if (innerObjectsWriter == null) DoWriteInt16(value);
			else innerObjectsWriter.WriteInt16(value);
		}


		/// <summary>
		/// Writes a 32 bit integer value.
		/// </summary>
		public void WriteInt32(int value) {
			if (innerObjectsWriter == null) DoWriteInt32(value);
			else innerObjectsWriter.WriteInt32(value);
		}


		/// <summary>
		/// Writes a 64 bit integer value.
		/// </summary>
		public void WriteInt64(long value) {
			if (innerObjectsWriter == null) DoWriteInt64(value);
			else innerObjectsWriter.WriteInt64(value);
		}


		/// <summary>
		/// Writes a single precision floating point number.
		/// </summary>
		public void WriteFloat(float value) {
			if (innerObjectsWriter == null) DoWriteFloat(value);
			else innerObjectsWriter.WriteFloat(value);
		}


		/// <summary>
		/// Writes a double precision floating point number.
		/// </summary>
		public void WriteDouble(double value) {
			if (innerObjectsWriter == null) DoWriteDouble(value);
			else innerObjectsWriter.WriteDouble(value);
		}


		/// <summary>
		/// Writes a character value.
		/// </summary>
		public void WriteChar(char value) {
			if (innerObjectsWriter == null) DoWriteChar(value);
			else innerObjectsWriter.WriteChar(value);
		}


		/// <summary>
		/// Writes a string value.
		/// </summary>
		public void WriteString(string value) {
			if (innerObjectsWriter == null) DoWriteString(value);
			else innerObjectsWriter.WriteString(value);
		}


		/// <summary>
		/// Writes a date and time value.
		/// </summary>
		public void WriteDate(DateTime value) {
			if (innerObjectsWriter == null) DoWriteDate(value);
			else innerObjectsWriter.WriteDate(value);
		}


		/// <summary>
		/// Writes an image value.
		/// </summary>
		public void WriteImage(System.Drawing.Image image) {
			if (innerObjectsWriter == null) DoWriteImage(image);
			else innerObjectsWriter.WriteImage(image);
		}


		/// <summary>
		/// Writes a template.
		/// </summary>
		public void WriteTemplate(Template template) {
			if (template != null && template.Id == null)
				throw new InvalidOperationException(string.Format("Template '{0}' was not inserted in the repository.", template.Name));
			if (innerObjectsWriter == null) {
				if (template == null) WriteId(null);
				else WriteId(template.Id);
			} else innerObjectsWriter.WriteTemplate(template);
		}


		/// <summary>
		/// Writes a style.
		/// </summary>
		public void WriteStyle(IStyle style) {
			if (style != null && style.Id == null) throw new InvalidOperationException(
				 string.Format("{0} '{1}' was not inserted in the repository.", style.GetType().Name, style.Name));
			if (innerObjectsWriter == null) {
				if (style == null) WriteId(null);
				else WriteId(style.Id);
			} else innerObjectsWriter.WriteStyle(style);
		}


		/// <summary>
		/// Writes a model object.
		/// </summary>
		public void WriteModelObject(IModelObject modelObject) {
			if (modelObject != null && modelObject.Id == null)
				throw new InvalidOperationException(string.Format("{0} '{1}' was not inserted in the repository.",
					modelObject.Type.FullName, modelObject.Name));
			if (innerObjectsWriter == null) {
				Debug.Assert(modelObject == null || modelObject.Id != null);
				if (modelObject == null) WriteId(null);
				else WriteId(modelObject.Id);
			} else innerObjectsWriter.WriteModelObject(modelObject);
		}

		#endregion


		#region [Protected] Properties

		/// <summary>
		/// Indicates the current index in the list of property info of the entity type.
		/// </summary>
		protected internal int PropertyIndex {
			get { return propertyIndex; }
			set { propertyIndex = value; }
		}


		/// <summary>
		/// When reading inner objects, this property stores the owner entity of the inner objects. Otherwise, this property is null/Nothing.
		/// </summary>
		protected IEntity Entity {
			get { return entity; }
		}


		/// <summary>
		/// The IStoreCache that contains the data to read.
		/// </summary>
		protected IStoreCache Cache {
			get { return cache; }
		}

		#endregion


		#region [Protected] Methods: Implementation

		/// <summary>
		/// Implementation of writing an IEntity.Id value.
		/// </summary>
		protected abstract void DoWriteId(object id);

		/// <summary>
		/// Implementation of writing a boolean value.
		/// </summary>
		protected abstract void DoWriteBool(bool value);

		/// <summary>
		/// Implementation of writing a byte value.
		/// </summary>
		protected abstract void DoWriteByte(byte value);

		/// <summary>
		/// Implementation of writing a 16 bit integer number.
		/// </summary>
		protected abstract void DoWriteInt16(short value);

		/// <summary>
		/// Implementation of writing a 32 bit integer number.
		/// </summary>
		protected abstract void DoWriteInt32(int value);

		/// <summary>
		/// Implementation of writing a 64 bit integer number.
		/// </summary>
		protected abstract void DoWriteInt64(long value);

		/// <summary>
		/// Implementation of writing a single precision floating point number.
		/// </summary>
		protected abstract void DoWriteFloat(float value);

		/// <summary>
		/// Implementation of writing a double precision floating point number.
		/// </summary>
		protected abstract void DoWriteDouble(double value);

		/// <summary>
		/// Implementation of writing a character value.
		/// </summary>
		protected abstract void DoWriteChar(char value);

		/// <summary>
		/// Implementation of writing a string value.
		/// </summary>
		protected abstract void DoWriteString(string value);

		/// <summary>
		/// Implementation of writing a date value.
		/// </summary>
		protected abstract void DoWriteDate(DateTime date);

		/// <summary>
		/// Implementation of writing an image.
		/// </summary>
		protected abstract void DoWriteImage(System.Drawing.Image image);

		/// <summary>
		/// Implementation of BeginWriteInnerObjects.
		/// </summary>
		protected abstract void DoBeginWriteInnerObjects();

		/// <summary>
		/// Implementation of EndWriteInnerObjects.
		/// </summary>
		protected abstract void DoEndWriteInnerObjects();

		// Must be called upon the outer cache writer.
		/// <summary>
		/// Implementation of BeginWriteInnerObject.
		/// </summary>
		protected abstract void DoBeginWriteInnerObject();

		// Must be called upon the outer cache writer.
		/// <summary>
		/// Implementation of EndWriteInnerObject.
		/// </summary>
		protected abstract void DoEndWriteInnerObject();

		/// <summary>
		/// Implementation of DeleteInnerObjects.
		/// </summary>
		protected abstract void DoDeleteInnerObjects();


		/// <summary>
		/// Reinitializes the writer to work with given property infos.
		/// </summary>
		protected internal virtual void Reset(IEnumerable<EntityPropertyDefinition> propertyInfos) {
			if (propertyInfos == null) throw new ArgumentNullException("propertyInfos");
			this.propertyInfos.Clear();
			this.propertyInfos.AddRange(propertyInfos);
		}


		/// <summary>
		/// Specifies the entity to write next. Is null when going to write an inner object.
		/// </summary>
		/// <param name="entity"></param>
		protected internal virtual void Prepare(IEntity entity) {
			this.entity = entity;
			// The first property is the internally written id.
			PropertyIndex = -2;
		}


		/// <summary>
		/// Commits inner object data to the data store.
		/// </summary>
		protected internal virtual void Finish() {
			// Nothing to do
		}

		#endregion


		#region Fields

		// When writing inner objects, reference to the responsible writer
		/// <summary>
		/// When reading inner objects, this field holds the reader used for reading these inner objects.
		/// </summary>
		protected RepositoryWriter innerObjectsWriter;

		// Description of the entity type currently writting
		/// <summary>
		/// A list of <see cref="T:Dataweb.NShape.Advanced.EntityPropertyDefinition" /> for the entity type.
		/// </summary>
		protected List<EntityPropertyDefinition> propertyInfos = new List<EntityPropertyDefinition>(20);

		private IStoreCache cache;
		// Current entity to write. Null when writing an inner object
		private IEntity entity;
		// Index of property currently being written
		private int propertyIndex;
		#endregion
	}

	#endregion


}