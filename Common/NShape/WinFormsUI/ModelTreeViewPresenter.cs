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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Dataweb.NShape.Advanced;
using Dataweb.NShape.Controllers;


namespace Dataweb.NShape.WinFormsUI {

	/// <summary>
	/// Connects a Windows.Forms.TreeView control to a model controller.
	/// </summary>
	[ToolboxItem(true)]
	public partial class ModelTreeViewPresenter : Component {

		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.ModelTreeViewPresenter" />.
		/// </summary>
		public ModelTreeViewPresenter() {
			InitializeComponent();

			imageList = new ImageList();
			imageList.ColorDepth = ColorDepth.Depth32Bit;
			imageList.TransparentColor = Color.White;
			imageList.ImageSize = new Size(imageSize, imageSize);

			imgDifferentShapes = GdiHelpers.GetIconBitmap(Properties.Resources.ModelObjectAttached, Color.Fuchsia, imageList.TransparentColor);
			imgNoShapes = GdiHelpers.GetIconBitmap(Properties.Resources.ModelObjectDetached, Color.Fuchsia, imageList.TransparentColor);
		}


		#region [Public] Events

		/// <summary>
		/// Raised when the current selection has changed.
		/// </summary>
		public event EventHandler SelectionChanged;

		#endregion


		#region [Public] Properties

		/// <summary>
		/// Specifies the version of the assembly containing the component.
		/// </summary>
		[Category("NShape")]
		public string ProductVersion {
			get { return this.GetType().Assembly.GetName().Version.ToString(); }
		}


		/// <summary>
		/// Specifies the model controller for this presenter.
		/// </summary>
		[Category("NShape")]
		public ModelController ModelTreeController {
			get { return modelTreeController; }
			set {
				UnregisterModelTreeControllerEvents();
				modelTreeController = value;
				RegisterModelTreeControllerEvents();
			}
		}


		/// <summary>
		/// Specifies a property controller for editing properties of the selected model object (optional).
		/// </summary>
		[Category("NShape")]
		public PropertyController PropertyController {
			get { return propertyController; }
			set { propertyController = value; }
		}
		
		
		/// <summary>
		/// Specifies a TreeView used as user interface for this presenter.
		/// </summary>
		public TreeView TreeView {
			get { return treeView; }
			set {
				if (treeView != null) {
					treeView.ImageList = null;
					treeView.ContextMenuStrip = null;
					UnregisterTreeViewEvents();
				}
				treeView = value;
				if (treeView != null) {
					treeView.FullRowSelect = true;
					treeView.ImageList = imageList;
					treeView.ContextMenuStrip = this.contextMenuStrip;
					treeView.AllowDrop = true;
					RegisterTreeViewEvents();
				}
			}
		}


		/// <summary>
		/// Gets a readonly collection of selected model objects.
		/// </summary>
		[Browsable(false)]
		public IReadOnlyCollection<IModelObject> SelectedModelObjects {
			get { return selectedModelObjects; }
		}


		/// <summary>
		/// Specifies if MenuItemDefs that are not granted should appear as MenuItems in the dynamic context menu.
		/// </summary>
		[Category("Behavior")]
		public bool HideDeniedMenuItems {
			get { return hideMenuItemsIfNotGranted; }
			set { hideMenuItemsIfNotGranted = value; }
		}


		/// <summary>
		/// Specifies whether the default context menu (created dynamically from MenuItemDefs)  or a user defined context menu is displayed.
		/// </summary>
		[Category("Behavior")]
		public bool ShowDefaultContextMenu {
			get { return showDefaultContextMenu; }
			set { showDefaultContextMenu = value; }
		}

		#endregion


		#region [Public] Methods

		/// <summary>
		/// Selects the given model object.
		/// </summary>
		/// <param name="modelObject">Model object to be selected.</param>
		/// <param name="addToSelection">Specifies if the model object should be added to the current selection or should replace it.</param>
		public void SelectModelObject(IModelObject modelObject, bool addToSelection) {
			if (modelObject == null) throw new ArgumentNullException("modelObject");

			// perform selection
			if (!addToSelection) selectedModelObjects.Clear();
			if (!selectedModelObjects.Contains(modelObject))
				selectedModelObjects.Add(modelObject);

			// Notify propertyPresenter (if attached) that a modelOject was selected
			if (propertyController != null) propertyController.SetObjects(1, selectedModelObjects);
			if (SelectionChanged != null) SelectionChanged(this, EventArgs.Empty);
		}


		/// <summary>
		/// Removes the given model object from the current selection.
		/// </summary>
		public void UnselectModelObject(IModelObject modelObject) {
			if (modelObject == null) throw new ArgumentNullException("modelObject");
			if (selectedModelObjects.Contains(modelObject))
				selectedModelObjects.Remove(modelObject);

			// Notify propertyPresenter (if attached) that a modelOject was selected
			if (propertyController != null)propertyController.SetObjects(1, selectedModelObjects);
			if (SelectionChanged != null) SelectionChanged(this, EventArgs.Empty);
		}


		/// <summary>
		/// Clears all selected model objects.
		/// </summary>
		public void UnselectAllModelObjects() {
			selectedModelObjects.Clear();
			// Notify propertyPresenter (if attached) that all modelOjects were unselected
			if (propertyController != null) propertyController.SetObjects(1, selectedModelObjects);
			if (SelectionChanged != null) SelectionChanged(this, EventArgs.Empty);
		}


		/// <summary>
		/// Find all loaded shapes in all loaded diagrams assigned to any of the given model objects.
		/// </summary>
		/// <param name="modelObjects"></param>
		public void FindShapes(IEnumerable<IModelObject> modelObjects) {
			if (modelObjects == null) throw new ArgumentNullException("modelObjects");
		   modelTreeController.FindShapes(modelObjects);
		}


		/// <summary>
		/// Returns a collection of <see cref="T:Dataweb.NShape.Advanced.MenuItemDef" /> for constructing context menus etc.
		/// </summary>
		public IEnumerable<MenuItemDef> GetMenuItemDefs() {
			foreach (MenuItemDef action in modelTreeController.GetMenuItemDefs(selectedModelObjects))
				yield return action;
			// ToDo: Add presenter's actions
		}

		#endregion


		#region [Private] Methods

		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.dummyItem = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip.SuspendLayout();
			// 
			// contextMenuStrip
			// 
			this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dummyItem});
			this.contextMenuStrip.Name = "contextMenuStrip1";
			this.contextMenuStrip.Size = new System.Drawing.Size(142, 26);
			this.contextMenuStrip.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.contextMenuStrip_Closed);
			this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
			// 
			// dummyItem
			// 
			this.dummyItem.Name = "dummyItem";
			this.dummyItem.Size = new System.Drawing.Size(141, 22);
			this.dummyItem.Text = "dummyItem";
			this.contextMenuStrip.ResumeLayout(false);

		}


		private void FillTree() {
			if (treeView == null) throw new NShapePropertyNotSetException(this, "TreeView");
			if (modelTreeController == null) throw new NShapePropertyNotSetException(this, "ModelTreeController");
			Debug.Assert(modelTreeController.Project.Repository.IsOpen);

			// Add all root model objects to the tree
			foreach (IModelObject modelObject in modelTreeController.Project.Repository.GetModelObjects(null))
				AddModelObjectNode(modelObject);
		}

		
		private void Clear() {
			treeView.Nodes.Clear();
		}


		private void SelectNode(TreeNode node) {
			treeView.SelectedNode = node;
			if (node == null) UnselectAllModelObjects();
			else {
				Debug.Assert(node.Tag is IModelObject);
				SelectModelObject((IModelObject)node.Tag, false);
			}
		}

		
		private void AddModelObjectNode(IModelObject modelObject) {
			if (modelObject == null) throw new ArgumentNullException("modelObject");
			foreach (Shape s in modelObject.Shapes) {
				// do not add nodes of template shapes
				// ToDo: Find a better way of determining whether the model belongs to a template.
				if (s.Template == null) return;	
			}
			if (modelObject.Parent != null) {
				TreeNode node = FindTreeNode(treeView.Nodes, modelObject.Parent);
				if (node != null && (node.IsExpanded || node.Nodes.Count == 0))
					node.Nodes.Add(CreateNode(modelObject));
			} else treeView.Nodes.Add(CreateNode(modelObject));
		}


		private void AddModelObjectNodes(IEnumerable<IModelObject> modelObjects) {
			try {
				treeView.BeginUpdate();
				foreach (IModelObject modelObject in modelObjects)
					AddModelObjectNode(modelObject);
			} finally {
				treeView.EndUpdate();
			}
		}


		private void UpdateModelObjectNode(IModelObject modelObject) {
			TreeNode node = FindTreeNode(treeView.Nodes, modelObject);
			if (node != null) {
				bool isSelected = (treeView.SelectedNode == node);
				TreeNode parentNode = null;
				int position = -1;
				if (node != null) {
					parentNode = node.Parent;
					// Remove old node
					if (parentNode == null) {
						position = treeView.Nodes.IndexOf(node);
						treeView.Nodes.RemoveAt(position);
					} else {
						position = parentNode.Nodes.IndexOf(node);
						parentNode.Nodes.RemoveAt(position);
					}
				}

				// Create and insert new node
				TreeNode newNode = CreateNode(modelObject);
				if (modelObject.Parent == null)
					treeView.Nodes.Insert(position, newNode);
				else {
					// If the parent has not changed, re-insert new node at the old position
					// otherwise insert new node under the new parent node
					if (parentNode == null) {
						if (modelObject.Parent == null) {
							// Re-insert at the original root position
							treeView.Nodes.Insert(position, newNode);
						} else {
							// Add to the new parent node
							parentNode = FindTreeNode(treeView.Nodes, modelObject.Parent);
							Debug.Assert(parentNode != null);
							parentNode.Nodes.Add(newNode);
						}
					} else {
						if (modelObject.Parent == null)
							// Add to root
							treeView.Nodes.Add(newNode);
						else if (parentNode.Tag == modelObject.Parent)
							// Re-insert at the original hierarchical position
							parentNode.Nodes.Insert(position, newNode);
						else {
							// Add to the new parent node
							parentNode = FindTreeNode(treeView.Nodes, modelObject.Parent);
							Debug.Assert(parentNode != null);
							parentNode.Nodes.Add(newNode);
						}
					}
				}
				if (isSelected) treeView.SelectedNode = newNode;
			}
		}


		private void UpdateModelObjectNodes(IEnumerable<IModelObject> modelObjects) {
			Debug.Assert(TreeView != null);
			try {
				treeView.BeginUpdate();
				foreach (IModelObject modelObject in modelObjects)
					UpdateModelObjectNode(modelObject);
			} finally {
				treeView.EndUpdate();
			}
		}


		private void DeleteModelObjectNode(IModelObject modelObject) {
			TreeNode node = FindTreeNode(treeView.Nodes, modelObject);
			if (node != null) {
				TreeNode parentNode = node.Parent;
				if (parentNode == null)
					treeView.Nodes.Remove(node);
				else
					parentNode.Nodes.Remove(node);
			}
		}


		private void DeleteModelObjectNodes(IEnumerable<IModelObject> modelObjects) {
			try {
				treeView.BeginUpdate();
				foreach (IModelObject modelObject in modelObjects)
					DeleteModelObjectNode(modelObject);
			} finally {
				treeView.EndUpdate();
			}
		}


		private void DeleteNodeImage(string imageKey) {
			if (string.IsNullOrEmpty(imageKey)) throw new ArgumentNullException("imageKey");
			if (imageList.Images.ContainsKey(imageKey)) {
				Image img = imageList.Images[imageKey];
				imageList.Images.RemoveByKey(imageKey);
				img.Dispose();
				img = null;
			}
		}


		private void RedrawNodes(IEnumerable<TreeNode> treeNodes) {
			// create and assign new icon(s)
			if (treeView.SelectedNode != null) {
				treeView.SelectedNode.SelectedImageKey = string.Empty;
				treeView.SelectedNode.ImageKey = string.Empty;
			}
			foreach (TreeNode node in treeNodes) {
				Debug.Assert(node.Tag is IModelObject);
				node.SelectedImageKey = node.ImageKey = CreateNodeImage((IModelObject)node.Tag);
			}
		}


		private TreeNode CreateNode(IModelObject modelObject) {
			TreeNode result = new TreeNode(modelObject.Name);
			result.Name = modelObject.Name;
			result.Text = modelObject.Name;
			result.Tag = modelObject;
			result.Nodes.Add(keyDummyNode, string.Empty);
			result.SelectedImageKey = result.ImageKey = CreateNodeImage(modelObject);
			return result;
		}


		private string CreateNodeImage(IModelObject modelObject) {
			string imageKey = imgKeyNoShape;
			// check if all registered shapes are created from the same template
			Template template = null;
			foreach (Shape shape in modelObject.Shapes) {
				if (template == null && shape.Template != null) {
					template = shape.Template;
					imageKey = template.Name;
				} else if (template != shape.Template) {
					template = null;
					imageKey = imgKeyDifferentShapes;
					break;
				}
			}
			// if an image with the desired key exists, reuse it
			if (!imageList.Images.ContainsKey(imageKey)) {
				Image img;
				if (imageKey == imgKeyDifferentShapes) img = imgDifferentShapes;
				else if (imageKey == imgKeyNoShape) img = imgNoShapes;
				else img = template.CreateThumbnail(imageList.ImageSize.Width, imgMargin);
				imageList.Images.Add(imageKey, img);
			}
			return imageKey;
		}


		private TreeNode FindTreeNode(TreeNodeCollection nodesCollection, IModelObject modelObject) {
			if (nodesCollection == null) throw new ArgumentNullException("nodesCollection");
			if (modelObject == null) throw new ArgumentNullException("modelObject");
			TreeNode result = null;
			int dummyNodeIdx = nodesCollection.IndexOfKey(keyDummyNode);
			for (int i = nodesCollection.Count - 1; i >= 0; --i) {
				if (i == dummyNodeIdx) continue;
				TreeNode node = nodesCollection[i];
				Debug.Assert(node.Tag is IModelObject);
				//
				if (node.Tag == modelObject)
					result = node;
				else if (node.Nodes.Count > 0)
					result = FindTreeNode(node.Nodes, modelObject);
				if (result != null) break;
			}
			return result;
		}


		private IEnumerable<TreeNode> FindTreeNodes(TreeNodeCollection nodesCollection, Shape shape) {
			if (nodesCollection == null) throw new ArgumentNullException("nodesCollection");
			if (shape == null) throw new ArgumentNullException("shape");
			int dummyNodeIdx = nodesCollection.IndexOfKey(keyDummyNode);
			for (int i = 0; i < nodesCollection.Count; ++i) {
				if (i == dummyNodeIdx) continue;
				TreeNode node = nodesCollection[i];
				Debug.Assert(node.Tag is IModelObject);
				//
				// search all registered shapes of the node's ModelObject
				IModelObject modelObject = (IModelObject)node.Tag;
				foreach (Shape s in modelObject.Shapes) {
					if (s == shape) {
						yield return node;
						break;
					}
				}
				// search all child nodes
				if (node.Nodes.Count > 1 || node.Nodes.IndexOfKey(keyDummyNode) < 0)
					foreach (TreeNode n in FindTreeNodes(node.Nodes, shape))
						yield return n;
			}
		}


		private IEnumerable<TreeNode> FindTreeNodes(TreeNodeCollection nodesCollection, Template template) {
			if (nodesCollection == null) throw new ArgumentNullException("nodesCollection");
			if (template == null) throw new ArgumentNullException("template");
			int dummyNodeIdx = nodesCollection.IndexOfKey(keyDummyNode);
			for (int i = nodesCollection.Count - 1; i >= 0; --i) {
				if (i == dummyNodeIdx) continue;
				TreeNode node = nodesCollection[i];
				Debug.Assert(node.Tag is IModelObject);
				//
				// search all registered shapes of the node's ModelObject
				IModelObject modelObject = (IModelObject)node.Tag;
				foreach (Shape shape in modelObject.Shapes) {
					if (shape.Template != null && shape.Template == template) {
						yield return node;
						break;
					}
				}
				// if the template was not found, search the child nodes
				if (node.Nodes.Count > 1 || node.Nodes.IndexOfKey(keyDummyNode) < 0)
					foreach (TreeNode n in FindTreeNodes(node.Nodes, template))
						yield return n;
			}
		}


		#endregion


		#region [Private] Methods: (Un)Registering for events

		private void RegisterTreeViewEvents() {
			treeView.AfterCollapse += treeView_AfterCollapse;
			treeView.BeforeExpand += treeView_BeforeExpand;
			
			treeView.ItemDrag += treeView_ItemDrag;
			treeView.DragEnter += treeView_DragEnter;
			treeView.DragOver += treeView_DragOver;
			treeView.DragLeave += treeView_DragLeave;
			treeView.DragDrop += treeView_DragDrop;
			
			treeView.DoubleClick += treeView_DoubleClick;
			treeView.MouseDown += treeView_MouseDown;
			treeView.MouseUp += treeView_MouseUp;
		}


		private void UnregisterTreeViewEvents() {
			treeView.AfterCollapse -= treeView_AfterCollapse;
			treeView.BeforeExpand -= treeView_BeforeExpand;

			treeView.ItemDrag -= treeView_ItemDrag;
			treeView.DragEnter -= treeView_DragEnter;
			treeView.DragOver -= treeView_DragOver;
			treeView.DragLeave -= treeView_DragLeave;
			treeView.DragDrop -= treeView_DragDrop;
			
			treeView.DoubleClick -= treeView_DoubleClick;
			treeView.MouseDown -= treeView_MouseDown;
			treeView.MouseUp -= treeView_MouseUp;
		}


		private void RegisterModelTreeControllerEvents() {
			if (modelTreeController != null) {
				modelTreeController.Initialized += modelTreeController_Initialized;
				modelTreeController.Uninitialized += modelTreeController_Uninitialized;
				modelTreeController.ModelObjectsCreated += modelTreeController_ModelObjectsAdded;
				modelTreeController.ModelObjectsChanged += modelTreeController_ModelObjectsUpdated;
				modelTreeController.ModelObjectsDeleted += modelTreeController_ModelObjectsDeleted;
				modelTreeController.Changed += modelTreeController_Changed;
			}
		}


		private void UnregisterModelTreeControllerEvents() {
			if (modelTreeController != null) {
				modelTreeController.Initialized -= modelTreeController_Initialized;
				modelTreeController.Uninitialized -= modelTreeController_Uninitialized;
				modelTreeController.ModelObjectsCreated -= modelTreeController_ModelObjectsAdded;
				modelTreeController.ModelObjectsChanged -= modelTreeController_ModelObjectsUpdated;
				modelTreeController.ModelObjectsDeleted -= modelTreeController_ModelObjectsDeleted;
				modelTreeController.Changed -= modelTreeController_Changed;
			}
		}

		#endregion


		#region [Private] Methods: ModelTreeController event handler implementations

		private void modelTreeController_Initialized(object sender, EventArgs e) {
			FillTree();
		}


		private void modelTreeController_Uninitialized(object sender, EventArgs e) {
			Clear();
		}


		private void modelTreeController_Changed(object sender, EventArgs e) {
			// ToDo:
			// Replace this dummy implementation by a real one
			treeView.SuspendLayout();
			foreach (Template template in ModelTreeController.Project.Repository.GetTemplates()) {
				DeleteNodeImage(template.Name);
				RedrawNodes(FindTreeNodes(treeView.Nodes, template));
			}
			treeView.ResumeLayout();
		}
		
		
		private void modelTreeController_ModelObjectsSelected(object sender, ModelObjectSelectedEventArgs e) {
			// nothing to do
		}


		private void modelTreeController_TemplateShapeReplaced(object sender, RepositoryTemplateShapeReplacedEventArgs e) {
			treeView.SuspendLayout();
			DeleteNodeImage(e.Template.Name);
			RedrawNodes(FindTreeNodes(treeView.Nodes, e.Template));
			treeView.ResumeLayout();
		}


		private void modelTreeController_TemplateUpdated(object sender, RepositoryTemplateEventArgs e) {
			treeView.SuspendLayout();
			DeleteNodeImage(e.Template.Name);
			RedrawNodes(FindTreeNodes(treeView.Nodes, e.Template));
			treeView.ResumeLayout();
		}


		private void modelTreeController_ModelObjectsAdded(object sender, RepositoryModelObjectsEventArgs e) {
			AddModelObjectNodes(e.ModelObjects);
		}


		private void modelTreeController_ModelObjectsUpdated(object sender, RepositoryModelObjectsEventArgs e) {
			UpdateModelObjectNodes(e.ModelObjects);
		}


		private void modelTreeController_ModelObjectsDeleted(object sender, RepositoryModelObjectsEventArgs e) {
			DeleteModelObjectNodes(e.ModelObjects);
		}

		#endregion


		#region [Private] Methods: ContextMenu event handler implementation

		private void contextMenuStrip_Opening(object sender, CancelEventArgs e) {
			if (showDefaultContextMenu && treeView != null && treeView.ContextMenuStrip != null) {
				if (modelTreeController != null && modelTreeController.Project != null) {
					// Remove DummyItem
					if (contextMenuStrip.Items.Contains(dummyItem))
						contextMenuStrip.Items.Remove(dummyItem);
					// Collect all actions provided by the display itself
					WinFormHelpers.BuildContextMenu(contextMenuStrip, GetMenuItemDefs(), modelTreeController.Project, hideMenuItemsIfNotGranted);
				}
			}
		}


		private void contextMenuStrip_Closed(object sender, ToolStripDropDownClosedEventArgs e) {
			contextMenuStrip.Items.Clear();
			contextMenuStrip.Items.Add(dummyItem);
		}

		#endregion


		#region [Private] Methods: TreeView event handler implementations

		// Expanding / collapsing model object nodes
		private void treeView_AfterCollapse(object sender, TreeViewEventArgs e) {
			treeView.SuspendLayout();
			e.Node.Nodes.Clear();
			e.Node.Nodes.Add(keyDummyNode, string.Empty);
			treeView.ResumeLayout();
		}


		private void treeView_BeforeExpand(object sender, TreeViewCancelEventArgs e) {
			treeView.SuspendLayout();
			if (e.Node.Nodes.Count > 0)
				e.Node.Nodes.Clear();			
			List<IModelObject> childs = new List<IModelObject>(modelTreeController.Project.Repository.GetModelObjects((IModelObject)e.Node.Tag));
			foreach (IModelObject child in childs)
				e.Node.Nodes.Add(CreateNode(child));
			treeView.ResumeLayout();
		}


		private void treeView_DoubleClick(object sender, EventArgs e) {
			FindShapes(SelectedModelObjects);
		}


		// Mouse event handling
		private void treeView_MouseUp(object sender, MouseEventArgs e) {
			// nothing to do...
		}

		
		private void treeView_MouseDown(object sender, MouseEventArgs e) {
			if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right) {
				// Select the clicked node
				TreeViewHitTestInfo hitTestInfo = treeView.HitTest(e.Location);
				SelectNode(hitTestInfo.Node);
			}
		}


		// Drag'n'drop handling
		private void treeView_ItemDrag(object sender, ItemDragEventArgs e) {
			if (e.Button == MouseButtons.Left) {
				if (treeView.SelectedNode != null) {
					IModelObject selectedModelObject = (IModelObject)treeView.SelectedNode.Tag;
					treeView.DoDragDrop(new ModelObjectDragInfo(selectedModelObject), DragDropEffects.Move | DragDropEffects.Link | DragDropEffects.Scroll);
				}
			}
		}


		private void treeView_DragEnter(object sender, DragEventArgs e) {
			// nothing to do
		}


		private void treeView_DragOver(object sender, DragEventArgs e) {
			if (e.Data.GetDataPresent(typeof(ModelObjectDragInfo))) {
				e.Effect = DragDropEffects.Move;
			} else e.Effect = DragDropEffects.None;
		}


		private void treeView_DragLeave(object sender, EventArgs e) {
			// Do nothing...
		}


		private void treeView_DragDrop(object sender, DragEventArgs e) {
			if (e.Data.GetDataPresent(typeof(ModelObjectDragInfo)) && e.Effect == DragDropEffects.Move) {
				// Get dragged data
				ModelObjectDragInfo dragInfo = (ModelObjectDragInfo)e.Data.GetData(typeof(ModelObjectDragInfo));
				IModelObject parentModelObject = null;

				// Find the target node
				Point mousePos = Point.Empty;
				mousePos.Offset(e.X, e.Y);
				mousePos = treeView.PointToClient(mousePos);
				TreeViewHitTestInfo hitTestInfo = treeView.HitTest(mousePos);
				if (hitTestInfo.Node != null && dragInfo.ModelObject != hitTestInfo.Node.Tag)
					parentModelObject = (IModelObject)hitTestInfo.Node.Tag;

				if (dragInfo.ModelObject.Parent != parentModelObject)
					ModelTreeController.SetModelObjectParent(dragInfo.ModelObject, parentModelObject);
			}
		}

		#endregion


		#region [Private] Enumerator for ModelTree Items

		private class Enumerator : IEnumerator<IModelObject> {

			public Enumerator(TreeView treeView) {
				this.treeView = treeView;
				index = -1;
			}

			#region IEnumerator<IModelObject> Members

			public bool MoveNext() { return (bool)(++index < 1); }

			public void Reset() { index = -1; }

			IModelObject IEnumerator<IModelObject>.Current {
				get {
					IModelObject modelObject = null;
					if (treeView.SelectedNode != null)
						modelObject = (IModelObject)treeView.SelectedNode.Tag;
					return modelObject;
				}
			}

			#endregion

			#region IDisposable Members
			public void Dispose() { }
			#endregion

			#region IEnumerator Members

			object IEnumerator.Current { get { return (IModelObject)treeView.SelectedNode.Tag; } }

			#endregion

			#region Fields
			private TreeView treeView;
			private int index;
			#endregion
		}

		#endregion


		#region Fields
		// Constants
		private const int imageSize = 16;
		private const int imgMargin = 2;
		private const string keyDummyNode = "DummyNode";
		private const string keyNoImageAvailable = "NoImageAvailable";
		private const string imgKeyNoShape = "NoShape";
		private const string imgKeyDifferentShapes = "DifferentShapes";

		// NShape Controllers
		private ModelController modelTreeController;
		private PropertyController propertyController;

		private bool hideMenuItemsIfNotGranted = false;
		private bool showDefaultContextMenu = true;
		private HybridDictionary dict = new HybridDictionary();
		private ReadOnlyList<IModelObject> selectedModelObjects = new ReadOnlyList<IModelObject>();

		private List<IModelObject> modelObjectBuffer = new List<IModelObject>();
		
		private ImageList imageList;
		private Image imgDifferentShapes;
		private Image imgNoShapes;
		private IContainer components;
		private TreeView treeView;
		private ContextMenuStrip contextMenuStrip;
		#endregion
	}


	/// <summary>
	/// Drag'n'Drop info for dragging model objects out of the model tree presenter
	/// </summary>
	public class ModelObjectDragInfo {

		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.ModelObjectDragInfo" />.
		/// </summary>
		/// <param name="modelObject"></param>
		public ModelObjectDragInfo(IModelObject modelObject) {
			if (modelObject == null) throw new ArgumentNullException("modelObject");
			this.modelObject = modelObject;
		}


		/// <summary>
		/// Specifies the dragged model object.
		/// </summary>
		public IModelObject ModelObject {
			get { return modelObject; }
		}


		private IModelObject modelObject;
	}


	#region ModelTreeSelectedObjects

	internal class ModelTreeAdapterSelectedObjects : ICollection<IModelObject> {

		public ModelTreeAdapterSelectedObjects(TreeView treeView) {
			this.treeView = treeView;
		}

		#region ICollection<IModelObject> Members

		public void Add(IModelObject item) {
			TreeNode node = null;
			foreach (TreeNode n in treeView.Nodes) {
				if (n.Tag == item) {
					node = n;
					break;
				}
			}
			treeView.SelectedNode = node;
		}

		public void Clear() {
			treeView.SelectedNode = null;
		}

		public bool Contains(IModelObject item) {
			return (treeView.SelectedNode.Tag == item);
		}

		public void CopyTo(IModelObject[] array, int arrayIndex) {
			if (arrayIndex >= array.Length)
				throw new IndexOutOfRangeException();
			array[arrayIndex] = (IModelObject)treeView.SelectedNode.Tag;
		}

		public int Count {
			get { return (treeView.SelectedNode == null) ? 0 : 1; }
		}

		public bool IsReadOnly {
			get { return true; }
		}

		public bool Remove(IModelObject item) {
			bool result = false;
			if (treeView.SelectedNode != null) {
				treeView.SelectedNode = null;
				result = true;
			}
			return result;
		}

		#endregion

		#region IEnumerable<IModelObject> Members

		public IEnumerator<IModelObject> GetEnumerator() {
			foreach (TreeNode node in treeView.Nodes)
				yield return (IModelObject)node.Tag;
			//return new ModelTreeWindowsEnumerator(treeView);
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator() {
			foreach (TreeNode node in treeView.Nodes)
				yield return (IModelObject)node.Tag;
			//return new ModelTreeWindowsEnumerator(treeView);
		}

		#endregion

		#region Fields
		private TreeView treeView;
		#endregion
	}
	#endregion
}