using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Vixen.IO;
using Vixen.IO.Result;
using Vixen.Services;
using Vixen.Sys.Attribute;

namespace Vixen.Sys {
	public class Program : IProgram {
		private const string DIRECTORY_NAME = "Program";

		private List<ISequence> _sequences = new List<ISequence>();

		public const string Extension = ".pro";

		[DataPath]
		static public string ProgramDirectory {
			get { return Path.Combine(Paths.DataRootPath, DIRECTORY_NAME); }
		}

		static public IEnumerable<Program> GetAll() {
			return Directory.GetFiles(ProgramDirectory, "*" + Extension).Select(Load);
		}

		static public Program Load(string filePath) {
			VersionedFileSerializer serializer = FileService.Instance.CreateProgramSerializer();
			ISerializationResult result = serializer.Read(filePath);
			return (Program)result.Object;
		}

		public Program() {
		}

		public Program(string name) {
			FilePath = Path.Combine(ProgramDirectory, Path.ChangeExtension(name, Extension));
		}

		public Program(ISequence sequence)
			: this(sequence.Name) {
			Add(sequence);
		}

		public Program(IProgram original) {
			FilePath = original.FilePath;
			_sequences.AddRange(original.Sequences);
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
		}

		public void Save(string filePath) {
			VersionedFileSerializer serializer = FileService.Instance.CreateProgramSerializer();
			serializer.Write(this, filePath);
		}

		public void Save() {
			Save(FilePath);
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
