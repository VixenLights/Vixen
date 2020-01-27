using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
			var selectedindexes = elementList.SelectedIndices;
			var indexMap = new Dictionary<int, ListViewItem>();
			int IterationCounter = 0;

			NumberDialog numberDialog = new NumberDialog("ZigZag Length", "How many pixels to ZigZag?", 2, 2, selectedindexes.Count);
			DialogResult Answer;
			do
			{
				Answer = numberDialog.ShowDialog();
				if (Answer == DialogResult.OK)
				{
					if (selectedindexes.Count % numberDialog.Value == 0) // Selected pixels must be evenly divisable by zigzag length.
					{
						int ZigZagLength = numberDialog.Value;
						for (int i = 1; i < (selectedindexes.Count / (ZigZagLength * 2) + .5); i++)
						{
							for (int Zig = 1; Zig <= ZigZagLength; Zig++)
							{
								IterationCounter++;
								indexMap.Add(selectedindexes[IterationCounter - 1], elementList.Items[selectedindexes[IterationCounter - 1]]);
							}

							if (IterationCounter >= selectedindexes.Count)
							{
								break;
							}
							int LastZig = IterationCounter;

							for (int Zag = ZigZagLength; Zag >= 1; Zag--)
							{
								IterationCounter++;
								indexMap.Add(selectedindexes[LastZig + Zag - 1], elementList.Items[selectedindexes[IterationCounter - 1]]);
							}

							if (IterationCounter >= selectedindexes.Count)
							{
								break;
							}
						}

						foreach (var item in indexMap)
						{
							elementList.Items.RemoveAt(item.Key);
							elementList.Items.Insert(item.Key, (ListViewItem)item.Value.Clone());
							elementList.Items[item.Key].Selected = true;
						}

						ReIndexElementNodes();
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
			} while (Answer == DialogResult.OK);
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
