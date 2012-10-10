using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Dataweb.NShape;
using Dataweb.NShape.Advanced;
using Dataweb.NShape.Controllers;
using Vixen.Data.Flow;
using Vixen.Module.OutputFilter;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Sys.Output;
using VixenApplication.FiltersAndPatching;

// TODO:
// (maybe) add some way to resize the filter shapes, in case they have a lot of outputs?
// add labels for filter shapes (so outputs can be labelled)


namespace VixenApplication
{
	public partial class ConfigFiltersAndPatching : Form
	{
		// map of data types, to the shape(s) that represent them. There should only be (potentially) multiple
		// shapes to represent a given channel node; this is because a node can be in multiple groups, and may
		// be displayed multiple times.
		private readonly Dictionary<ChannelNode, List<ChannelNodeShape>> _channelNodeToChannelShapes;
		private readonly Dictionary<IOutputDevice, ControllerShape> _controllerToControllerShape;
		private readonly Dictionary<IOutputFilterModuleInstance, FilterShape> _filterToFilterShape;
		private readonly Dictionary<IDataFlowComponent, List<FilterSetupShapeBase>> _dataFlowComponentToShapes;

		// list of all (root) shapes, in the order they should appear. (Child shapes for channels and
		// controllers are not in the list; they are of type NestingShape and handle their own bits.)
		private List<ChannelNodeShape> _channelShapes;
		private List<ControllerShape> _controllerShapes;
		private List<FilterShape> _filterShapes;

		private readonly Layer _visibleLayer;
		private readonly Layer _hiddenLayer;

		private int _channelsXPosition;
		private int _controllersXPosition;
		private int _previousDiagramWidth;		// used when resizing; what the width was BEFORE resize,
												// so we know how to layout filter shapes (proportionally)

		private VixenApplicationData _applicationData;

		private List<FilterShape> _filterShapeClipboard;

		private Timer _relayoutOnResizeTimer;

		public ConfigFiltersAndPatching(VixenApplicationData applicationData)
		{
			InitializeComponent();

			_applicationData = applicationData;

			project.LibrarySearchPaths.Add(@"Common\");
			project.AutoLoadLibraries = true;
			project.AddLibraryByName("VixenApplication", false);
			
			project.Name = "filterProject";
			project.Create();

			_visibleLayer = new Layer("Visible");
			_hiddenLayer = new Layer("Hidden");
			_controllerToControllerShape = new Dictionary<IOutputDevice, ControllerShape>();
			_channelNodeToChannelShapes = new Dictionary<ChannelNode, List<ChannelNodeShape>>();
			_filterToFilterShape = new Dictionary<IOutputFilterModuleInstance, FilterShape>();
			_dataFlowComponentToShapes = new Dictionary<IDataFlowComponent, List<FilterSetupShapeBase>>();
			_channelShapes = new List<ChannelNodeShape>();
			_controllerShapes = new List<ControllerShape>();
			_filterShapes = new List<FilterShape>();
			_previousDiagramWidth = 0;
		}

		private void ConfigFiltersAndPatching_Load(object sender, EventArgs e)
		{
			diagramDisplay.Diagram = diagramSetController.CreateDiagram("filterDiagram");
			diagramDisplay.Diagram.Size = new Size(0, 0);
			diagramDisplay.BackColor = Color.FromArgb(250, 250, 250);

			diagramDisplay.Diagram.Layers.Add(_visibleLayer);
			diagramDisplay.Diagram.Layers.Add(_hiddenLayer);
			diagramDisplay.SetLayerVisibility(_visibleLayer.Id, true);
			diagramDisplay.SetLayerVisibility(_hiddenLayer.Id, false);

			diagramDisplay.ShowDefaultContextMenu = false;
			diagramDisplay.ClicksOnlyAffectTopShape = true;
			diagramDisplay.HighQualityRendering = true;

			// A: fixed shapes with no connection points: nothing (parent nested shapes: node groups, controllers)
			((RoleBasedSecurityManager)diagramDisplay.Project.SecurityManager).SetPermissions(
				SECURITY_DOMAIN_FIXED_SHAPE_NO_CONNECTIONS, StandardRole.Operator, Permission.Insert);
			// B: fixed shapes with connection points: connect only (channel nodes (leaf), output shapes)
			((RoleBasedSecurityManager)diagramDisplay.Project.SecurityManager).SetPermissions(
				SECURITY_DOMAIN_FIXED_SHAPE_WITH_CONNECTIONS, StandardRole.Operator, Permission.Connect | Permission.Insert);
			// C: movable shapes: connect, layout (movable), and deleteable (filters, patch lines)
			((RoleBasedSecurityManager)diagramDisplay.Project.SecurityManager).SetPermissions(
				SECURITY_DOMAIN_MOVABLE_SHAPE_WITH_CONNECTIONS, StandardRole.Operator, Permission.Connect | Permission.Insert | Permission.Layout | Permission.Delete);
			// D: fixed shapes with no connection points, but deletable: only for established patch lines (so the user can't move them again, but an still delete them)
			((RoleBasedSecurityManager)diagramDisplay.Project.SecurityManager).SetPermissions(
				SECURITY_DOMAIN_FIXED_SHAPE_NO_CONNECTIONS_DELETABLE, StandardRole.Operator, Permission.Insert | Permission.Delete);

			((RoleBasedSecurityManager) diagramDisplay.Project.SecurityManager).SetPermissions(StandardRole.Operator, Permission.All);
			((RoleBasedSecurityManager)diagramDisplay.Project.SecurityManager).CurrentRole = StandardRole.Operator;

			FillStyle styleChannelGroup = new FillStyle("ChannelGroup",
				new ColorStyle("", Color.FromArgb(120, 160, 240)), new ColorStyle("", Color.FromArgb(90, 120, 180)));
			styleChannelGroup.FillMode = FillMode.Gradient;
			FillStyle styleChannelLeaf = new FillStyle("ChannelLeaf",
				new ColorStyle("", Color.FromArgb(200, 220, 255)), new ColorStyle("", Color.FromArgb(140, 160, 200)));
			styleChannelLeaf.FillMode = FillMode.Gradient;
			FillStyle styleFilter = new FillStyle("Filter",
				new ColorStyle("", Color.FromArgb(255, 220, 150)), new ColorStyle("", Color.FromArgb(230, 200, 100)));
			styleFilter.FillMode = FillMode.Gradient;
			FillStyle styleController = new FillStyle("Controller",
				new ColorStyle("", Color.FromArgb(100, 200, 100)), new ColorStyle("", Color.FromArgb(50, 200, 50)));
			styleController.FillMode = FillMode.Gradient;
			FillStyle styleOutput = new FillStyle("Output",
				new ColorStyle("", Color.FromArgb(180, 230, 180)), new ColorStyle("", Color.FromArgb(120, 210, 120)));
			styleOutput.FillMode = FillMode.Gradient;

			project.Design.FillStyles.Add(styleChannelGroup, styleChannelGroup);
			project.Design.FillStyles.Add(styleChannelLeaf, styleChannelLeaf);
			project.Design.FillStyles.Add(styleFilter, styleFilter);
			project.Design.FillStyles.Add(styleController, styleController);
			project.Design.FillStyles.Add(styleOutput, styleOutput);

			_InitializeShapesFromChannels();
			_InitializeShapesFromFilters();
			_InitializeShapesFromControllers();
			_RelayoutAllShapes();
			_CreateConnectionsFromExistingLinks();

			diagramDisplay.CurrentTool = new ConnectionTool();

			_populateComboBox();

			diagramDisplay.SelectedShapes.Clear();
		}

		private void _populateComboBox()
		{
			comboBoxNewFilterTypes.Items.Clear();
			foreach (KeyValuePair<Guid, string> kvp in ApplicationServices.GetAvailableModules<IOutputFilterModuleInstance>()) {
				comboBoxNewFilterTypes.Items.Add(new FilterTypeComboBoxEntry(kvp.Key, kvp.Value));
			}
			if (comboBoxNewFilterTypes.Items.Count > 0) {
				comboBoxNewFilterTypes.SelectedIndex = 0;
			}
		}

		public class FilterTypeComboBoxEntry
		{
			public FilterTypeComboBoxEntry(Guid guid, string description)
			{
				Guid = guid;
				Description = description;
			}

			public Guid Guid { get; private set; }
			private string Description { get; set; }

			public override string ToString()
			{
				if (Description.Length <= 0)
					return "Unnamed Filter";

				return Description;
			}
		}


		private void buttonAddFilter_Click(object sender, EventArgs e)
		{
			FilterTypeComboBoxEntry item = comboBoxNewFilterTypes.SelectedItem as FilterTypeComboBoxEntry;
			if (item == null) {
				MessageBox.Show("Please select a filter type first.", "Select filter type");
				return;
			}

			_CreateNewFilterInstanceAndShape(item.Guid, true);
		}


		private void buttonZoomIn_Click(object sender, EventArgs e)
		{
			diagramDisplay.ZoomLevel = (int)((float)diagramDisplay.ZoomLevel * 1.08);
		}

		private void buttonZoomOut_Click(object sender, EventArgs e)
		{
			diagramDisplay.ZoomLevel = (int)((float)diagramDisplay.ZoomLevel * 0.92);
		}

		private void buttonDelete_Click(object sender, EventArgs e)
		{
			_DeleteShapes(diagramDisplay.SelectedShapes);
		}

		private void diagramDisplay_KeyDown(object sender, KeyEventArgs e)
		{
			// if Delete was pressed, iterate through all selected shapes, and remove them, unlinking components as necessary
			if (e.KeyCode == Keys.Delete) {
				_DeleteShapes(diagramDisplay.SelectedShapes);
				e.Handled = true;
			}

			// CTRL-C and CTRL-V are copy/paste for filters, respectively
			if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control) {
				CopySelectedFiltersToClipboard();
				e.Handled = true;
			}

			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control) {
				PasteClipboardFilters(Cursor.Position);
				e.Handled = true;
			}
		}

		private void displayDiagram_ShapeDoubleClick(object sender, DiagramPresenterShapeClickEventArgs e)
		{
			var shape = (FilterSetupShapeBase) e.Shape;

			// workaround: only modify the shape if it's currently selected. The diagram likes to
			// send click events to all shapes under the mouse, even if they're not active.
			if (!diagramDisplay.SelectedShapes.Contains(shape)) {
				return;
			}

			if (shape is NestingSetupShape) {
				NestingSetupShape s = (shape as NestingSetupShape);
				s.Expanded = !s.Expanded;
			}
			else if (shape is FilterShape) {
				FilterShape filterShape = shape as FilterShape;
				filterShape.RunSetup();
			}

			if (shape is ChannelNodeShape)
				_ResizeAndPositionChannelShapes(_channelsXPosition);

			if (shape is ControllerShape)
				_ResizeAndPositionControllerShapes(_controllersXPosition);
		}

		private void ConfigFiltersAndPatching_ResizeEnd(object sender, EventArgs e)
		{
			_RelayoutAllShapes();
		}

		private void ConfigFiltersAndPatching_Resize(object sender, EventArgs e)
		{
			if (_relayoutOnResizeTimer == null) {
				_relayoutOnResizeTimer = new Timer();
				_relayoutOnResizeTimer.Interval = 250;
				_relayoutOnResizeTimer.Tick += new EventHandler(_relayoutOnResizeTimer_Tick);
				_relayoutOnResizeTimer.Start();
			} else {
				_relayoutOnResizeTimer.Stop();
				_relayoutOnResizeTimer.Start();
			}
		}

		void _relayoutOnResizeTimer_Tick(object sender, EventArgs e)
		{
			_relayoutOnResizeTimer.Stop();
			_relayoutOnResizeTimer = null;
			_RelayoutAllShapes();
		}

		private void cachedRepository_ShapesUpdated(object sender, RepositoryShapesEventArgs e)
		{
			foreach (Shape shape in e.Shapes) {
				FilterShape filterShape = shape as FilterShape;
				if (filterShape != null) {
					_UpdateFilterPositionDataForFilter(filterShape);
				}
			}
		}

		private void diagramContextMenuStrip_Opening(object sender, CancelEventArgs e)
		{
			copyFilterToolStripMenuItem.Enabled = diagramDisplay.SelectedShapes.Any(x => (x is FilterShape));
			pasteFilterToolStripMenuItem.Enabled = _filterShapeClipboard != null && _filterShapeClipboard.Count > 0;
			pasteFilterMultipleToolStripMenuItem.Enabled = _filterShapeClipboard != null && _filterShapeClipboard.Count > 0;
		}

		private void copyFilterToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CopySelectedFiltersToClipboard();
		}

		private void CopySelectedFiltersToClipboard()
		{
			if (!diagramDisplay.SelectedShapes.Any(x => (x is FilterShape)))
				return;

			_filterShapeClipboard = new List<FilterShape>();
			foreach (Shape selectedShape in diagramDisplay.SelectedShapes) {
				if (selectedShape is FilterShape) {
					_filterShapeClipboard.Add(selectedShape as FilterShape);
				}
			}
		}

		private void pasteFilterToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PasteClipboardFilters(Cursor.Position);
		}

		private void pasteFilterMultipleToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PasteClipboardFiltersMultipleTimes();
		}

		private void PasteClipboardFiltersMultipleTimes()
		{
			Point cursor = Cursor.Position;
			using (
				NumberDialog numberDialog = new NumberDialog("Number of Copies", "How many copies of the given filter(s)?", 1, 1, 1000)
				) {
				if (numberDialog.ShowDialog() == DialogResult.OK) {
					PasteClipboardFilters(cursor, numberDialog.Value);
				}
			}
		}

		private void PasteClipboardFilters(Point cursorPosition, int numberOfCopies = 1)
		{
			if (_filterShapeClipboard == null || _filterShapeClipboard.Count <= 0)
				return;

			Point newPosition = diagramDisplay.PointToClient(cursorPosition);
			newPosition.X -= diagramDisplay.GetDiagramPosition().X;
			newPosition.Y += diagramDisplay.GetDiagramOffset().Y - (SHAPE_FILTERS_HEIGHT / 2);

			DuplicateFilterShapes(_filterShapeClipboard, numberOfCopies, newPosition);
		}

		public IEnumerable<FilterShape> DuplicateFilterShapes(
			IEnumerable<FilterShape> sourceShapes,
			int numberOfCopies,
			Point? startPosition = null,
			double horizontalPositionProportion = 0.5
			)
		{
			return DuplicateFilterInstancesToShapes(sourceShapes.Select(x => x.FilterInstance), numberOfCopies, startPosition, horizontalPositionProportion);
		}

		public IEnumerable<FilterShape> DuplicateFilterInstancesToShapes(
			IEnumerable<IOutputFilterModuleInstance> sourceInstances,
			int numberOfCopies,
			Point? startPosition = null,
			double horizontalPositionProportion = 0.5
			)
		{
			if (sourceInstances == null)
				return null;

			Point pos;
			if (startPosition == null) {
				pos = new Point();
				int shapesWidth = diagramDisplay.Width - (2 * diagramDisplay.GetDiagramPosition().X);
				pos.X = (int)(shapesWidth * horizontalPositionProportion);
				pos.Y = diagramDisplay.GetDiagramOffset().Y + (diagramDisplay.Height / 4);
			} else {
				pos = (Point)startPosition;
			}

			List<FilterShape> result = new List<FilterShape>();

			for (int i = 0; i < numberOfCopies; i++) {
				foreach (IOutputFilterModuleInstance instance in sourceInstances) {
					FilterShape newShape = _CreateNewFilterInstanceAndShape(instance.TypeId, false);
					newShape.FilterInstance.ModuleData = instance.ModuleData.Clone();
					newShape.ModuleDataUpdated();

					newShape.X = pos.X;
					newShape.Y = pos.Y;

					pos.Y += newShape.Height + SHAPE_VERTICAL_SPACING;

					// save the new shape position in the application data references
					FilterSetupFormShapePosition position = new FilterSetupFormShapePosition();
					position.xPositionProportion = (double)newShape.X / _previousDiagramWidth;
					position.yPosition = newShape.Y;
					_applicationData.FilterSetupFormShapePositions[newShape.FilterInstance.InstanceId] = position;

					result.Add(newShape);
				}
			}

			return result;
		}

		private bool _patchingWizardRunning;
		private void buttonPatchWizard_Click(object sender, EventArgs e)
		{
			if (!_patchingWizardRunning) {
				PatchingWizard wizard = new PatchingWizard(this);
				wizard.WizardFinished += wizard_WizardFinished;
				_patchingWizardRunning = true;
				wizard.Start(false);
			}
		}

		void wizard_WizardFinished(object sender, EventArgs e)
		{
			PatchingWizard wizard = sender as PatchingWizard;
			_patchingWizardRunning = false;
		}

		public event EventHandler DiagramShapesSelected
		{
			add { diagramDisplay.ShapesSelected += value; }
			remove { diagramDisplay.ShapesSelected -= value; }
		}

		public IShapeCollection SelectedShapes
		{
			get { return diagramDisplay.SelectedShapes; }
		}




		private void _DeleteShapes(IEnumerable<Shape> shapes)
		{
			foreach (var shape in shapes) {
				DataFlowConnectionLine line = shape as DataFlowConnectionLine;
				if (line != null) {
					VixenSystem.DataFlow.ResetComponentSource(line.DestinationDataComponent);
					_RemoveShape(line);
				}

				// we COULD use FilterSetupShapeBase, as all the operations below are generic.... but, we only want
				// to be able to delete filter shapes. We want to enforce all channels and outputs to be kept.
				FilterShape filterShape = shape as FilterShape;
				if (filterShape != null) {
					ControlPointId pointId;

					// go through all outputs for the filter, and check for connections to other shapes.
					// For any that we find, remove them (ie. set the other shape source to null).
					for (int i = 0; i < filterShape.OutputCount; i++) {
						pointId = filterShape.GetControlPointIdForOutput(i);
						_RemoveDataFlowLinksFromShapePoint(filterShape, pointId);
					}

					// now check the source of the filter; if it's connected to anything, remove the connecting shape.
					// (don't really need to reset the source for the filter, since we're removing it, but may as well
					// anyway, just in case there's something else that's paying attention...)
					pointId = filterShape.GetControlPointIdForInput(0);
					_RemoveDataFlowLinksFromShapePoint(filterShape, pointId);

					VixenSystem.Filters.RemoveFilter(filterShape.FilterInstance);
					_RemoveShape(filterShape);
				}
			}
		}

		private void _RemoveDataFlowLinksFromShapePoint(FilterSetupShapeBase shape, ControlPointId controlPoint)
		{
			foreach (ShapeConnectionInfo ci in shape.GetConnectionInfos(controlPoint, null)) {
				if (ci.OtherShape == null)
					continue;

				DataFlowConnectionLine line = ci.OtherShape as DataFlowConnectionLine;
				if (line == null)
					throw new Exception("a shape was connected to something other than a DataFlowLine!");

				if (line.DestinationDataComponent == null || line.SourceDataFlowComponentReference == null)
					throw new Exception("Can't remove a link that isn't fully connected!");

				// if the line is connected with the given shape as the SOURCE, remove the unknown DESTINATION's
				// source (on the other end of the line). Otherwise, it (should) be that the given shape is the
				// destination; so reset it's source. If neither of these are true, freak out.
				if (line.GetConnectionInfo(ControlPointId.FirstVertex, null).OtherShape == shape) {
					VixenSystem.DataFlow.ResetComponentSource(line.DestinationDataComponent);
				} else if (line.GetConnectionInfo(ControlPointId.LastVertex, null).OtherShape == shape) {
					VixenSystem.DataFlow.ResetComponentSource(shape.DataFlowComponent);
				} else {
					throw new Exception("Can't reset a link that has neither the source or destination for the given shape!");
				}

				_RemoveShape(line);
			}
		}

		private void _InitializeShapesFromChannels()
		{
			_channelShapes = new List<ChannelNodeShape>();

			foreach (ChannelNode node in VixenSystem.Nodes.GetRootNodes()) {
				_CreateShapeFromChannel(node);
			}
		}

		private ChannelNodeShape _CreateShapeFromChannel(ChannelNode node)
		{
			ChannelNodeShape channelShape = _MakeChannelNodeShape(node, 1);
			if (channelShape != null)
				_channelShapes.Add(channelShape);
			return channelShape;
		}

		private void _InitializeShapesFromControllers()
		{
			_controllerShapes = new List<ControllerShape>();

			foreach (IOutputDevice controller in VixenSystem.ControllerManagement.Devices) {
				_CreateShapeFromController(controller);
			}
		}

		private ControllerShape _CreateShapeFromController(IOutputDevice controller)
		{
			ControllerShape controllerShape = _MakeControllerShape(controller);
			if (controllerShape != null)
				_controllerShapes.Add(controllerShape);
			return controllerShape;
		}

		private void _InitializeShapesFromFilters()
		{
			_filterShapes = new List<FilterShape>();
			foreach (IOutputFilterModuleInstance filter in VixenSystem.Filters) {
				_CreateShapeFromFilter(filter);
			}
		}

		private FilterShape _CreateShapeFromFilter(IOutputFilterModuleInstance filter)
		{
			FilterShape filterShape = _MakeFilterShape(filter);
			if (filterShape != null)
				_filterShapes.Add(filterShape);
			return filterShape;
		}

		private void _CreateConnectionsFromExistingLinks()
		{
			// go through the existing system-side patches (DataFlow sources) and make connections for them all.
			// There's nothing to do for channel shapes; they don't have sources; only do filters and outputs

			// go through the filter shapes and build up links
			foreach (FilterShape filterShape in _filterShapes) {
				_LookupAndConnectShapeToSource(filterShape);
			}

			// go through the output shapes and build up links
			foreach (ControllerShape controllerShape in _controllerShapes) {
				foreach (OutputShape outputShape in controllerShape.ChildFilterShapes) {
					_LookupAndConnectShapeToSource(outputShape);
				}
			}
		}

		private void _LookupAndConnectShapeToSource(FilterSetupShapeBase shape)
		{
			if (shape.DataFlowComponent != null && shape.DataFlowComponent.Source != null) {
				IDataFlowComponentReference source = shape.DataFlowComponent.Source;
				if (!_dataFlowComponentToShapes.ContainsKey(source.Component)) {
					VixenSystem.Logging.Error("CreateConnectionsFromExistingLinks: can't find shape for source " + source.Component + source.OutputIndex);
					return;
				}
				List<FilterSetupShapeBase> sourceShapes = _dataFlowComponentToShapes[source.Component];
				// TODO: deal with multiple instances of the source data flow component: eg. a channel existing as
				// multiple shapes (currently, we'll assume it's the first shape in the list)
				ConnectShapes(sourceShapes.First(), source.OutputIndex, shape);
			}
		}

		public void ConnectShapes(FilterSetupShapeBase source, int sourceOutputIndex, FilterSetupShapeBase destination, bool removeExistingSource = true)
		{
			DataFlowConnectionLine line = (DataFlowConnectionLine)project.ShapeTypes["DataFlowConnectionLine"].CreateInstance();
			diagramDisplay.InsertShape(line);
			diagramDisplay.Diagram.Shapes.SetZOrder(line, 100);
			line.EndCapStyle = project.Design.CapStyles.ClosedArrow;
			line.SecurityDomainName = SECURITY_DOMAIN_FIXED_SHAPE_NO_CONNECTIONS_DELETABLE;

			if (removeExistingSource) {
				IEnumerable<ShapeConnectionInfo> connectionInfos = destination.GetConnectionInfos(destination.GetControlPointIdForInput(0), null);
				foreach (ShapeConnectionInfo ci in connectionInfos) {
					if (!ci.IsEmpty && ci.OtherShape is DataFlowConnectionLine) {
						diagramDisplay.DeleteShape(ci.OtherShape, false);
					}
				}
			}

			line.SourceDataFlowComponentReference = new DataFlowComponentReference(source.DataFlowComponent, sourceOutputIndex);
			line.DestinationDataComponent = destination.DataFlowComponent;
			line.Connect(ControlPointId.FirstVertex, source, source.GetControlPointIdForOutput(sourceOutputIndex));
			line.Connect(ControlPointId.LastVertex, destination, destination.GetControlPointIdForInput(0));
		}

		private void _RelayoutAllShapes()
		{
			// take off some pixels for natural spacing and widths of the shapes (since their X/Y coords are centred)
			int width = diagramDisplay.Width - 300;

			_channelsXPosition = 0;
			_controllersXPosition = width;

			_ResizeAndPositionChannelShapes(_channelsXPosition);
			_ResizeAndPositionFilterShapes(_previousDiagramWidth, width);
			_ResizeAndPositionControllerShapes(_controllersXPosition);
		}

		private void _ResizeAndPositionChannelShapes(int xLocation)
		{
			int y = SHAPE_Y_TOP;
			foreach (ChannelNodeShape channelShape in _channelShapes) {
				_ResizeAndPositionNestingShape(channelShape, SHAPE_CHANNELS_WIDTH, xLocation, y, true);
				y += channelShape.Height + SHAPE_VERTICAL_SPACING;
			}
		}

		private void _ResizeAndPositionControllerShapes(int xLocation)
		{
			int y = SHAPE_Y_TOP;
			foreach (ControllerShape controllerShape in _controllerShapes) {
				_ResizeAndPositionNestingShape(controllerShape, SHAPE_CONTROLLERS_WIDTH, xLocation, y, true);
				y += controllerShape.Height + SHAPE_VERTICAL_SPACING;
			}
		}

		private void _UpdateFilterPositionDataForFilter(FilterShape filterShape)
		{
			FilterSetupFormShapePosition position = new FilterSetupFormShapePosition();
			position.xPositionProportion = (double)filterShape.X / _previousDiagramWidth;
			position.yPosition = filterShape.Y;

			_applicationData.FilterSetupFormShapePositions[filterShape.FilterInstance.InstanceId] = position;
		}

		private void _ResizeAndPositionFilterShapes(int oldDiagramWidth, int newDiagramWidth)
		{
			double defaultXPositionProportion = 0.5;
			int y = SHAPE_Y_TOP;

			foreach (FilterShape filterShape in _filterShapes) {
				filterShape.Width = SHAPE_FILTERS_WIDTH;
				filterShape.Height = SHAPE_FILTERS_HEIGHT;

				FilterSetupFormShapePosition position = null;
				bool updatePosition = true;

				// if the filtershape is unpositioned, it's either first load (nothing positioned yet), or maybe new?
				// look it up in the application data for a position, otherwise default it.
				// (same if there's no old diagram width given; we'd have nothing to proportion from!)
				if ((filterShape.X <= 0 && filterShape.Y <= 0) || oldDiagramWidth <= 0) {
					if (_applicationData.FilterSetupFormShapePositions.TryGetValue(filterShape.FilterInstance.InstanceId, out position)) {
						updatePosition = false;
					} else {
						position = new FilterSetupFormShapePosition();
						position.xPositionProportion = defaultXPositionProportion;
						position.yPosition = y;

						// because we've used a default position, increment the default Y position
						y += filterShape.Height + SHAPE_VERTICAL_SPACING;
					}
				} else {
					position = new FilterSetupFormShapePosition();
					position.xPositionProportion = (double)filterShape.X / oldDiagramWidth;
					position.yPosition = filterShape.Y;
				}

				filterShape.X = (int)(newDiagramWidth * position.xPositionProportion);
				filterShape.Y = position.yPosition;

				if (updatePosition) {
					_applicationData.FilterSetupFormShapePositions[filterShape.FilterInstance.InstanceId] = position;
				}
			}

			_previousDiagramWidth = newDiagramWidth;
		}

		private void _ResizeAndPositionNestingShape(FilterSetupShapeBase shape, int width, int x, int y, bool visible)
		{
			if (visible) {
				_ShowShape(shape);
			} else {
				_HideShape(shape);
			}

			if (visible && (shape is NestingSetupShape) && (shape as NestingSetupShape).Expanded &&
				(shape as NestingSetupShape).ChildFilterShapes.Count > 0)
			{
				int curY = y + SHAPE_GROUP_HEADER_HEIGHT;
				foreach (FilterSetupShapeBase childShape in (shape as NestingSetupShape).ChildFilterShapes) {
					_ResizeAndPositionNestingShape(childShape, width - SHAPE_CHILD_WIDTH_REDUCTION, x, curY, true);
					curY += childShape.Height + SHAPE_VERTICAL_SPACING;
				}
				shape.Width = width;
				shape.Height = (curY - SHAPE_VERTICAL_SPACING + SHAPE_GROUP_FOOTER_HEIGHT) - y;
			} else {
				shape.Width = width;
				shape.Height = SHAPE_CHANNELS_HEIGHT;
				if (shape is NestingSetupShape) {
					foreach (FilterSetupShapeBase childShape in (shape as NestingSetupShape).ChildFilterShapes) {
						_ResizeAndPositionNestingShape(childShape, width, x, y, false);
					}
				}
			}
			shape.X = x;
			shape.Y = y + shape.Height / 2;
		}


		private ChannelNodeShape _MakeChannelNodeShape(ChannelNode node, int zOrder)
		{
			ChannelNodeShape shape = (ChannelNodeShape) project.ShapeTypes["ChannelNodeShape"].CreateInstance();
			shape.SetChannelNode(node);
			shape.Title = node.Name;
			diagramDisplay.InsertShape(shape);
			diagramDisplay.Diagram.Shapes.SetZOrder(shape, zOrder);
			diagramDisplay.Diagram.AddShapeToLayers(shape, _visibleLayer.Id);

			if (!_channelNodeToChannelShapes.ContainsKey(node))
				_channelNodeToChannelShapes[node] = new List<ChannelNodeShape>();
			_channelNodeToChannelShapes[node].Add(shape);

			if (shape.DataFlowComponent != null) {
				if (!_dataFlowComponentToShapes.ContainsKey(shape.DataFlowComponent))
					_dataFlowComponentToShapes[shape.DataFlowComponent] = new List<FilterSetupShapeBase>();
				_dataFlowComponentToShapes[shape.DataFlowComponent].Add(shape);
			}

			if (node.Children.Count() > 0) {
				foreach (var child in node.Children) {
					FilterSetupShapeBase childSetupShapeBase = _MakeChannelNodeShape(child, zOrder + 1);
					shape.ChildFilterShapes.Add(childSetupShapeBase);
				}
				shape.SecurityDomainName = SECURITY_DOMAIN_FIXED_SHAPE_NO_CONNECTIONS;
				shape.FillStyle = project.Design.FillStyles["ChannelGroup"];
			} else {
				shape.SecurityDomainName = SECURITY_DOMAIN_FIXED_SHAPE_WITH_CONNECTIONS;
				shape.FillStyle = project.Design.FillStyles["ChannelLeaf"];
			}
			return shape;
		}

		private ControllerShape _MakeControllerShape(IOutputDevice controller)
		{
			// TODO: deal with other controller types (smart controllers)
			OutputController outputController = controller as OutputController;
			if (outputController == null)
				return null;

			ControllerShape controllerShape = (ControllerShape)project.ShapeTypes["ControllerShape"].CreateInstance();
			controllerShape.Title = controller.Name;
			controllerShape.SecurityDomainName = SECURITY_DOMAIN_FIXED_SHAPE_NO_CONNECTIONS;
			controllerShape.FillStyle = project.Design.FillStyles["Controller"];

			diagramDisplay.InsertShape(controllerShape);
			diagramDisplay.Diagram.Shapes.SetZOrder(controllerShape, 1);
			diagramDisplay.Diagram.AddShapeToLayers(controllerShape, _visibleLayer.Id);

			if (controllerShape.DataFlowComponent != null) {
				if (!_dataFlowComponentToShapes.ContainsKey(controllerShape.DataFlowComponent))
					_dataFlowComponentToShapes[controllerShape.DataFlowComponent] = new List<FilterSetupShapeBase>();
				_dataFlowComponentToShapes[controllerShape.DataFlowComponent].Add(controllerShape);
			}

			if (_controllerToControllerShape.ContainsKey(outputController))
				throw new Exception("controller->shape map already has an entry when it shouldn't");
			_controllerToControllerShape[outputController] = controllerShape;

			for (int i = 0; i < outputController.OutputCount; i++) {
				CommandOutput output = outputController.Outputs[i];
				OutputShape outputShape = (OutputShape)project.ShapeTypes["OutputShape"].CreateInstance();
				outputShape.SetController(outputController);
				outputShape.SetOutput(output);
				outputShape.SecurityDomainName = SECURITY_DOMAIN_FIXED_SHAPE_WITH_CONNECTIONS;
				outputShape.FillStyle = project.Design.FillStyles["Output"];

				if (output.Name.Length <= 0)
					outputShape.Title = outputController.Name + " [" + (i + 1) + "]";
				else
					outputShape.Title = output.Name;

				diagramDisplay.InsertShape(outputShape);
				diagramDisplay.Diagram.Shapes.SetZOrder(outputShape, 2);
				diagramDisplay.Diagram.AddShapeToLayers(outputShape, _visibleLayer.Id);

				controllerShape.ChildFilterShapes.Add(outputShape);
			}

			return controllerShape;
		}

		private FilterShape _MakeFilterShape(IOutputFilterModuleInstance filter)
		{
			FilterShape filterShape = (FilterShape)project.ShapeTypes["FilterShape"].CreateInstance();
			filterShape.Title = filter.Descriptor.TypeName;
			filterShape.SecurityDomainName = SECURITY_DOMAIN_MOVABLE_SHAPE_WITH_CONNECTIONS;
			filterShape.FillStyle = project.Design.FillStyles["Filter"];
			filterShape.SetFilterInstance(filter);

			diagramDisplay.InsertShape(filterShape);
			diagramDisplay.Diagram.Shapes.SetZOrder(filterShape, 10);  // Z Order of 10; should be above other channels/outputs, but under lines
			diagramDisplay.Diagram.AddShapeToLayers(filterShape, _visibleLayer.Id);

			if (filterShape.DataFlowComponent != null) {
				if (!_dataFlowComponentToShapes.ContainsKey(filterShape.DataFlowComponent))
					_dataFlowComponentToShapes[filterShape.DataFlowComponent] = new List<FilterSetupShapeBase>();
				_dataFlowComponentToShapes[filterShape.DataFlowComponent].Add(filterShape);
			}

			if (_filterToFilterShape.ContainsKey(filter))
				throw new Exception("filter->shape map already has an entry when it shouldn't");
			_filterToFilterShape[filter] = filterShape;

			return filterShape;
		}

		private FilterShape _CreateNewFilterInstanceAndShape(Guid filterTypeId, bool defaultLayout)
		{
			IOutputFilterModuleInstance moduleInstance = ApplicationServices.Get<IOutputFilterModuleInstance>(filterTypeId);
			VixenSystem.Filters.AddFilter(moduleInstance);
			FilterShape shape = _CreateShapeFromFilter(moduleInstance);

			shape.Width = SHAPE_FILTERS_WIDTH;
			shape.Height = SHAPE_FILTERS_HEIGHT;

			if (defaultLayout) {
				shape.X = (diagramDisplay.Width / 2) - diagramDisplay.GetDiagramPosition().X;
				shape.Y = diagramDisplay.GetDiagramOffset().Y + (diagramDisplay.Height / 2);
			}

			return shape;
		}



		private void _RemoveShape(Shape shape)
		{
			diagramDisplay.DeleteShape(shape);
			if (shape is NestingSetupShape) {
				foreach (FilterSetupShapeBase child in (shape as NestingSetupShape).ChildFilterShapes) {
					_RemoveShape(child);
				}
			}
		}


		private void _HideShape(FilterSetupShapeBase setupShapeBase)
		{
			diagramDisplay.Diagram.AddShapeToLayers(setupShapeBase, _hiddenLayer.Id);
			diagramDisplay.Diagram.RemoveShapeFromLayers(setupShapeBase, _visibleLayer.Id);
		}

		private void _ShowShape(FilterSetupShapeBase setupShapeBase)
		{
			diagramDisplay.Diagram.AddShapeToLayers(setupShapeBase, _visibleLayer.Id);
			diagramDisplay.Diagram.RemoveShapeFromLayers(setupShapeBase, _hiddenLayer.Id);
		}

		private void _HideShapeAndChildren(NestingSetupShape nestingShape)
		{
			_HideShape(nestingShape);
			foreach (var childFilterShape in nestingShape.ChildFilterShapes) {
				if (childFilterShape is NestingSetupShape)
					_HideShapeAndChildren((NestingSetupShape)childFilterShape);
			}
		}

		private void _ShowShapeAndChildren(NestingSetupShape nestingShape)
		{
			_ShowShape(nestingShape);
			foreach (var childFilterShape in nestingShape.ChildFilterShapes) {
				if (childFilterShape is NestingSetupShape)
					_ShowShapeAndChildren((NestingSetupShape)childFilterShape);
			}
		}

		// with size, we're aiming for a 'default' of 800 pixels, total. To get that, we have (from left to right):
		//
		//  1:  20 pixels (-20 ->   0): forced display border
		//  2: 160 pixels (  0 -> 160): channel shapes (centered on 80)
		//  3:  10 pixels (160 -> 170): spacing between channels and filters
		//  4: 420 pixels (170 -> 590): filters (aiming for 2x shape widths, if possible)
		//  5:  10 pixels (590 -> 610): spacing between filters and outputs
		//  6: 160 pixels (610 -> 770): output shapes (centered on 680)
		//  7:  20 pixels (770 -> 780): forced display border
		//
		// (there's 20 pixels forced bordering by the diagram display control, as a const (scrollAreaMargin) inside it.)

		// the (base) width of all shapes (inner children will be smaller)
		internal const int SHAPE_CHANNELS_WIDTH = 160;
		internal const int SHAPE_CONTROLLERS_WIDTH = 160;
		internal const int SHAPE_FILTERS_WIDTH = 180;

		// the starting top of all shapes
		internal const int SHAPE_Y_TOP = 10;

		// the default height of all shapes
		internal const int SHAPE_CHANNELS_HEIGHT = 32;
		internal const int SHAPE_CONTROLLERS_HEIGHT = 32;
		internal const int SHAPE_FILTERS_HEIGHT = 40;

		// the vertical spacing between channels
		internal const int SHAPE_VERTICAL_SPACING = 10;

		// how much the width of inner children is reduced
		internal const int SHAPE_CHILD_WIDTH_REDUCTION = 16;

		// how much of a parent shape should be reserved/kept for the wrapping above/below
		internal const int SHAPE_GROUP_HEADER_HEIGHT = 32;
		internal const int SHAPE_GROUP_FOOTER_HEIGHT = 8;

		// security domains for different shape types
		internal const char SECURITY_DOMAIN_FIXED_SHAPE_NO_CONNECTIONS = 'A';
		internal const char SECURITY_DOMAIN_FIXED_SHAPE_WITH_CONNECTIONS = 'B';
		internal const char SECURITY_DOMAIN_MOVABLE_SHAPE_WITH_CONNECTIONS = 'C';
		internal const char SECURITY_DOMAIN_FIXED_SHAPE_NO_CONNECTIONS_DELETABLE = 'D';

	}
}
