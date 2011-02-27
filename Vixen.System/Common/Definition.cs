using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace Vixen.Common {
	abstract public class Definition {
		private const string ELEMENT_ROOT = "Definition";

		// No caching.

		[DataPath]
		static protected readonly string _definitionDirectory = Path.Combine(Paths.DataRootPath, "Definition");

		protected Definition() {
		}

		//*** don't forget a version!
		//-> Should probably be defined by the subclasses
		public void Save(string fileName) {
			using(FileStream fileStream = new FileStream(fileName, FileMode.Create)) {
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Indent = true;
				using(XmlWriter writer = XmlWriter.Create(fileStream, settings)) {
					writer.WriteStartElement(ELEMENT_ROOT);
					WriteAttributes(writer);
					WriteBody(writer);
					writer.WriteEndElement();
				}
			}
			FileName = fileName;
		}

		public void Save() {
			Save(FileName);
		}

		public void Load(string fileName) {
			using(FileStream fileStream = new FileStream(fileName, FileMode.Open)) {
				XmlReaderSettings settings = new XmlReaderSettings();
				settings.IgnoreWhitespace = true;
				using(XmlReader reader = XmlReader.Create(fileStream, settings)) {
					reader.Read();
					if(reader.NodeType == XmlNodeType.XmlDeclaration) {
						reader.Read();
					}

					ReadAttributes(reader);
					if(reader.ElementsExistWithin(ELEMENT_ROOT)) // Entity element
					{
						ReadBody(reader);
						reader.ReadEndElement(); // Definition
					}

				}
			}
			FileName = fileName;
		}

		public string Name {
			get { return Path.GetFileNameWithoutExtension(FileName); }
		}

		static protected T _GetInstance<T>(string directory, string definitionName, string fileExtension)
			where T : Definition, new() {
			T definition = null;
			string filePath = Path.Combine(directory, Path.ChangeExtension(definitionName, fileExtension));
			if(File.Exists(filePath)) {
				definition = new T();
				definition.Load(filePath);
			}
			return definition;
		}
		
		static protected IEnumerable<T> _GetAll<T>(string directory, string fileExtension)
			where T : Definition, new() {
			T definition = null;
			foreach(string fileName in Directory.GetFiles(directory, "*" + fileExtension)) {
				definition = new T();
				definition.Load(fileName);

				if(definition != null) {
					yield return definition;
				}
			}
		}

		public string FileName { get; private set; }

		public string DefinitionName {
			get { return Path.GetFileNameWithoutExtension(FileName); }
		}

		abstract protected void ReadAttributes(XmlReader reader);
		abstract protected void ReadBody(XmlReader reader);
		abstract protected void WriteAttributes(XmlWriter writer);
		abstract protected void WriteBody(XmlWriter writer);
	}
}
