using Vixen.Marks;
using VixenModules.App.Marks;

namespace VixenModules.Editor.TimedSequenceEditor.Forms.WPF.MarksDocker.Services
{
	internal static class MarkCollectionImportCommitter
	{
		internal static void Commit(ICollection<IMarkCollection> target, IEnumerable<IMarkCollection> candidates)
		{
			ArgumentNullException.ThrowIfNull(target);
			ArgumentNullException.ThrowIfNull(candidates);

			var selectedCandidates = candidates.ToList();
			if (selectedCandidates.Any(candidate => candidate is null))
			{
				throw new ArgumentException("Candidates cannot contain null collections.", nameof(candidates));
			}
			if (selectedCandidates.Count == 0)
			{
				return;
			}

			NormalizeLinks(target, selectedCandidates);
			NormalizeDefaults(target, selectedCandidates);
			AppendWithUniqueNames(target, selectedCandidates);
		}

		private static void NormalizeLinks(ICollection<IMarkCollection> target, IReadOnlyCollection<IMarkCollection> candidates)
		{
			var validLinkedCollectionIds = target.Select(collection => collection.Id)
				.Concat(candidates.Select(collection => collection.Id))
				.ToHashSet();
			foreach (var candidate in candidates)
			{
				if (!validLinkedCollectionIds.Contains(candidate.LinkedMarkCollectionId))
				{
					candidate.LinkedMarkCollectionId = Guid.Empty;
				}
			}
		}

		private static void NormalizeDefaults(ICollection<IMarkCollection> target, IReadOnlyList<IMarkCollection> candidates)
		{
			if (target.Any(collection => collection.IsDefault))
			{
				SetDefault(candidates, null);
				return;
			}

			var defaultCandidate = candidates.FirstOrDefault(collection => collection.IsDefault)
				?? candidates.FirstOrDefault(collection => collection.IsVisible)
				?? candidates[0];
			SetDefault(candidates, defaultCandidate);
		}

		private static void SetDefault(IEnumerable<IMarkCollection> candidates, IMarkCollection defaultCandidate)
		{
			foreach (var candidate in candidates)
			{
				candidate.IsDefault = ReferenceEquals(candidate, defaultCandidate);
			}
		}

		private static void AppendWithUniqueNames(ICollection<IMarkCollection> target, IEnumerable<IMarkCollection> candidates)
		{
			foreach (var candidate in candidates)
			{
				candidate.Name = MarkCollectionNameService.GetUniqueName(candidate.Name, target);
				target.Add(candidate);
			}
		}
	}
}
