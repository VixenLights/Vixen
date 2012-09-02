using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Data.Flow;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Rule.Patch {
	public class ChannelsToSingleController : IPatchingRule {
		private IDataFlowComponent[] _channelComponents;
		private IDataFlowComponent[] _controllerOutputComponents;
		private int _startingOutputIndex;
		private int _outputsPerChannel;

		public ChannelsToSingleController(IEnumerable<Channel> channels, IControllerDevice controllerDevice, int startingOutputIndex = 0, int outputsPerChannel = 1)
			: this(channels, controllerDevice, controllerDevice.OutputCount, startingOutputIndex, outputsPerChannel) {
		}

		public ChannelsToSingleController(IEnumerable<Channel> channels, ISmartControllerDevice controllerDevice, int startingOutputIndex = 0, int outputsPerChannel = 1)
			: this(channels, controllerDevice, controllerDevice.OutputCount, startingOutputIndex, outputsPerChannel) {
		}

		private ChannelsToSingleController(IEnumerable<Channel> channels, IOutputDevice controllerDevice, int controllerOutputCount, int startingOutputIndex, int outputsPerChannel) {
			if(channels == null) throw new ArgumentNullException("channels");
			if(controllerDevice == null) throw new ArgumentNullException("controllerDevice");
			if(startingOutputIndex < 0 || startingOutputIndex >= controllerOutputCount) throw new IndexOutOfRangeException("Starting output index invalid for the controller.");
			if(outputsPerChannel < 1 || outputsPerChannel > controllerOutputCount) throw new InvalidOperationException("Invalid output count.");
			//if(outputsPerChannel >= (controllerOutputCount - startingOutputIndex)) throw new InvalidOperationException("Not enough outputs to patch.");

			_channelComponents = channels.Select(VixenSystem.Channels.GetDataFlowComponentForChannel).ToArray();
			_controllerOutputComponents = Enumerable.Range(_startingOutputIndex, controllerOutputCount - _startingOutputIndex).Select(x => VixenSystem.ControllerManagement.GetDataFlowComponentForOutput(controllerDevice, x)).ToArray();
			_startingOutputIndex = startingOutputIndex;
			_outputsPerChannel = outputsPerChannel;
		}

		public string Description {
			get { return "Channels to a single controller"; }
		}

		public IEnumerable<IDataFlowComponentReference> GenerateSourceReferences() {
			return _ZipChannelsAndOutputs(_GenerateSourceChannelReference);
		}

		public IEnumerable<DataFlowPatch> GeneratePatches() {
			return _ZipChannelsAndOutputs(_GenerateChannelOutputPatch);
		}

		private IEnumerable<T> _ZipChannelsAndOutputs<T>(Func<int,int,T> generator) {
			int channelIndex = 0;
			int outputIndex = 0;

			while(_HaveEnoughOutputsLeft(outputIndex) && _HaveChannelsLeft(channelIndex)) {
				for(int i = 0; i < _outputsPerChannel; i++) {
					yield return generator(channelIndex, outputIndex + i);
				}

				channelIndex++;
				outputIndex += _outputsPerChannel;
			}
		}

		private IDataFlowComponentReference _GenerateSourceChannelReference(int channelIndex, int outputIndex) {
			return new DataFlowComponentReference(_channelComponents[channelIndex], 0);
		}

		private DataFlowPatch _GenerateChannelOutputPatch(int channelIndex, int outputIndex) {
			return new DataFlowPatch(_controllerOutputComponents[outputIndex].DataFlowComponentId, _channelComponents[channelIndex].DataFlowComponentId, 0);
		}

		private bool _HaveEnoughOutputsLeft(int outputIndex) {
			return outputIndex + _outputsPerChannel < _controllerOutputComponents.Length;
		}

		private bool _HaveChannelsLeft(int channelIndex) {
			return channelIndex < _channelComponents.Length;
		}
	}
}
