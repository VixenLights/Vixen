using System.ComponentModel;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Property.Location {
	public partial class SetupForm : BaseForm
	{
		public SetupForm(LocationData data) {
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			X = data.X;
			Y = data.Y;
			Z = data.Z;
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int X {
			get { return (int)numericUpDownXPosition.Value; }
			set {
				numericUpDownXPosition.Value = value;

			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Y {
			get { return (int)numericUpDownYPosition.Value; }
			set {
				numericUpDownYPosition.Value = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Z {
			get { return (int)numericUpDownZPosition.Value; }
			set {
				numericUpDownZPosition.Value = value;
			}
		}
	}
}
