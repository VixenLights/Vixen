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


namespace Dataweb.NShape.Advanced
{
	/// <ToBeCompleted></ToBeCompleted>
	public class Model : IEntity
	{
		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.Model" />
		/// </summary>
		public Model()
		{
		}

		#region IEntity Members

		/// <summary>
		/// Specifies the entity type name of <see cref="T:Dataweb.NShape.Advanced.Model" />.
		/// </summary>
		public static string EntityTypeName
		{
			get { return entityTypeName; }
		}


		/// <summary>
		/// Retrieves the persistable properties of <see cref="T:Dataweb.NShape.Advanced.Model" />.
		/// </summary>
		public static IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version)
		{
			yield break;
		}


		/// <summary>
		/// The <see cref="T:Dataweb.NShape.Advanced.IEntity" />.Id of this <see cref="T:Dataweb.NShape.Advanced.Model" />
		/// </summary>
		public object Id
		{
			get { return id; }
		}


		/// <summary>
		/// See <see cref="T:Dataweb.NShape.Advanced.IEntity" />
		/// </summary>
		void IEntity.AssignId(object id)
		{
			if (id == null) throw new ArgumentNullException("id");
			if (this.id != null)
				throw new InvalidOperationException(string.Format("{0} has already an id.", GetType().Name));
			this.id = id;
		}


		void IEntity.LoadFields(IRepositoryReader reader, int version)
		{
			// nothing to do
		}


		void IEntity.LoadInnerObjects(string propertyName, IRepositoryReader reader, int version)
		{
			// nothing to do
		}


		void IEntity.SaveFields(IRepositoryWriter writer, int version)
		{
			// nothing to do
		}


		void IEntity.SaveInnerObjects(string propertyName, IRepositoryWriter writer, int version)
		{
			// nothing to do
		}


		void IEntity.Delete(IRepositoryWriter writer, int version)
		{
			// nothing to do
		}

		#endregion

		private const string entityTypeName = "Core.Model";
		private object id = null;
	}


	/// <summary>
	/// Defines a connection port for a model object.
	/// </summary>
	/// <status>reviewed</status>
	public struct TerminalId : IEquatable<TerminalId>
	{
		/// <summary>Specifies the invalid connection port.</summary>
		public static readonly TerminalId Invalid;

		/// <summary>Specifies a port for the model object as a whole.</summary>
		public static readonly TerminalId Generic;


		/// <summary>Converts a <see cref="T:Dataweb.NShape.Advanced.TerminalId" /> to a <see cref="T:System.Int32" />.</summary>
		public static implicit operator int(TerminalId tid)
		{
			return tid.id;
		}


		/// <summary>Converts a <see cref="T:System.Int32" /> to a <see cref="T:Dataweb.NShape.Advanced.TerminalId" />.</summary>
		public static implicit operator TerminalId(int value)
		{
			TerminalId result = Invalid;
			result.id = value;
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool operator ==(TerminalId tid1, TerminalId tid2)
		{
			return tid1.id == tid2.id;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static bool operator !=(TerminalId tid1, TerminalId tid2)
		{
			return tid1.id != tid2.id;
		}


		/// <override></override>
		public override bool Equals(object obj)
		{
			return obj is TerminalId && (TerminalId) obj == this;
		}


		/// <override></override>
		public bool Equals(TerminalId other)
		{
			return other == this;
		}


		/// <override></override>
		public override int GetHashCode()
		{
			return id.GetHashCode();
		}


		/// <override></override>
		public override string ToString()
		{
			return id.ToString();
		}


		static TerminalId()
		{
			Invalid.id = int.MinValue;
			Generic.id = 0;
		}


		private int id;
	}


	/// <summary>
	/// Defines the interface between the NShape framework and the model objects.
	/// </summary>
	public interface IModelObject : IEntity, ISecurityDomainObject
	{
		/// <summary>
		/// Name of the model object. Is unique with siblings.
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Type of this model object
		/// </summary>
		ModelObjectType Type { get; }

		/// <summary>
		/// Owning model object, can be null if this object is a root object.
		/// </summary>
		IModelObject Parent { get; set; }

		/// <summary>
		/// Creates a copy of this model object.
		/// </summary>
		/// <remarks>Composite objects are also cloned, references to aggregated objects are just copied.</remarks>
		IModelObject Clone();

		/// <summary>
		/// Connects the model object with another one.
		/// </summary>
		void Connect(TerminalId ownTerminalId, IModelObject otherModelObject, TerminalId otherTerminalId);

		/// <summary>
		/// Disconnects the model object from another one.
		/// </summary>
		void Disconnect(TerminalId ownTerminalId, IModelObject otherModelObject, TerminalId otherTerminalId);

		/// <summary>
		/// Retrieves the attached shapes.
		/// </summary>
		IEnumerable<Shape> Shapes { get; }

		/// <summary>
		/// Returns the number of attached shapes.
		/// </summary>
		int ShapeCount { get; }

		/// <summary>
		/// Attaches an observing shape.
		/// </summary>
		/// <param name="shape"></param>
		void AttachShape(Shape shape);

		/// <summary>
		/// Detaches an observing shape.
		/// </summary>
		/// <param name="shape"></param>
		void DetachShape(Shape shape);

		/// <summary>
		/// Retrieves the integer value of a field.
		/// </summary>
		/// <param name="propertyId"></param>
		/// <returns></returns>
		int GetInteger(int propertyId);

		/// <summary>
		/// Retrieves the float value of a field.
		/// </summary>
		/// <param name="propertyId"></param>
		/// <returns></returns>
		float GetFloat(int propertyId);

		/// <summary>
		/// Retrieves the string value of a field.
		/// </summary>
		/// <param name="propertyId"></param>
		/// <returns></returns>
		string GetString(int propertyId);

		/// <summary>
		/// Returns a collection of <see cref="T:Dataweb.NShape.Advanced.MenuItemDef" /> for constructing context menus etc.
		/// </summary>
		IEnumerable<MenuItemDef> GetMenuItemDefs();
	}


	/// <summary>
	/// Represents the method that is called to create a model object.
	/// </summary>
	/// <param name="modelObjectType"></param>
	/// <returns></returns>
	/// <status>reviewed</status>
	public delegate IModelObject CreateModelObjectDelegate(ModelObjectType modelObjectType);


	/// <summary>
	/// Represents a model object type.
	/// </summary>
	// Libraries register their model object types via:
	public abstract class ModelObjectType
	{
		/// <summary>
		/// Constructs a model object type.
		/// </summary>
		public ModelObjectType(string name, string libraryName, string categoryTitle,
		                       CreateModelObjectDelegate createModelObjectDelegate,
		                       GetPropertyDefinitionsDelegate getPropertyDefinitionsDelegate)
		{
			if (name == null) throw new ArgumentNullException("name");
			if (!Project.IsValidName(name))
				throw new ArgumentException(string.Format("'{0}' is not a valid model object type name.", name));
			if (libraryName == null) throw new ArgumentNullException("libraryName");
			if (!Project.IsValidName(libraryName))
				throw new ArgumentException(string.Format("'{0}' is not a valid library name.", libraryName));
			if (createModelObjectDelegate == null) throw new ArgumentNullException("createModelObjectDelegate");
			if (getPropertyDefinitionsDelegate == null) throw new ArgumentNullException("getPropertyDefinitionsDelegate");
			//
			this.name = name;
			this.libraryName = libraryName;
			this.categoryTitle = categoryTitle;
			this.createModelObjectDelegate = createModelObjectDelegate;
			this.getPropertyDefinitionsDelegate = getPropertyDefinitionsDelegate;
		}


		/// <summary>
		/// Specifies the language invariant name of the model object type.
		/// </summary>
		public string Name
		{
			get { return name; }
		}


		/// <summary>
		/// Indicates the name of the library where the model object type is implemented.
		/// </summary>
		public string LibraryName
		{
			get { return libraryName; }
		}


		/// <summary>
		/// Specifies the full language invariant name of the model object type.
		/// </summary>
		public string FullName
		{
			get { return string.Format("{0}.{1}", libraryName, name); }
		}


		/// <summary>
		/// Specifies the culture depending description of the model type.
		/// </summary>
		public string Description
		{
			get { return description; }
			set { description = value; }
		}


		/// <summary>
		/// Indicates the default for the culture depending category name.
		/// </summary>
		public string DefaultCategoryTitle
		{
			get { return categoryTitle; }
		}


		/// <summary>
		/// Creates a model object instance of this type.
		/// </summary>
		public IModelObject CreateInstance()
		{
			return createModelObjectDelegate(this);
		}


		/// <summary>
		/// Retrieves the persistable properties of <see cref="T:Dataweb.NShape.Advanced.ModelObjectType" />.
		/// </summary>
		public IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version)
		{
			return getPropertyDefinitionsDelegate(version);
		}


		/// <summary>
		/// Indicates largest available terminal id for this type.
		/// </summary>
		public abstract TerminalId MaxTerminalId { get; }


		/// <summary>
		/// Retreives the name of a terminal.
		/// </summary>
		public abstract string GetTerminalName(TerminalId terminalId);

		/// <summary>
		/// Retrieves the id of a terminal.
		/// </summary>
		public abstract TerminalId FindTerminalId(string terminalName);


		internal string GetDefaultName()
		{
			return string.Format("{0} {1}", name, ++nameCounter);
		}

		#region Fields

		private string name;
		private string libraryName;
		private string description;
		private string categoryTitle = string.Empty;
		private CreateModelObjectDelegate createModelObjectDelegate;
		private GetPropertyDefinitionsDelegate getPropertyDefinitionsDelegate;

		private int nameCounter = 0;

		#endregion
	}


	/// <ToBeCompleted></ToBeCompleted>
	[TypeDescriptionProvider(typeof (TypeDescriptionProviderDg))]
	public class GenericModelObjectType : ModelObjectType
	{
		/// <ToBeCompleted></ToBeCompleted>
		public GenericModelObjectType(string name, string namespaceName, string categoryTitle,
		                              CreateModelObjectDelegate createModelObjectDelegate,
		                              GetPropertyDefinitionsDelegate getPropertyDefinitionsDelegate, TerminalId maxTerminalId)
			: base(name, namespaceName, categoryTitle, createModelObjectDelegate, getPropertyDefinitionsDelegate)
		{
			this.maxTerminalId = maxTerminalId;
			terminals.Add(TerminalId.Generic, "Generic Terminal");
			for (int i = 1; i <= maxTerminalId; ++i)
				terminals.Add(i, "Terminal " + Convert.ToString(i));
		}


		/// <override></override>
		public override TerminalId MaxTerminalId
		{
			get { return maxTerminalId; }
		}


		/// <override></override>
		public override string GetTerminalName(TerminalId terminalId)
		{
			if (terminalId < 0 || terminalId > maxTerminalId) throw new ArgumentOutOfRangeException("terminalId");
			string result;
			if (terminals.TryGetValue(terminalId, out result)) return result;
			else throw new NShapeException("No terminal name found for terminal {0}", terminalId);
		}


		/// <override></override>
		public override TerminalId FindTerminalId(string terminalName)
		{
			if (string.IsNullOrEmpty(terminalName)) return TerminalId.Invalid;
			foreach (KeyValuePair<TerminalId, string> item in terminals) {
				if (item.Value.Equals(terminalName, StringComparison.InvariantCultureIgnoreCase))
					return item.Key;
			}
			return TerminalId.Invalid;
		}

		#region Fields

		private TerminalId maxTerminalId;
		private Dictionary<TerminalId, string> terminals = new Dictionary<TerminalId, string>();

		#endregion
	}


	/// <summary>
	/// Defines a read-only collection of model object types.
	/// </summary>
	public interface IReadOnlyModelObjectTypeCollection : IReadOnlyCollection<ModelObjectType>
	{
		/// <ToBeCompleted></ToBeCompleted>
		ModelObjectType this[string modelObjectTypeName] { get; }
	}


	/// <summary>
	/// Manages a list of model object types.
	/// </summary>
	public class ModelObjectTypeCollection : IReadOnlyModelObjectTypeCollection
	{
		internal ModelObjectTypeCollection()
		{
		}


		/// <summary>
		/// Adds a model object type to the collection.
		/// </summary>
		/// <param name="modelObjectType"></param>
		public void Add(ModelObjectType modelObjectType)
		{
			if (modelObjectType == null) throw new ArgumentNullException("modelObjectType");
			modelObjectTypes.Add(modelObjectType.FullName, modelObjectType);
		}


		/// <summary>
		/// Removes a model object type from the collection.
		/// </summary>
		/// <param name="modelObjectType"></param>
		public bool Remove(ModelObjectType modelObjectType)
		{
			if (modelObjectType == null) throw new ArgumentNullException("modelObjectType");
			return modelObjectTypes.Remove(modelObjectType.FullName);
		}


		/// <summary>
		/// Retrieves the model object type with the given name.
		/// </summary>
		/// <param name="typeName">Either a full (i.e. including the namespace) or partial model object type name</param>
		/// <returns>ModelObjectTypes object type with given name.</returns>
		public ModelObjectType GetModelObjectType(string typeName)
		{
			if (typeName == null) throw new ArgumentNullException("typeName");
			ModelObjectType result = null;
			if (!modelObjectTypes.TryGetValue(typeName, out result)) {
				foreach (KeyValuePair<string, ModelObjectType> item in modelObjectTypes) {
					// If no matching type name was found, check if the given type projectName was a type projectName without namespace
					if (string.Compare(item.Value.Name, typeName, StringComparison.InvariantCultureIgnoreCase) == 0) {
						if (result == null) result = item.Value;
						else
							throw new ArgumentException("The model object type '{0}' is ambiguous. Please specify the library name.",
							                            typeName);
					}
				}
			}
			if (result == null)
				throw new ArgumentException(string.Format("Model object type '{0}' was not registered.", typeName));
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public ModelObjectType this[string modelObjectTypeName]
		{
			get { return GetModelObjectType(modelObjectTypeName); }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public int Count
		{
			get { return modelObjectTypes.Count; }
		}


		internal bool IsModelObjectTypeRegistered(ModelObjectType modelObjectType)
		{
			return modelObjectTypes.ContainsKey(modelObjectType.FullName);
		}


		internal void Clear()
		{
			modelObjectTypes.Clear();
		}

		#region IEnumerable<Type> Members

		/// <ToBeCompleted></ToBeCompleted>
		public IEnumerator<ModelObjectType> GetEnumerator()
		{
			return modelObjectTypes.Values.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		/// <ToBeCompleted></ToBeCompleted>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return modelObjectTypes.Values.GetEnumerator();
		}

		#endregion

		#region ICollection Members

		/// <ToBeCompleted></ToBeCompleted>
		public void CopyTo(Array array, int index)
		{
			if (array == null) throw new ArgumentNullException("array");
			modelObjectTypes.Values.CopyTo((ModelObjectType[]) array, index);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public bool IsSynchronized
		{
			get { return false; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public object SyncRoot
		{
			get { throw new NotSupportedException(); }
		}

		#endregion

		#region Fields

		// Key = ModelObjectType.FullName, Value = ModelObjectType
		private Dictionary<string, ModelObjectType> modelObjectTypes = new Dictionary<string, ModelObjectType>();

		#endregion
	}


	/// <summary>
	/// Base class for model objects implementing naming, model hierarchy and shape management.
	/// </summary>
	/// <remarks>ModelObjectTypes objects can inherit from this class but need not.</remarks>
	[TypeDescriptionProvider(typeof (TypeDescriptionProviderDg))]
	public abstract class ModelObjectBase : IModelObject, IEntity
	{
		/// <ToBeCompleted></ToBeCompleted>
		protected internal ModelObjectBase(ModelObjectBase source)
		{
			id = null;
			modelObjectType = source.Type;
			name = modelObjectType.GetDefaultName();
			parent = source.Parent;
		}

		#region IModelObject Members

		/// <ToBeCompleted></ToBeCompleted>
		[Description("Indicates the name used to identify the Device.")]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		[Description("The type of the ModelObject.")]
		public ModelObjectType Type
		{
			get { return modelObjectType; }
		}


		/// <summary>
		/// Parent of the model objects. Only the root object has no parent. Sometimes
		/// temporary objects have no parent and are therefore orphaned. E.g. when cloning
		/// model objects the clones do not have parents.
		/// </summary>
		[Browsable(false)]
		public virtual IModelObject Parent
		{
			get { return parent; }
			set { parent = value; }
		}


		/// <override></override>
		[Browsable(false)]
		public IEnumerable<Shape> Shapes
		{
			get { return (IEnumerable<Shape>) shapes ?? EmptyEnumerator<Shape>.Empty; }
		}


		/// <override></override>
		[Browsable(false)]
		public int ShapeCount
		{
			get { return (shapes != null) ? shapes.Count : 0; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public abstract IModelObject Clone();


		/// <ToBeCompleted></ToBeCompleted>
		public virtual int GetInteger(int propertyId)
		{
			throw new NShapeException("No integer property with PropertyId {0} found.", propertyId);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public virtual float GetFloat(int propertyId)
		{
			throw new NShapeException("No float property with PropertyId {0} found.", propertyId);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public virtual string GetString(int propertyId)
		{
			throw new NShapeException("No string property with PropertyId {0} found.", propertyId);
		}


		/// <summary>
		/// Returns a collection of <see cref="T:Dataweb.NShape.Advanced.MenuItemDef" /> for constructing context menus etc.
		/// </summary>
		public abstract IEnumerable<MenuItemDef> GetMenuItemDefs();


		/// <ToBeCompleted></ToBeCompleted>
		public abstract void Connect(TerminalId ownTerminalId, IModelObject targetConnector, TerminalId targetTerminalId);


		/// <ToBeCompleted></ToBeCompleted>
		public abstract void Disconnect(TerminalId ownTerminalId, IModelObject targetConnector, TerminalId targetTerminalId);


		/// <override></override>
		public void AttachShape(Shape shape)
		{
			if (shape == null) throw new ArgumentNullException("shape");
			if (shapes == null) shapes = new List<Shape>(1);
			else if (shapes.Contains(shape))
				throw new NShapeException("{0} '{1}' is already attached to this shape.", Type.Name, Name);
			if (shape.ModelObject != this) shape.ModelObject = this;
			else shapes.Add(shape);
		}


		/// <override></override>
		public void DetachShape(Shape shape)
		{
			if (shape == null) throw new ArgumentNullException("shape");
			if (shapes == null) throw new NShapeException("{0} '{1}' is not attached to any shape.", Type.Name, Name);
			int idx = shapes.IndexOf(shape);
			if (idx < 0) throw new NShapeException("{0} '{1}' is not attached to this shape.", Type.Name, Name);
			if (shape.ModelObject == this) shape.ModelObject = null;
			else shapes.RemoveAt(idx);
		}

		#endregion

		#region IEntity Members

		/// <summary>
		/// Retrieves the persistable properties of <see cref="T:Dataweb.NShape.Advanced.ModelObjectBase" />.
		/// </summary>
		public static IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version)
		{
			yield return new EntityFieldDefinition("Name", typeof (string));
			if (version >= 4) yield return new EntityFieldDefinition("SecurityDomainName", typeof (string));
		}


		// unique id of object, does never change
		object IEntity.Id
		{
			get { return id; }
		}


		void IEntity.AssignId(object id)
		{
			if (id == null) throw new ArgumentNullException("id");
			if (this.id != null) throw new InvalidOperationException("Model object has already an id.");
			this.id = id;
		}


		void IEntity.LoadFields(IRepositoryReader reader, int version)
		{
			if (reader == null) throw new ArgumentNullException("reader");
			LoadFieldsCore(reader, version);
		}


		void IEntity.LoadInnerObjects(string propertyName, IRepositoryReader reader, int version)
		{
			if (propertyName == null) throw new ArgumentNullException("propertyName");
			if (reader == null) throw new ArgumentNullException("reader");
			LoadInnerObjectsCore(propertyName, reader, version);
		}


		void IEntity.SaveFields(IRepositoryWriter writer, int version)
		{
			if (writer == null) throw new ArgumentNullException("writer");
			SaveFieldsCore(writer, version);
		}


		void IEntity.SaveInnerObjects(string propertyName, IRepositoryWriter writer, int version)
		{
			if (propertyName == null) throw new ArgumentNullException("propertyName");
			if (writer == null) throw new ArgumentNullException("writer");
			SaveInnerObjectsCore(propertyName, writer, version);
		}


		void IEntity.Delete(IRepositoryWriter writer, int version)
		{
			if (writer == null) throw new ArgumentNullException("writer");
			foreach (EntityPropertyDefinition pi in GetPropertyDefinitions(version)) {
				if (pi is EntityInnerObjectsDefinition) {
					writer.DeleteInnerObjects();
				}
			}
		}

		#endregion

		/// <summary>
		/// Indicates the name of the security domain this shape belongs to.
		/// </summary>
		[Category("General")]
		[Description("Modify the security domain of the shape.")]
		[RequiredPermission(Permission.Security)]
		public abstract char SecurityDomainName { get; set; }


		/// <ToBeCompleted></ToBeCompleted>
		protected internal ModelObjectBase(ModelObjectType modelObjectType)
		{
			if (modelObjectType == null) throw new ArgumentNullException("ModelObjectType");
			this.modelObjectType = modelObjectType;
			this.name = modelObjectType.GetDefaultName();
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void OnPropertyChanged(int propertyId)
		{
			foreach (Shape shape in Shapes)
				shape.NotifyModelChanged(propertyId);
		}

		#region [Protected] IEntity implementation

		/// <ToBeCompleted></ToBeCompleted>
		protected virtual void LoadFieldsCore(IRepositoryReader reader, int version)
		{
			name = reader.ReadString();
			if (version >= 4) SecurityDomainName = reader.ReadChar();
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected virtual void LoadInnerObjectsCore(string propertyName, IRepositoryReader reader, int version)
		{
			// nothing to do here
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected virtual void SaveFieldsCore(IRepositoryWriter writer, int version)
		{
			writer.WriteString(name);
			if (version >= 4) writer.WriteChar(SecurityDomainName);
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected virtual void SaveInnerObjectsCore(string propertyName, IRepositoryWriter writer, int version)
		{
			// nothing to do here
		}

		#endregion

		#region Fields

		/// <ToBeCompleted></ToBeCompleted>
		protected int terminalCount;

		private const string persistentTypeName = "ModelObject";

		private object id = null;
		private ModelObjectType modelObjectType = null;
		private string name = string.Empty;
		private IModelObject parent = null;
		private List<Shape> shapes = null;

		#endregion
	}


	/// <summary>
	/// ModelObjectTypes object with configurable number and type of properties.
	/// </summary>
	public class GenericModelObject : ModelObjectBase
	{
		/// <ToBeCompleted></ToBeCompleted>
		public static GenericModelObject CreateInstance(ModelObjectType modelObjectType)
		{
			if (modelObjectType == null) throw new ArgumentNullException("modelObjectType");
			return new GenericModelObject(modelObjectType);
		}


		/// <override></override>
		public override IModelObject Clone()
		{
			return new GenericModelObject(this);
		}


		/// <override></override>
		public override int GetInteger(int propertyId)
		{
			if (propertyId == PropertyIdIntegerValue) return IntegerValue;
			else return base.GetInteger(propertyId);
		}


		/// <override></override>
		public override float GetFloat(int propertyId)
		{
			if (propertyId == PropertyIdFloatValue) return FloatValue;
			else return base.GetFloat(propertyId);
		}


		/// <override></override>
		public override string GetString(int propertyId)
		{
			if (propertyId == PropertyIdStringValue) return StringValue;
			else return base.GetString(propertyId);
		}


		/// <ToBeCompleted></ToBeCompleted>
		[PropertyMappingId(PropertyIdIntegerValue)]
		[Description("The value of the device. This value is represented by the assigned Shape.")]
		public int IntegerValue
		{
			get { return integerValue; }
			set
			{
				integerValue = value;
				OnPropertyChanged(PropertyIdIntegerValue);
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		[PropertyMappingId(PropertyIdFloatValue)]
		[Description("The value of the device. This value is represented by the assigned Shape.")]
		public float FloatValue
		{
			get { return floatValue; }
			set
			{
				floatValue = value;
				OnPropertyChanged(PropertyIdFloatValue);
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		[PropertyMappingId(PropertyIdStringValue)]
		[Description("The value of the device. This value is represented by the assigned Shape.")]
		public string StringValue
		{
			get { return stringValue; }
			set
			{
				stringValue = value;
				OnPropertyChanged(PropertyIdStringValue);
			}
		}


		/// <override></override>
		public override char SecurityDomainName
		{
			get { return securityDomainName; }
			set
			{
				if (value < 'A' || value > 'Z')
					throw new ArgumentOutOfRangeException("SecurityDomainName",
					                                      "The domain qualifier has to be an upper case  ANSI letter (A-Z).");
				securityDomainName = value;
			}
		}


		/// <override></override>
		public override IEnumerable<MenuItemDef> GetMenuItemDefs()
		{
			//yield return new NotImplementedAction("Set State");
			yield break;
		}


		/// <override></override>
		public override void Connect(TerminalId ownTerminalId, IModelObject targetConnector, TerminalId targetTerminalId)
		{
			throw new NotImplementedException("Not yet implemented");
		}


		/// <override></override>
		public override void Disconnect(TerminalId ownTerminalId, IModelObject targetConnector, TerminalId targetTerminalId)
		{
			throw new NotImplementedException("Not yet implemented");
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected internal GenericModelObject(ModelObjectType modelObjectType)
			: base(modelObjectType)
		{
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected internal GenericModelObject(GenericModelObject source)
			: base(source)
		{
		}

		#region IEntity Members

		/// <summary>
		/// Retrieves the persistable properties of <see cref="T:Dataweb.NShape.Advanced.GenericModelObject" />.
		/// </summary>
		public new static IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version)
		{
			foreach (EntityPropertyDefinition pi in ModelObjectBase.GetPropertyDefinitions(version))
				yield return pi;
			yield return new EntityFieldDefinition("IntegerValue", typeof (int));
			yield return new EntityFieldDefinition("FloatValue", typeof (float));
			yield return new EntityFieldDefinition("StringValue", typeof (string));
		}


		/// <override></override>
		protected override void LoadFieldsCore(IRepositoryReader reader, int version)
		{
			base.LoadFieldsCore(reader, version);
			integerValue = reader.ReadInt32();
			floatValue = reader.ReadFloat();
			stringValue = reader.ReadString();
		}


		/// <override></override>
		protected override void SaveFieldsCore(IRepositoryWriter writer, int version)
		{
			base.SaveFieldsCore(writer, version);
			writer.WriteInt32(integerValue);
			writer.WriteFloat(floatValue);
			writer.WriteString(stringValue);
		}

		#endregion

		#region Fields

		/// <ToBeCompleted></ToBeCompleted>
		protected const int PropertyIdGenericValue = 1;

		/// <ToBeCompleted></ToBeCompleted>
		protected const int PropertyIdStringValue = 2;

		/// <ToBeCompleted></ToBeCompleted>
		protected const int PropertyIdIntegerValue = 3;

		/// <ToBeCompleted></ToBeCompleted>
		protected const int PropertyIdFloatValue = 4;

		private int integerValue;
		private float floatValue;
		private string stringValue;
		private char securityDomainName = 'A';

		#endregion
	}
}