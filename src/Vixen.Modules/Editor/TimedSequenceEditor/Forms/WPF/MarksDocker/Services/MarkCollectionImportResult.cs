using Vixen.Marks;

namespace VixenModules.Editor.TimedSequenceEditor.Forms.WPF.MarksDocker.Services
{
	/// <summary>
	/// Represents the detached collections produced by one Mark Collection import attempt.
	/// </summary>
	internal sealed record MarkCollectionImportResult
	{
		private MarkCollectionImportResult(MarkCollectionImportType importType, MarkCollectionImportStatus status, IReadOnlyList<IMarkCollection> collections)
		{
			ImportType = importType;
			Status = status;
			Collections = collections;
		}

		/// <summary>
		/// Gets the source format that produced this result.
		/// </summary>
		public MarkCollectionImportType ImportType { get; }

		/// <summary>
		/// Gets the completion status of the import attempt.
		/// </summary>
		public MarkCollectionImportStatus Status { get; }

		/// <summary>
		/// Gets the detached mark collection candidates in source order.
		/// </summary>
		public IReadOnlyList<IMarkCollection> Collections { get; }

		/// <summary>
		/// Creates a successful import result.
		/// </summary>
		/// <param name="importType">One of the enumeration values that specifies the source format.</param>
		/// <param name="collections">The detached mark collection candidates in source order.</param>
		/// <returns>A successful import result.</returns>
		public static MarkCollectionImportResult Succeeded(MarkCollectionImportType importType, IEnumerable<IMarkCollection> collections)
		{
			ArgumentNullException.ThrowIfNull(collections);

			return new MarkCollectionImportResult(importType, MarkCollectionImportStatus.Succeeded, collections.ToList());
		}

		/// <summary>
		/// Creates a cancelled import result.
		/// </summary>
		/// <param name="importType">One of the enumeration values that specifies the source format.</param>
		/// <returns>A cancelled import result with no candidates.</returns>
		public static MarkCollectionImportResult Cancelled(MarkCollectionImportType importType)
		{
			return new MarkCollectionImportResult(importType, MarkCollectionImportStatus.Cancelled, Array.Empty<IMarkCollection>());
		}

		/// <summary>
		/// Creates a failed import result.
		/// </summary>
		/// <param name="importType">One of the enumeration values that specifies the source format.</param>
		/// <returns>A failed import result with no candidates.</returns>
		public static MarkCollectionImportResult Failed(MarkCollectionImportType importType)
		{
			return new MarkCollectionImportResult(importType, MarkCollectionImportStatus.Failed, Array.Empty<IMarkCollection>());
		}
	}
}
