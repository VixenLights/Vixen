namespace VixenModules.App.CustomPropEditor.Persistence;

internal sealed class PropPersistenceException : Exception
{
	public PropPersistenceException(string safeMessage, string diagnosticMessage = null, Exception innerException = null)
		: base(safeMessage, innerException)
	{
		DiagnosticMessage = diagnosticMessage ?? safeMessage;
	}

	public string DiagnosticMessage { get; }
}
