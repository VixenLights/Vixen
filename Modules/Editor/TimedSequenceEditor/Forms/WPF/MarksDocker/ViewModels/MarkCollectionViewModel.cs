using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Catel.Data;
using Catel.IoC;
using Catel.MVVM;
using Vixen.Marks;
using VixenModules.App.Marks;

namespace VixenModules.Editor.TimedSequenceEditor.Forms.WPF.MarksDocker.ViewModels
{
	public class MarkCollectionViewModel: ViewModelBase
	{
		private System.Timers.Timer _nameclickTimer = null;
		private string _textHold = String.Empty;
		private ObservableCollection<IMarkCollection> _markCollections;

		public MarkCollectionViewModel(MarkCollection markCollection)
		{
			MarkCollection = markCollection;
			var mgr = ServiceLocator.Default.ResolveType<IViewModelManager>();
			var pvm = mgr.GetFirstOrDefaultInstance<MarkDockerViewModel>();
			_markCollections = pvm.MarkCollections;
			_markCollections.CollectionChanged += ParentMarkCollections_CollectionChanged;
			SetupCollectionTypeCheckboxes();
			SetupLevelCheckboxes();
			SetupLinkedToCheckboxes();
		}

		private void ParentMarkCollections_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			SetupLinkedToCheckboxes();
		}

		#region Overrides of ViewModelBase

		/// <inheritdoc />
		protected override Task CloseAsync()
		{
			return Task.Factory.StartNew(() =>
			{
				foreach (var checkBoxState in CollectionTypeCheckBoxStates)
				{
					checkBoxState.CloseViewModelAsync(true);
				}

				foreach (var checkBoxState in LevelCheckBoxStates)
				{
					checkBoxState.CloseViewModelAsync(true);
				}

				foreach (var checkBoxState in LinkedToCheckBoxStates)
				{
					checkBoxState.CloseViewModelAsync(true);
				}

				_markCollections.CollectionChanged -= ParentMarkCollections_CollectionChanged;
			});
		}

		#endregion

		#region MarkCollection model property

		/// <summary>
		/// Gets or sets the MarkCollection value.
		/// </summary>
		[Model]
		public MarkCollection MarkCollection
		{
			get { return GetValue<MarkCollection>(MarkCollectionProperty); }
			private set { SetValue(MarkCollectionProperty, value); }
		}

		/// <summary>
		/// MarkCollection property data.
		/// </summary>
		public static readonly PropertyData MarkCollectionProperty = RegisterProperty("MarkCollection", typeof(MarkCollection));

		#endregion

		#region Name property

		/// <summary>
		/// Gets or sets the Name value.
		/// </summary>
		[ViewModelToModel("MarkCollection")]
		public string Name
		{
			get { return GetValue<string>(NameProperty); }
			set { SetValue(NameProperty, value); }
		}

		/// <summary>
		/// Name property data.
		/// </summary>
		public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string), null);

		#endregion

		#region ShowGridLines property

		/// <summary>
		/// Gets or sets the IsEnabled value.
		/// </summary>
		[ViewModelToModel("MarkCollection")]
		public bool ShowGridLines
		{
			get { return GetValue<bool>(ShowGridLinesProperty); }
			set { SetValue(ShowGridLinesProperty, value); }
		}

		/// <summary>
		/// IsEnabled property data.
		/// </summary>
		public static readonly PropertyData ShowGridLinesProperty = RegisterProperty("ShowGridLines", typeof(bool), null);

		#endregion

		#region ShowMarkBar property

		/// <summary>
		/// Gets or sets the ShowMarkBar value.
		/// </summary>
		[ViewModelToModel("MarkCollection")]
		public bool ShowMarkBar
		{
			get { return GetValue<bool>(ShowMarkBarProperty); }
			set { SetValue(ShowMarkBarProperty, value); }
		}

		/// <summary>
		/// ShowMarkBar property data.
		/// </summary>
		public static readonly PropertyData ShowMarkBarProperty = RegisterProperty("ShowMarkBar", typeof(bool), null);

		#endregion

		#region IsDefault property

		/// <summary>
		/// Gets or sets the IsDefault value.
		/// </summary>
		[ViewModelToModel("MarkCollection")]
		public bool IsDefault
		{
			get { return GetValue<bool>(IsDefaultProperty); }
			set { SetValue(IsDefaultProperty, value); }
		}

		/// <summary>
		/// IsDefault property data.
		/// </summary>
		public static readonly PropertyData IsDefaultProperty = RegisterProperty("IsDefault", typeof(bool), null);

		#endregion

		#region Level property

		/// <summary>
		/// Gets or sets the SnapLevel value.
		/// </summary>
		[ViewModelToModel("MarkCollection")]
		public int Level
		{
			get { return GetValue<int>(LevelProperty); }
			set { SetValue(LevelProperty, value); }
		}

		/// <summary>
		/// SnapLevel property data.
		/// </summary>
		public static readonly PropertyData LevelProperty = RegisterProperty("Level", typeof(int), null);

		#endregion

		#region CollectionType property

		/// <summary>
		/// Gets or sets the CollectionType value.
		/// </summary>
		[ViewModelToModel("MarkCollection")]
		public MarkCollectionType CollectionType
		{
			get { return GetValue<MarkCollectionType>(CollectionTypeProperty); }
			set { SetValue(CollectionTypeProperty, value); }
		}

		/// <summary>
		/// CollectionType property data.
		/// </summary>
		public static readonly PropertyData CollectionTypeProperty = RegisterProperty("CollectionType", typeof(MarkCollectionType), null);

		#endregion
		
		#region BeginEdit command

		private Command<MouseButtonEventArgs> _beginEditCommand;

		/// <summary>
		/// Gets the LeftMouseUp command.
		/// </summary>
		
		public Command<MouseButtonEventArgs> BeginEditCommand
		{
			get { return _beginEditCommand ?? (_beginEditCommand = new Command<MouseButtonEventArgs>(BeginEdit)); }
		}

		/// <summary>
		/// Method to invoke when the LeftMouseUp command is executed.
		/// </summary>
		private void BeginEdit(MouseButtonEventArgs e)
		{
			if (_nameclickTimer == null)
			{
				_nameclickTimer = new System.Timers.Timer { Interval = 300};
				_nameclickTimer.Elapsed += nameClickTimer_Elapsed;
			}

			if (!_nameclickTimer.Enabled)
			{ // Equal: (e.ClickCount == 1)
				_nameclickTimer.Start();
			}
			else
			{ // Equal: (e.ClickCount == 2)
				_nameclickTimer.Stop();
				_textHold = Name;
				IsEditing = true;
			}

		}

		private void nameClickTimer_Elapsed(object sender, EventArgs e)
		{ // single-clicked
			_nameclickTimer.Stop();

		}

		#endregion

		#region IsEditing property

		/// <summary>
		/// Gets or sets the IsEditing value.
		/// </summary>
		public bool IsEditing
		{
			get { return GetValue<bool>(IsEditingProperty); }
			set { SetValue(IsEditingProperty, value); }
		}

		/// <summary>
		/// IsEditing property data.
		/// </summary>
		public static readonly PropertyData IsEditingProperty = RegisterProperty("IsEditing", typeof(bool));

		#endregion

		#region DoneEditing command

		private Command _doneEditingCommand;

		/// <summary>
		/// Gets the EditFocusLost command.
		/// </summary>
		
		public Command DoneEditingCommand
		{
			get { return _doneEditingCommand ?? (_doneEditingCommand = new Command(DoneEditing)); }
		}

		/// <summary>
		/// Method to invoke when the EditFocusLost command is executed.
		/// </summary>
		private void DoneEditing()
		{
			if (string.IsNullOrEmpty(Name))
			{
				Name = _textHold;
			}
			IsEditing = false;
			IsDirty = true;
		}

		#endregion

		#region CancelEditing command

		private Command _cancelEditingCommand;

		/// <summary>
		/// Gets the CancelEditing command.
		/// </summary>
		public Command CancelEditingCommand
		{
			get { return _cancelEditingCommand ?? (_cancelEditingCommand = new Command(CancelEditing)); }
		}

		/// <summary>
		/// Method to invoke when the CancelEditing command is executed.
		/// </summary>
		private void CancelEditing()
		{
			Name = _textHold;
			IsEditing = false;
		}

		#endregion	

		#region Decorator property

		/// <summary>
		/// Gets or sets the Decorator value.
		/// </summary>
		[ViewModelToModel("MarkCollection")]
		public MarkDecorator Decorator
		{
			get { return GetValue<MarkDecorator>(DecoratorProperty); }
			set { SetValue(DecoratorProperty, value); }
		}

		/// <summary>
		/// Decorator property data.
		/// </summary>
		public static readonly PropertyData DecoratorProperty = RegisterProperty("Decorator", typeof(IMarkDecorator), null);

		#endregion

		#region PickColor command

		private Command _pickColorCommand;

		/// <summary>
		/// Gets the PickColor command.
		/// </summary>
		public Command PickColorCommand
		{
			get { return _pickColorCommand ?? (_pickColorCommand = new Command(PickColor)); }
		}

		/// <summary>
		/// Method to invoke when the PickColor command is executed.
		/// </summary>
		private void PickColor()
		{
			var picker = new Common.Controls.ColorManagement.ColorPicker.ColorPicker();
			var result = picker.ShowDialog();
			if (result == DialogResult.OK)
			{
				Decorator.Color = picker.Color.ToRGB().ToArgb();
			}
		}

		#endregion

		#region Delete command

		private Command _deleteCommand;

		/// <summary>
		/// Gets the Delete command.
		/// </summary>
		public Command DeleteCommand
		{
			get { return _deleteCommand ?? (_deleteCommand = new Command(Delete)); }
		}

		/// <summary>
		/// Method to invoke when the Delete command is executed.
		/// </summary>
		private void Delete()
		{
			var model = ParentViewModel as MarkDockerViewModel;
			model?.DeleteCollection(MarkCollection);
		}

		#endregion

		#region CollectionTypeCheckBoxStates property

		/// <summary>
		/// Gets or sets the CheckBoxStates value.
		/// </summary>
		public ObservableCollection<CollectionTypeCheckBoxState> CollectionTypeCheckBoxStates
		{
			get { return GetValue<ObservableCollection<CollectionTypeCheckBoxState>>(CheckBoxStatesProperty); }
			set { SetValue(CheckBoxStatesProperty, value); }
		}

		/// <summary>
		/// CheckBoxStates property data.
		/// </summary>
		public static readonly PropertyData CheckBoxStatesProperty = RegisterProperty("CollectionTypeCheckBoxStates", typeof(ObservableCollection<CollectionTypeCheckBoxState>));

		#endregion

		#region LinkedToCheckBoxStates property

		/// <summary>
		/// Gets or sets the LinkedToCheckBoxState value.
		/// </summary>
		public ObservableCollection<LinkedToCheckBoxState> LinkedToCheckBoxStates
		{
			get { return GetValue<ObservableCollection<LinkedToCheckBoxState>>(LinkedToCheckBoxStatesProperty); }
			set { SetValue(LinkedToCheckBoxStatesProperty, value); }
		}

		/// <summary>
		/// LinkedToCheckBoxState property data.
		/// </summary>
		public static readonly PropertyData LinkedToCheckBoxStatesProperty = RegisterProperty("LinkedToCheckBoxStates", typeof(ObservableCollection<LinkedToCheckBoxState>));

		#endregion

		#region LevelCheckBoxStates property

		/// <summary>
		/// Gets or sets the LevelCheckBoxStates value.
		/// </summary>
		public ObservableCollection<LevelCheckBoxState> LevelCheckBoxStates
		{
			get { return GetValue<ObservableCollection<LevelCheckBoxState>>(LevelCheckBoxStatesProperty); }
			set { SetValue(LevelCheckBoxStatesProperty, value); }
		}

		/// <summary>
		/// LevelCheckBoxStates property data.
		/// </summary>
		public static readonly PropertyData LevelCheckBoxStatesProperty = RegisterProperty("LevelCheckBoxStates", typeof(ObservableCollection<LevelCheckBoxState>));

		#endregion

		#region IsPhonemeType property

		/// <summary>
		/// Gets or sets the IsPhonemeType value.
		/// </summary>
		public bool IsLinkableType
		{
			get { return GetValue<bool>(IsLinkableTypeProperty); }
			set { SetValue(IsLinkableTypeProperty, value); }
		}

		/// <summary>
		/// IsPhonemeType property data.
		/// </summary>
		public static readonly PropertyData IsLinkableTypeProperty = RegisterProperty("IsLinkableType", typeof(bool));

		#endregion
		
		#region ResolveCollectionType command

		private Command<CollectionTypeCheckBoxState> _resolveCollectionTypeCommand;

		/// <summary>
		/// Gets the ResolveCollectionType command.
		/// </summary>
		public Command<CollectionTypeCheckBoxState> ResolveCollectionTypeCommand
		{
			get { return _resolveCollectionTypeCommand ?? (_resolveCollectionTypeCommand = new Command<CollectionTypeCheckBoxState>(ResolveCollectionType)); }
		}

		/// <summary>
		/// Method to invoke when the ResolveCollectionType command is executed.
		/// </summary>
		private void ResolveCollectionType(CollectionTypeCheckBoxState state)
		{
			if (state.Value)
			{
				CollectionType = state.Type;
				foreach (var checkBoxState in CollectionTypeCheckBoxStates.Where(x => x.Type != state.Type))
				{
					checkBoxState.Value = false;
				}

				SetupLinkedToCheckboxes();
			}
		}

		#endregion

		#region ResolveSnapLevelCommand command

		private Command<LevelCheckBoxState> _resolveLevelCommand;

		/// <summary>
		/// Gets the ResolveSnapLevelCommand command.
		/// </summary>
		public Command<LevelCheckBoxState> ResolveLevelCommand
		{
			get { return _resolveLevelCommand ?? (_resolveLevelCommand = new Command<LevelCheckBoxState>(ResolveLevel)); }
		}

		/// <summary>
		/// Method to invoke when the ResolveSnapLevelCommand command is executed.
		/// </summary>
		private void ResolveLevel(LevelCheckBoxState state)
		{
			if (state.Value)
			{
				Level = state.Level;
				foreach (var snapLevelCheckBoxState in LevelCheckBoxStates.Where(x => x.Level != state.Level))
				{
					snapLevelCheckBoxState.Value = false;
				}
			}
		}

		#endregion

		#region ResolveLinkTo command

		private Command<LinkedToCheckBoxState> _resolveLinkToCommand;

		/// <summary>
		/// Gets the ResolveLinkTo command.
		/// </summary>
		public Command<LinkedToCheckBoxState> ResolveLinkToCommand
		{
			get { return _resolveLinkToCommand ?? (_resolveLinkToCommand = new Command<LinkedToCheckBoxState>(ResolveLinkTo, CanResolveLinkTo)); }
		}

		/// <summary>
		/// Method to invoke when the ResolveLinkTo command is executed.
		/// </summary>
		private void ResolveLinkTo(LinkedToCheckBoxState state)
		{
			if (state.Value)
			{
				if (state.ParentId != Guid.Empty)
				{
					foreach (var markCollection in _markCollections)
					{
						if (markCollection.LinkedMarkCollectionId == state.ParentId)
						{
							markCollection.LinkedMarkCollectionId = Guid.Empty;
						}
					}
				}
				
				MarkCollection.LinkedMarkCollectionId = state.ParentId;
				foreach (var linkedToCheckBoxState in LinkedToCheckBoxStates.Where(x => x.ParentId != state.ParentId))
				{
					linkedToCheckBoxState.Value = false;
				}
			}
		}

		/// <summary>
		/// Method to check whether the ResolveLinkTo command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool CanResolveLinkTo(LinkedToCheckBoxState state)
		{
			return true;
		}

		#endregion

		#region MenuOpen command

		private Command _menuOpenCommand;

		/// <summary>
		/// Gets the MenuOpen command.
		/// </summary>
		public Command MenuOpenCommand
		{
			get { return _menuOpenCommand ?? (_menuOpenCommand = new Command(MenuOpen)); }
		}

		/// <summary>
		/// Method to invoke when the MenuOpen command is executed.
		/// </summary>
		private void MenuOpen()
		{
			SetupLinkedToCheckboxes();
		}

		#endregion
		
		private void SetupCollectionTypeCheckboxes()
		{
			CollectionTypeCheckBoxStates = new ObservableCollection<CollectionTypeCheckBoxState>();
			foreach (MarkCollectionType value in Enum.GetValues(typeof(MarkCollectionType)))
			{
				CollectionTypeCheckBoxStates.Add(new CollectionTypeCheckBoxState() { Text = value.ToString(), Value = (value == CollectionType), Type = value});
			}
		}

		private void SetupLevelCheckboxes()
		{
			LevelCheckBoxStates = new ObservableCollection<LevelCheckBoxState>();
			for (int i = 1; i <= 6; i++)
			{
				LevelCheckBoxStates.Add(new LevelCheckBoxState { Text = i.ToString(), Value = Level == i, Level = i });
			}
		}

		private void SetupLinkedToCheckboxes()
		{
			LinkedToCheckBoxStates = new ObservableCollection<LinkedToCheckBoxState>();

			MarkCollectionType linkToType;
			switch (MarkCollection.CollectionType)
			{
				case MarkCollectionType.Phoneme:
					linkToType = MarkCollectionType.Word;
					break;
				case MarkCollectionType.Word:
					linkToType = MarkCollectionType.Phrase;
					break;
				default:
					linkToType = MarkCollectionType.Generic;
					break;
			}

			if (linkToType == MarkCollectionType.Generic)
			{
				IsLinkableType = false;
				return;
			}

			IsLinkableType = true;

			foreach (var mc in _markCollections.Where(x => x.CollectionType == linkToType))
			{
				LinkedToCheckBoxStates.Add(new LinkedToCheckBoxState() { Text = mc.Name, Value = MarkCollection.LinkedMarkCollectionId == mc.Id, ParentId = mc.Id});
			}

			LinkedToCheckBoxStates.Add(new LinkedToCheckBoxState() { Text = @"None", Value = MarkCollection.LinkedMarkCollectionId == Guid.Empty, ParentId = Guid.Empty });
		}
	}
}
