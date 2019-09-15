using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using NLog;
using Vixen.Rule;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Utility;

namespace VixenApplication.Setup.ElementTemplates
{
	public partial class NumberedGroup : BaseForm, IElementTemplate
	{
		private static Logger Logging = LogManager.GetCurrentClassLogger();

		public NumberedGroup():this(@"Group", @"Item", 10)
		{
			
		}

		internal NumberedGroup(string groupName, string prefix, int count)
		{
			InitializeComponent();
			Icon = Resources.Icon_Vixen3;
			ThemeUpdateControls.UpdateControls(this);

			GroupName = groupName;
			Prefix = prefix;
			Count = count;
		}

		public string GroupName { get; set; }

		public string Prefix { get; set; }

		public int Count { get; set; }
		
		public virtual string TemplateName
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

			if (GroupName.Length == 0) {
				Logging.Error("groupname is null");
				return result;
			}

			if (Prefix.Length == 0) {
				Logging.Error("prefix is null");
				return result;
			}

			if (Count < 0) {
				Logging.Error("negative count");
				return result;
			}

			//Optimize the name check for performance. We know we are going to create a bunch of them and we can handle it ourselves more efficiently
			HashSet<string> elementNames = new HashSet<string>(VixenSystem.Nodes.Select(x => x.Name));

			ElementNode grouphead = ElementNodeService.Instance.CreateSingle(null, NamingUtilities.Uniquify(elementNames, GroupName), true, false);
			result.Add(grouphead);

			for (int i = 0; i < Count; i++) {
				string newname = Prefix + "-" + (i + 1);
				ElementNode newnode = ElementNodeService.Instance.CreateSingle(grouphead, NamingUtilities.Uniquify(elementNames, newname), true, false);
				result.Add(newnode);
			}

			return result;
		}

		private void NumberedGroup_Load(object sender, EventArgs e)
		{
			textBoxGroupName.Text = GroupName;
			textBoxItemPrefix.Text = Prefix;
			numericUpDownItemCount.Value = Count;
		}

		private void NumberedGroup_FormClosed(object sender, FormClosedEventArgs e)
		{
			GroupName = textBoxGroupName.Text;
			Prefix = textBoxItemPrefix.Text;
			Count = Decimal.ToInt32(numericUpDownItemCount.Value);
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
