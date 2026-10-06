using System.Globalization;
using Common.Controls;
using Common.Controls.Theme;
using Vixen.Rule;
using Vixen.Sys;

namespace VixenModules.Property.Order
{
	public partial class OrderSetupHelper : BaseForm, IElementSetupHelper
	{
		private readonly Dictionary<IElementNode, int>  _elementOrderLookup = new Dictionary<IElementNode, int>();
		private readonly ContextMenuStrip _contextMenu = new ContextMenuStrip();

		public OrderSetupHelper()
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			_contextMenu.Renderer = new ThemeToolStripRenderer();
			elementList.ItemDragDropCompleted += ElementList_ItemDragDropCompleted;
			elementList.MouseClick += ElementListOnMouseClick;
			elementList.KeyDown += OnKeyDown;
		}

		/// <summary>Handles the element list keyboard shortcut for selecting all rows.</summary>
		/// <param name="sender">The control that raised the keyboard event.</param>
		/// <param name="e">The keyboard event data.</param>
		/// <remarks>Pressing A with Control selects all rows and suppresses the key press. Other input continues through the control's normal handling.</remarks>
		protected void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.A && e.Control)
			{
				elementList.BeginUpdate();
				foreach (ListViewItem item in elementList.Items)
				{
					item.Selected = true;
				}
				e.SuppressKeyPress = true;
				elementList.EndUpdate();
			}
		}

		private void ElementListOnMouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				_contextMenu.Items.Clear();
				if (elementList.FocusedItem.Bounds.Contains(e.Location))
				{
					if (elementList.SelectedItems.Count > 1)
					{
						var reverseItems = new ToolStripMenuItem("Reverse");
						reverseItems.Click += ReverseItems_Click;
						_contextMenu.Items.Add(reverseItems);
						var zigzagItems = new ToolStripMenuItem("Zig Zag...");
						zigzagItems.Click += ZigZagItems_Click;
						_contextMenu.Items.Add(zigzagItems);

					}
					_contextMenu.Show(elementList, e.Location);
				}
			}
		}

		private void ReverseItems_Click(object sender, EventArgs e)
		{
			var selectedindexes = elementList.SelectedIndices;
			var indexMap = new Dictionary<int, ListViewItem>();
			int counter = selectedindexes.Count - 1;
			foreach (int selectedindex in selectedindexes)          
			{
				indexMap.Add(selectedindexes[counter--], elementList.Items[selectedindex]);
			}

			foreach (var item in indexMap)
			{
				elementList.Items.RemoveAt(item.Key);
				elementList.Items.Insert(item.Key, (ListViewItem)item.Value.Clone());
				elementList.Items[item.Key].Selected = true;
			}

			ReIndexElementNodes();
		}

		private void ZigZagItems_Click(object sender, EventArgs e)
		{
			var selectedCount = elementList.SelectedIndices.Count;
			if (selectedCount < 2)
			{
				return;
			}

			using var numberDialog = new NumberDialog("ZigZag Length", "How many pixels to ZigZag?",
				Math.Min(50, selectedCount), 2, selectedCount);
			var answer = numberDialog.ShowDialog();
			if (answer == DialogResult.OK)
			{
				btnOk.Enabled = false;
				btnCancel.Enabled = false;
				PerformZigZag(numberDialog.Value);
				btnOk.Enabled = true;
				btnCancel.Enabled = true;
			}
			
		}

		private void PerformZigZag(int every){

			int[] selectedIndexes = new int[elementList.SelectedIndices.Count];
			elementList.SelectedIndices.CopyTo(selectedIndexes,0);
			
			if (selectedIndexes.Length % every == 0) // Selected pixels must be evenly divisable by zigzag length.
			{
				Cursor = Cursors.WaitCursor;
				elementList.BeginUpdate();
				for(int i = every; i< selectedIndexes.Length;i+=2*every){
					int start = i;
					int end = i + every - 1;
					while (start<end){
						// Map positions within the selection to rows in the full list.
						var startIndex = selectedIndexes[start];
						var endIndex = selectedIndexes[end];
						var i1 = elementList.Items[startIndex];
						var i2 = elementList.Items[endIndex];
						elementList.Items[startIndex] = (ListViewItem)i2.Clone();
						elementList.Items[endIndex] = i1;
						elementList.Items[startIndex].Selected = true;
						elementList.Items[endIndex].Selected = true;
						start++;
						end--;
					}
				}
				
				ReIndexElementNodes();
				elementList.EndUpdate();
				Cursor = Cursors.Default;
				return;
			}
			else
			{
				//messageBox Arguments are (Text, Title, No Button Visible, Cancel Button Visible)
				var messageBox = new MessageBoxForm("The total selected pixels must be evenly divisable by the zigzag length", "Zigzag Error", false, false);
				MessageBoxForm.msgIcon = SystemIcons.Exclamation; //this is used if you want to add a system icon to the message form.
				messageBox.ShowDialog();
			}
				
			
		}

		#region Implementation of IElementSetupHelper

		/// <inheritdoc />
		public string HelperName => "Patching Order";

		/// <inheritdoc />
		public bool Perform(IEnumerable<IElementNode> selectedNodes)
		{
			PopulateElementList(selectedNodes);

			DialogResult dr = ShowDialog();
			if (dr != DialogResult.OK)
			{
				return false;
			}

			ReIndexElementNodes();

			foreach (var elementOrder in _elementOrderLookup)
			{
				if (elementOrder.Key.Properties.Contains(OrderDescriptor.ModuleId))
				{
					var orderProperty = elementOrder.Key.Properties.Get(OrderDescriptor.ModuleId) as OrderModule;
					if (orderProperty != null)
					{
						orderProperty.Order = elementOrder.Value;
					}
				}
				else
				{
					var order = elementOrder.Key.Properties.Add(OrderDescriptor.ModuleId) as OrderModule;
					if (order != null)
					{
						order.Order = elementOrder.Value;
					}
				}
			}

			return true;
		}

		#endregion

		private void PopulateElementList(IEnumerable<IElementNode> selectedNodes)
		{
			IEnumerable<IElementNode> leafElements = selectedNodes.SelectMany(x => x.GetLeafEnumerator()).Distinct();

			_elementOrderLookup.Clear();
			foreach (var leafElement in leafElements)
			{
				int order = Int32.MaxValue;
				if (leafElement.Properties.Contains(OrderDescriptor.ModuleId))
				{
					var orderProperty = leafElement.Properties.Get(OrderDescriptor.ModuleId) as OrderModule;
					order = orderProperty.Order;
				}

				_elementOrderLookup.Add(leafElement, order);
			}

			var orderedElements = _elementOrderLookup.OrderBy(x => x.Value);

			elementList.Items.Clear();

			int count = 1;
			foreach (var el in orderedElements)
			{
				ListViewItem item = new ListViewItem(count.ToString(CultureInfo.InvariantCulture));
				item.Tag = el.Key;
				item.SubItems.Add(el.Key.Name);
				elementList.Items.Add(item);
				count++;
			}

			elementList.ColumnAutoSize();
			elementList.SetLastColumnWidth();
		}

		private void ReIndexElementNodes()
		{
			int index = 1;
			foreach (ListViewItem item in elementList.Items)
			{
				var IElementNode = item.Tag as IElementNode;
				if (IElementNode == null)
				{
					continue; // This should not happen!
				}

				item.Text = index.ToString(CultureInfo.InvariantCulture);
				_elementOrderLookup[IElementNode] = index;
				index++;
			}
			elementList.Invalidate();
		}

		private void ElementList_ItemDragDropCompleted(object sender, Common.Controls.DragDropListView.ListViewItemDragEventArgs e)
		{
			ReIndexElementNodes();
		}

		}
	}
