using System;
using System.Windows.Forms;
using Vixen.Commands;
using Vixen.Module;
using Vixen.Module.OutputFilter;
using Vixen.Sys;

namespace Color {
	public class ColorModule : OutputFilterModuleInstanceBase {
		private ColorData _data;
		private Func<ICommand<LightingValue>, ICommand> _filter;
		private CommandDispatch _commandDispatch;

		public ColorModule() {
			_commandDispatch = new CommandDispatch();
		}

		public override ICommand Affect(ICommand command) {
			_commandDispatch.Filter = _filter;
			command.Dispatch(_commandDispatch);
			return _commandDispatch.Command;
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

		private ICommand _FilterRed(ICommand<LightingValue> command) {
			return new ByteValueCommand((byte)(command.CommandValue.Color.R * command.CommandValue.Intensity));
		}

		private ICommand _FilterGreen(ICommand<LightingValue> command) {
			return new ByteValueCommand((byte)(command.CommandValue.Color.G * command.CommandValue.Intensity));
		}

		private ICommand _FilterBlue(ICommand<LightingValue> command) {
			return new ByteValueCommand((byte)(command.CommandValue.Color.B * command.CommandValue.Intensity));
		}

		private ICommand _FilterYellow(ICommand<LightingValue> command) {
			double yellow = (command.CommandValue.Color.R + command.CommandValue.Color.G) / 2 * command.CommandValue.Intensity;
			return new ByteValueCommand((byte)yellow);
		}

		private ICommand _FilterWhite(ICommand<LightingValue> command) {
			double white = (command.CommandValue.Color.R + command.CommandValue.Color.G + command.CommandValue.Color.B) / 3 * command.CommandValue.Intensity;
			return new ByteValueCommand((byte)white);
		}

		private ICommand _FilterNone(ICommand<LightingValue> command) {
			return command;
		}
	}
}
