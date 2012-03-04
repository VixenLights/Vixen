using System.Collections.Generic;
using Vixen.Sys;

namespace Vixen.IO.Result {
	class ScriptSequenceSerializationResult : SerializationResult<ScriptSequence> {
		public ScriptSequenceSerializationResult(bool result, string message, ScriptSequence sequence, IEnumerable<IFileOperationResult> operationResults)
			: base(result, message, sequence, operationResults) {
		}
	}
}
