using System.Collections.ObjectModel;
using System.Diagnostics;
using Catel.Data;
using Catel.MVVM;
using GongSolutions.Wpf.DragDrop;
using Vixen.Sys;
using VixenModules.Property.State.Setup.Models;

namespace VixenModules.Property.State.Setup.ViewModels
{
	public class StateMapperViewModel : ViewModelBase, GongSolutions.Wpf.DragDrop.IDropTarget
	{
		private const string FormTitle = @"State Mapper";
		private readonly IEnumerable<IElementNode> _selectedNodes;

		public StateMapperViewModel(IEnumerable<IElementNode> selectedNodes)
		{
			Elements = selectedNodes.ToList();
			StateDefinitions = new ObservableCollection<StateDefinition>();
			Title = FormTitle;
		}

		protected override async Task InitializeAsync()
		{
			await base.InitializeAsync();
		}

		#region Title property

			/// <summary>
			/// Gets or sets the Title value.
			/// </summary>
		public new string Title
		{
			get => GetValue<string>(TitleProperty);
			set => SetValue(TitleProperty, value);
		}

		/// <summary>
		/// Title property data.
		/// </summary>
		public static readonly IPropertyData TitleProperty = RegisterProperty<string>(nameof(Title));

		#endregion

		#region Elements property

		/// <summary>
		/// Gets or sets the targeted Elements list.
		/// </summary>
		public List<IElementNode> Elements
		{
			get => GetValue<List<IElementNode>>(ElementsProperty);
			set => SetValue(ElementsProperty, value);
		}

		/// <summary>
		/// Elements property data.
		/// </summary>
		public static readonly IPropertyData ElementsProperty = RegisterProperty<List<IElementNode>>(nameof(Elements));

		#endregion

		#region StateDefinitions property

		/// <summary>
		/// Gets or sets the Elements value.
		/// </summary>
		public ObservableCollection<StateDefinition> StateDefinitions
		{
			get => GetValue<ObservableCollection<StateDefinition>> (StateDefinitionsProperty);
			set => SetValue(StateDefinitionsProperty, value);
		}

		/// <summary>
		/// Elements property data.
		/// </summary>
		public static readonly IPropertyData StateDefinitionsProperty = RegisterProperty<ObservableCollection<StateDefinition>>(nameof(StateDefinitions));

		#endregion

		#region AddDefinition command

		private TaskCommand _addDefinitionCommand;

		/// <summary>
		/// Gets the AddDefinition command.
		/// </summary>
		public TaskCommand AddDefinitionCommand => _addDefinitionCommand ??= new TaskCommand(AddDefinitionAsync);

		/// <summary>
		/// Method to invoke when the AddDefinition command is executed.
		/// </summary>
		private async Task AddDefinitionAsync()
		{
			StateDefinitions.Add(new StateDefinition()
			{
				StateColor = System.Drawing.Color.Silver
			});
		}

		#endregion

		#region EditColor command

		private TaskCommand<StateDefinition> _editColorCommand;

		/// <summary>
		/// Gets the EditColor command.
		/// </summary>
		public TaskCommand<StateDefinition> EditColorCommand => _editColorCommand ??= new TaskCommand<StateDefinition>(EditColorAsync);

		/// <summary>
		/// Method to invoke when the AddDefinition command is executed.
		/// </summary>
		private async Task EditColorAsync(StateDefinition stateDefinition)
		{
			if (stateDefinition != null)
			{
				stateDefinition.StateColor = System.Drawing.Color.Black;
				stateDefinition.StateName = Guid.NewGuid().ToString();
				 Debug.WriteLine("Test");
			}
		}

		#endregion

		#region Ok command

		private TaskCommand _okCommand;

		/// <summary>
		/// Gets the Ok command.
		/// </summary>
		public TaskCommand OkCommand => _okCommand ?? (_okCommand = new TaskCommand(OkAsync));

		/// <summary>
		/// Method to invoke when the Ok command is executed.
		/// </summary>
		private async Task OkAsync()
		{
			//if (MapModified)
			//{
			//	await this.SaveAndCloseViewModelAsync();
			//}
			//else
			//{
				await CloseViewModelAsync(true);
			//}
		}

		#endregion

		#region Cancel command

		private TaskCommand _cancelCommand;

		/// <summary>
		/// Gets the Cancel command.
		/// </summary>
		public TaskCommand CancelCommand => _cancelCommand ?? (_cancelCommand = new TaskCommand(CancelMapAsync));

		/// <summary>
		/// Method to invoke when the Cancel command is executed.
		/// </summary>
		private async Task CancelMapAsync()
		{
			//_elementMapService.CancelEdit();
			await this.CancelAndCloseViewModelAsync();
		}

		#endregion

		public void DragOver(IDropInfo dropInfo)
		{
			throw new NotImplementedException();
		}

		public void Drop(IDropInfo dropInfo)
		{
			throw new NotImplementedException();
		}
	}
}
