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
using Dataweb.NShape.Advanced;


namespace Dataweb.NShape
{
	/// <summary>
	/// Combines a shape and a model object to form a sample for shape creation.
	/// </summary>
	public class Template : IEntity
	{
		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.Template" />.
		/// </summary>
		public Template(string name, Shape shape)
		{
			if (name == null) throw new ArgumentNullException("name");
			if (shape == null) throw new ArgumentNullException("shape");
			this.name = name;
			this.shape = shape;
		}


		/// <override></override>
		public override string ToString()
		{
			return Title;
		}


		/// <summary>
		/// Creates a new <see cref="T:Dataweb.NShape.Advanced.Template" /> that is a copy of the current instance.
		/// </summary>
		public Template Clone()
		{
			Template result = new Template();
			result.CopyFrom(this);
			return result;
		}


		/// <summary>
		/// Copies all properties and fields from the source template to this template.
		/// The shape and the model obejcts will be cloned.
		/// </summary>
		public void CopyFrom(Template source)
		{
			if (source == null) throw new ArgumentNullException("source");

			this.name = source.name;
			this.title = source.title;
			this.description = source.description;

			// Clone or copy shape
			if (this.shape == null) this.shape = ShapeDuplicator.CloneShapeAndModelObject(source.shape);
			else this.shape.CopyFrom(source.shape); // Template will be copied although this is not desirable

			// copy connection point mapping
			this.connectionPointMappings.Clear();
			foreach (KeyValuePair<ControlPointId, TerminalId> item in source.connectionPointMappings)
				this.connectionPointMappings.Add(item.Key, item.Value);

			// copy property mapping
			this.propertyMappings.Clear();
			foreach (KeyValuePair<int, IModelMapping> item in source.propertyMappings)
				this.propertyMappings.Add(item.Key, item.Value.Clone());
		}


		/// <summary>
		/// Gets or sets an object that provides additional data.
		/// </summary>
		public object Tag
		{
			get { return tag; }
			set { tag = value; }
		}


		/// <summary>
		/// Specifies the culture independent name.
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}


		/// <summary>
		/// Specifies the culture dependent display name.
		/// </summary>
		public string Title
		{
			get { return string.IsNullOrEmpty(title) ? name : title; }
			set
			{
				if (value == name || string.IsNullOrEmpty(value))
					title = null;
				else title = value;
			}
		}


		/// <summary>
		/// A descriptive text for the template.
		/// </summary>
		public string Description
		{
			get { return description; }
			set { description = value; }
		}


		/// <summary>
		/// Defines the shape for this template. If the template contains a ModelObject, it will also become the shape's ModelObject.
		/// </summary>
		/// <remarks>Replacing the shape of a template with templated shapes results in 
		/// errors, if the templated shapes are not updated accordingly.</remarks>
		public Shape Shape
		{
			get { return shape; }
			set
			{
				if (shape != null) {
					if (shape.ModelObject != null && value != null && value.ModelObject != null) {
						// If both shapes have ModelObejct instances assigned, 
						// try to keep as many mappings as possible
						// ToDo: try to copy property mappings
						CopyTerminalMappings(shape.ModelObject, value.ModelObject);
					}
					else {
						// Delete all mappings to restore default behavior
						UnmapAllProperties();
						UnmapAllTerminals();
					}
				}
				shape = value;
			}
		}


		/// <summary>
		/// Creates a new shape from this template.
		/// </summary>
		/// <returns></returns>
		public Shape CreateShape()
		{
			Shape result = shape.Type.CreateInstance(this);
			if (shape.ModelObject != null)
				ShapeDuplicator.CloneModelObjectOnly(result);
			return result;
		}


		/// <summary>
		/// Creates a thumbnail of the template shape.
		/// </summary>
		/// <param name="size">Size of tumbnail in pixels</param>
		/// <param name="margin">Size of margin around shape in pixels</param>
		public Image CreateThumbnail(int size, int margin)
		{
			return CreateThumbnail(size, margin, Color.White);
		}


		/// <summary>
		/// Creates a thumbnail of the template shape.
		/// </summary>
		/// <param name="size">Size of tumbnail in pixels</param>
		/// <param name="margin">Size of margin around shape in pixels</param>
		/// <param name="transparentColor">Specifies a color that will be rendered transparent.</param>
		public Image CreateThumbnail(int size, int margin, Color transparentColor)
		{
			Image bmp = new Bitmap(size, size);
			using (Shape shapeClone = Shape.Clone())
				shapeClone.DrawThumbnail(bmp, margin, transparentColor);
			return bmp;
		}


		/// <summary>
		/// Returns a collection of <see cref="T:Dataweb.NShape.Advanced.MenuItemDef" /> for constructing context menus etc.
		/// </summary>
		public IEnumerable<MenuItemDef> GetMenuItemDefs()
		{
			yield break;
		}

		#region Visualization Mapping

		/// <ToBeCompleted></ToBeCompleted>
		public IEnumerable<IModelMapping> GetPropertyMappings()
		{
			return propertyMappings.Values;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public IModelMapping GetPropertyMapping(int modelPropertyId)
		{
			IModelMapping result = null;
			propertyMappings.TryGetValue(modelPropertyId, out result);
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void MapProperties(IModelMapping propertyMapping)
		{
			if (propertyMapping == null) throw new ArgumentNullException("propertyMapping");
			if (propertyMappings.ContainsKey(propertyMapping.ModelPropertyId))
				propertyMappings[propertyMapping.ModelPropertyId] = propertyMapping;
			else
				propertyMappings.Add(propertyMapping.ModelPropertyId, propertyMapping);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void UnmapProperties(IModelMapping propertyMapping)
		{
			if (propertyMapping == null) throw new ArgumentNullException("propertyMapping");
			propertyMappings.Remove(propertyMapping.ModelPropertyId);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void UnmapAllProperties()
		{
			propertyMappings.Clear();
		}

		#endregion

		#region Terminal Mapping

		/// <ToBeCompleted></ToBeCompleted>
		public TerminalId GetMappedTerminalId(ControlPointId connectionPointId)
		{
			// If there is a mapping, return it.
			TerminalId result;
			if (connectionPointMappings.TryGetValue(connectionPointId, out result))
				return result;
			else {
				// if there is no mapping, return default values:
				if (shape != null) {
					// - If the given point is no connectionPoint
					if (
						!shape.HasControlPointCapability(connectionPointId,
						                                 ControlPointCapabilities.Connect | ControlPointCapabilities.Glue))
						return TerminalId.Invalid;
						// - If a shape is set but no ModelObject, all connectionPoints are activated by default
					else if (shape.ModelObject == null) return TerminalId.Generic;
					else return TerminalId.Invalid;
				}
				else return TerminalId.Invalid;
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public string GetMappedTerminalName(ControlPointId connectionPointId)
		{
			TerminalId terminalId = GetMappedTerminalId(connectionPointId);
			if (terminalId == TerminalId.Invalid)
				return null;
			else {
				if (shape.ModelObject != null)
					return shape.ModelObject.Type.GetTerminalName(terminalId);
				else return activatedTag;
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void MapTerminal(TerminalId terminalId, ControlPointId connectionPointId)
		{
			// check if terminalId and connectionPointId are valid values
			if (shape == null)
				throw new InvalidOperationException("Template has no shape.");
			if (
				!shape.HasControlPointCapability(connectionPointId, ControlPointCapabilities.Glue | ControlPointCapabilities.Connect))
				throw new NShapeException("Control point {0} is not a valid glue- or connection point.", connectionPointId);
			//
			if (connectionPointMappings.ContainsKey(connectionPointId))
				connectionPointMappings[connectionPointId] = terminalId;
			else
				connectionPointMappings.Add(connectionPointId, terminalId);
		}


		/// <summary>
		/// Clears all mappings between the shape's connection points and the model's terminals.
		/// </summary>
		public void UnmapAllTerminals()
		{
			connectionPointMappings.Clear();
		}

		#endregion

		#region IEntity Members

		/// <summary>
		/// The entity type name of <see cref="T:Dataweb.NShape.Advanced.Template" />.
		/// </summary>
		public static string EntityTypeName
		{
			get { return entityTypeName; }
		}


		/// <summary>
		/// Retrieves the persistable properties of <see cref="T:Dataweb.NShape.Advanced.Template" />.
		/// </summary>
		public static IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version)
		{
			yield return new EntityFieldDefinition("Name", typeof (string));
			if (version >= 3) yield return new EntityFieldDefinition("Title", typeof (string));
			yield return new EntityFieldDefinition("Description", typeof (string));
			yield return
				new EntityInnerObjectsDefinition(connectionPtMappingName + "s", connectionPtMappingName,
				                                 connectionPtMappingAttrNames, connectionPtMappingAttrTypes);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public object Id
		{
			get { return id; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void AssignId(object id)
		{
			if (id == null) throw new ArgumentNullException("id");
			if (this.id != null) throw new InvalidOperationException("Template has already an id.");
			this.id = id;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void SaveFields(IRepositoryWriter writer, int version)
		{
			if (writer == null) throw new ArgumentNullException("writer");
			writer.WriteString(name);
			if (version >= 3) writer.WriteString(title);
			writer.WriteString(description);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void LoadFields(IRepositoryReader reader, int version)
		{
			if (reader == null) throw new ArgumentNullException("reader");
			name = reader.ReadString();
			if (version >= 3) title = reader.ReadString();
			description = reader.ReadString();
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void SaveInnerObjects(string propertyName, IRepositoryWriter writer, int version)
		{
			if (propertyName == null) throw new ArgumentNullException("propertyName");
			if (writer == null) throw new ArgumentNullException("writer");
			if (propertyName == "ConnectionPointMappings") {
				// Save ConnectionPoint mappings
				writer.BeginWriteInnerObjects();
				foreach (KeyValuePair<ControlPointId, TerminalId> item in connectionPointMappings) {
					writer.BeginWriteInnerObject();
					writer.WriteInt32((int) item.Key);
					writer.WriteInt32((int) item.Value);
					writer.EndWriteInnerObject();
				}
				writer.EndWriteInnerObjects();
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void LoadInnerObjects(string propertyName, IRepositoryReader reader, int version)
		{
			if (propertyName == null) throw new ArgumentNullException("propertyName");
			if (reader == null) throw new ArgumentNullException("reader");
			if (propertyName == "ConnectionPointMappings") {
				// load ConnectionPoint mappings			
				reader.BeginReadInnerObjects();
				while (reader.BeginReadInnerObject()) {
					ControlPointId connectionPointId = reader.ReadInt32();
					TerminalId terminalId = reader.ReadInt32();
					// The following is the essence of MapTerminal without the checks.
					if (connectionPointMappings.ContainsKey(connectionPointId))
						connectionPointMappings[connectionPointId] = terminalId;
					else
						connectionPointMappings.Add(connectionPointId, terminalId);
					reader.EndReadInnerObject();
				}
				reader.EndReadInnerObjects();
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void Delete(IRepositoryWriter writer, int version)
		{
			if (writer == null) throw new ArgumentNullException("writer");
			foreach (EntityPropertyDefinition pi in GetPropertyDefinitions(version)) {
				if (pi is EntityInnerObjectsDefinition)
					writer.DeleteInnerObjects();
			}
		}

		#endregion

		// Used to create templates for loading.
		internal Template()
		{
		}


		private int CountControlPoints(Shape shape)
		{
			if (shape == null) throw new ArgumentNullException("shape");
			int result = 0;
			foreach (ControlPointId id in shape.GetControlPointIds(ControlPointCapabilities.All))
				++result;
			return result;
		}


		/// <summary>
		/// Checks if the mappings between ConnectionPoints and Terminals can be reused
		/// </summary>
		private void CopyTerminalMappings(IModelObject oldModelObject, IModelObject newModelObject)
		{
			if (oldModelObject == null) throw new ArgumentNullException("oldModelObject");
			if (newModelObject == null) throw new ArgumentNullException("newModelObject");
			foreach (KeyValuePair<ControlPointId, TerminalId> item in connectionPointMappings) {
				string oldTerminalName = oldModelObject.Type.GetTerminalName(item.Value);
				string newTerminalName = newModelObject.Type.GetTerminalName(item.Value);
				if (oldTerminalName != newTerminalName)
					connectionPointMappings[item.Key] = TerminalId.Invalid;
			}
		}

		#region Fields

		private static string entityTypeName = "Core.Template";
		private static string connectionPtMappingName = "ConnectionPointMapping";

		private static string[] connectionPtMappingAttrNames = new string[] {"PointId", "TerminalId"};
		private static Type[] connectionPtMappingAttrTypes = new Type[] {typeof (int), typeof (int)};

		private const string deactivatedTag = "Deactivated";
		private const string activatedTag = "Activated";

		private object id = null;
		private string name;
		private string title;
		private string description;
		private Shape shape;
		private object tag;

		private Dictionary<ControlPointId, TerminalId> connectionPointMappings = new Dictionary<ControlPointId, TerminalId>();
		private SortedList<int, IModelMapping> propertyMappings = new SortedList<int, IModelMapping>();

		#endregion
	}
}