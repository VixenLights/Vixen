using System.Collections.Concurrent;
using System.IO;

namespace VixenModules.App.CustomPropEditor.Persistence;

internal sealed class AtomicPropFileWriter
{
	private static readonly ConcurrentDictionary<string, SemaphoreSlim> WriteLocks = new(StringComparer.OrdinalIgnoreCase);

	internal static string GetLegacyBackupPath(string destinationPath) => $"{Path.GetFullPath(destinationPath)}.legacy-litedb.bak";

	public async Task PublishAsync(string temporaryPath, string destinationPath, string legacyBackupPath = null, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(temporaryPath);
		ArgumentException.ThrowIfNullOrWhiteSpace(destinationPath);
		var canonicalDestination = Path.GetFullPath(destinationPath);
		var gate = WriteLocks.GetOrAdd(canonicalDestination, static _ => new SemaphoreSlim(1, 1));
		await gate.WaitAsync(cancellationToken);
		try
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (File.Exists(canonicalDestination))
			{
				if (legacyBackupPath != null && File.Exists(legacyBackupPath))
					throw new PropPersistenceException("The prop could not be saved.", "The requested legacy backup already exists and will not be overwritten.");
				try
				{
					File.Replace(temporaryPath, canonicalDestination, legacyBackupPath, ignoreMetadataErrors: true);
					return;
				}
				catch (PlatformNotSupportedException) { }
				catch (IOException) when (!OperatingSystem.IsWindows()) { }
			}

			// The source is already validated. Move with overwrite is an atomic replacement on supported file systems.
			File.Move(temporaryPath, canonicalDestination, overwrite: true);
		}
		finally
		{
			gate.Release();
		}
	}
}
