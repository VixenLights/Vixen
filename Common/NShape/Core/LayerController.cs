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
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using Dataweb.NShape.Advanced;
using Dataweb.NShape.Commands;


namespace Dataweb.NShape.Controllers
{
	/// <ToBeCompleted></ToBeCompleted>
	[ToolboxItem(true)]
	[ToolboxBitmap(typeof (LayerController), "LayerController.bmp")]
	public class LayerController : Component
	{
		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Controllers.LayerController" />.
		/// </summary>
		public LayerController()
		{
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.Controllers.LayerController" />.
		/// </summary>
		public LayerController(DiagramSetController diagramSetController)
			: this()
		{
			if (diagramSetController == null) throw new ArgumentNullException("diagramSetController");
			this.DiagramSetController = diagramSetController;
		}

		#region [Public] Events

		/// <summary>
		/// Occurs when the diagram of the LayerController has changed.
		/// </summary>
		public event EventHandler DiagramChanging;

		/// <summary>
		/// Raised when the diagram of the LayerController is going to change.
		/// </summary>
		public event EventHandler DiagramChanged;

		/// <summary>
		/// Occurs if layers has been added.
		/// </summary>
		public event EventHandler<LayersEventArgs> LayersAdded;

		/// <summary>
		/// Occurs if layers has been deleted.
		/// </summary>
		public event EventHandler<LayersEventArgs> LayersRemoved;

		/// <summary>
		/// Occurs if a layer has been modified.
		/// </summary>
		public event EventHandler<LayersEventArgs> LayerModified;

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


		/// <summary>
		/// Provides access to a DiagramSetController.
		/// </summary>
		[Category("NShape")]
		public DiagramSetController DiagramSetController
		{
			get { return diagramSetController; }
			set
			{
				if (diagramSetController != null) UnregisterDiagramSetControllerEvents();
				diagramSetController = value;
				if (diagramSetController != null) RegisterDiagramSetControllerEvents();
			}
		}


		/// <summary>
		/// Provides access to a <see cref="T:Dataweb.NShape.Project" />.
		/// </summary>
		[Category("NShape")]
		public Project Project
		{
			get
			{
				if (diagramSetController == null) return null;
				else return diagramSetController.Project;
			}
		}

		#endregion

		#region [Public] Methods

		/// <summary>
		/// Adds a new layer to the given diagram.
		/// </summary>
		public void AddLayer(Diagram diagram)
		{
			if (diagram == null) throw new ArgumentNullException("diagram");
			AssertDiagramSetControllerIsSet();
			string newLayerName = GetNewLayerName(diagram);
			AddLayer(diagram, newLayerName);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void AddLayer(Diagram diagram, string layerName)
		{
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (layerName == null) throw new ArgumentNullException("layerName");
			AssertDiagramSetControllerIsSet();
			if (diagram.Layers.FindLayer(layerName) != null)
				throw new NShapeException("Layer name '{0}' already exists.", layerName);
			Command cmd = new AddLayerCommand(diagram, layerName);
			Project.ExecuteCommand(cmd);
			if (LayersAdded != null)
				LayersAdded(this, LayerHelper.GetLayersEventArgs(LayerHelper.GetLayers(layerName, diagram)));
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void RemoveLayers(Diagram diagram, IEnumerable<Layer> layers)
		{
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (layers == null) throw new ArgumentNullException("layers");
			AssertDiagramSetControllerIsSet();
			Command cmd = new RemoveLayerCommand(diagram, layers);
			Project.ExecuteCommand(cmd);
			if (LayersRemoved != null) LayersRemoved(this, LayerHelper.GetLayersEventArgs(layers));
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void RemoveLayer(Diagram diagram, string layerName)
		{
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (string.IsNullOrEmpty(layerName)) throw new ArgumentNullException("layerName");
			AssertDiagramSetControllerIsSet();
			Layer layer = diagram.Layers.FindLayer(layerName);
			if (layer == null) throw new NShapeException("Layer '{0}' does not exist.", layerName);
			Command cmd = new RemoveLayerCommand(diagram, layer);
			Project.ExecuteCommand(cmd);
			if (LayersRemoved != null)
				LayersRemoved(this, LayerHelper.GetLayersEventArgs(LayerHelper.GetLayers(layerName, diagram)));
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void RenameLayer(Diagram diagram, Layer layer, string oldName, string newName)
		{
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (layer == null) throw new ArgumentNullException("layer");
			if (oldName == null) throw new ArgumentNullException("oldName");
			if (newName == null) throw new ArgumentNullException("newName");
			AssertDiagramSetControllerIsSet();
			ICommand cmd = new EditLayerCommand(diagram, layer, EditLayerCommand.ChangedProperty.Name, oldName, newName);
			Project.ExecuteCommand(cmd);
			if (LayerModified != null)
				LayerModified(this, LayerHelper.GetLayersEventArgs(LayerHelper.GetLayers(layer.Id, diagram)));
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void SetLayerZoomBounds(Diagram diagram, Layer layer, int lowerZoomBounds, int upperZoomBounds)
		{
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (layer == null) throw new ArgumentNullException("layer");
			AssertDiagramSetControllerIsSet();
			ICommand cmdMinZoom = null;
			ICommand cmdMaxZoom = null;
			if (layer.LowerZoomThreshold != lowerZoomBounds)
				cmdMinZoom = new EditLayerCommand(diagram, layer,
				                                  EditLayerCommand.ChangedProperty.LowerZoomThreshold,
				                                  layer.LowerZoomThreshold, lowerZoomBounds);
			if (layer.UpperZoomThreshold != upperZoomBounds)
				cmdMaxZoom = new EditLayerCommand(diagram, layer,
				                                  EditLayerCommand.ChangedProperty.UpperZoomThreshold,
				                                  layer.UpperZoomThreshold, upperZoomBounds);

			ICommand cmd;
			if (cmdMinZoom != null && cmdMaxZoom != null) {
				cmd = new AggregatedCommand();
				((AggregatedCommand) cmd).Add(cmdMinZoom);
				((AggregatedCommand) cmd).Add(cmdMaxZoom);
			}
			else if (cmdMinZoom != null && cmdMaxZoom == null)
				cmd = cmdMinZoom;
			else if (cmdMaxZoom != null && cmdMinZoom == null)
				cmd = cmdMaxZoom;
			else cmd = null;

			if (cmd != null) {
				Project.ExecuteCommand(cmd);
				if (LayerModified != null)
					LayerModified(this, LayerHelper.GetLayersEventArgs(LayerHelper.GetLayers(layer.Id, diagram)));
			}
		}


		/// <summary>
		/// Returns a collection of <see cref="T:Dataweb.NShape.Advanced.MenuItemDef" /> for constructing context menus etc.
		/// </summary>
		public IEnumerable<MenuItemDef> GetMenuItemDefs(Diagram diagram, Dataweb.NShape.Advanced.IReadOnlyCollection<Layer> selectedLayers)
		{
			if (diagram == null) throw new ArgumentNullException("diagram");
			if (selectedLayers == null) throw new ArgumentNullException("selectedLayers");
			bool isFeasible;
			string description;

			isFeasible = diagram.Layers.Count < Enum.GetValues(typeof (LayerIds)).Length;
			description = isFeasible ? "Add a new layer to the diagram" : "Maximum number of layers reached";
			yield return new DelegateMenuItemDef("Add Layer",
			                                     Properties.Resources.NewLayer, description, isFeasible, Permission.Data,
			                                     diagram != null ? diagram.SecurityDomainName : 'A',
			                                     (a, p) => AddLayer(diagram));

			isFeasible = selectedLayers.Count > 0;
			description = isFeasible ? string.Format("Delete {0} Layers", selectedLayers.Count) : "No layers selected";
			yield return new DelegateMenuItemDef(string.Format("Delete Layer{0}", selectedLayers.Count > 1 ? "s" : string.Empty),
			                                     Properties.Resources.DeleteBtn, description, isFeasible, Permission.Data,
			                                     diagram != null ? diagram.SecurityDomainName : 'A',
			                                     (a, p) => this.RemoveLayers(diagram, selectedLayers));
		}

		#endregion

		#region [Private] Methods

		private void RegisterDiagramSetControllerEvents()
		{
			diagramSetController.ProjectChanged += diagramSetController_ProjectChanged;
			diagramSetController.ProjectChanging += diagramSetController_ProjectChanging;
			if (diagramSetController.Project != null) RegisterProjectEvents();
		}


		private void UnregisterDiagramSetControllerEvents()
		{
			if (diagramSetController.Project != null) UnregisterProjectEvents();
			diagramSetController.ProjectChanging -= diagramSetController_ProjectChanging;
			diagramSetController.ProjectChanged -= diagramSetController_ProjectChanged;
		}


		private void RegisterProjectEvents()
		{
			AssertProjectIsSet();
			Project.Opened += project_ProjectOpen;
			Project.Closing += project_ProjectClosing;
			Project.Closed += project_ProjectClosed;
			if (Project.IsOpen) project_ProjectOpen(this, null);
		}


		private void UnregisterProjectEvents()
		{
			AssertProjectIsSet();
			if (Project.Repository != null)
				UnregisterRepositoryEvents();
			Project.Opened -= project_ProjectOpen;
			Project.Closing -= project_ProjectClosing;
			Project.Closed -= project_ProjectClosed;
		}


		private void RegisterRepositoryEvents()
		{
			AssertRepositoryIsSet();
			if (!repositoryEventsRegistered) {
				Project.Repository.DiagramUpdated += Repository_DiagramUpdated;
				repositoryEventsRegistered = true;
			}
		}


		private void UnregisterRepositoryEvents()
		{
			AssertRepositoryIsSet();
			if (repositoryEventsRegistered) {
				Project.Repository.DiagramUpdated += Repository_DiagramUpdated;
				repositoryEventsRegistered = false;
			}
		}


		private void AssertProjectIsSet()
		{
			if (Project == null)
				throw new NShapeException("{0}'s property 'Project' is not set.", typeof (DiagramSetController).FullName);
		}


		private void AssertRepositoryIsSet()
		{
			AssertProjectIsSet();
			if (Project.Repository == null) throw new NShapeException("Project's 'Repository' property is not set.");
		}


		private void AssertDiagramSetControllerIsSet()
		{
			if (diagramSetController == null) throw new NShapeException("Property 'DiagramController' is not set.");
		}


		private string GetNewLayerName(Diagram diagram)
		{
			string result = string.Empty;
			// get all used Layerids
			LayerIds usedLayerIds = LayerIds.None;
			foreach (Layer l in diagram.Layers)
				usedLayerIds |= l.Id;
			// find the first Id available
			foreach (LayerIds value in Enum.GetValues(typeof (LayerIds))) {
				if (value == LayerIds.None) continue;
				if ((usedLayerIds & value) == 0) {
					int bitNo = (int) Math.Log((int) value, 2);
					result = string.Format("Layer {0:D2}", bitNo + 1);
					break;
				}
			}
			return result;
		}

		#endregion

		#region [Private] Methods: EventHandler implementations

		private void diagramSetController_ProjectChanging(object sender, EventArgs e)
		{
			if (diagramSetController.Project != null) UnregisterProjectEvents();
		}


		private void diagramSetController_ProjectChanged(object sender, EventArgs e)
		{
			if (diagramSetController.Project != null) RegisterProjectEvents();
		}


		private void project_ProjectOpen(object sender, EventArgs e)
		{
			AssertRepositoryIsSet();
			RegisterRepositoryEvents();
		}


		private void project_ProjectClosing(object sender, EventArgs e)
		{
			// nothing to do...
		}


		private void project_ProjectClosed(object sender, EventArgs e)
		{
			AssertRepositoryIsSet();
			UnregisterRepositoryEvents();
		}


		private void Repository_DiagramUpdated(object sender, RepositoryDiagramEventArgs e)
		{
			// This is only a simple workaround.
			// ToDo: Create a new event for this purpose!
			if (DiagramChanging != null && DiagramChanged != null) {
				DiagramChanging(this, EventArgs.Empty);
				DiagramChanged(this, EventArgs.Empty);
			}
		}


		private void diagrammController_ActiveLayersChanged(object sender, LayersEventArgs e)
		{
			if (LayerModified != null) LayerModified(this, e);
		}


		private void diagramController_LayerVisibilityChanged(object sender, LayersEventArgs e)
		{
			if (LayerModified != null) LayerModified(this, e);
		}


		private void diagramController_DiagramChanging(object sender, EventArgs e)
		{
			if (DiagramChanging != null) DiagramChanging(this, e);
		}


		private void diagramController_DiagramChanged(object sender, EventArgs e)
		{
			if (DiagramChanged != null) DiagramChanged(this, e);
		}

		#endregion

		#region Fields

		/// <ToBeCompleted></ToBeCompleted>
		public const int MaxLayerCount = 31;

		private DiagramSetController diagramSetController = null;
		private bool repositoryEventsRegistered = false;

		private LayersEventArgs layersEventArgs = new LayersEventArgs();
		private LayerEventArgs layerEventArgs = new LayerEventArgs();
		private LayerRenamedEventArgs layerRenamedEventArgs = new LayerRenamedEventArgs();

		#endregion
	}

	#region EventArgs

	/// <ToBeCompleted></ToBeCompleted>
	public class LayerEventArgs : EventArgs
	{
		/// <ToBeCompleted></ToBeCompleted>
		public LayerEventArgs(Layer layer)
		{
			this.layer = layer;
		}

		/// <ToBeCompleted></ToBeCompleted>
		public Layer Layer
		{
			get { return layer; }
			internal set { layer = value; }
		}

		internal LayerEventArgs()
		{
		}

		private Layer layer = null;
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class LayersEventArgs : EventArgs
	{
		/// <ToBeCompleted></ToBeCompleted>
		public LayersEventArgs(IEnumerable<Layer> layers)
		{
			if (layers == null) throw new ArgumentNullException("layers");
			this.layers = new ReadOnlyList<Layer>(layers);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public Dataweb.NShape.Advanced.IReadOnlyCollection<Layer> Layers
		{
			get { return layers; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected internal LayersEventArgs()
		{
			layers = new ReadOnlyList<Layer>();
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected internal void SetLayers(ReadOnlyList<Layer> layers)
		{
			this.layers.Clear();
			this.layers.AddRange(layers);
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected internal void SetLayers(IEnumerable<Layer> layers)
		{
			this.layers.Clear();
			this.layers.AddRange(layers);
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected internal void SetLayers(Layer layer)
		{
			this.layers.Clear();
			this.layers.Add(layer);
		}


		private ReadOnlyList<Layer> layers = null;
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class LayerRenamedEventArgs : LayerEventArgs
	{
		/// <ToBeCompleted></ToBeCompleted>
		public LayerRenamedEventArgs(Layer layer, string oldName, string newName)
			: base(layer)
		{
			this.oldName = oldName;
			this.newName = newName;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public string OldName
		{
			get { return oldName; }
			internal set { oldName = value; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public string NewName
		{
			get { return newName; }
			internal set { newName = value; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected internal LayerRenamedEventArgs()
		{
		}


		private string oldName;
		private string newName;
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class LayerZoomThresholdChangedEventArgs : LayerEventArgs
	{
		/// <ToBeCompleted></ToBeCompleted>
		public LayerZoomThresholdChangedEventArgs(Layer layer, int oldZoomThreshold, int newZoomThreshold)
			: base(layer)
		{
			this.oldZoomThreshold = oldZoomThreshold;
			this.newZoomThreshold = newZoomThreshold;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public int OldZoomThreshold
		{
			get { return oldZoomThreshold; }
			internal set { oldZoomThreshold = value; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		public int NewZoomThreshold
		{
			get { return newZoomThreshold; }
			internal set { newZoomThreshold = value; }
		}


		/// <ToBeCompleted></ToBeCompleted>
		protected internal LayerZoomThresholdChangedEventArgs()
		{
		}


		private int oldZoomThreshold;
		private int newZoomThreshold;
	}

	#endregion

	/// <ToBeCompleted></ToBeCompleted>
	internal class LayerHelper
	{
		/// <ToBeCompleted></ToBeCompleted>
		public static IEnumerable<Layer> GetLayers(LayerIds layerId, Diagram diagram)
		{
			if (diagram == null) throw new ArgumentNullException("diagram");
			return diagram.Layers.GetLayers(layerId);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static IEnumerable<Layer> Getlayers(Layer layer)
		{
			yield return layer;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static IEnumerable<Layer> GetLayers(string layerName, Diagram diagram)
		{
			if (diagram == null) throw new ArgumentNullException("diagram");
			yield return diagram.Layers.FindLayer(layerName);
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static LayerEventArgs GetLayerEventArgs(string layerName, Diagram diagram)
		{
			if (diagram == null) throw new ArgumentNullException("diagram");
			Layer layer = diagram.Layers.FindLayer(layerName);
			Debug.Assert(layer != null);
			layerEventArgs.Layer = layer;
			return layerEventArgs;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static LayerEventArgs GetLayerEventArgs(Layer layer)
		{
			if (layer == null) throw new ArgumentNullException("layer");
			layerEventArgs.Layer = layer;
			return layerEventArgs;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static LayersEventArgs GetLayersEventArgs(Layer layer)
		{
			if (layer == null) throw new ArgumentNullException("layer");
			layersEventArgs.SetLayers(layer);
			return layersEventArgs;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static LayersEventArgs GetLayersEventArgs(ReadOnlyList<Layer> layers)
		{
			if (layers == null) throw new ArgumentNullException("layers");
			layersEventArgs.SetLayers(layers);
			return layersEventArgs;
		}


		/// <ToBeCompleted></ToBeCompleted>
		public static LayersEventArgs GetLayersEventArgs(IEnumerable<Layer> layers)
		{
			if (layers == null) throw new ArgumentNullException("layers");
			layersEventArgs.SetLayers(layers);
			return layersEventArgs;
		}


		private static LayerEventArgs layerEventArgs = new LayerEventArgs();
		private static LayersEventArgs layersEventArgs = new LayersEventArgs();
	}
}