using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Common.Resources.Properties;

namespace VixenModules.Preview.VixenPreview.CustomPropEditor.View
{
	public partial class CustomPropEditor : Form
	{

		private readonly PropEditorView _propEditorView;

		public CustomPropEditor()
		{
			InitializeComponent();
			Icon = Resources.Icon_Vixen3;
			var host = new ElementHost { Dock = DockStyle.Fill };

			Controls.Add(host);

			_propEditorView = new PropEditorView();

			host.Child = _propEditorView;
		}
	}
}
