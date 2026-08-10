using VixenModules.Effect.Video;
using Xunit;

namespace Vixen.Tests.Effect.Video;

public sealed class VideoCacheCleanupTests : IDisposable
{
	private readonly string _cacheRoot = Path.Combine(Path.GetTempPath(), $"VIX-3981-{Guid.NewGuid():N}");

	[Fact]
	public void RemovePairingFiles_WhenCacheRootDoesNotExist_DoesNotThrow()
	{
		// Act
		VideoCacheCleanup.RemovePairingFiles(_cacheRoot, Guid.NewGuid());

		// Assert
		Assert.False(Directory.Exists(_cacheRoot));
	}

	[Fact]
	public void RemoveUnpairedCacheDirectories_WhenCacheRootDoesNotExist_DoesNotThrow()
	{
		// Act
		VideoCacheCleanup.RemoveUnpairedCacheDirectories(_cacheRoot);

		// Assert
		Assert.False(Directory.Exists(_cacheRoot));
	}

	[Fact]
	public void RemovePairingFiles_RemovesOnlyMatchingInstanceFiles()
	{
		// Arrange
		var firstInstanceId = Guid.NewGuid();
		var secondInstanceId = Guid.NewGuid();
		var cacheKey = "0123456789ABCDEF0123456789ABCDEF";
		Directory.CreateDirectory(Path.Combine(_cacheRoot, cacheKey));
		var firstPairingFile = Path.Combine(_cacheRoot, $"{firstInstanceId}.{cacheKey}");
		var secondPairingFile = Path.Combine(_cacheRoot, $"{secondInstanceId}.{cacheKey}");
		File.WriteAllText(firstPairingFile, string.Empty);
		File.WriteAllText(secondPairingFile, string.Empty);

		// Act
		VideoCacheCleanup.RemovePairingFiles(_cacheRoot, firstInstanceId);

		// Assert
		Assert.False(File.Exists(firstPairingFile));
		Assert.True(File.Exists(secondPairingFile));
		Assert.True(Directory.Exists(Path.Combine(_cacheRoot, cacheKey)));
	}

	[Fact]
	public void RemoveUnpairedCacheDirectories_DeletesOnlyDirectoriesWithoutPairings()
	{
		// Arrange
		var pairedCacheKey = "0123456789ABCDEF0123456789ABCDEF";
		var unpairedCacheKey = "FEDCBA9876543210FEDCBA9876543210";
		var pairedDirectory = Directory.CreateDirectory(Path.Combine(_cacheRoot, pairedCacheKey));
		var unpairedDirectory = Directory.CreateDirectory(Path.Combine(_cacheRoot, unpairedCacheKey));
		File.WriteAllText(Path.Combine(_cacheRoot, $"{Guid.NewGuid()}.{pairedCacheKey}"), string.Empty);

		// Act
		VideoCacheCleanup.RemoveUnpairedCacheDirectories(_cacheRoot);

		// Assert
		Assert.True(pairedDirectory.Exists);
		Assert.False(unpairedDirectory.Exists);
	}

	[Fact]
	public void DeleteResolvedCacheDirectory_WhenCacheKeyIsEmpty_DoesNotDeleteRoot()
	{
		// Arrange
		Directory.CreateDirectory(_cacheRoot);
		var sentinelFile = Path.Combine(_cacheRoot, "sentinel.txt");
		File.WriteAllText(sentinelFile, "keep");

		// Act
		VideoCacheCleanup.DeleteResolvedCacheDirectory(_cacheRoot, string.Empty);

		// Assert
		Assert.True(Directory.Exists(_cacheRoot));
		Assert.True(File.Exists(sentinelFile));
	}

	[Fact]
	public void DeleteResolvedCacheDirectory_WhenCacheKeyResolvesToRoot_DoesNotDeleteRoot()
	{
		// Arrange
		Directory.CreateDirectory(_cacheRoot);
		var sentinelFile = Path.Combine(_cacheRoot, "sentinel.txt");
		File.WriteAllText(sentinelFile, "keep");

		// Act
		VideoCacheCleanup.DeleteResolvedCacheDirectory(_cacheRoot, ".");

		// Assert
		Assert.True(Directory.Exists(_cacheRoot));
		Assert.True(File.Exists(sentinelFile));
	}

	[Fact]
	public void DeleteResolvedCacheDirectory_WhenCacheKeyIsResolved_DeletesOnlyThatDirectory()
	{
		// Arrange
		var cacheKey = "0123456789ABCDEF0123456789ABCDEF";
		var cacheDirectory = Directory.CreateDirectory(Path.Combine(_cacheRoot, cacheKey));
		var sentinelFile = Path.Combine(_cacheRoot, "sentinel.txt");
		File.WriteAllText(sentinelFile, "keep");

		// Act
		VideoCacheCleanup.DeleteResolvedCacheDirectory(_cacheRoot, cacheKey);

		// Assert
		Assert.False(cacheDirectory.Exists);
		Assert.True(Directory.Exists(_cacheRoot));
		Assert.True(File.Exists(sentinelFile));
	}

	public void Dispose()
	{
		if (Directory.Exists(_cacheRoot))
		{
			Directory.Delete(_cacheRoot, true);
		}
	}
}
