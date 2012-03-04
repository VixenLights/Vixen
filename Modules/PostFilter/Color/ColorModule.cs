using System;
using System.Windows.Forms;
using Vixen.Module;
using Vixen.Module.PostFilter;

namespace Color {
	public class ColorModule : PostFilterModuleInstanceBase {
		private ColorData _data;
		private Func<System.Drawing.Color, System.Drawing.Color> _filter;

		public override System.Drawing.Color Affect(System.Drawing.Color value) {
			return _filter(value);
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
					//_commandDispatch.Filter = _FilterRed;
					break;
				case ColorFilter.Green:
					_filter = _FilterGreen;
					//_commandDispatch.Filter = _FilterGreen;
					break;
				case ColorFilter.Blue:
					_filter = _FilterBlue;
					//_commandDispatch.Filter = _FilterBlue;
					break;
				case ColorFilter.Yellow:
					_filter = _FilterYellow;
					//_commandDispatch.Filter = _FilterYellow;
					break;
				case ColorFilter.White:
					_filter = _FilterWhite;
					//_commandDispatch.Filter = _FilterWhite;
					break;
				default:
					_filter = _FilterNone;
					//_commandDispatch.Filter = _FilterNone;
					break;
			}
		}

		//Move the desired color to the lowest byte for use as a byte value going to a single output.
		private System.Drawing.Color _FilterRed(System.Drawing.Color color) {
			//return System.Drawing.Color.FromArgb(color.R, 0, 0);
			return System.Drawing.Color.FromArgb(0, 0, color.R);
		}

		private System.Drawing.Color _FilterGreen(System.Drawing.Color color) {
			//return System.Drawing.Color.FromArgb(0, color.G, 0);
			return System.Drawing.Color.FromArgb(0, 0, color.G);
		}

		private System.Drawing.Color _FilterBlue(System.Drawing.Color color) {
			//return System.Drawing.Color.FromArgb(0, 0, color.B);
			return System.Drawing.Color.FromArgb(0, 0, color.B);
		}

		private System.Drawing.Color _FilterYellow(System.Drawing.Color color) {
			int yellow = (color.R + color.G) / 2;
			//return System.Drawing.Color.FromArgb(yellow, yellow, 0);
			return System.Drawing.Color.FromArgb(0, 0, yellow);
		}

		private System.Drawing.Color _FilterWhite(System.Drawing.Color color) {
			int white = (color.R + color.G + color.B) / 3;
			//return System.Drawing.Color.FromArgb(white, white, white);
			return System.Drawing.Color.FromArgb(0, 0, white);
		}

		private System.Drawing.Color _FilterNone(System.Drawing.Color color) {
			return color;
		}
	}
}
