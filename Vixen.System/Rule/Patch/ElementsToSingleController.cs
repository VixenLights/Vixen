using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Data.Flow;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Rule.Patch {
	public class ElementsToSingleController : IPatchingRule {
		private IDataFlowComponent[] _elementComponents;
		private IDataFlowComponent[] _controllerOutputComponents;
		private int _startingOutputIndex;
		private int _outputsPerElement;

		public ElementsToSingleController(IEnumerable<Element> elements, IControllerDevice controllerDevice, int startingOutputIndex = 0, int outputsPerElement = 1)
			: this(elements, controllerDevice, controllerDevice.OutputCount, startingOutputIndex, outputsPerElement) {
		}

		public ElementsToSingleController(IEnumerable<Element> elements, ISmartControllerDevice controllerDevice, int startingOutputIndex = 0, int outputsPerElement = 1)
			: this(elements, controllerDevice, controllerDevice.OutputCount, startingOutputIndex, outputsPerElement) {
		}

		public ElementsToSingleController(IEnumerable<Element> elements, IOutputDevice controllerDevice, int startingOutputIndex = 0, int outputsPerElement = 1)
			: this(elements, controllerDevice, ((IHasOutputs)controllerDevice).OutputCount, startingOutputIndex, outputsPerElement) {
		}

		private ElementsToSingleController(IEnumerable<Element> elements, IOutputDevice controllerDevice, int controllerOutputCount, int startingOutputIndex, int outputsPerElement) {
			if(elements == null) throw new ArgumentNullException("elements");
			if(controllerDevice == null) throw new ArgumentNullException("controllerDevice");
			if(startingOutputIndex < 0 || startingOutputIndex >= controllerOutputCount) throw new IndexOutOfRangeException("Starting output index invalid for the controller.");
			if(outputsPerElement < 1 || outputsPerElement > controllerOutputCount) throw new InvalidOperationException("Invalid output count.");
			//if(outputsPerElement >= (controllerOutputCount - startingOutputIndex)) throw new InvalidOperationException("Not enough outputs to patch.");

			_startingOutputIndex = startingOutputIndex;
			_outputsPerElement = outputsPerElement;
			_elementComponents = elements.Select(VixenSystem.Elements.GetDataFlowComponentForElement).ToArray();
			_controllerOutputComponents = Enumerable.Range(_startingOutputIndex, controllerOutputCount - _startingOutputIndex).Select(x => VixenSystem.ControllerManagement.GetDataFlowComponentForOutput(controllerDevice, x)).ToArray();
		}

		public string Description {
			get { return "Elements to a single controller"; }
		}

		public IEnumerable<IDataFlowComponentReference> GenerateSourceReferences() {
			return _ZipElementsAndOutputs(_GenerateSourceElementReference);
		}

		public IEnumerable<DataFlowPatch> GeneratePatches() {
			return _ZipElementsAndOutputs(_GenerateElementOutputPatch);
		}

		private IEnumerable<T> _ZipElementsAndOutputs<T>(Func<int,int,T> generator) {
			int elementIndex = 0;
			int outputIndex = 0;

			while(_HaveEnoughOutputsLeft(outputIndex) && _HaveElementsLeft(elementIndex)) {
				for(int i = 0; i < _outputsPerElement; i++) {
					yield return generator(elementIndex, outputIndex + i);
				}

				elementIndex++;
				outputIndex += _outputsPerElement;
			}
		}

		private IDataFlowComponentReference _GenerateSourceElementReference(int elementIndex, int outputIndex) {
			return new DataFlowComponentReference(_elementComponents[elementIndex], 0);
		}

		private DataFlowPatch _GenerateElementOutputPatch(int elementIndex, int outputIndex) {
			return new DataFlowPatch(_controllerOutputComponents[outputIndex].DataFlowComponentId, _elementComponents[elementIndex].DataFlowComponentId, 0);
		}

		private bool _HaveEnoughOutputsLeft(int outputIndex) {
			return outputIndex + _outputsPerElement < _controllerOutputComponents.Length;
		}

		private bool _HaveElementsLeft(int elementIndex) {
			return elementIndex < _elementComponents.Length;
		}
	}
}
