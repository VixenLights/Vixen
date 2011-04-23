using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Vixen.IO;

namespace Vixen.Common {
	abstract public class Definition {
		[DataPath]
		static protected readonly string _definitionDirectory = Path.Combine(Paths.DataRootPath, "Definition");

		protected Definition() { }

		protected void _Save<Writer>(string filePath) 
			where Writer : IWriter, new() {
			IWriter writer = new Writer();
			writer.Write(filePath, this);
			this.FilePath = filePath;
		}

		static protected T Load<T,Reader>(string filePath)
			where T : Definition
			where Reader : IReader, new () {
			IReader reader = new Reader();

			T value = reader.Read(filePath) as T;
			value.FilePath = filePath;

			return value;
		}

		public string Name {
			get { return Path.GetFileNameWithoutExtension(FilePath); }
		}

		public string FilePath { get; protected set; }

		public string DefinitionFileName {
			get { return Path.GetFileName(FilePath); }
		}
	}
}
