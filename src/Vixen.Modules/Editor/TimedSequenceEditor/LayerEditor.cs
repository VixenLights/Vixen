using System.Windows;
using System.Windows.Forms.Integration;
using Vixen.Sys.LayerMixing;
using WeifenLuo.WinFormsUI.Docking;
using Application = System.Windows.Application;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class LayerEditor : DockContent
	{
		private readonly Editor.LayerEditor.LayerEditorView _layerEditorView;
		private ElementHost host;

		static LayerEditor()
		{
			ResourceDictionary dict = new ResourceDictionary
			{
				Source = new Uri("/LayerEditor;component/Themes/Generic.xaml", UriKind.Relative)
			};

			Application.Current.Resources.MergedDictionaries.Add(dict);
		}

		public LayerEditor(SequenceLayers layers)
		{
		
			InitializeComponent();
			

			host = new ElementHost { Dock = DockStyle.Fill };

			BackColor = Color.Black;

			_layerEditorView = new Editor.LayerEditor.LayerEditorView(layers);

			_layerEditorView.CollectionChanged += LayerEditorViewCollectionChanged;
			_layerEditorView.LayerChanged += LayerEditorViewOnLayerChanged;

			host.Child = _layerEditorView;
			Controls.Add(host);
		}

		private void LayerEditorViewOnLayerChanged(object sender, EventArgs eventArgs)
		{
			if (LayersChanged != null)
				LayersChanged(this, eventArgs);
		}

		private void LayerEditorViewCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			OnMixingLayerFilterCollectionChanged(new EventArgs());
		}

		public event EventHandler<EventArgs> LayersChanged;
		protected virtual void OnMixingLayerFilterCollectionChanged(EventArgs e)
		{
			if (LayersChanged != null)
				LayersChanged(this, e);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			if (_layerEditorView != null)
			{
				_layerEditorView.CollectionChanged -= LayerEditorViewCollectionChanged;
				_layerEditorView.LayerChanged -= LayerEditorViewOnLayerChanged;
			}
			base.Dispose(disposing);
		}
	}
}
