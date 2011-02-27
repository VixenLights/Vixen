using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.IO;
using System.IO;
using System.Xml;
using Vixen.Sequence;
using Vixen.Common;
using Vixen.Module.Sequence;

namespace Vixen.Sys {
	public class Program {
		private const string DIRECTORY_NAME = "Program";

		// Has to be a TimedSequence because otherwise there will be no end
		// time to watch for to move the program along.
		private List<ISequenceModuleInstance> _sequences = new List<ISequenceModuleInstance>();

		public const string Extension = ".pro";

		public Program() {
			// Used when loading a program.
			Name = "Unnamed program";
		}

		public Program(string name) {
			Name = name;
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
			ProgramReader reader = new ProgramReader();
			reader.Read(filePath);
			return reader.Program;
		}

		public string Name { get; set; }

		public void Add(ISequenceModuleInstance sequence) {
			_sequences.Add(sequence);
		}

		// Has to be a TimedSequence because otherwise there will be no end
		// time to watch for to move the program along.
		public IEnumerable<ISequenceModuleInstance> Sequences {
			get { return _sequences; }
		}

		public void Save() {
			string filePath = Path.Combine(Program.Directory, this.Name + Program.Extension);
			ProgramWriter writer = new ProgramWriter();
			writer.Program = this;
			writer.Write(filePath);
		}
	}
}
