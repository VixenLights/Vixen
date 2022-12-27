using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Common.WPFCommon.Input;
using Vixen.Module.MixingFilter;
using Vixen.Services;
using Vixen.Sys.LayerMixing;
using VixenModules.Editor.LayerEditor.Input;

namespace VixenModules.Editor.LayerEditor
{
	public class LayerEditorView : UserControl, IDragSourceAdvisor, IDropTargetAdvisor, INotifyCollectionChanged
	{
		private readonly SequenceLayers _layers;
		private List<ILayerMixingFilterInstance> _standardFilters = new List<ILayerMixingFilterInstance>();

		static LayerEditorView()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(LayerEditorView), new FrameworkPropertyMetadata(typeof(LayerEditorView)));
		}

		public LayerEditorView(SequenceLayers layers)
		{
			CommandBindings.Add(new CommandBinding(LayerEditorCommands.AddLayer, OnAddLayer, CanExecuteAddLayer));
			CommandBindings.Add(new CommandBinding(LayerEditorCommands.RemoveLayer, OnRemoveLayer, CanExecuteRemoveLayer));
			CommandBindings.Add(new CommandBinding(LayerEditorCommands.ConfigureLayer, ConfigureLayer, CanExecuteConfigureLayer));
			_layers = layers;
			InitializeLayers();
			DataContext = this;
			
			Loaded += LayerEditorView_Initialized;
			Layers.CollectionChanged += Layers_CollectionChanged;
		}

		private void LayerEditorView_Initialized(object sender, EventArgs e)
		{
			// I am not sure why the dependency property is not registering correctly so we are doing it manually. 
			var listBox = FindChild(this as UIElement, "_lbLayers", typeof(ListBox));
			if (listBox != null)
			{
				DragDropManager.SetDragSourceAdvisor(listBox, this);
				DragDropManager.SetDropTargetAdvisor(listBox, this);
			}
		}

		private void Layers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			OnCollectionChanged(e);
		}

		public ObservableCollection<ILayer> Layers
		{
			get { return _layers.Layers; }
		}

		public List<ILayerMixingFilterInstance> FilterTypes
		{
			get { return _standardFilters; }
		}

		private void InitializeLayers()
		{
			var descriptors = ApplicationServices.GetModuleDescriptors<ILayerMixingFilterInstance>();
			_standardFilters = descriptors.Select(filterType => ApplicationServices.Get<ILayerMixingFilterInstance>(filterType.TypeId)).ToList();
		}

		#region Commands

		private void OnAddLayer(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
		{
			var id = _standardFilters.First().Descriptor.TypeId;
			_layers.AddLayer(LayerMixingFilterService.Instance.GetInstance(id));
		}

		private void CanExecuteAddLayer(object sender, CanExecuteRoutedEventArgs canExecuteRoutedEventArgs)
		{
			canExecuteRoutedEventArgs.CanExecute = true;
		}

		private void OnRemoveLayer(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
		{
			ILayer layer = executedRoutedEventArgs.Parameter as ILayer;
			_layers.RemoveLayerAt(_layers.IndexOfLayer(layer));
		}

		private void CanExecuteRemoveLayer(object sender, CanExecuteRoutedEventArgs canExecuteRoutedEventArgs)
		{
			ILayer layer = canExecuteRoutedEventArgs.Parameter as ILayer;
			canExecuteRoutedEventArgs.CanExecute = layer!=null && !_layers.IsDefaultLayer(layer);
		}

		private void CanExecuteConfigureLayer(object sender, CanExecuteRoutedEventArgs canExecuteRoutedEventArgs)
		{
			ILayerMixingFilterInstance layerMixingFilterInstance = canExecuteRoutedEventArgs.Parameter as ILayerMixingFilterInstance;
			canExecuteRoutedEventArgs.CanExecute = layerMixingFilterInstance != null && layerMixingFilterInstance.HasSetup;
		}

		private void ConfigureLayer(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
		{
			ILayerMixingFilterInstance layerMixingFilterInstance = executedRoutedEventArgs.Parameter as ILayerMixingFilterInstance;
			var success = layerMixingFilterInstance.Setup();
			if (success)
			{
				OnLayerChanged(EventArgs.Empty);
			}
		}
		#endregion

		public Object SelectedItem { get; set; }

		#region DragSourceAdvisor

		public UIElement SourceUI { get; set; }
		public DragDropEffects SupportedEffects
		{
			get
			{
				return DragDropEffects.Move;
			}
		}
		public DataObject GetDataObject(UIElement draggedElt)
		{
			if (draggedElt != null)
			{
				var dataItem = FindAncestor<ListBoxItem>(draggedElt);
				var lb = FindAncestor<ListBox>(draggedElt);
				var item = lb.ItemContainerGenerator.ItemFromContainer(dataItem);
				return new DataObject(typeof(Layer), item);
			}

			return null;

		}

		public void FinishDrag(UIElement draggedElt, DragDropEffects finalEffects)
		{

		}

		public bool IsDraggable(UIElement dragElt)
		{
			if (dragElt != null)
			{
				var dataItem = FindAncestor<ListBoxItem>(dragElt);

				if (dataItem != null)
				{
					ListBox lb = SourceUI as ListBox;
					var item = lb.ItemContainerGenerator.ItemFromContainer(dataItem);
					Layer layer = item as Layer;
					if (layer != null && !_layers.IsDefaultLayer(layer))
					{
						return true;
					}

				}
			}

			return false;
		}

		#endregion

		#region DropTargetAdvisor

		public UIElement TargetUI { get; set; }
		public bool ApplyMouseOffset { get; private set; }
		public bool IsValidDataObject(IDataObject obj)
		{
			return obj.GetDataPresent(typeof(Layer));
		}

		public void OnDropCompleted(IDataObject obj, Point dropPoint)
		{
			ListBox lb = TargetUI as ListBox;
			if (lb == null)
			{
				return;
			}
			HitTestResult result = VisualTreeHelper.HitTest(lb, dropPoint);
			if (result == null)
			{
				return;
			}

			ListBoxItem lbi = FindAncestor<ListBoxItem>(result.VisualHit);
			if (lbi == null)
			{
				return;
			}
			int newIndex = lb.ItemContainerGenerator.IndexFromContainer(lbi);

			if (newIndex == lb.Items.Count - 1)
			{
				newIndex--;
			}
			var item = obj.GetData(typeof(Layer)) as Layer;
			int oldIndex = _layers.IndexOfLayer(item);
			_layers.MoveLayer(oldIndex, newIndex);
		}

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

		#endregion

		#region DragDrop Helper

		private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
		{
			var c = current;
			do
			{
				if (c is T)
				{
					return (T)c;
				}
				c = VisualTreeHelper.GetParent(c);
			}
			while (c != null);
			return null;
		}

		private static DependencyObject FindChild(DependencyObject reference, string childName, Type childType)
		{
			DependencyObject foundChild = null;
			if (reference != null)
			{
				int childrenCount = VisualTreeHelper.GetChildrenCount(reference);
				for (int i = 0; i < childrenCount; i++)
				{
					var child = VisualTreeHelper.GetChild(reference, i);
					// If the child is not of the request child type child
					if (child.GetType() != childType)
					{
						// recursively drill down the tree
						foundChild = FindChild(child, childName, childType);
						if (foundChild != null) break;
					}
					else if (!string.IsNullOrEmpty(childName))
					{
						var frameworkElement = child as FrameworkElement;
						// If the child's name is set for search
						if (frameworkElement != null && frameworkElement.Name == childName)
						{
							// if the child's name is of the request name
							foundChild = child;
							break;
						}
					}
					else
					{
						// child element found.
						foundChild = child;
						break;
					}
				}
			}
			return foundChild;
		}

		#endregion

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			if (CollectionChanged != null)
				CollectionChanged(this, args);
		}

		public event EventHandler<EventArgs> LayerChanged;
		private void OnLayerChanged(EventArgs e)
		{
			if (LayerChanged != null)
				LayerChanged(this, e);
		}
	}
}
