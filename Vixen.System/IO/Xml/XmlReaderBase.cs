using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Vixen.Sys;

namespace Vixen.IO.Xml {
	abstract class XmlReaderBase<T> : IReader 
		where T : class, IVersioned {
		private const string ATTR_VERSION = "version";

		object IReader.Read(string filePath) {
			return Read(filePath);
		}

		virtual public T Read(string filePath) {
			// Load the XML data.
			XElement element = _LoadContent(filePath);

			// Create an instance of the object.
			T instance = _CreateObject(element, filePath);

			// Perform any necessary version tranformations on the data.
			if(_MigrateData(ref element, instance)) {
				// Save a backup of the original file.
				_SaveBackup(filePath, _GetFileVersion(element));
				// Update the version in the file.
				_SetFileVersion(element, instance.Version);
				// Save the updated file contents.
				element.Save(filePath);
			}
			
			// Only after the data is current can it be applied to the object.
			_PopulateObject(instance, element);

			return instance;
		}

		virtual protected XElement _LoadContent(string filePath) {
			return Helper.LoadXml(filePath);
		}

		virtual protected void _SaveBackup(string filePath, int fileVersion) {
			string backupFilePath = filePath + "." + fileVersion;
			File.Copy(filePath, backupFilePath, true);
		}

		abstract protected T _CreateObject(XElement element, string filePath);

		abstract protected void _PopulateObject(T obj, XElement element);

		virtual protected IEnumerable<Func<XElement, XElement>> _ProvideMigrations(int versionAt, int targetVersion) {
			return new Func<XElement, XElement>[] { };
		}

		private bool _MigrateData(ref XElement element, IVersioned instance) {
			int fileVersion = _GetFileVersion(element);
			switch(Comparer<int>.Default.Compare(instance.Version, fileVersion)) {
				case -1: // object version < file version
					throw new Exception("This file was created by a newer version and cannot be loaded by this version.");
				case 1: // object version > file version
					IEnumerable<Func<XElement, XElement>> migrations = _ProvideMigrations(fileVersion, instance.Version);
					foreach(Func<XElement, XElement> migration in migrations) {
						element = migration(element);
					}
					return true;
			}

			return false;
		}

		private int _GetFileVersion(XElement rootElement) {
			return int.Parse(rootElement.Attribute(ATTR_VERSION).Value);
		}

		private void _SetFileVersion(XElement rootElement, int value) {
			rootElement.SetAttributeValue(ATTR_VERSION, value);
		}
	}
}
