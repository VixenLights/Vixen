namespace Vixen.Sys {
	class FileOperationResult : IFileOperationResult {
		public FileOperationResult(bool result, string message) {
			Success = result;
			Message = message;
		}

		public bool Success { get; private set; }

		public string Message { get; private set; }
	}
}
