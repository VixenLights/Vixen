using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Vixen.Commands;
using Vixen.Sys;

namespace TestPreview {
	public partial class TestPreviewForm : Form {
		private CommandHandler _commandHandler;
		private Dictionary<Guid, ChannelColorStateControl> _channelControls;

		public TestPreviewForm() {
			InitializeComponent();
			_commandHandler = new CommandHandler();
			_channelControls = new Dictionary<Guid, ChannelColorStateControl>();
			foreach(Channel channel in VixenSystem.Channels) {
				_AddChannel(channel);
			}
		}

		public void Update(ChannelCommands channelCommands) {
			if(channelCommands != null) {
				foreach(Guid channelId in channelCommands.Keys) {
					ICommand command = channelCommands[channelId];
					if(command != null) {
						ChannelColorStateControl control = _GetControlForChannel(channelId);
						if(control != null) {
							command.Dispatch(_commandHandler);
							control.ChannelColor = _commandHandler.Value;
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
