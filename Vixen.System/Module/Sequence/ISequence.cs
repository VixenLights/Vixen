using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Sequence {
	/// <summary>
	/// Not to be confused with Vixen.Sequence.ISequence.
	/// That defines the actual sequence interface while this is implemented to implement an actual sequence type with file extension.
	/// </summary>
	public interface ISequence : Vixen.Sequence.ISequence {
		/// <summary>
		/// Includes the leading period.
		/// </summary>
		string FileExtension { get; }
	}
}
