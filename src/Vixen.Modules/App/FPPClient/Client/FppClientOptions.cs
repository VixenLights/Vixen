namespace VixenModules.App.FPPClient.Client;

/// <summary>
/// Holds configuration for connecting to a single FPP instance.
/// </summary>
public sealed record FppClientOptions
{
	/// <summary>
	/// Gets the base URL of the FPP REST API, including a trailing slash.
	/// </summary>
	/// <value>The HTTP base URL, for example <c>http://fpp2.home/api/</c>.</value>
	public required string BaseUrl { get; init; }

	/// <summary>
	/// Gets the timeout applied to all non-upload HTTP requests.
	/// </summary>
	/// <value>The request timeout. The default is 30 seconds.</value>
	public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(30);

	/// <summary>
	/// Gets the timeout applied to file upload requests.
	/// </summary>
	/// <value>
	/// The upload timeout. The default is 10 minutes, which accommodates large FSEQ files
	/// sent as a single HTTP body.
	/// </value>
	public TimeSpan UploadTimeout { get; init; } = TimeSpan.FromMinutes(10);
}
