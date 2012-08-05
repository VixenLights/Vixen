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
using System.Drawing;


namespace Dataweb.NShape.Advanced {

	/// <summary>
	/// Simulates a string coming from a resource.
	/// </summary>
	/// <remarks>Later versions will hold a reference to a ResourceManager and read
	/// the string from there.</remarks>
	public class ResourceString {

		/// <ToBeCompleted></ToBeCompleted>
		static public implicit operator ResourceString(string s) {
			return new ResourceString(s);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public ResourceString(string s) {
			value = s;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public string Value {
			get { return value; }
		}


		private string value;

	}


	/// <summary>
	/// Provides services to shapes
	/// </summary>
	public interface IDisplayService {

		/// <summary>
		/// Invalidate the given rectangle.
		/// </summary>
		/// <param name="x">Left side of the rectangle</param>
		/// <param name="y">Top side of the rectangle</param>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		void Invalidate(int x, int y, int width, int height);

		/// <summary>
		/// Invalidate the given rectangle.
		/// </summary>
		/// <param name="rectangle">Rectangle to invalidate.</param>
		void Invalidate(Rectangle rectangle);

		/// <summary>
		/// Update layout according to the changed bounds.
		/// </summary>
		void NotifyBoundsChanged();	// ToDo: Find a better name...

		/// <summary>
		/// Info graphics for mearusing text, etc. Do not dispose!
		/// </summary>
		Graphics InfoGraphics { get; }

		/// <summary>
		/// Fill style for drawing hints.
		/// </summary>
		IFillStyle HintBackgroundStyle { get; }

		/// <summary>
		/// Line style for drawing hints.
		/// </summary>
		ILineStyle HintForegroundStyle { get; }
	}


	/// <summary>
	/// Represents a place, where shapes and model object typea are registered.
	/// </summary>
	/// <status>reviewed</status>
	public interface IRegistrar {

		/// <summary>
		/// Registers a library for shape or model objects.
		/// </summary>
		void RegisterLibrary(string name, int defaultRepositoryVersion);

		/// <summary>
		/// Registers a shape type implemented in the library.
		/// </summary>
		void RegisterShapeType(ShapeType shapeType);

		/// <summary>
		/// Registers a model object type implemented in the library.
		/// </summary>
		void RegisterModelObjectType(ModelObjectType modelObjectType);
	}


	/// <summary>
	/// Encapsulates the configuration on project level.
	/// </summary>
	public class ProjectSettings : IEntity {

		/// <summary>
		/// Constructs a projects projectData instance.
		/// </summary>
		public ProjectSettings() {
		}


		/// <summary>
		/// Empties the projectData.
		/// </summary>
		public void Clear() {
			this.id = null;
			this.lastSaved = DateTime.MinValue;
			this.libraries.Clear();
		}


		/// <summary>
		/// Copies all properties from the given projectData.
		/// </summary>
		/// <param name="source"></param>
		public void CopyFrom(ProjectSettings source) {
			if (source == null) throw new ArgumentNullException("source");
			id = ((IEntity)source).Id;
			lastSaved = source.LastSaved;
			for (int i = 0; i < source.libraries.Count; ++i) {
				if (!libraries.Contains(source.libraries[i]))
					libraries.Add(source.libraries[i]);
			}
		}


		/// <summary>
		/// Defines the date of the last saving of the project.
		/// </summary>
		public DateTime LastSaved {
			get { return lastSaved; }
			set { lastSaved = value; }
		}


		
		/// <summary>
		/// Adds a dynamic library to the project.
		/// </summary>
		public void AddLibrary(string name, string assemblyName, int repositoryVersion) {
			if (name == null) throw new ArgumentNullException("name");
			if (assemblyName == null) throw new ArgumentNullException("assemblyName");
			libraries.Add(new LibraryData(name, assemblyName, repositoryVersion));
		}


		/// <summary>
		/// Retrieves the cache version of the given library.
		/// </summary>
		public int GetRepositoryVersion(string libraryName) {
			if (libraryName == null) throw new ArgumentNullException("libraryName");
			LibraryData ld = FindLibraryData(libraryName, true);
			return ld.RepositoryVersion;
		}


		/// <summary>
		/// Indicates the library assemblies required for the project.
		/// </summary>
		public IEnumerable<string> AssemblyNames {
			get {
				foreach (LibraryData ld in libraries)
					yield return ld.AssemblyName;
			}
		}


		#region IEntity Members

		/// <summary>
		/// Receives the entity type name.
		/// </summary>
		public static string EntityTypeName {
			get { return entityTypeName; }
		}


		/// <summary>
		/// Retrieves the persistable properties of <see cref="T:Dataweb.NShape.Advanced.ProjectSettings" />.
		/// </summary>
		public static IEnumerable<EntityPropertyDefinition> GetPropertyDefinitions(int version) {
			yield return new EntityFieldDefinition("LastSavedUtc", typeof(DateTime));
			yield return new EntityInnerObjectsDefinition("Libraries", "Core.Library", librariesAttrNames, librariesAttrTypes);
		}


		object IEntity.Id {
			get { return id; }
		}


		void IEntity.AssignId(object id) {
			if (id == null) throw new ArgumentNullException("id");
			if (this.id != null)
				throw new InvalidOperationException("Project settings have already an id.");
			this.id = id;
		}


		void IEntity.SaveFields(IRepositoryWriter writer, int version) {
			writer.WriteDate(DateTime.Now);
		}


		void IEntity.LoadFields(IRepositoryReader reader, int version) {
			lastSaved = reader.ReadDate();
		}


		void IEntity.SaveInnerObjects(string propertyName, IRepositoryWriter writer, int version) {
			Project.AssertSupportedVersion(true, version);
			writer.BeginWriteInnerObjects();
			foreach (LibraryData ld in libraries) {
				writer.BeginWriteInnerObject();
				writer.WriteString(ld.Name);
				writer.WriteString(ld.AssemblyName);
				writer.WriteInt32(ld.RepositoryVersion);
				writer.EndWriteInnerObject();
			}
			writer.EndWriteInnerObjects();
		}


		void IEntity.LoadInnerObjects(string propertyName, IRepositoryReader reader, int version) {
			Project.AssertSupportedVersion(false, version);
			reader.BeginReadInnerObjects();
			while (reader.BeginReadInnerObject()) {
				LibraryData ld = new LibraryData(null, null, 0);
				ld.Name = reader.ReadString();
				ld.AssemblyName = reader.ReadString();
				ld.RepositoryVersion = reader.ReadInt32();
				libraries.Add(ld);
				reader.EndReadInnerObject();
			}
			reader.EndReadInnerObjects();
		}


		void IEntity.Delete(IRepositoryWriter writer, int version) {
			foreach (EntityPropertyDefinition pi in GetPropertyDefinitions(version)) {
				if (pi is EntityInnerObjectsDefinition) {
					writer.DeleteInnerObjects();
				}
			}
		}

		#endregion


		private class LibraryData {

			public LibraryData(string name, string assemblyName, int repositoryVersion) {
				Name = name; 
				AssemblyName = assemblyName; 
				RepositoryVersion = repositoryVersion;
			}

			// Specifies the name of the library
			public string Name;
			// Specifies the full assembly name including version and public key token.
			public string AssemblyName;
			// Specifies this library's repository version as used in the project.
			public int RepositoryVersion;
		}


		private LibraryData FindLibraryData(string libraryName, bool throwIfNotFound) {
			LibraryData result = null;
			foreach (LibraryData ld in libraries)
				if (ld.Name.Equals(libraryName, StringComparison.InvariantCultureIgnoreCase)) {
					result = ld;
					break;
				}
			if (result == null && throwIfNotFound) throw new ArgumentException(string.Format("Library '{0}' not found.", libraryName));
			return result;
		}


		#region Fields

		private static string entityTypeName = "Core.Project";
		private static string[] librariesAttrNames = new string[] { "Name", "AssemblyName", "RepositoryVersion" };
		private static Type[] librariesAttrTypes = new Type[] { typeof(string), typeof(string), typeof(int) };

		private object id;
		private DateTime lastSaved;
		private List<LibraryData> libraries = new List<LibraryData>();

		#endregion
	}


	/// <summary>
	/// Helper class used for converting a single instance into an IEnumerable&lt;T&gt;.
	/// </summary>
	public struct SingleInstanceEnumerator<T> : IEnumerable, IEnumerable<T>, IEnumerator<T>, IDisposable {

		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.SingleInstanceEnumerator`1" />.
		/// </summary>
		/// <returns></returns>
		public static SingleInstanceEnumerator<T> Create(T instance) {
			SingleInstanceEnumerator<T> result = SingleInstanceEnumerator<T>.Empty;
			result.instanceReturned = false;
			result.instance = instance;
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static readonly SingleInstanceEnumerator<T> Empty;


		#region IEnumerable<T> Members

		/// <ToBeCompleted></ToBeCompleted>
		public IEnumerator<T> GetEnumerator() {
			return this;
		}

		#endregion


		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator() {
			return this;
		}

		#endregion


		#region IEnumerator<T> Members

		/// <ToBeCompleted></ToBeCompleted>
		public T Current {
			get {
				if (instanceReturned) return default(T);
				else {
					instanceReturned = true;
					return instance;
				}
			}
		}

		#endregion


		#region IEnumerator Members

		object IEnumerator.Current {
			get { return Current; }
		}

		/// <ToBeCompleted></ToBeCompleted>
		public bool MoveNext() {
			return !instanceReturned;
		}

		/// <ToBeCompleted></ToBeCompleted>
		public void Reset() {
			instanceReturned = false;
		}

		#endregion


		#region IDisposable Members

		/// <ToBeCompleted></ToBeCompleted>
		public void Dispose() {
			// nothing to do
		}

		#endregion


		private bool instanceReturned;
		private T instance;
	}


	/// <summary>
	/// Helper class used for creating or comparing empty collections
	/// </summary>
	public struct EmptyEnumerator<T> : IEnumerable, IEnumerable<T>, IEnumerator<T>, IDisposable {

		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Advanced.EmptyEnumerator`1" />.
		/// </summary>
		/// <returns></returns>
		public static EmptyEnumerator<T> Create() {
			EmptyEnumerator<T> result = EmptyEnumerator<T>.Empty;
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static readonly EmptyEnumerator<T> Empty;


		#region IEnumerable<T> Members

		/// <ToBeCompleted></ToBeCompleted>
		public IEnumerator<T> GetEnumerator() {
			return this;
		}

		#endregion


		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator() {
			return this;
		}

		#endregion


		#region IEnumerator<T> Members

		/// <ToBeCompleted></ToBeCompleted>
		public T Current {
			get { return default(T); }
		}

		#endregion


		#region IEnumerator Members

		object IEnumerator.Current {
			get { return default(T); }
		}

		/// <ToBeCompleted></ToBeCompleted>
		public bool MoveNext() {
			return false; ;
		}

		/// <ToBeCompleted></ToBeCompleted>
		public void Reset() {
			// nothing to do
		}

		#endregion


		#region IDisposable Members

		/// <ToBeCompleted></ToBeCompleted>
		public void Dispose() {
			// nothing to do
		}

		#endregion

	}


	/// <summary>
	/// Helper class used for converting collections of <see cref="T:System.Object" /> to collections of &lt;T&gt;.
	/// </summary>
	public struct ConvertEnumerator<T> : IEnumerable, IEnumerable<T>, IEnumerator<T>, IDisposable {

		/// <ToBeCompleted></ToBeCompleted>
		public static ConvertEnumerator<T> Create(IEnumerable enumeration) {
			return Create(enumeration.GetEnumerator());
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static ConvertEnumerator<T> Create(IEnumerator enumerator) {
			ConvertEnumerator<T> result = ConvertEnumerator<T>.Empty;
			result.enumerator = enumerator;
			return result;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static readonly ConvertEnumerator<T> Empty;


		#region IEnumerable<T> Members

		/// <ToBeCompleted></ToBeCompleted>
		public IEnumerator<T> GetEnumerator() {
			return this;
		}

		#endregion


		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator() {
			return this;
		}

		#endregion


		#region IEnumerator<T> Members

		/// <ToBeCompleted></ToBeCompleted>
		public T Current {
			get {
				if (enumerator.Current is T)
					return (T)enumerator.Current;
				else return default(T); 
			}
		}

		#endregion


		#region IEnumerator Members

		object IEnumerator.Current {
			get {
				if (enumerator.Current is T)
					return (T)enumerator.Current;
				else return default(T);
			}
		}

		/// <ToBeCompleted></ToBeCompleted>
		public bool MoveNext() {
			bool result;
			do {
				result = enumerator.MoveNext();
				if (!result) break;
			} while (!(enumerator.Current is T));
			return result;
		}

		/// <ToBeCompleted></ToBeCompleted>
		public void Reset() {
			enumerator.Reset();
		}

		#endregion


		#region IDisposable Members

		/// <ToBeCompleted></ToBeCompleted>
		public void Dispose() {
			// nothing to do
		}

		#endregion


		private IEnumerator enumerator;
	}


	/// <summary>
	/// Helper class used for counting the number of instances in an enumeration.
	/// </summary>
	public static class Counter {

		/// <summary>
		/// Counts the number of instances in an enumeration.
		/// </summary>
		public static int GetCount<T>(IEnumerable<T> instances) {
			if (instances is ICollection)
				return ((ICollection)instances).Count;
			else if (instances is ICollection<T>)
				return ((ICollection<T>)instances).Count;
			else {
				int result = 0;
				IEnumerator<T> enumerator = instances.GetEnumerator();
				while (enumerator.MoveNext()) ++result;
				return result;
			}
		}

	}

}
