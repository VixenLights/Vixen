using Catel.Data;
using Catel.MVVM;
using System.Collections.ObjectModel;
using Catel.Collections;
using Vixen.Marks;
using VixenModules.Editor.TimedSequenceEditor.Forms.WPF.MarksDocker.Services;

namespace TimedSequenceEditor.Forms.WPF.MarksDocker.ViewModels
{
	internal class MarkExportWindowViewModel : ViewModelBase
	{
		public MarkExportWindowViewModel(ObservableCollection<IMarkCollection> markCollections)
		{
			MarkCollections = markCollections;
			//SelectedItems = new ObservableCollection<IMarkCollection>();
			ExportOptionsVmList = new ObservableCollection<MarkCollectionExportRowViewModel>();
			ExportOptionsVmList.AddRange(MarkCollections.Select(x => new MarkCollectionExportRowViewModel(x)));
			IsVixenFormat = true;
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

		#region ExportOptionsVmList property

		/// <summary>
		/// Gets or sets the ExportOptionsVmList value.
		/// </summary>
		public ObservableCollection<MarkCollectionExportRowViewModel> ExportOptionsVmList
		{
			get { return GetValue<ObservableCollection<MarkCollectionExportRowViewModel>>(MarkCollectionExportOptionsProperty); }
			set { SetValue(MarkCollectionExportOptionsProperty, value); }
		}

		/// <summary>
		/// ExportOptionsVmList property data.
		/// </summary>
		public static readonly IPropertyData MarkCollectionExportOptionsProperty = RegisterProperty<ObservableCollection<MarkCollectionExportRowViewModel>>(nameof(ExportOptionsVmList));

		#endregion

		#region IsVixenFormat property

		/// <summary>
		/// Gets or sets the IsVixenFormat value.
		/// </summary>
		public bool IsVixenFormat 
		{
			get { return GetValue<bool>(IsVixenFormatProperty); }
			set { SetValue(IsVixenFormatProperty, value); } 
		}

		/// <summary>
		/// IsVixenFormat property data.
		/// </summary>
		public static readonly IPropertyData IsVixenFormatProperty = RegisterProperty<bool>(nameof(IsVixenFormat));

		#endregion

		#region IsAudacityFormat property

		/// <summary>
		/// Gets or sets the IsAudacityFormat value.
		/// </summary>
		public bool IsAudacityFormat
		{
			get { return GetValue<bool>(IsAudacityFormatProperty); }
			set { SetValue(IsAudacityFormatProperty, value); }
		}

		/// <summary>
		/// IsAudacityFormat property data.
		/// </summary>
		public static readonly IPropertyData IsAudacityFormatProperty = RegisterProperty<bool>(nameof(IsAudacityFormat));

		#endregion

		#region IsPangolinBeyondFormat property

		/// <summary>
		/// Gets or sets the IsPangolinBeyondFormat value.
		/// </summary>
		public bool IsPangolinBeyondFormat
		{
			get { return GetValue<bool>(IsPangolinBeyondFormatProperty); }
			set
			{
				SetValue(IsPangolinBeyondFormatProperty, value);
				IsTextOptionVisible = value;
			}
		}

		/// <summary>
		/// IsPangolinBeyondFormat property data.
		/// </summary>
		public static readonly IPropertyData IsPangolinBeyondFormatProperty = RegisterProperty<bool>(nameof(IsPangolinBeyondFormat));

		#endregion

		#region IsTextOptionVisible property

		/// <summary>
		/// Gets or sets the IsTextOptionVisible value.
		/// </summary>
		public bool IsTextOptionVisible
		{
			get { return GetValue<bool>(IsTextOptionVisibleProperty); }
			set { SetValue(IsTextOptionVisibleProperty, value); }
		}

		/// <summary>
		/// IsTextOptionVisible property data.
		/// </summary>
		public static readonly IPropertyData IsTextOptionVisibleProperty = RegisterProperty<bool>(nameof(IsTextOptionVisible));

		#endregion

		public MarkExportType MarkExportType
		{
			get
			{
				if(IsAudacityFormat) return MarkExportType.Audacity;
				if(IsPangolinBeyondFormat) return MarkExportType.PangolinBeyond;
				return MarkExportType.Vixen;
			}
		}


		//#region SelectedItems property

		///// <summary>
		///// Gets or sets the SelectedItems value.
		///// </summary>
		//public ObservableCollection<IMarkCollection> SelectedItems
		//{
		//	get { return GetValue<ObservableCollection<IMarkCollection>>(SelectedProperty); }
		//	set { SetValue(SelectedProperty, value); }
		//}

		///// <summary>
		///// SelectedItems property data.
		///// </summary>
		//public static readonly IPropertyData SelectedProperty = RegisterProperty<ObservableCollection<IMarkCollection>>(nameof(SelectedItems));

		//#endregion


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
