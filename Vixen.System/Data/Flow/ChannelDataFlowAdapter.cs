using System;
using Vixen.Sys;

namespace Vixen.Data.Flow {
	/// <summary>
	/// Facilitates allowing channels to participate in the data flow system.
	/// </summary>
	class ChannelDataFlowAdapter : IDataFlowComponent<IntentsDataFlowData> {
		private Channel _channel;
		private IDataFlowOutput<IntentsDataFlowData>[] _outputs;

		public ChannelDataFlowAdapter(Channel channel) {
			_channel = channel;
		}

		public IDataFlowOutput<IntentsDataFlowData>[] Outputs {
			get {
				if(_outputs == null) {
					_outputs = new[] { new ChannelDataFlowOutputAdapter(_channel) };
				}
				return _outputs;
			}
		}

		IDataFlowOutput[] IDataFlowComponent.Outputs {
			get { return Outputs; }
		}

		public IDataFlowComponentReference<IntentsDataFlowData> Source {
			get { 
				// No input data type, so this is meaningless.
				return null;
			}
			set { /* Can't set this for a channel (in its role as a data flow participant) */ }
		}

		IDataFlowComponentReference IDataFlowComponent.Source {
			get { return Source; }
			set { Source = (IDataFlowComponentReference<IntentsDataFlowData>)value; }
		}

		public DataFlowType InputDataType {
			get { return DataFlowType.None; }
		}

		public DataFlowType OutputDataType {
			get { return DataFlowType.MultipleIntents; }
		}

		public Guid DataFlowComponentId {
			get { return _channel.Id; }
		}

		public string Name {
			get { return _channel.Name; }
		}
	}
}
