using System.Globalization;
using Common.Controls.Theme;
using Common.Resources.Properties;
using NLog;
using Vixen.Rule;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Utility;
using VixenModules.Property.Order;

namespace VixenApplication.Setup.ElementTemplates
{
	public partial class Megatree : ElementTemplateBase, IElementTemplate
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();

		private string _treeName;
		private int _stringCount;
		private bool _pixelTree;
		private int _pixelsPerString;
		private StartLocation _startLocation;
		private bool _zigZag;
		private int _zigZagEvery;

		public Megatree()
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);

			_treeName = "Megatree";
			_stringCount = 16;
			_pixelTree = false;
			_pixelsPerString = 50;
		}

		public string TemplateName
		{
			get { return "Megatree"; }
		}

		public bool SetupTemplate(IEnumerable<ElementNode>? selectedNodes = null)
		{
			DialogResult result = ShowDialog();

			if (result == DialogResult.OK)
				return true;

			return false;
		}

		public async Task<IEnumerable<ElementNode>> GenerateElements(IEnumerable<ElementNode>? selectedNodes = null)
		{
			List<ElementNode> result = new List<ElementNode>();

			if (_treeName.Length == 0)
			{
				Logging.Error("treename is null");
				return await Task.FromResult(result);
			}

			if (_stringCount < 0)
			{
				Logging.Error("negative count");
				return await Task.FromResult(result);
			}

			if (_pixelTree && _pixelsPerString < 0)
			{
				Logging.Error("negative pixelsperstring");
				return await Task.FromResult(result);
			}

			//Optimize the name check for performance. We know we are going to create a bunch of them and we can handle it ourselves more efficiently
			HashSet<string> elementNames = new HashSet<string>(VixenSystem.Nodes.Select(x => x.Name));

			ElementNode head = ElementNodeService.Instance.CreateSingle(null, NamingUtilities.Uniquify(elementNames, _treeName), true, false);
			result.Add(head);

			for (int i = 0; i < _stringCount; i++)
			{
				string stringname = head.Name + " " + textBoxStringPrefix.Text + (i + 1);
				ElementNode stringnode = ElementNodeService.Instance.CreateSingle(head, NamingUtilities.Uniquify(elementNames, stringname), true, false);
				result.Add(stringnode);

				if (_pixelTree)
				{
					for (int j = 0; j < _pixelsPerString; j++)
					{
						string pixelname = stringnode.Name + " " + textBoxPixelPrefix.Text + (j + 1);

						ElementNode pixelnode = ElementNodeService.Instance.CreateSingle(stringnode, NamingUtilities.Uniquify(elementNames, pixelname), true, false);
						result.Add(pixelnode);
					}
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
				leafNodes = result.First().Children.SelectMany(x => x.GetLeafEnumerator().Reverse());
			}
			else if (_startLocation == StartLocation.TopLeft)
			{
				leafNodes = result.First().Children.Reverse().SelectMany(x => x.GetLeafEnumerator());
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

		private void checkBoxPixelTree_CheckedChanged(object sender, EventArgs e)
		{
			numericUpDownPixelsPerString.Enabled = checkBoxPixelTree.Checked;
			textBoxPixelPrefix.Enabled = checkBoxPixelTree.Checked;
			grpPatching.Enabled = checkBoxPixelTree.Checked;
			grpWireStart.Enabled = checkBoxPixelTree.Checked;
			lblEveryValue.Enabled = checkBoxPixelTree.Checked;
			chkZigZag.Enabled = checkBoxPixelTree.Checked;
		}

		private void Megatree_Load(object sender, EventArgs e)
		{
			textBoxTreeName.Text = _treeName;
			numericUpDownStrings.Value = _stringCount;
			checkBoxPixelTree.Checked = _pixelTree;
			numericUpDownPixelsPerString.Value = _pixelsPerString;
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
			}
		}

		private void Megatree_FormClosed(object sender, FormClosedEventArgs e)
		{
			_treeName = textBoxTreeName.Text;
			_stringCount = Decimal.ToInt32(numericUpDownStrings.Value);
			_pixelTree = checkBoxPixelTree.Checked;
			_pixelsPerString = Decimal.ToInt32(numericUpDownPixelsPerString.Value);
			_zigZag = chkZigZag.Checked;
			_zigZagEvery = Convert.ToInt32(lblEveryValue.Text);
			if (radioBottomRight.Checked)
			{
				_startLocation = StartLocation.BottomRight;
			}
			else
			{
				_startLocation = StartLocation.BottomLeft;
			}
		}

		private void chkZigZag_CheckedChanged(object sender, EventArgs e)
		{
			UpdateZigZag();
		}

		private void UpdateZigZag()
		{
			if (chkZigZag.Checked)
			{

				lblEveryValue.Text = numericUpDownPixelsPerString.Value.ToString(CultureInfo.InvariantCulture);

			}
			else
			{
				lblEveryValue.Text = @"0";
			}
		}

		private void numericUpDownPixelsPerString_ValueChanged(object sender, EventArgs e)
		{
			UpdateZigZag();
		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}


	}
}
