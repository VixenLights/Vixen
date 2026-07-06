using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Common.WPFCommon.Input;
using Vixen.Sys.LayerMixing;
using VixenModules.Editor.LayerEditor.ViewModels;

namespace VixenModules.Editor.LayerEditor.Views
{
	/// <summary>
	/// Hosts the layer editor view model and adapts WPF drag/drop events to layer move operations.
	/// </summary>
	public partial class LayerEditorView : Catel.Windows.Controls.UserControl, IDragSourceAdvisor, IDropTargetAdvisor, INotifyCollectionChanged
	{
		private readonly LayerEditorViewModel _viewModel;

		/// <summary>
		/// Initializes a new instance of the <see cref="LayerEditorView" /> class.
		/// </summary>
		/// <param name="layers">The sequence layer collection to edit.</param>
		public LayerEditorView(SequenceLayers layers)
			: this(new LayerEditorViewModel(layers))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LayerEditorView" /> class.
		/// </summary>
		/// <param name="viewModel">The view model to host.</param>
		/// <exception cref="ArgumentNullException"><paramref name="viewModel" /> is <see langword="null" />.</exception>
		public LayerEditorView(LayerEditorViewModel viewModel)
			: base(viewModel)
		{
			_viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
			_viewModel.CollectionChanged += ViewModelCollectionChanged;
			_viewModel.LayerChanged += ViewModelLayerChanged;

			InitializeComponent();

			Loaded += LayerEditorViewLoaded;
			Unloaded += LayerEditorViewUnloaded;
		}

		/// <inheritdoc />
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>
		/// Occurs when a layer's filter setup changes.
		/// </summary>
		public event EventHandler<EventArgs> LayerChanged;

		/// <summary>
		/// Gets or sets the drag source UI element.
		/// </summary>
		public UIElement SourceUI { get; set; }

		/// <summary>
		/// Gets the supported drag effects.
		/// </summary>
		public DragDropEffects SupportedEffects => DragDropEffects.Move;

		/// <summary>
		/// Gets or sets the drop target UI element.
		/// </summary>
		public UIElement TargetUI { get; set; }

		/// <summary>
		/// Gets a value that indicates whether mouse offset should be applied.
		/// </summary>
		public bool ApplyMouseOffset { get; private set; }

		/// <inheritdoc />
		public DataObject GetDataObject(UIElement draggedElt)
		{
			if (draggedElt != null)
			{
				var dataItem = FindAncestor<System.Windows.Controls.ListBoxItem>(draggedElt);
				var listBox = FindAncestor<System.Windows.Controls.ListBox>(draggedElt);
				var item = listBox.ItemContainerGenerator.ItemFromContainer(dataItem);
				return new DataObject(typeof(Layer), item);
			}

			return null;
		}

		/// <inheritdoc />
		public void FinishDrag(UIElement draggedElt, DragDropEffects finalEffects)
		{
		}

		/// <inheritdoc />
		public bool IsDraggable(UIElement dragElt)
		{
			if (dragElt != null)
			{
				var dataItem = FindAncestor<System.Windows.Controls.ListBoxItem>(dragElt);

				if (dataItem != null)
				{
					var listBox = SourceUI as System.Windows.Controls.ListBox;
					var item = listBox.ItemContainerGenerator.ItemFromContainer(dataItem);
					if (item is ILayer layer && !_viewModel.IsDefaultLayer(layer))
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <inheritdoc />
		public bool IsValidDataObject(IDataObject obj)
		{
			return obj.GetDataPresent(typeof(Layer));
		}

		/// <inheritdoc />
		public void OnDropCompleted(IDataObject obj, Point dropPoint)
		{
			var result = VisualTreeHelper.HitTest(_lbLayers, dropPoint);
			if (result == null)
			{
				return;
			}

			var listBoxItem = FindAncestor<System.Windows.Controls.ListBoxItem>(result.VisualHit);
			if (listBoxItem == null)
			{
				return;
			}

			var newIndex = _lbLayers.ItemContainerGenerator.IndexFromContainer(listBoxItem);
			if (newIndex == _lbLayers.Items.Count - 1)
			{
				newIndex--;
			}

			var item = obj.GetData(typeof(Layer)) as ILayer;
			_viewModel.MoveLayer(item, newIndex);
		}

		/// <inheritdoc />
		public UIElement GetVisualFeedback(IDataObject obj)
		{
			return null;
		}

		UIElement IDropTargetAdvisor.GetTopContainer()
		{
			return TargetUI;
		}

		UIElement IDragSourceAdvisor.GetTopContainer()
		{
			return TargetUI;
		}

		private void LayerEditorViewLoaded(object sender, RoutedEventArgs e)
		{
			DragDropManager.SetDragSourceAdvisor(_lbLayers, this);
			DragDropManager.SetDropTargetAdvisor(_lbLayers, this);
		}

		private void LayerEditorViewUnloaded(object sender, RoutedEventArgs e)
		{
			Loaded -= LayerEditorViewLoaded;
			Unloaded -= LayerEditorViewUnloaded;
			_viewModel.CollectionChanged -= ViewModelCollectionChanged;
			_viewModel.LayerChanged -= ViewModelLayerChanged;
		}

		private void ViewModelCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			OnCollectionChanged(e);
		}

		private void ViewModelLayerChanged(object sender, EventArgs e)
		{
			OnLayerChanged(e);
		}

		private static T FindAncestor<T>(DependencyObject current)
			where T : DependencyObject
		{
			var c = current;
			do
			{
				if (c is T ancestor)
				{
					return ancestor;
				}

				c = VisualTreeHelper.GetParent(c);
			}
			while (c != null);

			return null;
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
