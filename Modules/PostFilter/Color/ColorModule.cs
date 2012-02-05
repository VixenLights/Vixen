using System;
using System.Windows.Forms;
using Vixen.Commands;
using Vixen.Module;
using Vixen.Module.PostFilter;

namespace Color {
	public class ColorModule : PostFilterModuleInstanceBase {
		private ColorData _data;
		private Func<System.Drawing.Color, System.Drawing.Color> _filter;

		public override Command Affect(Command command) {
			Lighting.Polychrome.SetColor setColorCommand = command as Lighting.Polychrome.SetColor;
			if(setColorCommand != null) {
				System.Drawing.Color newColor = _filter(setColorCommand.Color);
				command = new Lighting.Polychrome.SetColor(newColor);
			}
			return command;
		}

		public override IModuleDataModel ModuleData {
			get { return _data; }
			set { 
				_data = (ColorData)value;
				_SetFilter();
			}
		}

		public override bool HasSetup {
			get { return true; }
		}

		public override bool Setup() {
			using(ColorSetupForm colorSetupForm = new ColorSetupForm(_data)) {
				if(colorSetupForm.ShowDialog() == DialogResult.OK) {
					_data.ColorFilter = colorSetupForm.SelectedColorFilter;
					_SetFilter();
					return true;
				}
			}
			return false;
		}

		private void _SetFilter() {
			switch(_data.ColorFilter) {
				case ColorFilter.Red:
					_filter = _FilterRed;
					break;
				case ColorFilter.Green:
					_filter = _FilterGreen;
					break;
				case ColorFilter.Blue:
					_filter = _FilterBlue;
					break;
				case ColorFilter.Yellow:
					_filter = _FilterYellow;
					break;
				case ColorFilter.White:
					_filter = _FilterWhite;
					break;
				default:
					_filter = _FilterNone;
					break;
			}
		}

		private System.Drawing.Color _FilterRed(System.Drawing.Color color) {
			return System.Drawing.Color.FromArgb(color.R, 0, 0);
		}

		private System.Drawing.Color _FilterGreen(System.Drawing.Color color) {
			return System.Drawing.Color.FromArgb(0, color.G, 0);
		}

		private System.Drawing.Color _FilterBlue(System.Drawing.Color color) {
			return System.Drawing.Color.FromArgb(0, 0, color.B);
		}

		private System.Drawing.Color _FilterYellow(System.Drawing.Color color) {
			int yellow = (color.R + color.G) / 2;
			return System.Drawing.Color.FromArgb(yellow, yellow, 0);
		}

		private System.Drawing.Color _FilterWhite(System.Drawing.Color color) {
			int white = (color.R + color.G + color.B) / 3;
			return System.Drawing.Color.FromArgb(white, white, white);
		}

		private System.Drawing.Color _FilterNone(System.Drawing.Color color) {
			return color;
		}
	}
}
