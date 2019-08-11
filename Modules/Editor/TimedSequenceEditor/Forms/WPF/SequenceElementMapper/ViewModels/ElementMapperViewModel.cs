using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using Catel.Configuration;
using Catel.Data;
using Catel.IoC;
using Catel.Services;
using Common.WPFCommon.Services;
using GongSolutions.Wpf.DragDrop;
using NLog;
using Vixen.Sys;
using VixenModules.Editor.TimedSequenceEditor.Forms.WPF.SequenceElementMapper.Models;
using VixenModules.Editor.TimedSequenceEditor.Forms.WPF.SequenceElementMapper.Services;

namespace VixenModules.Editor.TimedSequenceEditor.Forms.WPF.SequenceElementMapper.ViewModels
{
	using Catel.MVVM;
	using System.Threading.Tasks;

	public class ElementMapperViewModel : ViewModelBase, IDropTarget
	{
		private static Logger Logging = LogManager.GetCurrentClassLogger();
		private const string FormTitle = @"Element Mapper";
		private readonly Dictionary<Guid, string> _sourceElementNames;
		private string _lastModelPath = String.Empty;

		public ElementMapperViewModel(Dictionary<Guid, string> elementNamesToMap, string sequenceName)
		{
			_sourceElementNames = elementNamesToMap;
			Elements = VixenSystem.Nodes.GetRootNodes().ToList();
			ElementMap = new ElementMap(elementNamesToMap){Name = sequenceName};
			MapModified = true;
		}

		#region Private fields

		private bool _mapModified;
		private bool MapModified
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
			((IEditableObject)ElementMap).EndEdit();
			var dependencyResolver = this.GetDependencyResolver();

			if (_lastModelPath == String.Empty || !File.Exists(_lastModelPath))
			{
				var saveFileService = dependencyResolver.Resolve<ISaveFileService>();
				saveFileService.Filter = "Element Map|*.map";
				saveFileService.Title = @"Save Element Map";
				if (await saveFileService.DetermineFileAsync())
				{
					_lastModelPath = saveFileService.FileName;
					ElementMap.Name = Path.GetFileNameWithoutExtension(_lastModelPath);
				}
				else
				{
					return true;
				}
			}
			
			var pleaseWaitService = dependencyResolver.Resolve<IPleaseWaitService>();
			var modelPersistenceService = dependencyResolver.Resolve<IModelPersistenceService<ElementMap>>();
			pleaseWaitService.Show();
			if(await modelPersistenceService.SaveModelAsync(ElementMap, _lastModelPath))
			{
				MapModified = false;
				((IEditableObject)ElementMap).BeginEdit();
				pleaseWaitService.Hide();
				return true;
			}
			
			pleaseWaitService.Hide();
			var mbs = dependencyResolver.Resolve<IMessageBoxService>();
			mbs.ShowError($"An error occured loading the map.", "Error Loading Map");
			
			return false;
		}

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
			((IEditableObject)ElementMap).CancelEdit();
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
				ElementMap = new ElementMap(_sourceElementNames){Name = result.Response};
				MapModified = true;
				_lastModelPath = string.Empty;
				((IEditableObject) ElementMap).BeginEdit();
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
			((IEditableObject)ElementMap).EndEdit();
			var dependencyResolver = this.GetDependencyResolver();
			var openFileService = dependencyResolver.Resolve<IOpenFileService>();

			openFileService.IsMultiSelect = false;
			//_openFileService.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			openFileService.CheckFileExists = true;
			openFileService.Title = @"Import Zara Playlist";
			openFileService.Filter = "Element Map (*.map) | *.map";
			if (await openFileService.DetermineFileAsync())
			{

				var pleaseWaitService = dependencyResolver.Resolve<IPleaseWaitService>();
				var modelPersistenceService = dependencyResolver.Resolve<IModelPersistenceService<ElementMap>>();

				pleaseWaitService.Show();
				var map = await modelPersistenceService.LoadModelAsync(openFileService.FileName);

				if (map != null)
				{
					_lastModelPath = openFileService.FileName;
					ElementMap = map;
					ElementMap.Name = Path.GetFileNameWithoutExtension(_lastModelPath);
					MapModified = false;
					((IEditableObject)ElementMap).BeginEdit();
					var missingSourceNames = DiscoverMissingSourceNames(map);
					if (missingSourceNames.Any())
					{
						var mbs = dependencyResolver.Resolve<IMessageBoxService>();
						var result = mbs.GetUserConfirmation(@"Add missing source elements to map?", "Add Missing Sources.");
						if (result.Result == MessageResult.OK)
						{
							var mapsToAdd = missingSourceNames.Select(x => new ElementMapping(x.Key, x.Value));
							ElementMap.AddRange(mapsToAdd);
							MapModified = true;
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
			get { return _saveMapAsCommand ?? (_saveMapAsCommand = new TaskCommand(SaveAsync, CanSaveAs)); }
		}

		/// <summary>
		/// Method to invoke when the SaveAs command is executed.
		/// </summary>
		private async Task SaveMapAsAsync()
		{
			_lastModelPath = String.Empty;
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
						MapModified = true;
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
			return _sourceElementNames.Except(map.GetSourceNameIds());
		}

		private void UpdateTitle()
		{
			Title = $"{FormTitle} - {ElementMap.Name} {(MapModified ? "*" : string.Empty)}";
		}

		protected void UpdateCommandStates()
		{
			var viewModelBase = this as ViewModelBase;
			var commandManager = viewModelBase.GetViewModelCommandManager();
			commandManager.InvalidateCommands();
		}
	}
}
