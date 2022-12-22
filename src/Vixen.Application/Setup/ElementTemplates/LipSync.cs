﻿using Common.Controls.Theme;
using Common.Resources.Properties;
using NLog;
using Vixen.Rule;
using Vixen.Services;
using Vixen.Sys;

namespace VixenApplication.Setup.ElementTemplates
{
	public partial class LipSync : ElementTemplateBase, IElementTemplate
    {
        private static Logger Logging = LogManager.GetCurrentClassLogger();
        private static string[] templateStrings = { "Outline", "Eyes Open", "Eyes Closed", "Mouth Top", "Mouth Middle", "Mouth Bottom", "Mouth Narrow", "Mouth O" };

        private string treename;

        public LipSync()
        {
			InitializeComponent();
			Icon = Resources.Icon_Vixen3;
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;
            treename = "LipSync";
        }

        public string TemplateName
        {
            get { return "LipSync"; }
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

            if (treename.Length == 0)
            {
                Logging.Error("LipSync name is null");
                return await Task.FromResult(result); ;
            }

            ElementNode head = ElementNodeService.Instance.CreateSingle(null, treename);
            result.Add(head);

            List<string> stringNames = new List<string>();

            foreach(string stringName in templateStrings)
            {
                ElementNode stringnode = ElementNodeService.Instance.CreateSingle(head, treename + " " + stringName);
                result.Add(stringnode);
                stringNames.Add(stringName);
            }

            return await Task.FromResult(result); ;
        }

        private void LipSync_Load(object sender, EventArgs e)
        {
            textBoxTreeName.Text = treename;
        }

        private void LipSync_FormClosed(object sender, FormClosedEventArgs e)
        {
            treename = textBoxTreeName.Text;
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
