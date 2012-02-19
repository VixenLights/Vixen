namespace Vixen.Sys {
	class FileOperationResult : IFileOperationResult {
		public FileOperationResult(bool result, string message) {
			Result = result;
			Message = message;
		}

		public bool Result { get; private set; }

		public string Message { get; private set; }
	}
}
