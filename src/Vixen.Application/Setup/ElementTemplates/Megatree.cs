﻿using System.Globalization;
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
		private static Logger Logging = LogManager.GetCurrentClassLogger();

		private string treename;
		private int stringcount;
		private bool pixeltree;
		private int pixelsperstring;
		private StartLocation startLocation;
		private bool zigZag;
		private int zigZagEvery;

		public Megatree()
		{
			InitializeComponent();
			Icon = Resources.Icon_Vixen3;
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);

			treename = "Megatree";
			stringcount = 16;
			pixeltree = false;
			pixelsperstring = 50;
		}

		public string TemplateName
		{
			get { return "Megatree"; }
		}

		public bool SetupTemplate(IEnumerable<ElementNode> selectedNodes = null)
		{
			DialogResult result = ShowDialog();

			if (result == DialogResult.OK)
				return true;

			return false;
		}

		public async Task<IEnumerable<ElementNode>> GenerateElements(IEnumerable<ElementNode> selectedNodes = null)
		{
			List<ElementNode> result = new List<ElementNode>();

			if (treename.Length == 0) {
				Logging.Error("treename is null");
				return await Task.FromResult(result);
			}

			if (stringcount < 0) {
				Logging.Error("negative count");
				return await Task.FromResult(result);
			}

			if (pixeltree && pixelsperstring < 0) {
				Logging.Error("negative pixelsperstring");
				return await Task.FromResult(result);
			}

			//Optimize the name check for performance. We know we are going to create a bunch of them and we can handle it ourselves more efficiently
			HashSet<string> elementNames = new HashSet<string>(VixenSystem.Nodes.Select(x => x.Name));

			ElementNode head = ElementNodeService.Instance.CreateSingle(null, NamingUtilities.Uniquify(elementNames, treename), true, false);
			result.Add(head);

			for (int i = 0; i < stringcount; i++) {
				string stringname = head.Name + " " + textBoxStringPrefix.Text + (i + 1);
				ElementNode stringnode = ElementNodeService.Instance.CreateSingle(head, NamingUtilities.Uniquify(elementNames, stringname), true, false);
				result.Add(stringnode);

				if (pixeltree) {
					for (int j = 0; j < pixelsperstring; j++) {
						string pixelname = stringnode.Name + " " + textBoxPixelPrefix.Text + (j + 1);

						ElementNode pixelnode = ElementNodeService.Instance.CreateSingle(stringnode, NamingUtilities.Uniquify(elementNames, pixelname), true, false);
						result.Add(pixelnode);
					}
				}
			}

			IEnumerable<ElementNode> leafNodes = Enumerable.Empty<ElementNode>();

			if (startLocation == StartLocation.BottomLeft)
			{
				if (zigZag)
				{
					leafNodes = result.First().GetLeafEnumerator();
					OrderModule.AddPatchingOrder(leafNodes, zigZagEvery);
				}

				return result;
			}

			if (startLocation == StartLocation.BottomRight)
			{
				leafNodes = result.First().Children.SelectMany(x => x.GetLeafEnumerator().Reverse());
			}
			else if (startLocation == StartLocation.TopLeft)
			{
				leafNodes = result.First().Children.Reverse().SelectMany(x => x.GetLeafEnumerator());
			}
			else if (startLocation == StartLocation.TopRight)
			{
				leafNodes = result.First().GetLeafEnumerator().Reverse();
			}

			if (zigZag)
			{
				OrderModule.AddPatchingOrder(leafNodes, zigZagEvery);
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
			textBoxTreeName.Text = treename;
			numericUpDownStrings.Value = stringcount;
			checkBoxPixelTree.Checked = pixeltree;
			numericUpDownPixelsPerString.Value = pixelsperstring;
			lblEveryValue.Text = zigZagEvery.ToString();
			chkZigZag.Checked = zigZag;
			switch (startLocation)
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
			treename = textBoxTreeName.Text;
			stringcount = Decimal.ToInt32(numericUpDownStrings.Value);
			pixeltree = checkBoxPixelTree.Checked ;
			pixelsperstring = Decimal.ToInt32(numericUpDownPixelsPerString.Value);
			zigZag = chkZigZag.Checked;
			zigZagEvery = Convert.ToInt32(lblEveryValue.Text);
			if (radioBottomRight.Checked)
			{
				startLocation = StartLocation.BottomRight;
			}
			else
			{
				startLocation = StartLocation.BottomLeft;
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
				lblEveryValue.Text = "0";
			}
		}

		private void numericUpDownPixelsPerString_ValueChanged(object sender, EventArgs e)
		{
			UpdateZigZag();
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImage;
		}

		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}

		
	}
}
