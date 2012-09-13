using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Vixen.Sys;

namespace VixenModules.Preview.TestPreview {
	public partial class TestPreviewForm : Form {
		private Dictionary<Guid, ChannelColorStateControl> _channelControls;

		public TestPreviewForm() {
			InitializeComponent();
			_channelControls = new Dictionary<Guid, ChannelColorStateControl>();
			foreach(Channel channel in VixenSystem.Channels) {
				_AddChannel(channel);
			}
		}

		public void Update(ChannelIntentStates channelIntentStates) {
			if(channelIntentStates != null) {
				foreach(Guid channelId in channelIntentStates.Keys) {
					IIntentStates intentStates = channelIntentStates[channelId];
					if(intentStates != null) {
						ChannelColorStateControl control = _GetControlForChannel(channelId);
						if(control != null) {
							control.ChannelState = intentStates;
						}
					}
				}
			}
		}

		private void _AddChannel(Channel channel) {
			ChannelColorStateControl control = new ChannelColorStateControl(channel.Name);
			_channelControls[channel.Id] = control;
			control.Dock = DockStyle.Top;
			Controls.Add(control);
		}

		private ChannelColorStateControl _GetControlForChannel(Guid channelId) {
			ChannelColorStateControl control;
			_channelControls.TryGetValue(channelId, out control);
			return control;
		}
	}
}
