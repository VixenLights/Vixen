using Common.Controls.Theme;
using Common.Resources.Properties;
using NLog;
using Vixen.Rule;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Utility;

namespace VixenApplication.Setup.ElementTemplates
{
	public partial class Icicles : ElementTemplateBase, IElementTemplate
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();

		private string _treeName;
		private int _stringCount;
		private string _pixelsPerStringPattern;

		public Icicles()
		{
			InitializeComponent();
			Icon = Resources.Icon_Vixen3;
			ThemeUpdateControls.UpdateControls(this);

			_treeName = "Icicles";
			_stringCount = 50;
			_pixelsPerStringPattern = "7,9,5";
		}

		public string TemplateName
		{
			get { return "Icicles"; }
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
				Logging.Error("treename is null");
				return await Task.FromResult(result);
			}

			if (_stringCount < 0)
			{
				Logging.Error("negative count");
				return await Task.FromResult(result);
			}

			//Optimize the name check for performance. We know we are going to create a bunch of them and we can handle it ourselves more efficiently
			HashSet<string> elementNames = new HashSet<string>(VixenSystem.Nodes.Select(x => x.Name));

			ElementNode head = ElementNodeService.Instance.CreateSingle(null, NamingUtilities.Uniquify(elementNames, _treeName), true, false);
			result.Add(head);

			int ii = 0;
			//Grabs the individual string sizes and removes any empty ones just in case the user entered something like 4,5,,9
			string[] pixelsPerStringArray = _pixelsPerStringPattern.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

			//Loops through the number of Strings to be added.
			for (int i = 0; i < _stringCount; i++)
			{
				if (i % pixelsPerStringArray.Count() == 0)
				{
					ii = 0;
				}
				else
				{
					ii++;
				}

				int pixelsPerString = Convert.ToInt16(pixelsPerStringArray[ii]);

				string stringname = head.Name + " " + textBoxStringPrefix.Text + (i + 1);
				ElementNode stringnode = ElementNodeService.Instance.CreateSingle(head, NamingUtilities.Uniquify(elementNames, stringname), true, false);
				result.Add(stringnode);

				//Loops through the and add each pixel to the String. Number of Pixels is determined by the String Pattern values.
				for (int j = 0; j < pixelsPerString; j++)
				{
					string pixelname = stringnode.Name + " " + textBoxPixelPrefix.Text + (j + 1);

					ElementNode pixelnode = ElementNodeService.Instance.CreateSingle(stringnode,
						NamingUtilities.Uniquify(elementNames, pixelname), true, false);
					result.Add(pixelnode);
				}
			}

			return await Task.FromResult(result);
		}

		private void Icicles_Load(object sender, EventArgs e)
		{
			textBoxTreeName.Text = _treeName;
			numericUpDownStrings.Value = _stringCount;
			textBoxStringPattern.Text = _pixelsPerStringPattern;
		}

		private void Icicles_FormClosed(object sender, FormClosedEventArgs e)
		{
			_treeName = textBoxTreeName.Text;
			_stringCount = Decimal.ToInt32(numericUpDownStrings.Value);
			_pixelsPerStringPattern = textBoxStringPattern.Text;
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

		private void textBoxStringPattern_KeyPress(object sender, KeyPressEventArgs e)
		{
			//Ensures only numbers, commas and backspace can be used in the Pattern textbox.
			if (e.KeyChar == ',' || char.IsControl(e.KeyChar))
				e.Handled = false;
			else if (!int.TryParse(e.KeyChar.ToString(), out _))
				e.Handled = true;
		}
	}
}
