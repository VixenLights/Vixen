using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
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
			Icon = Resources.Icon_Vixen3;
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			_contextMenu.Renderer = new ThemeToolStripRenderer();
			elementList.ItemDragDropCompleted += ElementList_ItemDragDropCompleted;
			elementList.MouseClick += ElementListOnMouseClick;
			elementList.KeyDown += OnKeyDown;
		}

		protected void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.A | e.Control)
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
			NumberDialog numberDialog = new NumberDialog("ZigZag Length", "How many pixels to ZigZag?", 50, 2, elementList.SelectedIndices.Count);
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
						//Swap places
						var i1 = elementList.Items[start];
						var i2 = elementList.Items[end];
						elementList.Items[start] = (ListViewItem)i2.Clone();
						elementList.Items[end] = i1;
						elementList.Items[start].Selected = true;
						elementList.Items[end].Selected = true;
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
				MessageBoxForm.msgIcon = System.Drawing.SystemIcons.Exclamation; //this is used if you want to add a system icon to the message form.
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
