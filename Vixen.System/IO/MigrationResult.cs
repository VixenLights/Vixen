using Vixen.Sys;

namespace Vixen.IO {
	class MigrationResult : IFileOperationResult {
		public MigrationResult(bool result, string message, int fromVersion, int toVersion) {
			Result = result;
			Message = "Migration from version " + fromVersion + " to " + toVersion + ". " + message;
		}

		public bool Result { get; private set; }

		public string Message { get; private set; }
	}
}
