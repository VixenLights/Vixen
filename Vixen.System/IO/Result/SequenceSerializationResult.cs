using System.Collections.Generic;
using Vixen.Sys;

namespace Vixen.IO.Result {
	class SequenceSerializationResult : SerializationResult<Sequence> {
		public SequenceSerializationResult(bool result, string message, Sequence sequence, IEnumerable<IFileOperationResult> operationResults)
			: base(result, message, sequence, operationResults) {
		}
	}
}
