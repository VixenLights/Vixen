using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NLog;
using Vixen.Rule;
using Vixen.Services;
using Vixen.Sys;

namespace VixenApplication.Setup.ElementTemplates
{
	public partial class Starburst : Form, IElementTemplate
	{
		private static Logger Logging = LogManager.GetCurrentClassLogger();

		private string treename;
		private int stringcount;
		private bool pixeltree;
		private int pixelsperstring;

		public Starburst()
		{
			InitializeComponent();

			treename = "Starburst";
			stringcount = 8;
			pixeltree = false;
			pixelsperstring = 50;
		}

		public string TemplateName
		{
			get { return "Starburst"; }
		}

		public bool SetupTemplate(IEnumerable<ElementNode> selectedNodes = null)
		{
			DialogResult result = ShowDialog();

			if (result == DialogResult.OK)
				return true;

			return false;
		}

		public IEnumerable<ElementNode> GenerateElements(IEnumerable<ElementNode> selectedNodes = null)
		{
			List<ElementNode> result = new List<ElementNode>();

			if (treename.Length == 0) {
				Logging.Error("starburst is null");
				return result;
			}

			if (stringcount < 0) {
				Logging.Error("negative count");
				return result;
			}

			if (pixeltree && pixelsperstring < 0) {
				Logging.Error("negative pixelsperstring");
				return result;
			}

			ElementNode head = ElementNodeService.Instance.CreateSingle(null, treename);
			result.Add(head);

			for (int i = 0; i < stringcount; i++) {
				string stringname = head.Name + " S" + (i + 1);
				ElementNode stringnode = ElementNodeService.Instance.CreateSingle(head, stringname);
				result.Add(stringnode);

				if (pixeltree) {
					for (int j = 0; j < pixelsperstring; j++) {
						string pixelname = stringnode.Name + " Px" + (j + 1);

						ElementNode pixelnode = ElementNodeService.Instance.CreateSingle(stringnode, pixelname);
						result.Add(pixelnode);
					}
				}
			}

			return result;
		}

		private void checkBoxPixelTree_CheckedChanged(object sender, EventArgs e)
		{
			numericUpDownPixelsPerString.Enabled = checkBoxPixelTree.Checked;
		}

		private void Megatree_Load(object sender, EventArgs e)
		{
			textBoxTreeName.Text = treename;
			numericUpDownStrings.Value = stringcount;
			checkBoxPixelTree.Checked = pixeltree;
			numericUpDownPixelsPerString.Value = pixelsperstring;
		}

		private void Megatree_FormClosed(object sender, FormClosedEventArgs e)
		{
			treename = textBoxTreeName.Text;
			stringcount = Decimal.ToInt32(numericUpDownStrings.Value);
			pixeltree = checkBoxPixelTree.Checked ;
			pixelsperstring = Decimal.ToInt32(numericUpDownPixelsPerString.Value);
		}
	}
}
