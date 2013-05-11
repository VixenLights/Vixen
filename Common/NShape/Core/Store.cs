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


namespace Dataweb.NShape.Advanced {

	#region IStoreCache Interface

	/// <summary>
	/// Provides access to NShape entities of one type for stores.
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public interface ICacheCollection<TEntity> : IEnumerable<EntityBucket<TEntity>> where TEntity : IEntity {

		/// <ToBeCompleted></ToBeCompleted>
		bool Contains(object id);

		/// <ToBeCompleted></ToBeCompleted>
		TEntity GetEntity(object id);

		/// <ToBeCompleted></ToBeCompleted>
		EntityBucket<TEntity> this[object id] { get; }

		/// <ToBeCompleted></ToBeCompleted>
		void Add(EntityBucket<TEntity> bucket);

	}


	/// <summary>
	/// Provides access to NShape entities for stores.
	/// </summary>
	public interface IStoreCache {

		/// <ToBeCompleted></ToBeCompleted>
		object ProjectId { get; }

		/// <ToBeCompleted></ToBeCompleted>
		string ProjectName { get; }

		/// <summary>
		/// Indicates the repository version of the core libraries.
		/// </summary>
		int RepositoryBaseVersion { get; }

		/// <summary>
		/// Sets the repository version of the core libraries from a loading project.
		/// </summary>
		/// <param name="version"></param>
		void SetRepositoryBaseVersion(int version);

		/// <ToBeCompleted></ToBeCompleted>
		ProjectSettings Project { get; }

		/// <ToBeCompleted></ToBeCompleted>
		void SetProjectOwnerId(object id);

		/// <ToBeCompleted></ToBeCompleted>
		Design ProjectDesign { get; }

		//---------

		/// <ToBeCompleted></ToBeCompleted>
		IEnumerable<IEntityType> EntityTypes { get; }

		/// <ToBeCompleted></ToBeCompleted>
		IEntityType FindEntityTypeByName(string entityTypeName);

		/// <ToBeCompleted></ToBeCompleted>
		IEntityType FindEntityTypeByElementName(string elementName);

		/// <ToBeCompleted></ToBeCompleted>
		string CalculateElementName(string entityTypeName);

		//---------

		/// <ToBeCompleted></ToBeCompleted>
		IStyle GetProjectStyle(object id);

		/// <ToBeCompleted></ToBeCompleted>
		bool ModelExists();
		
		/// <ToBeCompleted></ToBeCompleted>
		Model GetModel();

		/// <ToBeCompleted></ToBeCompleted>
		Template GetTemplate(object id);

		/// <ToBeCompleted></ToBeCompleted>
		Diagram GetDiagram(object id);

		/// <ToBeCompleted></ToBeCompleted>
		Shape GetShape(object id);

		/// <ToBeCompleted></ToBeCompleted>
		IModelObject GetModelObject(object id);

		/// <ToBeCompleted></ToBeCompleted>
		Design GetDesign(object id);

		//---------

		/// <ToBeCompleted></ToBeCompleted>
		ICacheCollection<Diagram> LoadedDiagrams { get; }

		/// <ToBeCompleted></ToBeCompleted>
		IEnumerable<KeyValuePair<Diagram, IEntity>> NewDiagrams { get; }

		/// <ToBeCompleted></ToBeCompleted>
		ICacheCollection<Shape> LoadedShapes { get; }

		/// <ToBeCompleted></ToBeCompleted>
		IEnumerable<KeyValuePair<Shape, IEntity>> NewShapes { get; }

		/// <ToBeCompleted></ToBeCompleted>
		ICacheCollection<ProjectSettings> LoadedProjects { get; }

		/// <ToBeCompleted></ToBeCompleted>
		IEnumerable<KeyValuePair<ProjectSettings, IEntity>> NewProjects { get; }

		/// <ToBeCompleted></ToBeCompleted>
		ICacheCollection<Model> LoadedModels { get; }

		/// <ToBeCompleted></ToBeCompleted>
		IEnumerable<KeyValuePair<Model, IEntity>> NewModels { get; }

		/// <ToBeCompleted></ToBeCompleted>
		ICacheCollection<Design> LoadedDesigns { get; }

		/// <ToBeCompleted></ToBeCompleted>
		IEnumerable<KeyValuePair<Design, IEntity>> NewDesigns { get; }

		/// <ToBeCompleted></ToBeCompleted>
		ICacheCollection<IStyle> LoadedStyles { get; }

		/// <ToBeCompleted></ToBeCompleted>
		IEnumerable<KeyValuePair<IStyle, IEntity>> NewStyles { get; }

		/// <ToBeCompleted></ToBeCompleted>
		ICacheCollection<Template> LoadedTemplates { get; }

		/// <ToBeCompleted></ToBeCompleted>
		ICacheCollection<IModelMapping> LoadedModelMappings { get; }

		/// <ToBeCompleted></ToBeCompleted>
		IEnumerable<KeyValuePair<Template, IEntity>> NewTemplates { get; }

		/// <ToBeCompleted></ToBeCompleted>
		IEnumerable<KeyValuePair<IModelMapping, IEntity>> NewModelMappings { get; }

		/// <ToBeCompleted></ToBeCompleted>
		IEnumerable<ShapeConnection> NewShapeConnections { get; }

		/// <ToBeCompleted></ToBeCompleted>
		IEnumerable<ShapeConnection> DeletedShapeConnections { get; }

		/// <ToBeCompleted></ToBeCompleted>
		ICacheCollection<IModelObject> LoadedModelObjects { get; }

		/// <ToBeCompleted></ToBeCompleted>
		IEnumerable<KeyValuePair<IModelObject, IEntity>> NewModelObjects { get; }

	}
	
	#endregion


	#region Store Class

	/// <ToBeCompleted></ToBeCompleted>
	public delegate bool IdFilter(object id);


	/// <summary>
	/// Stores cache data persistently in a data source.
	/// </summary>
	public abstract class Store : Component {

		/// <summary>
		/// Specifies the name of the project.
		/// </summary>
		[Browsable(false)]
		public abstract string ProjectName { get; set; }


		/// <summary>
		/// Tests whether the project already exists in the data source.
		/// </summary>
		/// <returns></returns>
		public abstract bool Exists();


		/// <summary>
		/// Reads the version of the project from the persistent store.
		/// </summary>
		public abstract void ReadVersion(IStoreCache cache);


		/// <summary>
		/// Creates a project store in the data source.
		/// </summary>
		/// <param name="storeCache"></param>
		public abstract void Create(IStoreCache storeCache);


		/// <summary>
		/// Opens a project store in the data source.
		/// </summary>
		/// <param name="storeCache"></param>
		public abstract void Open(IStoreCache storeCache);


		/// <summary>
		/// Closes the project store.
		/// </summary>
		/// <param name="storeCache"></param>
		public virtual void Close(IStoreCache storeCache) {
			if (storeCache == null) throw new ArgumentNullException("storeCache");
			// Nothing to do yet.
		}


		/// <summary>
		/// Deletes the project store in the data source.
		/// </summary>
		public abstract void Erase();


		/// <summary>
		/// Loads the current project into the given store cache.
		/// </summary>
		/// <param name="cache">The store cache associated with this store.</param>
		/// <param name="entityType">Project entity type</param>
		/// <param name="parameters">Ids of the project settings to load. if null, all projects are loaded.</param>
		/// <remarks>Optional parameter 'parameters' is not used in the current version.</remarks>
		public abstract void LoadProjects(IStoreCache cache, IEntityType entityType, params object[] parameters);


		/// <summary>
		/// Loads the model of the project into the given store cache.
		/// </summary>
		public abstract void LoadModel(IStoreCache cache, object projectId);


		/// <summary>
		/// Loads general designs or a project design.
		/// </summary>
		/// <param name="cache">Store cache to load to.</param>
		/// <param name="projectId">Project id for project design, null for general designs.</param>
		public abstract void LoadDesigns(IStoreCache cache, object projectId);


		/// <summary>
		/// Loads all templates of the project into the given store cache.
		/// </summary>
		public abstract void LoadTemplates(IStoreCache cache, object projectId);


		/// <summary>
		/// Loads all diagrams of the project into the given store cache.
		/// </summary>
		/// <remarks>
		/// If the store supports partial loading, the shapes shapes of the diagrams are not loaded.
		/// </remarks>
		public abstract void LoadDiagrams(IStoreCache cache, object projectId);


		/// <summary>
		/// Loads all shapes of the given diagram into the given store cache.
		/// </summary>
		/// <remarks>
		/// If the diagram's shapes have already been loaded, this method does nothing.
		/// </remarks>
		public abstract void LoadDiagramShapes(IStoreCache cache, Diagram diagram);


		/// <summary>
		/// Loads all shapes of the given template into the given store cache.
		/// </summary>
		/// <remarks>
		/// If the template's shapes have already been loaded, this method does nothing.
		/// </remarks>
		public abstract void LoadTemplateShapes(IStoreCache cache, object templateId);


		/// <summary>
		/// Loads all child shapes of the shape with the given Id into the given store cache.
		/// </summary>
		public abstract void LoadChildShapes(IStoreCache cache, object parentShapeId);


		/// <summary>
		/// Loads all model objects of the given template into the given store cache.
		/// </summary>
		/// <remarks>
		/// If the template's model objects have already been loaded, this method does nothing.
		/// </remarks>
		public abstract void LoadTemplateModelObjects(IStoreCache cache, object templateId);


		/// <summary>
		/// Loads all model objects of the given model into the given store cache.
		/// </summary>
		/// <remarks>
		/// If the model's model ojects have already been loaded, this method does nothing.
		/// </remarks>
		public abstract void LoadModelModelObjects(IStoreCache cache, object modelId);


		/// <summary>
		/// Loads all child model objects of the model object with the given Id into the given store cache.
		/// </summary>
		/// <param name="cache">Store cache associated with this store.</param>
		/// <param name="parentModelObjectId">Id of the parent model object.</param>
		public abstract void LoadChildModelObjects(IStoreCache cache, object parentModelObjectId);


		/// <summary>
		/// Checks whether the style associated with the given id is still in use.
		/// </summary>
		/// <param name="cache">The store cache associated with this store.</param>
		/// <param name="styleId">The id of the style to be checked.</param>
		/// <remarks>Applies only to stores that support partial loading.</remarks>
		public abstract bool CheckStyleInUse(IStoreCache cache, object styleId);


		/// <summary>
		/// Checks whether the template associated with the given id is still in use.
		/// </summary>
		/// <param name="cache">The store cache associated with this store.</param>
		/// <param name="templateId">The id of the template to be checked.</param>
		/// <remarks>Applies only to stores that support partial loading.</remarks>
		public abstract bool CheckTemplateInUse(IStoreCache cache, object templateId);


		/// <summary>
		/// Checks whether the model object associated with the given id is still in use.
		/// </summary>
		/// <param name="cache">The store cache associated with this store.</param>
		/// <param name="modelObjectId">The id of the model object to be checked.</param>
		/// <remarks>Applies only to stores that support partial loading.</remarks>
		public abstract bool CheckModelObjectInUse(IStoreCache cache, object modelObjectId);


		/// <summary>
		/// Checks whether the given shape type is still in use.
		/// </summary>
		/// <param name="cache">The store cache associated with this store.</param>
		/// <param name="typeName">The name of the shape type to check.</param>
		/// <remarks>Applies only to stores that support partial loading.</remarks>
		public abstract bool CheckShapeTypeInUse(IStoreCache cache, string typeName);


		/// <summary>
		/// Commits all modifications in the cache to the data store.
		/// </summary>
		public abstract void SaveChanges(IStoreCache storeCache);


		/// <summary>
		/// Specifies the main version of the storage format.
		/// </summary>
		protected internal abstract int Version { get; set; }

	}

	#endregion

}
