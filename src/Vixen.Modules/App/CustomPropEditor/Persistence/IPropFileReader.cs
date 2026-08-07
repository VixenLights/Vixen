namespace VixenModules.App.CustomPropEditor.Persistence;

internal interface IPropFileReader
{
	Task<PropFileReadResult> ReadAsync(string path, CancellationToken cancellationToken = default);
}
