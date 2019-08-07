using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Catel.Data;
using Catel.Services;
using Common.WPFCommon.Services;
using GongSolutions.Wpf.DragDrop;
using Vixen.Sys;
using VixenModules.Editor.TimedSequenceEditor.Forms.WPF.SequenceElementMapper.Models;

namespace VixenModules.Editor.TimedSequenceEditor.Forms.WPF.SequenceElementMapper.ViewModels
{
	using Catel.MVVM;
	using System.Threading.Tasks;

	public class ElementMapperViewModel : ViewModelBase, IDropTarget
	{
		public ElementMapperViewModel(List<string> elementNamesToMap)
		{
			Elements = VixenSystem.Nodes.GetRootNodes().ToList();
			LoadMaps();
			ElementMap = new ElementMap(elementNamesToMap);
		}

		public override string Title { get { return "Element Mapper"; } }

		// TODO: Register models with the vmpropmodel codesnippet
		// TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
		// TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

		protected override async Task InitializeAsync()
		{
			await base.InitializeAsync();

			// TODO: subscribe to events here
		}

		protected override async Task CloseAsync()
		{
			// TODO: unsubscribe from events here

			await base.CloseAsync();
		}

		#region Maps property

		/// <summary>
		/// Gets or sets the Maps value.
		/// </summary>
		public List<ElementMap> Maps
		{
			get { return GetValue<List<ElementMap>>(MapsProperty); }
			set { SetValue(MapsProperty, value); }
		}

		/// <summary>
		/// Maps property data.
		/// </summary>
		public static readonly PropertyData MapsProperty = RegisterProperty("Maps", typeof(List<ElementMap>));

		#endregion

		#region SelectedMap property

		/// <summary>
		/// Gets or sets the SelectedMap value.
		/// </summary>
		public ElementMap SelectedMap
		{
			get { return GetValue<ElementMap>(SelectedMapProperty); }
			set { SetValue(SelectedMapProperty, value); }
		}

		/// <summary>
		/// SelectedMap property data.
		/// </summary>
		public static readonly PropertyData SelectedMapProperty = RegisterProperty("SelectedMap", typeof(ElementMap));

		#endregion

		#region Elements property

		/// <summary>
		/// Gets or sets the Elements value.
		/// </summary>
		public List<ElementNode> Elements
		{
			get { return GetValue<List<ElementNode>>(ElementsProperty); }
			set { SetValue(ElementsProperty, value); }
		}

		/// <summary>
		/// Elements property data.
		/// </summary>
		public static readonly PropertyData ElementsProperty = RegisterProperty("Elements", typeof(List<ElementNode>));

		#endregion

		#region ElementMap model property

		/// <summary>
		/// Gets or sets the ElementMap value.
		/// </summary>
		[Model]
		public ElementMap ElementMap
		{
			get { return GetValue<ElementMap>(ElementMapProperty); }
			private set { SetValue(ElementMapProperty, value); }
		}

		/// <summary>
		/// ElementMap property data.
		/// </summary>
		public static readonly PropertyData ElementMapProperty = RegisterProperty("ElementMap", typeof(ElementMap));

		#endregion


		#region Ok command

		private TaskCommand _okCommand;

		/// <summary>
		/// Gets the Ok command.
		/// </summary>
		public TaskCommand OkCommand
		{
			get { return _okCommand ?? (_okCommand = new TaskCommand(OkAsync)); }
		}

		/// <summary>
		/// Method to invoke when the Ok command is executed.
		/// </summary>
		private async Task OkAsync()
		{
			await this.SaveAndCloseViewModelAsync();
		}

		#endregion


		#region Cancel command

		private TaskCommand _cancelCommand;

		/// <summary>
		/// Gets the Cancel command.
		/// </summary>
		public TaskCommand CancelCommand
		{
			get { return _cancelCommand ?? (_cancelCommand = new TaskCommand(CancelAsync)); }
		}

		/// <summary>
		/// Method to invoke when the Cancel command is executed.
		/// </summary>
		private async Task CancelAsync()
		{
			await CancelViewModelAsync();
		}

		#endregion


		#region CreateMap command

		private Command _createMapCommand;

		/// <summary>
		/// Gets the CreateMap command.
		/// </summary>
		public Command CreateMapCommand
		{
			get { return _createMapCommand ?? (_createMapCommand = new Command(CreateMap)); }
		}

		/// <summary>
		/// Method to invoke when the CreateMap command is executed.
		/// </summary>
		private void CreateMap()
		{
			MessageBoxService mbs = new MessageBoxService();
			var result = mbs.GetUserInput("Enter new map name.", "New Map - Name", "New Map");
			if (result.Result == MessageResult.OK)
			{
				var map = new ElementMap(){Name = result.Response};
				Maps.Add(map);
				SelectedMap = map;
				((IEditableObject) map).BeginEdit();
			}
		}

		#endregion

		#region RemoveMap command

		private Command _removeMapCommand;

		/// <summary>
		/// Gets the RemoveMap command.
		/// </summary>
		public Command RemoveMapCommand
		{
			get { return _removeMapCommand ?? (_removeMapCommand = new Command(RemoveMap, CanRemoveMap)); }
		}

		/// <summary>
		/// Method to invoke when the RemoveMap command is executed.
		/// </summary>
		private void RemoveMap()
		{
			Maps.Remove(SelectedMap);
		}

		/// <summary>
		/// Method to check whether the RemoveMap command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool CanRemoveMap()
		{
			return SelectedMap != null;
		}

		#endregion


		#region OpenMap command

		private TaskCommand _openMapCommand;

		/// <summary>
		/// Gets the OpenMap command.
		/// </summary>
		public TaskCommand OpenMapCommand
		{
			get { return _openMapCommand ?? (_openMapCommand = new TaskCommand(OpenMapAsync)); }
		}

		/// <summary>
		/// Method to invoke when the OpenMap command is executed.
		/// </summary>
		private async Task OpenMapAsync()
		{
			// TODO: Handle command logic here
		}

		#endregion

		private void LoadMaps()
		{
			Maps = new List<ElementMap>();
		}


		#region Implementation of IDropTarget

		/// <inheritdoc />
		public void DragOver(IDropInfo dropInfo)
		{
			dropInfo.Effects = DragDropEffects.Copy;
			dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
		}

		/// <inheritdoc />
		public void Drop(IDropInfo dropInfo)
		{
			if (dropInfo == null || dropInfo.DragInfo == null||dropInfo.TargetItem == null)
			{
				return;
			}
			
			var data = ExtractData(dropInfo.Data).OfType<object>().ToList();

			if (data.Any())
			{
				if(data.First() is IElementNode en)
				{
					if(dropInfo.TargetItem is ElementMapping mapping)
					{ 
					    mapping.TargetId = en.Id;
						mapping.TargetName = en.Name;
					}
				}
			}
		}

		public static IEnumerable ExtractData(object data)
		{
			if (data is IEnumerable && !(data is string))
			{
				return (IEnumerable)data;
			}
			else
			{
				return Enumerable.Repeat(data, 1);
			}
		}

		#endregion
	}
}
