using System.ComponentModel;
using System.Diagnostics;
using Common.Controls.Theme;
using Vixen.Data.Flow;
using Vixen.Export;
using Vixen.Factory;
using Vixen.Module;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Common.Controls
{
	public partial class ControllerTree : UserControl
	{
		public enum Direction
		{
			BACKWARD = -1,
			FORWARD = 1
		}

		// sets of data to keep track of which items in the treeview are open, selected, visible etc., so that
		// when we reload the tree, we can keep it looking relatively consistent with what the user had before.
		private HashSet<string> _expandedNodes; // TreeNode paths that are expanded
		private HashSet<string> _selectedNodes; // TreeNode paths that are selected
		private List<string> _topDisplayedNodes; // TreeNode paths that are at the top of the view. Should only
												 // need one, but will have multiple in case the top node is deleted.
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

		public void PopulateControllerTree(Dictionary<IControllerDevice, HashSet<int>> controllersAndOutputs)
		{
			List<string> selectedNodes = new List<string>();

			foreach (KeyValuePair<IControllerDevice, HashSet<int>> controllerAndOutput in controllersAndOutputs) {
				IControllerDevice controller = controllerAndOutput.Key;
				foreach (int i in controllerAndOutput.Value) {
					selectedNodes.Add(GenerateEquivalentTreeNodeFullPathFromControllerAndOutput(controller, i));
				}
			}

			_PopulateControllerTree(selectedNodes);
		}


		public void PopulateControllerTree(IControllerDevice controllerToSelect = null)
		{
			if (controllerToSelect == null) {
				_PopulateControllerTree();
				return;
			}

			List<string> treeNodes = new List<string>();
			treeNodes.Add(GenerateEquivalentTreeNodeFullPathFromController(controllerToSelect));
			_PopulateControllerTree(treeNodes);
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
			_selectedNodes.Clear();
			_selectedNodes.Add(GenerateEquivalentTreeNodeFullPathFromController(controller));

			var treeNode = treeview.Nodes[treeview.Nodes.Count - 1];
			//Select the new controller
			treeview.AddSelectedNode(treeview.Nodes[treeview.Nodes.Count - 1]);

			treeview.EndUpdate();

			treeNode.EnsureVisible();

			OnControllerSelectionChanged();
		}


		private void _PopulateControllerTree(IEnumerable<string> treeNodesToSelect = null)
		{
			// save metadata that is currently in the treeview
			_expandedNodes = new HashSet<string>();
			_selectedNodes = new HashSet<string>();
			_topDisplayedNodes = new List<string>();

			SaveTreeNodeState(treeview.Nodes);
			SaveTreeNodeTopVisible();

			// clear the treeview, and repopulate it
			treeview.BeginUpdate();
			treeview.Nodes.Clear();
			treeview.SelectedNodes.Clear();

			foreach (IControllerDevice controller in VixenSystem.OutputControllers) {
				AddControllerToTree(treeview.Nodes, controller);
			}


			// if a new controller has been passed in to select, select it instead.
			if (treeNodesToSelect != null) {
				_selectedNodes = new HashSet<string>(treeNodesToSelect);
			}

			foreach (string node in _selectedNodes) {
				TreeNode resultNode = FindNodeInTreeAtPath(treeview, node);

				if (resultNode != null) {
					treeview.AddSelectedNode(resultNode);
					//ensure selected are visible
					var parent = resultNode.Parent;
					while (parent != null)
					{
						parent.Expand();
						parent = parent.Parent;
					}
				}
			}

			// go through all the data we saved, and try to update the treeview to look
			// like it used to (expanded nodes, selected nodes, node at the top)

			foreach (string node in _expandedNodes)
			{
				TreeNode resultNode = FindNodeInTreeAtPath(treeview, node);

				if (resultNode != null)
				{
					resultNode.Expand();
				}
			}

			treeview.EndUpdate();

			// see stackoverflow.com/questions/626315/winforms-listview-remembering-scrolled-location-on-reload .
			// we can only set the topNode after EndUpdate(). Also, it might throw an exception -- weird?
			foreach (string node in _topDisplayedNodes) {
				TreeNode resultNode = FindNodeInTreeAtPath(treeview, node);

				if (resultNode != null) {
					try {
						treeview.TopNode = resultNode;
					} catch (Exception) {
						Logging.Warn("exception caught trying to set TopNode.");
					}
					break;
				}
			}

			// finally, if we were selecting another controller, make sure we raise the selection changed event
			if (treeNodesToSelect != null) {
				OnControllerSelectionChanged();
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
			foreach (TreeNode tn in collection) {
				if (tn.IsExpanded) {
					_expandedNodes.Add(GenerateTreeNodeFullPath(tn, treeview.PathSeparator));
				}

				if (treeview.SelectedNodes.Contains(tn)) {
					_selectedNodes.Add(GenerateTreeNodeFullPath(tn, treeview.PathSeparator));
				}

				SaveTreeNodeState(tn.Nodes);
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
					_topDisplayedNodes.Add(GenerateTreeNodeFullPath(current, treeview.PathSeparator));
					current = current.NextNode;
				}
			}
		}

		private void AddControllerToTree(TreeNodeCollection collection, IControllerDevice controller)
		{
			TreeNode controllerNode = new TreeNode();

			controllerNode.Name = controller.Id.ToString();
			controllerNode.Text = controller.Name;
			controllerNode.Tag = controller;

			SetControllerImage(controllerNode, controller.IsRunning);

			for (int i = 0; i < controller.OutputCount; i++) {
				TreeNode channelNode = new TreeNode();
				channelNode.Name = channelNode.Text = controller.Outputs[i].Name;
				channelNode.Tag = i;

				IDataFlowComponentReference source = controller.Outputs[i].Source;

				if (source == null) {
					channelNode.ImageKey = channelNode.SelectedImageKey = @"WhiteBall";
				} else if (source.Component == null || source.OutputIndex < 0) {
					channelNode.ImageKey = channelNode.SelectedImageKey = @"GreyBall";
				} else {
					channelNode.ImageKey = channelNode.SelectedImageKey = @"GreenBall";
				}

				controllerNode.Nodes.Add(channelNode);
			}

			collection.Add(controllerNode);
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
				foreach (TreeNode channelNode in node.Nodes)
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
					foreach (TreeNode channelNode in node.Nodes)
					{
						if (channelNode.Tag is int i)
						{
							IDataFlowComponentReference source = controller.Outputs[i].Source;

							if (source == null)
							{
								channelNode.ImageKey = channelNode.SelectedImageKey = "WhiteBall";
							}
							else if (source.Component == null || source.OutputIndex < 0)
							{
								channelNode.ImageKey = channelNode.SelectedImageKey = "GreyBall";
							}
							else
							{
								channelNode.ImageKey = channelNode.SelectedImageKey = "GreenBall";
							}
						}
					}
				}

			}

			treeview.EndUpdate();
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


		private void treeview_AfterSelect(object sender, TreeViewEventArgs e)
		{
			OnControllerSelectionChanged();
		}

		private void treeview_Deselected(object sender, EventArgs e)
		{
			OnControllerSelectionChanged();
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
			if (treeview.SelectedNode != null)
			{
				if (treeview.SelectedNode.Parent.Tag is OutputController outputController)
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
							outputController.InsertOutputsAt(treeview.SelectedNode.Index, nd.Value);

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
				if (node.Parent.Tag is OutputController controller)
				{
					if (node.Tag is int index)
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
		}

		/// <summary>
		/// Reorder the output controllers in the system to match the new order in the treeview.
		/// </summary>
		public void ReorderControllers()
		{
			// Get the list of controller names in the order they are displayed in the treeview.
			var sortList = new List<string>();
			foreach (TreeNode node in treeview.Nodes)
			{
				sortList.Add(node.Text);
			}

			// Reorder the controllers in the system to match the new order in the treeview.
			VixenSystem.OutputControllers.Reorder(sortList);
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

			this.unpatchControllerToolStripMenuItem.Enabled = false;

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
						this.unpatchControllerToolStripMenuItem.Enabled = true;
						break;
					}
			}

			// Show the menu item as singular or plural
			if (SelectedControllers.Count() > 1)
				this.unpatchControllerToolStripMenuItem.Text = "Unpatch Controllers";
			else
				this.unpatchControllerToolStripMenuItem.Text = "Unpatch Controller";

			_someSelectedControllersRunning = runningCount > 0;
			_someSelectedControllersNotRunning = notRunningCount > 0;
			startControllerToolStripMenuItem.Enabled = _someSelectedControllersNotRunning;
			stopControllerToolStripMenuItem.Enabled = _someSelectedControllersRunning;
		}

		private void treeView_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
		{
			if (isDoubleClick && e.Action == TreeViewAction.Collapse) e.Cancel = true;
		}

		private void treeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			if (isDoubleClick && e.Action == TreeViewAction.Expand)e.Cancel = true;
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
		}
	}
}
