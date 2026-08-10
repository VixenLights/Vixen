namespace VixenModules.Effect.Video;

internal static class VideoCacheCleanup
{
	internal static void RemovePairingFiles(string cacheRoot, Guid instanceId)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(cacheRoot);

		if (!Directory.Exists(cacheRoot))
		{
			return;
		}

		foreach (var pairingFile in Directory.EnumerateFiles(cacheRoot, $"{instanceId}.*", SearchOption.TopDirectoryOnly))
		{
			File.Delete(pairingFile);
		}
	}

	internal static void RemoveUnpairedCacheDirectories(string cacheRoot)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(cacheRoot);

		if (!Directory.Exists(cacheRoot))
		{
			return;
		}

		var referencedDirectories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (var pairingFile in Directory.EnumerateFiles(cacheRoot, "*", SearchOption.TopDirectoryOnly))
		{
			var cacheKey = Path.GetExtension(pairingFile);
			if (cacheKey.Length <= 1)
			{
				continue;
			}

			var cacheDirectory = Path.Combine(cacheRoot, cacheKey[1..]);
			if (Directory.Exists(cacheDirectory))
			{
				referencedDirectories.Add(cacheDirectory);
			}
		}

		foreach (var cacheDirectory in Directory.EnumerateDirectories(cacheRoot, "*", SearchOption.TopDirectoryOnly))
		{
			if (!referencedDirectories.Contains(cacheDirectory))
			{
				Directory.Delete(cacheDirectory, true);
			}
		}
	}

	internal static void DeleteResolvedCacheDirectory(string cacheRoot, string cacheKey)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(cacheRoot);

		if (string.IsNullOrWhiteSpace(cacheKey))
		{
			return;
		}

		var cacheRootPath = Path.GetFullPath(cacheRoot).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
		var cacheDirectory = Path.GetFullPath(Path.Combine(cacheRootPath, cacheKey));
		if (!cacheDirectory.StartsWith(cacheRootPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
		{
			return;
		}

		if (Directory.Exists(cacheDirectory))
		{
			Directory.Delete(cacheDirectory, true);
		}
	}
}
