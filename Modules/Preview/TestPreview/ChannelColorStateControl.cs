using System.Windows.Forms;
using Vixen.Data.Value;
using Vixen.Sys;

namespace VixenModules.Preview.TestPreview {
	public partial class ChannelColorStateControl : UserControl, IHandler<IIntentState<ColorValue>>, IHandler<IIntentState<LightingValue>>, IHandler<IIntentState<PositionValue>>, IHandler<IIntentState<CommandValue>> {
		private bool[] _valuesPresent;

		public ChannelColorStateControl(string channelName) {
			InitializeComponent();
			_valuesPresent = new bool[4];
			labelChannelName.Text = channelName;
		}

		public IIntentStates ChannelState {
			set {
				_Clear();
				foreach(IIntentState intentState in value) {
					intentState.Dispatch(this);
				}

				colorValueControl1.Visible = _valuesPresent[0];
				lightingValueControl1.Visible = _valuesPresent[1];
				positionValueControl1.Visible = _valuesPresent[2];
				commandValueControl1.Visible = _valuesPresent[3];
			}
		}

		public void Handle(IIntentState<ColorValue> obj) {
			_valuesPresent[0] = true;
			colorValueControl1.IntentState = obj;
		}

		public void Handle(IIntentState<LightingValue> obj) {
			_valuesPresent[1] = true;
			lightingValueControl1.IntentState = obj;
		}

		public void Handle(IIntentState<PositionValue> obj) {
			_valuesPresent[2] = true;
			positionValueControl1.IntentState = obj;
		}

		public void Handle(IIntentState<CommandValue> obj) {
			_valuesPresent[3] = true;
			commandValueControl1.IntentState = obj;
		}

		private void _Clear() {
			for(int i = 0; i < _valuesPresent.Length; i++) {
				_valuesPresent[i] = false;
			}
		}
	}
}
