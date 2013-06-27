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
using System.Drawing.Imaging;
using System.IO;
using System.Xml;
using Dataweb.NShape.Advanced;
using System.Drawing.Design;


namespace Dataweb.NShape
{
	/// <summary>
	/// Uses an XML file as the data store.
	/// </summary>
	/// <remarks>XML capability should go to CachedRepository or even higher. So we 
	/// can create the XML document from any cache. Only responsibilities left 
	/// here, is how ids are generated.</remarks>
	[ToolboxItem(true)]
	[ToolboxBitmap(typeof (XmlStore), "XmlStore.bmp")]
	public class XmlStore : Store
	{
		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.XmlStore" />.
		/// </summary>
		public XmlStore()
		{
			this.DirectoryName = string.Empty;
			this.FileExtension = ".xml";
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.XmlStore" />.
		/// </summary>
		public XmlStore(string directoryName, string fileExtension)
		{
			if (directoryName == null) throw new ArgumentNullException("directoryName");
			if (fileExtension == null) throw new ArgumentNullException("fileExtension");
			this.directoryName = directoryName;
			this.fileExtension = fileExtension;
		}

		#region [Public] Properties

		/// <summary>
		/// Specifies the version of the assembly containing the component.
		/// </summary>
		[Category("NShape")]
		public string ProductVersion
		{
			get { return this.GetType().Assembly.GetName().Version.ToString(); }
		}


		/// <summary>
		/// Defines the directory, where the NShape project is stored.
		/// </summary>
		[Category("NShape")]
		[Editor("Dataweb.NShape.WinFormsUI.DirectoryUITypeEditor, Dataweb.NShape.WinFormsUI", typeof (UITypeEditor))]
		public string DirectoryName
		{
			get { return directoryName; }
			set
			{
				if (value == null) throw new ArgumentNullException("DirectoryName");
				directoryName = value;
			}
		}


		/// <summary>
		/// Specifies the desired extension for the project file.
		/// </summary>
		[Category("NShape")]
		public string FileExtension
		{
			get { return fileExtension; }
			set
			{
				if (value == null) throw new ArgumentNullException("FileExtension");
				if (value.StartsWith("*")) value = value.Substring(1);
				fileExtension = value;
				if (!string.IsNullOrEmpty(fileExtension) && fileExtension[0] != '.')
					fileExtension = '.' + fileExtension;
			}
		}


		/// <summary>
		/// Defines the file name without extension, where the NShape designs are stored.
		/// </summary>
		[Category("NShape")]
		public string DesignFileName
		{
			get { return designFileName; }
			set { designFileName = value; }
		}


		/// <summary>
		/// Retrieves the file path of the project xml file.
		/// </summary>
		[Browsable(false)]
		public string ProjectFilePath
		{
			get
			{
				if (string.IsNullOrEmpty(directoryName))
					throw new InvalidOperationException("Directory for XML repository not set.");
				if (string.IsNullOrEmpty(projectName))
					throw new InvalidOperationException("Project name for XML repository not set.");
				string result = Path.Combine(directoryName, projectName);
				if (!string.IsNullOrEmpty(fileExtension)) result += fileExtension;
				return result;
			}
		}


		/// <summary>
		/// Retrieves the file path of the design xml file.
		/// </summary>
		[Browsable(false)]
		public string DesignFilePath
		{
			get
			{
				string result;
				if (string.IsNullOrEmpty(directoryName)) {
					result = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
					result += Path.DirectorySeparatorChar + "Dataweb" + Path.DirectorySeparatorChar + "NShape";
				}
				else result = directoryName;

				if (string.IsNullOrEmpty(DesignFileName))
					throw new InvalidOperationException("Project name for XML repository not set.");
				result += Path.DirectorySeparatorChar;
				result += DesignFileName;
				return result;
			}
		}

		#endregion

		#region [Public] Store Implementation

		/// <override></override>
		public override string ProjectName
		{
			get { return projectName; }
			set
			{
				if (value == null) projectName = string.Empty;
				else projectName = value;
				// Clear image directory
				imageDirectory = null;
			}
		}


		/// <override></override>
		public override bool Exists()
		{
			return File.Exists(ProjectFilePath);
		}


		/// <override></override>
		public override void ReadVersion(IStoreCache cache)
		{
			if (cache == null) throw new ArgumentNullException("cache");
			version = DoReadVersion(cache, false);
			cache.SetRepositoryBaseVersion(version);
		}


		/// <override></override>
		public override void Create(IStoreCache cache)
		{
			DoOpen(cache, true);
		}


		/// <override></override>
		public override void Open(IStoreCache cache)
		{
			DoOpen(cache, false);
		}


		/// <override></override>
		public override void Close(IStoreCache storeCache)
		{
			isOpen = false;
			isOpenComplete = false;
			CloseFile();
			base.Close(storeCache);
		}


		/// <override></override>
		public override void Erase()
		{
			if (File.Exists(ProjectFilePath)) {
				CreateBackupFiles(ProjectFilePath);
				File.Delete(ProjectFilePath);
				// The if prevents exceptions during debugging. The catch concurrency problems.
				if (Directory.Exists(ImageDirectory)) {
					try {
						Directory.Delete(ImageDirectory, true);
					}
					catch (DirectoryNotFoundException) {
						// It's ok if the directory does not exist
					}
				}
			}
		}


		/// <override></override>
		public override void LoadProjects(IStoreCache cache, IEntityType entityType, params object[] parameters)
		{
			if (cache == null) throw new ArgumentNullException("cache");
			if (entityType == null) throw new ArgumentNullException("entityType");
			// Do nothing. OpenComplete must be called after the libraries have been loaded.
		}


		/// <override></override>
		public override void LoadDesigns(IStoreCache cache, object projectId)
		{
			if (cache == null) throw new ArgumentNullException("cache");
			// Do nothing. OpenComplete must be called after the libraries have been loaded.
		}


		/// <override></override>
		public override void LoadModel(IStoreCache cache, object projectId)
		{
			if (isOpen) OpenComplete(cache);
		}


		/// <override></override>
		public override void LoadTemplates(IStoreCache cache, object projectId)
		{
			if (isOpen) OpenComplete(cache);
		}


		/// <override></override>
		public override void LoadDiagrams(IStoreCache cache, object projectId)
		{
			if (isOpen) OpenComplete(cache);
		}


		/// <override></override>
		public override void LoadDiagramShapes(IStoreCache cache, Diagram diagram)
		{
			if (isOpen) OpenComplete(cache);
		}


		/// <override></override>
		public override void LoadTemplateShapes(IStoreCache cache, object templateId)
		{
			if (isOpen) OpenComplete(cache);
		}


		/// <override></override>
		public override void LoadChildShapes(IStoreCache cache, object parentShapeId)
		{
			if (isOpen) OpenComplete(cache);
		}


		/// <override></override>
		public override void LoadTemplateModelObjects(IStoreCache cache, object templateId)
		{
			if (isOpen) OpenComplete(cache);
		}


		/// <override></override>
		public override void LoadModelModelObjects(IStoreCache cache, object modelId)
		{
			if (isOpen) OpenComplete(cache);
		}


		/// <override></override>
		public override void LoadChildModelObjects(IStoreCache cache, object parentModelObjectId)
		{
			if (isOpen) OpenComplete(cache);
		}


		/// <override></override>
		public override bool CheckStyleInUse(IStoreCache cache, object styleId)
		{
			// XML store does not support partial loading, so we have nothing to do here
			return false;
		}


		/// <override></override>
		public override bool CheckTemplateInUse(IStoreCache cache, object templateId)
		{
			// XML store does not support partial loading, so we have nothing to do here
			return false;
		}


		/// <override></override>
		public override bool CheckModelObjectInUse(IStoreCache cache, object modelObjectId)
		{
			// XML store does not support partial loading, so we have nothing to do here
			return false;
		}


		/// <override></override>
		public override bool CheckShapeTypeInUse(IStoreCache cache, string typeName)
		{
			// XML store does not support partial loading, so we have nothing to do here
			return false;
		}


		/// <override></override>
		public override void SaveChanges(IStoreCache cache)
		{
			if (cache == null) throw new ArgumentNullException("cache");
			if (!isOpen) throw new NShapeException("Store is not open.");
			if (string.IsNullOrEmpty(ProjectFilePath)) throw new NShapeException("File name was not specified.");
			string tempProjectFilePath = GetTemporaryProjectFilePath();
			string tempImageDirectory = CalcImageDirectoryName(tempProjectFilePath);
			try {
				// Save changes to temporary file
				try {
					// Set imageDirectoryPath to temporary directory, otherwise 
					// all images will be saved to original image directory
					imageDirectory = tempImageDirectory;
					DoSaveChanges(tempProjectFilePath, cache);
				}
				finally {
					imageDirectory = CalcImageDirectoryName(ProjectFilePath);
				}
				// Backup current project files
				if (File.Exists(ProjectFilePath))
					CreateBackupFiles(ProjectFilePath);
				System.Threading.Thread.Sleep(5);
				// Rename temporary files
				Debug.Assert(File.Exists(tempProjectFilePath));
				File.Move(tempProjectFilePath, ProjectFilePath);
				if (Directory.Exists(tempImageDirectory))
					Directory.Move(tempImageDirectory, ImageDirectory);
				// Re-Open project file
				OpenFile(cache, ProjectFilePath);
			}
			catch (Exception exc) {
				Debug.Print(exc.Message);
				throw;
			}
			finally {
				// Clean up temporary files
				if (File.Exists(tempProjectFilePath))
					File.Delete(tempProjectFilePath);
				if (Directory.Exists(tempImageDirectory))
					Directory.Exists(tempImageDirectory);
			}
		}


		/// <override></override>
		protected internal override int Version
		{
			get { return version; }
			set { version = value; }
		}

		#endregion

		#region [Protected] Methods: Implementation

		/// <summary>
		/// Loads connections between <see cref="T:Dataweb.NShape.Advanced.Shapes" /> instances.
		/// </summary>
		protected void LoadShapeConnections(IStoreCache cache, Diagram diagram)
		{
			Debug.Assert(isOpen);
			OpenComplete(cache);
		}


		/// <summary>
		/// Reads the save version from the XML file.
		/// </summary>
		protected int DoReadVersion(IStoreCache cache, bool keepFileOpen)
		{
			if (cache == null) throw new ArgumentNullException("cache");
			if (isOpen) throw new InvalidOperationException(string.Format("{0} is already open.", GetType().Name));
			OpenFile(cache, ProjectFilePath);
			try {
				xmlReader.MoveToContent();
				if (xmlReader.Name != rootTag || !xmlReader.HasAttributes)
					throw new NShapeException("XML file '{0}' is not a valid NShape project file.", ProjectFilePath);
				return int.Parse(xmlReader.GetAttribute(0));
			}
			finally {
				if (!keepFileOpen) CloseFile();
			}
		}


		/// <summary>
		/// Opens the XML file and read the project settings. The rest of the file is loaded on the first request of data.
		/// </summary>
		protected void DoOpen(IStoreCache cache, bool create)
		{
			if (cache == null) throw new ArgumentNullException("cache");
			if (isOpen) throw new InvalidOperationException(string.Format("{0} is already open.", GetType().Name));
			if (create) {
				// Nothing to do. Data is kept in memory until SaveChanges is called.
				isOpenComplete = true;
			}
			else {
				try {
					int fileVersion = DoReadVersion(cache, true);
					Debug.Assert(fileVersion == version);
					XmlSkipStartElement(rootTag);
					// We only read the designs and the project here. This gives the application
					// a chance to load required libraries. Templates and diagramControllers are then loaded
					// in OpenComplete.
					ReadProjectSettings(cache, xmlReader);
				}
				catch (Exception exc) {
					CloseFile();
					throw exc;
				}
			}
			isOpen = true;
		}


		/// <summary>
		/// Calculates the directory for the images given the complete file path.
		/// </summary>
		protected string CalcImageDirectoryName()
		{
			return CalcImageDirectoryName(ProjectFilePath);
		}


		/// <summary>
		/// Create a path with slashes ('/') instead of back slashes ('\') as path delimiter in order to ensure that xml repositories can be loaded from both, Windows and Linux operating systems.
		/// </summary>
		protected string UnifyPath(string path)
		{
			Uri resultUri;
			if (Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out resultUri))
				return Uri.UnescapeDataString(resultUri.AbsolutePath);
			return path;
		}


		/// <summary>
		/// Indicates whether the given id is interpreted as empty.
		/// </summary>
		protected internal bool IsNullOrEmpty<T>(object id)
		{
			if (object.ReferenceEquals(id, null)) return true;
			else return object.Equals(id, default(T));
		}

		#endregion

		#region [Protected] Types: XmlStoreReader and XmlStoreWriter

		/// <summary>
		/// Writes fields and inner objects to XML.
		/// </summary>
		protected class XmlStoreWriter : RepositoryWriter
		{
			/// <summary>
			/// Initializes a new instance of <see cref="T:Dataweb.NShape.XmlStore.XmlStoreWriter" />.
			/// </summary>
			public XmlStoreWriter(XmlWriter xmlWriter, XmlStore store, IStoreCache cache)
				: base(cache)
			{
				if (xmlWriter == null) throw new ArgumentNullException("xmlWriter");
				if (store == null) throw new ArgumentNullException("store");
				this.store = store;
				this.xmlWriter = xmlWriter;
			}

			#region RepositoryWriter Members

			/// <override></override>
			protected override void DoWriteId(object id)
			{
				++PropertyIndex;
				string fieldName = GetXmlAttributeName(PropertyIndex);
				if (id == null)
					XmlAddAttributeString(fieldName, Guid.Empty.ToString());
				else
					XmlAddAttributeString(fieldName, id.ToString());
			}


			/// <override></override>
			protected override void DoWriteBool(bool value)
			{
				++PropertyIndex;
				XmlAddAttributeString(GetXmlAttributeName(PropertyIndex), value.ToString());
			}


			/// <override></override>
			protected override void DoWriteByte(byte value)
			{
				++PropertyIndex;
				XmlAddAttributeString(GetXmlAttributeName(PropertyIndex), value.ToString());
			}


			/// <override></override>
			protected override void DoWriteInt16(short value)
			{
				++PropertyIndex;
				XmlAddAttributeString(GetXmlAttributeName(PropertyIndex), value.ToString());
			}


			/// <override></override>
			protected override void DoWriteInt32(int value)
			{
				++PropertyIndex;
				string fieldName = GetXmlAttributeName(PropertyIndex);
				XmlAddAttributeString(fieldName, value.ToString());
			}


			/// <override></override>
			protected override void DoWriteInt64(long value)
			{
				++PropertyIndex;
				XmlAddAttributeString(GetXmlAttributeName(PropertyIndex), value.ToString());
			}


			/// <override></override>
			protected override void DoWriteFloat(float value)
			{
				++PropertyIndex;
				XmlAddAttributeString(GetXmlAttributeName(PropertyIndex), value.ToString());
			}


			/// <override></override>
			protected override void DoWriteDouble(double value)
			{
				++PropertyIndex;
				XmlAddAttributeString(GetXmlAttributeName(PropertyIndex), value.ToString());
			}


			/// <override></override>
			protected override void DoWriteChar(char value)
			{
				++PropertyIndex;
				XmlAddAttributeString(GetXmlAttributeName(PropertyIndex), value.ToString());
			}


			/// <override></override>
			protected override void DoWriteString(string value)
			{
				if (string.IsNullOrEmpty(value)) value = string.Empty;
				++PropertyIndex;
				XmlAddAttributeString(GetXmlAttributeName(PropertyIndex), value);
			}


			/// <override></override>
			protected override void DoWriteDate(DateTime value)
			{
				++PropertyIndex;
				XmlAddAttributeString(GetXmlAttributeName(PropertyIndex), value.ToUniversalTime().ToString(datetimeFormat));
			}


			/// <override></override>
			protected override void DoWriteImage(Image image)
			{
				++PropertyIndex;
				if (image == null) {
					XmlAddAttributeString(GetXmlAttributeName(PropertyIndex), "");
				}
				else {
					// Retrieve image directory name and image name
					string filePath = store.ImageDirectory;
					string fileName = GetImageFileName(image);

					// Create directory if it does not exist
					if (!Directory.Exists(filePath))
						Directory.CreateDirectory(filePath);

					// Build image file path and set file extension
					filePath = store.UnifyPath(Path.Combine(filePath, fileName));

					if (image is Metafile) {
						using (Graphics gfx = Graphics.FromHwnd(IntPtr.Zero)) {
							IntPtr hdc = gfx.GetHdc();
							using (Metafile metaFile = new Metafile(filePath, hdc)) {
								gfx.ReleaseHdc(hdc);
								using (Graphics metaFileGfx = Graphics.FromImage(metaFile)) {
									Rectangle bounds = Rectangle.Empty;
									bounds.Width = image.Width;
									bounds.Height = image.Height;
									ImageAttributes imgAttribs = GdiHelpers.GetImageAttributes(ImageLayoutMode.Original);
									GdiHelpers.DrawImage(metaFileGfx, image, imgAttribs, ImageLayoutMode.Original, bounds, bounds);
								}
							}
						}
					}
					else image.Save(filePath, image.RawFormat);

					string currentDirName = store.UnifyPath(Path.GetDirectoryName(store.ProjectFilePath));
					if (!string.IsNullOrEmpty(currentDirName))
						filePath = filePath.Replace(currentDirName, ".");
					else {
						currentDirName = store.UnifyPath(Path.GetDirectoryName(Path.GetFullPath(store.ProjectFilePath)));
						filePath = filePath.Replace(currentDirName, ".");
					}
					XmlAddAttributeString(GetXmlAttributeName(PropertyIndex), filePath);
				}
			}


			/// <override></override>
			protected override void DoBeginWriteInnerObjects()
			{
				// Sanity checks
				if (propertyInfos == null)
					throw new InvalidOperationException("EntityType is not set.");
				if (Entity == null)
					throw new InvalidOperationException("InnerObject's parent object is not set to an instance of an object.");
				if (!(propertyInfos[PropertyIndex + 1] is EntityInnerObjectsDefinition))
					throw new InvalidOperationException(
						string.Format(
							"The current property info for '{0}' does not refer to inner objects. Check whether the writing methods fit the PropertyInfos property.",
							propertyInfos[PropertyIndex + 1]));
				// Advance to next inner objects property
				++PropertyIndex;
				innerObjectsWriter = new XmlStoreWriter(xmlWriter, store, Cache);
				innerObjectsWriter.Reset(((EntityInnerObjectsDefinition) propertyInfos[PropertyIndex]).PropertyDefinitions);
				xmlWriter.WriteStartElement(Cache.CalculateElementName(propertyInfos[PropertyIndex].Name));
			}


			/// <override></override>
			protected override void DoEndWriteInnerObjects()
			{
				xmlWriter.WriteEndElement();
			}


			/// <override></override>
			protected override void DoBeginWriteInnerObject()
			{
				Debug.Assert(Entity != null && innerObjectsWriter != null);
				xmlWriter.WriteStartElement(
					Cache.CalculateElementName(((EntityInnerObjectsDefinition) propertyInfos[PropertyIndex]).EntityTypeName));
				innerObjectsWriter.Prepare(null);
				// Skip the property index for the id since inner objects do not have one.
				++InnerObjectsWriter.PropertyIndex;
			}


			/// <override></override>
			protected override void DoEndWriteInnerObject()
			{
				Debug.Assert(Entity != null && innerObjectsWriter != null);
				xmlWriter.WriteEndElement();
			}


			/// <override></override>
			protected override void DoDeleteInnerObjects()
			{
				throw new NotImplementedException();
			}


			/// <override></override>
			protected internal override void Prepare(IEntity entity)
			{
				base.Prepare(entity);
			}

			#endregion

			#region [Private] Methods

			private string GetXmlAttributeName(int propertyIndex)
			{
				/* Not required for inner objects
				 * if (Entity == null) 
					throw new NShapeException("Persistable object to store is not set. Please assign an IEntity object to the property Object before calling a save method.");*/
				if (propertyInfos == null)
					throw new NShapeException(
						"EntityType is not set. Please assign an EntityType to the property EntityType before calling a save method.");
				return propertyIndex == -1 ? "id" : propertyInfos[propertyIndex].ElementName;
			}


			private string GetImageFileName(Image image)
			{
				string imageName = image.Tag.ToString();
				return GetImageFileName(image, imageName);
			}


			private string GetImageFileName(Image image, string imageName)
			{
				if (string.IsNullOrEmpty(imageName)) imageName = "Image";
				imageName += string.Format(" ({0})", Entity.Id.ToString());

				if (image.RawFormat.Guid == ImageFormat.Bmp.Guid) imageName += ".bmp";
				else if (image.RawFormat.Guid == ImageFormat.Emf.Guid) imageName += ".emf";
				else if (image.RawFormat.Guid == ImageFormat.Exif.Guid) imageName += ".exif";
				else if (image.RawFormat.Guid == ImageFormat.Gif.Guid) imageName += ".gif";
				else if (image.RawFormat.Guid == ImageFormat.Icon.Guid) imageName += ".ico";
				else if (image.RawFormat.Guid == ImageFormat.Jpeg.Guid) imageName += ".jpeg";
				else if (image.RawFormat.Guid == ImageFormat.MemoryBmp.Guid) imageName += ".bmp";
				else if (image.RawFormat.Guid == ImageFormat.Png.Guid) imageName += ".png";
				else if (image.RawFormat.Guid == ImageFormat.Tiff.Guid) imageName += ".tiff";
				else if (image.RawFormat.Guid == ImageFormat.Wmf.Guid) imageName += ".wmf";
				else Debug.Fail("Unsupported image format.");

				return imageName;
			}


			private void XmlAddAttributeString(string name, string value)
			{
				xmlWriter.WriteAttributeString(name, value);
			}


			private XmlStoreWriter InnerObjectsWriter
			{
				get { return (XmlStoreWriter) innerObjectsWriter; }
			}

			#endregion

			#region Fields

			private XmlStore store;

			private XmlWriter xmlWriter;

			#endregion
		}


		/// <summary>
		/// Implements a cache repositoryReader for XML.
		/// </summary>
		protected class XmlStoreReader : RepositoryReader
		{
			/// <summary>
			/// Initializes a new instance of <see cref="T:Dataweb.NShape.XmlStore.XmlStoreReader" />.
			/// </summary>
			public XmlStoreReader(XmlReader xmlReader, XmlStore store, IStoreCache cache)
				: base(cache)
			{
				if (xmlReader == null) throw new ArgumentNullException("xmlReader");
				if (store == null) throw new ArgumentNullException("store");
				this.store = store;
				this.xmlReader = xmlReader;
			}

			#region [Public] Methods: RepositoryReader Implementation

			/// <override></override>
			public override void BeginReadInnerObjects()
			{
				if (propertyInfos == null) throw new NShapeException("Property EntityType is not set.");
				if (innerObjectsReader != null) throw new InvalidOperationException("EndReadInnerObjects was not called.");
				++PropertyIndex;
				string elementName = Cache.CalculateElementName(propertyInfos[PropertyIndex].Name);
				if (!xmlReader.IsStartElement(elementName))
					throw new InvalidOperationException(string.Format("Element '{0}' expected.", elementName));
				if (!xmlReader.IsEmptyElement) xmlReader.Read();
				innerObjectsReader = new XmlStoreReader(xmlReader, store, Cache);
				// Set a marker to detect wrong call sequence
				InnerObjectsReader.PropertyIndex = int.MinValue;
				InnerObjectsReader.ResetFieldReading(
					((EntityInnerObjectsDefinition) propertyInfos[PropertyIndex]).PropertyDefinitions);
			}


			/// <override></override>
			public override void EndReadInnerObjects()
			{
				if (innerObjectsReader == null) throw new InvalidOperationException("BeginReadInnerObjects was not called.");
				Debug.Assert(xmlReader.IsEmptyElement || xmlReader.NodeType == XmlNodeType.EndElement);
				xmlReader.Read(); // read end tag of collection
				innerObjectsReader = null;
			}


			/// <override></override>
			public override void EndReadInnerObject()
			{
				xmlReader.Read(); // Read out of the attributes
				// Previous version: XmlSkipEndElement(store.CalculateElementName(((EntityInnerObjectsDefinition)propertyInfos[PropertyIndex]).Name));
				InnerObjectsReader.PropertyIndex = int.MinValue;
			}


			/// <override></override>
			protected override bool DoReadBool()
			{
				bool result = bool.Parse(xmlReader.GetAttribute(PropertyIndex + xmlAttributeOffset));
				xmlReader.MoveToNextAttribute();
				return result;
			}


			/// <override></override>
			protected override byte DoReadByte()
			{
				byte result = byte.Parse(xmlReader.GetAttribute(PropertyIndex + xmlAttributeOffset));
				xmlReader.MoveToNextAttribute();
				return result;
			}


			/// <override></override>
			protected override short DoReadInt16()
			{
				short result = short.Parse(xmlReader.GetAttribute(PropertyIndex + xmlAttributeOffset));
				xmlReader.MoveToNextAttribute();
				return result;
			}


			/// <override></override>
			protected override int DoReadInt32()
			{
				int result = int.Parse(xmlReader.GetAttribute(PropertyIndex + xmlAttributeOffset));
				xmlReader.MoveToNextAttribute();
				return result;
			}


			/// <override></override>
			protected override long DoReadInt64()
			{
				long result = long.Parse(xmlReader.GetAttribute(PropertyIndex + xmlAttributeOffset));
				xmlReader.MoveToNextAttribute();
				return result;
			}


			/// <override></override>
			protected override float DoReadFloat()
			{
				float result = float.Parse(xmlReader.GetAttribute(PropertyIndex + xmlAttributeOffset));
				xmlReader.MoveToNextAttribute();
				return result;
			}


			/// <override></override>
			protected override double DoReadDouble()
			{
				double result = double.Parse(xmlReader.GetAttribute(PropertyIndex + xmlAttributeOffset));
				xmlReader.MoveToNextAttribute();
				return result;
			}


			/// <override></override>
			protected override char DoReadChar()
			{
				char result = char.Parse(xmlReader.GetAttribute(PropertyIndex + xmlAttributeOffset));
				xmlReader.MoveToNextAttribute();
				return result;
			}


			/// <override></override>
			protected override string DoReadString()
			{
				string result = xmlReader.GetAttribute(PropertyIndex + xmlAttributeOffset);
				xmlReader.MoveToNextAttribute();
				return result;
			}


			/// <override></override>
			protected override DateTime DoReadDate()
			{
				System.Globalization.DateTimeFormatInfo info = new System.Globalization.DateTimeFormatInfo();
				string attrValue = xmlReader.GetAttribute(PropertyIndex + xmlAttributeOffset);
				DateTime dateTime;
				if (
					!DateTime.TryParseExact(attrValue, datetimeFormat, null, System.Globalization.DateTimeStyles.AssumeUniversal,
					                        out dateTime))
					dateTime = Convert.ToDateTime(attrValue);
						// ToDo: This is for compatibility with older file versions - Remove later
				return dateTime.ToLocalTime();
			}


			/// <override></override>
			protected override Image DoReadImage()
			{
				// GDI+ only reads the file header and keeps the image file locked for loading the 
				// image data later on demand. 
				// So we have to read the entire image to a buffer and create the image from a 
				// MemoryStream in order to avoid locked (and thus unaccessible) image files.
				Image result = null;
				string filePath = xmlReader.GetAttribute(PropertyIndex + xmlAttributeOffset);
				xmlReader.MoveToNextAttribute();
				if (!string.IsNullOrEmpty(filePath)) {
					string fileName = store.UnifyPath(Path.Combine(store.ImageDirectory, Path.GetFileName(filePath)));
					byte[] buffer = null;
					using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read)) {
						buffer = new byte[fileStream.Length];
						fileStream.Read(buffer, 0, buffer.Length);
						fileStream.Close();
					}
					if (buffer != null) {
						MemoryStream memStream = new MemoryStream(buffer);
						result = Image.FromStream(memStream);
					}
				}
				return result;
			}

			#endregion

			#region [Protected] Methods: Implementation

			/// <override></override>
			protected internal override object ReadId()
			{
				++PropertyIndex;
				ValidatePropertyIndex();
				object result = null;
				string resultString = xmlReader.GetAttribute(PropertyIndex + xmlAttributeOffset);
				if (!string.IsNullOrEmpty(resultString)) {
					result = new Guid(resultString);
					if (result.Equals(Guid.Empty))
						result = null;
				}
				xmlReader.MoveToNextAttribute();
				return result;
			}


			// Assumes that the current tag is of the expected type or the end element of 
			// the enclosing element. Moves to the first attribute for subsequent reading.
			/// <override></override>
			protected internal override bool DoBeginObject()
			{
				// If the enclosing element is empty, it is still the current tag and indicates 
				// an empty inner objects list.
				if (xmlReader.NodeType == XmlNodeType.EndElement || (xmlReader.IsEmptyElement && !xmlReader.HasAttributes))
					return false;
				else {
					xmlReader.MoveToFirstAttribute();
					PropertyIndex = -2;
					return true;
				}
			}


			/// <override></override>
			protected internal override void DoEndObject()
			{
				// Read over the end tag of the object.
			}


			/// <override></override>
			protected override void ValidatePropertyIndex()
			{
				base.ValidatePropertyIndex();
				if (PropertyIndex + xmlAttributeOffset >= xmlReader.AttributeCount)
					throw new NShapeException(
						"An entity tries to read {0} properties although there are only {1} properties stored in the repository. Check whether the repository is valid and/or up-to-date.",
						PropertyIndex + xmlAttributeOffset + 1, xmlReader.AttributeCount);
			}


			internal override void ResetFieldReading(IEnumerable<EntityPropertyDefinition> propertyInfos)
			{
				base.ResetFieldReading(propertyInfos);
			}

			#endregion

			private XmlStoreReader InnerObjectsReader
			{
				get { return (XmlStoreReader) innerObjectsReader; }
			}

			#region Fields

			// There is always one more XML attribute (the id) than there are properties 
			// in the entity type.
			private const int xmlAttributeOffset = 1;

			private XmlStore store;

			private XmlReader xmlReader;

			#endregion
		}

		#endregion

		internal string ImageDirectory
		{
			get
			{
				if (string.IsNullOrEmpty(imageDirectory))
					imageDirectory = CalcImageDirectoryName();
				return imageDirectory;
			}
		}

		#region [Private] Methods: Save project files

		private string CalcImageDirectoryName(string projectFilePath)
		{
			string result = Path.GetDirectoryName(projectFilePath);
			if (string.IsNullOrEmpty(result)) throw new ArgumentException("XML repository file name must be a complete path.");
			result = UnifyPath(Path.Combine(result, Path.GetFileNameWithoutExtension(projectFilePath) + " Images"));
			return result;
		}


		private string GetTemporaryProjectFilePath()
		{
			return Path.Combine(Path.GetDirectoryName(ProjectFilePath), "~" + Path.GetFileName(ProjectFilePath));
		}


		private void CreateBackupFiles(string projectFilePath)
		{
			if (!File.Exists(projectFilePath)) throw new FileNotFoundException("Project file does not exist.", projectFilePath);
			const string backupExtension = ".bak";
			string projectImageDir = CalcImageDirectoryName(projectFilePath);
			// Calculate backup file names 
			string backupFilepath = Path.Combine(Path.GetDirectoryName(projectFilePath),
			                                     Path.GetFileNameWithoutExtension(projectFilePath) + backupExtension);
			string backupImageDir = CalcImageDirectoryName(projectFilePath) + backupExtension;
			// Delete old backup (if one exists)
			try {
				if (File.Exists(backupFilepath)) {
					File.Delete(backupFilepath);
					if (Directory.Exists(backupImageDir))
						Directory.Delete(backupImageDir, true);
				}
				// Rename current files
				File.Move(projectFilePath, backupFilepath);
				if (Directory.Exists(projectImageDir))
					Directory.Move(projectImageDir, backupImageDir);
			}
			catch (Exception) {
				throw;
			}
		}


		private void DoSaveChanges(string filePath, IStoreCache cache)
		{
			try {
				// If it is a new project, we must create the file. Otherwise it is already open.
				string imageDirectory = CalcImageDirectoryName(filePath);
				if (cache.ProjectId == null) {
					CreateFile(cache, filePath, false);
				}
				else if (cache.LoadedProjects[cache.ProjectId].State == ItemState.Deleted) {
					// First delete the file, so the image directory still exists, when the file cannot be deleted.
					if (File.Exists(filePath)) File.Delete(filePath);
					if (Directory.Exists(imageDirectory))
						Directory.Delete(imageDirectory, true);
				}
				else {
					OpenComplete(cache);
					// TODO 2: We should keep the file open and clear it here instead of re-creating it.
					CloseFile();
					CreateFile(cache, filePath, true);
				}
				WriteProject(cache);
			}
			finally {
				// Close and reopen to update Windows directory and keep file ownership
				CloseFile();
			}
		}

		#endregion

		#region [Private] Methods: Read from XML file

		private void ReadProjectSettings(IStoreCache cache, XmlReader xmlReader)
		{
			if (!XmlSkipToElement(projectTag)) throw new NShapeException("Invalid XML file. Project tag not found.");
			// Load project data
			IEntityType projectSettingsEntityType = cache.FindEntityTypeByName(ProjectSettings.EntityTypeName);
			ProjectSettings project = (ProjectSettings) projectSettingsEntityType.CreateInstanceForLoading();
			repositoryReader.ResetFieldReading(projectSettingsEntityType.PropertyDefinitions);
			XmlSkipStartElement(projectTag);
			repositoryReader.DoBeginObject();
			object id = repositoryReader.ReadId();
			if (id == null) {
				((IEntity) project).AssignId(Guid.Empty);
			}
			else ((IEntity) project).AssignId(id);

			((IEntity) project).LoadFields(repositoryReader, cache.RepositoryBaseVersion);
			xmlReader.Read(); // read out of attributes
			foreach (EntityPropertyDefinition pi in projectSettingsEntityType.PropertyDefinitions)
				if (pi is EntityInnerObjectsDefinition)
					((IEntity) project).LoadInnerObjects(pi.Name, repositoryReader, cache.RepositoryBaseVersion);
			cache.LoadedProjects.Add(new EntityBucket<ProjectSettings>(project, null, ItemState.Original));

			// Load the project styles
			Design projectDesign = new Design();
			// project design GUID only needed for runtime.
			((IEntity) projectDesign).AssignId(Guid.NewGuid());
			ReadAllStyles(cache, projectDesign);
			cache.LoadedDesigns.Add(new EntityBucket<Design>(projectDesign, project, ItemState.Original));
		}


		private void ReadDesigns(IStoreCache cache, XmlReader xmlReader)
		{
			IEntityType designEntityType = cache.FindEntityTypeByName(Design.EntityTypeName);
			string designCollectionTag = GetElementCollectionTag(designEntityType);
			XmlSkipToElement(designCollectionTag);
			if (XmlSkipStartElement(designCollectionTag)) {
				do {
					if (XmlSkipStartElement(designEntityType.ElementName)) {
						Design design = (Design) designEntityType.CreateInstanceForLoading();
						repositoryReader.ResetFieldReading(designEntityType.PropertyDefinitions);
						repositoryReader.DoBeginObject();
						((IEntity) design).LoadFields(repositoryReader, designEntityType.RepositoryVersion);
						xmlReader.Read(); // read out of attributes
						foreach (EntityPropertyDefinition pi in designEntityType.PropertyDefinitions)
							if (pi is EntityInnerObjectsDefinition)
								((IEntity) design).LoadInnerObjects(pi.Name, repositoryReader, designEntityType.RepositoryVersion);
						// Global designs are stored with parent id DBNull
						cache.LoadedDesigns.Add(new EntityBucket<Design>(design, null, ItemState.Original));
						design.Clear();
						ReadAllStyles(cache, design);
					}
				} while (xmlReader.ReadToNextSibling(designEntityType.ElementName));
				XmlSkipEndElement(designCollectionTag);
			}
		}


		private void OpenComplete(IStoreCache cache)
		{
			if (cache == null) throw new ArgumentNullException("cache");
			if (!isOpenComplete) {
				// The position is on the model
				ReadModel(cache, xmlReader);
				ReadTemplates(cache, xmlReader);
				ReadDiagrams(cache, xmlReader);
				XmlSkipEndElement(projectTag);
				XmlSkipEndElement(rootTag);
				isOpenComplete = true;
			}
		}


		private void ReadAllStyles(IStoreCache cache, Design design)
		{
			ReadStyles(cache, xmlReader, design, cache.FindEntityTypeByName(ColorStyle.EntityTypeName));
			ReadStyles(cache, xmlReader, design, cache.FindEntityTypeByName(CapStyle.EntityTypeName));
			ReadStyles(cache, xmlReader, design, cache.FindEntityTypeByName(CharacterStyle.EntityTypeName));
			ReadStyles(cache, xmlReader, design, cache.FindEntityTypeByName(FillStyle.EntityTypeName));
			ReadStyles(cache, xmlReader, design, cache.FindEntityTypeByName(LineStyle.EntityTypeName));
			ReadStyles(cache, xmlReader, design, cache.FindEntityTypeByName(ParagraphStyle.EntityTypeName));
			if (string.Compare(xmlReader.Name, shapeStyleTag, StringComparison.InvariantCultureIgnoreCase) == 0) {
				xmlReader.Read();
				XmlReadEndElement(shapeStyleTag);
			}
		}


		private void ReadStyles(IStoreCache cache, XmlReader xmlReader, Design design, IEntityType styleEntityType)
		{
			if (!xmlReader.IsStartElement(GetElementCollectionTag(styleEntityType)))
				throw new NShapeException("Element '{0}' expected but not found.", GetElementCollectionTag(styleEntityType));
			xmlReader.Read(); // Read over the collection tag
			repositoryReader.ResetFieldReading(styleEntityType.PropertyDefinitions);
			while (string.Compare(xmlReader.Name, GetElementTag(styleEntityType), StringComparison.InvariantCultureIgnoreCase) ==
			       0) {
				Style style = (Style) styleEntityType.CreateInstanceForLoading();
				repositoryReader.DoBeginObject();
				style.AssignId(repositoryReader.ReadId());
				style.LoadFields(repositoryReader, cache.RepositoryBaseVersion);
				xmlReader.Read(); // read out of attributes
				foreach (EntityPropertyDefinition pi in styleEntityType.PropertyDefinitions)
					if (pi is EntityInnerObjectsDefinition)
						style.LoadInnerObjects(pi.Name, repositoryReader, styleEntityType.RepositoryVersion);
				cache.LoadedStyles.Add(new EntityBucket<IStyle>(style, design, ItemState.Original));
				design.AddStyle(style);
				// Reads the end tag of the specific style if present
				XmlReadEndElement(GetElementTag(styleEntityType));
			}
			XmlReadEndElement(GetElementCollectionTag(styleEntityType));
		}


		private void ReadModel(IStoreCache cache, XmlReader xmlReader)
		{
			IEntityType modelEntityType = cache.FindEntityTypeByName(Model.EntityTypeName);
			if (XmlSkipStartElement(modelEntityType.ElementName)) {
				if (xmlReader.NodeType != XmlNodeType.EndElement) {
					Model model = (Model) modelEntityType.CreateInstanceForLoading();
					repositoryReader.ResetFieldReading(modelEntityType.PropertyDefinitions);
					repositoryReader.DoBeginObject();
					((IEntity) model).AssignId(repositoryReader.ReadId());
					((IEntity) model).LoadFields(repositoryReader, modelEntityType.RepositoryVersion);
					xmlReader.Read(); // read out of attributes
					foreach (EntityPropertyDefinition pi in modelEntityType.PropertyDefinitions)
						if (pi is EntityInnerObjectsDefinition)
							((IEntity) model).LoadInnerObjects(pi.Name, repositoryReader, modelEntityType.RepositoryVersion);
					// Global models are stored with parent id DBNull
					cache.LoadedModels.Add(new EntityBucket<Model>(model, null, ItemState.Original));

					ReadModelObjects(cache, xmlReader, model);
				}
			}
			XmlSkipEndElement(modelEntityType.ElementName);
		}


		private void ReadModelObjects(IStoreCache cache, XmlReader xmlReader, IEntity owner)
		{
			if (XmlSkipStartElement(modelObjectsTag)) {
				while (xmlReader.NodeType != XmlNodeType.EndElement)
					ReadModelObject(cache, repositoryReader, owner);
				XmlSkipEndElement(modelObjectsTag);
			}
		}


		private void ReadTemplates(IStoreCache cache, XmlReader xmlReader)
		{
			IEntityType templateEntityType = cache.FindEntityTypeByName(Template.EntityTypeName);
			string templateCollectionTag = GetElementCollectionTag(templateEntityType);
			string templateTag = GetElementTag(templateEntityType);
			if (!xmlReader.IsStartElement(templateCollectionTag))
				throw new NShapeException("Element '{0}' expected but not found.", templateCollectionTag);
			xmlReader.Read(); // Read over the collection tag
			repositoryReader.ResetFieldReading(templateEntityType.PropertyDefinitions);
			XmlStoreReader innerReader = new XmlStoreReader(xmlReader, this, cache);
			while (xmlReader.IsStartElement(templateTag)) {
				// Read the template
				Template template = (Template) templateEntityType.CreateInstanceForLoading();
				repositoryReader.DoBeginObject();
				template.AssignId(repositoryReader.ReadId());
				template.LoadFields(repositoryReader, templateEntityType.RepositoryVersion);
				xmlReader.Read(); // read out of attributes
				// Read the model object
				IModelObject modelObject = null;
				if (XmlSkipStartElement("no_model")) XmlReadEndElement("no_model");
				else modelObject = ReadModelObject(cache, innerReader, template);
				// Read the shape
				template.Shape = ReadShape(cache, innerReader, null);
				cache.LoadedTemplates.Add(new EntityBucket<Template>(template, cache.Project, ItemState.Original));
				// Read the template's inner objects
				foreach (EntityPropertyDefinition pi in templateEntityType.PropertyDefinitions)
					if (pi is EntityInnerObjectsDefinition)
						template.LoadInnerObjects(pi.Name, repositoryReader, templateEntityType.RepositoryVersion);

				XmlSkipAttributes();
				XmlStoreReader reader = new XmlStoreReader(xmlReader, this, cache);
				ReadModelMappings(cache, reader, template);

				// Read the template end tag
				XmlReadEndElement(GetElementTag(templateEntityType));
			}
			XmlReadEndElement(GetElementCollectionTag(templateEntityType));
		}


		private void ReadModelMappings(IStoreCache cache, XmlStoreReader reader, Template template)
		{
			if (XmlSkipStartElement(modelmappingsTag)) {
				while (xmlReader.NodeType != XmlNodeType.EndElement) {
					IModelMapping modelMapping = ReadModelMapping(cache, reader, template);
					template.MapProperties(modelMapping);
					//XmlSkipAttributes();
				}
				XmlSkipEndElement(modelmappingsTag);
			}
		}


		private IModelMapping ReadModelMapping(IStoreCache cache, XmlStoreReader reader, IEntity owner)
		{
			Debug.Assert(xmlReader.NodeType == XmlNodeType.Element);
			string modelMappingTag = xmlReader.Name;
			IEntityType entityType = cache.FindEntityTypeByElementName(modelMappingTag);
			if (entityType == null)
				throw new NShapeException("No shape type found for tag '{0}'.", modelMappingTag);
			XmlSkipStartElement(modelMappingTag);
			IModelMapping modelMapping = (IModelMapping) entityType.CreateInstanceForLoading();
			reader.ResetFieldReading(entityType.PropertyDefinitions);
			reader.DoBeginObject();
			((IEntity) modelMapping).AssignId(reader.ReadId());
			((IEntity) modelMapping).LoadFields(reader, entityType.RepositoryVersion);
			xmlReader.Read(); // Reads out of attributes
			foreach (EntityPropertyDefinition pi in entityType.PropertyDefinitions)
				if (pi is EntityInnerObjectsDefinition)
					((IEntity) modelMapping).LoadInnerObjects(pi.Name, reader, entityType.RepositoryVersion);
			// Reads the end element
			XmlReadEndElement(modelMappingTag);
			// Insert shape into cache
			cache.LoadedModelMappings.Add(new EntityBucket<IModelMapping>(modelMapping, owner, ItemState.Original));
			return modelMapping;
		}


		private void ReadDiagrams(IStoreCache cache, XmlReader xmlReader)
		{
			IEntityType diagramEntityType = cache.FindEntityTypeByName(Diagram.EntityTypeName);
			string diagramCollectionTag = GetElementCollectionTag(diagramEntityType);
			string diagramTag = GetElementTag(diagramEntityType);
			XmlSkipToElement(diagramCollectionTag);
			if (XmlSkipStartElement(diagramCollectionTag)) {
				repositoryReader.ResetFieldReading(diagramEntityType.PropertyDefinitions);
				do {
					if (XmlSkipStartElement(diagramTag)) {
						Diagram diagram = (Diagram) diagramEntityType.CreateInstanceForLoading();
						repositoryReader.DoBeginObject();
						((IEntity) diagram).AssignId(repositoryReader.ReadId());
						((IEntity) diagram).LoadFields(repositoryReader, diagramEntityType.RepositoryVersion);
						xmlReader.Read(); // read out of attributes
						foreach (EntityPropertyDefinition pi in diagramEntityType.PropertyDefinitions)
							if (pi is EntityInnerObjectsDefinition)
								((IEntity) diagram).LoadInnerObjects(pi.Name, repositoryReader, diagramEntityType.RepositoryVersion);
						cache.LoadedDiagrams.Add(new EntityBucket<Diagram>(diagram, cache.Project, ItemState.Original));
						XmlSkipAttributes();
						XmlStoreReader reader = new XmlStoreReader(xmlReader, this, cache);
						ReadDiagramShapes(cache, reader, diagram);
						ReadDiagramShapeConnections(cache, reader);
					}
				} while (xmlReader.ReadToNextSibling(diagramTag));
				XmlSkipEndElement(diagramCollectionTag);
			}
		}


		private void ReadDiagramShapes(IStoreCache cache, XmlStoreReader reader, Diagram diagram)
		{
			if (XmlSkipStartElement(shapesTag)) {
				while (xmlReader.NodeType != XmlNodeType.EndElement) {
					Shape shape = ReadShape(cache, reader, diagram);
					diagram.Shapes.Add(shape);
					diagram.AddShapeToLayers(shape, shape.Layers); // not really necessary
					//XmlSkipAttributes();
				}
				XmlSkipEndElement(shapesTag);
			}
		}


		private Shape ReadShape(IStoreCache cache, XmlStoreReader reader, IEntity owner)
		{
			Debug.Assert(xmlReader.NodeType == XmlNodeType.Element);
			string shapeTag = xmlReader.Name;
			IEntityType shapeEntityType = cache.FindEntityTypeByElementName(shapeTag);
			if (shapeEntityType == null)
				throw new NShapeException("No shape type found for tag '{0}'.", shapeTag);
			XmlSkipStartElement(shapeTag);
			Shape shape = (Shape) shapeEntityType.CreateInstanceForLoading();
			reader.ResetFieldReading(shapeEntityType.PropertyDefinitions);
			reader.DoBeginObject();
			((IEntity) shape).AssignId(reader.ReadId());
			((IEntity) shape).LoadFields(reader, shapeEntityType.RepositoryVersion);
			xmlReader.Read(); // Reads out of attributes
			foreach (EntityPropertyDefinition pi in shapeEntityType.PropertyDefinitions)
				if (pi is EntityInnerObjectsDefinition)
					((IEntity) shape).LoadInnerObjects(pi.Name, reader, shapeEntityType.RepositoryVersion);
			// Read the child shapes
			if (XmlReadStartElement(childrenTag)) {
				do {
					Shape s = ReadShape(cache, reader, shape);
					shape.Children.Add(s);
				} while (xmlReader.Name != childrenTag && xmlReader.NodeType != XmlNodeType.EndElement);
				if (xmlReader.Name != childrenTag) throw new NShapeException("Shape children are invalid in XML document.");
				XmlReadEndElement(childrenTag);
			}
			// Reads the shape's end element
			XmlReadEndElement(shapeTag);
			// Insert shape into cache
			cache.LoadedShapes.Add(new EntityBucket<Shape>(shape, owner, ItemState.Original));
			return shape;
		}


		private void ReadDiagramShapeConnections(IStoreCache cache, XmlStoreReader reader)
		{
			if (XmlSkipStartElement(connectionsTag)) {
				while (string.Compare(xmlReader.Name, connectionTag, StringComparison.InvariantCultureIgnoreCase) == 0) {
					xmlReader.MoveToFirstAttribute();
					object connectorId = new Guid(xmlReader.GetAttribute(0));
					xmlReader.MoveToNextAttribute();
					int gluePointId = int.Parse(xmlReader.GetAttribute(1));
					xmlReader.MoveToNextAttribute();
					object targetShapeId = new Guid(xmlReader.GetAttribute(2));
					xmlReader.MoveToNextAttribute();
					int targetPointId = int.Parse(xmlReader.GetAttribute(3));
					xmlReader.MoveToNextAttribute();
					Shape connector = cache.GetShape(connectorId);
					Shape targetShape = cache.GetShape(targetShapeId);
					connector.Connect(gluePointId, targetShape, targetPointId);
					XmlSkipEndElement(connectionTag);
				}
				XmlSkipEndElement(connectionsTag);
			}
		}


		// TODO 2: This is more or less identical to ReadShape. Unify?
		// That would require a IEntityWithChildren interface.
		private IModelObject ReadModelObject(IStoreCache cache, XmlStoreReader reader, IEntity owner)
		{
			Debug.Assert(xmlReader.NodeType == XmlNodeType.Element);
			string modelObjectTag = xmlReader.Name;
			IEntityType entityType = cache.FindEntityTypeByElementName(modelObjectTag);
			if (entityType == null)
				throw new NShapeException("No model object type found for tag '{0}'.", modelObjectTag);
			XmlSkipStartElement(modelObjectTag);
			IModelObject modelObject = (IModelObject) entityType.CreateInstanceForLoading();
			reader.ResetFieldReading(entityType.PropertyDefinitions);
			reader.DoBeginObject();
			modelObject.AssignId(reader.ReadId());
			modelObject.LoadFields(reader, entityType.RepositoryVersion);
			xmlReader.Read(); // Reads out of attributes
			foreach (EntityPropertyDefinition pi in entityType.PropertyDefinitions)
				if (pi is EntityInnerObjectsDefinition)
					modelObject.LoadInnerObjects(pi.Name, reader, entityType.RepositoryVersion);
			// Read the child ModelObjects
			if (xmlReader.NodeType == XmlNodeType.Element) {
				do {
					IModelObject m = ReadModelObject(cache, reader, modelObject);
				} while (xmlReader.Name != childrenTag && xmlReader.NodeType != XmlNodeType.EndElement);
				if (xmlReader.Name != childrenTag) throw new NShapeException("ModelObject children are invalid in XML document.");
				XmlReadEndElement(childrenTag);
			}
			// Reads the model object's end element
			XmlReadEndElement(modelObjectTag);
			// Insert entity into cache
			cache.LoadedModelObjects.Add(new EntityBucket<IModelObject>(modelObject, owner, ItemState.Original));
			return modelObject;
		}

		#endregion

		#region [Private] Methods: Write to XML file

		private void CreateFile(IStoreCache cache, string pathName, bool overwrite)
		{
			Debug.Assert(repositoryWriter == null);
			string imageDirectoryName = CalcImageDirectoryName();
			if (!overwrite) {
				string tempPathName = GetTemporaryProjectFilePath();
				if (File.Exists(pathName)) {
					if (pathName == tempPathName)
						File.Delete(pathName);
					else throw new IOException(string.Format("File {0} already exists.", pathName));
				}
				if (Directory.Exists(imageDirectoryName)) {
					if (imageDirectoryName == CalcImageDirectoryName(tempPathName))
						Directory.Delete(imageDirectoryName, true);
					else throw new IOException(string.Format("Image directory {0} already exists.", imageDirectoryName));
				}
			}
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			xmlWriter = XmlWriter.Create(pathName, settings);
			repositoryWriter = new XmlStoreWriter(xmlWriter, this, cache);
			// Image directory will be created on demand.
		}


		private void OpenFile(IStoreCache cache, string fileName)
		{
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.CloseInput = true;
			settings.IgnoreComments = true;
			settings.IgnoreWhitespace = true;
			settings.IgnoreProcessingInstructions = true;
			xmlReader = XmlReader.Create(fileName, settings);
			xmlReader.Read();
			repositoryReader = new XmlStoreReader(xmlReader, this, cache);
		}


		private void CloseFile()
		{
			repositoryWriter = null;
			repositoryReader = null;
			if (xmlWriter != null) {
				xmlWriter.Close();
				xmlWriter = null;
			}
			if (xmlReader != null) {
				xmlReader.Close();
				xmlReader = null;
			}
		}


		private void WriteProject(IStoreCache cache)
		{
			xmlWriter.WriteStartDocument();
			XmlOpenElement(rootTag);
			xmlWriter.WriteAttributeString("version", version.ToString());
			WriteProjectSettings(cache);
			// Currently there is no other design than the project design
			// WriteDesigns();
			WriteModel(cache);
			WriteTemplates(cache);
			WriteDiagrams(cache);
			XmlCloseElement(); // project tag
			XmlCloseElement(); // NShape tag
			xmlWriter.WriteEndDocument();
		}


		private void WriteProjectSettings(IStoreCache cache)
		{
			IEntityType projectSettingsEntityType = cache.FindEntityTypeByName(ProjectSettings.EntityTypeName);
			IEntity entity = cache.Project;
			XmlOpenElement(projectTag);
			repositoryWriter.Reset(projectSettingsEntityType.PropertyDefinitions);
			// Save project settings
			repositoryWriter.Prepare(entity);
			if (entity.Id == null) entity.AssignId(Guid.NewGuid());
			repositoryWriter.WriteId(entity.Id);
			entity.SaveFields(repositoryWriter, projectSettingsEntityType.RepositoryVersion);
			foreach (EntityPropertyDefinition pi in projectSettingsEntityType.PropertyDefinitions)
				if (pi is EntityInnerObjectsDefinition)
					entity.SaveInnerObjects(pi.Name, repositoryWriter, projectSettingsEntityType.RepositoryVersion);
			// Save the pseudo design
			entity = cache.ProjectDesign;
			if (entity.Id == null) entity.AssignId(Guid.NewGuid());
			WriteAllStyles(cache, cache.ProjectDesign);
		}


		private void WriteDesigns(IStoreCache cache)
		{
			IEntityType designEntityType = cache.FindEntityTypeByName(Design.EntityTypeName);
			string designCollectionTag = GetElementCollectionTag(designEntityType);
			string designTag = GetElementTag(designEntityType);
			//
			// Save designs and styles
			XmlOpenElement(designCollectionTag);
			repositoryWriter.Reset(designEntityType.PropertyDefinitions);
			// Save loaded Designs
			foreach (EntityBucket<Design> designItem in cache.LoadedDesigns) {
				if (designItem.State == ItemState.Deleted) continue;
				WriteDesign(cache, designItem.ObjectRef);
			}
			// Save new designs
			foreach (KeyValuePair<Design, IEntity> d in cache.NewDesigns) {
				((IEntity) d.Key).AssignId(Guid.NewGuid());
				WriteDesign(cache, d.Key);
			}
			XmlCloseElement();
		}


		private void WriteDesign(IStoreCache cache, Design design)
		{
			IEntityType designEntityType = cache.FindEntityTypeByName(Design.EntityTypeName);
			string designTag = GetElementTag(designEntityType);
			XmlOpenElement(designTag);
			repositoryWriter.Prepare(design);
			repositoryWriter.WriteId(((IEntity) design).Id);
			((IEntity) design).SaveFields(repositoryWriter, designEntityType.RepositoryVersion);
			foreach (EntityPropertyDefinition pi in designEntityType.PropertyDefinitions)
				if (pi is EntityInnerObjectsDefinition)
					((IEntity) design).SaveInnerObjects(pi.Name, repositoryWriter, designEntityType.RepositoryVersion);
			WriteAllStyles(cache, design);
			XmlCloseElement();
		}


		private void WriteAllStyles(IStoreCache cache, Design design)
		{
			WriteStyles<ColorStyle>(cache, cache.FindEntityTypeByName(ColorStyle.EntityTypeName), design);
			WriteStyles<CapStyle>(cache, cache.FindEntityTypeByName(CapStyle.EntityTypeName), design);
			WriteStyles<CharacterStyle>(cache, cache.FindEntityTypeByName(CharacterStyle.EntityTypeName), design);
			WriteStyles<FillStyle>(cache, cache.FindEntityTypeByName(FillStyle.EntityTypeName), design);
			WriteStyles<LineStyle>(cache, cache.FindEntityTypeByName(LineStyle.EntityTypeName), design);
			WriteStyles<ParagraphStyle>(cache, cache.FindEntityTypeByName(ParagraphStyle.EntityTypeName), design);
		}


		private void WriteStyles<TStyle>(IStoreCache cache, IEntityType styleEntityType, Design design)
		{
			XmlOpenElement(GetElementCollectionTag(styleEntityType));
			repositoryWriter.Reset(styleEntityType.PropertyDefinitions);
			// Save Styles
			foreach (EntityBucket<IStyle> styleItem in cache.LoadedStyles) {
				if (styleItem.State == ItemState.Deleted) continue;
				if (styleItem.Owner == design && styleItem.ObjectRef is TStyle)
					WriteStyle(styleEntityType, styleItem.ObjectRef);
			}
			foreach (KeyValuePair<IStyle, IEntity> s in cache.NewStyles)
				if (s.Key is TStyle) {
					Debug.Assert(s.Key.Id == null);
					s.Key.AssignId(Guid.NewGuid());
					WriteStyle(styleEntityType, s.Key);
				}
			XmlCloseElement();
		}


		private void WriteStyle(IEntityType styleEntityType, IStyle style)
		{
			XmlOpenElement(GetElementTag(styleEntityType));
			repositoryWriter.Prepare(style);
			repositoryWriter.WriteId(style.Id);
			style.SaveFields(repositoryWriter, styleEntityType.RepositoryVersion);
			foreach (EntityPropertyDefinition pi in styleEntityType.PropertyDefinitions)
				if (pi is EntityInnerObjectsDefinition)
					style.SaveInnerObjects(pi.Name, repositoryWriter, styleEntityType.RepositoryVersion);
			XmlCloseElement();
		}


		private void WriteTemplates(IStoreCache cache)
		{
			// Save Templates, Shape and ModelObject
			IEntityType entityType = cache.FindEntityTypeByName(Template.EntityTypeName);
			XmlOpenElement(GetElementCollectionTag(entityType));
			repositoryWriter.Reset(entityType.PropertyDefinitions);
			// Save loaded templates
			foreach (EntityBucket<Template> templateItem in cache.LoadedTemplates) {
				if (templateItem.State == ItemState.Deleted) continue;
				WriteTemplate(cache, entityType, templateItem.ObjectRef);
			}
			// Save new templates, shapes and model objects
			foreach (KeyValuePair<Template, IEntity> t in cache.NewTemplates)
				WriteTemplate(cache, entityType, t.Key);
			XmlCloseElement();
		}


		private void WriteTemplate(IStoreCache cache, IEntityType entityType, Template template)
		{
			XmlOpenElement(templateTag);

			// Save template definition
			repositoryWriter.Reset(entityType.PropertyDefinitions);
			repositoryWriter.Prepare(template);
			if (template.Id == null) template.AssignId(Guid.NewGuid());
			repositoryWriter.WriteId(template.Id);
			template.SaveFields(repositoryWriter, entityType.RepositoryVersion);
			XmlStoreWriter innerObjectsWriter = new XmlStoreWriter(xmlWriter, this, cache);
			if (template.Shape.ModelObject == null) {
				XmlOpenElement("no_model");
				XmlCloseElement();
			}
			else
				WriteModelObject(cache, template.Shape.ModelObject, innerObjectsWriter);
			WriteShape(cache, template.Shape, innerObjectsWriter);
			innerObjectsWriter = null;
			foreach (EntityPropertyDefinition pi in entityType.PropertyDefinitions)
				if (pi is EntityInnerObjectsDefinition)
					template.SaveInnerObjects(pi.Name, repositoryWriter, version);
			repositoryWriter.Finish();

			// Save template's model mappings
			WriteModelMappings(cache, template);

			XmlCloseElement(); // template tag
		}


		private void WriteModelMappings(IStoreCache cache, Template template)
		{
			XmlOpenElement(modelmappingsTag);
			foreach (EntityBucket<IModelMapping> eb in cache.LoadedModelMappings) {
				if (eb.State == ItemState.Deleted) continue;
				// Template shapes have a null Owner
				if (eb.Owner != null && eb.Owner == template)
					WriteModelMapping(cache, eb.ObjectRef, repositoryWriter);
			}
			foreach (KeyValuePair<IModelMapping, IEntity> sp in cache.NewModelMappings) {
				if (sp.Value == template)
					WriteModelMapping(cache, sp.Key, repositoryWriter);
			}
			XmlCloseElement();
		}


		private void WriteModelMapping(IStoreCache cache, IModelMapping modelMapping, XmlStoreWriter writer)
		{
			//IEntityType entityType = cache.FindEntityTypeByName(modelMapping.EntityTypeName);
			string entityTypeName;
			if (modelMapping is NumericModelMapping) entityTypeName = NumericModelMapping.EntityTypeName;
			else if (modelMapping is FormatModelMapping) entityTypeName = FormatModelMapping.EntityTypeName;
			else if (modelMapping is StyleModelMapping) entityTypeName = StyleModelMapping.EntityTypeName;
			else throw new NShapeUnsupportedValueException(modelMapping);

			IEntityType entityType = cache.FindEntityTypeByName(entityTypeName);
			// write Shape-Tag with EntityType
			XmlOpenElement(entityType.ElementName);
			writer.Reset(entityType.PropertyDefinitions);
			writer.Prepare(modelMapping);
			if (((IEntity) modelMapping).Id == null) ((IEntity) modelMapping).AssignId(Guid.NewGuid());
			writer.WriteId(((IEntity) modelMapping).Id);
			((IEntity) modelMapping).SaveFields(writer, entityType.RepositoryVersion);
			foreach (EntityPropertyDefinition pi in entityType.PropertyDefinitions)
				if (pi is EntityInnerObjectsDefinition)
					((IEntity) modelMapping).SaveInnerObjects(pi.Name, writer, entityType.RepositoryVersion);
			XmlCloseElement();
		}


		private void WriteModel(IStoreCache cache)
		{
			if (cache.ModelExists()) {
				IEntityType modelEntityType = cache.FindEntityTypeByName(Model.EntityTypeName);
				string modelTag = GetElementTag(modelEntityType);
				XmlOpenElement(modelTag);
				// Write model
				Model model = cache.GetModel();
				Debug.Assert(model != null);
				repositoryWriter.Prepare(model);
				if (model.Id == null) ((IEntity) model).AssignId(Guid.NewGuid());
				repositoryWriter.WriteId(((IEntity) model).Id);
				((IEntity) model).SaveFields(repositoryWriter, modelEntityType.RepositoryVersion);
				foreach (EntityPropertyDefinition pi in modelEntityType.PropertyDefinitions)
					if (pi is EntityInnerObjectsDefinition)
						((IEntity) model).SaveInnerObjects(pi.Name, repositoryWriter, modelEntityType.RepositoryVersion);

				// Write all model objects
				WriteModelObjects(cache, model);
				XmlCloseElement();
			}
		}


		private void WriteModelObjects(IStoreCache cache, Model model)
		{
			XmlOpenElement(modelObjectsTag);
			// We do not want to write template model objects here.
			foreach (EntityBucket<IModelObject> mob in cache.LoadedModelObjects) {
				if (mob.State == ItemState.Deleted) continue;
				if (mob.Owner == model || mob.Owner is IModelObject)
					WriteModelObject(cache, mob.ObjectRef, repositoryWriter);
			}
			foreach (KeyValuePair<IModelObject, IEntity> mokvp in cache.NewModelObjects)
				if (mokvp.Value == model || mokvp.Value is IModelObject)
					WriteModelObject(cache, mokvp.Key, repositoryWriter);
			XmlCloseElement();
		}


		private void WriteModelObject(IStoreCache cache, IModelObject modelObject, XmlStoreWriter writer)
		{
			IEntityType modelObjectEntityType = cache.FindEntityTypeByName(modelObject.Type.FullName);
			string modelObjectTag = GetElementTag(modelObjectEntityType);
			XmlOpenElement(modelObjectTag);
			writer.Reset(modelObjectEntityType.PropertyDefinitions);
			writer.Prepare(modelObject);
			if (modelObject.Id == null) modelObject.AssignId(Guid.NewGuid());
			writer.WriteId(modelObject.Id);
			modelObject.SaveFields(writer, modelObjectEntityType.RepositoryVersion);
			writer.Finish();
			// ToDo: Save model object's children
			if (cache is IRepository) {
				foreach (IModelObject child in ((IRepository) cache).GetModelObjects(modelObject))
					WriteModelObject(cache, child, writer);
			}
			else throw new NotImplementedException();
			XmlCloseElement();
		}


		private void WriteDiagrams(IStoreCache cache)
		{
			IEntityType diagramEntityType = cache.FindEntityTypeByName(Diagram.EntityTypeName);
			string diagramCollectionTag = GetElementCollectionTag(diagramEntityType);
			XmlOpenElement(diagramCollectionTag);
			// Save loaded diagramControllers
			foreach (EntityBucket<Diagram> diagramItem in cache.LoadedDiagrams) {
				if (diagramItem.State == ItemState.Deleted) continue;
				WriteDiagram(cache, diagramEntityType, diagramItem.ObjectRef);
			}
			// Save new diagrams
			foreach (KeyValuePair<Diagram, IEntity> d in cache.NewDiagrams) {
				Debug.Assert(((IEntity) d.Key).Id == null);
				((IEntity) d.Key).AssignId(Guid.NewGuid());
				WriteDiagram(cache, diagramEntityType, d.Key);
			}
			XmlCloseElement();
		}


		private void WriteDiagram(IStoreCache cache, IEntityType diagramEntityType, Diagram diagram)
		{
			string diagramTag = GetElementTag(diagramEntityType);
			XmlOpenElement(diagramTag);
			repositoryWriter.Reset(diagramEntityType.PropertyDefinitions);
			repositoryWriter.Prepare(diagram);
			repositoryWriter.WriteId(((IEntity) diagram).Id);
			((IEntity) diagram).SaveFields(repositoryWriter, diagramEntityType.RepositoryVersion);
			foreach (EntityPropertyDefinition pi in diagramEntityType.PropertyDefinitions)
				if (pi is EntityInnerObjectsDefinition)
					((IEntity) diagram).SaveInnerObjects(pi.Name, repositoryWriter, diagramEntityType.RepositoryVersion);
			WriteDiagramShapes(cache, diagram);
			WriteDiagramShapeConnections(diagram);
			XmlCloseElement();
		}


		private void WriteDiagramShapes(IStoreCache cache, Diagram diagram)
		{
			XmlOpenElement(shapesTag);
			foreach (EntityBucket<Shape> eb in cache.LoadedShapes) {
				// Template shapes have a null Owner
				if (eb.Owner != null && eb.Owner == diagram && eb.State != ItemState.Deleted)
					WriteShape(cache, eb.ObjectRef, repositoryWriter);
			}
			foreach (KeyValuePair<Shape, IEntity> sp in cache.NewShapes) {
				if (sp.Value == diagram)
					WriteShape(cache, sp.Key, repositoryWriter);
			}
			XmlCloseElement();
		}


		private void WriteShape(IStoreCache cache, Shape shape, XmlStoreWriter writer)
		{
			IEntityType shapeEntityType = cache.FindEntityTypeByName(shape.Type.FullName);
			// write Shape-Tag with EntityType
			XmlOpenElement(shapeEntityType.ElementName);
			writer.Reset(shapeEntityType.PropertyDefinitions);
			writer.Prepare(shape);
			if (((IEntity) shape).Id == null) ((IEntity) shape).AssignId(Guid.NewGuid());
			writer.WriteId(((IEntity) shape).Id);
			((IEntity) shape).SaveFields(writer, shapeEntityType.RepositoryVersion);
			foreach (EntityPropertyDefinition pi in shapeEntityType.PropertyDefinitions)
				if (pi is EntityInnerObjectsDefinition)
					((IEntity) shape).SaveInnerObjects(pi.Name, writer, shapeEntityType.RepositoryVersion);
			// Write children
			if (shape.Children.Count > 0) {
				XmlOpenElement("children");
				foreach (Shape s in shape.Children)
					WriteShape(cache, s, writer);
				XmlCloseElement();
			}
			XmlCloseElement();
		}


		private void WriteDiagramShapeConnections(Diagram diagram)
		{
			XmlOpenElement(connectionTag + "s");
			foreach (Shape shape in diagram.Shapes) {
				// find all gluePoints of the shape
				foreach (ControlPointId gluePointId in shape.GetControlPointIds(ControlPointCapabilities.Glue)) {
					// get connection for each glue point
					ShapeConnectionInfo sci = shape.GetConnectionInfo(gluePointId, null);
					if (!sci.IsEmpty) {
						XmlOpenElement(connectionTag);
						xmlWriter.WriteAttributeString(activeShapeTag, ((IEntity) shape).Id.ToString());
						xmlWriter.WriteAttributeString(gluePointIdTag, gluePointId.ToString());
						xmlWriter.WriteAttributeString(passiveShapeTag, ((IEntity) sci.OtherShape).Id.ToString());
						xmlWriter.WriteAttributeString(connectionPointIdTag, sci.OtherPointId.ToString());
						XmlCloseElement();
					}
				}
			}
			XmlCloseElement();
		}

		#endregion

		#region [Private] Methods: Obtain object tags and field structure

		// TODO 2: Replace this access in place.
		private string GetElementTag(IEntityType entityType)
		{
			return entityType.ElementName;
		}


		private string GetElementCollectionTag(IEntityType entityType)
		{
			return GetElementTag(entityType) + "s";
		}

		#endregion

		#region [Private] Methods: XML helper functions

		private void XmlOpenElement(string name)
		{
			xmlWriter.WriteStartElement(name);
		}


		private void XmlCloseElement()
		{
			xmlWriter.WriteFullEndElement();
		}


		// If the current element is a start element with the given, the function reads
		// it and returns true. If it is not, the function does nothing and returns false.
		private bool XmlReadStartElement(string name)
		{
			if (xmlReader.IsStartElement(name)) {
				xmlReader.Read();
				return true;
			}
			else return false;
		}


		// The current element is either <x a1="1"... /x> or </x>
		private void XmlReadEndElement(string name)
		{
			if (string.Compare(xmlReader.Name, name, StringComparison.InvariantCultureIgnoreCase) == 0
			    && xmlReader.NodeType == XmlNodeType.EndElement)
				xmlReader.ReadEndElement();
		}


		private bool XmlSkipToElement(string nodeName)
		{
			if (string.Compare(xmlReader.Name, nodeName, StringComparison.InvariantCultureIgnoreCase) == 0)
				return true;
			else
				return xmlReader.ReadToFollowing(nodeName);
		}


		// Tests whether we are currently at the beginning of an element with the
		// given name. If so, read into it and return true. Otherwise false.
		private bool XmlSkipStartElement(string nodeName)
		{
			// In case we are at an attribute
			if (xmlReader.NodeType != XmlNodeType.Element && xmlReader.NodeType != XmlNodeType.EndElement) {
				xmlReader.Read();
				xmlReader.MoveToContent();
			}
			if (xmlReader.EOF || string.Compare(xmlReader.Name, nodeName, StringComparison.InvariantCultureIgnoreCase) != 0)
				return false;
			if (xmlReader.IsEmptyElement && !xmlReader.HasAttributes) {
				xmlReader.ReadStartElement(nodeName);
				return false;
			}
			if (!xmlReader.IsEmptyElement && !xmlReader.HasAttributes)
				xmlReader.ReadStartElement(nodeName);
			return true;
		}


		private void XmlSkipEndElement(string nodeName)
		{
			XmlSkipAttributes();
			if (string.Compare(xmlReader.Name, nodeName, StringComparison.InvariantCultureIgnoreCase) == 0) {
				// skip end element
				if (xmlReader.NodeType == XmlNodeType.EndElement)
					xmlReader.ReadEndElement();
					// skip empty element
				else if (xmlReader.NodeType == XmlNodeType.Element && !xmlReader.HasAttributes) {
					xmlReader.Read();
					xmlReader.MoveToContent();
				}
			}
		}


		private void XmlSkipAttributes()
		{
			if (xmlReader.NodeType == XmlNodeType.Attribute) {
				xmlReader.Read();
				xmlReader.MoveToContent();
			}
		}

		#endregion

		#region Fields

		/// <ToBeCompleted></ToBeCompleted>
		protected const string ProjectFileExtension = ".xml";

		// Predefined XML Element Tags
		private const string projectTag = "project";
		private const string shapesTag = "shapes";
		private const string modelObjectsTag = "model_objects";
		private const string rootTag = "dataweb_nshape";
		private const string templateTag = "template";
		private const string modelmappingsTag = "model_mappings";
		private const string connectionsTag = "shape_connections";
		private const string connectionTag = "shape_connection";
		private const string activeShapeTag = "active_shape";
		private const string gluePointIdTag = "glue_point";
		private const string passiveShapeTag = "passive_shape";
		private const string connectionPointIdTag = "connection_point";
		private const string childrenTag = "children";
		private const string shapeStyleTag = "shape_styles";
		// Format string for DateTimes
		private const string datetimeFormat = "yyyy-MM-dd HH:mm:ss";
		// Indicates the highest supported cache version of the built-in entities.
		private const int currentVersion = 100;

		// Directory name of project file. Always != null
		private string directoryName = string.Empty;
		// Name of the project. Always != null
		private string projectName = string.Empty;
		// File extension of project file. Maybe null.
		private string fileExtension = ".xml";

		private bool isOpen;

		// Repository version of the built-in entities.
		private int version;

		// File name of design cache file. Always != null
		private string designFileName = string.Empty;

		/// <summary>Indicates that the whole file is in memory.</summary>
		private bool isOpenComplete = false;

		// element attributes
		private Dictionary<string, string[]> attributeFields = new Dictionary<string, string[]>();

		private string imageDirectory;

		//private MemoryStream memoryStream = null;
		private XmlReader xmlReader;
		private XmlWriter xmlWriter;
		private XmlStoreWriter repositoryWriter = null;
		private XmlStoreReader repositoryReader = null;

		private Type idType = typeof (Guid);

		#endregion
	}
}