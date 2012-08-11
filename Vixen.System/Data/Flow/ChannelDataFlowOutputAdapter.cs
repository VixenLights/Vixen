using Vixen.Sys;

namespace Vixen.Data.Flow {
	class ChannelDataFlowOutputAdapter : IDataFlowOutput<IntentsDataFlowData> {
		private Channel _channel;

		public ChannelDataFlowOutputAdapter(Channel channel) {
			_channel = channel;
		}

		public IntentsDataFlowData Data {
			get { return new IntentsDataFlowData(_channel.State); }
		}

		public string Name {
			get { return _channel.Name; }
		}

		IDataFlowData IDataFlowOutput.Data {
			get { return Data; }
		}
	}
}
