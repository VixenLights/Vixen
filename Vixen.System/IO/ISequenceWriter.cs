using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sequence;

namespace Vixen.IO {
	public interface ISequenceWriter {
		void Write(string filePath, ISequence sequence);
	}

	public interface ISequenceWriter<T> : ISequenceWriter
		where T : ISequence {
		void Write(string filePath, T sequence);
	}
}
