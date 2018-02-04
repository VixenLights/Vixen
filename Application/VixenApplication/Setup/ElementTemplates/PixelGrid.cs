using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using NLog;
using Vixen.Rule;
using Vixen.Services;
using Vixen.Sys;

namespace VixenApplication.Setup.ElementTemplates
{
	public partial class PixelGrid : BaseForm, IElementTemplate
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

		public IEnumerable<ElementNode> GenerateElements(IEnumerable<ElementNode> selectedNodes = null)
		{
			List<ElementNode> result = new List<ElementNode>();

			if (gridname.Length == 0) {
				Logging.Error("gridname is null");
				return result;
			}

			if (rows < 0) {
				Logging.Error("negative rows");
				return result;
			}

			if (columns < 0) {
				Logging.Error("negative columns");
				return result;
			}

			//Optimize the name check for performance. We know we are going to create a bunch of them and we can handle it ourselves more efficiently
			HashSet<string> elementNames = new HashSet<string>(VixenSystem.Nodes.Select(x => x.Name));

			ElementNode head = ElementNodeService.Instance.CreateSingle(null, TemplateUtilities.Uniquify(elementNames,gridname), true, false);
			result.Add(head);

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
				ElementNode firstnode = ElementNodeService.Instance.CreateSingle(head, TemplateUtilities.Uniquify(elementNames,firstname),true, false);
				result.Add(firstnode);

				for (int j = 0; j < secondlimit; j++) {
					string secondname = firstnode.Name + secondprefix + (j + 1);
					ElementNode secondnode = ElementNodeService.Instance.CreateSingle(firstnode, TemplateUtilities.Uniquify(elementNames, secondname), true, false);
					result.Add(secondnode);
				}
			}

			return result;
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
