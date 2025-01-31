using Common.Controls.Theme;
using Common.Resources.Properties;
using NLog;
using Vixen.Rule;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Utility;
using VixenModules.Property.Order;
using VixenModules.Property.Orientation;
using Orientation = VixenModules.Property.Orientation.Orientation;

namespace VixenApplication.Setup.ElementTemplates
{
	public partial class PixelGrid : ElementTemplateBase, IElementTemplate
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();

		private string _gridName;
		private int _rows;
		private int _columns;
		private bool _rowsFirst;
		private bool _zigZag;
		private int _zigZagEvery;
		private StartLocation _startLocation;

		public PixelGrid()
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);

			_gridName = "Grid 1";
			_rows = 20;
			_columns = 32;
			_rowsFirst = true;
			_zigZag = false;
			_zigZagEvery = 0;
			_startLocation = StartLocation.BottomLeft;
		}

		public string TemplateName
		{
			get { return "Pixel Grid / Matrix"; }
		}

		public bool SetupTemplate(IEnumerable<ElementNode>? selectedNodes = null)
		{
			DialogResult result = ShowDialog();

			if (result == DialogResult.OK)
			{
				return true;
			}

			return false;
		}

		public async Task<IEnumerable<ElementNode>> GenerateElements(IEnumerable<ElementNode>? selectedNodes = null)
		{
			List<ElementNode> result = new List<ElementNode>();

			if (_gridName.Length == 0)
			{
				Logging.Error("gridname is null");
				return await Task.FromResult(result);
			}

			if (_rows < 0)
			{
				Logging.Error("negative rows");
				return await Task.FromResult(result);
			}

			if (_columns < 0)
			{
				Logging.Error("negative columns");
				return await Task.FromResult(result);
			}

			//Optimize the name check for performance. We know we are going to create a bunch of them and we can handle it ourselves more efficiently
			HashSet<string> elementNames = new HashSet<string>(VixenSystem.Nodes.Select(x => x.Name));

			ElementNode head = ElementNodeService.Instance.CreateSingle(null, NamingUtilities.Uniquify(elementNames, _gridName), true, false);
			result.Add(head);

			if (radioButtonHorizontalFirst.Checked)
			{
				OrientationModule? om;
				if (head.Properties.Contains(OrientationDescriptor.ModuleId))
				{
					om = head.Properties.Get(OrientationDescriptor.ModuleId) as OrientationModule;
				}
				else
				{
					om = head.Properties.Add(OrientationDescriptor.ModuleId) as OrientationModule;
				}

				if (om == null)
				{
					throw new InvalidOperationException("Cannot create Orientation Module");
				}
				om.Orientation = Orientation.Horizontal;
			}


			int firstlimit, secondlimit;

			if (_rowsFirst)
			{
				firstlimit = _rows;
				secondlimit = _columns;
			}
			else
			{
				firstlimit = _columns;
				secondlimit = _rows;
			}

			string firstprefix = " " + textBoxFirstPrefix.Text;
			string secondprefix = " " + textBoxSecondPrefix.Text;

			for (int i = 0; i < firstlimit; i++)
			{
				string firstname = head.Name + firstprefix + (i + 1);
				ElementNode firstnode = ElementNodeService.Instance.CreateSingle(head, NamingUtilities.Uniquify(elementNames, firstname), true, false);
				result.Add(firstnode);

				for (int j = 0; j < secondlimit; j++)
				{
					string secondname = firstnode.Name + secondprefix + (j + 1);
					ElementNode secondnode = ElementNodeService.Instance.CreateSingle(firstnode, NamingUtilities.Uniquify(elementNames, secondname), true, false);
					result.Add(secondnode);
				}
			}

			IEnumerable<ElementNode> leafNodes = Enumerable.Empty<ElementNode>();

			if (_startLocation == StartLocation.BottomLeft)
			{
				if (_zigZag)
				{
					leafNodes = result.First().GetLeafEnumerator();
					OrderModule.AddPatchingOrder(leafNodes, _zigZagEvery);
				}

				return result;
			}

			if (_startLocation == StartLocation.BottomRight)
			{
				if (radioButtonHorizontalFirst.Checked)
				{
					leafNodes = result.First().Children.SelectMany(x => x.GetLeafEnumerator()).Reverse();
				}
				else
				{
					leafNodes = result.First().Children.Reverse().SelectMany(x => x.GetLeafEnumerator());
				}
			}
			else if (_startLocation == StartLocation.TopLeft)
			{
				if (radioButtonHorizontalFirst.Checked)
				{
					leafNodes = result.First().Children.Reverse().SelectMany(x => x.GetLeafEnumerator());
				}
				else
				{
					leafNodes = result.First().Children.SelectMany(x => x.GetLeafEnumerator().Reverse());
				}
			}
			else if (_startLocation == StartLocation.TopRight)
			{
				leafNodes = result.First().GetLeafEnumerator().Reverse();
			}

			if (_zigZag)
			{
				OrderModule.AddPatchingOrder(leafNodes, _zigZagEvery);
			}
			else
			{
				OrderModule.AddPatchingOrder(leafNodes);
			}

			return await Task.FromResult(result);
		}



		private void PixelGrid_Load(object sender, EventArgs e)
		{
			textBoxName.Text = _gridName;
			numericUpDownHeight.Value = _rows;
			numericUpDownWidth.Value = _columns;
			if (_rowsFirst)
				radioButtonHorizontalFirst.Checked = true;
			else
				radioButtonVerticalFirst.Checked = true;
			lblEveryValue.Text = _zigZagEvery.ToString();
			chkZigZag.Checked = _zigZag;
			switch (_startLocation)
			{
				case StartLocation.BottomLeft:
					radioBottomLeft.Checked = true;
					break;
				case StartLocation.BottomRight:
					radioBottomRight.Checked = true;
					break;
				case StartLocation.TopLeft:
					radioTopLeft.Checked = true;
					break;
				case StartLocation.TopRight:
					radioTopRight.Checked = true;
					break;
			}
		}

		private void PixelGrid_FormClosed(object sender, FormClosedEventArgs e)
		{
			_gridName = textBoxName.Text;
			_rows = Decimal.ToInt32(numericUpDownHeight.Value);
			_columns = Decimal.ToInt32(numericUpDownWidth.Value);
			_rowsFirst = radioButtonHorizontalFirst.Checked;
			_zigZag = chkZigZag.Checked;
			_zigZagEvery = Convert.ToInt32(lblEveryValue.Text);
			if (radioTopRight.Checked)
			{
				_startLocation = StartLocation.TopRight;
			}
			else if (radioTopLeft.Checked)
			{
				_startLocation = StartLocation.TopLeft;
			}
			else if (radioBottomRight.Checked)
			{
				_startLocation = StartLocation.BottomRight;
			}
			else
			{
				_startLocation = StartLocation.BottomLeft;
			}
		}

		private void radioButtonOrientation_CheckedChanged(object sender, EventArgs e)
		{
			string temp = textBoxFirstPrefix.Text;
			textBoxFirstPrefix.Text = textBoxSecondPrefix.Text;
			textBoxSecondPrefix.Text = temp;
			UpdateZigZag();
		}

		private void chkZigZag_CheckedChanged(object sender, EventArgs e)
		{
			UpdateZigZag();
		}

		private void UpdateZigZag()
		{
			if (chkZigZag.Checked)
			{
				if (radioButtonHorizontalFirst.Checked)
				{
					lblEveryValue.Text = numericUpDownWidth.Value.ToString();
				}
				else
				{
					lblEveryValue.Text = numericUpDownHeight.Value.ToString();
				}
			}
			else
			{
				lblEveryValue.Text = @"0";
			}
		}

		private void numericUpDownHeight_ValueChanged(object sender, EventArgs e)
		{
			UpdateZigZag();
		}

		private void numericUpDownWidth_ValueChanged(object sender, EventArgs e)
		{
			UpdateZigZag();
		}

	}
}
