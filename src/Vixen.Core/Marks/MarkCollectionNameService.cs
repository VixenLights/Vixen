namespace Vixen.Marks
{
	/// <summary>
	/// Provides name validation and repair helpers for Mark Collections.
	/// </summary>
	public static class MarkCollectionNameService
	{
		private const string DefaultName = "Mark Collection";

		/// <summary>
		/// Determines whether a proposed Mark Collection name is unique.
		/// </summary>
		/// <param name="collections">The collections to compare against.</param>
		/// <param name="proposedName">The proposed collection name.</param>
		/// <param name="excludedCollectionId">The optional collection identifier to ignore during comparison.</param>
		/// <returns><see langword="true" /> if the proposed name is non-empty and unique; otherwise, <see langword="false" />.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="collections" /> is <see langword="null" />.</exception>
		public static bool IsUniqueName(IEnumerable<IMarkCollection> collections, string proposedName, Guid? excludedCollectionId = null)
		{
			ArgumentNullException.ThrowIfNull(collections);

			var normalizedName = NormalizeName(proposedName);
			if (normalizedName.Length == 0)
			{
				return false;
			}

			return collections
				.Where(collection => !excludedCollectionId.HasValue || collection.Id != excludedCollectionId.Value)
				.All(collection => !NamesMatch(collection.Name, normalizedName));
		}

		/// <summary>
		/// Gets a unique Mark Collection name from a desired name.
		/// </summary>
		/// <param name="desiredName">The desired collection name.</param>
		/// <param name="collections">The collections to compare against.</param>
		/// <param name="excludedCollectionId">The optional collection identifier to ignore during comparison.</param>
		/// <returns>A unique collection name using the desired name or a numeric suffix.</returns>
		/// <exception cref="ArgumentException"><paramref name="desiredName" /> is empty or contains only white-space characters.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="collections" /> is <see langword="null" />.</exception>
		public static string GetUniqueName(string desiredName, IEnumerable<IMarkCollection> collections, Guid? excludedCollectionId = null)
		{
			ArgumentNullException.ThrowIfNull(collections);

			var baseName = NormalizeName(desiredName);
			if (baseName.Length == 0)
			{
				throw new ArgumentException("The desired name cannot be empty.", nameof(desiredName));
			}

			var names = collections
				.Where(collection => !excludedCollectionId.HasValue || collection.Id != excludedCollectionId.Value)
				.Select(collection => NormalizeName(collection.Name))
				.Where(name => name.Length > 0)
				.ToHashSet(StringComparer.OrdinalIgnoreCase);

			return GetUniqueName(baseName, names);
		}

		/// <summary>
		/// Renames duplicate Mark Collections so each collection has a unique name.
		/// </summary>
		/// <param name="collections">The ordered collection list to repair.</param>
		/// <returns>The records describing each renamed collection.</returns>
		/// <remarks>
		/// The first occurrence of each name is preserved. Later duplicates receive numeric suffixes such as
		/// <c> - 2</c> and <c> - 3</c>, skipping suffixes that are already in use.
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="collections" /> is <see langword="null" />.</exception>
		public static IReadOnlyList<MarkCollectionRenameRecord> RenameDuplicates(IList<IMarkCollection> collections)
		{
			ArgumentNullException.ThrowIfNull(collections);

			var renameRecords = new List<MarkCollectionRenameRecord>();
			var occupiedNames = collections
				.Select(collection => NormalizeName(collection.Name))
				.Where(name => name.Length > 0)
				.ToHashSet(StringComparer.OrdinalIgnoreCase);
			var seenNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			foreach (var collection in collections)
			{
				var originalName = NormalizeName(collection.Name);
				if (originalName.Length == 0)
				{
					var replacementName = GetUniqueName(DefaultName, occupiedNames);
					var blankName = collection.Name;
					collection.Name = replacementName;
					occupiedNames.Add(replacementName);
					seenNames.Add(replacementName);
					renameRecords.Add(new MarkCollectionRenameRecord(collection.Id, blankName, replacementName));
					continue;
				}

				if (seenNames.Add(originalName))
				{
					if (!string.Equals(collection.Name, originalName, StringComparison.Ordinal))
					{
						var previousName = collection.Name;
						collection.Name = originalName;
						renameRecords.Add(new MarkCollectionRenameRecord(collection.Id, previousName, originalName));
					}

					continue;
				}

				var uniqueName = GetUniqueName(originalName, occupiedNames);
				var oldName = collection.Name;
				collection.Name = uniqueName;
				occupiedNames.Add(uniqueName);
				renameRecords.Add(new MarkCollectionRenameRecord(collection.Id, oldName, uniqueName));
			}

			return renameRecords;
		}

		private static string GetUniqueName(string baseName, ISet<string> occupiedNames)
		{
			if (!occupiedNames.Contains(baseName))
			{
				return baseName;
			}

			var suffix = 2;
			string candidateName;
			do
			{
				candidateName = $"{baseName} - {suffix}";
				suffix++;
			} while (occupiedNames.Contains(candidateName));

			return candidateName;
		}

		private static bool NamesMatch(string currentName, string normalizedName)
		{
			return string.Equals(NormalizeName(currentName), normalizedName, StringComparison.OrdinalIgnoreCase);
		}

		private static string NormalizeName(string name)
		{
			return name?.Trim() ?? string.Empty;
		}
	}
}
