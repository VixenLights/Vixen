using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys {
	class SystemConfigSerializationResult : SerializationResult<SystemConfig> {
		public SystemConfigSerializationResult(bool result, string message, SystemConfig systemConfig, IEnumerable<IFileOperationResult> operationResults)
			: base(result, message, systemConfig, operationResults) {
		}
	}
}
