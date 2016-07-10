using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Resources;
using Common.Resources.Properties;
using Dataweb.NShape;
using Dataweb.NShape.Advanced;
using Dataweb.NShape.Controllers;
using Vixen.Data.Flow;
using Vixen.Module;
using Vixen.Module.OutputFilter;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Sys.Output;
using VixenApplication.FiltersAndPatching;

namespace VixenApplication.Setup
{
	public partial class SetupPatchingGraphical : UserControl, ISetupPatchingControl
	{
		//Logger Class
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		// map of data types, to the shape(s) that represent them. There should only be (potentially) multiple
		// shapes to represent a given element node; this is because a node can be in multiple groups, and may
		// be displayed multiple times.
		private Dictionary<ElementNode, List<ElementNodeShape>> _elementNodeToElementShapes;
		private Dictionary<IOutputDevice, ControllerShape> _controllerToControllerShape;
		private Dictionary<IOutputFilterModuleInstance, FilterShape> _filterToFilterShape;

		// a map of data flow component to the shape(s) that represent it on the diagram. (May be many; eg. element in multiple groups)
		private Dictionary<IDataFlowComponent, List<FilterSetupShapeBase>> _dataFlowComponentToShapes;
		// a map of data flow component to filters that are children/destinations of it. (We only care about filters for now)
		private Dictionary<IDataFlowComponent, List<FilterSetupShapeBase>> _dataFlowComponentToChildFilterShapes;
		// a map of elements to the number of layers of filters they have under them
		private Dictionary<IDataFlowComponent, int> _elementDataFlowComponentsToMaxFilterDepth;
		// a map of filter data flow components to the ancestral element DFC source (if any) -- to quickly know what 'chain' a filter is in
		private Dictionary<IDataFlowComponent, IDataFlowComponent> _filterDataFlowComponentsToSourceElementDataFlowComponents;

		// list of all (root) shapes, in the order they should appear. (Child shapes for elements and
		// controllers are not in the list; they are of type NestingShape and handle their own bits.)
		private List<ElementNodeShape> _elementShapes;
		private List<ControllerShape> _controllerShapes;
		private List<FilterShape> _filterShapes;
		private List<OutputShape> _outputShapes;

		private readonly Layer _visibleLayer;
		private readonly Layer _hiddenLayer;

		private List<FilterShape> _filterShapeClipboard;



		// security domains for different shape types
		internal const char SECURITY_DOMAIN_FIXED_SHAPE_NO_CONNECTIONS = 'A';
		internal const char SECURITY_DOMAIN_FIXED_SHAPE_WITH_CONNECTIONS = 'B';
		internal const char SECURITY_DOMAIN_MOVABLE_SHAPE_WITH_CONNECTIONS = 'C';
		internal const char SECURITY_DOMAIN_FIXED_SHAPE_NO_CONNECTIONS_DELETABLE = 'D';
		internal const char SECURITY_DOMAIN_ALL_PERMISSIONS = 'E';





		public SetupPatchingGraphical()
		{
			InitializeComponent();
			int iconSize = (int)(24 * ScalingTools.GetScaleFactor());
			buttonAddFilter.Image = Tools.GetIcon(Resources.add, iconSize);
			buttonAddFilter.Text = "";
			buttonDeleteFilter.Image = Tools.GetIcon(Resources.delete, iconSize);
			buttonDeleteFilter.Text = "";
			buttonZoomIn.Image = Tools.GetIcon(Resources.zoom_in, iconSize);
			buttonZoomIn.Text = "";
			buttonZoomOut.Image = Tools.GetIcon(Resources.zoom_out, iconSize);
			buttonZoomOut.Text = "";
			buttonZoomFit.Image = Tools.GetIcon(Resources.zoom_fit, iconSize);
			buttonZoomFit.Text = "";
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			diagramDisplay.BackColorGradient = ThemeColorTable.TextBoxBackgroundColor;
			diagramDisplay.BackColor = ThemeColorTable.TextBoxBackgroundColor;

			project.LibrarySearchPaths.Add(@"Common\");
			project.AutoLoadLibraries = true;
			project.AddLibraryByName("VixenApplication", false);

			project.Name = "filterProject";
			project.Create();

			_visibleLayer = new Layer("Visible");
			_hiddenLayer = new Layer("Hidden");
			_elementNodeToElementShapes = new Dictionary<ElementNode, List<ElementNodeShape>>();
			_controllerToControllerShape = new Dictionary<IOutputDevice, ControllerShape>();
			_filterToFilterShape = new Dictionary<IOutputFilterModuleInstance, FilterShape>();

			_dataFlowComponentToShapes = new Dictionary<IDataFlowComponent, List<FilterSetupShapeBase>>();
			_dataFlowComponentToChildFilterShapes = new Dictionary<IDataFlowComponent, List<FilterSetupShapeBase>>();
			_elementDataFlowComponentsToMaxFilterDepth = new Dictionary<IDataFlowComponent, int>();
			_filterDataFlowComponentsToSourceElementDataFlowComponents = new Dictionary<IDataFlowComponent, IDataFlowComponent>();

			_elementShapes = new List<ElementNodeShape>();
			_controllerShapes = new List<ControllerShape>();
			_filterShapes = new List<FilterShape>();
			_outputShapes = new List<OutputShape>();

		}


		private void SetupPatchingGraphical_Load(object sender, EventArgs e)
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
			((RoleBasedSecurityManager) diagramDisplay.Project.SecurityManager).SetPermissions(
				SECURITY_DOMAIN_FIXED_SHAPE_NO_CONNECTIONS, StandardRole.Operator, Permission.Insert);
			// B: fixed shapes with connection points: connect only (element nodes (leaf), output shapes)
			((RoleBasedSecurityManager) diagramDisplay.Project.SecurityManager).SetPermissions(
				SECURITY_DOMAIN_FIXED_SHAPE_WITH_CONNECTIONS, StandardRole.Operator, Permission.Connect | Permission.Insert);
			// C: movable shapes: connect, layout (movable), and deleteable (filters, patch lines)
			((RoleBasedSecurityManager) diagramDisplay.Project.SecurityManager).SetPermissions(
				SECURITY_DOMAIN_MOVABLE_SHAPE_WITH_CONNECTIONS, StandardRole.Operator,
				Permission.Connect | Permission.Insert | Permission.Layout | Permission.Delete);
			// D: fixed shapes with no connection points, but deletable: only for established patch lines (so the user can't move them again, but an still delete them)
			((RoleBasedSecurityManager) diagramDisplay.Project.SecurityManager).SetPermissions(
				SECURITY_DOMAIN_FIXED_SHAPE_NO_CONNECTIONS_DELETABLE, StandardRole.Operator, Permission.Insert | Permission.Delete);
			// E: all permissions
			((RoleBasedSecurityManager) diagramDisplay.Project.SecurityManager).SetPermissions(
				SECURITY_DOMAIN_ALL_PERMISSIONS, StandardRole.Operator, Permission.All);

			((RoleBasedSecurityManager) diagramDisplay.Project.SecurityManager).SetPermissions(StandardRole.Operator,
			                                                                                   Permission.All);
			((RoleBasedSecurityManager) diagramDisplay.Project.SecurityManager).CurrentRole = StandardRole.Operator;

			FillStyle styleElementGroup = new FillStyle("ElementGroup",
			                                            new ColorStyle(string.Empty, Color.FromArgb(120, 160, 240)),
			                                            new ColorStyle(string.Empty, Color.FromArgb(90, 120, 180)));
			styleElementGroup.FillMode = FillMode.Gradient;
			FillStyle styleElementLeaf = new FillStyle("ElementLeaf",
			                                           new ColorStyle(string.Empty, Color.FromArgb(200, 220, 255)),
			                                           new ColorStyle(string.Empty, Color.FromArgb(140, 160, 200)));
			styleElementLeaf.FillMode = FillMode.Gradient;
			FillStyle styleFilter = new FillStyle("Filter",
			                                      new ColorStyle(string.Empty, Color.FromArgb(255, 220, 150)),
			                                      new ColorStyle(string.Empty, Color.FromArgb(230, 200, 100)));
			styleFilter.FillMode = FillMode.Gradient;
			FillStyle styleController = new FillStyle("Controller",
			                                          new ColorStyle(string.Empty, Color.FromArgb(100, 200, 100)),
			                                          new ColorStyle(string.Empty, Color.FromArgb(50, 200, 50)));
			styleController.FillMode = FillMode.Gradient;
			FillStyle styleOutput = new FillStyle("Output",
			                                      new ColorStyle(string.Empty, Color.FromArgb(180, 230, 180)),
			                                      new ColorStyle(string.Empty, Color.FromArgb(120, 210, 120)));
			styleOutput.FillMode = FillMode.Gradient;

			project.Design.FillStyles.Add(styleElementGroup, styleElementGroup);
			project.Design.FillStyles.Add(styleElementLeaf, styleElementLeaf);
			project.Design.FillStyles.Add(styleFilter, styleFilter);
			project.Design.FillStyles.Add(styleController, styleController);
			project.Design.FillStyles.Add(styleOutput, styleOutput);

			ConnectionTool tool = new ConnectionTool();
			tool.DataFlowModificationMade += tool_DataFlowModificationMade;
			diagramDisplay.CurrentTool = tool;
		}


		private void tool_DataFlowModificationMade(object sender, EventArgs e)
		{
			OnPatchingUpdated();
		}




		private void InitializeAllShapes(IEnumerable<ElementNode> nodes, IEnumerable<IOutputFilterModuleInstance> filters,
		                                 IEnumerable<IOutputDevice> controllers)
		{
			diagramDisplay.DoSuspendUpdate();

			foreach (ElementNodeShape shape in _elementShapes) {
				_RemoveShapeFromDiagram(shape, false);
			}
			foreach (FilterShape shape in _filterShapes) {
				_RemoveShapeFromDiagram(shape, false);
			}
			foreach (ControllerShape shape in _controllerShapes) {
				_RemoveShapeFromDiagram(shape, false);
			}

			diagramDisplay.SelectedShapes.Clear();

			_dataFlowComponentToShapes = new Dictionary<IDataFlowComponent, List<FilterSetupShapeBase>>();
			_dataFlowComponentToChildFilterShapes = new Dictionary<IDataFlowComponent, List<FilterSetupShapeBase>>();
			_elementDataFlowComponentsToMaxFilterDepth = new Dictionary<IDataFlowComponent, int>();
			_filterDataFlowComponentsToSourceElementDataFlowComponents = new Dictionary<IDataFlowComponent, IDataFlowComponent>();

			_InitializeShapesFromElements(nodes);
			_InitializeFilterShapesFromFilters(filters);
			_InitializeControllerShapesFromControllers(controllers);

			_RelayoutAllShapes();
			//_CreateConnectionsFromExistingLinks();
			UpdateConnections();

			diagramDisplay.DoResumeUpdate();
		}



		private void _addShapeToDataFlowMap(FilterSetupShapeBase shape, Dictionary<IDataFlowComponent, List<FilterSetupShapeBase>> map)
		{
			if (shape.DataFlowComponent != null) {
				if (!map.ContainsKey(shape.DataFlowComponent))
					map[shape.DataFlowComponent] = new List<FilterSetupShapeBase>();

				if (!map[shape.DataFlowComponent].Contains(shape))
					map[shape.DataFlowComponent].Add(shape);
			}
		}

		private IDataFlowComponent _getDataFlowComponentSourceFromShape(FilterSetupShapeBase shape)
		{
			IDataFlowComponent source = null;
			if (shape.DataFlowComponent != null &&
				shape.DataFlowComponent.Source != null &&
				shape.DataFlowComponent.Source.Component != null) {
					source = shape.DataFlowComponent.Source.Component;
			}

			return source;
		}





		#region Element shape creation

		private void _InitializeShapesFromElements(IEnumerable<ElementNode> nodes)
		{
			diagramDisplay.DoSuspendUpdate();

			_elementShapes = new List<ElementNodeShape>();
			_elementNodeToElementShapes = new Dictionary<ElementNode, List<ElementNodeShape>>();

			_elementDataFlowComponentsToMaxFilterDepth = new Dictionary<IDataFlowComponent, int>();

			foreach (ElementNode node in nodes) {
				_CreateShapeFromElement(node);
			}

			diagramDisplay.DoResumeUpdate();
		}

		private void _UpdateElementShapesFromElements(IEnumerable<ElementNode> nodes)
		{
			diagramDisplay.DoSuspendUpdate();

			// to try and update the displayed elements with new elements, we need to:
			// 1. add any new incoming elements that don't already exist
			// 2. remove any existing element shapes that aren't in the incoming list

			List<ElementNodeShape> oldElementShapes = _elementShapes;
			Dictionary<ElementNode, List<ElementNodeShape>> oldElementNodeToElementShapes = _elementNodeToElementShapes;

			_elementShapes = new List<ElementNodeShape>();
			_elementNodeToElementShapes = new Dictionary<ElementNode, List<ElementNodeShape>>();

			foreach (ElementNode node in nodes) {
				// if all parents of this node are in the incoming set of nodes that will be displayed as roots,
				// don't display it: it will be displayed in the other group(s) as children
				if (_allNodeParentsAreInSet(node, nodes)) {
					continue;
				}

				// if it's currently in the root shapes list, don't make a new shape for it
				List<ElementNodeShape> matchingRootNodes = oldElementShapes.Where(x => x.Node == node).ToList();
				if (matchingRootNodes.Count() == 0) {
					_CreateShapeFromElement(node);
				}
				else {
					// otherwise just reuse the existing shape; remap the existing shape to the new lists/maps
					if (matchingRootNodes.Count() > 1) {
						Logging.Warn("new element '" + node.Name + "' is in existing root nodes more than once");
					}
					_addElementShapeToList(matchingRootNodes.First(), _elementShapes);
					_addElementShapeToNodeMap(matchingRootNodes.First(), _elementNodeToElementShapes);
					_addShapeToDataFlowMap(matchingRootNodes.First(), _dataFlowComponentToShapes);
				}
			}

			// compare the new lists to the old ones, and delete any that aren't persisted in the new lists
			foreach (ElementNodeShape elementNodeShape in oldElementShapes.Except(_elementShapes).ToArray()) {
				_RemoveShapeFromDiagram(elementNodeShape, false);
			}

			diagramDisplay.DoResumeUpdate();
		}

		private bool _allNodeParentsAreInSet(ElementNode node, IEnumerable<ElementNode> nodes)
		{
			List<ElementNode> nodesList = nodes.ToList();
			List<ElementNode> parentList = node.Parents.ToList();
			return (parentList.Any() && parentList.All(x => nodesList.Contains(x) || _allNodeParentsAreInSet(x, nodesList)));
		}

		private void _addElementShapeToList(ElementNodeShape ens, List<ElementNodeShape> list)
		{
			if (ens != null)
				list.Add(ens);
		}

		private void _addElementShapeToNodeMap(ElementNodeShape ens, Dictionary<ElementNode, List<ElementNodeShape>> map)
		{
			if (!map.ContainsKey(ens.Node))
				map[ens.Node] = new List<ElementNodeShape>();
			map[ens.Node].Add(ens);
		}


		private ElementNodeShape _CreateShapeFromElement(ElementNode node)
		{
			ElementNodeShape elementShape = _MakeElementNodeShape(node, 1);
			_addElementShapeToList(elementShape, _elementShapes);
			return elementShape;
		}

		private ElementNodeShape _MakeElementNodeShape(ElementNode node, int zOrder)
		{
			ElementNodeShape shape = (ElementNodeShape) project.ShapeTypes["ElementNodeShape"].CreateInstance();
			shape.SetElementNode(node);
			shape.Title = node.Name;
			shape.HeaderHeight = SHAPE_GROUP_HEADER_HEIGHT;
			diagramDisplay.InsertShape(shape);
			diagramDisplay.Diagram.Shapes.SetZOrder(shape, zOrder);
			diagramDisplay.Diagram.AddShapeToLayers(shape, _visibleLayer.Id);

			_addElementShapeToNodeMap(shape, _elementNodeToElementShapes);
			_addShapeToDataFlowMap(shape, _dataFlowComponentToShapes);

			if (node.Children.Count() > 0) {
				foreach (var child in node.Children) {
					FilterSetupShapeBase childSetupShapeBase = _MakeElementNodeShape(child, zOrder + 1);
					shape.ChildFilterShapes.Add(childSetupShapeBase);
				}
				shape.SecurityDomainName = SECURITY_DOMAIN_FIXED_SHAPE_NO_CONNECTIONS;
				shape.FillStyle = project.Design.FillStyles["ElementGroup"];
			}
			else {
				shape.SecurityDomainName = SECURITY_DOMAIN_FIXED_SHAPE_WITH_CONNECTIONS;
				shape.FillStyle = project.Design.FillStyles["ElementLeaf"];
			}
			return shape;
		}

		#endregion





		private IEnumerable<IOutputFilterModuleInstance> _findFiltersThatDescendFromElements(IEnumerable<ElementNode> elements)
		{
			// this is assuming that the elements given are actual leaf/output elements (ie. have a Element object, and
			// will be patched to stuff).  If you only have group element nodes, iterate to the leaf nodes first.
			return elements
				.Where(x => x.Element != null)
				.SelectMany(x => _findComponentsOfTypeInTreeFromComponent(VixenSystem.DataFlow.GetComponent(x.Element.Id), typeof(IOutputFilterModuleInstance)))
				.Cast<IOutputFilterModuleInstance>();
		}

		private IEnumerable<IDataFlowComponent> _findComponentsOfTypeInTreeFromComponent(IDataFlowComponent dataFlowComponent, Type dfctype)
		{
			return VixenSystem.DataFlow.GetDestinationsOfComponent(dataFlowComponent)
				.SelectMany(x => _findComponentsOfTypeInTreeFromComponent(x, dfctype))
				.Concat(new[] {dataFlowComponent})
				.Where(dfc => dfctype.IsAssignableFrom(dfc.GetType()))
				;
		}





		#region Filter shape creation

		private void _InitializeFilterShapesFromFilters(IEnumerable<IOutputFilterModuleInstance> filters)
		{
			diagramDisplay.DoSuspendUpdate();

			_filterShapes = new List<FilterShape>();
			_filterToFilterShape = new Dictionary<IOutputFilterModuleInstance, FilterShape>();

			_filterDataFlowComponentsToSourceElementDataFlowComponents = new Dictionary<IDataFlowComponent, IDataFlowComponent>();
			_dataFlowComponentToChildFilterShapes = new Dictionary<IDataFlowComponent, List<FilterSetupShapeBase>>();

			foreach (IOutputFilterModuleInstance filter in filters) {
				_CreateShapeFromFilter(filter);
			}

			diagramDisplay.DoResumeUpdate();
		}

		private void _UpdateFilterShapesFromFilters(IEnumerable<IOutputFilterModuleInstance> filters)
		{
			diagramDisplay.DoSuspendUpdate();

			List<FilterShape> oldFilterShapes = _filterShapes;
			Dictionary<IOutputFilterModuleInstance, FilterShape> oldFilterToFilterShape = _filterToFilterShape;
			Dictionary<IDataFlowComponent, List<FilterSetupShapeBase>> oldDataFlowComponentToChildFilterShapes = _dataFlowComponentToChildFilterShapes;

			_filterShapes = new List<FilterShape>();
			_filterToFilterShape = new Dictionary<IOutputFilterModuleInstance, FilterShape>();
			_dataFlowComponentToChildFilterShapes = new Dictionary<IDataFlowComponent, List<FilterSetupShapeBase>>();

			foreach (IOutputFilterModuleInstance filter in filters) {
				// if it's currently displayed -- ie. in the old shapes list -- don't make a new shape for it
				FilterShape filterShape = null;
				oldFilterToFilterShape.TryGetValue(filter, out filterShape);

				if (filterShape == null) {
					_CreateShapeFromFilter(filter);
				} else {
					// otherwise just reuse the existing shape; remap the existing shape to the new lists/maps
					_addFilterShapeToList(filterShape, _filterShapes);
					_addFilterShapeToFilterMap(filterShape, _filterToFilterShape);
					_addShapeToDataFlowMap(filterShape, _dataFlowComponentToShapes);
					_addFilterShapeToParentDataFlowComponentMap(filterShape, _dataFlowComponentToChildFilterShapes);
				}
			}

			// compare the new lists to the old ones, and delete any that aren't persisted in the new lists
			foreach (FilterShape filterShape in oldFilterShapes.Except(_filterShapes).ToArray()) {
				_RemoveShapeFromDiagram(filterShape, false);
			}

			diagramDisplay.DoResumeUpdate();
		}

		private void _addFilterShapeToList(FilterShape filterShape, List<FilterShape> list)
		{
			if (filterShape != null)
				list.Add(filterShape);
		}

		private void _addFilterShapeToFilterMap(FilterShape filterShape, Dictionary<IOutputFilterModuleInstance, FilterShape> map)
		{
			if (map.ContainsKey(filterShape.FilterInstance))
				Logging.Warn("Adding filter to map, but it already exists");

			map[filterShape.FilterInstance] = filterShape;
		}

		private void _addFilterShapeToParentDataFlowComponentMap(FilterShape filterShape, Dictionary<IDataFlowComponent, List<FilterSetupShapeBase>> map)
		{
			IDataFlowComponent source = _getDataFlowComponentSourceFromShape(filterShape);
			if (source != null) {
				if (!map.ContainsKey(source))
					map[source] = new List<FilterSetupShapeBase>();
				map[source].Add(filterShape);
			}
		}

		private void _calculateFilterDepthFromSource(FilterShape filterShape)
		{
			// trace this filter all the way back to the source element (if possible), and remember how 'deep' we are
			// TODO: just reviewing this code I wrote.... why the FUCK do we need to trace it back in 'shape' space? Why can't we just keep
			// TODO: following the DFC source back to an element, then look that up in the map?....
			int depth = 1;
			IDataFlowComponent source = _getDataFlowComponentSourceFromShape(filterShape);
			IDataFlowComponent elementComponent = null;
			while (true) {
				if (source == null) {
					depth = 0;
					break;
				}

				List<FilterSetupShapeBase> shapes;
				_dataFlowComponentToShapes.TryGetValue(source, out shapes);
				if (shapes != null) {
					FilterSetupShapeBase shape = shapes.FirstOrDefault();
					if (shape is ElementNodeShape) {
						elementComponent = (shape as ElementNodeShape).DataFlowComponent;
						break;
					}
				}

				if (source.Source != null) {
					source = source.Source.Component;
					depth++;
				} else {
					depth = 0;
					break;
				}
			}
			filterShape.LevelsFromElementSource = depth;
			if (elementComponent != null) {
				if (!_elementDataFlowComponentsToMaxFilterDepth.ContainsKey(elementComponent))
					_elementDataFlowComponentsToMaxFilterDepth[elementComponent] = depth;
				else if (_elementDataFlowComponentsToMaxFilterDepth[elementComponent] < depth)
					_elementDataFlowComponentsToMaxFilterDepth[elementComponent] = depth;

				if (filterShape.DataFlowComponent != null)
					_filterDataFlowComponentsToSourceElementDataFlowComponents[filterShape.DataFlowComponent] = elementComponent;
			}
		}


		private FilterShape _CreateShapeFromFilter(IOutputFilterModuleInstance filter)
		{
			FilterShape filterShape = _MakeFilterShape(filter);
			if (filterShape != null)
				_filterShapes.Add(filterShape);
			return filterShape;
		}

		private FilterShape _MakeFilterShape(IOutputFilterModuleInstance filter)
		{
			FilterShape filterShape = (FilterShape)project.ShapeTypes["FilterShape"].CreateInstance();
			filterShape.Title = filter.Descriptor.TypeName;
			filterShape.SecurityDomainName = SECURITY_DOMAIN_MOVABLE_SHAPE_WITH_CONNECTIONS;
			filterShape.FillStyle = project.Design.FillStyles["Filter"];
			filterShape.SetFilterInstance(filter);

			diagramDisplay.InsertShape(filterShape);
			diagramDisplay.Diagram.Shapes.SetZOrder(filterShape, 10);
			// Z Order of 10; should be above other elements/outputs, but under lines
			diagramDisplay.Diagram.AddShapeToLayers(filterShape, _visibleLayer.Id);

			_addShapeToDataFlowMap(filterShape, _dataFlowComponentToShapes);
			_addFilterShapeToFilterMap(filterShape, _filterToFilterShape);
			_addFilterShapeToParentDataFlowComponentMap(filterShape, _dataFlowComponentToChildFilterShapes);

			_calculateFilterDepthFromSource(filterShape);

			return filterShape;
		}

#endregion








#region Controller shape creation

		private void _InitializeControllerShapesFromControllers(IEnumerable<IOutputDevice> controllers)
		{
			ControllersAndOutputsSet data = new ControllersAndOutputsSet();

			foreach (IControllerDevice controller in controllers) {
				HashSet<int> outputs = new HashSet<int>();
				for (int i = 0; i < controller.OutputCount; i++) {
					outputs .Add(i);
				}
				data[controller] = outputs;
			}

			_InitializeControllerShapesFromControllers(data);
		}

		private void _InitializeControllerShapesFromControllers(ControllersAndOutputsSet controllersAndOutputs)
		{
			diagramDisplay.DoSuspendUpdate();

			_controllerShapes = new List<ControllerShape>();
			_controllerToControllerShape = new Dictionary<IOutputDevice, ControllerShape>();
			_outputShapes = new List<OutputShape>();

			foreach (KeyValuePair<IControllerDevice, HashSet<int>> pair in controllersAndOutputs) {
				_CreateShapeFromController(pair.Key, pair.Value);
			}

			diagramDisplay.DoResumeUpdate();
		}

		private void _updateControllerShapesFromControllersAndOutputs(ControllersAndOutputsSet controllersAndOutputs)
		{
			if (controllersAndOutputs == null)
				return;

			diagramDisplay.DoSuspendUpdate();

			List<ControllerShape> oldControllerShapes = _controllerShapes;
			Dictionary<IOutputDevice, ControllerShape> oldControllerToControllerShape = _controllerToControllerShape;
			List<OutputShape> oldOutputShapes = _outputShapes;

			_controllerShapes = new List<ControllerShape>();
			_controllerToControllerShape = new Dictionary<IOutputDevice, ControllerShape>();
			_outputShapes = new List<OutputShape>();

			foreach (KeyValuePair<IControllerDevice, HashSet<int>> pair in controllersAndOutputs) {
				ControllerShape controllerShape = null;
				oldControllerToControllerShape.TryGetValue(pair.Key, out controllerShape);

				if (controllerShape == null) {
					_CreateShapeFromController(pair.Key, pair.Value);
				} else {
					_updateControllerShapeFromOutputSet(controllerShape, pair.Value);
					_addControllerShapeToList(controllerShape, _controllerShapes);
					_addControllerShapeToControllerMap(controllerShape, _controllerToControllerShape);
				}
			}

			// compare the new lists to the old ones, and delete any that aren't persisted in the new lists
			foreach (ControllerShape controllerShape in oldControllerShapes.Except(_controllerShapes).ToArray()) {
				_RemoveShapeFromDiagram(controllerShape, false);
			}

			diagramDisplay.DoResumeUpdate();
		}


		private void _updateControllerShapeFromOutputSet(ControllerShape controllerShape, HashSet<int> outputs)
		{
			List<int> sortedOutputs = new List<int>(outputs);
			sortedOutputs.Sort();

			foreach (int outputIndex in sortedOutputs) {
				OutputShape oldShape = controllerShape.ChildFilterShapes.Cast<OutputShape>()
					.Where(x => x.Controller == controllerShape.Controller && x.OutputIndex == outputIndex).FirstOrDefault();

				if (oldShape != null) {
					_addOutputShapeToList(oldShape, _outputShapes);
				} else {
					_CreateOutputShape(controllerShape, outputIndex);
				}
			}

			foreach (OutputShape outputShape in controllerShape.ChildFilterShapes.Cast<OutputShape>().Where(x => !outputs.Contains(x.OutputIndex)).ToArray()) {
				controllerShape.ChildFilterShapes.Remove(outputShape);
				_RemoveShapeFromDiagram(outputShape, false);
			}

			// sort the nested shapes of the outputs in the controller shape; the list above has them in order, but the list of shapes might not
			controllerShape.ChildFilterShapes.Sort(delegate (FilterSetupShapeBase a, FilterSetupShapeBase b)
			                                       	{
			                                       		OutputShape osa = a as OutputShape;
			                                       		OutputShape osb = b as OutputShape;
														if (a == null || b == null)
															return 0;
			                                       		return osa.OutputIndex.CompareTo(osb.OutputIndex);
			                                       	});
		}





		private void _addControllerShapeToList(ControllerShape controllerShape, List<ControllerShape> list)
		{
			if (controllerShape != null)
				list.Add(controllerShape);
		}

		private void _addOutputShapeToList(OutputShape outputShape, List<OutputShape> list)
		{
			if (outputShape != null)
				list.Add(outputShape);
		}

		private void _addControllerShapeToControllerMap(ControllerShape controllerShape, Dictionary<IOutputDevice, ControllerShape> map)
		{
			if (controllerShape.Controller == null) {
				Logging.Error("null controller");
				return;
			}

			if (map.ContainsKey(controllerShape.Controller))
				Logging.Warn("Adding filter to map, but it already exists");

			map[controllerShape.Controller] = controllerShape;
		}


		private ControllerShape _CreateShapeFromController(IOutputDevice controller, IEnumerable<int> outputs)
		{
			ControllerShape controllerShape = _MakeControllerShape(controller, outputs);
			_addControllerShapeToList(controllerShape, _controllerShapes);
			return controllerShape;
		}

		private OutputShape _CreateOutputShape(ControllerShape controllerShape, int outputIndex)
		{
			OutputShape outputShape = _MakeOutputShape(controllerShape.Controller, outputIndex);
			_addOutputShapeToList(outputShape, _outputShapes);

			controllerShape.ChildFilterShapes.Add(outputShape);
			
			return outputShape;
		}


		private ControllerShape _MakeControllerShape(IOutputDevice controller, IEnumerable<int> outputs)
		{
			// TODO: deal with other controller types (smart controllers)
			OutputController outputController = controller as OutputController;
			if (outputController == null)
				return null;

			ControllerShape controllerShape = (ControllerShape)project.ShapeTypes["ControllerShape"].CreateInstance();
			controllerShape.Title = controller.Name;
			controllerShape.Controller = controller;
			controllerShape.SecurityDomainName = SECURITY_DOMAIN_FIXED_SHAPE_NO_CONNECTIONS;
			controllerShape.FillStyle = project.Design.FillStyles["Controller"];
			controllerShape.HeaderHeight = SHAPE_GROUP_HEADER_HEIGHT;

			diagramDisplay.InsertShape(controllerShape);
			diagramDisplay.Diagram.Shapes.SetZOrder(controllerShape, 1);
			diagramDisplay.Diagram.AddShapeToLayers(controllerShape, _visibleLayer.Id);

			_addShapeToDataFlowMap(controllerShape, _dataFlowComponentToShapes);
			_addControllerShapeToControllerMap(controllerShape, _controllerToControllerShape);

			List<int> sortedOutputs = outputs.ToList();
			sortedOutputs.Sort();

			foreach (int output in sortedOutputs) {
				_CreateOutputShape(controllerShape, output);
			}

			return controllerShape;
		}

		private OutputShape _MakeOutputShape(IOutputDevice controller, int outputIndex)
		{
			// TODO: deal with other controller types (smart controllers)
			OutputController outputController = controller as OutputController;
			if (outputController == null)
				return null;

			CommandOutput output = outputController.Outputs[outputIndex];
			OutputShape outputShape = (OutputShape) project.ShapeTypes["OutputShape"].CreateInstance();
			outputShape.SetController(outputController);
			outputShape.SetOutput(output, outputIndex);
			outputShape.SecurityDomainName = SECURITY_DOMAIN_FIXED_SHAPE_WITH_CONNECTIONS;
			outputShape.FillStyle = project.Design.FillStyles["Output"];

			if (output.Name.Length <= 0)
				outputShape.Title = string.Format("{0} [{1}]", outputController.Name, (outputIndex + 1));
			else
				outputShape.Title = output.Name;

			diagramDisplay.InsertShape(outputShape);
			diagramDisplay.Diagram.Shapes.SetZOrder(outputShape, 2);
			diagramDisplay.Diagram.AddShapeToLayers(outputShape, _visibleLayer.Id);

			_addShapeToDataFlowMap(outputShape, _dataFlowComponentToShapes);

			return outputShape;
		}

		#endregion









#region Layout positioning of all shapes
		
		//  00 - 05% : space
		//  05 - 25% : elements
		//  25 - 35% : space
		//  35 - 65% : filters (possibly multiple columns)
		//  65 - 75% : space
		//  75 - 95% : channels
		//  95 - 100%: space

		private int totalWidth;
		private int elementWidth;
		private int elementX;
		private int elementY;
		private int filterWidth;
		private int filterX;
		private int filterY;
		private int channelWidth;
		private int channelX;
		private int channelY;

		// the starting top of all shapes
		internal const int SHAPE_Y_TOP = 10;

		// the maximum and minimum widths and heights for shapes
		internal const int SHAPE_MAX_WIDTH = 160;
		internal const int SHAPE_MIN_WIDTH = 32;
		internal const int SHAPE_DEFAULT_HEIGHT = 20;
		internal const int SHAPE_MIN_HEIGHT = 10;

		// the vertical spacing between elements
		internal const int SHAPE_VERTICAL_SPACING = 6;

		// how much the width of inner children is reduced
		internal const int SHAPE_CHILD_WIDTH_REDUCTION = 10;

		// how much of a parent shape should be reserved/kept for the wrapping above/below
		internal const int SHAPE_GROUP_HEADER_HEIGHT = 20;
		internal const int SHAPE_GROUP_FOOTER_HEIGHT = 6;





		private void _RelayoutAllShapes()
		{
			diagramDisplay.DoSuspendUpdate();

			_ResizeAndPositionElementShapes();
			_ResizeAndPositionFilterShapes();
			_ResizeAndPositionControllerShapes();

			diagramDisplay.DoResumeUpdate();
		}

		private void _ResizeAndPositionElementShapes()
		{
			diagramDisplay.DoSuspendUpdate();

			totalWidth = diagramDisplay.Width;
			elementWidth = totalWidth * 20 / 100;
			elementX = totalWidth * 5 / 100;
			elementY = SHAPE_Y_TOP;

			foreach (ElementNodeShape elementShape in _elementShapes) {
				_ResizeAndPositionNestingShape(elementShape, elementWidth, elementX, elementY, true);
				elementY += elementShape.Height + SHAPE_VERTICAL_SPACING;
			}

			diagramDisplay.DoResumeUpdate();
		}

		private void _ResizeAndPositionControllerShapes()
		{
			diagramDisplay.DoSuspendUpdate();

			totalWidth = diagramDisplay.Width;
			channelWidth = totalWidth * 20 / 100;
			channelX = totalWidth * 75 / 100;
			channelY = SHAPE_Y_TOP;

			foreach (ControllerShape controllerShape in _controllerShapes) {
				_ResizeAndPositionNestingShape(controllerShape, channelWidth, channelX, channelY, true);
				channelY += controllerShape.Height + SHAPE_VERTICAL_SPACING;
			}

			diagramDisplay.DoResumeUpdate();
		}

		private void _ResizeAndPositionFilterShapes()
		{
			// iterate through all the shapes we have to layout, and order them by 'depth' from source element (based
			// on the value in the shape).  Then we can process them in that order, to make sure dependent shapes
			// are laid out AFTER their parents (as the sizes and positions will be needed).
			List<List<FilterShape>> sortedFilterShapes = new List<List<FilterShape>>();
			foreach (FilterShape filterShape in _filterShapes) {
				int depth = filterShape.LevelsFromElementSource;
				while (sortedFilterShapes.Count <= depth)
					sortedFilterShapes.Add(new List<FilterShape>());

				sortedFilterShapes[depth].Add(filterShape);
			}

			diagramDisplay.DoSuspendUpdate();

			totalWidth = diagramDisplay.Width;
			filterWidth = totalWidth * 30 / 100;
			filterX = totalWidth * 35 / 100;
			filterY = SHAPE_Y_TOP;

			// a map of data flow component to number of shapes laid out against it; so we can count what position we're up to with the drawing
			Dictionary<IDataFlowComponent, int> shapesLaidOutPerComponent = new Dictionary<IDataFlowComponent, int>();

			// go through filters, column by column
			for (int i = 0; i < sortedFilterShapes.Count; i++) {

				// if these are filters without a known element source (ie. free-floating), draw them at the top, in order
				if (i == 0) {
					int totalShapes = sortedFilterShapes[0].Count;
					int shapesSoFar = 0;
		
					foreach (FilterShape filterShape in sortedFilterShapes[0]) {

						int shapeHeight = SHAPE_DEFAULT_HEIGHT;
						int ypos = SHAPE_Y_TOP;

						int columnWidth = filterWidth / totalShapes;
						int shapeWidth = Math.Max(Math.Min(columnWidth, SHAPE_MAX_WIDTH), SHAPE_MIN_WIDTH);
						int internalColumnOffset = (columnWidth - shapeWidth) / 2;
						int xpos = filterX + (int)(shapesSoFar * columnWidth) + internalColumnOffset;

						shapesSoFar++;

						filterShape.X = xpos;
						filterShape.Y = ypos;
						filterShape.Width = shapeWidth;
						filterShape.Height = shapeHeight;
					}
				}
				else {
					// go through all the filter shapes for this column, and draw them against their corresponding 'parent' (source)
					foreach (FilterShape filterShape in sortedFilterShapes[i]) {

						// there should always be a source, since it's been checked earlier.... find the source shape for this item
						IDataFlowComponent filter = null;
						IDataFlowComponent source = null;
						if (filterShape.DataFlowComponent != null) {
							filter = filterShape.DataFlowComponent;
						}

						if (filter == null)
							throw new Exception("null filter; this shouldn't ever happen!");

						if (filter.Source != null && filter.Source.Component != null) {
							source = filter.Source.Component;
						}

						if (source == null)
							throw new Exception("null source; this shouldn't ever happen!");

						// get the shape for the source, and list of shapes for the filters siblings
						List<FilterSetupShapeBase> sourceShapes;
						_dataFlowComponentToShapes.TryGetValue(source, out sourceShapes);

						FilterSetupShapeBase sourceShape = null;
						// not ideal; we'll have to figure out what to do when there's multiple instances of an element, say
						if (sourceShapes != null)
							sourceShape = sourceShapes.FirstOrDefault();

						List<FilterSetupShapeBase> childShapes;
						_dataFlowComponentToChildFilterShapes.TryGetValue(source, out childShapes);
						if (childShapes == null)
							childShapes = new List<FilterSetupShapeBase> {filterShape};

						// find out how many filters have been drawn against this source (if any), and figure out the y co-ord based on that
						int totalShapes = childShapes.Count;
						int shapesSoFar;
						shapesLaidOutPerComponent.TryGetValue(source, out shapesSoFar);
						// will default to '0' if not in dictionary; how convenient!

						float verticalProportionOffset = (float) shapesSoFar/totalShapes;
						int shapeHeight = SHAPE_DEFAULT_HEIGHT;
						int ypos = filterY;
						if (sourceShape != null) {
							shapeHeight = sourceShape.Height/totalShapes;
							ypos = sourceShape.Y + (int) (verticalProportionOffset*sourceShape.Height);
						}

						// let's try and get 4 pixels vertical spacing between each filter when there's multiple: take 2px off the bottom
						// of each non-end shape, and take 2px off the top of each non-start shape
						if (shapesSoFar > 0) {
							shapeHeight -= 2;
							ypos += 2;
						}
						if (shapesSoFar < totalShapes - 1) {
							shapeHeight -= 2;
						}
						if (shapeHeight < SHAPE_MIN_HEIGHT)
							shapeHeight = SHAPE_MIN_HEIGHT;

						shapesSoFar++;
						shapesLaidOutPerComponent[source] = shapesSoFar;

						// figure out how many 'columns' there are total for the chain this filter is in, which column it is, and thus the X co-ords
						if (!_filterDataFlowComponentsToSourceElementDataFlowComponents.ContainsKey(filter))
							_calculateFilterDepthFromSource(filterShape);

						IDataFlowComponent ancestralElement = _filterDataFlowComponentsToSourceElementDataFlowComponents[filter];
						if (!_elementDataFlowComponentsToMaxFilterDepth.ContainsKey(ancestralElement))
							_calculateFilterDepthFromSource(filterShape);

						int filterColumns = _elementDataFlowComponentsToMaxFilterDepth[ancestralElement];
						int column = filterShape.LevelsFromElementSource; // 1-offset: a value of 0 would mean it's actually free-floating

						float horizontalProportionOffset = (float) (column - 1)/filterColumns;
						int columnWidth = filterWidth / filterColumns;
						int shapeWidth = Math.Max(Math.Min(columnWidth, SHAPE_MAX_WIDTH), SHAPE_MIN_WIDTH);
						if (shapeWidth < SHAPE_MAX_WIDTH && shapeWidth > SHAPE_MIN_WIDTH && columnWidth - shapeWidth < 20)
							shapeWidth = columnWidth - 20;

						int internalColumnOffset = (columnWidth - shapeWidth)/2;
						int xpos = filterX + (int)(horizontalProportionOffset * filterWidth) + internalColumnOffset;

						filterShape.X = xpos;
						filterShape.Y = ypos;
						filterShape.Width = shapeWidth;
						filterShape.Height = shapeHeight;
					}
				}
			}

			diagramDisplay.DoResumeUpdate();
		}

		private void _ResizeAndPositionNestingShape(FilterSetupShapeBase shape, int width, int x, int y, bool visible)
		{
			if (visible) {
				_ShowShape(shape);
			} else {
				_HideShape(shape);
			}

			if (width < SHAPE_MIN_WIDTH)
				width = SHAPE_MIN_WIDTH;

			if (visible && (shape is NestingSetupShape) && (shape as NestingSetupShape).Expanded &&
			    (shape as NestingSetupShape).ChildFilterShapes.Count > 0) {
				int curY = y + SHAPE_GROUP_HEADER_HEIGHT;
				foreach (FilterSetupShapeBase childShape in (shape as NestingSetupShape).ChildFilterShapes) {
					_ResizeAndPositionNestingShape(childShape, width - SHAPE_CHILD_WIDTH_REDUCTION, x, curY, true);
					curY += childShape.Height + SHAPE_VERTICAL_SPACING;
				}
				shape.Width = width;
				shape.Height = (curY - SHAPE_VERTICAL_SPACING + SHAPE_GROUP_FOOTER_HEIGHT) - y;
			} else {
				shape.Width = width;
				shape.Height = SHAPE_DEFAULT_HEIGHT;
				if (shape is NestingSetupShape) {
					foreach (FilterSetupShapeBase childShape in (shape as NestingSetupShape).ChildFilterShapes) {
						_ResizeAndPositionNestingShape(childShape, width, x, y, false);
					}
				}
			}
			shape.X = x;
			shape.Y = y + shape.Height/2;
		}

#endregion




#region  Selection of Shapes
		
		private IEnumerable<Shape> _getCurrentlySelectedShapes()
		{
			return diagramDisplay.SelectedShapes;
		}

		private void _selectShapesIfPresent(IEnumerable<Shape> shapes)
		{
			diagramDisplay.SelectShapes(Enumerable.Empty<Shape>(), false);

			foreach (Shape shape in shapes) {
				if (diagramDisplay.Diagram.Shapes.Contains(shape))
					diagramDisplay.SelectShape(shape, true);
			}
		}

#endregion






		#region Copying, Pasting, Deleting, Duplicating shapes

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

		private void PasteClipboardFiltersMultipleTimes()
		{
			Point cursor = Cursor.Position;
			using (
				NumberDialog numberDialog = new NumberDialog("Number of Copies", "How many copies of the given filter(s)?", 1, 1, 1000)) {
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
			newPosition.Y += diagramDisplay.GetDiagramOffset().Y - (SHAPE_DEFAULT_HEIGHT / 2);

			DuplicateFilterShapes(_filterShapeClipboard, numberOfCopies, newPosition);
		}


		private void _RemoveShapeFromDiagram(Shape shape, bool removePatching)
		{
			FilterSetupShapeBase fssb = shape as FilterSetupShapeBase;
			if (fssb != null) {
				_RemoveDataFlowLinksFromAllShapePoints(shape as FilterSetupShapeBase, removePatching);

				if (fssb.DataFlowComponent != null) {
					if (_dataFlowComponentToShapes.ContainsKey(fssb.DataFlowComponent)) {
						_dataFlowComponentToShapes[fssb.DataFlowComponent].Remove(fssb);
						if (_dataFlowComponentToShapes[fssb.DataFlowComponent].Count == 0) {
							_dataFlowComponentToShapes.Remove(fssb.DataFlowComponent);
						}
					} else {
						Logging.Warn("can't remove shape '" + fssb.Text + "' from DFCtS as it's not in the map");
					}

					if (_elementDataFlowComponentsToMaxFilterDepth.ContainsKey(fssb.DataFlowComponent)) {
						_elementDataFlowComponentsToMaxFilterDepth.Remove(fssb.DataFlowComponent);
					}

					if (_filterDataFlowComponentsToSourceElementDataFlowComponents.ContainsKey(fssb.DataFlowComponent)) {
						_filterDataFlowComponentsToSourceElementDataFlowComponents.Remove(fssb.DataFlowComponent);
					}
				}
			}

			shape.SecurityDomainName = SECURITY_DOMAIN_ALL_PERMISSIONS;
			diagramDisplay.DeleteShape(shape);

			if (shape is NestingSetupShape) {
				foreach (FilterSetupShapeBase child in (shape as NestingSetupShape).ChildFilterShapes) {
					_RemoveShapeFromDiagram(child, removePatching);
				}
			}
		}


		private void _DeleteShapeAndAssociatedComponents(Shape shape)
		{
			DataFlowConnectionLine line = shape as DataFlowConnectionLine;
			if (line != null) {
				VixenSystem.DataFlow.ResetComponentSource(line.DestinationDataComponent);
				_RemoveShapeFromDiagram(line, true);
				OnPatchingUpdated();
			}

			// we COULD use FilterSetupShapeBase, as all the operations below are generic.... but, we only want
			// to be able to delete filter shapes. We want to enforce all elements and outputs to be kept.
			FilterShape filterShape = shape as FilterShape;
			if (filterShape != null) {
				_RemoveShapeFromDiagram(filterShape, true);
				VixenSystem.Filters.RemoveFilter(filterShape.FilterInstance);
			}
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
				pos.X = (int) (shapesWidth * horizontalPositionProportion);
				pos.Y = diagramDisplay.GetDiagramOffset().Y + (diagramDisplay.Height / 4);
			}
			else {
				pos = (Point) startPosition;
			}

			List<FilterShape> result = new List<FilterShape>();

			for (int i = 0; i < numberOfCopies; i++) {
				foreach (IOutputFilterModuleInstance instance in sourceInstances) {
					FilterShape newShape = _CreateNewFilterInstanceAndShape(instance.TypeId, false, instance.ModuleData);
					newShape.ModuleDataUpdated();

					newShape.X = pos.X;
					newShape.Y = pos.Y;

					pos.Y += newShape.Height + SHAPE_VERTICAL_SPACING;

					result.Add(newShape);
				}
			}

			return result;
		}

		private FilterShape _CreateNewFilterInstanceAndShape(Guid filterTypeId, bool defaultLayout,
													 IModuleDataModel dataModelToCopy = null)
		{
			IOutputFilterModuleInstance moduleInstance = ApplicationServices.Get<IOutputFilterModuleInstance>(filterTypeId);
			if (dataModelToCopy != null) {
				moduleInstance.ModuleData = dataModelToCopy.Clone();
			}
			FilterShape shape = _CreateShapeFromFilter(moduleInstance);
			VixenSystem.Filters.AddFilter(moduleInstance);

			shape.Width = SHAPE_MAX_WIDTH;
			shape.Height = SHAPE_DEFAULT_HEIGHT;

			if (defaultLayout) {
				shape.X = (diagramDisplay.Width / 2) - diagramDisplay.GetDiagramPosition().X;
				shape.Y = diagramDisplay.GetDiagramOffset().Y + (diagramDisplay.Height / 2);
			}

			return shape;
		}

#endregion









#region UI Event Handlers

		private void diagramDisplay_KeyDown(object sender, KeyEventArgs e)
		{
			// if Delete was pressed, iterate through all selected shapes, and remove them, unlinking components as necessary
			if (e.KeyCode == Keys.Delete) {
				foreach (var shape in diagramDisplay.SelectedShapes) {
					if (shape is DataFlowConnectionLine) {
						_DeleteShapeAndAssociatedComponents(shape);
					} else if (shape is FilterShape) {
						_DeleteShapeAndAssociatedComponents(shape);
					}
				}
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

			// Ctrl X is Cut and needs to Copy to clipboard and remove items.
			if (e.KeyCode == Keys.X && e.Modifiers == Keys.Control)
			{
				CopySelectedFiltersToClipboard();
				foreach (var shape in diagramDisplay.SelectedShapes)
				{
					if (shape is DataFlowConnectionLine)
					{
						_DeleteShapeAndAssociatedComponents(shape);
					} else if (shape is FilterShape)
					{
						_DeleteShapeAndAssociatedComponents(shape);
					}
				}
				e.Handled = true;
			}
		}

		private void displayDiagram_ShapeDoubleClick(object sender, DiagramPresenterShapeClickEventArgs e)
		{
			var shape = (FilterSetupShapeBase)e.Shape;

			// workaround: only modify the shape if it's currently selected. The diagram likes to
			// send click events to all shapes under the mouse, even if they're not active.
			if (!diagramDisplay.SelectedShapes.Contains(shape)) {
				return;
			}

			if (shape is NestingSetupShape) {
				NestingSetupShape s = (shape as NestingSetupShape);
				s.Expanded = !s.Expanded;
			} else if (shape is FilterShape) {
				FilterShape filterShape = shape as FilterShape;
				filterShape.RunSetup();
			}

			if (shape is ElementNodeShape)
				_ResizeAndPositionElementShapes();

			if (shape is ControllerShape)
				_ResizeAndPositionControllerShapes();
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

		private void pasteFilterToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PasteClipboardFilters(Cursor.Position);
		}

		private void pasteFilterMultipleToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PasteClipboardFiltersMultipleTimes();
		}

		private void diagramDisplay_ShapesSelected(object sender, EventArgs e)
		{
			buttonDeleteFilter.Enabled = diagramDisplay.SelectedShapes.Any(x => x is FilterShape);
		}



#endregion











#region Hiding and Showing shapes

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

#endregion












#region  Connecting / Disconnecting shapes/lines


		private void _RemoveDataFlowLinksFromAllShapePoints(FilterSetupShapeBase shape, bool removePatching)
		{
			ControlPointId pointId;

			// go through all outputs for the shape, and check for connections to other shapes.
			for (int i = 0; i < shape.OutputCount; i++) {
				pointId = shape.GetControlPointIdForOutput(i);
				_RemoveDataFlowLinksFromShapePoint(shape, pointId, removePatching);
			}

			// now check the source of the filter; if it's connected to anything, remove the connecting shape.
			if (shape.InputCount > 0) {
				pointId = shape.GetControlPointIdForInput(0);
				_RemoveDataFlowLinksFromShapePoint(shape, pointId, removePatching);
			}
		}

		private void _RemoveDataFlowLinksFromShapePoint(FilterSetupShapeBase shape, ControlPointId controlPoint, bool removePatching)
		{
			foreach (ShapeConnectionInfo ci in shape.GetConnectionInfos(controlPoint, null)) {
				if (ci.OtherShape == null)
					continue;

				DataFlowConnectionLine line = ci.OtherShape as DataFlowConnectionLine;
				if (line == null)
					throw new Exception("a shape was connected to something other than a DataFlowLine!");

				// only try and unpatch if we're supposed to; this function can also be used to hide/remove lines from the display
				if (removePatching) {
					// if the line is connected with the given shape as the SOURCE, remove the unknown DESTINATION's
					// source (on the other end of the line). Otherwise, it (should) be that the given shape is the
					// destination; so reset it's source. If neither of these are true, freak out.
					if (line.GetConnectionInfo(ControlPointId.FirstVertex, null).OtherShape == shape) {
						if (line.DestinationDataComponent != null)
							VixenSystem.DataFlow.ResetComponentSource(line.DestinationDataComponent);
					}
					else if (line.GetConnectionInfo(ControlPointId.LastVertex, null).OtherShape == shape) {
						VixenSystem.DataFlow.ResetComponentSource(shape.DataFlowComponent);
					}
					else {
						throw new Exception("Can't reset a link that has neither the source or destination for the given shape!");
					}

					OnPatchingUpdated();
				}

				_RemoveShapeFromDiagram(line, false);
			}
		}


		private void _CreateConnectionsFromExistingLinks()
		{
			// go through the existing system-side patches (DataFlow sources) and make connections for them all.
			// There's nothing to do for element shapes; they don't have sources; only do filters and outputs

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
					Logging.Error("CreateConnectionsFromExistingLinks: can't find shape for source " + source.Component + source.OutputIndex);
					return;
				}
				List<FilterSetupShapeBase> sourceShapes = _dataFlowComponentToShapes[source.Component];
				// TODO: deal with multiple instances of the source data flow component: eg. a element existing as
				// multiple shapes (currently, we'll assume it's the first shape in the list)
				ConnectShapes(sourceShapes.First(), source.OutputIndex, shape);
			}
		}



		//public void ConnectShapes(FilterSetupShapeBase source, int sourceOutputIndex, FilterSetupShapeBase destination,
		//                          bool removeExistingSource = true)
		public void ConnectShapes(FilterSetupShapeBase source, int sourceOutputIndex, FilterSetupShapeBase destination)
		{
			DataFlowConnectionLine line = (DataFlowConnectionLine)project.ShapeTypes["DataFlowConnectionLine"].CreateInstance();
			diagramDisplay.InsertShape(line);
			diagramDisplay.Diagram.Shapes.SetZOrder(line, 100);
			line.EndCapStyle = project.Design.CapStyles.ClosedArrow;
			line.SecurityDomainName = SECURITY_DOMAIN_FIXED_SHAPE_NO_CONNECTIONS_DELETABLE;

			//if (removeExistingSource) {
			//    IEnumerable<ShapeConnectionInfo> connectionInfos =
			//        destination.GetConnectionInfos(destination.GetControlPointIdForInput(0), null);
			//    foreach (ShapeConnectionInfo ci in connectionInfos) {
			//        if (!ci.IsEmpty && ci.OtherShape is DataFlowConnectionLine) {
			//            _RemoveShapeFromDiagram(ci.OtherShape, true);
			//        }
			//    }
			//}

			line.SourceDataFlowComponentReference = new DataFlowComponentReference(source.DataFlowComponent, sourceOutputIndex);
			line.DestinationDataComponent = destination.DataFlowComponent;
			line.Connect(ControlPointId.FirstVertex, source, source.GetControlPointIdForOutput(sourceOutputIndex));
			line.Connect(ControlPointId.LastVertex, destination, destination.GetControlPointIdForInput(0));
		}



		public void FakeShapeConnection(FilterSetupShapeBase shape, ControlPointId controlPoint, bool shapeIsSource)
		{
			DataFlowConnectionLine line = (DataFlowConnectionLine)project.ShapeTypes["DataFlowConnectionLine"].CreateInstance();
			diagramDisplay.InsertShape(line);
			diagramDisplay.Diagram.Shapes.SetZOrder(line, 100);
			line.SecurityDomainName = SECURITY_DOMAIN_FIXED_SHAPE_NO_CONNECTIONS_DELETABLE;

			line.EndCapStyle = new CapStyle("fakecapstyle", CapShape.Flat, project.Design.ColorStyles.Blue);
			//line.LineStyle = new LineStyle("fakelinestyle", 1, project.Design.ColorStyles.Gray);
			line.LineStyle = project.Design.LineStyles.Dashed;

			if (shapeIsSource) {
				line.SourceDataFlowComponentReference = new DataFlowComponentReference(shape.DataFlowComponent, shape.GetOutputNumberForControlPoint(controlPoint));
				line.Connect(ControlPointId.FirstVertex, shape, controlPoint);
				line.MoveControlPointTo(ControlPointId.LastVertex,
					shape.GetControlPointPosition(controlPoint).X + 40,
					shape.GetControlPointPosition(controlPoint).Y,
					ResizeModifiers.None);
			} else {
				line.DestinationDataComponent = shape.DataFlowComponent;
				line.Connect(ControlPointId.LastVertex, shape, controlPoint);
				line.MoveControlPointTo(ControlPointId.FirstVertex,
					shape.GetControlPointPosition(controlPoint).X - 40,
					shape.GetControlPointPosition(controlPoint).Y,
					ResizeModifiers.None);
			}
		}







		public void UpdateConnections()
		{
			// we want to two main things here:
			// 1) link up all shapes with their source. That is, only filters and controllers: elements don't have sources
			// 2) handle the cases of vixen components that are patched to other components that might not be displayed.  That is,
			//    elements patched to items that are not displayed, or controllers/filters patched from items not displayed, etc.
			//    this is only elements and filters: controllers can't be patched FROM anything.

			// these actually don't work out too nicely. May as well do the whole thing.
			//UpdateConnectionsForControllers();
			//UpdateConnectionsForFilters();
			//UpdateConnectionsForElements();

			// patch from their respective sources. If a source is not displayed, it should do something to show that it's patched but non-existant.
			foreach (OutputShape shape in _outputShapes) {
				UpdateShapeSourceConnections(shape);
			}
			foreach (FilterShape shape in _filterShapes) {
				UpdateShapeSourceConnections(shape);
			}

			// patch everything to their destination components. (Most of this should be done as part of the source patching).
			// The main things we want to check are patches to shapes that don't exist anymore -- removing real ones, and adding
			// dummy ones. It doesn't hurt to sanity check destination patching though.)
			foreach (ElementNodeShape shape in _elementShapes) {
				UpdateShapeDestinationConnections(shape);
			}
			foreach (FilterShape shape in _filterShapes) {
				UpdateShapeDestinationConnections(shape);
			}
		}

		private void UpdateConnectionsForElements()
		{
			// patch everything to their destination components. (Most of this should be done as part of the source patching).
			// The main things we want to check are patches to shapes that don't exist anymore -- removing real ones, and adding
			// dummy ones. It doesn't hurt to sanity check destination patching though.)
			foreach (ElementNodeShape shape in _elementShapes) {
				UpdateShapeDestinationConnections(shape);
			}
		}

		private void UpdateConnectionsForFilters()
		{
			// patch from their respective sources. If a source is not displayed, it should do something to show that it's patched but non-existant.
			foreach (FilterShape shape in _filterShapes) {
				UpdateShapeSourceConnections(shape);
			}
			// patch everything to their destination components. (Most of this should be done as part of the source patching).
			// The main things we want to check are patches to shapes that don't exist anymore -- removing real ones, and adding
			// dummy ones. It doesn't hurt to sanity check destination patching though.)
			foreach (FilterShape shape in _filterShapes) {
				UpdateShapeDestinationConnections(shape);
			}
		}

		private void UpdateConnectionsForControllers()
		{
			// patch from their respective sources. If a source is not displayed, it should do something to show that it's patched but non-existant.
			foreach (OutputShape shape in _outputShapes) {
				UpdateShapeSourceConnections(shape);
			}

			// check element and filter destination connections; we might need to potentially
			// remove some faked connections that are now connected to the real thing.
			foreach (ElementNodeShape shape in _elementShapes) {
				UpdateShapeDestinationConnections(shape);
			}
			foreach (FilterShape shape in _filterShapes) {
				UpdateShapeDestinationConnections(shape);
			}
		}

		private void UpdateShapeSourceConnections(FilterSetupShapeBase shape)
		{
			// find any existing lines that are connected to the point (it might already be connected to something -- right or wrong)
			IEnumerable<ShapeConnectionInfo> inputConnectionInfos = shape.GetConnectionInfos(shape.GetControlPointIdForInput(0), null);

			// find the intended source of this particular block
			IDataFlowComponentReference source = null;
			if (shape.DataFlowComponent != null && shape.DataFlowComponent.Source != null) {
				source = shape.DataFlowComponent.Source;
			}

			// if this shape has a source, we need to determine what should be on the other end -- a real shape (if displayed),
			// or nothing (if it's not).  Once we have that, iterate through all the existing connection infos and remove any
			// that aren't connected to what we expect.
			if (source == null) {
				// if this shape has no actual source, there shouldn't be anything displayed.  Remove all the connected lines.
				foreach (ShapeConnectionInfo ci in inputConnectionInfos) {
					DataFlowConnectionLine line = ci.OtherShape as DataFlowConnectionLine;
					if (line == null) {
						Logging.Error("a shape was connected to something other than a DataFlowLine! Shape text: " + shape.Text);
					}
					else {
						_RemoveShapeFromDiagram(line, false);
					}
				}
			}
			else {
				List<FilterSetupShapeBase> sourceShapes = null;
				if (source.Component != null)
					_dataFlowComponentToShapes.TryGetValue(source.Component, out sourceShapes);
				if (sourceShapes == null)
					sourceShapes = new List<FilterSetupShapeBase>();

				// iterate through all the existing connections for the shape source, and determine if any actually MATCH
				// the expected source.  If so, keep it and delete the rest.  If not, delete everything and make a new line.
				Shape accurateLine = null;
				foreach (ShapeConnectionInfo ci in inputConnectionInfos) {
					DataFlowConnectionLine line = ci.OtherShape as DataFlowConnectionLine;
					if (line == null) {
						Logging.Error("a shape was connected to something other than a DataFlowLine! Shape text: " + shape.Text);
						continue;
					}

					if (line.DestinationDataComponent != shape.DataFlowComponent) {
						Logging.Warn("Well this is embarassing; a line shape is destined for a shape that isn't what it should be.");
					}

					if ((line.SourceDataFlowComponentReference != null &&
							line.SourceDataFlowComponentReference.Component == shape.DataFlowComponent.Source.Component &&
							line.SourceDataFlowComponentReference.OutputIndex == shape.DataFlowComponent.Source.OutputIndex) ||
						(line.SourceDataFlowComponentReference == null && sourceShapes.Count == 0))
					{
						accurateLine = line;
					}
					else {
						_RemoveShapeFromDiagram(line, false);
					}
				}

				// if there's a shape in the diagram that corresponds to the component source, keep it.  Otherwise, fake a line.
				if (accurateLine == null) {
					if (sourceShapes.Count > 0) {
						foreach (FilterSetupShapeBase sourceShape in sourceShapes) {
							ConnectShapes(sourceShape, source.OutputIndex, shape);
						}
					} else {
						FakeShapeConnection(shape, shape.GetControlPointIdForInput(0), false);
					}
				}
			}
		}



		private void UpdateShapeDestinationConnections(FilterSetupShapeBase shape)
		{
			// check shape connection lines against their destination patches. A lot of these may have been done as part of
			// source patching; we can double check anyway.  We also want to remove invalid patches, and add 'fake' ones if
			// there's a patch to a destination that isn't on the diagram.

			for (int i = 0; i < shape.OutputCount; i++) {
				// find any existing lines that are connected to the point (it might already be connected to something -- right or wrong)
				List<ShapeConnectionInfo> outputConnectionInfos = shape.GetConnectionInfos(shape.GetControlPointIdForOutput(i), null).ToList();

				// get what it SHOULD be connected to
				IEnumerable<IDataFlowComponent> intendedDestinations = Enumerable.Empty<IDataFlowComponent>();

				if (shape.DataFlowComponent != null)
					intendedDestinations = VixenSystem.DataFlow.GetDestinationsOfComponentOutput(shape.DataFlowComponent, i);

				HashSet<IDataFlowComponent> correctDestinations = new HashSet<IDataFlowComponent>();
				int realShapeConnections = 0;
				DataFlowConnectionLine dummyLine = null;

				foreach (ShapeConnectionInfo ci in outputConnectionInfos) {
					DataFlowConnectionLine line = ci.OtherShape as DataFlowConnectionLine;
					if (line == null) {
						Logging.Error("a shape was connected to something other than a DataFlowLine! Shape text: " + shape.Text);
					}

					if (line.SourceDataFlowComponentReference.Component != shape.DataFlowComponent ||
						line.SourceDataFlowComponentReference.OutputIndex != i) {
						
						Logging.Warn("shape destination patch doesn't have a source of this shape DFC! shape: " + shape.Text);
					}

					IDataFlowComponent lineDestination = line.DestinationDataComponent;

					if (lineDestination != null && correctDestinations.Contains(lineDestination)) {
						Logging.Warn("line destination has already been processed and it's being processed again");
					}

					// see if the line that is there is invalid: ie. doesn't even go to a DFC that matches one we expect to see. If so, remove it.
					// if not, it could be OK; double check that the shape it's connected to is the shape that represents that DFC (if any)
					if (lineDestination == null && intendedDestinations.Any()  ||  !intendedDestinations.Contains(lineDestination)) {
						_RemoveShapeFromDiagram(line, false);
					} else {
						List<FilterSetupShapeBase> destinationShapes = new List<FilterSetupShapeBase>();

						if (lineDestination != null && _dataFlowComponentToShapes.ContainsKey(lineDestination)) {
							destinationShapes = _dataFlowComponentToShapes[lineDestination];
						}

						if (destinationShapes.Count == 0) {
							// a null destination is OK if it's the ONLY line; it may be patched to a hidden shape.  Otherwise, remove it.
							if (lineDestination == null && outputConnectionInfos.Count == 1) {
								// line should be OK
								dummyLine = line;
							}
							else {
								_RemoveShapeFromDiagram(line, false);
							}
						}
						else {
							// really, only element nodes should have multiple shapes for a single element, and they can never be connected
							// to (as a destination). So, warn if there's multiple shapes here that correspond to a single endpoint.
							if (destinationShapes.Count > 1) {
								Logging.Warn("multiple destination shapes for a single DFC for updating destination connections");
							}

							// if the line is connected to a shape that isn't the expected one (based on DFC), delete the line, otherwise we're good
							if (line.GetConnectionInfo(ControlPointId.LastVertex, null).OtherShape != destinationShapes.First()) {
								_RemoveShapeFromDiagram(line, false);
							}
							else {
								if (lineDestination != null) {
									correctDestinations.Add(lineDestination);
									realShapeConnections++;
								}
							}
						}
					}
				}

				List<IDataFlowComponent> needConnecting = intendedDestinations.Except(correctDestinations).ToList();
				foreach (IDataFlowComponent component in needConnecting) {
					if (component != null && _dataFlowComponentToShapes.ContainsKey(component)) {
						List<FilterSetupShapeBase> destinationShapes = _dataFlowComponentToShapes[component];
						foreach (FilterSetupShapeBase destinationShape in destinationShapes) {
							ConnectShapes(shape, i, destinationShape);
							realShapeConnections++;
						}
					}
				}

				if (realShapeConnections == 0 && intendedDestinations.Count() > 0 && dummyLine == null) {
					// fake connection since there's nothing else connected
					FakeShapeConnection(shape, shape.GetControlPointIdForOutput(i), true);
				}
			}



		}

#endregion














#region Resize timer

		private Timer _relayoutOnResizeTimer;

		private void SetupPatchingGraphical_Resize(object sender, EventArgs e)
		{
			if (_relayoutOnResizeTimer == null) {
				_relayoutOnResizeTimer = new Timer();
				_relayoutOnResizeTimer.Interval = 250;
				_relayoutOnResizeTimer.Tick += _relayoutOnResizeTimer_Tick;
				_relayoutOnResizeTimer.Start();
			} else {
				_relayoutOnResizeTimer.Stop();
				_relayoutOnResizeTimer.Start();
			}
		}

		private void _relayoutOnResizeTimer_Tick(object sender, EventArgs e)
		{
			_relayoutOnResizeTimer.Stop();
			_relayoutOnResizeTimer = null;
			_RelayoutAllShapes();
		}

#endregion










#region Properties

		public IShapeCollection SelectedShapes
		{
			get { return diagramDisplay.SelectedShapes; }
		}

#endregion





		public void ZoomToFitAll()
		{
			double a = diagramDisplay.Width / (float)diagramDisplay.ScrollAreaBounds.Width;
			double b = diagramDisplay.Height / (float)diagramDisplay.ScrollAreaBounds.Height;

			double zoom = Math.Max(Math.Min(Math.Min(a, b), 1.0), 0.2);
			int zoomPercent = (int)(zoom * 100);
			if (zoomPercent < 100)
				zoomPercent--;		// fix some weird graphical bugs with scrollbars

			diagramDisplay.ZoomLevel = zoomPercent;

			diagramDisplay.ScrollBy(-diagramDisplay.ScrollAreaBounds.Width, -diagramDisplay.ScrollAreaBounds.Height);
		}



		private void buttonAddFilter_Click(object sender, EventArgs e)
		{

			List<KeyValuePair<string, object>> filters = new List<KeyValuePair<string, object>>();
			foreach (KeyValuePair<Guid, string> kvp in ApplicationServices.GetAvailableModules<IOutputFilterModuleInstance>()) {
				filters.Add(new KeyValuePair<string, object>(kvp.Value, kvp.Key));
			}
			using (ListSelectDialog addForm = new ListSelectDialog("Add Filter", (filters))) {
				addForm.SelectionMode = SelectionMode.One;
				if (addForm.ShowDialog() == DialogResult.OK) {
					List<IOutputFilterModuleInstance> newModuleInstances = new List<IOutputFilterModuleInstance>();
					foreach (KeyValuePair<string, object> item in addForm.SelectedItems) {
						IOutputFilterModuleInstance moduleInstance = ApplicationServices.Get<IOutputFilterModuleInstance>((Guid)item.Value);
						FilterShape shape = _CreateShapeFromFilter(moduleInstance);
						VixenSystem.Filters.AddFilter(moduleInstance);

						shape.Width = SHAPE_MAX_WIDTH;
						shape.Height = SHAPE_DEFAULT_HEIGHT;

						shape.X = (diagramDisplay.Width/2) - diagramDisplay.GetDiagramPosition().X;
						shape.Y = diagramDisplay.GetDiagramOffset().Y + (diagramDisplay.Height/2);

						newModuleInstances.Add(moduleInstance);
					}

					OnFiltersAdded(new FiltersEventArgs(newModuleInstances));
				}
			}
		}

		private void buttonDeleteFilter_Click(object sender, EventArgs e)
		{
			foreach (Shape selectedShape in diagramDisplay.SelectedShapes) {
				_DeleteShapeAndAssociatedComponents(selectedShape);	
			}
		}

		private void buttonZoomIn_Click(object sender, EventArgs e)
		{
			diagramDisplay.ZoomLevel = (int)((float)diagramDisplay.ZoomLevel*1.1);
		}

		private void buttonZoomOut_Click(object sender, EventArgs e)
		{
			diagramDisplay.ZoomLevel = (int)((float)diagramDisplay.ZoomLevel*0.9);
		}

		private void buttonZoomFit_Click(object sender, EventArgs e)
		{
			ZoomToFitAll();
		}




#region ISetupPatchingControl implementation and form linking

		public event EventHandler<FiltersEventArgs> FiltersAdded;
		public void OnFiltersAdded(FiltersEventArgs e)
		{
			if (FiltersAdded == null)
				return;

			FiltersAdded(this, e);
		}


		public event EventHandler PatchingUpdated;
		public void OnPatchingUpdated()
		{
			if (PatchingUpdated == null)
				return;

			PatchingUpdated(this, EventArgs.Empty);
		}


		private void _updateElementDisplay(IEnumerable<ElementNode> nodes)
		{
			diagramDisplay.DoSuspendUpdate();

			List<Shape> selectedShapes = _getCurrentlySelectedShapes().ToList();

			List<ElementNode> rootNodes = nodes.ToList();
			List<ElementNode> leafNodes = rootNodes.SelectMany(x => x.GetLeafEnumerator()).ToList();
			_UpdateElementShapesFromElements(rootNodes);
			IEnumerable<IOutputFilterModuleInstance> filters = _findFiltersThatDescendFromElements(leafNodes);
			
			// this assumption here is that the leaf node shapes will have been created as part of making the root node shapes...
			_UpdateFilterShapesFromFilters(filters);

			_ResizeAndPositionElementShapes();
			_ResizeAndPositionFilterShapes();

			UpdateConnections();
			_selectShapesIfPresent(selectedShapes);

			diagramDisplay.DoResumeUpdate();

			ZoomToFitAll();
		}

		public void UpdateElementSelection(IEnumerable<ElementNode> nodes)
		{
			_updateElementDisplay(nodes);
		}

		public void UpdateElementDetails(IEnumerable<ElementNode> nodes)
		{
			_updateElementDisplay(nodes);
		}

		private void _updateControllerDisplay(ControllersAndOutputsSet controllersAndOutputs)
		{
			diagramDisplay.DoSuspendUpdate();

			List<Shape> selectedShapes = _getCurrentlySelectedShapes().ToList();
			_updateControllerShapesFromControllersAndOutputs(controllersAndOutputs);
			_ResizeAndPositionControllerShapes();
			UpdateConnections();
			_selectShapesIfPresent(selectedShapes);

			diagramDisplay.DoResumeUpdate();

			ZoomToFitAll();
		}


		public void UpdateControllerSelection(ControllersAndOutputsSet controllersAndOutputs)
		{
			_updateControllerDisplay(controllersAndOutputs);
		}

		public void UpdateControllerDetails(ControllersAndOutputsSet controllersAndOutputs)
		{
			_updateControllerDisplay(controllersAndOutputs);
		}

		public Control SetupPatchingControl
		{
			get { return this; }
		}

		public DisplaySetup MasterForm { get; set; }

		public void OnFiltersAdded(IEnumerable<IOutputFilterModuleInstance> filters)
		{
			if (FiltersAdded != null)
				FiltersAdded(this, new FiltersEventArgs(filters));
		}

#endregion

	}
}
