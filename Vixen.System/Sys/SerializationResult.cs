using System.Collections.Generic;

namespace Vixen.Sys {
	class SerializationResult<ObjectType> : IFileOperationResult {
		public SerializationResult(bool result, string message, ObjectType obj, IEnumerable<IFileOperationResult> operationResults) {
			Result = result;
			Message = message;
			OperationResults = operationResults;
			Object = obj;
		}

		public bool Result { get; private set; }

		public string Message { get; private set; }

		public ObjectType Object { get; private set; }

		public IEnumerable<IFileOperationResult> OperationResults { get; private set; }
	}
}
