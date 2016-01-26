using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls;
using Vixen.Rule;
using Vixen.Services;
using Vixen.Sys;

namespace VixenApplication.Setup.ElementTemplates
{
	class SingleItem : IElementTemplate
	{
		private string itemName;

		public SingleItem()
		{
			itemName = "New Item";
		}

		public string TemplateName
		{
			get { return "Single Item"; }
		}
		public bool TemplateEnabled { get { return true; } }
		public bool SetupTemplate(IEnumerable<ElementNode> selectedNodes = null)
		{
			using (TextDialog td = new TextDialog("New Element Name?", "Element Name", itemName, true)) {
				DialogResult dr = td.ShowDialog();
				if (dr == DialogResult.OK) {
					itemName = td.Response;
					if (itemName == "") {
						itemName = "New Item";
					}
					return true;
				}
			}
			return false;
		}

		public IEnumerable<ElementNode> GenerateElements(IEnumerable<ElementNode> selectedNodes = null)
		{
			ElementNode newNode = ElementNodeService.Instance.CreateSingle(null, itemName);
			return new[] {newNode};
		}
	}
}
