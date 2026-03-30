using Common.Controls;
using Common.Controls.Theme;
using System.ComponentModel;

namespace VixenModules.Property.Orientation {
	public partial class SetupForm : BaseForm
	{
		public SetupForm(Orientation defaultOrientation) {
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			foreach (var item in Enum.GetValues(typeof(Orientation)))
			{
				comboBoxOrientation.Items.Add(item);
			}

			Orientation = defaultOrientation;
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Orientation Orientation
		{
			get { return (Orientation)comboBoxOrientation.SelectedItem; }
			set
			{
				comboBoxOrientation.SelectedItem = value;
			}
		}

		private void SetupForm_Load(object sender, EventArgs e)
		{
			
		}
	}
}
