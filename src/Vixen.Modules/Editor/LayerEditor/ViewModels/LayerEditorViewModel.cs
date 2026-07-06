using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Catel.Data;
using Catel.MVVM;
using Vixen.Module.MixingFilter;
using Vixen.Services;
using Vixen.Sys.LayerMixing;
using VixenModules.Editor.LayerEditor.Services;

namespace VixenModules.Editor.LayerEditor.ViewModels
{
	/// <summary>
	/// Manages layer editor state and commands for a sequence's layer collection.
	/// </summary>
	public class LayerEditorViewModel : ViewModelBase, INotifyCollectionChanged
	{
		private readonly SequenceLayers _sequenceLayers;
		private readonly ILayerEditorLayerService _layerService;
		private readonly List<ILayerMixingFilterInstance> _filterTypes;
		private Command _addLayerCommand;
		private Command<ILayer> _removeLayerCommand;
		private Command<ILayerMixingFilterInstance> _configureLayerCommand;

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
		{
			_sequenceLayers = sequenceLayers ?? throw new ArgumentNullException(nameof(sequenceLayers));
			_layerService = layerService ?? throw new ArgumentNullException(nameof(layerService));
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

		private void LayersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			RemoveLayerCommand.RaiseCanExecuteChanged();
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
