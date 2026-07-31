using System.ComponentModel;
using Common.Controls.Theme;
using Vixen.Data.Flow;
using Vixen.Factory;
using Vixen.Module;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Common.Controls
{
	public partial class ControllerTree : UserControl
	{
		private const string VirtualNodeName = @"VIRT";
		private const int OutputPageSize = 5000;

		private sealed record OutputRange(IControllerDevice Controller, int StartIndex, int Count);
		private sealed record OutputIdentity(Guid ControllerId, int OutputIndex);
		private sealed record RangeIdentity(Guid ControllerId, int StartIndex);
		private sealed record NodeIdentity(Guid ControllerId, int? OutputIndex, int? RangeStart);
		public enum Direction
		{
			BACKWARD = -1,
			FORWARD = 1
		}
		private bool listReordered = false;

		// sets of data to keep track of which items in the treeview are open, selected, visible etc., so that
		// when we reload the tree, we can keep it looking relatively consistent with what the user had before.
		private HashSet<Guid> _expandedControllerIds = [];
		private HashSet<RangeIdentity> _expandedRanges = [];
		private HashSet<Guid> _selectedControllerIds = [];
		private HashSet<OutputIdentity> _selectedOutputs = [];
		private List<NodeIdentity> _topDisplayedNodes = [];
		private bool _projectingLogicalSelection;
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private bool _someSelectedControllersRunning;
		private bool _someSelectedControllersNotRunning;
		private bool isDoubleClick;

		public ControllerTree()
		{
			InitializeComponent();
			AutoSize = true;
			treeview.Dock = DockStyle.Fill;
			contextMenuStripTreeView.Renderer = new ThemeToolStripRenderer();
		}

		public bool SomeSelectedControllersRunning
		{
			get {return _someSelectedControllersRunning;}
		}

		public bool SomeSelectedControllersNotRunning
		{
			get {return _someSelectedControllersNotRunning;}
		}

		private void ControllerTree_Load(object sender, EventArgs e)
		{
			if (!(DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)) {
				PopulateControllerTree();
			}
		}


		#region Tree view population

		/// <summary>
		/// Populates the tree and selects the specified controller output indexes.
		/// </summary>
		/// <param name="controllersAndOutputs">The controllers and zero-based output indexes to select.</param>
		public void PopulateControllerTree(Dictionary<IControllerDevice, HashSet<int>> controllersAndOutputs)
		{
			SetLogicalSelection(controllersAndOutputs);
			_PopulateControllerTree();
			OnControllerSelectionChanged();
		}


		/// <summary>
		/// Populates the tree and optionally selects a controller root without materializing its outputs.
		/// </summary>
		/// <param name="controllerToSelect">The controller root to select, or <see langword="null" /> to preserve tree state.</param>
		public void PopulateControllerTree(IControllerDevice controllerToSelect = null)
		{
			if (controllerToSelect == null) {
				_PopulateControllerTree();
				return;
			}

			_PopulateControllerTree();
			SelectController(controllerToSelect);
			OnControllerSelectionChanged();
		}

		internal TreeView TreeViewForTests => treeview;

		internal void PopulateControllerTreeForTests(IEnumerable<IControllerDevice> controllers)
		{
			treeview.BeginUpdate();
			try
			{
				treeview.Nodes.Clear();
				treeview.SelectedNodes.Clear();

				foreach (IControllerDevice controller in controllers)
				{
					AddControllerToTree(treeview.Nodes, controller);
				}
			}
			finally
			{
				treeview.EndUpdate();
			}
		}

		internal void SelectOutputForTests(IControllerDevice controller, int outputIndex)
		{
			SelectOutput(controller, outputIndex);
		}

		internal void SetLogicalSelectionForTests(Dictionary<IControllerDevice, HashSet<int>> controllersAndOutputs)
		{
			treeview.BeginUpdate();
			try
			{
				SetLogicalSelection(controllersAndOutputs);
				ProjectLogicalSelection();
			}
			finally
			{
				treeview.EndUpdate();
			}
		}

		internal void ExpandNodeForTests(TreeNode node)
		{
			MaterializeNode(node);
		}

		internal void CollapseNodeForTests(TreeNode node)
		{
			EvictCollapsedNodeChildren(node);
		}

		public void UpdateScrollPosition()
		{
			if (treeview.SelectedNodes.Count > 0)
				treeview.TopNode = treeview.SelectedNodes[0];
		}

		/// <summary>
		/// Add a new controller without rebuilding the entire tree
		/// </summary>
		/// <param name="controller"></param>
		public void AddControllerToTree(IControllerDevice controller)
		{
			treeview.BeginUpdate();
			treeview.SelectedNodes.Clear();
			_topDisplayedNodes.Clear();
			AddControllerToTree(treeview.Nodes, controller);
			_selectedControllerIds.Clear();
			_selectedControllerIds.Add(controller.Id);

			var treeNode = treeview.Nodes[treeview.Nodes.Count - 1];
			//Select the new controller
			treeview.AddSelectedNode(treeview.Nodes[treeview.Nodes.Count - 1]);

			treeview.EndUpdate();

			treeNode.EnsureVisible();

			OnControllerSelectionChanged();
		}


		private void _PopulateControllerTree()
		{
			_expandedControllerIds = [];
			_expandedRanges = [];
			_topDisplayedNodes = [];

			SaveTreeNodeState(treeview.Nodes);
			SaveTreeNodeTopVisible();

			// clear the treeview, and repopulate it
			treeview.BeginUpdate();
			treeview.Nodes.Clear();
			treeview.SelectedNodes.Clear();

			foreach (IControllerDevice controller in VixenSystem.OutputControllers) {
				AddControllerToTree(treeview.Nodes, controller);
			}


			foreach (Guid controllerId in _expandedControllerIds)
				ExpandController(controllerId);
			foreach (RangeIdentity range in _expandedRanges)
				ExpandRange(range);
			ProjectLogicalSelection();

			treeview.EndUpdate();

			// see stackoverflow.com/questions/626315/winforms-listview-remembering-scrolled-location-on-reload .
			// we can only set the topNode after EndUpdate(). Also, it might throw an exception -- weird?
			foreach (NodeIdentity node in _topDisplayedNodes) {
				TreeNode resultNode = FindNode(node);

				if (resultNode != null) {
					try {
						treeview.TopNode = resultNode;
					} catch (Exception) {
						Logging.Warn("exception caught trying to set TopNode.");
					}
					break;
				}
			}

		}



		private string GenerateTreeNodeFullPath(TreeNode node, string separator)
		{
			string result = node.Name;
			TreeNode parent = node.Parent;
			while (parent != null) {
				result = parent.Name + separator + result;
				parent = parent.Parent;
			}

			return result;
		}

		private string GenerateEquivalentTreeNodeFullPathFromController(IControllerDevice controller)
		{
			return controller.Id.ToString();
		}

		private string GenerateEquivalentTreeNodeFullPathFromControllerAndOutput(IControllerDevice controller, int output)
		{
			return controller.Id + treeview.PathSeparator + controller.Outputs[output].Name;//treeview.PathSeparator + "#" + (output + 1);
		}

		private TreeNode FindTopParentInTreeAtPath(TreeView tree, string path)
		{
			string[] subnodes = path.Split(new string[] { tree.PathSeparator }, StringSplitOptions.None);
			return FindNodeInTreeAtPath(tree, subnodes[0]);
		}

		private TreeNode FindNodeInTreeAtPath(TreeView tree, string path)
		{

			string[] subnodes = path.Split(new string[] { tree.PathSeparator }, StringSplitOptions.None);
			TreeNodeCollection searchNodes = tree.Nodes;
			TreeNode currentNode = null;
			foreach (string search in subnodes) {
				bool found = false;
				foreach (TreeNode tn in searchNodes) {
					if (tn.Name == search) {
						found = true;
						currentNode = tn;
						searchNodes = tn.Nodes;
						break;
					}
				}
				if (!found) {
					currentNode = null;
					break;
				}
			}

			return currentNode;
		}

		private void SaveTreeNodeState(TreeNodeCollection collection)
		{
			foreach (TreeNode node in collection) {
				if (node.Tag is IControllerDevice controller) {
					if (node.IsExpanded)
						_expandedControllerIds.Add(controller.Id);
			}
				else if (node.Tag is OutputRange range && node.IsExpanded) {
					_expandedRanges.Add(new RangeIdentity(range.Controller.Id, range.StartIndex));
				}
				SaveTreeNodeState(node.Nodes);
			}
		}

		private void SaveTreeNodeTopVisible()
		{
			// this will iterate through all root nodes -- starting with the topmost visible
			// node -- adding their path to a list in order. Later on, when refreshing the tree,
			// we can try them in order to place at the top of the display. We should only
			// need a single node, but in case the top node gets deleted (or the top few),
			// we keep a list of 'preferred' nodes.
			if (treeview.Nodes.Count > 0) {
				TreeNode current = treeview.TopNode;
				while (current != null) {
					NodeIdentity identity = GetNodeIdentity(current);
					if (identity != null)
						_topDisplayedNodes.Add(identity);
					current = current.NextNode;
				}
			}
		}

		private NodeIdentity GetNodeIdentity(TreeNode node)
		{
			if (node.Tag is IControllerDevice controller)
				return new NodeIdentity(controller.Id, null, null);
			if (node.Tag is OutputRange range)
				return new NodeIdentity(range.Controller.Id, null, range.StartIndex);
			if (node.Tag is int outputIndex && FindOwningController(node) is IControllerDevice outputController)
				return new NodeIdentity(outputController.Id, outputIndex, null);
			return null;
		}

		private TreeNode FindNode(NodeIdentity identity)
		{
			TreeNode controllerNode = FindControllerNode(identity.ControllerId);
			if (controllerNode == null)
				return null;
			if (identity.OutputIndex is int outputIndex) {
				SelectOutput(identity.ControllerId, outputIndex);
				return treeview.SelectedNodes.LastOrDefault(node => node.Tag is int index && index == outputIndex);
			}
			if (identity.RangeStart is int rangeStart) {
				ExpandController(identity.ControllerId);
				return controllerNode.Nodes.Cast<TreeNode>().FirstOrDefault(node => node.Tag is OutputRange range && range.StartIndex == rangeStart);
			}
			return controllerNode;
		}

		private void AddControllerToTree(TreeNodeCollection collection, IControllerDevice controller)
		{
			TreeNode controllerNode = new TreeNode();

			controllerNode.Name = controller.Id.ToString();
			controllerNode.Text = controller.Name;
			controllerNode.Tag = controller;

			SetControllerImage(controllerNode, controller.IsRunning);

			if (controller.OutputCount > 0)
			{
				AddVirtualChild(controllerNode);
			}

			collection.Add(controllerNode);
		}

		private static void AddVirtualChild(TreeNode node)
		{
			node.Nodes.Add(new TreeNode { Name = VirtualNodeName });
		}

		private void AddControllerChildren(TreeNode controllerNode, IControllerDevice controller)
		{
			if (!HasOnlyVirtualChild(controllerNode))
				return;

			controllerNode.Nodes.Clear();
			if (controller.OutputCount <= OutputPageSize)
			{
				AddOutputLeaves(controllerNode.Nodes, controller, 0, controller.OutputCount);
				return;
			}

			for (int startIndex = 0; startIndex < controller.OutputCount; startIndex += OutputPageSize)
			{
				int count = Math.Min(OutputPageSize, controller.OutputCount - startIndex);
				var rangeNode = new TreeNode($"Outputs {startIndex + 1}-{startIndex + count}")
				{
					Name = $"{controller.Id}:{startIndex}",
					Tag = new OutputRange(controller, startIndex, count)
				};
				AddVirtualChild(rangeNode);
				controllerNode.Nodes.Add(rangeNode);
			}
		}

		private void AddOutputLeaves(TreeNodeCollection target, IControllerDevice controller, int startIndex, int count)
		{
			for (int outputIndex = startIndex; outputIndex < startIndex + count; outputIndex++)
			{
				var output = controller.Outputs[outputIndex];
				var outputNode = new TreeNode
				{
					Name = output.Name,
					Text = output.Name,
					Tag = outputIndex
				};
				SetOutputImage(outputNode, output.Source);
				target.Add(outputNode);
				if (_selectedOutputs.Contains(new OutputIdentity(controller.Id, outputIndex)))
					treeview.AddSelectedNode(outputNode);
			}
		}

		private static void SetOutputImage(TreeNode outputNode, IDataFlowComponentReference source)
		{
			outputNode.ImageKey = outputNode.SelectedImageKey = source switch
			{
				null => @"WhiteBall",
				{ Component: null } or { OutputIndex: < 0 } => @"GreyBall",
				_ => @"GreenBall"
			};
		}

		private static bool HasOnlyVirtualChild(TreeNode node) =>
			node.Nodes.Count == 1 && node.Nodes[0].Name == VirtualNodeName;

		private void AddRangeChildren(TreeNode rangeNode, OutputRange range)
		{
			if (!HasOnlyVirtualChild(rangeNode))
				return;

			rangeNode.Nodes.Clear();
			AddOutputLeaves(rangeNode.Nodes, range.Controller, range.StartIndex, range.Count);
		}

		private IControllerDevice FindOwningController(TreeNode outputNode)
		{
			for (TreeNode node = outputNode.Parent; node != null; node = node.Parent)
			{
				if (node.Tag is IControllerDevice controller)
					return controller;
			}

			return null;
		}

		private void SelectOutput(IControllerDevice controller, int outputIndex)
		{
			if (outputIndex < 0 || outputIndex >= controller.OutputCount)
				return;

			var controllerNode = treeview.Nodes.Cast<TreeNode>()
				.FirstOrDefault(node => node.Tag is IControllerDevice current && current.Id == controller.Id);
			if (controllerNode == null)
				return;

			MaterializeNode(controllerNode);
			controllerNode.Expand();
			TreeNode outputNode;
			if (controller.OutputCount <= OutputPageSize)
			{
				outputNode = controllerNode.Nodes.Cast<TreeNode>().FirstOrDefault(node => node.Tag is int index && index == outputIndex);
			}
			else
			{
				int pageStart = outputIndex / OutputPageSize * OutputPageSize;
				var rangeNode = controllerNode.Nodes.Cast<TreeNode>().FirstOrDefault(node => node.Tag is OutputRange range && range.StartIndex == pageStart);
				if (rangeNode == null)
					return;
				MaterializeNode(rangeNode);
				rangeNode.Expand();
				outputNode = rangeNode.Nodes.Cast<TreeNode>().FirstOrDefault(node => node.Tag is int index && index == outputIndex);
			}

			if (outputNode != null)
				treeview.AddSelectedNode(outputNode);
		}

		private void SetLogicalSelection(Dictionary<IControllerDevice, HashSet<int>> controllersAndOutputs)
		{
			_selectedControllerIds.Clear();
			_selectedOutputs.Clear();

			foreach (var (controller, outputIndexes) in controllersAndOutputs)
			{
				foreach (int outputIndex in outputIndexes)
				{
					if (outputIndex >= 0 && outputIndex < controller.OutputCount)
						_selectedOutputs.Add(new OutputIdentity(controller.Id, outputIndex));
				}
			}
		}

		private void ProjectLogicalSelection()
		{
			_projectingLogicalSelection = true;
			try
			{
				treeview.SelectedNodes.Clear();
				foreach (Guid controllerId in _selectedControllerIds)
					SelectController(controllerId);
				foreach (var selectedController in _selectedOutputs.GroupBy(output => output.ControllerId))
				{
					TreeNode controllerNode = FindControllerNode(selectedController.Key);
					if (controllerNode?.Tag is not IControllerDevice controller)
						continue;

					MaterializeNode(controllerNode);
					controllerNode.Expand();
					if (controller.OutputCount <= OutputPageSize)
						continue;

					foreach (int pageStart in selectedController
						.Select(output => output.OutputIndex / OutputPageSize * OutputPageSize)
						.Distinct())
					{
						TreeNode rangeNode = controllerNode.Nodes.Cast<TreeNode>()
							.FirstOrDefault(node => node.Tag is OutputRange range && range.StartIndex == pageStart);
						if (rangeNode == null)
							continue;
						MaterializeNode(rangeNode);
						rangeNode.Expand();
					}
				}
			}
			finally
			{
				_projectingLogicalSelection = false;
			}
		}

		private TreeNode FindControllerNode(Guid controllerId) =>
			treeview.Nodes.Cast<TreeNode>().FirstOrDefault(node =>
				node.Tag is IControllerDevice controller && controller.Id == controllerId);

		private void ExpandController(Guid controllerId)
		{
			TreeNode controllerNode = FindControllerNode(controllerId);
			if (controllerNode == null)
				return;
			MaterializeNode(controllerNode);
			controllerNode.Expand();
		}

		private void ExpandRange(RangeIdentity range)
		{
			ExpandController(range.ControllerId);
			TreeNode controllerNode = FindControllerNode(range.ControllerId);
			TreeNode rangeNode = controllerNode?.Nodes.Cast<TreeNode>()
				.FirstOrDefault(node => node.Tag is OutputRange descriptor && descriptor.StartIndex == range.StartIndex);
			if (rangeNode == null)
				return;
			MaterializeNode(rangeNode);
			rangeNode.Expand();
		}

		private void SelectController(IControllerDevice controller) => SelectController(controller.Id);

		private void SelectController(Guid controllerId)
		{
			TreeNode controllerNode = FindControllerNode(controllerId);
			if (controllerNode != null)
				treeview.AddSelectedNode(controllerNode);
		}

		private void SelectOutput(Guid controllerId, int outputIndex)
		{
			TreeNode controllerNode = FindControllerNode(controllerId);
			if (controllerNode?.Tag is IControllerDevice controller)
				SelectOutput(controller, outputIndex);
		}

		public void RefreshControllerName(IControllerDevice controller)
		{
			var path = GenerateEquivalentTreeNodeFullPathFromController(controller);
			var node = FindNodeInTreeAtPath(treeview, path);
			if (node.Tag == controller)
			{
				node.Text = controller.Name;
			}
		}

		public void RefreshControllerStatus()
		{
			treeview.BeginUpdate();
			foreach (TreeNode controllerNode in treeview.Nodes)
			{
				if (controllerNode.Tag is IControllerDevice controller)
				{
					SetControllerImage(controllerNode, controller.IsRunning);
				}
			}

			treeview.EndUpdate();
		}

		private void SetControllerImage(TreeNode controllerNode, bool isRunning)
		{
			if (isRunning)
				controllerNode.ImageKey = controllerNode.SelectedImageKey = @"Group";
			else
				controllerNode.ImageKey = controllerNode.SelectedImageKey = @"RedBall";
		}

		public void RefreshControllerOutputNames(IControllerDevice controller)
		{
			treeview.BeginUpdate();
			var path = GenerateEquivalentTreeNodeFullPathFromController(controller);
			var node = FindNodeInTreeAtPath(treeview, path);
			if (node.Tag == controller)
			{
				foreach (TreeNode channelNode in GetMaterializedOutputNodes(node))
				{
					if (channelNode.Tag is int i)
					{
						channelNode.Name = channelNode.Text = controller.Outputs[i].Name;
					}
				}
			}
			treeview.EndUpdate();
		}

		public void RefreshControllerOutputStatus()
		{
			treeview.BeginUpdate();
			foreach (TreeNode node in treeview.Nodes)
			{
				if (node.Tag is IControllerDevice controller)
				{
					foreach (TreeNode channelNode in GetMaterializedOutputNodes(node))
					{
						if (channelNode.Tag is int i)
						{
							SetOutputImage(channelNode, controller.Outputs[i].Source);
						}
					}
				}

			}

			treeview.EndUpdate();
		}

		private static IEnumerable<TreeNode> GetMaterializedOutputNodes(TreeNode controllerNode)
		{
			foreach (TreeNode child in controllerNode.Nodes)
			{
				if (child.Tag is int)
				{
					yield return child;
				}
				else if (child.Tag is OutputRange)
				{
					foreach (TreeNode outputNode in child.Nodes)
					{
						if (outputNode.Tag is int)
							yield return outputNode;
					}
				}
			}
		}

		#endregion



		#region Events

		public List<TreeNode> SelectedTreeNodes
		{
			get { return treeview.SelectedNodes; }
		}

		public IEnumerable<IControllerDevice> SelectedControllers
		{
			get
			{
				return treeview.SelectedNodes.Select(node => node.Tag).OfType<IControllerDevice>();
			}
		}

		/// <summary>
		/// Gets the logical controller-output selection, including outputs that are not currently materialized in the tree.
		/// </summary>
		/// <returns>A sequence of selected controllers and their zero-based output indexes.</returns>
		public IEnumerable<KeyValuePair<IControllerDevice, IReadOnlyCollection<int>>> GetSelectedControllerOutputs()
		{
			var result = new List<KeyValuePair<IControllerDevice, IReadOnlyCollection<int>>>();

			foreach (TreeNode controllerNode in treeview.Nodes)
			{
				if (controllerNode.Tag is not IControllerDevice controller)
					continue;

				if (_selectedControllerIds.Contains(controller.Id))
				{
					result.Add(new KeyValuePair<IControllerDevice, IReadOnlyCollection<int>>(controller,
						Enumerable.Range(0, controller.OutputCount).ToHashSet()));
					continue;
				}

				var outputs = _selectedOutputs
					.Where(output => output.ControllerId == controller.Id)
					.Select(output => output.OutputIndex)
					.Where(outputIndex => outputIndex >= 0 && outputIndex < controller.OutputCount)
					.ToHashSet();
				if (outputs.Count > 0)
					result.Add(new KeyValuePair<IControllerDevice, IReadOnlyCollection<int>>(controller, outputs));
			}

			return result;
		}


		private void treeview_AfterSelect(object sender, TreeViewEventArgs e)
		{
			CaptureLogicalSelectionFromTree();
			OnControllerSelectionChanged();
		}

		private void treeview_Deselected(object sender, EventArgs e)
		{
			CaptureLogicalSelectionFromTree();
			OnControllerSelectionChanged();
		}

		private void CaptureLogicalSelectionFromTree()
		{
			if (_projectingLogicalSelection)
				return;

			_selectedControllerIds.Clear();
			_selectedOutputs.Clear();
			foreach (TreeNode node in treeview.SelectedNodes)
			{
				switch (node.Tag)
				{
					case IControllerDevice controller:
						_selectedControllerIds.Add(controller.Id);
						break;
					case int outputIndex when FindOwningController(node) is IControllerDevice controller:
						_selectedOutputs.Add(new OutputIdentity(controller.Id, outputIndex));
						break;
				}
			}
		}


		public event EventHandler ControllerSelectionChanged;
		public void OnControllerSelectionChanged(EventArgs e = null)
		{
			if (e == null)
				e = EventArgs.Empty;
			if (ControllerSelectionChanged != null)
				ControllerSelectionChanged(this, e);
		}


		public event EventHandler ControllersChanged;
		public void OnControllersChanged(EventArgs e = null)
		{
			if (e == null)
				e = EventArgs.Empty;
			EventHandler handler = ControllersChanged;
			if (handler != null) handler(this, e);
		}

		#endregion




		#region Helper functions

		public bool AddNewControllerOfTypeWithPrompts(Guid controllerTypeId)
		{
			IModuleDescriptor moduleDescriptor = ApplicationServices.GetModuleDescriptor(controllerTypeId);
			if (moduleDescriptor == null) {
				Logging.Error("couldn't get descriptor for controller of type ID: " + controllerTypeId);
				return false;
			}

			string defaultName = moduleDescriptor.TypeName;
			string name;
			using (TextDialog textDialog = new TextDialog("New Controller Name?", "Controller Name", defaultName, true)) {
				if (textDialog.ShowDialog() != DialogResult.OK)
					return false;

				name = textDialog.Response;
				if (name.Length <= 0)
					name = defaultName;
			}

			ControllerFactory controllerFactory = new ControllerFactory();
			OutputController oc = (OutputController)controllerFactory.CreateDevice(controllerTypeId, name);

			int outputCount;
			using (NumberDialog nd = new NumberDialog("Controller Output Count", "Outputs on this controller?", 1, 1,  oc.OutputLimit)) {
				if (nd.ShowDialog() != DialogResult.OK)
					return false;

				outputCount = nd.Value;
			}

			oc.OutputCount = outputCount;
			VixenSystem.OutputControllers.Add(oc);

			//PopulateControllerTree(oc);
			AddControllerToTree(oc);
			OnControllersChanged();

			return true;
		}


		public bool RenameControllerWithPrompt(IControllerDevice outputController)
		{
			using (TextDialog textDialog = new TextDialog("Controller Name?", "Controller Name", outputController.Name, true)) {
				if (textDialog.ShowDialog() == DialogResult.OK) {
					if (textDialog.Response != string.Empty) {
						outputController.Name = textDialog.Response;
						OnControllersChanged();
						RefreshControllerName(outputController);
						return true;
					}
				}
			}
			return false;
		}

		public bool DeleteControllersWithPrompt(IEnumerable<IControllerDevice> controllers)
		{
			string message, title;
			if (controllers.Count() > 1) {
				message = "Are you sure you want to delete the selected controllers?";
				title = "Delete Controllers?";
			} else {
				message = "Are you sure you want to delete the selected controller?";
				title = "Delete Controller?";
			}

			if (controllers.Count() > 0) {
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				MessageBoxForm.msgIcon = SystemIcons.Warning; //this is used if you want to add a system icon to the message form.
				var messageBox = new MessageBoxForm(message, title, true, false);
				messageBox.ShowDialog();
				if (messageBox.DialogResult == DialogResult.OK)
				{
					foreach (OutputController oc in controllers) {
						VixenSystem.OutputControllers.Remove(oc);
					}
					OnControllersChanged();
					PopulateControllerTree();
					return true;
				}
			}
			return false;
		}

		public bool ConfigureController(IControllerDevice controller)
		{
			bool result = false;
			if (controller.HasSetup) {
				result = controller.Setup();
				if (result)
				{
					OnControllersChanged();
					RefreshControllerOutputNames(controller);
				}
			}
			return result;
		}

		public bool SetControllerOutputCount(IControllerDevice controller)
		{
			using (NumberDialog nd = new NumberDialog("Controller Output Count", "Outputs on this controller?", controller.OutputCount)) {
				if (nd.ShowDialog() == DialogResult.OK) {
					// TODO: blergh, dodgy hack
					(controller as OutputController).OutputCount = nd.Value;
					OnControllersChanged();
					PopulateControllerTree();
					return true;
				}
			}
			return false;
		}

		public async Task<bool> InsertOutputs()
		{
			if (treeview.SelectedNode?.Tag is int outputIndex)
			{
				if (FindOwningController(treeview.SelectedNode) is OutputController outputController)
				{
					using NumberDialog nd = new NumberDialog("Insert Outputs", "Number of outputs to insert.", 10);
					if (nd.ShowDialog() == DialogResult.OK)
					{

						if (TopLevelControl != null)
						{
							TopLevelControl.UseWaitCursor = true;
						}
						await Task.Factory.StartNew(() =>
						{
							var restartController = outputController.IsRunning;
							if (outputController.IsRunning)
							{
								VixenSystem.OutputControllers.Pause(outputController);
							}
							outputController.InsertOutputsAt(outputIndex, nd.Value);

							if (restartController)
							{
								VixenSystem.OutputControllers.Resume(outputController);
							}
						});

						OnControllersChanged();
						PopulateControllerTree();
						if (TopLevelControl != null)
						{
							TopLevelControl.UseWaitCursor = false;
						}
						return true;
					}
				}
			}

			return false;
		}

		public bool RemoveSelectedOutputs()
		{
			if (SelectedControllers.Any()) return false;

			if (TopLevelControl != null)
			{
				TopLevelControl.UseWaitCursor = true;
			}

			var outputsToRemove = new Dictionary<OutputController, List<CommandOutput>>();

			foreach (var node in treeview.SelectedNodes)
			{
				if (node.Tag is int index && FindOwningController(node) is OutputController controller)
				{
					if (outputsToRemove.TryGetValue(controller, out var outputs))
					{
						outputs.Add(controller.Outputs[index]);
					}
					else
					{
						outputsToRemove.Add(controller, new List<CommandOutput>() { controller.Outputs[index] });
					}
				}
			}

			if (outputsToRemove.Any())
			{
				foreach (var controllerOutputs in outputsToRemove)
				{
					var restartController = controllerOutputs.Key.IsRunning;
					if (controllerOutputs.Key.IsRunning)
					{
						VixenSystem.OutputControllers.Pause(controllerOutputs.Key);
					}

					controllerOutputs.Key.RemoveOutputs(controllerOutputs.Value);

					if (restartController)
					{
						VixenSystem.OutputControllers.Resume(controllerOutputs.Key);
					}
				}
				treeview.ClearSelectedNodes();
				OnControllersChanged();
				PopulateControllerTree();
				if (TopLevelControl != null)
				{
					TopLevelControl.UseWaitCursor = false;
				}
				return true;
			}

			if (TopLevelControl != null)
			{
				TopLevelControl.UseWaitCursor = false;
			}

			return false;
		}

		public void ClearSelectedNodes()
		{
			treeview.ClearSelectedNodes();
		}

		/// <summary>
		/// Moves selected nodes up or down in the treeview.
		/// </summary>
		/// <param name="direction">Specifies the direction to move the selected nodes.</param>
		public void ReorderSelectedNodes(Direction direction)
		{
			// Iterate through the selected nodes in reverse order, so that we can
			// remove them from the treeview without messing up the indexes of the
			// remaining selected nodes.
			var holdTreeNode = new List<TreeNode>();
			for (int index = treeview.SelectedNodes.Count - 1; index >= 0; index--)
			{
				// Only move primary nodes
				if (treeview.SelectedNodes[index].Level == 0)
				{
					holdTreeNode.Add(treeview.SelectedNodes[index]);
					treeview.Nodes.Remove(treeview.SelectedNodes[index]);
				}
			}

			// Now we can insert the selected nodes back into the treeview.
			// We need to insert them in reverse order, so that the first selected
			// node is at the top of the list.
			int insertionPoint = -1;
			for (int index = holdTreeNode.Count - 1; index >= 0; index--)
			{
				// Do some validation when inserting nodes at the top of the tree.
				if (holdTreeNode[index].Index + (int)direction <= insertionPoint)
					insertionPoint = holdTreeNode[index].Index;
				else
					insertionPoint = holdTreeNode[index].Index + (int)direction;

				// Reinsert the node at the new index.
				treeview.Nodes.Insert(insertionPoint, holdTreeNode[index]);
			}

			listReordered = true;
		}

		/// <summary>
		/// Reorder the output controllers in the system to match the new order in the treeview.
		/// </summary>
		public void ReorderControllers()
		{
			if (listReordered)
			{
				// Get the list of controller names in the order they are displayed in the treeview.
				var sortList = new List<Guid>();
				foreach (TreeNode node in treeview.Nodes)
				{
					if (node.Tag is IControllerDevice device)
					{
						sortList.Add(device.Id);
					}
				}

				// Reorder the controllers in the system to match the new order in the treeview.
				VixenSystem.OutputControllers.Reorder(sortList);

				listReordered = false;
			}
		}

		#endregion



		#region Context Menus

		private void contextMenuStripTreeView_Opening(object sender, CancelEventArgs e)
		{
			//e.Cancel = (!SelectedControllers.Any());
			if (SelectedControllers.Any())
			{
				insertChannelsToolStripMenuItem.Visible = false;
				removeChannelsToolStripMenuItem.Visible = false;
				unpatchChannelsToolStripMenuItem.Visible = false;
				findPatchedChannelsToolStripMenuItem.Visible = false;
				toolStripSeparator.Visible = true;
				configureToolStripMenuItem.Visible = true;
				channelCountToolStripMenuItem.Visible = true;
				renameToolStripMenuItem.Visible = true;
				deleteToolStripMenuItem.Visible = true;
				startControllerToolStripMenuItem.Visible = true;
				stopControllerToolStripMenuItem.Visible = true;
				unpatchControllerToolStripMenuItem.Visible = true;
				configureToolStripMenuItem.Enabled = (SelectedControllers.Count() == 1);
				channelCountToolStripMenuItem.Enabled = (SelectedControllers.Count() == 1);
				renameToolStripMenuItem.Enabled = (SelectedControllers.Count() == 1);
				deleteToolStripMenuItem.Enabled = (SelectedControllers.Any());
				CheckIfSelectedControllersRunning();
				return;
			}

			if (treeview.SelectedNodes.Any())
			{
				if (treeview.SelectedNodes.Any(node => node.Tag is not int))
				{
					e.Cancel = true;
					return;
				}

				unpatchChannelsToolStripMenuItem.Enabled = false;
				findPatchedChannelsToolStripMenuItem.Enabled = false;

				// Search for at least one channel that is patched. If found, then
				// we can enable the Unpatch Channel menu option.
				foreach (var node in treeview.SelectedNodes)
				{
					if (node.ImageKey == "GreenBall")
					{
						unpatchChannelsToolStripMenuItem.Enabled = true;
						findPatchedChannelsToolStripMenuItem.Enabled = true;
						break;
					}
				}

				// Show the menu items as singular or plural
				if (treeview.SelectedNodes.Count == 1)
				{
					insertChannelsToolStripMenuItem.Visible = true;
					unpatchChannelsToolStripMenuItem.Text = "Unpatch Channel";
					findPatchedChannelsToolStripMenuItem.Text = "Find Patched Element";
				}
				else
				{
					insertChannelsToolStripMenuItem.Visible = false;
					unpatchChannelsToolStripMenuItem.Text = "Unpatch Channels";
					findPatchedChannelsToolStripMenuItem.Text = "Find Patched Elements";
				}
				removeChannelsToolStripMenuItem.Visible = true;
				unpatchChannelsToolStripMenuItem.Visible = true;
				findPatchedChannelsToolStripMenuItem.Visible = true;
				toolStripSeparator.Visible = false;
				configureToolStripMenuItem.Visible = false;
				channelCountToolStripMenuItem.Visible = false;
				renameToolStripMenuItem.Visible = false;
				deleteToolStripMenuItem.Visible = false;
				startControllerToolStripMenuItem.Visible = false;
				stopControllerToolStripMenuItem.Visible = false;
				unpatchControllerToolStripMenuItem.Visible = false;
				return;
			}

			e.Cancel = true;
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DeleteControllersWithPrompt(SelectedControllers);
		}

		private void renameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			RenameControllerWithPrompt(SelectedControllers.First());
		}

		private void configureToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ConfigureController(SelectedControllers.First());
		}

		private void channelCountToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SetControllerOutputCount(SelectedControllers.First());
		}

		private async void insertChannelsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			await InsertOutputs();
		}

		private void deleteChannelsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.WaitCursor;
			RemoveSelectedOutputs();
			Cursor = Cursors.Arrow;
		}

		private void startControllerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			StartController();
		}

		private void stopControllerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			StopController();
		}

		#endregion



		private void treeview_KeyDown(object sender, KeyEventArgs e)
		{
			// do our own deleting of items here
			if (e.KeyCode == Keys.Delete) {
				if (SelectedControllers.Count() > 0) {
					DeleteControllersWithPrompt(SelectedControllers);
				}
			}
		}

		public void StartController()
		{
			bool changes = false;

			foreach (IControllerDevice controller in SelectedControllers){
				if (!controller.IsRunning){
					VixenSystem.OutputControllers.Start(VixenSystem.OutputControllers.GetController(controller.Id));
					changes = true;
				}
			}

			if (changes){
				RefreshControllerStatus();
				OnControllersChanged();
			}
		}

		public void StopController()
		{
			bool changes = false;

			foreach (IControllerDevice controller in SelectedControllers) {
				if (controller.IsRunning) {
					VixenSystem.OutputControllers.Stop(VixenSystem.OutputControllers.GetController(controller.Id));
					changes = true;
				}
			}

			if (changes){
				RefreshControllerStatus();
				OnControllersChanged();
			}
		}

		public void CheckIfSelectedControllersRunning()
		{
			int runningCount = 0;
			int notRunningCount = 0;

			unpatchControllerToolStripMenuItem.Enabled = false;

			foreach (IControllerDevice controller in SelectedControllers) {
				if (controller.IsRunning) {
					runningCount++;
				}else {
					notRunningCount++;
				}

				// Search for at least one channel that is patched. If found, then
				// we can enable the Unpatch Controller menu option.
				foreach (var output in controller.Outputs)
					if (output.Source != null && output.Source.Component != null)
					{
						unpatchControllerToolStripMenuItem.Enabled = true;
						break;
					}
			}

			// Show the menu item as singular or plural
			if (SelectedControllers.Count() > 1)
				unpatchControllerToolStripMenuItem.Text = "Unpatch Controllers";
			else
				unpatchControllerToolStripMenuItem.Text = "Unpatch Controller";

			_someSelectedControllersRunning = runningCount > 0;
			_someSelectedControllersNotRunning = notRunningCount > 0;
			startControllerToolStripMenuItem.Enabled = _someSelectedControllersNotRunning;
			stopControllerToolStripMenuItem.Enabled = _someSelectedControllersRunning;
		}

		private void treeView_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
		{
			if (isDoubleClick && e.Action == TreeViewAction.Collapse) e.Cancel = true;
			if (!e.Cancel)
				EvictCollapsedNodeChildren(e.Node);
		}

		private void treeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			if (isDoubleClick && e.Action == TreeViewAction.Expand)
			{
				e.Cancel = true;
				return;
			}

			MaterializeNode(e.Node);
		}

		private void MaterializeNode(TreeNode node)
		{
			switch (node.Tag)
			{
				case IControllerDevice controller:
					AddControllerChildren(node, controller);
					break;
				case OutputRange range:
					AddRangeChildren(node, range);
					break;
			}
		}

		private void EvictCollapsedNodeChildren(TreeNode node)
		{
			if (node.Tag is not IControllerDevice && node.Tag is not OutputRange)
				return;
			if (HasOnlyVirtualChild(node))
				return;

			treeview.BeginUpdate();
			_projectingLogicalSelection = true;
			try
			{
				node.Nodes.Clear();
				AddVirtualChild(node);
				RestoreMaterializedLogicalSelection();
			}
			finally
			{
				_projectingLogicalSelection = false;
				treeview.EndUpdate();
			}
		}

		private void RestoreMaterializedLogicalSelection()
		{
			treeview.ClearSelectedNodes();
			foreach (TreeNode controllerNode in treeview.Nodes)
			{
				if (controllerNode.Tag is not IControllerDevice controller)
					continue;
				if (_selectedControllerIds.Contains(controller.Id))
					treeview.AddSelectedNode(controllerNode);
				foreach (TreeNode outputNode in GetMaterializedOutputNodes(controllerNode))
				{
					if (outputNode.Tag is int outputIndex &&
						_selectedOutputs.Contains(new OutputIdentity(controller.Id, outputIndex)))
					{
						treeview.AddSelectedNode(outputNode);
					}
				}
			}
		}

		private void treeView_MouseDown(object sender, MouseEventArgs e)
		{
			isDoubleClick = e.Clicks > 1;
		}

		private void treeview_DoubleClick(object sender, EventArgs e)
		{
			if (SelectedControllers.Any()) ConfigureController(SelectedControllers.First());
		}

		private void Treeview_DragOverVerify(object sender, DragVerifyEventArgs e)
		{
			e.ValidDragTarget = e.DragBetweenNodes != DragBetweenNodes.DragOnTargetNode;
			listReordered = true;
		}
	}
}
