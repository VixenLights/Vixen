using System.Collections.Generic;
using Vixen.Sys;

namespace Vixen.IO.Result {
	class SerializationResult : IFileOperationResult {
		public SerializationResult(bool success, string message, object obj, IEnumerable<IFileOperationResult> operationResults) {
			Success = success;
			Message = message;
			OperationResults = operationResults;
			Object = obj;
		}

		public bool Success { get; private set; }

		public string Message { get; private set; }

		public object Object { get; private set; }

		public IEnumerable<IFileOperationResult> OperationResults { get; private set; }
	}

	class SerializationResult<ObjectType> : SerializationResult {
		public SerializationResult(bool success, string message, ObjectType obj, IEnumerable<IFileOperationResult> operationResults)
			: base(success, message, obj, operationResults) {
			Object = obj;
		}

		new public ObjectType Object { get; private set; }
	}
}
