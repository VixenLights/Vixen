using System.Collections.Generic;
using Vixen.Sys;

namespace Vixen.IO.Result {
	class SystemConfigSerializationResult : SerializationResult<SystemConfig> {
		public SystemConfigSerializationResult(bool result, string message, SystemConfig systemConfig, IEnumerable<IFileOperationResult> operationResults)
			: base(result, message, systemConfig, operationResults) {
		}
	}
}
