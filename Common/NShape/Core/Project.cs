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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Reflection;

using Dataweb.NShape.Advanced;
using Dataweb.NShape.Commands;


namespace Dataweb.NShape {

	/// <summary>
	/// Collection of elements making up a NShape project.
	/// </summary>
	/// <status>reviewed</status>
	[ToolboxItem(true)]
	[ToolboxBitmap(typeof(Project), "Project.bmp")]
	public sealed class Project : Component, IRegistrar, IStyleSetProvider {

		/// <summary>
		/// Checks whether a name is a valid identifier for NShape.
		/// </summary>
		/// <param name="name"></param>
		public static bool IsValidName(string name) {
			if (name == null) return false;
			foreach (char c in name) {
				if (c >= 'a' && c <= 'z') continue;
				if (c >= 'A' && c <= 'Z') continue;
				if (c >= '0' && c <= '9') continue;
				if (c == '_') continue;
				return false;
			}
			return true;
		}


		/// <summary>
		/// Constructs a new project instance.
		/// </summary>
		public Project() {
			Construct();
		}


		/// <summary>
		/// Constructs a new project instance.
		/// </summary>
		/// <param name="container"></param>
		public Project(IContainer container) {
			container.Add(this);
			Construct();
		}


		#region Events

		/// <summary>
		/// Occurs when the the project was opened.
		/// </summary>
		public event EventHandler Opened;

		/// <summary>
		/// Occurs when the the project is going to be closed.
		/// </summary>
		public event EventHandler Closing;

		/// <summary>
		/// Occurs when the project was closed.
		/// </summary>
		public event EventHandler Closed;

		/// <summary>
		/// Occurs when a NShape library was loaded.
		/// </summary>
		public event EventHandler<LibraryLoadedEventArgs> LibraryLoaded;

		/// <summary>
		/// Occurs when styles were changed.
		/// </summary>
		public event EventHandler StylesChanged;

		#endregion


		#region [Public] Properties

		/// <summary>
		/// Specifies the version of the assembly containing the component.
		/// </summary>
		[Category("NShape")]
		public string ProductVersion {
			get { return this.GetType().Assembly.GetName().Version.ToString(); }
		}


		/// <summary>
		/// Specifies the name of the project.
		/// </summary>
		/// <remarks>The name is used as the repository name as well.</remarks>
		[Category("NShape")]
		public string Name {
			get { return name; }
			set {
				name = value;
				if (repository != null) repository.ProjectName = value;
			}
		}


		/// <summary>
		/// Specifies the directories, where NShape libraries are looked for.
		/// </summary>
		[Category("NShape")]
		[Description("A collection of paths where shape and model library assemblies are expected to be found.")]
		[TypeConverter("Dataweb.NShape.WinFormsUI.TextTypeConverter, Dataweb.NShape.WinFormsUI")]
		[Editor("Dataweb.NShape.WinFormsUI.TextUITypeEditor, Dataweb.NShape.WinFormsUI", typeof(UITypeEditor))]
		public IList<string> LibrarySearchPaths {
			get { return librarySearchPaths; }
			set {
				if (value == null) librarySearchPaths = new List<string>();
				else librarySearchPaths = value;
			}
		}


		/// <summary>
		/// Specifies the repository used to store the project.
		/// </summary>
		[Category("NShape")]
		[Description("Specifies the IRepository class used for loading/saving project and diagram data.")]
		public IRepository Repository {
			get { return repository; }
			set {
				if (IsOpen) throw new InvalidOperationException("Project is still open.");
				if (repository != null) {
					repository.StyleUpdated -= repository_StyleUpdated;
					repository.ProjectName = string.Empty;
				}
				repository = value;
				if (repository != null) {
					repository.ProjectName = name;
					repository.StyleUpdated += repository_StyleUpdated;
					repository.StyleDeleted += repository_StyleDeleted;
				}
			}
		}


		/// <summary>
		/// Specifies whether the project creates templates for each item in registered 
		/// shape and model libraries.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(true)]
		public bool AutoGenerateTemplates {
			get { return autoCreateTemplates; }
			set { autoCreateTemplates = value; }
		}


		/// <summary>
		/// Specifies whether the project automatically loads missing libraries when opening a repository.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(false)]
		public bool AutoLoadLibraries {
			get { return autoLoadLibraries; }
			set { autoLoadLibraries = value; }
		}


		/// <summary>
		/// Provides access to the registered model object types.
		/// </summary>
		[Browsable(false)]
		public IReadOnlyModelObjectTypeCollection ModelObjectTypes {
			get { return modelObjectTypes; }
		}


		/// <summary>
		/// Provides access to the registered shape types.
		/// </summary>
		[Browsable(false)]
		public IReadOnlyShapeTypeCollection ShapeTypes {
			get { return shapeTypes; }
		}


		/// <summary>
		/// Provides undo/redo capability for the project.
		/// </summary>
		[Browsable(false)]
		public History History {
			get { return history; }
		}


		/// <summary>
		/// Specifies the security manager used with this project.
		/// </summary>
		[Browsable(false)]
		public ISecurityManager SecurityManager {
			get { return security; }
			set {
				AssertClosed();
				if (value == null) throw new ArgumentNullException("Security");
				security = value;
			}
		}


		/// <summary>
		/// Accesses the project settings.
		/// </summary>
		[Browsable(false)]
		public ProjectSettings Settings {
			get { return settings; }
		}


		/// <summary>
		/// Accesses the styles used for the project.
		/// </summary>
		[Browsable(false)]
		public Design Design {
			get {
				AssertOpen();
				return repository.GetDesign(null);
			}
		}


		/// <summary>
		/// Retrieves the registered libraries.
		/// </summary>
		[Browsable(false)]
		public IEnumerable<Assembly> Libraries {
			get { foreach (Library l in libraries) yield return l.Assembly; }
		}

		#endregion


		#region [Public] Methods

		/// <summary>
		/// Uses the given design for the project.
		/// </summary>
		/// <param name="newDesign"></param>
		public void ApplyDesign(Design newDesign) {
			if (newDesign == null) throw new ArgumentNullException("newDesign");
			Design design = repository.GetDesign(null);
			bool styleFound = false;
			foreach (IStyle style in newDesign.Styles) {
				styleFound = design.AssignStyle(style);
				IStyle s = design.FindStyleByName(style.Name, style.GetType());
				if (styleFound) repository.Update(s);
				else repository.Insert(design, s);
			}
			repository.Update(design);
			repository.Update();

			if (StylesChanged != null) StylesChanged(this, EventArgs.Empty);
		}


		/// <summary>
		/// Uses the given design for the project.
		/// </summary>
		public void ApplyDesign(string designName) {
			if (designName == null) throw new ArgumentNullException("designName");
			Design design = null;
			foreach (Design d in repository.GetDesigns()) {
				if (designName.Equals(d.Name, StringComparison.InvariantCultureIgnoreCase)) {
					design = d;
					break;
				}
			}
			if (design == null)
				throw new NShapeException("A design named '{0}' does not exist.", designName);
			ApplyDesign(design);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public bool IsValidLibrary(string assemblyPath) {
			try {
				string fullAssemblyPath = GetFullAssemblyPath(assemblyPath);
				AssemblyName assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
				if (assemblyName != null) {
					Assembly assembly = Assembly.Load(assemblyName.FullName);
					return (GetInitializerType(assembly) != null);
				} else return false;
			} catch (Exception) {
				return false;
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		public bool IsValidLibrary(Assembly assembly) {
			try {
				return (GetInitializerType(assembly) != null);
			} catch (Exception) {
				return false;
			}
		}


		/// <summary>
		/// Adds the given library assembly to the project.
		/// </summary>
		/// <param name="assembly">Shape or model object library assembly to add.</param>
		/// <param name="unloadOnClose">
		/// Specifies whether the library should be unloaded when closing the project.
		/// When setting AutoLoadLibraries to false, libraries should not be unloaded when closing the project.
		/// </param>
		public void AddLibrary(Assembly assembly, bool unloadOnClose) {
			if (assembly == null) throw new ArgumentNullException("assembly");
			DoLoadLibrary(assembly, unloadOnClose);
			if (IsOpen) {
				DoRegisterLibrary(FindLibraryByAssemblyName(assembly.FullName), true);
				repository.Update();
			}
			if (LibraryLoaded != null) LibraryLoaded(this, new LibraryLoadedEventArgs(assembly.FullName));
		}


		/// <summary>
		/// Adds the library assembly with the given assembly name to the project.
		/// </summary>
		/// <param name="assemblyName">Shape library's assembly name.</param>
		/// <param name="unloadOnClose">
		/// Specifies whether the library should be unloaded when closing the project.
		/// When setting AutoLoadLibraries to false, libraries should not be unloaded when closing the project.
		/// </param>
		public void AddLibraryByName(string assemblyName, bool unloadOnClose) {
			if (string.IsNullOrEmpty(assemblyName)) throw new ArgumentNullException("assemblyName");
			Assembly a = Assembly.Load(assemblyName);
			AddLibrary(a, unloadOnClose);
		}


		/// <summary>
		/// Adds the library assembly at the given location to the project.
		/// </summary>
		/// <param name="assemblyPath">Complete file path to the library assembly.</param>
		/// <param name="unloadOnClose">
		/// Specifies whether the library should be unloaded when closing the project.
		/// When setting AutoLoadLibraries to false, libraries should not be unloaded when closing the project.
		/// </param>
		public void AddLibraryByFilePath(string assemblyPath, bool unloadOnClose) {
			Assembly a = Assembly.LoadFile(GetFullAssemblyPath(assemblyPath));
			AddLibrary(a, unloadOnClose);
		}


		/// <summary>
		/// Unloads and removes the given library.
		/// </summary>
		public void RemoveLibrary(Assembly assembly) {
			for (int i = libraries.Count - 1; i >= 0; --i) {
				if (libraries[i].Assembly == assembly)
					RemoveLibrary(libraries[i]);
			}
		}


		/// <summary>
		/// Unloads and removes all libraries.
		/// </summary>
		public void RemoveAllLibraries() {
			AssertClosed();
			shapeTypes.Clear();
			modelObjectTypes.Clear();
			libraries.Clear();
		}


		/// <summary>
		/// Executes a command and adds it to the project's history.
		/// </summary>
		/// <param name="command"></param>
		public void ExecuteCommand(ICommand command) {
			if (command == null) throw new ArgumentNullException("command");
			Exception exc = command.CheckAllowed(security);
			if (exc != null) throw exc;
			command.Repository = repository;
			history.ExecuteAndAddCommand(command);
		}


		/// <summary>
		/// Opens a new project.
		/// </summary>
		public void Create() {
			DoOpen(true);
		}


		/// <summary>
		/// Opens an existing project.
		/// </summary>
		public void Open() {
			DoOpen(false);
		}


		/// <summary>
		/// Closes the project.
		/// </summary>
		public void Close() {
			if (Closing != null) Closing(this, EventArgs.Empty);
			if (IsOpen) {
				if (repository.IsOpen) {
					// Get styleSet before closing the repository
					IStyleSet styleSet = ((IStyleSetProvider)this).StyleSet;
					// Delete GDI+ objects created from styles
					ToolCache.RemoveStyleSetTools(styleSet);

					// Close Repository
					repository.Close();
				}
				// Unload libraries that were loaded when opening the project
				for (int i = libraries.Count - 1; i >= 0; --i)
					if (libraries[i].UnloadOnClose)
						libraries.RemoveAt(i);
				// Clean up project data
				settings.Clear();
				model = null;
				history.Clear();
				// Create new settings
				settings = new ProjectSettings();
				// TODO 2: Unload dynamic libraries and remove the corresponding shape and model types.
				//RemoveAllLibraries();
			}
			if (Closed != null) Closed(this, EventArgs.Empty);
		}


		/// <summary>
		/// Indicates whether the project is open.
		/// </summary>
		[Browsable(false)]
		public bool IsOpen {
			get { return repository != null && repository.IsOpen; }
		}


		/// <summary>
		/// Registers all entities with the repository.
		/// </summary>
		public void RegisterEntityTypes() {
			RegisterEntityTypesCore(true);
		}

		#endregion


		#region IRegistrar Members

		/// <override></override>
		void IRegistrar.RegisterLibrary(string name, int defaultRepositoryVersion) {
			if (!Project.IsValidName(name)) throw new ArgumentException(string.Format("'{0}' is not a valid library name.", name));
			initializingLibrary.Name = name;
			if (addingLibrary) settings.AddLibrary(name, initializingLibrary.Assembly.FullName, defaultRepositoryVersion);
		}


		/// <override></override>
		void IRegistrar.RegisterShapeType(ShapeType shapeType) {
			if (initializingLibrary == null)
				throw new InvalidOperationException("RegisterShapeType can only be called while a library is initializing.");
			if (string.IsNullOrEmpty(initializingLibrary.Name))
				throw new InvalidOperationException("RegisterLibrary has not been called or the library has an empty library name.");
			if (shapeType == null) throw new ArgumentNullException("shapeType");
			if (shapeType.LibraryName != initializingLibrary.Name)
				throw new InvalidOperationException(string.Format("The library name of shape type '{0}' is '{1}' instead of '{2}'.", shapeType.GetType().Name, shapeType.LibraryName, initializingLibrary.Name));
			//
			shapeType.StyleSetProvider = this;
			shapeTypes.Add(shapeType);
			// If the cache is not open, the following actions will be performed
			// when opening it.
			if (repository != null && repository.IsOpen) {
				RegisterShapeEntityType(shapeType, addingLibrary);
				if (autoCreateTemplates && addingLibrary) CreateDefaultTemplate(shapeType);
			}
		}


		/// <override></override>
		void IRegistrar.RegisterModelObjectType(ModelObjectType modelObjectType) {
			if (initializingLibrary == null)
				throw new InvalidOperationException("RegisterModelObjectType can only be called while a library is initializing.");
			if (string.IsNullOrEmpty(initializingLibrary.Name))
				throw new InvalidOperationException("RegisterLibrary has not been called or the library has an empty library name.");
			if (modelObjectType == null) throw new ArgumentNullException("modelObjectType");
			if (!Project.IsValidName(modelObjectType.Name))
				throw new ArgumentException("'{0}' is not a valid model object type name.", modelObjectType.Name);
			if (modelObjectType.LibraryName != initializingLibrary.Name)
				throw new InvalidOperationException("All model objects of a registering library must have the library's library name.");
			if (modelObjectType.LibraryName != initializingLibrary.Name)
				throw new InvalidOperationException(string.Format("The library name of model object type '{0}' is '{1}' instead of '{2}'.", modelObjectType.GetType().Name, modelObjectType.LibraryName, initializingLibrary.Name));
			//
			modelObjectTypes.Add(modelObjectType);
			// Create a delegate that adds required parameters to the CreateModelObjectDelegate 
			// of the shape when called
			if (repository != null && repository.IsOpen) {
				RegisterModelObjectEntityType(modelObjectType, addingLibrary);
			}
		}

		#endregion


		#region IStyleSetProvider Members

		/// <override></override>
		[Browsable(false)]
		IStyleSet IStyleSetProvider.StyleSet {
			get {
				AssertOpen();
				return (IStyleSet)repository.GetDesign(null);
			}
		}

		#endregion


		/// <ToBeCompleted></ToBeCompleted>
		internal static void AssertSupportedVersion(bool save, int version) {
			int minVersion = save ? FirstSupportedSaveVersion : FirstSupportedLoadVersion;
			int maxVersion = save ? LastSupportedSaveVersion : LastSupportedLoadVersion;
			if (version < minVersion || version > maxVersion) {
				string msg = string.Format("{0}ing repository failed: The repository is version {1} but this application only supports repositories version {2} to {3}.",
					(save) ? "Save" : "Load", version, minVersion, maxVersion);
				throw new NShapeException(msg);
			}
		}


		#region [Private] Library Class

		/// <summary>
		/// Describes a shape or model object library.
		/// </summary>
		private class Library {

			public Library(Assembly assembly)
				: this(assembly, true) {
			}


			public Library(Assembly assembly, bool unloadOnClose) {
				if (assembly == null) throw new ArgumentNullException("assembly");
				this.assembly = assembly;
				this.name = null;
				this.unloadOnClose = unloadOnClose;
			}


			/// <summary>
			/// User-defined name to identify the library.
			/// </summary>
			public string Name {
				get { return name; }
				set { name = value; }
			}


			/// <summary>
			/// Specifies the assembly used for the library.
			/// </summary>
			public Assembly Assembly {
				get { return assembly; }
				set { assembly = value; }
			}


			/// <summary>
			/// Specifies whether the library will be unloaded when the project is closed.
			/// </summary>
			public bool UnloadOnClose {
				get { return unloadOnClose; }
			}


			#region Fields

			private string name;
			private Assembly assembly;
			private bool unloadOnClose = true;

			#endregion
		}

		#endregion


		#region [Private] Implementation

		private void Construct() {
			settings = new ProjectSettings();
			history = new History();
			registerArgs = new object[1] { ((IRegistrar)this) };
		}


		private void AssertOpen() {
			if (!IsOpen) throw new InvalidOperationException("Project must be open to execute this operation.");
		}


		private void AssertClosed() {
			if (IsOpen) throw new InvalidOperationException("Project must not be open to execute this operation.");
		}


		private void RegisterShapeEntityType(ShapeType shapeType, bool create) {
			int version = FindLibraryVersion(shapeType.LibraryName, create);
			IEntityType entityType = new EntityType(shapeType.FullName, EntityCategory.Shape,
				version, () => shapeType.CreateInstanceForLoading(), shapeType.GetPropertyDefinitions(version));
			repository.AddEntityType(entityType);
		}


		private void RegisterModelObjectEntityType(ModelObjectType modelObjectType, bool create) {
			int version = FindLibraryVersion(modelObjectType.LibraryName, create);
			IEntityType entityType = new EntityType(modelObjectType.FullName, EntityCategory.ModelObject,
				version, () => modelObjectType.CreateInstance(), modelObjectType.GetPropertyDefinitions(version));
			repository.AddEntityType(entityType);
		}


		private Library FindLibraryByAssemblyName(string assemblyName) {
			// Check whether already loaded
			AssemblyName soughtAssemblyName = new AssemblyName(assemblyName);
			foreach (Library l in libraries) {
				AssemblyName libAssemblyName = l.Assembly.GetName();
				if (AssemblyName.ReferenceMatchesDefinition(soughtAssemblyName, libAssemblyName))
					return l;
			}
			return null;
		}


		private Assembly FindAssemblyInSearchPath(string assemblyName) {
			if (!autoLoadLibraries)
				return null;
			AssemblyName soughtAssemblyName = new AssemblyName(assemblyName);
			int cnt = LibrarySearchPaths.Count;
			for (int pathIdx = 0; pathIdx < cnt; ++pathIdx) {
				string[] files = Directory.GetFiles(LibrarySearchPaths[pathIdx], "*.dll");
				for (int fileIdx = files.Length - 1; fileIdx >= 0; --fileIdx) {
					try {
						AssemblyName foundAssemblyName = AssemblyName.GetAssemblyName(files[fileIdx]);
						if (AssemblyName.ReferenceMatchesDefinition(soughtAssemblyName, foundAssemblyName)) {
							// Use Assembly.Load() here, because otherwise the assembly will be loaded into a 
							// different context which results in type conversion problems.
							// See http://blogs.msdn.com/b/aszego/archive/2009/10/16/avoid-using-assembly-loadfrom.aspx
							// for a detailed description on the problem.
							return Assembly.Load(foundAssemblyName);
						}
					} catch (BadImageFormatException ex) {
						Debug.Print(string.Format("An exception occured while searching assembly '{0}' in path {1}: {2}",
							assemblyName, files[fileIdx], ex.Message));
					} catch (NotImplementedException ex) {
						Debug.Print(string.Format("A NotImplementedException occured while searching assembly '{0}' in path {1}: {2}",
							assemblyName, files[fileIdx], ex.Message));
					}
				}
			}
			return null;
		}


		private void DoOpen(bool create) {
			if (IsOpen)
				throw new InvalidOperationException("Project is already open.");
			if (string.IsNullOrEmpty(Name))
				throw new InvalidOperationException("No name defined for the project.");
			// Create/Reset repository
			if (repository == null) {
				repository = new CachedRepository();
				repository.ProjectName = Name;
			} else {
				Debug.Assert(!repository.IsOpen);
				repository.Close();
				repository.RemoveAllEntityTypes();
			}
			modelObjectTypes.Clear();
			shapeTypes.Clear();
			if (create) {
				// Store libraries that should not be unloaded so we can reload them
				// right after re-creating the project settings
				List<Library> reloadLibs = new List<Library>(libraries);
				libraries.Clear();
				// Register core library
				repository.Version = LastSupportedSaveVersion;
				RegisterEntityTypesCore(true);

				// Create repository and model
				repository.Create();
				model = new Model();
				repository.Insert(model);
				// Re-initialize libraries
				try {
					settings = repository.GetProject();
					if (reloadLibs.Count > 0) {
						foreach (Library lib in reloadLibs) {
							libraries.Add(lib);
							DoRegisterLibrary(lib, true);
						}
						repository.Update();
					}
				} catch (Exception exc) {
					Debug.Print(exc.Message);
					repository.Close();
					throw;
				}
			} else {
				// We unload all shape and model object types here. Only the ones defined by the
				// project will be usable.
				repository.ReadVersion();
				if (repository.Version <= 0)
					throw new NShapeException("Invalid repository base version: {0}", repository.Version);
				RegisterBaseLibraryTypes(false);
				repository.Open();
				try {
					settings = repository.GetProject();
					// Load the project libraries
					foreach (string ln in settings.AssemblyNames) {
						Library lib = FindLibraryByAssemblyName(ln);
						if (lib == null) {
							if (autoLoadLibraries) {
								Assembly a = null;
								try {
									a = Assembly.Load(ln);
								} catch (FileLoadException exc) {
									a = FindAssemblyInSearchPath(ln);
									if (a == null) throw new LoadLibraryException(ln, exc);
								} catch (FileNotFoundException exc) {
									a = FindAssemblyInSearchPath(ln);
									if (a == null) throw new LoadLibraryException(ln, exc);
								}
								Debug.Assert(a != null);
								lib = DoLoadLibrary(a, true);
							} else throw new LoadLibraryException(ln);
						}
						Debug.Assert(lib != null);
						DoRegisterLibrary(lib, false);
					}
				} catch (Exception exc) {
					Debug.Print(exc.Message);
					repository.Close();
					throw;
				}
			}
			if (Opened != null) Opened(this, EventArgs.Empty);
		}


		private void RegisterEntityTypesCore(bool create) {
			repository.RemoveAllEntityTypes();
			RegisterBaseLibraryTypes(create);
			foreach (Library l in libraries)
				DoRegisterLibrary(l, true);

			// Register static model entity types
			foreach (ModelObjectType mot in modelObjectTypes)
				if (!mot.LibraryName.Equals("Core", StringComparison.InvariantCultureIgnoreCase))
					RegisterModelObjectEntityType(mot, create);
			// Register static shape entity types
			foreach (ShapeType st in shapeTypes)
				if (!st.LibraryName.Equals("Core", StringComparison.InvariantCultureIgnoreCase))
					RegisterShapeEntityType(st, create);
		}


		// Registers entity types for styles, designs, projectData, templates and diagramControllers 
		// with the cache.
		private void RegisterBaseLibraryTypes(bool create) {
			int version;
			// When creating a repository without a valid version, we use the last supported save version.
			if (create && repository.Version <= 0)
				repository.Version = LastSupportedSaveVersion;
			version = repository.Version;
			//
			repository.AddEntityType(new EntityType(CapStyle.EntityTypeName, EntityCategory.Style,
				version, () => new CapStyle(), CapStyle.GetPropertyDefinitions(version)));
			repository.AddEntityType(new EntityType(CharacterStyle.EntityTypeName, EntityCategory.Style,
				version, () => new CharacterStyle(), CharacterStyle.GetPropertyDefinitions(version)));
			repository.AddEntityType(new EntityType(ColorStyle.EntityTypeName, EntityCategory.Style,
				version, () => new ColorStyle(), ColorStyle.GetPropertyDefinitions(version)));
			repository.AddEntityType(new EntityType(FillStyle.EntityTypeName, EntityCategory.Style,
				version, () => new FillStyle(), FillStyle.GetPropertyDefinitions(version)));
			repository.AddEntityType(new EntityType(LineStyle.EntityTypeName, EntityCategory.Style,
				version, () => new LineStyle(), LineStyle.GetPropertyDefinitions(version)));
			repository.AddEntityType(new EntityType(ParagraphStyle.EntityTypeName, EntityCategory.Style,
				version, () => new ParagraphStyle(), ParagraphStyle.GetPropertyDefinitions(version)));
			repository.AddEntityType(new EntityType(Design.EntityTypeName, EntityCategory.Design,
				version, () => new Design(), Design.GetPropertyDefinitions(version)));
			repository.AddEntityType(new EntityType(ProjectSettings.EntityTypeName, EntityCategory.ProjectSettings,
				version, () => new ProjectSettings(), ProjectSettings.GetPropertyDefinitions(version)));
			repository.AddEntityType(new EntityType(Template.EntityTypeName, EntityCategory.Template,
				version, () => new Template(), Template.GetPropertyDefinitions(version)));
			repository.AddEntityType(new EntityType(Diagram.EntityTypeName, EntityCategory.Diagram,
				version, () => new Diagram(""), Diagram.GetPropertyDefinitions(version)));
			// Register ModelMapping types
			// Create mandatory Model type
			repository.AddEntityType(new EntityType(Model.EntityTypeName, EntityCategory.Model,
				version, () => new Model(), Model.GetPropertyDefinitions(version)));
			// Register mandatory ModelMapping types
			repository.AddEntityType(new EntityType(NumericModelMapping.EntityTypeName, EntityCategory.ModelMapping,
				version, () => new NumericModelMapping(), NumericModelMapping.GetPropertyDefinitions(version)));
			repository.AddEntityType(new EntityType(FormatModelMapping.EntityTypeName, EntityCategory.ModelMapping,
				version, () => new FormatModelMapping(), FormatModelMapping.GetPropertyDefinitions(version)));
			repository.AddEntityType(new EntityType(StyleModelMapping.EntityTypeName, EntityCategory.ModelMapping,
				version, () => new StyleModelMapping(), StyleModelMapping.GetPropertyDefinitions(version)));

			//
			// Create the mandatory shape types
			initializingLibrary = new Library(GetType().Assembly);
			((IRegistrar)this).RegisterLibrary("Core", LastSupportedSaveVersion);
			ShapeType groupShapeType = new ShapeType(
				"ShapeGroup", "Core", "Core", ShapeGroup.CreateInstance, ShapeGroup.GetPropertyDefinitions, false);
			((IRegistrar)this).RegisterShapeType(groupShapeType);
			// Create mandatory model object types
			ModelObjectType genericModelObjectType = new GenericModelObjectType(
				"GenericModelObject", "Core", "Core", GenericModelObject.CreateInstance, GenericModelObject.GetPropertyDefinitions, 4);
			((IRegistrar)this).RegisterModelObjectType(genericModelObjectType);
			initializingLibrary = null;
			//
			// Register static model entity types
			foreach (ModelObjectType mot in modelObjectTypes)
				RegisterModelObjectEntityType(mot, create);
			// Register static shape entity types
			foreach (ShapeType st in shapeTypes)
				RegisterShapeEntityType(st, create);
		}


		private Library DoLoadLibrary(Assembly a, bool unloadOnCLose) {
			if (a == null) throw new ArgumentNullException("a");
			Library result = FindLibraryByAssemblyName(a.FullName);
			if (result != null) throw new InvalidOperationException(string.Format("Library '{0}' is already loaded.", a.FullName));
			result = new Library(a, unloadOnCLose);
			libraries.Add(result);
			return result;
		}


		private void DoRegisterLibrary(Library library, bool adding) {
			addingLibrary = adding;
			initializingLibrary = library;
			try {
				InitializeLibrary(library);
			} catch (Exception exc) {
				DoRemoveLibrary(library);
				throw exc;
			} finally {
				addingLibrary = false;
				initializingLibrary = null;
			}
		}


		// ToDo: Implement RemoveLibrary, RemoveLibraryByPath and RemoveLibraryByName
		private void DoRemoveLibrary(Library l) {
			if (l == null) throw new ArgumentNullException("l");
			libraries.Remove(l);
		}


		private void RemoveLibrary(Library library) {
			if (library == null) throw new ArgumentNullException("library");
			List<ShapeType> registeredShapeTypes = new List<ShapeType>(GetRegisteredShapeTypes(library));
			List<ModelObjectType> registeredModelObjectTypes = new List<ModelObjectType>(GetRegisteredModelObjectTypes(library));
			List<Template> registeredShapeTypeTemplates = new List<Template>();
			bool canRemove = true;
			// Get and check all corresponding templates if they are referenced by shapes.
			foreach (Template template in Repository.GetTemplates()) {
				for (int i = registeredShapeTypes.Count - 1; i >= 0; --i) {
					if (IsShapeOfType(template.Shape, registeredShapeTypes[i])) {
						if (Repository.IsTemplateInUse(template))
							canRemove = false;
						else registeredShapeTypeTemplates.Add(template);
						if (canRemove == false) break;
					}
					if (canRemove == false) break;
				}
			}
			// Check if there are shapes without templates 
			if (Repository.IsShapeTypeInUse(registeredShapeTypes)) {
				canRemove = false;
				throw new NShapeException("Unable to unload library '{0}': At least one shape type of the library is still in use.", library.Assembly.GetName().Name);
			}
			if (Repository.IsModelObjectTypeInUse(registeredModelObjectTypes)) {
				canRemove = false;
				throw new NShapeException("Unable to unload library '{0}': At least one model object type of the library is still in use.", library.Assembly.GetName().Name);
			}

			if (canRemove) {
				// Remove (unused) Templates
				for (int i = registeredShapeTypeTemplates.Count - 1; i >= 0; --i)
					Repository.DeleteAll(registeredShapeTypeTemplates[i]);
				// Remove Shape EntityTypes
				for (int i = registeredShapeTypes.Count - 1; i >= 0; --i) {
					shapeTypes.Remove(registeredShapeTypes[i]);
					repository.RemoveEntityType(registeredShapeTypes[i].FullName);
				}
				// Remove Model EntityTypes
				for (int i = registeredModelObjectTypes.Count - 1; i >= 0; --i) {
					modelObjectTypes.Remove(registeredModelObjectTypes[i]);
					repository.RemoveEntityType(registeredModelObjectTypes[i].FullName);
				}
				libraries.Remove(library);

				// Clear the history in order to prevent the user to undo 
				// changes that require the removed shape types.
				if (history != null) history.Clear();
			}
		}


		private bool IsShapeOfType(Shape shape, ShapeType shapeType) {
			if (shape == null) throw new ArgumentNullException("shape");
			if (shapeType == null) throw new ArgumentNullException("shapeType");
			bool result = false;
			if (shape.Type == shapeType)
				result = true;
			else {
				foreach (Shape childShape in shape.Children)
					if (IsShapeOfType(childShape, shapeType)) {
						result = true;
						break;
					}
			}
			return result;
		}


		private IEnumerable<ShapeType> GetRegisteredShapeTypes(Library library) {
			return GetRegisteredShapeTypes(SingleInstanceEnumerator<Library>.Create(library));
		}


		private IEnumerable<ShapeType> GetRegisteredShapeTypes(IEnumerable<Library> libraries) {
			foreach (ShapeType shapeType in shapeTypes) {
				foreach (Library library in libraries) {
					if (shapeType.LibraryName == library.Name)
						yield return shapeType;
				}
			}
		}


		private IEnumerable<ModelObjectType> GetRegisteredModelObjectTypes(Library library) {
			return GetRegisteredModelObjectTypes(SingleInstanceEnumerator<Library>.Create(library));
		}


		private IEnumerable<ModelObjectType> GetRegisteredModelObjectTypes(IEnumerable<Library> libraries) {
			foreach (ModelObjectType modelObjectType in modelObjectTypes) {
				foreach (Library library in libraries) {
					if (modelObjectType.LibraryName == library.Name)
						yield return modelObjectType;
				}
			}
		}


		private string GetFullAssemblyPath(string assemblyPath) {
			if (assemblyPath == null) throw new ArgumentNullException("libraryFilePath");
			if (!Path.HasExtension(assemblyPath)) assemblyPath += ".dll";
			if (!Path.IsPathRooted(assemblyPath)) {
				string libDir = this.GetType().Assembly.Location;
				assemblyPath = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(libDir)), Path.GetFileName(assemblyPath));
			}
			if (!File.Exists(assemblyPath) && autoLoadLibraries) {
				string assemblyFileName = Path.GetFileName(assemblyPath);
				string libPath = string.Empty;
				foreach (string dir in LibrarySearchPaths) {
					libPath = dir;
					if (!libPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
						libPath += Path.DirectorySeparatorChar;
					if (File.Exists(libPath + assemblyFileName)) {
						assemblyPath = libPath + assemblyFileName;
						break;
					}
				}
				if (!File.Exists(assemblyPath))
					throw new NShapeException("Assembly '{0}' cannot be found at the specified path.", assemblyPath);
			}
			return assemblyPath;
		}


		private Type GetInitializerType(Assembly assembly) {
			if (assembly == null) throw new ArgumentNullException("assembly");
			Type initializerType = null;
			foreach (Type t in assembly.GetTypes()) {
				if (t is Type) {
					if (t.Name.Equals(NShapeLibraryInitializerClassName, StringComparison.InvariantCultureIgnoreCase)) {
						initializerType = t;
						break;
					}
				}
			}
			if (initializerType == null)
				throw new ArgumentException(string.Format("Assembly '{0}' is not a NShape library. (It does not implement static class {1}).", assembly.Location, NShapeLibraryInitializerClassName));
			MethodInfo methodInfo = initializerType.GetMethod(InitializeMethodName);
			if (methodInfo == null)
				throw new ArgumentException(string.Format("Assembly '{0}' is not a NShape library. (It does not implement {1}.{2}).", assembly.FullName, NShapeLibraryInitializerClassName, InitializeMethodName));
			return initializerType;
		}


		private void InitializeLibrary(Library library) {
			Type initializerType = GetInitializerType(library.Assembly);
			this.initializingLibrary = library;
			try {
				initializerType.InvokeMember(InitializeMethodName, BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Static, null, null, registerArgs);
			} catch (TargetInvocationException ex) {
				throw ex.InnerException;
			} finally {
				initializingLibrary = null;
			}
		}


		/// <summary>
		/// For each loaded Type, a new <see cref="T:Dataweb.NShape.Advanced.Template" /> is created.
		/// These automatically created templates are not stored by the cache.
		/// </summary>
		/// <param name="shapeType"></param>
		private void CreateDefaultTemplate(ShapeType shapeType) {
			if (shapeType.SupportsAutoTemplates) {
				Template template = new Template(shapeType.Name, shapeType.CreateInstance());
				Repository.InsertAll(template);
			}
		}


		// Notify ToolCache that a style has changed
		// Invalidate all (loaded) shapes using the changed style
		private void repository_StyleUpdated(object sender, RepositoryStyleEventArgs e) {
			IStyle changedStyle = e.Style;
			Design design = null;
			foreach (Design d in repository.GetDesigns()) {
				if (d.ContainsStyle(changedStyle)) {
					design = d;
					break;
				}
			}
			// Update Toolcache and PreviewStyle
			if (design != null) {
				if (changedStyle is CapStyle) {
					DoNotifyStyleChanged(design.CapStyles, (CapStyle)changedStyle);
					design.CapStyles.SetPreviewStyle((CapStyle)changedStyle, design.CreatePreviewStyle((ICapStyle)changedStyle));
				} else if (changedStyle is CharacterStyle) {
					DoNotifyStyleChanged(design.CharacterStyles, (CharacterStyle)changedStyle);
					design.CharacterStyles.SetPreviewStyle((CharacterStyle)changedStyle, design.CreatePreviewStyle((ICharacterStyle)changedStyle));
				} else if (changedStyle is ColorStyle) {
					DoNotifyStyleChanged(design.ColorStyles, (ColorStyle)changedStyle);
					design.ColorStyles.SetPreviewStyle((ColorStyle)changedStyle, design.CreatePreviewStyle((IColorStyle)changedStyle));
				} else if (changedStyle is FillStyle) {
					DoNotifyStyleChanged(design.FillStyles, (FillStyle)changedStyle);
					design.FillStyles.SetPreviewStyle((FillStyle)changedStyle, design.CreatePreviewStyle((IFillStyle)changedStyle));
				} else if (changedStyle is LineStyle) {
					DoNotifyStyleChanged(design.LineStyles, (LineStyle)changedStyle);
					design.LineStyles.SetPreviewStyle((LineStyle)changedStyle, design.CreatePreviewStyle((ILineStyle)changedStyle));
				} else if (changedStyle is ParagraphStyle) {
					DoNotifyStyleChanged(design.ParagraphStyles, (ParagraphStyle)changedStyle);
					design.ParagraphStyles.SetPreviewStyle((ParagraphStyle)changedStyle, design.CreatePreviewStyle((IParagraphStyle)changedStyle));
				}
			}

			// If the style is contained in the current design, notify all shapes that the style has changed
			if (design == Design) {
				foreach (Template template in repository.GetTemplates())
					template.Shape.NotifyStyleChanged(changedStyle);
				foreach (Diagram diagram in repository.GetDiagrams()) {
					foreach (Shape shape in diagram.Shapes)
						shape.NotifyStyleChanged(changedStyle);
				}
			}
		}


		private void DoNotifyStyleChanged<TStyle>(StyleCollection<TStyle> styleCollection, TStyle style) where TStyle : class, IStyle {
			if (styleCollection == null) throw new ArgumentNullException("styleCollection");
			if (style == null) throw new ArgumentNullException("style");
			// create and set new PreviewStyle if the style is in the currently active design
			if (styleCollection.ContainsPreviewStyle(style)) {
				TStyle previewStyle = styleCollection.GetPreviewStyle(style);
				Debug.Assert(previewStyle != null);
				ToolCache.NotifyStyleChanged(previewStyle);
			}
			ToolCache.NotifyStyleChanged(style);
		}


		private void repository_StyleDeleted(object sender, RepositoryStyleEventArgs e) {
			ToolCache.NotifyStyleChanged(e.Style);
		}


		// Determines the repository version to use for the given library.
		private int FindLibraryVersion(string libraryName, bool create) {
			int result;
			if (libraryName.Equals("Core", StringComparison.InvariantCultureIgnoreCase))
				result = repository.Version;
			else result = settings.GetRepositoryVersion(libraryName);
			if (result <= 0) throw new Exception(string.Format("Invalid repository version {0}.", result));
			return result;
		}

		#endregion


		// Supported repository versions of the Core library
		internal const int FirstSupportedSaveVersion = 1;
		internal const int LastSupportedSaveVersion = 4;
		internal const int FirstSupportedLoadVersion = 1;
		internal const int LastSupportedLoadVersion = 4;

		/// <ToBeCompleted></ToBeCompleted>
		public const string NShapeLibraryInitializerClassName = "NShapeLibraryInitializer";
		/// <ToBeCompleted></ToBeCompleted>
		public const string InitializeMethodName = "Initialize";

		#region Fields

		private const string entityTypeName = "Project";
		private string[] attributeNames = new string[] { "Name", "Date", "DesignId" };

		// -- Constituting Sub-Objects --
		private ShapeTypeCollection shapeTypes = new ShapeTypeCollection();
		private ModelObjectTypeCollection modelObjectTypes = new ModelObjectTypeCollection();
		private IRepository repository = null;
		private History history = null;
		private ISecurityManager security = new RoleBasedSecurityManager();

		// -- Properties --
		private string name;
		private bool autoCreateTemplates = true;
		private bool autoLoadLibraries = false;
		private ProjectSettings settings;
		private Model model = null;
		private IList<string> librarySearchPaths = new List<string>();

		// -- State --
		// List of actually loaded libraries. Different from ProjectSettings.libraries 
		// because there it is the required libraries (including required repository version).
		private List<Library> libraries = new List<Library>();

		// Set to current library during InitializingLibrary method.
		private Library initializingLibrary;

		// Indicates that a new library is currently being registered (in contrast to
		// one that has to be loaded during opening a project).
		private bool addingLibrary;

		// -- Helpers --
		private object[] registerArgs;

		#endregion
	}


	/// <summary>
	/// Provides information for the LibraryLoaded event.
	/// </summary>
	/// <status>reviewed</status>
	public class LibraryLoadedEventArgs : EventArgs {

		/// <ToBeCompleted></ToBeCompleted>
		public LibraryLoadedEventArgs(string libraryName) {
			if (libraryName == null) throw new ArgumentNullException("libraryName");
			this.libraryName = libraryName;
		}

		/// <ToBeCompleted></ToBeCompleted>
		public string LibraryName {
			get { return libraryName; }
		}

		private string libraryName;
	}

}