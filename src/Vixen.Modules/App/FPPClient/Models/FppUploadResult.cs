namespace VixenModules.App.FPPClient.Models;

/// <summary>
/// Maps the response envelope returned by the <c>POST file/:DirName/:Filename</c> upload endpoint.
/// </summary>
internal sealed record FppUploadResult
{
	/// <summary>Gets the upload status string (e.g. <c>"OK"</c>).</summary>
	public string Status { get; init; } = string.Empty;

	/// <summary>Gets the filename reported by FPP after the upload, or <see langword="null"/> if not present.</summary>
	public string? File { get; init; }

	/// <summary>Gets the directory reported by FPP after the upload, or <see langword="null"/> if not present.</summary>
	public string? Dir { get; init; }
}
