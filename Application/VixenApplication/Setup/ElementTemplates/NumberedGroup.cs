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
	public partial class NumberedGroup : BaseForm, IElementTemplate
	{
		private static Logger Logging = LogManager.GetCurrentClassLogger();

		private string groupname;
		private string prefix;
		private int count;

		public NumberedGroup()
		{
			InitializeComponent();
			Icon = Resources.Icon_Vixen3;
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);

			groupname = "Minitrees";
			prefix = "Tree";
			count = 10;
		}

		public string TemplateName
		{
			get { return "Generic Numbered Group"; }
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

			if (groupname.Length == 0) {
				Logging.Error("groupname is null");
				return result;
			}

			if (prefix.Length == 0) {
				Logging.Error("prefix is null");
				return result;
			}

			if (count < 0) {
				Logging.Error("negative count");
				return result;
			}

			// changed my mind; always make the group at the root level.

			//ElementNode parent = null;
			//if (selectedNodes != null) {
			//    parent = selectedNodes.First();
			//    if (selectedNodes.Count() > 0) {
			//        Logging.Warn("multiple parent nodes selected; creating new nodes under first parent: " + parent.Name);
			//    }
			//}

			//Optimize the name check for performance. We know we are going to create a bunch of them and we can handle it ourselves more efficiently
			HashSet<string> elementNames = new HashSet<string>(VixenSystem.Nodes.Select(x => x.Name));

			ElementNode grouphead = ElementNodeService.Instance.CreateSingle(null, TemplateUtilities.Uniquify(elementNames, groupname), true, false);
			result.Add(grouphead);

			for (int i = 0; i < count; i++) {
				string newname = prefix + "-" + (i + 1);
				ElementNode newnode = ElementNodeService.Instance.CreateSingle(grouphead, TemplateUtilities.Uniquify(elementNames, newname), true, false);
				result.Add(newnode);
			}

			return result;
		}

		private void NumberedGroup_Load(object sender, EventArgs e)
		{
			textBoxGroupName.Text = groupname;
			textBoxItemPrefix.Text = prefix;
			numericUpDownItemCount.Value = count;
		}

		private void NumberedGroup_FormClosed(object sender, FormClosedEventArgs e)
		{
			groupname = textBoxGroupName.Text;
			prefix = textBoxItemPrefix.Text;
			count = Decimal.ToInt32(numericUpDownItemCount.Value);
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
	}
}
