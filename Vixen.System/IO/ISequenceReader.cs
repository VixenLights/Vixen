using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sequence;

namespace Vixen.IO {
	// Doing it this way so that a sequence instance can have a Load method.
	// Otherwise, the reader creates an instance.
	public interface ISequenceReader {
		bool Read(string filePath, ISequence sequence);
	}

	public interface ISequenceReader<T> : ISequenceReader
		where T : ISequence {
		bool Read(string filePath, T sequence);
	}
}
