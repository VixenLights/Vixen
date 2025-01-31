using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Property.Order {
	public partial class SetupForm : BaseForm
	{
		public SetupForm(OrderData data) {
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
			Order = data.Order;
		}


		public int Order {
			get { return (int)numericUpDownXPosition.Value; }
			set {
				numericUpDownXPosition.Value = value;

			}
		}
	}
}
