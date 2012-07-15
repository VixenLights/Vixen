using System;
using System.Collections.Generic;

namespace Vixen.Sys {
	public interface IProgram : IEnumerable<ISequence> {
		string FilePath { get; set; }
		string Name { get; }
		void Add(ISequence sequence);
		void Clear();
		List<ISequence> Sequences { get; }
		void Save(string filePath);
		void Save();
		TimeSpan Length { get; }
	}
}
