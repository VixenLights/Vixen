using Common.Controls;
using Common.Controls.Theme;
using System.ComponentModel;

namespace VixenModules.Property.Order {
	public partial class SetupForm : BaseForm
	{
		public SetupForm(OrderData data) {
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			Order = data.Order;
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Order {
			get { return (int)numericUpDownXPosition.Value; }
			set {
				numericUpDownXPosition.Value = value;

			}
		}
	}
}
