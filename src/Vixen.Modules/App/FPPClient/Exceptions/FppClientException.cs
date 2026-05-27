namespace VixenModules.App.FPPClient.Exceptions;

/// <summary>
/// Thrown when an FPP REST API call fails due to a non-success HTTP status or an unexpected response body.
/// </summary>
public sealed class FppClientException : Exception
{
	/// <summary>
	/// Gets the HTTP status code returned by FPP, or <see langword="null"/> if the failure occurred before a response was received.
	/// </summary>
	public int? HttpStatusCode { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="FppClientException"/> class with a specified error message.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	public FppClientException(string message) : base(message) { }

	/// <summary>
	/// Initializes a new instance of the <see cref="FppClientException"/> class with a specified error message
	/// and the HTTP status code returned by the server.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	/// <param name="httpStatusCode">The HTTP status code returned by the FPP server.</param>
	public FppClientException(string message, int httpStatusCode)
		: base(message) { HttpStatusCode = httpStatusCode; }

	/// <summary>
	/// Initializes a new instance of the <see cref="FppClientException"/> class with a specified error message
	/// and a reference to the inner exception that caused this exception.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	/// <param name="inner">The exception that is the cause of the current exception.</param>
	public FppClientException(string message, Exception inner)
		: base(message, inner) { }
}
