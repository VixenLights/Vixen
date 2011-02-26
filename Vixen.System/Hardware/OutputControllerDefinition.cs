using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using Vixen.Common;


namespace Vixen.Hardware {
    public class OutputControllerDefinition : Definition {
		private const string DIRECTORY_NAME = "Controller";
        private const string ATTR_OUTPUT_COUNT = "outputCount";
        private const string FILE_EXT = ".def";
		private const string ATTR_HARDWARE_ID = "hardwareId";

		/// <summary>
		/// Do not use.  Use OutputControllerDefinition.NewDefinition instead.
		/// </summary>
		public OutputControllerDefinition() { }

        [DataPath]
		static protected readonly string _controllerDefinitionDirectory = Path.Combine(Definition._definitionDirectory, DIRECTORY_NAME);

        static public OutputControllerDefinition NewDefinition(string typeName, int outputCount, Guid outputModuleId) {
            OutputControllerDefinition controllerDefinition = new OutputControllerDefinition();
            controllerDefinition.OutputCount = outputCount;
            controllerDefinition.HardwareModuleId = outputModuleId;
			controllerDefinition.Save(Path.Combine(_controllerDefinitionDirectory, typeName + FILE_EXT));
            return controllerDefinition;
        }

        static public OutputControllerDefinition Get(string name) {
			return Definition._GetInstance<OutputControllerDefinition>(_controllerDefinitionDirectory, name, FILE_EXT);
        }

		static public IEnumerable<OutputControllerDefinition> GetAll() {
			return Definition._GetAll<OutputControllerDefinition>(_controllerDefinitionDirectory, FILE_EXT);
		}

		override protected void ReadAttributes(XmlReader reader) {
			HardwareModuleId = new Guid(reader.GetAttribute(ATTR_HARDWARE_ID));
			OutputCount = int.Parse(reader.GetAttribute(ATTR_OUTPUT_COUNT));
        }

        override protected void ReadBody(XmlReader reader) { }

        override protected void WriteAttributes(XmlWriter writer) {
			writer.WriteAttributeString(ATTR_HARDWARE_ID, HardwareModuleId.ToString());
			writer.WriteAttributeString(ATTR_OUTPUT_COUNT, OutputCount.ToString());
        }

        override protected void WriteBody(XmlWriter writer) { }

		public int OutputCount;

		// Must be a property for data binding.
		public Guid HardwareModuleId { get; set; }
	}
}
