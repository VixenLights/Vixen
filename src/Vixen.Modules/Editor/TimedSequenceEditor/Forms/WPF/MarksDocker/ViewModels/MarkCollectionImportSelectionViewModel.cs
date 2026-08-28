using System.Collections.ObjectModel;
using Catel.Data;
using Catel.MVVM;
using Vixen.Marks;

namespace TimedSequenceEditor.Forms.WPF.MarksDocker.ViewModels
{
	internal sealed class MarkCollectionImportSelectionViewModel : ViewModelBase
	{
		private TaskCommand _cancelCommand;
		private TaskCommand _okCommand;
		private Command _toggleSelectedOptionCommand;

		internal MarkCollectionImportSelectionViewModel(
			IEnumerable<IMarkCollection> liveExistingCollections,
			IEnumerable<IMarkCollection> candidates)
		{
			ArgumentNullException.ThrowIfNull(liveExistingCollections);
			ArgumentNullException.ThrowIfNull(candidates);

			var existingCollections = liveExistingCollections.ToList();
			var candidateList = candidates.ToList();
			if (candidateList.Any(candidate => candidate is null))
			{
				throw new ArgumentException("Candidates cannot contain null values.", nameof(candidates));
			}

			Options = new ObservableCollection<MarkCollectionImportOptionViewModel>(candidateList.Select(candidate =>
				new MarkCollectionImportOptionViewModel(candidate, existingCollections)));
			foreach (var option in Options)
			{
				option.InclusionChanged += OptionInclusionChanged;
			}

			SelectedOption = Options.FirstOrDefault();
		}

		public ObservableCollection<MarkCollectionImportOptionViewModel> Options
		{
			get => GetValue<ObservableCollection<MarkCollectionImportOptionViewModel>>(OptionsProperty);
			private set => SetValue(OptionsProperty, value);
		}

		public static readonly IPropertyData OptionsProperty = RegisterProperty<ObservableCollection<MarkCollectionImportOptionViewModel>>(nameof(Options));

		public MarkCollectionImportOptionViewModel SelectedOption
		{
			get => GetValue<MarkCollectionImportOptionViewModel>(SelectedOptionProperty);
			set => SetValue(SelectedOptionProperty, value);
		}

		public static readonly IPropertyData SelectedOptionProperty = RegisterProperty<MarkCollectionImportOptionViewModel>(nameof(SelectedOption));

		public bool HasIncludedOptions => Options.Any(option => option.IsIncluded);

		public IReadOnlyList<IMarkCollection> SelectedCollections { get; private set; } = [];

		public TaskCommand OkCommand => _okCommand ??= new TaskCommand(OkAsync, CanOk);

		public TaskCommand CancelCommand => _cancelCommand ??= new TaskCommand(CancelDialogAsync);

		public Command ToggleSelectedOptionCommand => _toggleSelectedOptionCommand ??= new Command(ToggleSelectedOption);

		private bool CanOk() => HasIncludedOptions;

		private Task OkAsync()
		{
			if (!HasIncludedOptions)
			{
				return Task.CompletedTask;
			}

			SelectedCollections = Options
				.Where(option => option.IsIncluded)
				.Select(option => option.Candidate)
				.ToList();
			return this.SaveAndCloseViewModelAsync();
		}

		private Task CancelDialogAsync() => this.CancelAndCloseViewModelAsync();

		private void ToggleSelectedOption()
		{
			if (SelectedOption is not null)
			{
				SelectedOption.IsIncluded = !SelectedOption.IsIncluded;
			}
		}

		private void OptionInclusionChanged(object sender, EventArgs e)
		{
			RaisePropertyChanged(nameof(HasIncludedOptions));
			_okCommand?.RaiseCanExecuteChanged();
		}
	}
}
