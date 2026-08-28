using Catel.Data;
using Catel.MVVM;
using Vixen.Marks;

namespace TimedSequenceEditor.Forms.WPF.MarksDocker.ViewModels
{
	internal sealed class MarkCollectionImportOptionViewModel : ViewModelBase
	{
		internal MarkCollectionImportOptionViewModel(IMarkCollection candidate, IEnumerable<IMarkCollection> liveExistingCollections)
		{
			ArgumentNullException.ThrowIfNull(candidate);
			ArgumentNullException.ThrowIfNull(liveExistingCollections);

			Candidate = candidate;
			DisplayName = candidate.Name;
			HasDuplicateName = !MarkCollectionNameService.IsUniqueName(liveExistingCollections, candidate.Name);
			IsIncluded = true;
		}

		internal event EventHandler InclusionChanged;

		public IMarkCollection Candidate { get; }

		public string DisplayName { get; }

		public bool IsIncluded
		{
			get => GetValue<bool>(IsIncludedProperty);
			set
			{
				if (IsIncluded == value)
				{
					return;
				}

				SetValue(IsIncludedProperty, value);
				InclusionChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		public static readonly IPropertyData IsIncludedProperty = RegisterProperty<bool>(nameof(IsIncluded));

		public bool HasDuplicateName { get; }

		public string DuplicateNameMessage => HasDuplicateName ? "Name already exists in this sequence." : string.Empty;
	}
}
