using Catel.Data;
using Catel.MVVM;
using System.Collections.ObjectModel;
using Vixen.Marks;

namespace TimedSequenceEditor.Forms.WPF.MarksDocker.ViewModels
{
	internal class MarkExportWindowViewModel : ViewModelBase
	{
		public MarkExportWindowViewModel(ObservableCollection<IMarkCollection> markCollections)
		{
			MarkCollections = markCollections;
			SelectedItems = new ObservableCollection<IMarkCollection>();
		}

		#region MarkCollections model property

		/// <summary>
		/// Gets or sets the MarkCollections value.
		/// </summary>
		[Model]
		public ObservableCollection<IMarkCollection> MarkCollections
		{
			get { return GetValue<ObservableCollection<IMarkCollection>>(MarkCollectionsProperty); }
			set { SetValue(MarkCollectionsProperty, value); }
		}

		/// <summary>
		/// MarkCollections property data.
		/// </summary>
		public static readonly IPropertyData MarkCollectionsProperty = RegisterProperty<ObservableCollection<IMarkCollection>>(nameof(MarkCollections));

		#endregion

		#region SelectedItems property

		/// <summary>
		/// Gets or sets the SelectedItems value.
		/// </summary>
		public ObservableCollection<IMarkCollection> SelectedItems
		{
			get { return GetValue<ObservableCollection<IMarkCollection>>(SelectedProperty); }
			set { SetValue(SelectedProperty, value); }
		}

		/// <summary>
		/// SelectedItems property data.
		/// </summary>
		public static readonly IPropertyData SelectedProperty = RegisterProperty<ObservableCollection<IMarkCollection>>(nameof(SelectedItems));

		#endregion


		#region Cancel command

		private Command _cancelCommand;

		/// <summary>
		/// Gets the Cancel command.
		/// </summary>
		public Command CancelCommand
		{
			get { return _cancelCommand ??= new Command(Cancel); }
		}

		/// <summary>
		/// Method to invoke when the Cancel command is executed.
		/// </summary>
		public void Cancel()
		{
			this.CancelAndCloseViewModelAsync();
		}

		#endregion

		#region Ok command

		private Command _okCommand;

		/// <summary>
		/// Gets the Ok command.
		/// </summary>
		public Command OkCommand
		{
			get { return _okCommand ??= new Command(Ok); }
		}

		/// <summary>
		/// Method to invoke when the Ok command is executed.
		/// </summary>
		private void Ok()
		{
			this.SaveAndCloseViewModelAsync();

		}

		#endregion
	}
}
