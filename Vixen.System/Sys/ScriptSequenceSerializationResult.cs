using System.Collections.Generic;

namespace Vixen.Sys {
	class ScriptSequenceSerializationResult : SerializationResult<ScriptSequence> {
		public ScriptSequenceSerializationResult(bool result, string message, ScriptSequence sequence, IEnumerable<IFileOperationResult> operationResults)
			: base(result, message, sequence, operationResults) {
		}
	}
}
