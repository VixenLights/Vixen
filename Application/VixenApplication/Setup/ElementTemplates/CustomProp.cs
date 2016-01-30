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
using Vixen.Module.App;
using Vixen.Module;
using System.IO;
using Vixen;

namespace VixenApplication.Setup.ElementTemplates
{
	public partial class CustomProp : BaseForm, IElementTemplate
	{
		private static Logger Logging = LogManager.GetCurrentClassLogger();
		private static string[] templateStrings = { "Outline", "Eyes Open", "Eyes Closed", "Mouth Top", "Mouth Middle", "Mouth Bottom", "Mouth Narrow", "Mouth O" };

		private string treename;

		public CustomProp()
		{
			InitializeComponent();
			Icon = Resources.Icon_Vixen3;
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;
			treename = "CustomProps";
			cboPropName.DataSource = Props;
			cboPropName.DisplayMember = "Name";

		}

		public string TemplateName
		{
			get { return "Custom Prop"; }
		}

		public bool SetupTemplate(IEnumerable<ElementNode> selectedNodes = null)
		{
			DialogResult result = ShowDialog();

			if (result == DialogResult.OK)
				return true;

			return false;
		}

		string defaultPath = Path.Combine(Vixen.Sys.Paths.ModuleDataFilesPath, "VixenDisplayPreview", "CustomProps");

		private Prop FromFile(string fileName)
		{
			Prop output = new Prop();
			//using (var fs = new FileStream(fileName, FileMode.Open))
			//{
			//	using (var sr = new StreamReader(fs))
			//	{
			//		while (!sr.EndOfStream)
			//		{
			//			var line = sr.ReadLine();
			//			if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
			//			{
			//				var splits = line.Split(',');
			//				switch ((FileLineType)Convert.ToInt32(splits[0]))
			//				{
			//					case FileLineType.DefinitionRow:

			//						output.Name = splits[3] as string;

			//						break;
			//					case FileLineType.ChannelRow:
			//						var propChannel = new KeyValuePair<int, string>(Convert.ToInt32(splits[1]), splits[2] as string);

			//						output.Channels.Add(propChannel);
			//						break;

			//					default:
			//						break;
			//				}
			//			}
			//		}
			//	}
			//}
			return output;
		}
		private List<Prop> Props
		{
			get
			{
				return (Directory.GetFiles(defaultPath, "*.prop")).Select(s => FromFile(s)).OrderBy(o => o.Name).ToList();
			}
		}

		public bool TemplateEnabled
		{
			get
			{

				return (Directory.GetFiles(defaultPath, "*.prop")).Any();
			}
		}

		public IEnumerable<ElementNode> GenerateElements(IEnumerable<ElementNode> selectedNodes = null)
		{
			List<ElementNode> result = new List<ElementNode>();

			if (treename.Length == 0)
			{
				Logging.Error("Custom Prop name is null");
				return result;
			}

			ElementNode head = ElementNodeService.Instance.CreateSingle(null, treename);
			result.Add(head);

			 
			foreach (KeyValuePair<int,string> stringName in SelectedItem.Channels)
			{
				ElementNode stringnode = ElementNodeService.Instance.CreateSingle(head, string.Format("{0} [{1}]",treename , stringName.Value));
				result.Add(stringnode);
			}

			return result;
		}

		private void CustomProp_Load(object sender, EventArgs e)
		{
			textBoxTreeName.Text = cboPropName.Text;
		}

		private void CustomProp_FormClosed(object sender, FormClosedEventArgs e)
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



		class Prop
		{
			public Prop()
			{
				Channels = new List<KeyValuePair<int, string>>();
			}
			public string Name { get; set; }
			public List<KeyValuePair<int, string>> Channels { get; set; }
		}

		Prop SelectedItem
		{
			get
			{
				return cboPropName.SelectedItem as Prop;
			}
		}

		private void cboPropName_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.textBoxTreeName.Text = SelectedItem.Name;

		}
	}
}
