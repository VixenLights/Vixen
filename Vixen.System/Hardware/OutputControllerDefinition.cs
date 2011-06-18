using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using Vixen.Common;
using Vixen.IO;

namespace Vixen.Hardware {
    public class OutputControllerDefinition : Definition {
		private const string DIRECTORY_NAME = "Controller";
        private const string ATTR_OUTPUT_COUNT = "outputCount";
        private const string FILE_EXT = ".def";
		private const string ATTR_HARDWARE_ID = "hardwareId";

		/// <summary>
		/// Do not use.  Use OutputControllerDefinition.NewDefinition instead.
		/// </summary>
		private OutputControllerDefinition() { }

        [DataPath]
		static protected readonly string _controllerDefinitionDirectory = Path.Combine(Definition._definitionDirectory, DIRECTORY_NAME);

		public OutputControllerDefinition(string name, int outputCount, Guid outputModuleId) {
			OutputCount = outputCount;
			HardwareModuleId = outputModuleId;
			FilePath = Path.Combine(_controllerDefinitionDirectory, name + FILE_EXT);
		}

		static public IEnumerable<string> GetAllFileNames() {
			return Directory.GetFiles(_controllerDefinitionDirectory, "*" + FILE_EXT);
		}

		static public OutputControllerDefinition ReadXml(XElement element) {
			OutputControllerDefinition controllerDefinition = new OutputControllerDefinition();
			controllerDefinition.HardwareModuleId = new Guid(element.Attribute(ATTR_HARDWARE_ID).Value);
			controllerDefinition.OutputCount = int.Parse(element.Attribute(ATTR_OUTPUT_COUNT).Value);
			return controllerDefinition;
		}

		static public XElement WriteXml(OutputControllerDefinition controllerDefinition) {
			XElement controllerElement = new XElement("Controller",
				new XAttribute(ATTR_HARDWARE_ID, controllerDefinition.HardwareModuleId),
				new XAttribute(ATTR_OUTPUT_COUNT, controllerDefinition.OutputCount)
				);
			return controllerElement;
		}

		public int OutputCount;

		// Must be a property for data binding.
		public Guid HardwareModuleId { get; set; }

		public void Save(string filePath) {
			if(string.IsNullOrWhiteSpace(filePath)) throw new InvalidOperationException("A name is required.");
			filePath = Path.Combine(_controllerDefinitionDirectory, Path.GetFileName(filePath));
			base._Save<OutputControllerDefinitionWriter>(filePath);
		}

		public void Save() {
			Save(FilePath);
		}

		public void Delete() {
			// Find and delete controllers based on this definition.
			OutputController.DeleteAll(this);
			// Delete this definition.
			if(File.Exists(FilePath)) {
				File.Delete(FilePath);
			}
		}

		static public OutputControllerDefinition Load(string filePath) {
			filePath = Path.Combine(_controllerDefinitionDirectory, Path.ChangeExtension(Path.GetFileNameWithoutExtension(filePath), FILE_EXT));
			return Definition.Load<OutputControllerDefinition, OutputControllerDefinitionReader>(filePath);
		}
	}
}
