using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Vixen.IO;
using Vixen.IO.Xml;
using Vixen.Sys;

namespace Vixen.Sys {
	public class Program : IEnumerable<ISequence>, IVersioned {
		private const string DIRECTORY_NAME = "Program";
		private const int VERSION = 1;

		private List<ISequence> _sequences = new List<ISequence>();

		public const string Extension = ".pro";

		public Program(string name) {
			FilePath = Path.Combine(Program.Directory, Path.ChangeExtension(name, Program.Extension));
		}

		public Program(ISequence sequence)
			: this(sequence.Name) {
			Add(sequence);
		}

		public Program(Program original) {
			FilePath = original.FilePath;
			_sequences.AddRange(original.Sequences);
		}

		[DataPath]
		static public string Directory {
			get { return Path.Combine(Paths.DataRootPath, DIRECTORY_NAME); }
		}

		static public IEnumerable<Program> GetAll() {
			foreach(string filePath in System.IO.Directory.GetFiles(Program.Directory, "*" + Extension)) {
				yield return Load(filePath);
			}
		}

		static public Program Load(string filePath) {
			if(string.IsNullOrWhiteSpace(filePath)) return null;

			filePath = Path.ChangeExtension(filePath, Program.Extension);
			IReader reader = new XmlProgramReader();
			if(!Path.IsPathRooted(filePath)) filePath = Path.Combine(Directory, filePath);
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

		public void Clear() {
			_sequences.Clear();
		}

		public List<ISequence> Sequences {
			get { return _sequences; }
			set { _sequences = value; }
		}

		public void Save(string filePath) {
			if(string.IsNullOrWhiteSpace(filePath)) throw new InvalidOperationException("A name is required.");
			filePath = Path.Combine(Directory, Path.GetFileName(filePath));
			filePath = Path.ChangeExtension(filePath, Program.Extension);

			IWriter writer = new XmlProgramWriter();
			writer.Write(filePath, this);

			FilePath = filePath;
		}

		public void Save() {
			Save(FilePath);
		}

		public int Version {
			get { return VERSION; }
		}

		public TimeSpan Length {
			get { return _sequences.Aggregate(TimeSpan.Zero, (value, sequence) => value + sequence.Length); }
		}

		public IEnumerator<ISequence> GetEnumerator() {
			return _sequences.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
