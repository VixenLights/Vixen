using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CommandStandard.Types;

namespace CommandStandard {
    public class CommandParameterSignature : IEnumerable<CommandParameterSpecification> {
        private CommandParameterSpecification[] _parameters = null;

        private const string ELEMENT_PARAMETER = "Parameter";
		private const char PARAMETER_DELIMITER = ',';

		public CommandParameterSignature(params CommandParameterSpecification[] parameters) {
			_parameters = parameters;
		}

        public CommandParameterSignature(string commandParameters) {
            // commandParameters is in the form "type name,type name, ..."
            List<CommandParameterSpecification> parameters = new List<CommandParameterSpecification>();
			string trimmedParameterString;
            foreach(string parameterString in commandParameters.Split(PARAMETER_DELIMITER)) {
				trimmedParameterString = parameterString.Trim();
                if(!string.IsNullOrEmpty(trimmedParameterString)) {
					parameters.Add(new CommandParameterSpecification(trimmedParameterString));
                }
            }
            _parameters = parameters.ToArray();
        }

        public CommandParameterSpecification this[int index] {
            get { return _parameters[index]; }
            set { _parameters[index] = value; }
        }

        public int Length {
            get { return _parameters.Length; }
        }

        // Use for self-documentation
        internal void SaveToXml(XmlNode node) {
            // "node" will be the node to write to as created by the owner
            // For this object, it will be the collection node that will contain all parameter nodes
            XmlNode parameterNode;
            foreach(CommandParameterSpecification parameter in _parameters) {
                parameterNode = node.OwnerDocument.CreateElement(ELEMENT_PARAMETER);
                node.AppendChild(node);
                parameter.SaveToXml(parameterNode);
            }
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            foreach(CommandParameterSpecification parameter in _parameters) {
                sb.AppendFormat("{0}{1}", parameter, PARAMETER_DELIMITER);
            }
            // Remove trailing comma
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }




        public IEnumerator<CommandParameterSpecification> GetEnumerator() {
            foreach(CommandParameterSpecification parameter in _parameters) {
                yield return parameter;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return _parameters.GetEnumerator();
        }
    }

}
