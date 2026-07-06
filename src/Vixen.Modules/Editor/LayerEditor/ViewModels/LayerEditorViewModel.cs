using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using Catel.Data;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Common.WPFCommon.Services;
using Vixen.Module.MixingFilter;
using Vixen.Services;
using Vixen.Sys.LayerMixing;
using VixenModules.Editor.LayerEditor.ImportExport;
using VixenModules.Editor.LayerEditor.Services;

namespace VixenModules.Editor.LayerEditor.ViewModels
{
	/// <summary>
	/// Manages layer editor state and commands for a sequence's layer collection.
	/// </summary>
	public class LayerEditorViewModel : ViewModelBase, INotifyCollectionChanged
	{
		private const string V3LFileFilter = "Vixen 3 Layers (*.v3l)|*.v3l|All Files (*.*)|*.*";

		private readonly SequenceLayers _sequenceLayers;
		private readonly ILayerEditorLayerService _layerService;
		private readonly ILayerImportExportService _importExportService;
		private readonly List<ILayerMixingFilterInstance> _filterTypes;
		private Command _addLayerCommand;
		private Command<ILayer> _removeLayerCommand;
		private Command<ILayerMixingFilterInstance> _configureLayerCommand;
		private Command<ILayer> _quickRenameLayerCommand;
		private TaskCommand _exportLayersCommand;
		private TaskCommand _importLayersCommand;

		/// <summary>
		/// Initializes a new instance of the <see cref="LayerEditorViewModel" /> class.
		/// </summary>
		/// <param name="sequenceLayers">The sequence layer collection to edit.</param>
		/// <exception cref="ArgumentNullException"><paramref name="sequenceLayers" /> is <see langword="null" />.</exception>
		public LayerEditorViewModel(SequenceLayers sequenceLayers)
			: this(sequenceLayers, new LayerEditorLayerService())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LayerEditorViewModel" /> class.
		/// </summary>
		/// <param name="sequenceLayers">The sequence layer collection to edit.</param>
		/// <param name="layerService">The service used to mutate layers.</param>
		/// <exception cref="ArgumentNullException"><paramref name="sequenceLayers" /> or <paramref name="layerService" /> is <see langword="null" />.</exception>
		public LayerEditorViewModel(SequenceLayers sequenceLayers, ILayerEditorLayerService layerService)
			: this(sequenceLayers, layerService, new LayerImportExportService(layerService, new LayerMixingFilterResolver()))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LayerEditorViewModel" /> class.
		/// </summary>
		/// <param name="sequenceLayers">The sequence layer collection to edit.</param>
		/// <param name="layerService">The service used to mutate layers.</param>
		/// <param name="importExportService">The service used to export and import `.v3l` files.</param>
		/// <exception cref="ArgumentNullException"><paramref name="sequenceLayers" />, <paramref name="layerService" />, or <paramref name="importExportService" /> is <see langword="null" />.</exception>
		public LayerEditorViewModel(SequenceLayers sequenceLayers, ILayerEditorLayerService layerService, ILayerImportExportService importExportService)
		{
			_sequenceLayers = sequenceLayers ?? throw new ArgumentNullException(nameof(sequenceLayers));
			_layerService = layerService ?? throw new ArgumentNullException(nameof(layerService));
			_importExportService = importExportService ?? throw new ArgumentNullException(nameof(importExportService));
			_filterTypes = LoadFilterTypes();
			Layers = _sequenceLayers.Layers;
			Layers.CollectionChanged += LayersCollectionChanged;
		}

		/// <summary>
		/// Gets the editable layer collection.
		/// </summary>
		public ObservableCollection<ILayer> Layers
		{
			get { return GetValue<ObservableCollection<ILayer>>(LayersProperty); }
			private set { SetValue(LayersProperty, value); }
		}

		/// <summary>
		/// Defines the <see cref="Layers" /> property.
		/// </summary>
		public static readonly IPropertyData LayersProperty = RegisterProperty<ObservableCollection<ILayer>>(nameof(Layers));

		/// <summary>
		/// Gets the available standard layer mixing filter types.
		/// </summary>
		public IReadOnlyList<ILayerMixingFilterInstance> FilterTypes => _filterTypes;

		/// <summary>
		/// Gets the AddLayer command.
		/// </summary>
		public Command AddLayerCommand => _addLayerCommand ??= new Command(AddLayer);

		/// <summary>
		/// Gets the RemoveLayer command.
		/// </summary>
		public Command<ILayer> RemoveLayerCommand => _removeLayerCommand ??= new Command<ILayer>(RemoveLayer, CanRemoveLayer);

		/// <summary>
		/// Gets the ConfigureLayer command.
		/// </summary>
		public Command<ILayerMixingFilterInstance> ConfigureLayerCommand => _configureLayerCommand ??= new Command<ILayerMixingFilterInstance>(ConfigureLayer, CanConfigureLayer);

		/// <summary>
		/// Gets the QuickRenameLayer command.
		/// </summary>
		public Command<ILayer> QuickRenameLayerCommand => _quickRenameLayerCommand ??= new Command<ILayer>(QuickRenameLayer, CanQuickRenameLayer);

		/// <summary>
		/// Gets the ExportLayers command.
		/// </summary>
		public TaskCommand ExportLayersCommand => _exportLayersCommand ??= new TaskCommand(ExportLayersAsync, CanExportLayers);

		/// <summary>
		/// Gets the ImportLayers command.
		/// </summary>
		public TaskCommand ImportLayersCommand => _importLayersCommand ??= new TaskCommand(ImportLayersAsync);

		/// <summary>
		/// Occurs when the layer collection changes.
		/// </summary>
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>
		/// Occurs when a layer's filter setup changes.
		/// </summary>
		public event EventHandler<EventArgs> LayerChanged;

		/// <summary>
		/// Gets a value that indicates whether the specified layer is the default layer.
		/// </summary>
		/// <param name="layer">The layer to inspect.</param>
		/// <returns><see langword="true" /> if <paramref name="layer" /> is the default layer; otherwise, <see langword="false" />.</returns>
		public bool IsDefaultLayer(ILayer layer)
		{
			return layer != null && _sequenceLayers.IsDefaultLayer(layer);
		}

		/// <summary>
		/// Moves a standard layer to a new visible index.
		/// </summary>
		/// <param name="layer">The layer to move.</param>
		/// <param name="newIndex">The destination index.</param>
		public void MoveLayer(ILayer layer, int newIndex)
		{
			_layerService.MoveLayer(_sequenceLayers, layer, newIndex);
		}

		/// <inheritdoc />
		protected override Task CloseAsync()
		{
			Layers.CollectionChanged -= LayersCollectionChanged;
			return base.CloseAsync();
		}

		private static List<ILayerMixingFilterInstance> LoadFilterTypes()
		{
			var descriptors = ApplicationServices.GetModuleDescriptors<ILayerMixingFilterInstance>();
			if (descriptors == null)
			{
				return new List<ILayerMixingFilterInstance>();
			}

			return descriptors.Select(filterType => ApplicationServices.Get<ILayerMixingFilterInstance>(filterType.TypeId)).ToList();
		}

		private void AddLayer()
		{
			var firstFilter = _filterTypes.FirstOrDefault();
			if (firstFilter == null)
			{
				return;
			}

			_layerService.AddLayer(_sequenceLayers, firstFilter);
		}

		private void RemoveLayer(ILayer layer)
		{
			if (!CanRemoveLayer(layer))
			{
				return;
			}

			_layerService.RemoveLayer(_sequenceLayers, layer);
		}

		private bool CanRemoveLayer(ILayer layer)
		{
			return layer != null && !IsDefaultLayer(layer);
		}

		private void ConfigureLayer(ILayerMixingFilterInstance layerMixingFilterInstance)
		{
			if (!CanConfigureLayer(layerMixingFilterInstance))
			{
				return;
			}

			if (_layerService.ConfigureLayer(layerMixingFilterInstance))
			{
				OnLayerChanged(EventArgs.Empty);
			}
		}

		private static bool CanConfigureLayer(ILayerMixingFilterInstance layerMixingFilterInstance)
		{
			return layerMixingFilterInstance != null && layerMixingFilterInstance.HasSetup;
		}

		private void QuickRenameLayer(ILayer layer)
		{
			if (!CanQuickRenameLayer(layer))
			{
				return;
			}

			_layerService.QuickRenameLayer(_sequenceLayers, layer);
		}

		private bool CanQuickRenameLayer(ILayer layer)
		{
			return layer != null && !IsDefaultLayer(layer) && layer.LayerMixingFilter != null;
		}

		private bool CanExportLayers()
		{
			return _layerService.HasExportableLayers(_sequenceLayers);
		}

		private async Task ExportLayersAsync()
		{
			var dependencyResolver = this.GetDependencyResolver();
			var saveFileService = dependencyResolver.Resolve<ISaveFileService>();

			var determineFileContext = new DetermineSaveFileContext
			{
				Filter = V3LFileFilter,
				Title = "Export Layers",
				CheckPathExists = true
			};

			var result = await saveFileService.DetermineFileAsync(determineFileContext);
			if (!result.Result)
			{
				return;
			}
			
			if (File.Exists(result.FileName))
			{
				// IMessageBoxService.GetUserConfirmation shows a Yes/No dialog whose "confirm" button
				// reports MessageResult.OK (see MessageBoxForm.Designer.cs); it never reports Cancel.
				var messageService = dependencyResolver.Resolve<IMessageBoxService>();
				var confirmResult = messageService.GetUserConfirmation(
					"A file with this name already exists. Do you want to replace it?",
					"Confirm Save As");

				if (confirmResult.Result != MessageResult.OK)
				{
					return;
				}
			}
			
			try
			{
				await _importExportService.ExportAsync(_sequenceLayers, result.FileName);
			}
			catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
			{
				var messageBoxService = dependencyResolver.Resolve<IMessageBoxService>();
				messageBoxService.ShowError($"The layers could not be exported: {ex.Message}", "Export Layers");
			}
		}

		private async Task ImportLayersAsync()
		{
			var dependencyResolver = this.GetDependencyResolver();
			var openFileService = dependencyResolver.Resolve<IOpenFileService>();

			var determineFileContext = new DetermineOpenFileContext
			{
				IsMultiSelect = false,
				CheckFileExists = true,
				Filter = V3LFileFilter,
				Title = "Import Layers"
			};

			var result = await openFileService.DetermineFileAsync(determineFileContext);
			if (!result.Result)
			{
				return;
			}

			var messageBoxService = dependencyResolver.Resolve<IMessageBoxService>();

			LayerImportPlan plan;
			try
			{
				plan = await _importExportService.ReadImportPlanAsync(result.FileName);
			}
			catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
			{
				messageBoxService.ShowError($"The file could not be read: {ex.Message}", "Import Layers");
				return;
			}

			if (!plan.IsValid)
			{
				messageBoxService.ShowError(plan.FailureReason, "Import Layers");
				return;
			}

			if (plan.ImportableLayers.Count == 0)
			{
				messageBoxService.ShowError("The file does not contain any layers that can be imported.", "Import Layers");
				return;
			}

			if (plan.HasSkippedLayers)
			{
				var skippedSummary = string.Join(Environment.NewLine,
					plan.SkippedLayers.Select(skippedLayer => $"{skippedLayer.Name}: {skippedLayer.Reason}"));
				var confirmation = messageBoxService.GetUserConfirmation(
					$"The following layers cannot be imported:{Environment.NewLine}{Environment.NewLine}{skippedSummary}{Environment.NewLine}{Environment.NewLine}Continue importing the remaining layers?",
					"Import Layers");

				if (confirmation.Result != MessageResult.OK)
				{
					return;
				}
			}

			_importExportService.Import(_sequenceLayers, plan);
		}

		private void LayersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			RemoveLayerCommand.RaiseCanExecuteChanged();
			QuickRenameLayerCommand.RaiseCanExecuteChanged();
			ExportLayersCommand.RaiseCanExecuteChanged();
			OnCollectionChanged(e);
		}

		private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			CollectionChanged?.Invoke(this, args);
		}

		private void OnLayerChanged(EventArgs e)
		{
			LayerChanged?.Invoke(this, e);
		}
	}
}
