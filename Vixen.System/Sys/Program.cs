using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Vixen.IO;
using Vixen.IO.Xml;
using Vixen.Common;

namespace Vixen.Sys {
	public class Program : IVersioned {
		private const string DIRECTORY_NAME = "Program";
		private const int VERSION = 1;

		// Has to be a TimedSequence because otherwise there will be no end
		// time to watch for to move the program along.
		private List<ISequence> _sequences = new List<ISequence>();

		public const string Extension = ".pro";

		public Program(string name) {
			FilePath = Path.Combine(Program.Directory, Path.ChangeExtension(name, Program.Extension));
		}

		[DataPath]
		static private string Directory {
			get { return Path.Combine(Paths.DataRootPath, DIRECTORY_NAME); }
		}

		static public IEnumerable<Program> GetAll() {
			foreach(string filePath in System.IO.Directory.GetFiles(Program.Directory, "*" + Extension)) {
				yield return _LoadFromFile(filePath);
			}
		}

		static public Program Load(string name) {
			if(!name.EndsWith(Program.Extension)) name += Program.Extension;
			return _LoadFromFile(Path.Combine(Program.Directory, name));
		}

		static private Program _LoadFromFile(string filePath) {
			IReader reader = new XmlProgramReader();
			Program program = (Program)reader.Read(filePath);
			return program;
		}

		public string FilePath { get; set; }

		public string Name {
			get { return Path.GetFileNameWithoutExtension(FilePath); }
		}

		public void Add(ISequence sequence) {
			_sequences.Add(sequence);
		}

		// Has to be a TimedSequence because otherwise there will be no end
		// time to watch for to move the program along.
		public IEnumerable<ISequence> Sequences {
			get { return _sequences; }
		}

		public void Save() {
			IWriter writer = new XmlProgramWriter();
			writer.Write(FilePath, this);
		}

		public int Version {
			get { return VERSION; }
		}
	}
}
