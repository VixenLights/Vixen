using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Catel.MVVM;
using Vixen.Services;
using Vixen.Sys;

namespace Common.ElementTagManager.ViewModels
{
	/// <summary>
	/// Backs <see cref="Views.ElementTagManagerWindow"/>, listing the built-in element tags for color editing.
	/// </summary>
	/// <remarks>
	/// Exposes an <see cref="ObservableCollection{T}"/> rather than fixed fields for the three built-in tags so a
	/// future Tag Manager feature can extend this same list to include user-defined tags without changing this
	/// view model's shape.
	/// </remarks>
	public class ElementTagManagerWindowViewModel : ViewModelBase
	{
		private readonly bool _saveOnClose;

		public ElementTagManagerWindowViewModel(bool saveOnClose)
		{
			_saveOnClose = saveOnClose;
			Tags = new ObservableCollection<TagColorItem>(
				ElementTagService.Instance.GetAll()
					.Where(tag => tag.IsBuiltIn)
					.OrderBy(tag => tag.SortOrder)
					.Select(tag => new TagColorItem(tag)));
		}

		/// <inheritdoc />
		public override string Title => "Manage Tag Colors";

		/// <summary>
		/// Gets the tags being edited.
		/// </summary>
		public ObservableCollection<TagColorItem> Tags { get; }

		private Command _okCommand;

		/// <summary>
		/// Gets the command that saves the staged color changes and closes the window.
		/// </summary>
		public Command OkCommand => _okCommand ??= new Command(Ok);

		/// <summary>
		/// Method to invoke when <see cref="OkCommand"/> is executed.
		/// </summary>
		private void Ok()
		{
			this.SaveAndCloseViewModelAsync();
		}

		private Command _cancelCommand;

		/// <summary>
		/// Gets the command that discards the staged color changes and closes the window.
		/// </summary>
		public Command CancelCommand => _cancelCommand ??= new Command(Cancel);

		/// <summary>
		/// Method to invoke when <see cref="CancelCommand"/> is executed.
		/// </summary>
		private void Cancel()
		{
			this.CancelAndCloseViewModelAsync();
		}

		/// <inheritdoc />
		/// <remarks>
		/// Always commits every <see cref="TagColorItem"/>'s staged color back to its <see cref="ElementTagDefinition"/>.
		/// Whether that also persists to disk is controlled by the <c>saveOnClose</c> constructor argument: a caller
		/// with its own OK/Cancel-gated save (Display Setup, Preview Setup) passes <see langword="false"/> and leaves
		/// persistence to its own save path; a caller with no other save path (the Sequencer) passes
		/// <see langword="true"/> so this method persists the change itself.
		/// </remarks>
		protected override async Task<bool> SaveAsync()
		{
			foreach (TagColorItem item in Tags)
			{
				item.CommitColor();
			}

			if (_saveOnClose)
			{
				await VixenSystem.SaveSystemConfigAsync();
			}

			return true;
		}
	}
}
