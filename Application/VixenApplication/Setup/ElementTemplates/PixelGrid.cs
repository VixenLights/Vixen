using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using NLog;
using Vixen.Rule;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Utility;
using VixenModules.Property.Orientation;
using Orientation = VixenModules.Property.Orientation.Orientation;

namespace VixenApplication.Setup.ElementTemplates
{
	public partial class PixelGrid : ElementTemplateBase, IElementTemplate
	{
		private static Logger Logging = LogManager.GetCurrentClassLogger();

		private string gridname;
		private int rows;
		private int columns;
		private bool rowsfirst;
		
		public PixelGrid()
		{
			InitializeComponent();
			Icon = Resources.Icon_Vixen3;
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);

			gridname = "Grid";
			rows = 20;
			columns = 32;
			rowsfirst = true;
		}

		public string TemplateName
		{
			get { return "Pixel Grid"; }
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

			if (gridname.Length == 0) {
				Logging.Error("gridname is null");
				return await Task.FromResult(result);
			}

			if (rows < 0) {
				Logging.Error("negative rows");
				return await Task.FromResult(result);
			}

			if (columns < 0) {
				Logging.Error("negative columns");
				return await Task.FromResult(result);
			}

			//Optimize the name check for performance. We know we are going to create a bunch of them and we can handle it ourselves more efficiently
			HashSet<string> elementNames = new HashSet<string>(VixenSystem.Nodes.Select(x => x.Name));

			ElementNode head = ElementNodeService.Instance.CreateSingle(null, NamingUtilities.Uniquify(elementNames,gridname), true, false);
			result.Add(head);

			if (radioButtonHorizontalFirst.Checked)
			{
				OrientationModule om;
				if (head.Properties.Contains(OrientationDescriptor.ModuleId))
				{
					om = head.Properties.Get(OrientationDescriptor.ModuleId) as OrientationModule;
				}
				else
				{
					om = head.Properties.Add(OrientationDescriptor.ModuleId) as OrientationModule;
				}

				om.Orientation = Orientation.Horizontal;
			}


			int firstlimit, secondlimit;

			if (rowsfirst) {
				firstlimit = rows;
				secondlimit = columns;
			} else {
				firstlimit = columns;
				secondlimit = rows;
			}

			string firstprefix = " " + textBoxFirstPrefix.Text;
			string secondprefix = " " + textBoxSecondPrefix.Text;

			for (int i = 0; i < firstlimit; i++) {
				string firstname = head.Name + firstprefix + (i + 1);
				ElementNode firstnode = ElementNodeService.Instance.CreateSingle(head, NamingUtilities.Uniquify(elementNames,firstname),true, false);
				result.Add(firstnode);

				for (int j = 0; j < secondlimit; j++) {
					string secondname = firstnode.Name + secondprefix + (j + 1);
					ElementNode secondnode = ElementNodeService.Instance.CreateSingle(firstnode, NamingUtilities.Uniquify(elementNames, secondname), true, false);
					result.Add(secondnode);
				}
			}

			return await Task.FromResult(result);
		}
		
		private void PixelGrid_Load(object sender, EventArgs e)
		{
			textBoxName.Text = gridname;
			numericUpDownHeight.Value = rows;
			numericUpDownWidth.Value = columns;
			if (rowsfirst)
				radioButtonHorizontalFirst.Checked = true;
			else
				radioButtonVerticalFirst.Checked = true;
		}

		private void PixelGrid_FormClosed(object sender, FormClosedEventArgs e)
		{
			gridname = textBoxName.Text;
			rows = Decimal.ToInt32(numericUpDownHeight.Value);
			columns = Decimal.ToInt32(numericUpDownWidth.Value);
			rowsfirst = radioButtonHorizontalFirst.Checked;
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

		private void radioButtonOrientation_CheckedChanged(object sender, EventArgs e)
		{
			string temp = textBoxFirstPrefix.Text;
			textBoxFirstPrefix.Text = textBoxSecondPrefix.Text;
			textBoxSecondPrefix.Text = temp;
		}
	}
}
