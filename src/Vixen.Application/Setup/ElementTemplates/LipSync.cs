using Common.Controls.Theme;
using Common.Resources.Properties;
using NLog;
using Vixen.Rule;
using Vixen.Services;
using Vixen.Sys;

namespace VixenApplication.Setup.ElementTemplates
{
	public partial class LipSync : ElementTemplateBase, IElementTemplate
    {
        private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
        private static readonly string[] TemplateStrings = { "Outline", "Eyes Open", "Eyes Closed", "Mouth Top", "Mouth Middle", "Mouth Bottom", "Mouth Narrow", "Mouth O" };

        private string _treeName;

        public LipSync()
        {
			InitializeComponent();
			Icon = Resources.Icon_Vixen3;
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;
            _treeName = "LipSync";
        }

        public string TemplateName
        {
            get { return "LipSync"; }
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
                Logging.Error("LipSync name is null");
                return await Task.FromResult(result);
            }

            ElementNode head = ElementNodeService.Instance.CreateSingle(null, _treeName);
            result.Add(head);

            foreach(string stringName in TemplateStrings)
            {
                ElementNode stringNode = ElementNodeService.Instance.CreateSingle(head, _treeName + " " + stringName);
                result.Add(stringNode);
            }

            return await Task.FromResult(result); 
        }

        private void LipSync_Load(object sender, EventArgs e)
        {
            textBoxTreeName.Text = _treeName;
        }

        private void LipSync_FormClosed(object sender, FormClosedEventArgs e)
        {
            _treeName = textBoxTreeName.Text;
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
