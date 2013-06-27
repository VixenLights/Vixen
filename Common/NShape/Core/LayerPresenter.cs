/******************************************************************************
  Copyright 2009-2012 dataweb GmbH
  This file is part of the NShape framework.
  NShape is free software: you can redistribute it and/or modify it under the 
  terms of the GNU General Public License as published by the Free Software 
  Foundation, either version 3 of the License, or (at your option) any later 
  version.
  NShape is distributed in the hope that it will be useful, but WITHOUT ANY
  WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
  A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
  You should have received a copy of the GNU General Public License along with 
  NShape. If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Dataweb.NShape.Advanced;


namespace Dataweb.NShape.Controllers
{
	/// <summary>
	/// Non-visual component implementing the functionality of a layer presenter.
	/// Provides methods and properties for connecting an ILayerView user interface widget with a LayerController.
	/// </summary>
	[ToolboxItem(true)]
	[ToolboxBitmap("LayerPresenter.bmp")]
	public class LayerPresenter : Component
	{
		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Controllers.LayerPresenter" />.
		/// </summary>
		public LayerPresenter()
			: base()
		{
		}

		#region [Public] Events

		/// <ToBeCompleted></ToBeCompleted>
		public event EventHandler<LayersEventArgs> LayerSelectionChanged;

		#endregion

		#region [Public] Properties

		/// <summary>
		/// Specifies the version of the assembly containing the component.
		/// </summary>
		[Category("NShape")]
		public string ProductVersion
		{
			get { return this.GetType().Assembly.GetName().Version.ToString(); }
		}


		/// <ToBeCompleted></ToBeCompleted>
		[Category("NShape")]
		public LayerController LayerController
		{
			get { return layerController; }
			set
			{
				if (layerController != null) UnregisterLayerControllerEvents();
				layerController = value;
				if (layerController != null) RegisterLayerControllerEvents();
			}
		}


		/// <ToBeCompleted></ToBeCompleted>
		[Category("NShape")]
		public IDiagramPresenter DiagramPresenter
		{
			get { return diagramPresenter; }
			set
			{
				if (diagramPresenter != null) {
					// Unregister events and clear layer view
					UnregisterDiagramPresenterEvents();
					if (layerView != null) layerView.Clear();
				}
				diagramPresenter = value;
				if (diagramPresenter != null) {
					RegisterDiagramPresenterEvents();
					TryFillLayerView();
				}
			}
		}


		/// <summary>
		/// Provides access to a <see cref="T:Dataweb.NShape.Project" />.
		/// </summary>
		[Category("NShape")]
		public Project Project
		{
			get { return (layerController == null) ? null : layerController.Project; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		[Category("NShape")]
		public ILayerView LayerView
		{
			get { return layerView; }
			set
			{
				if (layerView != null) {
					// Unregister events and clear view
					UnregisterLayerViewEvents();
					layerView.Clear();
				}
				layerView = value;
				if (layerView != null) {
					// Register events
					RegisterLayerViewEvents();
					TryFillLayerView();
				}
			}
		}


		/// <summary>
		/// Specifies if MenuItemDefs that are not granted should appear as MenuItems in the dynamic context menu.
		/// </summary>
		[Category("Behavior")]
		public bool HideDeniedMenuItems
		{
			get { return hideMenuItemsIfNotGranted; }
			set { hideMenuItemsIfNotGranted = value; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		[Browsable(false)]
		public IReadOnlyCollection<Layer> SelectedLayers
		{
			get { return selectedLayers; }
		}

		#endregion

		#region Methods (protected)

		/// <summary>
		/// Returns a collection of <see cref="T:Dataweb.NShape.Advanced.MenuItemDef" /> for constructing context menus etc.
		/// </summary>
		protected IEnumerable<MenuItemDef> GetMenuItemDefs()
		{
			if (layerController == null || diagramPresenter == null)
				yield break;

			string pluralPostFix = (selectedLayers.Count > 1) ? "s" : string.Empty;

			bool separatorNeeded = false;
			foreach (MenuItemDef controllerAction in LayerController.GetMenuItemDefs(diagramPresenter.Diagram, selectedLayers)) {
				if (!separatorNeeded) separatorNeeded = true;
				yield return controllerAction;
			}

			if (separatorNeeded) yield return new SeparatorMenuItemDef();

			bool isFeasible;
			string description;
			char securityDomain = (diagramPresenter.Diagram != null) ? diagramPresenter.Diagram.SecurityDomainName : '-';

			isFeasible = selectedLayers.Count == 1;
			if (selectedLayers.Count == 0) description = string.Empty;
			else if (selectedLayers.Count == 1) description = string.Format("Rename layer '{0}'", selectedLayers[0].Title);
			else description = "Too many layers selected";
			yield return new DelegateMenuItemDef("Rename Layer", Properties.Resources.RenameBtn,
			                                     description, isFeasible, Permission.Data, securityDomain,
			                                     (a, p) => BeginRenameSelectedLayer());

			isFeasible = selectedLayers.Count > 0;
			if (isFeasible)
				description = string.Format("Set {0} layer{1} as the active layer{1}", selectedLayers.Count, pluralPostFix);
			else description = "No layers selected";
			yield return new DelegateMenuItemDef(string.Format("Activate Layer{0}", pluralPostFix),
			                                     Properties.Resources.Enabled, description, isFeasible, Permission.Data,
			                                     securityDomain,
			                                     (a, p) => ActivateSelectedLayers());

			isFeasible = selectedLayers.Count > 0;
			description = isFeasible
			              	? string.Format("Deactivate {0} layer{1}", selectedLayers.Count, pluralPostFix)
			              	: "No layers selected";
			yield return new DelegateMenuItemDef(string.Format("Deactivate Layer{0}", pluralPostFix),
			                                     Properties.Resources.Disabled, description, isFeasible, Permission.Data,
			                                     securityDomain,
			                                     (a, p) => DeactivateSelectedLayers());

			yield return new SeparatorMenuItemDef();

			isFeasible = selectedLayers.Count > 0;
			description = isFeasible
			              	? string.Format("Show {0} layer{1}", selectedLayers.Count, pluralPostFix)
			              	: "No layers selected";
			yield return new DelegateMenuItemDef(string.Format("Show Layer{0}", pluralPostFix),
			                                     Properties.Resources.Visible, description, isFeasible, Permission.None,
			                                     (a, p) => ShowSelectedLayers());

			isFeasible = selectedLayers.Count > 0;
			description = isFeasible
			              	? string.Format("Hide {0} layer{1}", selectedLayers.Count, pluralPostFix)
			              	: "No layers selected";
			yield return new DelegateMenuItemDef(string.Format("Hide Layer{0}", pluralPostFix),
			                                     Properties.Resources.Invisible, description, isFeasible, Permission.None,
			                                     (a, p) => HideSelectedLayers());
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void OnSelectedLayersChanged(object sender, LayersEventArgs e)
		{
			if (LayerSelectionChanged != null) LayerSelectionChanged(sender, e);
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void ShowSelectedLayers()
		{
			AssertLayerControllerIsSet();
			for (int i = selectedLayers.Count - 1; i >= 0; --i)
				diagramPresenter.SetLayerVisibility(selectedLayers[i].Id, true);
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void HideSelectedLayers()
		{
			AssertLayerControllerIsSet();
			for (int i = selectedLayers.Count - 1; i >= 0; --i)
				diagramPresenter.SetLayerVisibility(selectedLayers[i].Id, false);
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void ActivateSelectedLayers()
		{
			AssertLayerControllerIsSet();
			for (int i = selectedLayers.Count - 1; i >= 0; --i)
				diagramPresenter.SetLayerActive(selectedLayers[i].Id, true);
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void DeactivateSelectedLayers()
		{
			AssertLayerControllerIsSet();
			for (int i = selectedLayers.Count - 1; i >= 0; --i)
				diagramPresenter.SetLayerActive(selectedLayers[i].Id, false);
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected void BeginRenameSelectedLayer()
		{
			if (selectedLayers.Count == 0) throw new NShapeException("No layers selected.");
			layerView.BeginEditLayerName(selectedLayers[0]);
		}

		#endregion

		#region Methods (private)

		private void RegisterDiagramPresenterEvents()
		{
			diagramPresenter.ActiveLayersChanged += diagramPresenter_ActiveLayersChanged;
			diagramPresenter.LayerVisibilityChanged += diagramPresenter_LayerVisibilityChanged;
			diagramPresenter.DiagramChanged += diagramPresenter_DiagramChanged;
		}


		private void UnregisterDiagramPresenterEvents()
		{
			diagramPresenter.ActiveLayersChanged -= diagramPresenter_ActiveLayersChanged;
			diagramPresenter.LayerVisibilityChanged -= diagramPresenter_LayerVisibilityChanged;
		}


		private void SetSelectedLayers(Layer layer)
		{
			selectedLayers.Clear();
			selectedLayers.Add(layer);
			OnSelectedLayersChanged(this, LayerHelper.GetLayersEventArgs(selectedLayers));
		}


		private void SetSelectedLayers(IEnumerable<Layer> layers)
		{
			selectedLayers.Clear();
			foreach (Layer l in layers) selectedLayers.Add(l);
			OnSelectedLayersChanged(this, LayerHelper.GetLayersEventArgs(selectedLayers));
		}


		private void SelectLayer(Layer layer)
		{
			selectedLayers.Add(layer);
			OnSelectedLayersChanged(this, LayerHelper.GetLayersEventArgs(selectedLayers));
		}


		private void UnselectLayer(Layer layer)
		{
			selectedLayers.Remove(layer);
			OnSelectedLayersChanged(this, LayerHelper.GetLayersEventArgs(selectedLayers));
		}


		private void UnselectAllLayers()
		{
			selectedLayers.Clear();
			OnSelectedLayersChanged(this, LayerHelper.GetLayersEventArgs(selectedLayers));
		}


		private void AssertLayerControllerIsSet()
		{
			if (layerController == null) throw new Exception("Property 'LayerController' is not set.");
		}


		private void RegisterLayerControllerEvents()
		{
			layerController.DiagramChanging += layerController_DiagramChanging;
			layerController.DiagramChanged += layerController_DiagramChanged;
			layerController.LayersAdded += layerController_LayerAdded;
			layerController.LayersRemoved += layerController_LayerRemoved;
			layerController.LayerModified += layerController_LayerModified;
			RegisterProjectEvents(layerController.Project);
		}


		private void UnregisterLayerControllerEvents()
		{
			layerController.DiagramChanging -= layerController_DiagramChanging;
			layerController.DiagramChanged -= layerController_DiagramChanged;
			layerController.LayersAdded -= layerController_LayerAdded;
			layerController.LayersRemoved -= layerController_LayerRemoved;
			layerController.LayerModified -= layerController_LayerModified;
			UnregisterProjectEvents(layerController.Project);
		}


		private void RegisterProjectEvents(Project project)
		{
			if (project != null) {
				project.Closing += project_Closing;
				project.Closed += project_Closed;
			}
		}


		private void UnregisterProjectEvents(Project project)
		{
			if (project != null) {
				project.Closing -= project_Closing;
				project.Closed -= project_Closed;
			}
		}


		private void RegisterLayerViewEvents()
		{
			layerView.LayerRenamed += layerView_LayerItemRenamed;
			layerView.LayerLowerZoomThresholdChanged += layerView_LayerLowerZoomThresholdChanged;
			layerView.LayerUpperZoomThresholdChanged += layerView_LayerUpperZoomThresholdChanged;
			layerView.LayerViewMouseDown += layerView_MouseDown;
			layerView.LayerViewMouseMove += layerView_MouseMove;
			layerView.LayerViewMouseUp += layerView_MouseUp;
		}


		private void UnregisterLayerViewEvents()
		{
			layerView.LayerRenamed -= layerView_LayerItemRenamed;
			layerView.LayerLowerZoomThresholdChanged -= layerView_LayerLowerZoomThresholdChanged;
			layerView.LayerUpperZoomThresholdChanged -= layerView_LayerUpperZoomThresholdChanged;
			layerView.LayerViewMouseDown -= layerView_MouseDown;
			layerView.LayerViewMouseMove -= layerView_MouseMove;
			layerView.LayerViewMouseUp -= layerView_MouseUp;
		}


		private LayerIds GetSelectedLayerIds()
		{
			LayerIds result = LayerIds.None;
			for (int i = selectedLayers.Count - 1; i >= 0; --i)
				result |= selectedLayers[i].Id;
			return result;
		}


		private void GetLayerState(Layer layer, out bool isActive, out bool isVisible)
		{
			if (layer == null) throw new ArgumentNullException("layer");
			if (diagramPresenter == null) throw new ArgumentNullException("DiagramPresener");
			isActive = diagramPresenter.IsLayerActive(layer.Id);
			isVisible = diagramPresenter.IsLayerVisible(layer.Id);
		}


		private void TryFillLayerView()
		{
			// Fill view with layer items if necessary
			if (layerController != null && diagramPresenter != null && diagramPresenter.Diagram != null)
				AddLayerItemsToLayerView(diagramPresenter.Diagram.Layers);
		}


		private void AddLayerItemsToLayerView(IEnumerable<Layer> layers)
		{
			layerView.BeginUpdate();
			bool isActive, isVisible;
			foreach (Layer layer in layers) {
				GetLayerState(layer, out isActive, out isVisible);
				layerView.AddLayer(layer, isActive, isVisible);
			}
			layerView.EndUpdate();
		}


		private void AssertControllerIsSet()
		{
			if (LayerController == null) throw new ArgumentNullException("Controller");
		}

		#endregion

		#region LayerController EventHandler implementations

		private void layerController_DiagramChanging(object sender, EventArgs e)
		{
			if (layerView != null) layerView.Clear();
		}


		private void layerController_DiagramChanged(object sender, EventArgs e)
		{
			TryFillLayerView();
		}


		private void layerController_LayerAdded(object sender, LayersEventArgs e)
		{
			// Create LayerView items for the new layers
			if (e.Layers != null && layerView != null)
				AddLayerItemsToLayerView(e.Layers);
		}


		private void layerController_LayerModified(object sender, LayersEventArgs e)
		{
			if (e.Layers != null && layerView != null) {
				bool isActive, isVisible;
				foreach (Layer layer in e.Layers) {
					GetLayerState(layer, out isActive, out isVisible);
					layerView.RefreshLayer(layer, isActive, isVisible);
				}
			}
		}


		private void layerController_LayerRemoved(object sender, LayersEventArgs e)
		{
			foreach (Layer layer in e.Layers)
				layerView.RemoveLayer(layer);
		}

		#endregion

		#region Project  EventHandler Implementations

		private void project_Closing(object sender, EventArgs e)
		{
			// Nothing to do
		}


		private void project_Closed(object sender, EventArgs e)
		{
			if (LayerView != null) LayerView.Clear();
		}

		#endregion

		#region IDiagramPresenter EventHandler implementations

		private void diagramPresenter_LayerVisibilityChanged(object sender, LayersEventArgs e)
		{
			foreach (Layer layer in e.Layers)
				layerView.RefreshLayer(layer, diagramPresenter.IsLayerActive(layer.Id), diagramPresenter.IsLayerVisible(layer.Id));
		}


		private void diagramPresenter_ActiveLayersChanged(object sender, LayersEventArgs e)
		{
			foreach (Layer layer in e.Layers)
				layerView.RefreshLayer(layer, diagramPresenter.IsLayerActive(layer.Id), diagramPresenter.IsLayerVisible(layer.Id));
		}


		private void diagramPresenter_DiagramChanged(object sender, EventArgs e)
		{
			if (layerView != null && DiagramPresenter.Diagram != null)
				AddLayerItemsToLayerView(diagramPresenter.Diagram.Layers);
		}

		#endregion

		#region ILayerView EventHandler implementations

		private void layerView_MouseDown(object sender, LayerMouseEventArgs e)
		{
			if (e.Layer != null) {
				bool multiSelect = e.Modifiers == KeysDg.Shift || e.Modifiers == KeysDg.Control;
				if (multiSelect) {
					if (selectedLayers.Contains(e.Layer))
						UnselectLayer(e.Layer);
					else SelectLayer(e.Layer);
				}
				else SetSelectedLayers(e.Layer);
			}
			else UnselectAllLayers();
		}


		private void layerView_MouseMove(object sender, LayerMouseEventArgs e)
		{
			// ToDo: MouseHover image highlighting
		}


		private void layerView_MouseUp(object sender, LayerMouseEventArgs e)
		{
			switch (e.Buttons) {
				case MouseButtonsDg.Left:
					switch (e.Item) {
						case LayerItem.Name:
							if (e.Layer != null && selectedLayers.Contains(e.Layer))
								layerView.BeginEditLayerName(e.Layer);
							break;
						case LayerItem.ActiveState:
							if (e.Layer != null)
								diagramPresenter.SetLayerActive(e.Layer.Id, !diagramPresenter.IsLayerActive(e.Layer.Id));
							break;
						case LayerItem.Visibility:
							if (e.Layer != null)
								diagramPresenter.SetLayerVisibility(e.Layer.Id, !diagramPresenter.IsLayerVisible(e.Layer.Id));
							break;
						case LayerItem.MinZoom:
							if (e.Layer != null && selectedLayers.Contains(e.Layer))
								layerView.BeginEditLayerMinZoomBound(e.Layer);
							break;
						case LayerItem.MaxZoom:
							if (e.Layer != null && selectedLayers.Contains(e.Layer))
								layerView.BeginEditLayerMaxZoomBound(e.Layer);
							break;
						case LayerItem.None:
							// nothing to do
							break;
						default:
							throw new NShapeUnsupportedValueException(e.Item);
					}
					break;

					// Open context menu
				case MouseButtonsDg.Right:
					layerView.OpenContextMenu(e.Position.X, e.Position.Y, GetMenuItemDefs(),
					                          LayerController.DiagramSetController.Project);
					break;

				default:
					// Ignore all other buttons as well as combinations of buttons
					break;
			}
		}


		private void layerView_LayerItemRenamed(object sender, LayerRenamedEventArgs e)
		{
			layerController.RenameLayer(diagramPresenter.Diagram, e.Layer, e.OldName, e.NewName);
		}


		private void layerView_LayerUpperZoomThresholdChanged(object sender, LayerZoomThresholdChangedEventArgs e)
		{
			layerController.SetLayerZoomBounds(diagramPresenter.Diagram, e.Layer, e.Layer.LowerZoomThreshold, e.NewZoomThreshold);
		}


		private void layerView_LayerLowerZoomThresholdChanged(object sender, LayerZoomThresholdChangedEventArgs e)
		{
			layerController.SetLayerZoomBounds(diagramPresenter.Diagram, e.Layer, e.NewZoomThreshold, e.Layer.UpperZoomThreshold);
		}

		#endregion

		#region Fields

		private LayerController layerController;
		private IDiagramPresenter diagramPresenter;
		private ILayerView layerView;
		private ReadOnlyList<Layer> selectedLayers = new ReadOnlyList<Layer>();
		private bool hideMenuItemsIfNotGranted = false;

		private LayerEventArgs layerEventArgs = new LayerEventArgs();
		private LayersEventArgs layersEventArgs = new LayersEventArgs();

		#endregion
	}


	/// <summary>
	/// Interface for a layer presenter's user interface implementation.
	/// </summary>
	public interface ILayerView
	{
		#region Events

		/// <ToBeCompleted></ToBeCompleted>
		event EventHandler<LayersEventArgs> SelectedLayerChanged;

		/// <ToBeCompleted></ToBeCompleted>
		event EventHandler<LayerRenamedEventArgs> LayerRenamed;

		/// <ToBeCompleted></ToBeCompleted>
		event EventHandler<LayerZoomThresholdChangedEventArgs> LayerUpperZoomThresholdChanged;

		/// <ToBeCompleted></ToBeCompleted>
		event EventHandler<LayerZoomThresholdChangedEventArgs> LayerLowerZoomThresholdChanged;

		/// <ToBeCompleted></ToBeCompleted>
		event EventHandler<LayerMouseEventArgs> LayerViewMouseDown;

		/// <ToBeCompleted></ToBeCompleted>
		event EventHandler<LayerMouseEventArgs> LayerViewMouseMove;

		/// <ToBeCompleted></ToBeCompleted>
		event EventHandler<LayerMouseEventArgs> LayerViewMouseUp;

		#endregion

		#region Methods

		/// <summary>
		/// Clears the contents of the layer view.
		/// </summary>
		void Clear();

		/// <summary>
		/// Signals that the user interface is going to be updated.
		/// </summary>
		void BeginUpdate();

		/// <summary>
		/// Signals that the user interface was updated.
		/// </summary>
		void EndUpdate();

		/// <summary>
		/// Adds a new item to the user interface representing the given layer.
		/// </summary>
		void AddLayer(Layer layer, bool isActive, bool isVisible);

		/// <summary>
		/// Removes the item representing the given layer from the layerView's content.
		/// </summary>
		/// <param name="layer"></param>
		void RemoveLayer(Layer layer);

		/// <summary>
		/// Refreshes the contents of the item representing the given layer.
		/// </summary>
		void RefreshLayer(Layer layer, bool isActive, bool isVisible);

		/// <summary>
		/// Signals that the given layer is going to be renamed.
		/// </summary>
		void BeginEditLayerName(Layer layer);

		/// <summary>
		/// Signals that the lower zoom treshold of the given layer is going to be changed.
		/// </summary>
		void BeginEditLayerMinZoomBound(Layer layer);

		/// <summary>
		/// Signals that the upper zoom treshold of the given layer is going to be changed.
		/// </summary>
		void BeginEditLayerMaxZoomBound(Layer layer);

		/// <summary>
		/// Signals that the user interface should be redrawn.
		/// </summary>
		void Invalidate();

		/// <summary>
		/// Signals that a context menu buildt from the given contextMenuItemDefs should be opened at the given coordinates.
		/// </summary>
		void OpenContextMenu(int x, int y, IEnumerable<MenuItemDef> contextMenuItemDefs, Project project);

		#endregion
	}


	/// <summary>
	/// Specifies the type of user interface item.
	/// </summary>
	public enum LayerItem
	{
		/// <summary>Specifies the user iterface item displaying the layer's name.</summary>
		Name,

		/// <summary>Specifies the user iterface item displaying the layer's visibility state.</summary>
		Visibility,

		/// <summary>Specifies the user iterface item displaying the layer's active state.</summary>
		ActiveState,

		/// <summary>Specifies the user iterface item displaying the layer's lower zoom treshold.</summary>
		MinZoom,

		/// <summary>Specifies the user iterface item displaying the layer's upper zoom treshold.</summary>
		MaxZoom,

		/// <summary>No user iterface item.</summary>
		None
	}


	/// <summary>
	/// Mouse event args for layer view inplementations.
	/// </summary>
	public class LayerMouseEventArgs : MouseEventArgsDg
	{
		/// <ToBeCompleted></ToBeCompleted>
		public LayerMouseEventArgs(Layer layer, LayerItem item,
		                           MouseEventType eventType, MouseButtonsDg buttons, int clickCount, int wheelDelta,
		                           Point position, KeysDg modifiers)
			: base(eventType, buttons, clickCount, wheelDelta, position, modifiers)
		{
			this.layer = layer;
			this.item = item;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public LayerMouseEventArgs(Layer layer, LayerItem item, MouseEventArgsDg mouseEventArgs)
			: this(
				layer, item, mouseEventArgs.EventType, mouseEventArgs.Buttons, mouseEventArgs.Clicks, mouseEventArgs.WheelDelta,
				mouseEventArgs.Position, mouseEventArgs.Modifiers)
		{
			if (layer == null) throw new ArgumentNullException("layer");
			this.layer = layer;
			this.item = item;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public Layer Layer
		{
			get { return layer; }
			protected internal set { layer = value; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public LayerItem Item
		{
			get { return item; }
			protected internal set { item = value; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected internal LayerMouseEventArgs()
			: base()
		{
			layer = null;
			item = LayerItem.None;
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected internal void SetMouseEvent(MouseEventType eventType, MouseButtonsDg buttons,
		                                      int clickCount, int wheelDelta, Point position, KeysDg modifiers)
		{
			this.eventType = eventType;
			this.buttons = buttons;
			this.clicks = clickCount;
			this.modifiers = modifiers;
			this.position = position;
			this.wheelDelta = wheelDelta;
		}


		private Layer layer;
		private LayerItem item;
	}
}