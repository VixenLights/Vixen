using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Catel;
using Catel.Collections;
using Catel.Data;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Common.WPFCommon.Services;
using GongSolutions.Wpf.DragDrop;
using Vixen.Sys;
using VixenModules.App.TimedSequenceMapper.SequenceElementMapper.Models;
using VixenModules.App.TimedSequenceMapper.SequenceElementMapper.Services;

namespace VixenModules.App.TimedSequenceMapper.SequenceElementMapper.ViewModels
{
	public class ElementMapperViewModel : ViewModelBase, IDropTarget
	{
		private const string FormTitle = @"Element Mapper";
		private readonly Dictionary<Guid, string> _sourceActiveElements;
		private IElementMapService _elementMapService;
		private readonly string _sequenceName;
		
		public ElementMapperViewModel(Dictionary<Guid, string> sourceActiveElements, string sequenceName)
		{
			SourceTreeView = Visibility.Collapsed;
			BasicView = Visibility.Collapsed;
			_sourceActiveElements = sourceActiveElements;
			_sequenceName = sequenceName;
			Title = FormTitle;
		}

		public ElementMapperViewModel(Dictionary<Guid, string> sourceActiveElements, string sequenceName, string elementTreeFileName, string elementMapFileName):this(sourceActiveElements, sequenceName)
		{
			ElementTreeFilePath = elementTreeFileName;
			ElementMapFilePath = elementMapFileName;
			Title = FormTitle;
		}

		private void OnElementMapChanged(ElementMapService.MapMessage obj)
		{
			ElementMap = _elementMapService.ElementMap;
			PopulateSourceTree(_elementMapService.ElementMap.SourceTree);
		}

		#region Private fields

		public string ElementMapFilePath { get; protected set; }

		public string ElementTreeFilePath { get; protected set; }

		private bool _mapModified;
		internal bool MapModified
		{
			get => _mapModified;
			set
			{
				_mapModified = value;
				UpdateTitle();
				UpdateCommandStates();
			}
		}

		#endregion

		#region Title property

		/// <summary>
		/// Gets or sets the Title value.
		/// </summary>
		public new string Title
		{
			get { return GetValue<string>(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		/// <summary>
		/// Title property data.
		/// </summary>
		public static readonly PropertyData TitleProperty = RegisterProperty("Title", typeof(string));

		#endregion
		// TODO: Register models with the vmpropmodel codesnippet
		// TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
		// TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

		protected override async Task InitializeAsync()
		{
			await base.InitializeAsync();

			// TODO: subscribe to events here
			var dependencyResolver = this.GetDependencyResolver();
			_elementMapService = dependencyResolver.Resolve<IElementMapService>();
			_elementMapService.SourceActiveElements = _sourceActiveElements;
			Elements = VixenSystem.Nodes.GetRootNodes().ToList();
			if (string.IsNullOrEmpty(ElementMapFilePath))
			{
				ElementMap = _elementMapService.InitializeMap(_sourceActiveElements, _sequenceName);
				MapModified = true;
			}
			else
			{
				var success = await _elementMapService.LoadMapAsync(ElementMapFilePath);
				
				if (success)
				{
					_elementMapService.ElementMap.Name = Path.GetFileNameWithoutExtension(ElementMapFilePath);
					ElementMap = _elementMapService.ElementMap;
					MapModified = false;
					if (_elementMapService.ElementMap.SourceTree != null)
					{
						PopulateSourceTree(_elementMapService.ElementMap.SourceTree);
					}
				}
			}

			_elementMapService.RegisterMapMessages(this, OnElementMapChanged);
			
			if (!string.IsNullOrEmpty(ElementTreeFilePath))
			{
				await LoadElementTree(ElementTreeFilePath);
				MapModified = true;
			}

			if (SourceElementTree != null)
			{
				SourceTreeView = Visibility.Visible;
			}
			else
			{
				BasicView = Visibility.Visible;
			}
		}

		protected override async Task CloseAsync()
		{
			// TODO: unsubscribe from events here
			await base.CloseAsync();
		}

		#region Overrides of ViewModelBase

		/// <inheritdoc />
		protected override async Task<bool> SaveAsync()
		{
			var dependencyResolver = this.GetDependencyResolver();

			if (ElementMapFilePath == String.Empty || !File.Exists(ElementMapFilePath))
			{
				var saveFileService = dependencyResolver.Resolve<ISaveFileService>();
				saveFileService.Filter = $"Element Map|*.{Constants.MapExtension}";
				saveFileService.Title = @"Save Element Map";
				if (await saveFileService.DetermineFileAsync())
				{
					ElementMapFilePath = saveFileService.FileName;
					_elementMapService.ElementMap.Name = Path.GetFileNameWithoutExtension(ElementMapFilePath);
				}
				else
				{
					return true;
				}
			}
			
			var pleaseWaitService = dependencyResolver.Resolve<IPleaseWaitService>();
			pleaseWaitService.Show();
			if(await _elementMapService.SaveMapAsync(ElementMapFilePath))
			{
				MapModified = false;
				pleaseWaitService.Hide();
				return true;
			}
			
			pleaseWaitService.Hide();
			var mbs = dependencyResolver.Resolve<IMessageBoxService>();
			mbs.ShowError($"An error occured loading the map.", "Error Loading Map");
			
			return false;
		}

		#endregion

		#region SourceTreeView property

		/// <summary>
		/// Gets or sets the SourceTreeView value.
		/// </summary>
		public Visibility SourceTreeView
		{
			get { return GetValue<Visibility>(SourceTreeViewProperty); }
			set { SetValue(SourceTreeViewProperty, value); } 
		}

		/// <summary>
		/// SourceTreeView property data.
		/// </summary>
		public static readonly PropertyData SourceTreeViewProperty = RegisterProperty("SourceTreeView", typeof(Visibility));

		#endregion

		#region BasicView property

		/// <summary>
		/// Gets or sets the BasicView value.
		/// </summary>
		public Visibility BasicView
		{
			get { return GetValue<Visibility>(BasicViewProperty); }
			set { SetValue(BasicViewProperty, value); }
		}

		/// <summary>
		/// BasicView property data.
		/// </summary>
		public static readonly PropertyData BasicViewProperty = RegisterProperty("BasicView", typeof(Visibility));

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

		#region SourceElementTree property

		/// <summary>
		/// Gets or sets the SourceElementTree value.
		/// </summary>
		public FastObservableCollection<ElementNodeProxy> SourceElementTree
		{
			get { return GetValue<FastObservableCollection<ElementNodeProxy>>(SourceElementTreeProperty); }
			set { SetValue(SourceElementTreeProperty, value); }
		}

		/// <summary>
		/// SourceElementTree property data.
		/// </summary>
		public static readonly PropertyData SourceElementTreeProperty = RegisterProperty("SourceElementTree", typeof(FastObservableCollection<ElementNodeProxy>));

		#endregion

		#region SelectedMapping property

		/// <summary>
		/// Gets or sets the SelectedMapping value.
		/// </summary>
		public ElementMapping SelectedMapping
		{
			get { return GetValue<ElementMapping>(SelectedMappingProperty); }
			set
			{
				SetValue(SelectedMappingProperty, value);
				UpdateCommandStates();
			}
		}

		/// <summary>
		/// SelectedMapping property data.
		/// </summary>
		public static readonly PropertyData SelectedMappingProperty = RegisterProperty("SelectedMapping", typeof(ElementMapping));

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
			if (MapModified)
			{
				await this.SaveAndCloseViewModelAsync();
			}
			else
			{
				await this.CloseViewModelAsync(true);
			}
		}

		#endregion


		#region Cancel command

		private TaskCommand _cancelCommand;

		/// <summary>
		/// Gets the Cancel command.
		/// </summary>
		public TaskCommand CancelCommand
		{
			get { return _cancelCommand ?? (_cancelCommand = new TaskCommand(CancelMapAsync)); }
		}

		/// <summary>
		/// Method to invoke when the Cancel command is executed.
		/// </summary>
		private async Task CancelMapAsync()
		{
			_elementMapService.CancelEdit();
			await this.CancelAndCloseViewModelAsync();
		}

		#endregion


		#region NewMap command

		private Command _newMapCommand;

		/// <summary>
		/// Gets the CreateMap command.
		/// </summary>
		public Command NewMapCommand
		{
			get { return _newMapCommand ?? (_newMapCommand = new Command(NewMap)); }
		}

		/// <summary>
		/// Method to invoke when the CreateMap command is executed.
		/// </summary>
		private void NewMap()
		{
			MessageBoxService mbs = new MessageBoxService();
			var result = mbs.GetUserInput("Enter new map name.", "New Map - Name", "New Map");
			if (result.Result == MessageResult.OK)
			{
				_elementMapService.InitializeMap(_sourceActiveElements, result.Response);
				MapModified = true;
				ElementMapFilePath = string.Empty;
			}
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
			_elementMapService.EndEdit();
			var dependencyResolver = this.GetDependencyResolver();
			var openFileService = dependencyResolver.Resolve<IOpenFileService>();

			openFileService.IsMultiSelect = false;
			//_openFileService.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			openFileService.CheckFileExists = true;
			openFileService.Title = @"Open Element Mapping";
			openFileService.Filter = $"Element Map (*.{Constants.MapExtension}) | *.{Constants.MapExtension}";
			if (await openFileService.DetermineFileAsync())
			{

				var pleaseWaitService = dependencyResolver.Resolve<IPleaseWaitService>();
				//var modelPersistenceService = dependencyResolver.Resolve<IModelPersistenceService<ElementMap>>();

				pleaseWaitService.Show();
				var success = await _elementMapService.LoadMapAsync(openFileService.FileName);

				if (success)
				{
					ElementMapFilePath = openFileService.FileName;
					_elementMapService.ElementMap.Name = Path.GetFileNameWithoutExtension(ElementMapFilePath);
					MapModified = false;

					if (_elementMapService.ElementMap.SourceTree == null)
					{
						var missingSourceNames = DiscoverMissingSourceNames(_elementMapService.ElementMap);
						if (missingSourceNames.Any())
						{
							var mbs = dependencyResolver.Resolve<IMessageBoxService>();
							var result = mbs.GetUserConfirmation(@"Add missing source elements to map?", "Add Missing Sources.");
							if (result.Result == MessageResult.OK)
							{
								var mapsToAdd = missingSourceNames.Select(x => new ElementMapping(x.Key, x.Value));
								_elementMapService.ElementMap.AddRange(mapsToAdd);
								MapModified = true;
							}
						}
					}
				}

				pleaseWaitService.Hide();

			}
		}

		#endregion

		#region SaveMap command

		private TaskCommand _saveMapCommand;

		/// <summary>
		/// Gets the Save command.
		/// </summary>
		public TaskCommand SaveMapCommand
		{
			get { return _saveMapCommand ?? (_saveMapCommand = new TaskCommand(SaveAsync, CanSaveMap)); }
		}

		/// <summary>
		/// Method to check whether the Save command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool CanSaveMap()
		{
			return MapModified;
		}

		#endregion

		#region SaveMapAs command

		private TaskCommand _saveMapAsCommand;

		/// <summary>
		/// Gets the SaveAs command.
		/// </summary>
		public TaskCommand SaveMapAsCommand
		{
			get { return _saveMapAsCommand ?? (_saveMapAsCommand = new TaskCommand(SaveMapAsAsync, CanSaveAs)); }
		}

		/// <summary>
		/// Method to invoke when the SaveAs command is executed.
		/// </summary>
		private async Task SaveMapAsAsync()
		{
			ElementMapFilePath = String.Empty;
			await SaveAsync();
		}

		/// <summary>
		/// Method to check whether the SaveAs command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool CanSaveAs()
		{
			return true;
		}

		#endregion

		#region OpenIncomingElementTree command

		private TaskCommand _openIncomingElementTreeCommand;

		/// <summary>
		/// Gets the OpenIncomingElementTree command.
		/// </summary>
		public TaskCommand OpenIncomingElementTreeCommand
		{
			get { return _openIncomingElementTreeCommand ?? (_openIncomingElementTreeCommand = new TaskCommand(OpenIncomingElementTreeAsync)); }
		}

		/// <summary>
		/// Method to invoke when the OpenIncomingElementTree command is executed.
		/// </summary>
		private async Task OpenIncomingElementTreeAsync()
		{
			var dependencyResolver = this.GetDependencyResolver();
			var openFileService = dependencyResolver.Resolve<IOpenFileService>();

			openFileService.IsMultiSelect = false;
			//_openFileService.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			openFileService.CheckFileExists = true;
			openFileService.Title = @"Load Element Tree";
			openFileService.Filter = $"Element Tree (*.{Constants.ElementTreeExtension}) | *.{Constants.ElementTreeExtension}";
			if (await openFileService.DetermineFileAsync())
			{
				await LoadElementTree(openFileService.FileName);
			}
		}
		public async Task LoadElementTree(string fileName)
		{
			Argument.IsNotNullOrEmpty(nameof(fileName), fileName);
			if (!File.Exists(fileName))
			{
				throw new FileNotFoundException($"Element Tree file does not exist. {fileName}");
			}
			var dependencyResolver = this.GetDependencyResolver();
			var modelPersistenceService = dependencyResolver.Resolve<IModelPersistenceService<ElementMap>>();
			var pleaseWaitService = dependencyResolver.Resolve<IPleaseWaitService>();

			pleaseWaitService.Show();

			var incomingElementNodes = await modelPersistenceService.LoadElementNodeProxyAsync(fileName);
			if (incomingElementNodes != null && incomingElementNodes.Children.Any())
			{
				PopulateSourceTree(incomingElementNodes);
			}

			pleaseWaitService.Hide();
		}

		#endregion


		#region Help command

		private Command _helpCommand;

		/// <summary>
		/// Gets the Help command.
		/// </summary>
		public Command HelpCommand
		{
			get { return _helpCommand ?? (_helpCommand = new Command(Help)); }
		}

		/// <summary>
		/// Method to invoke when the Help command is executed.
		/// </summary>
		private void Help()
		{
			var url = "http://www.vixenlights.com/vixen-3-documentation/sequencer/sequence-import/";
			System.Diagnostics.Process.Start(url);
		}

		#endregion

		#region ClearMapping command

		private Command _clearMappingCommand;

		/// <summary>
		/// Gets the DeleteMapping command.
		/// </summary>
		public Command ClearMappingCommand
		{
			get { return _clearMappingCommand ?? (_clearMappingCommand = new Command(ClearMapping, CanClearMapping)); }
		}

		/// <summary>
		/// Method to invoke when the DeleteMapping command is executed.
		/// </summary>
		private void ClearMapping()
		{
			SelectedMapping.ClearTarget();
		}

		/// <summary>
		/// Method to check whether the DeleteMapping command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool CanClearMapping()
		{
			return SelectedMapping != null;
		}

		#endregion


		#region Implementation of IDropTarget

		/// <inheritdoc />
		public void DragOver(IDropInfo dropInfo)
		{
			var data = ExtractData(dropInfo.Data).OfType<object>().ToList();
			var canDrop = false;

			if (data.First() is ElementNodeProxyViewModel enp)
			{
				if (!_elementMapService.ElementMap.Contains(enp.ElementNodeProxy.Id))
				{
					canDrop = true;
				}
			}else if (data.First() is IElementNode)
			{
				canDrop = true;
			}

			if(canDrop)
			{
				dropInfo.Effects = DragDropEffects.Copy;
				dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
			}
			else
			{
				dropInfo.Effects = DragDropEffects.None;
			}
		}

		/// <inheritdoc />
		public void Drop(IDropInfo dropInfo)
		{
			if (dropInfo?.DragInfo == null || dropInfo.TargetItem == null)
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
						MapModified = true;
					}
				}
				else if(data.First() is ElementNodeProxyViewModel enp)
				{
					if (!ElementMap.Contains(enp.ElementNodeProxy.Id))
					{
						ElementMapping em = new ElementMapping(enp.ElementNodeProxy.Id, enp.Name);
						ElementMap.Add(em);
						enp.ElementMapping = em;
						SelectedMapping = em;
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

		private IEnumerable<KeyValuePair<Guid, string>> DiscoverMissingSourceNames(ElementMap map)
		{
			return _sourceActiveElements.Except(map.GetSourceNameIds());
		}

		private void UpdateTitle()
		{
			Title = $"{FormTitle} - {_elementMapService.ElementMap.Name} {(MapModified ? "*" : string.Empty)}";
		}

		protected void UpdateCommandStates()
		{
			var viewModelBase = this as ViewModelBase;
			var commandManager = viewModelBase.GetViewModelCommandManager();
			commandManager.InvalidateCommands();
		}

		private void PopulateSourceTree(ElementNodeProxy sourceTree)
		{
			_elementMapService.ElementMap.SourceTree = sourceTree;
			if (sourceTree != null)
			{
				SourceElementTree = new FastObservableCollection<ElementNodeProxy>(sourceTree.Children);
			}
		}
	}
}
