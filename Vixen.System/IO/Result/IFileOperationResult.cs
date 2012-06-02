namespace Vixen.IO.Result {
	interface IFileOperationResult {
		bool Success { get; }
		string Message { get; }
	}
}
