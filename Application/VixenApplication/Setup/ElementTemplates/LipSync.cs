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
using Vixen.Module.App;

namespace VixenApplication.Setup.ElementTemplates
{
    public partial class LipSync : Form, IElementTemplate
    {
        private static Logger Logging = LogManager.GetCurrentClassLogger();
        private static string[] templateStrings = { "Outline", "Eyes Open", "Eyes Closed", "Mouth Top", "Mouth Middle", "Mouth Bottom", "Mouth Narrow", "Mouth O" };

        private string treename;

        public LipSync()
        {
            InitializeComponent();
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

        public IEnumerable<ElementNode> GenerateElements(IEnumerable<ElementNode> selectedNodes = null)
        {
            List<ElementNode> result = new List<ElementNode>();

            if (treename.Length == 0)
            {
                Logging.Error("LipSync name is null");
                return result;
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

            return result;
        }

        private void LipSync_Load(object sender, EventArgs e)
        {
            textBoxTreeName.Text = treename;
        }

        private void LipSync_FormClosed(object sender, FormClosedEventArgs e)
        {
            treename = textBoxTreeName.Text;
        }
    }
}
