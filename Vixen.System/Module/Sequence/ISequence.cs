using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Why is there Vixen.Sequence.ISequence and Vixen.Module.ISequence?
//
// The former implements the actual sequence interface since a sequence is an
// application-defined entity and is not implemented solely by the sequence module.
// The latter is due to a sequence module being a sequence module and,
// to follow the established pattern, implements an ISequence interface.
//
// What is the difference?
//
// Vixen.Module.ISequence is the actual sequence interface.  Vixen.Module.ISequence
// only implements a single member, the file extension, which is what identifies
// a sequence file being of a sequence module's type.  It has no connection
// to Vixen.Sequence.ISequence.  A sequence module will implement both, indirectly.

namespace Vixen.Module.Sequence {
	/// <summary>
	/// Not to be confused with Vixen.Sequence.ISequence.
	/// That defines the actual sequence interface while this is implemented to implement an actual sequence type with file extension.
	/// </summary>
	//public interface ISequence : Vixen.Sequence.ISequence {
	public interface ISequence {
		/// <summary>
		/// Includes the leading period.
		/// </summary>
		string FileExtension { get; }
	}
}
