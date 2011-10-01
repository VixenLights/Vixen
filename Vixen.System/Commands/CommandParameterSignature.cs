using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Vixen.Commands {
    public class CommandParameterSignature : IEnumerable<CommandParameterSpecification> {
        private CommandParameterSpecification[] _parameters = null;

		private const string PARAMETER_DELIMITER = ",";

		public CommandParameterSignature(params CommandParameterSpecification[] parameters) {
			_parameters = parameters;
		}

        public CommandParameterSignature(string commandParameters) {
            // commandParameters is in the form "type name,type name, ..."
            List<CommandParameterSpecification> parameters = new List<CommandParameterSpecification>();
			string trimmedParameterString;
            foreach(string parameterString in commandParameters.Split(new[] { PARAMETER_DELIMITER }, StringSplitOptions.RemoveEmptyEntries)) {
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

        public int Count {
            get { return _parameters.Length; }
        }

        public override string ToString() {
			return string.Join(PARAMETER_DELIMITER, (object[])_parameters);
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
