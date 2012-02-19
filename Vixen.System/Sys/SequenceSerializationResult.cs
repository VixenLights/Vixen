using System.Collections.Generic;

namespace Vixen.Sys {
	class SequenceSerializationResult : SerializationResult<Sequence> {
		public SequenceSerializationResult(bool result, string message, Sequence sequence, IEnumerable<IFileOperationResult> operationResults)
			: base(result, message, sequence, operationResults) {
		}
	}
}
